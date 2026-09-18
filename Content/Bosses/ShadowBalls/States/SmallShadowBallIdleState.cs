using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 待机环绕：跟着本体锁环的当前姿态在三维环上占一个位，同时也是所有招式的收招出口（小球的"连接段"）。<br/>
    /// 旧 <c>SmallShadowBall.Idle</c>（P1S.Idle.cs:9-91）。<br/>
    /// 环绕几何读的是本体的 <c>LockState</c> / <c>LockTimer</c> / <c>LockLerpPercent</c> 与自身的 <c>selfIndex</c>、小球总数 ——
    /// 这几个量本体都在两端每帧同算（名册每帧重建、锁环在 <c>AI()</c> 里两端同跑），所以这里是确定性的（C1 / C3）。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.Idle, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallIdleState : SmallShadowBallStateBase
    {
        /// <summary>待机的子拍。旧 <c>SonState</c> 0~1。</summary>
        private enum Beat
        {
            /// <summary>朝环上的位置追过去。</summary>
            Chase,
            /// <summary>已经贴住，跟着锁环的插值走。</summary>
            Locked,
        }

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.Idle;

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            SmallShadowBall ball = ctx.Ball;

            // 小球总数可能为 0（本体名册还没建起来 / 这颗球刚被剔出名册）；除法要先保底，否则位置整个变 NaN。
            int count = Math.Max(1, owner.smallBalls.Count);
            float percent = ball.selfIndex / (float)count;

            int dir = ball.selfIndex % 2 == 0 ? -1 : 1;
            float baseRot = owner.LockTimer * SmallShadowBallDirector.IdleBaseRotPerTick * dir;
            float tiltSplit = dir * SmallShadowBallDirector.IdleTiltSplit;

            ball.lockRotation = ball.lockRotation.AngleLerp((ctx.Npc.Center - owner.NPC.Center).ToRotation(),
                SmallShadowBallDirector.IdleLockRotLerp);

            GetRingAngles(owner, tiltSplit, out float zyRot, out float xyRot);

            Vector2 targetPos = owner.NPC.Center + ball.Rotate3D(percent,
                SmallShadowBallDirector.IdleRadiusBase + (count * SmallShadowBallDirector.IdleRadiusPerBall),
                baseRot, zyRot, xyRot);

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Chase:
                    {
                        float chaseF = Helper.Clamp(Timer / SmallShadowBallDirector.IdleChaseRampFrames, 0, 1);
                        ctx.Npc.Center = Vector2.SmoothStep(ctx.Npc.Center, targetPos,
                            SmallShadowBallDirector.IdleChaseLerpBase + (chaseF * SmallShadowBallDirector.IdleChaseLerpGain));

                        if (Vector2.Distance(ctx.Npc.Center, targetPos) < SmallShadowBallDirector.IdleLockOnDistance)
                        {
                            // 不走 SwitchBeat：这一拍靠 Timer 爬坡决定追位速度，清零反而会让它每次贴不上都从头慢起步（旧代码同样不清）。
                            BeatIndex = (int)Beat.Locked;
                        }
                    }

                    break;

                case Beat.Locked:
                    ctx.Npc.velocity = Vector2.Zero;
                    ctx.Npc.Center = Vector2.Lerp(ctx.Npc.Center, targetPos, owner.LockLerpPercent);

                    // 旧 P1S.Idle.cs:82 每帧把 Timer 压回 0，于是退回追位时爬坡也从头开始。
                    Timer = 0;

                    if (Vector2.DistanceSquared(ctx.Npc.Center, targetPos)
                        > SmallShadowBallDirector.IdleBreakDistance * SmallShadowBallDirector.IdleBreakDistance)
                    {
                        BeatIndex = (int)Beat.Chase;
                    }

                    break;
            }
        }

        /// <summary>按本体当前的锁环姿态取环面的两个旋转角。旧 P1S.Idle.cs:31-58。</summary>
        private static void GetRingAngles(ShadowBall owner, float tiltSplit, out float zyRot, out float xyRot)
        {
            switch (owner.LockState)
            {
                default:
                case ShadowBall.LockStates.Normal:
                    zyRot = (SmallShadowBallDirector.IdleNormalTiltBase
                        + (MathF.Sin(owner.LockTimer * SmallShadowBallDirector.IdleNormalTiltFreq) * SmallShadowBallDirector.IdleNormalTiltAmp)
                        + tiltSplit) % MathHelper.TwoPi;
                    xyRot = (SmallShadowBallDirector.IdleNormalSpinBase
                        + (owner.LockTimer * SmallShadowBallDirector.IdleNormalSpinRate)) % MathHelper.TwoPi;
                    break;

                case ShadowBall.LockStates.ConcentricCircles:
                    zyRot = MathHelper.PiOver2 + tiltSplit;
                    xyRot = 0f;
                    break;

                case ShadowBall.LockStates.ConcentricCirclesAngled:
                    zyRot = SmallShadowBallDirector.IdleAngledTilt + tiltSplit;
                    xyRot = owner.NPC.rotation + MathHelper.PiOver2;
                    break;

                case ShadowBall.LockStates.AngledRotate:
                    zyRot = ((owner.LockTimer * SmallShadowBallDirector.IdleAngledRotRate * -1f) + tiltSplit) % MathHelper.TwoPi;
                    xyRot = owner.NPC.rotation + MathHelper.PiOver2;
                    break;
            }
        }
    }
}
