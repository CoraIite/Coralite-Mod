using Coralite.Core.Loaders;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
   public class Tornado : ModParticle
   {
      public override bool ShouldUpdateCenter(Particle particle) => false;

      public override void OnSpawn(Particle particle)
      {

      }

      public override void Update(Particle particle)
      {
      }
   }
}