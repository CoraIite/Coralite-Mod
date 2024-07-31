using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Glistent
{
    public class LeafStone : BaseMaterial  //, IMagikeRemodelable
    {
        public LeafStone() : base(9999, Item.sellPrice(0, 0, 0, 50), ItemRarityID.Green, AssetDirectory.GlistentItems) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 5)
                .AddTile(TileID.Furnaces)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Vine)
                .AddTile(TileID.Furnaces)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.VineRope, 5)
                .AddTile(TileID.Furnaces)
                .Register();
        }

        //public void AddMagikeRemodelRecipe()
        //{
        //    MagikeSystem.AddRemodelRecipe<LeafStone>(0f, ItemID.Wood, 20, selfStack: 15);
        //}
    }
}
