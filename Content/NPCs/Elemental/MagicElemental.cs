using Coralite.Content.Items.Materials;
using Coralite.Core;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.NPCs.Elemental
{
    public class MagicElemental : ModNPC
    {
        public override string Texture => AssetDirectory.ElementalNPCs + Name;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.damage = 10;
            NPC.lifeMax = 40;
            NPC.width = NPC.height = 30;
            NPC.defense = 2;
            NPC.knockBackResist = 0.75f;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.scale = Main.rand.NextFloat(1, 1.3f);

            NPC.HitSound = CoraliteSoundID.Fairy_NPCHit5;
            NPC.DeathSound = CoraliteSoundID.FairyDeath_NPCDeath7;
        }

        public override void AI()
        {
            bool flag16 = false;

            if (NPC.justHit)
                NPC.ai[2] = 0f;

            if (NPC.ai[2] >= 0f)
            {
                int num298 = 16;
                bool flag18 = false;
                bool flag19 = false;
                if (NPC.position.X > NPC.ai[0] - num298 && NPC.position.X < NPC.ai[0] + num298)
                    flag18 = true;
                else if ((NPC.velocity.X < 0f && NPC.direction > 0) || (NPC.velocity.X > 0f && NPC.direction < 0))
                    flag18 = true;

                num298 += 24;
                if (NPC.position.Y > NPC.ai[1] - (float)num298 && NPC.position.Y < NPC.ai[1] + num298)
                    flag19 = true;

                if (flag18 && flag19)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 30f && num298 == 16)
                        flag16 = true;

                    if (NPC.ai[2] >= 60f)
                    {
                        NPC.ai[2] = -200f;
                        NPC.direction *= -1;
                        NPC.velocity.X *= -1f;
                        NPC.collideX = false;
                    }
                }
                else
                {
                    NPC.ai[0] = NPC.position.X;
                    NPC.ai[1] = NPC.position.Y;
                    NPC.ai[2] = 0f;
                }

                NPC.TargetClosest();
            }
            else
            {
                NPC.ai[2] += 1f;

                if (Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2) > NPC.position.X + (NPC.width / 2))
                    NPC.direction = -1;
                else
                    NPC.direction = 1;
            }

            int num299 = (int)((NPC.position.X + (NPC.width / 2)) / 16f) + NPC.direction * 2;
            int num300 = (int)((NPC.position.Y + NPC.height) / 16f);
            bool flag20 = true;
            bool flag21 = false;
            int num301 = 10;

            {
                NPC.position += NPC.netOffset;
                Lighting.AddLight(NPC.Center, 0.6f, 0, 0.75f);
                NPC.alpha = 15;
                if (Main.rand.NextBool(3))
                {
                    int index = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch, 0f, 0f, 200);
                    Dust dust = Main.dust[index];
                    dust.velocity *= 0.3f;
                    dust.noGravity = true;
                }

                NPC.position -= NPC.netOffset;
                if (NPC.justHit)
                {
                    NPC.ai[3] = 0f;
                    NPC.localAI[1] = 0f;
                }

                float num313 = 5f;
                Vector2 center = NPC.Center;
                float num314 = Main.player[NPC.target].Center.X - center.X;
                float num315 = Main.player[NPC.target].Center.Y - center.Y;
                float num316 = (float)Math.Sqrt(num314 * num314 + num315 * num315);
                num316 = num313 / num316;
                num314 *= num316;
                num315 *= num316;
                if (num314 > 0f)
                    NPC.direction = 1;
                else
                    NPC.direction = -1;

                NPC.spriteDirection = NPC.direction;
                if (NPC.direction < 0)
                    NPC.rotation = (float)Math.Atan2(0f - num315, 0f - num314);
                else
                    NPC.rotation = (float)Math.Atan2(num315, num314);
            }

            if (NPC.position.Y + NPC.height > Main.player[NPC.target].position.Y)
            {
                {
                    for (int i = num300; i < num300 + num301; i++)
                    {
                        if ((Main.tile[num299, i].HasTile && Main.tileSolid[Main.tile[num299, i].TileType]) || Main.tile[num299, i].LiquidAmount > 0)
                        {
                            if (i <= num300 + 1)
                                flag21 = true;

                            flag20 = false;
                            break;
                        }
                    }
                }
            }

            if (Main.player[NPC.target].npcTypeNoAggro[NPC.type])
            {
                bool flag22 = false;
                for (int i = num300; i < num300 + num301 - 2; i++)
                {
                    if ((Main.tile[num299, i].HasTile && Main.tileSolid[Main.tile[num299, i].TileType]) || Main.tile[num299, i].LiquidAmount > 0)
                    {
                        flag22 = true;
                        break;
                    }
                }

                NPC.directionY = (!flag22).ToDirectionInt();
            }

            if (flag16)
            {
                flag21 = false;
                flag20 = true;
            }

            if (flag20)
            {
                {
                    NPC.velocity.Y += 0.2f;
                    if (NPC.velocity.Y > 2f)
                        NPC.velocity.Y = 2f;
                }
            }
            else
            {
                if ((NPC.directionY < 0 && NPC.velocity.Y > 0f) || flag21)
                    NPC.velocity.Y -= 0.2f;

                if (NPC.velocity.Y < -4f)
                    NPC.velocity.Y = -4f;
            }

            if (NPC.collideX)
            {
                NPC.velocity.X = NPC.oldVelocity.X * -0.4f;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 1f)
                    NPC.velocity.X = 1f;

                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -1f)
                    NPC.velocity.X = -1f;
            }

            if (NPC.collideY)
            {
                NPC.velocity.Y = NPC.oldVelocity.Y * -0.25f;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1f)
                    NPC.velocity.Y = 1f;

                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1f)
                    NPC.velocity.Y = -1f;
            }

            float speedMax = 2f;

            if (NPC.direction == -1 && NPC.velocity.X > 0f - speedMax)
            {
                NPC.velocity.X -= 0.1f;
                if (NPC.velocity.X > speedMax)
                    NPC.velocity.X -= 0.1f;
                else if (NPC.velocity.X > 0f)
                    NPC.velocity.X += 0.05f;

                if (NPC.velocity.X < 0f - speedMax)
                    NPC.velocity.X = 0f - speedMax;
            }
            else if (NPC.direction == 1 && NPC.velocity.X < speedMax)
            {
                NPC.velocity.X += 0.1f;
                if (NPC.velocity.X < 0f - speedMax)
                    NPC.velocity.X += 0.1f;
                else if (NPC.velocity.X < 0f)
                    NPC.velocity.X -= 0.05f;

                if (NPC.velocity.X > speedMax)
                    NPC.velocity.X = speedMax;
            }

            speedMax = 1.5f;
            if (NPC.directionY == -1 && NPC.velocity.Y > 0f - speedMax)
            {
                NPC.velocity.Y -= 0.04f;
                if (NPC.velocity.Y > speedMax)
                    NPC.velocity.Y -= 0.05f;
                else if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y += 0.03f;

                if (NPC.velocity.Y < 0f - speedMax)
                    NPC.velocity.Y = 0f - speedMax;
            }
            else if (NPC.directionY == 1 && NPC.velocity.Y < speedMax)
            {
                NPC.velocity.Y += 0.04f;
                if (NPC.velocity.Y < 0f - speedMax)
                    NPC.velocity.Y += 0.05f;
                else if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y -= 0.03f;

                if (NPC.velocity.Y > speedMax)
                    NPC.velocity.Y = speedMax;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;
            float num162 = 8f;

            if (NPC.frameCounter >= num162)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0.0;
            }

            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                NPC.frame.Y = 0;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 dir = Helpers.Helper.NextVec2Dir();
                Dust d = Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(20), DustID.ShadowbeamStaff, dir * Main.rand.NextFloat(1, 4),
                    Scale: Main.rand.NextFloat(1, 1.4f));
                d.noGravity = true;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.townNPCs > 2f)
                return 0;

            if (Main.dayTime && spawnInfo.Player.ZonePurity)
                return 0.02f;
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MagicalPowder>(), 2, 1, 3));
        }
    }
}
