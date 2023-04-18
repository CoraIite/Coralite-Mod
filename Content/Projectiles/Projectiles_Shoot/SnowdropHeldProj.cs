using System;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

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

        public override void Initialize()
        {
            base.Initialize();
            float rotation = TargetRot + (Owner.direction > 0 ? 0 : 3.141f);
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 center = Projectile.Center + dir * 32;
            for (int i = 0; i < 16; i++)
            {
                Dust dust = Dust.NewDustPerfect(center+Main.rand.NextVector2Circular(8, 8), DustID.Snow, dir.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(4f, 8f), Scale: Main.rand.NextFloat(0.8f, 1.2f));
                dust.noGravity = true;
            }
        }
    }
}