using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        /// <summary>
        /// 挖挖挖的世界！
        /// </summary>
        public static bool DigDigDigWorld { get; set; }

        public void ModifyDigdigdigWorldGen(List<GenPass> tasks, ref double totalWeight)
        {
            tasks.Clear();

            //设置一些基础值
            tasks.Add(new PassLegacy("Coralite DigReset", GenShoneBack));

            //放置一层石头背景板，并且随机生成背景墙以及泥沙块
            tasks.Add(new PassLegacy("Coralite DigStoneBackground", GenShoneBack));

            //生成丛林的基本样子
            tasks.Add(new PassLegacy("Coralite DigJungle", GenDigJungle));

        }

    }
}
