using Coralite.Content.Items.FlyingShields;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class VanillaNPCDrop : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //蝙蝠
            AddRemodelRecipe<BatfangShield>(0f, ItemID.BatBanner, 100, selfStack: 2);
        }
    }
}
