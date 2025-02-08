using Coralite.Content.Items.DigDigDig.EyeOfGlistent;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Bosses.DigDigDig.EyeOfGlistent
{
    [AutoloadBossHead]
    public class EyeOfGlistent : ModNPC
    {
        public override string Texture => AssetDirectory.DigDigDigBoss + "EyeOfGlistent";

        public Player Target => Main.player[NPC.target];

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);


        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 110;
            NPC.damage = 15;
            NPC.defense = 12;
            NPC.lifeMax = 3800;
            NPC.HitSound = SoundID.DD2_CrystalCartImpact;
            NPC.DeathSound = CoraliteSoundID.StoneBurst_Item70;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.value = 30000f;
            NPC.npcSlots = 5f;

            //NPC.BossBar = ModContent.GetInstance<StonelimeBossBar>();
            //ModContent.GetInstance<StonelimeBossBar>().Reset(NPC);

            Music = MusicID.Boss1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.7f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<GlistentRelic>()));
        }

        public override void AI()
        {
            bool flag2 = false;
            if (Main.expertMode && NPC.life < NPC.lifeMax * 0.12f)
                flag2 = true;

            bool flag3 = false;
            if (Main.expertMode && NPC.life < NPC.lifeMax * 0.04f)
                flag3 = true;

            float num4 = 20f;
            if (flag3)
                num4 = 10f;

            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active)
                NPC.TargetClosest();

            bool dead = Target.dead;
            float num5 = NPC.position.X + (NPC.width / 2) - Target.position.X - (Target.width / 2);
            float num6 = NPC.position.Y + NPC.height - 59f - Target.position.Y - (Target.height / 2);
            float num7 = (float)Math.Atan2(num6, num5) + 1.57f;
            if (num7 < 0f)
                num7 += 6.283f;
            else if (num7 > 6.283)
                num7 -= 6.283f;

            float num8 = 0f;
            if (NPC.ai[0] == 0f && NPC.ai[1] == 0f)
                num8 = 0.02f;

            if (NPC.ai[0] == 0f && NPC.ai[1] == 2f && NPC.ai[2] > 40f)
                num8 = 0.05f;

            if (NPC.ai[0] == 3f && NPC.ai[1] == 0f)
                num8 = 0.05f;

            if (NPC.ai[0] == 3f && NPC.ai[1] == 2f && NPC.ai[2] > 40f)
                num8 = 0.08f;

            if (NPC.ai[0] == 3f && NPC.ai[1] == 4f && NPC.ai[2] > num4)
                num8 = 0.15f;

            if (NPC.ai[0] == 3f && NPC.ai[1] == 5f)
                num8 = 0.05f;

            if (Main.expertMode)
                num8 *= 1.5f;

            if (flag3 && Main.expertMode)
                num8 = 0f;

            if (NPC.rotation < num7)
            {
                if ((num7 - NPC.rotation) > 3.1415)
                    NPC.rotation -= num8;
                else
                    NPC.rotation += num8;
            }
            else if (NPC.rotation > num7)
            {
                if ((NPC.rotation - num7) > 3.1415)
                    NPC.rotation += num8;
                else
                    NPC.rotation -= num8;
            }

            if (NPC.rotation > num7 - num8 && NPC.rotation < num7 + num8)
                NPC.rotation = num7;

            if (NPC.rotation < 0f)
                NPC.rotation += 6.283f;
            else if (NPC.rotation > 6.283)
                NPC.rotation -= 6.283f;

            if (NPC.rotation > num7 - num8 && NPC.rotation < num7 + num8)
                NPC.rotation = num7;

            if (Main.rand.NextBool(5))
            {
                int num9 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + NPC.height * 0.25f), NPC.width, (int)(NPC.height * 0.5f), DustID.GreenFairy, NPC.velocity.X, 2f);
                Main.dust[num9].velocity.X *= 0.5f;
                Main.dust[num9].velocity.Y *= 0.1f;
            }

            NPC.reflectsProjectiles = false;
            if (dead)
            {
                NPC.velocity.Y -= 0.04f;
                NPC.EncourageDespawn(10);
                return;
            }

            if (NPC.ai[0] == 0f)
            {
                if (NPC.ai[1] == 0f)
                {
                    float num10 = 5f;
                    float num11 = 0.04f;
                    if (Main.expertMode)
                    {
                        num11 = 0.15f;
                        num10 = 7f;
                    }

                    if (Main.getGoodWorld)
                    {
                        num11 += 0.05f;
                        num10 += 1f;
                    }

                    Vector2 npcCenter = NPC.Center;
                    float num12 = Target.Center.X - npcCenter.X;
                    float num13 = Target.Center.Y - 200f - npcCenter.Y;
                    float num14 = (float)Math.Sqrt(num12 * num12 + num13 * num13);
                    float num15 = num14;
                    num14 = num10 / num14;
                    num12 *= num14;
                    num13 *= num14;
                    if (NPC.velocity.X < num12)
                    {
                        NPC.velocity.X += num11;
                        if (NPC.velocity.X < 0f && num12 > 0f)
                            NPC.velocity.X += num11;
                    }
                    else if (NPC.velocity.X > num12)
                    {
                        NPC.velocity.X -= num11;
                        if (NPC.velocity.X > 0f && num12 < 0f)
                            NPC.velocity.X -= num11;
                    }

                    if (NPC.velocity.Y < num13)
                    {
                        NPC.velocity.Y += num11;
                        if (NPC.velocity.Y < 0f && num13 > 0f)
                            NPC.velocity.Y += num11;
                    }
                    else if (NPC.velocity.Y > num13)
                    {
                        NPC.velocity.Y -= num11;
                        if (NPC.velocity.Y > 0f && num13 < 0f)
                            NPC.velocity.Y -= num11;
                    }

                    NPC.ai[2] += 1f;
                    float num16 = 600f;
                    if (Main.expertMode)
                        num16 *= 0.35f;

                    if (NPC.ai[2] >= num16)
                    {
                        NPC.ai[1] = 1f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.target = 255;
                        NPC.netUpdate = true;
                    }
                    else if ((NPC.position.Y + NPC.height < Target.position.Y && num15 < 500f) || (Main.expertMode && num15 < 500f))
                    {
                        if (!Target.dead)
                            NPC.ai[3] += 1f;

                        float num17 = 110f;
                        if (Main.expertMode)
                            num17 *= 0.4f;

                        if (Main.getGoodWorld)
                            num17 *= 0.8f;

                        if (NPC.ai[3] >= num17)
                        {
                            NPC.ai[3] = 0f;
                            NPC.rotation = num7;
                            float num18 = 5f;
                            if (Main.expertMode)
                                num18 = 6f;

                            float num19 = Target.Center.X - npcCenter.X;
                            float num20 = Target.Center.Y - npcCenter.Y;
                            float num21 = (float)Math.Sqrt(num19 * num19 + num20 * num20);
                            num21 = num18 / num21;
                            Vector2 vector2 = npcCenter;
                            Vector2 vector3 = default(Vector2);
                            vector3.X = num19 * num21;
                            vector3.Y = num20 * num21;
                            vector2.X += vector3.X * 10f;
                            vector2.Y += vector3.Y * 10f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)vector2.X, (int)vector2.Y, 5);
                                Main.npc[index].velocity.X = vector3.X;
                                Main.npc[index].velocity.Y = vector3.Y;
                                if (VaultUtils.isServer && index < 200)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);
                            }

                            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, vector2);

                            for (int m = 0; m < 10; m++)
                            {
                                Dust.NewDust(vector2, 20, 20, DustID.GreenFairy, vector3.X * 0.4f, vector3.Y * 0.4f);
                            }
                        }
                    }
                }
                else if (NPC.ai[1] == 1f)
                {
                    NPC.rotation = num7;
                    float num23 = 6f;
                    if (Main.expertMode)
                        num23 = 7f;

                    if (Main.getGoodWorld)
                        num23 += 1f;

                    Vector2 vector4 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    float num24 = Target.position.X + (float)(Target.width / 2) - vector4.X;
                    float num25 = Target.position.Y + (float)(Target.height / 2) - vector4.Y;
                    float num26 = (float)Math.Sqrt(num24 * num24 + num25 * num25);
                    num26 = num23 / num26;
                    NPC.velocity.X = num24 * num26;
                    NPC.velocity.Y = num25 * num26;
                    NPC.ai[1] = 2f;
                    NPC.netUpdate = true;
                    if (NPC.netSpam > 10)
                        NPC.netSpam = 10;
                }
                else if (NPC.ai[1] == 2f)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 40f)
                    {
                        NPC.velocity *= 0.98f;
                        if (Main.expertMode)
                            NPC.velocity *= 0.985f;

                        if (Main.getGoodWorld)
                            NPC.velocity *= 0.99f;

                        if (NPC.velocity.X > -0.1f && NPC.velocity.X < 0.1f)
                            NPC.velocity.X = 0f;

                        if (NPC.velocity.Y > -0.1f && NPC.velocity.Y < 0.1f)
                            NPC.velocity.Y = 0f;
                    }
                    else
                    {
                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) - 1.57f;
                    }

                    int num27 = 150;
                    if (Main.expertMode)
                        num27 = 100;

                    if (Main.getGoodWorld)
                        num27 -= 15;

                    if (NPC.ai[2] >= (float)num27)
                    {
                        NPC.ai[3] += 1f;
                        NPC.ai[2] = 0f;
                        NPC.target = 255;
                        NPC.rotation = num7;
                        if (NPC.ai[3] >= 3f)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[3] = 0f;
                        }
                        else
                        {
                            NPC.ai[1] = 1f;
                        }
                    }
                }

                float switchPhase = 0.5f;
                if (Main.expertMode)
                    switchPhase = 0.65f;

                if (NPC.life < NPC.lifeMax * switchPhase)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                    if (NPC.netSpam > 10)
                        NPC.netSpam = 10;
                }

                return;
            }

            if (NPC.ai[0] == 1f || NPC.ai[0] == 2f)
            {
                if (NPC.ai[0] == 1f || NPC.ai[3] == 1f)
                {
                    NPC.ai[2] += 0.005f;
                    if (NPC.ai[2] > 0.5)
                        NPC.ai[2] = 0.5f;
                }
                else
                {
                    NPC.ai[2] -= 0.005f;
                    if (NPC.ai[2] < 0f)
                        NPC.ai[2] = 0f;
                }

                NPC.rotation += NPC.ai[2];
                NPC.ai[1] += 1f;
                if (Main.getGoodWorld)
                    NPC.reflectsProjectiles = true;

                int num29 = 20;
                if (Main.getGoodWorld && NPC.life < NPC.lifeMax / 3)
                    num29 = 10;

                if (Main.expertMode && NPC.ai[1] % (float)num29 == 0f)
                {
                    float num30 = 5f;
                    Vector2 vector5 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    float num31 = Main.rand.Next(-200, 200);
                    float num32 = Main.rand.Next(-200, 200);
                    if (Main.getGoodWorld)
                    {
                        num31 *= 3f;
                        num32 *= 3f;
                    }

                    float num33 = (float)Math.Sqrt(num31 * num31 + num32 * num32);
                    num33 = num30 / num33;
                    Vector2 vector6 = vector5;
                    Vector2 vector7 = default(Vector2);
                    vector7.X = num31 * num33;
                    vector7.Y = num32 * num33;
                    vector6.X += vector7.X * 10f;
                    vector6.Y += vector7.Y * 10f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num34 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)vector6.X, (int)vector6.Y, 5);
                        Main.npc[num34].velocity.X = vector7.X;
                        Main.npc[num34].velocity.Y = vector7.Y;
                        if (VaultUtils.isServer && num34 < 200)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num34);
                    }

                    for (int n = 0; n < 10; n++)
                    {
                        Dust.NewDust(vector6, 20, 20, DustID.GreenFairy, vector7.X * 0.4f, vector7.Y * 0.4f);
                    }
                }

                if (NPC.ai[1] >= 100f)
                {
                    if (NPC.ai[3] == 1f)
                    {
                        NPC.ai[3] = 0f;
                        NPC.ai[1] = 0f;
                    }
                    else
                    {
                        NPC.ai[0] += 1f;
                        NPC.ai[1] = 0f;
                        if (NPC.ai[0] == 3f)
                        {
                            NPC.ai[2] = 0f;
                        }
                        else
                        {
                            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, NPC.position);
                            for (int num35 = 0; num35 < 2; num35++)
                            {
                                Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 8);
                                Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 7);
                                Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 6);
                            }

                            for (int num36 = 0; num36 < 20; num36++)
                            {
                                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenFairy, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f);
                            }

                            SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.position);
                        }
                    }
                }

                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenFairy, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f);
                NPC.velocity.X *= 0.98f;
                NPC.velocity.Y *= 0.98f;
                if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                    NPC.velocity.X = 0f;

                if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                    NPC.velocity.Y = 0f;

                return;
            }

            NPC.defense = 0;
            int num37 = 23;
            int num38 = 18;
            if (Main.expertMode)
            {
                if (flag2)
                    NPC.defense = -15;

                if (flag3)
                {
                    num38 = 20;
                    NPC.defense = -30;
                }
            }

            NPC.damage = NPC.GetAttackDamage_LerpBetweenFinalValues(num37, num38);
            NPC.damage = NPC.GetAttackDamage_ScaledByStrength(NPC.damage);
            if (NPC.ai[1] == 0f && flag2)
                NPC.ai[1] = 5f;

            if (NPC.ai[1] == 0f)
            {
                float num39 = 6f;
                float num40 = 0.07f;
                Vector2 npcCenter = NPC.Center;
                float num41 = Target.Center.X - npcCenter.X;
                float num42 = Target.Center.Y - 120f - npcCenter.Y;
                float num43 = (float)Math.Sqrt(num41 * num41 + num42 * num42);
                if (num43 > 400f && Main.expertMode)
                {
                    num39 += 1f;
                    num40 += 0.05f;
                    if (num43 > 600f)
                    {
                        num39 += 1f;
                        num40 += 0.05f;
                        if (num43 > 800f)
                        {
                            num39 += 1f;
                            num40 += 0.05f;
                        }
                    }
                }

                if (Main.getGoodWorld)
                {
                    num39 += 1f;
                    num40 += 0.1f;
                }

                num43 = num39 / num43;
                num41 *= num43;
                num42 *= num43;
                if (NPC.velocity.X < num41)
                {
                    NPC.velocity.X += num40;
                    if (NPC.velocity.X < 0f && num41 > 0f)
                        NPC.velocity.X += num40;
                }
                else if (NPC.velocity.X > num41)
                {
                    NPC.velocity.X -= num40;
                    if (NPC.velocity.X > 0f && num41 < 0f)
                        NPC.velocity.X -= num40;
                }

                if (NPC.velocity.Y < num42)
                {
                    NPC.velocity.Y += num40;
                    if (NPC.velocity.Y < 0f && num42 > 0f)
                        NPC.velocity.Y += num40;
                }
                else if (NPC.velocity.Y > num42)
                {
                    NPC.velocity.Y -= num40;
                    if (NPC.velocity.Y > 0f && num42 < 0f)
                        NPC.velocity.Y -= num40;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= 200f)
                {
                    NPC.ai[1] = 1f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    if (Main.expertMode && NPC.life < NPC.lifeMax * 0.35)
                        NPC.ai[1] = 3f;

                    NPC.target = 255;
                    NPC.netUpdate = true;
                }

                if (Main.expertMode && flag3)
                {
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] -= 1000f;
                }
            }
            else if (NPC.ai[1] == 1f)
            {
                SoundEngine.PlaySound(CoraliteSoundID.ForceRoar, NPC.position);
                NPC.rotation = num7;
                float num44 = 6.8f;
                if (Main.expertMode && NPC.ai[3] == 1f)
                    num44 *= 1.15f;

                if (Main.expertMode && NPC.ai[3] == 2f)
                    num44 *= 1.3f;

                if (Main.getGoodWorld)
                    num44 *= 1.2f;

                Vector2 npcCenter = NPC.Center;
                float num45 = Target.Center.X - npcCenter.X;
                float num46 = Target.Center.Y - npcCenter.Y;
                float num47 = (float)Math.Sqrt(num45 * num45 + num46 * num46);
                num47 = num44 / num47;
                NPC.velocity.X = num45 * num47;
                NPC.velocity.Y = num46 * num47;
                NPC.ai[1] = 2f;
                NPC.netUpdate = true;
                if (NPC.netSpam > 10)
                    NPC.netSpam = 10;
            }
            else if (NPC.ai[1] == 2f)
            {
                float num48 = 40f;
                NPC.ai[2] += 1f;
                if (Main.expertMode)
                    num48 = 50f;

                if (NPC.ai[2] >= num48)
                {
                    NPC.velocity *= 0.97f;
                    if (Main.expertMode)
                        NPC.velocity *= 0.98f;

                    if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                        NPC.velocity.X = 0f;

                    if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                        NPC.velocity.Y = 0f;
                }
                else
                {
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) - 1.57f;
                }

                int num49 = 130;
                if (Main.expertMode)
                    num49 = 90;

                if (NPC.ai[2] >= (float)num49)
                {
                    NPC.ai[3] += 1f;
                    NPC.ai[2] = 0f;
                    NPC.target = 255;
                    NPC.rotation = num7;
                    if (NPC.ai[3] >= 3f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[3] = 0f;
                        if (Main.expertMode && Main.netMode != NetmodeID.MultiplayerClient && NPC.life < NPC.lifeMax * 0.5)
                        {
                            NPC.ai[1] = 3f;
                            NPC.ai[3] += Main.rand.Next(1, 4);
                        }

                        NPC.netUpdate = true;
                        if (NPC.netSpam > 10)
                            NPC.netSpam = 10;
                    }
                    else
                    {
                        NPC.ai[1] = 1f;
                    }
                }
            }
            else if (NPC.ai[1] == 3f)
            {
                if (NPC.ai[3] == 4f && flag2 && NPC.Center.Y > Target.Center.Y)
                {
                    NPC.TargetClosest();
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                    if (NPC.netSpam > 10)
                        NPC.netSpam = 10;
                }
                else if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.TargetClosest();
                    float num50 = 20f;
                    Vector2 vector10 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    float num51 = Target.Center.X - vector10.X;
                    float num52 = Target.Center.Y - vector10.Y;
                    float num53 = Math.Abs(Target.velocity.X) + Math.Abs(Target.velocity.Y) / 4f;
                    num53 += 10f - num53;
                    if (num53 < 5f)
                        num53 = 5f;

                    if (num53 > 15f)
                        num53 = 15f;

                    if (NPC.ai[2] == -1f && !flag3)
                    {
                        num53 *= 4f;
                        num50 *= 1.3f;
                    }

                    if (flag3)
                        num53 *= 2f;

                    num51 -= Target.velocity.X * num53;
                    num52 -= Target.velocity.Y * num53 / 4f;
                    num51 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
                    num52 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
                    if (flag3)
                    {
                        num51 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
                        num52 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
                    }

                    float num54 = (float)Math.Sqrt(num51 * num51 + num52 * num52);
                    float num55 = num54;
                    num54 = num50 / num54;
                    NPC.velocity.X = num51 * num54;
                    NPC.velocity.Y = num52 * num54;
                    NPC.velocity.X += (float)Main.rand.Next(-20, 21) * 0.1f;
                    NPC.velocity.Y += (float)Main.rand.Next(-20, 21) * 0.1f;
                    if (flag3)
                    {
                        NPC.velocity.X += (float)Main.rand.Next(-50, 51) * 0.1f;
                        NPC.velocity.Y += (float)Main.rand.Next(-50, 51) * 0.1f;
                        float num56 = Math.Abs(NPC.velocity.X);
                        float num57 = Math.Abs(NPC.velocity.Y);
                        if (NPC.Center.X > Target.Center.X)
                            num57 *= -1f;

                        if (NPC.Center.Y > Target.Center.Y)
                            num56 *= -1f;

                        NPC.velocity.X = num57 + NPC.velocity.X;
                        NPC.velocity.Y = num56 + NPC.velocity.Y;
                        NPC.velocity.Normalize();
                        NPC.velocity *= num50;
                        NPC.velocity.X += (float)Main.rand.Next(-20, 21) * 0.1f;
                        NPC.velocity.Y += (float)Main.rand.Next(-20, 21) * 0.1f;
                    }
                    else if (num55 < 100f)
                    {
                        if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                        {
                            float num58 = Math.Abs(NPC.velocity.X);
                            float num59 = Math.Abs(NPC.velocity.Y);
                            if (NPC.Center.X > Target.Center.X)
                                num59 *= -1f;

                            if (NPC.Center.Y > Target.Center.Y)
                                num58 *= -1f;

                            NPC.velocity.X = num59;
                            NPC.velocity.Y = num58;
                        }
                    }
                    else if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                    {
                        float num60 = (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) / 2f;
                        float num61 = num60;
                        if (NPC.Center.X > Target.Center.X)
                            num61 *= -1f;

                        if (NPC.Center.Y > Target.Center.Y)
                            num60 *= -1f;

                        NPC.velocity.X = num61;
                        NPC.velocity.Y = num60;
                    }

                    NPC.ai[1] = 4f;
                    NPC.netUpdate = true;
                    if (NPC.netSpam > 10)
                        NPC.netSpam = 10;
                }
            }
            else if (NPC.ai[1] == 4f)
            {
                if (NPC.ai[2] == 0f)
                    SoundEngine.PlaySound(CoraliteSoundID.ForceRoar, NPC.position);

                float num62 = num4;
                NPC.ai[2] += 1f;
                if (NPC.ai[2] == num62 && Vector2.Distance(NPC.position, Target.position) < 200f)
                    NPC.ai[2] -= 1f;

                if (NPC.ai[2] >= num62)
                {
                    NPC.velocity *= 0.95f;
                    if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                        NPC.velocity.X = 0f;

                    if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                        NPC.velocity.Y = 0f;
                }
                else
                {
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) - 1.57f;
                }

                float num63 = num62 + 13f;
                if (NPC.ai[2] >= num63)
                {
                    NPC.netUpdate = true;
                    if (NPC.netSpam > 10)
                        NPC.netSpam = 10;

                    NPC.ai[3] += 1f;
                    NPC.ai[2] = 0f;
                    if (NPC.ai[3] >= 5f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[3] = 0f;
                        if (NPC.target >= 0 && Main.getGoodWorld && Collision.CanHit(NPC.position, NPC.width, NPC.height, Target.position, NPC.width, NPC.height))
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.position);
                            NPC.ai[0] = 2f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 1f;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        NPC.ai[1] = 3f;
                    }
                }
            }
            else if (NPC.ai[1] == 5f)
            {
                float num64 = 600f;
                float num65 = 9f;
                float num66 = 0.3f;
                Vector2 vector11 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float num67 = Target.position.X + (float)(Target.width / 2) - vector11.X;
                float num68 = Target.position.Y + (float)(Target.height / 2) + num64 - vector11.Y;
                float num69 = (float)Math.Sqrt(num67 * num67 + num68 * num68);
                num69 = num65 / num69;
                num67 *= num69;
                num68 *= num69;
                if (NPC.velocity.X < num67)
                {
                    NPC.velocity.X += num66;
                    if (NPC.velocity.X < 0f && num67 > 0f)
                        NPC.velocity.X += num66;
                }
                else if (NPC.velocity.X > num67)
                {
                    NPC.velocity.X -= num66;
                    if (NPC.velocity.X > 0f && num67 < 0f)
                        NPC.velocity.X -= num66;
                }

                if (NPC.velocity.Y < num68)
                {
                    NPC.velocity.Y += num66;
                    if (NPC.velocity.Y < 0f && num68 > 0f)
                        NPC.velocity.Y += num66;
                }
                else if (NPC.velocity.Y > num68)
                {
                    NPC.velocity.Y -= num66;
                    if (NPC.velocity.Y > 0f && num68 < 0f)
                        NPC.velocity.Y -= num66;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= 70f)
                {
                    NPC.TargetClosest();
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = -1f;
                    NPC.ai[3] = Main.rand.Next(-3, 1);
                    NPC.netUpdate = true;
                }
            }

            if (flag3 && NPC.ai[1] == 5f)
                NPC.ai[1] = 3f;
        }

        public override void FindFrame(int frameheight)
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter < 7.0)
            {
                NPC.frame.Y = 0;
            }
            else if (NPC.frameCounter < 14.0)
            {
                NPC.frame.Y = 1;
            }
            else if (NPC.frameCounter < 21.0)
            {
                NPC.frame.Y = 2;
            }
            else
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = 0;
            }
            if (NPC.ai[0] > 1f)
                NPC.frame.Y += 3;
        }

        public override void OnKill()
        {

            NPC.SetEventFlagCleared(ref NPC.downedBoss1, GameEventClearedID.DefeatedEyeOfCthulu);
            if (VaultUtils.isServer)
                NetMessage.SendData(MessageID.WorldData);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            rand += (NPC.rotation - 1.57f).ToRotationVector2();

            Texture2D mainTex = NPC.GetTexture();
            Rectangle frameBox = mainTex.Frame(1, 6, 0, NPC.frame.Y);

            Effect effect = Filters.Scene["Crystal"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["basePos"].SetValue((NPC.Center + rand - Main.screenPosition) * Main.GameZoomTarget);
            effect.Parameters["scale"].SetValue(new Vector2(0.7f) / Main.GameZoomTarget);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f);
            effect.Parameters["lightRange"].SetValue(0.2f);
            effect.Parameters["lightLimit"].SetValue(0.2f);
            effect.Parameters["addC"].SetValue(0.75f);
            effect.Parameters["highlightC"].SetValue(new Color(127, 218, 153).ToVector4());
            effect.Parameters["brightC"].SetValue(new Color(0, 170, 95).ToVector4());
            effect.Parameters["darkC"].SetValue(new Color(93, 91, 77).ToVector4());

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.GameViewMatrix.ZoomMatrix);

            Main.graphics.GraphicsDevice.Textures[1] = GemTextures.CrystalNoiseP3.Value;
            Color c = drawColor;
            spriteBatch.Draw(mainTex, NPC.Center, frameBox, c, NPC.rotation, frameBox.Size() / 2, NPC.scale, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
