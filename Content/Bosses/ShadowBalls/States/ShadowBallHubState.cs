using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 连接段 + 唯一提交口。<br/><br/>
    /// <b>恢复的是哪条轮换</b>：旧代码里每个招式收尾调的是 <c>SwitchState_Test(AIStates.OnSpawnAnmi)</c>，
    /// 而出生动画在小球存在时又 <c>SwitchState_Test(AIStates.LunarEclipse)</c>——这是一条"出生动画 ↔ 月食"的调试死循环，
    /// 不是设计。判据有三：① <c>P1.ShadowSpike.cs:115</c> 与 <c>P1.SummonSmallBall.cs:139</c> 里真正的出口
    /// <c>SwitchP1State()</c> 就被注释在调试调用的上一行；② <c>P1.Revolution.cs:122</c> 与
    /// <c>P1.ShadowShoot.cs:26</c> 压根没改，收尾走的就是 <c>SwitchP1State()</c>；③ <c>SwitchState_Test</c> 这个名字。
    /// 所以本 hub 就是 <c>SwitchP1State</c> + <c>CompleteCurrentAttack</c> + <c>PickNextAttackState</c>
    /// （ShadowBall.cs:565-547 旧行号）三段的原样搬运：先看小球缺口决定要不要补球，再从权重表掷一手。
    /// </summary>
    [VaultState((int)ShadowBallStateId.Hub, typeof(ShadowBallContext))]
    public sealed class ShadowBallHubState : ShadowBallStateBase
    {
        public override ShadowBallStateId StateIndex => ShadowBallStateId.Hub;

        /// <summary>
        /// 一阶段招式权重表。四项等权 —— 旧 <c>ShadowBall.Phase1Picker</c>（ShadowBall.cs:500-506）一字不改。<br/>
        /// <b>注意</b>：影刺（<see cref="ShadowBallStateId.ShadowSpike"/>）招式体完整却不在表里，红移 / 蓝移招式体是空的也不在表里；
        /// 这是旧代码的原状，本轮按 D10 不动，列进报告等作者定。
        /// </summary>
        private static readonly WeightedRandomPicker<ShadowBallStateId> Phase1Picker = new(new (ShadowBallStateId, float)[]
        {
            (ShadowBallStateId.Revolution, 1f),
            (ShadowBallStateId.Starline, 1f),
            (ShadowBallStateId.LunarEclipse, 1f),
            (ShadowBallStateId.RollingLaser, 1f),
        });

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 连接段不改运动：旧代码收招当帧就切下一招，中间没有任何速度处理。
            ctx.DeclareKeep();
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            if (Timer < ShadowBallDirector.HubFrames)
            {
                return null;
            }

            return Commit(ctx);
        }

        /// <summary>
        /// 所有招式的收招出口。<see cref="ShadowBallDirector.HubFrames"/> &gt; 0 时进 hub 喘息；
        /// 为 0 时直接提交下一招，不多占一帧（旧代码零间隔，引入 hub 不得改变节奏）。<br/>
        /// Commit 建不出状态时退到 hub 驻留一帧再试，绝不让状态机停在收招那一帧（绝不锁空）。
        /// </summary>
        public static IVaultState<ShadowBallContext> EndAttack(ShadowBallContext ctx)
        {
            if (ShadowBallDirector.HubFrames > 0)
            {
                return Create(ShadowBallStateId.Hub);
            }

            return Commit(ctx) ?? Create(ShadowBallStateId.Hub);
        }

        /// <summary>
        /// 唯一提交口：小球缺口检查 → 补球 or 掷一手 → 记账。仅权威端调用。<br/>
        /// 旧 <c>SwitchP1State</c>（ShadowBall.cs:565-590）：现有小球数量低于"同场上限 − 难度缺口"就先去补球，
        /// 并把要补的个数写给召唤态；否则叫所有小球开始新一轮攻击，再从权重表掷一手。
        /// </summary>
        public static IVaultState<ShadowBallContext> Commit(ShadowBallContext ctx)
        {
            ShadowBall boss = ctx.Boss;

            int limit = ShadowBall.GetSmallBallSameTimeLimit();
            int current = boss.GetSmallBalls();

            ctx.Npc.TargetClosest();
            ctx.MarkDecision();

            // 补球条件多了一条"还有锁扣可弹"：旧代码没有这一条，锁扣耗尽后会一直往召唤态里塞，
            // 而召唤态发现没锁可弹就去阶段切换、阶段切换（二阶段没实现）又退回 hub —— 会来回弹。
            if (current < limit - ShadowBallDirector.SummonGapThreshold() && boss.HasActiveLock())
            {
                ctx.SummonCount = limit - current;
                return Accept(ctx, ShadowBallStateId.SummonSmallShdowBall);
            }

            boss.SmallBallStartAttack();
            return Accept(ctx, Roll(ctx));
        }

        /// <summary>
        /// 掷一手。<see cref="ShadowBallDirector.ForbidImmediateRepeat"/> 打开时与上一手相同就再掷一次避开；
        /// 两次都撞上就放行首选，绝不锁空（D4）。<br/>
        /// 旧代码没有防复读这一层，是本轮按 D4 补的记账，池子内容与权重一字未改。
        /// </summary>
        private static ShadowBallStateId Roll(ShadowBallContext ctx)
        {
            ShadowBallStateId pick = Phase1Picker.Pick(Main.rand.Next()).Item;

            if (!ShadowBallDirector.ForbidImmediateRepeat || (int)pick != ctx.LastPickedState)
            {
                return pick;
            }

            ShadowBallStateId retry = Phase1Picker.Pick(Main.rand.Next()).Item;
            return (int)retry == ctx.LastPickedState ? pick : retry;
        }

        /// <summary>过账：记上一手，然后实例化。补球态不进防复读账本（它是条件插入件，不算"一手"）。</summary>
        private static IVaultState<ShadowBallContext> Accept(ShadowBallContext ctx, ShadowBallStateId id)
        {
            if (id != ShadowBallStateId.SummonSmallShdowBall)
            {
                ctx.LastPickedState = (int)id;
            }

            return Create(id);
        }
    }
}
