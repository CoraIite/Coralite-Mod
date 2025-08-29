using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial1
{
    [VaultLoaden(AssetDirectory.CoraliteNote + "MagikeInterstitial1/")]
    public class MagikeInterstitial1Page : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        public static ATex SkyIslandUnlockGuide { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.5f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1.5f, 0.2f);

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH1(spriteBatch, Title, Coralite.CrystallinePurple);
            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);

            //绘制图
            SkyIslandUnlockGuide.Value.QuickBottomDraw(spriteBatch, Bottom - new Vector2(0, 10));

            //绘制光暗魂
            Vector2 picturePos = new Vector2(Center.X - 57, Center.Y + 103);
            Helper.DrawMouseOverScaleTex(spriteBatch, picturePos, ItemID.SoulofLight
                , ref _scale1, 3, 5, fadeWithOriginScale: true);

            picturePos = new Vector2(Center.X - 197, Center.Y + 70);
            Helper.DrawMouseOverScaleTex(spriteBatch, picturePos, ItemID.SoulofNight
                , ref _scale2, 3, 5, fadeWithOriginScale: true);
        }
    }
}
