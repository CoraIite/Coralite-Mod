using Coralite.Core.Systems.MagikeSystem.RemodelConditions;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class VanillaCrate : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //木板条箱
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.SailfishBoots);
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.TsunamiInABottle);
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.Extractinator);
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.Aglet);
            AddRemodelRecipe(ItemID.WoodenCrate, 50, ItemID.PortableStool);
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.ClimbingClaws);
            AddRemodelRecipe(ItemID.WoodenCrate, 50, ItemID.CordageGuide);
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.Radar);
            AddRemodelRecipe(ItemID.WoodenCrate, 50, ItemID.ApprenticeBait,3);
            AddRemodelRecipe(ItemID.WoodenCrate, 100, ItemID.JourneymanBait, 3);

        }
    }
}
