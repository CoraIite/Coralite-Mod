using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class OtherEnchant_UseSpeedBonus : OtherBonusEnchant
    {
        public OtherEnchant_UseSpeedBonus(Enchant.Level level, float bonus) : base(level)
        {
            bonus0 = bonus;
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            return 1f + bonus0/100f;
        }

        public override string Description => $"攻速 +{(int)bonus0}%";
    }
}
