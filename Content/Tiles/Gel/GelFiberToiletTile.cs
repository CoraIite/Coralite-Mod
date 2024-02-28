using Coralite.Content.Items.Gels;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Gel
{
    public class GelFiberToiletTile : BaseToiletTile<GelFiberToilet>
    {
        public GelFiberToiletTile() : base(DustID.Water, new Microsoft.Xna.Framework.Color(0, 138, 122), AssetDirectory.GelTiles)
        {
        }
    }
}
