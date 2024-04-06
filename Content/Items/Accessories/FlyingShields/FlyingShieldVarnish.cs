using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class FlyingShieldVarnish : BaseAccessory, IFlyingShieldAccessory
    {
        public FlyingShieldVarnish() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 10))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<FlyingShieldToolboxProMax>()//上位
                || equippedItem.type == ModContent.ItemType<FlyingShieldToolbox>()//上位
                || equippedItem.type == ModContent.ItemType<NanoAmplifier>()//上位

                || equippedItem.type == ModContent.ItemType<FlyingShieldCore>()//与重型冲突
                || equippedItem.type == ModContent.ItemType<FlyingShieldTerminalChip>()//与重型冲突
                || equippedItem.type == ModContent.ItemType<HeavyWedges>())//与重型冲突

                && incomingItem.type == ModContent.ItemType<FlyingShieldVarnish>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }
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
    }
}
