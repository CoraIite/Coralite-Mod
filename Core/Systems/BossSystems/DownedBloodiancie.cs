using Coralite.Core.Systems.WorldValueSystem;
using Terraria;

namespace Coralite.Core.Systems.BossSystems
{
    public partial class DownedBossSystem : ModSystem
    {
        public static void DownBloodiancie()
        {
            DownedBloodiancie d = ModContent.GetInstance<DownedBloodiancie>();

            bool b = false;
            NPC.SetEventFlagCleared(ref b, -1);

            if (!d.Value)
                d.SetAndSync(true);
        }
    }


    public class DownedBloodiancie : WorldFlag { }
}
