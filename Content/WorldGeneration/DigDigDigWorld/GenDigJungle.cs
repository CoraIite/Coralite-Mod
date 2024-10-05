using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigTheJungle { get; set; }

        public void GenDigJungle(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigTheJungle.Value;

            GenJungleBar(progress);
            GenJungleBlur(progress);
            GenJungleWall(progress);
        }

        /// <summary>
        /// 生成丛林泥块
        /// </summary>
        /// <param name="progress"></param>
        private static void GenJungleBar(GenerationProgress progress)
        {
            int jungleside = GenVars.dungeonSide * -1;

            int center = Main.maxTilesX / 2;
            int width = Main.maxTilesX / 7;
            int offset = Main.maxTilesX / 100;

            GenVars.jungleOriginX = center + jungleside * (Main.maxTilesX * 2 / 7 + WorldGen.genRand.Next(-offset, offset));
            GenVars.jungleMinX = GenVars.jungleOriginX - width / 2;
            GenVars.jungleMaxX = GenVars.jungleOriginX + width / 2;

            for (int i = 0; i < width; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile t = Main.tile[GenVars.jungleMinX + i, j];
                    if (WorldGen.genRand.NextBool(30))
                        t.ResetToType(TileID.JungleGrass);
                    else
                        t.ResetToType(TileID.Mud);
                }

            progress.Value = 0.3f;
        }

        /// <summary>
        /// 为丛林边缘添加一层渐变
        /// </summary>
        /// <param name="progress"></param>
        private static void GenJungleBlur(GenerationProgress progress)
        {
            int x1 = GenVars.jungleMinX;
            int x2 = GenVars.jungleMaxX;

            var wr = BlurRandom(20);

            for (int j = (int)(Main.maxTilesY * 0.01f); j < Main.maxTilesY * 0.99f; j++)
            {
                for (int i = 0; i < 7; i++)
                {
                    Main.tile[x1 - wr.Get(), j].ResetToType(TileID.Mud);
                    Main.tile[x2 - 1 + wr.Get(), j].ResetToType(TileID.Mud);
                }
            }

            progress.Value = 0.6f;
        }

        /// <summary>
        /// 生成丛林墙壁
        /// </summary>
        /// <param name="progress"></param>
        private static void GenJungleWall(GenerationProgress progress)
        {
            int wallCount;

            if (Main.maxTilesY > 8000)
                wallCount = 500;
            else if (Main.maxTilesX > 6000)
                wallCount = 400;
            else
                wallCount = 300;

            for (int i = GenVars.jungleMinX; i < GenVars.jungleMaxX; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                    Main.tile[i, j].Clear(Terraria.DataStructures.TileDataType.Wall);

            for (int i = 0; i < wallCount; i++)
            {
                int ballCount = WorldGen.genRand.Next(3, 7);
                ushort wallType = WorldGen.genRand.NextFromList(WallID.JungleUnsafe, WallID.JungleUnsafe1, WallID.JungleUnsafe2, WallID.JungleUnsafe3,
                    WallID.JungleUnsafe4);

                int originX = WorldGen.genRand.Next(GenVars.jungleMinX, GenVars.jungleMaxX);
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
                        , new Modifiers.OnlyTiles(TileID.Mud, TileID.JungleGrass),     //仅限丛林
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
                        , new Modifiers.OnlyTiles(TileID.Mud, TileID.JungleGrass),     //仅限丛林
                            new Actions.PlaceWall(wallType)));    //放置墙壁
                }

                progress.Value += 0.4f / wallCount;
            }
        }

        private static WeightedRandom<int> BlurRandom(int distance, ISmoother smoother = null)
        {
            var wr = new WeightedRandom<int>(WorldGen.genRand);
            smoother ??= Coralite.Instance.X2Smoother;

            for (int i = 1; i < distance; i++)
                wr.Add(i, smoother.Smoother(1 - i / (float)distance) * 3);

            return wr;
        }
    }
}
