
using Coralite.Content.NPCs.Elemental;
using Coralite.Content.NPCs.OtherNPC;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Banner
{
    public class MaragonBannerItem() : BaseBannerItem(Item.sellPrice(0, 0, 2), ItemRarityID.Blue
        , ModContent.TileType<MaragonBanner>())
    {

    }

    public class MaragonBanner() : BaseBannerTile(Color.SkyBlue, DustID.ShadowbeamStaff)
    {
        public override int NpcType => ModContent.NPCType<MaragonHead>();
    }
}
