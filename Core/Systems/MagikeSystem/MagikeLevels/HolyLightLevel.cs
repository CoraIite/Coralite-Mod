using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 圣光
    /// </summary>
    public class HolyLightLevel : MagikeLevel
    {
        public override bool Available => NPC.downedEmpressOfLight;

        public override float MagikeCostValue => 20f;

        public override int PolarizedFilterItemType => ModContent.ItemType<HolyLightPolarizedFilter>();

        public static ushort ID { get; private set; }

        public override void Load()
        {
            ID = Type;
        }
    }
}
