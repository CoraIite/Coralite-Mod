using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class ChlorophyteMedal : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public ChlorophyteMedal() : base(ItemRarityID.Lime, Item.sellPrice(0, 1, 50))
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
            projectile.canChase = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
