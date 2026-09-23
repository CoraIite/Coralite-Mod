using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 影之公转（小球侧）：按索引向内外扩展成不同半径的环绕本体旋转，中途持续向后抛影子弹幕。<br/>
    /// 旧 <c>SmallShadowBall.Revolution</c>（P1S.Revolution.cs:9-43），只有一拍。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.Revolution, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallRevolutionState : SmallShadowBallStateBase
    {

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.Revolution;

        public override void WriteHot(SmallShadowBallContext ctx)
        {
            base.WriteHot(ctx);
        }

        public override void ReadHot(SmallShadowBallContext ctx)
        {
            base.ReadHot(ctx);
        }

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            SmallShadowBall ball = ctx.Ball;

            // 旧代码写的是 Timer == 0，而旧包壳态是"跑完招式体再 Timer++"；新基座第一帧就读到 1，所以这里是 1。

            /*
             * ctx.Hot[BossSlots.A]记录旋转半径
             */
            if (Timer == 1)
            {
                ctx.Hot[BossSlots.A] = MathHelper.Clamp(Vector2.Distance(owner.NPC.Center, owner.Target.Center),
                    SmallShadowBallDirector.RevolutionRadiusMin, SmallShadowBallDirector.RevolutionRadiusMax);
            }

            // 第一个小球用基准半径，之后按索引向两侧交替扩展一圈一圈往外排（设计文档 §影之公转 "向两侧扩展"）。
            int ringOffset = ball.selfIndex == 0
                ? 0
                : (ball.selfIndex % 2 == 1 ? -(ball.selfIndex + 1) / 2 : ball.selfIndex / 2);

            float radius = Math.Max(SmallShadowBallDirector.RevolutionRadiusFloor,
                ctx.Hot[BossSlots.A] + (ringOffset * SmallShadowBallDirector.RevolutionRingGap));

            float rotation = (Timer * (SmallShadowBallDirector.RevolutionSpinBase
                    + ((ball.selfIndex % 2) * SmallShadowBallDirector.RevolutionSpinOddGain)))
                + (ball.selfIndex * SmallShadowBallDirector.RevolutionPhasePerIndex);

            MoveToAttackPosition(ctx, owner.NPC.Center + (rotation.ToRotationVector2() * radius),
                SmallShadowBallDirector.RevolutionMoveLerp);

            ball.zDepth = MathF.Sin(rotation) * radius;
        }

        protected override IVaultState<SmallShadowBallContext> AuthorityUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            if (Timer > SmallShadowBallDirector.RevolutionShootStart
                && Timer % SmallShadowBallDirector.RevolutionShootInterval == 0)
            {
                ctx.Npc.NewProjectileDirectInAI_Server<ShadowBallOrbitShadow>(ctx.Npc.Center,
                    ctx.Npc.velocity * SmallShadowBallDirector.RevolutionShadowSpeedScale,
                    SmallShadowBallDirector.RevolutionShadowDamage(), 0,
                    ai0: SmallShadowBallDirector.RevolutionShadowLife);
                ctx.MarkDecision();
            }

            if (Timer > SmallShadowBallDirector.RevolutionFrames)
            {
                return EndAttack();
            }

            return null;
        }
    }
}
