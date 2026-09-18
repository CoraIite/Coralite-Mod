using Coralite.Content.NPCs.Crystalline.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 一阶段闲逛（连接段）：朝当前朝向慢慢走，每 45 帧做一次决策；
    /// 走到没路或预充帧数倒数完就回去站立 4~6 秒。<br/>
    /// 旧 <c>P1Walk</c>（CrystallineSentinel.cs:705-729）。入场时随机左右，所以脱战时会来回踱步。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P1Walking, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP1WalkState : CrystallineSentinelStateBase
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P1Walking;

        /// <summary>本帧已经没路可走 / 时间到了：旧代码在这一帧直接 return，连走路加速都不做。</summary>
        private bool blocked;

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            blocked = !CanWalkForward(ctx) || CountdownTimer < 0;

            if (!blocked)
            {
                ctx.DeclareGroundWalk(CrystallineSentinelDirector.P1WalkMaxSpeed, CrystallineSentinelDirector.P1WalkAccel);
            }

            WalkFrame(ctx);
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (blocked)
            {
                return CrystallineSentinelHubState.ToIdle(ctx,
                    Main.rand.Next(CrystallineSentinelDirector.P1IdleFramesMin, CrystallineSentinelDirector.P1IdleFramesMax));
            }

            if (CountdownTimer % CrystallineSentinelDirector.P1DecisionInterval == 0)
            {
                IVaultState<CrystallineSentinelContext> guard = CrystallineSentinelHubState.TryGuard(ctx);
                if (guard != null)
                {
                    // 开盾后清零计数器，避免重复触发（旧 CrystallineSentinel.cs:723-725）
                    ctx.GuardCounter = 0;
                    return guard;
                }

                IVaultState<CrystallineSentinelContext> attack = CrystallineSentinelHubState.TryAttack(ctx);
                if (attack != null)
                {
                    return attack;
                }
            }

            return null;
        }
    }
}
