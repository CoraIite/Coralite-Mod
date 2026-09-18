using Coralite.Content.Bosses.BabyIceDragon.Core;
using InnoVault.StateMachines;
using System.Linq;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 连接段 + 唯一提交口。<br/>
    /// 旧 <c>ResetStates</c>（BabyIceDragon.cs:834-900）的选招语义原样搬进 <see cref="Commit"/>：
    /// 转二阶段固定先放吼叫动画（单发闸）→ 普通招计数到门槛出一次有破绽动作并重填招池 →
    /// 否则从「用过即移除」的可枯竭招池里用 <see cref="WeightedRandomPicker{T}"/> 等权掷一手，池空退冰吐息。<br/>
    /// 旧代码收招当帧即切下一招、零间隔（招式间的喘息由 rest / dizzy 两个显式状态承担），
    /// 所以 <see cref="BabyIceDragonDirector.HubFrames"/> = 0：<see cref="EndAttack"/> 直接经 Commit 返回下一招，
    /// 本状态只在 Commit 建不出状态时驻留一帧兜底（绝不锁空）。
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.hub, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonHubState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.hub;

        /// <summary>有破绽动作二选一，下标即旧 <c>Main.rand.Next(2)</c> 的结果。旧 BabyIceDragon.cs:875-879</summary>
        private static readonly BabyIceDragonStateId[] vulnerableMoves =
        {
            BabyIceDragonStateId.dive,
            BabyIceDragonStateId.accumulate,
        };

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            ctx.DeclareKeep();
            ctx.DeclareFlyingFrame();
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if (Timer < BabyIceDragonDirector.HubFrames)
            {
                return null;
            }

            return Commit(ctx);
        }

        /// <summary>
        /// 所有招式的收招出口。<see cref="BabyIceDragonDirector.HubFrames"/> &gt; 0 时进 hub 喘息；为 0 时直接提交下一招不多占一帧。
        /// Commit 建不出状态（注册表缺 id）时退到 hub 驻留一帧再试，绝不让状态机停在收招那一帧。<br/>
        /// <c>new</c> 是有意遮蔽：本方法是唯一实现，<see cref="BabyIceDragonStateBase.EndAttack"/> 只是转发给它的快捷方式。
        /// </summary>
        public static new IVaultState<BabyIceDragonContext> EndAttack(BabyIceDragonContext ctx)
        {
            if (BabyIceDragonDirector.HubFrames > 0)
            {
                return Create(BabyIceDragonStateId.hub);
            }

            return Commit(ctx) ?? Create(BabyIceDragonStateId.hub);
        }

        /// <summary>
        /// 唯一提交口：转阶段闸 → 破绽闸 → 可枯竭招池 → 记账。仅权威端调用（客户端不选招）。
        /// </summary>
        public static IVaultState<BabyIceDragonContext> Commit(BabyIceDragonContext ctx)
        {
            ctx.Npc.TargetClosest(false);
            ctx.MarkDecision();

            // 血量进入二阶段时固定先放吼叫动画：下雨 + 重填二阶段招池（单发闸，旧 BabyIceDragon.cs:858-867）
            if (ctx.Phase >= 2 && ctx.ExchangeState)
            {
                ctx.ExchangeState = false;
                Main.StartRain();
                Main.SyncRain();
                ctx.RefillMovePool(2);
                return Accept(ctx, BabyIceDragonStateId.roaringAnim);
            }

            // 普通招计数到门槛：归零、重填招池、出一次有破绽动作（旧 BabyIceDragon.cs:870-882）
            if (ctx.NormalMoveCount > BabyIceDragonDirector.VulnerableAfterMoves())
            {
                ctx.NormalMoveCount = 0;
                ctx.RefillMovePool(ctx.Phase);
                return Accept(ctx, vulnerableMoves[Main.rand.Next(vulnerableMoves.Length)]);
            }

            BabyIceDragonStateId next = TakeFromPool(ctx);
            ctx.NormalMoveCount++;
            return Accept(ctx, next);
        }

        /// <summary>
        /// 从可枯竭招池里等权掷一手并移除；池空退兜底招（旧代码此处不重填，等下一次破绽闸重填）。旧 BabyIceDragon.cs:885-896
        /// </summary>
        private static BabyIceDragonStateId TakeFromPool(BabyIceDragonContext ctx)
        {
            if (ctx.Moves.Count < 1)
            {
                return BabyIceDragonDirector.FallbackMove;
            }

            WeightedRandomPicker<BabyIceDragonStateId> picker = new(ctx.Moves.Select(m => (m, 1f)));
            (BabyIceDragonStateId move, int index) = picker.Pick(Main.rand.Next());
            ctx.Moves.RemoveAt(index);
            return move;
        }

        /// <summary>过账：写上一手 + 推查重窗口，然后实例化。所有出招都经这里，不留第二条记账路径。</summary>
        private static IVaultState<BabyIceDragonContext> Accept(BabyIceDragonContext ctx, BabyIceDragonStateId id)
        {
            ctx.RecordPick((int)id);
            return Create(id);
        }
    }
}
