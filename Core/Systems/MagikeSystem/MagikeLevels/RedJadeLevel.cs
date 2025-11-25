using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Core.Systems.BossSystems;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 赤玉
    /// </summary>
    public class RedJadeLevel : MagikeLevel
    {
        public override bool Available => ModContent.GetInstance<DownedRediancie>().Value;

        public override float MagikeCostValue => 0.5f;

        public override int PolarizedFilterItemType => ModContent.ItemType<RedJadePolarizedFilter>();

        public static ushort ID { get; private set; }

        public override void Load()
        {
            ID = Type;
        }
    }
}
