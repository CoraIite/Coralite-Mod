using Coralite.Content.Tiles.Plants;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class WaterDrinkerSeed : BaseSeed
    {
        public WaterDrinkerSeed() : base("饮水棘种子", "它强大的根茎甚至能在沙漠中汲取水分\n不是饮水机", 999, Item.sellPrice(0, 0, 0, 5), ItemRarityID.Blue, 18, 18, 0, 0, ModContent.TileType<WaterDrinker>()) { }
    }
}
