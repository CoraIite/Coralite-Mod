using Coralite.Content.Particles;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.Core
{
    /// <summary>移动方式</summary>
    public enum ShadowBallMoveMode
    {
        /// <summary>自由控制速度</summary>
        LetItGo,
        /// <summary>速度逐渐减慢</summary>
        SlowDown,
        /// <summary>速度朝向一个位置靠近，小于一定值后逐渐减速</summary>
        Approach,
    }

    /// <summary>角度方式</summary>
    public enum ShadowBallRotationMode
    {
        /// <summary>自由控制旋转</summary>
        LetItGo,
        /// <summary>每帧增加多少多少</summary>
        Add,
        /// <summary>朝 <see cref="ShadowBallContext.RotationTarget"/> 做 AngleLerp</summary>
        LerpTo,
        /// <summary>朝 <see cref="ShadowBallContext.RotationTarget"/> 做 AngleTowards</summary>
        Towards,
    }

    public sealed class ShadowBallContext : CoraliteBossContext
    {
        public ShadowBall Boss { get; }

        /// <summary>当前目标玩家</summary>
        public Player Target => Main.player[Npc.target];

        public Vector2 GravityAnchor {
          get=>  new Vector2(Hot[BossSlots.A], Hot[BossSlots.B]);
            set
            {
                Hot[BossSlots.A] = value.X;
                Hot[BossSlots.B] = value.Y;
            }
        }

        public float CoreOffsetAngle
        {
            get => Hot[BossSlots.C];
            set => Hot[BossSlots.C] = value;
        }
        public float CoreOffsetRadius
        {
            get => Hot[BossSlots.D];
            set => Hot[BossSlots.D] = value;
        }

        public float Recorder1
        {
            get => Hot[BossSlots.E];
            set => Hot[BossSlots.E] = value;
        }
        public float Recorder2
        {
            get => Hot[BossSlots.F];
            set => Hot[BossSlots.F] = value;
        }

        /// <summary>上一个状态</summary>
        public int LastPickedState
        {
            get => (int)Hot[BossSlots.G];
            set => Hot[BossSlots.G] = value;
        }

        /// <summary>
        /// 为true时表示进入一阶段的能够使用水银招式
        /// </summary>
        public bool Phase1Mucury => Hot[BossSlots.H] > 1;

        public ShadowBallContext(ShadowBall boss) : base(boss.NPC, boss)
        {
            Boss = boss;
            UseLegacySpeedValve = false;
        }

        #region 各种值

        internal ShadowBallMoveMode MoveMode { get; set; }
        internal ShadowBallRotationMode RotationMode { get; set; }

        /// <summary>Damp / Approach 死区内的衰减系数。</summary>
        public float DampFactor { get; set; }
        /// <summary>Approach 模式：目标点、死区半径、目标速度、每帧插值。</summary>
        public Vector2 ApproachPoint { get; set; }
        public float ApproachDeadZone { get; set; }
        public float ApproachSpeed { get; set; }
        public float ApproachLerp { get; set; }

        /// <summary>LerpTo 模式的目标角与插值系数。</summary>
        public float RotationTarget { get; set; }
        public float RotationF { get; set; }

        public override void BeginFrameDefaults()
        {
            base.BeginFrameDefaults();

            MoveMode = ShadowBallMoveMode.LetItGo;
            DampFactor = 1f;
            RotationMode = ShadowBallRotationMode.LetItGo;
            RotationF = 0f;
        }

        /// <summary>设置速度减慢</summary>
        public void SetVelocitySlowDown(float factor)
        {
            MoveMode = ShadowBallMoveMode.SlowDown;
            DampFactor = factor;
        }

        /// <summary>状态自管速度</summary>
        public void SetVelocityLetItGo() 
            => MoveMode = ShadowBallMoveMode.LetItGo;

        /// <summary>朝一点趋近：出死区插值加速、进死区衰减</summary>
        public void SetVelocityApparoch(Vector2 point, float deadZone, float speed, float lerp, float damp)
        {
            MoveMode = ShadowBallMoveMode.Approach;
            ApproachPoint = point;
            ApproachDeadZone = deadZone;
            ApproachSpeed = speed;
            ApproachLerp = lerp;
            DampFactor = damp;
        }

        /// <summary>不管角度</summary>
        public void SetRotationLetItGo()
        {
            RotationMode = ShadowBallRotationMode.LetItGo;
        }

        /// <summary>lerp角度</summary>
        public void SetRotationAdd(float howManyAdd)
        {
            RotationMode = ShadowBallRotationMode.Add;
            RotationTarget = Npc.rotation+howManyAdd;
        }

        /// <summary>lerp角度</summary>
        public void SetRotationLerp(float target, float lerp)
        {
            RotationMode = ShadowBallRotationMode.LerpTo;
            RotationTarget = target;
            RotationF = lerp;
        }

        /// <summary>lerp角度</summary>
        public void SetRotationToward(float target, float lerp)
        {
            RotationMode = ShadowBallRotationMode.Towards;
            RotationTarget = target;
            RotationF = lerp;
        }

        #endregion

        #region 引力牵引移动

        /// <summary>
        /// 准备进行引力移动，会生成一个弹幕用于确定位置
        /// </summary>
        /// <param name="anchor"></param>
        /// <param name="slowDownPercent"></param>
        public void GravityMoveReady(Vector2 anchor, float slowDownPercent = 0.5f)
        {
            FadeGravityMoveAnchorProj();

            Npc.velocity *= slowDownPercent;
            Npc.netUpdate = true;
            Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCirclesAngled);

            if (VaultUtils.isClient)
                return;

            Npc.NewProjectileDirectInAI_Server<DragProj>(anchor, Vector2.Zero, 0, 0, ai0: Npc.whoAmI);
            NetUpdate();
        }

        /// <summary>
        /// 需要每帧运行的引力移动
        /// </summary>
        public bool GravityMove(Vector2 anchor, int timer, float accTime = 45, float acc = 1.7f, float maxSpeed = 45f)
        {
            SetVelocityLetItGo();

            Boss.ShellBreath_ShowCore(0);
            if (Boss.LayerAlpha > 0.7f)
                Boss.LayerAlpha *= 0.95f;

            float speed = Npc.velocity.Length();
            Vector2 dir = anchor - Npc.Center;

            speed += (Helper.X3Ease(Helper.Clamp(timer / accTime, 0, 1)) * acc)
                + 0.01f;

            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }

            CoreOffset_SmoothTo(Npc.velocity, 0.15f);

            Npc.velocity = dir.SafeNormalize(Vector2.Zero) * speed;
            Npc.rotation = Npc.rotation.AngleLerp(dir.ToRotation(), 0.2f);

            if (dir.LengthSquared() >= speed * speed && timer < 240)
            {
                return false;
            }

            Npc.velocity = Vector2.Zero;
            FadeGravityMoveAnchorProj();
            Boss.LayerAlpha =1;
            Boss.CommonShellBreath(2);
            Boss.CoreOffset = Vector2.Zero;
            Boss.LockDistancePercent = Helper.Lerp(Boss.LockDistancePercent, 1, 0.15f);

            //TODO: 生成到达位置后的粒子

            return true;
        }

        /// <summary>
        /// 让引力移动的拖拽弹幕消逝
        /// </summary>
        public void FadeGravityMoveAnchorProj()
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

        #region 核心移动

        public void CoreOffset_SmoothTo(Vector2 targetPos,float f)
        {
            Boss.CoreOffset = Vector2.SmoothStep(Boss.CoreOffset, targetPos, f);
        }

        /// <summary>
        /// 使用hotC记录角度，hotD记录长度，跟随NPC速度
        /// </summary>
        public void CoreOffset_AngledToVel(float toVelF=0.2f,float coreF=0.15f)
        {
            CoreOffsetAngle = Npc.velocity.ToRotation();
            CoreOffsetRadius = Helper.Lerp(CoreOffsetRadius, Npc.velocity.Length(), 0.2f);
            CoreOffset_AngledTo(0.15f);

            Boss.CoreOffset = Vector2.SmoothStep(Boss.CoreOffset, CoreOffsetAngle.ToRotationVector2() * CoreOffsetRadius, coreF);
        }

        /// <summary>
        /// 使用hotC记录角度，hotD记录长度
        /// </summary>
        public void CoreOffset_AngledTo(float f)
        {
            Boss.CoreOffset = Vector2.SmoothStep(Boss.CoreOffset, CoreOffsetAngle.ToRotationVector2() * CoreOffsetRadius, f);
        }

        /// <summary>
        /// 核心归位，注意会设置hotCD
        /// </summary>
        /// <param name="f"></param>
        public void CoreOffset_BackToCenter(float f)
        {
            Boss.CoreOffset = Vector2.SmoothStep(Boss.CoreOffset, Vector2.Zero, f);
            CoreOffsetAngle = 0;
            CoreOffsetRadius = 0;
        }

        #endregion

        /// <summary>预警线粒子</summary>
        public void SpawnAimLine(float targetLength, int spawnTime, int contiunedTime)
        {
            if (VaultUtils.isServer)
                return;

            var p = PRTLoader.NewParticle<BeamShotParticle>(Npc.Center, Vector2.Zero, Coralite.ShadowPurple);

            p.bottomWidth = Npc.width / 3;
            p.targetLength = targetLength;
            p.followNpcIndex = Npc.whoAmI;
            p.followNpcRot = true;
            p.spawnTime = spawnTime;
            p.contiundTime = contiunedTime;
            p.Rotation = Npc.rotation;
        }

        public Vector2 DizzyStarCenter()
            => Npc.Center + new Vector2(0, -80);

        public void SpawnMucuryDust(Vector2 pos, float rot, float scale = 1)
        {
            Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<MucuryBurstDust>(), Vector2.Zero, Scale: scale);
            d.noGravity = true;
            d.rotation = rot;
        }
    }
}
