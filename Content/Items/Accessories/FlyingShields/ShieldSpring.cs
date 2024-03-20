using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class ShieldSpring : BaseAccessory, IFlyingShieldAccessory
    {
        public ShieldSpring() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 15, 0))
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnStartDashing(BaseFlyingShieldGuard projectile)
        {
            float speedAdder = projectile.dashSpeed * 0.1f;
            if (speedAdder > 2)
            {
                speedAdder = 2;
            }

            projectile.dashSpeed += speedAdder;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood,20)
                .AddIngredient(ItemID.CrimtaneBar, 5)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 20)
                .AddIngredient(ItemID.DemoniteBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
