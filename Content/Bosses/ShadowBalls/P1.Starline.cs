using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

/// <summary>
/// 影子球本体的星线（Starline）招式
/// 招式流程：判断距离 -> 引力移动/快速接近 -> 持续攻击 -> 结束
/// </summary>
public partial class ShadowBall
{
    public void Starline()
    {
        // 子状态定义
        const int _0_CheckDistance = 0;        // 检查距离，决定移动方式
        const int _1_GravityMove = 1;          // 引力移动到玩家上方
        const int _2_FastApproach = 2;         // 快速接近玩家并发射环形弹幕
        const int _3_ContinuousAttack = 3;     // 持续攻击阶段
        const int _4_End = 4;                  // 结束阶段

        switch (SonState)
        {
            default:
            case _0_CheckDistance:
                {
                    // 检查与玩家的距离
                    float distance = Vector2.Distance(NPC.Center, Target.Center);
                    
                    if (distance > 16 * 30)
                    {
                        // 距离超过16*50，切换到引力移动状态
                        // 使用Recorder记录目标位置：玩家头顶+(0,-250)
                        Vector2 targetPos = Target.Center + new Vector2(0, -350);
                        GravityMoveMentReady(targetPos);
                        
                        SonState = _1_GravityMove;
                        Timer = 0;
                    }
                    else
                    {
                        // 距离较近，直接进入快速接近状态
                        SonState = _2_FastApproach;
                        Timer = 0;
                    }
                }
                break;
                
            case _1_GravityMove:
                {
                    // 引力移动，参考ShadowSpike招式
                    if (GravityMovement())
                    {
                        // 到达目标位置，切换到快速接近状态
                        SonState = _2_FastApproach;
                        Timer = 0;
                    }
                }
                break;
                
            case _2_FastApproach:
                {
                    float distance = Vector2.Distance(NPC.Center, Target.Center);
                    
                    if (distance > 16 * 30)
                    {
                        // 距离大于16*20，瞄准玩家加速
                        // 速度计算：距离越远速度越快（16*40最快=30速度，16*10最慢=10速度）
                        float targetSpeed = Helper.Lerp(10, 30, 
                            Helper.Clamp((distance - 16 * 10) / (16 * 30), 0, 1));
                        
                        Vector2 direction = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        NPC.velocity =Vector2.SmoothStep(NPC.velocity, direction * targetSpeed,Helper.Clamp(Timer/80f,0,1));
                    }
                    else
                    {
                        // 距离小于16*20，清空速度并切换到持续攻击状态
                        NPC.velocity = Vector2.Zero;
                        
                        // 发射一圈12个弹幕
                        if (!VaultUtils.isClient)
                        {
                            int damage = Helper.GetProjDamage(18, 25, 32);
                            // 记录当前NPC与玩家之间的距离
                            float baseDistance = Vector2.Distance(NPC.Center, Target.Center);
                            
                            for (int i = 0; i < 12; i++)
                            {
                                float angle = i * MathHelper.TwoPi / 12;
                                Vector2 velocity = new Vector2(12, Main.rand.NextFloat(0.015f, 0.025f));
                                
                                // 每个弹幕单独随机增减16*10
                                float randomOffset = Main.rand.NextFloat(-16 * 10, 16 * 10);
                                float projDistance = baseDistance + randomOffset;
                                
                                // ai0存储距离，ai1存储NPC索引
                                NPC.NewProjectileDirectInAI_Server<ShadowBallStar>(NPC.Center+ angle.ToRotationVector2(),
                                    velocity, damage, 0, ai0: projDistance, ai1: NPC.whoAmI);
                            }
                        }
                        
                        // 记录当前与玩家的距离，用于状态3的距离检查
                        Recorder2 = (int)Vector2.Distance(NPC.Center, Target.Center);
                        if (Recorder2<16*20)
                        {
                            Recorder2 = 16 * 20;
                        }
                        SonState = _3_ContinuousAttack;
                        Timer = 0;
                    }
                }
                break;
                
            case _3_ContinuousAttack:
                {
                    float currentDistance = Vector2.Distance(NPC.Center, Target.Center);
                    
                    // 如果与玩家的距离大于记录的距离，缓慢靠近玩家
                    if (currentDistance > Recorder2)
                    {
                        Vector2 direction = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, direction * 4, 0.05f);
                    }
                    else
                    {
                        // 靠近后减速
                        NPC.velocity *= 0.95f;
                    }
                    
                    // 每30帧向随机方向射出一个弹幕
                    if (!VaultUtils.isClient && Timer % 30 == 0)
                    {
                        int damage = Helper.GetProjDamage(18, 25, 32);

                        Vector2 randomDir = Helper.NextVec2Dir();

                        // 每个弹幕单独随机增减16*10
                        float baseDistance = Vector2.Distance(NPC.Center, Target.Center);

                        float randomOffset = Main.rand.NextFloat(-16 * 10, 16 * 10);
                        float projDistance = baseDistance + randomOffset;

                        // 使用ShadowBallStar，AI先留空（ai0和ai1设为-1表示不使用新的轨道AI）
                        NPC.NewProjectileDirectInAI_Server<ShadowBallStar>(NPC.Center+ randomDir,
                            new Vector2(12,Main.rand.NextFloat(0.015f,0.025f)), damage, 0, ai0: projDistance, ai1: NPC.whoAmI);
                    }
                    
                    // 持续60*10帧后进入状态4
                    if (Timer > 60 * 10)
                    {
                        // 遍历所有活跃弹幕，将ShadowBallStar类型的设置成消失状态
                        if (!VaultUtils.isClient)
                        {
                            foreach (Projectile proj in Main.ActiveProjectiles)
                            {
                                if (proj.type == ModContent.ProjectileType<ShadowBallStar>())
                                {
                                    // TODO: 设置消失状态的具体逻辑留空，由用户自己实现
                                }
                            }
                        }
                        
                        SonState = _4_End;
                        Timer = 0;
                    }
                }
                break;
                
            case _4_End:
                {
                    NPC.velocity *= 0.95f;
                    
                    // 等待60帧后切换状态
                    if (Timer > 60)
                    {
                        // 暂时切换回OnSpawnAnmi测试
                        SwitchState_Test(AIStates.OnSpawnAnmi);
                    }
                }
                break;
        }
    }
}
