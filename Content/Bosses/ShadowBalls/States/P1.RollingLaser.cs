using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 旋转激光：小球按索引交错成三层圈围着本体转，逐层发射激光<br/>
    /// </summary>
    [VaultState((int)ShadowBallStateId.RollingLaser, typeof(ShadowBallContext))]
    public sealed class ShadowBallRollingLaserState : ShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>分层派活</summary>
            CallSmallBalls,
            /// <summary>吊在玩家头顶等小球打完</summary>
            KeepDistance,
        }

        public override ShadowBallStateId StateIndex => ShadowBallStateId.RollingLaser;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 保持身位的落点：玩家头顶上方。沿用旧值 P1.RollingLaser.cs:63。
            const float RollingHoverHeight = 300f;

            // 落点死区半径；在死区外靠近、死区内减速。沿用旧值 P1.RollingLaser.cs:64,67,70。
            const float RollingDeadZone = 16 * 5;
            const float RollingApproachSpeed = 4f;
            const float RollingApproachLerp = 0.03f;
            const float RollingHoldDamp = 0.9f;

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.CallSmallBalls:
                    ctx.SetVelocityLetItGo();
                    ctx.Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);

                    if (Timer>=1)
                        SwitchBeat(ctx, (int)Beat.KeepDistance);
                    break;

                case Beat.KeepDistance:
                    ctx.SetVelocityApparoch(ctx.Target.Center + new Vector2(0, -RollingHoverHeight),
                        RollingDeadZone, RollingApproachSpeed,
                        RollingApproachLerp, RollingHoldDamp);
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

                    }
                    break;
                case Beat.KeepDistance:
                    if (Timer == 0)
                    {
                        int RollingLayerCount = Helper.ScaleValueForDiffMode(3, 3, 4, 5);//小球的层数

                        return CommandRollingLaserLayers(ctx, RollingLayerCount)
                            ? null
                            : EndAttack(ctx);
                    }

                    if (Timer > 800 && ctx.Boss.CheckSmallBallReady())
                    {
                        return EndAttack(ctx);
                    }

                    break;
                default:
                    break;
            }

            /// <summary>
            /// 旋转激光的分层派活：小球按索引均分成 <paramref name="layerCount"/> 层，每个小球记下自己在本层的序号、层号与本层总数；
            /// 除不尽剩下的直接标就绪。小球不足层数就返回 false（本体据此收招）。旧 P1.RollingLaser.cs:17-58。
            /// </summary>
            bool CommandRollingLaserLayers(ShadowBallContext ctx,int layerCount)
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


            return null;
        }
    }
}
