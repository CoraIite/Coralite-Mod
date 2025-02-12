using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader.IO;
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
        public static bool PlaceLightSoul { get; set; }
        /// <summary>
        /// 是否放置暗影之魂
        /// </summary>
        public static bool PlaceNightSoul { get; set; }
        /// <summary>
        /// 是否有权限进入蕴魔空岛
        /// </summary>
        public static bool HasPermission { get; set; }

        public void GenCrystallineSkyIsland(GenerationProgress progress, GameConfiguration configuration)
        {
            //生成地表结构
            GenGroundLock(out Point altarPoint);

            //GenMainSkyIsland();
        }

        public void GenGroundLock(out Point altarPoint)
        {
            //找到丛林，在地表处选择一个地方
            Point searchOrigin = new Point((int)(Main.LocalPlayer.Center.X / 16), 0);

            Point p = new Point(searchOrigin.X + WorldGen.genRand.Next(-30, 30), (int)(Main.worldSurface * 0.4f));

            for (int i = 0; i < 1000; i++)
            {
                Tile t = Main.tile[p.X, p.Y];
                if (t.HasTile && Main.tileSolid[t.TileType])
                    break;

                p.Y++;
            }

            altarPoint = p;

            //p就是中心点，放置主祭坛
            //altarPoint = p;

            //ushort basalt = (ushort)ModContent.TileType<BasaltTile>();
            //ushort beam = (ushort)ModContent.TileType<BasaltBeamTile>();

            //for (int j = -1; j < 2; j += 2)//防止底部空
            //    if (!Main.tile[p.X + j, p.Y + 2].HasTile)
            //    {
            //        WorldGen.KillTile(p.X - 1, p.Y + 2);
            //        Main.tile[p.X - 1, p.Y + 2].ResetToType(basalt);
            //    }

            //int h = WorldGen.genRand.Next(3, 5);

            //for (int i = -1; i < h; i++)//放置两条玄武岩柱子
            //{
            //    Main.tile[p.X - 1, p.Y - i].ResetToType(beam);
            //    Main.tile[p.X + 1, p.Y - i].ResetToType(beam);
            //}

            ////放置一条玄武岩
            //for (int i = -2; i < 3; i++)
            //    Main.tile[p.X + i, p.Y - h + 1].ResetToType(basalt);

            //Point topP = p + new Point(-1, -h - 2);
            //for (int i = 0; i < 3; i++)
            //    for (int j = 0; j < 3; j++)
            //    {
            //        WorldGen.KillTile(topP.X + i, topP.Y + j);
            //    }

            ////放置主要祭坛
            //WorldGen.PlaceTile(topP.X + 1, topP.Y + 2, ModContent.TileType<PremissionAltarTile>(), true);
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

            int genOrigin_x = Main.maxTilesX / 2 - (clearTex.Width / 2);
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

        public void SaveSkyIsland(TagCompound tag)
        {
            if (PlaceLightSoul)
                tag.Add(nameof(PlaceLightSoul), true);
            if (PlaceNightSoul)
                tag.Add(nameof(PlaceNightSoul), true);
            if (HasPermission)
                tag.Add(nameof(HasPermission), true);
        }

        public void LoadSkyIsland(TagCompound tag)
        {
            PlaceLightSoul = tag.ContainsKey(nameof(PlaceLightSoul));
            PlaceNightSoul = tag.ContainsKey(nameof(PlaceLightSoul));
            HasPermission = tag.ContainsKey(nameof(PlaceLightSoul));
        }
    }
}
