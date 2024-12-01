using Coralite.Content.Tiles.DigDigDig;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;

namespace Coralite.Content.Items.DigDigDig.EyeOfGlistent
{
    public class GlistentRelic : BaseRelicItem
    {
        public GlistentRelic() : base(ModContent.TileType<GlistentRelicTile>(), AssetDirectory.GlistentItems+ "GlistentBarFlip", true) { }
    }
}
