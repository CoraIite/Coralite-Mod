using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem
    {
        /// <summary> 是否学习过 书页：魔能基础 </summary>
        public static bool learnedMagikeBase;
        public static bool learnedMagikeAdvanced;

        //额...每增加一个变量就要在这边多写一段，说实话显得很蠢，以后有机会需要修改掉
        public override void SaveWorldData(TagCompound tag)
        {
            List<string> Knowledge = new List<string>();
            if (learnedMagikeBase)
                Knowledge.Add("learnedMagikeBase");
            if (learnedMagikeAdvanced)
                Knowledge.Add("learnedMagikeAdvanced");

            tag.Add("Knowledge", Knowledge);

        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> list = tag.GetList<string>("Knowledge");
            learnedMagikeBase = list.Contains("learnedMagikeBase");
            learnedMagikeAdvanced = list.Contains("learnedMagikeAdvanced");

        }
    }
}
