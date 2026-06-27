using Coralite.Core.Systems.BossSystem;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    /// <summary>
    /// 血玉灵 Boss FSM 上下文（与 Rediancie 结构平行）。<br/>
    /// 复用 <see cref="CoraliteBossContext"/> 的 ai[0]=状态ID 同步约定；DamageCount/MoveCyclingType/OwnedFollowersCount 仍占用 ai[1..3]，
    /// 故招式状态<b>不</b>走基座 ResetAttackLocals（见 <see cref="BloodiancieBossState"/>）。
    /// </summary>
    public sealed class BloodiancieContext : CoraliteBossContext
    {
        public Bloodiancie Boss { get; }

        public BloodiancieContext(Bloodiancie boss) : base(boss.NPC, boss)
        {
            Boss = boss;
        }
    }
}
