using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using Coralite.Content.CoraliteNotes.SlimeChapter1;
using InnoVault.StateMachines;
using Terraria;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 连接段 + 唯一提交口。<br/>
    /// 旧 <c>ResetStates</c> / <c>NormallySetState</c> / <c>FTWSetState</c> / <c>ChallengeSetState</c> 的三张轮换表原样搬进这里：
    /// 表内顺序、分支条件、游标推进与回绕全部照抄，<see cref="Commit"/> 只补上硬锁与记账。<br/>
    /// 旧代码收招当帧即切下一招、零间隔，所以 <see cref="SlimeEmperorDirector.HubFrames"/> = 0：<see cref="EndAttack"/> 直接经 Commit 返回下一招，
    /// 本状态只在 Commit 建不出状态时驻留一帧兜底（绝不锁空）。
    /// </summary>
    [VaultState((int)AIStates.Hub, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorHubState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.Hub;

        //==================== 轮换表的阶段枚举：原样搬自 SlimeEmperor.cs:628-658 ====================

        private enum NormalAIPhases
        {
            MiniJump = 0,
            Shoot1 = 1,
            Melee1 = 2,
            BigJump = 3,
            Shoot2 = 4,
            Melee2 = 5,
        }

        private enum FTWAIPhases
        {
            Shoot1 = 0,
            Melee1 = 1,
            Jump = 2,
            Shoot2 = 3,
            Melee2 = 4,
        }

        private enum ChallengeAIPhases
        {
            Shoot1 = 0,
            Summon,
            BigJump,
            Melee1,
            Jump,
            Poly,
            Shoot2,
            Melee2,
            BigJump2,
        }

        //==================== 掷骰池：下标即旧 switch 的掷骰结果，重复项 = 旧权重 ====================

        /// <summary>普通 / FTW 表 [远程招式1] 大师模式：rand(2) → 凝胶射击 / 凝胶僚机。沿用旧值 SlimeEmperor.cs:693-698,768-773</summary>
        private static readonly AIStates[] shoot1Master = [AIStates.GelShoot, AIStates.GelFlippy];
        /// <summary>FTW 表 [近战招式1]：rand(2) → 王冠冲击 / 泰山压顶。沿用旧值 SlimeEmperor.cs:779-783</summary>
        private static readonly AIStates[] ftwMelee1 = [AIStates.CrownStrike, AIStates.BodySlam];
        /// <summary>FTW 表 [跳跃]：rand(2) → 小跳步 / 大跳。沿用旧值 SlimeEmperor.cs:788-792</summary>
        private static readonly AIStates[] ftwJump = [AIStates.MiniJump, AIStates.BigJump];
        /// <summary>挑战表 [远程招式1]：rand(3) → 凝胶射击 / 黏黏凝胶 / 尖刺凝胶球。沿用旧值 SlimeEmperor.cs:845-850</summary>
        private static readonly AIStates[] challengeShoot1 = [AIStates.GelShoot, AIStates.StickyGel, AIStates.SpikeGelBall];
        /// <summary>挑战表 [召唤]：rand(3) → 凝胶僚机 / 移位分裂 / 分裂。沿用旧值 SlimeEmperor.cs:853-858</summary>
        private static readonly AIStates[] challengeSummon = [AIStates.GelFlippy, AIStates.TransportSplit, AIStates.Split];
        /// <summary>挑战表 [近战招式1]：rand(2) → 泰山压顶 / 王冠冲击。沿用旧值 SlimeEmperor.cs:868-872</summary>
        private static readonly AIStates[] challengeMelee1 = [AIStates.BodySlam, AIStates.CrownStrike];
        /// <summary>挑战表 [跳跃]：rand(3) → 分裂 / 小跳步 / 大跳。沿用旧值 SlimeEmperor.cs:876-881</summary>
        private static readonly AIStates[] challengeJump = [AIStates.Split, AIStates.MiniJump, AIStates.BigJump];

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            if (Timer < SlimeEmperorDirector.HubFrames)
            {
                return null;
            }

            return Commit(ctx);
        }

        /// <summary>
        /// 所有招式的收招出口。HubFrames &gt; 0 时进 hub 喘息；为 0 时直接提交下一招不多占一帧。
        /// Commit 建不出状态（注册表缺 id）时退到 hub 驻留一帧再试，绝不让状态机停在收招那一帧。
        /// </summary>
        public static IVaultState<SlimeEmperorContext> EndAttack(SlimeEmperorContext ctx)
        {
            if (SlimeEmperorDirector.HubFrames > 0)
            {
                return Create(AIStates.Hub);
            }

            return Commit(ctx) ?? Create(AIStates.Hub);
        }

        /// <summary>
        /// 唯一提交口：按难度选轮换表 → 表内取本手 → 推进游标 → 重锁目标 → 记账。旧 SlimeEmperor.cs:664-921。
        /// </summary>
        public static IVaultState<SlimeEmperorContext> Commit(SlimeEmperorContext ctx)
        {
            AIStates pick;
            if (ctx.Dangerous(Slime1Knowledge.Dangerous.SpeedBonus3_1))
            {
                pick = ChallengePick(ctx);
            }
            else if (Main.getGoodWorld)
            {
                pick = FtwPick(ctx);
            }
            else
            {
                pick = NormalPick(ctx);
            }

            ctx.Npc.TargetClosest();
            ctx.MarkDecision();
            return Accept(ctx, pick);
        }

        /// <summary>固定首招（出生态 → 泰山压顶）：只记账，不动轮换游标。沿用旧值 SlimeEmperor.cs:404</summary>
        public static IVaultState<SlimeEmperorContext> CommitFixed(SlimeEmperorContext ctx, AIStates id)
        {
            ctx.Npc.TargetClosest();
            ctx.MarkDecision();
            return Accept(ctx, id);
        }

        //==================== 三张轮换表：表内顺序与游标推进逐字照搬 ====================

        /// <summary>普通轮换表：小跳步 → 远程1 → 近战1 → 大跳 → 远程2 → 近战2。沿用旧值 SlimeEmperor.cs:683-760</summary>
        private static AIStates NormalPick(SlimeEmperorContext ctx)
        {
            bool master = Main.masterMode;
            AIStates state;

            switch (ctx.MovePhase)
            {
                default:
                case (int)NormalAIPhases.MiniJump:
                    state = AIStates.MiniJump;
                    break;

                case (int)NormalAIPhases.Shoot1:
                    state = master ? Roll(ctx, shoot1Master) : AIStates.GelShoot;
                    break;

                case (int)NormalAIPhases.Melee1:
                    state = AIStates.CrownStrike;
                    break;

                case (int)NormalAIPhases.BigJump:
                    state = CanHitTarget(ctx) ? AIStates.BigJump : AIStates.TransportSplit;
                    break;

                case (int)NormalAIPhases.Shoot2:
                    state = master
                        ? ctx.Shoot2State switch
                        {
                            0 => AIStates.SpikeGelBall,
                            1 => AIStates.StickyGel,
                            _ => AIStates.PolymerizeShot
                        }
                        : ctx.Shoot2State switch
                        {
                            0 => AIStates.SpikeGelBall,
                            1 => AIStates.SpikeGelBall,
                            _ => AIStates.PolymerizeShot
                        };

                    ctx.Shoot2State++;
                    if (ctx.Shoot2State > SlimeEmperorDirector.Shoot2CycleCount - 1)
                    {
                        ctx.Shoot2State = 0;
                    }

                    break;

                case (int)NormalAIPhases.Melee2:
                    state = master
                        ? ctx.Melee2State switch
                        {
                            0 => AIStates.Split,
                            1 => AIStates.TransportSplit,
                            _ => AIStates.BodySlam
                        }
                        : ctx.Melee2State switch
                        {
                            0 => AIStates.Split,
                            1 => AIStates.Split,
                            _ => AIStates.BodySlam
                        };

                    ctx.Melee2State++;
                    if (ctx.Melee2State > SlimeEmperorDirector.Melee2CycleCount - 1)
                    {
                        ctx.Melee2State = 0;
                    }

                    break;
            }

            ctx.MovePhase++;
            if (ctx.MovePhase > SlimeEmperorDirector.NormalPhaseCount - 1)
            {
                ctx.MovePhase = 0;
            }

            return state;
        }

        /// <summary>
        /// FTW 轮换表：远程1 → 近战1 → 跳跃 → (落入 default 的远程1) → 远程2。<br/>
        /// 旧表的后两个 case 标号写的是普通表的 4/5，而游标上限是 4，所以阶段 3 落到 default、阶段 5 的近战2 分支不可达——
        /// 这是既有战斗手感的一部分，原样保留。沿用旧值 SlimeEmperor.cs:762-837
        /// </summary>
        private static AIStates FtwPick(SlimeEmperorContext ctx)
        {
            bool master = Main.masterMode;
            AIStates state;

            switch (ctx.MovePhase)
            {
                default:
                case (int)FTWAIPhases.Shoot1:
                    state = master ? Roll(ctx, shoot1Master) : AIStates.GelShoot;
                    break;

                case (int)FTWAIPhases.Melee1:
                    state = Roll(ctx, ftwMelee1);
                    break;

                case (int)FTWAIPhases.Jump:
                    state = CanHitTarget(ctx) ? Roll(ctx, ftwJump) : AIStates.TransportSplit;
                    break;

                case (int)NormalAIPhases.Shoot2:
                    state = master
                        ? ctx.Shoot2State switch
                        {
                            0 => AIStates.SpikeGelBall,
                            1 => AIStates.StickyGel,
                            _ => AIStates.PolymerizeShot
                        }
                        : ctx.Shoot2State switch
                        {
                            0 => AIStates.SpikeGelBall,
                            1 => AIStates.SpikeGelBall,
                            _ => AIStates.PolymerizeShot
                        };

                    ctx.Shoot2State++;
                    if (ctx.Shoot2State > SlimeEmperorDirector.Shoot2CycleCount - 1)
                    {
                        ctx.Shoot2State = 0;
                    }

                    break;

                case (int)NormalAIPhases.Melee2:
                    state = master
                        ? ctx.Melee2State switch
                        {
                            0 => AIStates.TransportSplit,
                            _ => AIStates.Split
                        }
                        : AIStates.Split;

                    ctx.Melee2State++;
                    if (ctx.Melee2State > SlimeEmperorDirector.Melee2CycleCountShort - 1)
                    {
                        ctx.Melee2State = 0;
                    }

                    break;
            }

            ctx.MovePhase++;
            if (ctx.MovePhase > SlimeEmperorDirector.FtwPhaseCount - 1)
            {
                ctx.MovePhase = 0;
            }

            return state;
        }

        /// <summary>危险挑战轮换表（九个阶段位）。沿用旧值 SlimeEmperor.cs:839-921</summary>
        private static AIStates ChallengePick(SlimeEmperorContext ctx)
        {
            AIStates state;

            switch ((ChallengeAIPhases)ctx.MovePhase)
            {
                default:
                case ChallengeAIPhases.Shoot1:
                    state = Roll(ctx, challengeShoot1);
                    break;

                case ChallengeAIPhases.Summon:
                    state = Roll(ctx, challengeSummon);
                    break;

                case ChallengeAIPhases.BigJump:
                case ChallengeAIPhases.BigJump2:
                    state = CanHitTarget(ctx) ? AIStates.BigJump : AIStates.TransportSplit;
                    break;

                case ChallengeAIPhases.Melee1:
                    state = Roll(ctx, challengeMelee1);
                    break;

                case ChallengeAIPhases.Jump:
                    state = CanHitTarget(ctx) ? Roll(ctx, challengeJump) : AIStates.TransportSplit;
                    break;

                case ChallengeAIPhases.Poly:
                    state = AIStates.PolymerizeShot;
                    break;

                case ChallengeAIPhases.Shoot2:
                    state = ctx.Shoot2State switch
                    {
                        0 => AIStates.SpikeGelBall,
                        1 => AIStates.StickyGel,
                        _ => AIStates.GelFlippy
                    };

                    ctx.Shoot2State++;
                    if (ctx.Shoot2State > SlimeEmperorDirector.Shoot2CycleCount - 1)
                    {
                        ctx.Shoot2State = 0;
                    }

                    break;

                case ChallengeAIPhases.Melee2:
                    state = CanHitTarget(ctx)
                        ? ctx.Melee2State switch
                        {
                            0 => AIStates.TransportSplit,
                            _ => AIStates.Split
                        }
                        : AIStates.TransportSplit;

                    ctx.Melee2State++;
                    if (ctx.Melee2State > SlimeEmperorDirector.Melee2CycleCountShort - 1)
                    {
                        ctx.Melee2State = 0;
                    }

                    break;
            }

            ctx.MovePhase++;
            if (ctx.MovePhase > (int)ChallengeAIPhases.BigJump2)
            {
                ctx.MovePhase = 0;
            }

            return state;
        }

        /// <summary>与玩家之间没有实心物块阻挡（决定跳过去还是瞬移过去）。沿用旧值 SlimeEmperor.cs:708</summary>
        private static bool CanHitTarget(SlimeEmperorContext ctx)
            => Collision.CanHitLine(ctx.Npc.Center, 1, 1, ctx.Target.MountedCenter, 1, 1);

        /// <summary>
        /// 从池里掷一次。<see cref="SlimeEmperorDirector.ForbidImmediateRepeat"/> 打开时与上一手相同就再掷一次避开
        /// （池只有一种招时放行，绝不锁空）；本 boss 的轮换表允许同招相邻，默认关闭。
        /// </summary>
        private static AIStates Roll(SlimeEmperorContext ctx, AIStates[] pool)
        {
            AIStates pick = pool[Main.rand.Next(pool.Length)];
            if (SlimeEmperorDirector.ForbidImmediateRepeat && (int)pick == ctx.LastPickedState && HasOtherMember(pool, pick))
            {
                AIStates retry = pool[Main.rand.Next(pool.Length)];
                if ((int)retry != ctx.LastPickedState)
                {
                    pick = retry;
                }
            }

            return pick;
        }

        private static bool HasOtherMember(AIStates[] pool, AIStates member)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i] != member)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>过账：写上一手 + 环形窗口，然后实例化。</summary>
        private static IVaultState<SlimeEmperorContext> Accept(SlimeEmperorContext ctx, AIStates id)
        {
            ctx.RecordPick((int)id);
            return Create(id);
        }
    }
}
