using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Items.Magike
{
    public class SplendorMagicore : BaseMaterial
    {
        public SplendorMagicore() : base(Item.CommonMaxStack, Item.sellPrice(0, 1), ModContent.RarityType<SplendorMagicoreRarity>(), AssetDirectory.MagikeItems)
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.GetMagikeItem().magikeAmount = 6000;
        }
    }
}
