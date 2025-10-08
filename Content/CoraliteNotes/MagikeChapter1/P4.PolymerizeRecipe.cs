using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    [VaultLoaden(AssetDirectory.NoteMagikeS1)]
    public class PolymerizeRecipe : KnowledgePage
    {
        public static LocalizedText PolymerizeRecipeLike { get; private set; }
        private ScaleController _scale1 = new ScaleController(0.9f, 0.05f);

        public static ATex PolymerizeRecipeTex { get; private set; }

        public override void OnInitialize()
        {
            PolymerizeRecipeLike = this.GetLocalization(nameof(PolymerizeRecipeLike));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 30);

            float scale1 = 0.9f;

            //描述段1
            Helper.DrawTextParagraph(spriteBatch, PolymerizeRecipeLike.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);
            Texture2D tex = PolymerizeRecipeTex.Value;
            pos.Y += textSize.Y + 40 + tex.Height * scale1 / 2;

            //绘制图1
            Helper.DrawMouseOverScaleTex(spriteBatch, tex, pos, ref _scale1, 6);
        }
    }
}
