using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkarnChest : BaseChestItem
    {
        public SkarnChest() : base(Item.sellPrice(0, 0, 0, 10), ModContent.RarityType<CrystallineMagikeRarity>(), ModContent.TileType<SkarnChestTile>(), AssetDirectory.MagikeSeries2Item)
        { }
    }
}
