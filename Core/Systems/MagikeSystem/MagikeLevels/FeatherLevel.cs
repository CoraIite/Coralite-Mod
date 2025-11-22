using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 羽蛇
    /// </summary>
    public class FeatherLevel : MagikeLevel
    {
        public override bool Available => NPC.downedPlantBoss;

        public override float MagikeCostValue => 14f;

        public override int PolarizedFilterItemType => ModContent.ItemType<FeatherPolarizedFilter>();
    }
}
