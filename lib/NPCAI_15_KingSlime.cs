using System;
using Terraria;
using Terraria.ID;

namespace Coralite.lib
{
    public class NPCAI_15_KingSlime
    {
        public NPC NPC { get; set; }
        public Player Target => Main.player[NPC.target];

        public void AI()
        {
            float num236 = 1f;
            float num237 = 1f;
            bool flag6 = false;
            bool flag7 = false;
            bool flag8 = false;
            float num238 = 2f;
            if (Main.getGoodWorld)
            {
                num238 -= 1f - NPC.life / (float)NPC.lifeMax;
                num237 *= num238;
            }

            NPC.aiAction = 0;
            if (NPC.ai[3] == 0f && NPC.life > 0)
                NPC.ai[3] = NPC.lifeMax;

            if (NPC.localAI[3] == 0f)
            {
                NPC.localAI[3] = 1f;
                flag6 = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[0] = -100f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            int escapeDistance = 3000;

            #region 脱战时离开
            if (Target.dead || Vector2.Distance(NPC.Center, Target.Center) > escapeDistance)
            {
                NPC.TargetClosest();
                if (Target.dead || Vector2.Distance(NPC.Center, Target.Center) > escapeDistance)
                {
                    NPC.EncourageDespawn(10);
                    if (Target.Center.X < NPC.Center.X)
                        NPC.direction = 1;
                    else
                        NPC.direction = -1;

                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] != 5f)
                    {
                        NPC.netUpdate = true;
                        NPC.ai[2] = 0f;
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 5f;
                        NPC.localAI[1] = Main.maxTilesX * 16;
                        NPC.localAI[2] = Main.maxTilesY * 16;
                    }
                }
            }

            #endregion

            if (!Target.dead && NPC.timeLeft > 10 && NPC.ai[2] >= 300f && NPC.ai[1] < 5f && NPC.velocity.Y == 0f)
            {
                NPC.ai[2] = 0f;
                NPC.ai[0] = 0f;
                NPC.ai[1] = 5f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.TargetClosest(faceTarget: false);
                    Point point3 = NPC.Center.ToTileCoordinates();
                    Point point4 = Target.Center.ToTileCoordinates();
                    Vector2 vector30 = Target.Center - NPC.Center;
                    int num240 = 10;
                    int num241 = 0;
                    int num242 = 7;
                    int num243 = 0;
                    bool flag9 = false;
                    if (NPC.localAI[0] >= 360f || vector30.Length() > 2000f)
                    {
                        if (NPC.localAI[0] >= 360f)
                            NPC.localAI[0] = 360f;

                        flag9 = true;
                        num243 = 100;
                    }

                    while (!flag9 && num243 < 100)
                    {
                        num243++;
                        int num244 = Main.rand.Next(point4.X - num240, point4.X + num240 + 1);
                        int num245 = Main.rand.Next(point4.Y - num240, point4.Y + 1);
                        if ((num245 >= point4.Y - num242 && num245 <= point4.Y + num242 && num244 >= point4.X - num242 && num244 <= point4.X + num242) || (num245 >= point3.Y - num241 && num245 <= point3.Y + num241 && num244 >= point3.X - num241 && num244 <= point3.X + num241) || Main.tile[num244, num245].HasUnactuatedTile)
                            continue;

                        int num246 = num245;
                        int num247 = 0;
                        if (Main.tile[num244, num246].HasUnactuatedTile && Main.tileSolid[Main.tile[num244, num246].TileType] && !Main.tileSolidTop[Main.tile[num244, num246].TileType])
                        {
                            num247 = 1;
                        }
                        else
                        {
                            for (; num247 < 150 && num246 + num247 < Main.maxTilesY; num247++)
                            {
                                int num248 = num246 + num247;
                                if (Main.tile[num244, num248].HasUnactuatedTile && Main.tileSolid[Main.tile[num244, num248].TileType] && !Main.tileSolidTop[Main.tile[num244, num248].TileType])
                                {
                                    num247--;
                                    break;
                                }
                            }
                        }

                        num245 += num247;
                        bool flag10 = true;
                        if (flag10 && Main.tile[num244, num245].LiquidType == LiquidID.Lava)
                            flag10 = false;

                        if (flag10 && !Collision.CanHitLine(NPC.Center, 0, 0, Target.Center, 0, 0))
                            flag10 = false;

                        if (flag10)
                        {
                            NPC.localAI[1] = num244 * 16 + 8;
                            NPC.localAI[2] = num245 * 16 + 16;
                            flag9 = true;
                            break;
                        }
                    }

                    if (num243 >= 100)
                    {
                        Vector2 bottom = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)].Bottom;
                        NPC.localAI[1] = bottom.X;
                        NPC.localAI[2] = bottom.Y;
                    }
                }
            }

            if (!Collision.CanHitLine(NPC.Center, 0, 0, Target.Center, 0, 0) || Math.Abs(NPC.Top.Y - Target.Bottom.Y) > 160f)
            {
                NPC.ai[2]++;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.localAI[0]++;
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0]--;
                if (NPC.localAI[0] < 0f)
                    NPC.localAI[0] = 0f;
            }

            if (NPC.timeLeft < 10 && (NPC.ai[0] != 0f || NPC.ai[1] != 0f))
            {
                NPC.ai[0] = 0f;
                NPC.ai[1] = 0f;
                NPC.netUpdate = true;
                flag7 = false;
            }

            Dust dust;
            if (NPC.ai[1] == 5f)
            {
                flag7 = true;
                NPC.aiAction = 1;
                NPC.ai[0]++;
                num236 = MathHelper.Clamp((60f - NPC.ai[0]) / 60f, 0f, 1f);
                num236 = 0.5f + num236 * 0.5f;
                if (NPC.ai[0] >= 60f)
                    flag8 = true;

                //瞬移时的王冠
                if (NPC.ai[0] == 60f)
                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-40f, -NPC.height / 2), NPC.velocity, GoreID.KingSlimeCrown);
                //瞬移
                if (NPC.ai[0] >= 60f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.Bottom = new Vector2(NPC.localAI[1], NPC.localAI[2]);
                    NPC.ai[1] = 6f;
                    NPC.ai[0] = 0f;
                    NPC.netUpdate = true;
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[0] >= 120f)
                {
                    NPC.ai[1] = 6f;
                    NPC.ai[0] = 0f;
                }

                if (!flag8)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int num250 = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, DustID.TintableDust, NPC.velocity.X, NPC.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
                        Main.dust[num250].noGravity = true;
                        dust = Main.dust[num250];
                        dust.velocity *= 0.5f;
                    }
                }
            }
            else if (NPC.ai[1] == 6f)
            {
                flag7 = true;
                NPC.aiAction = 0;
                NPC.ai[0]++;
                num236 = MathHelper.Clamp(NPC.ai[0] / 30f, 0f, 1f);
                num236 = 0.5f + num236 * 0.5f;
                if (NPC.ai[0] >= 30f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 0f;
                    NPC.netUpdate = true;
                    NPC.TargetClosest();
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[0] >= 60f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 0f;
                    NPC.TargetClosest();
                }

                for (int i = 0; i < 10; i++)
                {
                    int num252 = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, DustID.TintableDust, NPC.velocity.X, NPC.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
                    Main.dust[num252].noGravity = true;
                    dust = Main.dust[num252];
                    dust.velocity *= 2f;
                }
            }

            NPC.dontTakeDamage = (NPC.hide = flag8);
            if (NPC.velocity.Y == 0f)
            {
                NPC.velocity.X *= 0.8f;
                if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                    NPC.velocity.X = 0f;

                if (!flag7)
                {
                    NPC.ai[0] += 2f;
                    if (NPC.life < NPC.lifeMax * 0.8)
                        NPC.ai[0] += 1f;

                    if (NPC.life < NPC.lifeMax * 0.6)
                        NPC.ai[0] += 1f;

                    if (NPC.life < NPC.lifeMax * 0.4)
                        NPC.ai[0] += 2f;

                    if (NPC.life < NPC.lifeMax * 0.2)
                        NPC.ai[0] += 3f;

                    if (NPC.life < NPC.lifeMax * 0.1)
                        NPC.ai[0] += 4f;

                    if (NPC.ai[0] >= 0f)
                    {
                        NPC.netUpdate = true;
                        NPC.TargetClosest();
                        if (NPC.ai[1] == 3f)
                        {
                            NPC.velocity.Y = -13f;
                            NPC.velocity.X += 3.5f * NPC.direction;
                            NPC.ai[0] = -200f;
                            NPC.ai[1] = 0f;
                        }
                        else if (NPC.ai[1] == 2f)
                        {
                            NPC.velocity.Y = -6f;
                            NPC.velocity.X += 4.5f * NPC.direction;
                            NPC.ai[0] = -120f;
                            NPC.ai[1] += 1f;
                        }
                        else
                        {
                            NPC.velocity.Y = -8f;
                            NPC.velocity.X += 4f * NPC.direction;
                            NPC.ai[0] = -120f;
                            NPC.ai[1] += 1f;
                        }
                    }
                    else if (NPC.ai[0] >= -30f)
                    {
                        NPC.aiAction = 1;
                    }
                }
            }
            else if (NPC.target < 255)
            {
                float num253 = 3f;
                if (Main.getGoodWorld)
                    num253 = 6f;

                if ((NPC.direction == 1 && NPC.velocity.X < num253) || (NPC.direction == -1 && NPC.velocity.X > 0f - num253))
                {
                    if ((NPC.direction == -1 && NPC.velocity.X < 0.1) || (NPC.direction == 1 && NPC.velocity.X > -0.1))
                        NPC.velocity.X += 0.2f * NPC.direction;
                    else
                        NPC.velocity.X *= 0.93f;
                }
            }

            int index = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust, NPC.velocity.X, NPC.velocity.Y, 255, new Color(0, 80, 255, 80), NPC.scale * 1.2f);
            Main.dust[index].noGravity = true;
            dust = Main.dust[index];
            dust.velocity *= 0.5f;
            if (NPC.life <= 0)
                return;

            float num255 = NPC.life / (float)NPC.lifeMax;
            num255 = num255 * 0.5f + 0.75f;
            num255 *= num236;
            num255 *= num237;
            if (num255 != NPC.scale || flag6)
            {
                NPC.position.X += NPC.width / 2;
                NPC.position.Y += NPC.height;
                NPC.scale = num255;
                NPC.width = (int)(98f * NPC.scale);
                NPC.height = (int)(92f * NPC.scale);
                NPC.position.X -= NPC.width / 2;
                NPC.position.Y -= NPC.height;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            //以下为仅在服务器端进行的AI

            int num256 = (int)(NPC.lifeMax * 0.05);
            if (!(NPC.life + num256 < NPC.ai[3]))
                return;

            NPC.ai[3] = NPC.life;
            int num257 = Main.rand.Next(1, 4);
            for (int num258 = 0; num258 < num257; num258++)
            {
                int x = (int)(NPC.position.X + Main.rand.Next(NPC.width - 32));
                int y = (int)(NPC.position.Y + Main.rand.Next(NPC.height - 32));
                int num259 = 1;
                if (Main.expertMode && Main.rand.NextBool(4))
                    num259 = 535;

                int num260 = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, num259);
                Main.npc[num260].SetDefaults(num259);
                Main.npc[num260].velocity.X = Main.rand.Next(-15, 16) * 0.1f;
                Main.npc[num260].velocity.Y = Main.rand.Next(-30, 1) * 0.1f;
                Main.npc[num260].ai[0] = -1000 * Main.rand.Next(3);
                Main.npc[num260].ai[1] = 0f;
                if (Main.netMode == NetmodeID.Server && num260 < 200)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num260);
            }

            return;
        }

    }
}

