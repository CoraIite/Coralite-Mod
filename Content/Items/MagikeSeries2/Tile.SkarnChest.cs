using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.MagikeInterstitial3;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkarnChest : BaseChestItem,IConsultableItem
    {
        public SkarnChest() : base(Item.sellPrice(0, 0, 0, 10), ModContent.RarityType<CrystallineMagikeRarity>(), ModContent.TileType<SkarnChestTile>(), AssetDirectory.MagikeSeries2Item)
        { }

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<MagikeInterstitial3Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial3Page3>();
    }
}
