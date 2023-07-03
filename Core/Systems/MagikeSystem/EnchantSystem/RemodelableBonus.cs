
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class SpecialEnchant_RemodelableBonus : SpecialEnchant
    {
        public SpecialEnchant_RemodelableBonus() : base(Enchant.Level.Max) { }

        public override string Description => "这个物品内充满了魔能，似乎能够通过重塑获得进化";
    }
}
