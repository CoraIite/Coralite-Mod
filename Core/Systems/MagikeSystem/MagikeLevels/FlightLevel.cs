using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 飞翔
    /// </summary>
    public class FlightLevel : MagikeLevel
    {
        public override bool Available => Main.hardMode;

        public override float MagikeCostValue => 7.5f;

        public override int PolarizedFilterItemType => ModContent.ItemType<FlightPolarizedFilter>();
    }
}
