﻿using Coralite.Content.CoraliteNotes.MagikeChapter1;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Content.WorldGeneration.Generators;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public static List<Point16> MagicCrystalCaveCenters = [];

        public static LocalizedText MagicCrystalCaveText { get; set; }

        public void GenMagicCrystalCave(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = MagicCrystalCaveText.Value;// "正在生成魔力水晶洞";

            int howManyToGen = 2;
            if (Main.maxTilesX > 8000)
            {
                howManyToGen += 2;
            }

            if (Main.maxTilesX > 6000)
            {
                howManyToGen += 2;
            }

            int howManyGened = 0;

            //最小X和最大X，这里是在X坐标在1/4 - 3/4内才能生成
            int minX = Main.maxTilesX / 4;
            int maxX = 3 * Main.maxTilesX / 4;

            //y坐标，限制在岩石层下方
            int minY = Main.maxTilesY / 2;
            int maxY = Main.maxTilesY - 300;

            for (int i = 0; i < 1000; i++)
            {
                //随机选取一个中心点
                Point origin = new(WorldGen.genRand.Next(minX, maxX), WorldGen.genRand.Next(minY, maxY));

                if (MagicCrystalCave(origin))
                {
                    howManyGened++;
                    progress.Value += howManyGened / (float)howManyToGen;
                    if (howManyGened >= howManyToGen)
                        break;
                }
            }

        }

        public bool MagicCrystalCave(Point origin)
        {
            //物块类型，提前声明出来变量方便后面使用
            ushort basalt = (ushort)ModContent.TileType<BasaltTile>();
            ushort crystalBasalt = (ushort)ModContent.TileType<CrystalBasaltTile>();
            ushort crystalBlock = (ushort)ModContent.TileType<MagicCrystalBlockTile>();

            //半径最小和最大值以及一半的墙壁厚度
            int radiusMin = 50;
            int radiusMax = 65;
            int wallWidth = 3;
            if (Main.maxTilesX > 8000)
            {
                radiusMin += 5;
                radiusMax += 5;
                wallWidth += 2;
            }

            if (Main.maxTilesX > 6000)
            {
                radiusMin += 5;
                radiusMax += 5;
                wallWidth += 1;
            }

            // 通过使用TileScanner，检查以原点为中心的50x50区域是否大部分是泥土或石头。
            Dictionary<ushort, int> tileDictionary = new();
            WorldUtils.Gen(
                new Point(origin.X - 25, origin.Y - 25),
                new Shapes.Rectangle(50, 50),
                new Actions.TileScanner(TileID.Dirt, TileID.Stone).Output(tileDictionary));
            if (tileDictionary[TileID.Dirt] + tileDictionary[TileID.Stone] < 750)
                return false; //如果不是，则返回false，这将导致调用方法尝试一个不同的origin。

            //随机一下X和Y的半径大小，让这个洞不那么圆
            float xScale = WorldGen.genRand.NextFloat(0.8f, 1.1f);
            float yScale = WorldGen.genRand.NextFloat(0.8f, 1.1f);

            int width = (int)(WorldGen.genRand.Next(radiusMin, radiusMax) * xScale);
            int height = (int)(WorldGen.genRand.Next(radiusMin, radiusMax) * yScale);
            // 检查结构图中我们希望放置的预定区域是否有任何现有冲突。
            if (!GenVars.structures.CanPlace(new Rectangle(origin.X - width, origin.Y - height, width * 2, height * 2)))
                return false;

            //原版提供的形状，可以使用一些生成器生成出形状来并用这个变量记录
            ShapeData innerData = new();
            ShapeData outerData = new();

            #region 主体圆环
            //挖出一个球，内部全空
            WorldUtils.Gen(
                origin,  //中心点
                new Shapes.Circle(width, height),   //形状：圆
                Actions.Chain(  //如果要添加多个效果得使用这个chain
                    new Modifiers.Blotches(4, 0.4),     //添加边缘的抖动，让边缘处不那么平滑
                    new Actions.Clear(),    //清除形状内所有物块
                    new Actions.SetFrames(true).Output(innerData)));   //通过output记录当前的形状

            //生成外围圈
            WorldUtils.Gen(
                origin,
                new ModShapes.OuterOutline(innerData),  //使用刚刚生成出来的形状，该形状的取外边缘
                Actions.Chain(
                    new Modifiers.Expand(wallWidth, wallWidth),  //扩展这个边缘线，得到一个圆环
                    new Actions.SetTile(basalt),    //放置物块
                    new Actions.SetFrames(true).Output(outerData)));    //存储圆环形状
            #endregion

            try
            {
                int howMany = BasaltFloatIsland(origin, width, height);
                howMany = WorldGen.genRand.Next(12, 24);

                //生成小起伏
                BasaltBall(origin, width, height, howMany);

                howMany = GenCrystalSpike(origin, width, height);
            }
            catch (System.Exception)
            {
                return false;
            }

            Point topLeft = origin - new Point(width, height);
            #region 废弃内容
            //int circleRadius = WorldGen.genRand.Next(8, 12);

            //int w = (int)(width * 0.75f);
            //int h = (int)(height * 0.75f);
            //Point circleCenter = new Point(point.X + WorldGen.genRand.Next(-w, w), point.Y + WorldGen.genRand.Next(-h, h));

            //ShapeData circle = new ShapeData();
            //ShapeData circleOut = new ShapeData();
            ////放置空心晶体砖球
            //WorldUtils.Gen(
            //    circleCenter,
            //    new Shapes.Circle(circleRadius),
            //    new Actions.Clear().Output(circle));

            //WorldUtils.Gen(
            //    circleCenter,
            //    new ModShapes.OuterOutline(circle, true),
            //    Actions.Chain(
            //        new Modifiers.Expand(1),
            //        new Actions.SetTileKeepWall(crystalBrick),
            //        new Actions.SetFrames(true).Output(circleOut)));

            ////添加破损
            //WorldUtils.Gen(
            //    circleCenter,
            //    new ModShapes.All(circleOut),
            //    Actions.Chain(
            //        new Modifiers.Dither(0.9),
            //        new Modifiers.Blotches(2, 0.1),
            //        new Modifiers.OnlyTiles(crystalBrick),
            //        new Actions.SetTileKeepWall(crystalBlock, setSelfFrames: true),
            //        new Modifiers.Dither(),
            //        new Actions.SetTileKeepWall(crystalBasalt, setSelfFrames: true)));


            ////放置装饰
            //for (int i = -1; i < 2; i += 2)
            //{
            //    int w2 = WorldGen.genRand.Next(3, circleRadius - 2);
            //    int h2 = WorldGen.genRand.Next(-4, 4);

            //    Point brokenLensPos = circleCenter + new Point(w2 * i, h2);
            //    Point bottom = brokenLensPos + new Point(0, 1);
            //    for (int k = 0; k < 20; k++)
            //    {
            //        Tile tile = Framing.GetTileSafely(bottom);
            //        if (tile.HasTile)
            //            break;

            //        bottom.Y++;
            //    }

            //    WorldUtils.Gen(
            //        brokenLensPos + new Point(0, 1),
            //        new Shapes.Rectangle(1, bottom.Y - brokenLensPos.Y),
            //        Actions.Chain(
            //        new Modifiers.Blotches(2, 0.6),
            //        new Actions.SetTile(crystalBrick)));

            //    WorldGen.PlaceTile(brokenLensPos.X, brokenLensPos.Y, crystalBrick);
            //    WorldGen.PlaceTile(brokenLensPos.X + 1, brokenLensPos.Y, crystalBrick);

            //    int brokenLensType = ModContent.TileType<BrokenLens>();
            //    TileObjectData data = TileObjectData.GetTileData(brokenLensType, 0);

            //    WorldGenHelper.ObjectPlace(brokenLensPos.X, brokenLensPos.Y - 1, brokenLensType, WorldGen.genRand.Next(data.RandomStyleRange));
            //}

            ////放置箱子
            //for (int i = 0; i < 4; i++)
            //    WorldGen.PlaceTile(circleCenter.X - 2 + i, circleCenter.Y + 1, crystalBrick);
            //for (int i = 0; i < 2; i++)
            //    WorldGen.PlaceTile(circleCenter.X - 1 + i, circleCenter.Y + 2, crystalBrick);
            #endregion

            GenCrystaClusters(origin, width, height);
            MagicCrystalCaveChest(origin);

            #region 废弃内容
            //for (int i = 0; i < 4; i++)
            //    for (int j = 0; j < 3; j++)
            //        WorldGen.KillTile(circleCenter.X - 2 + i, circleCenter.Y - j, false, true, true);

            //WorldGen.AddBuriedChest(circleCenter.X, circleCenter.Y,
            //    ModContent.ItemType<Page_MagikeBase>(), notNearOtherChests: false, 0, trySlope: false, chestTileType);


            //Point bottomPos = circleCenter + new Point(0, circleRadius + 3);
            //WorldUtils.Find(
            //    bottomPos,
            //    Searches.Chain(new Searches.Down(200),
            //    new Conditions.IsSolid()), out Point soildPoint);

            //WorldUtils.Gen(
            //    bottomPos,
            //    new Shapes.Rectangle(w, soildPoint.Y - bottomPos.Y),
            //    Actions.Chain(
            //        new Modifiers.Blotches(3, 0.2),
            //        new Actions.SetTile(basalt)));
            //随机房间
            //Rectangle room = new Rectangle(point.X + WorldGen.genRand.Next(-width, width),
            //    point.Y + WorldGen.genRand.Next(-height, height),
            //    WorldGen.genRand.Next(8, 18), WorldGen.genRand.Next(6, 8));


            //放置房间
            //WorldUtils.Gen(
            //    new Point(room.X, room.Y),
            //    new Shapes.Rectangle(room.Width, room.Height), 
            //    Actions.Chain(
            //        new Actions.SetTileKeepWall(crystalBrick), 
            //        new Actions.SetFrames(frameNeighbors: true)));

            ////清空内部并放置墙
            //WorldUtils.Gen(
            //    new Point(room.X + 1, room.Y + 1), 
            //    new Shapes.Rectangle(room.Width - 2, room.Height - 2), 
            //    Actions.Chain(
            //        //由于没有墙所以就仙布放
            //        new Actions.ClearTile(frameNeighbors: true)/*,  new Actions.PlaceWall(WallType)*/));

            //以下部分都莫得
            //放置椅子
            //放置门
            //放置平台
            //放置支撑梁
            //

            //为房间添加破旧效果
            //添加随机破损并替换方块

            //添加蜘蛛网?  不懂这个Stalagtite是什么意思
            //WorldUtils.Gen(
            //    new Point(room.X + 1, room.Y),
            //    new Shapes.Rectangle(room.Width - 2, 1), 
            //    Actions.Chain(
            //        new Modifiers.Dither(), 
            //        new Modifiers.OnlyTiles(crystalBasalt, basalt), 
            //        new Modifiers.Offset(0, 1), 
            //        new ActionStalagtite()));

            //WorldUtils.Gen(
            //    new Point(room.X + 1, room.Y + room.Height - 1), 
            //    new Shapes.Rectangle(room.Width - 2, 1), 
            //    Actions.Chain(
            //        new Modifiers.Dither(), 
            //        new Modifiers.OnlyTiles(crystalBasalt, basalt), 
            //        new Modifiers.Offset(0, 1), 
            //        new ActionStalagtite()));

            //替换其他的墙
            //WorldUtils.Gen(
            //    new Point(room.X, room.Y), 
            //    new Shapes.Rectangle(room.Width, room.Height), 
            //    Actions.Chain(
            //        new Modifiers.Dither(0.8), 
            //        new Modifiers.Blotches(), 
            //        new Modifiers.OnlyWalls(base.WallType), 
            //        new Actions.PlaceWall(216)));

            //ushort chestTileType = (ushort)ModContent.TileType<BasaltChestTile>();

            //WorldGen.AddBuriedChest(WorldGen.genRand.Next(2, room.Width - 2) + room.X, room.Height - 1 + room.Y, 
            //    ModContent.ItemType<Page_MagikeBase>(), notNearOtherChests: false, 0, trySlope: false, chestTileType);

            #endregion

            AddCrystalCaveDecoration(width, height, topLeft);
            GenVars.structures.AddProtectedStructure(new Rectangle(origin.X - width, origin.Y - height, width * 2, height * 2), 10);

            MagicCrystalCaveCenters.Add(new Point16(origin.X, origin.Y));
            return true;
        }

        /// <summary>
        /// 添加装饰
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="topLeft"></param>
        private static void AddCrystalCaveDecoration(int width, int height, Point topLeft)
        {
            ushort basalt = (ushort)ModContent.TileType<BasaltTile>();
            ushort crystalBasalt = (ushort)ModContent.TileType<CrystalBasaltTile>();
            ushort crystalBlock = (ushort)ModContent.TileType<MagicCrystalBlockTile>();

            //添加斜坡
            WorldGenHelper.SmoothSlope(topLeft.X, topLeft.Y, 0, 0, width * 2, height * 2, basalt, 5);
            WorldGenHelper.SmoothSlope(topLeft.X, topLeft.Y, 0, 0, width * 2, height * 2, crystalBasalt, 5);
            WorldGenHelper.SmoothSlope(topLeft.X, topLeft.Y, 0, 0, width * 2, height * 2, crystalBlock, 2);

            //生成小装饰物块
            WorldGenHelper.PlaceOnTopDecorations(topLeft.X, topLeft.Y, 0, 0, (int)(width * 2.2f), (int)(height * 3f), (ushort)ModContent.TileType<BigCrystalStalactiteTop>(), 8, 0, basalt);
            WorldGenHelper.PlaceOnGroundDecorations(topLeft.X, topLeft.Y, 0, 0, (int)(width * 2.2f), (int)(height * 3f), (ushort)ModContent.TileType<BigCrystalStalactiteBottom>(), 8, 0, basalt);

            WorldGenHelper.PlaceOnLeftDecorations(topLeft.X, topLeft.Y, 0, 0, (int)(width * 2.2f), (int)(height * 3f), (ushort)ModContent.TileType<CrystalStalactiteLeft>(), 12, 0, basalt);
            WorldGenHelper.PlaceOnRightDecorations(topLeft.X, topLeft.Y, 0, 0, (int)(width * 2.2f), (int)(height * 3f), (ushort)ModContent.TileType<CrystalStalactiteRight>(), 12, 0, basalt);
            WorldGenHelper.PlaceOnTopDecorations(topLeft.X, topLeft.Y, 0, 0, (int)(width * 2.2f), (int)(height * 3f), (ushort)ModContent.TileType<CrystalStalactiteTop>(), 12, 0, basalt);
            WorldGenHelper.PlaceOnGroundDecorations(topLeft.X, topLeft.Y, 0, 0, (int)(width * 2.2f), (int)(height * 3f), (ushort)ModContent.TileType<CrystalStalactiteBottom>(), 12, 0, basalt);

            WorldGenHelper.PlaceOnTopDecorations(topLeft.X, topLeft.Y, 0, 0, (int)(width * 2.2f), (int)(height * 3f), (ushort)ModContent.TileType<BasaltStalactiteTop2>(), 3, 0, basalt);
            WorldGenHelper.PlaceOnGroundDecorations(topLeft.X, topLeft.Y, 0, 0, (int)(width * 2.2f), (int)(height * 3f), (ushort)ModContent.TileType<BasaltStalactiteBottom2>(), 3, 0, basalt);

            WorldGenHelper.PlaceOnTopDecorations(topLeft.X, topLeft.Y, 0, 0, (int)(width * 2.2f), (int)(height * 3f), (ushort)ModContent.TileType<BasaltStalactiteTop>(), 4, 0, basalt);
            WorldGenHelper.PlaceOnGroundDecorations(topLeft.X, topLeft.Y, 0, 0, (int)(width * 2.2f), (int)(height * 3f), (ushort)ModContent.TileType<BasaltStalactiteBottom>(), 4, 0, basalt);
        }

        /// <summary>
        /// 生成水晶簇
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void GenCrystaClusters(Point origin, int width, int height)
        {
            ushort basalt = (ushort)ModContent.TileType<BasaltTile>();

            int clustersSpawnCount = WorldGen.genRand.Next(1, 4);

            int clustersX = origin.X + width / 6;
            int clustersY = origin.Y + height / 6;
            int clustersType = ModContent.TileType<MagicCrystalClustersTile>();

            for (int i = 0; i < clustersSpawnCount; i++)
                for (int j = 0; j < 1000; j++)
                {
                    if (j == 999)
                    {
                        int x1 = clustersX + WorldGen.genRand.Next(0, width * 5 / 6);
                        int y1 = clustersY + WorldGen.genRand.Next(0, height * 5 / 6);

                        for (int k = 0; k < 3; k++)//检测底部空间
                            Main.tile[x1 + k, y1].ResetToType(basalt);

                        for (int m = 0; m < 3; m++)//清空底部空间
                            for (int n = -1; n > -5; n--)
                                Main.tile[x1 + m, y1 + n].ClearTile();

                        WorldGen.PlaceObject(x1, y1 - 1, clustersType);
                    }

                    int x = clustersX + WorldGen.genRand.Next(0, width * 5 / 6);
                    int y = clustersY + WorldGen.genRand.Next(0, height * 5 / 6);

                    bool canGenerate = true;

                    for (int k = 0; k < 3; k++)//检测底部空间
                    {
                        Tile bottonTile = Main.tile[x + k, y];
                        if (!bottonTile.HasTile || bottonTile.TileType != basalt)
                        {
                            canGenerate = false;
                            break;
                        }
                    }

                    if (!canGenerate)
                        continue;

                    for (int m = 0; m < 3; m++)//检测底部空间
                        for (int n = -1; n > -5; n--)
                        {
                            Tile t = Main.tile[x + m, y + n];
                            if (t.HasTile)
                            {
                                canGenerate = false;
                                break;
                            }
                        }

                    if (!canGenerate)
                        continue;

                    WorldGen.PlaceObject(x, y - 1, clustersType);
                    break;
                }
        }

        /// <summary>
        /// 生成水晶锥
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static int GenCrystalSpike(Point origin, int width, int height)
        {
            ushort crystalBasalt = (ushort)ModContent.TileType<CrystalBasaltTile>();
            ushort crystalBlock = (ushort)ModContent.TileType<MagicCrystalBlockTile>();

            //生成玄武岩水晶锥
            int howMany = WorldGen.genRand.Next(8, 14);
            for (int i = 0; i < howMany; i++)
            {
                //在圆环上取点，并获取该点指向中心的单位向量
                Point selfPoint = origin + Main.rand.NextVector2CircularEdge(width, height).ToPoint() + new Point(WorldGen.genRand.Next(-3, 3), WorldGen.genRand.Next(-3, 3));
                if (!WorldGen.InWorld(selfPoint.X, selfPoint.Y, 100))
                    continue;
                Vector2 dir = (origin.ToVector2() - selfPoint.ToVector2()).SafeNormalize(Vector2.Zero).RotatedBy(WorldGen.genRand.NextFloat(-0.4f, 0.4f));
                WorldUtils.Gen(
                    selfPoint,
                    //锥形，第一个参数是初始宽度，第二个参数是方向*距离
                    new Shapes.Tail(WorldGen.genRand.NextFloat(width * 0.08f, width * 0.1f), dir.ToVector2D() * WorldGen.genRand.NextFloat(width * 0.05f, width * 0.3f)),
                    Actions.Chain(
                        new Actions.SetTile(crystalBasalt),
                        new Actions.SetFrames(true)));
            }

            howMany = WorldGen.genRand.Next(12, 24);

            //生成水晶锥
            for (int i = 0; i < howMany; i++)
            {
                Point selfPoint = origin + Main.rand.NextVector2CircularEdge(width, height).ToPoint() + new Point(WorldGen.genRand.Next(-3, 3), WorldGen.genRand.Next(-3, 3));
                if (!WorldGen.InWorld(selfPoint.X, selfPoint.Y, 100))
                    continue;

                Vector2 dir = (origin.ToVector2() - selfPoint.ToVector2()).SafeNormalize(Vector2.Zero).RotatedBy(WorldGen.genRand.NextFloat(-0.4f, 0.4f));
                WorldUtils.Gen(
                    selfPoint,
                    new Shapes.Tail(WorldGen.genRand.NextFloat(width * 0.1f, width * 0.2f), dir.ToVector2D() * WorldGen.genRand.NextFloat(width * 0.05f, width * 0.55f)),
                    Actions.Chain(
                        new Actions.SetTile(crystalBlock),
                        new Actions.SetFrames(true)));
            }

            return howMany;
        }

        private static void BasaltBall(Point origin, int width, int height, int howMany)
        {
            ushort basalt = (ushort)ModContent.TileType<BasaltTile>();

            for (int i = 0; i < howMany; i++)
            {
                //在圆环上取点，之后在圆环上随机添加一些小凸起
                Point selfPoint = origin + Main.rand.NextVector2CircularEdge(width, height).ToPoint() + new Point(WorldGen.genRand.Next(-8, 8), WorldGen.genRand.Next(-3, 3));
                if (!WorldGen.InWorld(selfPoint.X, selfPoint.Y, 100))
                    continue;

                WorldUtils.Gen(
                    selfPoint,
                    new Shapes.Slime((int)WorldGen.genRand.NextFloat(width * 0.1f, width * 0.2f)),
                    Actions.Chain(
                        new Actions.SetTile(basalt),
                        new Actions.SetFrames(true)));
            }
        }

        /// <summary>
        /// 生成浮岛
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static int BasaltFloatIsland(Point origin, int width, int height)
        {
            //生成中心水晶小浮岛
            int howMany = WorldGen.genRand.Next(4, 10);

            ushort basalt = (ushort)ModContent.TileType<BasaltTile>();
            ushort crystalBasalt = (ushort)ModContent.TileType<CrystalBasaltTile>();
            ushort crystalBlock = (ushort)ModContent.TileType<MagicCrystalBlockTile>();

            for (int i = 0; i < howMany; i++)
            {
                //随机取点，加了点限制让这个点不会在圆环外
                Point selfPoint = origin + Main.rand.NextVector2Circular(width * 0.75f, height * 0.75f).ToPoint() + new Point(WorldGen.genRand.Next(-8, 8), WorldGen.genRand.Next(-3, 3));
                if (!WorldGen.InWorld(selfPoint.X, selfPoint.Y, 100))
                    continue;

                WorldUtils.Gen(
                    selfPoint,
                    //史莱姆形状，同时压缩Y方向的大小，让它变得扁平
                    new Shapes.Slime((int)WorldGen.genRand.NextFloat(width * 0.05f, width * 0.08f), 1f, WorldGen.genRand.NextFloat(0.25f, 0.5f)),
                    Actions.Chain(
                        new Modifiers.Flip(false, true),    //竖直翻转一下
                        new Modifiers.Blotches(2, 0.4),     //添加边缘抖动
                        new Actions.SetTile(crystalBasalt),     //放置物块
                        new Actions.SetFrames(true)));      //设置物块帧
            }

            howMany = WorldGen.genRand.Next(3, 6);

            for (int i = 0; i < howMany; i++)
            {
                Point selfPoint = origin + Main.rand.NextVector2Circular(width * 0.75f, height * 0.75f).ToPoint() + new Point(WorldGen.genRand.Next(-8, 8), WorldGen.genRand.Next(-3, 3));
                if (!WorldGen.InWorld(selfPoint.X, selfPoint.Y, 100))
                    continue;

                WorldUtils.Gen(
                    selfPoint,
                    new Shapes.Slime((int)WorldGen.genRand.NextFloat(width * 0.05f, width * 0.08f), 1f, WorldGen.genRand.NextFloat(0.25f, 0.5f)),
                    Actions.Chain(
                        new Modifiers.Flip(false, true),
                        new Modifiers.Blotches(2, 0.4),
                        new Actions.SetTile(crystalBlock),
                        new Actions.SetFrames(true)));
            }

            //生成中心大浮岛
            for (int i = 0; i < 3; i++)
            {
                Point selfPoint = origin + Main.rand.NextVector2Circular(width * 0.9f, height * 0.9f).ToPoint() + new Point(WorldGen.genRand.Next(-8, 8), WorldGen.genRand.Next(-3, 3));
                WorldUtils.Gen(
                    selfPoint,
                    new Shapes.Slime((int)(width * 0.4f), WorldGen.genRand.NextFloat(1.2f, 1.5f), WorldGen.genRand.NextFloat(0.1f, 0.3f)),
                    Actions.Chain(
                        new Modifiers.Blotches(2, 0.4),
                        new Actions.SetTile(basalt),
                        new Actions.SetFrames(true)));
            }

            howMany = WorldGen.genRand.Next(8, 12);

            for (int i = 0; i < howMany; i++)
            {
                Point selfPoint = origin + Main.rand.NextVector2Circular(width, height).ToPoint() + new Point(WorldGen.genRand.Next(-8, 8), WorldGen.genRand.Next(-3, 3));
                WorldUtils.Gen(
                    selfPoint,
                    new Shapes.Slime((int)WorldGen.genRand.NextFloat(width * 0.05f, width * 0.15f), WorldGen.genRand.NextFloat(1f, 1.5f), WorldGen.genRand.NextFloat(0.3f, 0.6f)),
                    Actions.Chain(
                        new Modifiers.Blotches(2, 0.4),
                        new Actions.SetTile(basalt),
                        new Actions.SetFrames(true)));
            }

            return howMany;
        }

        /// <summary>
        /// 生成魔力水晶洞中心小神龛
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="basalt"></param>
        /// <param name="crystalBlock"></param>
        /// <param name="crystalBrick"></param>
        private Point MagicCrystalCaveChest(Point origin)
        {
            int whichOne = WorldGen.genRand.Next(5);

            ushort basalt = (ushort)ModContent.TileType<BasaltTile>();
            ushort crystalBlock = (ushort)ModContent.TileType<MagicCrystalBlockTile>();
            ushort crystalBrick = (ushort)ModContent.TileType<MagicCrystalBrickTile>();

            Texture2D shrineTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "MagicCrystalShrine" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;
            Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "MagicCrystalShrineClear" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;
            Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "MagicCrystalWall" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;

            int genOrigin_x = origin.X - (clearTex.Width / 2);
            int genOrigin_y = origin.Y - (clearTex.Height / 2);

            Point chestPos = new(genOrigin_x + 13, genOrigin_y + 13);
            Point lightPoint1 = new(genOrigin_x + 7, genOrigin_y + 14);
            Point lightPoint2 = new(genOrigin_x + 17, genOrigin_y + 14);

            Dictionary<Color, int> clearDic = new()
            {
                [Color.White] = -2,
                [Color.Black] = -1
            };
            Dictionary<Color, int> mainDic = new()
            {
                [new Color(255, 112, 210)] = crystalBlock,
                [new Color(255, 177, 230)] = crystalBrick,
                [new Color(142, 43, 170)] = ModContent.TileType<HardBasaltTile>(),
                [new Color(183, 12, 232)] = basalt,
                [new Color(90, 100, 80)] = TileID.Chain,
                [Color.Black] = -1
            };
            Dictionary<Color, int> wallDic = new()
            {
                [new Color(255, 255, 0)] = ModContent.WallType<Walls.Magike.HardBasaltWall>(),
                [Color.Black] = -1
            };

            Task.Run(async () =>
            {
                await GenShrine(clearTex, shrineTex, wallTex, clearDic, mainDic, wallDic, genOrigin_x, genOrigin_y);
            }).Wait();

            //放置灯
            int brokenLensType = ModContent.TileType<BrokenLens>();
            TileObjectData data = TileObjectData.GetTileData(brokenLensType, 0);

            WorldGen.PlaceObject(lightPoint1.X, lightPoint1.Y, brokenLensType, true, WorldGen.genRand.Next(data.RandomStyleRange));
            WorldGen.PlaceObject(lightPoint2.X, lightPoint2.Y, brokenLensType, true, WorldGen.genRand.Next(data.RandomStyleRange));

            //放置箱子
            ushort chestTileType = (ushort)ModContent.TileType<BasaltChestTile>();
            WorldGen.AddBuriedChest(chestPos.X, chestPos.Y,
                ModContent.ItemType<MagikeBasePage>(), notNearOtherChests: false, 0, trySlope: false, chestTileType);

            return chestPos;
        }

        public Task GenShrine(Texture2D clearTex, Texture2D shrineTex, Texture2D wallTex,
             Dictionary<Color, int> clearDic, Dictionary<Color, int> shrineDic, Dictionary<Color, int> wallDic,
             int genOrigin_x, int genOrigin_y)
        {
            bool genned = false;
            bool placed = false;
            while (!genned)
            {
                if (placed)
                    continue;

                Main.QueueMainThreadAction(() =>
                {
                    //清理范围
                    Texture2TileGenerator clearGenerator = TextureGeneratorDatas.GetTex2TileGenerator(clearTex, clearDic);
                    clearGenerator.Generate(genOrigin_x, genOrigin_y, true);

                    //生成主体地形
                    Texture2TileGenerator shrineGenerator = TextureGeneratorDatas.GetTex2TileGenerator(shrineTex, shrineDic);
                    shrineGenerator.Generate(genOrigin_x, genOrigin_y, true);

                    //生成墙壁
                    Texture2WallGenerator wallGenerator = TextureGeneratorDatas.GetTex2WallGenerator(wallTex, wallDic);
                    wallGenerator.Generate(genOrigin_x, genOrigin_y, true);

                    genned = true;
                });
                placed = true;
            }

            return Task.CompletedTask;
        }

        public void LoadCrystalCave(TagCompound tag)
        {
            MagicCrystalCaveCenters.Clear();

            int i = 0;
            while (tag.TryGet("CrystalCave" + i, out Point16 pos))
            {
                MagicCrystalCaveCenters.Add(pos);
                i++;
            }
        }

        public void SaveCrystalCave(TagCompound tag)
        {
            int i = 0;
            foreach (var pos in MagicCrystalCaveCenters)
            {
                tag.Add("CrystalCave" + i, pos);
                i++;
            }
        }
    }
}
