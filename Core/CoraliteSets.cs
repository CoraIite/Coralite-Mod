using Coralite.Content.Bosses.Rediancie;
using Coralite.Content.Items.RedJades;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core
{
    public class CoraliteSets : ModSystem
    {
        /// <summary>
        /// 爆破弹幕
        /// </summary>
        public static bool[] ProjectileExplosible;

        /// <summary>
        /// 粘性物块
        /// </summary>
        public static bool[] TileSticky;

        /// <summary>
        /// 会被判定为影之城环境
        /// </summary>
        public static bool[] WallShadowCastle;
        /// <summary>
        /// 会被判定为影子物块
        /// </summary>
        public static bool[] TileShadowCastle;
        /// <summary>
        /// 特殊绘制
        /// </summary>
        public static bool[] TileSpecialDraw;

        public static List<int> SpecialDraw = new List<int>();

        public override void PostSetupContent()
        {
            InitAll();

            #region 爆炸弹幕
            ProjectileExplosible[ProjectileID.Bomb] = true;
            ProjectileExplosible[ProjectileID.BouncyBomb] = true;
            ProjectileExplosible[ProjectileID.StickyBomb] = true;
            ProjectileExplosible[ProjectileID.ScarabBomb] = true;
            ProjectileExplosible[ProjectileID.Dynamite] = true;
            ProjectileExplosible[ProjectileID.BouncyDynamite] = true;
            ProjectileExplosible[ProjectileID.StickyDynamite] = true;
            ProjectileExplosible[ProjectileID.Grenade] = true;
            ProjectileExplosible[ProjectileID.BouncyGrenade] = true;
            ProjectileExplosible[ProjectileID.StickyGrenade] = true;
            ProjectileExplosible[ProjectileID.Beenade] = true;

            //由于不清楚加载顺序的问题....不知道这个到底该怎么写比较好
            ProjectileExplosible[ProjectileType<Rediancie_BigBoom>()] = true;
            ProjectileExplosible[ProjectileType<Rediancie_Explosion>()] = true;
            ProjectileExplosible[ProjectileType<RedJadeBigBoom>()] = true;
            ProjectileExplosible[ProjectileType<RedJadeBoom>()] = true;

            //后面的暂时懒得写了

            #endregion

            #region 粘性物块
            TileSticky[TileID.Cobweb] = true;
            TileSticky[TileType<Content.Bosses.VanillaReinforce.SlimeEmperor.StickyGelTile>()] = true;
            #endregion

            #region 影之城墙壁
            WallShadowCastle[WallType<Content.Tiles.ShadowCastle.BlackHoleWall>()] = true;
            WallShadowCastle[WallType<Content.Tiles.ShadowCastle.ShadowBrickWall>()] = true;
            #endregion

            #region 影之城物块
            TileShadowCastle[TileType<Content.Tiles.ShadowCastle.ShadowBrickTile>()] = true;
            TileShadowCastle[TileType<Content.Tiles.ShadowCastle.ShadowImaginaryBrickTile>()] = true;
            TileShadowCastle[TileType<Content.Tiles.ShadowCastle.ShadowQuadrelTile>()] = true;
            TileShadowCastle[TileType<Content.Tiles.ShadowCastle.MercuryPlatformTile>()] = true;
            #endregion

            foreach (var type in SpecialDraw)
            {
                if (TileSpecialDraw.IndexInRange(type))
                    TileSpecialDraw[type] = true;
            }

            TileID.Sets.CanBeClearedDuringOreRunner[TileID.Chain] = false;
        }

        private void InitAll()
        {
            ProjectileExplosible = new bool[ProjectileLoader.ProjectileCount];
            Array.Fill(ProjectileExplosible, false);

            TileSticky = new bool[TileLoader.TileCount];
            Array.Fill(TileSticky, false);

            WallShadowCastle = new bool[WallLoader.WallCount];
            Array.Fill(WallShadowCastle, false);

            TileShadowCastle = new bool[TileLoader.TileCount];
            Array.Fill(TileShadowCastle, false);

            TileSpecialDraw = new bool[TileLoader.TileCount];
            Array.Fill(TileSpecialDraw, false);
        }
    }
}
