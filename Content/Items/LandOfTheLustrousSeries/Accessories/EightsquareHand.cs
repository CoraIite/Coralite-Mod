using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter;
using Coralite.Content.Items.Steel;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries.Accessories
{
    [PlayerEffect]
    public class EightsquareHand() : BaseAccessory(ItemRarityID.Orange, Item.sellPrice(0, 1)),IConsultableItem
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<LandOfTheLustrousKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<LandOfTheLustrousPage2>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(EightsquareHand));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(12)
                .AddIngredient(ItemID.Granite, 8)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient<B9Alloy>(12)
                .AddIngredient(ItemID.Granite, 8)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();
        }
    }
}
