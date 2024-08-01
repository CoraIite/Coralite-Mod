using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Thunder
{
    public class TheGazeOfTheThunderveinTile : ModTile
    {
        public override string Texture => AssetDirectory.ThunderTiles + Name;

        public override void SetStaticDefaults()
        {
            this.PaintingPrefab(3, 2, Coralite.ThunderveinYellow, DustID.YellowTorch);
        }

    }
}
