using Coralite.Content.Items.Magike.Filters.PolarizedFilters;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 无等级
    /// </summary>
    public class NoneLevel : MagikeLevel
    {
        public override bool Available => true;

        public override float MagikeCostValue => 0;

        public override int PolarizedFilterItemType => ModContent.ItemType<NonePolarizedFilter>();

        public override Color LevelColor => Color.DarkGray;

        /// <summary>
        /// 无等级的ID
        /// </summary>
        public static ushort ID { get; private set; }

        public override void SetStaticDefaults()
        {
            ID = Type;
        }
    }
}
