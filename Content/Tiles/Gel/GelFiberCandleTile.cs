using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.Gel
{
    public class GelFiberCandleTile : ModTile
    {
        public override string Texture => AssetDirectory.GelTiles + Name;

        public override void SetStaticDefaults()
        {
            this.NormalCandlePrefab(DustID.Water, new Microsoft.Xna.Framework.Color(0, 138, 122));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.3f;
            g = 0.4f;
            b = 0.6f;
        }
    }
}
