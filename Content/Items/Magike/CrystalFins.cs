using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike
{
    public class CrystalFins : BaseMaterial
    {
        public CrystalFins() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 10), ItemRarityID.Pink, AssetDirectory.MagikeItems)
        {
        }
    }
}
