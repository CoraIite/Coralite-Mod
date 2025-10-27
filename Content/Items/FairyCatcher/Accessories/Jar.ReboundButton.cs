using Coralite.Content.RecipeGroups;
using Coralite.Core.Systems.FairyCatcherSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Accessories
{
    public class ReboundButton() : BaseFairyAccessory(ItemRarityID.Blue, Item.sellPrice(0, 0, 20))
    {
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                fcp.AutoShootJar = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SilverWatch)
                .AddRecipeGroup(PressurePlateGroup.GroupName)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.TungstenWatch)
                .AddRecipeGroup(PressurePlateGroup.GroupName)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
