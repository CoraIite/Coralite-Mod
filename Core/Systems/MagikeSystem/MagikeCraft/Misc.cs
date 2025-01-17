using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class Misc : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //泥土
            AddRemodelRecipe(ItemID.DirtBlock, ItemID.Acorn, 5, mainStack: 8);

            //橡实
            MagikeCraftRecipe.CreateRecipe(ItemID.Acorn, ItemID.Wood, CalculateMagikeCost(MagicCrystal, 2, 30), resultItemStack: 25)
                .RegisterNew(ItemID.AshWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.RichMahogany, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.Ebonwood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.Shadewood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.BorealWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.PalmWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.Pearlwood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .AddCondition(Condition.Hardmode)
                .Register();
        }
    }
}
