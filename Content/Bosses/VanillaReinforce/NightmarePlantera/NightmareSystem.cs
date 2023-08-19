using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareSystem : ModSystem
    {
        public override void PostUpdateTime()
        {
            //防止梦魇之花因为意外事故消失时玩家BUFF还在的情况
            if (NightmarePlantera.NightmarePlanteraAlive(out NPC np))
                if (!np.active || np.type != ModContent.NPCType<NightmarePlantera>())
                    NightmarePlantera.NPBossIndex = -1;
        }
    }
}
