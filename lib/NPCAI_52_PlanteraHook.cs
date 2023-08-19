using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.lib
{
    public class NPCAI_52_PlanteraHook
    {
        public NPC NPC;

        public void AI()
        {
            bool flag42 = false;
            bool flag43 = false;
            if (NPC.plantBoss < 0)
            {
                NPC.StrikeNPC(NPC.CalculateHitInfo(9999, 0));
                NPC.netUpdate = true;
                return;
            }

            if (Main.player[Main.npc[NPC.plantBoss].target].dead)
                flag43 = true;

            if ((NPC.plantBoss != -1 && !Main.player[Main.npc[NPC.plantBoss].target].ZoneJungle) || Main.player[Main.npc[NPC.plantBoss].target].position.Y < Main.worldSurface * 16.0 || Main.player[Main.npc[NPC.plantBoss].target].position.Y > Main.UnderworldLayer * 16 || flag43)
            {
                NPC.localAI[0] -= 4f;
                flag42 = true;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                    NPC.ai[0] = (int)(NPC.Center.X / 16f);

                if (NPC.ai[1] == 0f)
                    NPC.ai[1] = (int)(NPC.Center.X / 16f);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f || NPC.ai[1] == 0f)
                    NPC.localAI[0] = 0f;

                NPC.localAI[0] -= 1f;
                if (Main.npc[NPC.plantBoss].life < Main.npc[NPC.plantBoss].lifeMax / 2)
                    NPC.localAI[0] -= 2f;

                if (Main.npc[NPC.plantBoss].life < Main.npc[NPC.plantBoss].lifeMax / 4)
                    NPC.localAI[0] -= 2f;

                if (flag42)
                    NPC.localAI[0] -= 6f;

                if (!flag43 && NPC.localAI[0] <= 0f && NPC.ai[0] != 0f)
                {
                    for (int num811 = 0; num811 < 200; num811++)
                    {
                        if (num811 != NPC.whoAmI && Main.npc[num811].active && Main.npc[num811].type == NPC.type && (Main.npc[num811].velocity.X != 0f || Main.npc[num811].velocity.Y != 0f))
                            NPC.localAI[0] = Main.rand.Next(60, 300);
                    }
                }

                if (NPC.localAI[0] <= 0f)
                {
                    NPC.localAI[0] = Main.rand.Next(300, 600);
                    bool flag44 = false;
                    int num812 = 0;
                    while (!flag44 && num812 <= 1000)
                    {
                        num812++;
                        int num813 = (int)(Main.player[Main.npc[NPC.plantBoss].target].Center.X / 16f);
                        int num814 = (int)(Main.player[Main.npc[NPC.plantBoss].target].Center.Y / 16f);
                        if (NPC.ai[0] == 0f)
                        {
                            num813 = (int)((Main.player[Main.npc[NPC.plantBoss].target].Center.X + Main.npc[NPC.plantBoss].Center.X) / 32f);
                            num814 = (int)((Main.player[Main.npc[NPC.plantBoss].target].Center.Y + Main.npc[NPC.plantBoss].Center.Y) / 32f);
                        }

                        if (flag43)
                        {
                            num813 = (int)Main.npc[NPC.plantBoss].position.X / 16;
                            num814 = (int)(Main.npc[NPC.plantBoss].position.Y + 400f) / 16;
                        }

                        int num815 = 20;
                        num815 += (int)(100f * (num812 / 1000f));
                        int num816 = num813 + Main.rand.Next(-num815, num815 + 1);
                        int num817 = num814 + Main.rand.Next(-num815, num815 + 1);
                        if (Main.npc[NPC.plantBoss].life < Main.npc[NPC.plantBoss].lifeMax / 2 && Main.rand.NextBool(6))
                        {
                            NPC.TargetClosest();
                            int num818 = (int)(Main.player[NPC.target].Center.X / 16f);
                            int num819 = (int)(Main.player[NPC.target].Center.Y / 16f);
                            if (Main.tile[num818, num819].WallType > 0)
                            {
                                num816 = num818;
                                num817 = num819;
                            }
                        }

                        try
                        {
                            if (WorldGen.InWorld(num816, num817) && (WorldGen.SolidTile(num816, num817) || (Main.tile[num816, num817].WallType > 0 && (num812 > 500 || Main.npc[NPC.plantBoss].life < Main.npc[NPC.plantBoss].lifeMax / 2))))
                            {
                                flag44 = true;
                                NPC.ai[0] = num816;
                                NPC.ai[1] = num817;
                                NPC.netUpdate = true;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            if (!(NPC.ai[0] > 0f) || !(NPC.ai[1] > 0f))
                return;

            float num820 = 6f;
            if (Main.npc[NPC.plantBoss].life < Main.npc[NPC.plantBoss].lifeMax / 2)
                num820 = 8f;

            if (Main.npc[NPC.plantBoss].life < Main.npc[NPC.plantBoss].lifeMax / 4)
                num820 = 10f;

            if (Main.expertMode)
                num820 += 1f;

            if (Main.expertMode && Main.npc[NPC.plantBoss].life < Main.npc[NPC.plantBoss].lifeMax / 2)
                num820 += 1f;

            if (flag42)
                num820 *= 2f;

            if (flag43)
                num820 *= 2f;

            Vector2 vector101 = new Vector2(NPC.Center.X, NPC.Center.Y);
            float num821 = NPC.ai[0] * 16f - 8f - vector101.X;
            float num822 = NPC.ai[1] * 16f - 8f - vector101.Y;
            float num823 = (float)Math.Sqrt(num821 * num821 + num822 * num822);
            if (num823 < 12f + num820)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld && NPC.localAI[3] == 1f)
                {
                    NPC.localAI[3] = 0f;
                    WorldGen.SpawnPlanteraThorns(NPC.Center);
                }

                NPC.velocity.X = num821;
                NPC.velocity.Y = num822;
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld)
                    NPC.localAI[3] = 1f;

                num823 = num820 / num823;
                NPC.velocity.X = num821 * num823;
                NPC.velocity.Y = num822 * num823;
            }

            Vector2 vector102 = new Vector2(NPC.Center.X, NPC.Center.Y);
            float num824 = Main.npc[NPC.plantBoss].Center.X - vector102.X;
            float num825 = Main.npc[NPC.plantBoss].Center.Y - vector102.Y;
            NPC.rotation = (float)Math.Atan2(num825, num824) - 1.57f;
        }

    }
}
