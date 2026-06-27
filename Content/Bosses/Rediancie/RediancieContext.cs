using Coralite.Core.Systems.BossSystem;

namespace Coralite.Content.Bosses.Rediancie
{
    /// <summary>
    /// 赤玉灵 Boss FSM 上下文。<br/>
    /// 复用 <see cref="CoraliteBossContext"/> 的 ai[0]=状态ID 同步约定；DamageCount/MoveCyclingType/OwnedFollowersCount 仍占用 ai[1..3]，
    /// 故其招式状态<b>不</b>走基座的 ResetAttackLocals（见 <see cref="RediancieBossState"/>）。
    /// </summary>
    public sealed class RediancieContext : CoraliteBossContext
    {
        public Rediancie Boss { get; }

        public RediancieContext(Rediancie boss) : base(boss.NPC, boss)
        {
            Boss = boss;
        }
    }
}
