using Coralite.Content.Tiles.Plants;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class TreeJokeSeed : BaseSeed
    {
        public TreeJokeSeed() : base("树生树树果", "”你没搞错吧？“", 999, Item.sellPrice(0, 0, 0, 1), ItemRarityID.White, 5, 5, 0, 0, ModContent.TileType<TreeJoke>()) { }
    }
}
