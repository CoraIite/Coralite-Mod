using Coralite.Content.NPCs.Crystalline.States;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.Core
{
    /// <summary>
    /// 状态索引，写入 <c>npc.ai[0]</c> 同步。成员与数值沿用旧 <c>CrystallineSentinel.AIStates</c>（线格式不变），追加 <see cref="hub"/>。
    /// </summary>
    internal enum CrystallineSentinelStateId : int
    {
        /// <summary> 一阶段站立 </summary>
        P1Idle = 0,
        /// <summary> 一阶段闲逛 </summary>
        P1Walking = 1,
        /// <summary> 一阶段护盾 </summary>
        P1Guard = 2,
        /// <summary> 一阶段刺击 </summary>
        P1Spurt = 3,
        /// <summary> 一阶段飞弹 </summary>
        P1Missile = 4,
        /// <summary> 一阶段碎岩攻击 </summary>
        P1Rock = 5,
        /// <summary> 转阶段 </summary>
        Exchange = 6,
        /// <summary> 二阶段悬浮 </summary>
        P2Idle = 7,
        /// <summary> 二阶段螺旋冲刺 </summary>
        P2Rolling = 8,
        /// <summary> 二阶段挥刀 </summary>
        P2Swing = 9,
        /// <summary> 二阶段旋风斩 </summary>
        P2WhirlSlash = 10,
        /// <summary> 二阶段旋风斩无前摇、短距离版 </summary>
        P2WhirlSlashShort = 11,
        /// <summary> 二阶段休息动作 </summary>
        P2Rest = 12,
        /// <summary> 二阶段死亡动画 </summary>
        P2Dying = 13,
        /// <summary>连接段 + 唯一提交口（新增；<see cref="CrystallineSentinelDirector.HubFrames"/> 为 0 时通常不驻留）</summary>
        hub = 14,
    }

    /// <summary>
    /// 结晶战斗体状态基类。<br/>
    /// · <c>SharedUpdate</c>：两端同跑——写运动 / 朝向 / 伤害窗声明、推进确定性子拍与帧图、客户端粒子音效。<br/>
    /// · <see cref="AuthorityUpdate"/>：仅权威端——弹幕 / 召唤 / 掷骰 / 选招，返回下一状态；本类把死亡请求、转阶段闸与超时兜底垫在它前面。<br/>
    /// · 收招一律 <see cref="ReturnToIdle"/>，出招一律经 <see cref="CrystallineSentinelHubState"/> 的提交口；招式体内不得出现 <c>ChangeState</c>。
    /// </summary>
    internal abstract class CrystallineSentinelStateBase : CoraliteBossState<CrystallineSentinelContext>
    {
        /// <summary>入场预充帧数（旧 <c>overrideTime</c>）占用的热字段槽，基座只用到 0..2，这里取末位不与子类的 A..G 打架。</summary>
        private const int EntryFramesSlot = CoraliteBossHotSlots.H;

        public abstract CrystallineSentinelStateId StateIndex { get; }

        public override int StateId => (int)StateIndex;

        /// <summary>本态跟哪一份常态 AI（宿主按它分派，旧 AI() 末尾的 <c>IsPhase2 ? P2NormalAI : P1NormalAI</c>）。</summary>
        public virtual CrystallineSentinelCommon Common => CrystallineSentinelCommon.PhaseOne;

        /// <summary>超时兜底帧数；演出态覆盖为更大值。</summary>
        protected virtual int TimeoutFrames => CrystallineSentinelDirector.StateTimeoutFrames;

        /// <summary>收招时回到待机态的预充帧数（旧各招 <c>SwitchState(P1Idle/P2Idle, n)</c> 的 n）。</summary>
        protected virtual int IdleFramesOnFinish => 0;

        /// <summary>
        /// 入场预充帧数：旧 <c>SwitchStateP1/P2</c> 的 <c>overrideTime</c>。正数 = 本态提前起跑（缩短时长），负数 = 先静默这么多帧。<br/>
        /// 随热字段过线，客户端不再需要旧代码那句“给客户端 6 帧同步延迟”的补偿。
        /// </summary>
        protected float EntryFrames { get; private set; }

        /// <summary>
        /// 旧招式体的计时值：<see cref="EntryFrames"/> + 已过帧数。<br/>
        /// 基座的 <c>Timer</c> 在招式体首帧读到 1，而旧代码是“先跑体、体内再 ++”、首帧读到 <c>overrideTime</c>，
        /// 所以这里 −1 对齐——搬过来的 <c>Timer == N</c> 拍点一帧不差，Director 里也就能保留旧数字本身。<br/>
        /// <c>internal</c> 是给 <see cref="CrystallineSentinelContext.StateAttackTimer"/> 用的：
        /// 旧代码的从属弹幕直接拿本体 <c>ai[1]</c> 当计时读，那个槽现在归基座，得有个对外只读口把这个等价值交出去。
        /// </summary>
        protected internal float AttackTimer => EntryFrames + Timer - 1;

        /// <summary>递减型计时的等价值：旧 P1Idle / P1Walking 用 <c>Timer--</c> 从 <see cref="EntryFrames"/> 倒数到负数再换态。</summary>
        protected float CountdownTimer => EntryFrames - Timer + 1;

        public override void OnEnter(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            base.OnEnter(machine, ctx);

            if (!VaultUtils.isClient)
            {
                EntryFrames = ctx.ConsumeEntryFrames();
            }

            // 旧代码每次换状态都放一次机体音（在两端跑的 SwitchState 里），这里挪到入场钩子上，客户端被 NetSync 换态时同样会响。
            if (!Main.dedServ && ctx.SwitchSoundArmed)
            {
                Helper.PlayPitchedVariants(AssetDirectory.Sounds.Crystalline + "Sentinel_Normal", 0.4f, 0, 0, 3, ctx.Npc.Center);
            }
        }

        public override void WriteHot(CrystallineSentinelContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[EntryFramesSlot] = EntryFrames;
            ctx.Hot.SetFlag(CrystallineSentinelContext.CanHitFlag, ctx.CanHit);
        }

        public override void ReadHot(CrystallineSentinelContext ctx)
        {
            base.ReadHot(ctx);
            EntryFrames = ctx.Hot[EntryFramesSlot];
            ctx.CanHit = ctx.Hot.HasFlag(CrystallineSentinelContext.CanHitFlag);
        }

        protected sealed override IVaultState<CrystallineSentinelContext> ServerUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            // CheckDead 只登记请求，换态统一从这里经返回值走（客户端读 ai[0] 跟随）。
            if (ctx.KillRequested && StateIndex != CrystallineSentinelStateId.P2Dying)
            {
                return CrystallineSentinelHubState.ToState(ctx, CrystallineSentinelStateId.P2Dying);
            }

            // 转阶段闸：旧代码写在 P1NormalAI 里、对所有一阶段状态生效（含刚进 Exchange 的那一帧自查）。
            if (Common == CrystallineSentinelCommon.PhaseOne && StateIndex != CrystallineSentinelStateId.Exchange && ctx.ShouldExchange)
            {
                return CrystallineSentinelHubState.ToState(ctx, CrystallineSentinelStateId.Exchange);
            }

            // 超时兜底：状态机永远不许死在一个状态里；收招不留残速。
            if (Counter++ > TimeoutFrames)
            {
                ctx.Npc.velocity *= 0.6f;
                return ReturnToIdle(ctx, IdleFramesOnFinish);
            }

            return AuthorityUpdate(machine, ctx);
        }

        /// <summary>仅权威端：弹幕生成、召唤、掷骰、选招；返回下一状态或 null。</summary>
        protected abstract IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx);

        #region 公共小件

        /// <summary>
        /// 换拍并清掉入场预充。旧代码换拍就是一句 <c>Timer = 0</c>，<c>overrideTime</c> 只作用于第一拍，
        /// 所以换拍之后 <see cref="AttackTimer"/> 必须从 0 重新起算；本 boss 一律用这个口换拍。
        /// </summary>
        /// <param name="startAt">
        /// 新拍第一帧的 <see cref="AttackTimer"/>。默认 0；旧代码有几处换拍之后同一帧还会再走一次 <c>Timer++</c>，
        /// 那些拍点传 1 才和旧节拍逐帧对齐。
        /// </param>
        protected void NextBeat(CrystallineSentinelContext ctx, int beat, float startAt = 0f)
        {
            EntryFrames = startAt;
            SwitchBeat(ctx, beat);
        }

        /// <summary>
        /// 把旧代码里“直接给 Timer 赋一个值”（含负值：护盾中插入一段发射动作就是倒数到 0）的写法搬过来。<br/>
        /// 基座的 Timer 保持单调自增，这里只改入场预充这个偏置，使 <see cref="AttackTimer"/> 当场等于 <paramref name="value"/>；
        /// 偏置随热字段过线，客户端收养后能自洽地推出同一段动作。
        /// </summary>
        protected void RebaseTimer(CrystallineSentinelContext ctx, float value)
        {
            EntryFrames = value - Timer + 1;
            ctx.MarkDecision();
        }

        /// <summary>按 id 建状态；未注册返回 null（调用方要兜底）。</summary>
        protected static IVaultState<CrystallineSentinelContext> Create(CrystallineSentinelStateId id)
            => VaultStateRegistry<CrystallineSentinelContext>.Create((int)id);

        /// <summary>收招：回到本阶段的待机态（二阶段会经休息闸改道）。</summary>
        protected static IVaultState<CrystallineSentinelContext> ReturnToIdle(CrystallineSentinelContext ctx, int entryFrames)
            => CrystallineSentinelHubState.ToIdle(ctx, entryFrames);

        /// <summary>帧图直接赋值。旧 <c>SetFrame</c></summary>
        protected static void SetFrame(CrystallineSentinelContext ctx, int frameX, int frameY)
        {
            ctx.Npc.frame.X = frameX;
            ctx.Npc.frame.Y = frameY;
        }

        /// <summary>按帧速推进帧图并顶在末帧（旧各招 <c>Timer % frameRate == 0</c> 那段）。</summary>
        protected static void AdvanceFrame(CrystallineSentinelContext ctx, int maxFrameY)
        {
            ctx.Npc.frame.Y++;
            if (ctx.Npc.frame.Y > maxFrameY)
            {
                ctx.Npc.frame.Y = maxFrameY;
            }
        }

        /// <summary>走路帧：帧列固定、帧速随水平速度变快。旧 <c>WalkFrame</c>，CrystallineSentinel.cs:731-743</summary>
        protected static void WalkFrame(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.frame.X = CrystallineSentinelDirector.WalkFrameColumn;
            float xSpeed = CrystallineSentinelDirector.WalkFrameRateBase - MathF.Abs(npc.velocity.X);

            if (++npc.frameCounter > xSpeed)
            {
                npc.frameCounter = 0;
                npc.frame.Y++;
            }

            if (npc.frame.Y > CrystallineSentinelDirector.WalkFrameMaxY)
            {
                npc.frame.Y = 0;
            }
        }

        /// <summary>
        /// 前方能不能继续走：脚下 <c>width/16</c> 列、每列往下 3 格内有实心块的列数不少于总列数 − 1。<br/>
        /// 旧 <c>CanWalkForward</c>，CrystallineSentinel.cs:746-777
        /// </summary>
        protected static bool CanWalkForward(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 pos = npc.Bottom;
            pos.X += npc.direction * npc.width / 2;
            pos.Y += CrystallineSentinelDirector.WalkCheckYOffset;

            int checkX = (int)(npc.width / 16f);
            int soildTileCount = 0;

            for (int i = 0; i < checkX; i++)
            {
                bool hasTile = false;

                for (int j = 0; j < CrystallineSentinelDirector.WalkCheckRows; j++)
                {
                    Tile t = Framing.GetTileSafely(pos + new Vector2(npc.direction * i * 16, j * 16));
                    if (t.HasSolidTile())
                    {
                        hasTile = true;
                        break;
                    }
                }

                if (hasTile)
                {
                    soildTileCount++;
                }
            }

            return soildTileCount >= checkX - 1;
        }

        /// <summary>
        /// 能否攻击目标：玩家没隐身（隐身 = 有隐身效果且没在使用物品）、距离小于 1000 且视线可达。<br/>
        /// 旧 <c>CanHitTarget</c>，CrystallineSentinel.cs:1998-2003
        /// </summary>
        protected static bool CanHitTarget(CrystallineSentinelContext ctx, out float distance)
        {
            Player target = ctx.Target;
            distance = Vector2.Distance(ctx.Npc.Center, target.Center);
            return !(target.invis && target.itemAnimation == 0) && distance < CrystallineSentinelDirector.CanHitDistance
                && Collision.CanHit(ctx.Npc.Center, 1, 1, target.TopLeft, target.width, target.height);
        }

        /// <summary>
        /// 确定性噪声 [0,1)：只吃已同步的量（状态 <c>Timer</c> + <c>ai[1]</c> 攻击种子），两端算出同一个值。<br/>
        /// 旧代码的二阶段飞行直接用 <c>Main.rand</c> 给速度加扰动——那是两端各自掷骰、位置必然分叉（C1/C3），
        /// 换成这个之后抖动幅度不变，但两端的轨迹一致。
        /// </summary>
        protected float Noise(CrystallineSentinelContext ctx, int salt = 0)
        {
            float h = MathF.Sin(((Timer + salt) * 12.9898f) + ((int)ctx.AttackSeed % 1000 * 0.7853f)) * 43758.5453f;
            return h - MathF.Floor(h);
        }

        /// <summary>确定性抖动量，取值 [−amplitude, amplitude]。</summary>
        protected float Wobble(CrystallineSentinelContext ctx, float amplitude, int salt = 0)
            => (Noise(ctx, salt) - 0.5f) * 2f * amplitude;

        /// <summary>撞墙反向飞。旧 <c>CollideSpeed</c>，CrystallineSentinel.cs:1281-1294</summary>
        protected static void CollideSpeed(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            if (npc.collideX)
            {
                ctx.MarkDecision();
                npc.velocity.X = npc.oldVelocity.X * CrystallineSentinelDirector.P2CollideBounce;
            }

            if (npc.collideY)
            {
                ctx.MarkDecision();
                npc.velocity.Y = npc.oldVelocity.Y * CrystallineSentinelDirector.P2CollideBounce;
            }
        }

        /// <summary>把速度推向目标模长，超速则衰减。旧 <c>SpeedUp</c>，CrystallineSentinel.cs:1297-1307</summary>
        protected static void SpeedUp(CrystallineSentinelContext ctx, float maxSpeed, float acc, float blurSpeed = CrystallineSentinelDirector.P2SpeedUpBlur)
        {
            NPC npc = ctx.Npc;
            float speed = npc.velocity.Length();
            if (speed < maxSpeed)
            {
                speed += acc;
                npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * speed;
            }
            else if (speed > maxSpeed + blurSpeed)
            {
                npc.velocity *= CrystallineSentinelDirector.P2SpeedUpOverDamp;
            }
        }

        /// <summary>
        /// 二阶段悬停位：绕到玩家背侧、水平拉开至少 120 px 的 0.7 倍、抬高 120 px；离目标点还远才补速度。<br/>
        /// 旧 P2Idle / P2Swing 里那段一模一样的代码（CrystallineSentinel.cs:1258-1264,1353-1359）。
        /// </summary>
        protected void HoverBesideTarget(CrystallineSentinelContext ctx, float hoverSpeed)
        {
            NPC npc = ctx.Npc;
            int dir = npc.Center.X < ctx.Target.Center.X ? -1 : 1;
            float xdist = MathHelper.Clamp(MathF.Abs(ctx.Target.Center.X - npc.Center.X), CrystallineSentinelDirector.P2HoverMinX, int.MaxValue);
            Vector2 targetPos = ctx.Target.Center + new Vector2(dir * xdist * CrystallineSentinelDirector.P2HoverXFactor, CrystallineSentinelDirector.P2HoverOffsetY);

            if (npc.Distance(targetPos) > CrystallineSentinelDirector.P2HoverArriveRange)
            {
                Vector2 wish = npc.DirectionTo(targetPos).RotatedBy(Wobble(ctx, CrystallineSentinelDirector.P2HoverTurnAngle, 7)) * hoverSpeed;
                npc.velocity = Vector2.Lerp(npc.velocity, wish, CrystallineSentinelDirector.P2HoverLerp);
            }
        }

        /// <summary>二阶段身体帧：每 3 帧一格、8 帧循环。旧各招开头那段 <c>frameCounter</c>。</summary>
        protected static void P2BodyFrame(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            if (++npc.frameCounter > CrystallineSentinelDirector.P2BodyFrameRate)
            {
                npc.frameCounter = 0;
                if (++npc.frame.Y > CrystallineSentinelDirector.P2BodyFrameMaxY)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        /// <summary>
        /// 一次性拍：与旧代码一样用相等判定。<see cref="AttackTimer"/> 的两个分量都是整数值的 float，比较是精确的；
        /// 客户端的 Timer 由热字段带 ±2 帧容差收养，正常抖动下不会漏拍，真发生大跳（中途加入）时宁可静默跳过也不补放（C9）。
        /// </summary>
        protected bool AtFrame(float cueFrame) => AttackTimer == cueFrame;

        #endregion
    }
}
