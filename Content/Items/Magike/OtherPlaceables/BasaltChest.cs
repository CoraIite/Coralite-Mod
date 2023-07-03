using Coralite.Content.Raritys;
using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class BasaltChest : BaseChestItem
    {
        public BasaltChest() : base(Item.sellPrice(0,0,0,10),ModContent.RarityType<MagikeCrystalRarity>(),ModContent.TileType<BasaltChestTile>(),AssetDirectory.MagikeItems)
        { }
    }
}
