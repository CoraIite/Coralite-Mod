using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 三阶段（噩梦）的连接段 + <b>唯一选招提交口</b>。<br/>
    /// 旧 <c>SetPhase3States</c>（Phase3_Nightemare.cs:1484-1535）是一张 11 槽固定轮换表，其中三个槽是"随机一种咬击"、
    /// 两个槽是"随机一种撕咬类"；语义原样保留在 <see cref="SlotOf"/> 里，只把出口收成一个 <see cref="Commit"/> 并加上记账。<br/>
    /// 本状态本身只在提交口建不出状态时被踩到（注册表缺 id），停一帧再试，绝不空转。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.nightemare_P3, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareP3State : NightmarePlanteraStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.nightemare_P3;

        protected override int TimeoutFrames => int.MaxValue;

        /// <summary>咬击二选一。权重全 1，与旧 <c>P3BitePicker</c> 一致。</summary>
        private static readonly WeightedRandomPicker<NightmarePlanteraStateId> BitePicker = new(new (NightmarePlanteraStateId, float)[]
        {
            (NightmarePlanteraStateId.p3_nightmareBite, 1f),
            (NightmarePlanteraStateId.p3_nightmareDash, 1f),
        });

        /// <summary>撕咬类二选一。权重全 1，与旧 <c>P3Bite2Picker</c> 一致。</summary>
        private static readonly WeightedRandomPicker<NightmarePlanteraStateId> Bite2Picker = new(new (NightmarePlanteraStateId, float)[]
        {
            (NightmarePlanteraStateId.illusionBite, 1f),
            (NightmarePlanteraStateId.p3_fakeBite, 1f),
        });

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            P3Begin(ctx);
            ctx.DeclareDamp(NightmarePlanteraDirector.P3RecoverDamp);
            P3End(ctx);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => Commit(ctx);

        /// <summary>
        /// 三阶段的唯一出招口：复位招内槽 + 换种子 + 按轮换表取顺位候选 + 记账。<br/>
        /// 顺位 = 首选（表里那一手）→ 等价替补（表里那一类的另一个成员）→ 无门槛末位（噩梦撕咬：任何距离任何高度都能起手）。<br/>
        /// 候选全被查重挡下时放行首选——锁空比复读更糟。
        /// </summary>
        internal static IVaultState<NightmarePlanteraContext> Commit(NightmarePlanteraContext ctx)
        {
            if (VaultUtils.isClient)
            {
                return null;
            }

            NightmarePlantera boss = ctx.Boss;
            boss.RollAttackSeedForNextMove();
            ctx.ResetAttackLocals();
            ctx.MarkDecision();

            int slot = (int)ctx.MoveCount;
            ctx.MoveCount = slot >= NightmarePlanteraDirector.P3MoveCycleLength ? 0 : slot + 1;

            NightmarePlanteraStateId primary = SlotOf(ctx, slot);
            IVaultState<NightmarePlanteraContext> picked =
                TryTake(ctx, primary, checkRepeat: true)
                ?? TryTake(ctx, Substitute(primary), checkRepeat: true)
                ?? TryTake(ctx, NightmarePlanteraStateId.p3_nightmareBite, checkRepeat: true)
                ?? TryTake(ctx, primary, checkRepeat: false);

            // 注册表连一个都建不出来：回连接段停一帧再试，绝不留在原招里。
            return picked ?? Create(NightmarePlanteraStateId.nightemare_P3);
        }

        /// <summary>
        /// 旧轮换表原样：0 幻影撕咬 / 1 瞬移闪光 / 2 随机咬 / 3 随机撕咬 / 4 超级爪击 / 5 随机咬 /
        /// 6 尖刺与闪光 / 7 随机咬 / 8 随机撕咬 / 9 三重尖刺地狱 / 其它 群花乱舞。
        /// </summary>
        private static NightmarePlanteraStateId SlotOf(NightmarePlanteraContext ctx, int slot) => slot switch
        {
            0 => NightmarePlanteraStateId.illusionBite,
            1 => NightmarePlanteraStateId.p3_teleportSparkles,
            2 => RandomBite(ctx),
            3 => RandomBite2(ctx),
            4 => NightmarePlanteraStateId.superHookSlash,
            5 => RandomBite(ctx),
            6 => NightmarePlanteraStateId.p3_spikesAndSparkles,
            7 => RandomBite(ctx),
            8 => RandomBite2(ctx),
            9 => NightmarePlanteraStateId.tripleSpikeHell,
            _ => NightmarePlanteraStateId.flowerDance,
        };

        /// <summary>
        /// 等价替补：压力招退成压力招。咬击类互换、撕咬类互换；纯压制招（转圈弹幕、爪击、尖刺）各自退到
        /// 同类的另一手，都不依赖场地与站位，所以"替补失效"这件事在本 boss 里不存在。
        /// </summary>
        private static NightmarePlanteraStateId Substitute(NightmarePlanteraStateId primary) => primary switch
        {
            NightmarePlanteraStateId.p3_nightmareBite => NightmarePlanteraStateId.p3_nightmareDash,
            NightmarePlanteraStateId.p3_nightmareDash => NightmarePlanteraStateId.p3_nightmareBite,
            NightmarePlanteraStateId.illusionBite => NightmarePlanteraStateId.p3_fakeBite,
            NightmarePlanteraStateId.p3_fakeBite => NightmarePlanteraStateId.illusionBite,
            NightmarePlanteraStateId.p3_teleportSparkles => NightmarePlanteraStateId.p3_spikesAndSparkles,
            NightmarePlanteraStateId.p3_spikesAndSparkles => NightmarePlanteraStateId.p3_teleportSparkles,
            NightmarePlanteraStateId.tripleSpikeHell => NightmarePlanteraStateId.flowerDance,
            NightmarePlanteraStateId.flowerDance => NightmarePlanteraStateId.tripleSpikeHell,
            _ => NightmarePlanteraStateId.superHookSlash,
        };

        private static NightmarePlanteraStateId RandomBite(NightmarePlanteraContext ctx)
            => BitePicker.Pick(ctx.AttackRandom.Next()).Item;

        private static NightmarePlanteraStateId RandomBite2(NightmarePlanteraContext ctx)
            => Bite2Picker.Pick(ctx.AttackRandom.Next()).Item;

        /// <summary>试着领走这一手：查重不过就让给下一顺位，建得出来就记账并配好招内参数。</summary>
        private static IVaultState<NightmarePlanteraContext> TryTake(NightmarePlanteraContext ctx, NightmarePlanteraStateId id, bool checkRepeat)
        {
            if (checkRepeat)
            {
                if (NightmarePlanteraDirector.P3ForbidImmediateRepeat && (int)id == ctx.LastPickedState)
                {
                    return null;
                }

                if (ctx.CountRecentPicks((int)id) >= NightmarePlanteraDirector.P3RepeatCap)
                {
                    return null;
                }
            }

            IVaultState<NightmarePlanteraContext> state = Create(id);
            if (state == null)
            {
                return null;
            }

            // 旧代码在选招时只给幻影撕咬配了发数，其余一律清零。
            ctx.ShootCount = id == NightmarePlanteraStateId.illusionBite
                ? ctx.Boss.PickIllusionBiteShootCount()
                : 0;

            ctx.RecordPick((int)id);
            return state;
        }
    }
}
