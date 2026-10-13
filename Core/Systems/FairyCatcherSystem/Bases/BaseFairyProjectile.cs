using Coralite.Content.DamageClasses;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    /// <summary>
    /// 负责仙灵生命周期、技能轮换和受伤；具体技能逻辑由 <see cref="FairySkill"/> 提供。
    /// </summary>
    public abstract class BaseFairyProjectile : ModProjectile
    {
        private const float MaxOwnerDistance = 4000f;
        private const float MaxTargetOwnerDistance = 1200f;
        private const int ActiveTimeLeft = 2;
        private const int ReturnTimeLeft = 1200;
        private const int BaseRestTime = 150;
        private const int BaseReturnAccelerationTime = 240;
        private const float MovementInertia = 20f;
        private const float MinReturnSpeed = 1f;
        private const float MinReturnDistance = 8f;
        private const int FrameSpacing = 6;
        public Player Owner => Main.player[Projectile.owner];

        /// <summary>
        /// 生成时间，生成仙灵时传入
        /// </summary>
        public ref float SpawnTime => ref Projectile.ai[0];
        /// <summary>
        /// 耐力调整，部分特殊武器会增加或减少耐力
        /// </summary>
        public ref float StaminaAdjust => ref Projectile.ai[1];
        /// <summary>
        /// 当前使用技能的索引
        /// </summary>
        public int UseSkillIndex { get; private set; }
        /// <summary>
        /// 目标敌怪的索引
        /// </summary>
        public int TargetIndex { get; set; } = -1;

        public AIStates State { get; private set; }
        public int Timer { get; set; }

        /// <summary>
        /// 受击后无敌时间
        /// </summary>
        public int ImmuneTimer { get; set; }
        /// <summary>
        /// 剩余耐力；初始根据 <see cref="FairyIV.Stamina"/> 设置，无法承担任何可用技能时返航
        /// </summary>
        public int CurrentStamina { get; set; }

        /// <summary>
        /// 由本地端同步给其他端的速度，无需使用仙灵物品获取速度个体值
        /// </summary>
        public float IVSpeed { get; set; }
        /// <summary>
        /// 由本地端同步给其他端的技能等级，无需使用仙灵物品获取技能等级个体值
        /// </summary>
        protected int IVSkillLevel { get; set; }

        /// <summary>
        /// 在开始攻击时设置，经过玩家饰品等增强过的技能等级
        /// </summary>
        public int SkillLevel { get; set; }
        /// <summary>
        /// X多少帧
        /// </summary>
        protected virtual int FrameX => 1;
        /// <summary>
        /// Y多少帧
        /// </summary>
        public virtual int FrameY => 4;
        /// <summary>
        /// 能够开始攻击的距离
        /// </summary>
        public float AttackDistance = 400;

        private bool _initialized;
        private bool _skillsInitialized;
        private bool _skillStarted;
        private bool _changingState;
        private int _skillStaminaCost;
        protected bool canDamage = true;

        private float MovementSpeed => float.IsFinite(IVSpeed) ? Math.Max(0.1f, IVSpeed) : 0.1f;
        private float TargetSearchDistance => float.IsFinite(AttackDistance) ? Math.Max(0, AttackDistance) : 0;
        private bool HasValidOwner => (uint)Projectile.owner < (uint)Main.maxPlayers
            && Main.player[Projectile.owner] is { active: true, dead: false };
        private bool CanReceiveDamage => Projectile.active && State != AIStates.Backing
            && HasValidOwner && Projectile.IsOwnedByLocalPlayer() && FairyItem != null;

        /// <summary>
        /// 存储所有的仙灵技能
        /// </summary>
        private FairySkill[] _skills = Array.Empty<FairySkill>();

        /// <summary>
        /// 仅本地端可用！用它获取仙灵个体值以及执行仙灵掉血等操作
        /// </summary>
        public BaseFairyItem FairyItem { get; set; }

        private List<FairyBuff> _buffs;
        private IReadOnlyList<FairyBuff> _buffView;
        private Queue<(BuffOperation Operation, FairyBuff Buff)> _pendingBuffOperations;
        private int _buffIterationDepth;

        /// <summary>
        /// 只读的仙灵 Buff 集合；通过 AddBuff、RemoveBuff 和 ClearBuffs 修改结构。
        /// Buff 对象本身（如剩余时间）仍可修改。
        /// </summary>
        public IReadOnlyList<FairyBuff> Buffs
        {
            get
            {
                _buffs ??= new List<FairyBuff>();
                return _buffView ??= _buffs.AsReadOnly();
            }
        }

        public bool HasBuff => _buffs != null && _buffs.Count > 0;

        private enum BuffOperation : byte
        {
            Add,
            Remove,
            Clear,
        }

        public enum AIStates : byte
        {
            /// <summary>
            /// 刚从仙灵捕手中射出的时候
            /// </summary>
            Shooting,
            /// <summary>
            /// 执行自身的特定AI
            /// </summary>
            Skill,
            /// <summary>
            /// 释放技能后的后摇
            /// </summary>
            Rest,
            /// <summary>
            /// 返回自身，此时无敌
            /// </summary>
            Backing,
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;

            Projectile.width = Projectile.height = 16;

            Projectile.penetrate = -1;
        }

        public sealed override void AI()
        {
            if (!Projectile.active)
                return;

            if (!HasValidOwner)
            {
                Projectile.Kill();
                return;
            }

            Initialize();
            if (!Projectile.active)
                return;

            if (Vector2.DistanceSquared(Projectile.Center, Owner.Center) > MaxOwnerDistance * MaxOwnerDistance)
            {
                Projectile.Kill();
                return;
            }

            if (State != AIStates.Backing)
                Projectile.timeLeft = ActiveTimeLeft;

            AIStates previousState = State;
            switch (State)
            {
                case AIStates.Shooting:
                    Shooting();
                    break;
                case AIStates.Skill:
                    Skill();
                    break;
                case AIStates.Rest:
                    Timer++;
                    if (Timer > Math.Max(1f, BaseRestTime - MovementSpeed * 4))
                        RestartAttack();
                    else
                        Rest();
                    break;
                case AIStates.Backing:
                    Backing();
                    break;
                default:
                    Projectile.Kill();
                    return;
            }

            if (!Projectile.active)
                return;

            // 切换状态的这一帧不再执行旧状态的表现。
            if (State == previousState && State != AIStates.Skill)
                SpawnFairyDust(Projectile.Center, Projectile.velocity);

            if (!Projectile.active)
                return;

            UpdateBuffs();
            if (Projectile.active)
                AIAfter();
        }

        /// <summary>
        /// 用于发光等特殊效果
        /// </summary>
        public virtual void AIAfter() { }

        public override void PostAI()
        {
            // tML 在 AI 内 Kill 后仍可能调用本帧的 PostAI。
            if (!Projectile.active || !HasValidOwner)
                return;

            if (State != AIStates.Skill)
                SetDirectionNormally();
            UpdateFrameY(FrameSpacing);

            CheckHostileProjectileHits();
            if (Projectile.active && ImmuneTimer > 0)
                ImmuneTimer--;
        }

        private void CheckHostileProjectileHits()
        {
            if (!CanReceiveDamage || ImmuneTimer > 0)
                return;

            Rectangle hitbox = Projectile.getRect();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (!proj.hostile || !proj.Colliding(proj.getRect(), hitbox))
                    continue;

                int damage = proj.damage;
                foreach (var skill in _skills)
                {
                    skill.ModifyHitByProj(this, proj, ref damage);
                    if (!CanReceiveDamage)
                        return;
                }

                ApplyBuffDamageReduction(ref damage);
                if (!CanReceiveDamage)
                    return;

                FairyItem.HurtByProjectile(this, proj, damage);
                Projectile.netUpdate = true;
                if (Projectile.active && State is AIStates.Rest or AIStates.Shooting)
                    Projectile.velocity += (Projectile.Center - proj.Center).SafeNormalize(Vector2.Zero) * proj.knockBack;

                return;
            }
        }

        /// <summary>
        /// 保留旧拼写供已有调用使用；初始化是幂等的。
        /// </summary>
        public void Initilize() => Initialize();

        public void Initialize()
        {
            if (_initialized || !Projectile.active)
                return;

            _initialized = true;
            if (!EnsureSkillsInitialized())
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.IsOwnedByLocalPlayer() && FairyItem != null)
            {
                float scale = FairyItem.FairyIV.Scale;
                if (!float.IsFinite(scale) || scale <= 0)
                    scale = 1;

                Projectile.Resize(Math.Max(1, (int)(Projectile.width * scale)),
                    Math.Max(1, (int)(Projectile.height * scale)));
                Projectile.scale = scale;
                IVSpeed = FairyItem.FairyIV.Speed;
                IVSpeed = MovementSpeed;
                IVSkillLevel = FairyItem.FairyIV.SkillLevel;
                CurrentStamina = (int)Math.Clamp((long)FairyItem.FairyIV.Stamina + (int)StaminaAdjust, 1, int.MaxValue);
                AttackDistance = Math.Max(0, 325f + IVSkillLevel * 75f);
                Projectile.netUpdate = true;
            }

            OnInitialize();
        }

        // 收到技能数据可能早于首次 AI；只创建技能，不重置目标、耐力或物品数据。
        private bool EnsureSkillsInitialized()
        {
            if (_skillsInitialized)
                return _skills.Length > 0;

            _skillsInitialized = true;
            FairySkill[] skills = InitSkill();
            if (skills != null && skills.Length > 0 && Array.TrueForAll(skills, skill => skill != null))
            {
                _skills = skills;
                return true;
            }

            "仙灵必须有至少一个技能，且技能数组不能包含空项！".Dump();
            return false;
        }

        private bool TryGetCurrentSkill(out FairySkill skill)
        {
            if ((uint)UseSkillIndex < (uint)_skills.Length)
            {
                skill = _skills[UseSkillIndex];
                return true;
            }

            skill = null;
            return false;
        }

        /// <summary>
        /// 初始化仙灵技能
        /// </summary>
        /// <returns></returns>
        public abstract FairySkill[] InitSkill();

        /// <summary>
        /// 帮助方法，获取技能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FairySkill NewSkill<T>() where T : FairySkill
            => ModContent.GetInstance<T>().NewInstance();

        public virtual void OnInitialize() { }

        /// <summary>
        /// 生成粒子，使用<see cref="State"/>判断当前的状态
        /// </summary>
        public virtual void SpawnFairyDust(Vector2 center, Vector2 velocity)
        {

        }

        /// <summary>
        /// 在刚射出时执行，飞行指定时间后开始攻击
        /// </summary>
        public virtual void Shooting()
        {
            Timer++;
            if (Timer > SpawnTime)
            {
                TryExchangeToAttack();
                return;
            }

            float speed = Projectile.velocity.Length();
            if (speed > MovementSpeed)
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Helper.Lerp(speed, MovementSpeed, 0.08f);
        }

        /// <summary>
        /// 执行技能；失去有效目标时先重新索敌，仍无目标则结束技能并进入后摇。
        /// </summary>
        public virtual void Skill()
        {
            if (_changingState || !Projectile.active || State != AIStates.Skill)
                return;

            if (!TryGetCurrentSkill(out FairySkill skill))
            {
                ExchangeToBack();
                return;
            }

            bool hasTarget = TryGetCurrentTarget(out _);
            if (!hasTarget || Timer % Math.Max(1, skill.TargetClosestTime) == 0)
            {
                bool foundTarget = TryFindTarget(out NPC target);
                if (!Projectile.active || State != AIStates.Skill)
                    return;

                if (foundTarget)
                    SetTarget(target.whoAmI);
                else if (!hasTarget)
                {
                    SetTarget(-1);
                    ExchangeToRest();
                    return;
                }
            }

            if (!_skillStarted)
            {
                BeginCurrentSkill(skill);
                if (!Projectile.active || State != AIStates.Skill)
                    return;
            }

            Timer++;
            int previousStamina = CurrentStamina;
            int previousTarget = TargetIndex;
            bool finished = skill.Update(this);
            if (!Projectile.active)
                return;

            // 分段消耗耐力的特殊技能也需要请求同步。
            if (previousStamina != CurrentStamina || previousTarget != TargetIndex)
                Projectile.netUpdate = true;
            if (State == AIStates.Skill && finished)
                ExchangeToRest();
        }

        private void BeginCurrentSkill(FairySkill skill)
        {
            _skillStarted = true;
            Projectile.DamageType = TrueFairyDamage.Instance;
            Projectile.StartAttack();
            if (!Main.dedServ)
                skill.SpawnSkillText(Projectile.Top);
            if (!Projectile.active || State != AIStates.Skill)
                return;

            skill.OnStartAttack(this);
            if (Projectile.active)
                Projectile.netUpdate = true;
        }

        public virtual void Rest()
        {
            Vector2 restSpeed = GetRestSpeed();
            if (!Projectile.active || State != AIStates.Rest)
                return;

            if (TryGetCurrentTarget(out NPC target))
            {
                float attackDistance = TargetSearchDistance;
                if (Vector2.DistanceSquared(target.Center, Projectile.Center) > attackDistance * attackDistance)
                    restSpeed += (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * MovementSpeed;
            }
            else
                SetTarget(-1);

            Projectile.velocity = (Projectile.velocity * MovementInertia + restSpeed) / (MovementInertia + 1);
        }

        /// <summary>
        /// 获取后摇时的原地转圈速度
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetRestSpeed()
        {
            return Vector2.Zero;
        }

        /// <summary>
        /// 返回玩家，根据仙灵的速度个体值，一段时间后逐渐加速
        /// </summary>
        public virtual void Backing()
        {
            float speed = Math.Max(MinReturnSpeed, MovementSpeed);
            Vector2 dir = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            Timer++;

            if (Timer > Math.Max(1f, BaseReturnAccelerationTime - MovementSpeed * 4))
            {
                // 零速度不能通过乘法恢复，至少保留一个正的返航速度。
                speed = Math.Max(MinReturnSpeed, Projectile.velocity.Length()) * 1.01f;
                Projectile.velocity = dir * speed;
            }
            else
            {
                float inertia = Math.Max(1, (int)(MovementInertia - MovementInertia * Timer / BaseReturnAccelerationTime));
                Projectile.velocity = (Projectile.velocity * inertia + dir * speed) / (inertia + 1);
            }

            float arrivalDistance = Math.Max(MinReturnDistance, speed * 2);
            if (Vector2.DistanceSquared(Projectile.Center, Owner.Center) < arrivalDistance * arrivalDistance)
            {
                if (Projectile.IsOwnedByLocalPlayer() && FairyItem != null
                    && Owner.TryGetModPlayer(out FairyCatcherPlayer fcp)
                    && fcp.TryGetFairyBottle(out BaseFairyBottle bottle))
                    bottle.OnFairyBack(Owner, FairyItem);

                if (Projectile.active)
                    Projectile.Kill();
            }
        }

        public void UpdateFrameY(int spacing)
        {
            Projectile.UpdateFrameNormally(Math.Max(1, spacing), Math.Max(1, FrameY) - 1);
        }

        public void SetDirectionNormally()
        {
            // 静止时保留上次朝向，避免 direction/spriteDirection 变成 0。
            if (float.IsFinite(Projectile.velocity.X) && Projectile.velocity.X != 0)
                Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        }

        public override bool? CanDamage()
            => Projectile.active && State != AIStates.Backing && canDamage ? null : false;

        // 部分 GlobalProjectile 会直接允许 CanDamage，因此 NPC 命中入口也要保护。
        public override bool? CanHitNPC(NPC target)
            => Projectile.active && State != AIStates.Backing && canDamage ? null : false;

        #region 状态切换与技能选择

        private void EnterState(AIStates state)
        {
            State = state;
            Timer = 0;
            canDamage = state != AIStates.Backing;
            Projectile.friendly = state != AIStates.Backing;
            Projectile.tileCollide = state == AIStates.Shooting;
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.timeLeft = state == AIStates.Backing ? ReturnTimeLeft : ActiveTimeLeft;
            Projectile.netUpdate = true;
        }

        /// <summary>
        /// 仅结算真正开始过的技能，一次释放最多结算一次。
        /// </summary>
        private void FinishCurrentSkill()
        {
            if (!_skillStarted)
                return;

            _skillStarted = false;
            CurrentStamina = Math.Max(0, Math.Max(0, CurrentStamina) - _skillStaminaCost);
            _skillStaminaCost = 0;
            if (TryGetCurrentSkill(out FairySkill skill))
            {
                skill.OnEndAttack(this);
                UseSkillIndex = (UseSkillIndex + 1) % _skills.Length;
            }
            Projectile.netUpdate = true;
        }

        /// <summary>
        /// 切换至返航：不能攻击 NPC，也不会受到 NPC 或敌对弹幕伤害。
        /// </summary>
        public virtual void ExchangeToBack() => EndSkillAndEnterState(AIStates.Backing);

        /// <summary>
        /// 结束当前技能并进入后摇；没有开始技能的等待不扣耐力，也不轮换索引。
        /// </summary>
        public virtual void ExchangeToRest() => EndSkillAndEnterState(AIStates.Rest);

        private void EndSkillAndEnterState(AIStates state)
        {
            if (_changingState || !Projectile.active || State == state || State == AIStates.Backing)
                return;

            // 结束钩子只负责清理；不允许在结算尚未完成时再次进入或执行技能。
            _changingState = true;
            try
            {
                FinishCurrentSkill();
                if (!Projectile.active)
                    return;

                if (state == AIStates.Backing)
                    SetTarget(-1);
                EnterState(state);
            }
            finally
            {
                _changingState = false;
            }
        }

        private int GetSkillLevel(FairySkill skill)
            => Owner.TryGetModPlayer(out FairyCatcherPlayer fcp)
                ? fcp.GetFairySkillBonus(skill.Type, IVSkillLevel) : IVSkillLevel;

        /// <summary>
        /// 从指定位置环形查找一轮；检查和最终赋值始终使用同一个候选索引。
        /// </summary>
        private bool TrySelectUsableSkill(int startIndex)
        {
            if (_skills.Length == 0)
                return false;

            AIStates expectedState = State;
            int index = (startIndex % _skills.Length + _skills.Length) % _skills.Length;
            for (int checkedCount = 0; checkedCount < _skills.Length; checkedCount++)
            {
                FairySkill skill = _skills[index];
                int level = GetSkillLevel(skill);
                int cost = Math.Max(0, skill.GetStaminaCost(level));
                if (!Projectile.active || State != expectedState)
                    return false;

                bool canUse = skill.CanUseSkill(this);
                if (!Projectile.active || State != expectedState)
                    return false;

                if (canUse && CurrentStamina >= cost)
                {
                    UseSkillIndex = index;
                    SkillLevel = level;
                    _skillStaminaCost = skill.ConsumeStaminaOnFinish ? cost : 0;
                    return true;
                }

                index = (index + 1) % _skills.Length;
            }

            return false;
        }

        private void TryStartAttack(bool waitIfNoTarget)
        {
            if (_changingState || !Projectile.active || !HasValidOwner || State is AIStates.Skill or AIStates.Backing)
                return;

            AIStates expectedState = State;
            bool foundTarget = TryFindTarget(out NPC target);
            if (!Projectile.active || State != expectedState)
                return;

            SetTarget(foundTarget ? target.whoAmI : -1);
            bool selectedSkill = TrySelectUsableSkill(UseSkillIndex);
            if (!Projectile.active || State != expectedState)
                return;

            if (!selectedSkill)
            {
                ExchangeToBack();
                return;
            }

            if (!foundTarget)
            {
                if (waitIfNoTarget)
                    ExchangeToRest();
                else
                    ExchangeToBack();
                return;
            }

            EnterState(AIStates.Skill);
            OnStartUseSkill(target);
        }

        /// <summary>
        /// 首次索敌失败时等待一次，等待本身不消耗耐力。
        /// </summary>
        public virtual void TryExchangeToAttack() => TryStartAttack(waitIfNoTarget: true);

        /// <summary>
        /// 后摇结束后重新选择可负担的技能；没有目标或没有可用技能则返航。
        /// </summary>
        public virtual void RestartAttack() => TryStartAttack(waitIfNoTarget: false);

        /// <summary>
        /// 成功选定技能和目标、进入技能状态后调用。
        /// </summary>
        public virtual void OnStartUseSkill(NPC target) { }

        #endregion

        #region 索敌

        private void SetTarget(int index)
        {
            if (TargetIndex == index)
                return;

            TargetIndex = index;
            Projectile.netUpdate = true;
        }

        private bool IsValidTarget(NPC target)
            => target.active && !target.friendly && target.CanBeChasedBy()
                && Vector2.DistanceSquared(target.Center, Owner.Center) < MaxTargetOwnerDistance * MaxTargetOwnerDistance;

        // 已选定目标可以暂时离开索敌半径，以保留追击与冲刺行为。
        private bool TryGetCurrentTarget(out NPC target)
        {
            if ((uint)TargetIndex < (uint)Main.maxNPCs && IsValidTarget(Main.npc[TargetIndex]))
            {
                target = Main.npc[TargetIndex];
                return true;
            }

            target = null;
            return false;
        }

        protected virtual bool TryFindTarget(out NPC target)
        {
            target = null;
            float distance = TargetSearchDistance;
            float closestDistanceSquared = distance * distance;
            foreach (var npc in Main.ActiveNPCs)
            {
                float distanceSquared = Vector2.DistanceSquared(Projectile.Center, npc.Center);
                if (distanceSquared >= closestDistanceSquared || !IsValidTarget(npc)
                    || !Collision.CanHit(Projectile, npc))
                    continue;

                closestDistanceSquared = distanceSquared;
                target = npc;
            }

            return target != null;
        }

        #endregion

        #region Buff 管理

        /// <summary>
        /// 同类且 IsSame 的 Buff 只刷新为较长的持续时间，不替换实例或叠加强度。
        /// specialAction 立即执行一次；遍历期间的结构修改延迟到最外层遍历结束，
        /// 新增 Buff 不参与当前遍历，删除也从下次遍历起生效。
        /// </summary>
        public void AddBuff<T>(int time, Action<T> specialAction = null) where T : FairyBuff
        {
            if (time <= 0)
                return;

            FairyBuff newBuff = CoraliteContent.GetFairyBuff<T>().NewInstance();
            newBuff.TimeRemain = time;
            specialAction?.Invoke((T)newBuff);
            QueueBuffOperation(BuffOperation.Add, newBuff);
        }

        public void RemoveBuff(FairyBuff buff)
        {
            if (buff != null)
                QueueBuffOperation(BuffOperation.Remove, buff);
        }

        public void ClearBuffs() => QueueBuffOperation(BuffOperation.Clear, null);

        private void QueueBuffOperation(BuffOperation operation, FairyBuff buff)
        {
            _pendingBuffOperations ??= new Queue<(BuffOperation, FairyBuff)>();
            _pendingBuffOperations.Enqueue((operation, buff));
            FlushBuffOperations();
        }

        private void FlushBuffOperations()
        {
            if (_buffIterationDepth != 0 || _pendingBuffOperations == null)
                return;

            // IsSame 等回调也可能重入添加入口；继续排队而不是改动正在遍历的集合。
            _buffIterationDepth++;
            try
            {
                while (_pendingBuffOperations.TryDequeue(out var change))
                {
                    switch (change.Operation)
                    {
                        case BuffOperation.Add:
                            AddOrRefreshBuff(change.Buff);
                            break;
                        case BuffOperation.Remove:
                            if (_buffs != null)
                                for (int i = _buffs.Count - 1; i >= 0; i--)
                                    if (ReferenceEquals(_buffs[i], change.Buff))
                                    {
                                        _buffs.RemoveAt(i);
                                        break;
                                    }
                            break;
                        case BuffOperation.Clear:
                            _buffs?.Clear();
                            break;
                    }
                }
            }
            finally
            {
                _buffIterationDepth--;
            }
        }

        private void AddOrRefreshBuff(FairyBuff newBuff)
        {
            if (newBuff.TimeRemain <= 0)
                return;

            _buffs ??= new List<FairyBuff>();
            for (int i = _buffs.Count - 1; i >= 0; i--)
            {
                FairyBuff buff = _buffs[i];
                if (buff.Type == newBuff.Type && buff.IsSame(newBuff))
                {
                    buff.TimeRemain = Math.Max(newBuff.TimeRemain, buff.TimeRemain);
                    return;
                }
            }

            _buffs.Add(newBuff);
        }

        private void EndBuffIteration()
        {
            _buffIterationDepth--;
            FlushBuffOperations();
        }

        private void UpdateBuffs()
        {
            if (!HasBuff)
                return;

            _buffIterationDepth++;
            try
            {
                for (int i = _buffs.Count - 1; i >= 0; i--)
                {
                    FairyBuff buff = _buffs[i];
                    if (buff.TimeRemain <= 0)
                        continue;

                    buff.UpdateInProj(this);
                    if (!Projectile.active)
                        return;
                    buff.UpdateTimeRemain();
                    if (!Projectile.active)
                        return;
                }
            }
            finally
            {
                // 先提交续时，再清理过期项，避免本轮续时的实例被误删。
                EndBuffIteration();
                if (_buffIterationDepth == 0)
                    _buffs.RemoveAll(buff => buff.TimeRemain <= 0);
            }
        }

        private void ApplyBuffDamageReduction(ref int damage)
        {
            if (!HasBuff)
                return;

            _buffIterationDepth++;
            try
            {
                foreach (var buff in _buffs)
                {
                    buff.DamageReduce(this, ref damage);
                    if (!CanReceiveDamage)
                        return;
                }
            }
            finally
            {
                EndBuffIteration();
            }
        }

        #endregion

        #region 受击部分

        public sealed override void OnKill(int timeLeft)
        {
            if (FairyItem != null && FairyItem.IsDead)
            {
                foreach (var skill in _skills)
                    skill.OnFairyKill(this);

                OnKill_DeadEffect();
            }

            OnKill_OtherEffect(timeLeft);
        }

        public virtual void OnKill_DeadEffect() { }

        public virtual void OnKill_OtherEffect(int timeleft) { }

        /// <summary>
        /// 在仙灵血量减为0后执行，用于生成死亡效果
        /// </summary>
        /// <param name="target"></param>
        public virtual void OnKillByNPC(NPC target) { }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!Projectile.active || State == AIStates.Backing || !HasValidOwner)
                return;

            // 使用默认模式伤害作为仙灵反伤基数，避免额外难度倍率重复放大。
            int damage = ContentSamples.NpcsByNetId.TryGetValue(target.type, out NPC sample)
                ? sample.damage : target.damage;

            if (State == AIStates.Skill && TryGetCurrentSkill(out FairySkill activeSkill))
                activeSkill.ModifyHitNPC_Active(this, target, ref modifiers, ref damage);
            if (!Projectile.active || State == AIStates.Backing)
                return;

            foreach (var skill in _skills)
            {
                skill.ModifyHitNPC_Inactive(this, target, ref modifiers, ref damage);
                if (!Projectile.active || State == AIStates.Backing)
                    return;
            }

            // 保留原有规则：NPC 接触反伤不检查 ImmuneTimer，仅敌对弹幕共享该冷却。
            if (!CanReceiveDamage)
                return;

            ApplyBuffDamageReduction(ref damage);
            if (!CanReceiveDamage)
                return;

            FairyItem.HurtByNPC(this, target, modifiers, damage);
            Projectile.netUpdate = true;
            if (Projectile.active && State is AIStates.Rest or AIStates.Shooting)
                Projectile.velocity += (Projectile.Center - target.Center).SafeNormalize(Vector2.Zero) * 2;
        }

        #endregion

        #region 简单状态同步

        // 公共变量始终成对读写，避免初始化数据被技能状态覆盖；不在这里扩展联机权威模型。
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)State);
            writer.Write(UseSkillIndex);
            writer.Write(TargetIndex);
            writer.Write(Timer);
            writer.Write(CurrentStamina);
            writer.Write(ImmuneTimer);
            writer.Write(IVSpeed);
            writer.Write(IVSkillLevel);
            writer.Write(SkillLevel);
            writer.Write(AttackDistance);
            writer.Write(canDamage);
            writer.Write(Projectile.tileCollide);
            writer.Write(Projectile.scale);
            writer.Write(Projectile.width);
            writer.Write(Projectile.height);
            writer.Write(_skillStaminaCost);

            bool hasSkillState = State == AIStates.Skill && _skillStarted && TryGetCurrentSkill(out _);
            writer.Write(hasSkillState);
            if (hasSkillState)
                _skills[UseSkillIndex].SendState(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AIStates state = (AIStates)reader.ReadByte();
            int skillIndex = reader.ReadInt32();
            int targetIndex = reader.ReadInt32();
            int timer = reader.ReadInt32();
            int stamina = reader.ReadInt32();
            int immuneTimer = reader.ReadInt32();
            float ivSpeed = reader.ReadSingle();
            int ivSkillLevel = reader.ReadInt32();
            int skillLevel = reader.ReadInt32();
            float attackDistance = reader.ReadSingle();
            bool receivedCanDamage = reader.ReadBoolean();
            bool receivedTileCollide = reader.ReadBoolean();
            float scale = reader.ReadSingle();
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            int staminaCost = reader.ReadInt32();
            bool skillStarted = reader.ReadBoolean();

            if (state > AIStates.Backing || !EnsureSkillsInitialized() || (uint)skillIndex >= (uint)_skills.Length
                || (skillStarted && state != AIStates.Skill))
                throw new InvalidDataException("无效的仙灵状态或技能索引。");

            AIStates previousState = State;
            bool sameSkill = State == state && UseSkillIndex == skillIndex;
            bool startSkill = skillStarted && (!_skillStarted || !sameSkill);
            if (_skillStarted && (!skillStarted || !sameSkill) && TryGetCurrentSkill(out FairySkill oldSkill))
            {
                // 网络恢复只清理旧技能，不再次扣费，也不允许清理钩子触发本地状态转换。
                _changingState = true;
                _skillStarted = false;
                try
                {
                    oldSkill.OnEndAttack(this);
                }
                finally
                {
                    _changingState = false;
                }
            }

            State = state;
            UseSkillIndex = skillIndex;
            TargetIndex = (uint)targetIndex < (uint)Main.maxNPCs ? targetIndex : -1;
            Timer = Math.Max(0, timer);
            CurrentStamina = Math.Max(0, stamina);
            ImmuneTimer = Math.Max(0, immuneTimer);
            IVSpeed = ivSpeed;
            IVSkillLevel = ivSkillLevel;
            SkillLevel = skillLevel;
            AttackDistance = attackDistance;
            _skillStaminaCost = Math.Max(0, staminaCost);
            _skillStarted = skillStarted;

            if (skillStarted)
            {
                FairySkill skill = _skills[skillIndex];
                // 建立技能临时字段后再读计时和额外变量，避免初始化覆盖同步结果。
                if (Projectile.active && HasValidOwner && startSkill)
                {
                    Vector2 velocity = Projectile.velocity;
                    float rotation = Projectile.rotation;
                    Projectile.StartAttack();
                    skill.OnStartAttack(this);
                    Projectile.velocity = velocity;
                    Projectile.rotation = rotation;
                }
                skill.ReceiveState(reader);
            }

            if (!Projectile.active)
                return;

            canDamage = State != AIStates.Backing && receivedCanDamage;
            Projectile.friendly = State != AIStates.Backing;
            Projectile.tileCollide = State != AIStates.Backing && receivedTileCollide;
            Projectile.DamageType = State == AIStates.Skill && _skillStarted
                ? TrueFairyDamage.Instance : FairyDamage.Instance;
            Projectile.scale = float.IsFinite(scale) && scale > 0 ? scale : 1;
            Projectile.Resize(Math.Max(1, width), Math.Max(1, height));
            if (State != AIStates.Backing)
                Projectile.timeLeft = ActiveTimeLeft;
            else if (previousState != AIStates.Backing)
                Projectile.timeLeft = ReturnTimeLeft;
        }

        #endregion

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.active && State == AIStates.Skill && TryGetCurrentSkill(out FairySkill skill))
                skill.OnTileCollide(this, oldVelocity);

            return false;
        }

        #region 绘制部分

        public override bool PreDraw(ref Color lightColor)
        {
            if (State == AIStates.Skill && TryGetCurrentSkill(out FairySkill skill))
                skill.PreDrawSpecial(this, ref lightColor);

            float alpha = ImmuneTimer > 0 && ImmuneTimer % 8 < 4 ? 0.4f : 1f;
            _buffIterationDepth++;
                if (HasBuff)
                    foreach (var buff in _buffs)
                        buff.PreDraw(Projectile.Center, Projectile.Size, ref lightColor, alpha);

                DrawSelf(Main.spriteBatch, Main.screenPosition, lightColor * alpha);

                if (HasBuff)
                    foreach (var buff in _buffs)
                        buff.PostDraw(Projectile.Center, Projectile.Size, lightColor, alpha);


            if (State == AIStates.Skill && TryGetCurrentSkill(out skill))
                skill.PostDrawSpecial(this, lightColor);
            if (!Projectile.active)
                return false;

            if (Projectile.IsOwnedByLocalPlayer() && FairyItem != null)
                DrawHealthBar(Projectile.Bottom + new Vector2(0, 12), FairyItem.Life, FairyItem.FairyIV.LifeMax);

            return false;
        }

        /// <summary>
        /// 绘制自己
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="screenPos"></param>
        /// <param name="lightColor"></param>
        public virtual void DrawSelf(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            int frameX = Math.Clamp(FrameX, 1, mainTex.Width);
            int frameY = Math.Clamp(FrameY, 1, mainTex.Height);
            var frame = mainTex.Frame(frameX, frameY, 0, Math.Clamp(Projectile.frame, 0, frameY - 1));

            spriteBatch.Draw(mainTex, Projectile.Center - screenPos, frame, lightColor, Projectile.rotation, frame.Size() / 2,
                Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        public void DrawHealthBar(Vector2 center, float Health, float MaxHealth, float alpha = 0.9f)
        {
            if (!float.IsFinite(Health) || !float.IsFinite(MaxHealth) || Health <= 0 || MaxHealth <= 0)
                return;

            float factor = Math.Clamp(Health / MaxHealth, 0, 1);

            Color backColor = Color.Lerp(Color.DarkRed, Color.DarkGreen, factor);
            Color barColor = Color.Lerp(Color.OrangeRed, Color.LawnGreen, factor);
            float totalBarLength = 12 * 3f;

            Texture2D tex = CoraliteAssets.Sparkle.BarSPA.Value;

            float scale = totalBarLength / tex.Width * Projectile.scale;
            backColor *= alpha;
            barColor *= alpha * factor;

            barColor.A = 0;

            center -= Main.screenPosition;

            //绘制底部条，固定绘制一个横杠
            tex.QuickCenteredDraw(Main.spriteBatch, center, backColor, scale: scale);

            //绘制顶部，裁剪矩形绘制
            Rectangle rect = new Rectangle(0, 0, (int)(factor * tex.Width), tex.Height);
            Main.spriteBatch.Draw(tex, center + new Vector2(-scale * tex.Width / 2, 0), rect, barColor, 0, new Vector2(0, tex.Height / 2), scale, 0, 0);

            //绘制指针
            Texture2D tex2 = CoraliteAssets.Sparkle.ShotLineSPA.Value;

            Vector2 pos = center + new Vector2(-scale * tex.Width / 2 + factor * scale * tex.Width, 0);

            Vector2 scale1 = new Vector2(scale * tex2.Height / tex2.Width * 0.75f, scale) * 0.55f;
            Main.spriteBatch.Draw(tex2, pos, null, backColor
                , MathHelper.PiOver2, tex2.Size() / 2, scale1, 0, 0);
            Main.spriteBatch.Draw(tex2, pos, null, barColor
                , MathHelper.PiOver2, tex2.Size() / 2, scale1, 0, 0);
        }

        #endregion
    }
}
