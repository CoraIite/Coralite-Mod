using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderWorkBenchTile : BaseWorkBenchTile
    {
        public ThunderWorkBenchTile() : base(DustID.YellowTorch, Coralite.Instance.ThunderveinYellow, AssetDirectory.ThunderTiles)
        {
        }
    }
}
