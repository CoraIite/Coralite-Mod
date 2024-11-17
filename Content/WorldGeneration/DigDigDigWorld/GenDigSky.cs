using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigSky { get; set; }

        private static int DigSkyWidth;

        public static void GenDigSky(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigSky.Value;

            DirtBar(progress);
            DirtBlur(progress);
            ClayBall(progress);
            DirtWall(progress);

            CloudBar(progress);
            CloudBall(progress);
            RainCloudBall(progress);
        }

        private static void DirtBar(GenerationProgress progress)
        {
            DigSkyWidth = Main.maxTilesX / 14 + WorldGen.genRand.Next(30);

            int skyside = GenVars.dungeonSide;
            int x = skyside > 0 ? Main.maxTilesX : 0;

            for (int i = 0; i < DigHellWidth; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile t = Main.tile[x - i * skyside, j];
                    if (WorldGen.genRand.NextBool(40))
                        t.ResetToType(TileID.Grass);
                    else
                        t.ResetToType(TileID.Dirt);
                }

            progress.Value += 0.125f;
        }

        private static void DirtBlur(GenerationProgress progress)
        {
            int skyside = GenVars.dungeonSide;
            int x = skyside > 0 ? Main.maxTilesX : 0;

            x -= DigHellWidth * skyside;

            var wr = BlurRandom(12);

            for (int j = 0; j < Main.maxTilesY; j++)
            {
                int width = WorldGen.genRand.Next(20);

                for (int i = 0; i < width; i++)//随机长度的刺刺
                {
                    Tile t = Main.tile[x - skyside * i, j];
                    t.ResetToType(TileID.Dirt);
                }

                for (int i = 0; i < 6; i++)
                    Main.tile[x - skyside * (wr.Get() + width), j].ResetToType(TileID.Dirt);
            }

            progress.Value += 0.125f;
        }

        private static void ClayBall(GenerationProgress progress)
        {
            int ballCount;
            if (Main.maxTilesY > 8000)
                ballCount = 500;
            else if (Main.maxTilesX > 6000)
                ballCount = 350;
            else
                ballCount = 200;

            Modifiers.Dither Blotches = new Modifiers.Dither(0.25);
            Modifiers.OnlyTiles onlyTiles = new Modifiers.OnlyTiles(TileID.Dirt, TileID.Grass);
            Actions.SetTile setTile = new Actions.SetTile(TileID.ClayBlock);

            for (int i = 0; i < ballCount; i++)
            {
                int x = GenVars.dungeonSide > 0
                    ? Main.maxTilesX - WorldGen.genRand.Next(DigSkyWidth / 2, DigSkyWidth)
                    : WorldGen.genRand.Next(DigSkyWidth / 2, DigSkyWidth);
                int y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.1f), (int)(Main.maxTilesY * 0.95f));

                int width = WorldGen.genRand.Next(4, 12);
                int height = WorldGen.genRand.Next(4, 12);

                WorldUtils.Gen(
                    new Point(x, y),  //中心点
                    new Shapes.Circle(width, height),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        Blotches,     //添加边缘的抖动，让边缘处不那么平滑
                        onlyTiles,
                        setTile));    //清除形状内所有物块

                progress.Value += 0.125f / ballCount;
            }
        }

        private static void DirtWall(GenerationProgress progress)
        {
            int wallCount;

            if (Main.maxTilesY > 8000)
                wallCount = 400;
            else if (Main.maxTilesX > 6000)
                wallCount = 300;
            else
                wallCount = 250;

            int left = GenVars.dungeonSide > 0 ? Main.maxTilesX - DigSkyWidth : DigSkyWidth / 2;
            int right = GenVars.dungeonSide > 0 ? Main.maxTilesX - DigSkyWidth / 2 : DigSkyWidth;

            for (int i = left; i < right; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                    Main.tile[i, j].Clear(Terraria.DataStructures.TileDataType.Wall);

            Modifiers.Blotches actions = new Modifiers.Blotches(2, 0.4);
            Modifiers.OnlyTiles onlyTiles = new Modifiers.OnlyTiles(TileID.Dirt, TileID.ClayBlock, TileID.Grass);

            for (int i = 0; i < wallCount; i++)
            {
                int ballCount = WorldGen.genRand.Next(3, 7);
                ushort wallType = WorldGen.genRand.NextFromList(WallID.DirtUnsafe, WallID.DirtUnsafe1
                    , WallID.DirtUnsafe2, WallID.DirtUnsafe3, WallID.DirtUnsafe4);

                int originX = GenVars.dungeonSide > 0
                    ? Main.maxTilesX - WorldGen.genRand.Next(DigSkyWidth / 2, DigSkyWidth)
                    : WorldGen.genRand.Next(DigSkyWidth / 2, DigSkyWidth);
                int originY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.2f), (int)(Main.maxTilesY * 0.8f));
                int x = originX;
                int y = originY;

                int width = WorldGen.genRand.Next(4, 18);
                int height = WorldGen.genRand.Next(4, 18);

                WorldUtils.Gen(
                    new Point(x, y),  //中心点
                    new Shapes.Circle(width, height),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        actions//添加边缘的抖动，让边缘处不那么平滑
                        , onlyTiles,     //仅限泥土
                        new Actions.PlaceWall(wallType)));    //放置墙壁

                for (int j = 0; j < ballCount; j++)
                {
                    Vector2 dir = WorldGen.genRand.NextFloat(6.282f).ToRotationVector2() * WorldGen.genRand.NextFloat(4, 15);
                    x = originX + (int)(dir.X);
                    y = originY + (int)(dir.Y);

                    width = WorldGen.genRand.Next(4, 10);
                    height = WorldGen.genRand.Next(4, 10);

                    WorldUtils.Gen(
                        new Point(x, y),  //中心点
                        new Shapes.Circle(width, height),   //形状：圆
                        Actions.Chain(  //如果要添加多个效果得使用这个chain
                            actions//添加边缘的抖动，让边缘处不那么平滑
                            , onlyTiles,     //仅限泥土
                            new Actions.PlaceWall(wallType)));    //放置墙壁
                }

                progress.Value += 0.125f / wallCount;
            }
        }

        private static void CloudBar(GenerationProgress progress)
        {
            int width = DigSkyWidth / 2;

            int skyside = GenVars.dungeonSide;
            int x = skyside > 0 ? Main.maxTilesX : 0;

            for (int i = 0; i < width; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile t = Main.tile[x - i * skyside, j];
                    t.ResetToType(TileID.Cloud);
                    t.Clear(Terraria.DataStructures.TileDataType.Wall);
                }

            progress.Value += 0.5f / 3;
        }

        private static void CloudBall(GenerationProgress progress)
        {
            int ballCount;
            if (Main.maxTilesY > 8000)
                ballCount = 800;
            else if (Main.maxTilesX > 6000)
                ballCount = 450;
            else
                ballCount = 200;

            int x = GenVars.dungeonSide > 0 ? Main.maxTilesX - DigSkyWidth / 2 : DigSkyWidth / 2;

            Actions.SetTile setTile = new Actions.SetTile(TileID.Cloud);

            for (int i = 0; i < ballCount; i++)
            {
                int y = WorldGen.genRand.Next(15, Main.maxTilesY - 15);

                int width = WorldGen.genRand.Next(4, 15);

                WorldUtils.Gen(
                    new Point(x + WorldGen.genRand.Next(-10, 10), y),  //中心点
                    new Shapes.Circle(width, width),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        setTile));    //设置云朵块

                progress.Value += 0.5f / 3 / ballCount;
            }
        }

        private static void RainCloudBall(GenerationProgress progress)
        {
            int ballCount;
            if (Main.maxTilesY > 8000)
                ballCount = 650;
            else if (Main.maxTilesX > 6000)
                ballCount = 350;
            else
                ballCount = 150;

            Modifiers.Blotches Blotches = new Modifiers.Blotches(2, 0.5);
            Modifiers.Blotches Blotches2 = new Modifiers.Blotches(1, 0.4);
            Modifiers.Flip flip = new Modifiers.Flip(false, true);
            Modifiers.OnlyTiles onlyTiles = new Modifiers.OnlyTiles(TileID.Cloud);
            Actions.SetTile setTile = new Actions.SetTile(TileID.RainCloud);
            Actions.ClearTile clearTile = new Actions.ClearTile();
            Actions.SetLiquid setLiquid = new Actions.SetLiquid(LiquidID.Water);

            for (int i = 0; i < ballCount; i++)
            {
                int x = GenVars.dungeonSide > 0
                    ? Main.maxTilesX - WorldGen.genRand.Next(10, DigSkyWidth / 2)
                    : WorldGen.genRand.Next(10, DigSkyWidth / 2);
                int y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.1f), (int)(Main.maxTilesY * 0.8f));

                int width = WorldGen.genRand.Next(3, 8);

                ShapeData data = new();

                WorldUtils.Gen(
                    new Point(x, y),  //中心点
                    new Shapes.Slime(width, 1, WorldGen.genRand.NextFloat(0.8f, 1)),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        Blotches,     //添加边缘的抖动，让边缘处不那么平滑
                        flip,
                        onlyTiles,
                        clearTile,
                        setLiquid.Output(data)));    //防止水

                WorldUtils.Gen(
                new Point(x, y),  //中心点
                    new ModShapes.OuterOutline(data),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        new Modifiers.Expand(WorldGen.genRand.Next(1, 2), WorldGen.genRand.Next(1, 2)), //向外扩展
                        Blotches2,
                        onlyTiles,
                        setTile));    //放置雨云

                progress.Value += 0.5f / 3 / ballCount;
            }
        }
    }
}
