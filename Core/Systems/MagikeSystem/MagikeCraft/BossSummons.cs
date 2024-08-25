using Coralite.Content.Items.BossSummons;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.RedJades;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class BossSummons : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //赤玉晶核
            AddRemodelRecipe<RedJade, RedJadeCore>(150, selfStack: 50);

            //冰龙心
            AddRemodelRecipe<IcicleCrystal, IcicleHeart>(300, selfStack: 10);
        }
    }
}
