using Coralite.Content.Items.Magike.Filters.PolarizedFilters;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 羽毛
    /// </summary>
    public class EiderdownLevel : MagikeLevel
    {
        public override bool Available => true;

        public override float MagikeCostValue => 0.5f;

        public override int PolarizedFilterItemType => ModContent.ItemType<EiderdownPolarizedFilter>();
    }
}
