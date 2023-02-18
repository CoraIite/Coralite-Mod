using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.RedJades
{
    public class RediancieRelic : BaseBossRelicTile
    {
        public RediancieRelic() : base(ModContent.ItemType<Items.RedJadeItems.RediancieRelic>()) { }

        public override string RelicTextureName => AssetDirectory.RedJadeTiles + Name;
    }
}
