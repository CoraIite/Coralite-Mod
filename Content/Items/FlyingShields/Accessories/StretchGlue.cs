using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class StretchGlue : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public StretchGlue() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 10))
        { }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.maxJump++;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bottle)
                .AddIngredient(ItemID.PinkGel, 5)
                .Register();
        }
    }
}
