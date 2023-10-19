using Coralite.Content.Items.BossSummons;
using Coralite.Core.Systems.BossSystems;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Compat.BossCheckList
{
    public static class BossCheckListCalls
    {
        public static void CallBossCheckList()
        {
#pragma warning disable CS8974 // 将方法组转换为非委托类型

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
                bcl.Call(
                    "LogBoss",
                    Coralite.Instance,
                    "赤玉灵",
                    0.9f,
                    () => DownedBossSystem.downedRediancie,
                    NPCType<Content.Bosses.Rediancie.Rediancie>(),
                    new Dictionary<string, object>()
                    {
                        ["spawnInfo"] = Language.GetOrRegister($"Mods.Coralite.Compat.BossChecklist.Rediancie.SpawnInfo", () => RediancieInfo),
                        ["despawnMessage"] = Language.GetOrRegister($"Mods.Coralite.Compat.BossChecklist.Rediancie.Despawn", () => "赤玉灵回归了地底"),
                        ["spawnItems"] = ItemType<RedBerry>(),
                        ["customPortrait"] = RedianciePortrait.DrawPortrait,
                        ["collectibles"] = RediancieCollection
                    });

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
                bcl.Call(
                    "LogBoss",
                    Coralite.Instance,
                    "冰龙宝宝",
                    3.1f,
                    () => DownedBossSystem.downedBabyIceDragon,
                    NPCType<Content.Bosses.BabyIceDragon.BabyIceDragon>(),
                    new Dictionary<string, object>()
                    {
                        ["spawnInfo"] = Language.GetOrRegister($"Mods.Coralite.Compat.BossChecklist.BabyIceDragon.SpawnInfo", () => BabyIceDragonInfo),
                        ["despawnMessage"] = Language.GetOrRegister($"Mods.Coralite.Compat.BossChecklist.BabyIceDragon.Despawn", () => "冰龙宝宝逃走了"),
                        ["spawnItems"] = ItemType<IcicleHeart>(),
                        ["customPortrait"] = BabyIceDragonPortrait.DrawPortrait,
                        ["collectibles"] = BabyIceDragonCollection
                    });

                //史莱姆皇帝
                List<int> SlimeEmperorCollection = new List<int>()
                {
                    //ItemType<Content.Items.Icicle.IcicleCrystal>(),
                    ItemType<Content.Items.Gels.SlimeEmperorSoulBox>(),
                    ItemType<Content.Items.Gels.EmperorSabre>(),
                    ItemType<Content.Items.Gels.SlimeEruption>(),
                    ItemType<Content.Items.Gels.GelWhip>(),
                    ItemType<Content.Items.Gels.RoyalClassics>(),
                    ItemType<Content.Items.Gels.GelThrone>(),
                    ItemType<Content.Items.Gels.RoyalGelCannon>(),
                    //ItemType<Content.Items.Icicle.RedianciePet>(),
                };

                string SlimeEmperorInfo = "一位伟大的凝胶领袖，在史莱姆中有着极高的地位";
                bcl.Call(
                    "LogBoss",
                    Coralite.Instance,
                    "史莱姆皇帝",
                    3.2f,
                    () => DownedBossSystem.downedSlimeEmperor,
                    NPCType<Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor>(),
                    new Dictionary<string, object>()
                    {
                        ["spawnInfo"] = Language.GetOrRegister($"Mods.Coralite.Compat.BossChecklist.SlimeEmperor.SpawnInfo", () => SlimeEmperorInfo),
                        ["despawnMessage"] = Language.GetOrRegister($"Mods.Coralite.Compat.BossChecklist.SlimeEmperor.Despawn", () => "史莱姆皇帝回归了它的王国"),
                        ["spawnItems"] = ItemType<GelInvitation>(),
                        ["collectibles"] = SlimeEmperorCollection
                    });


                //梦魇之花
                List<int> NightmarePlanteraCollection = new List<int>()
                {
                    //ItemType<Content.Items.Icicle.IcicleCrystal>(),
                    ItemType<Content.Items.Nightmare.GriefSeed>(),
                    //ItemType<Content.Items.Gels.EmperorSabre>(),
                    //ItemType<Content.Items.Gels.GelThrone>(),
                    ItemType<Content.Items.Nightmare.LostSevensideHook>(),
                    ItemType<Content.Items.Nightmare.DreamShears>(),
                    ItemType<Content.Items.Nightmare.EuphorbiaMilii>(),
                    ItemType<Content.Items.Nightmare.Lycoris>(),


                    ItemType<Content.Items.Nightmare.DevilsClaw>(),
                    ItemType<Content.Items.Nightmare.BarrenThornsStaff>(),

                    ItemType<Content.Items.Nightmare.Dreamcatcher>(),
                    ItemType<Content.Items.Nightmare.Eden>(),

                    //ItemType<Content.Items.Gels.RoyalGelCannon>(),
                    //ItemType<Content.Items.Icicle.RedianciePet>(),
                };

                string NightmarePlanteraInfo = "噩梦占据了曾经被击败的世纪之花的躯体，变成了强大的梦魇之神";
                bcl.Call(
                    "LogBoss",
                    Coralite.Instance,
                    "梦魇之花",
                    18.1f,//18是月总
                    () => DownedBossSystem.downedNightmarePlantera,
                    NPCType<Content.Bosses.VanillaReinforce.NightmarePlantera.NightmarePlantera>(),
                    new Dictionary<string, object>()
                    {
                        ["spawnInfo"] = Language.GetOrRegister($"Mods.Coralite.Compat.BossChecklist.NightmarePlantera.SpawnInfo", () => NightmarePlanteraInfo),
                        ["despawnMessage"] = Language.GetOrRegister($"Mods.Coralite.Compat.BossChecklist.NightmarePlantera.Despawn", () => "梦境消散了"),
                        ["spawnItems"] = ItemType<NightmareHarp>(),
                        ["collectibles"] = NightmarePlanteraCollection
                    });

#pragma warning restore CS8974 // 将方法组转换为非委托类型
            }
        }
    }
}
