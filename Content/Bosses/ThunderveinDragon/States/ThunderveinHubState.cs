using Coralite.Content.Bosses.ThunderveinDragon.Core;
using InnoVault.StateMachines;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon.States
{
    /// <summary>
    /// 连接段 + 唯一提交口。<br/>
    /// 旧 <c>ResetStates</c>（ThunderveinDragon.cs:645-739）的选招语义原样搬进 <see cref="Commit"/>：
    /// 一张每手现搭的加权列表（同一个 id 重复入列就是旧代码的权重），按“玩家在背后 / 上一手不是短冲 / 距离档位 / 阶段 / 累计手数”
    /// 追加权重，再剔掉上一手（上一手是短冲时连上上手一起剔），最后 <c>Main.rand.NextFromList</c> 一次掷出。<br/>
    /// 旧代码收招当帧即 <c>ResetStates</c> 切下一招、零间隔（后摇已含在各招尾段），所以 <see cref="ThunderveinDirector.HubFrames"/> = 0：
    /// <see cref="EndAttack"/> 直接经 Commit 返回下一招，本状态只在 Commit 建不出状态时驻留一帧兜底（绝不锁空）。<br/>
    /// 引力雷球的固定接招（旧 <c>ResetToSelectedState</c>）走 <see cref="CommitFixed"/>，同样过账。
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.Hub, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinHubState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.Hub;

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            // 兜底驻留帧：保持速度、继续扇翅，不改变任何战斗量
            ctx.DeclareKeep();
            ctx.Boss.FlyingFrame();
        }

        protected override IVaultState<ThunderveinDragonContext> AuthorityUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            if (Timer < ThunderveinDirector.HubFrames)
            {
                return null;
            }

            return Commit(ctx);
        }

        /// <summary>
        /// 所有招式的收招出口（<see cref="ThunderveinStateBase.EndAttack"/> 转发到这里，同名是刻意的）。
        /// <see cref="ThunderveinDirector.HubFrames"/> = 0，收招当帧直接提交下一招、不多占一帧（保持旧节奏）；
        /// Commit 建不出状态（注册表缺 id）时退到 hub 驻留一帧再试，绝不让状态机停在收招那一帧。
        /// 若把 HubFrames 改成正数，这里要改回先 <c>Create(Hub)</c> 进喘息段。
        /// </summary>
        public static new IVaultState<ThunderveinDragonContext> EndAttack(ThunderveinDragonContext ctx)
            => Commit(ctx) ?? Create(ThunderveinDragon.AIStates.Hub);

        /// <summary>
        /// 唯一提交口：搭加权列表 → 剔重 → 一次掷骰 → 记账 → 推进累计手数。旧 ThunderveinDragon.cs:645-739，权重与顺序逐条对齐。
        /// </summary>
        public static IVaultState<ThunderveinDragonContext> Commit(ThunderveinDragonContext ctx)
        {
            int oldState = ctx.CurrentStateId;
            float distance = ctx.TargetDistance;
            int dir = ctx.TargetDirX;
            int phase = ctx.Phase;

            List<int> moves = new()
            {
                (int)ThunderveinDragon.AIStates.LightningRaid,
                (int)ThunderveinDragon.AIStates.FallingThunder,
                (int)ThunderveinDragon.AIStates.LightningBall,
                (int)ThunderveinDragon.AIStates.LightningBreath,
            };

            if (Main.masterMode)
            {
                moves.Add((int)ThunderveinDragon.AIStates.CrossLightingBall);
            }

            // 玩家在背后时大概率使用闪电突袭
            if (dir != ctx.Npc.spriteDirection)
            {
                Repeat(moves, ThunderveinDragon.AIStates.LightningRaid, ThunderveinDirector.BehindRaidWeight);
            }

            // 上一手不是短冲就小冲一下
            if (oldState != (int)ThunderveinDragon.AIStates.SmallDash)
            {
                Repeat(moves, ThunderveinDragon.AIStates.SmallDash, ThunderveinDirector.SmallDashWeight);
            }

            // 距离较近时大概率放电
            if (distance < ThunderveinDirector.DischargeDistance)
            {
                Repeat(moves, ThunderveinDragon.AIStates.Discharging, ThunderveinDirector.DischargeWeight);
            }

            if (phase == 1)
            {
                if (distance > ThunderveinDirector.FarDistance)
                {
                    Repeat(moves, distance > ThunderveinDirector.VeryFarDistance
                        ? ThunderveinDragon.AIStates.FallingThunder
                        : ThunderveinDragon.AIStates.LightningRaid, ThunderveinDirector.FarWeight);
                }
            }
            else
            {
                moves.Add((int)ThunderveinDragon.AIStates.DashDischarging);

                if (distance > ThunderveinDirector.FarDistance)
                {
                    Repeat(moves, distance > ThunderveinDirector.VeryFarDistance
                        ? ThunderveinDragon.AIStates.FallingThunder
                        : ThunderveinDragon.AIStates.DashDischarging, ThunderveinDirector.FarWeight);
                }

                // 二阶段累计出招超过门槛后，引力雷球按累计手数加权
                if (ctx.UseMoveCount > ThunderveinDirector.GravitationUnlockCount)
                {
                    Repeat(moves, ThunderveinDragon.AIStates.GravitationThunder, ctx.UseMoveCount);
                }
            }

            // 上一手是短冲时连上上手一起剔，避免“A → 短冲 → A”
            if (oldState == (int)ThunderveinDragon.AIStates.SmallDash)
            {
                moves.RemoveAll(i => i == ctx.StateBeforeSmallDash);
            }

            moves.RemoveAll(i => i == oldState);

            if (moves.Count == 0)
            {
                // 旧代码不会走到这里（基础四招恒在列且不可能同时被剔空），留作防御：绝不锁空
                return null;
            }

            int next = Main.rand.NextFromList(moves.ToArray());

            if (phase != 1)
            {
                ctx.UseMoveCount++;
                // 用过引力雷球即归零
                if (next == (int)ThunderveinDragon.AIStates.GravitationThunder)
                {
                    ctx.UseMoveCount = 0;
                }
            }

            if (next == (int)ThunderveinDragon.AIStates.SmallDash)
            {
                ctx.StateBeforeSmallDash = oldState;
            }

            ctx.MarkDecision();
            return Accept(ctx, (ThunderveinDragon.AIStates)next);
        }

        /// <summary>
        /// 固定接招（引力雷球收招后三选一）：只记账，不动累计手数、不重新索敌。旧 <c>ResetToSelectedState</c> ThunderveinDragon.cs:742-751
        /// </summary>
        public static IVaultState<ThunderveinDragonContext> CommitFixed(ThunderveinDragonContext ctx, ThunderveinDragon.AIStates id)
        {
            ctx.MarkDecision();
            return Accept(ctx, id) ?? Create(ThunderveinDragon.AIStates.Hub);
        }

        /// <summary>把同一个 id 重复入列 <paramref name="weight"/> 次——旧代码的加权方式。</summary>
        private static void Repeat(List<int> moves, ThunderveinDragon.AIStates id, int weight)
        {
            for (int i = 0; i < weight; i++)
            {
                moves.Add((int)id);
            }
        }

        /// <summary>过账：写上一手 + 推环形窗口，然后实例化。</summary>
        private static IVaultState<ThunderveinDragonContext> Accept(ThunderveinDragonContext ctx, ThunderveinDragon.AIStates id)
        {
            ctx.RecordPick((int)id);
            return Create(id);
        }
    }
}
