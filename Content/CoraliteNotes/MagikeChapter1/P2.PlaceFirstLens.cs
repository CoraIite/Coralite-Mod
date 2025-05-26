using Coralite.Content.Items.Magike.Lens.ExtractLens;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using static Coralite.Core.Systems.FairyCatcherSystem.FairySystem;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class PlaceFirstLens : KnowledgePage
    {
        public static LocalizedText BuildYourFirstMagike { get; private set; }
        public static LocalizedText ExtractLens { get; private set; }
        public static LocalizedText TryMouseHover { get; private set; }
        public static LocalizedText FourWayPlace { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale2 = new ScaleController(0.9f, 0.1f);

        public override void OnInitialize()
        {
            BuildYourFirstMagike = this.GetLocalization(nameof(BuildYourFirstMagike));
            ExtractLens = this.GetLocalization(nameof(ExtractLens));
            TryMouseHover = this.GetLocalization(nameof(TryMouseHover));
            FourWayPlace = this.GetLocalization(nameof(FourWayPlace));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH2(spriteBatch, BuildYourFirstMagike, Coralite.MagicCrystalPink);
            Vector2 pos = PageTop + new Vector2(0, TitleHeight);

            DrawParaNormal(spriteBatch, ExtractLens, pos.Y, out Vector2 textSize);

            var tex1 = TextureAssets.Item[ModContent.ItemType<BasicExtractLens>()].Value;
            var tex2 = CoraliteAssets.MagikeChapter1.PlaceFirstLens.Value;
            pos.Y += textSize.Y + tex2.Height / 2;

            Vector2 picturePos = new Vector2(Position.X + (PageWidth - tex2.Width) / 2, pos.Y);

            //绘制左边的透镜贴图
            Helper.DrawMouseOverScaleTex<BasicExtractLens>(spriteBatch, picturePos,ref _scale1
                ,3, 5, fadeWithOriginScale: true);
            Utils.DrawBorderString(spriteBatch, TryMouseHover.Value, picturePos + new Vector2(0, -tex1.Height * 1.5f), Coralite.MagicCrystalPink
                , 1f, 0.5f, 0.5f);

            picturePos.X = Position.X + PageWidth - tex2.Width / 2;

            //绘制右边的图片
            Helper.DrawMouseOverScaleTex(spriteBatch, tex2, picturePos,ref _scale2, 10);

            pos += new Vector2(0, tex2.Height / 2 + 20);
            DrawParaNormal(spriteBatch, FourWayPlace, pos.Y, out _);
        }
    }
}
