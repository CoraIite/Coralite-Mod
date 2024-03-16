using System;
using Terraria;

namespace Coralite.lib
{
    public class AI_BabyBird
    {
        public void AI_158_BabyBird(Projectile Projectile)
        {
            Player player = Main.player[Projectile.owner];

            #region 一些召唤物通用的东西以及帧图控制

            if (player.dead)
                player.babyBird = false;

            if (player.babyBird)
                Projectile.timeLeft = 2;

            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type] - 1)
                    Projectile.frame = 0;
            }

            #endregion

            float normalSpeed = 6f;
            float maxSpeed = 8f;
            int maxDistance = 800;
            float _150f = 150f;
            int attackTarget = -1;

            #region 寻敌
            Projectile.Minion_FindTargetInRange(maxDistance, ref attackTarget, skipIfCannotHitWithOwnBody: false);
            if (attackTarget != -1)
            {
                NPC nPC = Main.npc[attackTarget];
                if (player.Distance(nPC.Center) > maxDistance)
                    attackTarget = -1;
            }
            #endregion

            if (attackTarget != -1)
            {
                if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    Projectile.tileCollide = true;

                NPC Target = Main.npc[attackTarget];
                float DistanceToTarget = Projectile.Distance(Target.Center);
                Rectangle rectangle = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
                Rectangle value = new Rectangle((int)Target.position.X, (int)Target.position.Y, Target.width, Target.height);
                if (rectangle.Intersects(value))//攻击到的时候
                {
                    Projectile.tileCollide = false;
                    if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < maxSpeed)//为什么是速度X和Y的长度和
                        Projectile.velocity *= 1.1f;

                    if (Projectile.velocity.Length() > maxSpeed)//防止超速
                        Projectile.velocity *= maxSpeed / Projectile.velocity.Length();
                }
                else if (DistanceToTarget > _150f)//距离比较远的时候
                {
                    Vector2 Direction = Projectile.DirectionTo(Target.Center);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Direction * normalSpeed, 0.15f);
                }
                else
                {
                    Projectile.tileCollide = false;
                    Vector2 Direction = Projectile.DirectionTo(Target.Center);
                    Projectile.velocity += new Vector2(Math.Sign(Direction.X), Math.Sign(Direction.Y)) * 0.35f;
                    if (Projectile.velocity.Length() > maxSpeed)
                        Projectile.velocity *= maxSpeed / Projectile.velocity.Length();
                }

                float num6 = 0.025f;
                float num7 = Projectile.width * 3;
                for (int i = 0; i < 1000; i++)
                {
                    if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type && Math.Abs(Projectile.position.X - Main.projectile[i].position.X) + Math.Abs(Projectile.position.Y - Main.projectile[i].position.Y) < num7)
                    {
                        if (Projectile.position.X < Main.projectile[i].position.X)
                            Projectile.velocity.X -= num6;
                        else
                            Projectile.velocity.X += num6;

                        if (Projectile.position.Y < Main.projectile[i].position.Y)
                            Projectile.velocity.Y -= num6;
                        else
                            Projectile.velocity.Y += num6;
                    }
                }

                Projectile.rotation = Projectile.velocity.X * 0.1f;
                Projectile.direction = ((Projectile.velocity.X > 0f) ? 1 : (-1));
                Projectile.spriteDirection = ((Projectile.velocity.X > 0f) ? 1 : (-1));
                return;
            }

            //    Projectile.tileCollide = false;
            //    List<int> ai158_blacklistedTargets = _ai158_blacklistedTargets;
            //    ai158_blacklistedTargets.Clear();
            //    AI_GetMyGroupIndexAndFillBlackList(ai158_blacklistedTargets, out int index, out int _);
            //    Projectile.localAI[0] = index;
            //    Vector2 vector2 = AI_158_GetHomeLocation(player, index);
            //    float num8 = Projectile.Distance(vector2);
            //    bool flag = player.gravDir > 0f && player.fullRotation == 0f && player.headRotation == 0f;
            //    if (num8 > 2000f)
            //    {
            //        Projectile.Center = vector2;
            //        Projectile.frame = Main.projFrames[Projectile.type] - 1;
            //        Projectile.frameCounter = 0;
            //        Projectile.velocity = Vector2.Zero;
            //        Projectile.direction = (Projectile.spriteDirection = player.direction);
            //        Projectile.rotation = 0f;
            //    }
            //    else if (num8 > 40f)
            //    {
            //        float num9 = normalSpeed + num8 * 0.006f;
            //        Vector2 value3 = Projectile.DirectionTo(vector2);
            //        value3 *= MathHelper.Lerp(1f, 5f, Utils.GetLerpValue(40f, 800f, num8, clamped: true));
            //        Projectile.velocity = Vector2.Lerp(Projectile.velocity, value3 * num9, 0.025f);
            //        if (Projectile.velocity.Length() > num9)
            //            Projectile.velocity *= num9 / Projectile.velocity.Length();

            //        float num10 = 0.05f;
            //        float num11 = Projectile.width;
            //        for (int j = 0; j < 1000; j++)
            //        {
            //            if (j != Projectile.whoAmI && Main.projectile[j].active && Main.projectile[j].owner == Projectile.owner && Main.projectile[j].type == Projectile.type && Math.Abs(Projectile.position.X - Main.projectile[j].position.X) + Math.Abs(Projectile.position.Y - Main.projectile[j].position.Y) < num11)
            //            {
            //                if (Projectile.position.X < Main.projectile[j].position.X)
            //                    Projectile.velocity.X -= num10;
            //                else
            //                    Projectile.velocity.X += num10;

            //                if (Projectile.position.Y < Main.projectile[j].position.Y)
            //                    Projectile.velocity.Y -= num10;
            //                else
            //                    Projectile.velocity.Y += num10;
            //            }
            //        }

            //        Projectile.rotation = Projectile.velocity.X * 0.04f;
            //        Projectile.direction = ((Projectile.velocity.X > 0f) ? 1 : (-1));
            //        Projectile.spriteDirection = ((Projectile.velocity.X > 0f) ? 1 : (-1));
            //    }
            //    else if (num8 > 8f + player.velocity.Length())
            //    {
            //        Vector2 vector3 = Projectile.DirectionTo(vector2);
            //        Projectile.velocity += new Vector2(Math.Sign(vector3.X), Math.Sign(vector3.Y)) * 0.05f;
            //        if (Projectile.velocity.Length() > normalSpeed)
            //            Projectile.velocity *= normalSpeed / Projectile.velocity.Length();

            //        Projectile.rotation = Projectile.velocity.X * 0.1f;
            //        Projectile.direction = ((Projectile.velocity.X > 0f) ? 1 : (-1));
            //        Projectile.spriteDirection = ((Projectile.velocity.X > 0f) ? 1 : (-1));
            //    }
            //    else if (flag)
            //    {
            //        Projectile.Center = vector2;
            //        Projectile.frame = Main.projFrames[Projectile.type] - 1;
            //        Projectile.frameCounter = 0;
            //        Projectile.velocity = Vector2.Zero;
            //        Projectile.direction = (Projectile.spriteDirection = player.direction);
            //        Projectile.rotation = 0f;
            //    }
        }
    }
}
