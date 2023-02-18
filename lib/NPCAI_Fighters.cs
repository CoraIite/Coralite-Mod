using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.lib
{
    public class NPCAI_Fighters
    {
        public void AI_003_Fighters(NPC NPC)
        {
            Player target = Main.player[NPC.target];

            if (target.Bottom.Y == NPC.Bottom.Y)
                NPC.directionY = -1;

            bool flag5 = false;
            bool notMoving = false;
            if (NPC.velocity.X == 0f)
                notMoving = true;

            if (NPC.justHit)
                notMoving = false;

            int ai3Count = 60;

            //是否向后走
            bool moveBack = false;

            if (NPC.velocity.Y == 0f && ((NPC.velocity.X > 0f && NPC.direction < 0) || (NPC.velocity.X < 0f && NPC.direction > 0)))
                moveBack = true;

            if (NPC.position.X == NPC.oldPosition.X || NPC.ai[3] >= ai3Count || moveBack)
                NPC.ai[3] += 1f;
            else if (Math.Abs(NPC.velocity.X) > 0.9f && NPC.ai[3] > 0f)
                NPC.ai[3] -= 1f;

            if (NPC.ai[3] > (ai3Count * 10))
                NPC.ai[3] = 0f;

            if (NPC.justHit)
                NPC.ai[3] = 0f;

            if (NPC.ai[3] == ai3Count)
                NPC.netUpdate = true;

            if (target.Hitbox.Intersects(NPC.Hitbox))
                NPC.ai[3] = 0f;

            if (NPC.ai[3] < ai3Count && DespawnEncouragement_AIStyle3_Fighters_NotDiscouraged(NPC.position, NPC))
            {
                NPC.TargetClosest();
                if (NPC.directionY > 0 && target.Center.Y <= NPC.Bottom.Y)
                    NPC.directionY = -1;
            }
            else//NPC消失时执行的？
            {
                if (Main.dayTime && (double)(NPC.position.Y / 16f) < Main.worldSurface)
                    NPC.EncourageDespawn(10);

                if (NPC.velocity.X == 0f && NPC.velocity.Y == 0f)
                {
                    NPC.ai[0] += 1f;
                    if (NPC.ai[0] >= 2f)
                    {
                        NPC.direction *= -1;
                        NPC.spriteDirection = NPC.direction;
                        NPC.ai[0] = 0f;
                    }
                }
                else
                    NPC.ai[0] = 0f;

                if (NPC.direction == 0)
                    NPC.direction = 1;
            }

            if (NPC.ai[1] > 0f)
                NPC.ai[1] -= 1f;

            if (NPC.justHit)
            {
                NPC.ai[1] = 30f;
                NPC.ai[2] = 0f;
            }

            int ShootCount = 30;
            int ShouldShootTimer = ShootCount / 2;

            if (NPC.confused)
                NPC.ai[2] = 0f;

            if (NPC.ai[2] > 0f)
            {
                NPC.TargetClosest();

                if (NPC.ai[1] == ShouldShootTimer)
                {
                    float num142 = 9f;
                    Vector2 NPCCenterTopPlus10 = NPC.Center;
                    NPCCenterTopPlus10.Y -= 10f;

                    float Distance2PlayerX = target.Center.X - NPCCenterTopPlus10.X;
                    float num144 = Math.Abs(Distance2PlayerX) * 0.1f;
                    float Distance2PlayerY = target.Center.Y - NPCCenterTopPlus10.Y - num144;
                    float Distance2Player = (float)Math.Sqrt(Distance2PlayerX * Distance2PlayerX + Distance2PlayerY * Distance2PlayerY);

                    NPC.netUpdate = true;

                    Distance2Player = num142 / Distance2Player;
                    Distance2PlayerX *= Distance2Player;
                    Distance2PlayerY *= Distance2Player;

                    NPCCenterTopPlus10.X += Distance2PlayerX;
                    NPCCenterTopPlus10.Y += Distance2PlayerY;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        //生成圣骑士锤子弹幕
                        //int HammerProjType = 300;
                        //int NPCShootProjDamage = 60;
                        //NPCShootProjDamage = NPC.GetAttackDamage_ForProjectiles(NPCShootProjDamage, NPCShootProjDamage * 0.75f);
                        //Projectile.NewProjectile(GetSpawnSource_ForProjectile(), vector22.X, vector22.Y, num143, num145, num148, num147, 0f, Main.myPlayer);
                    }

                    if (Math.Abs(Distance2PlayerY) > Math.Abs(Distance2PlayerX) * 2f)
                    {
                        if (Distance2PlayerY > 0f)
                            NPC.ai[2] = 1f;
                        else
                            NPC.ai[2] = 5f;
                    }
                    else if (Math.Abs(Distance2PlayerX) > Math.Abs(Distance2PlayerY) * 2f)
                        NPC.ai[2] = 3f;
                    else if (Distance2PlayerY > 0f)
                        NPC.ai[2] = 2f;
                    else
                        NPC.ai[2] = 4f;
                }

                if (NPC.velocity.Y != 0f || NPC.ai[1] <= 0f)
                {
                    NPC.ai[2] = 0f;
                    NPC.ai[1] = 0f;
                }
                else
                {
                    NPC.velocity.X *= 0.9f;
                    NPC.spriteDirection = NPC.direction;
                }
            }

            if (NPC.ai[2] <= 0f && NPC.velocity.Y == 0f && NPC.ai[1] <= 0f && !target.dead)
            {
                bool CanHitPlayer = Collision.CanHit(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);

                if (target.stealth == 0f && target.itemAnimation == 0)
                    CanHitPlayer = false;

                if (CanHitPlayer)
                {
                    float num152 = 10f;
                    Vector2 NPCCenter = NPC.Center;
                    float Distance2PlayerX = target.Center.X - NPCCenter.X;
                    float num154 = Math.Abs(Distance2PlayerX) * 0.1f;
                    float Distance2PlayerY = target.Center.Y - NPCCenter.Y - num154;
                    //随机加距离
                    Distance2PlayerX += Main.rand.Next(-40, 41);
                    Distance2PlayerY += Main.rand.Next(-40, 41);
                    float Distance2Player = (float)Math.Sqrt(Distance2PlayerX * Distance2PlayerX + Distance2PlayerY * Distance2PlayerY);
                    float ShootDistance = 700f;

                    if (Distance2Player < ShootDistance)
                    {
                        NPC.netUpdate = true;
                        NPC.velocity.X *= 0.5f;
                        Distance2Player = num152 / Distance2Player;
                        Distance2PlayerX *= Distance2Player;
                        Distance2PlayerY *= Distance2Player;
                        NPC.ai[2] = 3f;
                        NPC.ai[1] = ShootCount;

                        if (Math.Abs(Distance2PlayerY) > Math.Abs(Distance2PlayerX) * 2f)
                        {
                            if (Distance2PlayerY > 0f)
                                NPC.ai[2] = 1f;
                            else
                                NPC.ai[2] = 5f;
                        }
                        else if (Math.Abs(Distance2PlayerX) > Math.Abs(Distance2PlayerY) * 2f)
                            NPC.ai[2] = 3f;
                        else if (Distance2PlayerY > 0f)
                            NPC.ai[2] = 2f;
                        else
                            NPC.ai[2] = 4f;
                    }
                }
            }

            if (NPC.ai[2] <= 0f)
            {
                float VelocityLimitX = 1f;
                float accelX = 0.07f;
                float SlowDownAccel = 0.8f;

                if (Math.Abs(NPC.velocity.X) > VelocityLimitX)
                {
                    if (NPC.velocity.Y == 0f)
                        NPC.velocity *= SlowDownAccel;
                }
                //控制X方向的移动，大概就是起步加速。
                else if (Math.Sign(NPC.velocity.X) == NPC.direction)
                {
                    NPC.velocity.X += NPC.direction * accelX;
                    if (Math.Abs(NPC.velocity.X) > VelocityLimitX)
                        NPC.velocity.X = NPC.direction * VelocityLimitX;
                }
            }

            if (NPC.velocity.Y == 0f)
            {
                int num164 = (int)(NPC.Bottom.Y + 7f) / 16;
                int num165 = (int)(NPC.Top.Y - 9f) / 16;
                int num166 = (int)NPC.position.X / 16;
                int num167 = (int)(NPC.Right.X) / 16;
                int num168 = (int)(NPC.position.X + 8f) / 16;
                int num169 = (int)(NPC.Right.X - 8f) / 16;
                bool flag20 = false;
                for (int num170 = num168; num170 <= num169; num170++)
                {
                    if (num170 >= num166 && num170 <= num167 && Main.tile[num170, num164] == null)
                    {
                        flag20 = true;
                        continue;
                    }

                    if (Main.tile[num170, num165] != null && Main.tile[num170, num165].HasUnactuatedTile && Main.tileSolid[Main.tile[num170, num165].TileType])
                    {
                        flag5 = false;
                        break;
                    }

                    if (!flag20 && num170 >= num166 && num170 <= num167 && Main.tile[num170, num164].HasUnactuatedTile && Main.tileSolid[Main.tile[num170, num164].TileType])
                        flag5 = true;
                }

                if (!flag5 && NPC.velocity.Y < 0f)
                    NPC.velocity.Y = 0f;

                if (flag20)
                    return;
            }

            if (NPC.velocity.Y >= 0f && NPC.directionY != 1)
            {
                int VelocityXDir = Math.Sign(NPC.velocity.X);

                Vector2 position3 = NPC.position;
                position3.X += NPC.velocity.X;
                int num172 = (int)((position3.X + (NPC.width / 2) + ((NPC.width / 2 + 1) * VelocityXDir)) / 16f);
                int num173 = (int)((position3.Y + NPC.height - 1f) / 16f);
                if (WorldGen.InWorld(num172, num173, 4))
                {
                    if ((num172 * 16) < position3.X + NPC.width && (num172 * 16 + 16) > position3.X && ((Main.tile[num172, num173].HasUnactuatedTile && !Main.tile[num172, num173].topSlope() && !Main.tile[num172, num173 - 1].topSlope() && Main.tileSolid[Main.tile[num172, num173].TileType] && !Main.tileSolidTop[Main.tile[num172, num173].TileType]) || (Main.tile[num172, num173 - 1].IsHalfBlock && Main.tile[num172, num173 - 1].HasUnactuatedTile)) && (!Main.tile[num172, num173 - 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[num172, num173 - 1].TileType] || Main.tileSolidTop[Main.tile[num172, num173 - 1].TileType] || (Main.tile[num172, num173 - 1].IsHalfBlock && (!Main.tile[num172, num173 - 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[num172, num173 - 4].TileType] || Main.tileSolidTop[Main.tile[num172, num173 - 4].TileType]))) && (!Main.tile[num172, num173 - 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[num172, num173 - 2].TileType] || Main.tileSolidTop[Main.tile[num172, num173 - 2].TileType]) && (!Main.tile[num172, num173 - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[num172, num173 - 3].TileType] || Main.tileSolidTop[Main.tile[num172, num173 - 3].TileType]) && (!Main.tile[num172 - VelocityXDir, num173 - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[num172 - VelocityXDir, num173 - 3].TileType]))
                    {
                        float num174 = num173 * 16;
                        if (Main.tile[num172, num173].IsHalfBlock)
                            num174 += 8f;

                        if (Main.tile[num172, num173 - 1].IsHalfBlock)
                            num174 -= 8f;

                        if (num174 < position3.Y + NPC.height)
                        {
                            float num175 = position3.Y + NPC.height - num174;
                            float num176 = 16.1f;

                            if (num175 <= num176)
                            {
                                NPC.gfxOffY += NPC.position.Y + NPC.height - num174;
                                NPC.position.Y = num174 - NPC.height;
                                if (num175 < 9f)
                                    NPC.stepSpeed = 1f;
                                else
                                    NPC.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }

            if (flag5)
            {
                int num177 = (int)((NPC.position.X + (NPC.width / 2) + ((NPC.width / 2 + 16) * NPC.direction)) / 16f);
                int num178 = (int)((NPC.position.Y + NPC.height - 15f) / 16f);

                #region 目测是用于判断NPC是否能开门的，但用不到
                //if (Main.tile[num177, num178 - 1].HasUnactuatedTile && (TileLoader.IsClosedDoor(Main.tile[num177, num178 - 1]) || Main.tile[num177, num178 - 1].TileType == 388) && false)
                //{
                //    NPC.ai[2] += 1f;
                //    NPC.ai[3] = 0f;
                //    if (NPC.ai[2] >= 60f)
                //    {
                //        bool flag21 = false;
                //        bool flag22 = target.ZoneGraveyard && Main.rand.NextBool(60);
                //        if ((!Main.bloodMoon || Main.getGoodWorld) && !flag22 && flag21)
                //            NPC.ai[1] = 0f;

                //        NPC.velocity.X = 0.5f * -NPC.direction;
                //        int num179 = 5;
                //        if (Main.tile[num177, num178 - 1].TileType == 388)
                //            num179 = 2;

                //        NPC.ai[1] += num179;
                //        NPC.ai[2] = 0f;
                //        bool flag23 = false;
                //        if (NPC.ai[1] >= 10f)
                //        {
                //            flag23 = true;
                //            NPC.ai[1] = 10f;
                //        }

                //        WorldGen.KillTile(num177, num178 - 1, fail: true);
                //        if ((Main.netMode != NetmodeID.MultiplayerClient || !flag23) && flag23 && Main.netMode != NetmodeID.MultiplayerClient)
                //        {
                //            if (TileLoader.OpenDoorID(Main.tile[num177, num178 - 1]) >= 0)
                //            {
                //                bool flag24 = WorldGen.OpenDoor(num177, num178 - 1, NPC.direction);
                //                if (!flag24)
                //                {
                //                    NPC.ai[3] = num52;
                //                    NPC.netUpdate = true;
                //                }

                //                if (Main.netMode == NetmodeID.Server && flag24)
                //                    NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, num177, num178 - 1, NPC.direction);
                //            }

                //            if (Main.tile[num177, num178 - 1].TileType == 388)
                //            {
                //                bool flag25 = WorldGen.ShiftTallGate(num177, num178 - 1, closing: false);
                //                if (!flag25)
                //                {
                //                    NPC.ai[3] = num52;
                //                    NPC.netUpdate = true;
                //                }

                //                if (Main.netMode == NetmodeID.Server && flag25)
                //                    NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, num177, num178 - 1);
                //            }
                //        }
                //    }
                //}
                //else
                //{
                #endregion
                int SpriteDirection = NPC.spriteDirection;

                if ((NPC.velocity.X < 0f && SpriteDirection == -1) || (NPC.velocity.X > 0f && SpriteDirection == 1))
                {
                    if (NPC.height >= 32 && Main.tile[num177, num178 - 2].HasUnactuatedTile && Main.tileSolid[Main.tile[num177, num178 - 2].TileType])
                    {
                        if (Main.tile[num177, num178 - 3].HasUnactuatedTile && Main.tileSolid[Main.tile[num177, num178 - 3].TileType])
                        {
                            NPC.velocity.Y = -8f;
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.velocity.Y = -7f;
                            NPC.netUpdate = true;
                        }
                    }
                    else if (Main.tile[num177, num178 - 1].HasUnactuatedTile && Main.tileSolid[Main.tile[num177, num178 - 1].TileType])
                    {
                        NPC.velocity.Y = -6f;
                        NPC.netUpdate = true;
                    }
                    else if (NPC.position.Y + NPC.height - (num178 * 16) > 20f && Main.tile[num177, num178].HasUnactuatedTile && !Main.tile[num177, num178].topSlope() && Main.tileSolid[Main.tile[num177, num178].TileType])
                    {
                        NPC.velocity.Y = -5f;
                        NPC.netUpdate = true;
                    }
                    else if (NPC.directionY < 0 && (!Main.tile[num177, num178 + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[num177, num178 + 1].TileType]) && (!Main.tile[num177 + NPC.direction, num178 + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[num177 + NPC.direction, num178 + 1].TileType]))
                    {
                        NPC.velocity.Y = -8f;
                        NPC.velocity.X *= 1.5f;
                        NPC.netUpdate = true;
                    }

                    if (NPC.velocity.Y == 0f && notMoving && NPC.ai[3] == 1f)
                        NPC.velocity.Y = -5f;

                    if (NPC.velocity.Y == 0f && Main.expertMode && target.Bottom.Y < NPC.Top.Y && Math.Abs(NPC.Center.X - target.Center.X) < (Main.player[NPC.target].width * 3) && Collision.CanHit(NPC, Main.player[NPC.target]))
                    {
                        if (NPC.velocity.Y == 0f)
                        {
                            int num183 = 6;
                            if (target.Bottom.Y > NPC.Top.Y - (num183 * 16))
                                NPC.velocity.Y = -7.9f;
                            else
                            {
                                int num184 = (int)(NPC.Center.X / 16f);
                                int num185 = (int)(NPC.Bottom.Y / 16f) - 1;
                                for (int num186 = num185; num186 > num185 - num183; num186--)
                                    if (Main.tile[num184, num186].HasUnactuatedTile && TileID.Sets.Platforms[Main.tile[num184, num186].TileType])
                                        NPC.velocity.Y = -7.9f;

                            }
                        }
                    }
                }
            }

            #region 以下内容只有某个NPC才会执行到，所以忽略不看了
            //if (Main.netMode == NetmodeID.MultiplayerClient || true || !(NPC.ai[3] >= (float)num52))
            //    return;

            //int num189 = (int)Main.player[target].position.X / 16;
            //int num190 = (int)Main.player[target].position.Y / 16;
            //int num191 = (int)base.position.X / 16;
            //int num192 = (int)base.position.Y / 16;
            //int num193 = 20;
            //int num194 = 0;
            //bool flag26 = false;
            //if (Math.Abs(base.position.X - Main.player[target].position.X) + Math.Abs(base.position.Y - Main.player[target].position.Y) > 2000f)
            //{
            //    num194 = 100;
            //    flag26 = true;
            //}

            //while (!flag26 && num194 < 100)
            //{
            //    num194++;
            //    int num195 = Main.rand.Next(num189 - num193, num189 + num193);
            //    for (int num196 = Main.rand.Next(num190 - num193, num190 + num193); num196 < num190 + num193; num196++)
            //    {
            //        if ((num196 < num190 - 4 || num196 > num190 + 4 || num195 < num189 - 4 || num195 > num189 + 4) && (num196 < num192 - 1 || num196 > num192 + 1 || num195 < num191 - 1 || num195 > num191 + 1) && Main.tile[num195, num196].nactive())
            //        {
            //            bool flag27 = true;
            //            if (type == 32 && Main.tile[num195, num196 - 1].wall == 0)
            //                flag27 = false;
            //            else if (Main.tile[num195, num196 - 1].lava())
            //                flag27 = false;

            //            if (flag27 && Main.tileSolid[Main.tile[num195, num196].type] && !Collision.SolidTiles(num195 - 1, num195 + 1, num196 - 4, num196 - 1))
            //            {
            //                base.position.X = num195 * 16 - width / 2;
            //                base.position.Y = num196 * 16 - height;
            //                netUpdate = true;
            //                ai[3] = -120f;
            //            }
            //        }
            //    }
            //}
            #endregion
        }

        public static bool DespawnEncouragement_AIStyle3_Fighters_NotDiscouraged(Vector2 position, NPC npcInstance)
        {
            if (!Main.eclipse && Main.dayTime && !npcInstance.SpawnedFromStatue && !(position.Y > Main.worldSurface * 16.0) && (!Main.player[npcInstance.target].ZoneGraveyard))
            {
                return false;
            }

            return true;
        }
    }
}
