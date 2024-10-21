using Coralite.Content.Tiles.DigDigDig;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.DigDigDig.Stonelimes
{
    public class StoneMakerCore() : BasePlaceableItem(Item.sellPrice(copper: 50), ItemRarityID.Blue, ModContent.TileType<StoneMakerCoreTile>(), AssetDirectory.MiscItems)
    {
    }
}
