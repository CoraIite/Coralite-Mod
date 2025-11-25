using Coralite.Content.Items.Magike.Filters.PolarizedFilters;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 蕴魔水晶
    /// </summary>
    public class BrilliantLevel : MagikeLevel
    {
        public override bool Available => ModContent.GetInstance<LearnedMagikeAdvanced>().Value;

        public override float MagikeCostValue => 5.3f;

        public override int PolarizedFilterItemType => ModContent.ItemType<CrystallineMagikePolarizedFilter>();

        public static ushort ID { get; private set; }

        public override void Load()
        {
            ID = Type;
        }
    }
}
