using Coralite.Content.NPCs.Magike;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Banner
{
    public class CrystalEyeBannerItem() : BaseBannerItem(Item.sellPrice(0, 0, 2), ItemRarityID.Blue
        , ModContent.TileType<CrystalEyeBanner>())
    {

    }

    public class CrystalEyeBanner() : BaseBannerTile(Color.Pink, DustID.PinkCrystalShard)
    {
        public override int NpcType => ModContent.NPCType<CrystalEye>();
    }
}
