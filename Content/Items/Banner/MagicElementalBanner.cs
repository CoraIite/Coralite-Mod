using Coralite.Content.NPCs.Elemental;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Banner
{
    public class MagicElementalBannerItem() : BaseBannerItem(Item.sellPrice(0, 0, 2), ItemRarityID.Blue
        , ModContent.TileType<MagicElementalBanner>())
    {

    }

    public class MagicElementalBanner() : BaseBannerTile(Color.SkyBlue, DustID.ShadowbeamStaff)
    {
        public override int NpcType => ModContent.NPCType<MagicElemental>();
    }
}
