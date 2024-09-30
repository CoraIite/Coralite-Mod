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

            //放置一层石头背景板，并且
            tasks.Add(new PassLegacy("Coralite StoneBackground", GenShoneBack));

        }

        public void GenShoneBack(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = StoneBack.Value;

            for (int i = 0; i < Main.maxTilesX; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Main.tile[i, j].ResetToType(TileID.Stone);
                }   
        }
    }
}
