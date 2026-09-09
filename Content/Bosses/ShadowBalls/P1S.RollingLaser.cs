using Coralite.Content.Prefixes.FairyWeaponPrefixes;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class SmallShadowBall
{
    public void RollingLaser(NPC bigBall)
    {
        const int _1_GatherAndSlowDown = 0;
        const int _1_FireByLayer = 1;
        ShadowBall ball = (bigBall.ModNPC as ShadowBall);

        int layer = (int)Recorder2;
        ShellMove();

        switch (SonState)
        {
            default:
            case _1_GatherAndSlowDown://根据球层移动位置
                {

                    if (Timer > 150)
                    {
                        SonState = _1_FireByLayer;
                        Timer = 0;
                    }
                }
                break;
            case _1_FireByLayer:
                {

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


        void ShellMove()
        {
            int shell = (int)Recorder2;
            float index = (int)Recorder;
            int howmany = (int)Recorder3;

            float zyRot = MathHelper.PiOver2;
            float xyRot = 0;
            float baseRot = ball.LockTimer * 0.005f * shell;

            Vector2 targetPos = _3DRotate(index / howmany, 120 + shell * 30, baseRot, zyRot, xyRot) + bigBall.Center;

            NPC.Center = Vector2.SmoothStep(NPC.Center, targetPos, 0.2f);
            NPC.rotation = NPC.rotation.AngleLerp((NPC.Center - bigBall.Center).ToRotation(), 0.2f);
        }
    }
}
