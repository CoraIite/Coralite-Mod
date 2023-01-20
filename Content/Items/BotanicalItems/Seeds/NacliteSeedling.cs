using Coralite.Content.Tiles.Plants;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class NacliteSeedling : BaseSeed
    {
        public NacliteSeedling() : base("盐晶苔藓幼体", "NaClTiXe", 9999, Item.sellPrice(0, 0, 0, 32), ItemRarityID.White, 20, 20, 0, 0, ModContent.TileType<NacliteMoss>()) { }
    }
}
