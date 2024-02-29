using Coralite.Content.Biomes;
using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.Botanical.Seeds;
using Coralite.Content.Items.CoreKeeper;
using Coralite.Content.Items.Magike;
using Coralite.Content.Items.RedJades;
using Coralite.Content.Projectiles.Globals;
using Coralite.Content.UI;
using Coralite.Core;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer : ModPlayer
    {
        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public int DashDir = -1;


        public float yujianUIAlpha;
        public bool ownedYujianProj;
        public float nianli;
        public const float BaseNianliMax = 300f;
        public float nianliMax = BaseNianliMax;
        public const float BaseNianliRegain = 0.5f;
        public float nianliRegain = BaseNianliRegain;

        public short rightClickReuseDelay = 0;

        public int DashDelay = 0;
        public int DashTimer = 0;

        public int parryTime;

        /// <summary> 装备赤玉吊坠 </summary>
        public bool equippedRedJadePendant;
        /// <summary> 装备影魔镜 </summary>
        public bool equippedShadowMirror;
        /// <summary> 装备影魔镜 </summary>
        public bool equippedPhantomMirror;

        public byte nightmareCount;
        /// <summary> 使用梦魇之花的噩梦能量 </summary>
        public int nightmareEnergy;
        public int nightmareEnergyMax;
        public bool equippedBoneRing;
        /// <summary> 抵抗梦蚀 </summary>
        public bool resistDreamErosion;

        /// <summary>
        /// 爆伤加成
        /// </summary>
        public float critDamageBonus;
        /// <summary>
        /// 回复量乘算加成
        /// </summary>
        public float lifeReganBonus;
        /// <summary>
        /// 来自BOSS的伤害减免
        /// </summary>
        public float bossDamageReduce;

        /// <summary>
        /// 同时存在的飞盾弹幕数量
        /// </summary>
        public int MaxFlyingShield;
        /// <summary>
        /// 是否能够同时使用左右键
        /// </summary>
        public bool FlyingShieldLRMeantime;
        /// <summary>
        /// 飞盾格挡后获得的伤害减免
        /// </summary>
        public float FlyingShieldDamageReduce;
        /// <summary>
        /// 防御时间，
        /// </summary>
        public int FlyingShieldGuardTime;


        /// <summary>
        /// 地心护核者的闪避
        /// </summary>
        public float coreKeeperDodge;

        public bool useSpecialAttack;

        public override void ResetEffects()
        {
            equippedRedJadePendant = false;
            equippedShadowMirror = false;
            equippedPhantomMirror = false;
            equippedBoneRing = false;
            resistDreamErosion = false;

            critDamageBonus = 0;
            lifeReganBonus = 0;
            bossDamageReduce = 0;

            MaxFlyingShield = 1;
            FlyingShieldLRMeantime = false;
            if (FlyingShieldGuardTime > 0)
            {
                FlyingShieldGuardTime--;
                if (FlyingShieldGuardTime <= 25)
                    FlyingShieldDamageReduce -= FlyingShieldDamageReduce / FlyingShieldGuardTime;
                if (FlyingShieldGuardTime == 0)
                    FlyingShieldDamageReduce = 0;
            }

            coreKeeperDodge = 0;

            nightmareEnergyMax = 7;
            if (parryTime > 0)
            {
                parryTime--;
                if (parryTime <= 0)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.MaxMana, Player.Center);
                    for (int i = 0; i < 5; i++)
                    {
                        int index = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Clentaminator_Red, 0f, 0f, 255, Scale: Main.rand.Next(20, 26) * 0.1f);
                        Main.dust[index].noLight = true;
                        Main.dust[index].noGravity = true;
                        Main.dust[index].velocity = Player.velocity * 0.5f;
                    }
                }
            }

            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15)
                DashDir = DashDown;
            else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
                DashDir = DashUp;
            else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
                DashDir = DashRight;
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
                DashDir = DashLeft;
            else
                DashDir = -1;

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
                    Rectangle rectangle = new Rectangle((int)Player.Center.X, (int)Player.Center.Y, 2, 2);

                    CombatText.NewText(rectangle, Coralite.Instance.MagicCrystalPink, "协调的力量被临时封印了");
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
            if (DashDelay == 0 && DashDir != -1 && Player.grappling[0] == -1 && !Player.tongued)
                do
                {
                    if (UsingVanillaDash())
                        break;

                    if (Player.HeldItem.ModItem is IDashable dashItem)
                        if (dashItem.Dash(Player, DashDir))
                            break;

                    for (int i = 3; i < 10; i++)
                    {
                        if (!Player.armor[i].IsAir && Player.armor[i].ModItem is IDashable dashItem2)
                        {
                            if (dashItem2.Dash(Player, DashDir))
                                goto checkDashOver;
                        }
                    }

                } while (false);

            checkDashOver:

            if (DashDelay > 0)
            {
                DashDelay--;
                if (DashDelay == 0)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.MaxMana, Player.Center);
                    for (int i = 0; i < 5; i++)
                    {
                        int index = Dust.NewDust(Player.position, Player.width, Player.height, DustID.YellowTorch, 0f, 0f, 255, Scale: Main.rand.Next(20, 26) * 0.15f);
                        Main.dust[index].noLight = true;
                        Main.dust[index].noGravity = true;
                        Main.dust[index].velocity = Player.velocity;
                    }
                }
            }

            if (DashTimer > 0)
            {
                Player.armorEffectDrawShadowEOCShield = true;
                DashTimer--;
            }
        }

        public override void PreUpdateBuffs()
        {
            if (Player.HeldItem.ModItem is IBuffHeldItem buffHeldItem)
                buffHeldItem.UpdateBuffHeldItem(Player);
        }

        public override void PostUpdateEquips()
        {
            if (Player.HeldItem.ModItem is IEquipHeldItem ehi)
                ehi.UpdateEquipHeldItem(Player);

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
        }

        public override void UpdateLifeRegen()
        {
            Player.lifeRegen = (int)(Player.lifeRegen * (1 + lifeReganBonus));
        }

        public override void PostUpdate()
        {
            equippedRedJadePendant = false;
            if (rightClickReuseDelay > 0)
                rightClickReuseDelay--;

            nianli = Math.Clamp(nianli, 0f, nianliMax);  //只是防止意外发生
        }

        public override void UpdateDead()
        {
            if (yujianUIAlpha > 0f)
            {
                yujianUIAlpha -= 0.035f;
                yujianUIAlpha = MathHelper.Clamp(yujianUIAlpha, 0f, 1f);
            }

            rightClickReuseDelay = 0;
            equippedRedJadePendant = false;
        }

        #endregion

        #region 受击与攻击

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (proj.TryGetGlobalProjectile(out CoraliteGlobalProjectile cgp) && cgp.isBossProjectile)
            {
                modifiers.SourceDamage -= bossDamageReduce;
            }

            if (FlyingShieldGuardTime > 0)
            {
                modifiers.ModifyHurtInfo += FlyingShield_DamageReduce;
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (npc.boss)
            {
                modifiers.SourceDamage -= bossDamageReduce;
            }

            if (FlyingShieldGuardTime > 0)
            {
                modifiers.ModifyHurtInfo += FlyingShield_DamageReduce;
            }
        }

        private void FlyingShield_DamageReduce(ref Player.HurtInfo info)
        {
            info.Damage = (int)(info.Damage * (1 - FlyingShieldDamageReduce));
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (equippedRedJadePendant && Main.myPlayer == Player.whoAmI && hurtInfo.Damage > 5 && Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor.First((item) => item.type == ItemType<RedJadePendant>())),
                    Player.Center + (proj.Center - Player.Center).SafeNormalize(Vector2.One) * 16, Vector2.Zero, ProjectileType<RedJadeBoom>(), 80, 8f, Player.whoAmI);
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (equippedRedJadePendant && Main.myPlayer == Player.whoAmI && hurtInfo.Damage > 5 && Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor.First((item) => item.type == ItemType<RedJadePendant>())),
                    Player.Center + (npc.Center - Player.Center).SafeNormalize(Vector2.One) * 16, Vector2.Zero, ProjectileType<RedJadeBoom>(), 80, 8f, Player.whoAmI);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += critDamageBonus;
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (coreKeeperDodge > 0.9f)
                coreKeeperDodge = 0.9f;
            //coreKeeperDodge = 1f;
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

        #endregion

        #region 钓鱼系统

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;

            if (inWater && Player.ZoneBeach && !attempt.crate)
            {
                if (attempt.common)
                {
                    if (Main.rand.NextBool(15))
                        itemDrop = ItemType<NacliteSeedling>();
                }
                else if (attempt.uncommon)
                {
                    if (Main.hardMode)
                    {
                        if (Main.rand.NextBool(6))
                            itemDrop = ItemType<BubblePearlNecklace>();
                        if (Main.rand.NextBool(3))
                            itemDrop = ItemType<Items.FlyingShields.HorseshoeCrab>();
                    }
                    if (Main.rand.NextBool(3))
                        itemDrop = ItemType<Items.FlyingShields.PearlRay>();
                }
                else if (attempt.legendary)
                {
                    if (NPC.downedFishron && Main.rand.NextBool(5))
                        itemDrop = ItemType<OceanheartNecklace>();
                }

            }

            if (attempt.crate)
            {
                if (inWater)
                {
                    if (Player.InModBiome<MagicCrystalCave>())
                    {
                        // 如果钓到了的是匣子，就替换为自己的匣子

                        // 为了不替换掉最高级的匣子，所以做点限制
                        // Their drop conditions are "veryrare" or "legendary"
                        // (After that come biome crates ("rare"), then iron/mythril ("uncommon"), then wood/pearl (none of the previous))
                        // Let's replace biome crates 50% of the time (player could be in multiple (modded) biomes, we should respect that)
                        //增加50%的概率替换掉其他匣子，因为玩家有时候可能在多个重叠的环境中
                        if (!attempt.veryrare && !attempt.legendary && attempt.rare && Main.rand.NextBool())
                        {
                            itemDrop = ItemType<CrystalCrate>();
                            return; // This is important so your code after this that rolls items will not run
                        }

                    }
                }
            }

        }

        #endregion

        #region 绘制部分

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (equippedBoneRing)
                drawInfo.drawPlayer.handon = EquipLoader.GetEquipSlot(Mod, "BoneRing", EquipType.HandsOn);
        }

        #endregion

        public void GetNightmareEnergy(int howMany)
        {
            if (nightmareEnergy < nightmareEnergyMax)
            {
                nightmareEnergy += howMany;
                if (nightmareEnergy > nightmareEnergyMax)
                    nightmareEnergy = nightmareEnergyMax;
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Core.Loaders.KeybindLoader.ArmorBonus.JustPressed && Main.myPlayer == Player.whoAmI)
            {
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
            }

            useSpecialAttack = Core.Loaders.KeybindLoader.SpecialAttack.Current;
        }

        public bool UsingVanillaDash() => Player.dashType != 0 || Player.setSolar || Player.mount.Active;

        public void Guard(float damageReduce)
        {
            FlyingShieldGuardTime = 35;
            FlyingShieldDamageReduce = damageReduce;
        }
    }
}
