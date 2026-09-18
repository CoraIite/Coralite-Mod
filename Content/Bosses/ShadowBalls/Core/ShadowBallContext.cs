using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.Core
{
    /// <summary>运动声明模式；宿主 <c>ApplyDeclaredMovement</c> 两端同跑地把它翻译成 velocity。</summary>
    public enum ShadowBallMoveMode
    {
        /// <summary>保持帧间速度不变（默认；漏声明时不会突然刹车）。</summary>
        Keep,
        /// <summary>整体衰减 <c>velocity *= DampFactor</c>。</summary>
        Damp,
        /// <summary>朝一个点趋近：出死区就向它插值加速，进死区就衰减（旧代码里"保持身位"的通用写法）。</summary>
        Approach,
        /// <summary>状态本帧已直接写 velocity（引力牵引、上戳、环射到位），宿主不再动它。</summary>
        Direct,
    }

    /// <summary>朝向声明模式。</summary>
    public enum ShadowBallRotationMode
    {
        /// <summary>本帧不动 rotation。</summary>
        Keep,
        /// <summary>朝 <see cref="ShadowBallContext.RotationTarget"/> 做 AngleLerp。</summary>
        LerpTo,
    }

    /// <summary>
    /// 影子球本体 FSM 上下文：每帧声明总线 + 跨帧事实 + 引力牵引这套公共运动数学。<br/><br/>
    /// <b>ai 槽归位</b>：旧代码把 <c>SonState</c> 放 ai[1]、<c>Recorder</c> 放 ai[2]、<c>Timer</c> 放 ai[3]，
    /// 与基座约定（ai[1]=AttackSeed、ai[2]=SonState、ai[3]=SyncTimer）错开一格——基座 <c>OnEnter</c> 的
    /// <c>RollAttackSeed()</c> 会往 ai[1] 写一个几亿的随机数，正好落在旧 <c>SonState</c> 上。现在全部让位：
    /// 子拍走状态的 <c>BeatIndex</c>（热槽 Beat），计时器走状态 <c>Timer</c>（热槽 Timer），
    /// 各招式的 Recorder 走热槽 A..H，ai[1..3] 还给基座。
    /// </summary>
    public sealed class ShadowBallContext : CoraliteBossContext
    {
        public ShadowBall Boss { get; }

        /// <summary>当前目标玩家（<c>Npc.target</c> 原版同步；无效时指向 255 号占位玩家，不会为 null）。</summary>
        public Player Target => Main.player[Npc.target];

        public ShadowBallContext(ShadowBall boss) : base(boss.NPC, boss)
        {
            Boss = boss;
            // 已迁移：关闭 6 px/f 遗留阀，改走决策点 MarkDecision + 45 帧心跳 + 纠偏器。
            UseLegacySpeedValve = false;
        }

        #region 事实（跨帧）

        /// <summary>
        /// 本次召唤要放出多少个小球。旧代码由 <c>SwitchP1State</c> 写进 ai[2]（<c>Recorder</c>）。
        /// 客户端要用它跑同一份"挑哪些锁扣弹出"的确定性抽取，所以随 <see cref="WriteFacts"/> 过线。
        /// </summary>
        public int SummonCount { get; set; }

        /// <summary>上一手提交的状态 id（-1 无）。只有权威端选招读它。</summary>
        public int LastPickedState { get; set; } = -1;

        /// <summary>每帧只读事实：面向。旧代码没有独立的索敌/面向段，这里只保证招式体读到的是本帧新值。</summary>
        public void UpdateFacts()
        {
            Npc.direction = Target.Center.X > Npc.Center.X ? 1 : -1;
            Npc.directionY = Target.Center.Y > Npc.Center.Y ? 1 : -1;
        }

        /// <summary>小球数量 ≤ 90（天顶 30×3），一个字节装得下。</summary>
        protected override void WriteFacts(BinaryWriter writer)
        {
            writer.Write((byte)Utils.Clamp(SummonCount, 0, byte.MaxValue));
        }

        protected override void ReadFacts(BinaryReader reader)
        {
            SummonCount = reader.ReadByte();
        }

        #endregion

        #region 声明通道（每帧重声明，BeginFrameDefaults 回默认）

        internal ShadowBallMoveMode MoveMode { get; set; }
        /// <summary>Damp / Approach 死区内的衰减系数。</summary>
        public float DampFactor { get; set; }
        /// <summary>Approach 模式：目标点、死区半径、目标速度、每帧插值。</summary>
        public Vector2 ApproachPoint { get; set; }
        public float ApproachDeadZone { get; set; }
        public float ApproachSpeed { get; set; }
        public float ApproachLerp { get; set; }

        internal ShadowBallRotationMode RotationMode { get; set; }
        /// <summary>LerpTo 模式的目标角与插值系数。</summary>
        public float RotationTarget { get; set; }
        public float RotationLerp { get; set; }

        public override void BeginFrameDefaults()
        {
            base.BeginFrameDefaults();

            MoveMode = ShadowBallMoveMode.Keep;
            DampFactor = 1f;
            RotationMode = ShadowBallRotationMode.Keep;
            RotationLerp = 0f;
        }

        /// <summary>整体衰减。</summary>
        public void DeclareDamp(float factor)
        {
            MoveMode = ShadowBallMoveMode.Damp;
            DampFactor = factor;
        }

        /// <summary>保持速度。</summary>
        public void DeclareKeep() => MoveMode = ShadowBallMoveMode.Keep;

        /// <summary>状态自管速度（本帧已直接写 velocity）。</summary>
        public void DeclareDirect() => MoveMode = ShadowBallMoveMode.Direct;

        /// <summary>朝一点趋近：出死区插值加速、进死区衰减。</summary>
        public void DeclareApproach(Vector2 point, float deadZone, float speed, float lerp, float damp)
        {
            MoveMode = ShadowBallMoveMode.Approach;
            ApproachPoint = point;
            ApproachDeadZone = deadZone;
            ApproachSpeed = speed;
            ApproachLerp = lerp;
            DampFactor = damp;
        }

        /// <summary>朝向声明。</summary>
        public void DeclareRotation(float target, float lerp)
        {
            RotationMode = ShadowBallRotationMode.LerpTo;
            RotationTarget = target;
            RotationLerp = lerp;
        }

        #endregion

        #region 引力牵引（公转 / 星轨 / 月食 / 影刺共用）

        /// <summary>
        /// 起手：权威端放出牵引裂隙弹幕并标决策点，两端同步减速并把锁环切成带角度的同心圆。<br/>
        /// 旧 <c>ShadowBall.GravityMoveMentReady</c>（ShadowBall.cs:940-949）把弹幕下标写进 <c>Recorder</c>，
        /// 而 <c>NewProjectileDirectInAI_Server</c> 在客户端返回 <see langword="null"/>——联机客户端跑到那一行必然空引用崩溃。
        /// 现在改为：运动数学只认锚点（两端各自确定性算出同一个点，并随热槽过线给中途加入者），
        /// 弹幕退回纯表现，客户端不再需要知道它的下标（弹幕下标本来也不跨机器一致）。
        /// </summary>
        public void GravityMoveReady(Vector2 anchor, float slowDownPercent = ShadowBallDirector.GravitySlowDown)
        {
            FadeGravityRifts();

            Npc.velocity *= slowDownPercent;
            Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCirclesAngled);

            if (VaultUtils.isClient)
            {
                return;
            }

            Npc.NewProjectileDirectInAI_Server<DragProj>(anchor, Vector2.Zero, 0, 0, ai0: Npc.whoAmI);
            MarkDecision();
        }

        /// <summary>
        /// 每帧的引力牵引积分（两端同跑）：速度沿 <c>X3Ease</c> 爬坡、封顶 45，朝锚点转向；
        /// 返回 <see langword="true"/> 表示本帧到达（速度清零并让裂隙开始收束）。旧 <c>ShadowBall.GravityMovement</c>（ShadowBall.cs:957-985）。
        /// </summary>
        public bool GravityMove(Vector2 anchor, int timer)
        {
            DeclareDirect();

            float speed = Npc.velocity.Length();
            Vector2 dir = anchor - Npc.Center;

            speed += (Helper.X3Ease(Helper.Clamp(timer / ShadowBallDirector.GravityRampFrames, 0, 1)) * ShadowBallDirector.GravityAccel)
                + ShadowBallDirector.GravityAccelBase;

            if (speed > ShadowBallDirector.GravityMaxSpeed)
            {
                speed = ShadowBallDirector.GravityMaxSpeed;
            }

            Npc.velocity = dir.SafeNormalize(Vector2.Zero) * speed;
            Npc.rotation = Npc.rotation.AngleLerp(dir.ToRotation(), ShadowBallDirector.GravityRotationLerp);

            // 到点 = 本帧位移已经够到锚点；另加超时兜底（旧代码没有，牵引一旦出岔子就会一直飞下去，D2）。
            if (dir.LengthSquared() >= speed * speed && timer < ShadowBallDirector.GravityTimeoutFrames)
            {
                return false;
            }

            Npc.velocity = Vector2.Zero;
            FadeGravityRifts();
            return true;
        }

        /// <summary>
        /// 让本体名下还在张开的牵引裂隙开始收束（两端同跑，纯表现）。<br/>
        /// 不按下标找而是扫一遍：弹幕下标在服务端与客户端并不保证一致，本体 NPC 下标才一致。
        /// </summary>
        public void FadeGravityRifts()
        {
            int riftType = ModContent.ProjectileType<DragProj>();
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == riftType && proj.ModProjectile is DragProj rift
                    && (int)rift.NpcIndex == Npc.whoAmI && rift.State < 2)
                {
                    rift.TurnToFade();
                }
            }
        }

        #endregion
    }
}
