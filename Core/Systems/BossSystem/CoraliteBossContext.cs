using InnoVault;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// Coralite Boss AI 上下文基类，封装 NPC 引用、Blackboard 与同步 ai 槽约定。
    /// </summary>
    public abstract class CoraliteBossContext : INpcStateContext
    {
        /// <summary>FSM 状态 ID，占用 <c>npc.ai[0]</c> 并由 <see cref="AiSlotNetSync{TContext}"/> 同步。</summary>
        public const int StateAiSlot = 0;

        /// <summary>攻击随机种子，占用 <c>npc.ai[1]</c>。</summary>
        public const int AttackSeedAiSlot = 1;

        /// <summary>招式内子状态，占用 <c>npc.ai[2]</c>。</summary>
        public const int SonStateAiSlot = 2;

        /// <summary>同步计时器，占用 <c>npc.ai[3]</c>。</summary>
        public const int SyncTimerAiSlot = 3;

        public NPC Npc { get; }
        public ModNPC ModNpc { get; }
        public Blackboard Blackboard { get; }

        protected CoraliteBossContext(NPC npc, ModNPC modNpc, Blackboard blackboard = null)
        {
            Npc = npc;
            ModNpc = modNpc;
            Blackboard = blackboard ?? new Blackboard();
        }

        public ref float AttackSeed => ref Npc.ai[AttackSeedAiSlot];
        public ref float SonState => ref Npc.ai[SonStateAiSlot];
        public ref float SyncTimer => ref Npc.ai[SyncTimerAiSlot];

        /// <summary>
        /// 从已同步的 <see cref="AttackSeed"/> 派生确定性 RNG；两端调用顺序一致时结果一致。
        /// </summary>
        public Random CreateAttackRandom()
        {
            int seed = (int)AttackSeed;
            if (seed == 0)
            {
                seed = Npc.whoAmI + 1;
            }

            return new Random(seed);
        }

        /// <summary>仅权威端 roll 新种子并写入 ai 槽。</summary>
        public void RollAttackSeed()
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            AttackSeed = Main.rand.Next();
            Npc.netUpdate = true;
            Blackboard.Set(CoraliteBossKeys.AttackSeed, (int)AttackSeed);
        }

        public void ResetAttackLocals()
        {
            SonState = 0;
            SyncTimer = 0;
            if (!VaultUtils.isClient)
            {
                Npc.netUpdate = true;
            }
        }

        /// <summary>服务端生成敌对弹幕；客户端返回 -1。</summary>
        public int SpawnHostile(IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            if (VaultUtils.isClient)
            {
                return -1;
            }

            return Projectile.NewProjectile(source, position, velocity, type, damage, knockBack, owner, ai0, ai1, ai2);
        }

        /// <summary>服务端生成敌对 Mod 弹幕；客户端返回 -1。</summary>
        public int SpawnHostile<T>(IEntitySource source, Vector2 position, Vector2 velocity, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
            where T : ModProjectile
        {
            if (VaultUtils.isClient)
            {
                return -1;
            }

            return Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<T>(), damage, knockBack, owner, ai0, ai1, ai2);
        }
    }
}
