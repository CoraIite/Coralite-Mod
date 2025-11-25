using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 远洋
    /// </summary>
    public class PelagicLevel : MagikeLevel
    {
        public override bool Available => Main.hardMode;

        public override float MagikeCostValue => 7.5f;

        public override int PolarizedFilterItemType => ModContent.ItemType<PelagicPolarizedFilter>();

        public static ushort ID { get; private set; }

        public override void Load()
        {
            ID = Type;
        }
    }
}
