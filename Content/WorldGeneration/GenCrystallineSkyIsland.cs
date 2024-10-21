using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        /// <summary>
        /// 蕴魔空岛的范围
        /// </summary>
        public static Rectangle CrystallineSkyIslandArea { get; set; }
        /// <summary>
        /// 是否放置光明之魂
        /// </summary>
        public static bool PlaceLightSoul {  get; set; }
        /// <summary>
        /// 是否放置暗影之魂
        /// </summary>
        public static bool PlaceNightSoul {  get; set; }
        /// <summary>
        /// 是否有权限进入蕴魔空岛
        /// </summary>
        public static bool HasPermission {  get; set; }

        public void GenCrystallineSkyIsland(GenerationProgress progress, GameConfiguration configuration)
        {
            //生成地表结构


            GenMainSkyIsland();
        }

        public void GenGroundLock()
        {

        }

        private void GenMainSkyIsland()
        {
            ushort skarn = (ushort)ModContent.TileType<SkarnTile>();
            ushort crystallineSkarn = (ushort)ModContent.TileType<CrystallineSkarnTile>();
            ushort smoothSkarn = (ushort)ModContent.TileType<SmoothSkarnTile>();
            ushort skarnBrick = (ushort)ModContent.TileType<SkarnBrickTile>();

            ushort chalcedony = (ushort)ModContent.TileType<ChalcedonyTile>();
            ushort leafChalcedony = (ushort)ModContent.TileType<LeafChalcedonyTile>();

            ushort crystallineBrick = (ushort)ModContent.TileType<CrystallineBrickTile>();

            ushort skarnBrickPlatform = (ushort)ModContent.TileType<SkarnBrickPlatformTile>();

            Texture2D shrineTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CrystallineMainIsland" + 0.ToString(), AssetRequestMode.ImmediateLoad).Value;
            Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CrystallineMainIslandClear" + 0.ToString(), AssetRequestMode.ImmediateLoad).Value;
            Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.Shrines + "CrystallineMainIslandWall" + 0.ToString(), AssetRequestMode.ImmediateLoad).Value;

            int genOrigin_x = Main.maxTilesX/2 - (clearTex.Width / 2);
            int genOrigin_y = 200 - (clearTex.Height / 2);

            Dictionary<Color, int> clearDic = new()
            {
                [Color.White] = -2,
                [Color.Black] = -1
            };
            Dictionary<Color, int> mainDic = new()
            {
                [new Color(51, 76, 117)] = skarn,//334c75
                [new Color(165, 58, 255)] = crystallineSkarn,//a53aff
                [new Color(141, 171, 178)] = smoothSkarn,//8dabb2
                [new Color(184, 230, 207)] = skarnBrick,//b8e6cf

                [new Color(255, 239, 219)] = chalcedony,//ffefdb
                [new Color(170, 228, 143)] = leafChalcedony,//aae48f

                [new Color(241, 130, 255)] = crystallineBrick,//f182ff
                [new Color(90, 100, 80)] = TileID.Chain,//5a6450
                [Color.Black] = -1
            };
            Dictionary<Color, int> wallDic = new()
            {
                [new Color(85, 183, 206)] = ModContent.WallType<Walls.Magike.SmoothSkarnWallUnsafe>(),//55b7ce
                [new Color(188, 171, 150)] = ModContent.WallType<Walls.Magike.ChalcedonyWallUnsafe>(),//bcab96
                [Color.Black] = -1
            };

            GenShrine(clearTex, shrineTex, wallTex, clearDic, mainDic, wallDic, genOrigin_x, genOrigin_y);


            WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
                , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<CrystallineStalactite>(), () => 1, 3, 0);
            WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
                , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<CrystallineStalactite2x2>(), () => 1, 3, 0);

            WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
                , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<SkarnRubbles1x1>(), () => 1, 3, 0);
            WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
                , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<SkarnRubbles2x1>(), () => 1, 3, 0);
            WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
                , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<SkarnRubbles2x2>(), () => 1, 3, 0);
            WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
                , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<SkarnRubbles3x2>(), () => 1, 3, 0);
            WorldGenHelper.PlaceOnGroundDecorations_NoCheck(genOrigin_x, genOrigin_y
                , 0, 0, shrineTex.Width, shrineTex.Height, (ushort)ModContent.TileType<SkarnRubbles4x2>(), () => 1, 3, 0);
        }
    }
}
