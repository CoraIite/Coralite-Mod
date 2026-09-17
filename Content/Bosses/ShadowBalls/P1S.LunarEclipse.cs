using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public partial class SmallShadowBall
{
    public void LunarEclipse(NPC owner)
    {
        const int _0_WaitAndMove = 0;
        const int _1_ShootEclipse = 1;
        const int _2_SlowDown = 2;
        const int _3_ShootRing = 3;

        ShadowBall shadowBall = owner.ModNPC as ShadowBall;

        switch (SonState)
        {
            default:
            case _0_WaitAndMove:
                {
                    // 持续设置中心值，lerp到大球中心+大球角度的向量*100
                    // lerp插值使用时间/30
                    float lerpAmount = MathHelper.Min(Timer / 30f, 1f);
                    Vector2 targetCenter = owner.Center + owner.rotation.ToRotationVector2() * 100f;
                    NPC.Center = Vector2.Lerp(NPC.Center, targetCenter, lerpAmount);

                    // 等待30帧后切换到小球状态1
                    if (Timer >= 30)
                    {
                        // 给小球一个这个方向的速度，速度为20
                        Vector2 direction = owner.rotation.ToRotationVector2();
                        NPC.velocity = direction * 20f;

                        SonState = _1_ShootEclipse;
                        Timer = 0;
                    }
                }
                break;
            case _1_ShootEclipse:
                {
                    const int spawnProjTime = 5;
                    // 每10帧生成一个月食弹幕（先留空）
                    if (Timer % spawnProjTime == 0)
                    {
                        // TODO: 生成月食弹幕
                    }

                    // 9*10帧后切换到小球状态2
                    if (Timer >= 9* spawnProjTime)
                    {
                        SonState = _2_SlowDown;
                        Timer = 0;
                    }
                }
                break;
            case _2_SlowDown:
                {
                    // 速度每帧减慢0.9f
                    NPC.velocity *= 0.9f;

                    // 经过120帧后进入小球状态3
                    if (Timer >= 120)
                    {
                        SonState = _3_ShootRing;
                        Timer = 0;
                    }
                }
                break;
            case _3_ShootRing:
                {
                    // 每30帧生成一个圆环弹幕（先留空）
                    if (Timer % 30 == 0)
                    {
                        // TODO: 生成圆环弹幕
                    }

                    // 等待大球切换状态后自动结束（由大球控制切换到Idle）
                }
                break;
        }
    }
}
