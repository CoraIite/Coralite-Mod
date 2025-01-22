using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.RedJade
{
    public class RedJadePage:KnowledgePage
    {
        public static LocalizedText Title { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, Title.Value, Center + new Vector2(0, -PageWidth / 2), Coralite.RedJadeRed, 1, 0.5f, 0.5f);

            Vector2 pos = Position + new Vector2(0, 140);
        }
    }
}
