using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial3
{
    [VaultLoaden(AssetDirectory.CoraliteNote + "MagikeInterstitial3/")]
    public class MagikeInterstitial3Slab1 : KnowledgePage
    {
        public static ATex CrystallineSlab1 { get; private set; }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CrystallineSlab1.Value.QuickCenteredDraw(spriteBatch, Center, scale: 1.15f);
        }
    }
}
