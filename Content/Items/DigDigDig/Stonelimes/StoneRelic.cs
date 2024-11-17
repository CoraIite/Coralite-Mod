using Coralite.Content.Tiles.DigDigDig;
using Coralite.Core.Prefabs.Items;

namespace Coralite.Content.Items.DigDigDig.Stonelimes
{
    public class StoneRelic : BaseRelicItem
    {
        public StoneRelic() : base(ModContent.TileType<StoneRelicTile>(), "Terraria/Images/Item_3", true) { }
    }
}
