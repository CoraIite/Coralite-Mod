using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core
{
    /// <summary>
    /// 三阶段（噩梦）招式的公共外壳。把旧 <c>Nightemare_Phase3()</c> 分派器里那圈"每招都要做的事"收进来：<br/>
    /// 天空续命 + 补触手 + 推帧动画（<see cref="NightmarePlanteraStateBase.P3Begin"/>）→ 招式本体 → 推进触手模拟（<c>P3End</c>）。<br/>
    /// 招式只需要实现 <see cref="Phase3Update"/> 与 <see cref="NightmarePlanteraStateBase.AuthorityUpdate"/>。
    /// </summary>
    internal abstract class NPNightmareStateBase : NightmarePlanteraStateBase
    {
        /// <summary>
        /// 是否由外壳每帧摆触手目标点。旧分派器里群花乱舞与瞬移闪光两招前面没有 <c>NormallySetTentacle()</c>
        /// ——它们自己按节拍摆触手（转圈、收束），外壳再摆一次会把动作抹平。
        /// </summary>
        protected virtual bool AutoTentacle => true;

        /// <summary>进本状态时是否复位三阶段表现。转阶段演出要保留二阶段的触手配色渐变，所以它关掉。</summary>
        protected virtual bool ResetPresentationOnEnter => true;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);

            if (ResetPresentationOnEnter)
            {
                ApplyPhase3Presentation(ctx);
            }
        }

        protected sealed override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            P3Begin(ctx, AutoTentacle);
            Phase3Update(machine, ctx);
            P3End(ctx);
        }

        /// <summary>招式本体（双端）：写运动 / 表现声明，推进确定性子拍，调 <c>*_Server</c> 系列生成。</summary>
        protected abstract void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx);
    }
}
