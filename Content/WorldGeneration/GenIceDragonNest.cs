using Coralite.Content.Tiles.Icicle;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        /// <summary>
        /// 冰龙巢穴中心点
        /// </summary>
        public static Point NestCenter;

        public static LocalizedText IceDragonNest { get; set; }

        public void GenIceDragonNest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = IceDragonNest.Value;//"正在制作冰龙巢穴";
            progress.Set(0);
            //随机选择雪原上的某个地方
            int nestCenter_x = ((GenVars.snowOriginRight + GenVars.snowOriginLeft) / 2)
                + (WorldGen.genRand.Next(GenVars.snowOriginRight - GenVars.snowOriginLeft) / 3);
            int nestCenter_y = (int)(Main.worldSurface * 0.4f);

            for (; nestCenter_y < Main.worldSurface; nestCenter_y++)
            {
                Tile tile = Framing.GetTileSafely(nestCenter_x, nestCenter_y);
                if (tile.HasTile && tile.TileType != TileID.Cloud
                    && tile.TileType != TileID.RainCloud && tile.TileType != TileID.Sunplate
                    && tile.TileType != TileID.Containers && tile.TileType != TileID.Dirt
                    && tile.TileType != TileID.Grass)
                    break;
            }

            nestCenter_y += 5;

            int whichOne = WorldGen.genRand.Next(3);
            TextureGenerator generator = new TextureGenerator("IceNest", whichOne, AssetDirectory.IceNest);

            int genOrigin_x = nestCenter_x - (generator.Width / 2);
            int genOrigin_y = nestCenter_y - (generator.Height / 2);

            NestCenter = new Point(genOrigin_x + 52, genOrigin_y + 20);

            Dictionary<Color, int> nestDic = new()
            {
                [new Color(95, 205, 228)] = TileID.IceBlock,
                [new Color(215, 123, 186)] = TileID.IceBrick,
                [new Color(99, 155, 255)] = TileID.SnowBlock,
                [new Color(63, 63, 116)] = TileID.BreakableIce,
                [new Color(65, 36, 255)] = ModContent.TileType<IcicleStoneTile>(),
            };

            generator.GenerateByTopLeft(new Point(genOrigin_x, genOrigin_y), nestDic, null);

            progress.Set(0.75);
            //生成装饰物
            WorldGenHelper.PlaceOnTopDecorations(genOrigin_x, genOrigin_y, 0, 0, generator.Width, generator.Height, TileID.Stalactite, 10, 0);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, generator.Width, generator.Height, TileID.SmallPiles, 10, 5);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, generator.Width, generator.Height, TileID.SmallPiles, 10, 6);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, generator.Width, generator.Height, TileID.SmallPiles, 3, 24);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, generator.Width, generator.Height, TileID.LargePiles, 3, 8);

            //添加斜坡
            WorldGenHelper.SmoothSlope(genOrigin_x, genOrigin_y, 0, 0, generator.Width, generator.Height, TileID.IceBlock, 5);
            WorldGenHelper.SmoothSlope(genOrigin_x, genOrigin_y, 0, 0, generator.Width, generator.Height, TileID.SnowBlock, 5);
        }
    }

    public class IceDragonNestReplacer : ModItem
    {
        public override string Texture => AssetDirectory.MiscItems + "IcicleCoralite";

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
