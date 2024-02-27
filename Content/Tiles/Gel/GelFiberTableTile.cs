using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.Gel
{
    public class GelFiberTableTile : ModTile
    {
        public override string Texture => AssetDirectory.GelTiles + Name;

        public override void SetStaticDefaults()
        {
            this.TablePrefab(DustID.Water, new Microsoft.Xna.Framework.Color(0, 138, 122));
        }
    }
}
