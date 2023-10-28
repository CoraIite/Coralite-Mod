using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class OtherEnchant_ManaCostBonus : OtherEnchant
    {
        public OtherEnchant_ManaCostBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            reduce -= bonus0 / 100f;
        }

        public override string Description => $"魔力消耗 -{(int)bonus0}%";
    }

    public class OtherEnchant_ArmorManaCostBonus : OtherEnchant
    {
        public OtherEnchant_ArmorManaCostBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.manaCost -= bonus0 / 100f;
        }

        public override string Description => $"魔力消耗 -{(int)bonus0}%";
    }

    public class OtherEnchant_AccessoryManaCostBonus : OtherEnchant
    {
        public OtherEnchant_AccessoryManaCostBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.manaCost -= bonus0 / 100f;
        }

        public override string Description => $"魔力消耗 -{(int)bonus0}%";
    }
}
