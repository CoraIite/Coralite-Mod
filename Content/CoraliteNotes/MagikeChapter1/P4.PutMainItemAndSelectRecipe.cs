using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class PutMainItemAndSelectRecipe : KnowledgePage
    {
        public static LocalizedText PutMainItemIntoAltar { get; private set; }
        public static LocalizedText SelectRecipe { get; private set; }

        public override void OnInitialize()
        {
            PutMainItemIntoAltar = this.GetLocalization(nameof(PutMainItemIntoAltar));
            SelectRecipe = this.GetLocalization(nameof(SelectRecipe));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);

            float scale1 = 0.8f;

            //描述段1
            Helper.DrawTextParagraph(spriteBatch, PutMainItemIntoAltar.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);
            Texture2D tex = CoraliteAssets.MagikeChapter1.PutMainItemIn.Value;
            pos.Y += textSize.Y + 10 + tex.Height* scale1 / 2;

            //绘制图1
            tex.QuickCenteredDraw(spriteBatch, pos,scale: scale1);
            pos.Y += 20 + tex.Height* scale1 / 2;

            float scale2 = 0.9f;

            //描述段2
            Helper.DrawTextParagraph(spriteBatch, SelectRecipe.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
            tex = CoraliteAssets.MagikeChapter1.SelectRecipe.Value;
            pos.Y += textSize.Y + 10 + tex.Height* scale2 / 2;

            //绘制图2
            tex.QuickCenteredDraw(spriteBatch, pos,scale: scale2);
        }
    }
}