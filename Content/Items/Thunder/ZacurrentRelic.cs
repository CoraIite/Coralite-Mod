using Coralite.Content.Tiles.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;

namespace Coralite.Content.Items.Thunder
{
    public class ZacurrentRelic : BaseRelicItem
    {
        public ZacurrentRelic() : base(ModContent.TileType<ZacurrentRelicTile>(), AssetDirectory.ThunderItems) { }
    }
}
