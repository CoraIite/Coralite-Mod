using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceBurst : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Length => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1920;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.coldDamage = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<IceBurstParticle>(), Scale: 1.5f);
            float rotation = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < 3; i++)
            {
                Particle particle = Particle.NewParticleDirect(Projectile.Center + rotation.ToRotationVector2() * 64, Vector2.Zero, CoraliteContent.ParticleType<Strike_Reverse>(), Scale: 1f);
                particle.rotation = rotation + 2.2f;
                rotation += 2.094f;   //2/3 Pi
            }

            for (int j = 0; j < 40; j++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(34, 69), ModContent.DustType<CrushedIceDust>(),
                    -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f)) * Main.rand.Next(2, 7), Scale: Main.rand.NextFloat(1f, 1.4f));
            }
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 10 && Projectile.timeLeft % 4 == 0)
            {
                if (Projectile.timeLeft > 15)
                {
                    float rotation = Main.rand.NextFloat(6.282f);
                    for (int i = 0; i < 3; i++)
                    {
                        Particle particle2 = Particle.NewParticleDirect(Projectile.Center + (rotation + 0.2f).ToRotationVector2() * 96, Vector2.Zero, CoraliteContent.ParticleType<Strike_Reverse>(), Scale: 1.4f);
                        particle2.rotation = rotation + 2.2f;
                        Particle particle = Particle.NewParticleDirect(Projectile.Center + rotation.ToRotationVector2() * 96, Vector2.Zero, CoraliteContent.ParticleType<Strike>(), Coralite.Instance.IcicleCyan, 1.8f);
                        particle.rotation = rotation + 2.2f;
                        rotation += 2.094f;   //2/3 Pi
                    }
                }
                for (int i = 0; i < 3; i++)
                    Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo>(), Color.White, 0.25f);
            }

            Length += 96f;
            Projectile.netUpdate = true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return Vector2.Distance(Projectile.Center, target.Center) < Length;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.friendly)
                return Vector2.Distance(Projectile.Center, target.Center) < Length;

            return null;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
