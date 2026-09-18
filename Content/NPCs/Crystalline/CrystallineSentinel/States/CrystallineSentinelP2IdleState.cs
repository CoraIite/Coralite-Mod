using Coralite.Content.NPCs.Crystalline.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 二阶段悬浮（连接段）：绕到玩家背侧上方 120 px 处游走，每 30 帧换一次漂移方向，
    /// 每 35 帧做一次攻击决策（入场预充的静默帧内不决策，这就是两招之间的喘息）。<br/>
    /// 旧 <c>P2Idle</c>（CrystallineSentinel.cs:1237-1276）。收招回来预充 −60 帧、休息后 −120 帧、转阶段后 20 帧。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P2Idle, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP2IdleState : CrystallineSentinelStateBase
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P2Idle;

        public override CrystallineSentinelCommon Common => CrystallineSentinelCommon.PhaseTwo;

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            ctx.DeclareDirect();
            P2BodyFrame(ctx);

            if (AttackTimer % CrystallineSentinelDirector.P2WanderInterval == 0)
            {
                ctx.Npc.velocity = ctx.Npc.velocity.RotatedBy(Wobble(ctx, CrystallineSentinelDirector.P2WanderAngle));
            }

            HoverBesideTarget(ctx, CrystallineSentinelDirector.P2IdleHoverSpeed);
            CollideSpeed(ctx);
            SpeedUp(ctx, CrystallineSentinelDirector.P2IdleMaxSpeed, CrystallineSentinelDirector.P2IdleAccel);

            ctx.FaceTarget();
            ctx.Npc.velocity *= CrystallineSentinelDirector.P2IdleDamp;
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (AttackTimer >= 0 && AttackTimer % CrystallineSentinelDirector.P2DecisionInterval == 0)
            {
                return CrystallineSentinelHubState.TryAttack(ctx);
            }

            return null;
        }
    }
}
