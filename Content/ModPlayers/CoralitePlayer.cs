using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Content.Items.RedJades;
using Coralite.Content.Items.Steel;
using Coralite.Content.Items.Thunder;
using Coralite.Content.Projectiles.Globals;
using Coralite.Content.UI;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer : ModPlayer, ILocalizedModType
    {
        public string LocalizationCategory => "Players";

        public int rightClickReuseDelay = 0;
        public int parryTime;

        /// <summary>
        /// 各种效果
        /// </summary>
        public HashSet<string> Effects = new();
        public List<IInventoryCraftStation> inventoryCraftStations = new();

        #region 装备类字段

        /// <summary> 花粉火药计时器 </summary>
        public int PollenGunpowderEffect = 60;
        /// <summary> 玫瑰火药计时器 </summary>
        public int RoseGunpowderEffect = 90;

        /// <summary> 海盗王之魂 </summary>
        public int pirateKingSoul;
        /// <summary> 海盗王之魂的效果CD </summary>
        public int pirateKingSoulCD;

        /// <summary> 幸运星 </summary>
        public int luckyStar;

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

        public int nightmareCount;
        /// <summary> 使用梦魇之花的噩梦能量 </summary>
        public int nightmareEnergy;
        public int nightmareEnergyMax;

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

        public override void Load()
        {
            LoadDeathReasons();
        }

        public override void Unload()
        {
            UnloadDeathReasons();
        }

        public override void ResetEffects()
        {
            Effects ??= new HashSet<string>();
            inventoryCraftStations ??= new List<IInventoryCraftStation>();

            inventoryCraftStations.Clear();
            Effects.Clear();

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

            ResetFlyingShieldSets();

            coreKeeperDodge = 0;

            nightmareEnergyMax = 7;

            if (parryTime > 0)
            {
                parryTime--;
                if (parryTime <= 0)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.MaxMana, Player.Center);
                    float rot = Main.rand.NextFloat(6.282f);
                    for (int i = 0; i < 8; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Player.Center, DustID.Clentaminator_Red, (rot + (i * MathHelper.TwoPi / 8)).ToRotationVector2() * 3,
                            255, Scale: Main.rand.Next(20, 26) * 0.1f);
                        dust.noLight = true;
                        dust.noGravity = true;
                    }
                }
            }

            ResetDahsSets();
        }

        public override void OnRespawn()
        {
            nightmareCount = 0;
            nightmareEnergy = 0;
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
            nianliRegain = BaseNianliRegain;
            nianliMax = BaseNianliMax;
        }

        public override void PreUpdateMovement()
        {
            SetStartDash();
        }

        public override void PreUpdateBuffs()
        {
            if (Player.HeldItem.ModItem is IBuffHeldItem buffHeldItem)
                buffHeldItem.UpdateBuffHeldItem(Player);

            int tempLifeMax = Player.statLifeMax2;
            Player.statLifeMax2 = (int)LifeMaxModifyer.ApplyTo(Player.statLifeMax2);//防止一些意外事故
            if (Player.statLifeMax2 < 1)
                Player.statLifeMax2 = tempLifeMax;

            LifeMaxModifyer = new StatModifier();
        }

        public override void PostUpdateEquips()
        {
            if (Player.HeldItem.ModItem is IEquipHeldItem ehi)
                ehi.UpdateEquipHeldItem(Player);

            if (luckyStar > 1)//幸运星增加玩家幸运
                Player.luck += 0.2f;
            else if (luckyStar > 2)
                Player.luck += 0.3f;

            if (ownedYujianProj)
            {
                bool justCompleteCharge = nianli < nianliMax;
                nianli += nianliRegain;
                nianli = Math.Clamp(nianli, 0f, nianliMax);
                if (nianli == nianliMax && justCompleteCharge)      //蓄力完成的时刻发出声音
                    SoundEngine.PlaySound(SoundID.Item4);
            }
            else
                nianli = 0f;

            if (nightmareEnergy > nightmareEnergyMax)
                nightmareEnergy = nightmareEnergyMax;
        }

        public override void PostUpdateMiscEffects()
        {
            //有御剑弹幕那就让透明度增加，没有御剑减小透明度直到为0
            if (ownedYujianProj)
            {
                if (yujianUIAlpha < 1f)
                {
                    yujianUIAlpha += 0.035f;
                    yujianUIAlpha = MathHelper.Clamp(yujianUIAlpha, 0f, 1f);
                    NianliChargingBar.visible = true;
                }
            }
            else if (yujianUIAlpha > 0f)
            {
                yujianUIAlpha -= 0.035f;
                yujianUIAlpha = MathHelper.Clamp(yujianUIAlpha, 0f, 1f);
                if (yujianUIAlpha <= 0f)
                    NianliChargingBar.visible = false;
            }

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

            if (HasEffect(nameof(OsirisPillar)) && Player.statLife <= 60)
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
            Player.lifeRegen = (int)(Player.lifeRegen * (1 + lifeReganBonus));
        }

        public override void UpdateBadLifeRegen()
        {
            if (HasEffect(nameof(ThunderElectrified)))
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                int damage = (int)(3 + (Player.velocity.Length() * 1.5f));

                if (damage > 15)
                    damage = 15;

                Player.lifeRegen -= damage * 2;
            }

            if (HasEffect(nameof(LifePulseDevice)))
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                if (Player.statLife > 38)
                {
                    Player.lifeRegen -= 10 * 2;
                    Player.lifeRegenTime = 0;
                }
            }

            if (HasEffect(nameof(OsirisPillar)) && Player.statMana >= Player.statManaMax2 - 20)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                if (Player.statLife > 50)
                {
                    Player.lifeRegen -= 10 * 3;
                    Player.lifeRegenTime = 0;
                }
            }
        }

        public override void PostUpdate()
        {
            if (rightClickReuseDelay > 0)
                rightClickReuseDelay--;

            nianli = Math.Clamp(nianli, 0f, nianliMax);  //只是防止意外发生
            oldOldVelocity = oldVelocity;
            oldVelocity = Player.velocity;

            oldOldCenter = oldCenter;
            oldCenter = Player.Center;
        }

        public override void UpdateDead()
        {
            if (yujianUIAlpha > 0f)
            {
                yujianUIAlpha -= 0.035f;
                yujianUIAlpha = MathHelper.Clamp(yujianUIAlpha, 0f, 1f);
            }

            rightClickReuseDelay = 0;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (source.Item.useAmmo == AmmoID.Bullet)
            {
                if (PollenGunpowderEffect == 0)
                {
                    Projectile.NewProjectile(source, position, velocity.RotateByRandom(-0.1f, 0.1f),
                        ProjectileType<Items.HyacinthSeries.PollenGunpowderProj>(), damage, knockback, Player.whoAmI);
                    PollenGunpowderEffect = 60;
                }

                if (RoseGunpowderEffect == 0)
                {
                    Projectile.NewProjectile(source, position, velocity.RotateByRandom(-0.05f, 0.05f),
                        ProjectileType<Items.HyacinthSeries.RoseGunpowderProj>(), (int)(damage * 1.35f), knockback, Player.whoAmI);
                    RoseGunpowderEffect = 90;
                }
            }

            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }

        #endregion

        #region 受击与攻击

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
                modifiers.SourceDamage += 0.01f * Math.Clamp(defence / 5, 0, 10);
            }
            else if (HasEffect(nameof(GravitationalCatapultBuff)))
            {
                int defence = (int)Player.statDefense;
                modifiers.SourceDamage += 0.01f * Math.Clamp(defence / 5, 0, 20);
            }

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

                    pirateKingSoulCD = CD;
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
            if (HasEffect(nameof(FlaskOfThunderBuff)) && item.DamageType == DamageClass.Melee)
                target.AddBuff(BuffType<ThunderElectrified>(), 6 * 60);
            if (HasEffect(nameof(FlaskOfRedJadeBuff)) && item.DamageType == DamageClass.Melee && Main.rand.NextBool(4))
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ProjectileType<RedJadeBoom>(), (int)(item.damage * 0.75f), 0, Player.whoAmI);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HasEffect(nameof(FlaskOfThunderBuff)) && proj.DamageType == DamageClass.Melee)
                target.AddBuff(BuffType<ThunderElectrified>(), 6 * 60);
            if (HasEffect(nameof(FlaskOfRedJadeBuff)) && proj.DamageType == DamageClass.Melee && Main.rand.NextBool(4))
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ProjectileType<RedJadeBoom>(), (int)(proj.damage * 0.75f), 0, Player.whoAmI);
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (coreKeeperDodge > 0.9f)
                coreKeeperDodge = 0.9f;

            if (info.Dodgeable && Main.rand.NextBool((int)(coreKeeperDodge * 100), 100))
            {
                CombatText.NewText(new Rectangle((int)Player.Top.X, (int)Player.Top.Y, 1, 1)
                    , Color.White, "闪避");
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

        public void GetNightmareEnergy(int howMany)
        {
            if (nightmareEnergy < nightmareEnergyMax)
            {
                nightmareEnergy += howMany;
                if (nightmareEnergy > nightmareEnergyMax)
                    nightmareEnergy = nightmareEnergyMax;
            }
        }

        /// <summary>
        /// 玩家是否有某个效果，建议使用<see cref="nameof"/>来获取字符串
        /// </summary>
        /// <param name="effectName"></param>
        /// <returns></returns>
        public bool HasEffect(string effectName) => Effects.Contains(effectName);

        /// <summary>
        /// 为玩家添加某个效果，建议使用<see cref="nameof"/>来获取字符串
        /// </summary>
        /// <param name="effectName"></param>
        /// <returns></returns>
        public bool AddEffect(string effectName) => Effects.Add(effectName);

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Core.Loaders.KeybindLoader.ArmorBonus.JustPressed && Main.myPlayer == Player.whoAmI)
                for (int i = 0; i < 3; i++)
                {
                    if (Player.armor[i].ModItem is null)
                        continue;
                    if (Player.armor[i].ModItem.IsArmorSet(Player.armor[0], Player.armor[1], Player.armor[2])
                        && Player.armor[i].ModItem is IControllableArmorBonus conrtolableArmor)
                    {
                        conrtolableArmor.UseArmorBonus(Player);   //使用套装效果
                        break;
                    }
                }

            useSpecialAttack = Core.Loaders.KeybindLoader.SpecialAttack.Current;
            Item item = Player.inventory[Player.selectedItem];

            if (useSpecialAttack && Player.itemAnimation == 0 && item.useStyle != ItemUseStyleID.None)
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


        public override void OnEnterWorld()
        {
            if (CoraliteWorld.CoralCatWorld)
                Player.QuickSpawnItem(Player.GetSource_FromThis(), ItemID.Meowmere);
        }
    }
}
