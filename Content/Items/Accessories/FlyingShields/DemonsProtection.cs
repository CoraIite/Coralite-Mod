using Coralite.Content.Items.Shadow;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class DemonsProtection : BaseAccessory, IFlyingShieldAccessory
    {
        public DemonsProtection() : base(ItemRarityID.Orange, Item.sellPrice(0, 0, 20))
        { }

        public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        {
            projectile.StrongGuard += 0.15f;
            projectile.damageReduce *= 1.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GuideVoodooDoll)
                .AddIngredient(ItemID.Bone, 20)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.GuideVoodooDoll)
                .AddIngredient<ShadowCrystal>(8)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
