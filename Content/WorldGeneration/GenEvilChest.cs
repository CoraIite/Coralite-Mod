using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
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

                if (WorldGen.crimson)
                {

                }
                else
                {
                    int[] indexs = GetRandomArray(itemCount, 0, heartCount);
                    for (int j = 0; j < indexs.Length; j++)
                    {
                        Point position = heartPos[indexs[j]]+new Point(WorldGen.genRand.Next(-25,25), WorldGen.genRand.Next(-45, -25));
                        if (!WorldGen.InWorld(position.X, position.Y))
                            continue;
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
