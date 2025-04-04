using Coralite.Content.Dusts;
using Coralite.Content.NPCs.Crystalline;
using Coralite.Content.Raritys;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Banner
{
    public class WildMabirdBannerItem() : BaseBannerItem(Item.sellPrice(0, 0, 2), ModContent.RarityType<CrystallineMagikeRarity>()
        , ModContent.TileType<WildMabirdBanner>())
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.KillsToBanner[Type] = 50;
        }
    }

    public class WildMabirdBanner() : BaseBannerTile(Coralite.CrystallinePurple, ModContent.DustType<CrystallineDustSmall>())
    {
        public override int NpcType => ModContent.NPCType<WildMabird>();
    }
}
