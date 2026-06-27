using InnoVault.StateMachines;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// Coralite Boss 状态机共享 Blackboard 键。
    /// </summary>
    public static class CoraliteBossKeys
    {
        public static readonly BlackboardKey<int> Phase = new("coralite_boss_phase");
        public static readonly BlackboardKey<int> AttackSeed = new("coralite_boss_attack_seed");
        public static readonly BlackboardKey<int> AttackPickIndex = new("coralite_boss_attack_pick_index");
    }
}
