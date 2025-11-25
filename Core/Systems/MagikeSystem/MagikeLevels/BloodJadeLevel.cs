using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Core.Systems.BossSystems;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 赤血玉
    /// </summary>
    public class BloodJadeLevel : MagikeLevel
    {
        public override bool Available => ModContent.GetInstance<DownedBloodiancie>().Value;

        public override float MagikeCostValue => 10;

        public override int PolarizedFilterItemType => ModContent.ItemType<BloodJadePolarizedFilter>();

        public static ushort ID { get; private set; }

        public override void Load()
        {
            ID = Type;
        }
    }
}
