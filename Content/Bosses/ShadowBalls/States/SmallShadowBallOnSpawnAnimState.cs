using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 小球出生动画：被锁扣弹出去后向外飘，接到本体射来的影子弹幕后锁扣打开、自身显形，最后锁扣绕一圈进待机。<br/>
    /// 旧 <c>SmallShadowBall.OnSpawnAnmi</c>（P1S.Animations.cs:8-71）。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.OnSpawnAnim, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallOnSpawnAnimState : SmallShadowBallStateBase
    {
        /// <summary>出生动画的子拍。旧 <c>SonState</c> 0~3。</summary>
        private enum Beat
        {
            /// <summary>初始化朝向与初速。</summary>
            Burst,
            /// <summary>向外飘，等本体的影子弹幕追上来（超时也走）。</summary>
            Drift,
            /// <summary>锁扣打开、自身显形。</summary>
            Open,
            /// <summary>锁扣绕一圈收尾。</summary>
            Spin,
        }

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.OnSpawnAnim;

        /// <summary>
        /// 影子弹幕追上来了：跳过飘飞段直接显形。由 <c>ShadowProj</c> 在权威端调用（ShadowProj.cs:103-104），
        /// 拍号随热槽过线，客户端靠收养跟上。
        /// </summary>
        internal void AcceptShadow(SmallShadowBallContext ctx) => SwitchBeat(ctx, (int)Beat.Open);

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            SmallShadowBall ball = ctx.Ball;

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Burst:
                    {
                        Vector2 outward = (ctx.Npc.Center - owner.NPC.Center).SafeNormalize(Vector2.Zero);
                        ball.zDepth = SmallShadowBallDirector.SpawnDepth;
                        ball.LockDistance = SmallShadowBallDirector.SpawnLockDistance;
                        ball.lockRotation = outward.ToRotation();
                        ctx.Npc.velocity = outward * SmallShadowBallDirector.SpawnBurstSpeed;
                        ctx.Npc.scale = SmallShadowBallDirector.HiddenScale;

                        SwitchBeat(ctx, (int)Beat.Drift);
                    }

                    break;

                case Beat.Drift:
                    ctx.Npc.velocity *= SmallShadowBallDirector.SpawnDriftDamp;
                    if (Timer > SmallShadowBallDirector.SpawnDriftTimeout)
                    {
                        SwitchBeat(ctx, (int)Beat.Open);
                    }

                    break;

                case Beat.Open:
                    {
                        float f = Timer / SmallShadowBallDirector.SpawnOpenFrames;

                        ball.lockRotation = ball.lockRotation.AngleLerp(-MathHelper.PiOver2, SmallShadowBallDirector.SpawnOpenRotLerp);
                        ctx.Npc.scale = Helper.SqrtEase(f);
                        ball.LockDistance = Helper.HeavyEase(f) * SmallShadowBallDirector.SpawnOpenLockDistance;

                        if (Timer > SmallShadowBallDirector.SpawnOpenFrames)
                        {
                            SwitchBeat(ctx, (int)Beat.Spin);
                        }
                    }

                    break;

                case Beat.Spin:
                    {
                        float f = Timer / SmallShadowBallDirector.SpawnSpinFrames;

                        ball.lockRotation = -MathHelper.PiOver2 + (Helper.BezierEase(f) * MathHelper.TwoPi);
                        ball.LockDistance = SmallShadowBallDirector.SpawnOpenLockDistance
                            + (Helper.SinEase(f) * SmallShadowBallDirector.SpawnSpinLockSwing);
                    }

                    break;
            }
        }

        protected override IVaultState<SmallShadowBallContext> AuthorityUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            if ((Beat)BeatIndex == Beat.Spin && Timer > SmallShadowBallDirector.SpawnSpinFrames)
            {
                return EndAttack();
            }

            return null;
        }
    }
}
