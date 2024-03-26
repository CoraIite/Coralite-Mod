using Coralite.Content.Tiles.Thunder;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public class TheGazeOfTheThundervein : BaseThunderFurniture<TheGazeOfTheThunderveinTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZapCrystal>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
