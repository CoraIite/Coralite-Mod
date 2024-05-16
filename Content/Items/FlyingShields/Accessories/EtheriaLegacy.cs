using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class EtheriaLegacy : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public EtheriaLegacy() : base(ItemRarityID.Pink, Item.sellPrice(0, 2)) { }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.FlyingShieldAccessories.Add(this);
        }

        public void OnParryEffect(BaseFlyingShieldGuard projectile)
        {
            projectile.Owner.AddBuff(BuffID.ParryDamageBuff, 60 * 5);
        }
    }
}
