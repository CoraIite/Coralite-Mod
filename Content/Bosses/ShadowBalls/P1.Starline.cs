using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

/// <summary>
/// 影子球本体的星线（Starline）招式
/// 招式流程：移动到玩家上方 -> 释放旋转的星星弹幕，同时小球发射激光
/// </summary>
public partial class ShadowBall
{
    public void Starline()
    {
        // 子状态定义
        const int _0_MoveAbovePlayer = 0;  // 移动到玩家上方
        const int _1_ReleaseStars = 1;      // 释放星星弹幕

        switch (SonState)
        {
            default:
            case _0_MoveAbovePlayer:
                {
                    // 目标位置：玩家正上方360像素
                    Vector2 targetPosition = Target.Center + new Vector2(0, -360);
                    // 使用Lerp平滑移动，速度12，插值系数0.08
                    NPC.velocity = Vector2.Lerp(NPC.velocity,
                        (targetPosition - NPC.Center).SafeNormalize(Vector2.Zero) * 12, 0.08f);

                    // 45帧后开始攻击
                    if (Timer > 45)
                    {
                        // 让所有小球开始Starline攻击（发射激光）
                        if (!BeginSmallBallAttack(SmallShadowBall.AIStates.Starline))
                            return;

                        SonState = _1_ReleaseStars;
                        Timer = 0;
                    }
                }
                break;
            case _1_ReleaseStars:
                {
                    // 速度逐渐衰减
                    NPC.velocity *= 0.94f;
                    
                    // 每6帧生成一个星星弹幕，共生成12个（72帧内）
                    if (!VaultUtils.isClient && Timer < 72 && Timer % 6 == 0)
                    {
                        // 角度计算：每次递增约124.4度（2.17弧度），形成旋转图案
                        float angle = Timer / 6 * 2.17f;
                        int damage = Helper.ScaleValueForDiffMode(18, 25, 32, 40);
                        // 生成星星弹幕，速度9，ai0存储BOSS索引，ai1存储初始角度
                        NPC.NewProjectileDirectInAI_Server<ShadowBallStar>(NPC.Center,
                            angle.ToRotationVector2() * 9, damage, 0, ai0: NPC.whoAmI, ai1: angle);
                    }

                    // 结束条件：300帧后且小球就绪，或超过360帧强制结束
                    if ((Timer > 300 && CheckSmallBallReady()) || Timer > 360)
                        SwitchP1State();
                }
                break;
        }
    }
}
