using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 普通形态连段 3：闪电链（60 帧链） → 落雷 → 冲刺放电 → 聚集电流。<br/>
    /// 这是四条普通连段里唯一不带吼叫起手的，节奏最紧——起手就是绕飞布链。<br/>
    /// 旧 <c>ZacurrentDragon.RunNormalChainCombo</c>（ZacurrentDragon.Combos.cs:94-129）。
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.NormalChainCombo, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentNormalChainComboState : ZacurrentComboStateBase
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.NormalChainCombo;

        protected override bool RunAttack(ZacurrentDragonContext ctx)
        {
            switch (ctx.Combo)
            {
                case 1:
                    return Step(ctx, ZacurrentFallingThunderMove.Run(ctx), 2);
                case 2:
                    return Step(ctx, ZacurrentDashDischargingMove.Run(ctx), 3);
                case 3:
                    return Finish(ctx, ZacurrentGatherCurrentMove.Run(ctx));
                default:
                    return Step(ctx, ZacurrentElectricChainMove.Run(ctx, ZacurrentDirector.ChainTimeChainCombo), 1);
            }
        }
    }
}
