using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class FlyingShieldMaintenanceGuide : BaseAccessory, IFlyingShieldAccessory
    {
        public FlyingShieldMaintenanceGuide() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20))
        { }

        public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        {
            projectile.damageReduce *= 1.2f;
        }
    }
}
