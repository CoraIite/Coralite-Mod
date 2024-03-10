using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderCandleTile : BaseCandleTile<ThunderCandle>
    {
        public ThunderCandleTile() : base(DustID.YellowTorch, Coralite.Instance.ThunderveinYellow, AssetDirectory.ThunderTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.5f; g = 0.35f; b = 0.1f;
        }
    }
}
