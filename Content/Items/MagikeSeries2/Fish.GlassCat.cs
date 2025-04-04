using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class GlassCat : BaseMaterial
    {
        public GlassCat() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 20), ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeSeries2Item)
        {
        }
    }
}
