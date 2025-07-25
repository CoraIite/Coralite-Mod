﻿using Coralite.Content.Items.CoreKeeper;
using Coralite.Content.Items.Donator;
using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.LandOfTheLustrousSeries.Accessories;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.Misc;
using Coralite.Content.Items.Misc_Equip;
using Coralite.Content.Items.Placeable;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Content.Items.YujianHulu;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.GlobalNPCs
{
    public partial class CoraliteGlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                default: break;
                //case NPCID.DemonEye://恶魔眼
                //case NPCID.DemonEye2:
                //    npcLoot.Add(ItemDropRule.Common(ItemType<EyeballSeed>(), 50));
                //    break;
                //case NPCID.WanderingEye:
                //    npcLoot.Add(ItemDropRule.Common(ItemType<EyeballSeed>(), 25));
                //    break;
                case NPCID.CaveBat://蝙蝠掉落血鄂
                    npcLoot.Add(ItemDropRule.Common(ItemType<BatfangShield>(), 80, 1, 1));
                    break;
                case NPCID.GiantBat:
                    npcLoot.Add(ItemDropRule.Common(ItemType<BatfangShield>(), 60, 1, 1));
                    break;
                //case NPCID.DarkCaster://地牢怪掉落影子
                //    npcLoot.Add(ItemDropRule.Common(ItemType<ShadowEnergy>(), 3, 1, 3));
                //    break;

                case NPCID.EaterofSouls://远古腐化御剑
                case NPCID.CorruptGoldfish:
                case NPCID.DevourerHead:
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientDemoniteYujian>(), 100));
                    break;

                case NPCID.Crimera://远古血腥御剑
                case NPCID.FaceMonster:
                case NPCID.BloodCrawler:
                case NPCID.CrimsonGoldfish:
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientCrimtaneYujian>(), 100));
                    break;
                case NPCID.LeechHead://血蛭掉落血蛭
                    npcLoot.Add(ItemDropRule.Common(ItemType<BloodWorm>()));
                    break;
                case NPCID.Mimic://eek的盾
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotRemixSeed(), ItemType<EekShield>(), 10));
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.RemixSeedHardmode(), ItemType<EekShield>(), 10));
                    break;
                case NPCID.PossessedArmor://幻影胸甲
                    npcLoot.Add(ItemDropRule.Common(ItemType<PossessedChest>(), 22));
                    break;
                case NPCID.Tim://八角手
                    npcLoot.Add(ItemDropRule.Common(ItemType<EightsquareHand>()));
                    break;
                case NPCID.HallowBoss://光女掉落圣光残片
                    npcLoot.Add(ItemDropRule.ByCondition(new DownedGolemCondition(), ItemType<FragmentsOfLight>(), 1, 4, 6));
                    break;
                case NPCID.DukeFishron://猪鲨掉落皮
                    npcLoot.Add(ItemDropRule.ByCondition(new DownedGolemCondition(), ItemType<DukeFishronSkin>(), 1, 3, 5));
                    break;
                case NPCID.EyeballFlyingFish://血月怪掉落血红宝珠
                case NPCID.ZombieMerman:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemType<BloodyOrb>(), 4, 1, 3));
                    break;
                case NPCID.GoblinShark:
                case NPCID.BloodEelHead:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemType<BloodyOrb>(), 2, 1, 3));
                    break;
                case NPCID.BloodNautilus:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemType<BloodyOrb>(), 1, 1, 3));
                    break;
                case NPCID.Mothron://蛾怪掉落腐朽的盾，远古核心
                    npcLoot.Add(ItemDropRule.Common(ItemType<RustedShield>(), 4));
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientCore>(), 4));
                    break;
                case NPCID.PirateCaptain://海盗船长掉落海盗套装
                    {
                        IItemDropRule[] PirateKingTypes =
                        [
                            ItemDropRule.Common(ItemType<PirateKingHat>()),
                            ItemDropRule.Common(ItemType<PirateKingCoat>()),
                            ItemDropRule.Common(ItemType<PirateKingShoes>()),
                        ];
                        npcLoot.Add(new FewFromRulesRule(1, 4, PirateKingTypes));
                        npcLoot.Add(ItemDropRule.Common(ItemType<LegendaryCard>(), 2));
                    }
                    break;

                case NPCID.EyeofCthulhu://克眼，脑子，世吞掉落美味肉排
                case NPCID.BrainofCthulhu://克眼，脑子，世吞掉落美味肉排
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ItemType<DeliciousSteak>(), 5));
                    break;
                case NPCID.EaterofWorldsBody://克眼，脑子，世吞掉落美味肉排
                case NPCID.Creeper://克眼，脑子，世吞掉落美味肉排
                    npcLoot.Add(ItemDropRule.Common(ItemType<DeliciousSteak>(), 50));
                    break;
                case NPCID.PlanterasTentacle://世花小触手掉落再生触手
                    npcLoot.Add(ItemDropRule.Common(ItemType<RegrowthTentacle>(), 2, 1, 2));
                    break;
                case NPCID.SkeletronHead://骷髅王掉落乱流核心
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ItemType<TurbulenceCore>()));
                    break;
                case NPCID.FlyingSnake://羽蛇掉毛
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.DownedPlantera(), ItemType<FlyingSnakeFeather>(), 1, 1, 2));
                    break;
                case NPCID.WallofFlesh://肉山和鸟妖掉落破碎剑柄
                    npcLoot.Add(ItemDropRule.Common(ItemType<BrokenHandle>(), 10));
                    break;
                case NPCID.Harpy://女妖额外掉落天空戒指
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemType<BrokenHandle>(), 150, 1, 1));
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.DownedPlantera(), ItemType<SkyRing>(), 50, 1, 1));
                    break;

                case NPCID.RockGolem://岩石巨人掉落上古宝石，上级宝石原石
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientGemstone>(), 1, 1, 3));
                    npcLoot.Add(ItemDropRule.Common(ItemType<SeniorRoughGemstone>(), 2, 1, 3));
                    break;

                case NPCID.GraniteFlyer://岩石巨人,花岗岩敌怪，附魔剑 掉落上古宝石
                case NPCID.EnchantedSword:
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientGemstone>(), 8, 1, 3));
                    break;
                case NPCID.GraniteGolem://花岗岩巨人额外掉落上古守护者饰品
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientGemstone>(), 10, 1, 3));
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientGuardianNecklace>(), 125));
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientGuardianRing>(), 125));
                    break;

                case NPCID.AngryBones://愤怒骷髅掉落骨质戒指
                case NPCID.AngryBonesBig:
                case NPCID.AngryBonesBigHelmet:
                case NPCID.AngryBonesBigMuscle:
                    npcLoot.Add(ItemDropRule.Common(ItemType<CoreBoneRing>(), 200));
                    npcLoot.Add(ItemDropRule.Common(ItemType<RemainsOfSamurai>(), 100));
                    break;
                case NPCID.Medusa://美杜莎掉落美杜莎套装
                    {
                        IItemDropRule[] MedusaTypes =
                        [
                            ItemDropRule.Common(ItemType<MedusaMask>(), 1),
                            ItemDropRule.Common(ItemType<MedusaLightArmor>(), 1),
                            ItemDropRule.Common(ItemType<MedusaSlippers>(), 1),
                        ];
                        LeadingConditionRule leadingConditionRule = new(new Conditions.IsHardmode());

                        leadingConditionRule.OnSuccess(new FewFromRulesRule(1, 15, MedusaTypes));
                        npcLoot.Add(leadingConditionRule);
                        npcLoot.Add(ItemDropRule.Common(ItemType<LegendaryCard>(), 15));
                    }
                    break;
                case NPCID.TheDestroyer://毁灭者在天顶世界掉落美杜莎鞋
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.ZenithSeedIsUp(), ItemType<MedusaSlippers>(), 1));
                    break;
                case NPCID.Retinazer://双子魔眼在天顶世界掉落美杜莎面罩
                case NPCID.Spazmatism://双子魔眼在天顶世界掉落美杜莎面罩
                    {
                        LeadingConditionRule leadingConditionRule = new(new Conditions.MissingTwin());
                        leadingConditionRule.OnSuccess(ItemDropRule.ByCondition(new Conditions.ZenithSeedIsUp(), ItemType<MedusaMask>(), 1));

                        npcLoot.Add(leadingConditionRule);
                    }
                    break;
                case NPCID.BoneLee://骷髅李掉落家门旗
                    npcLoot.Add(ItemDropRule.ExpertGetsRerolls(ItemType<KamonFlag>(), 12, 1));
                    break;
                case NPCID.SkeletronPrime://机械佝偻王在天顶世界掉落美杜莎轻甲
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.ZenithSeedIsUp(), ItemType<MedusaLightArmor>(), 1));
                    break;
                case NPCID.MartianSaucerCore://火星飞碟掉落盾冲饰品
                    npcLoot.Add(ItemDropRule.Common(ItemType<PiezoArmorPanel>(), 3, 1, 1));
                    npcLoot.Add(ItemDropRule.Common(ItemType<SolarPanel>(), 3, 1, 1));
                    break;
                case NPCID.MoonLordCore:
                    foreach (var rule in npcLoot.Get())
                        if (rule is LeadingConditionRule leading)
                        {
                            foreach (var rule2 in leading.ChainedRules)
                            {
                                if (rule2 is Chains.TryIfSucceeded tis && tis.RuleToChain is FromOptionsWithoutRepeatsDropRule f && f.dropIds.Contains(ItemID.Meowmere))
                                {
                                    var original = f.dropIds.ToList();
                                    original.Add(ItemType<ConquerorOfTheSeas>());
                                    original.Add(ItemType<Aurora>());
                                    f.dropIds = [.. original];
                                    break;
                                }
                            }

                            break;
                        }

                    npcLoot.Add(ItemDropRule.Common(ItemType<Night_luminescentPearl>(), 1, 1, 3));
                    break;
                case NPCID.MoonLordHand:
                case NPCID.MoonLordHead:
                    npcLoot.Add(ItemDropRule.Common(ItemType<Night_luminescentPearl>()));
                    break;
                case NPCID.DD2OgreT2://二级食人魔
                    npcLoot.Add(ItemDropRule.Common(ItemType<EtheriaLegacy>(), 10));
                    break;
                case NPCID.DD2OgreT3://三级食人魔
                    npcLoot.Add(ItemDropRule.Common(ItemType<EtheriaLegacy>(), 20));
                    break;
                case NPCID.Deerclops://巨鹿
                    npcLoot.Add(ItemDropRule.Common(ItemType<ChocomintIce>(), 3));
                    break;
                case NPCID.Demon:
                    npcLoot.Add(ItemDropRule.Common(ItemType<FriedShrimpWhip>(), 40));
                    break;
                case NPCID.LavaSlime:
                case NPCID.Lavabat:
                    npcLoot.Add(ItemDropRule.Common(ItemType<FriedShrimpWhip>(), 100));
                    break;
                case NPCID.Pumpking://南瓜王掉落黄油
                    npcLoot.Add(ItemDropRule.Common(ItemType<Butter>(), 3));
                    break;
            }

            if (Main.slimeRainNPC[npc.type])
                npcLoot.Add(ItemDropRule.Common(ItemType<SlimeSapling>(), 50));
        }
    }
}
