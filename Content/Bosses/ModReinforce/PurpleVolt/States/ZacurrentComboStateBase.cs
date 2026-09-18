using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 连段态基类。连段是一条固定序列，不是随机轮换：<see cref="ZacurrentDragonContext.Combo"/> 记当前第几手，
    /// 每手复用与单招完全相同的招式体（<c>Zacurrent*Move.Run</c>），返回 true 就过手。<br/><br/>
    /// 两件事与单招不同：<br/>
    /// · 超时兜底放宽到 <see cref="ZacurrentDirector.ComboTimeoutFrames"/>——整段比任何单招长一个量级，用单招的阈值会误杀。<br/>
    /// · 过手时只清招内量（子拍 / 计时 / 记录位）而<b>不清 Combo</b>，否则整段会从头开始。<br/><br/>
    /// 连段整段作为一个顶层状态出门，所以它和单招一样经 hub 的 <see cref="ZacurrentHubState.Commit"/> 提交、一样记账（D4）。
    /// </summary>
    public abstract class ZacurrentComboStateBase : ZacurrentAttackState
    {
        /// <inheritdoc/>
        protected override int TimeoutFrames => ZacurrentDirector.ComboTimeoutFrames;

        /// <summary>
        /// 过手：子招没结束就继续，结束了就清招内量（保留 Combo）并跳到 <paramref name="nextCombo"/>。
        /// 永远返回 false——连段整段的结束只由 <see cref="Finish"/> 判定。
        /// </summary>
        protected static bool Step(ZacurrentDragonContext ctx, bool subMoveFinished, int nextCombo)
        {
            if (!subMoveFinished)
            {
                return false;
            }

            ctx.ResetAttackFields(false);
            ctx.Combo = nextCombo;
            ctx.MarkDecision();
            return false;
        }

        /// <summary>收尾手：结束时连 Combo 一起清零，整段返回 true 回 hub。</summary>
        protected static bool Finish(ZacurrentDragonContext ctx, bool subMoveFinished)
        {
            if (!subMoveFinished)
            {
                return false;
            }

            ctx.ResetAttackFields();
            return true;
        }
    }
}
