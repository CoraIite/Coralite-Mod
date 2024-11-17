using Coralite.Content.Items.BossSummons;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.RedJades;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class BossSummons : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //赤玉晶核
            AddRemodelRecipe<RedJade, RedJadeCore>(CalculateMagikeCost(MALevel.RedJade, 3), mainStack: 50);

            //冰龙心
            AddRemodelRecipe<IcicleCrystal, IcicleHeart>(CalculateMagikeCost(Icicle, 3), mainStack: 10);
        }
    }
}
