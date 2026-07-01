using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial3
{
    [VaultLoaden(AssetDirectory.CoraliteNote + "MagikeInterstitial3/")]
    public class MagikeInterstitial3Page : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        public static ATex SkyIslandEnemyWarn { get; private set; }

        public override bool AlwaysShowInLeft => true;

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH1(spriteBatch, Title, Coralite.CrystallinePurple);

            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);

            SkyIslandEnemyWarn.Value.QuickBottomDraw(spriteBatch, Bottom - new Vector2(0, 10), scale: 1f);
        }
    }
}
