using Coralite.Core;
using Coralite.Core.Configs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public int DashDir = -1;

        public int DashDelay = 0;
        public int DashTimer = 0;

        public bool JustStartDash;
        public Vector2 DashStartVelocity;

        public StatModifier DashDelayModifyer = StatModifier.Default;

        public bool UsingVanillaDash() => Player.dashType != 0;

        public void BanVanillaDash()
        {
            Player.dashType = 0;
            Player.dash = 0;
            //Player.setSolar = false;
        }

        public void UpdateDash()
        {
            if (DashDelay == 0 && DashDir != -1 && Player.grappling[0] == -1 && !Player.tongued && !Player.mount.Active)
                do
                {
                    if (GamePlaySystem.SpecialDashFirst)
                    {
                        if (HeldItemDash())
                        {
                            BanVanillaDash();
                            OnJustStartDash();
                            break;
                        }

                        if (AccessoryDash())
                        {
                            BanVanillaDash();
                            OnJustStartDash();
                            break;
                        }
                    }
                    else
                    {
                        if (UsingVanillaDash())
                            break;
                        if (HeldItemDash())
                        {
                            OnJustStartDash();
                            break;
                        }
                        if (AccessoryDash())
                        {
                            OnJustStartDash();
                            break;
                        }
                    }

                } while (false);

            if (DashDelay > 0)
            {
                DashDelay--;
                if (DashDelay == 0)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.MaxMana, Player.Center);
                    SpawnDashRechargeDusts();
                }
            }

            if (DashTimer > 0)
            {
                if (GamePlaySystem.SpecialDashFirst)
                    BanVanillaDash();

                DashTimer--;
            }
        }

        /// <summary>
        /// 记录下玩家速度
        /// </summary>
        public void OnJustStartDash()
        {
            JustStartDash = true;
            DashStartVelocity = Player.velocity;
        }

        /// <summary>
        /// 开始冲刺时直接设定玩家速度
        /// </summary>
        public void SetStartDash()
        {
            if (JustStartDash)
            {
                JustStartDash = false;
                Player.velocity = DashStartVelocity;
            }
        }

        /// <summary>
        /// 检测手持物品的冲刺，并调用<br></br>
        /// 如果不调用也将返回<see langword="true"/>
        /// </summary>
        /// <returns></returns>
        public bool HeldItemDash(bool justCheck = false)
        {
            if (Player.HeldItem.ModItem is IDashable dashItem)
            {
                if (justCheck)
                    return true;

                if (dashItem.Dash(Player, DashDir))
                {
                    DashDelay = (int)(DashDelay * DashDelayModifyer.ApplyTo(1));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检测饰品的冲刺，并选择性调用
        /// </summary>
        /// <param name="justCheck"></param>
        /// <returns></returns>
        public bool AccessoryDash(bool justCheck = false)//TODO: 以后有机会改成list存储方式，目前这样无法适配额外饰品栏
        {
            for (int i = 3; i < 10; i++)
            {
                if (!Player.armor[i].IsAir && Player.armor[i].ModItem is IDashable dashItem2)
                {
                    if (justCheck)
                        return true;

                    if (dashItem2.Dash(Player, DashDir))
                    {
                        DashDelay = (int)(DashDelay * DashDelayModifyer.ApplyTo(1));
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 在<see cref="ResetEffects"/>中调用
        /// </summary>
        public void ResetDahsSets()
        {
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

            DashDelayModifyer = StatModifier.Default;
        }

        public void SpawnDashRechargeDusts()
        {
            float rot = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Player.Center, DustID.YellowTorch, (rot + i * MathHelper.TwoPi / 8).ToRotationVector2() * 3,
                    255, Scale: Main.rand.Next(20, 26) * 0.15f);
                dust.noLight = true;
                dust.noGravity = true;
            }
        }
    }
}
