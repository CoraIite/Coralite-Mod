using Coralite.Content.Items.Corruption;
using Coralite.Content.Items.Crimson;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        private static Point[] CorruptionLeft = new Point[6]
        {
            new Point(4,10),
            new Point(4,6),
            new Point(7, 9),
            new Point(3, 9),
            new Point(5, 8),
            new Point(6, 11),
        };
        private static Point[] CorruptionRight = new Point[6]
        {
            new Point(13,9),
            new Point(14, 9),
            new Point(11,9),
            new Point(15,9),
            new Point(13,9),
            new Point(12,11),
        };

        private static Point[] CorruptionTorch1 = new Point[6]
        {
            new Point(5,8),
            new Point(16, 5),
            new Point(7, 5),
            new Point(7, 7),
            new Point(7, 5),
            new Point(6, 8),
        };
        private static Point[] CorruptionTorch2 = new Point[6]
        {
            Point.Zero,
            Point.Zero,
            new Point(13, 6),
            new Point(12, 7),
            Point.Zero,
            new Point(13, 8),
        };

        private static Point[] CrimsonLeft = new Point[7]
        {
            new Point(6,9),
            new Point(4,6),
            new Point(4, 6),
            new Point(3, 9),
            new Point(3, 3),
            new Point(2, 11),
            new Point(11, 9),
        };
        private static Point[] CrimsonRight = new Point[7]
        {
            new Point(12,9),
            new Point(14, 6),
            new Point(9,3),
            new Point(15,9),
            new Point(17,6),
            new Point(15,12),
            new Point(15,9),
        };

        private static Point[] CrimsonTorch1 = new Point[7]
        {
            new Point(8,6),
            new Point(7, 4),
            new Point(15, 7),
            new Point(8, 5),
            new Point(4, 7),
            new Point(7, 7),
            new Point(7, 4),
        };
        private static Point[] CrimsonTorch2 = new Point[7]
        {
            new Point(11, 6),
            new Point(12, 4),
            Point.Zero,
            new Point(11, 5),
            Point.Zero,
            new Point(12, 7),
            new Point(14, 5),
        };

        public void GenEvilChest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "正在生成邪恶箱";

            try
            {
                //不知道为什么就是获取不到那两个B东西的值 所以干脆换方法了
                //Type type = typeof(WorldGen);

                //FieldInfo heartCountInfo = type.GetField("heartCount", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
                //object o = heartCountInfo.GetValue(null);
                //int heartCount = (int)o;

                //FieldInfo heartPosInfo = type.GetField("heartPos", BindingFlags.Static | BindingFlags.NonPublic);
                //Point[] heartPos = (Point[])(heartPosInfo.GetValue(null));

                int itemCount = 4;
                int gened = 0;

                if (Main.maxTilesX > 8000)
                {
                    itemCount++;
                }

                if (Main.maxTilesX > 6000)
                {
                    itemCount++;
                }

                //if (itemCount > heartCount)
                //    itemCount = heartCount;

                Dictionary<Color, int> clearDic = new Dictionary<Color, int>()
                {
                    [Color.White] = -2,
                    [Color.Black] = -1
                };

                int[] arr = new int[100];
                for (int i = 0; i < 100; i++)
                    arr[i] = i;

                //随机打乱
                for (int i = 0; i < 100; i++)
                {
                    int index = WorldGen.genRand.Next(100);
                    (arr[index], arr[i]) = (arr[i], arr[index]);
                }

                if (WorldGen.crimson)
                {
                    Dictionary<Color, int> mainDic = new Dictionary<Color, int>()
                    {
                        [new Color(184, 106, 97)] = TileID.CrimstoneBrick,
                        [new Color(236, 74, 79)] = TileID.CrimtaneBrick,
                        [new Color(95, 96, 110)] = TileID.Shadewood,
                        [new Color(219, 15, 6)] = TileID.FleshBlock,
                        [Color.Black] = -1
                    };
                    Dictionary<Color, int> wallDic = new Dictionary<Color, int>()
                    {
                        [new Color(144, 82, 90)] = WallID.CrimstoneBrick,
                        [new Color(64, 38, 27)] = WallID.CrimtaneBrick,
                        [new Color(102, 32, 32)] = WallID.CrimsonUnsafe2,
                        [new Color(159, 150, 114)] = WallID.CrimsonUnsafe3,
                        [new Color(212, 109, 24)] = WallID.CrimsonUnsafe4,
                        [new Color(144, 146, 167)] = WallID.Shadewood,
                        [new Color(255, 0, 255)] = WallID.ArcaneRunes,

                        [Color.Black] = -1
                    };

                    for (int i = 0; i < 100; i++)
                    {
                        //每隔一段选取一个点并检测是否有邪恶地形
                        Point position = new Point(100, (int)GenVars.worldSurface)
                            + new Point(WorldGen.genRand.Next(-25, 25), WorldGen.genRand.Next(-100, 100))
                            + new Point(arr[i] * (Main.maxTilesX - 200) / 100, 175);

                        Dictionary<ushort, int> tileDictionary = new Dictionary<ushort, int>();
                        WorldUtils.Gen(
                            new Point(position.X - 25, position.Y - 25),
                            new Shapes.Rectangle(50, 50),
                            new Actions.TileScanner(TileID.Crimstone, TileID.Crimsand, TileID.CrimsonGrass).Output(tileDictionary));

                        if (tileDictionary[TileID.Crimstone] + tileDictionary[TileID.Crimsand] + tileDictionary[TileID.CrimsonGrass] < 750)
                            continue; //如果不是，则返回false，这将导致调用方法尝试一个不同的origin。

                        int whichOne = WorldGen.genRand.Next(7);
                        Texture2D shrineTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CrimsonChestShrine" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;
                        Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CrimsonChestClear" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;
                        Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CrimsonChestWall" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;

                        position += new Point(-10, -7);
                        if (!WorldGen.InWorld(position.X, position.Y))
                            continue;

                        Task.Run(async () =>
                        {
                            await GenShrine(clearTex, shrineTex, wallTex, clearDic, mainDic, wallDic, position.X, position.Y);
                        }).Wait();

                        //放置板条箱
                        Point createLeftPos = position + CrimsonLeft[whichOne];
                        Point createRightPos = position + CrimsonRight[whichOne];

                        WorldGen.PlaceObject(createLeftPos.X, createLeftPos.Y, TileID.FishingCrate, true, 4);
                        WorldGen.PlaceObject(createRightPos.X, createRightPos.Y, TileID.FishingCrate, true, 4);

                        //放火把
                        Point torchPos = position + CrimsonTorch1[whichOne];
                        WorldGen.PlaceObject(torchPos.X, torchPos.Y, TileID.Torches, true, 19);

                        if (CrimsonTorch2[whichOne] != Point.Zero)
                        {
                            torchPos = position + CrimsonTorch2[whichOne];
                            WorldGen.PlaceObject(torchPos.X, torchPos.Y, TileID.Torches, true, 19);
                        }

                        //放置箱子
                        Point chestPos = position + new Point(10, 7);

                        int itemType;
                        if (gened == 0)
                            itemType = ModContent.ItemType<BloodyHook>();
                        else if (gened == 1)
                            itemType = ModContent.ItemType<VertebraeBlade>();
                        else if (gened == 2)
                            itemType = ModContent.ItemType<BloodAmulet>();
                        else
                            itemType = WorldGen.genRand.NextFromList(
                                ModContent.ItemType<BloodyHook>(),
                                ModContent.ItemType<VertebraeBlade>(),
                                ModContent.ItemType<BloodAmulet>());

                        if (WorldGen.AddBuriedChest(chestPos.X, chestPos.Y, itemType,
                             notNearOtherChests: false, 0, trySlope: false, (ushort)ModContent.TileType<VertebraeChestTile>()))
                        {
                            int index = Chest.FindChest(chestPos.X - 1, chestPos.Y);
                            if (index != -1)
                            {
                                bool a = Chest.Lock(Main.chest[index].x, Main.chest[index].y);
                            }
                        }

                        progress.Set(i / (float)itemCount);
                        gened++;
                        if (gened >= itemCount)
                            break;
                    }
                }
                else
                {
                    //int[] indexs = GetRandomArray(itemCount, 0, heartCount);

                    Dictionary<Color, int> mainDic = new Dictionary<Color, int>()
                    {
                        [new Color(71, 49, 57)] = TileID.LesionBlock,
                        [new Color(100, 61, 184)] = TileID.EbonstoneBrick,
                        [new Color(155, 144, 179)] = TileID.Ebonwood,
                        [new Color(150, 0, 106)] = TileID.Ebonstone,
                        [new Color(160, 29, 203)] = TileID.DemoniteBrick,

                        [Color.Black] = -1
                    };
                    Dictionary<Color, int> wallDic = new Dictionary<Color, int>()
                    {
                        [new Color(160, 116, 255)] = WallID.EbonstoneBrick,
                        [new Color(128, 85, 100)] = WallID.Corruption3Echo,
                        [new Color(211, 189, 224)] = WallID.Ebonwood,
                        [new Color(149, 0, 255)] = WallID.CorruptionUnsafe4,
                        [new Color(255, 0, 255)] = WallID.ArcaneRunes,

                        [Color.Black] = -1
                    };

                    for (int i = 0; i < 100; i++)
                    {
                        //每隔一段选取一个点并检测是否有邪恶地形
                        Point position = new Point(100, (int)GenVars.worldSurface)
                            + new Point(WorldGen.genRand.Next(-25, 25), WorldGen.genRand.Next(-60, 60))
                            + new Point(arr[i] * (Main.maxTilesX - 200) / 100, 125);

                        Dictionary<ushort, int> tileDictionary = new Dictionary<ushort, int>();
                        WorldUtils.Gen(
                            new Point(position.X - 25, position.Y - 25),
                            new Shapes.Rectangle(50, 50),
                            new Actions.TileScanner(TileID.Ebonstone, TileID.Ebonsand, TileID.CorruptGrass).Output(tileDictionary));

                        if (tileDictionary[TileID.Ebonstone] + tileDictionary[TileID.Ebonsand] + tileDictionary[TileID.CorruptGrass] < 750)
                            continue; //如果不是，则返回false，这将导致调用方法尝试一个不同的origin。

                        int whichOne = WorldGen.genRand.Next(6);
                        Texture2D shrineTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CorruptionChestShrine" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;
                        Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CorruptionChestClear" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;
                        Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CorruptionChestWall" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;

                        position += new Point(-10, -7);
                        if (!WorldGen.InWorld(position.X, position.Y))
                            continue;

                        Task.Run(async () =>
                        {
                            await GenShrine(clearTex, shrineTex, wallTex, clearDic, mainDic, wallDic, position.X, position.Y);
                        }).Wait();

                        //放置板条箱
                        Point createLeftPos = position + CorruptionLeft[whichOne];
                        Point createRightPos = position + CorruptionRight[whichOne];

                        WorldGen.PlaceObject(createLeftPos.X, createLeftPos.Y, TileID.FishingCrate, true, 3);
                        WorldGen.PlaceObject(createRightPos.X, createRightPos.Y, TileID.FishingCrate, true, 3);

                        //放火把
                        Point torchPos = position + CorruptionTorch1[whichOne];
                        WorldGen.PlaceObject(torchPos.X, torchPos.Y, TileID.Torches, true, 18);

                        if (CorruptionTorch2[whichOne] != Point.Zero)
                        {
                            torchPos = position + CorruptionTorch2[whichOne];
                            WorldGen.PlaceObject(torchPos.X, torchPos.Y, TileID.Torches, true, 18);
                        }

                        //放置箱子
                        Point chestPos = position + new Point(10, 7);
                        if (whichOne == 5)//鲨比特判
                            chestPos = position + new Point(10, 8);

                        int itemType;
                        if (gened == 0)
                            itemType = ModContent.ItemType<CorruptJavelin>();
                        else if (gened == 1)
                            itemType = ModContent.ItemType<CadaverousDragonHead>();
                        else if (gened == 2)
                            itemType = ModContent.ItemType<RottenAmulet>();
                        else
                            itemType = WorldGen.genRand.NextFromList(
                                ModContent.ItemType<CorruptJavelin>(),
                                ModContent.ItemType<CadaverousDragonHead>(),
                                ModContent.ItemType<RottenAmulet>());

                        if (WorldGen.AddBuriedChest(chestPos.X, chestPos.Y,itemType ,
                             notNearOtherChests: false, 0, trySlope: false, (ushort)ModContent.TileType<RottenChestTile>()))
                        {
                            int index = Chest.FindChest(chestPos.X - 1, chestPos.Y);
                            if (index != -1)
                            {
                                bool a = Chest.Lock(Main.chest[index].x, Main.chest[index].y);
                            }
                        }

                        progress.Set(i / (float)itemCount);
                        gened++;
                        if (gened >= itemCount)
                            break;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public int[] GetRandomArray(int number, int min, int max)
        {
            int[] b = new int[number];

            for (int j = 0; j < number; j++)
            {
                int i = WorldGen.genRand.Next(min, max + 1);
                int num = 0;
                for (int k = 0; k < j; k++)
                    if (b[k] == i)
                        num++;

                if (num == 0)
                    b[j] = i;
                else
                    j--;
            }

            return b;
        }
    }
}
