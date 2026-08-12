using Coralite.Helpers;
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
            int self = 0;

            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type == ModContent.NPCType<SmallShadowBall>())
                {
                    count++;
                    if (npc.whoAmI == NPC.whoAmI)
                        self = count;
                }
            }

            float percent = (float)self / count;
            ShadowBall ball = (bigBall.ModNPC as ShadowBall);

            float baseRot = ball.LockTimer * 0.01f;
            float zyRot = (1.57f + ball.LockTimer * 0.015f +percent*MathHelper.TwoPi*4f) % MathHelper.TwoPi;
            float xyRot = (ball.LockTimer * (0.02f)) % MathHelper.TwoPi;

            Vector2 targetPos = _3DRotate(percent, 120 + count * 4, baseRot, zyRot, xyRot) + bigBall.Center;

            float chaseF = Helper.Clamp(Timer / 120, 0, 1);

            lockRotation = lockRotation.AngleLerp((NPC.Center- bigBall.Center).ToRotation(), 0.2f);

            switch (SonState)
            {
                default:
                case 0:
                    {
                        int n = (int)(30 - 25 * chaseF);
                        NPC.ChaseGradually(targetPos, 15 + chaseF * 15, n, n + 1);

                        if (Vector2.Distance(NPC.Center, targetPos) < 16 + chaseF * 16)
                        {
                            SonState = 1;
                        }
                    }
                    break;
                case 1:
                    {
                        NPC.velocity = Vector2.Zero;
                        NPC.Center = targetPos;
                        Timer = 0;

                        if (Vector2.DistanceSquared(NPC.Center, targetPos) > 120 * 120)
                        {
                            SonState = 0;
                        }
                    }
                    break;
            }
        }

        public Vector2 _3DRotate(float indexPercent,float Radius, float baseRot, float zyRot, float xyRot)
        {
            float rot = baseRot + indexPercent * MathHelper.TwoPi;

            Vector2 vector2D = rot.ToRotationVector2();
            Vector3 vector3D = Vector3.Transform(vector2D.Vec3(), Matrix.CreateRotationX(zyRot));
            ///将二维的向量转为3维的并绕着X轴旋转一下
            vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationZ(xyRot));///以Z为轴旋转，用来配合影子球自身的旋转

            //将3维向量投影到二维
            float k1 = -1000 / (vector3D.Z - 1000);

            Vector2 targetDir = k1 * new Vector2(vector3D.X, vector3D.Y);
            Vector2 targetCenter = (targetDir * Radius);

            //vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationX(-MathHelper.PiOver2));///以Z为轴旋转，用来配合影子球自身的旋转

            zDepth = vector3D.Z * Radius;

            return targetCenter;// smoother.Update(1 / 60f, targetCenter);
        }
    }
}
