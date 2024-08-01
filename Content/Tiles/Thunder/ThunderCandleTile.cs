using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderCandleTile : BaseCandleTile<ThunderCandle>
    {
        public ThunderCandleTile() : base(DustID.YellowTorch, Coralite.ThunderveinYellow, AssetDirectory.ThunderTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.8f; g = 0.55f; b = 0.2f;
        }
    }
}
