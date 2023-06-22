//using Coralite.Core;
//using Coralite.Core.Systems.ParticleSystem;
//using Coralite.Helpers;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using Terraria;

//namespace Coralite.Content.Particles
//{
//    public class CrossLight : ModParticle
//    {
//        public override string Texture => AssetDirectory.Blank;

//        public override void OnSpawn(Particle particle)
//        {
//            base.OnSpawn(particle);
//        }

//        public override void Update(Particle particle)
//        {
//            particle.fadeIn--;
//            if (particle.fadeIn < 0)
//            {
//                particle.active = false;
//            }
//        }

//        public override void Draw(SpriteBatch spriteBatch, Particle particle)
//        {
//            float factor = 1 - (particle.fadeIn / particle.oldRot[0]);
//            //float num3 = Utils.Remap(factor, 0f, 0.6f, 0f, 1f) * Utils.Remap(factor, 0.6f, 1f, 1f, 0f);

//            ProjectilesHelper.DrawPrettyStarSparkle(1, SpriteEffects.None, particle.center - Main.screenPosition,
//                new Color(255, 255, 255, 0)  * 0.5f, particle.color, factor, 0f, 0.5f, 0.5f, 1f,
//                particle.rotation, new Vector2(particle.oldRot[1], particle.oldRot[2]), Vector2.One);
//        }


//        public static Particle Spawn(Vector2 center, Vector2 velocity, float rotation, int activeTime,Vector2 sparkleSize, Color newColor = default)
//        {
//            Particle particle = Particle.NewParticleDirect(center, velocity, CoraliteContent.ParticleType<CrossLight>(), newColor, 1);
//            particle.rotation = rotation;
//            particle.fadeIn = activeTime;
//            particle.oldRot = new float[3]
//            {
//                activeTime,
//                sparkleSize.X,
//                sparkleSize.Y
//            };

//            return particle;
//        }
//    }
//}
