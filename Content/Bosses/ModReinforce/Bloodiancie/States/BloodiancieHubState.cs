using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using InnoVault.StateMachines;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 连接段 + 唯一提交口。<br/>
    /// 旧 <c>ResetState</c> / <c>RefillAIList</c> 的选招语义原样搬进来：近战、远程两张<b>可枯竭列表</b>（列表里的重复项就是权重），
    /// 按 <see cref="BloodiancieCyclingType"/> 循环表决定本手抽哪张，抽走的一项本轮不再出；一轮走完重新灌表并重掷循环方式；二阶段首手固定召唤。<br/>
    /// 旧代码收招当帧即切下一招、零间隔，所以 <see cref="BloodiancieDirector.HubFrames"/> = 0：<see cref="EndAttack"/> 直接经 <see cref="Commit"/> 返回下一招，
    /// 本状态只在 Commit 建不出状态时驻留一帧兜底（绝不锁空）。
    /// </summary>
    [VaultState((int)BloodiancieStateId.hub, typeof(BloodiancieContext))]
    internal sealed class BloodiancieHubState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.hub;

        //==================== 选招表：重复项 = 旧权重，抽走即本轮不再出 ====================

        /// <summary>一阶段大师近战：横向爆炸 + 两份多段爆炸。旧 AI.cs:859-861</summary>
        private static readonly BloodiancieStateId[] p1MasterMelee =
        {
            BloodiancieStateId.explosionHorizontally, BloodiancieStateId.explosion, BloodiancieStateId.explosion
        };
        /// <summary>一阶段大师远程：炸弹 / 向上射击 / 两份激光。旧 AI.cs:863-866</summary>
        private static readonly BloodiancieStateId[] p1MasterShoot =
        {
            BloodiancieStateId.shootBomb, BloodiancieStateId.upShoot, BloodiancieStateId.magicShoot, BloodiancieStateId.magicShoot
        };
        /// <summary>一阶段普通近战：三份多段爆炸（所以允许两手相连）。旧 AI.cs:870-872</summary>
        private static readonly BloodiancieStateId[] p1NormalMelee =
        {
            BloodiancieStateId.explosion, BloodiancieStateId.explosion, BloodiancieStateId.explosion
        };
        /// <summary>一阶段普通远程：两份向上射击 + 两份激光。旧 AI.cs:874-877</summary>
        private static readonly BloodiancieStateId[] p1NormalShoot =
        {
            BloodiancieStateId.upShoot, BloodiancieStateId.upShoot, BloodiancieStateId.magicShoot, BloodiancieStateId.magicShoot
        };
        /// <summary>二阶段大师近战：横向爆炸 / 多段爆炸 / 爆冲。旧 AI.cs:883-885</summary>
        private static readonly BloodiancieStateId[] p2MasterMelee =
        {
            BloodiancieStateId.explosionHorizontally, BloodiancieStateId.explosion, BloodiancieStateId.dash
        };
        /// <summary>二阶段大师远程：炸弹 / 血雨 / 脉冲 / 烟花 / 召唤（激光被移除）。旧 AI.cs:887-891</summary>
        private static readonly BloodiancieStateId[] p2MasterShoot =
        {
            BloodiancieStateId.shootBomb, BloodiancieStateId.bloodRain, BloodiancieStateId.pulse,
            BloodiancieStateId.firework, BloodiancieStateId.summon
        };
        /// <summary>二阶段普通近战：三份多段爆炸。旧 AI.cs:895-897</summary>
        private static readonly BloodiancieStateId[] p2NormalMelee =
        {
            BloodiancieStateId.explosion, BloodiancieStateId.explosion, BloodiancieStateId.explosion
        };
        /// <summary>二阶段普通远程：炸弹 / 向上射击 / 激光 / 召唤。旧 AI.cs:899-902</summary>
        private static readonly BloodiancieStateId[] p2NormalShoot =
        {
            BloodiancieStateId.shootBomb, BloodiancieStateId.upShoot, BloodiancieStateId.magicShoot, BloodiancieStateId.summon
        };

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            ctx.DeclareKeep();
            ctx.DeclareRotation(BloodiancieRotationMode.Normal);
            ctx.UpdateFollowersIdle(Timer);
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (Timer < BloodiancieDirector.HubFrames)
            {
                return null;
            }

            return Commit(ctx);
        }

        /// <summary>
        /// 所有招式的收招出口。HubFrames &gt; 0 时进 hub 喘息；为 0 时直接提交下一招不多占一帧。
        /// Commit 建不出状态（注册表缺 id）时退到 hub 驻留一帧再试，绝不让状态机停在收招那一帧。
        /// </summary>
        public static IVaultState<BloodiancieContext> EndAttack(BloodiancieContext ctx)
        {
            if (BloodiancieDirector.HubFrames > 0)
            {
                return Create(BloodiancieStateId.hub);
            }

            return Commit(ctx) ?? Create(BloodiancieStateId.hub);
        }

        /// <summary>
        /// 唯一提交口：循环表 → 可枯竭列表抽一项 → 记账 → 推进循环计数（一轮走完重灌表并重掷循环方式）。旧 AI.cs:794-846。
        /// 转阶段首招（召唤）也从这里出；出生动画的固定首招走 <see cref="CommitFixed"/>。
        /// </summary>
        public static IVaultState<BloodiancieContext> Commit(BloodiancieContext ctx)
        {
            BloodiancieDirector.GetCycling(ctx.MoveCyclingType, out int meleeCount, out int shootCount);
            bool useMelee = ctx.MoveCount < meleeCount;

            BloodiancieStateId? pick = null;

            if (ctx.ExchangeState && ctx.Phase >= 2)
            {
                // 血量低于一半固定先放小弟（单发闸）
                pick = BloodiancieStateId.summon;
                ctx.ExchangeState = false;
            }
            else
            {
                pick = Draw(ctx, useMelee ? ctx.MeleeList : ctx.ShootList, useMelee);
            }

            // 一轮全部执行完成就重灌两张表并重新随机循环方式（旧代码在选招之后、切换之前做这件事，顺序保持）
            ctx.MoveCount += 1;
            if (ctx.MoveCount >= meleeCount + shootCount)
            {
                ctx.MoveCount = 0;
                RefillLists(ctx, ctx.Phase);
                ctx.MoveCyclingType = (BloodiancieCyclingType)Main.rand.Next(BloodiancieDirector.CyclingTypeCount);
            }

            ctx.Npc.TargetClosest();
            ctx.MarkDecision();

            if (!pick.HasValue)
            {
                // 表被抽空且重灌也拿不到（理论不可达），不切换留给 hub 兜底
                return null;
            }

            return Accept(ctx, pick.Value);
        }

        /// <summary>固定首招（出生动画 → 多段爆炸）：只记账，不动循环计数。旧 AI.cs:192-199</summary>
        public static IVaultState<BloodiancieContext> CommitFixed(BloodiancieContext ctx, BloodiancieStateId id)
        {
            ctx.Npc.TargetClosest();
            ctx.MarkDecision();
            return Accept(ctx, id);
        }

        /// <summary>
        /// 按阶段 / 难度灌满近战与远程两张可枯竭列表。旧 <c>RefillAIList</c>，AI.cs:848-906。
        /// 出生动画在解除无敌那一帧用 phase = 1 先灌一次。
        /// </summary>
        public static void RefillLists(BloodiancieContext ctx, int phase)
        {
            bool master = Main.masterMode || Main.getGoodWorld;
            Fill(ctx.MeleeList, phase >= 2 ? (master ? p2MasterMelee : p2NormalMelee) : (master ? p1MasterMelee : p1NormalMelee));
            Fill(ctx.ShootList, phase >= 2 ? (master ? p2MasterShoot : p2NormalShoot) : (master ? p1MasterShoot : p1NormalShoot));
        }

        private static void Fill(List<int> list, BloodiancieStateId[] table)
        {
            list.Clear();
            for (int i = 0; i < table.Length; i++)
            {
                list.Add((int)table[i]);
            }
        }

        /// <summary>
        /// 从可枯竭列表里抽走一项。列表空了先按当前阶段重灌再抽（旧代码依赖“一轮用量 ≤ 表长”所以不会空，
        /// 这里补一道防御，免得极端情况下对空列表掷骰抛异常）。
        /// </summary>
        private static BloodiancieStateId? Draw(BloodiancieContext ctx, List<int> list, bool melee)
        {
            if (list.Count == 0)
            {
                RefillLists(ctx, ctx.Phase);
                list = melee ? ctx.MeleeList : ctx.ShootList;
                if (list.Count == 0)
                {
                    return null;
                }
            }

            int index = Main.rand.Next(list.Count);
            int pick = list[index];
            list.RemoveAt(index);
            return (BloodiancieStateId)pick;
        }

        /// <summary>过账：写上一手 + 环形窗口，然后实例化。</summary>
        private static IVaultState<BloodiancieContext> Accept(BloodiancieContext ctx, BloodiancieStateId id)
        {
            ctx.RecordPick((int)id);
            return Create(id);
        }
    }
}
