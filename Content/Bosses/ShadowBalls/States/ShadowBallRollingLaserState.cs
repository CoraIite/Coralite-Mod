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
                    ctx.DeclareApproach(ctx.Target.Center + new Vector2(0, -ShadowBallDirector.RollingHoverHeight),
                        ShadowBallDirector.RollingDeadZone, ShadowBallDirector.RollingApproachSpeed,
                        ShadowBallDirector.RollingApproachLerp, ShadowBallDirector.RollingHoldDamp);
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            if ((Beat)BeatIndex != Beat.KeepDistance)
            {
                return null;
            }

            // Timer == 0 = 本帧 SharedUpdate 刚换到这一拍，跨实体编排只在权威端发生一次。
            if (Timer == 0)
            {
                return ctx.Boss.CommandRollingLaserLayers(ShadowBallDirector.RollingLayerCount)
                    ? null
                    : EndAttack(ctx);
            }

            if (Timer > ShadowBallDirector.RollingMinFrames && ctx.Boss.CheckSmallBallReady())
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
