using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 星轨（小球侧）：均匀排在本体头顶的圆环上，然后按索引依次向玩家射激光。<br/>
    /// 旧 <c>SmallShadowBall.Starline</c>（P1S.Starline.cs:14-63）。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.Starline, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallStarlineState : SmallShadowBallStateBase
    {
        /// <summary>星轨的子拍。旧 <c>SonState</c> 0~1。</summary>
        private enum Beat
        {
            /// <summary>移动到环绕轨道位。</summary>
            MoveToOrbit,
            /// <summary>依次发射激光。</summary>
            FireLaser,
        }

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.Starline;

        /// <summary>轨道位：本体头顶 190 px 处再沿索引角偏出 150 px 的圆环。旧 P1S.Starline.cs:22-27。</summary>
        private static Vector2 OrbitPosition(SmallShadowBallContext ctx, ShadowBall owner)
        {
            int count = Math.Max(1, owner.smallBalls.Count);
            float angle = ctx.Ball.selfIndex * MathHelper.TwoPi / count;

            return owner.NPC.Center
                + new Vector2(0, -SmallShadowBallDirector.StarlineOrbitHeight)
                + (angle.ToRotationVector2() * SmallShadowBallDirector.StarlineOrbitRadius);
        }

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.MoveToOrbit:
                    MoveToAttackPosition(ctx, OrbitPosition(ctx, owner));
                    if (Timer > SmallShadowBallDirector.StarlineMoveFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.FireLaser);
                    }

                    break;

                case Beat.FireLaser:
                    ctx.Npc.velocity *= SmallShadowBallDirector.StarlineFireDamp;
                    ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp(
                        (owner.Target.Center - ctx.Npc.Center).ToRotation(), SmallShadowBallDirector.StarlineFireRotLerp);
                    break;
            }
        }

        protected override IVaultState<SmallShadowBallContext> AuthorityUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            if ((Beat)BeatIndex != Beat.FireLaser)
            {
                return null;
            }

            // 基础延迟 + 自身索引错开，多个小球依次开火形成连续激光。
            if (Timer == SmallShadowBallDirector.StarlineFireDelay
                + (ctx.Ball.selfIndex * SmallShadowBallDirector.StarlineFireStagger))
            {
                ctx.Npc.NewProjectileDirectInAI_Server<SmallLaser>(ctx.Npc.Center, Vector2.Zero,
                    SmallShadowBallDirector.RollingLaserDamage(), 0,
                    ai0: ctx.Npc.whoAmI, ai1: SmallShadowBallDirector.StarlineLaserTime);
                ctx.MarkDecision();
            }

            if (Timer > SmallShadowBallDirector.StarlineFrames)
            {
                return EndAttack();
            }

            return null;
        }
    }
}
