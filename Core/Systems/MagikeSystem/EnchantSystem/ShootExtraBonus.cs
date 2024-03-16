using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class SpecialEnchant_ShootExtraBonus : SpecialEnchant
    {
        public SpecialEnchant_ShootExtraBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI && player.ItemAnimationJustStarted)
            {
                Projectile p = Projectile.NewProjectileDirect(source, position, (Main.MouseWorld - player.Center).SafeNormalize(velocity) * 12, (int)bonus0, (int)(damage * bonus1), knockback, player.whoAmI);
                p.friendly = true;
                p.hostile = false;
            }
        }

        public override string Description => $"射出额外射弹，伤害系数{bonus1}";
    }
}
