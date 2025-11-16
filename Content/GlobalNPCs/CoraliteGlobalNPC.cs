using Coralite.Content.Biomes;
using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.CoraliteNotes.NightmareChapter;
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
        public bool RainbowBonus;
        public float SlowDownPercent;

        public override bool InstancePerEntity => true;


        public override void SetDefaults(NPC entity)
        {
            if (!Main.gameMenu && CoraliteWorld.CoralCatWorld)
                CoralCatWorldChange(entity);
        }

        private void CoralCatWorldChange(NPC entity)
        {
            switch (entity.type)
            {
                default:
                    break;
                case NPCID.RainbowSlime:
                    entity.scale = 0.5f;
                    break;
                case NPCID.Shark:
                case NPCID.SandShark:
                case NPCID.SandsharkCorrupt:
                case NPCID.SandsharkCrimson:
                case NPCID.SandsharkHallow:
                    {
                        entity.scale = Main.rand.NextFloat(2, 3.5f);
                        entity.lifeMax = (int)(entity.lifeMax * Main.rand.NextFloat(3, 5));
                    }
                    break;
                case NPCID.BlueJellyfish:
                case NPCID.PinkJellyfish:
                case NPCID.GreenJellyfish:
                case NPCID.Squid:
                case NPCID.Crab:
                case NPCID.SeaSnail:
                    {
                        entity.scale = Main.rand.NextFloat(0.35f, 3.5f);
                        entity.lifeMax = (int)(entity.lifeMax * Main.rand.NextFloat(3, 5));
                    }
                    break;
            }

            if (!entity.boss)
            {
                RainbowBonus = Main.rand.NextBool(100);
                if (RainbowBonus)
                {
                    entity.lifeMax *= 3;
                    entity.damage *= 3;
                    entity.defense *= 2;
                    entity.value *= 10;
                    entity.scale *= 0.75f;
                }
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

            UpdateAttachProj(npc);
        }


        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (!npc.boss && StopHitPlayer)
                return false;

            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (PrisonArmorBreak)
                modifiers.ArmorPenetration += 8;

            ModifyHitByProj_AttachProj(npc, projectile, ref modifiers);

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
            {
                pool.Add(NPCID.Shark, 0.8f);
                pool.Add(NPCID.BlueJellyfish, 0.33f);
                pool.Add(NPCID.GreenJellyfish, 0.33f);
                pool.Add(NPCID.PinkJellyfish, 0.33f);
                pool.Add(NPCID.Squid, 0.33f);
                pool.Add(NPCID.Crab, 0.33f);
                pool.Add(NPCID.SeaSnail, 0.33f);
            }
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
                            KnowledgeSystem.CheckForUnlock<Thunder1Knowledge>(npc.Center, Coralite.ThunderveinYellow);
                    }

                    break;
                case NPCID.MoonLordCore:
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        KnowledgeSystem.CheckForUnlock<NightmareKnowledge>(npc.Center, NightmarePlantera.nightPurple);
                    break;
            }

            if (RainbowBonus)//TODO: 同步
            {
                for (int i = 0; i < 3; i++)
                    NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.RainbowSlime);

                int drop1 = Main.rand.NextFromList(ItemID.RainbowBrick
                    , ItemID.RainbowBrickWall
                    , ItemID.RainbowWallpaper
                    , ItemID.RainbowTorch
                    , ItemID.RainbowFlare
                    , ItemID.RainbowCampfire
                    );

                Item.NewItem(npc.GetSource_FromThis(), npc.Center
                    , drop1, Main.rand.Next(1, 100));

                int drop2 = Main.rand.NextFromList(ItemID.RainbowDye
                    , ItemID.IntenseRainbowDye
                    , ItemID.LivingRainbowDye
                    , ItemID.MidnightRainbowDye
                    , ItemID.RainbowHairDye
                    , ItemID.RainbowString
                    );

                Item.NewItem(npc.GetSource_FromThis(), npc.Center
                    , drop2, 1);

                Item.NewItem(npc.GetSource_FromThis(), npc.Center
                    , drop2, Main.rand.Next(1, 100));

                int drop3 = Main.rand.NextFromList(ItemID.RainbowCursor
                        , ItemID.CopperShortsword, ItemID.TinShortsword
                        , ItemID.ZombieArm, ItemID.BladedGlove, ItemID.Flymeal
                        , ItemID.StylistKilLaKillScissorsIWish, ItemID.Ruler
                        , ItemID.Umbrella, ItemID.TragicUmbrella, ItemID.ChainKnife
                        , ItemID.CandyCaneSword, ItemID.BoneSword, ItemID.EnchantedSword
                        , ItemID.FalconBlade, ItemID.Rally, ItemID.Trident
                        , ItemID.FruitcakeChakram, ItemID.BloodyMachete, ItemID.Shroomerang
                        , ItemID.Terragrim
                        , ItemID.AshWoodBow, ItemID.RedRyder, ItemID.Revolver, ItemID.Minishark
                        , ItemID.PaperAirplaneA, ItemID.PaperAirplaneB
                        , ItemID.StarAnise, ItemID.MolotovCocktail
                        , ItemID.Sandgun, ItemID.AleThrowingGlove
                        , ItemID.WandofFrosting, ItemID.DemonScythe
                        , ItemID.SlimeStaff
                        );

                if (Main.hardMode && Main.rand.NextBool(2))
                {
                    drop3 = Main.rand.NextFromList(ItemID.SlapHand
                        , ItemID.Frostbrand, ItemID.BeamSword
                        , ItemID.TitaniumSword, ItemID.Bladetongue
                        , ItemID.HamBat
                        , ItemID.FormatC, ItemID.HiveFive
                        , ItemID.AdamantiteGlaive, ItemID.Bananarang, ItemID.DaoofPow
                        , ItemID.Arkhalis, ItemID.ShadowFlameBow
                        , ItemID.Gatligator, ItemID.Uzi
                        , ItemID.Toxikarp, ItemID.CoinGun
                        , ItemID.MeteorStaff, ItemID.SkyFracture
                        , ItemID.UnholyTrident, ItemID.GoldenShower, ItemID.NimbusRod
                        , ItemID.SpiritFlame, ItemID.MedusaHead, ItemID.SanguineStaff
                        , ItemID.CoolWhip, ItemID.BunnyCannon
                        , ItemID.LandMine
                        );
                }

                Item.NewItem(npc.GetSource_FromThis(), npc.Center
                    , drop3, 1);
            }
        }


        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (PrisonArmorBreak)
                return new Color(102, 92, 194);

            if (RainbowBonus)
                return Main.DiscoColor;

            return base.GetAlpha(npc, drawColor);
        }
    }
}
