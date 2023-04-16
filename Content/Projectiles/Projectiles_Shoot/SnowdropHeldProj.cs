using System;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    public class SnowdropHeldProj:BaseGunHeldProj
    {
        public SnowdropHeldProj() : base(1f, 18, -10, AssetDirectory.Weapons_Shoot) { }

        public override float Ease()
        {
            float x = 1.465f * Projectile.timeLeft / MaxTime;
            return x * MathF.Sin(x * x * x) / 1.186f;
        }
    }
}