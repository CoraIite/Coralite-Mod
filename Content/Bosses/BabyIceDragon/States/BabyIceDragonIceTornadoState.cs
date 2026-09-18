using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 冰晶龙卷风（大师专属）：冲到目标身边 → 原地画一个整圆再后撤（60 帧的长预告，看得见"要转起来了"）→ 化作龙卷追着玩家绕 200 帧 → 刹车解除无敌 → 进后摇。<br/>
    /// 公平阀：龙卷期间 boss 无敌但限速 9.5 px/f 且靠近才加速，玩家可以持续风筝；伤害来自每 8 帧一枚的短命透明弹幕，离开身位就不吃。旧 AI.IceTornado.cs
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.iceTornado, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonIceTornadoState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.iceTornado;

        private enum Beat
        {
            /// <summary>冲到 600 px 内。</summary>
            Approach,
            /// <summary>画圆 + 后撤的起手预告。</summary>
            Prepare,
            /// <summary>龙卷追击。</summary>
            Spin,
            /// <summary>刹车、解除无敌。</summary>
            Brake,
        }

        /// <summary>刹车段帧数：旧代码 200~209 共 10 帧。</summary>
        private static int BrakeFrames => BabyIceDragonDirector.TornadoBrakeEnd - BabyIceDragonDirector.TornadoSpinAttackFrames;

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Prepare:
                    UpdatePrepare(ctx);
                    break;
                case Beat.Spin:
                    UpdateSpin(ctx);
                    break;
                case Beat.Brake:
                    UpdateBrake(ctx);
                    break;
                default:
                    UpdateApproach(ctx);
                    break;
            }
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Spin:
                    if (Timer % BabyIceDragonDirector.TornadoProjInterval == 0)
                    {
                        FireTornadoProj(ctx);
                    }

                    break;
                case Beat.Brake:
                    if (Timer > BrakeFrames)
                    {
                        return EnterRest(ctx, BabyIceDragonDirector.RestFramesAfterBreak);
                    }

                    break;
                case Beat.Prepare:
                    break;
                default:
                    if (Timer > BabyIceDragonDirector.TornadoApproachTimeout)
                    {
                        return EndAttack(ctx);
                    }

                    break;
            }

            return null;
        }

        /// <summary>就位：距离超过 600 就追到目标上方 200 px；到位就闪光并以 8 px/f 扑过去。旧 AI.IceTornado.cs:18-55</summary>
        private void UpdateApproach(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            if (Vector2.Distance(npc.Center, ctx.Target.Center) > BabyIceDragonDirector.TornadoApproachDistance)
            {
                ctx.FaceTarget();
                ctx.DeclareFlyingFrame();
                ctx.DeclareHoverY(BabyIceDragonDirector.TornadoHoverY, BabyIceDragonDirector.TornadoHoverY, BabyIceDragonDirector.TornadoDeadZoneY,
                    BabyIceDragonDirector.TornadoApproachSpeedY, BabyIceDragonDirector.TornadoApproachAccelY, BabyIceDragonDirector.TornadoApproachTurnY,
                    BabyIceDragonDirector.TornadoApproachDamp);
                ctx.DeclareApproachX(BabyIceDragonDirector.TornadoApproachDeadZoneX, BabyIceDragonDirector.TornadoApproachSpeedX,
                    BabyIceDragonDirector.TornadoApproachAccelX, BabyIceDragonDirector.TornadoApproachTurnX,
                    BabyIceDragonDirector.TornadoApproachDamp, BabyIceDragonDirector.TornadoApproachIdleDampX);
                return;
            }

            ctx.FaceTarget();
            SparkCue(ctx, BabyIceDragonDirector.TornadoCueSparkScale);
            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, npc.Center);
            }

            npc.velocity = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * BabyIceDragonDirector.TornadoApproachLaunchSpeed;
            npc.rotation = npc.velocity.ToRotation() + ctx.FacingFlip;
            ctx.DeclareDirect();
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            ChangeBeat(ctx, (int)Beat.Prepare);
        }

        /// <summary>起手预告：30 帧内匀速画一整圈，30~60 帧以 2 px/f 后撤并把朝向摆正。旧 AI.IceTornado.cs:57-88</summary>
        private void UpdatePrepare(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareFlyingFrame(0, false);
            ctx.DeclareDirect();

            if (Timer < BabyIceDragonDirector.TornadoSpinInitFrames)
            {
                npc.velocity = Vector2.UnitY * BabyIceDragonDirector.TornadoSpinSpeed;
            }

            if (Timer < BabyIceDragonDirector.TornadoSpinFrames)
            {
                ctx.FaceTarget();
                npc.velocity = npc.velocity.RotatedBy(BabyIceDragonDirector.TornadoSpinStepAngle);
                npc.rotation = npc.velocity.ToRotation() + ctx.FacingFlip;
                ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
                return;
            }

            if (Timer < BabyIceDragonDirector.TornadoBackFrames)
            {
                ctx.FaceTarget();
                npc.velocity = -(ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * BabyIceDragonDirector.TornadoBackSpeed;
                ctx.DeclareRotation(BabyIceDragonRotationMode.TowardsZero, BabyIceDragonDirector.TornadoRotationStep);
                return;
            }

            WindSound(ctx);
            ctx.FaceTarget();
            npc.velocity = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.UnitX).RotatedBy(BabyIceDragonDirector.TornadoLaunchAngle)
                * BabyIceDragonDirector.TornadoLaunchSpeed;
            ctx.Invulnerable = true;
            ChangeBeat(ctx, (int)Beat.Spin);
        }

        /// <summary>龙卷：持续朝目标加速（贴脸更凶）并限速，全程无敌 + 风声 + 霜尘 + 龙卷粒子。旧 AI.IceTornado.cs:92-137</summary>
        private void UpdateSpin(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;

            if (Timer >= BabyIceDragonDirector.TornadoSpinAttackFrames)
            {
                //旧代码这一帧就已经在跑刹车块了，换拍后补上本帧的刹车声明，别让它落回 Hold 的默认衰减
                ChangeBeat(ctx, (int)Beat.Brake);
                UpdateBrake(ctx);
                return;
            }

            ctx.Invulnerable = true;
            ctx.DeclareFlyingFrame(1, false);

            float distance = Vector2.Distance(ctx.Target.Center, npc.Center);
            float accel = distance < BabyIceDragonDirector.TornadoCloseDistance
                ? BabyIceDragonDirector.TornadoCloseAccel
                : BabyIceDragonDirector.TornadoFarAccel;
            npc.velocity += Vector2.Normalize(ctx.Target.Center - npc.Center) * accel;
            if (npc.velocity.Length() > BabyIceDragonDirector.TornadoMaxSpeed)
            {
                npc.velocity = Vector2.Normalize(npc.velocity) * BabyIceDragonDirector.TornadoMaxSpeed;
            }

            ctx.DeclareDirect();
            ctx.DeclareRotation(BabyIceDragonRotationMode.FaceVelocity);

            if (Main.dedServ)
            {
                return;
            }

            if (Timer % BabyIceDragonDirector.TornadoWindInterval == 0)
            {
                WindSound(ctx);
            }

            if (Timer % BabyIceDragonDirector.TornadoDustInterval == 0)
            {
                Dust dust = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(BabyIceDragonDirector.TornadoDustSpread, BabyIceDragonDirector.TornadoDustSpread),
                    DustID.FrostStaff, -npc.velocity * BabyIceDragonDirector.TornadoDustBack,
                    Scale: Main.rand.NextFloat(BabyIceDragonDirector.TornadoDustScaleMin, BabyIceDragonDirector.TornadoDustScaleMax));
                dust.noGravity = true;
            }

            Color tornadoColor = BabyIceDragonDirector.TornadoColors[Main.rand.Next(BabyIceDragonDirector.TornadoColors.Length)];
            Tornado.Spawn(npc.Center + (npc.velocity * BabyIceDragonDirector.TornadoParticleAhead),
                npc.velocity * BabyIceDragonDirector.TornadoParticleSpeedFactor, tornadoColor, BabyIceDragonDirector.TornadoParticleFadeIn,
                npc.velocity.ToRotation(), Main.rand.NextFloat(BabyIceDragonDirector.TornadoParticleScaleMin, BabyIceDragonDirector.TornadoParticleScaleMax));
        }

        /// <summary>
        /// 刹车：解除无敌、朝向摆正。旧代码先被飞行倾斜改写朝向、再向 0 回正，所以等效是「从倾斜值出发回正」，这里直接写等效值。旧 AI.IceTornado.cs:139-146
        /// </summary>
        private void UpdateBrake(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareFlyingFrame(0, false);
            ctx.DeclareDamp(BabyIceDragonDirector.TornadoBrakeDamp);

            float tilt = npc.direction * npc.velocity.Y * BabyIceDragonDirector.TiltPerSpeedY;
            npc.rotation = tilt.AngleTowards(0f, BabyIceDragonDirector.TornadoRotationStep);
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
        }

        /// <summary>每 8 帧一枚短命透明弹幕，跟在龙卷前方。旧 AI.IceTornado.cs:110-116</summary>
        private static void FireTornadoProj(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            int index = ctx.SpawnHostile<IceTornado>(npc.GetSource_FromAI(),
                npc.Center + (npc.velocity * BabyIceDragonDirector.TornadoProjAhead),
                npc.velocity * BabyIceDragonDirector.TornadoProjSpeedFactor,
                BabyIceDragonDirector.TornadoDamage(), BabyIceDragonDirector.TornadoProjKnockback);

            if (index > -1 && index < Main.maxProjectiles)
            {
                Main.projectile[index].timeLeft = BabyIceDragonDirector.TornadoProjLife;
                Main.projectile[index].netUpdate = true;
            }

            ctx.MarkDecision();
        }

        /// <summary>冰风声（纯本地）。旧 AI.IceTornado.cs:81</summary>
        private static void WindSound(BabyIceDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            Helper.PlayPitched("Icicle/Wind" + Main.rand.Next(1, 3).ToString(), BabyIceDragonDirector.WindSoundVolume, 0f, ctx.Npc.Center);
        }
    }
}
