using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 蜂蜡
    /// </summary>
    public class BeeswaxLevel : MagikeLevel
    {
        public override bool Available => NPC.downedQueenBee;

        public override float MagikeCostValue => 2f;

        public override int PolarizedFilterItemType => ModContent.ItemType<BeeswaxPolarizedFilter>();

        public override Color LevelColor => Color.Honeydew;

        public static ushort ID { get; private set; }

        public override void SetStaticDefaults()
        {
            ID = Type;
        }
    }
}
