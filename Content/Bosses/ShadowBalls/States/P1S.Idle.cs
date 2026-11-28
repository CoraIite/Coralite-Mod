using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)SmallShadowBallStateId.Idle, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallIdleState : SmallShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>朝环上的位置追过去</summary>
            Chase,
            /// <summary>已经贴住，跟着锁环的插值走</summary>
            Locked,
        }

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.Idle;

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            SmallShadowBall ball = ctx.Ball;

            ball.totalBalls = owner.smallBalls.Count;

            int count = Math.Max(1, ball.totalBalls);
            float percent = ball.selfIndex / (float)count;

            int dir = ball.selfIndex % 2 == 0 ? -1 : 1;
            float baseRot = owner.LockTimer * 0.01f * dir;
            float tiltSplit = dir * 0.35f;

            ball.lockRotation = ball.lockRotation.AngleLerp((ctx.Npc.Center - owner.NPC.Center).ToRotation(),0.2f);

            GetRingAngles(owner, tiltSplit, out float zyRot, out float xyRot);
            Vector2 targetPos = owner.NPC.Center + ball.Rotate3D(percent, 110 + (count * 2), baseRot, zyRot, xyRot, owner.LockLerpPercent);

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Chase:
                    {
                        float chaseF = Helper.Clamp(Timer / 120f, 0, 1);
                        ctx.Npc.Center = Vector2.SmoothStep(ctx.Npc.Center, targetPos,
                            0.02f + (chaseF * 0.5f));

                        if (Vector2.Distance(ctx.Npc.Center, targetPos) < 4)
                           SwitchBeat(ctx , (int)Beat.Locked);
                    }
                    break;
                case Beat.Locked:
                    ball.LockDistance = 40 + MathF.Sin(ball.selfIndex / (float)ball.totalBalls * MathHelper.TwoPi + Timer * 0.1f) * 10;

                    ctx.Npc.velocity = Vector2.Zero;
                    if (owner.LockLerpPercent < 0.1f)
                    {
                        SwitchBeat(ctx, (int)Beat.Chase);
                        break;
                    }
                    ctx.Npc.Center = Vector2.Lerp(ctx.Npc.Center, targetPos, owner.LockLerpPercent);

                    if (Vector2.DistanceSquared(ctx.Npc.Center, targetPos)
                        > 120 * 120)
                        SwitchBeat(ctx, (int)Beat.Chase);

                    break;
            }
        }

        /// <summary>按本体当前的锁环姿态取环面的两个旋转角</summary>
        private static void GetRingAngles(ShadowBall owner, float tiltSplit, out float zyRot, out float xyRot)
        {
            switch (owner.LockState)
            {
                default:
                case ShadowBall.LockStates.Normal:
                    zyRot = (MathHelper.PiOver2
                        + (MathF.Sin(owner.LockTimer * 0.01f + (3 * 0.005f)) * 0.4f + (3 * 0.1f))
                        + tiltSplit) % MathHelper.TwoPi;
                    xyRot = (owner.LockTimer * 0.02f) % MathHelper.TwoPi;
                    break;

                case ShadowBall.LockStates.ConcentricCircles:
                    zyRot = MathHelper.PiOver2 + tiltSplit;
                    xyRot = 0f;
                    break;

                case ShadowBall.LockStates.ConcentricCirclesAngled:
                    zyRot = 1f + tiltSplit;
                    xyRot = owner.NPC.rotation + MathHelper.PiOver2;
                    break;

                case ShadowBall.LockStates.AngledRotate:
                    zyRot = ((owner.LockTimer * 0.01f + (4 * 0.005f) * -1f) + tiltSplit) % MathHelper.TwoPi;
                    xyRot = owner.NPC.rotation + MathHelper.PiOver2;
                    break;
            }
        }
    }
}
