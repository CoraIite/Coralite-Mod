using Coralite.Content.Bosses.BabyIceDragon.States;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon.Core
{
    /// <summary>
    /// 状态索引，写入 <c>npc.ai[0]</c> 同步。成员与数值沿用旧 <c>BabyIceDragon.AIStates</c>（线格式不变），追加 <see cref="hub"/>。
    /// </summary>
    public enum BabyIceDragonStateId : int
    {
        //动画
        onSpawnAnim = 0,
        roaringAnim = 1,
        onKillAnim = 2,

        //非攻击动作，也非动画：声明的间隙
        dizzy = 3,
        rest = 4,

        //有可能会有破绽的攻击动作
        dive = 5,
        accumulate = 6,

        //普通攻击动作
        iceBreath = 7,
        horizontalDash = 8,
        smashDown = 9,

        //二阶段追加攻击动作
        iceThornsTrap = 10,
        iceCloud = 11,
        doubleDash = 12,

        //大师模式专属，特殊攻击动作
        iceTornado = 13,
        iciclesFall = 14,

        /// <summary>连接段 + 唯一提交口（新增；<see cref="BabyIceDragonDirector.HubFrames"/> 为 0 时通常不驻留）</summary>
        hub = 15,
    }

    /// <summary>
    /// 冰龙宝宝状态基类。<br/>
    /// · <c>SharedUpdate</c>：两端同跑——写运动 / 朝向 / 帧图 / 无敌 / 重力 / 碰撞声明，推进确定性子拍，客户端粒子音效。<br/>
    /// · <see cref="AuthorityUpdate"/>：仅权威端——弹幕 / 召唤 / 掷骰，返回下一状态；本类把死亡请求、外部眩晕 / 休息请求与超时兜底垫在它前面。<br/>
    /// · 收招一律 <see cref="EndAttack"/>，出招一律经 <see cref="BabyIceDragonHubState.Commit"/>；招式体内不得出现 <c>ChangeState</c>。<br/>
    /// · 一次性演出拍用 <see cref="CueDue"/>（带 <see cref="CoraliteBossState{TContext}.CueCatchUpGrace"/> 宽限），权威端出手仍用精确 <c>Timer == N</c>。
    /// </summary>
    internal abstract class BabyIceDragonStateBase : CoraliteBossState<BabyIceDragonContext>
    {
        public abstract BabyIceDragonStateId StateIndex { get; }

        public override int StateId => (int)StateIndex;

        /// <summary>超时兜底帧数；演出态可覆盖为更大值。</summary>
        protected virtual int TimeoutFrames => BabyIceDragonDirector.StateTimeoutFrames;

        /// <summary>本拍内已补放过的最高一次性拍点（-1 无）；换拍或收养到新拍时清零。</summary>
        private int cueMark = -1;

        protected sealed override IVaultState<BabyIceDragonContext> ServerUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            // CheckDead 只登记请求，换态统一从这里经返回值走（客户端读 ai[0] 跟随）。
            if (ctx.KillRequested && StateIndex != BabyIceDragonStateId.onKillAnim)
            {
                return Create(BabyIceDragonStateId.onKillAnim);
            }

            // 外部请求（俯冲撞墙 / 冰球被击破 → 眩晕；冰球爆炸 → 休息）。旧代码从任何状态直接 ChangeState，这里保留“任何状态都响应”，死亡演出除外。
            if (StateIndex != BabyIceDragonStateId.onKillAnim)
            {
                if (ctx.PendingDizzyFrames > 0 && StateIndex != BabyIceDragonStateId.dizzy)
                {
                    return Create(BabyIceDragonStateId.dizzy);
                }

                if (ctx.PendingRestFrames > 0 && StateIndex != BabyIceDragonStateId.rest)
                {
                    return Create(BabyIceDragonStateId.rest);
                }
            }

            // 超时兜底：状态机永远不许死在一个状态里；收招不留残速。
            if (Counter++ > TimeoutFrames)
            {
                ctx.Npc.velocity *= 0.6f;
                return EndAttack(ctx);
            }

            return AuthorityUpdate(machine, ctx);
        }

        /// <summary>仅权威端：弹幕生成、召唤、掷骰；返回下一状态或 null。</summary>
        protected abstract IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx);

        public override void OnEnter(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            base.OnEnter(machine, ctx);
            cueMark = -1;
        }

        public override void ReadHot(BabyIceDragonContext ctx)
        {
            int oldBeat = BeatIndex;
            base.ReadHot(ctx);
            if (BeatIndex != oldBeat)
            {
                cueMark = -1;
            }
        }

        #region 公共小件

        /// <summary>按 id 建状态；未注册返回 null（调用方要兜底）。</summary>
        protected static IVaultState<BabyIceDragonContext> Create(BabyIceDragonStateId id)
            => VaultStateRegistry<BabyIceDragonContext>.Create((int)id);

        /// <summary>收招：经 hub 的唯一提交口选下一招（<see cref="BabyIceDragonDirector.HubFrames"/> = 0 时不多占一帧）。</summary>
        protected static IVaultState<BabyIceDragonContext> EndAttack(BabyIceDragonContext ctx)
            => BabyIceDragonHubState.EndAttack(ctx);

        /// <summary>收招进休息（旧 <c>HaveARest</c>）：登记请求并切到 rest，rest 的 OnEnter 消费帧数。</summary>
        protected static IVaultState<BabyIceDragonContext> EnterRest(BabyIceDragonContext ctx, int frames)
        {
            ctx.PendingRestFrames = frames;
            return Create(BabyIceDragonStateId.rest);
        }

        /// <summary>撞墙进眩晕（旧 <c>Dizzy</c>）。</summary>
        protected static IVaultState<BabyIceDragonContext> EnterDizzy(BabyIceDragonContext ctx, int frames)
        {
            ctx.PendingDizzyFrames = frames;
            return Create(BabyIceDragonStateId.dizzy);
        }

        /// <summary>换拍并清一次性拍点记录（拍号与 Timer 随热字段过线）。</summary>
        protected void ChangeBeat(BabyIceDragonContext ctx, int beat)
        {
            SwitchBeat(ctx, beat);
            cueMark = -1;
        }

        /// <summary>
        /// 一次性演出拍：Timer 首次到达 <paramref name="frame"/>（含收养后刚越过、不超过宽限帧数的情况）返回一次 true；
        /// 越过太多视为中途加入，静默跳过（C9）。只用于粒子 / 音效 / 震屏，权威端出手不用它。
        /// </summary>
        protected bool CueDue(int frame)
        {
            if (Timer < frame || cueMark >= frame)
            {
                return false;
            }

            cueMark = frame;
            return Timer - frame <= CueCatchUpGrace;
        }

        /// <summary>吼叫震屏（纯本地）。旧 BabyIceDragon.cs:448</summary>
        protected static void RoarShake(BabyIceDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            PunchCameraModifier modifier = new(ctx.Npc.Center, new Vector2(BabyIceDragonDirector.RoarShakeDir, BabyIceDragonDirector.RoarShakeDir),
                BabyIceDragonDirector.RoarShakeStrength, BabyIceDragonDirector.RoarShakeVibration, BabyIceDragonDirector.RoarShakeFrames,
                BabyIceDragonDirector.ShakeFalloffDistance, "BabyIceDragon");
            Main.instance.CameraModifiers.Add(modifier);
        }

        /// <summary>吼叫起手：张嘴帧 + 吼声 + 嘴前一道吼叫线 + 震屏（纯本地部分自带守卫）。旧 BabyIceDragon.cs:438-450</summary>
        protected static void RoarCue(BabyIceDragonContext ctx)
        {
            ctx.SetFrame(1, 1);
            if (Main.dedServ)
            {
                return;
            }

            SoundEngine.PlaySound(SoundID.Roar, ctx.Npc.Center);
            PRTLoader.NewParticle(ctx.MouthCenter(), Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, BabyIceDragonDirector.RoarParticleScale);
            RoarShake(ctx);
        }

        /// <summary>
        /// 吼叫持续粒子：每 10 帧波纹、每 20 帧线条（纯本地）。旧 BabyIceDragon.cs:455-462<br/>
        /// <paramref name="tick"/> 传负值表示用本状态的 <see cref="VaultState{TContext}.Timer"/>；出场动画按旧口径从 0 起算，把 <c>Timer - 1</c> 传进来保持相位一致。
        /// </summary>
        protected void RoarParticles(BabyIceDragonContext ctx, int tick = -1)
        {
            if (Main.dedServ)
            {
                return;
            }

            int t = tick < 0 ? Timer : tick;
            Vector2 mouth = ctx.MouthCenter();
            if (t % BabyIceDragonDirector.RoarWaveInterval == 0)
            {
                PRTLoader.NewParticle(mouth, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Color.White, BabyIceDragonDirector.RoarParticleScale);
            }

            if (t % BabyIceDragonDirector.RoarLineInterval == 0)
            {
                PRTLoader.NewParticle(mouth, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, BabyIceDragonDirector.RoarParticleScale);
            }
        }

        /// <summary>蓄力提示：冰魔法音效 + 4 颗冰星聚向嘴前 + 两圈反向光环（纯本地部分自带守卫）。旧 BabyIceDragon.cs:621-632</summary>
        protected static void ChargeCue(BabyIceDragonContext ctx, bool halos = true)
        {
            if (Main.dedServ)
            {
                return;
            }

            SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, ctx.Npc.Center);
            NPC npc = ctx.Npc;
            for (int i = 0; i < BabyIceDragonDirector.ChargeStarCount; i++)
            {
                IceStarLight.Spawn(npc.Center + Main.rand.NextVector2CircularEdge(BabyIceDragonDirector.ChargeStarRadius, BabyIceDragonDirector.ChargeStarRadius),
                    Main.rand.NextVector2CircularEdge(BabyIceDragonDirector.ChargeStarSpeed, BabyIceDragonDirector.ChargeStarSpeed), 1f,
                    () => npc.Center + ((npc.rotation + (npc.direction > 0 ? 0f : 3.141f)).ToRotationVector2() * BabyIceDragonDirector.ChargeStarMouthOffset),
                    BabyIceDragonDirector.ChargeStarSpeedLimit);
            }

            if (halos)
            {
                Vector2 mouth = ctx.MouthCenter();
                PRTLoader.NewParticle(mouth, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: BabyIceDragonDirector.ChargeHaloScaleSmall);
                PRTLoader.NewParticle(mouth, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: BabyIceDragonDirector.ChargeHaloScaleBig);
            }
        }

        /// <summary>起手闪光（纯本地）。旧 AI.HorizontalDash.cs:52</summary>
        protected static void SparkCue(BabyIceDragonContext ctx, float scale)
        {
            if (Main.dedServ)
            {
                return;
            }

            PRTLoader.NewParticle(ctx.Npc.Center, Vector2.Zero, CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.IcicleCyan, scale);
        }

        /// <summary>吼叫招（冰刺陷阱 / 冰雹）就位段：距离超过 440 就追目标上方 200 px。旧 AI.IceThronsTrap.cs:20-35</summary>
        protected static bool RoarAttackApproach(BabyIceDragonContext ctx)
        {
            if (Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) <= BabyIceDragonDirector.RoarAttackApproachDistance)
            {
                return false;
            }

            ctx.FaceTarget();
            ctx.DeclareHoverY(BabyIceDragonDirector.RoarAttackHoverY, BabyIceDragonDirector.RoarAttackHoverY, BabyIceDragonDirector.RoarAttackDeadZoneY,
                BabyIceDragonDirector.RoarAttackSpeedY, BabyIceDragonDirector.RoarAttackAccelY, BabyIceDragonDirector.RoarAttackTurnY, BabyIceDragonDirector.RoarAttackDamp);
            ctx.DeclareApproachX(BabyIceDragonDirector.RoarAttackDeadZoneX, BabyIceDragonDirector.RoarAttackSpeedX, BabyIceDragonDirector.RoarAttackAccelX,
                BabyIceDragonDirector.RoarAttackTurnX, BabyIceDragonDirector.RoarAttackDamp, BabyIceDragonDirector.RoarAttackDamp);
            ctx.DeclareFlyingFrame();
            return true;
        }

        /// <summary>
        /// 吼叫招（冰刺陷阱 / 冰雹）吼叫段的双端部分：全程 0.97 衰减、朝向回正，30 帧合翅停住，50 帧吼叫，90 帧前吼叫粒子，之后飞行帧。
        /// 旧 AI.IceThronsTrap.cs:50-108；60 帧的生成由各自的 AuthorityUpdate 负责。
        /// </summary>
        protected void RoarAttackShared(BabyIceDragonContext ctx)
        {
            ctx.DeclareDamp(BabyIceDragonDirector.RoarAttackDamp2);
            ctx.DeclareRotation(BabyIceDragonRotationMode.TowardsZero, BabyIceDragonDirector.RoarAttackRotationStep);

            if (Timer == BabyIceDragonDirector.RoarAttackStopFrame)
            {
                ctx.SetFrame(0, 3);
                ctx.Npc.velocity = Vector2.Zero;
                ctx.DeclareDirect();
            }

            if (Timer < BabyIceDragonDirector.RoarAttackRoarFrame)
            {
                return;
            }

            if (CueDue(BabyIceDragonDirector.RoarAttackRoarFrame))
            {
                RoarCue(ctx);
            }

            if (Timer < BabyIceDragonDirector.RoarAttackParticleEnd)
            {
                RoarParticles(ctx);
                return;
            }

            ctx.DeclareFlyingFrame();
        }

        #endregion
    }
}
