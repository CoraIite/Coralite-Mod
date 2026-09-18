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
            ShadowBall boss = ctx.Boss;

            boss.LightStrength = 1f;
            boss.MaskAlpha = 0f;
            boss.LockDistancePercent = 1f;
            ctx.DeclareDamp(ShadowBallDirector.SpawnDamp);

            // 天空是纯表现；旧代码没门控，专用服上 SkyManager 不可用。
            if (!Main.dedServ && Timer == 1 && !SkyManager.Instance[nameof(StarlinesSky)].IsActive())
            {
                SkyManager.Instance.Activate(nameof(StarlinesSky));
            }

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.RaiseCore:
                    if (Timer < ShadowBallDirector.SpawnShellFrames)
                    {
                        boss.LayerAlpha = Timer / (float)ShadowBallDirector.SpawnShellFrames;

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
                    boss.ShellFrame = ShadowBallDirector.SpawnShellRestFrame;
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            if (Timer > ShadowBallDirector.SpawnExitFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
