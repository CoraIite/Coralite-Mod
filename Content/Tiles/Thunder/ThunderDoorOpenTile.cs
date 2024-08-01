using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderDoorOpenTile : BaseDoorOpenTile<ThunderDoor, ThunderDoorClosedTile>
    {
        public ThunderDoorOpenTile() : base(DustID.YellowTorch, Coralite.ThunderveinYellow, AssetDirectory.ThunderTiles)
        {
        }
    }
}
