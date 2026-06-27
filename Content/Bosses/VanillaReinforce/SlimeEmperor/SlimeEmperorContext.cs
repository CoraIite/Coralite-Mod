using Coralite.Core.Systems.BossSystem;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    /// <summary>
    /// 史莱姆皇帝 Boss FSM 上下文。<br/>
    /// ai 槽约定（继承自 <see cref="CoraliteBossContext"/>）：<br/>
    /// ai[0]=顶层招式状态ID（=<see cref="SlimeEmperor.AIStates"/> 数值），ai[1]=AttackSeed，ai[2]=SonState，ai[3]=Timer。<br/>
    /// MovePhase（招式轮换计数）仅服务端使用，改为后备字段；MovingMode（王冠/常规外形）经 SendExtraAI 同步。
    /// </summary>
    public sealed class SlimeEmperorContext : CoraliteBossContext
    {
        public SlimeEmperor Boss { get; }

        public SlimeEmperorContext(SlimeEmperor boss) : base(boss.NPC, boss)
        {
            Boss = boss;
        }
    }
}
