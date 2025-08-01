using Coralite.Content.Items.Glistent;
using Coralite.Core.Systems.FairyCatcherSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Accessories
{
    public class LeafScope() : BaseFairyAccessory(ItemRarityID.Blue, Item.sellPrice(0, 0, 20))
    {
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                fcp.DrawJarAimLine = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
