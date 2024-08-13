using Coralite.Content.Tiles.ShadowCastle;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.WorldGeneration.ShadowCastleRooms
{
    public class BossRoom : ShadowCastleRoom
    {
        public override Point[] DownCorridor => new Point[]
        {
            new(47,128)
        };

        public BossRoom(Point center) : base(center + new Point(0, -64), RoomType.BossRoom)
        {
        }

        public override void PostGenerateSelf()
        {
            GenerateObject();
        }

        public override void CreateSelfCorridor()
        {
            if (childrenRooms == null)
                return;

            foreach (var child in childrenRooms)
            {
                Direction dir = child.parentDirection;
                Point endPoint = new Point(child.roomRect.X, child.roomRect.Y) + child.GetCorridorPoint(dir);
                dir = ReverseDirection(dir);
                Point startPoint = new Point(roomRect.X, roomRect.Y) + GetCorridorPoint(dir);

                int shadowBrick = WorldGen.genRand.Next(3) switch
                {
                    0 => ModContent.TileType<ShadowBrickTile>(),
                    _ => ModContent.TileType<ShadowImaginaryBrickTile>()
                };
                //墙壁
                int shadowWall = WorldGen.genRand.Next(3) switch
                {
                    0 => ModContent.WallType<ShadowBrickWall>(),
                    _ => ModContent.WallType<ShadowBrickWall>(),
                };

                int CorridorHeight = 6;
                int WallWidth = 6;

                switch (dir)
                {
                    default:
                    case Direction.Up:
                    case Direction.Down:
                        {
                            //方向
                            int dir2 = endPoint.Y > startPoint.Y ? 1 : -1;
                            //生成多远
                            int count = Math.Abs(endPoint.Y - startPoint.Y);
                            if (dir == Direction.Up)
                                startPoint += new Point(0, -1);

                            //把中心点挪到最左边
                            startPoint -= new Point(CorridorHeight / 2 + WallWidth, 0);
                            endPoint -= new Point(CorridorHeight / 2 + WallWidth, 0);

                            int offset = 0;
                            int targetOffset = 0;
                            //能保持最大偏移值的概率，会逐渐减小
                            int offsetKeepCount = 20;

                            int placePlatformChance = 50;

                            for (int y = 0; y < count; y++)
                            {
                                //当前的y位置
                                int currentY = startPoint.Y + y * dir2;
                                int baseX = (int)Math.Round(Helper.Lerp(startPoint.X, endPoint.X, y / (float)(count - 1)));

                                if (Math.Abs(offset) > (count - y))
                                {
                                    offset -= Math.Sign(offset);
                                }
                                else
                                {
                                    if (offset == 0)//偏移量为0，随机最大偏移值，如果不为0则进入偏移阶段
                                    {
                                        targetOffset = WorldGen.genRand.NextFromList(0, WorldGen.genRand.Next(-8, 8));
                                        if (Math.Abs(targetOffset) > (count - y) * 2)//防止出现连接不上的情况
                                            targetOffset = Math.Sign(targetOffset) * (count - y);
                                        offset += Math.Sign(targetOffset);
                                    }
                                    else
                                    {
                                        if (offset != targetOffset)//偏移过程中
                                        {
                                            if (targetOffset == 0)
                                                offset -= Math.Sign(offset);
                                            else
                                                offset += Math.Sign(targetOffset);
                                        }
                                        else//偏移完成，持续保持一小段，之后回归
                                        {
                                            bool shouldBack = WorldGen.genRand.NextBool(1, offsetKeepCount);
                                            if (shouldBack)
                                            {
                                                targetOffset = 0;
                                                offsetKeepCount = 20;
                                            }
                                            else if (offsetKeepCount > 2)
                                                offsetKeepCount--;
                                        }
                                    }
                                }

                                baseX += offset;

                                //随机摆放平台
                                bool canPlacePlatform = WorldGen.genRand.NextBool(1, placePlatformChance);
                                bool genedWaterCandle = false;

                                if (canPlacePlatform)
                                {
                                    placePlatformChance = 50;
                                }
                                else if (placePlatformChance > 2)
                                    placePlatformChance--;

                                for (int x = 0; x < CorridorHeight + WallWidth * 2 + 1; x++)
                                {
                                    int currentX = baseX + x;

                                    Tile tile = Main.tile[currentX, currentY];

                                    //放置两边墙壁块
                                    if (x < WallWidth || x > CorridorHeight + WallWidth)
                                    {
                                        Main.tile[currentX, currentY].ClearEverything();
                                        WorldGen.PlaceTile(currentX, currentY, shadowBrick);
                                        //放墙
                                        if (x > 0 && x < CorridorHeight + WallWidth * 2 - 1)
                                            WorldGen.PlaceWall(currentX, currentY, shadowWall);
                                    }
                                    else//清空中间范围
                                    {
                                        Main.tile[currentX, currentY].ClearEverything();
                                        //放墙
                                        WorldGen.PlaceWall(currentX, currentY, shadowWall);
                                        if (canPlacePlatform)
                                        {
                                            //如果头上没有物块那么随机摆放水蜡烛
                                            WorldGen.PlaceTile(currentX, currentY, ModContent.TileType<MercuryPlatformTile>());
                                            if (!genedWaterCandle && WorldGen.genRand.NextBool(10)
                                                && !Main.tile[currentX, currentY - 1].HasTile)
                                            {
                                                genedWaterCandle = true;
                                                WorldGen.PlaceTile(currentX, currentY - 1, TileID.WaterCandle);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.Left:
                    case Direction.Right:
                        break;
                }

            }
        }
    }
}
