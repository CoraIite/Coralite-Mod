using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 龙车变种（二阶段）：前半段和龙车一样横冲（越过判定收紧到 180 px），刹停后按目标方位二选一接后手——<br/>
    /// 目标落在水平 ±30° 内就再来一次短程锁向冲撞，否则改成强化吐息（吐息更密并每 15 帧掺一枚冰锥）。<br/>
    /// 接哪一手只取决于两端都能算的角度，所以换拍两端各自做；锁向角与冲程随自用热槽 A / B 过线，客户端不靠自己重算（C3）。旧 AI.DoubleDash.cs
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.doubleDash, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonDoubleDashState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.doubleDash;

        private enum Beat
        {
            /// <summary>对齐水平线。</summary>
            Approach,
            /// <summary>第一段横冲。</summary>
            Dash,
            /// <summary>硬刹并选后手。</summary>
            Brake,
            /// <summary>后手甲：强化吐息。</summary>
            Breath,
            /// <summary>后手乙：短程锁向冲撞。</summary>
            Charge,
        }

        /// <summary>锁定的冲撞方向（弧度），随热槽 A 过线。</summary>
        private float chargeAngle;
        /// <summary>锁定的冲程（px），随热槽 B 过线。</summary>
        private float chargeLength;

        /// <summary>刹车段帧数：旧代码 76~85 共 10 帧。</summary>
        private static int BrakeFrames => BabyIceDragonDirector.DashEndFrame - BabyIceDragonDirector.DashBrakeStart;

        public override void WriteHot(BabyIceDragonContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = chargeAngle;
            ctx.Hot[CoraliteBossHotSlots.B] = chargeLength;
        }

        public override void ReadHot(BabyIceDragonContext ctx)
        {
            base.ReadHot(ctx);
            chargeAngle = ctx.Hot[CoraliteBossHotSlots.A];
            chargeLength = ctx.Hot[CoraliteBossHotSlots.B];
        }

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Dash:
                    BabyIceDragonHorizontalDashState.UpdateDashBody(ctx, Timer, false);
                    if (BabyIceDragonHorizontalDashState.DashShouldBrake(ctx, Timer, BabyIceDragonDirector.DoubleDashPassDistance))
                    {
                        ChangeBeat(ctx, (int)Beat.Brake);
                    }

                    break;
                case Beat.Brake:
                    UpdateBrake(ctx);
                    break;
                case Beat.Breath:
                    UpdateBreath(ctx);
                    break;
                case Beat.Charge:
                    UpdateCharge(ctx);
                    break;
                default:
                    if (BabyIceDragonHorizontalDashState.DashApproach(ctx))
                    {
                        ChangeBeat(ctx, (int)Beat.Dash);
                    }

                    break;
            }
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Breath:
                    if (Timer >= BabyIceDragonDirector.DoubleBreathFireEnd)
                    {
                        return EnterRest(ctx, BabyIceDragonDirector.RestFramesAfterBreath());
                    }

                    if (Timer >= BabyIceDragonDirector.BreathFireStart)
                    {
                        Fire(ctx);
                    }

                    break;
                case Beat.Charge:
                    if (Timer > BabyIceDragonDirector.DoubleDashEndFrame)
                    {
                        return EndAttack(ctx);
                    }

                    break;
                case Beat.Dash:
                case Beat.Brake:
                    break;
                default:
                    if (Timer > BabyIceDragonDirector.DashApproachTimeout)
                    {
                        return EndAttack(ctx);
                    }

                    break;
            }

            return null;
        }

        /// <summary>刹车并在 10 帧后按目标方位选后手。旧 AI.DoubleDash.cs:87-105</summary>
        private void UpdateBrake(BabyIceDragonContext ctx)
        {
            ctx.DeclareDamp(BabyIceDragonDirector.DashBrakeDamp);
            ctx.DeclareFlyingFrame(1);

            if (Timer <= BrakeFrames)
            {
                return;
            }

            NPC npc = ctx.Npc;
            npc.velocity = Vector2.Zero;
            ctx.DeclareDirect();
            ctx.FaceTarget();

            float angle = (ctx.Target.Center - npc.Center).ToRotation();
            float abs = Math.Abs(angle);
            bool charge = abs < BabyIceDragonDirector.DoubleDashChargeAngle || abs > MathHelper.Pi - BabyIceDragonDirector.DoubleDashChargeAngle;
            ChangeBeat(ctx, (int)(charge ? Beat.Charge : Beat.Breath));
        }

        /// <summary>强化吐息：与冰吐息同一套跟随与蓄力，只是吐得更密并掺冰锥。旧 AI.DoubleDash.cs:108-178</summary>
        private void UpdateBreath(BabyIceDragonContext ctx)
        {
            ctx.DeclareFlyingFrame(1);

            if (Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) > BabyIceDragonDirector.BreathKeepDistance)
            {
                ctx.FaceTarget();
                ctx.DeclareHoverY(BabyIceDragonDirector.BreathHoverY, BabyIceDragonDirector.BreathHoverY, BabyIceDragonDirector.BreathDeadZoneY,
                    BabyIceDragonDirector.BreathFollowSpeed, BabyIceDragonDirector.BreathFollowAccel, BabyIceDragonDirector.BreathFollowTurn,
                    BabyIceDragonDirector.BreathApproachDamp);
                ctx.DeclareApproachX(BabyIceDragonDirector.BreathFollowDeadZoneX, BabyIceDragonDirector.BreathFollowSpeed,
                    BabyIceDragonDirector.BreathFollowAccel, BabyIceDragonDirector.BreathFollowTurn,
                    BabyIceDragonDirector.BreathApproachDamp, BabyIceDragonDirector.BreathApproachIdleDampX);
            }
            else
            {
                ctx.DeclareKeep();
            }

            if (CueDue(BabyIceDragonDirector.BreathChargeCueFrame))
            {
                ChargeCue(ctx);
            }

            if (Timer >= BabyIceDragonDirector.BreathFireStart && Timer < BabyIceDragonDirector.DoubleBreathFireEnd
                && Timer % BabyIceDragonDirector.DoubleBreathFireInterval == 0 && !Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, ctx.Npc.Center);
            }
        }

        /// <summary>每 10 帧两发吐息、每 15 帧一枚冰锥，都瞄向目标 ±30 px 的散布点。旧 AI.DoubleDash.cs:149-172</summary>
        private void Fire(BabyIceDragonContext ctx)
        {
            bool breath = Timer % BabyIceDragonDirector.DoubleBreathFireInterval == 0;
            bool icicle = Timer % BabyIceDragonDirector.DoubleBreathIcicleInterval == 0;
            if (!breath && !icicle)
            {
                return;
            }

            NPC npc = ctx.Npc;
            Vector2 mouth = ctx.MouthCenter();
            Vector2 targetDir = (ctx.Target.Center + Main.rand.NextVector2CircularEdge(BabyIceDragonDirector.BreathAimSpread, BabyIceDragonDirector.BreathAimSpread) - npc.Center)
                .SafeNormalize(Vector2.Zero);
            int damage = BabyIceDragonDirector.BreathDamage();

            if (breath)
            {
                for (int i = -1; i < 1; i++)
                {
                    ctx.SpawnHostile<IceBreath>(npc.GetSource_FromAI(), mouth,
                        targetDir.RotatedBy(i * BabyIceDragonDirector.DoubleBreathSpreadStep) * BabyIceDragonDirector.BreathSpeed,
                        damage, BabyIceDragonDirector.BreathKnockback);
                }
            }

            if (icicle)
            {
                ctx.SpawnHostile<IcicleProj_Hostile>(npc.GetSource_FromAI(), mouth,
                    targetDir.RotatedBy(Main.rand.NextFloat(-BabyIceDragonDirector.DoubleBreathIcicleSpread, BabyIceDragonDirector.DoubleBreathIcicleSpread))
                        * BabyIceDragonDirector.DoubleBreathIcicleSpeed,
                    damage, BabyIceDragonDirector.DoubleBreathIcicleKnockback);
            }

            ctx.MarkDecision();
        }

        /// <summary>短程锁向冲撞：首帧锁角度与冲程（冲程 = 距离 + 100，钳 120~400）→ 15 帧起冲 → 35 帧后刹车 → 55 帧收招。旧 AI.DoubleDash.cs:181-239</summary>
        private void UpdateCharge(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareKeep();

            if (Timer < BabyIceDragonDirector.DoubleDashLockFrames)
            {
                ctx.FaceTarget();
                chargeAngle = (ctx.Target.Center - npc.Center).ToRotation();
                chargeLength = Math.Clamp(npc.Distance(ctx.Target.Center) + BabyIceDragonDirector.DoubleDashLengthBonus,
                    BabyIceDragonDirector.DoubleDashLengthMin, BabyIceDragonDirector.DoubleDashLengthMax);
                ctx.MarkDecision();
                ChargeCue(ctx);
            }

            if (Timer < BabyIceDragonDirector.DoubleDashLaunchFrame)
            {
                ctx.DeclareFlyingFrame();
                return;
            }

            if (Timer == BabyIceDragonDirector.DoubleDashLaunchFrame)
            {
                npc.velocity = chargeAngle.ToRotationVector2() * chargeLength / BabyIceDragonDirector.DoubleDashLengthToSpeed;
                ctx.DeclareDirect();
                ctx.DrawShadows = true;
                return;
            }

            if (Timer < BabyIceDragonDirector.DoubleDashFlightEnd)
            {
                ctx.DrawShadows = true;
                ctx.DeclareFlyingFrame(1);
                SpeedLines(ctx);
                return;
            }

            if (Timer > BabyIceDragonDirector.DoubleDashFlightEnd)
            {
                ctx.DeclareFlyingFrame();
                ctx.DeclareDamp(BabyIceDragonDirector.DoubleDashBrakeDamp);
            }
        }

        /// <summary>冲撞速度线（纯本地）。旧 AI.DoubleDash.cs:218-220</summary>
        private void SpeedLines(BabyIceDragonContext ctx)
        {
            if (Main.dedServ || Timer % BabyIceDragonDirector.SpeedLineInterval != 0)
            {
                return;
            }

            NPC npc = ctx.Npc;
            PRTLoader.NewParticle(npc.Center + Main.rand.NextVector2Circular(BabyIceDragonDirector.SpeedLineSpread, BabyIceDragonDirector.SpeedLineSpread),
                -npc.velocity * Main.rand.NextFloat(BabyIceDragonDirector.SpeedLineSpeedMin, BabyIceDragonDirector.SpeedLineSpeedMax),
                CoraliteContent.ParticleType<SpeedLine>(), Coralite.IcicleCyan,
                Main.rand.NextFloat(BabyIceDragonDirector.SpeedLineScaleMin, BabyIceDragonDirector.SpeedLineScaleMax));
        }
    }
}
