using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class SpecialEnchant_ArmorMoveSpeedBonus : SpecialEnchant
    {
        public SpecialEnchant_ArmorMoveSpeedBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.moveSpeed += bonus0 / 100f;
        }

        public override string Description => $"移动速度 +{(int)bonus0}%";
    }

    public class SpecialEnchant_AccessoryMoveSpeedBonus : SpecialEnchant
    {
        public SpecialEnchant_AccessoryMoveSpeedBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.moveSpeed += bonus0 / 100f;
        }

        public override string Description => $"移动速度 +{(int)bonus0}%";
    }
}
