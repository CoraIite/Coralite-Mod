using Coralite.Content.Items.Gels;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Gel
{
    public class GelFiberSofaTile : BaseSofaTile<GelFiberSofa>
    {
        public GelFiberSofaTile() : base(DustID.Water, new Microsoft.Xna.Framework.Color(0, 138, 122), AssetDirectory.GelTiles)
        {
        }
    }
}
