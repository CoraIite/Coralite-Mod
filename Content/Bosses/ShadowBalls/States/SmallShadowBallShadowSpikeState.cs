using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 影刺（小球侧）：按索引错开出发，平移到本体一侧的一字阵位，生成预警线后自下而上戳出激光；大师模式多瞄一次再补一发。<br/>
    /// 旧 <c>SmallShadowBall.ShadowSpike</c>（P1S.ShadowSpike.cs:10-126）。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.ShadowSpike, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallShadowSpikeState : SmallShadowBallStateBase
    {
        /// <summary>影刺的子拍。旧 <c>SonState</c> 0~5。</summary>
        private enum Beat
        {
            /// <summary>按索引错开等待出发。</summary>
            Stagger,
            /// <summary>固定时长平移到阵位。</summary>
            LerpIn,
            /// <summary>预备动作 + 预警线。</summary>
            Channel,
            /// <summary>激光发射中。</summary>
            Firing,
            /// <summary>大师模式：再瞄一次。</summary>
            Reaim,
            /// <summary>大师模式：第二发激光发射中。</summary>
            Refiring,
        }

        /// <summary>出发时记下的起点深度与坐标（旧 <c>Recorder</c> / <c>Recorder2</c> / <c>Recorder3</c>）。平移插值两端都要读，进热槽 A/B/C。</summary>
        private float startDepth;
        private Vector2 startCenter;

        /// <summary>第二发激光是否已出膛。只在权威端记账，不过线。</summary>
        private bool secondLaserFired;

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.ShadowSpike;

        public override void OnEnter(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            secondLaserFired = false;
        }

        public override void WriteHot(SmallShadowBallContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = startDepth;
            ctx.Hot[CoraliteBossHotSlots.B] = startCenter.X;
            ctx.Hot[CoraliteBossHotSlots.C] = startCenter.Y;
        }

        public override void ReadHot(SmallShadowBallContext ctx)
        {
            base.ReadHot(ctx);
            startDepth = ctx.Hot[CoraliteBossHotSlots.A];
            startCenter = new Vector2(ctx.Hot[CoraliteBossHotSlots.B], ctx.Hot[CoraliteBossHotSlots.C]);
        }

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            SmallShadowBall ball = ctx.Ball;

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Stagger:
                    if (Timer > ball.selfIndex * SmallShadowBallDirector.SpikeStaggerFrames)
                    {
                        startDepth = ball.zDepth;
                        startCenter = ctx.Npc.Center;
                        SwitchBeat(ctx, (int)Beat.LerpIn);
                    }

                    break;

                case Beat.LerpIn:
                    {
                        int lerpTime = ShadowBallDirector.SpikeBallLerpTime();
                        float f = Helper.BezierEase(Timer / (float)lerpTime);

                        int dir = MathF.Sign(owner.Target.Center.X - owner.NPC.Center.X);
                        Vector2 targetPos = owner.NPC.Center
                            + new Vector2(dir * ShadowBallDirector.SpikePerLength * ball.selfIndex, 0);

                        ctx.Npc.velocity = Vector2.Zero;
                        ctx.Npc.Center = Vector2.Lerp(startCenter, targetPos, f);
                        ball.zDepth = Helper.Lerp(startDepth, 1f, f);

                        if (Timer > lerpTime)
                        {
                            int time = ShadowBallDirector.SpikeBallChannelTime();
                            ball.SpawnAimLine(SmallShadowBallDirector.AimLineLength, time / 3, time / 3 * 2);
                            SwitchBeat(ctx, (int)Beat.Channel);
                        }
                    }

                    break;

                case Beat.Channel:
                    ctx.Npc.velocity *= SmallShadowBallDirector.SpikeChannelDamp;
                    ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp(-MathHelper.PiOver2, SmallShadowBallDirector.SpikeChannelRotLerp);

                    if (Timer > ShadowBallDirector.SpikeBallChannelTime())
                    {
                        SwitchBeat(ctx, (int)Beat.Firing);
                        PlayLaserSound(ctx);
                    }

                    break;

                case Beat.Firing:
                    if (Main.masterMode && Timer > ShadowBallDirector.SpikeBallLaserTime + SmallShadowBallDirector.SpikeLaserTail)
                    {
                        ball.SpawnAimLine(SmallShadowBallDirector.AimLineLength,
                            SmallShadowBallDirector.SpikeReaimLineSpawn, SmallShadowBallDirector.SpikeReaimLineHold);
                        SwitchBeat(ctx, (int)Beat.Reaim);
                    }

                    break;

                case Beat.Reaim:
                    if (Timer < SmallShadowBallDirector.SpikeReaimFrames)
                    {
                        ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp(
                            (owner.Target.Center - ctx.Npc.Center).ToRotation(), SmallShadowBallDirector.SpikeChannelRotLerp);
                    }

                    if (Timer > SmallShadowBallDirector.SpikeRefireFrame)
                    {
                        // 不清 Timer：旧 P1S.ShadowSpike.cs:109 这里写的是 Timer++ 而不是 Timer = 0（疑似笔误），
                        // 于是第二发激光的收招只等约 30 帧而不是完整的 65 帧。按 D10 原样保留这个节奏。
                        BeatIndex = (int)Beat.Refiring;
                        PlayLaserSound(ctx);
                    }

                    break;

                case Beat.Refiring:
                    break;
            }
        }

        protected override IVaultState<SmallShadowBallContext> AuthorityUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            switch ((Beat)BeatIndex)
            {
                // Timer == 0 = 本帧 SharedUpdate 刚换到这一拍。
                case Beat.Firing when Timer == 0:
                    ShootLaser(ctx);
                    return null;

                case Beat.Firing:
                    // 非大师模式没有第二发，打完就收招（大师模式的分支在 SharedUpdate 里换拍）。
                    if (!Main.masterMode && Timer > ShadowBallDirector.SpikeBallLaserTime + SmallShadowBallDirector.SpikeLaserTail)
                    {
                        return EndAttack();
                    }

                    return null;

                case Beat.Refiring:
                    if (!secondLaserFired)
                    {
                        secondLaserFired = true;
                        ShootLaser(ctx);
                    }
                    else if (Timer > ShadowBallDirector.SpikeBallLaserTime + SmallShadowBallDirector.SpikeLaserTail)
                    {
                        return EndAttack();
                    }

                    return null;

                default:
                    return null;
            }
        }

        /// <summary>出膛：激光弹幕挂在自己身上，ai1 是持续帧数。旧 P1S.ShadowSpike.cs:78 / :113。</summary>
        private static void ShootLaser(SmallShadowBallContext ctx)
        {
            ctx.Npc.NewProjectileDirectInAI_Server<SmallLaser>(ctx.Npc.Center, Vector2.Zero,
                SmallShadowBallDirector.SpikeLaserDamage(), 2,
                ai0: ctx.Npc.whoAmI, ai1: ShadowBallDirector.SpikeBallLaserTime);
            ctx.MarkDecision();
        }

        /// <summary>出膛音效，纯本地：只在权威端放的话客户端什么也听不到。</summary>
        private static void PlayLaserSound(SmallShadowBallContext ctx)
        {
            if (!Main.dedServ)
            {
                Helper.PlayPitched("Shadows/ShadowLaser", 0.2f, 0f, ctx.Npc.Center);
            }
        }
    }
}
