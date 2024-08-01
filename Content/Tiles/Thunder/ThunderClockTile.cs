using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderClockTile : BaseClockTile
    {
        public ThunderClockTile() : base(DustID.YellowTorch, Coralite.ThunderveinYellow, 5, new int[] { 16, 16, 16, 16, 16 }, AssetDirectory.ThunderTiles)
        {
        }
    }
}
