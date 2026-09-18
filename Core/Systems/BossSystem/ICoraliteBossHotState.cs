namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// 状态的非泛型热字段视图：让 <see cref="CoraliteBossContext"/>（非泛型）不知道 <c>TContext</c> 也能对当前状态做
    /// <see cref="WriteHot"/> / <see cref="ReadHot"/>。由 <see cref="CoraliteBossState{TContext}"/> 显式实现，业务代码不用碰。
    /// </summary>
    public interface ICoraliteBossHotState
    {
        /// <summary>注册表 id，与 <c>npc.ai[0]</c> 比对判断“这包是不是本状态的”。</summary>
        int StateId { get; }

        /// <summary>本地状态计时，收包时用它减包内 Timer 得到帧差（必须在收养之前读）。</summary>
        int Timer { get; }

        /// <summary>权威端：把 Timer / Counter / Beat 与自用槽写进 <see cref="CoraliteBossContext.Hot"/>。</summary>
        void WriteHot(CoraliteBossContext ctx);

        /// <summary>客户端：从 <see cref="CoraliteBossContext.Hot"/> 收养热字段（Timer 带容差）。</summary>
        void ReadHot(CoraliteBossContext ctx);
    }

    /// <summary>
    /// 状态机的非泛型视图，供 <see cref="CoraliteBossContext"/> 反查当前状态。
    /// <see cref="CoraliteBossStateMachine{TContext}"/> 构造时把自己挂到 <see cref="CoraliteBossContext.Machine"/> 上。
    /// </summary>
    public interface ICoraliteBossMachine
    {
        /// <summary>当前状态的热字段视图；状态机未初始化或状态不是 <see cref="CoraliteBossState{TContext}"/> 时为 null。</summary>
        ICoraliteBossHotState CurrentHotState { get; }
    }
}
