using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Coralite.lib
{
    public class AI_Pet
    {
        static float gravityAccel = 0.4f;

        public void PetAI(Projectile Projectile, float offset, int closedLenth = 500)
        {
            Player owner = Main.player[Projectile.owner];

            //判断玩家是否还活着（如果前面判断过可以直接删了这段
            if (!owner.active)
            {
                Projectile.active = false;
                return;
            }

            bool projInRight = false;
            bool projInLeft = false;
            bool FaceSoildTile = false;

            int ClosedLenth = closedLenth;
            int Lenth_85 = 85;

            float State = Projectile.ai[0];

            // 判断弹幕在人物左还是右
            if (owner.Center.X - owner.direction * offset < Projectile.Center.X - Lenth_85)
            {
                projInRight = true;
            }
            else if (owner.Center.X - owner.direction * offset > Projectile.Center.X + Lenth_85)
            {
                projInLeft = true;
            }

            Vector2 Center = Projectile.Center;

            float DistanceToOwner_X = owner.Center.X - owner.direction * offset - Center.X;
            float DistanceToOwner_Y = owner.Center.Y - Center.Y;
            float DistanceToOwner = (float)Math.Sqrt(DistanceToOwner_X * DistanceToOwner_X + DistanceToOwner_Y * DistanceToOwner_Y);

            if (DistanceToOwner > 2000f)//距离太远直接传送
            {
                Projectile.Center = owner.Center;
            }
            else if (DistanceToOwner > ClosedLenth || (Math.Abs(DistanceToOwner_Y) > 300f))//距离远了
            {
                if (DistanceToOwner_Y > 0f && Projectile.velocity.Y < 0f)
                    Projectile.velocity.Y = 0f;

                if (DistanceToOwner_Y < 0f && Projectile.velocity.Y > 0f)
                    Projectile.velocity.Y = 0f;

                State = 1;
            }
            //Main.NewText(State, Color.White);
            if (State != 0)
            {
                Projectile.tileCollide = false;
                float accel = 0.2f;
                //float distance = 10f;
                //int Distance200 = 200;

                //和玩家距离小于200，玩家Y方向速度为0，弹幕Y小于玩家Y，弹幕没有和物块碰撞
                //原版的代码，因为删了一些东西导致这一段永远不会被运行
                //if (DistanceToOwner < Distance200 
                //    && owner.velocity.Y == 0f 
                //    && Projectile.Bottom.Y <= owner.Bottom.Y 
                //    && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                //{//限制向上的最大速度
                //    State = 0f;
                //    if (Projectile.velocity.Y < -6f)
                //        Projectile.velocity.Y = -6f;
                //}

                //if (DistanceToOwner < 60f)
                //{
                //    DistanceToOwner_X = Projectile.velocity.X;
                //    DistanceToOwner_Y = Projectile.velocity.Y;
                //}
                //else
                //{
                //    DistanceToOwner = distance / DistanceToOwner;
                //    DistanceToOwner_X *= DistanceToOwner;
                //    DistanceToOwner_Y *= DistanceToOwner;
                //}

                if (Projectile.velocity.X < DistanceToOwner_X)
                {
                    Projectile.velocity.X += accel;
                    if (Projectile.velocity.X < 0f)
                        Projectile.velocity.X += accel * 1.5f;
                }

                if (Projectile.velocity.X > DistanceToOwner_X)
                {
                    Projectile.velocity.X -= accel;
                    if (Projectile.velocity.X > 0f)
                        Projectile.velocity.X -= accel * 1.5f;
                }

                if (Projectile.velocity.Y < DistanceToOwner_Y)
                {
                    Projectile.velocity.Y += accel;
                    if (Projectile.velocity.Y < 0f)
                        Projectile.velocity.Y += accel * 1.5f;
                }

                if (Projectile.velocity.Y > DistanceToOwner_Y)
                {
                    Projectile.velocity.Y -= accel;
                    if (Projectile.velocity.Y > 0f)
                        Projectile.velocity.Y -= accel * 1.5f;
                }

                if (Projectile.velocity.X > 0.5)
                    Projectile.spriteDirection = -1;
                else if (Projectile.velocity.X < -0.5)
                    Projectile.spriteDirection = 1;

                Projectile.rotation = Projectile.velocity.Y * 0.05f * Projectile.direction;
            }
            else
            {
                Projectile.rotation = 0f;
                Projectile.tileCollide = true;

                float accel = 0.08f;
                float speedmax = 6.5f;

                //向玩家移动
                if (projInRight)
                {
                    if (Projectile.velocity.X > -3.5)
                        Projectile.velocity.X -= accel;
                    else
                        Projectile.velocity.X -= accel * 0.25f;
                }
                else if (projInLeft)
                {
                    if (Projectile.velocity.X < 3.5)
                        Projectile.velocity.X += accel;
                    else
                        Projectile.velocity.X += accel * 0.25f;
                }
                else//距离玩家较近减速
                {
                    Projectile.velocity.X *= 0.9f;
                    if (Projectile.velocity.X >= 0f - accel && Projectile.velocity.X <= accel)
                        Projectile.velocity.X = 0f;
                }
                //判断面前是否有方块
                if (projInRight || projInLeft)
                {
                    int ProjToTile_X = (int)Projectile.Center.X / 16;
                    int ProjToTile_Y = (int)Projectile.Center.Y / 16;

                    if (projInRight)
                        ProjToTile_X--;

                    if (projInLeft)
                        ProjToTile_X++;

                    ProjToTile_X += (int)Projectile.velocity.X;
                    if (WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y))
                        FaceSoildTile = true;
                }
                //上台阶
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
                //判断是否低于玩家
                bool LowerToOwner;
                if (owner.Bottom.Y - 8f < Projectile.Bottom.Y)
                    LowerToOwner = true;
                else
                    LowerToOwner = false;

                if (!LowerToOwner && (Projectile.velocity.X < 0f || Projectile.velocity.X > 0f))
                {
                    int ProjToTile_X = (int)Projectile.Center.X / 16;
                    int ProjToTile_YPlus1 = (int)Projectile.Center.Y / 16 + 1;
                    if (projInRight)
                        ProjToTile_X--;

                    if (projInLeft)
                        ProjToTile_X++;

                    WorldGen.SolidTile(ProjToTile_X, ProjToTile_YPlus1);
                }

                if (Projectile.velocity.Y == 0f)
                {
                    //面前有方块时起跳
                    if (FaceSoildTile)
                    {
                        int ProjToTile_X = (int)Projectile.Center.X / 16;
                        int ProjToTile_Y = (int)Projectile.Bottom.Y / 16;
                        if (WorldGen.SolidTileAllowBottomSlope(ProjToTile_X, ProjToTile_Y) || Main.tile[ProjToTile_X, ProjToTile_Y].IsHalfBlock || (byte)Main.tile[ProjToTile_X, ProjToTile_Y].Slope > 0)
                        {
                            try
                            {
                                ProjToTile_X = (int)Projectile.Center.X / 16;
                                ProjToTile_Y = (int)Projectile.Center.Y / 16;
                                if (projInRight)
                                    ProjToTile_X--;

                                if (projInLeft)
                                    ProjToTile_X++;

                                ProjToTile_X += (int)Projectile.velocity.X;
                                if (!WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y - 1) && !WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y - 2))
                                    Projectile.velocity.Y = -5.1f;
                                else if (!WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y - 2))
                                    Projectile.velocity.Y = -7.1f;
                                else if (WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y - 5))
                                    Projectile.velocity.Y = -11.1f;
                                else if (WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y - 4))
                                    Projectile.velocity.Y = -10.1f;
                                else
                                    Projectile.velocity.Y = -9.1f;
                            }
                            catch
                            {
                                Projectile.velocity.Y = -9.1f;
                            }
                        }
                    }
                }
                else if (Projectile.velocity.Y != 0f && LowerToOwner)
                {
                    Projectile.velocity.Y -= 0.5f;

                    int ProjToTile_X = (int)Projectile.Top.X / 16;
                    int ProjToTilePlus1_Y = ((int)Projectile.Top.Y / 16) - 1;
                    if (WorldGen.SolidTile(ProjToTile_X, ProjToTilePlus1_Y))
                        Projectile.tileCollide = false;
                    else
                        Projectile.tileCollide = true;
                }

                if (Projectile.velocity.X > speedmax)
                    Projectile.velocity.X = speedmax;

                if (Projectile.velocity.X < 0f - speedmax)
                    Projectile.velocity.X = 0f - speedmax;

                if (Projectile.velocity.X != 0f)
                    Projectile.direction = Math.Sign(Projectile.velocity.X);
                else
                    Projectile.direction = Math.Sign(owner.position.X - Projectile.position.X);

                if (Projectile.velocity.X > accel || projInLeft)
                    Projectile.direction = 1;

                if (Projectile.velocity.X < 0f - accel || projInRight)
                    Projectile.direction = -1;

                Projectile.spriteDirection = -Projectile.direction;

                //重力以及最大速度
                Gravity(Projectile, gravityAccel);
            }
        }

        public void Gravity(Projectile Projectile, float grivityAccel)
        {
            Projectile.velocity.Y += grivityAccel;
            if (Projectile.velocity.Y > 10f)
                Projectile.velocity.Y = 10f;
        }
    }
}

