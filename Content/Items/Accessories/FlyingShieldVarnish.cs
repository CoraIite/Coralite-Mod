using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories
{
    public class FlyingShieldVarnish : BaseAccessory, IFlyingShieldAccessory
    {
        public FlyingShieldVarnish() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 10))
        { }

        public void PostInitialize(BaseFlyingShield projectile)
        {
            projectile.shootSpeed += 2.5f / (projectile.Projectile.extraUpdates+1);//加速
            projectile.backSpeed += 2.5f / (projectile.Projectile.extraUpdates+1);
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
    }
}
