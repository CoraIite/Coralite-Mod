using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class SpecialEnchant_ArmorMoveSpeedBonus:OtherBonusEnchant
    {
        public SpecialEnchant_ArmorMoveSpeedBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.moveSpeed += bonus0 / 100f;
        }

        public override string Description => $"移动速度 +{(int)bonus0}%";
    }

    public class SpecialEnchant_AccessoryMoveSpeedBonus : OtherBonusEnchant
    {
        public SpecialEnchant_AccessoryMoveSpeedBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.moveSpeed += bonus0 / 100f;
        }

        public override string Description => $"移动速度 +{(int)bonus0}%";
    }
}
