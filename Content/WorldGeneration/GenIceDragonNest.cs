using Coralite.Content.WorldGeneration.Generators;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

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
            progress.Message = "正在制作冰龙巢穴";
            progress.Set(0);
            //随机选择雪原上的某个地方
            int nestCenter_x = (GenVars.snowOriginRight + GenVars.snowOriginLeft) / 2
                + WorldGen.genRand.Next(GenVars.snowOriginRight - GenVars.snowOriginLeft) / 3;
            int nestCenter_y = (int)(Main.worldSurface * 0.4f);

            for (; nestCenter_y < Main.worldSurface; nestCenter_y++)
            {
                Tile tile = Framing.GetTileSafely(nestCenter_x, nestCenter_y);
                if (tile.HasTile && tile.TileType != TileID.Cloud
                    && tile.TileType != TileID.RainCloud && tile.TileType != TileID.Sunplate 
                    && tile.TileType != TileID.Containers&&tile.TileType!=TileID.Dirt
                    &&tile.TileType!=TileID.Grass)
                    break;
            }

            nestCenter_y += 5;

            int whichOne = WorldGen.genRand.Next(3);
            Texture2D nestTex = ModContent.Request<Texture2D>(AssetDirectory.IceNest + "IceNest" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;
            Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.IceNest + "IceNestClear" + whichOne.ToString(), AssetRequestMode.ImmediateLoad).Value;

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

            Task.Run(async () =>
            {
                await GenIceNestWithTex(clearTex, nestTex, clearDic, nestDic, genOrigin_x, genOrigin_y);
            }).Wait();

            progress.Set(0.75);
            //生成装饰物
            WorldGenHelper.PlaceOnTopDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.Stalactite, 10, 0);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SmallPiles, 10, 5);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SmallPiles, 10, 6);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SmallPiles, 3, 24);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.LargePiles, 3, 8);

            //添加斜坡
            WorldGenHelper.SmoothSlope(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.IceBlock, 5);
            WorldGenHelper.SmoothSlope(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SnowBlock, 5);
        }

        private static Task GenIceNestWithTex(Texture2D clearTex, Texture2D nestTex, Dictionary<Color, int> clearDic, Dictionary<Color, int> nestDic, int genOrigin_x, int genOrigin_y)
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
                    Texture2TileGenerator nestGenerator = TextureGeneratorDatas.GetTex2TileGenerator(nestTex, nestDic);
                    nestGenerator.Generate(genOrigin_x, genOrigin_y, true);
                    genned = true;
                });
                placed = true;
            }

            return Task.CompletedTask;
        }
    }

    public class IceDragonNestReplacer : ModItem
    {
        public override string Texture => AssetDirectory.Misc + "WorldGenCoralite";

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (CoraliteWorld.NestCenter != Point.Zero)
                    Main.NewText("冰龙巢穴位于" + CoraliteWorld.NestCenter);
                else
                    Main.NewText("当前世界中不存在冰龙巢穴");
                return true;
            }

            CoraliteWorld.NestCenter = (player.Center / 16).ToPoint();
            Main.NewText("已将冰龙巢穴的位置设置到" + CoraliteWorld.NestCenter);
            return base.CanUseItem(player);
        }
    }
}
