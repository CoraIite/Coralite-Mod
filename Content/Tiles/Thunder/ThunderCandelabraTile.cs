using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderCandelabraTile : BaseCandelabraTile<ThunderCandelabra>
    {
        public ThunderCandelabraTile() : base(DustID.YellowTorch, Coralite.Instance.ThunderveinYellow, AssetDirectory.ThunderTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1.2f; g = 0.85f; b = 0.35f;
        }
    }
}
