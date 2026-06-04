using Coralite.Content.Items.Magike.Refractors;
using Coralite.Content.Items.Magike.Tools.MiniColumns;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeToolWeapon1
{
    [VaultLoaden(AssetDirectory.CoraliteNote + "MagikeToolWeapon1")]
    public class MagikeToolWeaponPage1 : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText MagikeCannonDescription { get; private set; }

        public static ATex MagikeCannon { get; private set; }

        private ScaleController _scale1 = new ScaleController(1f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale3 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale4 = new ScaleController(1.4f, 0.2f);

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            MagikeCannonDescription = this.GetLocalization(nameof(MagikeCannonDescription));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            _scale3.ResetScale();
            _scale4.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D tex = MagikeCannon.Value;

            //绘制图2
            tex.QuickBottomDraw(spriteBatch, Bottom, scale: 1);


            DrawTitleH1(spriteBatch, Title, Coralite.MagicCrystalPink);

            Vector2 pos = Position + new Vector2(PageWidth / 2, TitleHeight);
            DrawParaNormal(spriteBatch, MagikeCannonDescription, pos.Y, out _);

            #region 绘制物品
            Vector2 picturePos = Position + new Vector2(240, PageHeight - 230);

            Helper.DrawMouseOverScaleTex<MagikeLaserCannon>(spriteBatch, picturePos
                , ref _scale1, 2, 5, fadeWithOriginScale: true);

            picturePos = Position + new Vector2(395, PageHeight - 350);

            Helper.DrawMouseOverScaleTex<LASERCore>(spriteBatch, picturePos
                , ref _scale2, 3, 5, fadeWithOriginScale: true);

            picturePos = Position + new Vector2(455, PageHeight - 350);

            Helper.DrawMouseOverScaleTex(spriteBatch, picturePos, ItemID.HellstoneBar
                , ref _scale3, 3, 5, fadeWithOriginScale: true);

            picturePos = Position + new Vector2(285, PageHeight - 60);

            Helper.DrawMouseOverScaleTex<MiniCrystalColumn>(spriteBatch, picturePos
                , ref _scale4, 3, 5, fadeWithOriginScale: true);
            #endregion
        }
    }
}
