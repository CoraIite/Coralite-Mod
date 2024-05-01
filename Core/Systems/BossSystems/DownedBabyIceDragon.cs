using Terraria;

namespace Coralite.Core.Systems.BossSystems
{
    public partial class DownedBossSystem : ModSystem
    {
        public static void DownBabyIceDragon()
        {
            downedBabyIceDragon = true;
            NPC.SetEventFlagCleared(ref NPC.downedBoss2, 14);
        }
    }
}
