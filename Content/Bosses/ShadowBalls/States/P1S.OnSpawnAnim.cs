using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)SmallShadowBallStateId.OnSpawnAnim, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallOnSpawnAnimState : SmallShadowBallStateBase
    {
        /// <summary>出生动画的子拍。旧 <c>SonState</c> 0~3。</summary>
        private enum Beat
        {
            /// <summary>初始化朝向与初速</summary>
            Burst,
            /// <summary>向外飘，等本体的影子弹幕追上来</summary>
            Drift,
            /// <summary>锁扣打开、自身显形</summary>
            Open,
            /// <summary>锁扣绕一圈收尾</summary>
            Spin,
            /// <summary>球层出现</summary>
            ShellSppear,
            /// <summary>结束</summary>
            Over,
        }

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.OnSpawnAnim;

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="ctx"></param>
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

                        ball.BackLayerAlpha = f;
                        if (ball.BackLayerAlpha>1)
                            ball.BackLayerAlpha = 1;

                        if (Timer > SmallShadowBallDirector.SpawnOpenFrames)
                        {
                            ball.CanDrawEffect = true;
                            SwitchBeat(ctx, (int)Beat.Spin);
                        }
                    }

                    break;

                case Beat.Spin:
                    {
                        float f = Timer / 30f;

                        if (++ball.EffectFrameCounter > 2)//特效展开
                        {
                            ball.EffectFrameCounter = 0;
                            if (ball.EffectFrame < 12)
                            {
                                ball.EffectFrame++;
                            }
                        }

                        //ball.lockRotation = -MathHelper.PiOver2 + (Helper.BezierEase(f) * MathHelper.TwoPi);
                        ball.LockDistance = SmallShadowBallDirector.SpawnOpenLockDistance
                            + (Helper.SinEase(f) * 20);

                        if (Timer>30)
                        {
                            ball.DrawTopLayer = true;
                            ball.NPC.frame.Y = SmallShadowBall.MaxFrameY - 1;
                            SwitchBeat(ctx, (int)Beat.ShellSppear);
                        }
                    }

                    break;
                case Beat.ShellSppear:
                    {
                        //球层出现
                        if (Timer < 35)
                        {
                            if ( ball.NPC.frame.Y > 0)
                            {
                                ball.NPC.frame.Y--;
                            }
                        }
                        else if (Timer < 35  + 19)
                        {
                            if ( ball.NPC.frame.Y < 16)
                            {
                                ball.NPC.frame.Y++;
                            }
                        }
                        else
                        {
                            SwitchBeat(ctx, (int)Beat.Over);
                        }
                    }
                    break;
            }
        }

        protected override IVaultState<SmallShadowBallContext> AuthorityUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            if ((Beat)BeatIndex == Beat.Over)
            {
                return EndAttack();
            }

            return null;
        }
    }
}
