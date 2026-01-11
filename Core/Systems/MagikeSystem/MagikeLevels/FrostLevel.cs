using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 寒霜
    /// </summary>
    public class FrostLevel : MagikeLevel
    {
        public override bool Available => Main.hardMode;

        public override float MagikeCostValue => 7.5f;

        public override int PolarizedFilterItemType => ModContent.ItemType<ForbiddenPolarizedFilter>();

        public override Color LevelColor => Color.LightSkyBlue;

        public static ushort ID { get; private set; }

        public override void SetStaticDefaults()
        {
            ID = Type;
        }
    }
}
