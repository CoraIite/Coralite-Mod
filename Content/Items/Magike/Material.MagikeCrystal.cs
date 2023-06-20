using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class MagicCrystal : BaseMaterial
    {
        public MagicCrystal() : base(Item.CommonMaxStack, Item.sellPrice(0,0,1), ModContent.RarityType<MagikeCrystalRarity>(), AssetDirectory.MagikeItems)
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.GetMagikeItem().magiteAmount = 25;
        }
    }
}
