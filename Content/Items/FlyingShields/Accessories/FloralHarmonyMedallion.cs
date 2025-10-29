using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class FloralHarmonyMedallion : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public FloralHarmonyMedallion() : base(ItemRarityID.Yellow, Item.sellPrice(0, 2, 0))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return Helpers.Helper.CanBeEquipedWith<FloralHarmonyMedallion>(equippedItem, incomingItem
                , ModContent.ItemType<LevitationBlossom>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.flyingTime = (int)(projectile.flyingTime * 1.5f);
            projectile.canChase = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChlorophyteMedal>()
                .AddIngredient<LevitationBlossom>()
                .AddIngredient(ItemID.BeetleHusk, 4)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
