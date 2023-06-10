using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class Watermelon : BaseMaterial
    {
        public Watermelon() : base( 999, Item.sellPrice(0, 0, 1, 0), ItemRarityID.Blue, AssetDirectory.Plants) { }
    }
}
