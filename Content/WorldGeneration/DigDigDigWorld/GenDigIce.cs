using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigTheIce { get; set; }

        public static void GenDigIce(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigTheIce.Value;

            GenSnowBar(progress);
            GenSnowBlur(progress);
            GenIceSpikes(progress);
            GenSnowWall(progress);
        }

        private static void GenSnowBar(GenerationProgress progress)
        {
            int snowside = GenVars.dungeonSide * -1;

            int center = Main.maxTilesX / 2;
            int width = Main.maxTilesX / 12;
            int offset = Main.maxTilesX / 150;

            int snowCenter = center + snowside * (Main.maxTilesX / 6 + WorldGen.genRand.Next(-offset, offset));
            GenVars.snowOriginLeft = snowCenter - width / 2;
            GenVars.snowOriginRight = snowCenter + width / 2;

            for (int i = 0; i < width; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile t = Main.tile[GenVars.snowOriginLeft + i, j];
                        t.ResetToType(TileID.SnowBlock);
                }
            
            progress.Value = 0.25f;
        }

        /// <summary>
        /// 为雪地添加一层渐变
        /// </summary>
        /// <param name="progress"></param>
        private static void GenSnowBlur(GenerationProgress progress)
        {
            int x1 = GenVars.snowOriginLeft;
            int x2 = GenVars.snowOriginRight;

            var wr = BlurRandom(20);

            for (int j = (int)(Main.maxTilesY * 0.01f); j < Main.maxTilesY * 0.99f; j++)
            {
                for (int i = 0; i < 7; i++)
                {
                    Main.tile[x1 - wr.Get(), j].ResetToType(TileID.SnowBlock);
                    Main.tile[x2 - 1 + wr.Get(), j].ResetToType(TileID.SnowBlock);
                }
            }

            progress.Value = 0.5f;
        }

        private static void PlaceSnowSiltBall(GenerationProgress progress)
        {
            //生成雪泥块的球
            int ballCount;
            if (Main.maxTilesY > 8000)
                ballCount = 200;
            else if (Main.maxTilesX > 6000)
                ballCount = 150;
            else
                ballCount = 100;

            for (int i = 0; i < ballCount; i++)
            {
                int x = WorldGen.genRand.Next(GenVars.snowOriginLeft, GenVars.snowOriginRight);
                int y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.2f), (int)(Main.maxTilesY * 0.95f));

                int width = WorldGen.genRand.Next(4, 15);
                int height = WorldGen.genRand.Next(4, 15);

                WorldUtils.Gen(
                    new Point(x, y),  //中心点
                    new Shapes.Circle(width, height),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        new Modifiers.Blotches(4, 0.4),     //添加边缘的抖动，让边缘处不那么平滑
                        new Modifiers.OnlyTiles(TileID.SnowBlock),
                        new Actions.SetTile(TileID.Slush)));    //清除形状内所有物块

            }

            progress.Value = 0.7f;
        }

        /// <summary>
        /// 生成一些冰刺
        /// </summary>
        /// <param name="progress"></param>
        private static void GenIceSpikes(GenerationProgress progress)
        {
            int width = Main.maxTilesX / 12;
            int minWidth = width / 5;
            int maxWidth = width * 3 / 5;

            int x1 = GenVars.snowOriginLeft - 12;
            int x2 = GenVars.snowOriginRight + 12;

            int spikeCount;

            if (Main.maxTilesY > 8000)
                spikeCount = 140;
            else if (Main.maxTilesX > 6000)
                spikeCount = 100;
            else
                spikeCount = 80;

            for (int i = 0; i < spikeCount / 2; i++)
            {
                int y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.2f), (int)(Main.maxTilesY * 0.95f));

                WorldUtils.Gen(
                    new Point(x1, y),  //中心点
                    new Shapes.Tail(WorldGen.genRand.Next(6,30)
                    ,new ReLogic.Utilities.Vector2D(1,-1)*WorldGen.genRand.Next(minWidth, maxWidth)),   //形状：三角锥
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        new Modifiers.Blotches(2, 0.4),     //添加边缘的抖动，让边缘处不那么平滑
                        new Modifiers.OnlyTiles(TileID.SnowBlock, TileID.Slush),
                        new Actions.SetTile(WorldGen.genRand.NextFromList(TileID.IceBlock, TileID.BreakableIce))));    //放置墙壁


                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.2f), (int)(Main.maxTilesY * 0.95f));

                WorldUtils.Gen(
                    new Point(x2, y),  //中心点
                    new Shapes.Tail(WorldGen.genRand.Next(6, 30)
                    , new ReLogic.Utilities.Vector2D(-1, -1) * WorldGen.genRand.Next(minWidth, maxWidth)),   //形状：三角锥
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        new Modifiers.Blotches(2, 0.4),     //添加边缘的抖动，让边缘处不那么平滑
                        new Modifiers.OnlyTiles(TileID.SnowBlock, TileID.Slush),
                        new Actions.SetTile(WorldGen.genRand.NextFromList(TileID.IceBlock, TileID.BreakableIce))));    //放置墙壁


                progress.Value += 0.15f / (spikeCount / 2);
            }
        }

        private static void GenSnowWall(GenerationProgress progress)
        {
            int wallCount;

            if (Main.maxTilesY > 8000)
                wallCount = 400;
            else if (Main.maxTilesX > 6000)
                wallCount = 300;
            else
                wallCount = 250;

            for (int i = GenVars.snowOriginLeft; i < GenVars.snowOriginRight; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                    Main.tile[i, j].Clear(Terraria.DataStructures.TileDataType.Wall);

            for (int i = 0; i < wallCount; i++)
            {
                int ballCount = WorldGen.genRand.Next(3, 7);
                ushort wallType = WorldGen.genRand.NextFromList(WallID.SnowWallUnsafe, WallID.IceUnsafe);

                int originX = WorldGen.genRand.Next(GenVars.snowOriginLeft, GenVars.snowOriginRight);
                int originY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.2f), (int)(Main.maxTilesY * 0.8f));
                int x = originX;
                int y = originY;

                int width = WorldGen.genRand.Next(4, 24);
                int height = WorldGen.genRand.Next(4, 24);

                WorldUtils.Gen(
                    new Point(x, y),  //中心点
                    new Shapes.Circle(width, height),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        new Modifiers.Blotches(2, 0.4)//添加边缘的抖动，让边缘处不那么平滑
                        , new Modifiers.OnlyTiles(TileID.SnowBlock, TileID.IceBlock,TileID.BreakableIce),     //仅限丛林
                        new Actions.PlaceWall(wallType)));    //放置墙壁

                for (int j = 0; j < ballCount; j++)
                {
                    Vector2 dir = WorldGen.genRand.NextFloat(6.282f).ToRotationVector2() * WorldGen.genRand.NextFloat(4, 15);
                    x = originX + (int)(dir.X);
                    y = originY + (int)(dir.Y);

                    width = WorldGen.genRand.Next(4, 16);
                    height = WorldGen.genRand.Next(4, 16);

                    WorldUtils.Gen(
                        new Point(x, y),  //中心点
                        new Shapes.Circle(width, height),   //形状：圆
                        Actions.Chain(  //如果要添加多个效果得使用这个chain
                        new Modifiers.Blotches(2, 0.4)//添加边缘的抖动，让边缘处不那么平滑
                        , new Modifiers.OnlyTiles(TileID.SnowBlock, TileID.IceBlock, TileID.BreakableIce),     //仅限丛林
                            new Actions.PlaceWall(wallType)));    //放置墙壁

                }

                progress.Value += 0.15f / wallCount;
            }
        }
    }
}
