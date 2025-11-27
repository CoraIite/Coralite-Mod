using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 辉界
    /// </summary>
    public class SplendorLevel : MagikeLevel
    {
        public override bool Available => NPC.downedMoonlord;

        public override float MagikeCostValue => 50f;

        public override int PolarizedFilterItemType => ModContent.ItemType<SplendorMagicorePolarizedFilter>();

        public override Color LevelColor => Coralite.SplendorMagicoreLightBlue;

        public static ushort ID { get; private set; }

        public override void SetStaticDefaults()
        {
            ID = Type;
        }
    }
}
