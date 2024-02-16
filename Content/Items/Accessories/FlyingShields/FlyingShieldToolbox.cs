using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class FlyingShieldToolbox : BaseAccessory, IFlyingShieldAccessory
    {
        public FlyingShieldToolbox() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<FlyingShieldToolboxProMax>()//上位

                || equippedItem.type == ModContent.ItemType<FlyingShieldVarnish>()//素材
                || equippedItem.type == ModContent.ItemType<FlyingShieldMaintenanceGuide>()//素材
                || equippedItem.type == ModContent.ItemType<StretchGlue>()//素材

                || equippedItem.type == ModContent.ItemType<FlyingShieldCore>()//与重型冲突
                || equippedItem.type == ModContent.ItemType<HeavyWedges>())//与重型冲突

                && incomingItem.type == ModContent.ItemType<FlyingShieldToolbox>());
        }

        public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        {
            projectile.damageReduce *= 1.2f;
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.maxJump++;
        }

        public void PostInitialize(BaseFlyingShield projectile)
        {
            projectile.shootSpeed += 2.5f / (projectile.Projectile.extraUpdates + 1);//加速
            projectile.backSpeed += 4f / (projectile.Projectile.extraUpdates + 1);
            if (projectile.shootSpeed > projectile.Projectile.width / (projectile.Projectile.extraUpdates + 1))
            {
                projectile.Projectile.extraUpdates++;
                projectile.shootSpeed /= 2;
                projectile.backSpeed /= 2;
                projectile.flyingTime *= 2;
                projectile.backTime *= 2;
                projectile.trailCachesLength *= 2;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<StretchGlue>()
                .AddIngredient<FlyingShieldVarnish>()
                .AddIngredient<FlyingShieldMaintenanceGuide>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
