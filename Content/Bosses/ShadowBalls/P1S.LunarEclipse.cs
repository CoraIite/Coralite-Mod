using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class SmallShadowBall
{
    public void LunarEclipse(NPC owner)
    {
        const int MoveToDashStart = 0;
        const int DashAndLeaveStars = 1;
        const int WaitForMoon = 2;

        Player targetPlayer = Main.player[owner.target];
        int delay = selfIndex * 10;
        Vector2 dashDirection = new(selfIndex % 2 == 0 ? -1 : 1,
            (selfIndex % 3 - 1) * 0.35f);

        switch (SonState)
        {
            default:
            case MoveToDashStart:
                {
                    if (Timer < delay)
                        break;

                    Vector2 targetPosition = targetPlayer.Center - dashDirection * 220;
                    MoveToAttackPosition(targetPosition, 0.1f);
                    if (Vector2.DistanceSquared(NPC.Center, targetPosition) < 40 * 40 || Timer > delay + 70)
                    {
                        NPC.velocity = dashDirection.SafeNormalize(Vector2.UnitX) * 18;
                        SonState = DashAndLeaveStars;
                        Timer = 0;
                    }
                }
                break;
            case DashAndLeaveStars:
                {
                    if (!VaultUtils.isClient && Timer < 55 && Timer % 8 == 0)
                        NPC.NewProjectileDirectInAI_Server<ShadowBallStar>(NPC.Center, Vector2.Zero,
                            Helper.ScaleValueForDiffMode(16, 22, 28, 36), 0, ai0: -1);

                    if (Timer >= 55)
                    {
                        if (!VaultUtils.isClient)
                            NPC.NewProjectileDirectInAI_Server<ShadowBallEclipseMoon>(NPC.Center,
                                Vector2.Zero, Helper.ScaleValueForDiffMode(25, 35, 45, 55), 0,
                                ai0: selfIndex % 8);

                        SonState = WaitForMoon;
                        Timer = 0;
                    }
                }
                break;
            case WaitForMoon:
                {
                    NPC.velocity *= 0.9f;
                    if (Timer > 65)
                        SwitchState(AIStates.Idle);
                }
                break;
        }
    }
}
