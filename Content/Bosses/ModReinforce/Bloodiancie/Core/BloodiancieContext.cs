using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core
{
    /// <summary>运动声明模式；宿主 <c>ApplyDeclaredMovement</c> 两端同跑地把它翻译成 velocity。</summary>
    public enum BloodiancieMoveMode
    {
        /// <summary>未声明：按 <see cref="BloodiancieDirector.HoldDamp"/> 刹停。正常迁移后不应命中，只是漏声明的兜底。</summary>
        Hold,
        /// <summary>保持帧间速度不变（出生下沉的匀速段、爆冲起手与冲刺段、hub 的过渡帧）。</summary>
        Keep,
        /// <summary>整体衰减 <c>velocity *= DampFactor</c>。</summary>
        Damp,
        /// <summary>悬停上浮：X 衰减、Y 向上加速到上限（旧 <c>SlowDownAndGoUp</c>）。</summary>
        Hover,
        /// <summary>分轴追踪目标（旧 <c>Helper.Movement_SimpleOneLine</c> X / Y），任一轴方向为 0 时该轴改走死区衰减。</summary>
        Chase,
        /// <summary>状态本帧已直接写 velocity（起冲那一帧），宿主不再动它。</summary>
        Direct,
    }

    /// <summary>朝向声明模式。</summary>
    public enum BloodiancieRotationMode
    {
        /// <summary>常规：目标角 = 速度模长 × 0.04 × 面向，AngleTowards 0.01。</summary>
        Normal,
        /// <summary>与 Normal 同目标角，但用 AngleLerp（爆冲刹车段）。</summary>
        LerpToSpeed,
        /// <summary>本帧已由状态直接写 rotation，不再动。</summary>
        Keep,
    }

    /// <summary>招式循环方式（与赤玉灵同源的“基础循环模式”）。</summary>
    public enum BloodiancieCyclingType
    {
        /// <summary>一次近战一次远程。</summary>
        one_one,
        /// <summary>两次近战一次远程。</summary>
        two_one,
        /// <summary>两次近战两次远程。</summary>
        two_two,
    }

    /// <summary>
    /// 赤血玉灵 FSM 上下文：每帧声明总线 + 跨帧事实 + 弹药环绕（Followers）编排。<br/>
    /// 旧代码重占的 ai[1..3]（DamageCount / MoveCyclingType / OwnedFollowersCount）与普通字段 Timer、localAI[1]=MoveCount 全部搬到这里：
    /// <see cref="OwnedFollowersCount"/> 是客户端判定会读到的量（防御加成、弹药环半径、出膛点），经 <see cref="WriteFacts"/> 随 SendExtraAI 过线；
    /// 其余是权威端选招记账，不过线。Timer 由状态热字段承载。ai[1..3] 还给基座约定。
    /// </summary>
    public sealed class BloodiancieContext : CoraliteBossContext
    {
        public Bloodiancie Boss { get; }

        /// <summary>当前目标玩家（<c>Npc.target</c> 原版同步；无效时指向 255 号占位玩家，不会为 null）。</summary>
        public Player Target => Main.player[Npc.target];

        public BloodiancieContext(Bloodiancie boss) : base(boss.NPC, boss)
        {
            Boss = boss;
            Array.Fill(RecentPicks, -1);
            // 已迁移：关闭 6 px/f 遗留阀，改走决策点 MarkDecision + 45 帧心跳 + 纠偏器。
            UseLegacySpeedValve = false;
        }

        #region 事实（跨帧；权威端裁决量只有权威端读写才算数）

        /// <summary>阶段：1 血量不低于一半，2 低于一半。两端由已同步的 life 各自推导（整数除法照旧）。</summary>
        public int Phase { get; private set; } = 1;

        /// <summary>CheckDead 请求死亡演出（权威端写，<see cref="BloodiancieStateBase"/> 的 ServerUpdate 消费）。</summary>
        public bool KillRequested { get; set; }

        /// <summary>累计受伤，每满 <see cref="BloodiancieDirector.FollowerBreakDamage"/> 击碎一发弹药。旧 ai[1]。</summary>
        public float DamageCount { get; set; }

        /// <summary>拥有的弹药数。旧 ai[3]；随 SendExtraAI 过线，客户端据此重建 <see cref="Followers"/> 与防御。</summary>
        public int OwnedFollowersCount { get; set; }

        /// <summary>当前循环方式。旧 ai[2]；只有权威端选招读它。</summary>
        internal BloodiancieCyclingType MoveCyclingType { get; set; }

        /// <summary>本轮循环已出的手数。旧 localAI[1]。</summary>
        public int MoveCount { get; set; }

        /// <summary>进二阶段时固定先放一次召唤的单发闸。旧 <c>Bloodiancie.ExchangeState</c>。</summary>
        public bool ExchangeState { get; set; } = true;

        /// <summary>本轮可枯竭的近战招列表（抽走一项后本轮不再出，重复项即权重）。旧 <c>meleeList</c>。</summary>
        public List<int> MeleeList { get; } = new List<int>();

        /// <summary>本轮可枯竭的远程招列表。旧 <c>shootList</c>。</summary>
        public List<int> ShootList { get; } = new List<int>();

        /// <summary>上一手提交的状态 id（-1 无）。</summary>
        public int LastPickedState { get; set; } = -1;

        /// <summary>最近几手的环形记录（查重记账，本轮只记不裁决）。</summary>
        public int[] RecentPicks { get; } = new int[BloodiancieDirector.RecentPickWindow];
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

        /// <summary>每帧只读事实：阶段、面向、弹药列表对账。旧 AI() 在 Update 之前算 direction/directionY，招式体读到的是本帧新值。</summary>
        public void UpdateFacts()
        {
            Phase = Npc.life < Npc.lifeMax / BloodiancieDirector.Phase2LifeDivisor ? 2 : 1;

            float distanceX = Target.Center.X - Npc.Center.X;
            Npc.direction = distanceX > 0 ? 1 : -1;
            Npc.directionY = Target.Center.Y > Npc.Center.Y ? 1 : -1;

            if (OwnedFollowersCount != Followers.Count)
            {
                RespawnFollowers();
            }
        }

        /// <summary>受击记账：累计伤害每满门槛击碎一发弹药（权威端与命中方客户端都会跑，客户端只是预测，随包纠正）。</summary>
        public void OnHit(int damage)
        {
            DamageCount += damage;
            int threshold = BloodiancieDirector.FollowerBreakDamage();
            while (DamageCount > threshold)
            {
                DamageCount -= threshold;
                DespawnFollowers(1);
            }

            MarkDecision();
        }

        #endregion

        #region 声明通道（每帧重声明，BeginFrameDefaults 回默认）

        internal BloodiancieMoveMode MoveMode { get; set; }
        /// <summary>Damp 模式的衰减系数。</summary>
        public float DampFactor { get; set; }
        /// <summary>Hover 模式：X 衰减 / Y 加速 / Y 上限（负值向上）。</summary>
        public float HoverDampX { get; set; }
        public float HoverAccelY { get; set; }
        public float HoverLimitY { get; set; }
        /// <summary>Chase 模式 X 轴：方向（0 = 落入死区，改按 <see cref="ChaseDeadDampX"/> 衰减）、限速、加速、转向加速、超速衰减。</summary>
        public int ChaseDirX { get; set; }
        public float ChaseSpeedX { get; set; }
        public float ChaseAccelX { get; set; }
        public float ChaseTurnX { get; set; }
        public float ChaseDampX { get; set; }
        public float ChaseDeadDampX { get; set; }
        /// <summary>Chase 模式 Y 轴；<see cref="ChaseY"/> 为 false 时 Y 由状态自管。</summary>
        public bool ChaseY { get; set; }
        public int ChaseDirY { get; set; }
        public float ChaseSpeedY { get; set; }
        public float ChaseAccelY { get; set; }
        public float ChaseTurnY { get; set; }
        public float ChaseDampY { get; set; }
        public float ChaseDeadDampY { get; set; }

        internal BloodiancieRotationMode RotationMode { get; set; }
        /// <summary>Lerp 类朝向模式的插值系数。</summary>
        public float RotationLerp { get; set; }

        /// <summary>本帧无敌（<c>dontTakeDamage</c>）。原版不同步这个标志，必须两端每帧同声明。</summary>
        public bool Invulnerable { get; set; }
        /// <summary>本帧反弹弹幕。</summary>
        public bool ReflectsProjectiles { get; set; }

        public override void BeginFrameDefaults()
        {
            base.BeginFrameDefaults();

            MoveMode = BloodiancieMoveMode.Hold;
            DampFactor = BloodiancieDirector.HoldDamp;
            ChaseY = false;
            ChaseDeadDampX = BloodiancieDirector.ChaseDeadZoneDamp;
            ChaseDeadDampY = BloodiancieDirector.ChaseDeadZoneDamp;
            RotationMode = BloodiancieRotationMode.Normal;
            RotationLerp = 0f;
            Invulnerable = false;
            ReflectsProjectiles = false;
        }

        /// <summary>悬停上浮（旧 SlowDownAndGoUp）。</summary>
        public void DeclareHover(float dampX, float accelY, float limitY)
        {
            MoveMode = BloodiancieMoveMode.Hover;
            HoverDampX = dampX;
            HoverAccelY = accelY;
            HoverLimitY = limitY;
        }

        /// <summary>远程招通用身位：比玩家高不到 150 px 就上浮，已经够高就整体 0.99 衰减。旧 Pulse / MagicShoot / Summon 开头。</summary>
        public void DeclareHoverAboveTarget()
        {
            float yLength = Npc.Center.Y - Target.Center.Y;
            if (yLength > BloodiancieDirector.HoverAboveTargetY)
            {
                DeclareHover(BloodiancieDirector.HoverDampX, BloodiancieDirector.HoverAccelY, BloodiancieDirector.HoverLimitY);
            }
            else
            {
                DeclareDamp(BloodiancieDirector.HoverFarDamp);
            }
        }

        /// <summary>整体衰减。</summary>
        public void DeclareDamp(float factor)
        {
            MoveMode = BloodiancieMoveMode.Damp;
            DampFactor = factor;
        }

        /// <summary>保持速度。</summary>
        public void DeclareKeep() => MoveMode = BloodiancieMoveMode.Keep;

        /// <summary>状态自管速度（本帧已直接写 velocity）。</summary>
        public void DeclareDirect() => MoveMode = BloodiancieMoveMode.Direct;

        /// <summary>分轴追踪 X；<paramref name="dir"/> 为 0 表示落入死区，宿主改按 <paramref name="deadDamp"/> 衰减该轴。</summary>
        public void DeclareChaseX(float speed, float accel, float turnAccel, float damp, int? dir = null, float deadDamp = BloodiancieDirector.ChaseDeadZoneDamp)
        {
            MoveMode = BloodiancieMoveMode.Chase;
            ChaseDirX = dir ?? Npc.direction;
            ChaseSpeedX = speed;
            ChaseAccelX = accel;
            ChaseTurnX = turnAccel;
            ChaseDampX = damp;
            ChaseDeadDampX = deadDamp;
        }

        /// <summary>追踪 Y，带 50 px 死区（死区内 Y 按 0.96 衰减）。需先声明 X；方向取 <c>Npc.directionY</c>。</summary>
        public void DeclareChaseYWithDeadZone(float speed, float accel, float turnAccel, float damp)
        {
            ChaseY = true;
            ChaseDirY = Math.Abs(Target.Center.Y - Npc.Center.Y) > BloodiancieDirector.ChaseDeadZone ? Npc.directionY : 0;
            ChaseSpeedY = speed;
            ChaseAccelY = accel;
            ChaseTurnY = turnAccel;
            ChaseDampY = damp;
            ChaseDeadDampY = BloodiancieDirector.ChaseDeadZoneDamp;
        }

        /// <summary>横向距离超过 <paramref name="threshold"/> 才追 X，否则死区衰减（旧代码大量出现的 <c>xLength &gt; N</c> 分支）。</summary>
        public void DeclareChaseXBeyond(float threshold, float speed, float accel, float turnAccel, float damp)
        {
            int dir = Math.Abs(Target.Center.X - Npc.Center.X) > threshold ? Npc.direction : 0;
            DeclareChaseX(speed, accel, turnAccel, damp, dir);
        }

        /// <summary>
        /// 维持一条横向距离带：远于 <paramref name="far"/> 靠近、近于 <paramref name="near"/> 后退、带内衰减。
        /// 旧 explosionHorizontally 的站位法则（AI.cs:370-376）。
        /// </summary>
        public void DeclareChaseXBand(float near, float far, float speed, float accel, float turnAccel, float damp)
        {
            float xLength = Math.Abs(Target.Center.X - Npc.Center.X);
            int dir = xLength > far ? Npc.direction : (xLength < near ? -Npc.direction : 0);
            DeclareChaseX(speed, accel, turnAccel, damp, dir);
        }

        /// <summary>朝向声明。</summary>
        public void DeclareRotation(BloodiancieRotationMode mode, float lerp = 0f)
        {
            RotationMode = mode;
            RotationLerp = lerp;
        }

        #endregion

        #region 同步事实

        /// <summary>弹药数 ≤ 30，一个字节；客户端据此重建弹药列表、环半径与防御加成。</summary>
        protected override void WriteFacts(BinaryWriter writer)
        {
            writer.Write((byte)Math.Clamp(OwnedFollowersCount, 0, byte.MaxValue));
        }

        protected override void ReadFacts(BinaryReader reader)
        {
            OwnedFollowersCount = reader.ReadByte();
        }

        #endregion

        #region 弹药（Followers）：数量账 + 环绕几何，双端同算

        /// <summary>弹药实体列表（纯视觉 + 出膛坐标）。</summary>
        public List<BloodiancieFollower> Followers { get; } = new List<BloodiancieFollower>();

        /// <summary>获得弹药，超过上限截断；每发 +1 防御。旧 AI.cs:960-987</summary>
        public void SpawnFollowers(int howMany)
        {
            int maxFollowers = BloodiancieDirector.MaxFollowers();
            if (OwnedFollowersCount >= maxFollowers)
            {
                return;
            }

            int count = Math.Min(howMany, maxFollowers - OwnedFollowersCount);
            OwnedFollowersCount += count;
            for (int i = 0; i < count; i++)
            {
                Followers.Add(new BloodiancieFollower(Npc.Center));
            }

            ApplyFollowerDefense();
        }

        /// <summary>按已同步的数量重建弹药列表（客户端收包后数量不一致时）。旧 AI.cs:989-1003</summary>
        public void RespawnFollowers()
        {
            Followers.Clear();

            int maxFollowers = BloodiancieDirector.MaxFollowers();
            if (OwnedFollowersCount >= maxFollowers)
            {
                OwnedFollowersCount = maxFollowers;
            }

            for (int i = 0; i < OwnedFollowersCount; i++)
            {
                Followers.Add(new BloodiancieFollower(Npc.Center));
            }

            ApplyFollowerDefense();
        }

        /// <summary>消耗弹药；返回 false 表示消耗前就没有、或消耗后归零（旧语义：两种情况调用方都收招）。旧 AI.cs:1008-1025</summary>
        public bool DespawnFollowers(int howMany)
        {
            if (OwnedFollowersCount == 0 || Followers.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < howMany; i++)
            {
                OwnedFollowersCount -= 1;
                Followers.RemoveAt(Followers.Count - 1);
            }

            ApplyFollowerDefense();
            return OwnedFollowersCount != 0 && Followers.Count != 0;
        }

        /// <summary>是否还有弹药可消耗。</summary>
        public bool CanDespawnFollower() => OwnedFollowersCount != 0;

        /// <summary>弹药耗尽（数量或列表任一为空）。旧代码多处 <c>OwnedFollowersCount == 0 || followers.Count == 0</c>。</summary>
        public bool FollowersEmpty => OwnedFollowersCount == 0 || Followers.Count == 0;

        private void ApplyFollowerDefense()
        {
            Npc.defense = Npc.defDefense + (OwnedFollowersCount * BloodiancieDirector.DefensePerFollower);
        }

        /// <summary>环面绕 X 轴倾角：随玩家高低俯仰。</summary>
        private float CircleTilt()
            => BloodiancieDirector.CircleTiltBase
               - (Math.Clamp((Target.Center.Y - Npc.Center.Y) / BloodiancieDirector.CircleTiltRange, -1f, 1f) * BloodiancieDirector.CircleTiltMax);

        /// <summary>默认待机环（半径随弹药数增长）。旧 UpdateFollower_Idle</summary>
        public void UpdateFollowersIdle(int timer, float centerLerpSpeed = BloodiancieDirector.IdleLerp)
        {
            float velLength = Npc.velocity.Length();
            float baseRot = (timer * BloodiancieDirector.IdleRotPerFrame) + (velLength * BloodiancieDirector.IdleRotPerSpeed);
            float length = BloodiancieDirector.IdleRadius + (velLength * BloodiancieDirector.IdleRadiusPerSpeed)
                + (OwnedFollowersCount * BloodiancieDirector.IdleRadiusPerFollower);
            float circleRot = CircleTilt();
            for (int i = 0; i < Followers.Count; i++)
            {
                FollowerIdle(Followers[i], i, baseRot, length, circleRot, centerLerpSpeed,
                    BloodiancieDirector.IdleScaleBase, BloodiancieDirector.IdleScaleDepth);
            }
        }

        /// <summary>向上射击：环逐渐张开、缩放深度随半径加深。旧 UpdateFollower_UpShoot</summary>
        public void UpdateFollowersUpShoot(int timer, float centerLerpSpeed = BloodiancieDirector.IdleLerp)
        {
            float baseRot = timer * BloodiancieDirector.UpShootRotPerFrame;
            float length = BloodiancieDirector.IdleRadius + (timer * BloodiancieDirector.UpShootRadiusPerFrame);
            float circleRot = CircleTilt();
            float scaleDepth = BloodiancieDirector.IdleScaleDepth
                + (BloodiancieDirector.UpShootScaleDepthGain * Math.Clamp(length / BloodiancieDirector.UpShootScaleLengthRef, 0f, 1f));
            for (int i = 0; i < Followers.Count; i++)
            {
                FollowerIdle(Followers[i], i, baseRot, length, circleRot, centerLerpSpeed,
                    BloodiancieDirector.UpShootScaleBase, scaleDepth);
            }
        }

        /// <summary>召唤 / 死亡：慢转、略外扩。旧 UpdateFollower_Summon</summary>
        public void UpdateFollowersSummon(int timer)
        {
            float baseRot = timer * BloodiancieDirector.SummonRotPerFrame;
            float length = BloodiancieDirector.IdleRadius
                + Math.Clamp(timer * BloodiancieDirector.SummonRadiusGrow, 0f, BloodiancieDirector.SummonRadiusGrow);
            float circleRot = CircleTilt();
            for (int i = 0; i < Followers.Count; i++)
            {
                FollowerIdle(Followers[i], i, baseRot, length, circleRot, BloodiancieDirector.IdleLerp,
                    BloodiancieDirector.IdleScaleBase, BloodiancieDirector.IdleScaleDepth);
            }
        }

        /// <summary>烟花 / 血雨：平面正圆。旧 UpdateFollower_Firework</summary>
        public void UpdateFollowersFirework(int timer)
        {
            float baseRot = timer * BloodiancieDirector.SummonRotPerFrame;
            float length = BloodiancieDirector.FireworkRadius
                + Math.Clamp(timer * BloodiancieDirector.SummonRadiusGrow, 0f, BloodiancieDirector.SummonRadiusGrow);
            float lerp = BloodiancieDirector.FireworkLerpBase
                + (BloodiancieDirector.FireworkLerpGain * Math.Clamp(timer / BloodiancieDirector.FireworkLerpRamp, 0f, 1f));

            for (int i = 0; i < Followers.Count; i++)
            {
                BloodiancieFollower follower = Followers[i];
                float rot = baseRot + (i / (float)Followers.Count * MathHelper.TwoPi);
                follower.center = Vector2.Lerp(follower.center, Npc.Center + (rot.ToRotationVector2() * length), lerp);
                follower.rotation = follower.rotation.AngleLerp(Npc.rotation, BloodiancieDirector.FireworkRotLerp);
                follower.drawBehind = false;
                follower.scale = 1f;
            }
        }

        /// <summary>瞄准几何：到目标方向、距离因子（0..1）、炮口点。旧 Pulse / MagicShoot 开头。</summary>
        public void AimGeometry(out Vector2 targetDir, out float factor, out Vector2 muzzle)
        {
            Vector2 targetVec = Target.Center - Npc.Center;
            factor = Math.Clamp(targetVec.Length() / BloodiancieDirector.AimFactorRange, 0f, 1f);
            targetDir = targetVec.SafeNormalize(Vector2.One);
            muzzle = Npc.Center + (targetDir * (BloodiancieDirector.AimMuzzleBase + (factor * BloodiancieDirector.AimMuzzleGain)));
        }

        /// <summary>
        /// 激光：末位弹药顶在炮口，其余排成朝向玩家的斜环。旧 UpdateFollower_MagicShoot。
        /// 调用方须先确认 <see cref="FollowersEmpty"/> 为 false。
        /// </summary>
        public void UpdateFollowersMagicShoot(int timer, Vector2 muzzle, Vector2 targetDir, float factor, float centerLerpSpeed = BloodiancieDirector.IdleLerp)
        {
            BloodiancieFollower last = Followers[^1];
            last.center = Vector2.Lerp(last.center, muzzle, BloodiancieDirector.IdleLerp);
            last.rotation = last.rotation.AngleLerp(targetDir.ToRotation() + MathHelper.PiOver2, BloodiancieDirector.FollowerRotLerp);
            last.drawBehind = false;
            last.scale = 1f;

            // 旧式整数除法：timer % 15 / 15 恒为 0 → 后坐量实际恒定，原样保留
            float recoil = timer < BloodiancieDirector.MagicChargeFrames
                ? 0f
                : RecoilCurve(1f - (timer % BloodiancieDirector.MagicRecoilPeriod / BloodiancieDirector.MagicRecoilPeriod));
            float length = BloodiancieDirector.MagicRingRadius + (recoil * BloodiancieDirector.MagicRingRecoil);
            UpdateAimRing(timer * BloodiancieDirector.AimRingRotPerFrame, length, muzzle, targetDir, factor, centerLerpSpeed);
        }

        /// <summary>
        /// 脉冲：末位弹药顶在炮口前 16 px 并放大，其余成斜环；<paramref name="cycleTimer"/> &lt; 0 无后坐。旧 UpdateFollower_Pulse。
        /// 调用方须先确认 <see cref="FollowersEmpty"/> 为 false。
        /// </summary>
        public void UpdateFollowersPulse(int timer, Vector2 muzzle, Vector2 targetDir, float factor, float cycleTimer, float centerLerpSpeed = BloodiancieDirector.IdleLerp)
        {
            BloodiancieFollower last = Followers[^1];
            last.center = Vector2.Lerp(last.center, muzzle + (targetDir * BloodiancieDirector.PulseMuzzleForward), BloodiancieDirector.IdleLerp);
            last.rotation = last.rotation.AngleLerp(targetDir.ToRotation() + MathHelper.PiOver2, BloodiancieDirector.FollowerRotLerp);
            last.drawBehind = false;
            last.scale = BloodiancieDirector.PulseMuzzleScale;

            float recoil = cycleTimer < 0f ? 0f : RecoilCurve(1f - (cycleTimer / BloodiancieDirector.PulseRecoilNorm));
            float length = BloodiancieDirector.PulseRingRadius + (recoil * BloodiancieDirector.PulseRingRecoil);
            UpdateAimRing(timer * BloodiancieDirector.AimRingRotPerFrame, length, muzzle, targetDir, factor, centerLerpSpeed);
        }

        /// <summary>后坐曲线 x·sin(x³)/norm：先猛后缓。</summary>
        private static float RecoilCurve(float timeFactor)
        {
            float x = BloodiancieDirector.RecoilCurveX * timeFactor;
            return x * MathF.Sin(x * x * x) / BloodiancieDirector.RecoilCurveNorm;
        }

        /// <summary>除末位外的弹药排成朝向玩家的斜环（先绕 X 再绕 Y 旋转的圆，透视投影）。旧 FollowerAI_MagicShoot。</summary>
        private void UpdateAimRing(float baseRot, float length, Vector2 center, Vector2 targetDir, float factor, float centerLerpSpeed)
        {
            Matrix xRot = Matrix.CreateRotationX(BloodiancieDirector.AimTiltBase + (targetDir.Y * factor * BloodiancieDirector.AimTiltGain));
            Matrix yRot = Matrix.CreateRotationY(-(BloodiancieDirector.AimTiltBase + (targetDir.X * factor * BloodiancieDirector.AimTiltGain)));
            int ringCount = Followers.Count - 1;
            float totalCount = ringCount == 0 ? 1f : ringCount;

            for (int i = 0; i < ringCount; i++)
            {
                BloodiancieFollower follower = Followers[i];
                float rot = baseRot + (MathHelper.TwoPi * i / totalCount);
                Vector3 vector3D = Vector3.Transform(rot.ToRotationVector2().Vec3(), xRot);
                vector3D = Vector3.Transform(vector3D, yRot);

                float k1 = -BloodiancieDirector.ProjectionDepth / (vector3D.Z - BloodiancieDirector.ProjectionDepth);
                Vector2 circleDir = k1 * new Vector2(vector3D.X, vector3D.Y);
                follower.center = Vector2.Lerp(follower.center,
                    center + (circleDir * length * (follower.lengthOffset + BloodiancieDirector.AimRingLengthOffsetGain)), centerLerpSpeed);
                follower.rotation = follower.rotation.AngleLerp(circleDir.ToRotation() + MathHelper.PiOver2, BloodiancieDirector.AimRingRotLerp);
                follower.drawBehind = vector3D.Z > 0;
                follower.scale = BloodiancieDirector.IdleScaleBase - (vector3D.Z * BloodiancieDirector.AimRingScaleDepth);
            }
        }

        /// <summary>单发弹药的环绕位：XY 平面圆先绕 X 轴倾斜、再随本体 rotation 绕 Z，透视到二维。旧 FollowersAI_Idle / FollowersAI_UpShoot。</summary>
        private void FollowerIdle(BloodiancieFollower follower, int index, float baseRot, float length, float circleRot,
            float centerLerpSpeed, float scaleBase, float scaleDepth)
        {
            float rot = baseRot + (index / (float)Followers.Count * MathHelper.TwoPi);
            Vector3 vector3D = Vector3.Transform(rot.ToRotationVector2().Vec3(), Matrix.CreateRotationX(circleRot));
            vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationZ(Npc.rotation));

            float k1 = -BloodiancieDirector.ProjectionDepth / (vector3D.Z - BloodiancieDirector.ProjectionDepth);
            Vector2 targetDir = k1 * new Vector2(vector3D.X, vector3D.Y);
            Vector2 targetCenter = Npc.Center + (targetDir * length * follower.lengthOffset)
                + new Vector2(0, MathF.Sin(index * BloodiancieDirector.IdleBobFreq) * BloodiancieDirector.IdleBobAmp);
            follower.center = Vector2.Lerp(follower.center, targetCenter, centerLerpSpeed);
            follower.rotation = follower.rotation.AngleLerp(Npc.rotation, BloodiancieDirector.FollowerRotLerp);
            follower.drawBehind = vector3D.Z > 0;
            follower.scale = scaleBase - (vector3D.Z * scaleDepth);
        }

        #endregion
    }
}
