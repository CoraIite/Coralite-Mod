using Coralite.Content.NPCs.Magike;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Banner
{
    public class CrystalGolemBannerItem() : BaseBannerItem(Item.sellPrice(0, 0, 2), ItemRarityID.Orange
        , ModContent.TileType<CrystalGolemBanner>())
    {

    }

    public class CrystalGolemBanner() : BaseBannerTile(Color.Pink, DustID.PinkCrystalShard)
    {
        public override int NpcType => ModContent.NPCType<CrystalGolem>();
    }
}
