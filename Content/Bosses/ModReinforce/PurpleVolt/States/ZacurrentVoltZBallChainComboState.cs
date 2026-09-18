using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 紫伏连段：Z 电球 → 闪电突袭（固定 3 轮） → 电伏击穿 → 指针电球（180 帧追踪） → 落雷。<br/>
    /// 起手的 Z 电球没有单招入口，只在这里与超长连段里出现。<br/>
    /// 旧 <c>ZacurrentDragon.RunVoltZBallChainCombo</c>（ZacurrentDragon.Combos.cs:286-331）。
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.VoltZBallChainCombo, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentVoltZBallChainComboState : ZacurrentComboStateBase
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.VoltZBallChainCombo;

        // 旧实现这里不调 ZThunderBall 的 SetStartValue：Recorder 维持 0，起手固定是"单电球 + 直线链球"那一式，照搬不动。

        protected override bool RunAttack(ZacurrentDragonContext ctx)
        {
            switch (ctx.Combo)
            {
                case 1:
                    if (ZacurrentLightningRaidVoltMove.Run(ctx))
                    {
                        ctx.ResetAttackFields(false);
                        ZacurrentVoltBreakMove.SetStartValue(ctx);
                        ctx.Combo = 2;
                        ctx.MarkDecision();
                    }

                    return false;
                case 2:
                    return Step(ctx, ZacurrentVoltBreakMove.Run(ctx), 3);
                case 3:
                    return Step(ctx, ZacurrentPointerBallMove.Run(ctx, ZacurrentDirector.PointerAimTimeZBallCombo), 4);
                case 4:
                    return Finish(ctx, ZacurrentFallingThunderMove.Run(ctx));
                default:
                    if (ZacurrentZThunderBallMove.Run(ctx))
                    {
                        ctx.ResetAttackFields(false);
                        ZacurrentLightningRaidNormalMove.SetStartValue(ctx);
                        ctx.Recorder2 = ZacurrentDirector.LightningRaidLongDashesCombo;
                        ctx.Combo = 1;
                        ctx.MarkDecision();
                    }

                    return false;
            }
        }
    }
}
