using Coralite.Content.WorldGeneration.Generators;
using Coralite.Core;
using Coralite.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.WorldGeneration
{
    public static class WorldGenHelper
    {
        /// <summary>
        /// 向箱子中的随机位置添加物品
        /// </summary>
        /// <param name="chest"></param>
        /// <param name="itemtype"></param>
        /// <param name="stack"></param>
        public static void RandChestItem(Chest chest, int itemtype, int stack = 1)
        {
            int itemIndex = WorldGen.genRand.Next(0, chest.item.Length);
            int limit = 0;
            while (!chest.item[itemIndex].IsAir && limit < chest.item.Length * 2)
            {
                limit++;
                itemIndex = WorldGen.genRand.Next(0, chest.item.Length);
            }

            chest.item[itemIndex].SetDefaults(itemtype);
            chest.item[itemIndex].stack = stack;
        }

        /// <summary>
        /// 生成矿物
        /// </summary>
        /// <param name="type">矿物类型</param>
        /// <param name="frequency">生成数量，建议为0.0001X的</param>
        /// <param name="depth">最高深度，是百分比，是百分比</param>
        /// <param name="depthLimit">最低深度</param>
        /// <param name="Condition">可以用来让矿物只能在指定的物块上生成</param>
        public static void GenerateOre(int type, double frequency, float depth, float depthLimit, Func<int, int, bool> Condition)
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < (int)(x * y * frequency); i++)
                {
                    int tilesX = WorldGen.genRand.Next(0, x);
                    int tilesY = WorldGen.genRand.Next((int)(y * depth), (int)(y * depthLimit));
                    if (Condition(tilesX, tilesY))
                        WorldGen.OreRunner(tilesX, tilesY, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                }
            }
        }

        public static void ObjectPlace(int x, int y, int TileType, int style = 0, int direction = -1)
        {
            WorldGen.PlaceObject(x, y, TileType, true, style, 0, -1, direction);
            //NetMessage.SendObjectPlacement(-1, x, y, TileType, style, 0, -1, direction);
        }

        /// <summary>
        /// 平滑边缘，使边缘变成斜坡，前提是这个物块要能变成斜坡的才行
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="tileType"></param>
        /// <param name="random"></param>
        public static void SmoothSlope(Point origin, int startX, int startY, int endX, int endY, ushort tileType, int random)
        {
            SmoothSlope(origin.X, origin.Y, startX, startY, endX, endY, tileType, random);
        }

        public static void SmoothSlope(int origin_x, int origin_y, int startX, int startY, int endX, int endY, ushort tileType, int random)
        {
            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    Tile me = Framing.GetTileSafely(origin_x + i, origin_y + j);
                    if (me.TileType == tileType)
                        if (WorldGen.genRand.NextBool(random))
                            Tile.SmoothSlope(origin_x + i, origin_y + j, false);
                }
            }
        }

        /// <summary>
        /// 用于随机放置底部必须有物块的大型物块
        /// 需要满足底部一条有方块且大型物块的边缘一圈必须没有方块才能将放下来
        /// </summary>
        /// <param name="origin">你的origin</param>
        /// <param name="startX">要在哪个区域随机生成的左上角X值</param>
        /// <param name="startY">要在哪个区域随机生成的左上角Y值</param>
        /// <param name="endX">要在哪个区域随机生成的右下角X值</param>
        /// <param name="endY">要在哪个区域随机生成的右下角X值</param>
        /// <param name="tileType">物块种类</param>
        /// <param name="random">决定是否生成的随机数，越大越难生成</param>
        /// <param name="bottomTileType">底部物块种类，如有需要的话</param>
        public static void PlaceOnGroundDecorations(int origin_x, int origin_y, int startX, int startY, int endX, int endY, ushort tileType, int random = 10, int style = 0, int bottomTileType = -1)
        {
            TileObjectData data = TileObjectData.GetTileData(tileType, style);
            int width = data == null ? 1 : data.Width;
            int height = data == null ? 1 : data.Height;
            int randomTile = data == null ? 1 : data.RandomStyleRange;

            for (int i = startX; i < endX; i++)
                for (int j = startY; j < endY; j++)
                {
                    Tile tile;
                    int current_x = origin_x + i;
                    int current_y = origin_y + j;
                    //判断底部一条是不是都有方块,且方块类型是不是指定的
                    for (int m = 0; m < width; m++)
                    {
                        tile = Framing.GetTileSafely(current_x + m, current_y + height);
                        if (!tile.HasTile || tile.Slope is SlopeType.SlopeDownLeft or SlopeType.SlopeDownRight || tile.IsHalfBlock)
                            goto over1;

                        if (bottomTileType != -1)
                            if (tile.TileType != bottomTileType)
                                goto over1;
                    }

                    for (int m = 0; m < width; m++)
                        for (int n = 0; n < height; n++)
                        {
                            tile = Framing.GetTileSafely(current_x + m, current_y + n);
                            if (tile.HasTile)
                                goto over1;
                        }

                    //添加一些随机性
                    if (WorldGen.genRand.NextBool(random))
                    {
                        int currentStyle;
                        if (randomTile <= 1)
                            currentStyle = 0;
                        else
                            currentStyle = WorldGen.genRand.Next(0, randomTile);
                        ObjectPlace(current_x, current_y, tileType, currentStyle);
                    }

                over1: continue;             //<--因为不知道有没有什么办法直接跳出2层for，索性写了个goto
                }
        }

        /// <summary>
        /// 用于随机放置底部必须有物块的大型物块
        /// 需要满足底部一条有方块且大型物块的边缘一圈必须没有方块才能将放下来，并且是在左下角为基点放置
        /// </summary>
        /// <param name="origin">你的origin</param>
        /// <param name="startX">要在哪个区域随机生成的左上角X值</param>
        /// <param name="startY">要在哪个区域随机生成的左上角Y值</param>
        /// <param name="endX">要在哪个区域随机生成的右下角X值</param>
        /// <param name="endY">要在哪个区域随机生成的右下角X值</param>
        /// <param name="tileType">物块种类</param>
        /// <param name="random">决定是否生成的随机数，越大越难生成</param>
        /// <param name="bottomTileType">底部物块种类，如有需要的话</param>
        public static void PlaceDecorations_NoCheck(int origin_x, int origin_y, int startX, int startY, int endX, int endY, ushort tileType, Func<int> direction, int random = 10)
        {
            TileObjectData data = TileObjectData.GetTileData(tileType, 0);
            int randomTile = data == null ? 1 : data.RandomStyleRange;

            for (int i = startX; i < endX; i++)
                for (int j = startY; j < endY; j++)
                {
                    int current_x = origin_x + i;
                    int current_y = origin_y + j;
                    //添加一些随机性
                    if (WorldGen.genRand.NextBool(random))
                    {
                        int currentStyle;
                        if (randomTile <= 1)
                            currentStyle = 0;
                        else
                            currentStyle = WorldGen.genRand.Next(0, randomTile);
                        ObjectPlace(current_x, current_y, tileType, currentStyle, direction());
                    }
                }
        }

        public static void PlaceDecorations_NoCheck(Rectangle area, ushort tileType, int random = 10, Func<int> direction = null, Rectangle? avoidArea = null)
        {
            TileObjectData data = TileObjectData.GetTileData(tileType, 0);
            int randomTile = data == null ? 1 : data.RandomStyleRange;

            for (int i = 0; i < area.Width; i++)
                for (int j = 0; j < area.Height; j++)
                {
                    int current_x = area.X + i;
                    int current_y = area.Y + j;

                    if (avoidArea != null)
                        if (avoidArea.Value.Contains(current_x, current_y))
                            continue;

                    //添加一些随机性
                    if (WorldGen.genRand.NextBool(random))
                    {
                        int currentStyle;
                        if (randomTile <= 1)
                            currentStyle = 0;
                        else
                            currentStyle = WorldGen.genRand.Next(0, randomTile);
                        ObjectPlace(current_x, current_y, tileType, currentStyle, direction == null ? -1 : direction());
                    }
                }
        }

        public static void PlaceDecorations_NoCheck2(Rectangle area, ushort tileType, int random = 10, Func<int> direction = null, List<Rectangle> avoidArea = null)
        {
            TileObjectData data = TileObjectData.GetTileData(tileType, 0);
            int randomTile = data == null ? 1 : data.RandomStyleRange;

            for (int i = 0; i < area.Width; i++)
                for (int j = 0; j < area.Height; j++)
                {
                    int current_x = area.X + i;
                    int current_y = area.Y + j;

                    if (avoidArea != null)
                    {
                        bool contain = false;
                        foreach (var r in avoidArea)
                            if (r.Contains(current_x, current_y))
                            {
                                contain = true;
                                break;
                            }

                        if (contain)
                            continue;
                    }

                    //添加一些随机性
                    if (WorldGen.genRand.NextBool(random))
                    {
                        int currentStyle;
                        if (randomTile <= 1)
                            currentStyle = 0;
                        else
                            currentStyle = WorldGen.genRand.Next(0, randomTile);
                        ObjectPlace(current_x, current_y, tileType, currentStyle, direction == null ? -1 : direction());
                    }
                }
        }

        public static void PlaceOnTopDecorations(int origin_x, int origin_y, int startX, int startY, int endX, int endY, ushort tileType, int random = 10, int style = 0, int topTileType = -1)
        {
            TileObjectData data = TileObjectData.GetTileData(tileType, style);
            int width = data == null ? 1 : data.Width;
            int height = data == null ? 1 : data.Height;
            int randomTile = data == null ? 1 : data.RandomStyleRange;

            for (int i = startX; i < endX; i++)
                for (int j = startY; j < endY; j++)
                {
                    Tile tile;
                    int current_x = origin_x + i;
                    int current_y = origin_y + j;
                    //判断顶部一条是不是都有方块,且方块类型是不是指定的
                    for (int m = 0; m < width; m++)
                    {
                        tile = Framing.GetTileSafely(current_x + m, current_y - 1);
                        if (!tile.HasTile || tile.Slope is SlopeType.SlopeUpLeft or SlopeType.SlopeUpRight)
                            goto over1;

                        if (topTileType != -1)
                            if (tile.TileType != topTileType)
                                goto over1;
                    }

                    for (int m = 0; m < width; m++)
                        for (int n = 0; n < height; n++)
                        {
                            tile = Framing.GetTileSafely(current_x + m, current_y + n);
                            if (tile.HasTile)
                                goto over1;
                        }

                    //添加一些随机性
                    if (WorldGen.genRand.NextBool(random))
                    {
                        int currentStyle;
                        if (randomTile <= 1)
                            currentStyle = 0;
                        else
                            currentStyle = WorldGen.genRand.Next(0, randomTile);
                        ObjectPlace(current_x, current_y, tileType, currentStyle);
                    }

                over1: continue;             //<--因为不知道有没有什么办法直接跳出2层for，索性写了个goto
                }
        }

        public static void PlaceOnLeftDecorations(int origin_x, int origin_y, int startX, int startY, int endX, int endY, ushort tileType, int random = 10, int style = 0, int topTileType = -1)
        {
            TileObjectData data = TileObjectData.GetTileData(tileType, style);
            int width = data == null ? 1 : data.Width;
            int height = data == null ? 1 : data.Height;
            int randomTile = data == null ? 1 : data.RandomStyleRange;

            for (int i = startX; i < endX; i++)
                for (int j = startY; j < endY; j++)
                {
                    Tile tile;
                    int current_x = origin_x + i;
                    int current_y = origin_y + j;
                    //判断左边一条是不是都有方块,且方块类型是不是指定的
                    for (int m = 0; m < height; m++)
                    {
                        tile = Framing.GetTileSafely(current_x - 1, current_y + m);
                        if (!tile.HasTile || tile.Slope is SlopeType.SlopeUpLeft or SlopeType.SlopeDownLeft || tile.IsHalfBlock)
                            goto over1;

                        if (topTileType != -1)
                            if (tile.TileType != topTileType)
                                goto over1;
                    }

                    for (int m = 0; m < width; m++)
                        for (int n = 0; n < height; n++)
                        {
                            tile = Framing.GetTileSafely(current_x + m, current_y + n);
                            if (tile.HasTile)
                                goto over1;
                        }

                    //添加一些随机性
                    if (WorldGen.genRand.NextBool(random))
                    {
                        int currentStyle;
                        if (randomTile <= 1)
                            currentStyle = 0;
                        else
                            currentStyle = WorldGen.genRand.Next(0, randomTile);
                        ObjectPlace(current_x, current_y, tileType, currentStyle);
                    }

                over1: continue;             //<--因为不知道有没有什么办法直接跳出2层for，索性写了个goto
                }
        }

        public static void PlaceOnRightDecorations(int origin_x, int origin_y, int startX, int startY, int endX, int endY, ushort tileType, int random = 10, int style = 0, int topTileType = -1)
        {
            TileObjectData data = TileObjectData.GetTileData(tileType, style);
            int width = data == null ? 1 : data.Width;
            int height = data == null ? 1 : data.Height;
            int randomTile = data == null ? 1 : data.RandomStyleRange;

            for (int i = startX; i < endX; i++)
                for (int j = startY; j < endY; j++)
                {
                    Tile tile;
                    int current_x = origin_x + i;
                    int current_y = origin_y + j;
                    //判断右边一条是不是都有方块,且方块类型是不是指定的
                    for (int m = 0; m < height; m++)
                    {
                        tile = Framing.GetTileSafely(current_x + width, current_y + m);
                        if (!tile.HasTile || tile.Slope is SlopeType.SlopeUpRight or SlopeType.SlopeDownRight || tile.IsHalfBlock)
                            goto over1;

                        if (topTileType != -1)
                            if (tile.TileType != topTileType)
                                goto over1;
                    }

                    for (int m = 0; m < width; m++)
                        for (int n = 0; n < height; n++)
                        {
                            tile = Framing.GetTileSafely(current_x + m, current_y + n);
                            if (tile.HasTile)
                                goto over1;
                        }

                    //添加一些随机性
                    if (WorldGen.genRand.NextBool(random))
                    {
                        int currentStyle;
                        if (randomTile <= 1)
                            currentStyle = 0;
                        else
                            currentStyle = WorldGen.genRand.Next(0, randomTile);
                        ObjectPlace(current_x, current_y, tileType, currentStyle);
                    }

                over1:
                    continue;             //<--因为不知道有没有什么办法直接跳出2层for，索性写了个goto
                }
        }


        public static bool KillChestAndItems(int x, int y)
        {
            Tile t = Main.tile[x, y];
            TileObjectData data = TileObjectData.GetTileData(t);
            int i = Chest.FindChest(x, y);
            if (i != -1)
            {
                Chest.DestroyChestDirect(x, y, i);
                return true;
            }

            for (int m = 0; m < data.Width; m++)
                for (int n = 0; n < data.Height; n++)
                {
                    Main.tile[x + m, y + n].Clear(TileDataType.Tile);
                }

            return false;
        }

        public static void GenerateLiquid(int x, int y, int liquidType, bool updateFlow = true, int liquidHeight = 255, bool sync = true)
        {
            Tile Mtile = Main.tile[x, y];

            if (!WorldGen.InWorld(x, y))
                return;

            liquidHeight = (int)MathHelper.Clamp(liquidHeight, 0, 255);
            Main.tile[x, y].LiquidAmount = (byte)liquidHeight;

            if (liquidType == 0)
                Mtile.LiquidType = LiquidID.Water;
            else if (liquidType == 1)
                Mtile.LiquidType = LiquidID.Lava;
            else if (liquidType == 2)
                Mtile.LiquidType = LiquidID.Honey;
            if (updateFlow)
                Liquid.AddWater(x, y);
            if (sync && Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, x, y, 1);
        }

        public static void Texture2TileGenerate(int x, int y, int tileType, int tileStyle = 0, bool active = true)
        {
            try
            {
                Tile Mtile = Main.tile[x, y];

                if (!WorldGen.InWorld(x, y))
                    return;

                TileObjectData data = tileType <= -1 ? null : TileObjectData.GetTileData(tileType, tileStyle);
                int width = data == null ? 1 : data.Width;
                int height = data == null ? 1 : data.Height;
                int tileWidth = tileType == -1 || data == null ? 1 : data.Width;
                int tileHeight = tileType == -1 || data == null ? 1 : data.Height;
                byte oldSlope = (byte)Main.tile[x, y].Slope;
                bool oldHalfBrick = Main.tile[x, y].IsHalfBlock;

                if (tileType != GenerateType.Ignore)
                {
                    ClearTile(x, y, width, height);

                    if (active)
                    {
                        if (tileWidth <= 1 && tileHeight <= 1 && !Main.tileFrameImportant[tileType])        //直接放置没有帧图的物块
                        {
                            Main.tile[x, y].ResetToType((ushort)tileType);
                            WorldGen.SquareTileFrame(x, y);
                        }
                        else
                        {
                            WorldGen.destroyObject = true;
                            for (int x1 = 0; x1 < tileWidth; x1++)
                                for (int y1 = 0; y1 < tileHeight; y1++)
                                {
                                    Point p3 = Helper.FindTopLeftPoint(x + x1, y + y1);
                                    Main.tile[p3].Clear(TileDataType.Tile);
                                    Main.tile[p3].Clear(TileDataType.Slope);
                                    if (Main.tileContainer[Main.tile[p3].TileType])//这总不能有问题了吧
                                        KillChestAndItems(x, y);

                                    WorldGen.KillTile(x + x1, y + y1, false, false, true);

                                    Main.tile[x, y].Clear(TileDataType.Tile);
                                    Main.tile[x, y].Clear(TileDataType.Slope);
                                }

                            WorldGen.destroyObject = false;

                            int genX = x;
                            int genY = tileType == TileID.ClosedDoor ? y : y + height;
                            WorldGen.PlaceTile(genX, genY, tileType, true, true, -1, tileStyle);    //放置有帧图的物块
                            for (int x1 = 0; x1 < tileWidth; x1++)
                                for (int y1 = 0; y1 < tileHeight; y1++)
                                    WorldGen.SquareTileFrame(x + x1, y + y1);
                        }
                    }
                }

                //if (sync && Main.netMode != NetmodeID.SinglePlayer)
                //{
                //    int sizeWidth = tileWidth + Math.Max(0, width - 1);
                //    int sizeHeight = tileHeight + Math.Max(0, height - 1);
                //    int size = sizeWidth > sizeHeight ? sizeWidth : sizeHeight;
                //    NetMessage.SendTileSquare(-1, x + (int)(size * 0.5f), y + (int)(size * 0.5f), size + 1);        //同步 感觉我这只在世界生成是调用的话这个就没太大必要了
                //}
            }
            catch (Exception e)
            {
                DEBUGHelper.LogFancy("Coralite:TILEGEN ERROR:", e);
            }
        }

        /// <summary>
        /// 清除指定位置的物块，会先找到左上角然后从左上角开始清除所有的物块
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void ClearTile(int x, int y, int width, int height)
        {
            WorldGen.destroyObject = true;
            if (width > 1 || height > 1)
            {
                int xs = x;
                int ys = y;
                Point newPos = Helper.FindTopLeftPoint(xs, ys);    //找到左上角

                for (int x1 = 0; x1 < width; x1++)      //把原有物块清了
                    for (int y1 = 0; y1 < height; y1++)
                    {
                        int x2 = newPos.X + x1;
                        int y2 = newPos.Y + y1;
                        if (x1 == 0 && y1 == 0 && Main.tileContainer[Main.tile[x2, y2].TileType])
                            KillChestAndItems(x2, y2);
                        Main.tile[x, y].Clear(TileDataType.Tile);
                        Main.tile[x, y].Clear(TileDataType.Slope);

                        //在考虑这个东西是否有保留的必要
                        WorldGen.KillTile(x, y, false, false, true);
                        GenerateLiquid(x2, y2, 0, true, 0, false);
                    }

                for (int x1 = 0; x1 < width; x1++)      //帧图
                    for (int y1 = 0; y1 < height; y1++)
                    {
                        int x2 = newPos.X + x1;
                        int y2 = newPos.Y + y1;
                        WorldGen.SquareTileFrame(x2, y2);
                        WorldGen.SquareWallFrame(x2, y2);
                    }
            }
            else
            {
                WorldGen.KillTile(x, y, false, false, true);
                Main.tile[x, y].Clear(TileDataType.Tile);
                Main.tile[x, y].Clear(TileDataType.Slope);
            }

            WorldGen.destroyObject = false;
        }

        public static void Texture2WallGenerate(int x, int y, int wall)
        {
            try
            {
                if (!WorldGen.InWorld(x, y))
                    return;

                if (wall != GenerateType.Ignore)
                {
                    if (wall == GenerateType.Clear)
                    {
                        wall = 0;
                        Main.tile[x, y].Clear(TileDataType.Wall);//WorldGen.KillWall(x, y);
                        return;
                    }

                    Main.tile[x, y].Clear(TileDataType.Wall);
                    WorldGen.PlaceWall(x, y, wall, true);
                }
            }
            catch (Exception e)
            {
                DEBUGHelper.LogFancy("Coralite:TILEGEN ERROR:", e);
            }
        }

        public static void ClearLiuid(int x, int y, int width, int height)
        {
            for (int x1 = 0; x1 < width; x1++)
                for (int y1 = 0; y1 < height; y1++)
                {
                    int current_x = x + x1;
                    int current_y = y + y1;

                    Framing.GetTileSafely(current_x, current_y).Clear(TileDataType.Liquid);
                }
        }


        /// <summary>
        /// 向箱子内加入一个物品，会按顺序遍历箱子之后添加在最后一个
        /// </summary>
        /// <param name="chest"></param>
        /// <param name="itemtype"></param>
        public static void AddItem(this Chest chest, int itemtype)
        {
            foreach (var item in chest.item)
                if (item.IsAir)
                {
                    item.SetDefaults(itemtype);
                    return;
                }
        }

        /// <summary>
        /// 向箱子内加入一个物品，会按顺序遍历箱子之后添加在最后一个
        /// </summary>
        /// <param name="chest"></param>
        /// <param name="itemtype"></param>
        public static void AddItem<T>(this Chest chest) where T : ModItem
        {
            foreach (var item in chest.item)
                if (item.IsAir)
                {
                    item.SetDefaults(ModContent.ItemType<T>());
                    return;
                }
        }

        /// <summary>
        /// 向上查找物块，检测有多少格没有物块
        /// </summary>
        /// <param name="baseP"></param>
        /// <param name="maxCheck"></param>
        /// <returns></returns>
        public static int CheckUpAreaEmpty(Point baseP, int maxCheck = 10)
        {
            int check = 0;
            for (int m = 0; m < maxCheck; m++)
            {
                if (Main.tile[baseP.X, baseP.Y - m].HasTile)
                    break;

                check++;
            }

            return check;
        }

        public static int CheckBottomAreaEmpty(Point baseP, int maxCheck = 10)
        {
            int check = 0;
            for (int m = 0; m < maxCheck; m++)
            {
                if (Main.tile[baseP.X, baseP.Y + m].HasTile)
                    break;

                check++;
            }

            return check;
        }
    }
}
