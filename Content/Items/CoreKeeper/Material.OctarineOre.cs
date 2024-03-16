using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.CoreKeeper
{
    public class OctarineOre : BaseMaterial
    {
        public OctarineOre() : base(999, Item.sellPrice(0, 0, 5)
            , ModContent.RarityType<EpicRarity>(), AssetDirectory.CoreKeeperItems)
        { }
    }
}
