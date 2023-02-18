using Coralite.Content.Tiles.Plants;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class WatermelonSeed : BaseSeed
    {
        public WatermelonSeed() : base("西瓜种子", "", 9999, Item.sellPrice(0, 0, 0, 16), ItemRarityID.Blue, 10, 10, 0, 0, ModContent.TileType<WatermelonTile>()) { }
    }
}
