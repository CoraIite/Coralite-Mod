using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleThornExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 250;
            Projectile.timeLeft = 12;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Helper.PlayPitched("Icicle/Broken", 0.4f, 0f, Projectile.Center);
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor) => false;

    }
}