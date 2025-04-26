using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter2
{
    public class GetMagikeKnowledge2Page : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Welcome { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Welcome = this.GetLocalization(nameof(Welcome));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, Title.Value, Center + new Vector2(0, -PageWidth / 2), Coralite.CrystallinePurple, 1, 0.5f, 0.5f);

            Vector2 pos = Position + new Vector2(0, 140);
            Helper.DrawText(spriteBatch, Welcome.Value, PageWidth, pos, Vector2.Zero, Vector2.One
                , new Color(40, 40, 40), Color.White, out Vector2 size);
        }
    }
}
