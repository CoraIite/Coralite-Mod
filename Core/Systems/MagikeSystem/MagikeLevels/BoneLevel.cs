using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 骨头
    /// </summary>
    public class BoneLevel : MagikeLevel
    {
        public override bool Available => NPC.downedQueenBee;

        public override float MagikeCostValue => 2f;

        public override int PolarizedFilterItemType => ModContent.ItemType<BonePolarizedFilter>();
    }
}
