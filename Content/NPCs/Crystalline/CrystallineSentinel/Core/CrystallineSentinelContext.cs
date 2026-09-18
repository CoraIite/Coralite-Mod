using Coralite.Core;
using Coralite.Core.SmoothFunctions;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using System;
using System.IO;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.Core
{
    /// <summary>运动声明模式；宿主 <c>ApplyDeclaredMovement</c> 两端同跑地把它翻译成 velocity。</summary>
    public enum CrystallineSentinelMoveMode
    {
        /// <summary>
        /// 保持帧间速度不变（默认）。旧代码里没写运动的状态就是这个语义——一阶段有重力自然下坠、二阶段靠各自的衰减收速，
        /// 所以默认值不能像赤玉灵那样取“刹停”，否则会改掉整套手感。
        /// </summary>
        Keep,
        /// <summary>状态本帧已直接写 velocity（冲刺、出手、撞墙反弹），宿主不再动它。</summary>
        Direct,
        /// <summary>水平站定（<c>velocity.X = 0</c>），重力照旧。</summary>
        StandStill,
        /// <summary>地面行走：<c>Collision.StepUp</c> + 朝 <c>direction</c> 加速到上限，可选反向即刹死。</summary>
        GroundWalk,
        /// <summary>整体衰减 <c>velocity *= DampFactor</c>。</summary>
        Damp,
    }

    /// <summary>本态跟哪一份常态 AI（旧 <c>P1NormalAI</c> / <c>P2NormalAI</c>）。</summary>
    public enum CrystallineSentinelCommon
    {
        /// <summary>都不跟（只有 hub 这个提交口用）。</summary>
        None,
        /// <summary>一阶段常态：冷却推进、警戒与索敌、转阶段闸。</summary>
        PhaseOne,
        /// <summary>二阶段常态：部位跟随、脱战离场、关重力。</summary>
        PhaseTwo,
    }

    /// <summary>颜文字类型，成员与数值沿用旧 <c>CrystallineSentinel.TextTypes</c>。</summary>
    public enum CrystallineSentinelTextType
    {
        None,
        /// <summary> 刚发现玩家 </summary>
        Surprise,
        /// <summary> 失去目标 </summary>
        Confusion,
        /// <summary> 开盾并收到伤害时 </summary>
        Block,
        /// <summary> 发射火箭 </summary>
        Fire,
        /// <summary> 被自己的火箭打到 </summary>
        HitSelf,
        /// <summary> 切换状态碎掉的时候 </summary>
        Broken,
        /// <summary> 切换状态结束 </summary>
        Angry,
    }

    /// <summary>
    /// 结晶战斗体 FSM 上下文：每帧声明总线 + 跨帧事实 + 两份常态 AI。<br/>
    /// 旧代码占用的 ai[1..3]（Timer / Recorder / CanHit）与 localAI[0..3]（Recorder2 / GuardCounter / TextType / Alerted）全部搬走：
    /// Timer / Recorder(子拍) / Recorder2 进状态热字段，CanHit 与 Alerted / ReleasedRock 进位标志，
    /// 其余是权威端选招记账或纯表现量，ai[1..3] 还给基座约定。
    /// </summary>
    public sealed class CrystallineSentinelContext : CoraliteBossContext
    {
        /// <summary>热字段位标志：本帧接触伤害开关（旧 ai[3]）。声明通道每帧重写，这一位只负责把收包瞬间的值带给客户端。</summary>
        public const int CanHitFlag = 0;

        /// <summary>同步事实位标志：已警戒（旧 localAI[3]）。</summary>
        private const int AlertedFactBit = 0x1;
        /// <summary>同步事实位标志：已放过碎岩（旧 ReleasedRock）。</summary>
        private const int ReleasedRockFactBit = 0x2;

        public CrystallineSentinel Boss { get; }

        /// <summary>当前目标玩家（<c>Npc.target</c> 原版同步；无效时指向 255 号占位玩家，不会为 null）。</summary>
        public Player Target => Main.player[Npc.target];

        /// <summary>当前顶层状态 id；状态机未建立时读 ai[0]（中途加入者第一帧也能拿到正确值）。</summary>
        public int CurrentStateId => Machine?.CurrentHotState?.StateId ?? (int)Npc.ai[StateAiSlot];

        public CrystallineSentinelContext(CrystallineSentinel boss) : base(boss.NPC, boss)
        {
            Boss = boss;
            Array.Fill(RecentPicks, -1);
            // 已迁移：关闭 6 px/f 遗留阀，改走决策点 MarkDecision + 45 帧心跳 + 纠偏器。
            UseLegacySpeedValve = false;
        }

        #region 事实（跨帧；权威端裁决量只有权威端读写才算数）

        /// <summary>
        /// 是否已进入二阶段。旧 <c>IsPhase2State</c> 拿状态 id 与 Exchange 比大小，新增的 hub 顶在枚举末尾会误判，
        /// 所以改成“进过 P2Idle..P2Dying 就锁定”，两端都从已同步的 ai[0] 推导。
        /// </summary>
        public bool IsPhase2 { get; private set; }

        /// <summary>CheckDead 请求死亡演出（权威端写，状态基类的 ServerUpdate 消费）。</summary>
        public bool KillRequested { get; set; }

        /// <summary>
        /// 自己的飞弹打中防御中的本体 → 请求破盾（旧 <c>OnHitByMissile</c> 直接在钩子里 <c>Recorder++</c> 强推到收招拍）。<br/>
        /// 钩子只登记请求，真正换拍由 <see cref="CrystallineSentinelStateId.P1Guard"/> 的权威端消费，客户端靠热字段里的拍号跟上（D5）。
        /// </summary>
        public bool ShieldBreakRequested { get; set; }

        /// <summary>取走破盾请求（权威端）。</summary>
        public bool ConsumeShieldBreak()
        {
            if (!ShieldBreakRequested)
            {
                return false;
            }

            ShieldBreakRequested = false;
            return true;
        }

        /// <summary>血量已跌破二阶段血线、且还没演过转阶段。状态基类在一阶段状态里消费它换态。</summary>
        public bool ShouldExchange { get; private set; }

        /// <summary>一阶段仇恨计数：玩家攻击时拉满，否则每帧减少；到底重新索敌。旧 <c>AggroCounter</c></summary>
        public int AggroCounter { get; set; }

        /// <summary>开盾计数：目标在 <see cref="CrystallineSentinelDirector.GuardCounterRange"/> 外造成的伤害累计。旧 localAI[1]</summary>
        public float GuardCounter { get; set; }

        /// <summary>开盾 / 飞弹冷却，两端每帧同减（只有权威端选招读它）。旧 <c>GuardCooldown</c> / <c>MissileCooldown</c></summary>
        public float GuardCooldown { get; set; }
        public float MissileCooldown { get; set; }

        /// <summary>已警戒（发现玩家）。旧 localAI[3]；随 <see cref="WriteFacts"/> 过线。</summary>
        public bool Alerted { get; set; }

        /// <summary>已放过一次碎岩（血线一次性替换飞弹）。旧 <c>ReleasedRock</c>；随 <see cref="WriteFacts"/> 过线，浮石粒子据此重建。</summary>
        public bool ReleasedRock { get; set; }

        /// <summary>二阶段攻击计数，满 <see cref="CrystallineSentinelDirector.RestAttackCount"/> 次强制休息。旧 <c>RestCounter</c></summary>
        public int RestCounter { get; set; }

        /// <summary>上一手重复次数（旧 <c>AttackRepeater</c>）与上一手是什么（旧 <c>lastAttack</c>），权威端防复读用。</summary>
        public int AttackRepeater { get; set; }
        public int LastAttack { get; set; } = -1;

        /// <summary>上一手提交的状态 id（-1 无）。</summary>
        public int LastPickedState { get; set; } = -1;

        /// <summary>最近几手的环形记录（查重记账，本轮只记不裁决）。</summary>
        public int[] RecentPicks { get; } = new int[CrystallineSentinelDirector.RecentPickWindow];
        private int recentPickCursor;

        /// <summary>
        /// 下一个状态的入场预充帧数（旧 <c>SwitchStateP1/P2</c> 的 <c>overrideTime</c>：正数 = 缩短本态、负数 = 先静默这么多帧）。
        /// 由提交口写、由新状态的 <c>OnEnter</c> 在权威端取走并存进热字段带给客户端。
        /// </summary>
        public int PendingEntryFrames { get; set; }

        /// <summary>下一个状态入场时随机左右（旧 <c>randDirection</c>）。</summary>
        public bool PendingRandomDirection { get; set; }

        /// <summary>
        /// 换状态机体音是否已武装。旧代码的音效放在 <c>SwitchState</c> 里，新结构挪到状态 <c>OnEnter</c>（客户端被 NetSync 换态时也能响），
        /// 但初态是靠 <c>SetInitialState</c> 进的、旧代码在出生时并不发声，所以宿主建完机器才把它打开。
        /// </summary>
        public bool SwitchSoundArmed { get; set; }

        /// <summary>过账：写上一手 + 推环形窗口。所有出招都必须经 hub 的 Commit 到这里。</summary>
        public void RecordPick(int stateId)
        {
            LastPickedState = stateId;
            RecentPicks[recentPickCursor] = stateId;
            recentPickCursor = (recentPickCursor + 1) % RecentPicks.Length;
        }

        /// <summary>窗口内某招出现次数。</summary>
        public int RecentCountOf(int stateId)
        {
            int count = 0;
            for (int i = 0; i < RecentPicks.Length; i++)
            {
                if (RecentPicks[i] == stateId)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>取走入场预充帧数（权威端），顺手清掉，避免被下一个状态重复吃掉。</summary>
        public int ConsumeEntryFrames()
        {
            int frames = PendingEntryFrames;
            PendingEntryFrames = 0;
            return frames;
        }

        /// <summary>每帧只读事实：阶段锁定、转阶段闸。旧 AI() 在招式体之后才算这些，但它们只被换态判定读，提前一格不改行为。</summary>
        public void UpdateFacts()
        {
            int stateId = CurrentStateId;
            if (stateId is >= (int)CrystallineSentinelStateId.P2Idle and <= (int)CrystallineSentinelStateId.P2Dying)
            {
                IsPhase2 = true;
            }

            ShouldExchange = !IsPhase2 && Npc.life < Npc.lifeMax * CrystallineSentinelDirector.Phase2Threshold;
        }

        #endregion

        #region 声明通道（每帧重声明，BeginFrameDefaults 回默认）

        internal CrystallineSentinelMoveMode MoveMode { get; set; }
        /// <summary>Damp 模式的衰减系数。</summary>
        public float DampFactor { get; set; } = 1f;
        /// <summary>GroundWalk 模式：水平速度上限、加速度、反向时是否直接刹死。</summary>
        public float WalkMaxSpeed { get; set; }
        public float WalkAccel { get; set; }
        public bool WalkZeroOnReverse { get; set; }
        /// <summary>是否踏台阶。旧飞弹接近段没有 <c>Collision.StepUp</c>（走到台阶前就会被卡住转而结算），保留这个差异。</summary>
        public bool WalkStepUp { get; set; }

        /// <summary>本帧接触伤害开关（旧 ai[3]）。原版不同步这个量，两端按同一份声明每帧落地。</summary>
        public bool CanHit { get; set; }
        /// <summary>本帧无敌（<c>dontTakeDamage</c>）。原版不同步，必须两端每帧同声明。</summary>
        public bool Invulnerable { get; set; }
        /// <summary>本帧霸体（<c>SuperArmor</c>，护盾展开期间）。</summary>
        public bool SuperArmor { get; set; }
        /// <summary>护盾已成形：血条与鼠标悬停框都在这段时间里隐藏（旧 <c>State == P1Guard &amp;&amp; Recorder is 1 or 2</c>）。</summary>
        public bool ShieldUp { get; set; }
        /// <summary>本帧穿墙 / 无重力。二阶段默认无重力（旧 P2NormalAI 每帧写 true，且脱战分支提前 return 不会把它关掉）。</summary>
        public bool NoTileCollide { get; set; }
        public bool NoGravity { get; set; }
        /// <summary>
        /// 本帧受击要不要减速（旧 <c>State == P1Spurt &amp;&amp; Recorder == 0</c> 时的 <c>OnHitSlow</c>：走向玩家的路上被打会被拖慢）。<br/>
        /// 受击钩子在 AI 之外跑，读的是上一帧留下的声明——这与 <see cref="CanHit"/> 的用法一致。
        /// </summary>
        public bool HitSlowdown { get; set; }
        /// <summary>
        /// 本帧是碎岩的出手拍：环绕的浮石粒子读到它就自毁（本体同时长出实体浮石）。<br/>
        /// 旧代码是 <c>CheckCanReleaseRock()</c> 直接比对 ai[1]，Timer 进了热字段之后改成这条单帧声明；
        /// 粒子在 <c>On_Main.DrawDust</c> 上更新、晚于 NPC 的 AI，所以同帧读得到。
        /// </summary>
        public bool RockReleaseCue { get; set; }

        public override void BeginFrameDefaults()
        {
            base.BeginFrameDefaults();

            MoveMode = CrystallineSentinelMoveMode.Keep;
            DampFactor = 1f;
            WalkZeroOnReverse = false;
            WalkStepUp = true;
            CanHit = false;
            RockReleaseCue = false;
            HitSlowdown = false;
            Invulnerable = false;
            SuperArmor = false;
            ShieldUp = false;
            NoTileCollide = false;
            NoGravity = IsPhase2;
        }

        /// <summary>水平站定。</summary>
        public void DeclareStandStill() => MoveMode = CrystallineSentinelMoveMode.StandStill;

        /// <summary>状态自管速度（本帧已直接写 velocity）。</summary>
        public void DeclareDirect() => MoveMode = CrystallineSentinelMoveMode.Direct;

        /// <summary>整体衰减。</summary>
        public void DeclareDamp(float factor)
        {
            MoveMode = CrystallineSentinelMoveMode.Damp;
            DampFactor = factor;
        }

        /// <summary>地面行走：踏台阶 + 朝当前 <c>direction</c> 加速到上限。</summary>
        public void DeclareGroundWalk(float maxSpeed, float accel, bool zeroOnReverse = false, bool stepUp = true)
        {
            MoveMode = CrystallineSentinelMoveMode.GroundWalk;
            WalkMaxSpeed = maxSpeed;
            WalkAccel = accel;
            WalkZeroOnReverse = zeroOnReverse;
            WalkStepUp = stepUp;
        }

        /// <summary>转向目标（同时写 <c>direction</c> 与 <c>spriteDirection</c>，两端同算）。</summary>
        public void FaceTarget()
        {
            Npc.direction = Target.Center.X > Npc.Center.X ? 1 : -1;
            Npc.spriteDirection = Npc.direction;
        }

        #endregion

        #region 同步事实

        /// <summary>警戒 / 碎岩两个一次性旗标，一个字节。</summary>
        protected override void WriteFacts(BinaryWriter writer)
        {
            int flags = 0;
            if (Alerted)
            {
                flags |= AlertedFactBit;
            }

            if (ReleasedRock)
            {
                flags |= ReleasedRockFactBit;
            }

            writer.Write((byte)flags);
        }

        protected override void ReadFacts(BinaryReader reader)
        {
            int flags = reader.ReadByte();
            Alerted = (flags & AlertedFactBit) != 0;
            ReleasedRock = (flags & ReleasedRockFactBit) != 0;
        }

        #endregion

        #region 常态 AI（旧 P1NormalAI / P2NormalAI，两端同跑；顺序保持在招式体之后）

        /// <summary>一阶段常态：冷却推进、头部发光、警戒进出、贴脸拉仇恨。旧 CrystallineSentinel.cs:601-654</summary>
        public void UpdatePhaseOneCommon()
        {
            GuardCooldown--;
            MissileCooldown--;
            AggroCounter--;
            AggroCounter = (int)MathHelper.Clamp(AggroCounter, CrystallineSentinelDirector.AggroCounterMin, CrystallineSentinelDirector.AggroCounterMax);

            float light = CrystallineSentinelDirector.HeadLight;
            Lighting.AddLight(HeadPos, light, light, light);

            // 确保有当前目标引用
            if (!Target.Alives())
            {
                Npc.TargetClosest(false);
            }

            float distanceToTarget = float.MaxValue;
            if (Target.Alives())
            {
                distanceToTarget = Vector2.Distance(Npc.Center, Target.Center);
            }

            // 进入警戒范围：朝向玩家、把速度压成纯水平（起步姿态），弹出警戒标记
            if (Target.Alives() && distanceToTarget <= CrystallineSentinelDirector.AlertRange && !Alerted)
            {
                Alerted = true;
                Npc.direction = Target.Center.X > Npc.Center.X ? 1 : -1;
                Npc.spriteDirection = Npc.direction;
                Npc.velocity = Vector2.UnitX * Npc.velocity.Length() * Npc.direction;
                MarkDecision();

                SetText(CrystallineSentinelTextType.Surprise, CrystallineSentinelDirector.AlertTextFrames);

                if (!Main.dedServ)
                {
                    CrystallineAlertParticle prt = PRTLoader.NewParticle<CrystallineAlertParticle>(HeadPos, Vector2.Zero, Color.Red);
                    if (prt != null)
                    {
                        prt.FollowNPCIndex = Npc.whoAmI;
                        prt.Rotation = Npc.AngleTo(Target.Center);
                    }

                    Helper.PlayPitchedVariants(AssetDirectory.Sounds.Crystalline + "Sentinel_Alert", 0.4f, 0, 0, 3, HeadPos);
                }
            }

            // 玩家离开较远则清除警戒
            if (Alerted && (!Target.Alives() || distanceToTarget > CrystallineSentinelDirector.AlertRange + CrystallineSentinelDirector.AlertClearMargin))
            {
                Alerted = false;
                MarkDecision();
                SetText(CrystallineSentinelTextType.Confusion, CrystallineSentinelDirector.ConfusionTextFrames);
            }

            // 玩家进入近战范围，直接进入战斗状态
            if (Target.Alives() && distanceToTarget <= CrystallineSentinelDirector.MeleeRange)
            {
                AggroCounter = CrystallineSentinelDirector.AggroCounterMax;
            }
        }

        /// <summary>
        /// 二阶段常态：部位跟随、脱战离场、关重力。旧 CrystallineSentinel.cs:1209-1235。<br/>
        /// 脱战分支直接写 velocity 并盖掉状态的声明（旧代码跑在招式体之后，同样是后写优先）。
        /// </summary>
        public void UpdatePhaseTwoCommon()
        {
            UpdatePartFollow();

            bool targetLost = Npc.target < 0 || Npc.target == 255 || Target.dead || !Target.active
                || Target.Distance(Npc.Center) > CrystallineSentinelDirector.DespawnDistance;

            if (targetLost)
            {
                Npc.TargetClosest();

                // 没有玩家存活（或不在雪原）时离开
                if (Target.dead || !Target.active || Target.Distance(Npc.Center) > CrystallineSentinelDirector.DespawnDistance || !Target.ZoneSnow)
                {
                    Invulnerable = false;
                    Npc.rotation = Npc.rotation.AngleTowards(0f, CrystallineSentinelDirector.DespawnRotationStep);
                    Npc.velocity.X *= CrystallineSentinelDirector.DespawnDampX;
                    if (Npc.velocity.Y > CrystallineSentinelDirector.DespawnLimitY)
                    {
                        Npc.velocity.Y -= CrystallineSentinelDirector.DespawnAccelY;
                    }

                    Npc.velocity.Y -= CrystallineSentinelDirector.DespawnExtraAccelY;
                    NoTileCollide = true;
                    MoveMode = CrystallineSentinelMoveMode.Direct;
                    Npc.EncourageDespawn(CrystallineSentinelDirector.DespawnEncourageFrames);
                    return;
                }
            }

            NoTileCollide = false;
            NoGravity = true;
        }

        #endregion

        #region 表现层（纯本地：部位跟随、护盾开合、手部帧、颜文字；不参与判定，也不过线）

        /// <summary>头部挂点：发光、警戒标记、音源。</summary>
        public Vector2 HeadPos => Npc.Center + CrystallineSentinelDirector.HeadOffset;
        /// <summary>二阶段浮游炮的目标挂点（X 随朝向翻转）。</summary>
        public Vector2 FloatPos => Npc.Center + new Vector2(CrystallineSentinelDirector.FloatOffsetX * Npc.spriteDirection, CrystallineSentinelDirector.FloatOffsetY);
        public Vector2 LeftHandPos => Npc.Center + CrystallineSentinelDirector.LeftHandOffset;
        public Vector2 RightHandPos => Npc.Center + CrystallineSentinelDirector.RightHandOffset;

        /// <summary>浮游炮与双手的平滑后位置（绘制用）。</summary>
        public Vector2 FloatCenter { get; private set; }
        public Vector2[] HandCenter { get; } = new Vector2[2];
        /// <summary>双手帧号（0 = 出手姿态，<see cref="CrystallineSentinelDirector.MaxHandFrame"/> = 收拢）。</summary>
        public int[] HandFrame { get; } = new int[2];
        /// <summary>螺旋冲刺的刀刃帧号。</summary>
        public int HandSpurtFrameY { get; set; }
        /// <summary>护盾视觉开合系数 0~1（不同步，仅视觉）。</summary>
        public float GuardFactor { get; set; }
        /// <summary>休息进度 0~1，绘制抖动幅度按它放大（旧 P2Rest 借用 ai[2] Recorder 传给 PreDraw，纯视觉）。</summary>
        public float RestFactor { get; set; }
        /// <summary>当前状态的 Timer（绘制用；螺旋冲刺的刀刃抬升高度读它）。状态机未建立时为 0。</summary>
        public int StateTimer => Machine?.CurrentHotState?.Timer ?? 0;
        /// <summary>距上次受击的帧数，护盾受击闪光用。</summary>
        public int OnHitTimer { get; set; }
        /// <summary>挥刀用哪只手：+1 左手、−1 右手。旧 P2Swing 的 ai[2]，挥刀弹幕按它取挂点。</summary>
        public int SwingHandSign { get; set; } = 1;

        public CrystallineSentinelTextType TextType { get; private set; }
        private int textTime;

        /// <summary>颜文字是否该画。</summary>
        public bool ShowText => textTime > 0;

        /// <summary>设置颜文字。旧 CrystallineSentinel.cs:2072-2076</summary>
        public void SetText(CrystallineSentinelTextType type, int time)
        {
            TextType = type;
            textTime = time;
        }

        /// <summary>颜文字倒计时。旧 CrystallineSentinel.cs:552-560</summary>
        public void UpdateText()
        {
            if (textTime > 0)
            {
                textTime--;
                if (textTime == 0)
                {
                    TextType = CrystallineSentinelTextType.None;
                }
            }
        }

        private SecondOrderDynamics_Vec2 floatMover;
        private SecondOrderDynamics_Vec2[] handMovers;

        /// <summary>
        /// 懒建二阶段部位动力学并把部位归位。旧代码只在转阶段演出的最后一帧建（CrystallineSentinel.cs:1856-1867），
        /// 中途加入或一阶段直接被打死进死亡演出的客户端会拿到 null 而崩，所以这里改成按需建。
        /// </summary>
        public void EnsurePartRig()
        {
            if (floatMover != null)
            {
                return;
            }

            FloatCenter = FloatPos;
            floatMover = new SecondOrderDynamics_Vec2(CrystallineSentinelDirector.P2FloatF, CrystallineSentinelDirector.P2FloatZ,
                CrystallineSentinelDirector.P2FloatR, FloatPos);
            handMovers =
            [
                new SecondOrderDynamics_Vec2(CrystallineSentinelDirector.P2FloatF, CrystallineSentinelDirector.P2FloatZ, CrystallineSentinelDirector.P2HandR, LeftHandPos),
                new SecondOrderDynamics_Vec2(CrystallineSentinelDirector.P2FloatF, CrystallineSentinelDirector.P2FloatZ, CrystallineSentinelDirector.P2HandR, RightHandPos),
            ];
            HandCenter[0] = LeftHandPos;
            HandCenter[1] = RightHandPos;
            HandFrame[0] = CrystallineSentinelDirector.MaxHandFrame;
            HandFrame[1] = CrystallineSentinelDirector.MaxHandFrame;
        }

        /// <summary>转阶段演出结束时重建部位（等价于旧代码在那一帧 new 出三个 mover）。</summary>
        public void ResetPartRig()
        {
            floatMover = null;
            EnsurePartRig();
        }

        /// <summary>部位跟随：浮游炮的响应随速度变硬，双手固定。旧 UpdateP2HandPosNormally，CrystallineSentinel.cs:1801-1807</summary>
        public void UpdatePartFollow()
        {
            EnsurePartRig();

            float t = Utils.Remap(Npc.velocity.Length(), 0, CrystallineSentinelDirector.P2FloatFollowSpeedRef,
                CrystallineSentinelDirector.P2FloatFollowMin, CrystallineSentinelDirector.P2FloatFollowMax);
            FloatCenter = floatMover.Update(t, FloatPos);
            HandCenter[0] = handMovers[0].Update(CrystallineSentinelDirector.P2HandFollow, LeftHandPos);
            HandCenter[1] = handMovers[1].Update(CrystallineSentinelDirector.P2HandFollow, RightHandPos);
        }

        /// <summary>一阶段环绕浮石（纯视觉）：出生与脱战回血后各生成一组。旧 CrystallineSentinel.cs:565-582</summary>
        public void SpawnFloatStones()
        {
            if (Main.dedServ)
            {
                return;
            }

            for (int i = 0; i < CrystallineSentinelDirector.FloatStoneCount; i++)
            {
                CrystallineSentinelFloatStone prt = PRTLoader.NewParticle<CrystallineSentinelFloatStone>(Npc.Center, Vector2.Zero);
                if (prt == null)
                {
                    continue;
                }

                prt.FollowNPCIndex = Npc.whoAmI;
                prt.ai[0] = i;
                float f = i == 0 ? CrystallineSentinelDirector.FloatStoneFirstF : CrystallineSentinelDirector.FloatStoneF;
                float z = i == 0 ? CrystallineSentinelDirector.FloatStoneFirstZ : CrystallineSentinelDirector.FloatStoneZ;
                prt.FloatStoneMoves ??= new SecondOrderDynamics_Vec2(f, z, CrystallineSentinelDirector.FloatStoneR, Npc.Center);
            }
        }

        #endregion
    }
}
