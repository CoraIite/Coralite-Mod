using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class BasicEnchant_ArmorDefenceBonus:BasicBonusEnchant
    {
        public BasicEnchant_ArmorDefenceBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.statDefense += (int)bonus0;
        }

        public override string Description => $"防御 +{(int)bonus0}%";

    }

    public class SpecialEnchant_ArmorDefenceBonus : BasicBonusEnchant
    {
        public SpecialEnchant_ArmorDefenceBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.statDefense += (int)bonus0;
        }

        public override string Description => $"防御 +{(int)bonus0}%";

    }

    public class BasicEnchant_AccessoryDefenceBonus : BasicBonusEnchant
    {
        public BasicEnchant_AccessoryDefenceBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.statDefense += (int)bonus0;
        }

        public override string Description => $"防御 +{(int)bonus0}%";
    }

    public class SpecialEnchant_AccessoryDefenceBonus : BasicBonusEnchant
    {
        public SpecialEnchant_AccessoryDefenceBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.statDefense += (int)bonus0;
        }

        public override string Description => $"防御 +{(int)bonus0}%";
    }

}
