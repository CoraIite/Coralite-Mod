using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class StoneMaker : KnowledgePage
    {
        public static LocalizedText UseMagikeToDoSomething { get; private set; }
        public static LocalizedText MakeAStoneMaker { get; private set; }
        public static LocalizedText InsertPolarizedFilter { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale2 = new ScaleController(0.6f, 0.1f);

        public override void OnInitialize()
        {
            UseMagikeToDoSomething = this.GetLocalization(nameof(UseMagikeToDoSomething));
            MakeAStoneMaker = this.GetLocalization(nameof(MakeAStoneMaker));
            InsertPolarizedFilter = this.GetLocalization(nameof(InsertPolarizedFilter));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2 = new ScaleController(0.5f, 0.1f);
            _scale2.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //标题
            DrawTitleH1(spriteBatch, UseMagikeToDoSomething, Coralite.MagicCrystalPink);

            Vector2 pos = PageTop + new Vector2(0, TitleHeight);

            //绘制造石机
            Vector2 picturePos = new Vector2(pos.X, pos.Y + 40);

            Helper.DrawMouseOverScaleTex<Items.Magike.Factorys.StoneMaker>(spriteBatch, picturePos
                , ref _scale1, 4, 5, fadeWithOriginScale: true);


            pos.Y += 100;

            //绘制右边文字
            DrawParaNormal(spriteBatch, MakeAStoneMaker, pos.Y, out Vector2 textSize);


            var tex = CoraliteAssets.MagikeChapter1.StoneMakerExample.Value;
            pos.Y += textSize.Y + tex.Height * 0.6f / 2;

            Helper.DrawMouseOverScaleTex(spriteBatch, tex, pos, ref _scale2, 8);
            pos.Y += tex.Height * 0.6f / 2 + 10;

            //描述段2
            DrawParaNormal(spriteBatch, InsertPolarizedFilter, pos.Y, out _);
        }
    }
}
