using Coralite.Content.NPCs.Magike;
using Coralite.Content.Raritys;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Banner
{
    public class CrystalSpiritBannerItem() : BaseBannerItem(Item.sellPrice(0, 0, 2), ModContent.RarityType<MagicCrystalRarity>()
        , ModContent.TileType<CrystalSpiritBanner>())
    {

    }

    public class CrystalSpiritBanner() : BaseBannerTile(Coralite.MagicCrystalPink, DustID.PinkCrystalShard)
    {
        public override int NpcType => ModContent.NPCType<CrystalSpirit>();
    }
}
