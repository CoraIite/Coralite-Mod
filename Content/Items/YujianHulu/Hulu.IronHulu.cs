using Coralite.Core.Systems.YujianSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.YujianHulu
{
    public class IronHulu : BaseHulu
    {
        public IronHulu() : base(ItemRarityID.White, Item.sellPrice(0, 0, 5, 0), 7, 1.5f) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
