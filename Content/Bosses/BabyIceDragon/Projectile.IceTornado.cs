using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    /// <summary>
    /// 请使用Projectile.ai[0]以控制存活时间
    /// </summary>
    public class IceTornado : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        //public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1000;
            Projectile.netImportant = true;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            //if (Projectile.ai[1] == 0)
            //{
            //    Projectile.timeLeft = (int)Projectile.ai[0];
            //    Projectile.ai[1] = 1;
            //}
        }

        public override bool PreDraw(ref Color lightColor) => false;

    }
}