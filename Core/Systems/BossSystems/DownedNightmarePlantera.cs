using Coralite.Core.Systems.WorldValueSystem;
using Terraria;

namespace Coralite.Core.Systems.BossSystems
{
    public partial class DownedBossSystem : ModSystem
    {
        public static void DownNightmarePlantera()
        {
            DownedNightmarePlantera d = ModContent.GetInstance<DownedNightmarePlantera>();

            bool b = false;
            NPC.SetEventFlagCleared(ref b, -1);

            if (!d.Value)
                d.SetAndSync(true);
        }
    }


    public class DownedNightmarePlantera : WorldFlag { }
}
