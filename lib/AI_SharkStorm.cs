using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.lib
{
    public class AI_SharkStorm
    {
        private void AI_062(Projectile projectile)
        {
            float _20f = 20f;
            float _40f = 40f;
            float _069f = 0.69f;

            if (Main.player[projectile.owner].dead)
                Main.player[projectile.owner].sharknadoMinion = false;

            if (Main.player[projectile.owner].sharknadoMinion)//关于召唤物存活的部分
                projectile.timeLeft = 2;

            float num10 = 0.05f;
            float Width = projectile.width;

            num10 = 0.1f;
            Width *= 2f;

            for (int m = 0; m < 1000; m++)
            {
                if (m != projectile.whoAmI && Main.projectile[m].active && Main.projectile[m].owner == projectile.owner && Main.projectile[m].type == projectile.type && Math.Abs(projectile.position.X - Main.projectile[m].position.X) + Math.Abs(projectile.position.Y - Main.projectile[m].position.Y) < Width)
                {
                    if (projectile.position.X < Main.projectile[m].position.X)
                        projectile.velocity.X -= num10;
                    else
                        projectile.velocity.X += num10;

                    if (projectile.position.Y < Main.projectile[m].position.Y)
                        projectile.velocity.Y -= num10;
                    else
                        projectile.velocity.Y += num10;
                }
            }

            Vector2 Position = projectile.position;
            float _2000f = 2000f;

            bool HasTarget = false;
            int num13 = -1;
            //projectile.tileCollide = true;

            #region 关于在墙里的话的透明度变化

            projectile.tileCollide = false;
            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.alpha += 20;
                if (projectile.alpha > 150)
                    projectile.alpha = 150;
            }
            else
            {
                projectile.alpha -= 50;
                if (projectile.alpha < 60)
                    projectile.alpha = 60;
            }
            #endregion

            #region 查找攻击目标
            Vector2 OwnerCenter = Main.player[projectile.owner].Center;
            Vector2 HalfSize = new Vector2(0.5f);

            NPC ownerMinionAttackTargetNPC = projectile.OwnerMinionAttackTargetNPC;

            if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(this))
            {
                Vector2 TargetCenter = ownerMinionAttackTargetNPC.position + ownerMinionAttackTargetNPC.Size * HalfSize;
                float _6000f = _2000f * 3f;
                float DistanceToTarget = Vector2.Distance(TargetCenter, OwnerCenter);

                if (DistanceToTarget < _6000f && !HasTarget && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, ownerMinionAttackTargetNPC.position, ownerMinionAttackTargetNPC.width, ownerMinionAttackTargetNPC.height))
                {
                    _2000f = DistanceToTarget;
                    Position = TargetCenter;
                    HasTarget = true;
                    num13 = ownerMinionAttackTargetNPC.whoAmI;
                }
            }

            if (!HasTarget)
            {
                for (int n = 0; n < 200; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.CanBeChasedBy(this))
                    {
                        Vector2 NPCCenter = npc.position + npc.Size * HalfSize;
                        float DistanceToNPC = Vector2.Distance(NPCCenter, OwnerCenter);
                        if (!(DistanceToNPC >= _2000f) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            _2000f = DistanceToNPC;
                            Position = NPCCenter;
                            HasTarget = true;
                            num13 = n;
                        }
                    }
                }
            }
            #endregion

            int _500or1000 = 500;

            if (HasTarget)
                _500or1000 = 1000;

            Player player = Main.player[projectile.owner];
            if (Vector2.Distance(player.Center, projectile.Center) > _500or1000)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }

            if (projectile.ai[0] == 1f)
                projectile.tileCollide = false;//与玩家距离大于500或1000的话不会被墙阻挡

            //感觉永远也不会有>=2的情况出现
            if (projectile.ai[0] >= 2f)
            {
                projectile.ai[0] += 1f;

                if (!HasTarget)
                    projectile.ai[0] += 1f;

                if (projectile.ai[0] > _40f)
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }//大于40归零

                projectile.velocity *= _069f;
            }
            else if (HasTarget && projectile.ai[0] == 0f)
            {
                Vector2 DirectionToTarget = Position - projectile.Center;
                float LenthToTarget = DirectionToTarget.Length();
                DirectionToTarget = DirectionToTarget.SafeNormalize(Vector2.Zero);

                if (LenthToTarget > 400f)
                {
                    float _3f = 3f;
                    DirectionToTarget *= _3f;
                    projectile.velocity = (projectile.velocity * 20f + DirectionToTarget) / 21f;//渐进目标
                }
                else
                {
                    projectile.velocity *= 0.96f;//与目标保持距离
                }

                if (LenthToTarget > 200f)
                {
                    float _6f = 6f;
                    DirectionToTarget *= _6f;
                    float _40ff = _20f * 2f;
                    projectile.velocity.X = (projectile.velocity.X * _40ff + DirectionToTarget.X) / (_40ff + 1f);
                    projectile.velocity.Y = (projectile.velocity.Y * _40ff + DirectionToTarget.Y) / (_40ff + 1f);
                }
                else if (projectile.velocity.Y > -1f)
                {
                    projectile.velocity.Y -= 0.1f;//距离近的时候向上更快？
                }
            }
            else//无目标
            {
                float _9f = 9f;

                Vector2 Center = projectile.Center;
                Vector2 DistanceToOwner = player.Center - Center + new Vector2(0f, -60f);

                DistanceToOwner += new Vector2(0f, 40f);

                float LenthToOwner = DistanceToOwner.Length();

                if (LenthToOwner < 100f && projectile.ai[0] == 1f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }

                if (LenthToOwner > 2000f)//距离过远直接传送
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - projectile.width / 2;
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - projectile.width / 2;
                }

                if (Math.Abs(DistanceToOwner.X) > 40f || Math.Abs(DistanceToOwner.Y) > 10f)//距离玩家有一定距离时候
                {
                    DistanceToOwner = DistanceToOwner.SafeNormalize(Vector2.Zero);
                    DistanceToOwner *= _9f;
                    DistanceToOwner *= new Vector2(1.25f, 0.65f);
                    projectile.velocity = (projectile.velocity * 20f + DistanceToOwner) / 21f;  //？？？ 是看不懂的操作 总之是渐进玩家
                }
                else//距离玩家近时候
                {
                    if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                    {
                        projectile.velocity.X = -0.15f;
                        projectile.velocity.Y = -0.05f;
                    }  //速度为0时让它动一动

                    projectile.velocity *= 1.01f;
                }

            }

            projectile.rotation = projectile.velocity.X * 0.05f;
            projectile.frameCounter++;

            int _2 = 2;
            if (projectile.frameCounter >= 6 * _2)
                projectile.frameCounter = 0;

            projectile.frame = projectile.frameCounter / _2;
            //生成粒子，可忽略
            if (Main.rand.NextBool(5))
            {
                int num42 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.FishronWings, 0f, 0f, 100, default(Color), 2f);
                Main.dust[num42].velocity *= 0.3f;
                Main.dust[num42].noGravity = true;
                Main.dust[num42].noLight = true;
            }

            if (projectile.velocity.X > 0f)
                projectile.spriteDirection = (projectile.direction = -1);
            else if (projectile.velocity.X < 0f)
                projectile.spriteDirection = (projectile.direction = 1);



            //生成弹幕部分
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += 1f;
                if (!Main.rand.NextBool(3))
                    projectile.ai[1] += 1f;
            }

            if (projectile.ai[1] > 50f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }


            if (projectile.ai[0] != 0f)
                return;

            //float num44 = 0f;
            //int TypeOfShark = 0;

            //num44 = 20f;
            //TypeOfShark = 408;

            if (!HasTarget)
                return;

            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                return;

            // if (projectile.ai[1] == 0f)
            //{
            //    Vector2 v4 = Position - projectile.Center;
            //    projectile.ai[1] += 1f;
            //    if (Main.myPlayer == projectile.owner)
            //    {
            //        v4 = v4.SafeNormalize(Vector2.Zero);
            //        v4 *= num44;
            //        int num50 = Projectile.NewProjectile(GetProjectileSource_FromThis(), projectile.Center.X, projectile.Center.Y, v4.X, v4.Y, TypeOfShark, projectile.damage, projectile.knockBack, Main.myPlayer);
            //        Main.projectile[num50].timeLeft = 300;
            //        Main.projectile[num50].netUpdate = true;
            //        projectile.netUpdate = true;
            //    }
            //}
        }
    }
}
