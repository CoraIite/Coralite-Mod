using Coralite.Content.Bosses.Rediancie;
using Coralite.Content.Items.RedJades;
using Terraria.ID;
using Terraria.ModLoader;
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

        public override void SetStaticDefaults()
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
            ProjectileExplosible[ModContent.ProjectileType<Rediancie_BigBoom>()] = true;
            ProjectileExplosible[ModContent.ProjectileType<Rediancie_Explosion>()] = true;
            ProjectileExplosible[ModContent.ProjectileType<RedJadeBigBoom>()] = true;
            ProjectileExplosible[ModContent.ProjectileType<RedJadeBoom>()] = true;

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

        }

        private void InitAll()
        {
            ProjectileExplosible = new bool[ProjectileLoader.ProjectileCount];
            for (int i = 0; i < ProjectileExplosible.Length; i++)
                ProjectileExplosible[i] = false;

            TileSticky = new bool[TileLoader.TileCount];
            for (int i = 0; i < TileSticky.Length; i++)
                TileSticky[i] = false;

            WallShadowCastle = new bool[WallLoader.WallCount];
            for (int i = 0; i < WallShadowCastle.Length; i++)
                WallShadowCastle[i] = false;

            TileShadowCastle = new bool[TileLoader.TileCount];
            for (int i = 0; i < TileShadowCastle.Length; i++)
                TileShadowCastle[i] = false;
        }
    }
}
