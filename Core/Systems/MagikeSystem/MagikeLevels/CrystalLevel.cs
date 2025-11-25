using Coralite.Content.Items.Magike.Filters.PolarizedFilters;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 魔力晶体
    /// </summary>
    public class CrystalLevel : MagikeLevel
    {
        public override bool Available => ModContent.GetInstance<LearnedMagikeBase>().Value;

        public override float MagikeCostValue => 0.25f;

        public override int PolarizedFilterItemType => ModContent.ItemType<MagicCrystalPolarizedFilter>();

        public static ushort ID { get; private set; }

        public override void Load()
        {
            ID = Type;
        }
    }
}
