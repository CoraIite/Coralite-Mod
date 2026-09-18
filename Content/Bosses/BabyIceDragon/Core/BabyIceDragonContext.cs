using Coralite.Core.Systems.BossSystem;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon.Core
{
    /// <summary>整体运动声明模式；宿主 <c>ApplyDeclaredMovement</c> 两端同跑地把它翻译成 velocity。</summary>
    public enum BabyIceDragonMoveMode
    {
        /// <summary>未声明：按 <see cref="BabyIceDragonDirector.HoldDamp"/> 刹停。正常迁移后不应命中，只是漏声明的兜底。</summary>
        Hold,
        /// <summary>保持帧间速度不变（旧代码本帧没碰 velocity 的分支）。</summary>
        Keep,
        /// <summary>状态本帧已直接写 velocity（起冲、龙卷转向、砸地停住），宿主不再动它。</summary>
        Direct,
        /// <summary>分轴声明：X / Y 各按 <see cref="BabyIceDragonAxisMode"/> 落地。</summary>
        Axes,
    }

    /// <summary>单轴运动声明。</summary>
    public enum BabyIceDragonAxisMode
    {
        /// <summary>本轴不动。</summary>
        Keep,
        /// <summary>本轴乘衰减系数。</summary>
        Damp,
        /// <summary>本轴 <c>Helper.Movement_SimpleOneLine</c> 追踪（限速 / 加速 / 转向加速 / 超速衰减）。</summary>
        Chase,
        /// <summary>扇翅（仅 Y）：帧图 2 / 3 时向上加速，否则衰减，带上限。旧 <c>FlyUp</c>。</summary>
        Flap,
        /// <summary>匀加速（仅 Y）：每帧加 accel，带上限。旧下砸下落。</summary>
        Accel,
    }

    /// <summary>朝向声明模式。</summary>
    public enum BabyIceDragonRotationMode
    {
        /// <summary>本帧不动。</summary>
        Keep,
        /// <summary>飞行姿态：面向 × Y 速度 × 0.05。旧 <c>NormallyFlyingFrame(changeRot: true)</c>。</summary>
        TiltBySpeed,
        /// <summary>按步长向 0 回正（AngleTowards）。</summary>
        TowardsZero,
        /// <summary>按插值向 0 回正（AngleLerp）。</summary>
        LerpToZero,
        /// <summary>直接朝向速度方向（面向左时加 3.14）。</summary>
        FaceVelocity,
        /// <summary>按步长转向速度方向。</summary>
        TowardsVelocity,
        /// <summary>状态本帧已直接写 rotation。</summary>
        Direct,
    }

    /// <summary>帧图声明模式。</summary>
    public enum BabyIceDragonFrameMode
    {
        /// <summary>本帧不翻页（状态直接写了 frame 或保持）。</summary>
        Keep,
        /// <summary>飞行帧：每 7 帧翻页，Y 0..3 循环，X 由 <see cref="BabyIceDragonContext.FrameX"/> 指定。旧 <c>NormallyFlyingFrame</c>。</summary>
        Flying,
        /// <summary>眩晕帧：落地才翻页，悬空停在第 4 帧。旧 <c>DizzyBody</c>。</summary>
        Dizzy,
    }

    /// <summary>
    /// 冰龙宝宝 FSM 上下文：每帧声明总线 + 跨帧事实 + 外部请求口。<br/>
    /// 旧代码的 <c>ai[2]=Timer</c> 让位给基座 SonState 槽，Timer 改走状态热字段；<c>localAI[0..3]</c>（GlowAlpha / DoubleDashAngle / DoubleDashLength / DropScaleCount）
    /// 与 <c>movePhase</c> 分别变成每帧声明、状态自用热槽与权威端事实；帧图相位随 <see cref="WriteFacts"/> 过线（扇翅上飞的加速度读它，C3）。
    /// </summary>
    public sealed class BabyIceDragonContext : CoraliteBossContext
    {
        public BabyIceDragon Boss { get; }

        /// <summary>当前目标玩家（<c>Npc.target</c> 原版同步；无效时指向 255 号占位玩家，不会为 null）。</summary>
        public Player Target => Main.player[Npc.target];

        public BabyIceDragonContext(BabyIceDragon boss) : base(boss.NPC, boss)
        {
            Boss = boss;
            Array.Fill(RecentPicks, -1);
            RefillMovePool(1);
            // 已迁移：关闭 6 px/f 遗留阀，改走决策点 MarkDecision + 45 帧心跳 + 纠偏器。
            UseLegacySpeedValve = false;
        }

        #region 事实（跨帧；权威端裁决量只有权威端读写才算数）

        /// <summary>阶段：1 血量高于阈值，2 低于（大师 3/4，其余 1/2）。两端由已同步的 life 各自推导。</summary>
        public int Phase { get; private set; } = 1;

        /// <summary>CheckDead 请求死亡演出（权威端写，<see cref="BabyIceDragonStateBase"/> 的 ServerUpdate 消费）。</summary>
        public bool KillRequested { get; set; }

        /// <summary>是否仍待进入二阶段（进入时固定先放吼叫动画）。旧 <c>BabyIceDragon.ExchangeState</c>，只有权威端选招读它。</summary>
        public bool ExchangeState { get; set; } = true;

        /// <summary>普通招式计数，超过门槛后归零并出一次有破绽动作。旧 <c>NormalMoveCount</c>，权威端量。</summary>
        public int NormalMoveCount { get; set; }

        /// <summary>可枯竭招池：用过即移除，空了先兜底再重填。旧 <c>Moves</c>，权威端量。</summary>
        public List<BabyIceDragonStateId> Moves { get; } = new List<BabyIceDragonStateId>();

        /// <summary>低血量暴击掉落冰鳞的计数（上限 8）。旧 localAI[3]，只有权威端掉落。</summary>
        public int DropScaleCount { get; set; }

        /// <summary>外部请求：眩晕帧数（&gt; 0 有效）。俯冲撞墙与冰球被击破都走这里，由状态基类经 ServerUpdate 返回值切换。</summary>
        public int PendingDizzyFrames { get; set; }

        /// <summary>外部请求：休息帧数（&gt; 0 有效）。冰球爆炸后由 <c>IceCube.OnKill</c>（仅服务端）发起。</summary>
        public int PendingRestFrames { get; set; }

        /// <summary>离开雪原的累计帧数（在雪原内归零），超过门槛脱战。旧 <c>FlyAwayTimer</c>。</summary>
        public int FlyAwayTimer { get; set; }

        /// <summary>上一手提交的状态 id（-1 无）。</summary>
        public int LastPickedState { get; set; } = -1;

        /// <summary>最近几手的环形记录（查重记账，本轮只记不裁决）。</summary>
        public int[] RecentPicks { get; } = new int[BabyIceDragonDirector.RecentPickWindow];
        private int recentPickCursor;

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

        /// <summary>按阶段重填招池。旧 <c>ResetMovePool</c>（BabyIceDragon.cs:953-997）。</summary>
        public void RefillMovePool(int phase)
        {
            Moves.Clear();
            BabyIceDragonStateId[] pool = phase >= 2
                ? (Main.masterMode ? BabyIceDragonDirector.Phase2MasterPool : BabyIceDragonDirector.Phase2NormalPool)
                : BabyIceDragonDirector.Phase1Pool;
            Moves.AddRange(pool);
        }

        /// <summary>每帧只读事实：阶段。</summary>
        public void UpdateFacts()
        {
            Phase = Npc.life < (int)(Npc.lifeMax * BabyIceDragonDirector.Phase2LifeRatio()) ? 2 : 1;
        }

        /// <summary>请求眩晕（仅权威端有效）。</summary>
        public void RequestDizzy(int frames)
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            PendingDizzyFrames = frames;
            MarkDecision();
        }

        /// <summary>请求休息（仅权威端有效）。</summary>
        public void RequestRest(int frames)
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            PendingRestFrames = frames;
            MarkDecision();
        }

        #endregion

        #region 几何小件（两端同算）

        /// <summary>面向死区：与目标 X 距离小于 16 px 不改面向。旧 <c>SetDirection</c>。</summary>
        public void FaceTarget()
        {
            if (Math.Abs(Npc.Center.X - Target.Center.X) < BabyIceDragonDirector.FacingDeadZone)
            {
                return;
            }

            Npc.direction = Npc.Center.X > Target.Center.X ? -1 : 1;
            Npc.spriteDirection = Npc.direction;
        }

        /// <summary>面向左时叠加的翻转角。旧代码朝向用 3.14、嘴部用 3.141，原样沿用。</summary>
        public float FacingFlip => Npc.direction > 0 ? 0f : 3.14f;

        /// <summary>嘴部方向与位置：沿 rotation（面向左时加 3.141）前伸 40 × 缩放。旧 <c>GetMouseCenter</c>。</summary>
        public void MouthGeometry(out Vector2 mouthDir, out Vector2 mouthCenter)
        {
            mouthDir = (Npc.rotation + (Npc.direction > 0 ? 0f : 3.141f)).ToRotationVector2();
            mouthCenter = Npc.Center + (mouthDir * BabyIceDragonDirector.MouthOffset * Npc.scale);
        }

        /// <summary>嘴部位置。</summary>
        public Vector2 MouthCenter()
        {
            MouthGeometry(out _, out Vector2 center);
            return center;
        }

        /// <summary>眩晕星星锚点（悬空帧略低）。旧 <c>GetDizzyStarCenter</c>。</summary>
        public Vector2 DizzyStarCenter()
        {
            float offsetY = Npc.frame.Y == BabyIceDragonDirector.DizzyFrameAirY
                ? BabyIceDragonDirector.DizzyStarOffsetYAir
                : BabyIceDragonDirector.DizzyStarOffsetY;
            return Npc.Center + new Vector2(Npc.direction * BabyIceDragonDirector.DizzyStarOffsetX, offsetY);
        }

        #endregion

        #region 声明通道（每帧重声明，BeginFrameDefaults 回默认）

        internal BabyIceDragonMoveMode MoveMode { get; set; }
        internal BabyIceDragonAxisMode XMode { get; set; }
        internal BabyIceDragonAxisMode YMode { get; set; }
        /// <summary>X 轴：追踪方向 / 限速 / 加速 / 转向加速 / 衰减（Damp 模式只用衰减）。</summary>
        public int XDir { get; set; }
        public float XSpeed { get; set; }
        public float XAccel { get; set; }
        public float XTurn { get; set; }
        public float XDamp { get; set; }
        /// <summary>Y 轴：追踪方向 / 限速 / 加速 / 转向加速 / 衰减；Flap / Accel 模式用 <see cref="YAccel"/> 与 <see cref="YLimit"/>。</summary>
        public int YDir { get; set; }
        public float YSpeed { get; set; }
        public float YAccel { get; set; }
        public float YTurn { get; set; }
        public float YDamp { get; set; }
        public float YLimit { get; set; }

        internal BabyIceDragonRotationMode RotationMode { get; set; }
        /// <summary>Towards / Lerp 类朝向模式的步长或插值。</summary>
        public float RotationStep { get; set; }

        internal BabyIceDragonFrameMode FrameMode { get; set; }
        /// <summary>飞行帧的 X 列（0 普通、1 张嘴）。</summary>
        public int FrameX { get; set; }

        /// <summary>本帧无敌（<c>dontTakeDamage</c>）。原版不同步这个标志，必须两端每帧同声明。</summary>
        public bool Invulnerable { get; set; }
        /// <summary>本帧受重力（<c>!noGravity</c>）。原版不同步，两端每帧同声明；默认关。</summary>
        public bool Gravity { get; set; }
        /// <summary>本帧与物块碰撞（<c>!noTileCollide</c>）。原版不同步，两端每帧同声明；默认关。</summary>
        public bool TileCollide { get; set; }

        /// <summary>本帧绘制残影（纯表现，由宿主维护残影缓存）。旧 <c>canDrawShadows</c>。</summary>
        public bool DrawShadows { get; set; }
        /// <summary>本帧辉光透明度（纯表现）。旧 localAI[0]。</summary>
        public float GlowAlpha { get; set; }

        public override void BeginFrameDefaults()
        {
            base.BeginFrameDefaults();

            MoveMode = BabyIceDragonMoveMode.Hold;
            XMode = BabyIceDragonAxisMode.Keep;
            YMode = BabyIceDragonAxisMode.Keep;
            RotationMode = BabyIceDragonRotationMode.Keep;
            RotationStep = 0f;
            FrameMode = BabyIceDragonFrameMode.Keep;
            FrameX = 0;
            Invulnerable = false;
            Gravity = false;
            TileCollide = false;
            DrawShadows = false;
            GlowAlpha = 0f;
        }

        /// <summary>保持速度。</summary>
        public void DeclareKeep() => MoveMode = BabyIceDragonMoveMode.Keep;

        /// <summary>状态自管速度（本帧已直接写 velocity）。</summary>
        public void DeclareDirect() => MoveMode = BabyIceDragonMoveMode.Direct;

        /// <summary>整体衰减 <c>velocity *= factor</c>。</summary>
        public void DeclareDamp(float factor)
        {
            DeclareDampX(factor);
            DeclareDampY(factor);
        }

        /// <summary>X 轴衰减。</summary>
        public void DeclareDampX(float factor)
        {
            MoveMode = BabyIceDragonMoveMode.Axes;
            XMode = BabyIceDragonAxisMode.Damp;
            XDamp = factor;
        }

        /// <summary>Y 轴衰减。</summary>
        public void DeclareDampY(float factor)
        {
            MoveMode = BabyIceDragonMoveMode.Axes;
            YMode = BabyIceDragonAxisMode.Damp;
            YDamp = factor;
        }

        /// <summary>X 轴追踪（方向默认取 <c>Npc.direction</c>）。</summary>
        public void DeclareChaseX(float speed, float accel, float turnAccel, float damp, int? dir = null)
        {
            MoveMode = BabyIceDragonMoveMode.Axes;
            XMode = BabyIceDragonAxisMode.Chase;
            XDir = dir ?? Npc.direction;
            XSpeed = speed;
            XAccel = accel;
            XTurn = turnAccel;
            XDamp = damp;
        }

        /// <summary>X 轴就位：与目标 X 距离超过死区就追踪，否则按 idleDamp 衰减。旧代码各招就位段的 X 分支。</summary>
        public void DeclareApproachX(float deadZone, float speed, float accel, float turnAccel, float damp, float idleDamp)
        {
            if (Math.Abs(Target.Center.X - Npc.Center.X) > deadZone)
            {
                DeclareChaseX(speed, accel, turnAccel, damp);
            }
            else
            {
                DeclareDampX(idleDamp);
            }
        }

        /// <summary>Y 轴追踪（方向由调用方给出，同时写回 <c>Npc.directionY</c>，与旧代码一致）。</summary>
        public void DeclareChaseY(int dirY, float speed, float accel, float turnAccel, float damp)
        {
            MoveMode = BabyIceDragonMoveMode.Axes;
            YMode = BabyIceDragonAxisMode.Chase;
            Npc.directionY = dirY;
            YDir = dirY;
            YSpeed = speed;
            YAccel = accel;
            YTurn = turnAccel;
            YDamp = damp;
        }

        /// <summary>
        /// Y 轴悬停到目标上方：方向按 <paramref name="dirOffsetY"/> 判、距离按 <paramref name="lengthOffsetY"/> 算（旧代码休息态两者不同，原样保留），
        /// 超过死区就追踪，否则按同一衰减系数收 Y。旧代码各招“保持在玩家头顶”的 Y 分支。
        /// </summary>
        public void DeclareHoverY(float dirOffsetY, float lengthOffsetY, float deadZone, float speed, float accel, float turnAccel, float damp)
        {
            int dirY = (Target.Center.Y - dirOffsetY) > Npc.Center.Y ? 1 : -1;
            float yLength = Math.Abs(Target.Center.Y - lengthOffsetY - Npc.Center.Y);
            if (yLength > deadZone)
            {
                DeclareChaseY(dirY, speed, accel, turnAccel, damp);
            }
            else
            {
                Npc.directionY = dirY;
                DeclareDampY(damp);
            }
        }

        /// <summary>扇翅上飞（Y）：帧图 2 / 3 向上加速，否则衰减，带上限。</summary>
        public void DeclareFlapY(float accel, float damp, float limit)
        {
            MoveMode = BabyIceDragonMoveMode.Axes;
            YMode = BabyIceDragonAxisMode.Flap;
            YAccel = accel;
            YDamp = damp;
            YLimit = limit;
        }

        /// <summary>旧 <c>FlyUp</c>：扇翅上飞 + 飞行帧（不改朝向）。</summary>
        public void DeclareFlyUp()
        {
            DeclareFlapY(BabyIceDragonDirector.FlyUpAccel, BabyIceDragonDirector.FlyUpDamp, BabyIceDragonDirector.FlyUpLimit);
            DeclareFlyingFrame(0, false);
        }

        /// <summary>Y 匀加速（下落），带上限。</summary>
        public void DeclareAccelY(float accel, float max)
        {
            MoveMode = BabyIceDragonMoveMode.Axes;
            YMode = BabyIceDragonAxisMode.Accel;
            YAccel = accel;
            YLimit = max;
        }

        /// <summary>朝向声明。</summary>
        public void DeclareRotation(BabyIceDragonRotationMode mode, float step = 0f)
        {
            RotationMode = mode;
            RotationStep = step;
        }

        /// <summary>飞行帧声明；<paramref name="tilt"/> 为真时朝向按 Y 速度倾斜（旧 <c>NormallyFlyingFrame(xFrame, changeRot)</c>）。</summary>
        public void DeclareFlyingFrame(int xFrame = 0, bool tilt = true)
        {
            FrameMode = BabyIceDragonFrameMode.Flying;
            FrameX = xFrame;
            if (tilt)
            {
                RotationMode = BabyIceDragonRotationMode.TiltBySpeed;
            }
        }

        /// <summary>眩晕帧声明：帧列固定为眩晕列，落地才翻页、悬空定格。旧 <c>DizzyBody</c>。</summary>
        public void DeclareDizzyFrame() => FrameMode = BabyIceDragonFrameMode.Dizzy;

        /// <summary>只改帧行、保留当前帧列（旧代码若干处只写 <c>frame.Y</c>，列沿用上一招留下的值）。</summary>
        public void SetFrameY(int y) => SetFrame(Npc.frame.X, y);

        /// <summary>直接写一格帧图（吼叫 / 合翅等定格），本帧不翻页。</summary>
        public void SetFrame(int x, int y)
        {
            Npc.frame.X = x;
            Npc.frame.Y = y;
            FrameMode = BabyIceDragonFrameMode.Keep;
        }

        #endregion

        #region 同步事实

        /// <summary>
        /// 帧图相位（X 列、Y 行、翻页计数各一字节）：扇翅上飞的加速度读 <c>frame.Y</c>，客户端必须拿到同一相位才能复现爬升（C3）；顺带让动画不漂。
        /// </summary>
        protected override void WriteFacts(BinaryWriter writer)
        {
            writer.Write((byte)Math.Clamp(Npc.frame.X, 0, byte.MaxValue));
            writer.Write((byte)Math.Clamp(Npc.frame.Y, 0, byte.MaxValue));
            writer.Write((byte)Math.Clamp((int)Npc.frameCounter, 0, byte.MaxValue));
        }

        protected override void ReadFacts(BinaryReader reader)
        {
            Npc.frame.X = reader.ReadByte();
            Npc.frame.Y = reader.ReadByte();
            Npc.frameCounter = reader.ReadByte();
        }

        #endregion
    }
}
