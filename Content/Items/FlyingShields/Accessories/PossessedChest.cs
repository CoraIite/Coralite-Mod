using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class PossessedChest : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public PossessedChest() : base(ItemRarityID.LightRed, Item.sellPrice(0, 1))
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories.Add(this);
            }
        }

        public void OnStartDashing(BaseFlyingShieldGuard projectile)
        {
            if (projectile.Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldDashDamageReduce = 30;
            }
        }
    }
}
