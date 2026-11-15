using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.P1ToP2Exchange, typeof(ShadowBallContext))]
    public sealed class P1ToP2Exchange : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.P1ToP2Exchange;

        private enum Beat
        {
            /// <summary>
            /// 随便晃一晃和生成水银粒子
            /// </summary>
            Shaking,
            /// <summary>
            /// 生成一些冲击波和转阶段特效
            /// </summary>
            Burst,
            Over
        }

        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            ctx.Npc.TargetClosest();
        }

        public override void OnExit(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.Npc.dontTakeDamage = false;
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.SetVelocityLetItGo();
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
            => EndAttack(ctx);
    }
}
