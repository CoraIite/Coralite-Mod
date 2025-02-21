﻿using Coralite.Content.Items.BossSummons;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.RedJades;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    public class BossSummons : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //赤玉晶核
            AddRemodelRecipe<RedJade, RedJadeCore>(CalculateMagikeCost(MALevel.RedJade, 3), mainStack: 35);

            //冰龙心
            AddRemodelRecipe<IcicleCrystal, IcicleHeart>(CalculateMagikeCost(Icicle, 3), mainStack: 8);
        }
    }
}
