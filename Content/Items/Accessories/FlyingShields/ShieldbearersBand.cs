using Coralite.Content.Items.Shadow;
using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class ShieldbearersBand : BaseAccessory
    {
        public ShieldbearersBand() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20))
        { }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.MaxFlyingShield++;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Leather)
                .AddIngredient(ItemID.TissueSample, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
