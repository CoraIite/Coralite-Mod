using Coralite.Core.Systems.BossSystem;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core
{
    /// <summary>运动声明模式；宿主 <c>ApplyDeclaredMovement</c> 两端同跑地落地。</summary>
    public enum ZacurrentMoveMode
    {
        /// <summary>
        /// 未声明：速度保持不变。<b>这是默认值</b>——旧代码没有“漏声明就刹停”的语义，
        /// 全部招式体自管速度，默认刹停会改变手感（D10）。
        /// </summary>
        Keep,
        /// <summary>本帧由招式体直接写 <c>velocity</c>（冲刺、绕飞、追踪），宿主不再动它。</summary>
        Direct,
        /// <summary>整体衰减 <c>velocity *= DampFactor</c>（旧代码里大量的 <c>NPC.velocity *= 0.9f</c> 一类）。</summary>
        Damp,
    }

    /// <summary>
    /// 兹雷龙 FSM 上下文：每帧声明总线 + 跨帧事实 + 热字段搬运。<br/><br/>
    /// 槽位分工（C3：运动数学读到的每个量都要能在客户端重建）：<br/>
    /// · <c>ai[0]</c> 状态 ID、<c>ai[1]</c> AttackSeed、<c>ai[2]</c> <see cref="CoraliteBossContext.SonState"/> 招内子拍、
    ///   <c>ai[3]</c> <see cref="Timer"/> 招内计时 —— 全部由原版 SyncNPC 同步，招式体照旧读写它们，
    ///   所以子拍与拍点计时天然过线，两端从同一起点推进（这也是本 boss 不另设 BeatIndex 的原因：
    ///   基座的 ai[2]/ai[3] 就是它的子拍与计时槽，且比热字段多一层原版同步保障，拍点一帧不差）。<br/>
    /// · <see cref="Recorder"/> / <see cref="Recorder2"/> / <see cref="Combo"/>：旧 <c>localAI[0..1]</c> 与不同步的 C# 字段，
    ///   运动数学与弹幕朝向都读它们，改走热字段 A / B / C 随 <c>SendExtraAI</c> 原子过线。<br/>
    /// · <see cref="StateRecorder"/> / <see cref="UseMoveCount"/> / <see cref="ComboRecords"/>：只有权威端选招时读写，不过线。<br/>
    /// · <see cref="ZacurrentDragon.PurpleVolt"/> / <see cref="ZacurrentDragon.PurpleVoltCount"/>：boss 级事实，走 <see cref="WriteFacts"/>。
    /// </summary>
    public sealed class ZacurrentDragonContext : CoraliteBossContext
    {
        public ZacurrentDragon Boss { get; }

        /// <summary>当前目标玩家（<c>Npc.target</c> 原版同步；无效时指向 255 号占位玩家，不会为 null）。</summary>
        public Player Target => Main.player[Npc.target];

        public ZacurrentDragonContext(ZacurrentDragon boss) : base(boss.NPC, boss)
        {
            Boss = boss;
            Array.Fill(RecentPicks, -1);
            AttackRandom = new Random(boss.NPC.whoAmI + 1);
            // 已迁移：关闭 6 px/f 遗留阀，改走决策点 MarkDecision + 心跳 + 纠偏器。
            UseLegacySpeedValve = false;
        }

        /// <summary>招内计时，占用同步的 <c>ai[3]</c>（与旧代码 <c>Timer</c> 完全同语义：进入招式首帧为 0，由招式体自增）。</summary>
        public ref float Timer => ref SyncTimer;

        #region 热字段承载的招内量（随 SendExtraAI 过线）

        /// <summary>旧 <c>localAI[0]</c>：招内通用记录位（短冲剩余次数、电球样式、锁定角度、链球索引……）。热字段 A。</summary>
        public float Recorder { get; set; }

        /// <summary>旧 <c>localAI[1]</c>：招内第二记录位（长冲次数、绕飞起始角……）。热字段 B。</summary>
        public float Recorder2 { get; set; }

        /// <summary>
        /// 连段步进。旧代码是不同步的 C# 字段，而连段的运动数学、弹幕与视觉全看它，客户端不同步就会跑错子招——必须过线。热字段 C。
        /// </summary>
        public int Combo { get; set; }

        #endregion

        #region 权威端裁决量（不过线）

        /// <summary>上一个顶层状态（不记短冲），防复读用。旧 <c>localAI[2]</c>。</summary>
        internal ZacurrentDragon.AIStates StateRecorder { get; set; }

        /// <summary>连招解锁阈值计数。旧 <c>localAI[3]</c>。</summary>
        internal float UseMoveCount { get; set; }

        /// <summary>已用过的连招集合（同一批连招不复读）。旧 <c>comboRecords</c>。</summary>
        internal HashSet<ZacurrentDragon.AIStates> ComboRecords { get; } = [];

        /// <summary>登场后首次闪电突袭固定 3 次长冲（旧 onSpawnAnmi 行为）。</summary>
        internal bool ForceRecorder2OnNextLightningRaid { get; set; }

        /// <summary><c>CheckDead</c> 登记的死亡请求，由状态基类的 ServerUpdate 消费（命中方客户端也会跑 CheckDead，只能登记）。</summary>
        public bool KillRequested { get; set; }

        /// <summary>紫伏被击穿的请求（<c>ModifyIncomingHit</c> 里登记），由状态基类的 ServerUpdate 切到 Break。</summary>
        public bool BreakRequested { get; set; }

        /// <summary>招式是否完成：两端各自按同一份确定性时间轴算，只由权威端消费（返回下一状态）。</summary>
        public bool AttackFinished { get; set; }

        /// <summary>上一手提交的状态 id（-1 无）。</summary>
        public int LastPickedState { get; set; } = -1;

        /// <summary>
        /// 最近几手的环形记录。本轮只记账不裁决：旧设计的防复读就是“硬锁上一手”（<see cref="StateRecorder"/>），
        /// 窗口查重一旦启用就会改变已上线的出招分布（D10），所以先把账记上，裁决留给后续复议。
        /// </summary>
        public int[] RecentPicks { get; } = new int[ZacurrentDirector.RecentPickWindow];
        private int recentPickCursor;

        /// <summary>过账：写上一手 + 推环形窗口。所有出招都必须经 hub 的 Commit 到这里。</summary>
        public void RecordPick(int stateId)
        {
            LastPickedState = stateId;
            RecentPicks[recentPickCursor] = stateId;
            recentPickCursor = (recentPickCursor + 1) % RecentPicks.Length;
        }

        /// <summary>窗口内某招出现次数。</summary>
        public int RecentCountOf(int stateId)
        {
            int count = 0;
            for (int i = 0; i < RecentPicks.Length; i++)
            {
                if (RecentPicks[i] == stateId)
                {
                    count++;
                }
            }

            return count;
        }

        #endregion

        #region 确定性随机

        /// <summary>由已同步的 <see cref="CoraliteBossContext.AttackSeed"/> 派生；两端同序调用结果一致。</summary>
        public Random AttackRandom { get; private set; }

        /// <summary>
        /// 重建随机源。<b>必须在权威端 <c>RollAttackSeed</c> 之后调用</b>：
        /// 旧代码在 roll 之前刷新，于是权威端用旧种子、客户端用包里的新种子，两端序列不一致（已修）。
        /// </summary>
        public void RefreshAttackRandom() => AttackRandom = CreateAttackRandom();

        /// <summary>招内确定性随机浮点。</summary>
        public float AttackRandFloat(float min, float max) => min + ((max - min) * (float)AttackRandom.NextDouble());

        /// <summary>招内确定性随机正负号。</summary>
        public int AttackRandSign() => AttackRandom.Next(2) == 0 ? -1 : 1;

        #endregion

        #region 声明通道（每帧重声明，BeginFrameDefaults 回默认）

        internal ZacurrentMoveMode MoveMode { get; set; }

        /// <summary>Damp 模式的衰减系数。</summary>
        public float DampFactor { get; set; } = 1f;

        /// <summary>
        /// 本帧无敌。原版 SyncNPC <b>不同步</b> <c>dontTakeDamage</c>，所以凡是切换它的窗口都要做成两端每帧声明的通道。
        /// </summary>
        public bool Invulnerable { get; set; }

        public override void BeginFrameDefaults()
        {
            base.BeginFrameDefaults();

            MoveMode = ZacurrentMoveMode.Keep;
            DampFactor = 1f;
            Invulnerable = false;
        }

        /// <summary>招式体已自行写好本帧速度。</summary>
        public void DeclareDirect() => MoveMode = ZacurrentMoveMode.Direct;

        /// <summary>整体衰减速度。</summary>
        public void DeclareDamp(float factor)
        {
            MoveMode = ZacurrentMoveMode.Damp;
            DampFactor = factor;
        }

        #endregion

        #region 招内量重置（旧 ResetFields）

        /// <summary>
        /// 旧 <c>ZacurrentDragon.ResetFields</c>：清子拍 / 计时 / 记录位与视觉量。<br/>
        /// 连段在两个子招之间两端同调（拍边界是确定性的）；进入状态时只有权威端调（客户端的 ai[2]/ai[3] 由包决定，见 <see cref="ResetVisualFlags"/>）。
        /// </summary>
        public void ResetAttackFields(bool resetCombo = true)
        {
            if (resetCombo)
            {
                Combo = 0;
            }

            SonState = 0;
            Timer = 0;
            Recorder = 0;
            Recorder2 = 0;

            ResetVisualFlags();
        }

        /// <summary>
        /// 只清视觉 / 判定布尔。这些是 C# 字段、原版不同步，而 <c>currentSurrounding</c> 与 <c>IsDashing</c> 参与接触伤害与减伤判定，
        /// 命中方客户端会读到，所以换态时两端都要归位（旧代码只在权威端归位，客户端会留残影与残留判定）。
        /// </summary>
        public void ResetVisualFlags()
        {
            Boss.OpenMouse = false;
            Boss.IsDashing = false;
            Boss.canDrawShadows = false;
            Boss.shadowScale = 1f;
            Boss.shadowAlpha = 1f;
        }

        #endregion

        #region 同步事实

        /// <summary>紫伏布尔 + 紫电计数：血条绘制与阶段裁决都读它们。</summary>
        protected override void WriteFacts(BinaryWriter writer)
        {
            writer.Write(Boss.PurpleVolt);
            writer.Write(Boss.PurpleVoltCount);
        }

        protected override void ReadFacts(BinaryReader reader)
        {
            Boss.PurpleVolt = reader.ReadBoolean();
            Boss.PurpleVoltCount = reader.ReadSingle();
        }

        #endregion
    }
}
