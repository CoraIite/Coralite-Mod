using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 影刺：本体沉到玩家下方，小球在两侧一字排开，等全部就位后本体发光上戳。<br/>
    /// 旧 <c>ShadowBall.ShadowSpike</c>（P1.ShadowSpike.cs:11-121）；旧收尾是 <c>SwitchState_Test(OnSpawnAnmi)</c> 的调试死循环，
    /// 真出口 <c>SwitchP1State()</c> 就被注释在它上一行，已换回 hub。<br/><br/>
    /// <b>注意</b>：这一招招式体完整，但旧的一阶段权重表里没有它（ShadowBall.cs:500-506），当前谁也不会选到。按 D10 不擅自加进池子。
    /// </summary>
    [VaultState((int)ShadowBallStateId.ShadowSpike, typeof(ShadowBallContext))]
    public sealed class ShadowBallShadowSpikeState : ShadowBallStateBase
    {
        /// <summary>影刺的子拍。旧 <c>SonState</c> 0~3。</summary>
        private enum Beat
        {
            /// <summary>短暂前摇，算出玩家下方的落点。</summary>
            Ready,
            /// <summary>被裂隙牵引下去。</summary>
            Move,
            /// <summary>等小球一字排开。</summary>
            WaitBalls,
            /// <summary>上戳后摇。</summary>
            End,
        }

        public override ShadowBallStateId StateIndex => ShadowBallStateId.ShadowSpike;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Ready:
                    ctx.DeclareDamp(ShadowBallDirector.SpikeReadyDamp);
                    if (Timer > ShadowBallDirector.SpikeReadyFrames)
                    {
                        // 落点只取预判的 X，Y 固定在玩家下方（旧 P1.ShadowSpike.cs:38-45 就是这么写的）。
                        Vector2 predicted = PredictTarget(ctx, ShadowBallDirector.SpikeLeadLength, ShadowBallDirector.SpikeLeadLength);
                        GravityAnchor = new Vector2(predicted.X, ctx.Target.Center.Y + ShadowBallDirector.SpikeDropHeight);
                        ctx.GravityMoveReady(GravityAnchor);
                        SwitchBeat(ctx, (int)Beat.Move);
                    }

                    break;

                case Beat.Move:
                    if (ctx.GravityMove(GravityAnchor, Timer))
                    {
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.AngledRotate);
                        SwitchBeat(ctx, (int)Beat.WaitBalls);
                    }

                    break;

                case Beat.WaitBalls:
                    ctx.DeclareKeep();
                    ctx.DeclareRotation(0f, ShadowBallDirector.SpikeRotationLerp);
                    break;

                case Beat.End:
                    ctx.DeclareDamp(ShadowBallDirector.SpikeEndDamp);
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                // Timer == 0 = 本帧 SharedUpdate 刚到位，跨实体编排只在权威端发生一次。
                case Beat.WaitBalls when Timer == 0:
                    return DeploySmallBalls(ctx);

                case Beat.WaitBalls:
                    // "小球是否全部就位"只有权威端知道（Ready 是本地记账），所以这一拍的换拍与起跳速度都在权威端落地，
                    // 客户端靠同帧出门的快照（velocity 是原版同步量 + Beat 在热槽里）跟上。
                    if (!ctx.Boss.CheckSmallBallReady())
                    {
                        return null;
                    }

                    ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);
                    RiseUp(ctx);
                    SwitchBeat(ctx, (int)Beat.End);
                    return null;

                case Beat.End:
                    if (Timer > ShadowBallDirector.SpikeEndFrames)
                    {
                        return EndAttack(ctx);
                    }

                    return null;

                default:
                    return null;
            }
        }

        /// <summary>
        /// 叫小球到位：一个都没有就收招，否则按与玩家的 X 距离决定叫几个，多出来的直接标就绪免得卡住等待。
        /// 旧 P1.ShadowSpike.cs:62-84。
        /// </summary>
        private static IVaultState<ShadowBallContext> DeploySmallBalls(ShadowBallContext ctx)
        {
            ShadowBall boss = ctx.Boss;
            int smallBallCount = boss.GetSmallBalls();

            if (smallBallCount < 1)
            {
                return EndAttack(ctx);
            }

            int howMany = CallBackCount(ctx, smallBallCount, ShadowBallDirector.SpikePerLength);
            boss.CommandSmallBalls(SmallShadowBallStateId.ShadowSpike, howMany, readyRest: true);
            ctx.MarkDecision();
            return null;
        }

        /// <summary>上戳：纵向起跳，横向差得太远时补一脚。旧 P1.ShadowSpike.cs:100-106。</summary>
        private static void RiseUp(ShadowBallContext ctx)
        {
            ctx.Npc.velocity.Y = ShadowBallDirector.SpikeRiseSpeed;

            float dis = MathF.Abs(ctx.Target.Center.X - ctx.Npc.Center.X);
            if (dis > ShadowBallDirector.SpikeDashThreshold)
            {
                ctx.Npc.velocity.X += MathF.Sign(ctx.Target.Center.X - ctx.Npc.Center.X)
                    * Helper.Clamp(dis / ShadowBallDirector.SpikeDashRange, ShadowBallDirector.RevolutionLeadMin, 1f)
                    * ShadowBallDirector.SpikeDashSpeed;
            }

            ctx.MarkDecision();
        }
    }
}
