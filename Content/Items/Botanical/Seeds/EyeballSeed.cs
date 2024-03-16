using Coralite.Content.Tiles.Plants;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Botanical.Seeds
{
    public class EyeballSeed : BaseSeed
    {
        public EyeballSeed() : base(9999, Item.sellPrice(0, 0, 10, 0), ItemRarityID.Blue, 10, 10, 0, 0, ModContent.TileType<EyeballHerb>()) { }
    }
}
