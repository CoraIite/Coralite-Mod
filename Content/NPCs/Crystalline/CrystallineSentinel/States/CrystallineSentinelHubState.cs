using Coralite.Content.NPCs.Crystalline.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 连接段 + 唯一提交口。<br/>
    /// 旧代码的选招语义原样搬进 <see cref="Commit"/>（旧 <c>StartAttack</c>）与 <see cref="TryGuard"/>（旧 <c>TryTurnToGuard</c> 的条件插入件）：
    /// 一阶段按飞弹冷却二选一，二阶段按距离与“复读两次就强制换手”的记账三选一；<br/>
    /// 换状态时的复位、碎岩改道（血线一次性）、休息改道（每 6 手）、随机朝向与机体音全部收在 <see cref="ToState"/> 一处，
    /// 四条出招路径（选招 / 插入件 / 收招 / 转阶段与死亡）都从这里出去。<br/>
    /// 旧代码收招当帧即切下一状态、零间隔（喘息是 P1Idle / P2Idle 的入场预充时间），所以
    /// <see cref="CrystallineSentinelDirector.HubFrames"/> = 0，本状态只在注册表建不出目标时驻留一帧兜底，绝不锁空。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.hub, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelHubState : CrystallineSentinelStateBase
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.hub;

        /// <summary>兜底用的一帧过渡，不跟任何一份常态 AI（免得在二阶段误跑一阶段的转阶段闸）。</summary>
        public override CrystallineSentinelCommon Common => CrystallineSentinelCommon.None;

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            // 过渡帧不动运动，保持上一状态收招时的残速（默认 Keep）。
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (Timer < CrystallineSentinelDirector.HubFrames)
            {
                return null;
            }

            return ToIdle(ctx, 0);
        }

        #region 提交口

        /// <summary>
        /// 唯一选招口。旧 <c>StartAttack</c>（CrystallineSentinel.cs:2005-2057）：<br/>
        /// 一阶段站立 / 闲逛 → 飞弹冷却好了就飞弹（并压冷却），否则刺击；<br/>
        /// 二阶段悬浮 → 距离超过 500 拉近用螺旋冲刺，近身时按 <c>AttackRepeater</c> 防复读在旋风斩 / 挥刀之间强制换手，否则三选一。<br/>
        /// 记账记的是<b>改道前</b>的选择（碎岩与休息是改道、不是新的一手），与旧代码一致。
        /// </summary>
        public static IVaultState<CrystallineSentinelContext> Commit(CrystallineSentinelContext ctx, float distance)
        {
            CrystallineSentinelStateId nextState = CrystallineSentinelStateId.P1Idle;

            switch ((CrystallineSentinelStateId)ctx.CurrentStateId)
            {
                case CrystallineSentinelStateId.P1Idle:
                case CrystallineSentinelStateId.P1Walking:
                    if (ctx.MissileCooldown <= 0)
                    {
                        nextState = CrystallineSentinelStateId.P1Missile;
                        ctx.MissileCooldown = CrystallineSentinelDirector.MissileCooldownMax;
                    }
                    else
                    {
                        nextState = CrystallineSentinelStateId.P1Spurt;
                    }

                    break;
                case CrystallineSentinelStateId.P2Idle:
                    if (distance > CrystallineSentinelDirector.P2RollingPickDistance)
                    {
                        nextState = CrystallineSentinelStateId.P2Rolling;
                    }
                    else if (ctx.AttackRepeater > 0)
                    {
                        // 大于 0 说明至少复读了两次，强制切换另一个攻击
                        nextState = ctx.LastAttack == (int)CrystallineSentinelStateId.P2WhirlSlash
                            ? CrystallineSentinelStateId.P2Swing
                            : CrystallineSentinelStateId.P2WhirlSlash;
                    }
                    else
                    {
                        nextState = Main.rand.NextFromList(CrystallineSentinelStateId.P2WhirlSlash,
                            CrystallineSentinelStateId.P2Swing, CrystallineSentinelStateId.P2Rolling);
                    }

                    ctx.RestCounter++;
                    break;
                default:
                    break;
            }

            IVaultState<CrystallineSentinelContext> next = Accept(ctx, nextState, 0);

            if ((int)nextState == ctx.LastAttack)
            {
                ctx.AttackRepeater++;
            }
            else
            {
                ctx.AttackRepeater = 0;
            }

            ctx.LastAttack = (int)nextState;
            ctx.FaceTarget();

            return next;
        }

        /// <summary>
        /// 条件插入件：远程受伤累计过门槛且开盾不在冷却 → 开盾。旧 <c>TryTurnToGuard</c>（CrystallineSentinel.cs:1951-1959）。<br/>
        /// 不占选招记账（旧代码也不写 <c>lastAttack</c>），但照样经提交口。
        /// </summary>
        public static IVaultState<CrystallineSentinelContext> TryGuard(CrystallineSentinelContext ctx)
        {
            if (ctx.GuardCounter > CrystallineSentinelDirector.GuardCounterThreshold && ctx.GuardCooldown <= 0)
            {
                return Accept(ctx, CrystallineSentinelStateId.P1Guard, 0);
            }

            return null;
        }

        /// <summary>
        /// 索敌 + 选招。旧 <c>TryTurnToAttack</c>（CrystallineSentinel.cs:1961-1991）：<br/>
        /// 一阶段目标跑远且仇恨见底才换目标（换了就把仇恨压到 −1“敌不动我不动”），二阶段只有目标死了才换；<br/>
        /// 最后要求目标有效、没死、可命中（不隐身 / 视线通 / 1000 px 内）才真的出招。
        /// </summary>
        public static IVaultState<CrystallineSentinelContext> TryAttack(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;

            if (!ctx.IsPhase2)
            {
                if (ctx.Target.Distance(npc.Center) > CrystallineSentinelDirector.MeleeRange
                    && ctx.AggroCounter <= CrystallineSentinelDirector.AggroCounterMin)
                {
                    Helper.TargetCloestIgnoreIndex(npc, false, npc.target);
                    if (npc.target != npc.oldTarget)
                    {
                        ctx.AggroCounter = -1;
                        ctx.MarkDecision();
                    }
                }

                // 敌不动，我不动
                if (ctx.AggroCounter < 0)
                {
                    return null;
                }
            }
            else if (!ctx.Target.Alives())
            {
                npc.TargetClosest(false);
                ctx.MarkDecision();
            }

            if (npc.target >= 0 && npc.target < 255 && !Main.player[npc.target].dead && CanHitTarget(ctx, out float distance))
            {
                return Commit(ctx, distance);
            }

            return null;
        }

        /// <summary>收招：回到本阶段的待机态（二阶段照旧经休息闸）。</summary>
        public static IVaultState<CrystallineSentinelContext> ToIdle(CrystallineSentinelContext ctx, int entryFrames)
            => ToState(ctx, ctx.IsPhase2 ? CrystallineSentinelStateId.P2Idle : CrystallineSentinelStateId.P1Idle, entryFrames);

        /// <summary>
        /// 所有换态的唯一出口。旧 <c>SwitchStateP1</c> / <c>SwitchStateP2</c>（CrystallineSentinel.cs:1882-1949）合成一处：<br/>
        /// 复位护盾视觉 → 碎岩改道（血线低于 75% 的第一次飞弹换成碎岩）→ 二阶段的休息改道与手部帧复位 / 一阶段的帧图归零
        /// → 随机朝向 → 写入场预充帧数 → 标决策点。机体音挪到新状态的 <c>OnEnter</c>（客户端被 NetSync 换态时同样会响）。
        /// </summary>
        public static IVaultState<CrystallineSentinelContext> ToState(CrystallineSentinelContext ctx, CrystallineSentinelStateId target,
            int entryFrames = 0, bool randDirection = false)
        {
            ctx.GuardFactor = 0f;

            if (target == CrystallineSentinelStateId.P1Missile
                && ctx.Npc.life < ctx.Npc.lifeMax * CrystallineSentinelDirector.RockReleaseThreshold
                && !ctx.ReleasedRock)
            {
                target = CrystallineSentinelStateId.P1Rock;
                ctx.ReleasedRock = true;
            }

            if (IsPhaseTwoState(target))
            {
                if (ctx.RestCounter >= CrystallineSentinelDirector.RestAttackCount)
                {
                    ctx.RestCounter = 0;
                    target = CrystallineSentinelStateId.P2Rest;
                }

                ctx.HandFrame[0] = CrystallineSentinelDirector.MaxHandFrame;
                ctx.HandFrame[1] = CrystallineSentinelDirector.MaxHandFrame;
                ctx.HandSpurtFrameY = 0;
            }
            else
            {
                SetFrame(ctx, 0, 0);
            }

            if (randDirection)
            {
                ctx.Npc.direction = ctx.Npc.spriteDirection = Main.rand.NextFromList(-1, 1);
            }

            ctx.PendingEntryFrames = entryFrames;
            ctx.MarkDecision();

            return Create(target) ?? Create(CrystallineSentinelStateId.hub);
        }

        /// <summary>过账：写上一手 + 环形窗口，然后经 <see cref="ToState"/> 实例化。</summary>
        private static IVaultState<CrystallineSentinelContext> Accept(CrystallineSentinelContext ctx, CrystallineSentinelStateId id, int entryFrames)
        {
            ctx.RecordPick((int)id);
            return ToState(ctx, id, entryFrames);
        }

        /// <summary>是否二阶段状态（旧 <c>IsPhase2State</c> 的等价区间判定，把新增的 hub 排除在外）。</summary>
        private static bool IsPhaseTwoState(CrystallineSentinelStateId id)
            => id is >= CrystallineSentinelStateId.P2Idle and <= CrystallineSentinelStateId.P2Dying;

        #endregion
    }
}
