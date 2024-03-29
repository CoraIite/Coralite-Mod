using Coralite.Core;
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

        public StatModifier DashDelayModifyer = StatModifier.Default;

        public bool UsingVanillaDash() => Player.dashType != 0 || Player.setSolar || Player.mount.Active;

        public void UpdateDash()
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
                            {
                                DashDelay = (int)(DashDelay * DashDelayModifyer.ApplyTo(1));
                                goto checkDashOver;
                            }
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
                    SpawnDashRechargeDusts();
                }
            }

            if (DashTimer > 0)
            {
                Player.armorEffectDrawShadowEOCShield = true;
                DashTimer--;
            }
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
