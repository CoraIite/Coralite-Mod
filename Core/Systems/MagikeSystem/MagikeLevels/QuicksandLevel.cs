using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 赤玉
    /// </summary>
    public class QuicksandLevel : MagikeLevel
    {
        public override bool Available => NPC.downedBoss3;

        public override float MagikeCostValue => 2.5f;

        public override int PolarizedFilterItemType => ModContent.ItemType<QuicksandPolarizedFilter>();

        public static ushort ID { get; private set; }

        public override void Load()
        {
            ID = Type;
        }
    }
}
