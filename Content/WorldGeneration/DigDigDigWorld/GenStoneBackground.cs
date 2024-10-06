﻿using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText StoneBack { get; set; }

        public void GenShoneBack(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = StoneBack.Value;

            FillStone(progress);//填石头
            PlaceSiltBall(progress);//填泥沙块
            FillStoneWall(progress);//填墙壁
        }

        /// <summary>
        /// 将世界填充满石头
        /// </summary>
        /// <param name="progress"></param>
        private static void FillStone(GenerationProgress progress)
        {
            for (int i = 0; i < Main.maxTilesX; i++)//全部填充石头
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Main.tile[i, j].ResetToType(TileID.Stone);
                }

            progress.Value = 0.2f;
        }

        private static void PlaceSiltBall(GenerationProgress progress)
        {
            //生成泥沙块的球
            int ballCount;
            if (Main.maxTilesY > 8000)
                ballCount = 1050;
            else if (Main.maxTilesX > 6000)
                ballCount = 750;
            else
                ballCount = 500;

            Modifiers.Blotches actions = new Modifiers.Blotches(4, 0.4);
            Actions.SetTile setTile = new Actions.SetTile(TileID.Silt);

            for (int i = 0; i < ballCount; i++)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
                int y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.1f), (int)(Main.maxTilesY * 0.8f));

                int width = WorldGen.genRand.Next(4, 15);
                int height = WorldGen.genRand.Next(4, 15);

                WorldUtils.Gen(
                    new Point(x, y),  //中心点
                    new Shapes.Circle(width, height),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        actions,     //添加边缘的抖动，让边缘处不那么平滑
                        setTile));    //清除形状内所有物块

                progress.Value += 0.6f / ballCount;
            }

            progress.Value = 0.8f;
        }

        private static void FillStoneWall(GenerationProgress progress)
        {
            int wallCount;

            if (Main.maxTilesY > 8000)
                wallCount = 1000;
            else if (Main.maxTilesX > 6000)
                wallCount = 800;
            else
                wallCount = 600;

            Modifiers.Blotches actions = new Modifiers.Blotches(2, 0.4);

            for (int i = 0; i < wallCount; i++)
            {
                int ballCount = WorldGen.genRand.Next(3, 7);
                ushort wallType = WorldGen.genRand.NextFromList(WallID.CaveUnsafe, WallID.Cave2Unsafe, WallID.Cave3Unsafe, WallID.Cave4Unsafe,
                    WallID.Cave5Unsafe, WallID.Cave6Unsafe, WallID.Cave7Unsafe, WallID.Cave8Unsafe,
                    WallID.RocksUnsafe1, WallID.RocksUnsafe2, WallID.RocksUnsafe3, WallID.RocksUnsafe4);

                int originX = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.15f), (int)(Main.maxTilesX * 0.85f));
                int originY = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.2f), (int)(Main.maxTilesY * 0.8f));
                int x = originX;
                int y = originY;

                int width = WorldGen.genRand.Next(4, 12);
                int height = WorldGen.genRand.Next(4, 12);

                WorldUtils.Gen(
                    new Point(x, y),  //中心点
                    new Shapes.Circle(width, height),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        actions,     //添加边缘的抖动，让边缘处不那么平滑
                        new Actions.PlaceWall(wallType)));    //放置墙壁


                for (int j = 0; j < ballCount; j++)
                {
                    Vector2 dir = WorldGen.genRand.NextFloat(6.282f).ToRotationVector2() * WorldGen.genRand.NextFloat(4, 15);
                    x = originX + (int)(dir.X);
                    y = originY + (int)(dir.Y);

                    width = WorldGen.genRand.Next(4, 12);
                    height = WorldGen.genRand.Next(4, 12);

                    WorldUtils.Gen(
                        new Point(x, y),  //中心点
                        new Shapes.Circle(width, height),   //形状：圆
                        Actions.Chain(  //如果要添加多个效果得使用这个chain
                            actions,     //添加边缘的抖动，让边缘处不那么平滑
                            new Actions.PlaceWall(wallType)));    //放置墙壁

                }

                progress.Value += 0.2f / wallCount;
            }
        }
    }
}
