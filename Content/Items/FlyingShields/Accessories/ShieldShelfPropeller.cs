using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.FlyingShieldChapter;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class ShieldShelfPropeller : ModItem,IConsultableItem
    {
        public override string Texture => AssetDirectory.FlyingShieldAccessories + Name;

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<FlyingShieldKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<FlyingShieldAccessoryPage4>();

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.ShieldAbility_GuardShelf = true;
                cp.ShieldAbility_GuardShelfPropeller = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShieldShelf>()
                .AddIngredient(ItemID.Bone,4)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}
