using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 二阶段（梦境）宏观态 —— <b>第一步交付里的过渡态</b>。<br/>
    /// 二阶段的 20 余招还挂在旧 <c>Phase.P2_Dream.cs</c> 的 <c>localAI[0]</c> 招式 switch 上，这个状态负责把它整块托住：
    /// 每帧把控制权交给旧 <c>Dream_Phase2()</c>，招式结束由旧 <c>SetPhase2States()</c> 自己换招，转三阶段由
    /// <c>OnExchangeToP3()</c> 登记换态请求，仍然只从 <c>ServerUpdate</c> 出门。<br/>
    /// 第二步会把这些招拆成 <c>NPDream*State</c>，届时本类退化成纯粹的 hub（只剩选招表与 <c>Commit</c>）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.dream_P2, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamP2State : NightmarePlanteraStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.dream_P2;

        /// <summary>整个阶段都跑在这一个状态里，超时兜底必须关掉——否则 40 秒后会被踢回选招口。</summary>
        protected override int TimeoutFrames => int.MaxValue;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);

            // 未迁移的招式体没有热字段，子拍只活在 ai[2]/ai[3] 里，客户端只能靠"速度快就每帧同步"的遗留阀压住漂移。
            // 二阶段拆平之后（第二步）这两行连同 localAI[0] 一起删掉。
            ctx.UseLegacySpeedValve = true;

            if (VaultUtils.isClient)
            {
                return;
            }

            // 旧招式 switch 的 default 分支就是"没有合法招 → 重新选招"，所以这里不需要额外挑第一招；
            // 只有从脱战 / 狂暴回来时 localAI[0] 可能停在某一招上，交给它自己跑完。
            ctx.Boss.haveBeenPhase2 = true;
        }

        public override void OnExit(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnExit(machine, ctx);
            ctx.UseLegacySpeedValve = false;
        }

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.EnsureRotateTentacles();
            boss.Dream_Phase2();
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => null;
    }
}
