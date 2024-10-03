using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        /// <summary>
        /// 挖挖挖的世界！
        /// </summary>
        public static bool DigDigDigWorld { get; set; }

        public static LocalizedText StoneBack { get; set; }

        public void ModifyDigdigdigWorldGen(List<GenPass> tasks, ref double totalWeight)
        {
            tasks.Clear();

            //放置一层石头背景板，并且随机生成背景墙以及泥沙块
            tasks.Add(new PassLegacy("Coralite StoneBackground", GenShoneBack));

        }

    }
}
