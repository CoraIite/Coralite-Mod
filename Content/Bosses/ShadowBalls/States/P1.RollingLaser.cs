using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 旋转激光：小球按索引交错成三层圈围着本体转，逐层发射激光<br/>
    /// </summary>
    [VaultState((int)ShadowBallStates.RollingLaser, typeof(ShadowBallContext))]
    public sealed class ShadowBallRollingLaserState : ShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>准备</summary>
            Ready,
            /// <summary>引力移动</summary>
            GravityMove,
            /// <summary>分层派活</summary>
            CallSmallBalls,
            /// <summary>吊在玩家头顶等小球打完</summary>
            SmallBallShoot,
            /// <summary>结束</summary>
            Over,
        }

        public override ShadowBallStates StateIndex => ShadowBallStates.RollingLaser;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Ready://准备
                    ctx.GravityAnchor = ctx.Target.Center + ctx.Target.velocity * 20 + new Vector2(0, -350);
                    ctx.GravityMoveReady(ctx.GravityAnchor);

                    ctx.SetVelocityLetItGo();
                    SwitchBeat(ctx, (int)Beat.GravityMove);
                    break;

                case Beat.GravityMove://引力移动
                    if (ctx.GravityMove(ctx.GravityAnchor, Timer))
                    {
                        ctx.Hot[BossSlots.E] = MathHelper.Clamp(Vector2.Distance(ctx.Npc.Center, ctx.Target.Center), 600f, 1000f);
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.AngledRotate);
                        SwitchBeat(ctx, (int)Beat.CallSmallBalls);
                    }
                    break;
                case Beat.CallSmallBalls://控制小球
                    ctx.SetVelocityLetItGo();
                    ctx.Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);

                    if (Timer >= 1)
                        SwitchBeat(ctx, (int)Beat.SmallBallShoot);
                    break;

                case Beat.SmallBallShoot://小球射激光，自己就随便动一动，切换状态在下面
                    int tempTimer = Timer % SmallShadowBallRollingLaserState.PerShotTime;

                    ctx.SetRotationLetItGo();
                    ctx.Npc.rotation += (MathF.Sin(Timer * 0.02f) / 2 + 0.5f) * 0.3f;//sin旋转之随便转一转
                    ctx.Boss.CommonShellBreath(2);

                    int preTime = ShadowBallDirector.RollingLaserChannelTime();

                    if (tempTimer < preTime)
                    {
                        if (tempTimer == 1)
                            ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);

                        ctx.Boss.LockDistancePercent = Helper.Lerp(1, 0.6f, Helper.BezierEase(tempTimer / (float)preTime));
                        ctx.CoreOffset_BackToCenter(0.15f);
                        ctx.Boss.LayerAlpha = 0.5f + 0.5f * (MathF.Cos(tempTimer / (float)preTime * MathHelper.Pi * 5) / 2 + 0.5f);

                        if (tempTimer < preTime * 0.6f)
                        {
                            float currlength = (ctx.Npc.Center - ctx.Target.Center).Length();
                            float currrot = (ctx.Npc.Center - ctx.Target.Center).ToRotation();

                            float targetRot = -MathHelper.PiOver2 + ctx.Hot[BossSlots.E] + MathF.Sin((Timer / SmallShadowBallRollingLaserState.PerShotTime) * MathHelper.PiOver4) * MathHelper.PiOver2;


                            Vector2 targetPos = currrot.AngleLerp(targetRot, 0.1f).ToRotationVector2() * Helper.Lerp(currlength, 350, 0.1f);

                            ctx.SetVelocityApparoch(ctx.Target.Center + targetPos, 2, 10, 0.06f, 0.9f);
                        }
                        else
                            ctx.SetVelocitySlowDown(0.9f);
                    }
                    else if (tempTimer == preTime)
                    {
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);
                    }
                    else
                    {
                        if (tempTimer < preTime + 15)
                            ctx.Boss.LockDistancePercent = Helper.Lerp(0.6f, 2, Helper.SqrtEase((tempTimer - preTime) / 15f));
                        else
                            ctx.Boss.LockDistancePercent = Helper.Lerp(ctx.Boss.LockDistancePercent, 1, 0.15f);

                        ctx.Boss.LayerAlpha = Helper.Lerp(ctx.Boss.LayerAlpha, 1, 0.1f);
                        if (ctx.Boss.LayerAlpha > 1)
                            ctx.Boss.LayerAlpha = 1;

                        if (tempTimer % 30 == 0)
                        {
                            ctx.CoreOffsetAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                            ctx.CoreOffsetRadius = Main.rand.NextFloat(10, 40);
                        }

                        ctx.CoreOffset_AngledTo(0.15f);
                        ctx.SetVelocitySlowDown(0.9f);
                    }

                    break;
                case Beat.Over:
                    {
                        ctx.Boss.LayerAlpha = 1;
                        ctx.CoreOffset_BackToCenter(0.15f); 
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);
                    }
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.CallSmallBalls:
                    if (Timer == 0)//随机大球的跟随位置
                    {
                        ctx.Hot[BossSlots.E]=Main.rand.NextFromList(-0.7f,0,0.7f);
                        ctx.Npc.netUpdate = true; 
                        
                        return CommandRollingLaserLayers(ctx, HowmanyLayers()) ? null : EndAttack(ctx);
                    }
                    break;
                case Beat.SmallBallShoot:
                    if (Timer > 800 || ctx.Boss.CheckSmallBallReady())
                        SwitchBeat(ctx, (int)Beat.Over);

                    break;
                case Beat.Over:
                    if (Timer>1)
                    {
                        //return EndAttack(ctx);
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P1Test);
                    }

                    break;
                default:
                    break;
            }

            return null;

            /// <summary>
            /// 旋转激光的分层派活：小球按索引均分成 <paramref name="layerCount"/> 层，每个小球记下自己在本层的序号、层号与本层总数；
            /// 除不尽剩下的直接标就绪。小球不足层数就返回 false（本体据此收招）。旧 P1.RollingLaser.cs:17-58。
            /// </summary>
            static bool CommandRollingLaserLayers(ShadowBallContext ctx, int layerCount)
            {
                if (VaultUtils.isClient)
                    return true;

                int smallBallCount = ctx.Boss.GetSmallBalls();
                if (smallBallCount < layerCount)
                    return false;

                ctx.Boss.SmallBallStartAttack();

                int perLayer = smallBallCount / layerCount;
                for (int layer = 0; layer < layerCount; layer++)
                    for (int i = 0; i < perLayer; i++)
                    {
                        if (ctx.Boss.smallBalls[i + (layer * perLayer)].ModNPC is SmallShadowBall smallBall)
                        {
                            smallBall.SwitchState(SmallShadowBallStateId.RollingLaser);
                            smallBall.ServerSetRollingLayer(i, layer, perLayer);
                        }
                    }

                for (int i = perLayer * layerCount; i < ctx.Boss.smallBalls.Count; i++)//剩余的小球设置为
                    if (ctx.Boss.smallBalls[i].ModNPC is SmallShadowBall extra)
                        extra.SetReady();

                return true;
            }
        }

        /// <summary>
        /// 小球的层数
        /// </summary>
        /// <returns></returns>
        public static int HowmanyLayers()
            => Helper.ScaleValueForDiffMode(3, 3, 3, 2);
    }
}
