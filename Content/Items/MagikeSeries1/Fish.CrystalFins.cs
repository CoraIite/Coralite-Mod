using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class CrystalFins : BaseMaterial
    {
        public CrystalFins() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 10), ItemRarityID.Pink, AssetDirectory.MagikeSeries1Item)
        {
        }
    }
}
