using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IceStarLight : TrailParticle
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        private static BasicEffect effect;
        private GetCenter centerFunc;
        private float velocityLimit;

        public IceStarLight()
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
            color = Color.White;
            Frame = new Rectangle(0, 0, 18, 18);
            trail = new Trail(Main.instance.GraphicsDevice, 8, new NoTip(), factor => 2 * Scale, factor => Color.Lerp(new Color(0, 0, 0, 0), Coralite.Instance.IcicleCyan, factor.X));
            InitOldCenters(8);
        }

        public override void Update()
        {
            GetData(out Vector2 targetCenter);
            if (targetCenter == Vector2.Zero)
            {
                active = false;
                return;
            }

            //非常简单的追击
            float distance = Vector2.Distance(targetCenter, Center);
            if (distance < 20)
                Kill();
            else if (distance < 100)
                Velocity += Vector2.Normalize(targetCenter - Center) * 8;
            else if (distance < 200)
                Velocity += Vector2.Normalize(targetCenter - Center) * 4;
            else
                Velocity += Vector2.Normalize(targetCenter - Center);

            if (Velocity.Length() > velocityLimit)
                Velocity = Vector2.Normalize(Velocity) * velocityLimit;

            if (fadeIn % 2 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Center + Main.rand.NextVector2CircularEdge(8, 8), DustID.FrostStaff, -Velocity * 0.2f);
                dust.noGravity = true;
            }

            fadeIn++;
            if (fadeIn > 120)
                Kill();

            Center += Velocity;
            UpdatePosCachesNormally(8);
            trail.Positions = oldCenter;
        }

        public override void DrawPrimitives()
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            //effect.Texture = Texture2D.Value;
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            trail?.Render(effect);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle frame = Frame;
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);

            spriteBatch.Draw(GetTexture().Value, Center - Main.screenPosition, frame, color, Rotation, origin, 1.2f, SpriteEffects.None, 0f);
        }

        public static Particle Spawn(Vector2 center, Vector2 velocity, float scale, GetCenter function, float velocityLimit)
        {
            IceStarLight particle = NewParticle<IceStarLight>(center, velocity, Color.White, scale);
            particle.centerFunc = function;
            particle.velocityLimit = velocityLimit;

            return particle;
        }

        public void GetData(out Vector2 center)
        {
            if (centerFunc == null)
            {
                center = Vector2.Zero;
                velocityLimit = 100;
                return;
            }

            center = centerFunc.Invoke();
        }

        public void Kill()
        {
            active = false;
            NewParticle<HorizontalStar>(Center, Vector2.Zero, Coralite.Instance.IcicleCyan, 0.2f);
        }
    }
}
