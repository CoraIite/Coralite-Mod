using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.MagikeInterstitial3;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    internal class CrystallineEngram : BaseMaterial, IConsultableItem
    {
        public CrystallineEngram() : base(Item.CommonMaxStack, Item.sellPrice(0, 3), ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeSeries2Item)
        { }

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<MagikeInterstitial3Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial3Page4>();
    }
}
