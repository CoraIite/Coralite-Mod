using Coralite.Content.Dusts;
using Coralite.Content.NPCs.Crystalline;
using Coralite.Content.Raritys;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Banner
{
    public class CrystallineHummingBannerItem() : BaseBannerItem(Item.sellPrice(0, 0, 2), ModContent.RarityType<CrystallineMagikeRarity>()
        , ModContent.TileType<CrystallineHummingBanner>())
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.KillsToBanner[Type] = 30;
        }
    }

    public class CrystallineHummingBanner() : BaseBannerTile(Coralite.CrystallinePurple, ModContent.DustType<CrystallineDustSmall>())
    {
        public override int NpcType => ModContent.NPCType<CrystallineHumming>();
    }
}
