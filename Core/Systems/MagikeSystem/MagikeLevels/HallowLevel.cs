using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 神圣
    /// </summary>
    public class HallowLevel : MagikeLevel
    {
        public override bool Available => NPC.downedMechBossAny;

        public override float MagikeCostValue => 10f;

        public override int PolarizedFilterItemType => ModContent.ItemType<HallowPolarizedFilter>();
    }
}
