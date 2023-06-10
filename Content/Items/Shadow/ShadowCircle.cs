using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Shadow
{
    public class ShadowCircle : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowProjectiles + Name;

        public override void SetDefaults()
        {

        }

        public override void AI()
        {
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
