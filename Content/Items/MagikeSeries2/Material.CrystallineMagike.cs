using Coralite.Content.Items.Magike;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineMagike : BaseMaterial, IMagikeCraftable
    {
        public CrystallineMagike() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 25), ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeSeries2Item)
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.GetMagikeItem().magikeAmount = 450;
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeSystem.AddRemodelRecipe<CrystallineMagike, SplendorMagicore>(5000, conditions: Condition.DownedMoonLord);
        }
    }
}
