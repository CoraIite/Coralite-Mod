using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJadeItems
{
    public class RedJadeBed : BaseBedItem
    {
        public RedJadeBed() : base(Item.sellPrice(0, 0, 10), ItemRarityID.White, ModContent.TileType<Tiles.RedJades.RedJadeBed>(), AssetDirectory.RedJadeItems) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(6)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
               .Register();
        }
    }
}
