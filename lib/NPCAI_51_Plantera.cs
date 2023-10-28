using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.lib
{
    public class NPCAI_51_Plantera
    {
        public NPC NPC;

        public Player Target => Main.player[NPC.target];
        public ref float SpawnedHook => ref NPC.localAI[0];
        public ref float Timer => ref NPC.localAI[1];

        public void AI()
        {
            bool flag39 = false;
            bool flag40 = false;
            NPC.TargetClosest();
            if (Target.dead)
            {
                flag40 = true;
                flag39 = true;
            }

            else if (Main.netMode != NetmodeID.MultiplayerClient && NPC.target >= 0 && NPC.target < 255)
            {
                int maxDistance = 4800;
                if (NPC.timeLeft < NPC.activeTime && Vector2.Distance(NPC.Center, Target.Center) < maxDistance)
                    NPC.timeLeft = NPC.activeTime;
            }

            NPC.plantBoss = NPC.whoAmI;

            #region 生成爪子
            if (SpawnedHook == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                SpawnedHook = 1f;
                for (int i = 0; i < 3; i++)
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PlanterasHook, NPC.whoAmI);
            }
            #endregion

            float targetX = 0f;
            float targetY = 0f;
            int hookWhoAmI = 0;
            for (int i = 0; i < 200; i++)
                if (Main.npc[i].active && Main.npc[i].aiStyle == NPCAIStyleID.PlanteraHook)
                {
                    targetX += Main.npc[i].Center.X;
                    targetY += Main.npc[i].Center.Y;
                    hookWhoAmI++;
                    if (hookWhoAmI > 2)
                        break;
                }

            targetX /= hookWhoAmI;
            targetY /= hookWhoAmI;

            #region 移动
            float num779 = 2.5f;
            float speed = 0.025f;
            if (NPC.life < NPC.lifeMax / 2)
            {
                num779 = 5f;
                speed = 0.05f;
            }

            if (NPC.life < NPC.lifeMax / 4)
                num779 = 7f;

            if (!Target.ZoneJungle || Target.position.Y < Main.worldSurface * 16.0 || Target.position.Y > Main.UnderworldLayer * 16)
            {
                flag39 = true;
                num779 += 8f;
                speed = 0.15f;
            }

            if (Main.expertMode)
            {
                num779 += 1f;
                num779 *= 1.1f;
                speed += 0.01f;
                speed *= 1.1f;
            }

            if (Main.getGoodWorld)
            {
                num779 *= 1.15f;
                speed *= 1.15f;
            }

            //由3个爪子共同决定的目标点
            Vector2 targetPos = new Vector2(targetX, targetY);
            float num781 = Target.Center.X - targetPos.X;
            float num782 = Target.Center.Y - targetPos.Y;
            if (flag40) //玩家死了就远离
            {
                num782 *= -1f;
                num781 *= -1f;
                num779 += 8f;
            }

            float posToTargetLength = (float)Math.Sqrt(num781 * num781 + num782 * num782);
            int maxLength = 500;    //大概是最大延申距离
            if (flag39)
                maxLength += 350;

            if (Main.expertMode)
                maxLength += 150;

            if (posToTargetLength >= maxLength)
            {
                posToTargetLength = maxLength / posToTargetLength;
                num781 *= posToTargetLength;
                num782 *= posToTargetLength;
            }

            targetX += num781;
            targetY += num782;
            targetPos = new Vector2(NPC.Center.X, NPC.Center.Y);
            num781 = targetX - targetPos.X;
            num782 = targetY - targetPos.Y;
            posToTargetLength = (float)Math.Sqrt(num781 * num781 + num782 * num782);
            if (posToTargetLength < num779)
            {
                num781 = NPC.velocity.X;
                num782 = NPC.velocity.Y;
            }
            else
            {
                posToTargetLength = num779 / posToTargetLength;
                num781 *= posToTargetLength;
                num782 *= posToTargetLength;
            }

            if (NPC.velocity.X < num781)
            {
                NPC.velocity.X += speed;
                if (NPC.velocity.X < 0f && num781 > 0f)
                    NPC.velocity.X += speed * 2f;
            }
            else if (NPC.velocity.X > num781)
            {
                NPC.velocity.X -= speed;
                if (NPC.velocity.X > 0f && num781 < 0f)
                    NPC.velocity.X -= speed * 2f;
            }

            if (NPC.velocity.Y < num782)
            {
                NPC.velocity.Y += speed;
                if (NPC.velocity.Y < 0f && num782 > 0f)
                    NPC.velocity.Y += speed * 2f;
            }
            else if (NPC.velocity.Y > num782)
            {
                NPC.velocity.Y -= speed;
                if (NPC.velocity.Y > 0f && num782 < 0f)
                    NPC.velocity.Y -= speed * 2f;
            }

            #endregion

            NPC.rotation = (float)Math.Atan2(Target.Center.Y - NPC.Center.Y, Target.Center.X - NPC.Center.X) + MathHelper.PiOver2;
            if (NPC.life > NPC.lifeMax / 2) //一阶段
            {
                NPC.defense = 36;
                int damage = 50;
                if (flag39)
                {
                    NPC.defense *= 2;
                    damage *= 2;
                }

                NPC.damage = NPC.GetAttackDamage_ScaledByStrength(damage);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    return;

                Timer += 1f;
                if (NPC.life < NPC.lifeMax * 0.9)
                    Timer += 1f;

                if (NPC.life < NPC.lifeMax * 0.8)
                    Timer += 1f;

                if (NPC.life < NPC.lifeMax * 0.7)
                    Timer += 1f;

                if (NPC.life < NPC.lifeMax * 0.6)
                    Timer += 1f;

                if (flag39)
                    Timer += 3f;

                if (Main.expertMode)
                    Timer += 1f;

                if (Main.expertMode && NPC.justHit && Main.rand.NextBool(2))
                    NPC.localAI[3] = 1f;

                if (Main.getGoodWorld)
                    Timer += 1f;

                if (!(Timer > 80f))
                    return;

                Timer = 0f;
                bool canHit = Collision.CanHit(NPC.position, NPC.width, NPC.height, Target.position, Target.width, Target.height);
                if (NPC.localAI[3] > 0f)
                {
                    canHit = true;
                    NPC.localAI[3] = 0f;
                }

                if (canHit)
                {
                    Vector2 vector99 = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float num788 = 15f;
                    if (Main.expertMode)
                        num788 = 17f;

                    float num789 = Target.position.X + Target.width * 0.5f - vector99.X;
                    float num790 = Target.position.Y + Target.height * 0.5f - vector99.Y;
                    float num791 = (float)Math.Sqrt(num789 * num789 + num790 * num790);
                    num791 = num788 / num791;
                    num789 *= num791;
                    num790 *= num791;
                    int num792 = 22;
                    int num793 = 275;
                    int maxValue2 = 4;
                    int maxValue3 = 8;
                    if (Main.expertMode)
                    {
                        maxValue2 = 2;
                        maxValue3 = 6;
                    }

                    if (NPC.life < NPC.lifeMax * 0.8 && Main.rand.NextBool(maxValue2))
                    {
                        num792 = 27;
                        Timer = -30f;
                        num793 = 276;
                    }
                    else if (NPC.life < NPC.lifeMax * 0.8 && Main.rand.NextBool(maxValue3))
                    {
                        num792 = 31;
                        Timer = -120f;
                        num793 = 277;
                    }

                    if (flag39)
                        num792 *= 2;

                    num792 = NPC.GetAttackDamage_ForProjectiles(num792, num792 * 0.9f);
                    vector99.X += num789 * 3f;
                    vector99.Y += num790 * 3f;
                    int num794 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector99.X, vector99.Y, num789, num790, num793, num792, 0f, Main.myPlayer);
                    if (num793 != 277)
                        Main.projectile[num794].timeLeft = 300;
                }

                return;
            }

            NPC.defense = 10;
            int num795 = 70;
            if (flag39)
            {
                NPC.defense *= 4;
                num795 *= 2;
            }

            NPC.damage = NPC.GetAttackDamage_ScaledByStrength(num795);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (SpawnedHook == 1f)  //如果生成过钩子那么就生成小触手
                {
                    SpawnedHook = 2f;
                    int hoaMany = 8;
                    if (Main.getGoodWorld)
                        hoaMany += 6;

                    for (int i = 0; i < hoaMany; i++)
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PlanterasTentacle, NPC.whoAmI);

                    if (Main.expertMode)    //专家模式中的二阶段在爪子上生成小触手
                        for (int i = 0; i < 200; i++)
                            if (Main.npc[i].active && Main.npc[i].aiStyle == NPCAIStyleID.PlanteraHook)
                                for (int j = 0; j < hoaMany / 2 - 1; j++)
                                {
                                    int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PlanterasTentacle, NPC.whoAmI);
                                    Main.npc[index].ai[3] = i + 1;
                                }
                }
                else if (Main.expertMode && Main.rand.NextBool(60))
                {
                    int num802 = 0;
                    for (int num803 = 0; num803 < 200; num803++)
                        if (Main.npc[num803].active && Main.npc[num803].type == NPCID.PlanterasTentacle && Main.npc[num803].ai[3] == 0f)
                            num802++;

                    if (num802 < 8 && Main.rand.Next((num802 + 1) * 10) <= 1)
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, 264, NPC.whoAmI);
                }
            }

            if (NPC.localAI[2] == 0f)
            {
                Gore.NewGore(NPC.GetSource_FromAI(), new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), NPC.velocity, 378, NPC.scale);
                Gore.NewGore(NPC.GetSource_FromAI(), new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), NPC.velocity, 379, NPC.scale);
                Gore.NewGore(NPC.GetSource_FromAI(), new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height)), NPC.velocity, 380, NPC.scale);
                NPC.localAI[2] = 1f;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Timer += 1f;
            if (NPC.life < NPC.lifeMax * 0.4)
                Timer += 1f;

            if (NPC.life < NPC.lifeMax * 0.3)
                Timer += 1f;

            if (NPC.life < NPC.lifeMax * 0.2)
                Timer += 1f;

            if (NPC.life < NPC.lifeMax * 0.1)
                Timer += 1f;

            if (Timer >= 350f)
            {
                float num805 = 8f;
                Vector2 vector100 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
                float num806 = Target.position.X + Target.width * 0.5f - vector100.X + Main.rand.Next(-10, 11);
                float num807 = Math.Abs(num806 * 0.2f);
                float num808 = Target.position.Y + Target.height * 0.5f - vector100.Y + Main.rand.Next(-10, 11);
                if (num808 > 0f)
                    num807 = 0f;

                num808 -= num807;
                float num809 = (float)Math.Sqrt(num806 * num806 + num808 * num808);
                num809 = num805 / num809;
                num806 *= num809;
                num808 *= num809;
                int num810 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, 265);
                Main.npc[num810].velocity.X = num806;
                Main.npc[num810].velocity.Y = num808;
                Main.npc[num810].netUpdate = true;
                Timer = 0f;
            }
        }
    }
}
