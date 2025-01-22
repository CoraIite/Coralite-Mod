using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class CraftAltar2 : KnowledgePage
    {
        public static LocalizedText ActiveAltar { get; private set; }
        public static LocalizedText GetResult { get; private set; }

        public override void OnInitialize()
        {
            ActiveAltar = this.GetLocalization(nameof(ActiveAltar));
            GetResult = this.GetLocalization(nameof(GetResult));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);

            float scale1 = 0.7f;

            //描述段1
            Helper.DrawTextParagraph(spriteBatch, ActiveAltar.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);
            ATex tex1 = CoraliteAssets.MagikeChapter1.ActiveAltar;
            pos.Y += textSize.Y + 10 + tex1.Height() * scale1 / 2;

            //绘制图1
            tex1.Value.QuickCenteredDraw(spriteBatch, pos, scale: scale1);
            pos.Y += 20 + tex1.Height() * scale1 / 2;

            float scale2 = 0.9f;

            //描述段2
            Helper.DrawTextParagraph(spriteBatch, GetResult.Value, PageWidth, new Vector2(Position.X, pos.Y), out textSize);
            ATex tex2 = CoraliteAssets.MagikeChapter1.AltarCraftSuccess;
            pos.Y += textSize.Y + 10 + tex2.Height() * scale2 / 2;

            //绘制图2
            tex2.Value.QuickCenteredDraw(spriteBatch, pos, scale: scale2);
        }
    }
}
