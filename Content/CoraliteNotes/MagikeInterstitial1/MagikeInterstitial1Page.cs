using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeInterstitial1
{
    [VaultLoaden(AssetDirectory.CoraliteNote + "MagikeInterstitial1/")]
    public class MagikeInterstitial1Page : ItemShowPage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        //public static ATex SkyIslandEnemyWarn { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.5f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1.5f, 0.2f);

        public override bool AlwaysShowInLeft => true;

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
            AddImages();
        }

        public override void AddImages()
        {
            Vector2 pos = new Vector2(0, 20);

            ItemShowImage i0 = NewImage<LeprechaunBait>(pos + new Vector2( 80,0))
                .SetColor(Coralite.MagicCrystalPink);

            ItemShowImage i0_1 = NewImage<MagicCrystal>(pos + new Vector2( -80,0))
                .SetColor(Coralite.MagicCrystalPink);

            ItemShowImage i0_2 = NewImage(ItemID.Rope,pos + new Vector2(-40, -80),Readfragment.KnowledgeButtonType.None)
                .SetColor(Color.White);
            ItemShowImage i0_3 = NewImage(ItemID.Wood, pos + new Vector2(-40, 80), Readfragment.KnowledgeButtonType.None)
                .SetColor(Color.White);

            i0_1.AddChainedElement(i0);
            i0_2.AddChainedElement(i0);
            i0_3.AddChainedElement(i0);
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH1(spriteBatch, Title, Coralite.MagicCrystalPink);

            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);

        }
    }
}
