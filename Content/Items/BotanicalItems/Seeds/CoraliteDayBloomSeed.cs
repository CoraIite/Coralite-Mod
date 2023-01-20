using Coralite.Content.Tiles.Plants;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class CoraliteDayBloomSeed : BaseSeed
    {
        public CoraliteDayBloomSeed() : base("太阳花种子", "", 9999, Item.sellPrice(0, 0, 0, 16), ItemRarityID.White, 8, 8, 0, 0, ModContent.TileType<CoraliteDaybloom>()) { }
    }
}
