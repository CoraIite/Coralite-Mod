using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.Botanical.Seeds;
using Coralite.Content.Items.RedJades;
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

        public bool redJadePendant;

        public byte nightmareCount;

        public override void ResetEffects()
        {
            redJadePendant = false;

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
        }

        public override bool CanUseItem(Item item)
        {
            if (item.type==ItemID.RodOfHarmony)
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
                        int index = Dust.NewDust(Player.position, Player.width, Player.height, 45, 0f, 0f, 255, default(Color), (float)Main.rand.Next(20, 26) * 0.1f);
                        Main.dust[index].noLight = true;
                        Main.dust[index].noGravity = true;
                        Main.dust[index].velocity *= 0.5f;
                    }
                }
            }

            if (DashTimer > 0)
            {
                Player.armorEffectDrawShadowEOCShield = true;
                DashTimer--;
            }
        }

        public override void PostUpdateEquips()
        {
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

        public override void PostUpdate()
        {
            redJadePendant = false;
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
            redJadePendant = false;
        }

        #endregion

        #region 受击与攻击

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (redJadePendant && Main.myPlayer == Player.whoAmI && hurtInfo.Damage > 5 && Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor.First((item) => item.type == ItemType<RedJadePendant>())),
                    Player.Center + (proj.Center - Player.Center).SafeNormalize(Vector2.One) * 16, Vector2.Zero, ProjectileType<RedJadeBoom>(), 80, 8f, Player.whoAmI);
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (redJadePendant && Main.myPlayer == Player.whoAmI && hurtInfo.Damage > 5 && Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor.First((item) => item.type == ItemType<RedJadePendant>())),
                    Player.Center + (npc.Center - Player.Center).SafeNormalize(Vector2.One) * 16, Vector2.Zero, ProjectileType<RedJadeBoom>(), 80, 8f, Player.whoAmI);
            }
        }

        #endregion

        #region 钓鱼系统

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;

            if (inWater && Player.ZoneBeach && attempt.common && !attempt.crate)
            {
                if (Main.rand.NextBool(15))
                    itemDrop = ItemType<NacliteSeedling>();
            }
        }

        #endregion

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Core.Loaders.KeybindLoader.ArmorBonus.JustPressed && Main.myPlayer == Player.whoAmI)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (Player.armor[i].ModItem.IsArmorSet(Player.armor[0], Player.armor[1], Player.armor[2])
                        && Player.armor[i].ModItem is IControllableArmorBonus conrtolableArmor)
                    {
                        conrtolableArmor.UseArmorBonus(Player);   //使用套装效果
                        break;
                    }
                }
            }
        }

        public bool UsingVanillaDash() => Player.dashType != 0 || Player.setSolar || Player.mount.Active;


    }
}
