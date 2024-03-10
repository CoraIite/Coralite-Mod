using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderSofaTile : BaseSofaTile<ThunderSofa>
    {
        public ThunderSofaTile() : base(DustID.YellowTorch, Coralite.Instance.ThunderveinYellow, AssetDirectory.ThunderTiles)
        {
        }
    }
}
