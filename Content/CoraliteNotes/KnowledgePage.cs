using Coralite.Content.UI.UILib;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes
{
    public class KnowledgePage : UIPage
    {
        public override bool CanShowInBook => true;

        public override string LocalizationCategory => "Knowledges";

        public const int TitleHeight = 100;

        public Vector2 TitlePos => PageTop + new Vector2(0, 20);

        public void DrawParaNormal(SpriteBatch spriteBatch, LocalizedText text, float yPos, out Vector2 textSize)
        {
            Helper.DrawTextParagraph(spriteBatch, text.Value, PageWidth, new Vector2(Position.X, yPos), out textSize);
        }

        public void DrawTitleH1(SpriteBatch spriteBatch, LocalizedText text, Color color)
        {
            Utils.DrawBorderStringBig(spriteBatch, text.Value, TitlePos, color, 1, 0.5f, 0f);
        }

        public void DrawTitleH2(SpriteBatch spriteBatch, LocalizedText text, Color color)
        {
            Utils.DrawBorderStringBig(spriteBatch, text.Value, PageTop + new Vector2(0, 30), color
                , 0.8f, 0.5f, 0f);
        }
    }
}
