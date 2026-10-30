using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>伽玛射线暴：按奇偶索引分到本体两极，在朝屏幕倾斜的三维小圆上环绕。</summary>
    [VaultState((int)SmallShadowBallStateId.GammaRayBurst, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallGammaRayBurstState : SmallShadowBallStateBase
    {
        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.GammaRayBurst;

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            // 小圆半径独立于极点距本体中心的距离；转速取缓慢环绕的初始调参值。
            const float orbitRadius = 50f;
            const float orbitSpeed = 0.02f;
            const float gatherFrames = 30f;

            SmallShadowBall ball = ctx.Ball;
            int index = ball.selfIndex;
            int count = Math.Max(1, owner.smallBalls.Count);
            bool odd = index % 2 != 0;
            int side = odd ? 1 : -1;

            //索引从 0 开始；两个极点分别均分相位，兼容总数为奇数或只有一颗小球。
            int groupCount = Math.Max(1, odd ? count / 2 : (count + 1) / 2);
            float percent = (index / 2) / (float)groupCount;
            float poleRotation = owner.NPC.rotation +MathHelper.PiOver2+ side * MathHelper.PiOver2;
            Vector2 poleCenter = owner.NPC.Center + poleRotation.ToRotationVector2()
                * (200 * owner.NPC.scale / 2f);

            // 圆面法线先指向对应极点，再朝屏幕（正 Z）偏转 30 度。
            // Rotate3D 从 XY 圆绕 X、Z 轴旋转，因此 X 轴倾角为带符号的 60 度。
            float tilt = -side * (MathHelper.PiOver2 - MathHelper.Pi / 6f);
            Vector2 targetPos = poleCenter + ball.Rotate3D(percent, orbitRadius,
                owner.LockTimer * orbitSpeed, tilt, owner.NPC.rotation+MathHelper.PiOver2);

            // 共用本体时钟保持同组相位一致；先收拢，随后准确贴住轨道并跟随本体移动、转向。
            ctx.Npc.velocity = Vector2.Zero;
            ctx.Npc.Center = Vector2.Lerp(ctx.Npc.Center, targetPos, MathHelper.Clamp(Timer / gatherFrames, 0f, 1f));
            ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp(poleRotation, 0.15f);
            ball.lockRotation = ball.lockRotation.AngleLerp((ctx.Npc.Center-owner.NPC.Center).ToRotation(), 0.15f);
        }
    }
}