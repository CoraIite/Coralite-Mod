using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.RedJades;
using Coralite.Core.Systems.MagikeSystem.RemodelConditions;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class CoraliteBossBag : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //赤玉灵宝藏袋
            AddRemodelRecipe<RediancieBossBag, RedJade>(150, 34);
            AddRemodelRecipe<RediancieBossBag, RediancieTrophy>(150);
            AddRemodelRecipe<RediancieBossBag, RedianciePet>(50, condition: MasterModeCondition.Instance);

            //冰龙宝宝宝藏袋
            AddRemodelRecipe<BabyIceDragonBossBag, IcicleCrystal>(225, 5);
            AddRemodelRecipe<BabyIceDragonBossBag, BabyIceDragonTrophy>(150);

        }
    }
}
