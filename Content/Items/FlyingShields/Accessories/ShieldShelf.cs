using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.FlyingShieldChapter;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class ShieldShelf : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.FlyingShieldAccessories + Name;

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<FlyingShieldKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<FlyingShieldAccessoryPage4>();

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.ShieldAbility_GuardShelf = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 20)
                .AddRecipeGroup(RecipeGroupID.IronBar, 5)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}
