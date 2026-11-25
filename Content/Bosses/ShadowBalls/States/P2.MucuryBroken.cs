using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Content.Prefixes.FairyWeaponPrefixes;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.MucuryBroken, typeof(ShadowBallContext))]
    public class MucuryBroken : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.MucuryBroken;

        private enum Beat
        {
            Ready,
            Shoot,
            SlowDown,
            Over,
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    if (Timer > 10)
                        SwitchBeat(ctx, (int)Beat.Shoot);
                    break;
                case Beat.Shoot:
                    ctx.SetVelocityLetItGo();

                    float f = Timer / 30f;
                    ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 25, f);
                    ctx.CoreOffset_AngledTo_P2(0.3f);

                    if (Timer > 30)
                    {
                        if (ctx.Recorder2.GetProjectileOwner<MucuryBurst>(out Projectile p))
                        {
                            ctx.Npc.velocity = (p.rotation + MathHelper.Pi).ToRotationVector2() * p.ai[2] / 32;
                        }

                        SwitchBeat(ctx, (int)Beat.SlowDown);
                    }
                    break;
                case Beat.SlowDown:
                    {
                        ctx.SetVelocitySlowDown(0.93f);
                        ctx.SetRotationLetItGo();
                        ctx.Npc.rotation += MathF.Sign(ctx.Npc.velocity.X) * ctx.Npc.velocity.Length() / 60f;
                        ctx.CoreOffsetRadius = Helper.Lerp(25, 5, Timer/25f);
                        ctx.CoreOffset_AngledTo_P2(0.3f);

                        if (Timer > 25)
                        {
                            ctx.Recorder1--;
                            SwitchBeat(ctx, (int)(ctx.Recorder1 < 1 ? Beat.Over : Beat.Shoot));
                        }
                    }
                    break;
                case Beat.Over:
                    break;
                default:
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    if (Timer == 1)//几轮
                    {
                        ctx.Recorder1 = Main.rand.Next(5, 9);
                        ctx.NetUpdate();
                    }
                    break;
                case Beat.Shoot:
                    {
                        if (Timer == 1)//射击
                        {
                            float dir = ctx.Recorder1 * MathHelper.Pi / 3;
                            dir += Main.rand.NextFloat(-MathHelper.TwoPi / 6, MathHelper.TwoPi / 6);

                            float distance = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center);
                            float speed = 5 + Helper.Clamp(distance / (16 * 80), 0, 1) * 10+Main.rand.NextFloat(-5,5);

                            if (Vector2.Distance(ctx.Target.Center,ctx.Npc.Center)>16*30)//超出一定距离固定靠近
                            {
                                dir = (ctx.Target.Center - ctx.Npc.Center).ToRotation() + Main.rand.NextFloat(-0.5f, 0.5f);
                            }

                            Projectile p = ctx.Npc.NewProjectileDirectInAI_Server<MucuryBurst>(ctx.Npc.Center, (dir + MathHelper.Pi).ToRotationVector2(), Helper.GetProjDamage(80, 90, 100), 0, ai0: ctx.Npc.whoAmI, ai1: 120, ai2: 32 * speed);
                            ctx.Recorder2 = p.whoAmI;

                            ctx.Npc.SimpleStrikeNPC((int)(ctx.Npc.lifeMax * 0.003f), 0);
                        }
                    }
                    break;
                case Beat.SlowDown:
                    break;
                case Beat.Over:
                    if (Timer > 60)
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P2Test);

                    break;
                default:
                    break;
            }

            return null;
        }
    }
}
