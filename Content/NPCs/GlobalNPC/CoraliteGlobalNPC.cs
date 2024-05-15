using Coralite.Content.Biomes;
using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.CoreKeeper;
using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Gels;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.Thunder;
using Coralite.Content.WorldGeneration;
using Coralite.Core.Configs;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.NPCs.GlobalNPC
{
    public partial class CoraliteGlobalNPC : Terraria.ModLoader.GlobalNPC
    {
        public bool IvyPosion;
        public bool EuphorbiaPoison;
        public bool ThunderElectrified;
        public bool PollenFire;

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
                        if (CoraliteWorld.coralCatWorld)
                        {
                            entity.scale = Main.rand.NextFloat(2, 3.5f);
                            entity.lifeMax = (int)(entity.lifeMax * Main.rand.NextFloat(3, 5));
                        }
                    }
                    break;
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            switch (npc.type)
            {
                default:
                    break;
                case NPCID.Shark:
                case NPCID.SandShark:
                case NPCID.SandsharkCorrupt:
                case NPCID.SandsharkCrimson:
                case NPCID.SandsharkHallow:
                    {
                    }
                    break;
            }
        }

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

                int damageCount = (int)(10 + npc.velocity.Length() * 1.5f);
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
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
        }

        public override void ModifyShop(NPCShop shop)
        {
            switch (shop.NpcType)
            {
                //case NPCID.ArmsDealer:
                //    {
                //        shop.Add<AncientCore>(Condition.DownedPlantera);//远古核心
                //        break;
                //    }
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
                if (i >= items.Length)
                    return;

                if (Main.hardMode)
                {
                    items[i] = new Item(ItemID.GlowTulip);
                    i++;
                    if (i >= items.Length)
                        return;

                    items[i] = new Item(ItemType<MineShield>());
                    i++;
                    if (i >= items.Length)
                        return;

                }

                if (NPC.downedPlantBoss)//花后获取符文羊皮纸
                {
                    items[i] = new Item(ItemType<RuneParchment>());
                    i++;
                    if (i >= items.Length)
                        return;

                }

                if (NPC.downedMoonlord)//月后卖海盾
                {
                    items[i] = new Item(ItemType<HylianShield>());
                    i++;
                    if (i >= items.Length)
                        return;

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
            if (npc.HasBuff<ThunderWhipDebuff>())
                modifiers.FlatBonusDamage += ThunderWhipDebuff.TagDamage * projTagMultiplier;

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

            if (CoraliteWorld.coralCatWorld && spawnInfo.Player.wet && spawnInfo.Player.position.Y < Main.worldSurface * 16)
            {
                pool.Add(NPCID.Shark, 1);
            }
        }

        public override bool PreKill(NPC npc)
        {
            if (npc.type == NPCID.RainbowSlime && CoraliteWorld.coralCatWorld)
            {
                return false;
            }

            return base.PreKill(npc);
        }
    }
}
