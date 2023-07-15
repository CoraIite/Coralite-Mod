using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class OtherEnchant_ItemScaleBonus : OtherEnchant
    {
        public OtherEnchant_ItemScaleBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            scale += bonus0 / 100f;
        }

        public override string Description => $"大小 +{(int)bonus0}%";
    }
}
