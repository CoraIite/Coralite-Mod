using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJadeItems
{
    public class RediancieRelic : BaseRelicItem
    {
        public RediancieRelic() : base(ModContent.TileType<Tiles.RedJades.RediancieRelic>(), AssetDirectory.RedJadeItems) { }
    }
}
