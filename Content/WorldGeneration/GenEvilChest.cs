using Coralite.Content.Items.Corruption;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Reflection;
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
        private static Point[] CorruptionLeft = new Point[3]
        {
            new Point(4,10),
            new Point(9,1),
            new Point(9, 1),
        };
        private static Point[] CorruptionRight = new Point[3]
        {
            new Point(13,9),
            new Point(9, 1),
            new Point(9,1),
        };

        private static Point[] CorruptionTorch = new Point[3]
        {
            new Point(5,8),
            new Point(9, 1),
            new Point(9, 1),
        };

        public void GenEvilChest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "正在生成邪恶箱";

            try
            {
                Type type = typeof(WorldGen);
                int heartCount = (int)type.GetField("heartCount", BindingFlags.Static | BindingFlags.NonPublic).GetValue(type);
                Point[] heartPos = (Point[])type.GetField("heartPos", BindingFlags.Static | BindingFlags.NonPublic).GetValue(type);

                int itemCount;

                if (WorldGen.crimson)
                    itemCount = 3;
                else
                    itemCount = 2;

                if (itemCount>heartCount)
                {
                    itemCount = heartCount;
                }

                Dictionary<Color, int> clearDic = new Dictionary<Color, int>()
                {
                    [Color.White] = -2,
                    [Color.Black] = -1
                };

                if (WorldGen.crimson)
                {

                }
                else
                {
                    int[] indexs = GetRandomArray(itemCount, 0, heartCount);

                    Dictionary<Color, int> mainDic = new Dictionary<Color, int>()
                    {
                        [new Color(71, 49, 57)] = TileID.LesionBlock,
                        [new Color(100, 61, 184)] = TileID.EbonstoneBrick,
                        [Color.Black] = -1
                    };
                    Dictionary<Color, int> wallDic = new Dictionary<Color, int>()
                    {
                        [new Color(160, 116, 255)] = WallID.EbonstoneBrick,
                        [new Color(128, 85, 100)] = WallID.Corruption3Echo,
                        [Color.Black] = -1
                    };


                    for (int j = 0; j < indexs.Length; j++)
                    {
                        int whichOne = 0;//WorldGen.genRand.Next(3);
                        Texture2D shrineTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CorruptionChestShrine" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;
                        Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CorruptionChestClear" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;
                        Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CorruptionChestWall" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;

                        Point position = heartPos[indexs[j]]+new Point(WorldGen.genRand.Next(-25,25), WorldGen.genRand.Next(-45, -25));
                        position += new Point(-10, -7);
                        if (!WorldGen.InWorld(position.X, position.Y))
                            continue;

                        Task.Run(async () =>
                        {
                            await GenShrine(clearTex, shrineTex, wallTex, clearDic, mainDic, wallDic, position.X, position.Y);
                        }).Wait();

                        Point createLeftPos = position + CorruptionLeft[whichOne];
                        Point createRightPos = position + CorruptionRight[whichOne];

                        WorldGen.PlaceObject(createLeftPos.X, createLeftPos.Y, TileID.FishingCrate, true, 4);
                        WorldGen.PlaceObject(createRightPos.X, createRightPos.Y, TileID.FishingCrate, true, 4);

                        Point torchPos = position + CorruptionTorch[whichOne];

                        WorldGen.PlaceObject(torchPos.X, torchPos.Y, TileID.Torches, true, 18);
                        Point chestPos = position+ new Point(10, 7);

                        if (WorldGen.AddBuriedChest(chestPos.X, chestPos.Y, ModContent.ItemType<CorruptJavelin>(),
                             notNearOtherChests: false, 0, trySlope: false, (ushort)ModContent.TileType<RottenChestTile>()))
                        {
                            Chest.Lock(chestPos.X, chestPos.Y);
                        }

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
