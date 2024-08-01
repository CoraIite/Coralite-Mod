using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderChairTile : BaseChairTile<ThunderChair>
    {
        public ThunderChairTile() : base(DustID.YellowTorch, Coralite.ThunderveinYellow, AssetDirectory.ThunderTiles)
        {
        }
    }
}
