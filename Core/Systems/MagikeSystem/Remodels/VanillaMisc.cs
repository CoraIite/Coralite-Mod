using Coralite.Core.Systems.MagikeSystem.CraftConditions;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class VanillaMisc : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //泥土
            AddRemodelRecipe(ItemID.DirtBlock, 5, ItemID.Acorn, selfStack: 8);

            //橡实
            AddRemodelRecipe(ItemID.Acorn, 25, ItemID.Wood, 6);
            AddRemodelRecipe(ItemID.Acorn, 25, ItemID.AshWood, 6);
            AddRemodelRecipe(ItemID.Acorn, 25, ItemID.RichMahogany, 6);
            AddRemodelRecipe(ItemID.Acorn, 25, ItemID.Ebonwood, 6);
            AddRemodelRecipe(ItemID.Acorn, 25, ItemID.Shadewood, 6);
            AddRemodelRecipe(ItemID.Acorn, 150, ItemID.Pearlwood, 6,condition:HardModeCondition.Instance);
            AddRemodelRecipe(ItemID.Acorn, 25, ItemID.BorealWood, 6);
            AddRemodelRecipe(ItemID.Acorn, 25, ItemID.PalmWood, 6);


        }
    }
}
