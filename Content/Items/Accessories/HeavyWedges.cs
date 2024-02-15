using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Accessories
{
    public class HeavyWedges : BaseAccessory, IFlyingShieldAccessory
    {
        public HeavyWedges() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 0, 50))
        { }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.05f;
        }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.shootSpeed *= 0.65f;
            projectile.backSpeed *= 0.65f;

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
