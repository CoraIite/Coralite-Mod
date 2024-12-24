using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;

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

        /// <summary>
        /// 开始冲刺时
        /// </summary>
        public bool JustStartDash;
        public Vector2 DashStartVelocity;

        public StatModifier DashDelayModifyer = StatModifier.Default;

        /// <summary>
        /// 存储所有的冲刺接口
        /// </summary>
        public List<IDashable> DashControllers = new List<IDashable>();

        public bool UsingVanillaDash() => Player.dashType != 0;

        /// <summary>
        /// 是否能够冲刺，判断条件为：钩爪勾到了，玩家被拖拽，玩家正在坐骑上
        /// </summary>
        public bool CanDash => Player.grappling[0] == -1 && !Player.tongued && !Player.mount.Active;

        public void BanVanillaDash()
        {
            Player.dashType = 0;
            Player.dash = 0;
            //Player.setSolar = false;
        }

        public void UpdateDash()
        {
            if (DashDelay == 0 && DashDir != -1 && DashControllers.Count > 0 && CanDash)
                do
                {
                    DashControllers.Sort((d1, d2) => d1.Priority.CompareTo(d2.Priority));

                    if (GamePlaySystem.SpecialDashFirst)
                    {
                        if (CheckDashControllers())
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
                        if (CheckDashControllers())
                        {
                            OnJustStartDash();
                            break;
                        }
                    }

                } while (false);

            if (DashDelay > 0)
            {
                BanVanillaDash();
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

            if (DashControllers.Count > 0)
                DashControllers.Clear();
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
        /// 调用dash
        /// </summary>
        /// <returns></returns>
        public bool CheckDashControllers()
        {
            if (DashControllers[0].Dash(Player, DashDir))
            {
                DashDelay = (int)(DashDelay * DashDelayModifyer.ApplyTo(1));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测手持物品的冲刺，并调用<br></br>
        /// 如果不调用也将返回<see langword="true"/>
        /// </summary>
        /// <returns></returns>
        //public bool HeldItemDash(bool justCheck = false)
        //{
        //    if (Player.HeldItem.ModItem is IDashable dashItem)
        //    {
        //        if (justCheck)
        //            return true;

        //        if (dashItem.Dash(Player, DashDir))
        //        {
        //            DashDelay = (int)(DashDelay * DashDelayModifyer.ApplyTo(1));
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        /// <summary>
        /// 检测饰品的冲刺，并选择性调用
        /// </summary>
        /// <param name="justCheck"></param>
        /// <returns></returns>
        //public bool AccessoryDash(bool justCheck = false)
        //{
        //    for (int i = 3; i < 10; i++)
        //    {
        //        if (!Player.armor[i].IsAir && Player.armor[i].ModItem is IDashable dashItem2)
        //        {
        //            if (justCheck)
        //                return true;

        //            if (dashItem2.Dash(Player, DashDir))
        //            {
        //                DashDelay = (int)(DashDelay * DashDelayModifyer.ApplyTo(1));
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

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
            //float rot = Main.rand.NextFloat(6.282f);
            //for (int i = 0; i < 8; i++)
            //{
            //    Dust dust = Dust.NewDustPerfect(Player.Center, DustID.YellowTorch, (rot + (i * MathHelper.TwoPi / 8)).ToRotationVector2() * 3,
            //        255, Scale: Main.rand.Next(20, 26) * 0.15f);
            //    dust.noLight = true;
            //    dust.noGravity = true;
            //}

            Vector2 pos = Player.Center + new Vector2(0, -6);

            for (int i = -1; i < 2; i += 2)
            {
                StaminaRecover.Spawn(pos, Vector2.Zero,-1.57f+ i * 0.5f, Color.Gold, 1, 0.7f, new Vector2(2f, 1f), Player.whoAmI);
                StaminaRecover.Spawn(pos, Vector2.Zero,1.57f+ i * 0.5f, Color.Gold, 1, 0.4f, new Vector2(1.5f, 1f), Player.whoAmI);
            }
        }

        /// <summary>
        /// 将自身添加进冲刺列表中
        /// </summary>
        /// <param name="dash"></param>
        public void AddDash(IDashable dash)
            =>DashControllers.Add(dash);
    }
}
