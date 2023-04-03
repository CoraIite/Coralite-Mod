using Terraria.ID;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        /// <summary>
        /// 冰龙巢穴中心点
        /// </summary>
        public static Point NestCenter;

        public void GenIceDragonNest(GenerationProgress progress, GameConfiguration configuration)
        {
            //#region TODO: 1.4.4之后得改
            //int dungeonSide = 0;

            //dungeonSide = ((WorldGen.genRand.Next(2) != 0) ? 1 : (-1));

            //int num938 = WorldGen.genRand.Next(Main.maxTilesX);
            //if (WorldGen.drunkWorldGen)
            //    dungeonSide *= -1;

            //if (dungeonSide == 1)
            //{
            //    while ((float)num938 < (float)Main.maxTilesX * 0.6f || (float)num938 > (float)Main.maxTilesX * 0.75f)
            //    {
            //        num938 = WorldGen.genRand.Next(Main.maxTilesX);
            //    }
            //}
            //else
            //{
            //    while ((float)num938 < (float)Main.maxTilesX * 0.25f || (float)num938 > (float)Main.maxTilesX * 0.4f)
            //    {
            //        num938 = WorldGen.genRand.Next(Main.maxTilesX);
            //    }
            //}


            //int num939 = WorldGen.genRand.Next(50, 90);
            //float num940 = Main.maxTilesX / 4200;
            //num939 += (int)((float)WorldGen.genRand.Next(20, 40) * num940);
            //num939 += (int)((float)WorldGen.genRand.Next(20, 40) * num940);
            //int num941 = num938 - num939;
            //num939 = WorldGen.genRand.Next(50, 90);
            //num939 += (int)((float)WorldGen.genRand.Next(20, 40) * num940);
            //num939 += (int)((float)WorldGen.genRand.Next(20, 40) * num940);
            //int num942 = num938 + num939;
            //if (num941 < 0)
            //    num941 = 0;

            //if (num942 > Main.maxTilesX)
            //    num942 = Main.maxTilesX;
            //#endregion

            ///*Terraria.WorldBuilding.GenVars.snowOriginLeft*/
            //int snowBiomeLeft = num941;
            //int snowBiomeRight = num942;

            //int nest_X = WorldGen.genRand.Next(snowBiomeLeft, snowBiomeRight);
            //int nest_Y = 0;

            //while (nest_Y < (int)Main.rockLayer)
            //{
            //    //没有物块，则向下一格
            //    if (!Main.tile[nest_X, nest_Y].HasTile)
            //    {
            //        nest_Y++;
            //        continue;
            //    }

            //    //必须得是冰雪块才行
            //    ushort tileType = Main.tile[nest_X, nest_Y].TileType;
            //    if (tileType == TileID.SnowBlock || tileType == TileID.IceBlock)
            //    {
            //        NestCenter = new Point(nest_X, nest_Y);
            //        break;
            //    }
            //}

            //生成冰龙巢中心位置

            //生成冰龙巢的周围冰刺

        }
    }
}
