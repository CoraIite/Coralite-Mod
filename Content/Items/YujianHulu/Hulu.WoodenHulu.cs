using Coralite.Core.Systems.YujianSystem;
using Terraria.ID;

namespace Coralite.Content.Items.Yujian
{
    public class WoodenHulu : BaseHulu
    {
        public WoodenHulu() : base(1, ItemRarityID.White, 0,3,1f) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood,20)
                .Register();
        }
    }
}
