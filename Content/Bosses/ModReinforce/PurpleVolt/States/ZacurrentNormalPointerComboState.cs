using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 普通形态连段 4：追踪雷球 → 闪电链（300 帧超长链） → 电流吐息（中） → 落雷 → 聚集电流。<br/>
    /// 这一段的链寿命是所有连段里最长的 300 帧：布完链之后的吐息与落雷都在链网里打，玩家要同时读三层威胁。<br/>
    /// 旧 <c>ZacurrentDragon.RunNormalPointerCombo</c>（ZacurrentDragon.Combos.cs:131-173）。
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.NormalPointerCombo, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentNormalPointerComboState : ZacurrentComboStateBase
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.NormalPointerCombo;

        protected override bool RunAttack(ZacurrentDragonContext ctx)
        {
            switch (ctx.Combo)
            {
                case 1:
                    return Step(ctx, ZacurrentElectricChainMove.Run(ctx, ZacurrentDirector.ChainTimePointerCombo), 2);
                case 2:
                    return Step(ctx, ZacurrentElectricBreathMiddleMove.Run(ctx), 3);
                case 3:
                    return Step(ctx, ZacurrentFallingThunderMove.Run(ctx), 4);
                case 4:
                    return Finish(ctx, ZacurrentGatherCurrentMove.Run(ctx));
                default:
                    return Step(ctx, ZacurrentAimThunderBallMove.Run(ctx, ZacurrentDirector.AimTimeNormalPointerCombo), 1);
            }
        }
    }
}
