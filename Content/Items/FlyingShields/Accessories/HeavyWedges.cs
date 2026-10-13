using Coralite.Content.CoraliteNotes.FlyingShieldChapter;
using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FlyingShieldSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class HeavyWedges : BaseFlyingShieldAccessory<FlyingShieldAccessoryPage1>, IFlyingShieldAccessory
    {
        public HeavyWedges() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 0, 50))
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 1;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<FlyingShieldCore>()//上位
                || equippedItem.type == ModContent.ItemType<FlyingShieldTerminalChip>())//上位

                && incomingItem.type == ModContent.ItemType<HeavyWedges>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.08f;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            //projectile.shootSpeed *= 1.05f;
            //projectile.backSpeed *= 1.05f;
            projectile.maxPuncture += 2;

            Projectile p = projectile.Projectile;

            p.scale *= 1.3f;
            p.Resize((int)(p.width * p.scale), (int)(p.height * p.scale));

            projectile.trailWidth = (int)(projectile.trailWidth * p.scale);
        }
    }
}
