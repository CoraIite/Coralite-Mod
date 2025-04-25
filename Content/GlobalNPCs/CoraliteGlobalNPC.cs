using Coralite.Content.Biomes;
using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.CoraliteNotes.ThunderChapter1;
using Coralite.Content.Items.Donator;
using Coralite.Content.Items.Gels;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.Thunder;
using Coralite.Content.WorldGeneration;
using Coralite.Core.Configs;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.GlobalNPCs
{
    public partial class CoraliteGlobalNPC : GlobalNPC
    {
        public bool IvyPosion;
        public bool EuphorbiaPoison;
        public bool ThunderElectrified;
        public bool PollenFire;
        /// <summary>
        /// 血印礼帽的黑曜石套装效果造成的护甲破坏
        /// </summary>
        public bool PrisonArmorBreak;

        public bool StopHitPlayer;
        public float SlowDownPercent;

        public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC entity)
        {
            switch (entity.type)
            {
                default:
                    break;
                case NPCID.Shark:
                case NPCID.SandShark:
                case NPCID.SandsharkCorrupt:
                case NPCID.SandsharkCrimson:
                case NPCID.SandsharkHallow:
                    {
                        if (CoraliteWorld.CoralCatWorld)
                        {
                            entity.scale = Main.rand.NextFloat(2, 3.5f);
                            entity.lifeMax = (int)(entity.lifeMax * Main.rand.NextFloat(3, 5));
                        }
                    }
                    break;
            }
        }

        //public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        //{
        //    switch (npc.type)
        //    {
        //        default:
        //            break;
        //        case NPCID.Shark:
        //        case NPCID.SandShark:
        //        case NPCID.SandsharkCorrupt:
        //        case NPCID.SandsharkCrimson:
        //        case NPCID.SandsharkHallow:
        //            {
        //            }
        //            break;
        //    }
        //}

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (IvyPosion)
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = (int)(0.25f * npc.lifeRegen);

            if (EuphorbiaPoison)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 4 * 100;
                if (damage < 100 / 2)
                    damage = 100 / 2;
            }

            if (PollenFire)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                int damageCount = 2;
                npc.lifeRegen -= damageCount * 8;
                if (damage < damageCount)
                    damage = damageCount;
            }

            if (ThunderElectrified)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                int damageCount = (int)(10 + (npc.velocity.Length() * 1.5f));
                if (damageCount > 30)
                    damageCount = 30;
                npc.lifeRegen -= damageCount * 8;
                if (damage < damageCount)
                    damage = damageCount;
            }
        }

        public override void ResetEffects(NPC npc)
        {
            IvyPosion = false;
            EuphorbiaPoison = false;
            ThunderElectrified = false;
            PollenFire = false;
            StopHitPlayer = false;
            PrisonArmorBreak = false;
            SlowDownPercent = 0;
        }

        public override void PostAI(NPC npc)
        {
            if (!npc.boss && SlowDownPercent > 0)
            {
                npc.velocity *= Helper.Lerp(SlowDownPercent, 1, Math.Clamp(1 - npc.knockBackResist, 0, 1));
            }
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (StopHitPlayer)
            {
                return false;
            }

            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (PrisonArmorBreak)
                modifiers.ArmorPenetration += 8;

            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;

            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<GelWhipDebuff>())
                modifiers.FlatBonusDamage += GelWhipDebuff.TagDamage * projTagMultiplier;
            if (npc.HasBuff<ThunderWhipDebuff>())
                modifiers.FlatBonusDamage += ThunderWhipDebuff.TagDamage * projTagMultiplier;

            if (npc.HasBuff<FriedShrimpBuff>())
            {
                modifiers.FlatBonusDamage += FriedShrimpBuff.TagDamage * projTagMultiplier;
                if (projectile.type == ProjectileType<ChocomintIceProj>())
                    modifiers.ArmorPenetration += 10;
            }

            if (npc.HasBuff<EdenDebuff>())
            {
                modifiers.FlatBonusDamage += EdenDebuff.TagDamage * projTagMultiplier;
                if (Main.rand.NextBool(14, 100))
                    modifiers.SetCrit();

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    Vector2 direction = (npc.Center - projectile.Center).SafeNormalize(-Vector2.UnitY);

                    Helper.SpawnDirDustJet(projectile.Center + (npc.Center / 2), () => direction.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)), 2, 2,
                        (i) => i * 0.7f * Main.rand.NextFloat(0.7f, 1.1f), DustType<NightmarePetal>(), newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(0.6f, 0.8f), noGravity: false, extraRandRot: 0.2f);
                }
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (PrisonArmorBreak)
                modifiers.ArmorPenetration += 8;
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<MagicCrystalCave>())
                pool[0] = 0.25f;

            if (spawnInfo.Player.InModBiome<CrystallineSkyIsland>() || spawnInfo.Player.InModBiome<ShadowCastleBiome>())
                pool[0] = 0f;

            if (CoraliteWorld.CoralCatWorld && spawnInfo.Player.wet && spawnInfo.Player.position.Y < Main.worldSurface * 16)
                pool.Add(NPCID.Shark, 1);
        }

        public override bool PreKill(NPC npc)
        {
            if (npc.type == NPCID.RainbowSlime && CoraliteWorld.CoralCatWorld)
            {
                return false;
            }

            return base.PreKill(npc);
        }

        public override void OnKill(NPC npc)
        {
            switch (npc.type)
            {
                default:
                    break;
                case NPCID.Retinazer://检测新三王的击杀
                case NPCID.Spazmatism:
                case NPCID.SkeletronPrime:
                case NPCID.TheDestroyer:
                    {
                        bool mechBoss1 = NPC.downedMechBoss1 || npc.type == NPCID.TheDestroyer;
                        bool mechBoss2 = NPC.downedMechBoss2 || (npc.type == NPCID.Retinazer && !NPC.AnyNPCs(NPCID.Spazmatism)) || (npc.type == NPCID.Spazmatism && !NPC.AnyNPCs(NPCID.Retinazer));
                        bool mechBoss3 = NPC.downedMechBoss3 || npc.type == NPCID.SkeletronPrime;

                        if (Main.netMode != NetmodeID.MultiplayerClient && Main.hardMode && mechBoss1 && mechBoss2 && mechBoss3)
                            KnowledgeSystem.CheckForUnlock<Thunder1Knowldege>(npc.Center, Coralite.ThunderveinYellow);
                    }

                    break;
            }
        }


        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (PrisonArmorBreak)
                return new Color(102, 92, 194);

            return base.GetAlpha(npc, drawColor);
        }
    }
}
