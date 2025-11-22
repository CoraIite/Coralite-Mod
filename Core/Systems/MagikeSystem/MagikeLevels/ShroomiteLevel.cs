using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 蘑菇矿锭
    /// </summary>
    public class ShroomiteLevel : MagikeLevel
    {
        public override bool Available => NPC.downedPlantBoss;

        public override float MagikeCostValue => 14;

        public override int PolarizedFilterItemType => ModContent.ItemType<ShroomitePolarizedFilter>();
    }
}
