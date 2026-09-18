using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 紫伏超长连段（全 boss 最长的一手，hub 里还要求"已经用过别的连招"才解锁，所以它是收尾大招）：<br/>
    /// 吼叫 → 闪电链（10 帧短链） → 循环 ×3（指针电球 → 闪电突袭 1 轮） → 引力电球（6 秒超长）
    /// → 电流吐息（中，后摇压到 2 帧） → Z 电球 → 电伏击穿 → 落雷。<br/>
    /// 中段的三轮循环用 <see cref="ZacurrentDragonContext.Combo"/> 的奇偶实现（偶数手=指针电球、奇数手=突袭），
    /// 与旧 <c>case 2/4/6</c> 与 <c>case 3/5/7</c> 的写法等价。<br/>
    /// 旧 <c>ZacurrentDragon.RunVoltBigCombo</c>（ZacurrentDragon.Combos.cs:175-253）。
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.VoltBigCombo, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentVoltBigComboState : ZacurrentComboStateBase
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.VoltBigCombo;

        /// <summary>循环段的第一手下标（Combo 2 起，偶数指针、奇数突袭，共 3 轮到 Combo 7）。</summary>
        private const int LoopStart = 2;

        private static int LoopEnd => LoopStart + (ZacurrentDirector.VoltBigComboRaidLoops * 2);

        protected override bool RunAttack(ZacurrentDragonContext ctx)
        {
            int combo = ctx.Combo;

            if (combo >= LoopStart && combo < LoopEnd)
            {
                return RunLoop(ctx, combo);
            }

            switch (combo)
            {
                case 1:
                    return Step(ctx, ZacurrentElectricChainMove.Run(ctx, ZacurrentDirector.ChainTimeVoltBigCombo), LoopStart);
                case 8:
                    return Step(ctx, ZacurrentGravitationThunderMove.Run(ctx, ZacurrentDirector.GravitationTimeBigCombo), 9);
                case 9:
                    if (ZacurrentElectricBreathMiddleMove.Run(ctx, ZacurrentDirector.BreathMiddleRestBigCombo))
                    {
                        ctx.ResetAttackFields(false);
                        ctx.Combo = 10;
                        ZacurrentZThunderBallMove.SetStartValue(ctx);
                        ctx.MarkDecision();
                    }

                    return false;
                case 10:
                    if (ZacurrentZThunderBallMove.Run(ctx))
                    {
                        ctx.ResetAttackFields(false);
                        ZacurrentVoltBreakMove.SetStartValue(ctx);
                        ctx.Combo = 11;
                        ctx.MarkDecision();
                    }

                    return false;
                case 11:
                    return Step(ctx, ZacurrentVoltBreakMove.Run(ctx), 12);
                case 12:
                    return Finish(ctx, ZacurrentFallingThunderMove.Run(ctx));
                default:
                    return Step(ctx, ZacurrentRoarMove.Run(ctx), 1);
            }
        }

        /// <summary>中段三轮循环：偶数手撒指针电球，奇数手接一轮闪电突袭（轮数被硬写成 1，不随机）。</summary>
        private static bool RunLoop(ZacurrentDragonContext ctx, int combo)
        {
            if ((combo - LoopStart) % 2 == 0)
            {
                if (ZacurrentPointerBallMove.Run(ctx, ZacurrentDirector.PointerBallAimTime))
                {
                    ctx.ResetAttackFields(false);
                    ZacurrentLightningRaidNormalMove.SetStartValue(ctx);
                    ctx.Recorder2 = ZacurrentDirector.LightningRaidLongDashesBigCombo;
                    ctx.Combo = combo + 1;
                    ctx.MarkDecision();
                }

                return false;
            }

            return Step(ctx, ZacurrentLightningRaidVoltMove.Run(ctx), combo + 1);
        }
    }
}
