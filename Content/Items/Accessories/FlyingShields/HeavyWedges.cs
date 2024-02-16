using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class HeavyWedges : BaseAccessory, IFlyingShieldAccessory
    {
        public HeavyWedges() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 0, 50))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<FlyingShieldToolboxProMax>()//与工具箱冲突
                || equippedItem.type == ModContent.ItemType<FlyingShieldToolbox>()//与工具箱冲突
                || equippedItem.type == ModContent.ItemType<FlyingShieldVarnish>()//与工具箱冲突

                || equippedItem.type == ModContent.ItemType<FlyingShieldCore>())//上位

                && incomingItem.type == ModContent.ItemType<HeavyWedges>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.08f;
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.shootSpeed *= 0.65f;
            //projectile.backSpeed *= 0.65f;

            Projectile p = projectile.Projectile;

            p.scale *= 1.3f;
            Vector2 cneter = p.Center;
            p.width = (int)(p.width * p.scale);
            p.height = (int)(p.height * p.scale);
            p.Center = cneter;

            projectile.trailWidth = (int)(projectile.trailWidth * p.scale);
        }
    }
}
