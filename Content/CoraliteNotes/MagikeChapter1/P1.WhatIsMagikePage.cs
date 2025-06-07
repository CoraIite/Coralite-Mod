using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class WhatIsMagikePage : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        private ScaleController _scale = new ScaleController(0.9f, 0.1f);

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        public override void Recalculate()
        {
            _scale.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH2(spriteBatch, Title, Coralite.MagicCrystalPink);
            Vector2 pos = PageTop + new Vector2(0, TitleHeight);

            //描述段
            DrawParaNormal(spriteBatch, Description, pos.Y, out Vector2 textSize);

            var tex = CoraliteAssets.MagikeChapter1.WhatIsMagike.Value;
            pos.Y += textSize.Y + 20 + tex.Height / 2;

            //绘制图片
            Helper.DrawMouseOverScaleTex(spriteBatch, tex, pos, ref _scale, 20);
        }
    }
}
