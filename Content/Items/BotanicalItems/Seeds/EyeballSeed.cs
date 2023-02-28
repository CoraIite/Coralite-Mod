using Coralite.Content.Tiles.Plants;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.BotanicalItems.Seeds
{
    public class EyeballSeed : BaseSeed
    {
        public EyeballSeed() : base("眼球草种子", "", 9999, Item.sellPrice(0, 0, 10, 0), ItemRarityID.Blue, 10, 10, 0, 0, ModContent.TileType<EyeballHerb>()) { }
    }
}
