using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>红移：蓄力 60 帧，释放冲击波与小球弹幕，再等待 120 帧收招。</summary>
    [VaultState((int)ShadowBallStateId.RedShift, typeof(ShadowBallContext))]
    public sealed class ShadowBallRedShiftState : ShadowBallStateBase
    {
        private enum Beat
        {
            Gather,
            Release,
            Recover,
        }

        private const int GatherFrames = 60;
        private const int GatherCircleInterval = 20;
        private const int RecoverFrames = 60 * 2;

        public override ShadowBallStateId StateIndex => ShadowBallStateId.RedShift;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.SetVelocityLetItGo();

            switch ((Beat)BeatIndex)
            {
                case Beat.Gather:
                    if (!Main.dedServ && Timer % GatherCircleInterval == 0)
                    {
                        // TODO: 生成聚集圈特效（第 20、40、60 帧）。
                    }

                    if (Timer >= GatherFrames)
                        SwitchBeat(ctx, (int)Beat.Release);
                    break;

                case Beat.Release:
                    // 释放当帧由权威端生成弹幕，下一帧两端一起开始收招计时。
                    SwitchBeat(ctx, (int)Beat.Recover);
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Release when Timer == 0:
                    // TODO: 生成大型红色冲击波弹幕；玩家击退由该弹幕实现。

                    ctx.Boss.GetSmallBalls();
                    foreach (NPC npc in ctx.Boss.smallBalls)
                    {
                        if (!npc.active || npc.ModNPC is not SmallShadowBall smallBall)
                            continue;

                        // TODO: 让 smallBall 放出红色旋转弹幕。
                    }
                    break;

                case Beat.Recover:
                    if (Timer >= RecoverFrames)
                        return EndAttack(ctx);
                    break;
            }

            return null;
        }
    }
}
