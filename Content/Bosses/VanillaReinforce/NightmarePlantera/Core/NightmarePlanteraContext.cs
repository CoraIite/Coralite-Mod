using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using System;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core
{
    /// <summary>运动声明通道。梦魇之花的招式绝大多数自己写 velocity，所以默认是 <see cref="Keep"/>（不插手）。</summary>
    internal enum NPMoveMode
    {
        /// <summary>不插手，速度由状态自己写死。</summary>
        Keep,
        /// <summary>每帧乘一个阻尼系数。</summary>
        Damp,
        /// <summary>朝锚点做"转向 + 距离决定目标速度"的追击，本 boss 里复用了十几处的同一套公式。</summary>
        Approach,
    }

    /// <summary>朝向声明通道。</summary>
    internal enum NPRotationMode
    {
        /// <summary>不插手。</summary>
        Keep,
        /// <summary>朝玩家转，每帧最多转 <see cref="NightmarePlanteraContext.RotationStep"/>。</summary>
        TowardsTarget,
        /// <summary>朝速度方向转。</summary>
        TowardsVelocity,
        /// <summary>直接锁到 <see cref="NightmarePlanteraContext.RotationTarget"/>。</summary>
        Absolute,
    }

    /// <summary>
    /// 梦魇世纪花的 AI 上下文：同步事实 + 每帧声明总线。<br/>
    /// 同步事实只放"客户端运动数学 / 判定会读、又不属于某一个状态"的量；状态私有量走热字段。
    /// </summary>
    internal class NightmarePlanteraContext : CoraliteBossContext
    {
        public NightmarePlantera Boss { get; }

        public NightmarePlanteraContext(NightmarePlantera boss) : base(boss.NPC, boss)
        {
            Boss = boss;
            // 已迁到新契约：关掉遗留限速阀，改走心跳 + 决策点 + 纠偏器。
            UseLegacySpeedValve = false;
        }

        //==================== 同步事实 ====================

        /// <summary>选招轮转计数，两个 hub 共用。旧 <c>localAI[1]</c>。</summary>
        public ref float MoveCount => ref Npc.localAI[1];

        /// <summary>招式内的"发数 / 轮数"参数，由 hub 选招时定、招式体读。旧本体字段同名。</summary>
        public ref float ShootCount => ref Boss.ShootCount;

        /// <summary>死亡请求：<c>CheckDead</c> 在命中方客户端也会跑，那里只登记，换态走 <c>ServerUpdate</c>。</summary>
        public bool KillRequested { get; set; }

        //==================== 选招记账（只有权威端读写，不过线：客户端不选招） ====================

        /// <summary>查重窗口长度。三阶段轮换表 11 槽，取 1/4~1/3 → 3 手。</summary>
        public const int RecentPickCapacity = 3;

        private readonly int[] recentPicks = new int[RecentPickCapacity];
        private int recentPickCursor;

        /// <summary>上一手选中的状态 id；-1 = 还没出过招。</summary>
        public int LastPickedState { get; private set; } = -1;

        /// <summary>记一手。轮换表返回 / 替补 / 转阶段首招都必须经过 hub 的提交口走到这里。</summary>
        public void RecordPick(int stateId)
        {
            LastPickedState = stateId;
            recentPicks[recentPickCursor] = stateId;
            recentPickCursor = (recentPickCursor + 1) % RecentPickCapacity;
        }

        /// <summary>这一招在最近窗口里出现过几次。</summary>
        public int CountRecentPicks(int stateId)
        {
            int count = 0;
            for (int i = 0; i < RecentPickCapacity; i++)
            {
                if (recentPicks[i] == stateId)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>玩家（可能为空，主控保证战斗中非空）。</summary>
        public Player Target => Npc.target >= 0 && Npc.target < Main.maxPlayers ? Main.player[Npc.target] : null;

        public Vector2 TargetCenter => Target?.Center ?? Npc.Center;

        /// <summary>当前状态所属的宏观阶段，供 BOSS 头像 / 伤害修正 / 外部弹幕判断。</summary>
        public NightmarePlantera.AIPhases MacroPhase => NightmarePlanteraStateBase.MacroPhaseOf((int)Npc.ai[StateAiSlot]);

        //==================== 外部换态请求 ====================

        private int pendingState = -1;

        /// <summary>
        /// 权威端登记一次强制换态（白天狂暴、脱战重置、噩梦值秒杀、转阶段）。<br/>
        /// 状态基类每帧在 <c>ServerUpdate</c> 最前面消费它，所以转移仍然只从 <c>ServerUpdate</c> 返回值出门（D5）。
        /// </summary>
        public void RequestState(NightmarePlanteraStateId id)
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            pendingState = (int)id;
        }

        public bool TryConsumePendingState(out int id)
        {
            id = pendingState;
            pendingState = -1;
            return id >= 0;
        }

        //==================== 声明总线 ====================

        public NPMoveMode MoveMode { get; set; }
        public float DampFactor { get; set; }
        public Vector2 ApproachAnchor { get; set; }
        public float ApproachTurn { get; set; }
        public float ApproachMaxSpeed { get; set; }
        public float ApproachBlend { get; set; }
        public float ApproachSpeedRange { get; set; }

        public NPRotationMode RotationMode { get; set; }
        public float RotationStep { get; set; }
        public float RotationTarget { get; set; }

        /// <summary>本帧无敌。原版 SyncNPC 不同步 <c>dontTakeDamage</c>，所以它必须是每帧两端声明的通道。</summary>
        public bool Invulnerable { get; set; }

        /// <summary>本帧撞人造成接触伤害。</summary>
        public bool MeleeDamage { get; set; }

        /// <summary>本帧只能被幻想之神打中（幻梦狩猎用）。</summary>
        public bool OnlyHitByFantasyGod { get; set; }

        public override void BeginFrameDefaults()
        {
            base.BeginFrameDefaults();

            MoveMode = NPMoveMode.Keep;
            DampFactor = 1f;
            ApproachAnchor = Npc.Center;
            ApproachTurn = 0f;
            ApproachMaxSpeed = 0f;
            ApproachBlend = 0f;
            ApproachSpeedRange = 1f;

            RotationMode = NPRotationMode.Keep;
            RotationStep = 0f;
            RotationTarget = Npc.rotation;

            Invulnerable = false;
            MeleeDamage = false;
            OnlyHitByFantasyGod = false;
        }

        /// <summary>声明"朝锚点追击"：转向速率 turn、距离 speedRange 外吃满 maxSpeed、当前速度与目标速度按 blend 混合。</summary>
        public void DeclareApproach(Vector2 anchor, float turn, float maxSpeed, float blend, float speedRange)
        {
            MoveMode = NPMoveMode.Approach;
            ApproachAnchor = anchor;
            ApproachTurn = turn;
            ApproachMaxSpeed = maxSpeed;
            ApproachBlend = blend;
            ApproachSpeedRange = speedRange <= 0f ? 1f : speedRange;
        }

        public void DeclareDamp(float factor)
        {
            MoveMode = NPMoveMode.Damp;
            DampFactor = factor;
        }

        public void DeclareRotationTowardsTarget(float step)
        {
            RotationMode = NPRotationMode.TowardsTarget;
            RotationStep = step;
        }

        public void DeclareRotationTowardsVelocity(float step)
        {
            RotationMode = NPRotationMode.TowardsVelocity;
            RotationStep = step;
        }

        public void DeclareRotation(float rotation)
        {
            RotationMode = NPRotationMode.Absolute;
            RotationTarget = rotation;
        }

        //==================== 同步事实读写（定长、顺序一致） ====================

        protected override void WriteFacts(BinaryWriter writer)
        {
            // localAI[0] 是尚未拆平的二阶段招式号，拆完第二步后这一格就可以撤掉。
            writer.Write(Npc.localAI[0]);
            writer.Write(MoveCount);
            writer.Write(ShootCount);
            writer.Write(Boss.EXai1);
            writer.Write(Boss.DreamMoveCount);
            writer.Write((byte)MathHelper.Clamp(Boss.fantasyKillCount, 0, 255));

            byte flags = 0;
            if (Boss.haveBeenPhase2) flags |= 1;
            if (Boss.useDreamMove) flags |= 2;
            if (Boss.useFantasyHelp) flags |= 4;
            if (KillRequested) flags |= 8;
            writer.Write(flags);
        }

        protected override void ReadFacts(BinaryReader reader)
        {
            Npc.localAI[0] = reader.ReadSingle();
            MoveCount = reader.ReadSingle();
            ShootCount = reader.ReadSingle();
            Boss.EXai1 = reader.ReadSingle();
            Boss.DreamMoveCount = reader.ReadSingle();
            Boss.fantasyKillCount = reader.ReadByte();

            byte flags = reader.ReadByte();
            Boss.haveBeenPhase2 = (flags & 1) != 0;
            Boss.useDreamMove = (flags & 2) != 0;
            Boss.useFantasyHelp = (flags & 4) != 0;
            KillRequested = (flags & 8) != 0;
        }

        //==================== 招式随机 ====================

        /// <summary>两端共用的确定性随机源，由同步种子 ai[1] 派生。</summary>
        public Random AttackRandom => Boss.AttackRandom;

        /// <summary>旧主控用来"改了同步字段，催一包"的口子，保留给尚未迁移的二阶段代码。</summary>
        public void SyncAttackFields()
        {
            if (!VaultUtils.isClient)
            {
                Npc.netUpdate = true;
            }
        }
    }
}
