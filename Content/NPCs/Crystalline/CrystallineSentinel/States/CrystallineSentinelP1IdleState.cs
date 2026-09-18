using Coralite.Content.NPCs.Crystalline.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 一阶段站立（连接段）：原地不动、每 45 帧做一次决策（先试开盾、再试出招），
    /// 入场预充的帧数倒数完就去闲逛；脱战且掉过血时顺手回 10% 血并把碎岩机会还回来。<br/>
    /// 旧 <c>P1Idle</c>（CrystallineSentinel.cs:656-703）。喘息长度由上一手收招时给的预充帧数决定，本状态不自带时长。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P1Idle, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP1IdleState : CrystallineSentinelStateBase
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P1Idle;

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            // 一动不动
            ctx.DeclareStandStill();
            SetFrame(ctx, 0, 0);
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (CountdownTimer % CrystallineSentinelDirector.P1DecisionInterval == 0)
            {
                IVaultState<CrystallineSentinelContext> next = CrystallineSentinelHubState.TryGuard(ctx)
                    ?? CrystallineSentinelHubState.TryAttack(ctx);
                if (next != null)
                {
                    return next;
                }
            }

            if (CountdownTimer < 0)
            {
                Heal(ctx);
                return CrystallineSentinelHubState.ToState(ctx, CrystallineSentinelStateId.P1Walking,
                    Main.rand.Next(CrystallineSentinelDirector.P1WalkFramesMin, CrystallineSentinelDirector.P1WalkFramesMax), true);
            }

            return null;
        }

        /// <summary>
        /// 脱战（仇恨见底）且掉过血：回 10% 上限血并清掉碎岩的一次性闸——浮石重新长回来，下一次飞弹又会变成碎岩。<br/>
        /// 旧代码把浮石粒子也生成在这个服务端分支里，联机时客户端永远看不到浮石回来；
        /// 现在只在这里改同步事实，粒子交给表现层按 <c>ReleasedRock</c> 的跳变重建。
        /// </summary>
        private static void Heal(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            if (ctx.AggroCounter >= 0 || npc.life >= npc.lifeMax)
            {
                return;
            }

            int count = (int)(npc.lifeMax * CrystallineSentinelDirector.P1IdleHealRatio);
            ctx.ReleasedRock = false;

            npc.life += count;
            if (npc.life > npc.lifeMax)
            {
                npc.life = npc.lifeMax;
            }

            npc.HealEffect(count);
            ctx.MarkDecision();
        }
    }
}
