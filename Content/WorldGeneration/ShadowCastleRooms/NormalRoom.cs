using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration.ShadowCastleRooms
{
    public class NormalRoom : ShadowCastleRoom
    {
        //public static Texture2TileGenerator clearGenerator;
        //public static Texture2TileGenerator roomGenerator;
        //public static Texture2WallGenerator wallClearGenerator;
        //public static Texture2WallGenerator wallGenerator;

        public override int RandomTypeCount => 3;

        public override Point[] UpCorridor => new Point[]
        {
            new Point(32,10),
            new Point(32,19),
            new Point(31,8),
        };
        public override Point[] DownCorridor => new Point[]
        {
            new Point(31,59),
            new Point(32,45),
            new Point(32,55),
        };
        public override Point[] LeftCorridor => new Point[]
        {
            new Point(12,28),
            new Point(19,32),
            new Point(8,32),
        };
        public override Point[] RightCorridor => new Point[]
        {
            new Point(53,31),
            new Point(45,32),
            new Point(55,32),
        };

        public NormalRoom(Point center) : base(center, RoomType.Normal)
        {
        }

        public override void InitializeChildrens(List<ShadowCastleRoom> rooms = null)
        {
            int rate = 28;
            if (Main.maxTilesX > 6000)
            {
                rate = 32;
            }
            if (Main.maxTilesY > 8000)
            {
                rate = 36;
            }

            bool canGen = WorldGen.genRand.NextBool(rate - depth, rate);

            if (canGen)
            {
                int howManyRoom = WorldGen.genRand.Next(1, 4);
                for (int i = 0; i < howManyRoom; i++)
                {
                    if (NextDirection(out Direction direction))
                    {
                        Point d = GetDir(direction);
                        NormalRoom childRoom = new NormalRoom(roomRect.Center);
                        int length = (roomRect.Width / 2) + (childRoom.Width / 2) + WorldGen.genRand.Next(-4, childRoom.Width / 4);
                        int height = (roomRect.Height / 2) + (childRoom.Height / 2) + WorldGen.genRand.Next(-4, childRoom.Height / 4);

                        Point newCenter = roomRect.Center + new Point(d.X * length, d.Y * height)
                            + new Point(WorldGen.genRand.Next(-2, 2), WorldGen.genRand.Next(-2, 2));

                        if (!CoraliteWorld.shadowCastleRestraint.Contains(newCenter.X, newCenter.Y))
                            continue;

                        Dictionary<ushort, int> tileDictionary = new Dictionary<ushort, int>();

                        WorldUtils.Gen(
                            new Point(newCenter.X - 32, newCenter.Y - 32),
                            new Shapes.Rectangle(58, 67),
                            new Actions.TileScanner(TileID.Sand, TileID.LihzahrdBrick).Output(tileDictionary));
                        //放置创了海沟
                        if (tileDictionary[TileID.Sand] > 20 * 20)
                            continue;
                        //防止创了神庙
                        if (tileDictionary[TileID.LihzahrdBrick] > 2)
                            continue;
                        //防止露出地面
                        if (newCenter.Y - 32 < Main.worldSurface)
                            continue;

                        childRoom.ResetCenter(newCenter);

                        Append(childRoom, direction, rooms);
                    }
                    else
                        break;
                }
            }
        }

        public override bool NextDirection(out Direction direction)
        {
            List<Direction> directions = new List<Direction>()
            { Direction.Up
            , Direction.Down, Direction.Down, Direction.Down, Direction.Down
            , Direction.Left
            , Direction.Right
            };

            directions.RemoveAll((d) => d == parentDirection);

            if (childrenRooms != null)
                foreach (var child in childrenRooms)
                    directions.RemoveAll(d => d == ReverseDirection(child.parentDirection));

            if (!directions.Any())
            {
                direction = Direction.Up;
                return false;
            }

            direction = WorldGen.genRand.NextFromList(directions.ToArray());
            return true;
        }

        public override void PostGenerateSelf()
        {
            GenerateObject();
        }

        //public override void GenerateSelf()
        //{
        //    if (roomGenerator == null)
        //    {
        //        string rand = "";
        //        if (RandomTypeCount > 1)
        //        {
        //            rand = WorldGen.genRand.Next(RandomTypeCount).ToString();
        //        }

        //        Texture2D roomTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleRooms + RoomGenTex + rand, AssetRequestMode.ImmediateLoad).Value;
        //        Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleRooms + RoomClearTex + rand, AssetRequestMode.ImmediateLoad).Value;
        //        Texture2D wallClearTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleRooms + WallClearTex + rand, AssetRequestMode.ImmediateLoad).Value;
        //        Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleRooms + WallGenTex + rand, AssetRequestMode.ImmediateLoad).Value;

        //        Task.Run(async () =>
        //        {
        //            await GetRoomGenerator(clearTex, roomTex, wallClearTex, wallTex, clearDic, GenDic, clearDic, WallDic);
        //        });
        //    }

        //    GenerateNormalRoom(roomRect.X, roomRect.Y);
        //}

        //public static Task GetRoomGenerator(Texture2D clearTex, Texture2D roomTex, Texture2D wallClearTex, Texture2D wallTex,
        //    Dictionary<Color, int> clearDic, Dictionary<Color, int> roomDic, Dictionary<Color, int> wallClearDic, Dictionary<Color, int> wallDic)
        //{
        //    bool genned = false;
        //    bool placed = false;
        //    while (!genned)
        //    {
        //        if (placed)
        //            continue;

        //        Main.QueueMainThreadAction(() =>
        //        {
        //            //清理范围
        //            clearGenerator = TextureGeneratorDatas.GetTex2TileGenerator(clearTex, clearDic);

        //            //生成主体地形
        //            roomGenerator = TextureGeneratorDatas.GetTex2TileGenerator(roomTex, roomDic);

        //            //清理范围
        //            wallClearGenerator = TextureGeneratorDatas.GetTex2WallGenerator(wallClearTex, wallClearDic);

        //            //生成墙壁
        //            wallGenerator = TextureGeneratorDatas.GetTex2WallGenerator(wallTex, wallDic);

        //            genned = true;
        //        });
        //        placed = true;
        //    }

        //    return Task.CompletedTask;
        //}

        //public static void GenerateNormalRoom(int genOrigin_x, int genOrigin_y)
        //{
        //    clearGenerator.Generate(genOrigin_x, genOrigin_y, true);
        //    //生成主体地形
        //    roomGenerator.Generate(genOrigin_x, genOrigin_y, true);
        //    //清理范围
        //    wallClearGenerator.Generate(genOrigin_x, genOrigin_y, true);
        //    //生成墙壁
        //    wallGenerator.Generate(genOrigin_x, genOrigin_y, true);
        //}
    }
}
