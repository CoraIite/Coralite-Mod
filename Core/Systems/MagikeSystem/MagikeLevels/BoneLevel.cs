using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 骨头
    /// </summary>
    public class BoneLevel : MagikeLevel
    {
        public override bool Available => NPC.downedBoss3;

        public override float MagikeCostValue => 2f;

        public override int PolarizedFilterItemType => ModContent.ItemType<BonePolarizedFilter>();

        public override Color LevelColor => Color.DimGray;

        public static ushort ID { get; private set; }

        public override void SetStaticDefaults()
        {
            ID = Type;
        }
    }
}
