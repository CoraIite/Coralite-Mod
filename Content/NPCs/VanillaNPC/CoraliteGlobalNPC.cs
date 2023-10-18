using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.Botanical.Seeds;
using Coralite.Content.Items.Gels;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.Misc;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.Placeable;
using Coralite.Content.Items.Shadow;
using Coralite.Content.Items.YujianHulu;
using Coralite.Content.ModPlayers;
using Coralite.Core.Configs;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.NPCs.VanillaNPC
{
    public class CoraliteGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                default: break;
                case NPCID.DemonEye:
                case NPCID.DemonEye2:
                    npcLoot.Add(ItemDropRule.Common(ItemType<EyeballSeed>(), 50));
                    break;
                case NPCID.WanderingEye:
                    npcLoot.Add(ItemDropRule.Common(ItemType<EyeballSeed>(), 25));
                    break;

                case NPCID.DarkCaster:
                    npcLoot.Add(ItemDropRule.Common(ItemType<ShadowEnergy>(), 3, 1, 3));
                    break;

                case NPCID.EaterofSouls:
                case NPCID.CorruptGoldfish:
                case NPCID.DevourerHead:
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientDemoniteYujian>(), 100));
                    break;

                case NPCID.Crimera:
                case NPCID.FaceMonster:
                case NPCID.BloodCrawler:
                case NPCID.CrimsonGoldfish:
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientCrimtaneYujian>(), 100));
                    break;
                case NPCID.HallowBoss:
                    npcLoot.Add(ItemDropRule.ByCondition(new DownedGolemCondition(), ItemType<FragmentsOfLight>(), 1, 3, 5));
                    break;
                case NPCID.DukeFishron:
                    npcLoot.Add(ItemDropRule.ByCondition(new DownedGolemCondition(), ItemType<DukeFishronSkin>(), 1, 3, 5));
                    break;
                case NPCID.EyeballFlyingFish:
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

            if (npc.HasBuff<EdenDebuff>() && !ProjectileID.Sets.IsAWhip[projectile.type])
            {
                npc.DelBuff(npc.FindBuffIndex(BuffType<EdenDebuff>()));
                modifiers.FlatBonusDamage += EdenDebuff.TagDamage * projTagMultiplier;

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    Vector2 direction = (npc.Center-projectile.Center).SafeNormalize(-Vector2.UnitY);

                    Helper.SpawnDirDustJet(projectile.Center+npc.Center/2, () => direction.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)), 2, 3,
                        (i) => i * 0.7f * Main.rand.NextFloat(0.7f, 1.1f), DustType<NightmarePetal>(), newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(0.6f, 0.8f), noGravity: false, extraRandRot: 0.2f);
                }
            }
        }
    }
}
