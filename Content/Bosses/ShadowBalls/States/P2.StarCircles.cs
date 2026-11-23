using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.StarCircles, typeof(ShadowBallContext))]
    public class StarCircles : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.StarCircles;

        private enum Beat
        {
            Ready,
            SpawnCrown,
            SpawnStarCircle,
            Over,
        }

        public static int WarnTime
            => Helper.ScaleValueForDiffMode(40, 40, 50, 60);

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    {
                        ctx.SetVelocitySlowDown(0.95f);

                        if (Timer > 5)
                        {
                            if (ctx.Recorder2.GetProjectileOwner<MucuryBurst>(out Projectile burst))
                            {
                                Vector2 dir = (burst.rotation + MathHelper.Pi).ToRotationVector2();
                                ctx.Npc.velocity = dir * ctx.Recorder3_P2Only;
                            }
                            SwitchBeat(ctx, (int)Beat.SpawnCrown);
                        }
                    }
                    break;
                case Beat.SpawnCrown:
                    ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 5, 0.1f);
                    ctx.Npc.rotation += Timer / 15f * 0.05f;
                    ctx.CoreOffset_AngledTo_P2(0.25f);

                    if (Timer > 15)
                    {
                        SwitchBeat(ctx, (int)Beat.SpawnStarCircle);
                    }
                    break;
                case Beat.SpawnStarCircle:
                    {
                        ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 2.5f+2.5f*MathF.Cos(Timer*0.08f), 0.2f);
                        ctx.Npc.rotation +=  0.05f;
                        ctx.CoreOffset_AngledTo_P2(0.25f);

                        if (Timer > 20 * 2)
                        {
                            SwitchBeat(ctx, (int)Beat.Over);
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
                    break;
                case Beat.SpawnCrown:
                    if (Timer == 1)
                    {
                        ctx.Npc.NewProjectileDirectInAI_Server<StarCrownP2>(ctx.Npc.Center, Vector2.Zero, 0, 0, ai0: ctx.Npc.whoAmI, ai1: 30 + 5 * 10 + 80);
                    }
                    break;
                case Beat.SpawnStarCircle:
                    if (Timer % 20 == 0)
                    {
                        int round = (int)(Timer / 20);

                        //生成很多圈的星环
                        float r = 16 * (30 + round * 25);
                        float offR = 16 * (round * 6);
                        Vector2 dir = (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero);

                        SpawnStarGuider(ctx, ctx.Npc.Center + dir * offR, r, WarnTime);
                    }
                    break;
                case Beat.Over:
                    return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.MucuryBurst);
                default:
                    break;
            }

            return null;
        }


        /// <summary>
        /// 生成星辰指路人
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="center"></param>
        /// <param name="length"></param>
        public void SpawnStarGuider(ShadowBallContext ctx, Vector2 center, float length, int warnTime)
        {
            Projectile p = ctx.Npc.NewProjectileDirectInAI_Server<StarsGuider>(center, Vector2.Zero, 0, 0, ai0: 2, ai1: length, ai2: warnTime);

            ctx.Recorder2 = p.whoAmI;
        }

    }
}
