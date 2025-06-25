using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Content.Items.RedJades;
using Coralite.Content.Items.Steel;
using Coralite.Content.Items.Thunder;
using Coralite.Content.Projectiles.Globals;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer : ModPlayer, ILocalizedModType
    {
        public string LocalizationCategory => "Players";

        public int rightClickReuseDelay = 0;

        public List<IInventoryCraftStation> inventoryCraftStations = new();

        public static LocalizedText CoreKeeperDodgeText { get; private set; }

        #region 装备类字段

        /// <summary> 花粉火药计时器 </summary>
        public byte PollenGunpowderEffect = 60;
        /// <summary> 玫瑰火药计时器 </summary>
        public byte RoseGunpowderEffect = 90;
        /// <summary> 迈达斯灰烬火药计时器 </summary>
        public byte MidasGunpowderEffect = 90;

        /// <summary> 海盗王之魂 </summary>
        public byte pirateKingSoul;
        /// <summary> 海盗王之魂的效果CD </summary>
        public byte pirateKingSoulCD;

        /// <summary> 幸运星 </summary>
        public byte luckyStar;

        /// <summary> 美杜莎之魂 </summary>
        public int medusaSoul;
        /// <summary> 分裂 </summary>
        public int split;

        /// <summary> 大田螺之魂 </summary>
        public int GreatRiverSnailSoul;//凉屋这命名也是挺直白的，虽说代码里甚至用的拼音
        /// <summary> 大田螺之魂的效果CD </summary>
        public int GreatRiverSnailSoulCD;
        /// <summary> 绝对专注 </summary>
        public int Concertration;
        #endregion

        /// <summary>
        /// 距离上次受伤的时间
        /// </summary>
        public int HurtTimer;
        public int CrystallineSkyIslandEffect;

        /// <summary> 爆伤加成 </summary>
        public float critDamageBonus;
        /// <summary> 回复量乘算加成 </summary>
        public float lifeReganBonus;
        /// <summary> 来自BOSS的伤害减免 </summary>
        public float bossDamageReduce;

        /// <summary> 地心护核者的闪避 </summary>
        public float coreKeeperDodge;

        /// <summary> 摔落伤害倍率 </summary>
        public StatModifier fallDamageModifyer = new();
        /// <summary> 生命上限加成 </summary>
        public StatModifier LifeMaxModifyer = new();

        /// <summary> 使用特殊攻击 </summary>
        public bool useSpecialAttack;

        public Vector2 oldOldVelocity;
        public Vector2 oldVelocity;
        public Vector2 oldOldCenter;
        public Vector2 oldCenter;

        /// <summary> 冷系伤害加成 </summary>
        public StatModifier coldDamageBonus;
        /// <summary> 美味伤害加成 </summary>
        public StatModifier deliciousDamageBonus;

        /// <summary> 宝石武器攻速加成 </summary>
        public StatModifier GemWeaponAttSpeedBonus;

        /// <summary> 储存御剑葫芦 </summary>
        public Item[] TempYujians;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            CoreKeeperDodgeText = this.GetLocalization(nameof(CoreKeeperDodgeText));
            LoadDeathReasons();
        }

        public override void Unload()
        {
            CoreKeeperDodgeText = null;
            UnloadDeathReasons();
        }

        public override void ResetEffects()
        {
            ResetCoraliteEffects();

            inventoryCraftStations ??= new List<IInventoryCraftStation>();
            inventoryCraftStations.Clear();

            CheckBloodPool();
            UpdateEmerorSlimeBoots();

            if (CrystallineSkyIslandEffect > 0)
                CrystallineSkyIslandEffect--;

            pirateKingSoul = 0;
            if (pirateKingSoulCD > 0)
                pirateKingSoulCD--;
            luckyStar = 0;
            medusaSoul = 0;
            split = 0;
            GreatRiverSnailSoul = 0;
            if (GreatRiverSnailSoulCD > 0)
                GreatRiverSnailSoulCD--;
            Concertration = 0;

            critDamageBonus = 0;
            lifeReganBonus = 0;
            bossDamageReduce = 0;

            fallDamageModifyer = new StatModifier();
            coldDamageBonus = new StatModifier();
            deliciousDamageBonus = new StatModifier();
            GemWeaponAttSpeedBonus = new StatModifier();

            coreKeeperDodge = 0;
            ResetNightmareEnergy();
            ResetFlyingShieldSets();
            UpdateParry();
            ResetDahsSets();
        }

        public override void OnRespawn()
        {
            ResetEmperorBoots();
            HurtTimer = 0;
            ResetNightmare_Respawn();
            bloodPoolCount = 0;
            TempYujians = new Item[BaseHulu.slotCount];
        }

        public override bool CanUseItem(Item item)
        {
            if (item.type == ItemID.RodOfHarmony)
            {
                if (NightmarePlantera.NightmarePlanteraAlive(out _))
                {
                    Rectangle rectangle = new((int)Player.Center.X, (int)Player.Center.Y, 2, 2);

                    CombatText.NewText(rectangle, Coralite.MagicCrystalPink, "协调的力量被临时封印了");
                    return false;
                }
            }

            return base.CanUseItem(item);
        }

        #region 各种更新

        public override void PreUpdate()
        {
            ResetYujianNianli();
        }

        public override void PreUpdateMovement()
        {
            SetStartDash();
            if (HasEffect(nameof(Items.Gels.EmperorSlimeBoots))
                && !Player.mount.Active && Player.grappling[0] == -1 && !Player.tongued && !Player.shimmering)
                EmperorSlimeMove();
        }

        public override void PreUpdateBuffs()
        {
            if (HurtTimer < 60 * 60)//更新受击时间
                HurtTimer++;

            if (Player.HeldItem.ModItem is IBuffHeldItem buffHeldItem)
                buffHeldItem.UpdateBuffHeldItem(Player);

            int tempLifeMax = Player.statLifeMax2;
            Player.statLifeMax2 = (int)LifeMaxModifyer.ApplyTo(Player.statLifeMax2);//防止一些意外事故
            if (Player.statLifeMax2 < 1)
                Player.statLifeMax2 = tempLifeMax;

            LifeMaxModifyer = new StatModifier();
        }

        public override void ModifyLuck(ref float luck)
        {
            if (luckyStar > 1)//幸运星增加玩家幸运
                luck += 0.2f;
            else if (luckyStar > 2)
                luck += 0.3f;
        }

        public override void PostUpdateEquips()
        {
            if (Player.HeldItem.ModItem is IEquipHeldItem ehi)
                ehi.UpdateEquipHeldItem(Player);

            UpdateNianli();
            UpdateFlyingShield();

            LimitNightmareEnergy();
        }


        public override void PostUpdateMiscEffects()
        {
            UpdateNianliUI();

            if (HasEffect(nameof(MedalOfLife)) && Player.statLifeMax2 - Player.statLife < 20)
            {
                if (Main.rand.NextBool(15))
                    Gore.NewGore(Player.GetSource_FromThis(),
                        Player.MountedCenter + Main.rand.NextVector2Circular(16, 24), -Vector2.UnitY
                        , 331);

                Player.GetDamage(DamageClass.Generic) += 0.07f;
                Player.GetAttackSpeed(DamageClass.Generic) += 0.05f;
                Player.moveSpeed += 0.05f;
            }

            if (HasEffect(nameof(CharmOfIsis)) && Player.statLifeMax2 - Player.statLife < 40)
            {
                if (Main.rand.NextBool(15))
                    Gore.NewGore(Player.GetSource_FromThis(),
                        Player.MountedCenter + Main.rand.NextVector2Circular(16, 24), -Vector2.UnitY
                        , 331);

                Player.GetDamage(DamageClass.Generic) += 0.10f;
                Player.GetAttackSpeed(DamageClass.Generic) += 0.05f;
                Player.moveSpeed += 0.08f;
            }

            if (HasEffect(nameof(LifePulseDevice)) && Player.statLife <= 40)
            {
                Player.GetDamage(DamageClass.Generic) *= 1.1f;
                Player.GetCritChance(DamageClass.Generic) += 4f;

                if (Main.rand.NextBool(3))
                {
                    Dust d = Dust.NewDustPerfect(Player.MountedCenter + Main.rand.NextVector2Circular(16, 24)
                        , DustID.Smoke, -Vector2.UnitY * Main.rand.NextFloat(1, 2), newColor: Color.Black, Scale: Main.rand.NextFloat(1, 1.75f));
                    d.noGravity = true;
                }
            }

            if (HasEffect(nameof(OsirisPillar)) && Player.statLife <= 80)
            {
                Player.GetDamage(DamageClass.Generic) *= 1.15f;
                Player.GetCritChance(DamageClass.Generic) += 6f;

                if (Main.rand.NextBool(3))
                {
                    Dust d = Dust.NewDustPerfect(Player.MountedCenter + Main.rand.NextVector2Circular(16, 24)
                        , DustID.Smoke, -Vector2.UnitY * Main.rand.NextFloat(1, 2), newColor: Color.Black, Scale: Main.rand.NextFloat(1, 1.75f));
                    d.noGravity = true;
                }
            }

            //为什么在这里呢，因为在这里才能覆盖掉原版冲刺
            //所以tml什么时候加个ModDash？？？？
            UpdateDash();
        }

        public override void UpdateLifeRegen()
        {
            if (HasEffect(Items.Gels.EmperorSlimeBoots.DefenceSet) && EmperorDefence > EmperorDefenctMax / 2)
                Player.lifeRegen += 4;

            Player.lifeRegen = (int)(Player.lifeRegen * (1 + lifeReganBonus));

            if (HasEffect(Items.Misc_Equip.BloodmarkTopper.BloodSet))
                Player.lifeRegenTime += 1f;
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            if (HasEffect(Items.Misc_Equip.BloodmarkTopper.BloodSet))
                regen *= 1.5f;
        }

        public override void UpdateBadLifeRegen()
        {
            if (HasEffect(nameof(ThunderElectrified)))
            {
                ClearGoodLifeRegan();

                Player.lifeRegenTime = 0;
                int damage = (int)(3 + (Player.velocity.Length() * 1.5f));

                if (damage > 15)
                    damage = 15;

                Player.lifeRegen -= damage * 2;
            }

            if (HasEffect(nameof(LifePulseDevice)))
            {
                ClearGoodLifeRegan();

                if (Player.statLife > 38)
                {
                    Player.lifeRegen -= 10 * 5;
                    Player.lifeRegenTime = 0;
                }
            }

            if (HasEffect(nameof(OsirisPillar)))
            {
                ClearGoodLifeRegan();

                if (Player.statLife > 80)
                {
                    Player.lifeRegen -= 10 * 8;
                    Player.lifeRegenTime = 0;
                }
            }
        }

        private void ClearGoodLifeRegan()
        {
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (HasEffect(Items.Misc_Equip.BloodmarkTopper.ShadowSet))
            {
                Player.runAcceleration *= 1.75f;
                Player.maxRunSpeed *= 1.15f;
                Player.accRunSpeed *= 1.15f;
                Player.runSlowdown *= 1.75f;
            }
        }

        public override void PostUpdate()
        {
            if (rightClickReuseDelay > 0)
                rightClickReuseDelay--;

            LimitNianli();
            oldOldVelocity = oldVelocity;
            oldVelocity = Player.velocity;

            oldOldCenter = oldCenter;
            oldCenter = Player.Center;
        }


        public override void UpdateDead()
        {
            NianliUIFade();

            rightClickReuseDelay = 0;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (source.Item.useAmmo == AmmoID.Bullet)
            {
                if (PollenGunpowderEffect == 0)
                {
                    if (damage > 60)
                        damage = 60;
                    Projectile.NewProjectile(source, position, velocity.RotateByRandom(-0.1f, 0.1f),
                        ProjectileType<Items.HyacinthSeries.PollenGunpowderProj>(), damage, knockback, Player.whoAmI);
                    PollenGunpowderEffect = 60;
                }

                if (RoseGunpowderEffect == 0)
                {
                    damage = (int)(damage * 1.35f);
                    if (damage > 110)
                        damage = 110;

                    Projectile.NewProjectile(source, position, velocity.RotateByRandom(-0.05f, 0.05f),
                        ProjectileType<Items.HyacinthSeries.RoseGunpowderProj>(), damage, knockback, Player.whoAmI);
                    RoseGunpowderEffect = 90;
                }

                if (MidasGunpowderEffect == 0)
                {
                    damage = (int)(damage * 1.35f);
                    if (damage > 185)
                        damage = 185;

                    Projectile.NewProjectile(source, position, velocity * 1.2f,
                        ProjectileType<Items.HyacinthSeries.MidasGunpowderProj>(), damage, knockback, Player.whoAmI);
                    MidasGunpowderEffect = 90;
                }
            }

            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }

        #endregion

        #region 受击与攻击

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            FlyingShieldHurt(ref modifiers);

            int tempHurtTime = HurtTimer;
            HurtTimer = 0;

            EmperorBootsHurt();

            modifiers.ModifyHurtInfo += Post;

            void Post(ref Player.HurtInfo info)
            {
                if (HasEffect(nameof(Items.MagikeSeries2.Luminward)))
                {
                    //Main.NewText(info.Damage);
                    if (info.Damage > 30)
                    {
                        info.SoundDisabled = true;
                        info.DustDisabled = true;
                        info.Damage = (int)(info.Damage * (1 - 0.2f));
                        //生成音效与粒子

                        Helper.PlayPitched(CoraliteSoundID.WindyBalloon_NPCDeath63, Player.Center);
                        Helper.PlayPitched(CoraliteSoundID.ShimmerContract, Player.Center);

                        var p = PRTLoader.NewParticle<Items.MagikeSeries2.LuminwardParticleExplosion>(Player.Center, Vector2.Zero, Color.White, 1);
                        p.player = Player;
                        p.Rotation = Main.rand.NextFloat(-0.4f, 0.4f);
                        p.effect = Main.rand.NextFromList(SpriteEffects.None, SpriteEffects.FlipHorizontally);
                    }
                    else
                        HurtTimer = tempHurtTime;
                }
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (proj.TryGetGlobalProjectile(out CoraliteGlobalProjectile cgp) && cgp.isBossProjectile)
                modifiers.SourceDamage -= bossDamageReduce;

            if (FlyingShieldGuardTime > 0)
                modifiers.ModifyHurtInfo += FlyingShield_DamageReduce;
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (npc.boss)
                modifiers.SourceDamage -= bossDamageReduce;

            if (FlyingShieldGuardTime > 0)
                modifiers.ModifyHurtInfo += FlyingShield_DamageReduce;
        }

        private void FlyingShield_DamageReduce(ref Player.HurtInfo info)
        {
            info.Damage = (int)(info.Damage * (1 - FlyingShieldDamageReduce));
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (HasEffect(nameof(Items.RedJades.RedJadePendant)) && Main.myPlayer == Player.whoAmI
                && hurtInfo.Damage > 5 && Main.rand.NextBool(3))
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor.First((item) => item.type == ItemType<Items.RedJades.RedJadePendant>())),
                    Player.Center + ((proj.Center - Player.Center).SafeNormalize(Vector2.One) * 16), Vector2.Zero, ProjectileType<Items.RedJades.RedJadeBoom>(), 80, 8f, Player.whoAmI);
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (HasEffect(nameof(Items.RedJades.RedJadePendant)) && Main.myPlayer == Player.whoAmI
                && hurtInfo.Damage > 5 && Main.rand.NextBool(3))
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor.First((item) => item.type == ItemType<Items.RedJades.RedJadePendant>())),
                    Player.Center + ((npc.Center - Player.Center).SafeNormalize(Vector2.One) * 16), Vector2.Zero, ProjectileType<Items.RedJades.RedJadeBoom>(), 80, 8f, Player.whoAmI);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += critDamageBonus;
            PriateKingSoulEffect(ref modifiers);
            MedusaSoulEffect(ref modifiers);

            if (HasEffect(nameof(ThunderveinNecklace)))
                target.AddBuff(BuffType<ThunderElectrified>(), 6 * 60);
            if (HasEffect(nameof(AlloySpringBuff)))
            {
                int defence = (int)Player.statDefense;
                modifiers.SourceDamage += 0.01f * Math.Clamp(defence / 5, 0, 15);
            }
            else if (HasEffect(nameof(GravitationalCatapultBuff)))
            {
                int defence = (int)Player.statDefense;
                modifiers.SourceDamage += 0.01f * Math.Clamp(defence / 4, 0, 30);
            }

            EmperorSlimeBootsHitNPC(target);

            if (Player.HasBuff<Items.Gels.EmperorSlimeBuff>())
                target.AddBuff(BuffID.Slimed, 60 * 5);

            #region 海盗王之魂的效果
            void PriateKingSoulEffect(ref NPC.HitModifiers modifiers)
            {
                if (pirateKingSoul < 2 || pirateKingSoulCD > 0)
                    return;

                int random = 20;
                float damageAdder = 1f;
                int CD = 120;
                if (pirateKingSoul > 2)//3件套效果强化
                {
                    random = 10;
                    damageAdder = 1.75f;
                    CD = 60;
                }
                if (Player.RollLuck(random) == random - 1)
                {
                    modifiers.SourceDamage += damageAdder;
                    modifiers.HideCombatText();
                    modifiers.ModifyHitInfo += PriateKingCombatText;

                    for (int i = 0; i < 3; i++)//爆金币粒子
                    {
                        int num36 = Gore.NewGore(new EntitySource_OnHit(Player, target), new Vector2(target.position.X, target.Center.Y - 10f), Vector2.Zero, 1218);
                        Main.gore[num36].velocity = new Vector2(Main.rand.Next(1, 10) * 0.3f * 2f * modifiers.HitDirection, 0f - (2.5f + (Main.rand.Next(4) * 0.3f)));
                    }

                    if (target.CanBeChasedBy() && !target.SpawnedFromStatue)//别想刷钱
                    {
                        int randomCoin = Main.rand.Next(0, 1000);
                        int itemtype = ItemID.CopperCoin;//大概是89%左右的概率为铜币
                        if (randomCoin == 0)//千分之一概率掉落铂金币
                            itemtype = ItemID.PlatinumCoin;
                        else if (randomCoin < 11)//百分之一概率掉落金币
                            itemtype = ItemID.GoldCoin;
                        else if (randomCoin < 111)//十分之一概率掉落银币
                            itemtype = ItemID.SilverCoin;

                        Item.NewItem(new EntitySource_OnHit(Player, target), target.Center, itemtype);
                    }

                    pirateKingSoulCD = (byte)CD;
                }
            }

            void PriateKingCombatText(ref NPC.HitInfo info)
            {
                double num = info.Damage;
                if (info.InstantKill)
                    num = target.realLife > 0 ? Main.npc[target.realLife].life : target.life;

                CombatText.NewText(new Rectangle((int)target.position.X, (int)target.position.Y, target.width, target.height)
                    , Color.Gold, (int)num, true);
            }
            #endregion

            #region 美杜莎之魂的效果

            void MedusaSoulEffect(ref NPC.HitModifiers modifiers)
            {
                if (medusaSoul < 2)
                    return;

                const int middleLength_Min = 20 * 16;
                const int middleLength_Max = 30 * 16;
                const int minLength = 5 * 16;
                const int maxLength = 80 * 16;

                float minDamage = 0.75f;
                float maxDamage = 0.15f;
                if (medusaSoul > 2)
                {
                    minDamage = 0.5f;
                    maxDamage = 0.25f;
                }

                float distanceToTarget = Vector2.Distance(Player.Center, target.Center);

                float factor;
                Color aimColor;
                if (distanceToTarget < middleLength_Min)//小于中间距离，减少伤害
                {
                    factor = 1 - MathHelper.Clamp((distanceToTarget - minLength) / (middleLength_Min - minLength), 0, 1);
                    aimColor = Color.Gray;
                    float damage = minDamage * factor;
                    modifiers.SourceDamage -= damage;
                    modifiers.HideCombatText();
                    modifiers.ModifyHitInfo += MedusaCombatText;
                }
                else if (distanceToTarget > middleLength_Max)//大于中间距离，增加伤害
                {
                    factor = MathHelper.Clamp((distanceToTarget - middleLength_Max) / (maxLength - middleLength_Max), 0, 1);
                    aimColor = Color.Purple;
                    float damage = maxDamage * factor;
                    modifiers.SourceDamage += damage;
                    modifiers.HideCombatText();
                    modifiers.ModifyHitInfo += MedusaCombatText;
                }

                void MedusaCombatText(ref NPC.HitInfo info)
                {
                    double num = info.Damage;
                    if (info.InstantKill)
                        num = target.realLife > 0 ? Main.npc[target.realLife].life : target.life;

                    Color c = info.Crit ? CombatText.DamagedFriendlyCrit : CombatText.DamagedFriendly;

                    c = Color.Lerp(c, aimColor, factor);

                    CombatText.NewText(new Rectangle((int)target.position.X, (int)target.position.Y, target.width, target.height)
                        , c, (int)num, info.Crit);
                }
            }

            #endregion
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HasEffect(nameof(FlaskOfThunderBuff)) && item.DamageType.CountsAsClass(DamageClass.Melee))
                target.AddBuff(BuffType<ThunderElectrified>(), 6 * 60);
            if (HasEffect(nameof(FlaskOfRedJadeBuff)) && item.DamageType.CountsAsClass(DamageClass.Melee) && Main.rand.NextBool(4))
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ProjectileType<RedJadeBoom>(), (item.damage * 0.75f) > 80 ? 80 : (int)(item.damage * 0.75f), 0, Player.whoAmI);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HasEffect(nameof(FlaskOfThunderBuff)) && proj.DamageType == DamageClass.Melee)
                target.AddBuff(BuffType<ThunderElectrified>(), 6 * 60);
            if (HasEffect(nameof(FlaskOfRedJadeBuff)) && proj.DamageType == DamageClass.Melee && Main.rand.NextBool(4))
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ProjectileType<RedJadeBoom>(), (proj.damage * 0.75f) > 80 ? 80 : (int)(proj.damage * 0.75f), 0, Player.whoAmI);
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (coreKeeperDodge > 0.9f)
                coreKeeperDodge = 0.9f;

            if (info.Dodgeable && Main.rand.NextBool((int)(coreKeeperDodge * 100), 100))
            {
                CombatText.NewText(new Rectangle((int)Player.Top.X, (int)Player.Top.Y, 1, 1)
                    , Color.White, CoreKeeperDodgeText.Value);
                Player.AddImmuneTime(ImmunityCooldownID.General, 20);
                Player.immune = true;
                return true;
            }

            return base.FreeDodge(info);
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (GreatRiverSnailSoul > 0 && GreatRiverSnailSoulCD == 0)
            {
                SpawnGreatRiverSnailSpike();
                return true;
            }

            return base.ConsumableDodge(info);

            void SpawnGreatRiverSnailSpike()
            {

            }
        }

        #endregion

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            if (HasEffect(Items.Misc_Equip.BloodmarkTopper.BloodSet))
                healValue += bloodPoolCount;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (damageSource.SourceItem == null
                && damageSource.SourceNPCIndex == -1
                && damageSource.SourcePlayerIndex == -1
                && damageSource.SourceProjectileLocalIndex == -1
                && damageSource.SourceOtherIndex == 8)//燃烧Debuff
            {
                ThunderElectrifiedDeathReason(ref damageSource);
            }
            return true;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Core.Loaders.KeybindLoader.ArmorBonus.JustPressed && Main.myPlayer == Player.whoAmI)
                for (int i = 0; i < 3; i++)
                {
                    if (Player.armor[i].ModItem is null)
                        continue;
                    if (Player.armor[i].ModItem.IsArmorSet(Player.HeadArmor(), Player.BodyArmor(), Player.LegArmor())
                        && Player.armor[i].ModItem is IControllableArmorBonus conrtolableArmor)
                    {
                        conrtolableArmor.UseArmorBonus(Player);   //使用套装效果
                        break;
                    }
                }

            useSpecialAttack = Core.Loaders.KeybindLoader.SpecialAttack.Current;
            Item item = Player.inventory[Player.selectedItem];

            if (useSpecialAttack && Player.itemAnimation == 0 && item.useStyle != ItemUseStyleID.None &&
                (item.type < ItemID.Count || item.ModItem.Mod.Name == "Coralite" || item.ModItem.Mod is ICoralite))//放置其他模组干扰
            {
                bool flag3 = !item.IsAir && CombinedHooks.CanUseItem(Player, item);

                if (item.mana > 0 && flag3 && Player.whoAmI == Main.myPlayer && item.buffType != 0 && item.buffTime != 0)
                    Player.AddBuff(item.buffType, item.buffTime);

                if (Player.whoAmI == Main.myPlayer && Player.gravDir == 1f && item.mountType != -1 && Player.mount.CanMount(item.mountType, Player))
                    Player.mount.SetMount(item.mountType, Player);

                if (flag3)
                    ItemCheck_StartActualUse(item);
            }
        }

        private void ItemCheck_StartActualUse(Item sItem)
        {
            bool flag = sItem.type == ItemID.GravediggerShovel;
            if (sItem.pick > 0 || sItem.axe > 0 || sItem.hammer > 0 || flag)
                Player.toolTime = 1;

            if (Player.grappling[0] > -1 || sItem.useTurnOnAnimationStart)
            { // useTurnOnAnimationStart check added by tML
                Player.pulley = false;
                Player.pulleyDir = 1;
                if (Player.controlRight)
                    Player.direction = 1;
                else if (Player.controlLeft)
                    Player.direction = -1;
            }

            Player.StartChanneling(sItem);
            Player.attackCD = 0;
            Player.ResetMeleeHitCooldowns();
            Player.ApplyItemAnimation(sItem);
            bool flag2 = ItemID.Sets.SkipsInitialUseSound[sItem.type];
            if (sItem.UseSound != null && !flag2)
                SoundEngine.PlaySound(sItem.UseSound, Player.Center);
        }

        #region 多人同步

        public override void SendClientChanges(ModPlayer clientPlayer)
        {

        }

        public override void CopyClientState(ModPlayer targetCopy)
        {

        }

        #endregion

        #region IO

        public override void SaveData(TagCompound tag)
        {
            SaveFlyingShield(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            LoadFlyingShield(tag);
        }

        #endregion

        public override void OnEnterWorld()
        {
            if (CoraliteWorld.CoralCatWorld)
                Player.QuickSpawnItem(Player.GetSource_FromThis(), ItemID.Meowmere);

            Main.NewText(CoraliteSystem.OnEnterWorld.Value, Color.Coral);
        }
    }
}
