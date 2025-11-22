using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 灵魂
    /// </summary>
    public class SoulLevel : MagikeLevel
    {
        public override bool Available => NPC.downedPlantBoss;

        public override float MagikeCostValue => 14f;

        public override int PolarizedFilterItemType => ModContent.ItemType<SoulPolarizedFilter>();
    }
}
