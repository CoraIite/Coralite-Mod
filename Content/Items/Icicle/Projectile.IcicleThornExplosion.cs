using Coralite.Core;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleThornExplosion : BaseHeldProj
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
            Projectile.coldDamage = true;
        }

        public override void Initialize()
        {
            Helper.PlayPitched("Icicle/Broken", 0.4f, 0f, Projectile.Center);
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor) => false;

    }
}