using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using InnoVault.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 连接段 + <b>唯一提交口</b>。<br/>
    /// 旧 <c>PickNextState</c> / <c>BuildPurpleVoltPool</c> / <c>BuildNormalPool</c> / <c>RecordCombo</c> / <c>AddCombo</c>
    /// 的全部选招语义原样搬进这里：两张权重池（紫伏 / 普通）、远近与上下区域的加权、防复读硬锁上一手、
    /// 连招解锁阈值 <c>UseMoveCount</c>、连招去重集合，最后 <c>WeightedRandomPicker.Pick(Main.rand.Next())</c> 掷一次。<br/>
    /// 旧代码招式返回 true 的当帧就选下一招、没有间隙，所以 <see cref="ZacurrentDirector.HubFrames"/> = 0：
    /// <see cref="NextAfterAttack"/> 直接经 <see cref="Commit"/> 返回下一招，本状态只在 Commit 建不出状态时驻留一帧兜底（绝不锁空）。<br/>
    /// 连段态也是普通候选项，同样经 <see cref="Commit"/> 出门并记账。
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.hub, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentHubState : ZacurrentStateBase
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.hub;

        protected override void SharedUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            // 只在 Commit 建不出状态时才会停在这里：正常飞一飞，等下一帧重试。
            ZacurrentDragon boss = ctx.Boss;
            boss.UpdateAllOldCaches();
            boss.FlyingFrame();
            boss.SetSpriteDirectionFoTarget();
            boss.TurnToNoRot();
            ctx.DeclareDamp(ZacurrentDirector.IdleDamp);
        }

        protected override IVaultState<ZacurrentDragonContext> AuthorityUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            if (ctx.Timer++ < ZacurrentDirector.HubFrames)
            {
                return null;
            }

            return Commit(ctx);
        }

        /// <summary>
        /// 所有招式的收招出口（基类 <c>ZacurrentStateBase.EndAttack</c> 转发到这里；名字不同是为了不隐藏那个继承下来的静态方法）。
        /// <see cref="ZacurrentDirector.HubFrames"/> &gt; 0 时进 hub 喘息；为 0 时直接提交下一招不多占一帧。
        /// Commit 建不出状态（注册表缺 id）时退到 hub 驻留一帧再试，绝不让状态机停在收招那一帧。
        /// </summary>
        public static IVaultState<ZacurrentDragonContext> NextAfterAttack(ZacurrentDragonContext ctx)
        {
            if (ZacurrentDirector.HubFrames > 0)
            {
                return Create(ZacurrentDragon.AIStates.hub);
            }

            return Commit(ctx) ?? Create(ZacurrentDragon.AIStates.hub);
        }

        /// <summary>
        /// 唯一提交口：记上一手 → 紫伏满则强制形态切换 → 建池 → 去掉上一手 → 掷一次 → 连招记账 → 过账。
        /// 旧 ZcurrentAI.cs:387-416。仅权威端裁决，客户端结构上没有入口。
        /// </summary>
        public static IVaultState<ZacurrentDragonContext> Commit(ZacurrentDragonContext ctx)
        {
            if (VaultUtils.isClient)
            {
                return null;
            }

            ZacurrentDragon boss = ctx.Boss;
            ZacurrentDragon.AIStates current = boss.State;

            //记录旧状态，不包括短冲（防复读）
            if (current is not ZacurrentDragon.AIStates.SmallDash and not ZacurrentDragon.AIStates.SmallDashVolt)
            {
                ctx.StateRecorder = current;
            }

            ctx.MarkDecision();

            //紫电攒满：进入紫伏形态切换
            if (!boss.PurpleVolt && boss.PurpleVoltCount == boss.GetPurpleVoltMax())
            {
                ctx.ComboRecords.Clear();
                return Accept(ctx, ZacurrentDragon.AIStates.PurpleVoltExchange);
            }

            List<(ZacurrentDragon.AIStates, float)> entries = [];
            if (boss.PurpleVolt)
            {
                BuildPurpleVoltPool(ctx, entries);
            }
            else
            {
                BuildNormalPool(ctx, entries);
            }

            //防复读：移除上一个状态（保证仍有候选项 —— 候选全违规时放行，绝不锁空）
            if (entries.Any(p => p.Item1 != ctx.StateRecorder))
            {
                entries.RemoveAll(p => p.Item1 == ctx.StateRecorder);
            }

            ZacurrentDragon.AIStates pick = new WeightedRandomPicker<ZacurrentDragon.AIStates>(entries).Pick(Main.rand.Next()).Item;
            RecordCombo(ctx, pick);
            return Accept(ctx, pick);
        }

        /// <summary>固定首招（登场演出 → 闪电突袭）：只过账，不掷骰。旧 ZacurrentDragon.States.cs:102</summary>
        public static IVaultState<ZacurrentDragonContext> CommitFixed(ZacurrentDragonContext ctx, ZacurrentDragon.AIStates id)
        {
            ctx.MarkDecision();
            return Accept(ctx, id);
        }

        /// <summary>紫伏状态的招池。旧 ZcurrentAI.cs:418-450</summary>
        private static void BuildPurpleVoltPool(ZacurrentDragonContext ctx, List<(ZacurrentDragon.AIStates, float)> entries)
        {
            entries.Add((ZacurrentDragon.AIStates.ElectricBreathSmall, ZacurrentDirector.WeightBreathSmallVolt));//小吐息概率降低

            entries.Add((ZacurrentDragon.AIStates.ElectricBreathMiddle,
                IsUpOrDown(ctx) ? ZacurrentDirector.WeightBreathMiddleUpDown : ZacurrentDirector.WeightBreathMiddleNormal));

            entries.Add((ZacurrentDragon.AIStates.ElectricBall, ZacurrentDirector.WeightElectricBallVolt));//普通电球概率降低
            entries.Add((ZacurrentDragon.AIStates.PointerBall, ZacurrentDirector.WeightPointerBall));

            float farawayPercent = FarAwayWeight(ctx);
            entries.Add((ZacurrentDragon.AIStates.DashDischarging, farawayPercent));
            entries.Add((ZacurrentDragon.AIStates.LightningRaidVolt, farawayPercent));
            //防止复读短冲
            if (ctx.Boss.State != ZacurrentDragon.AIStates.SmallDashVolt)
            {
                entries.Add((ZacurrentDragon.AIStates.SmallDashVolt, farawayPercent + ZacurrentDirector.SmallDashExtraWeightVolt));
            }

            ctx.UseMoveCount++;
            if (ctx.UseMoveCount > ZacurrentDirector.ComboUnlockMoves())
            {
                if (ctx.ComboRecords.Count > ZacurrentDirector.VoltBigComboRequireRecords)
                {
                    AddCombo(ctx, entries, ZacurrentDragon.AIStates.VoltBigCombo);
                }

                AddCombo(ctx, entries, ZacurrentDragon.AIStates.VoltChainCombo);
                AddCombo(ctx, entries, ZacurrentDragon.AIStates.VoltZBallChainCombo);
            }
        }

        /// <summary>普通状态的招池。旧 ZcurrentAI.cs:452-482</summary>
        private static void BuildNormalPool(ZacurrentDragonContext ctx, List<(ZacurrentDragon.AIStates, float)> entries)
        {
            entries.Add((ZacurrentDragon.AIStates.ElectricBreathSmall, ZacurrentDirector.WeightBreathSmallNormal));

            entries.Add((ZacurrentDragon.AIStates.ElectricBreathMiddle,
                IsUpOrDown(ctx) ? ZacurrentDirector.WeightBreathMiddleUpDown : ZacurrentDirector.WeightBreathMiddleNormal));

            entries.Add((ZacurrentDragon.AIStates.ElectricBall, ZacurrentDirector.WeightElectricBallNormal));

            float farawayPercent = FarAwayWeight(ctx);
            entries.Add((ZacurrentDragon.AIStates.DashDischarging, farawayPercent));
            entries.Add((ZacurrentDragon.AIStates.LightningRaidNormal, farawayPercent));
            //防止复读短冲
            if (ctx.Boss.State != ZacurrentDragon.AIStates.SmallDash)
            {
                entries.Add((ZacurrentDragon.AIStates.SmallDash, farawayPercent + ZacurrentDirector.SmallDashExtraWeightNormal));
            }

            ctx.UseMoveCount++;
            if (ctx.UseMoveCount > ZacurrentDirector.ComboUnlockMoves())
            {
                AddCombo(ctx, entries, ZacurrentDragon.AIStates.NormalRoarCombo1);
                AddCombo(ctx, entries, ZacurrentDragon.AIStates.NormalRoarCombo2);
                AddCombo(ctx, entries, ZacurrentDragon.AIStates.NormalChainCombo);
                AddCombo(ctx, entries, ZacurrentDragon.AIStates.NormalPointerCombo);
            }
        }

        /// <summary>玩家在正上 / 正下方（吐息横扫打不到的死角）。旧 ZcurrentAI.cs:423-425</summary>
        private static bool IsUpOrDown(ZacurrentDragonContext ctx)
            => MathF.Abs(ctx.Target.Center.X - ctx.Npc.Center.X) < ZacurrentDirector.UpDownCheckX
               && MathF.Abs(ctx.Target.Center.Y - ctx.Npc.Center.Y) > ZacurrentDirector.UpDownCheckY;

        /// <summary>距离远时提升位移招权重。旧 ZcurrentAI.cs:432-435</summary>
        private static float FarAwayWeight(ZacurrentDragonContext ctx)
        {
            float distance = ctx.Npc.Distance(ctx.Target.Center);
            return distance > ZacurrentDirector.FarAwayDistance
                ? ZacurrentDirector.FarAwayWeightBase + ((distance - ZacurrentDirector.FarAwayDistance) / ZacurrentDirector.FarAwayWeightDiv)
                : 1f;
        }

        /// <summary>选到连招就记下它并把解锁计数清零。旧 ZcurrentAI.cs:484-492</summary>
        private static void RecordCombo(ZacurrentDragonContext ctx, ZacurrentDragon.AIStates pick)
        {
            if (pick is ZacurrentDragon.AIStates.NormalChainCombo or ZacurrentDragon.AIStates.NormalRoarCombo1
                or ZacurrentDragon.AIStates.NormalRoarCombo2 or ZacurrentDragon.AIStates.NormalPointerCombo
                or ZacurrentDragon.AIStates.VoltBigCombo or ZacurrentDragon.AIStates.VoltChainCombo
                or ZacurrentDragon.AIStates.VoltZBallChainCombo)
            {
                ctx.ComboRecords.Add(pick);
                ctx.UseMoveCount = 0;
            }
        }

        /// <summary>把一个还没用过的连招放进池子；攒够一批就清空重新可用。旧 ZcurrentAI.cs:497-504</summary>
        private static void AddCombo(ZacurrentDragonContext ctx, List<(ZacurrentDragon.AIStates, float)> entries, ZacurrentDragon.AIStates combo)
        {
            if (ctx.ComboRecords.Count > ZacurrentDirector.ComboRecordCap)
            {
                ctx.ComboRecords.Clear();
            }

            if (!ctx.ComboRecords.Contains(combo))//没用过的连招才行
            {
                entries.Add((combo, ctx.UseMoveCount));
            }
        }

        /// <summary>过账：写上一手 + 推环形窗口，然后实例化。</summary>
        private static IVaultState<ZacurrentDragonContext> Accept(ZacurrentDragonContext ctx, ZacurrentDragon.AIStates id)
        {
            ctx.RecordPick((int)id);
            return Create(id);
        }
    }
}
