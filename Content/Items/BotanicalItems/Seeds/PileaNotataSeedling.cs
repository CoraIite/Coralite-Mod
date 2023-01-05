using Coralite.Content.Tiles.Plants;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class PileaNotataSeedling : BaseSeed
    {
        public PileaNotataSeedling() : base("冷水花幼苗", "”成熟后收获冷水花“", 999, Item.sellPrice(0, 0, 0, 3), ItemRarityID.White, 15, 15, 0, 0, ModContent.TileType<PileaNotata>()) { }
    }
}
