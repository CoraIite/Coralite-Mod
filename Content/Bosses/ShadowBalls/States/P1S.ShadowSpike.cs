using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)SmallShadowBallStateId.ShadowSpike, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallShadowSpikeState : SmallShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>按索引错开等待出发</summary>
            Waiting,
            /// <summary>固定时长平移到阵位</summary>
            MoveToBottom,
            /// <summary>预备动作 + 预警线</summary>
            Channel,
            /// <summary>激光发射中</summary>
            ShootLaser,
            /// <summary>激光发射中</summary>
            MoveToBallRound,
            /// <summary>大师模式：再瞄一次</summary>
            Channel2,
            /// <summary>大师模式：第二发激光发射中</summary>
            ShootLaser2,
            /// <summary>结束</summary>
            Over,
        }

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.ShadowSpike;

        /// <summary>小球移动的时间</summary>
        public int MoveTime()
            => Helper.ScaleValueForDiffMode(25, 20, 20, 20);

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            SmallShadowBall ball = ctx.Ball;
            const int laserTime = 35;

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Waiting:
                    if (Timer > ball.selfIndex * 5)
                    {
                        ctx.Recorder1 = MathF.Sign(owner.Target.Center.X - owner.NPC.Center.X);
                        ctx.Recorder2 = ctx.Npc.Center.X;
                        ctx.Recorder3 = ctx.Npc.Center.Y;
                        SwitchBeat(ctx, (int)Beat.MoveToBottom);
                    }
                    break;
                case Beat.MoveToBottom://lerp到目标位置
                    {
                        int lerpTime = MoveTime();
                        float f = Helper.BezierEase(Timer / (float)lerpTime);

                        int dir = (int)ctx.Recorder1;
                        Vector2 targetPos = owner.NPC.Center + new Vector2(dir * 16 * 7 * ball.selfIndex, 0);

                        ctx.Npc.velocity = Vector2.Zero;
                        ctx.Npc.Center = Vector2.Lerp(new Vector2(ctx.Recorder2, ctx.Recorder3), targetPos, f);
                        ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp((targetPos - owner.NPC.Center).ToRotation(), 0.15f);
                        ball.zDepth = Helper.Lerp(ball.zDepth, 1f, f);

                        if (Timer > lerpTime)
                        {
                            int time = ctx.LaserChannelTime();
                            ball.SpawnAimLine(SmallShadowBallDirector.AimLineLength, time / 3, time / 3 * 2);
                            SwitchBeat(ctx, (int)Beat.Channel);
                        }
                    }
                    break;
                case Beat.Channel://蓄力
                    ctx.Npc.velocity *= 0.9f;
                    ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp(-MathHelper.PiOver2, 0.1f);

                    if (Timer > ctx.LaserChannelTime())
                    {
                        SwitchBeat(ctx, (int)Beat.ShootLaser);
                        PlayLaserSound(ctx);
                    }
                    break;
                case Beat.ShootLaser://射激光中
                    if (Timer > laserTime + 16)
                    {
                        if (Main.masterMode)
                        {
                            ctx.Recorder2 = ctx.Npc.Center.X;
                            ctx.Recorder3 = ctx.Npc.Center.Y;

                            SwitchBeat(ctx, (int)Beat.MoveToBallRound);
                        }
                        else
                            SwitchBeat(ctx, (int)Beat.Over);
                    }
                    break;
                case Beat.MoveToBallRound://移动到大球环绕位置
                    {
                        int lerpTime = MoveTime();
                        float f = Helper.BezierEase(Timer / (float)lerpTime);

                        Vector2 targetPos = owner.NPC.Center + (-MathHelper.PiOver2+ MathHelper.TwoPi*ball.selfIndex/owner.smallBalls.Count).ToRotationVector2()*120;

                        ctx.Npc.velocity = Vector2.Zero;
                        ctx.Npc.Center = Vector2.Lerp(new Vector2(ctx.Recorder2, ctx.Recorder3), targetPos, f);

                        if (Timer < lerpTime * 2 / 3)
                            ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp((owner.Target.Center - owner.NPC.Center).ToRotation(), 0.15f);
                        ball.zDepth = Helper.Lerp(ball.zDepth, 1f, f);

                        if (Timer > lerpTime)
                        {
                            int time = ctx.LaserChannelTime();
                            ball.SpawnAimLine(SmallShadowBallDirector.AimLineLength, time / 3, time / 3 * 2);
                            SwitchBeat(ctx, (int)Beat.Channel2);
                        }
                    }
                    break;
                case Beat.Channel2:
                    ctx.Npc.velocity *= 0.9f;

                    if (Timer < 5)
                        ctx.Npc.rotation = ctx.Npc.rotation.AngleTowards(
                            (owner.Target.Center - ctx.Npc.Center).ToRotation(), 0.05f);

                    if (Timer > ctx.LaserChannelTime())
                    {
                        SwitchBeat(ctx, (int)Beat.ShootLaser2);
                    }
                    break;
                case Beat.ShootLaser2:
                    if (Timer > laserTime + 16)
                        SwitchBeat(ctx, (int)Beat.Over);
                    break;
                case Beat.Over:
                    break;
            }
        }

        protected override IVaultState<SmallShadowBallContext> AuthorityUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            const int laserTime = 35;

            switch ((Beat)BeatIndex)
            {
                case Beat.ShootLaser:
                case Beat.ShootLaser2:
                    if (Timer == 0)
                        ShootLaser(ctx, laserTime);
                    return null;
                case Beat.Over:
                    ctx.Ball.SetReady();
                    return EndAttack();
                default:
                    return null;
            }
        }

        private static void ShootLaser(SmallShadowBallContext ctx,int laserTime)
        {
            Player target = Main.player[Main.npc[(int)ctx.OwnerIndex].target];

            ctx.Npc.NewProjectileDirectInAI_Server<SmallLaser>(ctx.Npc.Center, Vector2.Zero, SmallShadowBallDirector.SpikeLaserDamage(), 2, ai0: ctx.Npc.whoAmI, ai1:laserTime, ai2: (target.Center - ctx.Npc.Center).Length());
            ctx.NetUpdate();
        }

        private static void PlayLaserSound(SmallShadowBallContext ctx)
        {
            if (!Main.dedServ)
            {
                Helper.PlayPitched("Shadows/ShadowLaser", 0.2f, 0f, ctx.Npc.Center);
            }
        }
    }
}
