using Coralite.Content.Bosses.Rediancie;
using Coralite.Core.Systems.BossSystems;
using System.Collections.Generic;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Compat.BossCheckList
{
    public static class BossCheckListCalls
    {
        public static void CallBossCheckList()
        {
            if (ModLoader.TryGetMod("BossCheckList",out Mod bcl))
            {
                //赤玉灵
                List<int> RediancieCollection = new List<int>()
                {
                    //ItemType<Content.Items.RedJadeItems.RedJade>(),
                    ItemType<Content.Items.RedJades.RediancieBossBag>(),
                    ItemType<Content.Items.RedJades.RedJadePendant>(),
                    ItemType<Content.Items.RedJades.RediancieRelic>(),
                    ItemType<Content.Items.RedJades.RediancieTrophy>()
                };

                string RediancieInfo = "一块奇异的巨石，通过不稳定的红色能量来驱动自身。";
                bcl.Call("AddBoss", Coralite.Instance, "赤玉灵", NPCType<Rediancie>(), 0.9f,
                    () => DownedBossSystem.downedRediancie,
                    () => true,
                    RediancieCollection, ItemType<Content.Items.BossSummons.RedBerry>(), RediancieInfo, "赤玉灵回归了地底。");
            }
        }
    }
}
