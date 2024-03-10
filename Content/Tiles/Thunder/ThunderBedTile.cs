using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderBedTile : BaseBedTile<ThunderBed>
    {
        public ThunderBedTile() : base(DustID.YellowTorch, Coralite.Instance.ThunderveinYellow, AssetDirectory.ThunderTiles)
        {
        }
    }
}
