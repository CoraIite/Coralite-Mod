using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 一阶段 → 二阶段的切换。<br/><br/>
    /// <b>这个状态目前是半成品，原样保留</b>：旧实现只有 <c>NPC.TargetClosest()</c> 一行，
    /// 改判定盒（<c>ApplyPhase2Hitbox</c>）与构造影子玩家（<c>ExchangeToPhase2VisualOnly</c>）两句都被作者注释掉了。<br/><br/>
    /// <b>修掉的软锁</b>：旧 <c>ServerUpdate</c> 直接 <c>return Create(SmashDown)</c>，而 <c>SmashDown</c> 既没有招式体也没有
    /// <c>[VaultState]</c> 注册 —— <c>VaultStateRegistry.Create</c> 返回 <see langword="null"/>，返回 null 等于不切换，
    /// 于是本体会永远停在这个状态里（还每帧记一条注册表错误日志）。<br/>
    /// 现在退回 hub。二阶段做出来之后，把出口改成 <c>Create(ShadowBallStateId.SmashDown)</c> 即可，
    /// 届时它会是一条真实注册的路径。
    /// </summary>
    [VaultState((int)ShadowBallStateId.P1ToP2Exchange, typeof(ShadowBallContext))]
    public sealed class ShadowBallP1ToP2ExchangeState : ShadowBallStateBase
    {
        public override ShadowBallStateId StateIndex => ShadowBallStateId.P1ToP2Exchange;

        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            ctx.Npc.TargetClosest();
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.DeclareKeep();
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
            => EndAttack(ctx);
    }
}
