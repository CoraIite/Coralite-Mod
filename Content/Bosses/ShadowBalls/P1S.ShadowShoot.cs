using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class SmallShadowBall
{
    public void ShadowShoot(NPC owner)
    {
        const int GatherNearOwner = 0;
        const int ReleaseMissiles = 1;

        Player targetPlayer = Main.player[owner.target];
        int count = Math.Max(1, (owner.ModNPC as ShadowBall).smallBalls.Count);
        Vector2 targetPosition = owner.Center
            + (selfIndex * MathHelper.TwoPi / count).ToRotationVector2() * 105;

        switch (SonState)
        {
            default:
            case GatherNearOwner:
                {
                    MoveToAttackPosition(targetPosition);
                    if (Timer > 55)
                    {
                        SonState = ReleaseMissiles;
                        Timer = 0;
                    }
                }
                break;
            case ReleaseMissiles:
                {
                    NPC.velocity *= 0.84f;
                    if (!VaultUtils.isClient && Timer > 10 && Timer % 38 == selfIndex % 5)
                        NPC.NewProjectileDirectInAI_Server<ShadowBallShadowMissile>(NPC.Center,
                            (targetPlayer.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 7,
                            Helper.ScaleValueForDiffMode(22, 32, 42, 52), 0,
                            ai0: owner.target);

                    if (Timer > 165)
                        SwitchState(AIStates.Idle);
                }
                break;
        }
    }
}
