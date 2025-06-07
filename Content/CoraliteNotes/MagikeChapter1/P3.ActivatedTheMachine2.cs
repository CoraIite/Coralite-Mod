using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class ActivatedTheMachine2 : KnowledgePage
    {
        public static LocalizedText WorkingBeLike { get; private set; }
        public static LocalizedText HarvestTheProduct { get; private set; }

        private ScaleController _scale = new ScaleController(1f, 0.05f);
        private ScaleController _scale2 = new ScaleController(0.5f, 0.05f);

        public override void OnInitialize()
        {
            WorkingBeLike = this.GetLocalization(nameof(WorkingBeLike));
            HarvestTheProduct = this.GetLocalization(nameof(HarvestTheProduct));
        }

        public override void Recalculate()
        {
            _scale = new ScaleController(1f, 0.05f);
            _scale2 = new ScaleController(0.5f, 0.05f);
            _scale.ResetScale();
            _scale2.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = TitlePos;

            //描述段1
            DrawParaNormal(spriteBatch, WorkingBeLike, pos.Y, out Vector2 textSize);

            Texture2D tex = CoraliteAssets.MagikeChapter1.Working.Value;
            pos.Y += textSize.Y + tex.Height / 2 * _scale.targetScale + 20;

            Helper.DrawMouseOverScaleTex(spriteBatch, tex
                , pos, ref _scale, 10);

            pos.Y += textSize.Y + tex.Height / 2 * _scale.targetScale + 20;

            //描述段2
            DrawParaNormal(spriteBatch, HarvestTheProduct, pos.Y, out textSize);

            tex = CoraliteAssets.MagikeChapter1.HarvestStone.Value;
            pos.Y += textSize.Y + tex.Height / 2 * _scale2.targetScale + 30;

            Helper.DrawMouseOverScaleTex(spriteBatch, tex
                , pos, ref _scale2, 10);
        }
    }
}
