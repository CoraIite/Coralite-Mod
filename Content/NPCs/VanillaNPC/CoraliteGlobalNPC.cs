using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.Botanical.Seeds;
using Coralite.Content.Items.CoreKeeper;
using Coralite.Content.Items.Gels;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.Misc;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.Placeable;
using Coralite.Content.Items.Shadow;
using Coralite.Content.Items.YujianHulu;
using Coralite.Core.Configs;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
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

                case NPCID.DarkCaster://地牢怪掉落影子
                    npcLoot.Add(ItemDropRule.Common(ItemType<ShadowEnergy>(), 3, 1, 3));
                    break;

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

                case NPCID.FlyingSnake://羽蛇掉毛
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.DownedPlantera(), ItemType<FlyingSnakeFeather>(), 1, 1, 2));
                    break;
                case NPCID.WallofFlesh://肉山和鸟妖掉落破碎剑柄
                    npcLoot.Add(ItemDropRule.Common(ItemType<BrokenHandle>(), 10, 1, 1));
                    break;
                case NPCID.Harpy:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(),ItemType<BrokenHandle>(), 100, 1, 1));
                    break;

                case NPCID.RockGolem://岩石巨人,花岗岩敌怪，附魔剑 掉落上古宝石
                case NPCID.GraniteFlyer:
                case NPCID.GraniteGolem:
                case NPCID.EnchantedSword:
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientGemstone>(), 20, 1, 3));
                    break;

            }

            if (Main.slimeRainNPC[npc.type])
                npcLoot.Add(ItemDropRule.Common(ItemType<SlimeSapling>(), 50));
        }

        public override void ModifyShop(NPCShop shop)
        {
            //if (shop.NpcType == NPCID.TravellingMerchant)   //游商售卖旅行手记
            //{
            //    shop.Add<TravelJournaling>();
            //}
        }

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            switch (npc.type)
            {
                case NPCID.ArmsDealer:
                    {
                        if (NPC.downedPlantBoss)    //花后售卖远古核心
                        {
                            int i = 0;
                            for (; i < items.Length - 1; i++)
                            {
                                if (items[i] == null || items[i].IsAir)
                                    break;
                            }

                            items[i] = new Item(ItemType<AncientCore>());
                        }
                        break;
                    }
                case NPCID.TravellingMerchant:
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
                        }

                        if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                        {
                            items[i] = new Item(ItemType<RuneParchment>());
                            i++;
                        }
                    }
                    break;
                default: break;
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
    }
}
