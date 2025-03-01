using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Content.Walls.Magike;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        private static int tileCounterNum;
        private static int tileCounterMax = 20;
        private static int[] tileCounterX = new int[tileCounterMax];
        private static int[] tileCounterY = new int[tileCounterMax];

        public static Vector2 AltarPos;

        /// <summary>
        /// 是否放置光明之魂
        /// </summary>
        public static bool PlaceLightSoul { get; set; }
        /// <summary>
        /// 是否放置暗影之魂
        /// </summary>
        public static bool PlaceNightSoul { get; set; }
        /// <summary>
        /// 是否有权限进入蕴魔空岛
        /// </summary>
        public static bool HasPermission { get; set; }

        public static LocalizedText CrystallineSkyIsland { get; set; }

        public void GenCrystallineSkyIsland(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = CrystallineSkyIsland.Value;

            PlaceLightSoul = false;
            PlaceNightSoul = false;
            HasPermission = false;

            //生成地表结构
            GenGroundLock(out Point altarPoint);
            progress.Value = 0.25f;

            //生成主体空岛
            GenMainSkyIsland(altarPoint,out Rectangle mainRect);
            progress.Value = 0.5f;

            //生成小岛
            GenSmallIslands(mainRect,out List<SmallIslandDatas> smallIslands);

            //生成主岛与地面遗迹之间的物块

        }

        #region 生成地面遗迹

        public void GenGroundLock(out Point altarPoint)
        {
            //找到丛林，在地表处选择一个地方
            Point p = new Point(0, 0);

            //在丛林中心寻找一个位置
            for (int i = 0; i < 100; i++)
            {
               Point p2 = new Point(PickAltarX(), (int)(Main.worldSurface * 0.4f));

                for (int j = 0; j < 500; j++)//向下遍历，找到地面
                {
                    Tile t = Main.tile[p2.X, p2.Y];
                    if ((t.HasTile && Main.tileSolid[t.TileType]&&t.TileType is not TileID.ClayBlock or TileID.Dirt or TileID.Grass or TileID.RainCloud)
                        || t.LiquidAmount > 0)//找到实心方块
                        break;

                    p2.Y++;
                }

                if (p == default)//查找附近最低的点
                    p = p2;
                else if (p2.Y > p.Y)
                    p = p2;
            }

            altarPoint = p + new Point(0, -8);

            ushort skarn = (ushort)ModContent.TileType<SkarnTile>();
            ushort smoothSkarn = (ushort)ModContent.TileType<SmoothSkarnTile>();
            ushort skarnBrick = (ushort)ModContent.TileType<SkarnBrickTile>();

            ushort crystallineBrick = (ushort)ModContent.TileType<CrystallineBrickTile>();

            TextureGenerator generator =new TextureGenerator("SkarnAltars",path: AssetDirectory.CrystallineSkyIsland);
            generator.SetWallTex();

            p -= new Point(generator.Width / 2, generator.Height / 2);

            Dictionary<Color, int> mainDic = new()
            {
                [new Color(51, 76, 117)] = skarn,//334c75
                [new Color(141, 171, 178)] = smoothSkarn,//8dabb2
                [new Color(184, 230, 207)] = skarnBrick,//b8e6cf

                [new Color(105, 97, 90)] = ModContent.TileType<BasaltBeamTile>(),//69615a
                [new Color(63, 76, 73)] = ModContent.TileType<BasaltTile>(),//3f4c49
                [new Color(166, 166, 166)] = ModContent.TileType<HardBasaltTile>(),//a6a6a6

                [new Color(71, 56, 53)] = TileID.Mud,//473835

                [new Color(241, 130, 255)] = crystallineBrick,//f182ff
                [Color.Black] = -1
            };
            Dictionary<Color, int> wallDic = new()
            {
                [new Color(85, 183, 206)] = ModContent.WallType<SmoothSkarnWallUnsafe>(),//55b7ce
                [new Color(29, 30, 28)] = ModContent.WallType<HardBasaltWall>(),//1d1e1c
                [Color.Black] = -1
            };

            generator.GenerateByTopLeft(p,mainDic,wallDic);

            WorldGen.PlaceObject(p.X + 7, p.Y + 6, ModContent.TileType<SoulOfNightAltarTile>());
            WorldGen.PlaceObject(p.X + 19, p.Y + 4, ModContent.TileType<PremissionAltarTile>());
            WorldGen.PlaceObject(p.X + 33, p.Y + 6, ModContent.TileType<SoulOfLightAltarTile>());

            GenVars.structures.AddProtectedStructure(new Rectangle(p.X, p.Y, generator.Width, generator.Height));
        }

        /// <summary>
        /// 随机找丛林中心附近的X
        /// </summary>
        /// <returns></returns>
        public int PickAltarX()
        {
            return (GenVars.jungleMinX + GenVars.jungleMaxX) / 2 + WorldGen.genRand.Next(-30, 30);
        }

        #endregion

        #region 生成主岛

        public void GenMainSkyIsland(Point altarPos,out Rectangle mainRect)
        {
            //矽卡岩
            ushort skarn = (ushort)ModContent.TileType<SkarnTile>();
            ushort smoothSkarn = (ushort)ModContent.TileType<SmoothSkarnTile>();
            ushort skarnBrick = (ushort)ModContent.TileType<SkarnBrickTile>();
            ushort CrystallineSkarn = (ushort)ModContent.TileType<CrystallineSkarnTile>();

            //光滑矽卡岩墙
            ushort skarnWall = (ushort)ModContent.WallType<SmoothSkarnWallUnsafe>();
            ushort crystallineBrick = (ushort)ModContent.TileType<CrystallineBrickTile>();

            //随机一下主岛的尺寸
            int baseX = 60;
            int xPlus = 18;

            int baseY = 60;
            int yPlus = 22;

            Point mainIslandSize = Main.maxTilesX switch
            {
                //小世界
                < 6000 => new Point(WorldGen.genRand.Next(baseX, baseX + xPlus), WorldGen.genRand.Next(baseY, baseY + yPlus)),
                //中世界
                > 6000 and < 8000 => new Point(WorldGen.genRand.Next(baseX + xPlus, baseX + xPlus * 2), WorldGen.genRand.Next(baseY + yPlus, baseY + yPlus * 2)),
                //大世界
                _ => new Point(WorldGen.genRand.Next(baseX + xPlus * 2, baseX + xPlus * 3), WorldGen.genRand.Next(baseY + yPlus * 2, baseY + yPlus * 3))
            };

            //主岛的中心点和左上角
            Point mainIslandTopLeft = new Point(altarPos.X - mainIslandSize.X / 2, ValueByWorldSize(
                WorldGen.genRand.Next(60, 80),
                WorldGen.genRand.Next(70, 90),
                WorldGen.genRand.Next(80, 100)));
            Point mainIslandCenter = new Point(altarPos.X, mainIslandTopLeft.Y + mainIslandSize.Y / 2);

            //
            int xExpand = ValueByWorldSize(8, 10, 12);
            int yExpand = ValueByWorldSize(8, 12, 16);
            Point mainIslandOutTopLeft = mainIslandTopLeft - new Point(xExpand, yExpand);
            Point mainIslandOutSize = mainIslandSize + new Point(xExpand * 2, yExpand * 2);

            //空岛外缘的矩形
            Rectangle mainIslandRect = Utils.CenteredRectangle(mainIslandCenter.ToVector2(), mainIslandSize.ToVector2());
            Rectangle innerRect = Utils.CenteredRectangle(mainIslandCenter.ToVector2(), (mainIslandSize - new Point(xExpand * 2, yExpand * 2)).ToVector2());
            Rectangle outerRect = new Rectangle(mainIslandOutTopLeft.X, mainIslandOutTopLeft.Y, mainIslandOutSize.X, mainIslandOutSize.Y);

            #region 主体形状

            // 清理范围
            //CSkyIslandClear(outerRect);

            // 生成主体
            CSkyIslandMainBlock(skarn, skarnWall, mainIslandSize, mainIslandCenter);

            // 随机加一些突起和凹坑
            CSkyIslandAddSmallBlocks(skarn, mainIslandSize, mainIslandTopLeft);


            #endregion

            #region 条纹与矿物

            // 用平滑矽卡岩画条纹
            int size = CSkyIslandSmoothStripe(smoothSkarn, mainIslandCenter, mainIslandOutSize);

            // 生成蕴魔水晶矿
            CSkyIslandGenCMOre(skarn, smoothSkarn, CrystallineSkarn, outerRect);

            #endregion

            #region 遗迹与通道

            // 生成墙壁缝隙和其他种类的墙壁
            CSkyIslandSPWalls(mainIslandRect);

            // 生成中心遗迹与两侧的主要通道
            Rectangle shrineRect = CSkyIslandShrineAndMainTunnel(skarn, smoothSkarn, skarnBrick, skarnWall, crystallineBrick, mainIslandSize, mainIslandTopLeft, outerRect);

            // 随机挖其余的通道
            CSkyIslandTunnels(innerRect, outerRect, shrineRect);

            #endregion

            //清理一下浮空物块，包括矽卡岩，平滑矽卡岩和蕴魔矽卡岩
            CSkyIslandQuickClear(skarn, smoothSkarn, CrystallineSkarn, mainIslandCenter, size, shrineRect);

            //生成贯穿的矽卡砖
            CSkyIslandBrickPillars(skarnBrick, outerRect, shrineRect);

            #region 生成云朵和云墙

            //生成云
            CSkyIslandClouds(outerRect, shrineRect);
            //生成云墙
            CSkyIslandCloudWalls(mainIslandSize, mainIslandTopLeft, shrineRect);

            #endregion

            #region 添加各类美化与装饰

            // 种植浮萍
            CSkyIslandGrass(skarn, smoothSkarn, mainIslandRect, shrineRect);

            // 添加斜坡
            CSkyIslandSlope(skarnBrick, outerRect, shrineRect);

            // 生成瀑布
            CSkyIslandWaterFall(skarnBrick, outerRect, shrineRect);

            // 生成装饰物
            CSkyIslandDecorations(outerRect, shrineRect);

            // 生成草
            CSkyIslandGrassDecorations(outerRect);

            #endregion

            #region 生成水池

            //Point poolPos = mainIslandCenter;
            //poolPos.Y = 10;

            //for (int i = 0; i < 1000; i++)
            //{
            //    Tile t = Main.tile[poolPos];
            //    if (t.HasTile && Main.tileSolid[t.TileType]&&t.TileType!=TileID.ClayBlock)//找到实心方块
            //        break;

            //    poolPos.Y++;
            //}

            //Texture2D poolTex = ModContent.Request<Texture2D>(AssetDirectory.CrystallineSkyIsland + "MainSkyIslandPool", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D poolClearTex = ModContent.Request<Texture2D>(AssetDirectory.CrystallineSkyIsland + "MainSkyIslandPoolClear", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D poolWallTex = ModContent.Request<Texture2D>(AssetDirectory.CrystallineSkyIsland + "MainSkyIslandPoolWall", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D poolWallClearTex = ModContent.Request<Texture2D>(AssetDirectory.CrystallineSkyIsland + "MainSkyIslandPoolWallClear", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D poolWaterTex = ModContent.Request<Texture2D>(AssetDirectory.CrystallineSkyIsland + "MainSkyIslandPoolWater", AssetRequestMode.ImmediateLoad).Value;

            //poolPos -= new Point(poolTex.Width / 2, poolTex.Height / 2);

            //GenByTexture(poolClearTex, poolTex, poolWallClearTex, poolWallTex, clearDic, mainDic, clearDic, wallDic, poolPos.X, poolPos.Y);
            //GenLiquidByTexture(poolWaterTex, new Dictionary<Color, int>() { [Color.Black] = -1, [Color.White] = LiquidID.Water }, poolPos);

            #endregion
            mainRect = outerRect;
        }

        private static void CSkyIslandGrassDecorations(Rectangle outerRect)
        {
            int Grass2x2 = ModContent.TileType<ChalcedonyGrass2x2>();
            int Grass1x1 = ModContent.TileType<ChalcedonyGrass1x1>();

            for (int i = 0; i < outerRect.Width; i++)
                for (int j = 0; j < outerRect.Height; j++)
                {
                    Point point = new Point(outerRect.X, outerRect.Y) + new Point(i, j);
                    Tile t = Main.tile[point];

                    if (!t.HasTile || (t.TileType != ModContent.TileType<ChalcedonySkarn>() && t.TileType != ModContent.TileType<ChalcedonySmoothSkarn>()))
                        continue;

                    Tile top = Main.tile[point.X, point.Y - 1];
                    if (!top.HasTile)
                    {
                        ushort tileType = (ushort)Main.rand.NextFromList(Grass1x1, Grass1x1, Grass2x2);
                        TileObjectData data = TileObjectData.GetTileData(tileType, 0);

                        WorldGen.PlaceObject(point.X, point.Y-1, tileType, true, WorldGen.genRand.Next(data.RandomStyleRange));
                        continue;
                    }

                    if (Main.tileSolid[top.TileType] || Main.tileContainer[top.TileType] || top.TileType == Grass2x2)
                        continue;

                    WorldGen.KillTile(point.X, point.Y - 1);
                    ushort tileType2 = (ushort)Main.rand.NextFromList(Grass1x1, Grass1x1, Grass2x2);
                    TileObjectData data2 = TileObjectData.GetTileData(tileType2, 0);

                    WorldGen.PlaceObject(point.X, point.Y - 1, tileType2, true, WorldGen.genRand.Next(data2.RandomStyleRange));
                }
        }

        private static void CSkyIslandDecorations(Rectangle outerRect, Rectangle shrineRect)
        {
            WorldGenHelper.PlaceDecorations_NoCheck(outerRect, (ushort)ModContent.TileType<CrystallineStalactite>(), 5);
            WorldGenHelper.PlaceDecorations_NoCheck(outerRect, (ushort)ModContent.TileType<CrystallineStalactite2x2>(), 4, avoidArea: shrineRect);

            WorldGenHelper.PlaceDecorations_NoCheck(outerRect, (ushort)ModContent.TileType<SkarnRubbles4x2>(), 15, avoidArea: shrineRect);
            WorldGenHelper.PlaceDecorations_NoCheck(outerRect, (ushort)ModContent.TileType<SkarnRubbles3x4>(), 13, avoidArea: shrineRect);
            WorldGenHelper.PlaceDecorations_NoCheck(outerRect, (ushort)ModContent.TileType<SkarnRubbles3x3>(), 11, avoidArea: shrineRect);
            WorldGenHelper.PlaceDecorations_NoCheck(outerRect, (ushort)ModContent.TileType<SkarnRubbles3x2>(), 10, avoidArea: shrineRect);
            WorldGenHelper.PlaceDecorations_NoCheck(outerRect, (ushort)ModContent.TileType<SkarnRubbles2x2>(), 5, avoidArea: shrineRect);
            WorldGenHelper.PlaceDecorations_NoCheck(outerRect, (ushort)ModContent.TileType<SkarnRubbles2x1>(), 3);
            WorldGenHelper.PlaceDecorations_NoCheck(outerRect, (ushort)ModContent.TileType<SkarnRubbles1x1>(), 2);
        }

        private void CSkyIslandWaterFall(ushort skarnBrick, Rectangle outerRect, Rectangle shrineRect)
        {
            int waterfallCount = ValueByWorldSize(WorldGen.genRand.Next(4, 7)
                    , WorldGen.genRand.Next(6, 11)
                    , WorldGen.genRand.Next(8, 16));

            for (int i = 0; i < waterfallCount; i++)
            {
                //随机找点
                Point p = new Point(0, 0);

                for (int j = 0; j < 1000; j++)//找到一个自身没物块的地方
                {
                    Point p2 = WorldGen.genRand.NextVector2FromRectangle(outerRect).ToPoint();

                    if (shrineRect.Contains(p2) || Main.tile[p2].TileType == skarnBrick)
                        continue;

                    Tile left = Main.tile[p2.X - 1, p2.Y];
                    Tile right = Main.tile[p2.X + 1, p2.Y];
                    Tile bottom = Main.tile[p2.X, p2.Y + 1];

                    if (!left.HasTile || !right.HasTile || !bottom.HasTile)//左右两侧必须有物块
                        continue;

                    Tile left2 = Main.tile[p2.X - 2, p2.Y];
                    Tile right2 = Main.tile[p2.X + 2, p2.Y];

                    if (left2.HasTile && right2.HasTile)//左右两侧至少有一个没物块的地方
                        continue;

                    p = p2;
                }

                if (p == default)
                    continue;

                Main.tile[p].ClearTile();//清除物块
                WorldGen.PlaceLiquid(p.X, p.Y, (byte)LiquidID.Water, 255);

                Tile left22 = Main.tile[p.X - 2, p.Y];
                Tile right22 = Main.tile[p.X + 2, p.Y];

                if (!left22.HasTile)
                    WorldGen.PoundTile(p.X - 1, p.Y);//左边敲半砖
                if (!right22.HasTile)
                    WorldGen.PoundTile(p.X + 1, p.Y);//右边敲半砖

                Main.tile[p.X, p.Y + 1].Clear(TileDataType.Slope);//底部变成整块
            }
        }

        private static void CSkyIslandSlope(ushort skarnBrick, Rectangle outerRect, Rectangle shrineRect)
        {
            for (int i = 0; i < outerRect.Width; i++)
                for (int j = 0; j < outerRect.Height; j++)
                {
                    Point point = outerRect.TopLeft().ToPoint() + new Point(i, j);
                    Tile t = Main.tile[point];
                    if (!t.HasTile || shrineRect.Contains(point) || t.TileType == skarnBrick)
                        continue;

                    if (WorldGen.genRand.NextBool())
                        Tile.SmoothSlope(point.X, point.Y, false);
                }
        }

        private void CSkyIslandGrass(ushort skarn, ushort smoothSkarn, Rectangle rect,Rectangle shrineRect)
        {
            int grassCount = ValueByWorldSize(WorldGen.genRand.Next(4, 7)
                    , WorldGen.genRand.Next(5, 8)
                    , WorldGen.genRand.Next(6, 10));

            const int size = 10;

            for (int i = 0; i < grassCount; i++)
            {
                //随机找点
                Point p = new Point(0, 0);

                for (int j = 0; j < 1000; j++)//找到一个自身没物块的地方
                {
                    Point p2 = WorldGen.genRand.NextVector2FromRectangle(rect).ToPoint();

                    if (Main.tile[p2].HasTile || shrineRect.Intersects(Utils.CenteredRectangle(p2.ToVector2(), new Vector2(size, size))))
                        continue;

                    p = p2;
                }

                if (p == default)
                    continue;

                for (int m = 0; m < size; m++)
                    for (int n = 0; n < size; n++)
                    {
                        Point point = p + new Point(-size/2 + m, -size/2 + n);
                        Tile t = Main.tile[point];
                        if (!t.HasTile || (t.TileType != skarn && t.TileType != smoothSkarn))//限制感染物块
                            continue;

                        //检测周围需要有一定空间才能生成浮萍
                        Tile top = Main.tile[point.X, point.Y - 1];
                        Tile bottom = Main.tile[point.X, point.Y + 1];
                        Tile left = Main.tile[point.X - 1, point.Y];
                        Tile right = Main.tile[point.X + 1, point.Y];

                        if (top.HasTile && bottom.HasTile && left.HasTile && right.HasTile)
                        {
                            Tile topLeft = Main.tile[point.X - 1, point.Y - 1];
                            Tile topRight = Main.tile[point.X + 1, point.Y - 1];
                            Tile bottomLeft = Main.tile[point.X - 1, point.Y + 1];
                            Tile bottomRight = Main.tile[point.X + 1, point.Y + 1];

                            int k = 0;
                            if (!topLeft.HasTile)
                                k++;
                            if (!topRight.HasTile)
                                k++;
                            if (!bottomLeft.HasTile)
                                k++;
                            if (!bottomRight.HasTile)
                                k++;

                            if (k < 2)
                                continue;
                        }

                        if (t.TileType == skarn)
                            Main.tile[point].ResetToType((ushort)(ModContent.TileType<ChalcedonySkarn>()));
                        else if (t.TileType == smoothSkarn)
                            Main.tile[point].ResetToType((ushort)(ModContent.TileType<ChalcedonySmoothSkarn>()));
                    }
            }
        }

        private void CSkyIslandCloudWalls(Point mainIslandSize, Point mainIslandTopLeft, Rectangle shrineRect)
        {
            int cloudWallcount = ValueByWorldSize(12, 16, 20);

            for (int i = 0; i < cloudWallcount; i++)
            {
                //随机在边缘上取点
                Point p1 = WorldGen.genRand.NextInRectangleEdge(mainIslandTopLeft, mainIslandSize);

                int cloudWidth = WorldGen.genRand.Next(5, 11);
                int dir = WorldGen.genRand.NextFromList(-1, 1);
                int cloudBallcount = WorldGen.genRand.Next(2, 4);

                for (int v = 0; v < cloudBallcount; v++)
                {
                    float scale = 1 + v * 0.1f;
                    float yScale = 1 - v * 0.2f;

                    int verticalRadius = (int)((cloudWidth - 2) / 2 * yScale);
                    if (verticalRadius < 1)
                        verticalRadius = 1;

                    int horizontalRadius = (int)(cloudWidth / 2 * scale);
                    if (horizontalRadius < 1)
                        horizontalRadius = 1;

                    Point cloudOrigin = p1 + new Point(horizontalRadius * dir * v, WorldGen.genRand.Next(-verticalRadius, verticalRadius));

                    if (shrineRect.Intersects(Utils.CenteredRectangle(cloudOrigin.ToVector2(), new Vector2(horizontalRadius * 2, verticalRadius * 2))))
                        break;

                    WorldUtils.Gen(
                        cloudOrigin,
                        new Shapes.Circle(horizontalRadius, verticalRadius),
                        Actions.Chain(
                             new Modifiers.Blotches(1)
                            , new Actions.PlaceWall(WallID.Cloud)
                            , new Actions.SetFrames()));
                }
            }
        }

        private void CSkyIslandClouds(Rectangle outerRect, Rectangle shrineRect)
        {
            int cloudCount = ValueByWorldSize(WorldGen.genRand.Next(8, 14)
                    , WorldGen.genRand.Next(20, 28)
                    , WorldGen.genRand.Next(30, 46));

            for (int i = 0; i < cloudCount; i++)
            {
                //随机找点
                Point p = new Point(0, 0);

                for (int j = 0; j < 1000; j++)//找到一个自身没物块的地方
                {
                    Point p2 = WorldGen.genRand.NextVector2FromRectangle(outerRect).ToPoint();
                    if (Main.tile[p2].HasTile || shrineRect.Intersects(Utils.CenteredRectangle(p2.ToVector2(), new Vector2(8, 8))))
                        continue;

                    p = p2;
                }

                if (p == default)
                    continue;

                //向外生成越来越扁的云
                 CSkyIslandCloudBall(shrineRect, p, WorldGen.genRand.Next(1, 4));
            }
        }

        /// <summary>
        /// 生成几团云，向外生成越来越扁
        /// </summary>
        /// <param name="shrineRect"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private void CSkyIslandCloudBall(Rectangle shrineRect, Point p, int cloudBallcount)
        {
            int cloudWidth = WorldGen.genRand.Next(5, 11);
            int dir = WorldGen.genRand.NextFromList(-1, 1);

            for (int v = 0; v < cloudBallcount; v++)
            {
                float scale = 1 + v * 0.1f;
                float yScale = 0.8f - v * 0.2f;

                int verticalRadius = (int)((cloudWidth - 2) / 2 * yScale);
                if (verticalRadius < 1)
                    verticalRadius = 1;

                int horizontalRadius = (int)(cloudWidth / 2 * scale);
                if (horizontalRadius < 1)
                    horizontalRadius = 1;

                Point cloudOrigin = p + new Point(horizontalRadius * dir * v, WorldGen.genRand.Next(-verticalRadius, verticalRadius));

                if (shrineRect.Intersects(Utils.CenteredRectangle(cloudOrigin.ToVector2(), new Vector2(horizontalRadius * 2, verticalRadius * 2))))
                    break;

                WorldUtils.Gen(
                    cloudOrigin,
                    new Shapes.Circle(horizontalRadius, verticalRadius),
                    Actions.Chain(
                        new Actions.PlaceTile(TileID.Cloud)
                        , new Actions.SetFrames()));
            }
        }

        private void CSkyIslandBrickPillars(ushort skarnBrick, Rectangle outerRect, Rectangle shrineRect)
        {
            int brickCount = ValueByWorldSize(WorldGen.genRand.Next(8, 14)
                    , WorldGen.genRand.Next(20, 28)
                    , WorldGen.genRand.Next(28, 38));

            for (int i = 0; i < brickCount; i++)
            {
                //随机找点
                Point p = new Point(0, 0);

                for (int j = 0; j < 1000; j++)//找到一个自身没物块，但是底部有物块的地方
                {
                    Point p2 = WorldGen.genRand.NextVector2FromRectangle(outerRect).ToPoint();
                    if (Main.tile[p2].HasTile || !Main.tile[p2.X, p2.Y + 1].HasTile || Main.tile[p2.X, p2.Y + 1].TileType == skarnBrick || shrineRect.Contains(p2))
                        continue;

                    Dictionary<ushort, int> scan = [];
                    WorldUtils.Gen(p2 - new Point(2, 2), new Shapes.Rectangle(4, 4)
                        , new Actions.TileScanner(skarnBrick).Output(scan));
                    if (scan[skarnBrick] > 0)
                        continue;

                    p = p2;
                }

                if (p == default)
                    continue;

                //额外向下的长度
                int exY = 1;

                int checkY = WorldGenHelper.CheckUpAreaEmpty(p);

                //检测上方的空间，如果有空间那么就向上突起
                if (checkY > 5)
                    exY = WorldGen.genRand.Next(-1, 1);
                if (checkY > 7)
                    exY = WorldGen.genRand.Next(-3, 1);

                int brickWidth = WorldGen.genRand.Next(2, 4);
                int maxY = ValueByWorldSize(WorldGen.genRand.Next(4, 10)
                    , WorldGen.genRand.Next(6, 14)
                    , WorldGen.genRand.Next(8, 18));

                //向下生成砖块
                SpawnSkarnBrick(skarnBrick, shrineRect, p, exY, brickWidth, maxY);

                if (WorldGen.genRand.NextBool(3))// 生成更多矽卡砖
                {
                    int dir = WorldGen.genRand.NextFromList(-1, 1);

                    SpawnSkarnBrick(skarnBrick, shrineRect, p + new Point(dir * brickWidth * 2, 0), exY, brickWidth, WorldGen.genRand.Next(4, 7), true);

                    int yoff = WorldGen.genRand.Next(1, 3);
                    for (int m = 0; m < brickWidth * 2; m++)
                    {
                        Point b2P = new Point(p.X + dir * m, p.Y + yoff + exY);
                        if (shrineRect.Contains(b2P))
                            break;

                        Main.tile[b2P].ResetToType(skarnBrick);
                    }
                }
            }
        }

        private static void CSkyIslandQuickClear(ushort skarn, ushort smoothSkarn, ushort CrystallineSkarn, Point mainIslandCenter, int size,Rectangle shrineRect)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    Point clearP = new Point(mainIslandCenter.X - size / 2 + i, mainIslandCenter.Y - size / 2 + j);
                    if (Main.tile[clearP].HasTile && !shrineRect.Contains(clearP) && SkyIslandTileCounter(clearP.X, clearP.Y, skarn, smoothSkarn, CrystallineSkarn) < 8)
                        SkyIslandTileCounterKill();
                }
        }

        private void CSkyIslandTunnels(Rectangle innerRect, Rectangle outerRect, Rectangle shrineRect)
        {
            int tunnelCount = ValueByWorldSize(5, 8, 12);
            float randDir = WorldGen.genRand.NextFloat(6.282f);

            for (int i = 0; i < tunnelCount; i++)
            {
                Vector2 dir = (randDir + MathHelper.TwoPi / tunnelCount * i).ToRotationVector2();

                //随机取点
                Point tunnelP1 = innerRect.Center
                        + (dir * WorldGen.genRand.NextVector2Circular(innerRect.Width / 2, innerRect.Height / 2).Length()).ToPoint();

                for (int m = 0; m < 2000; m++)//检测点是否符合要求
                {
                    if (Main.tile[tunnelP1].HasTile && !shrineRect.Contains(tunnelP1))
                        break;

                    tunnelP1 = innerRect.Center
                        + (dir * WorldGen.genRand.NextVector2Circular(innerRect.Width / 2, innerRect.Height / 2).Length()).ToPoint();
                }

                for (int k = 0; k < 5000; k++)//尝试生成通道
                {
                    if (DigSkyIslandTunnel(outerRect, shrineRect, tunnelP1, WorldGen.genRand.Next(2), WorldGen.genRand.NextFromList(-1, 1), GetTunnelLength))
                        break;
                }
            }
        }

        private Rectangle CSkyIslandShrineAndMainTunnel(ushort skarn, ushort smoothSkarn, ushort skarnBrick, ushort skarnWall, ushort crystallineBrick, Point mainIslandSize, Point mainIslandTopLeft, Rectangle outerRect)
        {
            //主要通道，用于生成小遗迹
            int type = WorldGen.genRand.Next(8);

            TextureGenerator generator = new TextureGenerator("MainSkyIslandShrine",type,AssetDirectory.CrystallineSkyIsland);
            generator.SetWallTex();

            int width = generator.Width;
            int height = generator.Height;
            /* 一一一一一一一一一一一
             * |    一一一一      |
             * |    |      |     |
             * |    |      |     |
             * |    |      |     |
             * |    |      |     |
             * |    一一一一      |
             * 一一一一一一一一一一一
             * 
             * 获取里面这个矩形
             */
            Rectangle shrineInnerRect = new Rectangle(mainIslandTopLeft.X + width, mainIslandTopLeft.Y + height, mainIslandSize.X - width * 2, mainIslandSize.Y - height * 2);

            //遗迹的中心
            Point shrineCenter = WorldGen.genRand.NextVector2FromRectangle(shrineInnerRect).ToPoint();
            Point shrineTopLeft = new Point(shrineCenter.X - width / 2, shrineCenter.Y - height / 2);
            //遗迹的尺寸
            Rectangle shrineRect = new Rectangle(shrineTopLeft.X, shrineTopLeft.Y, width, height);

            Dictionary<Color, int> mainDic = new()
            {
                [new Color(51, 76, 117)] = skarn,//334c75
                [new Color(141, 171, 178)] = smoothSkarn,//8dabb2
                [new Color(184, 230, 207)] = skarnBrick,//b8e6cf
                [new Color(158, 77, 255)] = ModContent.TileType<CrystallineSkarnTile>(),//9e4dff

                [new Color(255, 239, 219)] = ModContent.TileType<ChalcedonyTile>(),//ffefdb
                [new Color(170, 228, 143)] = ModContent.TileType<LeafChalcedonyTile>(),//aae48f

                [new Color(147, 186, 84)] = ModContent.TileType<ChalcedonySkarn>(),//93ba54
                [new Color(95, 212, 111)] = ModContent.TileType<ChalcedonySmoothSkarn>(),//5fd46f

                [new Color(241, 130, 255)] = crystallineBrick,//f182ff
                [new Color(90, 100, 80)] = TileID.Chain,//5a6450

                [Color.Black] = -1
            };
            Dictionary<Color, int> wallDic = new()
            {
                [new Color(85, 183, 206)] = skarnWall,//55b7ce
                [new Color(188, 171, 150)] = ModContent.WallType<WildChalcedonyWallUnsafe>(),//bcab96
                [new Color(113, 128, 131)] = ModContent.WallType<SkarnBrickWallUnsafe>(),//718083
                [Color.Black] = -1,
                [Color.White] = -2
            };

            //生成遗迹
            generator.GenerateByTopLeft(shrineTopLeft, mainDic, wallDic);

            if (type == 7)//特定样式生成水池
            {
                Texture2D liquidTex = ModContent.Request<Texture2D>(AssetDirectory.CrystallineSkyIsland + "MainSkyIslandShrineLiquid" + type, AssetRequestMode.ImmediateLoad).Value;
                GenLiquidByTexture(liquidTex, liquidDic, shrineTopLeft);
            }

            //放置中心的箱子
            ushort chestTileType = (ushort)ModContent.TileType<SkarnChestTile>();
            WorldGen.AddBuriedChest(shrineTopLeft.X + 14, shrineTopLeft.Y + 15,
                ModContent.ItemType<Reel_MagikeAdvance>(), notNearOtherChests: false, 1, trySlope: false, chestTileType);

            //挖通道的中心点，挖主要通道
            DigSkyIslandTunnel(outerRect, shrineRect, shrineTopLeft + new Point(27, 12), 0, 1, GetTunnelLength, true);
            DigSkyIslandTunnel(outerRect, shrineRect, shrineTopLeft + new Point(0, 12), 0, -1, GetTunnelLength, true);
            return shrineRect;
        }

        private void CSkyIslandSPWalls(Rectangle rect)
        {
            //糊几片粗糙墙

            int crackWallCount = ValueByWorldSize(WorldGen.genRand.Next(3, 6)
                , WorldGen.genRand.Next(4, 7)
                , WorldGen.genRand.Next(5, 8));

            for (int i = 0; i < crackWallCount; i++)
            {
                Point p = WorldGen.genRand.NextVector2FromRectangle(rect).ToPoint();

                //WorldUtils.Gen(
                //    p,
                //    new Shapes.Slime(WorldGen.genRand.Next(4, 12)),
                //    Actions.Chain(
                //        new Modifiers.Flip(false, true),
                //        new Modifiers.Blotches(5),
                //        new Modifiers.Dither(0.95f),
                //        new Modifiers.OnlyWalls((ushort)ModContent.WallType<SmoothSkarnWallUnsafe>()),
                //        new Actions.ClearWall()));

                int radius = WorldGen.genRand.Next(4, 12);
                WorldUtils.Gen(
                    p,
                    new Shapes.Slime(radius),
                    Actions.Chain(
                        new Modifiers.Flip(false, true),
                        new Modifiers.Blotches(3),
                        new Modifiers.RadialDither(radius / 2, radius * 1.25f),
                        new Modifiers.OnlyWalls((ushort)ModContent.WallType<SmoothSkarnWallUnsafe>()),
                        new Actions.ClearWall(),
                        new Actions.PlaceWall((ushort)ModContent.WallType<CrackedSkarnWallUnsafe>())));
            }

            int size = Math.Max(rect.Width, rect.Height);

            //按照噪声随机生成狂野木墙
            ushort wildWall = (ushort)(ModContent.WallType<WildChalcedonyWallUnsafe>());

            int wildWallCount = ValueByWorldSize(WorldGen.genRand.Next(3, 5)
                , WorldGen.genRand.Next(3, 5)
                , WorldGen.genRand.Next(4, 7));

            for (int i = 0; i < wildWallCount; i++)
            {
                Point p = WorldGen.genRand.NextVector2FromRectangle(rect).ToPoint();

                ShapeData shape = new ShapeData();

                int radius = ValueByWorldSize(WorldGen.genRand.Next(5, 10)
                    , WorldGen.genRand.Next(6, 10)
                    , WorldGen.genRand.Next(7, 12));

                //获取形状
                WorldUtils.Gen(
                    p,
                    new Shapes.Circle(radius),
                    Actions.Chain(
                        new Modifiers.Dither(0.1f).Output(shape)));

                Point topLeft = p - new Point(radius, radius);

                int x = (int)(WorldGen.genRand.NextFloat() * radius * 2);
                int y = (int)(WorldGen.genRand.NextFloat() * radius * 2);

                for (int m = 0; m < radius * 2; m++)
                    for (int n = 0; n < radius * 2; n++)
                    {
                        if (!shape.Contains(-radius + m, -radius + n))
                            continue;

                        Point currP = topLeft + new Point(m, n);

                        float mainNoise = MainNoise(new Vector2(x + m, y + n), new Vector2(radius * 2) * 6);
                        if (mainNoise > 0.8f)
                        {
                            if (Main.tile[currP].WallType > 0)
                            {
                                WorldGen.KillWall(currP.X, currP.Y);
                                WorldGen.PlaceWall(currP.X, currP.Y, wildWall);
                            }
                        }
                        //else if (WorldGen.genRand.NextBool(5))
                        //{
                        //    WorldGen.KillWall(currP.X, currP.Y);
                        //}
                    }
            }
        }

        private void CSkyIslandGenCMOre(ushort skarn, ushort smoothSkarn, ushort CrystallineSkarn, Rectangle outerRect)
        {
            int crystalCount = ValueByWorldSize(WorldGen.genRand.Next(6, 10)
                    , WorldGen.genRand.Next(12, 18)
                    , WorldGen.genRand.Next(16, 22));

            for (int i = 0; i < crystalCount; i++)
            {
                int crystalSize = WorldGen.genRand.Next(14, 17);
                crystalSize /= 2;
                crystalSize *= 2;
                crystalSize++;

                Point orecenter = default;

                for (int j = 0; j < 1000; j++)//找到一个自身没物块，但是底部有物块的地方
                {
                    Point p2 = WorldGen.genRand.NextVector2FromRectangle(outerRect).ToPoint();
                    if (!Main.tile[p2].HasTile || Main.tile[p2].TileType == CrystallineSkarn)
                        continue;

                    orecenter = p2;
                }

                if (orecenter == default)
                    continue;

                Point oreP = orecenter + new Vector2(0, -crystalSize / 4f).ToPoint();

                ShapeData oreData = new ShapeData();
                int y1 = (int)(crystalSize / 2f * 1.732f);

                WorldUtils.Gen(
                    oreP,
                    new Shapes.Tail(crystalSize, new ReLogic.Utilities.Vector2D(0, y1)),
                    new Actions.Blank().Output(oreData));

                //生成外面一圈倒的正三角形
                WorldUtils.Gen(
                    oreP,
                    new ModShapes.InnerOutline(oreData),
                    Actions.Chain(
                        new Modifiers.OnlyTiles(skarn, smoothSkarn)
                        , new Actions.ClearTile()
                        , new Actions.PlaceTile(CrystallineSkarn)
                        , new Actions.SetFrames())
                    );

                crystalSize /= 2;
                crystalSize++;

                oreP = orecenter + new Vector2(1, crystalSize / 4f).ToPoint();
                int y2 = (int)(-crystalSize / 2f * 1.732f);

                //生成里面的小正三角形
                WorldUtils.Gen(
                    oreP,
                    new Shapes.Tail(crystalSize, new ReLogic.Utilities.Vector2D(0, y2)),
                    Actions.Chain(
                        new Modifiers.OnlyTiles(skarn, smoothSkarn)
                        , new Actions.ClearTile()
                        , new Actions.PlaceTile(CrystallineSkarn)
                        , new Actions.SetFrames())
                    );
            }
        }

        /// <summary>
        /// 使用噪声方法在一团矽卡岩中刷上平滑矽卡岩
        /// </summary>
        /// <param name="smoothSkarn"></param>
        /// <param name="mainIslandCenter"></param>
        /// <param name="mainIslandOutSize"></param>
        /// <returns></returns>
        private int CSkyIslandSmoothStripe(ushort smoothSkarn, Point mainIslandCenter, Point mainIslandOutSize)
        {
            int x = (int)(WorldGen.genRand.NextFloat() * mainIslandOutSize.X);
            int y = (int)(WorldGen.genRand.NextFloat() * mainIslandOutSize.Y);
            int size = Math.Max(mainIslandOutSize.X, mainIslandOutSize.Y);

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    Tile t = Main.tile[mainIslandCenter.X - size / 2 + i, mainIslandCenter.Y - size / 2 + j];
                    if (!t.HasTile)
                        continue;

                    float mainNoise = MainNoise(new Vector2(x + i, y + j), new Vector2(size) * 6);
                    if (mainNoise > 0.8f)
                        t.ResetToType(smoothSkarn);
                }

            return size;
        }

        private void CSkyIslandAddSmallBlocks(ushort skarn, Point mainIslandSize, Point mainIslandTopLeft)
        {
            int count = ValueByWorldSize(16, 20, 26);

            for (int i = 0; i < count; i++)
            {
                bool add = WorldGen.genRand.NextBool();
                //随机在边缘上取点
                Point p1 = WorldGen.genRand.NextInRectangleEdge(mainIslandTopLeft, mainIslandSize);

                int r = WorldGen.genRand.Next(2, 5);

                WorldUtils.Gen(
                    p1,
                    new Shapes.Circle(r),
                    Actions.Chain(
                         new Modifiers.Blotches(2)
                         //new Modifiers.Dither()
                         , add ? new Actions.SetTile(skarn) : new Actions.ClearTile()
                         , new Actions.SetFrames(true)));
            }
        }

        private void CSkyIslandMainBlock(ushort skarn, ushort skarnWall, Point mainIslandSize, Point mainIslandCenter)
        {
            ShapeData mainData = new ShapeData();
            ShapeData OutlineData = new ShapeData();

            //生成主体圆形
            WorldUtils.Gen(
                mainIslandCenter,
                new Shapes.Circle(mainIslandSize.X / 2, mainIslandSize.Y / 2),
                Actions.Chain(
                     new Modifiers.Blotches(4)
                     //, new Modifiers.Flip(false, true)
                     , new Actions.SetTile(skarn)
                     , new Actions.SetFrames().Output(mainData)));

            int shapesCount = ValueByWorldSize(24, 32, 40);

            //添加一些方形
            for (int i = 0; i < shapesCount; i++)
            {
                ShapeData dataA = new ShapeData();

                int baseX2 = 4;
                int xPlus2 = 4;

                int baseY2 = 10;
                int yPlus2 = 6;

                Point smallBlockSize = Main.maxTilesY switch
                {
                    //小世界
                    < 6000 => new Point(WorldGen.genRand.Next(baseX2, baseX2 + xPlus2), WorldGen.genRand.Next(baseY2, baseY2 + yPlus2)),
                    //中世界
                    > 6000 and < 8000 => new Point(WorldGen.genRand.Next(baseX2 + xPlus2, baseX2 + xPlus2 * 2), WorldGen.genRand.Next(baseY2 + yPlus2, baseY2 + yPlus2 * 2)),
                    //大世界
                    _ => new Point(WorldGen.genRand.Next(baseX2 + xPlus2 * 2, baseX2 + xPlus2 * 3), WorldGen.genRand.Next(baseY2 + yPlus2 * 2, baseY2 + yPlus2 * 3))
                };

                Rectangle smallBlockRandRect = Utils.CenteredRectangle(mainIslandCenter.ToVector2(), (mainIslandSize - smallBlockSize).ToVector2());

                Point smallBlockPoint = WorldGen.genRand.NextVector2FromRectangle(smallBlockRandRect).ToPoint() - new Point(smallBlockSize.X / 2, smallBlockSize.Y / 2);
                WorldUtils.Gen(
                    smallBlockPoint,
                    new Shapes.Rectangle(smallBlockSize.X, smallBlockSize.Y),
                    Actions.Chain(
                         new Modifiers.Blotches(2)
                         //, new Modifiers.Flip(false, true)
                         , new Actions.SetTile(skarn)
                         , new Actions.SetFrames().Output(dataA)));

                mainData.Add(dataA, mainIslandCenter, smallBlockPoint);
            }

            //获取外边缘的形状
            WorldUtils.Gen(
                mainIslandCenter,
                new ModShapes.InnerOutline(mainData),
                Actions.Chain(
                    new Modifiers.Expand(3, 3)  //扩展这个边缘线，得到一个圆环
                    , new Modifiers.OnlyTiles(skarn).Output(OutlineData)));

            //两个相减，得到内部的形状，并放置背景墙
            WorldUtils.Gen(
                mainIslandCenter,
                new ModShapes.All(mainData),
                Actions.Chain(
                    new Modifiers.NotInShape(OutlineData)
                    , new Modifiers.Blotches(4)
                    , new Actions.PlaceWall(skarnWall)));
        }

        private void CSkyIslandClear(Rectangle rect)
        {
            rect.X -= 200;
            rect.Width += 400;
            if (rect.X < 40)
                rect.X = 40;

            if (rect.X + rect.Width > Main.maxTilesX - 40)
                rect.Width = Main.maxTilesX - 40 - rect.X;

            if (rect.Y < 40)
            {
                rect.Y = 40;
            }

            for (int i = 0; i < rect.Width; i++)
                for (int j = 0; j < rect.Height; j++)
                {
                    Framing.GetTileSafely(new Point(rect.X + i, rect.Y + j)).ClearEverything();
                }

        }

        private static void SpawnSkarnBrick(ushort skarnBrick, Rectangle shrineRect, Point p, int exY, int brickWidth, int maxY,bool skipEmptyCheck=false)
        {
            for (int k = 0; k < maxY; k++)
                for (int n = 0; n < brickWidth; n++)
                {
                    Point brickP = p + new Point(-brickWidth / 2 + n, exY + k);
                    if (shrineRect.Contains(brickP))
                        break;

                    if (!skipEmptyCheck && n == 0 && !Main.tile[brickP.X, brickP.Y].HasTile)
                    {
                        int yBottomCheck = WorldGenHelper.CheckBottomAreaEmpty(brickP);
                        if (yBottomCheck < 6 && yBottomCheck > 3)
                            return;
                    }

                    Main.tile[brickP.X, brickP.Y].ResetToType(skarnBrick);
                }
        }

        private int GetTunnelLength()
        {
            return ValueByWorldSize(
                            WorldGen.genRand.Next(8, 16)
                            , WorldGen.genRand.Next(12, 20)
                            , WorldGen.genRand.Next(12, 28));
        }

        /// <summary>
        /// 从指定点向外挖出通道
        /// </summary>
        /// <param name="outerRect">最外围的矩形</param>
        /// <param name="shrineRect">需要避让的矩形</param>
        /// <param name="tunnelCenter">从哪开始挖</param>
        /// <param name="digType">初始挖掘类型， 横向或竖向，0为竖向，1为横向</param>
        /// <param name="tunnelLength">初始挖多长</param>
        public bool DigSkyIslandTunnel(Rectangle outerRect, Rectangle shrineRect, Point tunnelCenter, int digType, int tunnelDir, Func<int> getTunnelLength, bool ignoreFirstDig = false)
        {
            int tunnelWidth = ValueByWorldSize(WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(4, 6), WorldGen.genRand.Next(4, 7));
            int tunnelLength = getTunnelLength();
            int digCount = 0;
            //计时器，每挖掘多少距离就开始随机一次中心点偏移
            int digRandRecord = WorldGen.genRand.Next(4, 10);
            //中心点偏移，随机偏移-1至1格
            int digRandPos = 0;

            //挖墙壁
            int DigWall = WorldGen.genRand.NextBool(9) ? tunnelLength : 0;
            int skipWall = DigWall > 0 ? WorldGen.genRand.Next(2,5) : 0;
            int BrickWall = WorldGen.genRand.Next(3, 6);

            while (outerRect.Contains(tunnelCenter))
            {
                switch (digType)
                {
                    default:
                    case 0:
                        {
                            //
                            for (; tunnelLength > 0; tunnelLength--)
                            {
                                int yTop = tunnelCenter.Y - tunnelWidth / 2 + digRandPos;
                                digRandRecord--;
                                if (digRandRecord < 0)//随机向一个方向扭一下
                                {
                                    digRandRecord = WorldGen.genRand.Next(6, 10);
                                    digRandPos = digRandPos switch
                                    {
                                        -1 => 0,
                                        0 => WorldGen.genRand.NextFromList(-1, 1),
                                        _ => 0,
                                    };
                                }

                                //遇到遗迹了就退出
                                if (!(ignoreFirstDig && digCount == 0))
                                    if (shrineRect.Intersects(new Rectangle(tunnelCenter.X, yTop, 1, tunnelWidth)))
                                        return false;

                                for (int i = 0; i < tunnelWidth; i++)
                                    Main.tile[tunnelCenter.X, yTop + i].Clear(TileDataType.Tile);

                                if (tunnelWidth < 6 && DigWall > 0)//放墙壁
                                {
                                    if (skipWall > 0)
                                        skipWall--;
                                    else
                                    {
                                        for (int i = -1; i < tunnelWidth + 1; i++)
                                        {
                                            if (Main.tile[tunnelCenter.X, yTop + i].WallType == 0)
                                                goto Placeend;
                                            if (Main.tile[tunnelCenter.X, yTop + i].WallType == ModContent.WallType<SmoothSkarnWallUnsafe>())
                                                Main.tile[tunnelCenter.X, yTop + i].Clear(TileDataType.Wall);
                                        }

                                        if (BrickWall > 0)//生成砖墙
                                            BrickWall--;
                                        else
                                        {
                                            for (int i = -1; i < tunnelWidth + 1; i++)
                                            {
                                                Main.tile[tunnelCenter.X, yTop + i].Clear(TileDataType.Wall);
                                                WorldGen.PlaceWall(tunnelCenter.X, yTop + i, ModContent.WallType<SkarnBrickWallUnsafe>(), true);
                                            }

                                            BrickWall = WorldGen.genRand.Next(3, 6);
                                        }

                                    Placeend:;
                                        DigWall--;
                                    }
                                }

                                tunnelCenter.X += tunnelDir;
                            }

                            //横向挖完就竖向挖
                            digType = 1;
                            tunnelDir = WorldGen.genRand.NextBool(8) ?
                                WorldGen.genRand.NextFromList(-1, 1)
                                : (outerRect.Center.Y > tunnelCenter.Y ? -1 : 1);
                            tunnelCenter.Y -= tunnelDir * tunnelWidth / 2;
                            tunnelLength = getTunnelLength();
                            digCount++;
                        }
                        break;
                    case 1:
                        {
                            for (; tunnelLength > 0; tunnelLength--)
                            {
                                int xLeft = tunnelCenter.X - tunnelWidth / 2 + digRandPos;
                                digRandRecord--;
                                if (digRandRecord < 0)
                                {
                                    digRandRecord = WorldGen.genRand.Next(6, 10);
                                    digRandPos = digRandPos switch
                                    {
                                        -1 => 0,
                                        0 => WorldGen.genRand.NextFromList(-1, 1),
                                        _ => 0,
                                    };
                                }

                                if (!(ignoreFirstDig && digCount == 0))
                                    if (shrineRect.Intersects(new Rectangle(xLeft, tunnelCenter.Y, tunnelWidth, 1)))
                                        return false;

                                for (int i = 0; i < tunnelWidth; i++)
                                    Main.tile[xLeft + i, tunnelCenter.Y].Clear(TileDataType.Tile);

                                tunnelCenter.Y += tunnelDir;
                            }

                            //竖向挖完就横向挖
                            digType = 0;
                            tunnelDir = WorldGen.genRand.NextBool(8) ?
                                WorldGen.genRand.NextFromList(-1, 1)
                                : (outerRect.Center.X > tunnelCenter.X ? -1 : 1);
                            tunnelCenter.X -= tunnelDir * tunnelWidth / 2;
                            tunnelLength = getTunnelLength();
                            digCount++;

                            DigWall = WorldGen.genRand.NextBool(9) ? tunnelLength : 0;
                            skipWall = DigWall > 0 ? WorldGen.genRand.Next(2, 5) : 0;
                            BrickWall = WorldGen.genRand.Next(3, 6);
                        }

                        break;
                }
            }

            return true;
        }

        public T ValueByWorldSize<T>(T smallWorld, T middleWorld, T bigWorld)
        {
            return Main.maxTilesX switch
            {
                //小世界
                < 6000 => smallWorld,
                //中世界
                > 6000 and < 8000 => middleWorld,
                //大世界
                _ => bigWorld
            };
        }

        public static int SkyIslandTileCounter(int x, int y, params ushort[] checkTilesType)
        {
            tileCounterNum = 0;
            SkyIslandTileCounterNext(x, y, checkTilesType);
            return tileCounterNum;
        }

        public static void SkyIslandTileCounterNext(int x, int y, params ushort[] checkTilesType)
        {
            if (tileCounterNum >= tileCounterMax || x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5
                || !Main.tile[x, y].HasTile || !Main.tileSolid[Main.tile[x, y].TileType])
                return;

            if (!checkTilesType.Contains(Main.tile[x, y].TileType))
                return;

            for (int i = 0; i < tileCounterNum; i++)
            {
                if (tileCounterX[i] == x && tileCounterY[i] == y)
                    return;
            }

            tileCounterX[tileCounterNum] = x;
            tileCounterY[tileCounterNum] = y;
            tileCounterNum++;
            SkyIslandTileCounterNext(x - 1, y, checkTilesType);
            SkyIslandTileCounterNext(x + 1, y, checkTilesType);
            SkyIslandTileCounterNext(x, y - 1,checkTilesType);
            SkyIslandTileCounterNext(x, y + 1, checkTilesType);
        }

        public static void SkyIslandTileCounterKill()
        {
            for (int i = 0; i < tileCounterNum; i++)
            {
                int num = tileCounterX[i];
                int num2 = tileCounterY[i];
                Main.tile[num, num2].ClearTile();
            }
        }

        #region 蕴魔空岛使用到的噪声方法

        float Random(Vector2 st)
        {
            float a = MathF.Sin(Vector2.Dot(st, new Vector2(12.9898f, 78.233f))) * 43758.5453123f;
            int a2 = (int)a;
            return a2 - a;
        }

        // Value noise by Inigo Quilez - iq/2013
        // https://www.shadertoy.com/view/lsf3WH
        float Noise(Vector2 st)
        {
            Vector2 i = new Vector2((float)Math.Floor(st.X), (float)Math.Floor(st.Y));
            Vector2 f = new Vector2(st.X - i.X, st.Y - i.Y);
            Vector2 u = f * f * (new Vector2(3f) - 2f * f);

            float a = Random(i);
            float b = Random(i + new Vector2(1f, 0f));
            float c = Random(i + new Vector2(0f, 1f));
            float d = Random(i + new Vector2(1f, 1f));

            float x1 = Helper.Lerp(a, b, u.X);
            float x2 = Helper.Lerp(c, d, u.X);

            return Helper.Lerp(x1, x2, u.Y);
        }

        // 2D旋转矩阵
        private Matrix Rotate2D(Vector2 pos, float angle)
        {
            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);
            return new Matrix(
                cosTheta, -sinTheta, 0f, 0f,
                sinTheta, cosTheta, 0f, 0f,
                0f, 0f, 1f, 0f,
                0f, 0f, 0f, 1f
            );
        }

        private float Lines(Vector2 pos, float b)
        {
            float scale = 10f;
            pos *= scale;
            return SmoothStep(0f, 0.5f + b * 0.5f, Math.Abs((float)Math.Sin(pos.X * Math.PI) + b * 2f) * 0.5f);
        }

        private float SmoothStep(float edge0, float edge1, float x)
        {
            float t = MathHelper.Clamp((x - edge0) / (edge1 - edge0), 0f, 1f);
            return t * t * (3f - 2f * t);
        }

        public float MainNoise(Vector2 selfPos, Vector2 size)
        {
            Vector2 st = selfPos / size;
            st.Y *= size.Y / size.X;

            Vector2 pos = new Vector2(st.Y, st.X) * new Vector2(7f, 8f);

            // 添加噪声并旋转
            float noiseValue = Noise(pos);
            pos = Vector2.Transform(pos, Rotate2D(pos, noiseValue));

            // 绘制线条
            return Lines(pos, 0.5f);
        }

        #endregion

        #endregion

        #region 生成周边小岛

        public enum SmallIslandType
        {
            /// <summary> 平平无奇的小石头岛 </summary>
            Normal,
            /// <summary> 有水池的岛 </summary>
            Pool,
            /// <summary> 山洞岛，中空 </summary>
            Cave,
            /// <summary> 遗迹岛 </summary>
            Ruins,
            /// <summary> 箱子岛 </summary>
            Chest,
            /// <summary> 柱子岛，比较长，比较窄 </summary>
            Pillar,
            /// <summary> 树岛，顶部比较平，用于种植树 </summary>
            Tree,

            Count
        }

        public struct SmallIslandDatas
        {
            public Rectangle Box;
            public SmallIslandType IslandType;
            public int RandomType;

            public void PostGenerate()
            {

            }
        }

        public void GenSmallIslands(Rectangle mainRect, out List<SmallIslandDatas> SmallIslandDatas)
        {
            int smallIslandCount = ValueByWorldSize(
                WorldGen.genRand.Next(5, 8),
                WorldGen.genRand.Next(7, 12),
                WorldGen.genRand.Next(9, 14)
                );

            List<Rectangle> avoidRects = [mainRect];
            SmallIslandDatas = [];

            Rectangle expandRect = mainRect;

            Dictionary<Color, int> mainDic = new()
            {
                [new Color(51, 76, 117)] = ModContent.TileType<SkarnTile>(),//334c75
                [new Color(141, 171, 178)] = ModContent.TileType<SmoothSkarnTile>(),//8dabb2
                [new Color(184, 230, 207)] = ModContent.TileType<SkarnBrickTile>(),//b8e6cf
                [new Color(158, 77, 255)] = ModContent.TileType<CrystallineSkarnTile>(),//9e4dff

                [new Color(255, 239, 219)] = ModContent.TileType<ChalcedonyTile>(),//ffefdb
                [new Color(170, 228, 143)] = ModContent.TileType<LeafChalcedonyTile>(),//aae48f

                [new Color(147, 186, 84)] = ModContent.TileType<ChalcedonySkarn>(),//93ba54
                [new Color(95, 212, 111)] = ModContent.TileType<ChalcedonySmoothSkarn>(),//5fd46f

                [new Color(241, 130, 255)] = ModContent.TileType<CrystallineBrickTile>(),//f182ff
                [new Color(134, 156, 255)] = ModContent.TileType<CrystallineBlockTile>(),//869cff

                [new Color(223, 255, 255)] = TileID.Cloud,//dfffff
                [new Color(147, 144, 178)] = TileID.RainCloud,//9390b2

                [new Color(255, 112, 210)] = ModContent.TileType<MagicCrystalBlockTile>(),//ff70d2
                [new Color(255, 177, 230)] = ModContent.TileType<MagicCrystalBrickTile>(),//ffb1e6
                [new Color(211, 103, 156)] = ModContent.TileType<CrystalBasaltTile>(),//d3679c
                [new Color(54, 52, 58)] = ModContent.TileType<HardBasaltTile>(),//36343a
                [new Color(105, 97, 90)] = ModContent.TileType<BasaltTile>(),//69615a

                [new Color(90, 100, 80)] = TileID.Chain,//5a6450

                [Color.Black] = -1
            };
            Dictionary<Color, int> wallDic = new()
            {
                [new Color(85, 183, 206)] = ModContent.WallType<SmoothSkarnWallUnsafe>(),//55b7ce
                [new Color(188, 171, 150)] = ModContent.WallType<WildChalcedonyWallUnsafe>(),//bcab96
                [new Color(113, 128, 131)] = ModContent.WallType<SkarnBrickWallUnsafe>(),//718083
                [new Color(64, 77, 100)] = ModContent.WallType<CrackedSkarnWallUnsafe>(),//404d64
                [new Color(152, 158, 149)] = ModContent.WallType<ChalcedonyWallUnsafe>(),//989e95

                [new Color(54, 52, 58)] = ModContent.WallType<Walls.Magike.HardBasaltWall>(),//36343a

                [new Color(189, 202, 222)] = WallID.Cloud,//bdcade
                [Color.Black] = -1,
                [Color.White] = -2
            };

            for (int i = 0; i < smallIslandCount; i++)
            {
                //随机选择生成类型
                //之后不断扩展中心矩形，指导能够容纳小岛的生成

                SmallIslandType smallIslandType = (SmallIslandType)WorldGen.genRand.Next((int)SmallIslandType.Count);
                smallIslandType = SmallIslandType.Normal;

                int style = CSkyIslandRandStyle(smallIslandType);

                //获取类型，尺寸和贴图集合
                TextureGenerator data = new TextureGenerator(Enum.GetName(smallIslandType), style, AssetDirectory.CrystallineSmallIsland);
                data.SetWallTex();

                //外部尺寸
                int protect = WorldGen.genRand.Next(8, 18);
                Point outerSize = data.Size + new Point(protect, protect + data.Height);//让高度高一些

                Point SpawnTopLeft = default;
                bool success = false;

                do
                {
                    expandRect.X--;//扩张基础矩形
                    expandRect.Width += 2;
                    Rectangle currentRect = default;
                    for (int k = 0; k < 50; k++)//每次随机50次中心点
                    {
                        bool findPoint = true;

                        //直到随机到一个可以容纳的情况
                        Point p = WorldGen.genRand.NextVector2FromRectangle(expandRect).ToPoint();
                        currentRect = new Rectangle(p.X, p.Y, outerSize.X, outerSize.Y);

                        foreach (var rect in avoidRects)//检测碰撞
                            if (rect.Intersects(currentRect))
                            {
                                findPoint = false;
                                break;
                            }

                        if (findPoint)//未发生碰撞，成功找到可生成的位置
                        {
                            SpawnTopLeft = p + new Point(protect / 2, protect / 2 + data.Height / 2);
                            success = true;
                            break;
                        }
                    }

                    if (success)//成功找到位置
                    {
                        avoidRects.Add(currentRect);
                        SmallIslandDatas.Add(new SmallIslandDatas()
                        {
                            Box = currentRect,
                            IslandType = smallIslandType,
                            RandomType = data.Style.Value
                        });
                    }

                } while (!success);

                //成功找到了位置，开始生成
                data.GenerateByTopLeft(SpawnTopLeft, mainDic, wallDic);
            }

            //后续生成各种杂物等
            foreach (var data in SmallIslandDatas)
            {
                data.PostGenerate();
            }
        }

        public int CSkyIslandRandStyle(SmallIslandType smallIslandType)
        {
            switch (smallIslandType)
            {
                case SmallIslandType.Normal:
                    return 0;
                case SmallIslandType.Pool:
                    break;
                case SmallIslandType.Cave:
                    break;
                case SmallIslandType.Ruins:
                    break;
                case SmallIslandType.Chest:
                    break;
                case SmallIslandType.Pillar:
                    break;
                case SmallIslandType.Tree:
                    break;
                case SmallIslandType.Count:
                    break;
                default:
                    break;
            }

            return 0;
        }

        #endregion

        /// <summary>
        /// 传入位置，并找到最高点，实心块向上找，空心块向下找
        /// </summary>
        /// <param name="point"></param>
        public void FindTop(ref Point point)
        {
            Tile selfTile = Framing.GetTileSafely(point);

            if (selfTile.HasTile && Main.tileSolid[selfTile.TileType])//实心块，向上查找
            {
                for (int i = 0; i < 100; i++)
                {
                    point.Y--;
                    Tile upTile = Framing.GetTileSafely(point);
                    if (!upTile.HasTile || !Main.tileSolid[upTile.TileType])//找到没有物块的地方了，返回
                        return;
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    point.Y++;
                    Tile upTile = Framing.GetTileSafely(point);
                    if (upTile.HasTile && Main.tileSolid[upTile.TileType])//找到有物块的地方了，返回
                        return;
                }
            }
        }

        #region 存储与加载

        public static void SaveSkyIsland(TagCompound tag)
        {
            if (PlaceLightSoul)
                tag.Add(nameof(PlaceLightSoul), true);
            if (PlaceNightSoul)
                tag.Add(nameof(PlaceNightSoul), true);
            if (HasPermission)
                tag.Add(nameof(HasPermission), true);
        }

        public static void LoadSkyIsland(TagCompound tag)
        {
            PlaceLightSoul = tag.ContainsKey(nameof(PlaceLightSoul));
            PlaceNightSoul = tag.ContainsKey(nameof(PlaceLightSoul));
            HasPermission = tag.ContainsKey(nameof(PlaceLightSoul));
        }

        #endregion
    }
}
