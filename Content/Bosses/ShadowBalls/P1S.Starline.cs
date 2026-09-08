using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class SmallShadowBall
{
    public void Starline(NPC owner)
    {
        const int MoveToOrbit = 0;
        const int FireLaser = 1;

        Player targetPlayer = Main.player[owner.target];
        int count = Math.Max(1, (owner.ModNPC as ShadowBall).smallBalls.Count);
        float angle = selfIndex * MathHelper.TwoPi / count;
        Vector2 targetPosition = owner.Center + new Vector2(0, -190) + angle.ToRotationVector2() * 150;

        switch (SonState)
        {
            default:
            case MoveToOrbit:
                {
                    MoveToAttackPosition(targetPosition);
                    if (Timer > 55)
                    {
                        SonState = FireLaser;
                        Timer = 0;
                    }
                }
                break;
            case FireLaser:
                {
                    NPC.velocity *= 0.82f;
                    NPC.rotation = NPC.rotation.AngleLerp((targetPlayer.Center - NPC.Center).ToRotation(), 0.1f);

                    if (!VaultUtils.isClient && Timer == 45 + selfIndex * 8)
                        NPC.NewProjectileDirectInAI_Server<SmallLaser>(NPC.Center, Vector2.Zero,
                            Helper.ScaleValueForDiffMode(25, 35, 45, 55), 0,
                            ai0: NPC.whoAmI, ai1: 32);

                    if (Timer > 225)
                        SwitchState(AIStates.Idle);
                }
                break;
        }
    }
}
