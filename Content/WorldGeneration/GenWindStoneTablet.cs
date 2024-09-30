using Coralite.Content.Items.FairyCatcher;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText WindStoneTablet { get; set; }

        public void GenWindStoneTablet(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = WindStoneTablet.Value;// "正在生成风石碑牌";

            int tileType = ModContent.TileType<VineStoneTabletTile>();

            int spawnCount = 0;

            try
            {
                for (int i = 0; i < 20000; i++)
                {
                    //随机选取点
                    int tabletCenter_x = (Main.maxTilesX / 2) + (WorldGen.genRand.NextFromList(-1, 1) * Main.maxTilesX / 3);
                    tabletCenter_x += WorldGen.genRand.Next(-Main.maxTilesX / 7, Main.maxTilesX / 7);
                    int tabletCenter_y = (int)(Main.worldSurface * 0.4f);

                    for (; tabletCenter_y < Main.worldSurface; tabletCenter_y++)
                    {
                        Tile tile = Framing.GetTileSafely(tabletCenter_x, tabletCenter_y);
                        if (tile.HasTile && Main.tileSolid[tile.TileType] && tile.TileType != TileID.Cloud
                            && tile.TileType != TileID.RainCloud && tile.TileType != TileID.Sunplate
                            && tile.TileType != TileID.Containers && tile.TileType != TileID.Dirt)
                            break;
                    }

                    Tile tile2 = Framing.GetTileSafely(tabletCenter_x, tabletCenter_y);
                    if (!TileID.Sets.Grass[tile2.TileType] && !TileID.Sets.Dirt[tile2.TileType])
                        continue;

                    Point position = new(tabletCenter_x, tabletCenter_y);

                    Dictionary<ushort, int> tileDictionary = new();
                    if (!WorldGen.InWorld(position.X - 30, position.Y - 15) || !WorldGen.InWorld(position.X + 30, position.Y + 15))
                        continue;
                    WorldUtils.Gen(
                        new Point(position.X - 30, position.Y - 15),
                        new Shapes.Rectangle(60, 30),
                        new Actions.TileScanner(TileID.FallenLog).Output(tileDictionary));

                    if (tileDictionary[TileID.FallenLog] < 1)
                        continue; //如果不是，则返回false，这将导致调用方法尝试一个不同的origin。

                    WorldGen.PlaceObject(tabletCenter_x, tabletCenter_y - 1, tileType);
                    WorldGen.PlaceObject(tabletCenter_x, tabletCenter_y - 2, tileType);
                    spawnCount++;
                    if (spawnCount > 6)
                        return;
                }

                bool spawned = true;
                while (spawned)
                {
                    int tabletCenter_x = (Main.maxTilesX / 2) + (WorldGen.genRand.NextFromList(-1, 1) * Main.maxTilesX / 3);
                    tabletCenter_x += WorldGen.genRand.Next(-Main.maxTilesX / 7, Main.maxTilesX / 7);
                    int tabletCenter_y = (int)(Main.worldSurface * 0.4f);

                    for (; tabletCenter_y < Main.worldSurface; tabletCenter_y++)
                    {
                        Tile tile = Framing.GetTileSafely(tabletCenter_x, tabletCenter_y);
                        if (tile.HasTile && Main.tileSolid[tile.TileType] && tile.TileType != TileID.Cloud
                            && tile.TileType != TileID.RainCloud && tile.TileType != TileID.Sunplate
                            && tile.TileType != TileID.Containers && tile.TileType != TileID.Dirt)
                            break;
                    }

                    Tile tile2 = Framing.GetTileSafely(tabletCenter_x, tabletCenter_y);
                    if (!TileID.Sets.Grass[tile2.TileType] && !TileID.Sets.Dirt[tile2.TileType])
                        continue;

                    WorldGen.PlaceObject(tabletCenter_x, tabletCenter_y - 1, tileType);
                    WorldGen.PlaceObject(tabletCenter_x, tabletCenter_y - 2, tileType);
                    spawned = false;
                }

            }
            catch (System.Exception)
            {

            }
        }
    }
}
