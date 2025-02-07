using Coralite.Content.Items.ShadowCastle;
using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class PorcelainPot : ModTile
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.StyleWrapLimit = 9;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.addTile(Type);

            AddMapEntry(Color.SandyBrown);
        }

        private static EntitySource_TileBreak GetProjectileSource_TileBreak(int x, int y) => new(x, y);
        public static IEntitySource GetItemSource_FromTileBreak(int x, int y) => new EntitySource_TileBreak(x, y);

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            SoundEngine.PlaySound(CoraliteSoundID.GlassBroken_Shatter, new Vector2(i * 16, j * 16));

            if (Main.netMode != NetmodeID.MultiplayerClient)
                SpawnThingsFromPot(i, j, frameX / 36);
        }

        private static void SpawnThingsFromPot(int i, int j, int style)
        {
            bool rockLayer = j < Main.rockLayer;
            bool underWorld = j < Main.UnderworldLayer;
            if (Main.remixWorld)
            {
                rockLayer = j > Main.rockLayer && j < Main.UnderworldLayer;
                underWorld = j > Main.worldSurface && j < Main.rockLayer;
            }

            float num = 1f;

            num = ((num * 2f) + 1f) / 3f;
            int range = (int)(500f / ((num + 1f) / 2f));
            if (WorldGen.gen)
                return;

            if (Player.GetClosestRollLuck(i, j, range) == 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(GetProjectileSource_TileBreak(i, j), (i * 16) + 16, (j * 16) + 16, 0f, -12f, 518, 0, 0f, Main.myPlayer);

                return;
            }

            if (WorldGen.genRand.NextBool(35) && CoraliteSets.WallShadowCastle[Main.tile[i, j].WallType] && j > Main.worldSurface)
            {
                Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<ShadowMagneticCard>());
                return;
            }

            if (Main.getGoodWorld && WorldGen.genRand.NextBool(6))
            {
                Projectile.NewProjectile(GetProjectileSource_TileBreak(i, j), (i * 16) + 16, (j * 16) + 8, Main.rand.Next(-100, 101) * 0.002f, 0f, 28, 0, 0f, Main.myPlayer, 16f, 16f);
                return;
            }

            if (Main.remixWorld && Main.netMode != NetmodeID.MultiplayerClient && WorldGen.genRand.NextBool(5))
            {
                if (Main.rand.NextBool(2))
                {
                    Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 75);
                }
                else
                {
                    Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 75);
                }

                return;
            }

            if (Main.remixWorld && i > Main.maxTilesX * 0.37 && i < Main.maxTilesX * 0.63 && j > Main.maxTilesY - 220)
            {
                int stack = Main.rand.Next(20, 41);
                Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 965, stack);
                return;
            }

            if (WorldGen.genRand.NextBool(45) || (Main.rand.NextBool(45) && Main.expertMode))
            {
                if (rockLayer)
                {
                    int num6 = WorldGen.genRand.Next(11);
                    if (num6 == 0)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 289);

                    if (num6 == 1)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 298);

                    if (num6 == 2)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 299);

                    if (num6 == 3)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 290);

                    if (num6 == 4)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 303);

                    if (num6 == 5)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 291);

                    if (num6 == 6)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 304);

                    if (num6 == 7)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 2322);

                    if (num6 == 8)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 2329);

                    if (num6 >= 7)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 2350, WorldGen.genRand.Next(1, 3));
                }
                else if (underWorld)
                {
                    int num7 = WorldGen.genRand.Next(15);
                    if (num7 == 0)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 296);

                    if (num7 == 1)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 295);

                    if (num7 == 2)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 299);

                    if (num7 == 3)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 302);

                    if (num7 == 4)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 303);

                    if (num7 == 5)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 305);

                    if (num7 == 6)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 301);

                    if (num7 == 7)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 302);

                    if (num7 == 8)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 297);

                    if (num7 == 9)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 304);

                    if (num7 == 10)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 2322);

                    if (num7 == 11)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 2323);

                    if (num7 == 12)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 2327);

                    if (num7 == 13)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 2329);

                    if (num7 >= 7)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 2350, WorldGen.genRand.Next(1, 3));
                }
                else
                {
                    int num8 = WorldGen.genRand.Next(14);
                    if (num8 == 0)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 296);

                    if (num8 == 1)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 295);

                    if (num8 == 2)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 293);

                    if (num8 == 3)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 288);

                    if (num8 == 4)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 294);

                    if (num8 == 5)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 297);

                    if (num8 == 6)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 304);

                    if (num8 == 7)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 305);

                    if (num8 == 8)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 301);

                    if (num8 == 9)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 302);

                    if (num8 == 10)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 288);

                    if (num8 == 11)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 300);

                    if (num8 == 12)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 2323);

                    if (num8 == 13)
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 2326);

                    if (WorldGen.genRand.NextBool(5))
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 4870);
                }

                return;
            }

            if (VaultUtils.isServer && Main.rand.NextBool(30))
            {
                Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 2997);
                return;
            }

            int num9 = Main.rand.Next(7);
            if (Main.expertMode)
                num9--;

            Player player2 = Main.player[Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16)];
            int num10 = 0;
            int num11 = 20;
            for (int k = 0; k < 50; k++)
            {
                Item item = player2.inventory[k];
                if (!item.IsAir && item.createTile == TileID.Torches)
                {
                    num10 += item.stack;
                    if (num10 >= num11)
                        break;
                }
            }

            bool flag4 = num10 < num11;
            if (num9 == 0 && player2.statLife < player2.statLifeMax2)
            {
                Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 58);
                if (Main.rand.NextBool(2))
                    Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 58);

                if (Main.expertMode)
                {
                    if (Main.rand.NextBool(2))
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 58);

                    if (Main.rand.NextBool(2))
                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 58);
                }

                return;
            }

            if (num9 == 1 || (num9 == 0 && flag4))
            {
                int num12 = Main.rand.Next(2, 7);
                if (Main.expertMode)
                    num12 += Main.rand.Next(1, 7);

                int type = 8;
                int type2 = 282;

                if (Main.tile[i, j].LiquidAmount > 0)
                    Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, type2, num12);
                else
                    Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, type, num12);

                return;
            }

            switch (num9)
            {
                case 2:
                    {
                        int stack2 = Main.rand.Next(10, 21);
                        int type4 = 40;
                        if (rockLayer && WorldGen.genRand.NextBool(2))
                            type4 = (!Main.hardMode) ? 42 : 168;

                        if (j > Main.UnderworldLayer)
                            type4 = 265;
                        else if (Main.hardMode)
                            type4 = (!Main.rand.NextBool(2)) ? 47 : ((WorldGen.SavedOreTiers.Silver != 168) ? 278 : 4915);

                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, type4, stack2);
                        return;
                    }
                case 3:
                    {
                        int type5 = 28;
                        if (j > Main.UnderworldLayer || Main.hardMode)
                            type5 = 188;

                        int num14 = 1;
                        if (Main.expertMode && !Main.rand.NextBool(3))
                            num14++;

                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, type5, num14);
                        return;
                    }
                case 4:
                    if (underWorld)
                    {
                        int type3 = 166;

                        int num13 = Main.rand.Next(4) + 1;
                        if (Main.expertMode)
                            num13 += Main.rand.Next(4);

                        Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, type3, num13);
                        return;
                    }
                    break;
            }

            if ((num9 == 4 || num9 == 5) && j < Main.UnderworldLayer && !Main.hardMode)
            {
                int stack3 = Main.rand.Next(20, 41);
                Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 965, stack3);
                return;
            }

            float num15 = 200 + WorldGen.genRand.Next(-100, 101);
            if (j < Main.worldSurface)
                num15 *= 0.5f;
            else if (rockLayer)
                num15 *= 0.75f;
            else if (j > Main.maxTilesY - 250)
                num15 *= 1.25f;

            num15 *= 1f + (Main.rand.Next(-20, 21) * 0.01f);
            if (Main.rand.NextBool(4))
                num15 *= 1f + (Main.rand.Next(5, 11) * 0.01f);

            if (Main.rand.NextBool(8))
                num15 *= 1f + (Main.rand.Next(10, 21) * 0.01f);

            if (Main.rand.NextBool(12))
                num15 *= 1f + (Main.rand.Next(20, 41) * 0.01f);

            if (Main.rand.NextBool(16))
                num15 *= 1f + (Main.rand.Next(40, 81) * 0.01f);

            if (Main.rand.NextBool(20))
                num15 *= 1f + (Main.rand.Next(50, 101) * 0.01f);

            if (Main.expertMode)
                num15 *= 2.5f;

            if (Main.expertMode && Main.rand.NextBool(2))
                num15 *= 1.25f;

            if (Main.expertMode && Main.rand.NextBool(3))
                num15 *= 1.5f;

            if (Main.expertMode && Main.rand.NextBool(4))
                num15 *= 1.75f;

            num15 *= num;
            if (NPC.downedBoss1)
                num15 *= 1.1f;

            if (NPC.downedBoss2)
                num15 *= 1.1f;

            if (NPC.downedBoss3)
                num15 *= 1.1f;

            if (NPC.downedMechBoss1)
                num15 *= 1.1f;

            if (NPC.downedMechBoss2)
                num15 *= 1.1f;

            if (NPC.downedMechBoss3)
                num15 *= 1.1f;

            if (NPC.downedPlantBoss)
                num15 *= 1.1f;

            if (NPC.downedQueenBee)
                num15 *= 1.1f;

            if (NPC.downedGolemBoss)
                num15 *= 1.1f;

            if (NPC.downedPirates)
                num15 *= 1.1f;

            if (NPC.downedGoblins)
                num15 *= 1.1f;

            if (NPC.downedFrost)
                num15 *= 1.1f;

            while ((int)num15 > 0)
            {
                if (num15 > 1000000f)
                {
                    int num16 = (int)(num15 / 1000000f);
                    if (num16 > 50 && Main.rand.NextBool(2))
                        num16 /= Main.rand.Next(3) + 1;

                    if (Main.rand.NextBool(2))
                        num16 /= Main.rand.Next(3) + 1;

                    num15 -= 1000000 * num16;
                    Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 74, num16);
                    continue;
                }

                if (num15 > 10000f)
                {
                    int num17 = (int)(num15 / 10000f);
                    if (num17 > 50 && Main.rand.NextBool(2))
                        num17 /= Main.rand.Next(3) + 1;

                    if (Main.rand.NextBool(2))
                        num17 /= Main.rand.Next(3) + 1;

                    num15 -= 10000 * num17;
                    Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 73, num17);
                    continue;
                }

                if (num15 > 100f)
                {
                    int num18 = (int)(num15 / 100f);
                    if (num18 > 50 && Main.rand.NextBool(2))
                        num18 /= Main.rand.Next(3) + 1;

                    if (Main.rand.NextBool(2))
                        num18 /= Main.rand.Next(3) + 1;

                    num15 -= 100 * num18;
                    Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 72, num18);
                    continue;
                }

                int num19 = (int)num15;
                if (num19 > 50 && Main.rand.NextBool(2))
                    num19 /= Main.rand.Next(3) + 1;

                if (Main.rand.NextBool(2))
                    num19 /= Main.rand.Next(4) + 1;

                if (num19 < 1)
                    num19 = 1;

                num15 -= num19;
                Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, 71, num19);
            }
        }
    }
}
