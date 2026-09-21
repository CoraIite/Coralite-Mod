using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 旋转激光：小球按索引交错成三层圈围着本体转，逐层发射激光；本体只负责吊在玩家头顶保持身位。<br/>
    /// 旧 <c>ShadowBall.RollingLaser</c>（P1.RollingLaser.cs:7-77）；旧收尾是 <c>SwitchState_Test(OnSpawnAnmi)</c> 的调试死循环，已换回 hub。
    /// </summary>
    [VaultState((int)ShadowBallStateId.RollingLaser, typeof(ShadowBallContext))]
    public sealed class ShadowBallRollingLaserState : ShadowBallStateBase
    {
        /// <summary>旋转激光的子拍。旧 <c>SonState</c> CallSmallBalls / KeepDistance。</summary>
        private enum Beat
        {
            /// <summary>分层派活。</summary>
            CallSmallBalls,
            /// <summary>吊在玩家头顶等小球打完。</summary>
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
                    // 锁环切换必须两端同跑：小球待机时的环绕几何读的就是本体的锁环状态。
                    ctx.DeclareKeep();
                    ctx.Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);
                    SwitchBeat(ctx, (int)Beat.KeepDistance);
                    break;

                case Beat.KeepDistance:
                    ctx.DeclareApproach(ctx.Target.Center + new Vector2(0, -RollingHoverHeight),
                        RollingDeadZone, RollingApproachSpeed,
                        RollingApproachLerp, RollingHoldDamp);
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 分层数；小球按索引交错成 3 圈。沿用旧值 P1.RollingLaser.cs:33,35。
            const int RollingLayerCount = 3;

            // 最短持续帧数；到点且全部小球就绪才收招。沿用旧值 P1.RollingLaser.cs:72。
            const int RollingMinFrames = 800;

            if ((Beat)BeatIndex != Beat.KeepDistance)
            {
                return null;
            }

            // Timer == 0 = 本帧 SharedUpdate 刚换到这一拍，跨实体编排只在权威端发生一次。
            if (Timer == 0)
            {
                return ctx.Boss.CommandRollingLaserLayers(RollingLayerCount)
                    ? null
                    : EndAttack(ctx);
            }

            if (Timer > RollingMinFrames && ctx.Boss.CheckSmallBallReady())
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
