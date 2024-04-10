using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Items.ShadowCastle
{
    public class ShadowDart : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.WaterBolt;
            ItemID.Sets.ShimmerTransformToItem[ItemID.WaterBolt] = Type;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }
    }

}
