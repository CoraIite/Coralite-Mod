using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;

namespace Coralite.Content.Tiles.Icicle
{
    public class BabyIceDragonRelic: BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.IcicleTiles + "BabyIceDragonRelicPedestal";
        public override string RelicTextureName => AssetDirectory.IcicleTiles + Name;
    }
}
