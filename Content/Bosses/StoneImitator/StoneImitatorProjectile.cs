using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.StoneImitator
{

    ///// <summary>
    ///// SIP是“Stone Imitator Projectile”的缩写
    ///// </summary>
    //public class SIP_Smash : ModProjectile
    //{

    //}

    /// <summary>
    /// SIP是“Stone Imitator Projectile”的缩写
    /// </summary>
    public class SIP_StoneBall : ModProjectile
    {
        public override string Texture => AssetDirectory.StoneImitator+"StoneBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("石球");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1200;

            Projectile.aiStyle = -1;

        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.Stone, Vector2.UnitX.RotatedByRandom(3.14f) * -1, 0, default, 3f);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.Stone, Vector2.One.RotatedByRandom(0.5f), 0, default, 1);
                }
            }
            return true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f;
        }
    }
}
