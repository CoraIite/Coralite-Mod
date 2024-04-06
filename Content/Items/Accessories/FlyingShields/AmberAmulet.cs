using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class AmberAmulet : BaseAccessory, IFlyingShieldAccessory
    {
        public AmberAmulet() : base(ItemRarityID.Orange, Item.sellPrice(0, 0, 30))
        { }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        {
            projectile.strongGuard += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Amber, 3)
                .AddIngredient(ItemID.FossilOre, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
