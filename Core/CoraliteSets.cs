using Coralite.Content.Bosses.Rediancie;
using Coralite.Content.Items.RedJades;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core
{
    public class CoraliteSets:ModSystem
    {
        public static bool[] ProjectileExplosible;

        public static bool[] TileSticky;

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
            TileSticky[ModContent.TileType<Content.Bosses.VanillaReinforce.SlimeEmperor.StickyGelTile>()] = true;
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
        }
    }
}
