using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.Starline, typeof(ShadowBallContext))]
    public sealed class ShadowBallStarlineState : ShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>看距离决定先牵引还是直接冲</summary>
            CheckDistance,
            /// <summary>被裂隙牵引到玩家头顶</summary>
            GravityMove,
            /// <summary>加速贴近，到位时环射一圈星星</summary>
            FastApproach,
            /// <summary>群星王冠出现，缓慢移动一下</summary>
            StarCrown,
            /// <summary>持续攻击段，保持记录下来的半径并每 30 帧补一颗星</summary>
            StarlineAttack,
            /// <summary>收招后摇</summary>
            Over,
        }

        public override ShadowBallStates StateIndex => ShadowBallStates.Starline;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.CheckDistance://检测距离
                    ctx.SetVelocityLetItGo();
                    if (Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) > 16*30)
                    {
                        ctx.GravityAnchor = ctx.Target.Center + new Vector2(0, -350);
                        ctx.GravityMoveReady(ctx.GravityAnchor);
                        SwitchBeat(ctx, (int)Beat.GravityMove);
                    }
                    else
                    {
                        SwitchBeat(ctx, (int)Beat.FastApproach);
                    }

                    break;

                case Beat.GravityMove://引力移动
                    if (ctx.GravityMove(ctx.GravityAnchor, Timer))
                    {
                        SwitchBeat(ctx, (int)Beat.StarCrown);
                    }

                    break;

                case Beat.FastApproach:

                    ctx.SetVelocityLetItGo();
                    float distance = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center);

                    if (Timer == 1)
                    {
                        ctx.Npc.velocity = (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero) * Math.Clamp(distance / 50, -30, 30);
                    }
                    else
                    {
                        ctx.Npc.velocity *= 0.97f;
                    }

                    if (Timer > 60)
                    {
                        SwitchBeat(ctx, (int)Beat.StarCrown);
                    }
                    break;

                case Beat.StarCrown:
                    ctx.SetVelocityApparoch(ctx.Target.Center, 16*30, 4, 0.05f, 0.95f);

                    if (Timer>30)
                    {
                        SwitchBeat(ctx, (int)Beat.StarlineAttack);
                    }
                    break;
                case Beat.StarlineAttack:
                    ctx.SetVelocityApparoch(ctx.Target.Center, ctx.Recorder1+16*10, 4, 0.05f, 0.95f);
                    break;

                case Beat.Over:
                    ctx.SetVelocitySlowDown(0.95f);
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            const int StarlineRoundTime = 60 * 4;

            switch ((Beat)BeatIndex)
            {
                case Beat.StarCrown:
                    {
                        if (Timer==0)
                        {
                            ctx.Recorder1 = Main.rand.Next(3, 5);
                            if (Main.getGoodWorld)
                            {
                                ctx.Recorder1 += 1;
                            }

                            SpawnStarCrown(ctx, StarlineRoundTime * (int)ctx.Recorder1 + 60);
                        }
                    }
                    return null;
                case Beat.StarlineAttack:
                    if (Timer == 0)
                    {
                        //ShootStarRing(ctx);
                        ctx.NetUpdate();
                    }
                    //if (Timer % StarlineShootInterval == 0)
                    //{
                    //    ShootStar(ctx, Helper.NextVec2Dir());
                    //    ctx.NetUpdate();
                    //}

                    if (Timer > StarlineRoundTime)
                    {
                        // 旧代码这里还有一个遍历 ShadowBallStar 的空循环（P1.Starline.cs:153-159，作者留的 TODO：
                        // "设置消失状态的具体逻辑留空，由用户自己实现"）。空循环没搬，TODO 记在这里。
                        SwitchBeat(ctx, (int)Beat.Over);
                    }

                    return null;

                case Beat.Over:
                    if (Timer > 120)
                    {
                        //return EndAttack(ctx);
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P1Test);
                    }

                    return null;

                default:
                    return null;
            }
        }

        public void SpawnStarCrown(ShadowBallContext ctx,int time)
        {
            ctx.Npc.NewProjectileDirectInAI_Server<StarCrown>(ctx.Npc.Center, Vector2.Zero, 0, 0, ai0: ctx.Npc.whoAmI, ai1:time);
        }

        public void SpawnStarGuider(ShadowBallContext ctx,Vector2 center,float length)
        {
            Projectile p = ctx.Npc.NewProjectileDirectInAI_Server<StarCrown>(ctx.Npc.Center, Vector2.Zero, 0, 0);
        }
    }
}
