using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)SmallShadowBallStateId.OnSpawnAnim, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallOnSpawnAnimState : SmallShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>初始化朝向与初速</summary>
            Start,
            /// <summary>向外飘，等本体的影子弹幕追上来</summary>
            Movement,
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
        internal void AcceptShadow(SmallShadowBallContext ctx) 
            => SwitchBeat(ctx, (int)Beat.Open);

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            SmallShadowBall ball = ctx.Ball;

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Start://刚被弹出来
                    {
                        Vector2 outward = (ctx.Npc.Center - owner.NPC.Center).SafeNormalize(Vector2.Zero);
                        ball.zDepth = 1;
                        ball.LockDistance = 4;
                        ball.lockRotation = outward.ToRotation();
                        ctx.Npc.velocity = outward * 12f;
                        ctx.Npc.scale = 0.00001f;

                        SwitchBeat(ctx, (int)Beat.Movement);
                    }
                    break;
                case Beat.Movement://逐渐减速
                    ctx.Npc.velocity *= 0.93f;
                    if (Timer > 60 * 3)
                    {
                        SwitchBeat(ctx, (int)Beat.Open);
                    }

                    break;
                case Beat.Open:
                    {
                        ctx.Npc.velocity *= 0.93f;

                        float f = Timer /30f;

                        ball.lockRotation = ball.lockRotation.AngleLerp(-MathHelper.PiOver2, 0.2f);
                        ctx.Npc.scale = Helper.SqrtEase(f);
                        ball.LockDistance = Helper.HeavyEase(f) * 40;

                        ball.BackLayerAlpha = f;
                        if (ball.BackLayerAlpha > 1)
                            ball.BackLayerAlpha = 1;

                        if (Timer > 30)
                        {
                            ball.CanDrawEffect = true;
                            SwitchBeat(ctx, (int)Beat.Spin);
                        }
                    }
                    break;
                case Beat.Spin:
                    {
                        float f = Timer / 20f;

                        if (++ball.EffectFrameCounter > 1)//特效展开
                        {
                            ball.EffectFrameCounter = 0;
                            if (ball.EffectFrame < 12)
                                ball.EffectFrame++;
                        }

                        //ball.lockRotation = -MathHelper.PiOver2 + (Helper.BezierEase(f) * MathHelper.TwoPi);
                        ball.LockDistance = 40 + (Helper.SinEase(f) * 20);

                        if (Timer>20)
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
                            if (ball.NPC.frame.Y > 0)
                                ball.NPC.frame.Y--;
                        }
                        else if (Timer < 35 + 19)
                        {
                            if (ball.NPC.frame.Y < 16)
                                ball.NPC.frame.Y++;
                        }
                        else
                        {
                            ctx.Npc.frameCounter = -ctx.Ball.selfIndex * 3;
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
