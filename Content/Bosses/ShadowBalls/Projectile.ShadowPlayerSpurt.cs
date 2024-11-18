using Coralite.Content.Items.Shadow;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using System;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowPlayerSpurt : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        ref float Timer => ref Projectile.ai[0];
        ref float Shadow => ref Projectile.ai[1];

        private Vector2 recordVelocity;

        public ParticleGroup triangles;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            recordVelocity = Projectile.velocity;
            Projectile.velocity *= 0;
            Shadow = 1;
        }

        public override bool? CanDamage()
        {
            if (Timer < 160 || Timer > 160 + 100)
            {
                return false;
            }
            return base.CanDamage();
        }

        public override void AI()
        {
            triangles ??= new ParticleGroup();

            if (Timer < 80 * 2)//反方向运动
            {
                float factor = Timer / 160;
                Shadow -= 0.95f / 160;

                Projectile.velocity = -recordVelocity * MathF.Cos(factor * MathHelper.Pi);
                triangles.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(24, 24)
                    , -Projectile.velocity * Main.rand.NextFloat(0.5f, 0.75f), CoraliteContent.ParticleType<ShadowTriangle>(),
                      new Color(100, 60, 200, 100) * Main.rand.NextFloat(0.9f, 1.15f)
                    , Main.rand.NextFloat(0.3f, 0.5f));
            }
            else if (Timer < 160 + 100)
            {
                Shadow += 0.3f / 100;
                for (int i = 0; i < 2; i++)
                    triangles.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(24, 24)
                        , -Projectile.velocity * Main.rand.NextFloat(0.5f, 0.75f), CoraliteContent.ParticleType<ShadowTriangle>(),
                         new Color(100, 60, 200, 100) * Main.rand.NextFloat(0.9f, 1.15f)
                    , Main.rand.NextFloat(0.3f, 0.5f));
            }
            else
            {
                if (Shadow < 1)
                    Shadow *= 1.05f;
                Projectile.velocity *= 0.93f;
                if (!triangles.Any())
                {
                    Projectile.Kill();
                }
            }

            triangles.Update();
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];

            //float shadow = Timer < 160 ? Math.Clamp(1 - Timer / 160, 0.1f, 1f) : Math.Clamp((Timer - 160) / 100, 0.1f, 1f);

            Main.PlayerRenderer.DrawPlayer(Main.Camera, owner, Projectile.Center + new Vector2(-16, -24), 0f, owner.fullRotationOrigin, Shadow);

            triangles?.Draw(Main.spriteBatch);

            return false;
        }
    }
}
