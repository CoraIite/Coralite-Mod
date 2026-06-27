using Coralite.Core.Systems.BossSystem;
using InnoVault;
using System;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    /// 影子球 Boss FSM 上下文。
    /// </summary>
    public sealed class ShadowBallContext : CoraliteBossContext
    {
        public ShadowBall Boss { get; }

        public ShadowBallContext(ShadowBall boss) : base(boss.NPC, boss)
        {
            Boss = boss;
        }

        public int Phase
        {
            get => Blackboard.Get(CoraliteBossKeys.Phase, (int)ShadowBall.AIPhases.WithSmallBalls);
            set => Blackboard.Set(CoraliteBossKeys.Phase, value);
        }

        /// <summary>与旧代码 <c>Timer</c> 对齐，使用同步 ai 槽。</summary>
        public ref float Timer => ref SyncTimer;

        public Random AttackRandom => Boss.AttackRandom;

        public void SyncSonState(float value)
        {
            SonState = value;
            if (!VaultUtils.isClient)
            {
                Npc.netUpdate = true;
            }
        }

        public void SyncTimerValue(float value)
        {
            Timer = value;
            if (!VaultUtils.isClient)
            {
                Npc.netUpdate = true;
            }
        }
    }
}
