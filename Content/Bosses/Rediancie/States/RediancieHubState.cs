using Coralite.Content.Bosses.Rediancie.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 连接段 + 唯一提交口。<br/>
    /// 旧 <c>ResetState</c> 的选招语义原样搬进 <see cref="Commit"/>：按 <see cref="RediancieCyclingType"/> 循环表决定本手是近战还是远程，
    /// 再按阶段 / 难度从对应池里 <c>Main.rand</c> 一次掷出（池里的重复项就是旧代码的权重）；二阶段首手固定召唤。
    /// 旧代码收招当帧即切下一招、零间隔，所以 <see cref="RediancieDirector.HubFrames"/> = 0：<see cref="EndAttack"/> 直接经 Commit 返回下一招，
    /// 本状态只在 Commit 建不出状态时驻留一帧兜底（绝不锁空）。
    /// </summary>
    [VaultState((int)RediancieStateId.hub, typeof(RediancieContext))]
    internal sealed class RediancieHubState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.hub;

        //==================== 选招池：下标即旧 switch 的掷骰结果，重复项 = 旧权重 ====================

        /// <summary>一阶段大师近战：rand(2) → 蓄力大爆炸 / 三连炸。旧 Rediancie.cs:883-887</summary>
        private static readonly RediancieStateId[] p1MasterMelee = { RediancieStateId.accumulate, RediancieStateId.explosion };
        /// <summary>一阶段大师远程：rand(4) → 0 赤玉雨，其余激光。旧 Rediancie.cs:892-896</summary>
        private static readonly RediancieStateId[] p1MasterShoot = { RediancieStateId.upShoot, RediancieStateId.magicShoot, RediancieStateId.magicShoot, RediancieStateId.magicShoot };
        /// <summary>普通近战只有三连炸。旧 Rediancie.cs:902,948</summary>
        private static readonly RediancieStateId[] normalMelee = { RediancieStateId.explosion };
        /// <summary>一阶段普通远程：rand(3) → 0 赤玉雨，其余激光。旧 Rediancie.cs:907-911</summary>
        private static readonly RediancieStateId[] p1NormalShoot = { RediancieStateId.upShoot, RediancieStateId.magicShoot, RediancieStateId.magicShoot };
        /// <summary>二阶段大师近战：rand(3) → 蓄力 / 三连炸 / 爆冲。旧 Rediancie.cs:926-931</summary>
        private static readonly RediancieStateId[] p2MasterMelee = { RediancieStateId.accumulate, RediancieStateId.explosion, RediancieStateId.dash };
        /// <summary>二阶段大师远程：rand(4) → 赤玉雨 / 烟花 / 脉冲 / 召唤（激光被移除）。旧 Rediancie.cs:936-942</summary>
        private static readonly RediancieStateId[] p2MasterShoot = { RediancieStateId.upShoot, RediancieStateId.firework, RediancieStateId.pulse, RediancieStateId.summon };
        /// <summary>二阶段普通远程：rand(3) → 赤玉雨 / 激光 / 召唤。旧 Rediancie.cs:953-958</summary>
        private static readonly RediancieStateId[] p2NormalShoot = { RediancieStateId.upShoot, RediancieStateId.magicShoot, RediancieStateId.summon };

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            ctx.DeclareKeep();
            ctx.DeclareRotation(RediancieRotationMode.Normal);
            ctx.UpdateFollowersIdle(Timer);
        }

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            if (Timer < RediancieDirector.HubFrames)
            {
                return null;
            }

            return Commit(ctx);
        }

        /// <summary>
        /// 所有招式的收招出口。HubFrames &gt; 0 时进 hub 喘息；为 0 时直接提交下一招不多占一帧。
        /// Commit 建不出状态（注册表缺 id）时退到 hub 驻留一帧再试，绝不让状态机停在收招那一帧。
        /// </summary>
        public static IVaultState<RediancieContext> EndAttack(RediancieContext ctx)
        {
            if (RediancieDirector.HubFrames > 0)
            {
                return Create(RediancieStateId.hub);
            }

            return Commit(ctx) ?? Create(RediancieStateId.hub);
        }

        /// <summary>
        /// 唯一提交口：循环表 → 阶段 / 难度池 → 一次掷骰 → 记账 → 推进循环计数。旧 Rediancie.cs:857-981。
        /// 转阶段首招（召唤）也从这里出；出生动画的固定首招走 <see cref="CommitFixed"/>。
        /// </summary>
        public static IVaultState<RediancieContext> Commit(RediancieContext ctx)
        {
            RediancieDirector.GetCycling(ctx.MoveCyclingType, out int meleeCount, out int shootCount);
            bool useMelee = ctx.MoveCount < meleeCount;
            bool useShoot = ctx.MoveCount < meleeCount + shootCount;
            bool master = Main.masterMode || Main.getGoodWorld;

            RediancieStateId[] pool = null;
            RediancieStateId? fixedPick = null;

            if (ctx.Phase >= 2 && ctx.ExchangeState)
            {
                // 血量低于一半固定先放小弟（单发闸）
                fixedPick = RediancieStateId.summon;
                ctx.ExchangeState = false;
            }
            else if (useMelee)
            {
                pool = master ? (ctx.Phase >= 2 ? p2MasterMelee : p1MasterMelee) : normalMelee;
            }
            else if (useShoot)
            {
                pool = ctx.Phase >= 2
                    ? (master ? p2MasterShoot : p2NormalShoot)
                    : (master ? p1MasterShoot : p1NormalShoot);
            }

            // 一轮全部执行完成就重新随机循环方式（旧代码在选招之后、切换之前做这件事，顺序保持）
            ctx.MoveCount += 1;
            if (ctx.MoveCount >= meleeCount + shootCount)
            {
                ctx.MoveCount = 0;
                ctx.MoveCyclingType = (RediancieCyclingType)Main.rand.Next(RediancieDirector.CyclingTypeCount);
            }

            ctx.Npc.TargetClosest();
            ctx.MarkDecision();

            if (fixedPick.HasValue)
            {
                return Accept(ctx, fixedPick.Value);
            }

            if (pool == null)
            {
                // 旧代码 chosen = -1 时不切换；循环表保证不可达，留作防御
                return null;
            }

            return Accept(ctx, Roll(ctx, pool));
        }

        /// <summary>固定首招（出生动画 → 三连炸）：只记账，不动循环计数。旧 Rediancie.cs:439-445</summary>
        public static IVaultState<RediancieContext> CommitFixed(RediancieContext ctx, RediancieStateId id)
        {
            ctx.Npc.TargetClosest();
            ctx.MarkDecision();
            return Accept(ctx, id);
        }

        /// <summary>
        /// 从池里掷一次。<see cref="RediancieDirector.ForbidImmediateRepeat"/> 打开时与上一手相同就再掷一次避开（池只有一种招时放行，绝不锁空）。
        /// </summary>
        private static RediancieStateId Roll(RediancieContext ctx, RediancieStateId[] pool)
        {
            RediancieStateId pick = pool[Main.rand.Next(pool.Length)];
            if (RediancieDirector.ForbidImmediateRepeat && (int)pick == ctx.LastPickedState && HasOtherMember(pool, pick))
            {
                RediancieStateId retry = pool[Main.rand.Next(pool.Length)];
                if ((int)retry != ctx.LastPickedState)
                {
                    pick = retry;
                }
            }

            return pick;
        }

        private static bool HasOtherMember(RediancieStateId[] pool, RediancieStateId member)
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
        private static IVaultState<RediancieContext> Accept(RediancieContext ctx, RediancieStateId id)
        {
            ctx.RecordPick((int)id);
            return Create(id);
        }
    }
}
