using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 腐化
    /// </summary>
    public class CorruptionLevel : MagikeLevel
    {
        public override bool Available => NPC.downedBoss2 && !WorldGen.crimson;

        public override float MagikeCostValue => 1.5f;

        public override int PolarizedFilterItemType => ModContent.ItemType<CorruptionPolarizedFilter>();

        public static ushort ID { get; private set; }

        public override void Load()
        {
            ID = Type;
        }
    }
}
