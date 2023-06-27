using Coralite.Content.Bosses.Rediancie;
using Coralite.Content.Items.Magike.SpecialLens;
using Coralite.Content.Items.RedJades;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem
{
    public class MagikeGlobalProjectile : GlobalProjectile
    {
        public static bool[] Explosible;

        public override void SetStaticDefaults()
        {
            Explosible = new bool[ProjectileLoader.ProjectileCount];
            for (int i = 0; i < Explosible.Length; i++)
                Explosible[i] = false;

            Explosible[ProjectileID.Bomb] = true;
            Explosible[ProjectileID.BouncyBomb] = true;
            Explosible[ProjectileID.StickyBomb] = true;
            Explosible[ProjectileID.ScarabBomb] = true;
            Explosible[ProjectileID.Dynamite] = true;
            Explosible[ProjectileID.BouncyDynamite] = true;
            Explosible[ProjectileID.StickyDynamite] = true;
            Explosible[ProjectileID.Grenade] = true;
            Explosible[ProjectileID.BouncyGrenade] = true;
            Explosible[ProjectileID.StickyGrenade] = true;
            Explosible[ProjectileID.Beenade] = true;

            //由于不清楚加载顺序的问题....不知道这个到底该怎么写比较好
            Explosible[ModContent.ProjectileType<Rediancie_BigBoom>()] = true;
            Explosible[ModContent.ProjectileType<Rediancie_Explosion>()] = true;
            Explosible[ModContent.ProjectileType<RedJadeBigBoom>()] = true;
            Explosible[ModContent.ProjectileType<RedJadeBoom>()] = true;

            //后面的暂时懒得写了
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            if (Explosible[projectile.type])    //爆炸类弹幕炸到赤玉透镜时
            {
                Point16 position = projectile.Center.ToTileCoordinates16() + Point16.NegativeOne;

                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        if (MagikeHelper.TryGetEntity(position.X + i, position.Y +j,out RedJadeLensEntity redJadeGen))
                        {
                            redJadeGen.Charge(1);
                            goto redJadeGenCharged;
                        }
                    }
            }
        redJadeGenCharged:;
        }
    }
}
