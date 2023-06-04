using Coralite.Core;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Shadow
{
    public class ShadowCircle:ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowProjectiles+Name;

        public override void SetDefaults()
        {
        }
    }
}
