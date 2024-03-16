using Coralite.Core;
using Coralite.Core.Prefabs.Items;

namespace Coralite.Content.Items.RedJades
{
    public class BloodiancieRelic : BaseRelicItem
    {
        public BloodiancieRelic() : base(ModContent.TileType<Tiles.RedJades.BloodiancieRelicTile>(), AssetDirectory.RedJadeItems) { }
    }
}
