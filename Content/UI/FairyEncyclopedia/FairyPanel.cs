using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    [VaultLoaden(AssetDirectory.FairyUI)]
    public class FairyPanel : UIPanel
    {
        public static ATex FairyPanelCorner { get; set; }

        public FairyPanel() : base(FairyEncyclopedia.FairyPanelBackGround
            , FairyEncyclopedia.FairyPanelBorder, 12, 20)
        {
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();

            const int Length = 42;
            const int Length2 = 52;

            FairyPanelCorner.Value.QuickCenteredDraw(spriteBatch, new Rectangle(0, 0, 2, 1)
                , dimensions.Position() + new Vector2(Length + 10, Length), Color.White * 0.6f);
            FairyPanelCorner.Value.QuickCenteredDraw(spriteBatch, new Rectangle(1, 0, 2, 1)
                , dimensions.Position() + new Vector2(dimensions.Width - 4 - Length2, dimensions.Height - Length2), Color.White * 0.6f);
        }
    }
}
