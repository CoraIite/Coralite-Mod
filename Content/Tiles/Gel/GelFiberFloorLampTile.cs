using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Gel
{
    public class GelFiberFloorLampTile : ModTile
    {
        public override string Texture => AssetDirectory.GelTiles + Name;

        public override void SetStaticDefaults()
        {
            this.FloorLampPrefab(2, 2, new int[2] { 16, 16 }, DustID.Water, new Microsoft.Xna.Framework.Color(0, 138, 122));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.5f;
            g = 0.7f;
            b = 0.9f;
        }
    }
}
