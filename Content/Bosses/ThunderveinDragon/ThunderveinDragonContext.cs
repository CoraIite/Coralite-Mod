using Coralite.Core.Systems.BossSystem;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// 雷龙 Boss FSM 上下文。<br/>
    /// ai 槽约定（继承自 <see cref="CoraliteBossContext"/>）：<br/>
    /// ai[0]=顶层招式状态ID（=<see cref="ThunderveinDragon.AIStates"/> 数值），ai[1]=AttackSeed，ai[2]=SonState，ai[3]=Timer。<br/>
    /// 阶段 <see cref="ThunderveinDragon.Phase"/> 改由后备字段承载并经 SendExtraAI 同步（不再占用 ai[0]）。
    /// </summary>
    public sealed class ThunderveinDragonContext : CoraliteBossContext
    {
        public ThunderveinDragon Boss { get; }

        public ThunderveinDragonContext(ThunderveinDragon boss) : base(boss.NPC, boss)
        {
            Boss = boss;
        }
    }
}
