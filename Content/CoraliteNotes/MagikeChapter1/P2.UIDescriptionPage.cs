using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class UIDescriptionPage : KnowledgePage
    {
        public static LocalizedText OpenUI { get; private set; }
        public static LocalizedText UIDescription { get; private set; }

        public override void OnInitialize()
        {
            OpenUI = this.GetLocalization(nameof(OpenUI));
            UIDescription = this.GetLocalization(nameof(UIDescription));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);
            //描述段1
            Helper.DrawTextParagraph(spriteBatch, OpenUI.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);

            var tex = CoraliteAssets.MagikeChapter1.UIDescription.Value;
            pos.Y += textSize.Y + tex.Height / 2 + 20;
            //UI图片，不带缩放功能
            Vector2 origin = tex.Size() / 2;
            spriteBatch.Draw(tex, pos, null, Color.White, 0, origin, 1, 0, 0);

            pos.Y += tex.Height / 2 + 20;
            Helper.DrawTextParagraph(spriteBatch, UIDescription.Value, PageWidth, new Vector2(Position.X, pos.Y), out _);
        }
    }
}
