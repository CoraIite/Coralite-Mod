using Coralite.Content.Tiles.Plants;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Botanical.Seeds
{
    public class EgenolfiaBuds : BaseSeed
    {
        public EgenolfiaBuds() : base(9999, Item.sellPrice(0, 0, 0, 16), ItemRarityID.Blue, 15, 15, 0, 0, ModContent.TileType<EgenolfiaSandy>()) { }
    }
}
