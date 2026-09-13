using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

/// <summary>
/// 小影子球的星线（Starline）招式
/// 招式流程：移动到环绕轨道位置 -> 瞄准玩家并发射激光
/// 配合大球的星星弹幕形成弹幕组合
/// </summary>
public partial class SmallShadowBall
{
    public void Starline(NPC bigball)
    {
        // 子状态定义
        const int _0_MoveToOrbit = 0;  // 移动到环绕轨道
        const int _1_FireLaser = 1;     // 发射激光

        Player targetPlayer = Main.player[bigball.target];
        // 获取当前存在的小球数量，至少为1
        int count = Math.Max(1, (bigball.ModNPC as ShadowBall).smallBalls.Count);
        // 根据自身索引计算环绕角度，使小球均匀分布在圆周上
        float angle = selfIndex * MathHelper.TwoPi / count;
        // 目标位置：大球上方190像素，再沿角度偏移150像素形成圆环
        Vector2 targetPosition = bigball.Center + new Vector2(0, -190) + angle.ToRotationVector2() * 150;

        switch (SonState)
        {
            default:
            case _0_MoveToOrbit:
                {
                    // 移动到目标轨道位置
                    MoveToAttackPosition(targetPosition);
                    // 55帧后进入发射状态
                    if (Timer > 55)
                    {
                        SonState = _1_FireLaser;
                        Timer = 0;
                    }
                }
                break;
            case _1_FireLaser:
                {
                    // 速度衰减
                    NPC.velocity *= 0.82f;
                    // 平滑旋转朝向玩家
                    NPC.rotation = NPC.rotation.AngleLerp((targetPlayer.Center - NPC.Center).ToRotation(), 0.1f);

                    // 延迟发射：基础延迟45帧 + 自身索引 * 8帧
                    // 这样多个小球会依次发射，形成连续激光效果
                    if (!VaultUtils.isClient && Timer == 45 + selfIndex * 8)
                        NPC.NewProjectileDirectInAI_Server<SmallLaser>(NPC.Center, Vector2.Zero,
                            Helper.ScaleValueForDiffMode(25, 35, 45, 55), 0,
                            ai0: NPC.whoAmI, ai1: 32);  // ai1=32 表示激光持续时间

                    // 225帧后返回待机状态
                    if (Timer > 225)
                        SwitchState(AIStates.Idle);
                }
                break;
        }
    }
}
