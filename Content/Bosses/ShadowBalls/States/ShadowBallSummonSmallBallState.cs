using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 召唤小影子球：锁环先张开、再收拢、最后把锁扣弹出去变成小球。<br/>
    /// 旧 <c>ShadowBall.SummonSmallBall</c>（P1.SummonSmallBall.cs:12-152）；要召几个由 hub 写进 <see cref="ShadowBallContext.SummonCount"/>。<br/><br/>
    /// <b>修掉的可达软锁</b>：旧 <c>case 0</c> 在锁环耗尽时调空方法 <c>SwitchToP1P2Exchange()</c> 然后 <c>return</c>，
    /// 下一帧重跑同一分支 —— 永久卡死（P1.SummonSmallBall.cs:36-39）。现在真的切到阶段切换态；
    /// 同时 hub 的补球条件加了"还有锁可用"，锁光了就不再往这个状态里塞，不会来回弹。<br/><br/>
    /// <b>修掉的联机分叉</b>：旧代码把"挑哪几个锁扣弹出"整段包在 <c>!VaultUtils.isClient</c> 里，
    /// 于是客户端的锁扣永远保持 <c>active</c>、画面上锁一个都没飞出去。现在抽取改用同步种子派生的确定性随机源、两端同跑，
    /// 只有生成 NPC / 弹幕留在权威端。
    /// </summary>
    [VaultState((int)ShadowBallStateId.SummonSmallShdowBall, typeof(ShadowBallContext))]
    public sealed class ShadowBallSummonSmallBallState : ShadowBallStateBase
    {
        /// <summary>召唤的子拍。旧 <c>SonState</c> 0~4。</summary>
        private enum Beat
        {
            /// <summary>检查还有没有锁扣可弹。</summary>
            CheckLocks,
            /// <summary>逐渐停下，锁环切同心圆并张开。</summary>
            SlowDown,
            /// <summary>先收拢再弹出，到点把锁扣推出去变成小球。</summary>
            PushLocks,
            /// <summary>等待小球自己的出生动画。</summary>
            WaitSpawn,
            /// <summary>锁环状态收回常规旋转。</summary>
            Restore,
        }

        public override ShadowBallStateId StateIndex => ShadowBallStateId.SummonSmallShdowBall;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 减速段每帧速度衰减。沿用旧值 P1.SummonSmallBall.cs:49。
            const float SummonSlowDamp = 0.94f;

            // 减速段第几帧把锁环切成同心圆。沿用旧值 P1.SummonSmallBall.cs:51。
            const int SummonLockSwitchFrame = 10;

            // 减速段总时长；期间锁环半径倍率朝 SummonLockExpand 插值。沿用旧值 P1.SummonSmallBall.cs:55,59。
            const int SummonSlowFrames = 55;

            // 减速段锁环张开到的半径倍率与每帧插值系数。沿用旧值 P1.SummonSmallBall.cs:57。
            const float SummonLockExpandLerp = 0.07f;

            // 小球出生动画的等待时长，期间锁环半径倍率从 SummonLockPush 收回 1。沿用旧值 P1.SummonSmallBall.cs:118,120。
            const int SummonWaitFrames = 60 * 2;

            // 收尾段第几帧把锁环切回常规旋转。沿用旧值 P1.SummonSmallBall.cs:132。
            const int SummonLockRestoreFrame = 2;

            ShadowBall boss = ctx.Boss;

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.CheckLocks:
                    ctx.DeclareKeep();

                    // "还有没有锁扣"两端算得出同一个答案（锁扣的弹出现在也是两端同跑的），所以换拍放在这里；
                    // 只有"没有锁扣了该去哪"是转移，留给权威端。
                    if (ctx.Boss.HasActiveLock())
                    {
                        SwitchBeat(ctx, (int)Beat.SlowDown);
                    }

                    break;

                case Beat.SlowDown:
                    ctx.DeclareDamp(SummonSlowDamp);

                    if (Timer == SummonLockSwitchFrame)
                    {
                        boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);
                    }
                    else if (Timer < SummonSlowFrames)
                    {
                        boss.LockDistancePercent = Helper.Lerp(boss.LockDistancePercent,
                            ShadowBallDirector.SummonLockExpand, SummonLockExpandLerp);
                    }
                    else if (Timer > SummonSlowFrames)
                    {
                        boss.LockDistancePercent = ShadowBallDirector.SummonLockExpand;
                        SwitchBeat(ctx, (int)Beat.PushLocks);
                    }

                    break;

                case Beat.PushLocks:
                    UpdatePushLocks(ctx, boss);
                    break;

                case Beat.WaitSpawn:
                    ctx.DeclareKeep();
                    boss.LockDistancePercent = Helper.Lerp(ShadowBallDirector.SummonLockPush, 1f,
                        Helper.BezierEase(Timer / (float)SummonWaitFrames));

                    if (Timer > SummonWaitFrames)
                    {
                        boss.LockDistancePercent = 1f;
                        SwitchBeat(ctx, (int)Beat.Restore);
                    }

                    break;

                case Beat.Restore:
                    ctx.DeclareKeep();
                    if (Timer == SummonLockRestoreFrame)
                    {
                        boss.SwitchLockState(ShadowBall.LockStates.Normal);
                    }

                    break;
            }
        }

        /// <summary>收拢 → 弹出 → 到点推锁。推锁的抽取两端同跑，只有生成留在权威端。</summary>
        private void UpdatePushLocks(ShadowBallContext ctx, ShadowBall boss)
        {
            // 推锁段：先收拢 40 帧到 SummonLockShrink，再 10 帧弹出到 SummonLockPush。沿用旧值 P1.SummonSmallBall.cs:69,70,74,78。
            const int SummonShrinkFrames = 40;
            const int SummonPushFrames = 10;
            const float SummonLockShrink = 0.8f;

            ctx.DeclareKeep();

            int shrink = SummonShrinkFrames;
            int push = SummonPushFrames;

            if (Timer < shrink)
            {
                boss.LockDistancePercent = Helper.Lerp(ShadowBallDirector.SummonLockExpand,
                    SummonLockShrink, Helper.X2Ease(Timer / (float)shrink));
            }
            else if (Timer < shrink + push)
            {
                boss.LockDistancePercent = Helper.Lerp(SummonLockShrink,
                    ShadowBallDirector.SummonLockPush, Helper.BezierEase((Timer - shrink) / (float)push));
            }
            else if (Timer == shrink + push)
            {
                PushOutLocks(ctx, boss);
                SwitchBeat(ctx, (int)Beat.WaitSpawn);
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
                if (shadowLock.active)
                {
                    tempLocks.Add(shadowLock);
                }
            }

            Random rand = ctx.CreateAttackRandom();

            int count = ctx.SummonCount;

            for (int i = 0; i < count && tempLocks.Count > 0; i++)
            {
                ShadowBall.ShadowLock picked = tempLocks[rand.Next(tempLocks.Count)];
                tempLocks.Remove(picked);

                if (VaultUtils.isClient)
                {
                    // 客户端只跟着让这把锁不再绘制；NPC 与弹幕由权威端生成后经原版同步过来。
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
            // 收尾段总时长（旧写法是 53 + 100）。沿用旧值 P1.SummonSmallBall.cs:137。
            const int SummonEndFrames = 53 + 100;

            switch ((Beat)BeatIndex)
            {
                case Beat.CheckLocks:
                    // 还停在这一拍就说明锁扣已经耗尽（有锁的话 SharedUpdate 本帧就换拍了）。
                    // 影子量耗尽 = 设计文档说的"小球被打完后……进入二阶段"。
                    // 旧代码这里调的是空方法 SwitchToP1P2Exchange() 然后 return，下一帧重跑本分支，永久卡死。
                    return Create(ShadowBallStateId.P1ToP2Exchange) ?? EndAttack(ctx);

                case Beat.Restore:
                    if (Timer > SummonEndFrames)
                    {
                        //return EndAttack(ctx);
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStateId.Gravity);

                    }

                    return null;

                default:
                    return null;
            }
        }
    }
}
