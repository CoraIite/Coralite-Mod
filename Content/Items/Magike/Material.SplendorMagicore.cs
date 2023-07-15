using Coralite.Content.Raritys;
using Coralite.Core;
using Terraria.ModLoader;
using Terraria;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;

namespace Coralite.Content.Items.Magike
{
    public class SplendorMagicore : BaseMaterial
    {
        public SplendorMagicore() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 15), ModContent.RarityType<SplendorMagicoreRarity>(), AssetDirectory.MagikeItems)
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.GetMagikeItem().magiteAmount = 1000;
        }
    }
}
