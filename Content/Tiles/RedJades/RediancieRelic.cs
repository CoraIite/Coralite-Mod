using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;

namespace Coralite.Content.Tiles.RedJades
{
    public class RediancieRelic : BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.RedJadeTiles + "RediancieRelicPedestal";
        public override string RelicTextureName => AssetDirectory.RedJadeTiles + Name;
    }
}
