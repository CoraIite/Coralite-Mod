using Coralite.Content.Biomes;
using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.Accessories.FlyingShields;
using Coralite.Content.Items.Botanical.Seeds;
using Coralite.Content.Items.CoreKeeper;
using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Gels;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.Misc;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.Placeable;
using Coralite.Content.Items.YujianHulu;
using Coralite.Core.Configs;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.NPCs.VanillaNPC
{
    public class CoraliteGlobalNPC : GlobalNPC
    {
        public bool IvyPosion;

        public override bool InstancePerEntity => true;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (IvyPosion)
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = (int)(0.25f * npc.lifeRegen);
        }

        public override void ResetEffects(NPC npc)
        {
            IvyPosion = false;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                default: break;
                case NPCID.DemonEye://恶魔眼
                case NPCID.DemonEye2:
                    npcLoot.Add(ItemDropRule.Common(ItemType<EyeballSeed>(), 50));
                    break;
                case NPCID.WanderingEye:
                    npcLoot.Add(ItemDropRule.Common(ItemType<EyeballSeed>(), 25));
                    break;
                case NPCID.CaveBat:
                    npcLoot.Add(ItemDropRule.Common(ItemType<BatfangShield>(), 80, 1, 1));
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
                case NPCID.Mimic:
                    npcLoot.Add(ItemDropRule.Common(ItemType<EekShield>(), 10));
                    break;
                case NPCID.HallowBoss://光女掉落圣光残片
                    npcLoot.Add(ItemDropRule.ByCondition(new DownedGolemCondition(), ItemType<FragmentsOfLight>(), 1, 3, 5));
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
                case NPCID.Mothron://蛾怪掉落腐朽的盾
                    npcLoot.Add(ItemDropRule.Common(ItemType<RustedShield>(), 4, 1, 1));
                    break;
                case NPCID.PirateCaptain://海盗船长掉落海盗套装
                    {
                        IItemDropRule[] PirateKingTypes = new IItemDropRule[]
                        {
                            ItemDropRule.Common(ItemType<PirateKingHat>(), 1, 1, 1),
                            ItemDropRule.Common(ItemType<PirateKingCoat>(), 1, 1, 1),
                            ItemDropRule.Common(ItemType<PirateKingShoes>(), 1, 1, 1),
                        };
                        npcLoot.Add(new FewFromRulesRule(1, 5, PirateKingTypes));
                    }
                    break;

                case NPCID.EyeofCthulhu://克眼，脑子，世吞掉落美味肉排
                case NPCID.BrainofCthulhu://克眼，脑子，世吞掉落美味肉排
                    npcLoot.Add(ItemDropRule.Common(ItemType<DeliciousSteak>(), 6, 1, 1));
                    break;
                case NPCID.EaterofWorldsBody://克眼，脑子，世吞掉落美味肉排
                case NPCID.Creeper://克眼，脑子，世吞掉落美味肉排
                    npcLoot.Add(ItemDropRule.Common(ItemType<DeliciousSteak>(), 50, 1, 1));
                    break;

                case NPCID.FlyingSnake://羽蛇掉毛
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.DownedPlantera(), ItemType<FlyingSnakeFeather>(), 1, 1, 2));
                    break;
                case NPCID.WallofFlesh://肉山和鸟妖掉落破碎剑柄
                    npcLoot.Add(ItemDropRule.Common(ItemType<BrokenHandle>(), 10, 1, 1));
                    break;
                case NPCID.Harpy://女妖额外掉落天空戒指
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemType<BrokenHandle>(), 150, 1, 1));
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.DownedPlantera(), ItemType<SkyRing>(), 50, 1, 1));
                    break;

                case NPCID.RockGolem://岩石巨人,花岗岩敌怪，附魔剑 掉落上古宝石
                case NPCID.GraniteFlyer:
                case NPCID.EnchantedSword:
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientGemstone>(), 20, 1, 3));
                    break;
                case NPCID.GraniteGolem://花岗岩巨人额外掉落上古守护者饰品
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientGemstone>(), 20, 1, 3));
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientGuardianNecklace>(), 250, 1, 1));
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientGuardianRing>(), 150, 1, 1));
                    break;

                case NPCID.AngryBones://愤怒骷髅掉落骨质戒指
                case NPCID.AngryBonesBig:
                case NPCID.AngryBonesBigHelmet:
                case NPCID.AngryBonesBigMuscle:
                    npcLoot.Add(ItemDropRule.Common(ItemType<CoreBoneRing>(), 200, 1, 1));
                    npcLoot.Add(ItemDropRule.Common(ItemType<RemainsOfSamurai>(), 100, 1, 1));
                    break;
                case NPCID.Medusa://美杜莎掉落美杜莎套装
                    {
                        IItemDropRule[] PirateKingTypes = new IItemDropRule[]
                        {
                            ItemDropRule.Common(ItemType<MedusaMask>(), 1, 1, 1),
                            ItemDropRule.Common(ItemType<MedusaLightArmor>(), 1, 1, 1),
                            ItemDropRule.Common(ItemType<MedusaSlippers>(), 1, 1, 1),
                        };
                        LeadingConditionRule leadingConditionRule = new LeadingConditionRule(new Conditions.IsHardmode());

                        leadingConditionRule.OnSuccess(new FewFromRulesRule(1, 25, PirateKingTypes));
                        npcLoot.Add(leadingConditionRule);
                    }
                    break;
                case NPCID.TheDestroyer://毁灭者在天顶世界掉落美杜莎鞋
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.ZenithSeedIsUp(), ItemType<MedusaSlippers>(), 1));
                    break;
                case NPCID.Retinazer://双子魔眼在天顶世界掉落美杜莎面罩
                case NPCID.Spazmatism://双子魔眼在天顶世界掉落美杜莎面罩
                    {
                        LeadingConditionRule leadingConditionRule = new LeadingConditionRule(new Conditions.MissingTwin());
                        leadingConditionRule.OnSuccess(ItemDropRule.ByCondition(new Conditions.ZenithSeedIsUp(), ItemType<MedusaMask>(), 1));

                        npcLoot.Add(leadingConditionRule);
                    }
                    break;
                case NPCID.BoneLee:
                    npcLoot.Add(ItemDropRule.Common(ItemType<KamonFlag>(), 12, 1, 1));
                    break;
                case NPCID.SkeletronPrime://机械佝偻王在天顶世界掉落美杜莎轻甲
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.ZenithSeedIsUp(), ItemType<MedusaLightArmor>(), 1));
                    break;
                case NPCID.MartianSaucerCore://火星飞碟掉落盾冲饰品
                    npcLoot.Add(ItemDropRule.Common(ItemType<PiezoArmorPanel>(), 4, 1, 1));
                    npcLoot.Add(ItemDropRule.Common(ItemType<SolarPanel>(), 4, 1, 1));
                    break;
                case NPCID.MoonLordCore:
                    npcLoot.Add(ItemDropRule.Common(ItemType<ConquerorOfTheSeas>(), 9, 1, 1));
                    break;
            }

            if (Main.slimeRainNPC[npc.type])
                npcLoot.Add(ItemDropRule.Common(ItemType<SlimeSapling>(), 50));
        }

        public override void ModifyShop(NPCShop shop)
        {
            switch (shop.NpcType)
            {
                case NPCID.ArmsDealer:
                    {
                        shop.Add<AncientCore>(Condition.DownedPlantera);//远古核心
                        break;
                    }
                //case NPCID.TravellingMerchant://游商
                //    {
                //        shop.Add<TravelJournaling>();//手记
                //        shop.Add(ItemID.GlowTulip, Condition.Hardmode);//发光郁金香
                //        shop.Add<MineShield>(Condition.Hardmode);//我的盾牌
                //        shop.Add<RuneParchment>(Condition.DownedPlantera);//花后获取符文羊皮纸
                //    }
                //    break;
                default: break;
            }
        }

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (npc.type == NPCID.TravellingMerchant)//游商
            {
                int i = 0;
                for (; i < items.Length - 1; i++)
                {
                    if (items[i] == null || items[i].IsAir)
                        break;
                }

                items[i] = new Item(ItemType<TravelJournaling>());
                i++;

                if (Main.hardMode)
                {
                    items[i] = new Item(ItemID.GlowTulip);
                    i++;
                    items[i] = new Item(ItemType<MineShield>());
                    i++;
                }

                if (NPC.downedPlantBoss)//花后获取符文羊皮纸
                {
                    items[i] = new Item(ItemType<RuneParchment>());
                    i++;
                }
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;

            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<GelWhipDebuff>())
                modifiers.FlatBonusDamage += GelWhipDebuff.TagDamage * projTagMultiplier;

            if (npc.HasBuff<EdenDebuff>())
            {
                modifiers.FlatBonusDamage += EdenDebuff.TagDamage * projTagMultiplier;
                if (Main.rand.NextBool(14, 100))
                    modifiers.SetCrit();

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    Vector2 direction = (npc.Center - projectile.Center).SafeNormalize(-Vector2.UnitY);

                    Helper.SpawnDirDustJet(projectile.Center + npc.Center / 2, () => direction.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)), 2, 2,
                        (i) => i * 0.7f * Main.rand.NextFloat(0.7f, 1.1f), DustType<NightmarePetal>(), newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(0.6f, 0.8f), noGravity: false, extraRandRot: 0.2f);
                }
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<MagicCrystalCave>())
            {
                pool[0] = 0.04f;
            }

            if (spawnInfo.Player.InModBiome<ShadowCastleBiome>())
                pool[0] = 0f;
        }

    }
}
