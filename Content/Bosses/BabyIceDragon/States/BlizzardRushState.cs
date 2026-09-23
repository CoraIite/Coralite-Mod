using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;

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
                case Beat.Climb:
                    UpdateClimb(ctx);
                    break;
                case Beat.Windup:
                    UpdateWindup(ctx);
                    break;
                case Beat.BreathDive:
                    UpdateBreathDive(ctx);
                    break;
                case Beat.Settle:
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
                const int duration = 60;
                if (Timer >= duration)
                {
                    ChangeBeat(ctx, (int)Beat.Settle);
                }
                else
                {
                    if (Timer % 8 == 0)
                    {
                        FireBreath(ctx);
                    }
                    if (Timer % 17 == 0)
                    {
                        FireIcicle(ctx);
                    }
                }
            }
            else if ((Beat)BeatIndex == Beat.Dash)
            {
                const int timeout = 90;
                if (HitWall(ctx) && ctx.Npc.Center.Y < ctx.Target.Center.Y - 60f)
                {
                    FireWallIcicles(ctx);
                    EnterBrake(ctx);
                }
                else if (ctx.Npc.Center.Y < ctx.Target.Center.Y + 200f || Timer > timeout)
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
            const float climbDistance = 400f;
            NPC npc = ctx.Npc;
            ctx.FaceTarget();
            ctx.DeclareFlyUp();
            npc.rotation = npc.rotation.AngleTowards(0f, 0.08f);
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            if (Math.Abs(npc.Center.Y - ctx.Target.Center.Y) > climbDistance)
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
            npc.velocity *= 0.85f;
            npc.rotation += 0.08f;
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            if (Timer >= windupFrames)
            {
                npc.velocity = (npc.rotation + ctx.FacingFlip).ToRotationVector2() * 10f;
                ChangeBeat(ctx, (int)Beat.BreathDive);
            }
        }

        private void UpdateBreathDive(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            const int endFrame = 60;
            npc.velocity.Y -= 0.3f;
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
            const float damp = 0.9f;
            const float rotationStep = 0.08f;
            const int settleFrames = 25;
            NPC npc = ctx.Npc;
            npc.velocity *= damp;
            npc.rotation = npc.rotation.AngleTowards(0f, rotationStep);
            ctx.DeclareDirect();
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            ctx.DeclareFlyingFrame(0, false);
            if (CueDue(1))
            {
                RoarCue(ctx);
            }
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
            ctx.DeclareFlyingFrame(1, false);
            if (npc.Center.Y > ctx.Target.Center.Y - 400f)
            {
                ctx.DeclareFlyUp();
            }
            else
            {
                npc.velocity.Y *= 0.9f;
                ctx.DeclareDirect();
            }

            if (Math.Abs(npc.Center.X - ctx.Target.Center.X) > 400f)
            {
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, Math.Sign(ctx.Target.Center.X - npc.Center.X) * 8f, 0.08f);
                ctx.DeclareDirect();
            }
            else
            {
                npc.velocity.X *= 0.9f;
                ctx.DeclareDirect();
            }

            if (Timer >= windupFrames)
            {
                Vector2 dir = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.UnitY);
                if (!Main.dedServ)
                {
                    WindCircle.Spawn(npc.Center, -dir * 2f, dir.ToRotation(), Coralite.IcicleCyan, 0.65f, 1f, new Vector2(1.4f, 1f));
                    SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, npc.Center);
                }
                npc.velocity = dir * 20f;
                npc.rotation = dir.ToRotation() + ctx.FacingFlip;
                ctx.DeclareDirect();
                ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
                ctx.DrawShadows = true;
                ChangeBeat(ctx, (int)Beat.Dash);
            }
        }

        private void UpdateDash(BabyIceDragonContext ctx)
        {
            ctx.DeclareKeep();
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            ctx.DrawShadows = true;
        }

        private void UpdateBrake(BabyIceDragonContext ctx)
        {
            const float damp = 0.9f;
            NPC npc = ctx.Npc;
            npc.velocity.X = 0f;
            npc.velocity.Y *= damp;
            npc.rotation = 0f;
            ctx.DeclareDirect();
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            ctx.DeclareFlyingFrame(0, false);
        }

        private static void FireBreath(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 dir = (npc.rotation + ctx.FacingFlip).ToRotationVector2();
            ctx.SpawnHostile<IceBreath>(npc.GetSource_FromAI(), ctx.MouthCenter(), dir * 10f,
                BabyIceDragonDirector.BreathDamage(), BabyIceDragonDirector.BreathKnockback);
        }

        private static void FireIcicle(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 dir = (npc.rotation + ctx.FacingFlip).ToRotationVector2();
            ctx.SpawnHostile<IcicleProj_Hostile>(npc.GetSource_FromAI(), ctx.MouthCenter(), dir * 8f,
                BabyIceDragonDirector.BreathDamage(), 0f);
        }

        private static bool HitWall(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 dir = (npc.rotation + ctx.FacingFlip).ToRotationVector2();
            Vector2 origin = ctx.MouthCenter();
            for (int forward = 0; forward <= 1; forward++)
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (Framing.GetTileSafely(origin + dir * ((i + forward) * 16f)).HasReallySolidTile())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void FireWallIcicles(BabyIceDragonContext ctx)
        {
            const int spikeCount = 3;
            const float spread = 0.06f;
            const float speed = 8f;
            NPC npc = ctx.Npc;
            Vector2 dir = (npc.rotation + ctx.FacingFlip).ToRotationVector2();
            Vector2 origin = ctx.MouthCenter();
            int damage = BabyIceDragonDirector.BreathDamage();
            for (int i = 0; i < spikeCount; i++)
            {
                float offset = (i - 1) * spread;
                ctx.SpawnHostile<IcicleProj_Hostile>(npc.GetSource_FromAI(), origin, dir.RotatedBy(offset) * speed, damage, 0f);
            }
        }

        private void EnterBrake(BabyIceDragonContext ctx)
        {
            ctx.Npc.velocity = new Vector2(0f, -3f);
            ctx.DeclareDirect();
            ChangeBeat(ctx, (int)Beat.Brake);
        }
    }
}
