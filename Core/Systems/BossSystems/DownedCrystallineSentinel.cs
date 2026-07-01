using Coralite.Core.Systems.WorldValueSystem;

namespace Coralite.Core.Systems.BossSystems
{
    public partial class DownedBossSystem : ModSystem
    {
        public static void DownCrystallineSentinel()
        {
            DownedCrystallineSentinel d = ModContent.GetInstance<DownedCrystallineSentinel>();

            if (!d.Value)
                d.SetAndSync(true);
        }
    }

    public class DownedCrystallineSentinel : WorldFlag { }
}
