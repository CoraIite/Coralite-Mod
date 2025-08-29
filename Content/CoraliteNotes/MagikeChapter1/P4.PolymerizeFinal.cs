using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    [AutoLoadTexture(Path = AssetDirectory.NoteMagikeS1)]
    public class PolymerizeFinal : KnowledgePage
    {
        public static LocalizedText NotRepeat { get; private set; }
        private ScaleController _scale1 = new ScaleController(0.9f, 0.05f);

        public static ATex NotRepeatTex { get; private set; }

        public override void OnInitialize()
        {
            NotRepeat = this.GetLocalization(nameof(NotRepeat));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 20);

            float scale1 = 0.9f;

            //描述段1
            Helper.DrawTextParagraph(spriteBatch, NotRepeat.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);
            Texture2D tex = NotRepeatTex.Value;
            pos.Y += textSize.Y + 40 + tex.Height * scale1 / 2;

            //绘制图1
            Helper.DrawMouseOverScaleTex(spriteBatch, tex, pos, ref _scale1, 6);
        }
    }
}
