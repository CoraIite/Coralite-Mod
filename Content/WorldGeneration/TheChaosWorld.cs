using System.Collections.Generic;
using Terraria;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static bool chaosWorld;

        public void GenChaosWorld(List<GenPass> tasks)
        {
            tasks.Clear();
        }

        public void FillWorld()
        {
            WorldGen.clearWorld();
        }
    }
}
