using Coralite.Content.Tiles.Plants;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class GloomSpores : BaseSeed
    {
        public GloomSpores() : base("吸光蘑菇孢子", "成熟后收获吸光蘑菇", 9999, Item.sellPrice(0, 0, 0, 50), ItemRarityID.Blue, 6, 6, 0, 0, ModContent.TileType<GloomMushroom>()) { }
    }
}
