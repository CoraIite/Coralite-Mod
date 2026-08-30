using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike
{
    public class SplendorMagicore : BaseMaterial, IMagikeCraftable
    {
        public SplendorMagicore() : base(Item.CommonMaxStack, Item.sellPrice(0, 1), ModContent.RarityType<SplendorMagicoreRarity>(), AssetDirectory.MagikeItems)
        { }

        public void AddMagikeCraftRecipe()
        {
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CrystallineMagike>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.GetMagikeItem().magikeAmount = 5000;
        }
    }
}
