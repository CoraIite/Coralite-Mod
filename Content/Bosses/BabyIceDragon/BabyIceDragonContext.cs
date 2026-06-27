using Coralite.Core.Systems.BossSystem;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    /// <summary>
    /// 小冰龙宝宝 Boss FSM 上下文。<br/>
    /// 复用 <see cref="CoraliteBossContext"/> 的 ai[0]=状态ID 同步约定；其余招式内部量（Timer/movePhase 等）仍由 Boss 自行管理。
    /// </summary>
    public sealed class BabyIceDragonContext : CoraliteBossContext
    {
        public BabyIceDragon Boss { get; }

        public BabyIceDragonContext(BabyIceDragon boss) : base(boss.NPC, boss)
        {
            Boss = boss;
        }
    }
}
