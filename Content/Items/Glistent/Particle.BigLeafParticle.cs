using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Content.Items.Glistent
{
    public abstract class BigLeafParticle:Particle
    {
        public sealed override string Texture => AssetDirectory.MagikeProjectiles+ "LeafShield";

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
