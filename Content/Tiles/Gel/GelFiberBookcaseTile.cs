using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Gel
{
    public class GelFiberBookcaseTile : ModTile
    {
        public override string Texture => AssetDirectory.GelTiles + Name;

        public override void SetStaticDefaults()
        {
            this.BookcasePrefab(DustID.Water, new Microsoft.Xna.Framework.Color(0, 138, 122));
        }
    }
}
