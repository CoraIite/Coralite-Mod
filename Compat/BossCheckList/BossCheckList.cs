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
                    //ItemType<Content.Items.RedJades.RedJade>(),
                    ItemType<Content.Items.RedJades.RediancieBossBag>(),
                    ItemType<Content.Items.RedJades.RedJadePendant>(),
                    ItemType<Content.Items.RedJades.RediancieTrophy>(),
                    ItemType<Content.Items.RedJades.RediancieRelic>(),
                    ItemType<Content.Items.RedJades.RedianciePet>(),
                };

                string RediancieInfo = "一块奇异的巨石，通过不稳定的红色能量来驱动自身。";
                bcl.Call("AddBoss", Coralite.Instance, "赤玉灵", NPCType<Content.Bosses.Rediancie.Rediancie>(), 0.9f,
                    () => DownedBossSystem.downedRediancie,
                    () => true,
                    RediancieCollection, ItemType<Content.Items.BossSummons.RedBerry>(), RediancieInfo, "赤玉灵回归了地底");

                //冰龙宝宝
                List<int> BabyIceDragonCollection = new List<int>()
                {
                    ItemType<Content.Items.Icicle.IcicleCrystal>(),
                    ItemType<Content.Items.Icicle.BabyIceDragonBossBag>(),
                    ItemType<Content.Items.Icicle.IcicleCoccyx>(),
                    ItemType<Content.Items.Icicle.BabyIceDragonTrophy>(),
                    ItemType<Content.Items.Icicle.BabyIceDragonRelic>(),
                    //ItemType<Content.Items.Icicle.RedianciePet>(),
                };

                string BabyIceDragonInfo = "刚刚破壳而出的小冰龙，还不能完全掌握冰魔法的力量";
                bcl.Call("AddBoss", Coralite.Instance, "冰龙宝宝", NPCType<Content.Bosses.BabyIceDragon.BabyIceDragon>(), 3.1f,
                    () => DownedBossSystem.downedBabyIceDragon,
                    () => true,
                    BabyIceDragonCollection, null, BabyIceDragonInfo, "冰龙宝宝逃走了");

            }
        }
    }
}
