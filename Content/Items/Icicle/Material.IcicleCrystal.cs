using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleCrystal : BaseMaterial
    {
        public IcicleCrystal() : base(9999, Item.sellPrice(0, 0, 50, 0), ItemRarityID.Orange, AssetDirectory.IcicleItems) { }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.IceMachine);
            recipe.AddIngredient<IcicleCrystal>(2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.IceRod);
            recipe.AddIngredient<IcicleCrystal>(2);
            recipe.AddTile(TileID.IceMachine);
            recipe.Register();
        }
    }
}
