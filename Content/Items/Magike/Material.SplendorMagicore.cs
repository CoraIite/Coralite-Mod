using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class SplendorMagicore : BaseMaterial
    {
        public SplendorMagicore() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 15), ModContent.RarityType<SplendorMagicoreRarity>(), AssetDirectory.MagikeItems)
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.GetMagikeItem().magikeAmount = 1000;
        }
    }
}
