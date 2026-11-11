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
                    if (Main.masterMode && Timer > laserTime + 30)
                    {
                        ball.SpawnAimLine(SmallShadowBallDirector.AimLineLength,
                            SmallShadowBallDirector.SpikeReaimLineSpawn, SmallShadowBallDirector.SpikeReaimLineHold);
                        SwitchBeat(ctx, (int)Beat.Channel2);
                    }
                    break;
                case Beat.MoveToBallRound:
                    break;
                case Beat.Channel2:
                    if (Timer < SmallShadowBallDirector.SpikeReaimFrames)
                    {
                        ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp(
                            (owner.Target.Center - ctx.Npc.Center).ToRotation(), 0.1f);
                    }

                    if (Timer > SmallShadowBallDirector.SpikeRefireFrame)
                    {
                        // 不清 Timer：旧 P1S.ShadowSpike.cs:109 这里写的是 Timer++ 而不是 Timer = 0（疑似笔误），
                        // 于是第二发激光的收招只等约 30 帧而不是完整的 65 帧。按 D10 原样保留这个节奏。
                        BeatIndex = (int)Beat.ShootLaser2;
                    }

                    break;
                case Beat.ShootLaser2:
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
                    if (Timer==0)
                        ShootLaser(ctx, laserTime);

                    // 非大师模式没有第二发，打完就收招（大师模式的分支在 SharedUpdate 里换拍）。
                    if (!Main.masterMode && Timer > laserTime + 30)
                    {
                        ctx.Ball.SetReady();

                        return EndAttack();
                    }

                    return null;
                case Beat.ShootLaser2:
                    ShootLaser(ctx, laserTime);
                    if (Timer > laserTime + 30)
                    {
                        ctx.Ball.SetReady();
                        return EndAttack();
                    }
                    return null;
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
