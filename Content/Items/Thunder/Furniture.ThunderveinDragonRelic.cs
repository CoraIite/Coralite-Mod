using Coralite.Core;
using Coralite.Core.Prefabs.Items;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderveinDragonRelic : BaseRelicItem
    {
        public ThunderveinDragonRelic() : base(ModContent.TileType<Tiles.Thunder.ThunderveinDragonRelic>(), AssetDirectory.ThunderItems) { }
    }
}
