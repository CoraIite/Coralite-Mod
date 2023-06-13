using Coralite.Content.Tiles.Plants;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Botanical.Seeds
{
    public class NacliteSeedling : BaseSeed
    {
        public NacliteSeedling() : base( 9999, Item.sellPrice(0, 0, 0, 32), ItemRarityID.White, 20, 20, 0, 0, ModContent.TileType<NacliteMoss>()) { }
    }
}
