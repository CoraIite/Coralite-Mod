using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Core.Systems.BossSystems;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 帝凝胶
    /// </summary>
    public class EmperorLevel : MagikeLevel
    {
        public override bool Available => ModContent.GetInstance<DownedSlimeEmperor>().Value;

        public override float MagikeCostValue => 1.5f;

        public override int PolarizedFilterItemType => ModContent.ItemType<EmperorPolarizedFilter>();

        public static ushort ID { get; private set; }

        public override void Load()
        {
            ID = Type;
        }
    }
}
