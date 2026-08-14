using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public partial class SmallShadowBall
    {
        public void Idle(NPC bigBall)
        {
            //环绕大球移动
            //SwitchState(AIStates.OnSpawnAnmi);

            int count = (bigBall.ModNPC as ShadowBall).smallBalls.Count;

            ShadowBall ball = (bigBall.ModNPC as ShadowBall);

            float percent = (float)selfIndex / ball.smallBalls.Count;

            float zyRot;
            float xyRot;


            float chaseF = Helper.Clamp(Timer / 120, 0, 1);

            lockRotation = lockRotation.AngleLerp((NPC.Center- bigBall.Center).ToRotation(), 0.2f);

            int dir = selfIndex % 2 == 0 ? -1 : 1;
            float baseRot = ball.LockTimer * 0.01f * dir;

            switch (ball.LockState)
            {
                default:
                case ShadowBall.LockStates.Normal:
                    {
                        zyRot = (1.57f + MathF.Sin(ball.LockTimer * (0.01f + 3 * 0.005f)) * (0.4f + 3 * 0.1f) + dir * 0.3f) % MathHelper.TwoPi;
                        xyRot = (MathHelper.TwoPi / 3 * 3 + ball.LockTimer * (0.02f)) % MathHelper.TwoPi;
                    }
                    break;
                case ShadowBall.LockStates.ConcentricCircles:
                    {
                        zyRot = MathHelper.PiOver2+ dir * 0.3f;
                        xyRot = 0;
                    }
                    break;
                case ShadowBall.LockStates.ConcentricCirclesAngled:
                    {
                        zyRot = 1f+ dir * 0.3f;
                        xyRot =bigBall.rotation + MathHelper.PiOver2;
                    }
                    break;
                case ShadowBall.LockStates.AngledRotate:
                    {
                        zyRot = ((ball.LockTimer * (0.01f + 4 * 0.005f)) * -1 + dir * 0.3f) % MathHelper.TwoPi;
                        xyRot = bigBall.rotation + MathHelper.PiOver2;
                    }
                    break;
            }

            Vector2 targetPos = _3DRotate(percent, 120 + count * 4, baseRot, zyRot, xyRot) + bigBall.Center;

            switch (SonState)
            {
                default:
                case 0:
                    {
                        int n = (int)(30 - 25 * chaseF);
                        NPC.ChaseGradually(targetPos, 32 + chaseF * 32, n, n + 1);

                        if (Vector2.Distance(NPC.Center, targetPos) < 33 + chaseF * 33)
                        {
                            SonState = 1;
                        }
                    }
                    break;
                case 1:
                    {
                        NPC.velocity = Vector2.Zero;
                        NPC.Center = Vector2.Lerp(NPC.Center, targetPos, ball.LockLerpPercent);
                        Timer = 0;

                        if (Vector2.DistanceSquared(NPC.Center, targetPos) > 120 * 120)
                        {
                            SonState = 0;
                        }
                    }
                    break;
            }
        }

        public Vector2 _3DRotate(float percent,float Radius, float baseRot, float zyRot, float xyRot)
        {
            float rot = baseRot + percent * MathHelper.TwoPi;

            Vector2 vector2D = rot.ToRotationVector2();
            Vector3 vector3D = Vector3.Transform(vector2D.Vec3(), Matrix.CreateRotationX(zyRot));
            ///将二维的向量转为3维的并绕着X轴旋转一下
            vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationZ(xyRot));///以Z为轴旋转，用来配合影子球自身的旋转

            //将3维向量投影到二维
            float k1 = -1000 / (vector3D.Z - 1000);

            Vector2 targetDir = k1 * new Vector2(vector3D.X, vector3D.Y);
            Vector2 targetCenter = (targetDir * Radius);

            //vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationX(-MathHelper.PiOver2));///以Z为轴旋转，用来配合影子球自身的旋转

            float targetZ = vector3D.Z * Radius;

            float lerpValue = Helper.Clamp(MathF.Abs(zDepth - targetZ) / 100f, 0.1f, 1);
            zDepth = Helper.Lerp(zDepth, targetZ, lerpValue);

            return targetCenter;// smoother.Update(1 / 60f, targetCenter);
        }
    }
}
