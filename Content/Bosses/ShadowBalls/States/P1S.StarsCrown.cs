using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)SmallShadowBallStateId.StarsCrown, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallStarsCrownState : SmallShadowBallStateBase
    {
        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.StarsCrown;

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            const float orbitSpeed = 0.02f;

            SmallShadowBall ball = ctx.Ball;
            int index = ball.selfIndex;
            float count = Math.Max(1, owner.smallBalls.Count);

            //索引从 0 开始；两个极点分别均分相位，兼容总数为奇数或只有一颗小球
            float percent = index  / count;
            float poleRotation = -MathHelper.PiOver2;
            Vector2 poleCenter = owner.NPC.Center + poleRotation.ToRotationVector2()
                * (180 * owner.NPC.scale / 2f);

            float tilt = (MathHelper.PiOver2 - MathHelper.Pi / 10f);
            Vector2 targetPos = poleCenter + ball.Rotate3D(percent, 140, owner.LockTimer * orbitSpeed, tilt, 0, owner.LockLerpPercent);

            ctx.Npc.velocity = Vector2.Zero;
            ctx.Npc.Center = Vector2.Lerp(ctx.Npc.Center, targetPos, MathHelper.Clamp(Timer / 30f, 0f, 1f));
            ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp(poleRotation, 0.15f);
            ball.lockRotation = ball.lockRotation.AngleLerp((ctx.Npc.Center - owner.NPC.Center).ToRotation(), 0.15f);
        }
    }
}
