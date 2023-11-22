using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.CraftConditions;
using Coralite.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class CrystallineMagike : BaseMaterial, IMagikeRemodelable
    {
        public CrystallineMagike() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 1), ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeItems)
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.GetMagikeItem().magikeAmount = 300;
        }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<CrystallineMagike, SplendorMagicore>(700, condition: DownedMoonlordCondition.Instance);
        }
    }
}
