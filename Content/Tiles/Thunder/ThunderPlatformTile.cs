using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderPlatformTile : BasePlatformTile
    {
        public ThunderPlatformTile() : base(DustID.YellowTorch, Coralite.Instance.ThunderveinYellow, AssetDirectory.ThunderTiles)
        {
        }
    }
}
