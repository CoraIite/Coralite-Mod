using Coralite.Content.Tiles.Plants;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class AgropyronSeed : BaseSeed
    {
        public AgropyronSeed() : base("寒霜冰草种子", "成熟后收获寒霜冰草", 9999, Item.sellPrice(0, 0, 0, 16), ItemRarityID.White, 15, 15, 0, 0, ModContent.TileType<AgropyronFrozen>()) { }
    }
}
