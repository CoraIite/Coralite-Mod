using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Utilities;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 召唤小影子球：锁环先张开、再收拢、最后把锁扣弹出去变成小球。<br/>
    /// </summary>
    [VaultState((int)ShadowBallStates.SummonSmallShdowBall, typeof(ShadowBallContext))]
    public sealed class ShadowBallSummonSmallBallState : ShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>准备</summary>
            Ready,
            /// <summary>逐渐停下，锁环切同心圆并张开</summary>
            SlowDown,
            /// <summary>先收拢再弹出，到点把锁扣推出去变成小球</summary>
            PushLocks,
            /// <summary>等待小球自己的出生动画</summary>
            WaitSpawn,
            /// <summary>锁环状态收回常规旋转</summary>
            Restore,
        }

        public override ShadowBallStates StateIndex => ShadowBallStates.SummonSmallShdowBall;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ShadowBall boss = ctx.Boss;

            switch ((Beat)BeatIndex)
            {
                case Beat.Ready:
                    ctx.SetVelocityLetItGo();
                    ctx.CoreOffset_BackToCenter(0.2f);

                    boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);
                    SwitchBeat(ctx, (int)Beat.SlowDown);

                    break;

                case Beat.SlowDown:
                    ctx.SetVelocitySlowDown(0.95f);
                    ctx.CoreOffset_BackToCenter(0.2f);

                    ctx.SetRotationAdd(Timer / 30f * 0.1f);

                    if (Timer < 30)
                    {
                        boss.LockDistancePercent = Helper.Lerp(boss.LockDistancePercent,
                           1.5f, 0.07f);
                    }
                    else if (Timer > 30)
                    {
                        boss.LockDistancePercent = 1.5f;
                        SwitchBeat(ctx, (int)Beat.PushLocks);
                    }

                    break;

                case Beat.PushLocks:
                    {
                        const int PreTime = 40;
                        const int SummonTime = 10;

                        ctx.SetVelocitySlowDown(0.95f);
                        ctx.SetRotationAdd(0.1f);

                        float f = Timer / (float)(PreTime + SummonTime);//常规核心旋转
                        ctx.CoreOffsetAngle += 0.1f * f;
                        ctx.CoreOffsetRadius = Helper.Lerp(0, 50, Helper.BezierEase(f));
                        ctx.CoreOffset_AngledTo(0.1f);

                        if (Timer < PreTime)//锁环收紧
                        {
                            boss.LockDistancePercent = Helper.Lerp(1.5f,
                                0.6f, Helper.X2Ease(Timer / (float)PreTime));
                            boss.ShellBreath_HideCore(0);
                        }
                        else if (Timer < PreTime + SummonTime)//锁环扩大
                        {
                            float f2 = Helper.SqrtEase((Timer - PreTime) / (float)SummonTime);
                            boss.LockDistancePercent = Helper.Lerp(0.6f, 1.8f, f2);
                            boss.LayerAlpha = Helper.Lerp(1, 0.6f, f2);//球层变透明
                            boss.ShellBreath_ShowCore(2);

                        }
                        else if (Timer >= PreTime + SummonTime)
                        {
                            PushOutLocks(ctx, boss);
                            SwitchBeat(ctx, (int)Beat.WaitSpawn);
                        }
                    }

                    break;

                case Beat.WaitSpawn:
                    {
                        const int waitTime = 60 * 2;
                        float f = Timer / (float)waitTime;//常规核心旋转

                        ctx.SetRotationAdd(0.1f * (1 - f));
                        ctx.SetVelocityLetItGo();
                        boss.ShellBreath_ShowCore(2);

                        boss.LockDistancePercent = Helper.Lerp(1.8f, 1f, Helper.BezierEase(f));
                       
                        ctx.CoreOffsetAngle += 0.1f;
                        ctx.CoreOffset_AngledTo(0.1f);


                        //球层变回实心
                        boss.LayerAlpha = Helper.Lerp(0.6f, 1f, Helper.X2Ease(f));//球层变透明

                        if (Timer > waitTime)
                        {
                            boss.LayerAlpha = 1f;
                            boss.LockDistancePercent = 1f;
                            boss.SwitchLockState(ShadowBall.LockStates.Normal);
                            ctx.SetVelocityLetItGo();

                            SwitchBeat(ctx, (int)Beat.Restore);
                        }
                    }

                    break;
                case Beat.Restore:
                    {
                        boss.CommonShellBreath(2);

                        if (Timer <= 45)
                        {
                            float f = Timer / 45f;

                            ctx.CoreOffsetRadius = 50 * (1 - Helper.X2Ease(f));
                            ctx.CoreOffsetAngle += 0.1f * Helper.X2Ease(1 - f);
                            ctx.CoreOffset_AngledTo(0.1f);
                        }
                        else
                            ctx.CoreOffset_BackToCenter(0.1f);

                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 从还活着的锁扣里抽 <see cref="ShadowBallContext.SummonCount"/> 个弹出去。<br/>
        /// 抽取顺序由已同步的 <see cref="CoraliteBossContext.AttackSeed"/> 派生，两端同一串数 → 同一批锁扣（C1 / C3）。
        /// </summary>
        private static void PushOutLocks(ShadowBallContext ctx, ShadowBall boss)
        {
            // 伴随小球一起放出的追逐影子弹幕初速。沿用旧值 P1.SummonSmallBall.cs:98。
            const float SummonShadowProjSpeed = 14f;

            List<ShadowBall.ShadowLock> tempLocks = [];
            foreach (ShadowBall.ShadowLock shadowLock in boss.shadowLocks)
            {
                if (shadowLock.active&&shadowLock.index%2==0)
                {
                    tempLocks.Add(shadowLock);
                }
            }

            UnifiedRandom rand = ctx.Rand();

            int count = ctx.Boss.HowmanyPerSummon();

            for (int i = 0; i < count && tempLocks.Count > 0; i++)
            {
                ShadowBall.ShadowLock picked = tempLocks[rand.Next(tempLocks.Count)];
                tempLocks.Remove(picked);

                if (VaultUtils.isClient)
                {
                    picked.active = false;
                    continue;
                }

                NPC smallBall = NPC.NewNPCDirect(boss.NPC.GetSource_FromThis(), picked.center + picked.offset,
                    ModContent.NPCType<SmallShadowBall>(), ai0: boss.NPC.whoAmI, target: boss.NPC.target);

                boss.NPC.NewProjectileDirectInAI_Server<ShadowProj>(boss.NPC.Center,
                    (picked.center + picked.offset - boss.NPC.Center).SafeNormalize(Vector2.Zero) * SummonShadowProjSpeed,
                    0, 0, ai0: 1, ai1: smallBall.whoAmI);

                picked.LockOut(smallBall);
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Restore:
                    if (Timer > 53 + 100)
                    {
                        ctx.Hot[BossSlots.H]++;
                        ctx.Npc.netUpdate = true;
                        //return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P1Test);
                        return EndAttack(ctx);
                    }

                    return null;

                default:
                    return null;
            }
        }
    }
}
