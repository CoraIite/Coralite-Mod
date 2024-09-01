using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class Misc : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //泥土
            AddRemodelRecipe(ItemID.DirtBlock, ItemID.Acorn, 5, mainStack: 8);

            //橡实
            AddRemodelRecipe(ItemID.Acorn, ItemID.Wood, 25, 6);
            AddRemodelRecipe(ItemID.Acorn, ItemID.AshWood, 25, 6);
            AddRemodelRecipe(ItemID.Acorn, ItemID.RichMahogany, 25, 6);
            AddRemodelRecipe(ItemID.Acorn, ItemID.Ebonwood, 25, 6);
            AddRemodelRecipe(ItemID.Acorn, ItemID.Shadewood, 25, 6);
            AddRemodelRecipe(ItemID.Acorn, ItemID.Pearlwood, 150, 6, conditions: Condition.Hardmode);
            AddRemodelRecipe(ItemID.Acorn, ItemID.BorealWood, 25, 6);
            AddRemodelRecipe(ItemID.Acorn, ItemID.PalmWood, 25, 6);


        }
    }
}
