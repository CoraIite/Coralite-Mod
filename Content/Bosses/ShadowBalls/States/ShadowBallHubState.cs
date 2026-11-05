using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 连接段
    /// </summary>
    [VaultState((int)ShadowBallStates.Hub, typeof(ShadowBallContext))]
    public sealed class ShadowBallHubState : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.Hub;

        /// <summary>
        /// 一阶段前半段招式<br/>
        /// </summary>
        private static readonly WeightedRandomPicker<ShadowBallStates> Phase1EarlyPicker = new(
        [
            (ShadowBallStates.Starline, 1f),
            (ShadowBallStates.LunarEclipse, 1f),
            (ShadowBallStates.RollingLaser, 1f),
            (ShadowBallStates.Gravity, 1f),
            (ShadowBallStates.RedShift, 0.1f),
            (ShadowBallStates.BlueShift, 0.1f),
        ]);

        /// <summary>
        /// 一阶段后半段招式<br/>
        /// </summary>
        private static readonly WeightedRandomPicker<ShadowBallStates> Phase1LastPicker = new(
        [
            (ShadowBallStates.Starline, 0.8f),
            (ShadowBallStates.LunarEclipse, 0.8f),
            (ShadowBallStates.RollingLaser, 0.8f),
            (ShadowBallStates.Gravity, 0.8f),
            (ShadowBallStates.GammaRayBurst, 1f),
            (ShadowBallStates.RedShift, 0.1f),
            (ShadowBallStates.BlueShift, 0.1f),
        ]);

        /// <summary>
        /// 一阶段后半段招式<br/>
        /// </summary>
        private static readonly WeightedRandomPicker<ShadowBallStates> Phase2Picker = new(
        [
            (ShadowBallStates.MucuryBurst, 1f),
            (ShadowBallStates.UnstableEruption, 1f),
            (ShadowBallStates.MultiEruption, 1f),
        ]);

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
            => ctx.SetVelocityLetItGo();

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            if (Timer <0)
                return null;

            return Commit(ctx);
        }

        /// <summary>
        /// 所有招式的收招出口。<see cref="ShadowBallDirector.HubFrames"/> &gt; 0 时进 hub 喘息；
        /// 为 0 时直接提交下一招，不多占一帧（旧代码零间隔，引入 hub 不得改变节奏）。<br/>
        /// Commit 建不出状态时退到 hub 驻留一帧再试，绝不让状态机停在收招那一帧（绝不锁空）。
        /// </summary>
        public new static IVaultState<ShadowBallContext> EndAttack(ShadowBallContext ctx)
        {
            return Commit(ctx) ?? Create(ShadowBallStates.Hub);
        }

        /// <summary>
        /// 唯一提交口：小球缺口检查 → 补球 or 掷一手 → 记账。仅权威端调用。<br/>
        /// 旧 <c>SwitchP1State</c>（ShadowBall.cs:565-590）：现有小球数量低于"同场上限 − 难度缺口"就先去补球，
        /// 并把要补的个数写给召唤态；否则叫所有小球开始新一轮攻击，再从权重表掷一手。
        /// </summary>
        public static IVaultState<ShadowBallContext> Commit(ShadowBallContext ctx)
        {
            ShadowBall boss = ctx.Boss;

            ctx.Npc.TargetClosest();
            ctx.NetUpdate();

            boss.SmallBallStartAttack();
            return Accept(ctx, Roll(ctx));
        }

        public static IVaultState<ShadowBallContext> CommitTest(ShadowBallContext ctx, ShadowBallStates id)
        {
            ShadowBall boss = ctx.Boss;
            boss.SmallBallStartAttack();
            return Accept(ctx, id);
        }

        /// <summary>
        /// 随机不重复
        /// </summary>
        private static ShadowBallStates Roll(ShadowBallContext ctx)
        {
            //选择招式
            WeightedRandomPicker<ShadowBallStates> picker;

            switch (ctx.Boss.Phase)
            {
                case ShadowBall.AIPhases.P1_WithSmallBalls://一阶段前半和后半用不同的
                    if (ctx.Phase1Mucury)
                        picker = Phase1EarlyPicker;
                    else
                        picker = Phase1LastPicker;
                    break;
                case ShadowBall.AIPhases.P2_BigBallSmash:
                    picker = Phase2Picker;
                    break;
                default:
                case ShadowBall.AIPhases.Others://默认返回星轨招式
                    return ShadowBallStates.Starline;
            }

            for (int i = 0; i < 3; i++)//尝试3次直到与上一招不同
            {
                ShadowBallStates pick = picker.Pick(Main.rand.Next()).Item;
                if ((int)pick != ctx.LastPickedState)
                    return pick;
            }

            return picker.Pick(Main.rand.Next()).Item;
        }

        private static IVaultState<ShadowBallContext> Accept(ShadowBallContext ctx, ShadowBallStates id)
        {
            if (id != ShadowBallStates.SummonSmallShdowBall)
                ctx.LastPickedState = (int)id;

            ctx.Recorder1 = 0;
            ctx.Recorder2 = 0;
            return Create(id);
        }
    }
}
