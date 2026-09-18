using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 普通形态连段 2：吼叫 → 闪电链（100 帧链） → 闪电突袭（固定 3 轮长冲） → 聚集电流。<br/>
    /// 第三手把 <see cref="ZacurrentDragonContext.Recorder2"/> 硬写成 3，绕过突袭自己的随机轮数——
    /// 连段里这一手是"确定的三连冲"，不随机。<br/>
    /// 旧 <c>ZacurrentDragon.RunNormalRoarCombo2</c>（ZacurrentDragon.Combos.cs:55-92）。
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.NormalRoarCombo2, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentNormalRoarCombo2State : ZacurrentComboStateBase
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.NormalRoarCombo2;

        protected override bool RunAttack(ZacurrentDragonContext ctx)
        {
            switch (ctx.Combo)
            {
                case 1:
                    if (ZacurrentElectricChainMove.Run(ctx, ZacurrentDirector.ChainTimeRoarCombo2))
                    {
                        ctx.ResetAttackFields();
                        ctx.Combo = 2;
                        ZacurrentLightningRaidNormalMove.SetStartValue(ctx);
                        ctx.Recorder2 = ZacurrentDirector.LightningRaidLongDashesCombo;
                        ctx.MarkDecision();
                    }

                    return false;
                case 2:
                    return Step(ctx, ZacurrentLightningRaidNormalMove.Run(ctx), 3);
                case 3:
                    return Finish(ctx, ZacurrentGatherCurrentMove.Run(ctx));
                default:
                    return Step(ctx, ZacurrentRoarMove.Run(ctx), 1);
            }
        }
    }
}
