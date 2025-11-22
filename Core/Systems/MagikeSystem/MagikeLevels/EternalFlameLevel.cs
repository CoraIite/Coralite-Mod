using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 薪火
    /// </summary>
    public class EternalFlameLevel : MagikeLevel
    {
        public override bool Available => Main.hardMode;

        public override float MagikeCostValue => 10;

        public override int PolarizedFilterItemType => ModContent.ItemType<EternalFlamePolarizedFilter>();
    }
}
