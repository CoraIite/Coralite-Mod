using Coralite.Content.Items.Materials;
using Terraria;
using Terraria.ID;

namespace Coralite.Core
{
    /// <summary>
    /// 专门用于添加合成表的类
    /// </summary>
    public class CoraliteRecipe : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe.Create(ItemID.Leather)
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddIngredient<MagicalPowder>()
                .Register();

            Recipe.Create(ItemID.Leather)
                .AddIngredient(ItemID.Bunny, 2)
                .AddIngredient<MagicalPowder>()
                .Register();
        }
    }
}
