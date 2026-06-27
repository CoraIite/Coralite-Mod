using Coralite.Core.Systems.BossSystem;
using InnoVault;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 梦魇世纪花 Boss FSM 上下文。<br/>
    /// ai[0]=宏观阶段/顶层状态 ID，ai[1]=AttackSeed，ai[2]=SonState，ai[3]=Timer。<br/>
    /// localAI[0]=招式 State，localAI[1]=MoveCount（经 SendExtraAI 同步）。
    /// </summary>
    public sealed class NightmarePlanteraContext : CoraliteBossContext
    {
        public const int AttackStateLocalSlot = 0;
        public const int MoveCountLocalSlot = 1;

        public NightmarePlantera Boss { get; }

        public NightmarePlanteraContext(NightmarePlantera boss) : base(boss.NPC, boss)
        {
            Boss = boss;
        }

        public ref float Timer => ref SyncTimer;

        public ref float AttackState => ref Npc.localAI[AttackStateLocalSlot];

        public ref float MoveCount => ref Npc.localAI[MoveCountLocalSlot];

        public Random AttackRandom => Boss.AttackRandom;

        public int MacroPhase => Boss.CurrentMacroPhase;

        public void SyncAttackFields()
        {
            if (!VaultUtils.isClient)
            {
                Npc.netUpdate = true;
            }
        }
    }
}
