using Coralite.Content.Items.Misc_Magic;
using Coralite.Core.Systems.MagikeSystem.RemodelConditions;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class SpecialWeapon : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //异界裂隙
            AddRemodelRecipe<CosmosFracture>(0f, ItemID.SkyFracture, 1_0000, condition: EnchantCondition.Instance);

        }
    }
}
