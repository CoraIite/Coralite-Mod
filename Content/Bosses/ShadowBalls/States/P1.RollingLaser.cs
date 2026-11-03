using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 旋转激光：小球按索引交错成三层圈围着本体转，逐层发射激光<br/>
    /// </summary>
    [VaultState((int)ShadowBallStates.RollingLaser, typeof(ShadowBallContext))]
    public sealed class ShadowBallRollingLaserState : ShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>准备</summary>
            Ready,
            /// <summary>引力移动</summary>
            GravityMove,
            /// <summary>分层派活</summary>
            CallSmallBalls,
            /// <summary>吊在玩家头顶等小球打完</summary>
            KeepDistance,
        }

        public override ShadowBallStates StateIndex => ShadowBallStates.RollingLaser;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Ready://准备
                    GravityAnchor = ctx.Target.Center + ctx.Target.velocity * 20 + new Vector2(0, -350);
                    ctx.GravityMoveReady(GravityAnchor);

                    ctx.SetVelocityLetItGo();
                    SwitchBeat(ctx, (int)Beat.GravityMove);
                    break;

                case Beat.GravityMove:
                    if (ctx.GravityMove(GravityAnchor, Timer))
                    {
                        ctx.Hot[BossSlots.E] = MathHelper.Clamp(Vector2.Distance(ctx.Npc.Center, ctx.Target.Center), 600f, 1000f);
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.AngledRotate);
                        SwitchBeat(ctx, (int)Beat.CallSmallBalls);
                    }
                    break;
                case Beat.CallSmallBalls:
                    ctx.SetVelocityLetItGo();
                    ctx.Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);

                    if (Timer >= 1)
                        SwitchBeat(ctx, (int)Beat.KeepDistance);
                    break;

                case Beat.KeepDistance:

                    Vector2 dir = (-MathHelper.PiOver2 + ctx.Hot[BossSlots.E] + MathF.Sin(Timer * 0.03f) * MathHelper.PiOver4).ToRotationVector2();

                    ctx.SetVelocityApparoch(ctx.Target.Center + dir * 350, 16 * 5, 7, 0.01f, 0.95f);
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {         
            switch ((Beat)BeatIndex)
            {
                case Beat.CallSmallBalls:
                    if (Timer == 0)//随机大球的跟随位置
                    {
                        ctx.Hot[BossSlots.E]=Main.rand.NextFromList(-0.7f,0,0.7f);
                        ctx.Npc.netUpdate = true;
                    }
                    break;
                case Beat.KeepDistance:
                    if (Timer == 0)
                    {
                        int RollingLayerCount = Helper.ScaleValueForDiffMode(3, 3, 4, 5);//小球的层数

                        return CommandRollingLaserLayers(ctx, RollingLayerCount) ? null : EndAttack(ctx);
                    }

                    if (Timer > 800 && ctx.Boss.CheckSmallBallReady())
                    {
                        //return EndAttack(ctx);
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P1Test);
                    }

                    break;
                default:
                    break;
            }

            return null;

            /// <summary>
            /// 旋转激光的分层派活：小球按索引均分成 <paramref name="layerCount"/> 层，每个小球记下自己在本层的序号、层号与本层总数；
            /// 除不尽剩下的直接标就绪。小球不足层数就返回 false（本体据此收招）。旧 P1.RollingLaser.cs:17-58。
            /// </summary>
            static bool CommandRollingLaserLayers(ShadowBallContext ctx, int layerCount)
            {
                if (VaultUtils.isClient)
                    return true;

                int smallBallCount = ctx.Boss.GetSmallBalls();
                if (smallBallCount < layerCount)
                    return false;

                ctx.Boss.SmallBallStartAttack();

                int perLayer = smallBallCount / layerCount;
                for (int layer = 0; layer < layerCount; layer++)
                    for (int i = 0; i < perLayer; i++)
                    {
                        if (ctx.Boss.smallBalls[i + (layer * perLayer)].ModNPC is SmallShadowBall smallBall)
                        {
                            smallBall.SwitchState(SmallShadowBallStateId.RollingLaser);
                            smallBall.ServerSetRollingLayer(i, layer, perLayer);
                        }
                    }

                for (int i = perLayer * layerCount; i < ctx.Boss.smallBalls.Count; i++)//剩余的小球设置为
                    if (ctx.Boss.smallBalls[i].ModNPC is SmallShadowBall extra)
                        extra.SetReady();

                return true;
            }
        }
    }
}
