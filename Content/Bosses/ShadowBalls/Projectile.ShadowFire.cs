using Coralite.Content.Particles;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowFire : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Blank;

        ref float State => ref Projectile.ai[0];

        private PrimitivePRTGroup fireParticles;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 64;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage()
        {
            if (State != 0)
                return false;
            return base.CanDamage();
        }

        public override void AI()
        {
            fireParticles ??= new PrimitivePRTGroup();

            switch (State)
            {
                default:
                case 0://下落
                    {
                        if (Projectile.velocity.Y < 16)
                            Projectile.velocity.Y += 0.15f;

                        //for (int i = 0; i < 2; i++)
                        fireParticles.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                        (Projectile.velocity * Main.rand.NextFloat(0.8f, 1.5f)).RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)),
                        CoraliteContent.ParticleType<FireParticle>(), new Color(140, 60, 255), Main.rand.NextFloat(0.8f, 1f));

                        Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.2f, 1.4f));

                        if (Projectile.Center.Y > CoraliteWorld.shadowBallsFightArea.Bottom)
                        {
                            Projectile.velocity *= 0;
                            State = 1;
                        }
                    }
                    break;
                case 1://他紫砂了
                    {
                        if (!fireParticles.Any())
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;
            }

            fireParticles.Update();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //var a= Main.graphics.GraphicsDevice.BlendState;
            // fireParticles?.DrawParticles(Main.spriteBatch);
            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            fireParticles?.Draw(Main.spriteBatch);
        }
    }
}
