using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 出场动画：从裂隙里探出发光核心，球壳逐帧退开，60 帧后交给 hub 选第一手。<br/>
    /// 旧 <c>ShadowBall.OnSpawnAnmi</c>（Others.Anmations.cs:7-88）。<br/>
    /// 旧出口有两条：小球为空走 <c>SwitchP1State()</c>（真出口），否则 <c>SwitchState_Test(LunarEclipse)</c>（调试死循环的另一半）；
    /// 本状态只保留前者 —— 补球与选招的判断本来就在 hub 里，出生态不该自己挑招。
    /// </summary>
    [VaultState((int)ShadowBallStateId.OnSpawnAnim, typeof(ShadowBallContext))]
    public sealed class ShadowBallOnSpawnAnimState : ShadowBallStateBase
    {
        /// <summary>出场动画的子拍。两拍都由整段累计 <c>Timer</c> 驱动，所以换拍时<b>不</b>清 Timer（退出判定读的是同一个 Timer）。</summary>
        private enum Beat
        {
            /// <summary>核心出现</summary>
            CoreAppear,
            /// <summary>球壳逐帧退开、外层逐渐显形。</summary>
            RaiseShell,
            /// <summary>锁扣出现</summary>
            LockAppear,
            /// <summary>球壳停在固定帧。</summary>
            Rest,
        }

        public override ShadowBallStateId StateIndex => ShadowBallStateId.OnSpawnAnim;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 出生动画每帧速度衰减。沿用旧值 Others.Anmations.cs:11。
            const float SpawnDamp = 0.95f;

            // 球壳退完后停在的帧号。沿用旧值 Others.Anmations.cs:83。
            const int SpawnShellRestFrame = 30;

            ShadowBall boss = ctx.Boss;

            boss.MaskAlpha = 0f;
            boss.LockDistancePercent = 1f;
            ctx.DeclareDamp(SpawnDamp);

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.CoreAppear:
                    if (Timer < SpawnProj.SplitExpandTime)
                    {
                        break;
                    }
                    else if (Timer < SpawnProj.SplitExpandTime + SpawnProj.ShadowBallAppearTime)
                    {
                        float f = (float)(Timer - SpawnProj.SplitExpandTime) / SpawnProj.ShadowBallAppearTime;
                        f = Helper.HeavyEase(f);
                        boss.CoreScale = boss.CoreAlpha = boss.LightStrength = f;
                    }
                    else
                        SwitchBeat(ctx, (int)Beat.RaiseShell);

                    break;
                case Beat.RaiseShell:
                    if (Timer < SpawnProj.ShadowProjSpawnTime)
                    {
                        boss.LayerAlpha = Timer / (float)SpawnProj.ShadowProjSpawnTime;

                        if (Timer % 2 == 0 && boss.ShellFrame > 0)
                        {
                            boss.ShellFrame--;
                        }
                    }
                    else
                    {
                        SwitchBeat(ctx, (int)Beat.LockAppear);
                    }

                    break;
                case Beat.LockAppear:
                    if (Timer < SpawnProj.LockAppearTime)
                    {
                        int t = Timer / 3;

                        if (Timer % 3 == 0 && boss.shadowLocks.IndexInRange(t))
                        {
                            boss.shadowLocks[t].Init = false;
                        }
                    }
                    else
                    {
                        SwitchBeat(ctx, (int)Beat.Rest);
                        boss.SwitchLockState(ShadowBall.LockStates.Normal);
                    }

                    break;
                case Beat.Rest:
                    boss.ShellFrame = SpawnShellRestFrame;
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 出生动画放完、可以进入轮换的帧数。沿用旧值 Others.Anmations.cs:30。
            const int SpawnExitFrames = 60;

            if ((Beat)BeatIndex == Beat.Rest && Timer > SpawnExitFrames)
            {
                if (ctx.Boss.smallBalls == null || ctx.Boss.smallBalls.Count == 0)
                {
                    return ShadowBallHubState.CommitTest(ctx, ShadowBallStateId.SummonSmallShdowBall);
                }

                return ShadowBallHubState.CommitTest(ctx, ShadowBallStateId.GammaRayBurst);
            }

            return null;
        }
    }
}
