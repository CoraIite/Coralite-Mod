using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter2
{
    [AutoLoadTexture(Path = AssetDirectory.NoteMagikeS2)]
    public class MabirdNestUI : KnowledgePage
    {
        public static LocalizedText UIDescription { get; private set; }

        public static ATex MabirdNestUITex { get; private set; }
        public static ATex MabirdNestUITex2 { get; private set; }

        public override void OnInitialize()
        {
            UIDescription = this.GetLocalization(nameof(UIDescription));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);

            float scale = 0.7f;
            pos.Y += MabirdNestUITex.Height() / 2 * scale;
            MabirdNestUITex.Value.QuickCenteredDraw(spriteBatch, pos, scale: scale);
            pos.Y += 20 + MabirdNestUITex.Height() / 2 * scale;

            //描述段1
            Helper.DrawTextParagraph(spriteBatch, UIDescription.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);
            pos.Y += textSize.Y + MabirdNestUITex2.Height() / 2;

            MabirdNestUITex2.Value.QuickCenteredDraw(spriteBatch, pos);
        }
    }
}
