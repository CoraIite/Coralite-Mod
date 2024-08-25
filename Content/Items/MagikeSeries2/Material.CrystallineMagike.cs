using Coralite.Content.Items.Magike;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineMagike : BaseMaterial, IMagikeRemodelable
    {
        public CrystallineMagike() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 1), ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeSeries2Item)
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.GetMagikeItem().magikeAmount = 300;
        }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<CrystallineMagike, SplendorMagicore>(700, conditions: Condition.DownedMoonLord);
        }
    }
}
