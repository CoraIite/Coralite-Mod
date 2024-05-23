using Coralite.Content.Tiles.Magike;
using Coralite.Content.Walls.Magike;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public void GenBasaltSmallBiome(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "正在生成玄武岩";

            ushort basalt = (ushort)ModContent.TileType<BasaltTile>();
            ushort hardBasalt = (ushort)ModContent.TileType<HardBasaltTile>();
            ushort crystalBasalt = (ushort)ModContent.TileType<CrystalBasaltTile>();


            WorldGenHelper.GenerateOre(basalt, 0.0001, 0.5f, 0.8f
                , (int x, int y) =>
                {
                    return Main.tile[x, y].TileType == TileID.Stone
                    && x > Main.maxTilesX / 3 && x < Main.maxTilesX * 2 / 3;
                });

            progress.Value = 0.2f;
            WorldGenHelper.GenerateOre(hardBasalt, 0.00008, 0.5f, 0.8f
                , (int x, int y) =>
                {
                    return Main.tile[x, y].TileType == basalt
                    && x > Main.maxTilesX / 3 && x < Main.maxTilesX * 2 / 3;
                });

            progress.Value = 0.4f;
            WorldGenHelper.GenerateOre(crystalBasalt, 0.00006, 0.5f, 0.8f
                , (int x, int y) =>
                {
                    return (Main.tile[x, y].TileType == basalt || Main.tile[x, y].TileType == hardBasalt)
                    && x > Main.maxTilesX / 3 && x < Main.maxTilesX * 2 / 3;
                });

            try
            {
                WorldGen.maxTileCount = 300;
                double spawns = Main.maxTilesX * 0.003;
                if (WorldGen.tenthAnniversaryWorldGen)
                    spawns *= 1.5;

                if (Main.starGame)
                    spawns *= Main.starGameMath(0.2);

                for (int k = 0; k < spawns; k++)
                {
                    double value4 = k / spawns;
                    progress.Set(value4 * 0.6f + 0.4f);
                    int tryCount = 0;
                    int x9 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
                    int y7 = WorldGen.genRand.Next(Main.maxTilesY / 2, Main.maxTilesY - 300);
                    if (WorldGen.remixWorldGen)
                        y7 = WorldGen.genRand.Next((int)Main.worldSurface + 30, (int)Main.rockLayer - 30);

                    int tileCount = WorldGen.countTiles(x9, y7);
                    while ((tileCount >= 300 || tileCount < 30 || WorldGen.lavaCount > 0 || WorldGen.iceCount > 0 || WorldGen.rockCount == 0) && tryCount < 1000)
                    {
                        tryCount++;
                        x9 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
                        y7 = WorldGen.genRand.Next(Main.maxTilesY / 2, Main.maxTilesY - 300);
                        if (WorldGen.remixWorldGen)
                            y7 = WorldGen.genRand.Next((int)Main.worldSurface + 30, (int)Main.rockLayer - 30);

                        tileCount = WorldGen.countTiles(x9, y7);
                    }

                    if (tryCount < 1000)
                        BasaltCave(x9, y7, WorldGen.genRand.NextFromList(basalt, hardBasalt));
                }
            }
            catch (System.Exception)
            {

            }
        }

        public static void BasaltCave(int x, int y,ushort tileType)
        {
            if (!WorldGen.InWorld(x, y))
                return;

            List<Point> list = new List<Point>();
            List<Point> list2 = new List<Point>();
            HashSet<Point> hashSet = new HashSet<Point>();
            list2.Add(new Point(x, y));
            while (list2.Count > 0)
            {
                list.Clear();
                list.AddRange(list2);
                list2.Clear();
                while (list.Count > 0)
                {
                    Point item = list[0];
                    if (!WorldGen.InWorld(item.X, item.Y, 1))
                    {
                        list.Remove(item);
                        continue;
                    }

                    hashSet.Add(item);
                    list.Remove(item);
                    Tile tile = Main.tile[item.X, item.Y];
                    if (WorldGen.SolidTile(item.X, item.Y) || tile.WallType != 0)
                    {
                        if (tile.HasTile)
                        {
                            if (Gemmable(tile.TileType))
                                tile.ResetToType(tileType);

                            if (WorldGen.genRand.NextBool(6))
                                tile.ResetToType((ushort)ModContent.TileType<CrystalBasaltTile>());
                        }
                    }
                    else
                    {
                        Main.tile[item.X, item.Y].Clear(Terraria.DataStructures.TileDataType.Wall);
                        WorldGen.PlaceWall(item.X, item.Y, ModContent.WallType<HardBasaltWall>());

                        {
                            Point item2 = new Point(item.X - 1, item.Y);
                            if (!hashSet.Contains(item2))
                                list2.Add(item2);

                            item2 = new Point(item.X + 1, item.Y);
                            if (!hashSet.Contains(item2))
                                list2.Add(item2);

                            item2 = new Point(item.X, item.Y - 1);
                            if (!hashSet.Contains(item2))
                                list2.Add(item2);

                            item2 = new Point(item.X, item.Y + 1);
                            if (!hashSet.Contains(item2))
                                list2.Add(item2);
                        }

                        for (int i = 2; i < 4; i++)
                        {
                            Point item2 = new Point(item.X - i, item.Y);
                            if (Main.tile[item2.X, item2.Y].HasTile && !hashSet.Contains(item2))
                                list2.Add(item2);

                            item2 = new Point(item.X + i, item.Y);
                            if (Main.tile[item2.X, item2.Y].HasTile && !hashSet.Contains(item2))
                                list2.Add(item2);

                            item2 = new Point(item.X, item.Y - i);
                            if (Main.tile[item2.X, item2.Y].HasTile && !hashSet.Contains(item2))
                                list2.Add(item2);

                            item2 = new Point(item.X, item.Y + i);
                            if (Main.tile[item2.X, item2.Y].HasTile && !hashSet.Contains(item2))
                                list2.Add(item2);
                        }
                    }
                }
            }
        }

        private static bool Gemmable(int type)
        {
            if (type != 0 && type != 1 && type != 40 && type != 59 && type != 60 && type != 70 && type != 147)
                return type == 161;

            return true;
        }
    }
}
