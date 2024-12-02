using Coralite.Content.Items.Steel;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries.Accessories
{
    public class EightsquareHand() : BaseAccessory(ItemRarityID.Orange, Item.sellPrice(0, 1))
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

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
        }
    }
}
