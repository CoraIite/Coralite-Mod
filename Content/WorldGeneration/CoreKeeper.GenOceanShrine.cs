using Coralite.Content.Items.CoreKeeper;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public void GenOceanShrine(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Set(0);

            int center_x = 100;
            int center_y = (int)(Main.worldSurface * 0.4f);

            for (; center_y < Main.worldSurface; center_y++)
            {
                Tile tile = Framing.GetTileSafely(center_x, center_y);
                if (tile.HasTile && tile.TileType != TileID.Cloud
                    && tile.TileType != TileID.RainCloud && tile.TileType != TileID.Sunplate
                    && tile.TileType != TileID.Containers && tile.TileType != TileID.Dirt
                    && tile.TileType != TileID.Grass && !TileID.Sets.Ore[tile.TileType])
                    break;
            }

            //center_y += 6;

            TextureGenerator generator = new TextureGenerator("LeftOceanShrine", path: AssetDirectory.WorldGen + "CoreKeeper/");

            int genOrigin_x = center_x - (generator.Width / 2);
            int genOrigin_y = center_y - (generator.Height / 2);

            Dictionary<Color, int> nestDic = new()
            {
                [new Color(215, 123, 186)] = TileID.ReefBlock,
                [new Color(217, 87, 99)] = TileID.Coralstone,
                [new Color(105, 106, 106)] = TileID.GrayBrick,
            };
            Dictionary<Color, int> wallDic = new()
            {
                [new Color(148, 103, 133)] = WallID.ReefWall,
                [new Color(83, 103, 103)] = WallID.GrayBrick,
                [new Color(60, 139, 139)] = WallID.CaveUnsafe,
            };

            generator.GenerateByTopLeft(new Point(genOrigin_x, genOrigin_y), nestDic, wallDic
                , ObjectPlace);

            center_x = Main.maxTilesX - 100;
            center_y = (int)(Main.worldSurface * 0.4f);

            for (; center_y < Main.worldSurface; center_y++)
            {
                Tile tile = Framing.GetTileSafely(center_x, center_y);
                if (tile.HasTile && tile.TileType != TileID.Cloud
                    && tile.TileType != TileID.RainCloud && tile.TileType != TileID.Sunplate
                    && tile.TileType != TileID.Containers && tile.TileType != TileID.Dirt
                    && tile.TileType != TileID.Grass && !TileID.Sets.Ore[tile.TileType])
                    break;
            }

            //center_y += 7;
            generator = new TextureGenerator("RightOceanShrine", path: AssetDirectory.WorldGen + "CoreKeeper/");
            genOrigin_x = center_x - (generator.Width / 2);
            genOrigin_y = center_y - (generator.Height / 2);

            generator.GenerateByTopLeft(new Point(genOrigin_x, genOrigin_y), nestDic, wallDic
                , ObjectPlace);

            static void ObjectPlace(Color c, int x, int y)
            {
                if (c == Color.White)
                    WorldGen.PlaceObject(x, y, TileID.Torches, true, 17);//珊瑚火把
                else if (c == new Color(153, 229, 80))
                    WorldGen.PlaceObject(x, y, TileID.Lamps, true, 40);//珊瑚火炬
                else if (c == new Color(251, 242, 54))//能量丝
                    WorldGen.AddBuriedChest(x, y, ModContent.ItemType<EnergyString>()
                      , notNearOtherChests: false, 14, trySlope: false, TileID.Containers2);
                else if (c == new Color(255, 0, 0))//弓臂碎片
                    WorldGen.AddBuriedChest(x, y, ModContent.ItemType<FracturedLimbs>()
                      , notNearOtherChests: false, 14, trySlope: false, TileID.Containers2);
            }
        }
    }
}
