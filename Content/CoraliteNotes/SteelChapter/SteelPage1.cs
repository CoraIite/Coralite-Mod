using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.SteelChapter
{
    public class SteelPage1 : ItemShowPage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
            AddImages();
        }

        public override void AddImages()
        {
            int x = -260;
            int y = 100;
            ItemShowImage i0_0 = NewImage(ItemID.IronBar, new Vector2(x, y), Readfragment.KnowledgeButtonType.None)
                .SetColor(Color.Brown);


        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH1(spriteBatch, Title, Color.LightGray);
            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);
        }
    }
}
