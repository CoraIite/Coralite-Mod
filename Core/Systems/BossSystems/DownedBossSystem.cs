using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.BossSystems
{
    public partial class DownedBossSystem : ModSystem
    {
        public static bool downedRediancie;
        public static bool downedBabyIceDragon;
        public static bool downedSlimeEmperor;
        public static bool downedBloodiancie;
        public static bool downedThunderveinDragon;
        public static bool downedZacurrentDragon;
        public static bool downedNightmarePlantera;

        public override void PostWorldGen()
        {
            downedRediancie = false;
            downedBabyIceDragon = false;
            downedSlimeEmperor = false;
            downedBloodiancie = false;
            downedThunderveinDragon = false;
            downedZacurrentDragon = false;
            downedNightmarePlantera = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            List<string> downed = new();
            if (downedRediancie)
                downed.Add("Rediancie");

            if (downedBabyIceDragon)
                downed.Add("BabyIceDragon");

            if (downedSlimeEmperor)
                downed.Add("SlimeEmperor");

            if (downedBloodiancie)
                downed.Add("Bloodiancie");

            if (downedThunderveinDragon)
                downed.Add("ThunderveinDragon");

            if (downedZacurrentDragon)
                downed.Add("ZacurrentDragon");

            if (downedNightmarePlantera)
                downed.Add("NightmarePlantera");

            tag.Add("downed", downed);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> list = tag.GetList<string>("downed");
            downedRediancie = list.Contains("Rediancie");
            downedBabyIceDragon = list.Contains("BabyIceDragon");
            downedSlimeEmperor = list.Contains("SlimeEmperor");
            downedBloodiancie = list.Contains("Bloodiancie");
            downedThunderveinDragon = list.Contains("ThunderveinDragon");
            downedZacurrentDragon = list.Contains("ZacurrentDragon");
            downedNightmarePlantera = list.Contains("NightmarePlantera");
        }

        public static void DownSlimeEmperor()
        {
            NPC.SetEventFlagCleared(ref downedSlimeEmperor, -1);
        }

        public static void DownBloodiancie()
        {
            NPC.SetEventFlagCleared(ref downedBloodiancie, -1);
        }

        public static void DownThunderveinDragon()
        {
            NPC.SetEventFlagCleared(ref downedThunderveinDragon, -1);
        }

        public static void DownZacurrentDragon()
        {
            NPC.SetEventFlagCleared(ref downedZacurrentDragon, -1);
        }

        public static void DownNightmarePlantera()
        {
            NPC.SetEventFlagCleared(ref downedNightmarePlantera, -1);
        }
    }
}
