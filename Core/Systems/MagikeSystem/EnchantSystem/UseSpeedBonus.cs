using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class OtherEnchant_UseSpeedBonus : OtherEnchant
    {
        public OtherEnchant_UseSpeedBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            return 1f + bonus0/100f;
        }

        public override string Description => $"攻速 +{(int)bonus0}%";
    }
}
