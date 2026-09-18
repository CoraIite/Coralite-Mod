using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 二阶段（梦境）的连接段 + <b>唯一选招提交口</b>。<br/>
    /// 二阶段有两套并行的轮换：常规战斗走 12 槽固定表（旧 <c>SetPhase2States</c>，Phase.P2_Dream.cs:2399-2461），
    /// 梦境战斗（<c>useDreamMove</c>，玩家没能保住美梦光时进入）走一条三拍循环（旧 <c>SetPhase2DreamingStates</c>，:2466-2527）。
    /// 两张表的语义原样保留在 <see cref="Commit"/> 与 <see cref="CommitDreaming"/> 里，只把出口收成提交口并加上记账。<br/>
    /// 本状态自身只在提交口建不出状态、或外部（美梦光 / 幻想之神）请求重新选招时被踩到，停一帧再试，绝不空转。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.dream_P2, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamP2State : NightmarePlanteraStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.dream_P2;

        protected override int TimeoutFrames => int.MaxValue;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);
            ctx.Boss.haveBeenPhase2 = true;
        }

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.EnsureRotateTentacles();

            if (!VaultUtils.isServer)
            {
                ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = NightmarePlanteraDirector.SkyTimeleft;
                boss.UpdateFrameNormally();
            }

            boss.NormallySetTentacle();
            ctx.DeclareDamp(NightmarePlanteraDirector.P2BiteRecoverDamp);
            boss.NormallyUpdateTentacle();
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => ctx.Boss.useDreamMove ? CommitDreaming(ctx) : Commit(ctx);

        #region 常规二阶段轮换

        /// <summary>
        /// 常规二阶段的唯一出招口：复位招内槽 + 换种子 + 按 12 槽轮换表取顺位候选 + 记账。<br/>
        /// 顺位 = 首选（表里那一手）→ 等价替补（同类的另一手）→ 无门槛末位（噩梦之咬：任何距离任何站位都能起手）。<br/>
        /// 候选全被查重挡下时放行首选——锁空比复读更糟。<br/>
        /// 两个插入件原样保留：血量跌破 1/5 直接转三阶段；场上有美梦光时强制咬光（这是"引它去打光"的机制本体）。
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

            if (ctx.Npc.life < ctx.Npc.lifeMax * NightmarePlanteraDirector.Phase2SelfExchangeLifeRatio)
            {
                boss.OnExchangeToP3();
                return Create(NightmarePlanteraStateId.exchange_P2_P3);
            }

            if (NightmarePlantera.FantasySparkleAlive(out _))
            {
                return TryTake(ctx, NightmarePlanteraStateId.nightmareBite, checkRepeat: false)
                    ?? Create(NightmarePlanteraStateId.dream_P2);
            }

            int slot = (int)ctx.MoveCount;
            NightmarePlanteraStateId primary = SlotOf(ctx, slot);

            ctx.MoveCount++;
            if (ctx.MoveCount > NightmarePlanteraDirector.P2MoveCycleLength)
            {
                ctx.MoveCount = 0;
            }

            IVaultState<NightmarePlanteraContext> picked =
                TryTake(ctx, primary, checkRepeat: true)
                ?? TryTake(ctx, Substitute(primary), checkRepeat: true)
                ?? TryTake(ctx, NightmarePlanteraStateId.nightmareBite, checkRepeat: true)
                ?? TryTake(ctx, primary, checkRepeat: false);

            // 注册表连一个都建不出来：回连接段停一帧再试，绝不留在原招里。
            return picked ?? Create(NightmarePlanteraStateId.dream_P2);
        }

        /// <summary>
        /// 旧 12 槽轮换表原样：0 转圈咬 / 下方闪光咬二选一；1 随机咬；2 瞬移闪光 / 蝙蝠乌鸦 / 鬼手冲刺三选一；
        /// 3 美梦相助（只出一次，之后退成随机咬）；4 尖刺球 / 蝙蝠乌鸦 / 尖刺与闪光三选一；5 同组里挑一个"不是上一手"的；
        /// 6 随机咬；7 爪击；8 随机咬；9 尖刺地狱；10 梦境之光；11（及以外）待机。
        /// </summary>
        private static NightmarePlanteraStateId SlotOf(NightmarePlanteraContext ctx, int slot) => slot switch
        {
            0 => Move0Picker.Pick(ctx.AttackRandom.Next()).Item,
            1 => RandomBite(ctx),
            2 => Move2Picker.Pick(ctx.AttackRandom.Next()).Item,
            3 => UseFantasyHelp(ctx),
            4 => Move4Picker.Pick(ctx.AttackRandom.Next()).Item,
            5 => SpecialMove2(ctx),
            6 => RandomBite(ctx),
            7 => NightmarePlanteraStateId.hookSlash,
            8 => RandomBite(ctx),
            9 => NightmarePlanteraStateId.spikeHell,
            10 => NightmarePlanteraStateId.dreamSparkle,
            _ => NightmarePlanteraStateId.p2_Idle,
        };

        /// <summary>咬击三选一，权重全 1。与旧 <c>P2BitePicker</c> 一致。</summary>
        private static readonly WeightedRandomPicker<NightmarePlanteraStateId> BitePicker = new(new (NightmarePlanteraStateId, float)[]
        {
            (NightmarePlanteraStateId.nightmareBite, 1f),
            (NightmarePlanteraStateId.fakeBite, 1f),
            (NightmarePlanteraStateId.nightmareDash, 1f),
        });

        private static readonly WeightedRandomPicker<NightmarePlanteraStateId> Move0Picker = new(new (NightmarePlanteraStateId, float)[]
        {
            (NightmarePlanteraStateId.rollingThenBite, 1f),
            (NightmarePlanteraStateId.belowSparkleThenBite, 1f),
        });

        private static readonly WeightedRandomPicker<NightmarePlanteraStateId> Move2Picker = new(new (NightmarePlanteraStateId, float)[]
        {
            (NightmarePlanteraStateId.teleportSparkle, 1f),
            (NightmarePlanteraStateId.batsAndCrows, 1f),
            (NightmarePlanteraStateId.ghostDash, 1f),
        });

        private static readonly WeightedRandomPicker<NightmarePlanteraStateId> Move4Picker = new(new (NightmarePlanteraStateId, float)[]
        {
            (NightmarePlanteraStateId.spikeBalls, 1f),
            (NightmarePlanteraStateId.batsAndCrows, 1f),
            (NightmarePlanteraStateId.spikesAndSparkles, 1f),
        });

        /// <summary>第 5 槽：在压制三件套里挑一个不等于上一手的（旧 <c>SpecialMove2</c> 的可枯竭列表语义）。</summary>
        private static readonly NightmarePlanteraStateId[] SpecialPool =
        {
            NightmarePlanteraStateId.spikeBalls,
            NightmarePlanteraStateId.batsAndCrows,
            NightmarePlanteraStateId.spikesAndSparkles,
        };

        private static NightmarePlanteraStateId RandomBite(NightmarePlanteraContext ctx)
            => BitePicker.Pick(ctx.AttackRandom.Next()).Item;

        private static NightmarePlanteraStateId SpecialMove2(NightmarePlanteraContext ctx)
        {
            List<NightmarePlanteraStateId> list = new(SpecialPool);
            list.Remove((NightmarePlanteraStateId)ctx.LastPickedState);
            return list[ctx.AttackRandom.Next(list.Count)];
        }

        /// <summary>第 3 槽：整场只出一次美梦相助，用完退成随机咬。</summary>
        private static NightmarePlanteraStateId UseFantasyHelp(NightmarePlanteraContext ctx)
        {
            if (!ctx.Boss.useFantasyHelp)
            {
                return RandomBite(ctx);
            }

            ctx.Boss.useFantasyHelp = false;
            return NightmarePlanteraStateId.fantasyHelp;
        }

        /// <summary>等价替补：压力招退成压力招。咬击类互换、压制类互换，都不依赖场地与站位。</summary>
        private static NightmarePlanteraStateId Substitute(NightmarePlanteraStateId primary) => primary switch
        {
            NightmarePlanteraStateId.nightmareBite => NightmarePlanteraStateId.nightmareDash,
            NightmarePlanteraStateId.nightmareDash => NightmarePlanteraStateId.fakeBite,
            NightmarePlanteraStateId.fakeBite => NightmarePlanteraStateId.nightmareBite,
            NightmarePlanteraStateId.rollingThenBite => NightmarePlanteraStateId.belowSparkleThenBite,
            NightmarePlanteraStateId.belowSparkleThenBite => NightmarePlanteraStateId.rollingThenBite,
            NightmarePlanteraStateId.teleportSparkle => NightmarePlanteraStateId.ghostDash,
            NightmarePlanteraStateId.ghostDash => NightmarePlanteraStateId.teleportSparkle,
            NightmarePlanteraStateId.batsAndCrows => NightmarePlanteraStateId.spikesAndSparkles,
            NightmarePlanteraStateId.spikesAndSparkles => NightmarePlanteraStateId.spikeBalls,
            NightmarePlanteraStateId.spikeBalls => NightmarePlanteraStateId.batsAndCrows,
            NightmarePlanteraStateId.spikeHell => NightmarePlanteraStateId.dreamSparkle,
            NightmarePlanteraStateId.dreamSparkle => NightmarePlanteraStateId.spikeHell,
            NightmarePlanteraStateId.fantasyHelp => NightmarePlanteraStateId.nightmareBite,
            _ => NightmarePlanteraStateId.hookSlash,
        };

        /// <summary>试着领走这一手：查重不过就让给下一顺位，建得出来就记账并配好招内参数。</summary>
        private static IVaultState<NightmarePlanteraContext> TryTake(NightmarePlanteraContext ctx, NightmarePlanteraStateId id, bool checkRepeat)
        {
            if (checkRepeat)
            {
                if (NightmarePlanteraDirector.P2ForbidImmediateRepeat && (int)id == ctx.LastPickedState)
                {
                    return null;
                }

                if (ctx.CountRecentPicks((int)id) >= NightmarePlanteraDirector.P2RepeatCap)
                {
                    return null;
                }
            }

            IVaultState<NightmarePlanteraContext> state = Create(id);
            if (state == null)
            {
                return null;
            }

            // 旧选招表只给待机配了时长，其余招式沿用上一手留下的 ShootCount（它们各自会在招内改写）。
            if (id == NightmarePlanteraStateId.p2_Idle)
            {
                ctx.ShootCount = NightmarePlanteraDirector.P2IdleFrames;
            }

            ctx.RecordPick((int)id);
            return state;
        }

        #endregion

        #region 梦境战斗轮换

        /// <summary>
        /// 梦境战斗的出招口（旧 <c>SetPhase2DreamingStates</c>）：三拍循环——咬光 / 钉光 / 猎光，每拍都先放一只新的美梦光。<br/>
        /// 攒够 7 只被杀的美梦光时 <c>KillFantasySparkle</c> 会把梦境战斗关掉，那之后这里挑出的招会以常规形态跑
        /// （旧代码就是这个行为：<c>useDreamMove</c> 已经是 false，分派器落到常规分支）。
        /// </summary>
        internal static IVaultState<NightmarePlanteraContext> CommitDreaming(NightmarePlanteraContext ctx)
        {
            if (VaultUtils.isClient)
            {
                return null;
            }

            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.RollAttackSeedForNextMove();
            ctx.ResetAttackLocals();
            ctx.MoveCount = 0;
            ctx.MarkDecision();

            if (boss.fantasyKillCount > NightmarePlanteraDirector.P2DreamingKillCap)
            {
                boss.KillFantasySparkle();
            }

            if (npc.life < npc.lifeMax * NightmarePlanteraDirector.Phase2SelfExchangeLifeRatio)
            {
                boss.OnExchangeToP3();
                return Create(NightmarePlanteraStateId.exchange_P2_P3);
            }

            // 旧代码这里的 DreamMoveCount 只增不回绕：0~5 是"咬 / 钉"交替的教学段，6 之后一律进猎光。
            NightmarePlanteraStateId id;
            switch ((int)boss.DreamMoveCount)
            {
                case 0:
                case 2:
                case 4:
                    id = NightmarePlanteraStateId.dreamingNightmareBite;
                    ctx.ShootCount = NightmarePlanteraDirector.P2DreamingStalkFrames;
                    break;
                case 1:
                case 3:
                case 5:
                    id = NightmarePlanteraStateId.dreamingSpikeHell;
                    break;
                default:
                    ctx.ShootCount = NightmarePlanteraDirector.P2DreamingStalkFrames;
                    id = NightmarePlanteraStateId.dreamingFantasyHunting;
                    break;
            }

            SpawnSparkle(ctx);
            boss.DreamMoveCount++;

            // KillFantasySparkle 可能刚把梦境战斗关掉；那就照旧走常规形态的同一招。
            if (!boss.useDreamMove)
            {
                id = id switch
                {
                    NightmarePlanteraStateId.dreamingNightmareBite => NightmarePlanteraStateId.nightmareBite,
                    NightmarePlanteraStateId.dreamingSpikeHell => NightmarePlanteraStateId.spikeHell,
                    _ => NightmarePlanteraStateId.dream_P2,
                };
            }

            IVaultState<NightmarePlanteraContext> picked = TryTake(ctx, id, checkRepeat: false);
            return picked ?? Create(NightmarePlanteraStateId.dream_P2);
        }

        /// <summary>每一拍都在玩家侧上方 500 / 300 处放一只新的美梦光，并把它记成当前的追击对象。</summary>
        private static void SpawnSparkle(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 center = ctx.TargetCenter + new Vector2(
                ctx.Boss.NextAttackFromList(-1, 1) * NightmarePlanteraDirector.P2DreamingSparkleSide,
                NightmarePlanteraDirector.P2DreamingSparkleHeight);
            NightmarePlantera.TargetFantasySparkle = npc.NewNpcInAI_Server<FantasySparkle>(center);
        }

        #endregion
    }
}
