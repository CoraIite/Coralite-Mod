using Coralite.Content.Biomes;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Coralite.Content.NPCs.Magike
{
    public class MagikeGlobalNPC:GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<MagicCrystalCave>())
            {
                pool[0] = 0.02f;
            }
        }
    }
}
