using Coralite.Content.Items.Magike.Pedestal;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    [VaultLoaden(AssetDirectory.NoteMagikeS1)]
    public class Pedestal : KnowledgePage
    {
        public static LocalizedText PlacePedestal { get; private set; }
        private ScaleController _scale1 = new ScaleController(1.5f, 0.15f);
        private ScaleController _scale2 = new ScaleController(0.9f, 0.05f);

        public static ATex PlacePedestalTex { get; private set; }

        public override void OnInitialize()
        {
            PlacePedestal = this.GetLocalization(nameof(PlacePedestal));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 60);

            Helper.DrawMouseOverScaleTex<BasicPedestal>(spriteBatch, pos
                , ref _scale1, 4, 5, fadeWithOriginScale: true);

            float scale1 = 0.9f;

            pos.Y += 70;

            //描述段1
            Helper.DrawTextParagraph(spriteBatch, PlacePedestal.Value, PageWidth, new Vector2(Position.X, pos.Y), out Vector2 textSize);
            Texture2D tex = PlacePedestalTex.Value;
            pos.Y += textSize.Y + 40 + tex.Height * scale1 / 2;

            //绘制图1
            Helper.DrawMouseOverScaleTex(spriteBatch, tex, pos, ref _scale2, 6);
        }
    }
}
