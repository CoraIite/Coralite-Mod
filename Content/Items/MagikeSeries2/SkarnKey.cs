using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.MagikeInterstitial3;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkarnKey : ModItem,IConsultableItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<MagikeInterstitial3Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial3Page3>();

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(3, 17));
        }

        public override void SetDefaults()
        {
            Item.maxStack = 99;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 1));
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
        }
    }
}
