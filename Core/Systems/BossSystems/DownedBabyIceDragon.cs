using Coralite.Core.Systems.WorldValueSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Core.Systems.BossSystems
{
    public partial class DownedBossSystem : ModSystem
    {
        public static void DownBabyIceDragon()
        {
            DownedBabyIceDragon d = ModContent.GetInstance<DownedBabyIceDragon>();

            NPC.SetEventFlagCleared(ref NPC.downedBoss2, 14);
            NetMessage.SendData(MessageID.WorldData);

            if (!d.Value)
                d.SetAndSync(true);
        }
    }

    public class DownedBabyIceDragon : WorldFlag { }
}
