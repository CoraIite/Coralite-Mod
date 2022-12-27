using Coralite.Content.Tiles.Plants;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class TestSeed : BaseSeed
    {
        public TestSeed() : base("测试种子", "", 999, Item.sellPrice(0, 0, 0, 1), ItemRarityID.Blue, 1, 1,1,1,ModContent.TileType<TestPlant>(), null,AssetDirectory.DefaultItem, true) { }
    }
}
