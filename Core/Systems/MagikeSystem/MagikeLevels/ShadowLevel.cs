using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 影子
    /// </summary>
    public class ShadowLevel : MagikeLevel
    {
        public override bool Available => NPC.downedBoss3;

        public override float MagikeCostValue => 2f;

        public override int PolarizedFilterItemType => ModContent.ItemType<ShadowPolarizedFilter>();
    }
}
