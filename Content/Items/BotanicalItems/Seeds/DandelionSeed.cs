using Coralite.Content.Tiles.Plants;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class DandelionSeed : BaseSeed
    {
        public DandelionSeed() : base("蒲公英种子", "”世纪小花“", 999, Item.sellPrice(0, 0, 0, 3), ItemRarityID.Blue, 8, 8, 0, 0, ModContent.TileType<Dandelion>()) { }
    }
}
