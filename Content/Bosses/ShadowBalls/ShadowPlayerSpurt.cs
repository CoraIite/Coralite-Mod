using Coralite.Content.Items.Shadow;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowPlayerSpurt : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        ref float Timer => ref Projectile.ai[0];
        private Vector2 recordVelocity;

        public ParticleGroup triangles;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 500;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            recordVelocity = Projectile.velocity;
            Projectile.velocity *= 0;
        }

        public override bool? CanDamage()
        {
            if (Timer<160||Timer > 160 + 120)
            {
                return false;
            }
            return base.CanDamage();
        }

        public override void AI()
        {
            triangles ??= new ParticleGroup();

            if (Timer < 80*2)//反方向运动
            {
                float factor = Timer / 160;

                Projectile.velocity = -recordVelocity * MathF.Cos(factor * MathHelper.Pi);
            }
            else if (Timer < 160 + 120)
            {
                for (int i = 0; i < 2; i++)
                triangles.NewParticle(Projectile.position + new Vector2(Main.rand.Next(Projectile.width), Main.rand.Next(Projectile.height))
                    , Helpers.Helper.NextVec2Dir() * Main.rand.NextFloat(0.5f, 1.5f), CoraliteContent.ParticleType<ShadowTriangle>(),
                    Main.rand.NextBool() ? new Color(0, 0, 0, 100) : new Color(189, 109, 255, 50)*Main.rand.NextFloat(0.7f,1.05f)
                    , Main.rand.NextFloat(0.5f, 0.75f));
            }
            else
            {
                Projectile.velocity *= 0.95f;
                if (!triangles.Any())
                {
                    Projectile.Kill();
                }
            }

            triangles.UpdateParticles();
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];

            float shadow = Timer < 120 ? Math.Clamp(1 - Timer / 120, 0.2f, 1f) : Math.Clamp((Timer - 120) / 120, 0.2f, 1f);

            Main.PlayerRenderer.DrawPlayer(Main.Camera, owner, Projectile.position, 0f, owner.fullRotationOrigin,shadow );

            triangles?.DrawParticles(Main.spriteBatch);

            return false;
        }
    }
}
