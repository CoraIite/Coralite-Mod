using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.FlowerGunChapter
{
    [VaultLoaden(AssetDirectory.NoteWeapons)]
    public class FlowerGunPage : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        public override bool DoublePageWithNext => true;

        public static ATex FlowerGun { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D tex = FlowerGun.Value;

            tex.QuickBottomDraw(spriteBatch, Bottom - new Vector2(0, 5));

            DrawTitleH1(spriteBatch, Title, Color.Red);
            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);
        }
    }
}
