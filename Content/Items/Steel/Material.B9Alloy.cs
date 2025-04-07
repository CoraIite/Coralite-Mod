using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class B9Alloy : BaseMaterial, IMagikeCraftable
    {
        public B9Alloy() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 60), ItemRarityID.Pink, AssetDirectory.SteelItems)
        {
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<SteelBar, B9Alloy>(MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 4, 20), 2,2)
                            .AddIngredient<IcicleCrystal>()
                            .AddIngredient(ItemID.HellstoneBar)
                            .Register();
        }
    }
}
