using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.CoraliteActivities
{
    [VaultLoaden(AssetDirectory.NoteActivities)]
    public class StructrueActivityP1 : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        public static ATex CorrtptionShrine { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH2(spriteBatch, Title, Color.Coral);
            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);

            Texture2D tex = CorrtptionShrine.Value;

            //绘制图2
            tex.QuickBottomDraw(spriteBatch, Bottom - new Vector2(0, 10), scale: 0.45f);
        }
    }
}
