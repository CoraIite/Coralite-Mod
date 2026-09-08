using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class SmallShadowBall
{
    public void RollingLaser(NPC owner)
    {
        const int GatherAndSlowDown = 0;
        const int FireByLayer = 1;

        int layer = selfIndex % 3;
        float radius = 120 + layer * 80;
        int direction = layer == 1 ? -1 : 1;

        switch (SonState)
        {
            default:
            case GatherAndSlowDown:
                {
                    float speed = 0.05f - Timer * 0.00025f;
                    float angle = selfIndex * 1.7f + Timer * speed * direction;
                    MoveToAttackPosition(owner.Center + angle.ToRotationVector2() * radius, 0.16f);

                    if (Timer > 150)
                    {
                        Recorder = angle;
                        SonState = FireByLayer;
                        Timer = 0;
                    }
                }
                break;
            case FireByLayer:
                {
                    float angle = Recorder + Timer * 0.0125f * direction;
                    MoveToAttackPosition(owner.Center + angle.ToRotationVector2() * radius, 0.16f);
                    NPC.rotation = NPC.rotation.AngleLerp((NPC.Center - owner.Center).ToRotation(), 0.12f);

                    int shootTime = layer * 45 + selfIndex / 3 * 7;
                    if (Timer == shootTime && !VaultUtils.isClient)
                        NPC.NewProjectileDirectInAI_Server<SmallLaser>(NPC.Center, Vector2.Zero,
                            Helper.ScaleValueForDiffMode(25, 35, 45, 55), 0,
                            ai0: NPC.whoAmI, ai1: 36);

                    if (Timer > 140)
                        SwitchState(AIStates.Idle);
                }
                break;
        }
    }
}
