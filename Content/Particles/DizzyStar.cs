using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Particles
{
    public delegate Vector2 GetCenter();

    public class DizzyStar : TrailParticle
    {
        public override string Texture => AssetDirectory.DefaultItem;

        private static BasicEffect effect;
        private GetCenter centerFunc;
        private float length;

        public DizzyStar()
        {
            if (Main.dedServ)
            {
                return;
            }

            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
            });
        }

        public override bool ShouldUpdateCenter() => false;

        public override void OnSpawn()
        {
            //使用oldRot充当改变帧图的 frameCounter
            Frame = new Rectangle(0, 0, 22, 26);
            Scale = 1f;
            InitOldCaches(12);
            trail = new Trail(Main.instance.GraphicsDevice, 12, new NoTip(), factor => 2, factor => Color.Lerp(new Color(0, 0, 0, 0), Color.Yellow, factor.X));
        }

        public override void Update()
        {
            if (centerFunc != null)
            {
                Vector2 center = centerFunc.Invoke();
                Center = center + Rotation.ToRotationVector2() * length * Helper.EllipticalEase(Rotation, 1, 2.4f);
                Rotation += 0.12f;
                Scale = 0.6f + MathF.Sin(Rotation) * 0.2f;

                //更新拖尾数组
                for (int i = 0; i < 11; i++)
                    oldRot[i] = oldRot[i + 1];

                oldRot[11] = Rotation;
                for (int i = 0; i < 12; i++)
                    oldCenter[i] = center + oldRot[i].ToRotationVector2() * length * Helper.EllipticalEase(oldRot[i], 1, 2.4f);
                trail.Positions = oldCenter;

                //使用oldRot充当改变帧图的 frameCounter
                Velocity.X += 1f;
                if (Velocity.X > 3f)
                {
                    Velocity.X = 0f;
                    Frame.Y += 26;
                    if (Frame.Y > 181)
                        Frame.Y = 0;
                }

                fadeIn -= 1f;
                if (fadeIn < 0f)
                    active = false;

                return;
            }

            active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Main.instance.LoadItem(ItemID.FallenStar);
            Texture2D mainTex = TextureAssets.Item[ItemID.FallenStar].Value;

            spriteBatch.Draw(mainTex, Center - Main.screenPosition, Frame, Color.White, 0f, new Vector2(11, 13), Scale, SpriteEffects.None, 0f);
        }

        public override void DrawPrimitives()
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

        public static DizzyStar Spawn(Vector2 center, float rotation, float dizzyTime, float length, GetCenter function)
        {
            DizzyStar particle = NewParticle<DizzyStar>(center, Vector2.Zero);
            particle.Rotation = rotation;
            particle.fadeIn = dizzyTime;
            particle.length = length;
            particle.centerFunc = function;

            return particle;
        }
    }
}