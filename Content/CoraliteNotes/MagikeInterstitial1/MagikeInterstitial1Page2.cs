using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial1
{
    [VaultLoaden(AssetDirectory.CoraliteNote + "MagikeInterstitial1/")]
    public class MagikeInterstitial1Page2 : KnowledgePage
    {
        public static LocalizedText Description { get; private set; }

        public static ATex SkyIslandEnemyWarn { get; private set; }

        public override void OnInitialize()
        {
            Description = this.GetLocalization(nameof(Description));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawParaNormal(spriteBatch, Description, TitlePos.Y, out _);

            SkyIslandEnemyWarn.Value.QuickBottomDraw(spriteBatch, Bottom - new Vector2(0, 10), scale: 1.15f);
        }
    }
}
