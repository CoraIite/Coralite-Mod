using Coralite.Core.Systems.BossSystem;
using System;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon.Core
{
    /// <summary>运动声明模式；宿主 <c>ApplyDeclaredMovement</c> 两端同跑地把它翻译成 velocity。</summary>
    public enum ThunderveinMoveMode
    {
        /// <summary>未声明：按 <see cref="ThunderveinDirector.HoldDamp"/> 刹停。正常迁移后不应命中，只是漏声明的兜底。</summary>
        Hold,
        /// <summary>保持帧间速度不变（旧代码本帧没有任何速度写入的拍：吼叫等待、收势、幻影期悬停）。</summary>
        Keep,
        /// <summary>状态本帧已直接写 velocity / Center（冲刺、绕飞、瞬移悬停），宿主不再动它。</summary>
        Direct,
        /// <summary>整体衰减 <c>velocity *= DampFactor</c>。</summary>
        Damp,
        /// <summary>按 <see cref="ThunderveinChaseProfile"/> 分轴追踪 <see cref="ThunderveinDragonContext.ChaseTarget"/>（旧各招式开头那段追击块）。</summary>
        Chase,
    }

    /// <summary>朝向声明模式。</summary>
    public enum ThunderveinRotationMode
    {
        /// <summary>本帧不动 rotation（状态已直接写，或旧代码本帧没有朝向代码）。</summary>
        Keep,
        /// <summary>飞行朝向：目标角 = 竖直速度 × 0.05 × 面向（旧 <c>SetRotationNormally</c>）。</summary>
        Normal,
        /// <summary>回正到水平（旧 <c>TurnToNoRot</c>）。</summary>
        NoRot,
    }

    /// <summary>
    /// 一组追踪参数（旧各招式开头 20 行追击块的参数化）：<br/>
    /// X 轴：横向距离小于 <see cref="Near"/> 后退、大于 <see cref="Far"/> 前进、之间衰减；<br/>
    /// Y 轴：目标在上方时向上飞（翅膀帧 ≤ 4 才有推力），否则纵向距离大于 <see cref="ThresholdY"/> 追、之间衰减。
    /// </summary>
    public readonly struct ThunderveinChaseProfile
    {
        /// <summary>横向距离小于此值就后退；0 = 从不后退。</summary>
        public readonly float Near;
        /// <summary>横向距离大于此值才前进。</summary>
        public readonly float Far;
        public readonly float SpeedX;
        public readonly float AccelX;
        public readonly float TurnX;
        /// <summary>纵向距离大于此值才追 Y。</summary>
        public readonly float ThresholdY;
        public readonly float SpeedY;
        public readonly float AccelY;
        public readonly float TurnY;
        /// <summary>目标在上方时是否走“向上飞”分支（冲刺放电蓄力段没有这一分支）。</summary>
        public readonly bool FlyUp;
        public readonly float FlyUpAccel;
        public readonly float FlyUpMax;
        public readonly float FlyUpSlow;
        /// <summary>飞行帧是否张嘴。</summary>
        public readonly bool OpenMouth;
        /// <summary>
        /// 追踪时是否由宿主推进飞行帧。只有冲刺放电的蓄力段为 false——那一拍自己按固定节奏把翅膀帧推到 4，
        /// 宿主再推一次会让帧跑过头、提前满足出手门槛。旧 AI.DashDischarging.cs:111-118
        /// </summary>
        public readonly bool AdvanceFlyingFrame;

        public ThunderveinChaseProfile(float near, float far, float speedX, float accelX, float turnX,
            float thresholdY, float speedY, float accelY, float turnY,
            bool flyUp, float flyUpAccel, float flyUpMax, float flyUpSlow, bool openMouth, bool advanceFlyingFrame = true)
        {
            Near = near;
            Far = far;
            SpeedX = speedX;
            AccelX = accelX;
            TurnX = turnX;
            ThresholdY = thresholdY;
            SpeedY = speedY;
            AccelY = accelY;
            TurnY = turnY;
            FlyUp = flyUp;
            FlyUpAccel = flyUpAccel;
            FlyUpMax = flyUpMax;
            FlyUpSlow = flyUpSlow;
            OpenMouth = openMouth;
            AdvanceFlyingFrame = advanceFlyingFrame;
        }
    }

    /// <summary>
    /// 荒雷龙 FSM 上下文：每帧声明总线 + 跨帧事实 + 确定性招内随机 + 选招记账。<br/>
    /// 旧 <c>localAI[0..3]</c>（Recorder / Recorder2 / StateRecorder / UseMoveCount）与 <c>phaseValue</c> 全部离开 NPC：
    /// 两个 Recorder 是招式私有量 → 各状态的热字段自用槽；StateRecorder / UseMoveCount 只有权威端选招读 → 本类记账字段；
    /// <see cref="Phase"/> 是客户端运动数学与天空会读到的 boss 级事实 → <see cref="WriteFacts"/> 随 SendExtraAI 过线。ai[1..3] 保持基座约定。
    /// </summary>
    public sealed class ThunderveinDragonContext : CoraliteBossContext
    {
        public ThunderveinDragon Boss { get; }

        /// <summary>当前目标玩家（<c>Npc.target</c> 原版同步；无效时指向 255 号占位玩家，不会为 null）。</summary>
        public Player Target => Main.player[Npc.target];

        public ThunderveinDragonContext(ThunderveinDragon boss) : base(boss.NPC, boss)
        {
            Boss = boss;
            Array.Fill(RecentPicks, -1);
            // 已迁移：关闭 6 px/f 遗留阀，改走决策点 MarkDecision + 45 帧心跳 + 纠偏器。
            UseLegacySpeedValve = false;
        }

        #region 事实（跨帧；权威端裁决量只有权威端读写才算数）

        /// <summary>
        /// 阶段 1～4。由 <c>PhaseController</c> 的 OnFire 在权威端写入（旧 <c>Phase = n</c> 副作用），经 <see cref="WriteFacts"/> 到客户端；
        /// 不能从血量推导——阈值只在可打断招式期间生效，客户端推导会早一拍。
        /// </summary>
        public int Phase { get; set; } = 1;

        /// <summary>CheckDead 请求死亡演出（权威端写，<see cref="ThunderveinStateBase"/> 的 ServerUpdate 消费）。</summary>
        public bool KillRequested { get; set; }

        /// <summary>目标与本体的距离 / 横向方向（每帧只读事实，供选招）。</summary>
        public float TargetDistance { get; private set; }
        public int TargetDirX { get; private set; } = 1;

        /// <summary>
        /// 当前吐息 / 电磁炮的瞄准角（弧度）。瞄准类状态每帧从自己的热字段槽发布到这里，
        /// 弹幕 <c>ElectromagneticCannon</c> 经本体 <c>Recorder</c> 属性读它跟随（旧代码直接读 localAI[0]）。
        /// </summary>
        public float AimAngle { get; set; }

        /// <summary>
        /// 冥雷期幻影的 NPC 索引（-1 = 当前没有幻影）。冥雷状态每帧从自己的热字段槽（已随 SendExtraAI 过线）发布到这里，
        /// 天空层 <c>ThunderveinSky</c> 经本体 <c>PhantomIndex</c> 属性读它绘制（旧代码直接读 localAI[0]）。
        /// </summary>
        public float PhantomIndex { get; set; } = -1f;

        /// <summary>每帧只读事实。</summary>
        public void UpdateFacts()
        {
            Vector2 toTarget = Target.Center - Npc.Center;
            TargetDistance = toTarget.Length();
            TargetDirX = toTarget.X > 0 ? 1 : -1;
        }

        #endregion

        #region 确定性招内随机（派生于已同步的 AttackSeed，两端同序消费）

        /// <summary>招内随机源。<see cref="ThunderveinStateBase.OnEnter"/> 在种子落定之后两端各自派生。</summary>
        public Random AttackRandom { get; private set; }

        public void RefreshAttackRandom() => AttackRandom = CreateAttackRandom();

        public float AttackRandFloat(float min, float max) => min + ((max - min) * (float)AttackRandom.NextDouble());
        public bool AttackRandBool() => AttackRandom.Next(2) == 0;
        public bool AttackRandBool(int num, int den) => AttackRandom.Next(den) < num;
        public int AttackRandSign() => AttackRandom.Next(2) == 0 ? -1 : 1;

        #endregion

        #region 选招记账（仅权威端读写；旧 localAI[2] / localAI[3]）

        /// <summary>上一次短冲之前用的招式：短冲之后选招时把它也剔掉，避免“A → 短冲 → A”。旧 localAI[2] StateRecorder。</summary>
        public int StateBeforeSmallDash { get; set; }

        /// <summary>二阶段起累计出招数，超过门槛后引力雷球按它加权，用过即归零。旧 localAI[3] UseMoveCount。</summary>
        public int UseMoveCount { get; set; }

        /// <summary>上一手提交的状态 id（-1 无）。</summary>
        public int LastPickedState { get; set; } = -1;

        /// <summary>最近几手的环形记录（查重记账，本轮只记不裁决）。</summary>
        public int[] RecentPicks { get; } = new int[ThunderveinDirector.RecentPickWindow];
        private int recentPickCursor;

        /// <summary>过账：写上一手 + 推环形窗口。所有出招都必须经 hub 的 Commit 到这里。</summary>
        public void RecordPick(int stateId)
        {
            LastPickedState = stateId;
            RecentPicks[recentPickCursor] = stateId;
            recentPickCursor = (recentPickCursor + 1) % RecentPicks.Length;
        }

        /// <summary>当前状态 id（ai[0]；旧 <c>ResetStates</c> 的 oldState）。</summary>
        public int CurrentStateId => Machine?.CurrentHotState?.StateId ?? (int)Npc.ai[StateAiSlot];

        /// <summary>是否处于可被阶段切换打断的常规招式（排除出生 / 死亡 / 阶段切换 / 冥雷自身；已请求死亡时也不打断）。旧 ThunderveinDragon.cs:466-473</summary>
        public bool IsInterruptibleAttack()
        {
            int id = CurrentStateId;
            return !KillRequested
                && id != (int)ThunderveinDragon.AIStates.onSpawnAnmi
                && id != (int)ThunderveinDragon.AIStates.onKillAnim
                && id != (int)ThunderveinDragon.AIStates.ExchangeP1_P2
                && id != (int)ThunderveinDragon.AIStates.StygianThunder;
        }

        #endregion

        #region 声明通道（每帧重声明，BeginFrameDefaults 回默认）

        public ThunderveinMoveMode MoveMode { get; set; }
        /// <summary>Damp 模式的衰减系数。</summary>
        public float DampFactor { get; set; }
        /// <summary>Chase 模式的目标点与参数。</summary>
        public Vector2 ChaseTarget { get; set; }
        public ThunderveinChaseProfile Chase { get; set; }

        public ThunderveinRotationMode RotationMode { get; set; }
        /// <summary>Normal / NoRot 的插值速率。</summary>
        public float RotationRate { get; set; }

        /// <summary>本帧无敌（<c>dontTakeDamage</c>）。原版不同步这个标志，必须两端每帧同声明。</summary>
        public bool Invulnerable { get; set; }
        /// <summary>本帧身上带电：受击减伤 40% + 持续电粒子（旧 <c>currentSurrounding</c>）。命中方客户端会读到，两端每帧同声明。</summary>
        public bool CurrentSurrounding { get; set; }
        /// <summary>本帧绘制残影（旧 <c>canDrawShadows</c>）。</summary>
        public bool DrawShadows { get; set; }
        /// <summary>本帧绘制冲刺特效贴图（旧 <c>isDashing</c>）。</summary>
        public bool IsDashing { get; set; }

        /// <summary>表现包络（跨帧、不过线）：本体透明度、残影透明度 / 缩放、死亡白光透明度。旧 selfAlpha / shadowAlpha / shadowScale / anmiAlpha。</summary>
        public float SelfAlpha { get; set; } = 1f;
        public float ShadowAlpha { get; set; } = 1f;
        public float ShadowScale { get; set; } = 1f;
        public float KillAnimAlpha { get; set; }

        public override void BeginFrameDefaults()
        {
            base.BeginFrameDefaults();

            MoveMode = ThunderveinMoveMode.Hold;
            DampFactor = ThunderveinDirector.HoldDamp;
            RotationMode = ThunderveinRotationMode.Keep;
            RotationRate = 0f;
            Invulnerable = false;
            CurrentSurrounding = false;
            DrawShadows = false;
            IsDashing = false;
        }

        /// <summary>保持速度（旧代码本帧无速度写入）。</summary>
        public void DeclareKeep() => MoveMode = ThunderveinMoveMode.Keep;

        /// <summary>状态自管速度（本帧已直接写 velocity 或 Center）。</summary>
        public void DeclareDirect() => MoveMode = ThunderveinMoveMode.Direct;

        /// <summary>整体衰减。</summary>
        public void DeclareDamp(float factor)
        {
            MoveMode = ThunderveinMoveMode.Damp;
            DampFactor = factor;
        }

        /// <summary>分轴追踪。面向（direction / directionY / spriteDirection）由调用方先设好。</summary>
        public void DeclareChase(Vector2 target, in ThunderveinChaseProfile profile)
        {
            MoveMode = ThunderveinMoveMode.Chase;
            ChaseTarget = target;
            Chase = profile;
        }

        /// <summary>飞行朝向（旧 <c>SetRotationNormally</c>）。</summary>
        public void DeclareRotationNormal(float rate = ThunderveinDirector.RotationNormalRate)
        {
            RotationMode = ThunderveinRotationMode.Normal;
            RotationRate = rate;
        }

        /// <summary>回正朝向（旧 <c>TurnToNoRot</c>）。</summary>
        public void DeclareNoRot(float rate = ThunderveinDirector.NoRotRate)
        {
            RotationMode = ThunderveinRotationMode.NoRot;
            RotationRate = rate;
        }

        /// <summary>冲刺三件套：残影 + 冲刺贴图 + 带电（旧代码起冲时同时置位的三个标志）。</summary>
        public void DeclareDashing()
        {
            DrawShadows = true;
            IsDashing = true;
            CurrentSurrounding = true;
        }

        #endregion

        #region 同步事实

        /// <summary>阶段 1～4 一个字节。</summary>
        protected override void WriteFacts(BinaryWriter writer)
        {
            writer.Write((byte)Math.Clamp(Phase, 1, byte.MaxValue));
        }

        protected override void ReadFacts(BinaryReader reader)
        {
            Phase = reader.ReadByte();
        }

        #endregion
    }
}
