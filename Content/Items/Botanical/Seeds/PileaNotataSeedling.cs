using Coralite.Content.Tiles.Plants;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Botanical.Seeds
{
    public class PileaNotataSeedling : BaseSeed
    {
        public PileaNotataSeedling() : base(9999, Item.sellPrice(0, 0, 0, 16), ItemRarityID.White, 15, 15, 0, 0, ModContent.TileType<PileaNotata>()) { }
    }
}
