using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Particles
{
    public delegate Vector2 GetCenter();

    public class DizzyStar : ModParticle, IDrawParticlePrimitive
    {
        public override string Texture => AssetDirectory.DefaultItem;

        BasicEffect effect;

        public DizzyStar()
        {
            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override bool ShouldUpdateCenter(Particle particle) => false;

        public override void OnSpawn(Particle particle)
        {
            //使用oldRot充当改变帧图的 frameCounter
            particle.frame = new Rectangle(0, 0, 22, 26);
            particle.scale = 1f;
            particle.InitOldCaches(12);
            particle.trail = new Trail(Main.instance.GraphicsDevice, 12, new NoTip(), factor => 2, factor => Color.Lerp(new Color(0, 0, 0, 0), Color.Yellow, factor.X));
        }

        public override void Update(Particle particle)
        {
            if (GetDatas(particle, out float length, out GetCenter function))
            {
                Vector2 center = function.Invoke();
                particle.center = center + particle.rotation.ToRotationVector2() * length * Helper.EllipticalEase(particle.rotation, 1, 2.4f);
                particle.rotation += 0.12f;
                particle.scale = 0.6f + MathF.Sin(particle.rotation) * 0.2f;

                //更新拖尾数组
                for (int i = 0; i < 11; i++)
                    particle.oldRot[i] = particle.oldRot[i + 1];

                particle.oldRot[11] = particle.rotation;
                for (int i = 0; i < 12; i++)
                    particle.oldCenter[i] = center + particle.oldRot[i].ToRotationVector2() * length * Helper.EllipticalEase(particle.oldRot[i], 1, 2.4f);
                particle.trail.Positions = particle.oldCenter;

                //使用oldRot充当改变帧图的 frameCounter
                particle.velocity.X += 1f;
                if (particle.velocity.X > 3f)
                {
                    particle.velocity.X = 0f;
                    particle.frame.Y += 26;
                    if (particle.frame.Y > 181)
                        particle.frame.Y = 0;
                }

                particle.fadeIn -= 1f;
                if (particle.fadeIn < 0f)
                    particle.active = false;

                return;
            }

            particle.active = false;
        }

        public override void Draw(SpriteBatch spriteBatch, Particle particle)
        {
            Texture2D mainTex = TextureAssets.Item[ItemID.FallenStar].Value;

            spriteBatch.Draw(mainTex, particle.center - Main.screenPosition, particle.frame, Color.White, 0f, new Vector2(11, 13), particle.scale, SpriteEffects.None, 0f);
        }

        public void DrawPrimitives(Particle particle)
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            particle.trail?.Render(effect);
        }


        public static Particle Spawn(Vector2 center, float rotation, float dizzyTime, float length, GetCenter function)
        {
            Particle particle = Particle.NewParticleDirect(center, Vector2.Zero, CoraliteContent.ParticleType<DizzyStar>());
            particle.rotation = rotation;
            particle.fadeIn = dizzyTime;
            particle.datas = new object[2]
            {
                length,
                function
            };

            return particle;
        }

        public static bool GetDatas(Particle particle, out float length, out GetCenter function)
        {
            if (particle.datas is null || particle.datas[0] is not float || particle.datas[1] is not GetCenter)
            {
                length = 0;
                function = null;
                return false;
            }

            length = (float)particle.datas[0];
            function = (GetCenter)particle.datas[1];
            return true;
        }
    }
}