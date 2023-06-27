using Terraria.ModLoader;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class Enchant
    {
        public readonly EnchantData[] datas;

        public Enchant()
        {
            datas = new EnchantData[3];
        }
    }

    public abstract class EnchantData
    {
        /// <summary> 当前的词条等级 </summary>
        public int level;
        /// <summary> 这个附魔词条应该在哪个位置上 </summary>
        public readonly int whichSlot;

        public abstract float EnchantChance { get; }

        public string Description { get; }

        public EnchantData(int level, int whichSlot)
        {
            this.level = level;
            this.whichSlot = whichSlot;
        }

        public abstract void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage);
    }

    public abstract class BasicBonusEnchant : EnchantData
    {
        public BasicBonusEnchant(int level) : base(level, 0)
        { }



    }

    public abstract class OtherBonusEnchant : EnchantData
    {
        public OtherBonusEnchant(int level) : base(level, 1)
        { }
    }

    public abstract class SpecialEnchant : EnchantData
    {
        public SpecialEnchant(int level) : base(level, 2)
        { }
    }
}
