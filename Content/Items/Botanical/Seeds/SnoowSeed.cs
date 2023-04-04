using Coralite.Content.Tiles.Plants;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Botanical.Seeds
{
    public class SnoowSeed : BaseSeed
    {
        public SnoowSeed() : base("雪融花种子", "成熟后能掉出雪块", 9999, Item.sellPrice(0, 0, 0, 8), ItemRarityID.Blue, 6, 6, 0, 0, ModContent.TileType<SnoowFlower>()) { }
    }
}
