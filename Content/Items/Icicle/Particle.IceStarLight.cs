using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
   public class IceStarLight : ModParticle, IDrawParticlePrimitive
   {
      public override string Texture => AssetDirectory.IcicleItems + Name;

      BasicEffect effect;

      public IceStarLight()
      {
         Main.QueueMainThreadAction(() =>
         {
            effect = new BasicEffect(Main.instance.GraphicsDevice);
            effect.VertexColorEnabled = true;
            //effect.TextureEnabled = true;
         });
      }

      public override bool ShouldUpdateCenter(Particle particle) => false;

      public override void OnSpawn(Particle particle)
      {
         particle.color = Color.White;
         particle.frame = new Rectangle(0, 0, 18, 18);
         particle.trail = new Trail(Main.instance.GraphicsDevice, 8, new NoTip(), factor => 2 * particle.scale, factor => Color.Lerp(new Color(0, 0, 0, 0), Coralite.Instance.IcicleCyan, factor.X));
         particle.InitOldCenters(8);
      }

      public override void Update(Particle particle)
      {
         Vector2 targetCenter = GetData(particle);
         if (targetCenter == Vector2.Zero)
         {
            particle.active = false;
            return;
         }

         //非常简单的追击
         float distance = Vector2.Distance(targetCenter, particle.center);
         if (distance < 20)
            Kill(particle);
         else if (distance < 200)
            particle.velocity += Vector2.Normalize(targetCenter - particle.center) * 4;
         else
            particle.velocity += Vector2.Normalize(targetCenter - particle.center);

         if (particle.velocity.Length() > 12)
            particle.velocity = Vector2.Normalize(particle.velocity) * 12;

         if (particle.fadeIn % 2 == 0)
         {
            Dust dust = Dust.NewDustPerfect(particle.center + Main.rand.NextVector2CircularEdge(8, 8), DustID.FrostStaff, -particle.velocity * 0.2f);
            dust.noGravity = true;
         }

         particle.fadeIn++;
         if (particle.fadeIn > 120)
            Kill(particle);

         particle.center += particle.velocity;
         particle.UpdateCachesNormally(8);
         particle.trail.Positions = particle.oldCenter;
      }

      public void DrawPrimitives(Particle particle)
      {
         if (effect == null)
            return;

         Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
         Matrix view = Main.GameViewMatrix.ZoomMatrix;
         Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

         //effect.Texture = Texture2D.Value;
         effect.World = world;
         effect.View = view;
         effect.Projection = projection;

         particle.trail.Render(effect);
      }

      public override void Draw(SpriteBatch spriteBatch, Particle particle)
      {
         ModParticle modParticle = ParticleLoader.GetParticle(particle.type);
         Rectangle frame = particle.frame;
         Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);

         spriteBatch.Draw(modParticle.Texture2D.Value, particle.center - Main.screenPosition, frame, particle.color, particle.rotation, origin, 1.2f, SpriteEffects.None, 0f);
      }

      public static Particle Spawn(Vector2 center, Vector2 velocity, float scale, GetCenter function)
      {
         Particle particle = Particle.NewParticleDirect(center, velocity, CoraliteContent.ParticleType<IceStarLight>(), Color.White, scale);
         particle.datas = new object[1]
         {
                function
         };

         return particle;
      }

      public static Vector2 GetData(Particle particle)
      {
         if (particle.datas is null || particle.datas[0] is not GetCenter)
            return Vector2.Zero;

         return (particle.datas[0] as GetCenter).Invoke();
      }

      public static void Kill(Particle particle)
      {
         particle.active = false;
         Particle.NewParticle(particle.center, Vector2.Zero, CoraliteContent.ParticleType<HorizontalStar>(), Coralite.Instance.IcicleCyan, 0.2f);
      }
   }
}
