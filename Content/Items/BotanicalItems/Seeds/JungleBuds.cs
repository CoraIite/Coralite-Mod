using Coralite.Content.Tiles.Plants;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class JungleBuds : BaseSeed
    {
        public JungleBuds() : base("丛林芽孢", "成熟后能释放出丛林孢子", 999, Item.sellPrice(0, 0, 0, 5), ItemRarityID.Blue, 20, 20, 0, 0, ModContent.TileType<CoraliteJungleSpores>()) { }
    }
}
