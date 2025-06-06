using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter
{
    [AutoLoadTexture(Path = AssetDirectory.NoteWeapons)]
    public class LandOfTheLustrousPage : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        public override bool DoublePageWithNext => true;

        public static ATex LandOfTheLustrous { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH1(spriteBatch, Title, new Color(247, 239, 208));
            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);

            Texture2D tex = LandOfTheLustrous.Value;

            //绘制图2
            tex.QuickBottomDraw(spriteBatch, Bottom - new Vector2(0, 10));
        }
    }
}
