using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class OtherEnchant_ItemScaleBonus : OtherBonusEnchant
    {
        private readonly float bonus;

        public OtherEnchant_ItemScaleBonus(Enchant.Level level, float bonus) : base(level)
        {
            this.bonus = bonus;
        }

        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            scale += bonus;
        }

        public override string Description => $"大小 +{(int)(bonus * 100)}%";
    }
}
