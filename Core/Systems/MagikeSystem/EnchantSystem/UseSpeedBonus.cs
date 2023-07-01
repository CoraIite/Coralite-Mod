using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class OtherEnchant_UseSpeedBonus : OtherBonusEnchant
    {
        private readonly float bonus;

        public OtherEnchant_UseSpeedBonus(Enchant.Level level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            return 1f + bonus;
        }

        public override string Description => $"攻速 +{(int)(bonus * 100)}%";
    }
}
