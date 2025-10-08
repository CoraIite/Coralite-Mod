using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.DashBowChapter
{
    [VaultLoaden(AssetDirectory.NoteWeapons)]
    public class DashBowPage : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        public override bool DoublePageWithNext => true;

        public static ATex DashBow { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH1(spriteBatch, Title, new Color(0, 228, 205));
            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);

            Texture2D tex = DashBow.Value;

            //绘制图2
            tex.QuickBottomDraw(spriteBatch, Bottom - new Vector2(0, 10));
        }
    }
}
