using Coralite.Content.Items.Gels;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Gel
{
    public class GelFiberDoorClosedTile : BaseDoorClosedTile<GelFiberDoor, GelFiberDoorOpen>
    {
        public GelFiberDoorClosedTile() : base(DustID.Water, new Microsoft.Xna.Framework.Color(0, 138, 122), AssetDirectory.GelTiles)
        {
        }
    }

    public class GelFiberDoorOpen : BaseDoorOpenTile<GelFiberDoor, GelFiberDoorClosedTile>
    {
        public GelFiberDoorOpen() : base(DustID.Water, new Microsoft.Xna.Framework.Color(0, 138, 122), AssetDirectory.GelTiles)
        {
        }
    }
}
