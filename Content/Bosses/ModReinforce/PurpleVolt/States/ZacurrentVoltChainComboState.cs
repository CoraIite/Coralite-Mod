using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 紫伏短连段：闪电链（140 帧链） → 电伏击穿 → 电流吐息（中）。<br/>
    /// 三手就完，是紫伏形态的"快连"，与超长连段形成节奏对比。<br/>
    /// 旧 <c>ZacurrentDragon.RunVoltChainCombo</c>（ZacurrentDragon.Combos.cs:255-284）。
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.VoltChainCombo, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentVoltChainComboState : ZacurrentComboStateBase
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.VoltChainCombo;

        protected override bool RunAttack(ZacurrentDragonContext ctx)
        {
            switch (ctx.Combo)
            {
                case 1:
                    return Step(ctx, ZacurrentVoltBreakMove.Run(ctx), 2);
                case 2:
                    return Finish(ctx, ZacurrentElectricBreathMiddleMove.Run(ctx));
                default:
                    if (ZacurrentElectricChainMove.Run(ctx, ZacurrentDirector.ChainTimeVoltChainCombo))
                    {
                        ctx.ResetAttackFields(false);
                        ZacurrentVoltBreakMove.SetStartValue(ctx);
                        ctx.Combo = 1;
                        ctx.MarkDecision();
                    }

                    return false;
            }
        }
    }
}
