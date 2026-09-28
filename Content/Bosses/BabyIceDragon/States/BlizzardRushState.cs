using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>暴风雪俯冲：爬升、蓄势、旋转吐息、吼叫后高速俯冲，撞墙或掠过目标后收势。</summary>
    [VaultState((int)BabyIceDragonStateId.blizzardRush, typeof(BabyIceDragonContext))]
    internal sealed class BlizzardRushState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.blizzardRush;

        private enum Beat
        {
            Climb,
            Windup,
            BreathDive,
            Settle,
            DashWindup,
            Dash,
            Brake,
        }

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Climb://向上飞
                    UpdateClimb(ctx);
                    break;
                case Beat.Windup://下冲前摇
                    UpdateWindup(ctx);
                    break;
                case Beat.BreathDive://下冲吐息
                    UpdateBreathDive(ctx);
                    break;
                case Beat.Settle://下冲前摇
                    UpdateSettle(ctx);
                    break;
                case Beat.DashWindup:
                    UpdateDashWindup(ctx);
                    break;
                case Beat.Dash:
                    UpdateDash(ctx);
                    break;
                default:
                    UpdateBrake(ctx);
                    break;
            }
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if ((Beat)BeatIndex == Beat.BreathDive)
            {
                const int duration = 40;
                if (Timer >= duration)
                {
                    ChangeBeat(ctx, (int)Beat.Settle);
                }
                else if (Timer > 6 && Timer < 4 * 8)
                {
                    if (Timer % 3 == 0)
                    {
                        FireBreath(ctx);
                    }
                    if (Timer % 7 == 0)
                    {
                        FireIcicle(ctx);
                    }
                }
            }
            else if ((Beat)BeatIndex == Beat.Dash)
            {
                const int timeout = 90;
                if (HitTile(ctx) && ctx.Npc.Center.Y > ctx.Target.Center.Y - 120f)
                {
                    SmashDownState.SpawnThorns(ctx,1);
                    SmashDownState.LandingShake(ctx);

                    EnterBrake(ctx);
                }
                else if (ctx.Npc.Center.Y > ctx.Target.Center.Y + 200f || Timer > timeout)
                {
                    EnterBrake(ctx);
                }
            }
            else if ((Beat)BeatIndex == Beat.Brake && Timer >= 30)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        private void UpdateClimb(BabyIceDragonContext ctx)
        {
            const float climbDistance = 350f;
            NPC npc = ctx.Npc;
            ctx.FaceTarget();
            ctx.DeclareFlyUp();
            npc.rotation = npc.rotation.AngleTowards(0f, 0.08f);
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            if (ctx.Target.Center.Y - npc.Center.Y < climbDistance)
            {
                return;
            }

            npc.direction = npc.spriteDirection = npc.Center.X > ctx.Target.Center.X ? -1 : 1;
            ChangeBeat(ctx, (int)Beat.Windup);
        }

        private void UpdateWindup(BabyIceDragonContext ctx)
        {
            const int windupFrames = 20;
            NPC npc = ctx.Npc;
            ctx.DeclareDirect();
            ctx.DeclareFlyingFrame(1, false);
            npc.velocity *= 0.95f;
            npc.rotation += npc.direction * 0.065f;
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            if (Timer >= windupFrames)
            {
                npc.velocity = (npc.rotation + ctx.FacingFlip).ToRotationVector2() * 10f;
                npc.velocity.X *= 2f;
                ChangeBeat(ctx, (int)Beat.BreathDive);
            }
        }

        private void UpdateBreathDive(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            const int endFrame = 40;
            npc.velocity.Y -= 0.27f;
            ctx.DeclareDirect();
            ctx.DeclareRotation(BabyIceDragonRotationMode.FaceVelocity);
            ctx.DrawShadows = true;
            if (Timer >= endFrame)
            {
                ChangeBeat(ctx, (int)Beat.Settle);
            }
        }

        private void UpdateSettle(BabyIceDragonContext ctx)
        {
            const float rotationStep = 0.08f;
            const int settleFrames = 30;
            NPC npc = ctx.Npc;
            npc.velocity *= 0.98f;
            npc.rotation = npc.rotation.AngleTowards(0f, rotationStep);
            ctx.DeclareDirect();
            ctx.DeclareRotation(BabyIceDragonRotationMode.TiltBySpeed);
            ctx.DeclareFlyingFrame(1, false);
            if (CueDue(1))
            {
                RoarCue(ctx);
            }
            if (Timer > 0)
                RoarParticles(ctx);

            if (Timer >= settleFrames)
            {
                ChangeBeat(ctx, (int)Beat.DashWindup);
            }
        }

        private void UpdateDashWindup(BabyIceDragonContext ctx)
        {
            const int windupFrames = 50;
            NPC npc = ctx.Npc;
            ctx.FaceTarget();
            
            if (npc.Center.Y > ctx.Target.Center.Y - 400f)
            {
                ctx.DeclareFlyUp();
            }
            else if (npc.Center.Y < ctx.Target.Center.Y - 600)
            {
                npc.velocity.Y += 0.2f;
                ctx.DeclareDirect();
                ctx.DeclareFlyingFrame(1, false);
            }
            else
            {
                npc.velocity.Y *= 0.96f;
                ctx.DeclareDirect();
                ctx.DeclareFlyingFrame(1, false);
            }

            if (MathF.Sign(npc.Center.X - ctx.Target.Center.X) > 8)
                npc.direction = npc.spriteDirection = npc.Center.X > ctx.Target.Center.X ? -1 : 1;

            if (Math.Abs(npc.Center.X - ctx.Target.Center.X) > 40f)
            {
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, Math.Sign(ctx.Target.Center.X - npc.Center.X) * 6f, 0.08f);
                //ctx.DeclareDirect();
            }
            else
            {
                npc.velocity.X *= 0.96f;
                //ctx.DeclareDirect();
            }

            if (Timer >= windupFrames)
            {
                Vector2 dir = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.UnitY);
                if (!Main.dedServ)
                {
                    WindCircle.Spawn(npc.Center, -dir * 2f, dir.ToRotation(), Coralite.IcicleCyan, 0.65f, 1f, new Vector2(1.4f, 1f));

                    SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, npc.Center);
                    PRTLoader.NewParticle(npc.Center, Vector2.Zero, CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.IcicleCyan, BabyIceDragonDirector.SmashCueSparkScale);
                }
                npc.velocity = dir * 0.5f;
                //npc.rotation = dir.ToRotation() + ctx.FacingFlip;
                ctx.DeclareDirect();
                ctx.DeclareRotation(BabyIceDragonRotationMode.TowardsVelocity,0.1f);
                ctx.DrawShadows = true;
                ChangeBeat(ctx, (int)Beat.Dash);
            }
        }

        private void UpdateDash(BabyIceDragonContext ctx)
        {
            ctx.DeclareKeep();
            ctx.DeclareRotation(BabyIceDragonRotationMode.TowardsVelocity,0.1f);
            ctx.DrawShadows = true;
            NPC npc = ctx.Npc;

            Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(npc.getRect()), DustID.Frost, -npc.velocity.SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(2, 5), Scale: Main.rand.NextFloat(1, 1.75f));
            d.noGravity = true;

            if (npc.velocity.Length() < 20)
            {
                npc.velocity *= 1.15f;
            }

            if (npc.velocity.Y < 10)
            {
                npc.velocity.Y *= 1.01f;
            }
        }

        private void UpdateBrake(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.velocity *= 0.9f;
            npc.rotation = 0f;
            ctx.DeclareDirect();
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            ctx.DeclareFlyingFrame(0, false);
        }

        private static void FireBreath(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 dir = (npc.rotation + ctx.FacingFlip).ToRotationVector2();
            ctx.SpawnHostile<IceBreath>(npc.GetSource_FromAI(), ctx.MouthCenter(), dir * Main.rand.NextFloat(6,12),
                BabyIceDragonDirector.BreathDamage(), BabyIceDragonDirector.BreathKnockback);
        }

        private static void FireIcicle(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 dir = (npc.rotation + ctx.FacingFlip).ToRotationVector2().RotateByRandom(-0.1f,0.1f);
            ctx.SpawnHostile<IcicleProj_Hostile>(npc.GetSource_FromAI(), ctx.MouthCenter(), dir * Main.rand.NextFloat(8,10),
                BabyIceDragonDirector.BreathDamage(), 0f);
        }

        private static bool HitTile(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 dir = (npc.rotation + ctx.FacingFlip).ToRotationVector2();
            Vector2 normal = dir.RotatedBy(MathHelper.PiOver2);
            Vector2 origin = ctx.MouthCenter()-normal*2;
            //for (int forward = 0; forward <= 1; forward++)
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (Framing.GetTileSafely(origin + dir * /*forward **/ 16f + normal * i * 16).HasSolidTopTile())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void EnterBrake(BabyIceDragonContext ctx)
        {
            ctx.Npc.velocity = new Vector2(0f, -3f);
            ctx.DeclareDirect();
            ChangeBeat(ctx, (int)Beat.Brake);
        }
    }
}
