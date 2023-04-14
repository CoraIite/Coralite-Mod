using Coralite.Content.Items.Botanical.Seeds;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.RedJades;
using Coralite.Content.UI;
using Coralite.Core;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.ModPlayers
{
    public class CoralitePlayer : ModPlayer
    {
        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;
        public int DashDir = -1;


        public float yujianUIAlpha;
        public bool ownedYujianProj;
        public float nianli;
        public float nianliMax = 300f;
        public float nianliRegain = 1f;

        public short rightClickReuseDelay = 0;

        public int DashDelay = 0;
        public int DashTimer = 0;

        public bool redJadePendant;

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

        #region 各种更新

        public override void PreUpdate()
        {
            nianliRegain = 1f;
            nianliMax = 300f;
        }

        public override void PreUpdateMovement()
        {
            if (DashDelay == 0 && DashDir != -1 && Player.grappling[0] == -1 && !Player.tongued)
            {
                //冰晶弓冲刺
                if (CanUseIcicleBowDash())
                {
                    Vector2 newVelocity = Player.velocity;
                    switch (DashDir)
                    {
                        case DashLeft:
                        case DashRight:
                            {
                                float dashDirection = DashDir == DashRight ? 1 : -1;
                                newVelocity.X = dashDirection * 10;
                                break;
                            }
                        default:
                            return;
                    }

                    DashDelay = 120;
                    DashTimer = 20;
                    Player.immuneTime = 20;
                    Player.immune = true;
                    Player.velocity = newVelocity;

                    if (Player.whoAmI == Main.myPlayer)
                    {
                        SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, Player.Center);
                        for (int i = 0; i < 4; i++)//生成冰晶粒子
                        {
                            Vector2 center = Player.Center + (-1.57f + i * 1.57f).ToRotationVector2() * 64;
                            Vector2 velocity = (i * 1.57f).ToRotationVector2() * 4;
                            IceStarLight.Spawn(center, velocity, 1f, () => Player.Center,16);
                        }

                        for (int i = 0; i < 1000; i++)
                        {
                            Projectile proj = Main.projectile[i];
                            if (proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.ModProjectile is IcicleBowHeldProj)
                            {
                                proj.Kill();
                                break;
                            }
                        }

                        //生成手持弹幕
                        Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<IcicleBowHeldProj>(),
                            Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, (Main.MouseWorld - Player.Center).ToRotation(), 1);
                    }
                }
            }

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

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (redJadePendant && Main.myPlayer == Player.whoAmI && damage > 5 && Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor.First((item) => item.type == ItemType<RedJadePendant>())),
                    Player.Center + (proj.Center - Player.Center).SafeNormalize(Vector2.One) * 16, Vector2.Zero, ProjectileType<RedJadeBoom>(), 80, 8f, Player.whoAmI);
            }
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (redJadePendant && Main.myPlayer == Player.whoAmI && damage > 5 && Main.rand.NextBool(3))
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

        public bool CanUseIcicleBowDash()
        {
            return Player.HeldItem.type==ModContent.ItemType<IcicleBow>()
                && Player.dashType == 0
                && !Player.setSolar
                && !Player.mount.Active;
        }

    }
}
