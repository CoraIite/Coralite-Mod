using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 普通形态连段 1：吼叫 → 电球 → 引力电球 → 电磁炮 → 聚集电流。<br/>
    /// 结构上是一条固定序列：<see cref="ZacurrentDragonContext.Combo"/> 记当前第几手，
    /// 每手的招式体返回 true 就清招内量并进下一手，最后一手打完整段结束回 hub。<br/>
    /// <b>Combo 必须过线</b>：它决定客户端这一帧该跑哪个招式体，不同步就会两端跑成两招（热字段 C 槽）。<br/>
    /// 旧 <c>ZacurrentDragon.RunNormalRoarCombo1</c>（ZacurrentDragon.Combos.cs:10-53）。
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.NormalRoarCombo1, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentNormalRoarCombo1State : ZacurrentComboStateBase
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.NormalRoarCombo1;

        protected override bool RunAttack(ZacurrentDragonContext ctx)
        {
            switch (ctx.Combo)
            {
                case 1:
                    return Step(ctx, ZacurrentElectricBallMove.Run(ctx), 2);
                case 2:
                    return Step(ctx, ZacurrentGravitationThunderMove.Run(ctx), 3);
                case 3:
                    return Step(ctx, ZacurrentElectromagneticCannonMove.Run(ctx), 4);
                case 4:
                    return Finish(ctx, ZacurrentGatherCurrentMove.Run(ctx));
                default:
                    if (ZacurrentRoarMove.Run(ctx))
                    {
                        ctx.ResetAttackFields();
                        ZacurrentElectricBallMove.SetStartValue(ctx);
                        ctx.Combo = 1;
                        ctx.MarkDecision();
                    }

                    return false;
            }
        }
    }
}
