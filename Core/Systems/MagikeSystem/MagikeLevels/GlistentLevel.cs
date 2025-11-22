using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 晶莹
    /// </summary>
    public class GlistentLevel : MagikeLevel
    {
        public override bool Available => NPC.downedBoss1;

        public override float MagikeCostValue => 1f;

        public override int PolarizedFilterItemType => ModContent.ItemType<GlistentPolarizedFilter>();
    }
}
