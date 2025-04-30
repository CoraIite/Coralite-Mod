using Coralite.Content.NPCs.Magike;
using Coralite.Content.Raritys;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Banner
{
    public class CrystalGolemBannerItem() : BaseBannerItem(Item.sellPrice(0, 0, 2), ModContent.RarityType<MagicCrystalRarity>()
        , ModContent.TileType<CrystalGolemBanner>())
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.KillsToBanner[Type] = 15;
        }
    }

    public class CrystalGolemBanner() : BaseBannerTile(Coralite.MagicCrystalPink, DustID.PinkCrystalShard)
    {
        public override int NpcType => ModContent.NPCType<CrystalGolem>();
    }
}
