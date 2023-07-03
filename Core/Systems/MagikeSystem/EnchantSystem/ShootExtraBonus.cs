using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class SpecialEnchant_ShootExtraBonus : SpecialEnchant
    {
        public SpecialEnchant_ShootExtraBonus(Enchant.Level level,int projType,float damageMult) : base(level)
        {
            bonus0 = projType;
            bonus1 = damageMult;
        }

        public override void Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer==player.whoAmI&&player.ItemAnimationJustStarted)
            {
                Projectile.NewProjectile(source, position, velocity, (int)bonus0, (int)(damage * bonus1), knockback, player.whoAmI);
            }
        }

        public override string Description => $"射出额外射弹，伤害系数{bonus1}";
    }
}
