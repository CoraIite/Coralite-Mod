using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    internal class CrystallineEngram : BaseMaterial
    {
        public CrystallineEngram() : base(Item.CommonMaxStack, Item.sellPrice(0, 3), ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeSeries2Item)
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }
    }
}
