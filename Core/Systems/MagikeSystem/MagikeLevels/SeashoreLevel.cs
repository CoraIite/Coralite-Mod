using Coralite.Content.Items.Magike.Filters.PolarizedFilters;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 海滨
    /// </summary>
    public class SeashoreLevel : MagikeLevel
    {
        public override bool Available => ModContent.GetInstance<LearnedMagikeBase>().Value;

        public override float MagikeCostValue => 0.25f;

        public override int PolarizedFilterItemType => ModContent.ItemType<SeashorePolarizedFilter>();

        public override Color LevelColor => Color.SeaShell;

        public static ushort ID { get; private set; }

        public override void SetStaticDefaults()
        {
            ID = Type;
        }
    }
}
