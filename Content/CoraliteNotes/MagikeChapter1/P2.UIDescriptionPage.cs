using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
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
            Vector2 pos = TitlePos;
            //描述段1
            DrawParaNormal(spriteBatch, OpenUI, pos.Y, out Vector2 textSize);

            var tex = CoraliteAssets.MagikeChapter1.UIDescription.Value;
            pos.Y += textSize.Y + tex.Height / 2 + 30;
            //UI图片，不带缩放功能
            tex.QuickCenteredDraw(spriteBatch, pos);

            pos.Y += tex.Height / 2 + 30;
            DrawParaNormal(spriteBatch, UIDescription, pos.Y, out _);
        }
    }
}
