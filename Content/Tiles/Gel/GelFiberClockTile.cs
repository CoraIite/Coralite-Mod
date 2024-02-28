using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Gel
{
    public class GelFiberClockTile : BaseClockTile
    {
        public GelFiberClockTile() : base(DustID.Water, new Microsoft.Xna.Framework.Color(0, 138, 122)
            , 6, new int[] { 16, 16, 16, 16, 16, 16 }, AssetDirectory.GelTiles)
        {
        }
    }
}
