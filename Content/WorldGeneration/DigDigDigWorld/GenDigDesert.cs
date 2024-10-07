using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigDesert { get; set; }

        public static void GenDigDesert(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigDesert.Value;

            GenDesertBar(progress);
            GenDesertBlur(progress);
            GenSandBall(progress);
            GenDesertWall(progress);
        }

        private static void GenDesertBar(GenerationProgress progress)
        {
            int desertside = GenVars.dungeonSide;

            int center = Main.maxTilesX / 2;
            int width = Main.maxTilesX / 10 + WorldGen.genRand.Next(-10, 25);
            int offset = Main.maxTilesX / 200;

            int desertCenter = center + desertside * (Main.maxTilesX / 5 + WorldGen.genRand.Next(-offset, offset));
            GenVars.desertHiveLeft = desertCenter - width / 2;
            GenVars.desertHiveRight = desertCenter + width / 2;

            for (int i = 0; i < width; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile t = Main.tile[GenVars.desertHiveLeft + i, j];
                    t.ResetToType(TileID.Sandstone);
                }

            progress.Value = 0.25f;
        }

        private static void GenDesertBlur(GenerationProgress progress)
        {
            int x1 = GenVars.desertHiveLeft;
            int x2 = GenVars.desertHiveRight;

            var wr = BlurRandom(30);

            for (int j = (int)(Main.maxTilesY * 0.01f); j < Main.maxTilesY * 0.99f; j++)
            {
                for (int i = 0; i < 15; i++)
                {
                    Main.tile[x1 - wr.Get(), j].ResetToType(TileID.Sandstone);
                    Main.tile[x2 - 1 + wr.Get(), j].ResetToType(TileID.Sandstone);
                }
            }

            progress.Value = 0.5f;
        }

        private static void GenSandBall(GenerationProgress progress)
        {
            //生成沙块的球
            int ballCount;
            if (Main.maxTilesY > 8000)
                ballCount = 600;
            else if (Main.maxTilesX > 6000)
                ballCount = 300;
            else
                ballCount = 250;

            var Blotches = new Modifiers.Blotches(2, 0.4);
            var OnlyTiles = new Modifiers.OnlyTiles(TileID.Sandstone);
            var SetTile1 = new Actions.SetTile(TileID.Sand);
            var SetTile2 = new Actions.SetTile(TileID.HardenedSand);

            for (int i = 0; i < ballCount; i++)
            {
                int x = WorldGen.genRand.Next(GenVars.desertHiveLeft, GenVars.desertHiveRight);
                int y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.1f), (int)(Main.maxTilesY * 0.95f));

                int width = WorldGen.genRand.Next(12, 24);
                int height = WorldGen.genRand.Next(2, 8);

                ShapeData data = new();

                WorldUtils.Gen(
                    new Point(x, y),  //中心点
                    new Shapes.Circle(width, height),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        Blotches,     //添加边缘的抖动，让边缘处不那么平滑
                        OnlyTiles,
                        SetTile1.Output(data)));    //放置沙块

                WorldUtils.Gen(
                    new Point(x, y),  //中心点
                    new ModShapes.OuterOutline(data),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        new Modifiers.Expand(WorldGen.genRand.Next(1, 10), WorldGen.genRand.Next(1, 4)), //向外扩展
                        OnlyTiles,
                        SetTile2));    //放置硬化沙子

                progress.Value += 0.25f / ballCount;
            }
        }

        private static void GenDesertWall(GenerationProgress progress)
        {
            for (int i = GenVars.desertHiveLeft - 30; i < GenVars.desertHiveRight + 30; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile t = Main.tile[i, j];
                    if (t.TileType is TileID.Sand or TileID.Sandstone or TileID.HardenedSand)
                    {
                        t.Clear(Terraria.DataStructures.TileDataType.Wall);
                        WorldGen.PlaceWall(i, j, WallID.Sandstone,true);
                    }
                }

                progress.Value += 0.25f / (GenVars.desertHiveLeft+60);
            }
        }
    }
}
