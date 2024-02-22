using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class RemainsOfSamurai : BaseMaterial
    {
        public RemainsOfSamurai() : base(9999, Item.sellPrice(0, 0, 50), ItemRarityID.Orange, AssetDirectory.Materials) { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 150)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
