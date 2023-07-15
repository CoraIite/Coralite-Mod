
namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class SpecialEnchant_RemodelableBonus : SpecialEnchant
    {
        public SpecialEnchant_RemodelableBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override string Description => "这个物品内充满了魔能，似乎能够通过重塑获得进化";
    }
}
