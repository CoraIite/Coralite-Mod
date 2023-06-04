using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.BossSystems
{
    public partial class DownedBossSystem : ModSystem
    {
        public static bool downedRediancie;
        public static bool downedBabyIceDragon;

        public override void PostWorldGen()
        {
            downedRediancie = false;
            downedBabyIceDragon = false;

        }

        public override void SaveWorldData(TagCompound tag)
        {
            List<string> downed = new List<string>();
            if (downedRediancie)
                downed.Add("Rediancie");

            if (downedBabyIceDragon)
                downed.Add("BabyIceDragon");

            tag.Add("downed", downed);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> list = tag.GetList<string>("downed");
            downedRediancie = list.Contains("Rediancie");
            downedBabyIceDragon = list.Contains("BabyIceDragon");

        }
    }
}
