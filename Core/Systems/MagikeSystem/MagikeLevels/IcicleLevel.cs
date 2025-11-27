using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Core.Systems.BossSystems;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 冰晶
    /// </summary>
    public class IcicleLevel : MagikeLevel
    {
        public override bool Available => ModContent.GetInstance<DownedBabyIceDragon>().Value;

        public override float MagikeCostValue => 1.5f;

        public override int PolarizedFilterItemType => ModContent.ItemType<IciclePolarizedFilter>();

        public override Color LevelColor => Coralite.IcicleCyan;

        public static ushort ID { get; private set; }

        public override void SetStaticDefaults()
        {
            ID = Type;
        }
    }
}
