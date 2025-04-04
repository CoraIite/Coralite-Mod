using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class CrystalFins : BaseMaterial
    {
        public CrystalFins() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 10), ModContent.RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeSeries1Item)
        {
        }
    }
}
