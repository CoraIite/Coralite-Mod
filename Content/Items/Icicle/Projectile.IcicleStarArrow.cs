using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleStarArrow : ModProjectile, IDrawPrimitive, IDrawAdditive
    {
        public override string Texture => AssetDirectory.IcicleItems + "IceStarLight";

        BasicEffect effect;
        private Trail trail;

        public IcicleStarArrow()
        {
            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 1000;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[16];
            for (int i = 0; i < 16; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
            for (int i = 0; i < 15; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;

            trail ??= new Trail(Main.instance.GraphicsDevice, 16, new NoTip(), factor => 2,
            factor =>
                {
                    if (factor.X > 0.5f)
                        return Color.Lerp(Coralite.Instance.IcicleCyan, Color.White, (factor.X - 0.5f) * 2);

                    return Color.Lerp(new Color(0, 0, 0, 0), Coralite.Instance.IcicleCyan, factor.X / 0.5f);//new Color(99, 83, 142, 0)
                });

            trail.Positions = Projectile.oldPos;
            Lighting.AddLight(Projectile.Center, Coralite.Instance.IcicleCyan.ToVector3());
            if (Projectile.timeLeft % 3 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), DustID.FrostStaff, -Projectile.velocity * 0.2f);
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 center = Projectile.Center - new Vector2(0, Main.rand.Next(140, 220)).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
                    Vector2 velocity = (Projectile.Center + Main.rand.NextVector2Circular(24, 24) - center).SafeNormalize(Vector2.UnitY) * 12;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, velocity,
                        ModContent.ProjectileType<IcicleFalling>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
                }
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                float rot = Main.rand.NextFloat(6.282f);
                for (int i = 0; i < 3; i++)
                    Particle.NewParticle(Projectile.Center, (rot + i * 2.094f).ToRotationVector2(), CoraliteContent.ParticleType<HorizontalStar>(), Coralite.Instance.IcicleCyan, Main.rand.NextFloat(0.2f, 0.3f));
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.Render(effect);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;
            int Timer = 1000 - Projectile.timeLeft;
            float sinProgress = MathF.Sin(Timer * 0.157f);

            spriteBatch.Draw(mainTex, center, null, Color.White, 0f, origin, 1.3f, SpriteEffects.None, 0f);
            float rotation = Timer * 0.04f;
            spriteBatch.Draw(mainTex, center, null, Color.White * 0.5f, rotation, origin, 2f + sinProgress, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTex, center, null, Color.White * 0.6f, rotation + 1.57f, origin, 2f - sinProgress, SpriteEffects.None, 0f);
        }
    }
}