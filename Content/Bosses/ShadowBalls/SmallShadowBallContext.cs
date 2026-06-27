using InnoVault;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    /// 小影子球 FSM 上下文。ai 槽位与主球不同：<c>ai[0]</c>=主人索引，<c>ai[1]</c>=顶层状态 ID（同步），
    /// <c>ai[2]</c>=招内 SonState，<c>ai[3]</c>=AttackSeed（同步，原 Sign 槽复用为种子）。
    /// </summary>
    public sealed class SmallShadowBallContext : INpcStateContext
    {
        public const int OwnerAiSlot = 0;
        public const int StateAiSlot = 1;
        public const int SonStateAiSlot = 2;
        public const int AttackSeedAiSlot = 3;

        public NPC Npc { get; }
        public SmallShadowBall Ball { get; }

        public SmallShadowBallContext(SmallShadowBall ball)
        {
            Ball = ball;
            Npc = ball.NPC;
        }

        public ref float OwnerIndex => ref Npc.ai[OwnerAiSlot];
        public ref float AttackSeed => ref Npc.ai[AttackSeedAiSlot];
        public ref float SonState => ref Npc.ai[SonStateAiSlot];

        public Random CreateAttackRandom()
        {
            int seed = (int)AttackSeed;
            if (seed == 0)
            {
                seed = Npc.whoAmI + 1;
            }

            return new Random(seed);
        }

        public void RollAttackSeed()
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            AttackSeed = Main.rand.Next();
            Npc.netUpdate = true;
        }

        /// <summary>由主球服务端下发种子，避免子球各自 roll 导致轨迹分歧。</summary>
        public void SetAttackSeedFromMain(int seed)
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            AttackSeed = seed;
            Npc.netUpdate = true;
        }

        public void ResetAttackLocals()
        {
            SonState = 0;
            Ball.Timer = 0;
            if (!VaultUtils.isClient)
            {
                Npc.netUpdate = true;
            }
        }

        public void SyncSonState(float value)
        {
            SonState = value;
            if (!VaultUtils.isClient)
            {
                Npc.netUpdate = true;
            }
        }
    }
}
