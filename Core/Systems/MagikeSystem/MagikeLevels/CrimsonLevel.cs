using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 猩红
    /// </summary>
    public class CrimsonLevel : MagikeLevel
    {
        public override bool Available => NPC.downedBoss2 && WorldGen.crimson;

        public override float MagikeCostValue => 1.5f;

        public override int PolarizedFilterItemType => ModContent.ItemType<CrimsonPolarizedFilter>();
    }
}
