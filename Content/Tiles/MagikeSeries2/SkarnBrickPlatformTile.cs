using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class SkarnBrickPlatformTile : BasePlatformTile
    {
        public SkarnBrickPlatformTile() : base(ModContent.DustType<SkarnDust>(), new Color(141, 171, 178), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }
}
