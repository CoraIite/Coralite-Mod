using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;

namespace Coralite.Content.WorldGeneration
{
    public static class WorldGenHelper
    {
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
    }
}
