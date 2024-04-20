using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class FlyingShieldMaintenanceGuide : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public FlyingShieldMaintenanceGuide() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20))
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
            projectile.damageReduce *= 1.2f;
        }
    }
}
