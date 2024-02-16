using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class FloralHarmonyMedallion : BaseAccessory, IFlyingShieldAccessory
    {
        public FloralHarmonyMedallion() : base(ItemRarityID.Yellow, Item.sellPrice(0, 2, 0))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !(equippedItem.type == ModContent.ItemType<LevitationBlossom>()
                && incomingItem.type == ModContent.ItemType<FloralHarmonyMedallion>());
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
