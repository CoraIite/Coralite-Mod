using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeChair : BaseChairItem
    {
        public RedJadeChair() : base(Item.sellPrice(0, 0, 10), ItemRarityID.White, ModContent.TileType<Tiles.RedJades.RedJadeChair>(), AssetDirectory.RedJadeItems) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(1)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
               .Register();
        }
    }
}
