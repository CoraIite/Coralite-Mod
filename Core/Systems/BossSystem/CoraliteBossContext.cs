using InnoVault.StateMachines;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// Coralite Boss AI 上下文基类，封装 NPC 引用、Blackboard 与同步 ai 槽约定；
    /// 并承载联机契约的三件基座：热字段槽 <see cref="Hot"/>、客户端纠偏器 <see cref="Net"/>、每帧声明总线的默认值 <see cref="BeginFrameDefaults"/>。<br/><br/>
    /// <b>主控接线（Phase 1 逐字镜像 Rediancie）</b><br/>
    /// <c>SendExtraAI(writer)  => Ctx.WriteNet(writer);</c><br/>
    /// <c>ReceiveExtraAI(reader) => Ctx.ReadNet(reader);</c><br/>
    /// <c>AI()</c> 固定顺序：<br/>
    /// <c>EnsureAiMachine();</c>（懒构造；初态从 ai[0] 重建）<br/>
    /// <c>if (VaultUtils.isClient) Ctx.Net.BeginClientFrame(NPC);</c><br/>
    /// <c>FindTarget();  Ctx.UpdateFacts();  Ctx.BeginFrameDefaults();</c><br/>
    /// <c>StateMachine.Update();</c>（状态只写声明；转移仅 ServerUpdate 返回值）<br/>
    /// <c>Ctx.ConsumePendingHotAdopt();</c><br/>
    /// <c>ApplyDeclaredMovement();</c>（两端同跑）<br/>
    /// <c>if (!Main.dedServ) UpdatePresentation();</c><br/>
    /// <c>if (VaultUtils.isClient) Ctx.Net.EndClientFrame(NPC);</c>
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

        /// <summary>
        /// 心跳兜底默认帧数。≥ 45：心跳只是慢频安全网，几像素的纠偏由 <see cref="Net"/> 消化；
        /// 战斗中真正的快照率由玩家命中率决定（约 12 Hz），再快的定时同步只会变成硬快照发生器。
        /// </summary>
        public const int DefaultHeartbeatFrames = 45;

        /// <summary>Timer 收养容差（帧）。只差一两帧是网络抖动常态，硬对齐会让 <c>Timer == N</c> 型一次性拍被跳过或重放。</summary>
        public const int TimerAdoptTolerance = 2;

        /// <summary>表现偏移每帧衰减系数：一次性后坐 / 抖动写进 <see cref="DrawOffset"/> 后约 10 帧自然归零。</summary>
        private const float DrawOffsetDecay = 0.8f;

        public NPC Npc { get; }
        public ModNPC ModNpc { get; }
        public Blackboard Blackboard { get; }

        /// <summary>热字段槽，随 <c>SendExtraAI</c> 过线；权威端每包前由当前状态 <c>WriteHot</c>，客户端收包后 <c>ReadHot</c>。</summary>
        public CoraliteBossHotSlots Hot { get; } = new CoraliteBossHotSlots();

        /// <summary>客户端位置纠偏器（替代原版 netOffset 平滑）。</summary>
        public CoraliteBossNetSmoother Net { get; } = new CoraliteBossNetSmoother();

        /// <summary>状态机的非泛型视图，由 <see cref="CoraliteBossStateMachine{TContext}"/> 构造时挂上。</summary>
        public ICoraliteBossMachine Machine { get; internal set; }

        /// <summary>
        /// 遗留路径：速度 &gt; 6 px/f 时每帧 <c>netUpdate</c> 的抗抽搐阀。<b>默认 true</b>，保住未迁移 boss 的现状；
        /// 迁移到新契约（热字段 + 纠偏器 + 决策点 <see cref="MarkDecision"/>）的 boss 在构造上下文时置 false，改走 <see cref="HeartbeatFrames"/> 心跳。
        /// </summary>
        public bool UseLegacySpeedValve { get; set; } = true;

        /// <summary>心跳兜底间隔（帧），仅 <see cref="UseLegacySpeedValve"/> 为 false 时生效；≤ 0 关闭心跳。</summary>
        public int HeartbeatFrames { get; set; } = DefaultHeartbeatFrames;

        /// <summary>心跳计数（权威端），由 <see cref="CoraliteBossState{TContext}"/> 的 OnUpdate 推进。</summary>
        internal int HeartbeatCounter { get; set; }

        /// <summary>
        /// 表现偏移：抖动、后坐、随机偏移写这里，<c>PreDraw</c> 把它加到绘制位上；<c>npc.position</c> 不动，判定盒不跟抖（C8）。
        /// 每帧在 <see cref="BeginFrameDefaults"/> 里按 <see cref="DrawOffsetDecay"/> 自衰减，状态可每帧重声明，也可一次性踢一脚让它自己收。
        /// </summary>
        public Vector2 DrawOffset { get; set; }

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
        /// 决策点：权威端标记 <c>netUpdate</c>（客户端调用无效果）。换态由框架自动标，其余——锁向、瞬移、出手、目标切换、掷骰、换拍——
        /// 由状态在做出决定的那一帧调用（C2）。不要按定时器调用它。
        /// </summary>
        public void MarkDecision()
        {
            if (!VaultUtils.isClient)
            {
                Npc.netUpdate = true;
            }
        }

        /// <summary>
        /// 每帧开头把所有声明通道清回安全默认值、包络量自衰减（D3）。子类 override 后<b>先调 base</b> 再补自己的通道；
        /// 新增通道时必须同步在这里补默认值，漏声明的通道回落到无害状态而不是留着上一状态的残值。
        /// </summary>
        public virtual void BeginFrameDefaults()
        {
            Vector2 offset = DrawOffset * DrawOffsetDecay;
            DrawOffset = offset.LengthSquared() < 0.25f ? Vector2.Zero : offset;
        }

        /// <summary>
        /// 权威端 <c>SendExtraAI</c> 接线：当前状态先把热字段写进 <see cref="Hot"/>，再整块写出。
        /// 这一包与位置 / 速度 / ai[] 同帧出门，换态那一包里就带着新状态此刻的计时器。
        /// </summary>
        public void WriteNet(BinaryWriter writer)
        {
            if (!VaultUtils.isClient)
            {
                Machine?.CurrentHotState?.WriteHot(this);
            }

            Hot.Write(writer);
            WriteFacts(writer);
        }

        /// <summary>
        /// 子类：写出 boss 级的同步事实（客户端运动数学或判定会读到、又不属于某个状态的量，如弹药数、阶段旗）。
        /// 与 <see cref="ReadFacts"/> 成对、定长、顺序一致；状态私有量走 <see cref="CoraliteBossState{TContext}.WriteHot"/> 而不是这里。
        /// </summary>
        protected virtual void WriteFacts(BinaryWriter writer) { }

        /// <summary>子类：读入 <see cref="WriteFacts"/> 写出的事实。此时已在包处理时刻，不要在这里换态或掷骰。</summary>
        protected virtual void ReadFacts(BinaryReader reader) { }

        /// <summary>
        /// 客户端 <c>ReceiveExtraAI</c> 接线（此时 position / velocity / ai[] 已被服务端值覆盖）：<br/>
        /// 1. 先把包读完（流对齐）；<br/>
        /// 2. 同一状态下两端 Timer 应相等，差值就是这一包的帧相位偏差——<b>在收养 Timer 之前</b>算出 frameDelta（钳 ±<see cref="TimerAdoptTolerance"/>）；<br/>
        /// 3. ai[0] 与当前状态 id 一致 → 立即 <c>ReadHot</c>；不一致 → 置 <see cref="CoraliteBossHotSlots.PendingAdopt"/>，
        ///    等下一帧 <c>StateMachine.Update()</c> 完成 NetSync 换态后收养（换态钩子在 <see cref="CoraliteBossStateMachine{TContext}"/>，兜底在 <see cref="ConsumePendingHotAdopt"/>）；<br/>
        /// 4. 交给纠偏器投影 + 对账。
        /// </summary>
        public void ReadNet(BinaryReader reader)
        {
            Hot.Read(reader);
            ReadFacts(reader);

            ICoraliteBossHotState state = Machine?.CurrentHotState;
            bool sameState = state != null && state.StateId == (int)Npc.ai[StateAiSlot];

            int frameDelta = 0;
            if (sameState)
            {
                int delta = state.Timer - (int)Hot[CoraliteBossHotSlots.Timer];
                if (Math.Abs(delta) <= TimerAdoptTolerance)
                {
                    frameDelta = delta;
                }
            }

            if (sameState)
            {
                Hot.PendingAdopt = false;
                state.ReadHot(this);
            }
            else
            {
                Hot.PendingAdopt = true;
            }

            Net.OnSnapshot(Npc, frameDelta);
        }

        /// <summary>
        /// 客户端：若上一包带来了换态、热字段还挂在 <see cref="Hot"/> 上，此刻（<c>StateMachine.Update()</c> 之后）新状态已就位，把它收养进去。
        /// 正常情况下换态钩子已经收养过，这里是状态机漏钩（注册表缺 id 等）时的兜底；权威端无事可做。
        /// </summary>
        public void ConsumePendingHotAdopt()
        {
            if (!Hot.PendingAdopt)
            {
                return;
            }

            ICoraliteBossHotState state = Machine?.CurrentHotState;
            if (state == null || state.StateId != (int)Npc.ai[StateAiSlot])
            {
                return;
            }

            Hot.PendingAdopt = false;
            state.ReadHot(this);
        }

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
