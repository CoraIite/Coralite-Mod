using Coralite.Content.Bosses.Rediancie;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    public class BloodBall : ModProjectile, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Timer => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];

        public ParticleGroup particles;

        public Vector2 shootDir;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 800;

            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void AI()
        {
            particles ??= new ParticleGroup();

            do
            {
                if (Timer == 0)
                {
                    RedShield.Spawn(Projectile, 250);
                }

                if (Timer < 200)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        particles.NewParticle(Projectile.Center, Helper.NextVec2Dir(2, 4),
                            CoraliteContent.ParticleType<Fog>(), Color.DarkRed, Main.rand.NextFloat(1f, 1.5f));
                    }

                    Vector2 targetCenter = Owner.Center + new Vector2(0, -450);
                    if (Vector2.Distance(Projectile.Center, targetCenter) < 100)
                        Projectile.velocity *= 0.95f;
                    else
                        Projectile.velocity = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero) * 9;

                    break;
                }

                if (Timer == 200)
                {
                    Projectile.velocity *= 0;
                    shootDir = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    particles.NewParticle(Projectile.Center + shootDir * 20, shootDir * 8, CoraliteContent.ParticleType<RedArrow>(),
                        Scale: 2f);

                    SoundEngine.PlaySound(CoraliteSoundID.AngryNimbus_NPCDeath33, Projectile.Center);
                }

                if (Timer < 260)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        particles.NewParticle(Projectile.Center, Helper.NextVec2Dir(2, 4),
                            CoraliteContent.ParticleType<Fog>(), Color.DarkRed, Main.rand.NextFloat(1f, 1.5f));
                    }
                    break;
                }

                if (Timer < 660)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        particles.NewParticle(Projectile.Center, Helper.NextVec2Dir(2, 4),
                            CoraliteContent.ParticleType<Fog>(), Color.DarkRed, Main.rand.NextFloat(1f, 1.5f));
                    }

                    if (Timer % 7 == 0)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Main.rand.NextVector2Circular(80, 80),
                            shootDir * 14, ModContent.ProjectileType<BloodRain>(), Helper.ScaleValueForDiffMode(30, 40, 50, 50), 2f);
                    }

                    break;
                }

                if (Timer > 700)
                {
                    Projectile.Kill();
                }

            } while (false);

            particles?.UpdateParticles();

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            particles?.DrawParticles(spriteBatch);
        }
    }

    public class BloodRain : ModProjectile
    {
        public override string Texture => AssetDirectory.Bloodiancie + "RedArrow";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 100;

            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 8; i++)
            {
                Projectile.SpawnTrailDust(DustID.Blood, 0.4f);
            }
        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
                Projectile.SpawnTrailDust(DustID.GemRuby, 0.4f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Projectile.SpawnTrailDust(DustID.Blood, 0.4f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 8, 1, 8, 1, new Vector2(1, 0.5f));

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, mainTex.Size() / 2,
                new Vector2(1, 0.5f), 0, 0);
            return false;
        }
    }

    public class RedArrow : ModParticle
    {
        public override string Texture => AssetDirectory.Bloodiancie + Name;

        public override void OnSpawn(Particle particle)
        {
            particle.color = Color.White;
            particle.frame = Texture2D.Frame();
            particle.rotation = particle.velocity.ToRotation();
        }

        public override void Update(Particle particle)
        {
            particle.fadeIn++;

            if (particle.fadeIn > 30)
            {
                particle.color.A = (byte)(particle.color.A * 0.7f);
            }

            if (particle.color.A < 10)
            {
                particle.active = false;
            }
        }
    }
}
