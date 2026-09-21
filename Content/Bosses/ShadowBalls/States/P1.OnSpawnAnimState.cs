using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Graphics.Effects;

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
            /// <summary>球壳逐帧退开、外层逐渐显形。</summary>
            RaiseCore,
            /// <summary>球壳停在固定帧。</summary>
            Rest,
        }

        public override ShadowBallStateId StateIndex => ShadowBallStateId.OnSpawnAnim;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 出生动画每帧速度衰减。沿用旧值 Others.Anmations.cs:11。
            const float SpawnDamp = 0.95f;

            // 球壳逐帧退掉（露出核心）的总时长 = 帧表行数 × 2，每 2 帧退一行。沿用旧值 Others.Anmations.cs:67,71。
            const int SpawnShellFrames = ShadowBall.MaxFrameY * 2;

            // 球壳退完后停在的帧号。沿用旧值 Others.Anmations.cs:83。
            const int SpawnShellRestFrame = 30;

            ShadowBall boss = ctx.Boss;

            boss.LightStrength = 1f;
            boss.MaskAlpha = 0f;
            boss.LockDistancePercent = 1f;
            ctx.DeclareDamp(SpawnDamp);

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.RaiseCore:
                    if (Timer < SpawnShellFrames)
                    {
                        boss.LayerAlpha = Timer / (float)SpawnShellFrames;

                        if (Timer % 2 == 0 && boss.ShellFrame > 0)
                        {
                            boss.ShellFrame--;
                        }
                    }
                    else
                    {
                        BeatIndex = (int)Beat.Rest;
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

            if (Timer > SpawnExitFrames)
            {
                if (ctx.Boss.smallBalls==null||ctx.Boss.smallBalls.Count==0)
                {
                    return ShadowBallHubState.CommitTest(ctx, ShadowBallStateId.SummonSmallShdowBall);
                }

                return ShadowBallHubState.CommitTest(ctx, ShadowBallStateId.LunarEclipse);

            }

            return null;
        }
    }
}
