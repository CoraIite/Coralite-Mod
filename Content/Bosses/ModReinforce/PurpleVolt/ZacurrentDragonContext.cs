using Coralite.Core.Systems.BossSystem;
using InnoVault;
using System;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    /// <summary>
    /// 紫伏龙 Boss FSM 上下文。<br/>
    /// 复用 <see cref="CoraliteBossContext"/> 的 ai 槽约定：
    /// <c>ai[0]</c>=状态ID（同步） / <c>ai[1]</c>=AttackSeed（同步） / <c>ai[2]</c>=SonState（同步） / <c>ai[3]</c>=Timer（同步）。
    /// </summary>
    public sealed class ZacurrentDragonContext : CoraliteBossContext
    {
        public ZacurrentDragon Boss { get; }

        public ZacurrentDragonContext(ZacurrentDragon boss) : base(boss.NPC, boss)
        {
            Boss = boss;
        }

        /// <summary>与旧代码 <c>Timer</c> 对齐，使用同步 ai[3] 槽。</summary>
        public ref float Timer => ref SyncTimer;

        /// <summary>由已同步的 <see cref="CoraliteBossContext.AttackSeed"/> 派生的确定性随机源（双端一致）。</summary>
        public Random AttackRandom => Boss.AttackRandom;
    }
}
