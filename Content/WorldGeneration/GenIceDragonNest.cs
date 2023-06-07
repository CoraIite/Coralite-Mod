using Terraria.ID;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Coralite.Core;
using Coralite.Content.WorldGeneration.Generators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        /// <summary>
        /// 冰龙巢穴中心点
        /// </summary>
        public static Point NestCenter;

        public async void GenIceDragonNest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "制作冰龙巢穴";
            //随机选择雪原上的某个地方
            int nestCenter_x = GenVars.snowOriginLeft+WorldGen.genRand.Next(GenVars.snowOriginRight - GenVars.snowOriginLeft);
            int nestCenter_y = 10;

            for (; nestCenter_y < Main.worldSurface; nestCenter_y++)
            {
                Tile tile = Framing.GetTileSafely(nestCenter_x, nestCenter_y);
                if (tile.HasTile)
                    break;
            }

            nestCenter_y += 5;

            Texture2D nestTex = ModContent.Request<Texture2D>(AssetDirectory.IceNest + "IceNest", AssetRequestMode.ImmediateLoad).Value;
            Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.IceNest + "IceNestClear", AssetRequestMode.ImmediateLoad).Value;
            
            int genOrigin_x = nestCenter_x - clearTex.Width / 2;
            int genOrigin_y = nestCenter_y - clearTex.Height / 2;

            NestCenter = new Point(genOrigin_x + 52, genOrigin_y + 20);

            Dictionary<Color, int> clearDic = new Dictionary<Color, int>()
            {
                [Color.White] = -2,
                [Color.Black] = -1
            };
            Dictionary<Color, int> nestDic = new Dictionary<Color, int>()
            {
                [new Color(95, 205, 228)] = TileID.IceBlock,
                [new Color(215, 123, 186)] = TileID.IceBrick,
                [new Color(99, 155, 255)] = TileID.SnowBlock,
                [new Color(63, 63, 116)] = TileID.BreakableIce,
                [Color.Black] = -1
            };

            await Task.Run(() =>
            {
                Main.QueueMainThreadAction(() =>
                {
                    //清理范围
                    Texture2TileGenerator clearGenerator = Texture2TileGeneratorDatas.GetTexGenerator(clearTex, clearDic);
                    clearGenerator.Generate(genOrigin_x, genOrigin_y, true);

                    //生成主体地形
                    Texture2TileGenerator nestGenerator = Texture2TileGeneratorDatas.GetTexGenerator(nestTex, nestDic);
                    nestGenerator.Generate(genOrigin_x, genOrigin_y, true);
                });
            });

            //生成装饰物
            WorldGenHelper.PlaceOnTopDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.Stalactite, 10, 0);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SmallPiles, 10, 5);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SmallPiles, 10, 6);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SmallPiles, 3, 24);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.LargePiles, 3, 8);

            //添加斜坡
            WorldGenHelper.SmoothSlope(genOrigin_x,genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.IceBlock, 5);
            WorldGenHelper.SmoothSlope(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SnowBlock, 5);
        }
    }
}
