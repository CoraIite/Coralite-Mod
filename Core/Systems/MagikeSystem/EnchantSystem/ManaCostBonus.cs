using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class OtherEnchant_ManaCostBonus : OtherBonusEnchant
    {
        private readonly float bonus;

        public OtherEnchant_ManaCostBonus(Enchant.Level level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            reduce -= bonus;
        }

        public override string Description => $"魔力消耗 -{(int)(bonus * 100)}%";
    }
}
