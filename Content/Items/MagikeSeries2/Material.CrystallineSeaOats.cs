using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineSeaOats() : BaseMaterial(Item.CommonMaxStack, Item.sellPrice(0, 0, 5), ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeSeries2Item)
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.GetMagikeItem().magikeAmount = 50;
        }
    }
}
