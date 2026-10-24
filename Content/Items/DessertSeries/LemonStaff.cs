using Coralite.Content.GlobalItems;
using Coralite.Core;
using Coralite.Core.Systems.DotSystem;
using Coralite.Core.Systems.DotSystem.BasicDots;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.DessertSeries
{
    public class LemonStaff : ModItem
    {
        public override string Texture => AssetDirectory.DessertSeriesItem+Name;

        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<LemonSlice>(), 20, 10, true);
            Item.SetWeaponValues(12, 4);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.mana = 6;

            CoraliteGlobalItem.SetEdibleDamage(Item);
        }
    }

    public class LemonSlice : ModProjectile
    {
        public override string Texture => AssetDirectory.DessertSeriesItem + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Dot.AddDot<Lemonade>(target, damageDone, Projectile.owner, Projectile.CritChance);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(lightColor, 0);

            return false;
        }
    }

    public class Lemonade : BasicSour
    {
        public override string Texture => AssetDirectory.DefaultItem;
        public override int BaseMaxDotLayer => 20;
    }
}
