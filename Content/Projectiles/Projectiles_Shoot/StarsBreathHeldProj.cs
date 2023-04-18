using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    public class StarsBreathHeldProj:BaseGunHeldProj
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot+Name;
        public StarsBreathHeldProj() : base(0.3f, 22, -6, AssetDirectory.Weapons_Shoot) { }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.8f;

        }

        public override void Initialize()
        {
            base.Initialize();
            float rotation = TargetRot + (Owner.direction > 0 ? 0 : 3.141f);
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 center = Projectile.Center + dir * 54;
            for (int i = 0; i < 3; i++)
            {
                Color color = Main.rand.Next(3) switch
                {
                    0 => new Color(126, 70, 219),
                    1 => new Color(219, 70, 178),
                    _ => Color.White
                };
                Particle.NewParticle(center + Main.rand.NextVector2Circular(6, 6), dir.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(1.2f, 2.3f), CoraliteContent.ParticleType<HorizontalStar>(),color,Main.rand.NextFloat(0.05f,0.15f));
            }
        }
    }
}