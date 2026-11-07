using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Content.Particles;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>伽玛射线暴：引力越位、两极预警、持续激光与交错环形激光，最后减速收招。</summary>
    [VaultState((int)ShadowBallStates.GammaRayBurst, typeof(ShadowBallContext))]
    public sealed class ShadowBallGammaRayBurstState : ShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>0：设置初始值，准备移动</summary>
            Ready,
            /// <summary>1：引力移动，到位后记录距离并编排小球</summary>
            Move,
            /// <summary>2：准备使用大激光</summary>
            Telegraph,
            /// <summary>3：每 120 帧交替生成环形激光</summary>
            Firing,
            /// <summary>4：减速 30 帧后收招</summary>
            Recover,
        }

        private const int WarnSperateTime = 15;
        private const int WarnCount = 3;
        private const int CycleFrames = 110;

        public override ShadowBallStates StateIndex => ShadowBallStates.GammaRayBurst;

        /// <summary>生成多少轮弹幕攻击</summary>
        private static int FiringFrames
            => Helper.ScaleValueForDiffMode(2, 2, 3, 4) * CycleFrames;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    int direction = Math.Sign(ctx.Target.Center.X - ctx.Npc.Center.X);
                    if (direction == 0)
                        direction = ctx.Npc.direction;

                    ctx.GravityAnchor = ctx.Target.Center + ctx.Target.velocity * 20 + new Vector2(direction * 300f, Main.rand.NextFromList(0, 150, -275));//目标位置随机一下
                    ctx.GravityMoveReady(ctx.GravityAnchor);
                    
                    ctx.SetVelocityLetItGo();
                    SwitchBeat(ctx, (int)Beat.Move);
                    break;

                case Beat.Move:
                    if (ctx.GravityMove(ctx.GravityAnchor, Timer))
                    {
                        ctx.Recorder1 = MathHelper.Clamp(Vector2.Distance(ctx.Npc.Center, ctx.Target.Center), 600f, 1000f);
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.AngledRotate);
                        SwitchBeat(ctx, (int)Beat.Telegraph);
                    }
                    break;

                case Beat.Telegraph://准备动作
                    ctx.SetVelocitySlowDown(0.95f);
                    ctx.SetRotationToward((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.08f);
                    ctx.Boss.ShellBreath_ShowCore(2);

                    if (!VaultUtils.isServer)
                    {
                        if (Timer % WarnSperateTime == 0)
                        {
                            int which = 2 - Timer / WarnSperateTime;
                            for (int side = -1; side <= 1; side += 2)
                            {
                                var line = PRTLoader.NewParticle<BigBeamShotParticle>(ctx.Npc.Center, Vector2.Zero, Coralite.ShadowPurple);
                                line.followNpcIndex = ctx.Npc.whoAmI;
                                line.bottomWidth = ctx.Npc.width / 3f;
                                line.targetLength = SmallShadowBallDirector.AimLineLength;
                                line.spawnTime = 5 + which * 5;
                                line.contiundTime = WarnSperateTime + which * 10 - line.spawnTime;
                                line.side = side;
                                line.followNpcRot = true;
                                line.Rotation = ctx.Npc.rotation + side * MathHelper.PiOver2;
                            }
                        }
                    }

                    if (Timer >= WarnSperateTime * WarnCount)
                    {
                        ctx.CoreOffsetAngle = Main.rand.NextFloat(MathHelper.TwoPi);

                        SwitchBeat(ctx, (int)Beat.Firing);
                    }
                    break;

                case Beat.Firing://生成环形激光
                    ctx.SetRotationToward((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.005f);

                    //核心环形移动，还有一点渐入渐出效果
                    int fadeTime = FiringFrames - 45;
                    if (Timer < fadeTime)
                    {
                        if (Timer <= 60)
                        {
                            float f = Timer / 60f;
                            ctx.CoreOffsetAngle += 0.1f * f;
                            ctx.CoreOffsetRadius = Helper.Lerp(0, 40, Helper.BezierEase(f));
                        }
                        else
                            ctx.CoreOffsetAngle += 0.1f;
                    }
                    else
                    {
                        ctx.CoreOffsetRadius *= 0.95f;
                        float coreAngleF = (Timer - fadeTime) / 45f;
                        ctx.CoreOffsetAngle += 0.1f * Helper.X2Ease(1 - coreAngleF);
                    }

                    ctx.CoreOffset_AngledTo(0.2f);

                    //生成水银环的循环AI
                    {
                        int tempTimer = Timer % CycleFrames;

                        if (tempTimer == 1)//设置初始速度
                        {
                            float distanceOffset = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) - ctx.Recorder1;
                            if (distanceOffset > 50)
                            {
                                ctx.Npc.velocity = (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero) * Math.Clamp(distanceOffset / 50, -20, 20);
                                ctx.SetVelocityLetItGo();
                            }
                        }

                        if (tempTimer<15)//锁环先缩小聚拢再逐渐扩散
                            ctx.Boss.LockDistancePercent = Helper.Lerp(ctx.Boss.LockDistancePercent, 0.6f, 0.2f);
                        else
                            ctx.Boss.LockDistancePercent += (0.4f + 0.7f) / (CycleFrames - 15f);
                    }

                    ctx.SetVelocitySlowDown(0.97f);

                    if (Timer >= FiringFrames)
                    {
                        ctx.Boss.CommonShellBreath(2);
                        SwitchBeat(ctx, (int)Beat.Recover);
                    }

                    break;

                case Beat.Recover:
                    ctx.Boss.LockDistancePercent = Helper.Lerp(ctx.Boss.LockDistancePercent, 1f, 0.2f);

                    ctx.SetVelocitySlowDown(0.95f);
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Telegraph:
                    if (Timer == 0)
                    {
                        ctx.Boss.GetSmallBalls();
                        ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.GammaRayBurst, ctx.Boss.smallBalls.Count);
                    }
                    break;

                case Beat.Firing:
                    if (Timer == 0)
                    {
                        for (int side = -1; side <= 1; side += 2)
                        {
                            ctx.Npc.NewProjectileDirectInAI_Server<GammaRayBurstLaser>(ctx.Npc.Center,
                                Vector2.Zero, 0, 0, ai0: ctx.Npc.whoAmI, ai1: FiringFrames, ai2: side);
                        }
                    }

                    int tempTimer = Timer % CycleFrames;
                    if (tempTimer == 0)
                    {
                        bool oddCycle = (Timer / CycleFrames) % 2 != 0;
                        int count = oddCycle ? 6 : 5;
                        if (Main.getGoodWorld)
                            count += 1;

                        float distance = ctx.Recorder1;
                        float startDistance = distance * (oddCycle ? 0.325f : 0.45f);

                        for (int k = -1; k < 2; k += 2)
                            for (int i = 0; i < count; i++)
                            {
                                float length = startDistance + i * distance / 4f;

                                Vector2 endPoint = ctx.Npc.Center + (ctx.Npc.rotation + (k > 0 ? 0 : MathHelper.Pi)).ToRotationVector2() * length;
                                ctx.Npc.NewProjectileDirectInAI_Server<GammaRayBurstRingLaser>(ctx.Npc.Center,
                                    endPoint, Helper.ScaleValueForDiffMode(25, 35, 45, 55), 0, ai0: ctx.Npc.whoAmI, ai1: i * 8, ai2: k);
                            }

                        ctx.NetUpdate();
                    }
                    break;

                case Beat.Recover:
                    if (Timer >= 26)
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P1Test);
                    //return EndAttack(ctx);
                    break;
            }
            return null;
        }

        public override void OnExit(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            if (!VaultUtils.isClient)
                ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.Idle, ctx.Boss.smallBalls.Count,true);

            base.OnExit(machine, ctx);
        }
    }

    public class BigBeamShotParticle: BeamShotParticle
    {
        public int side;

        public override void SetFollowRotation(NPC npc)
        {
            Rotation = Rotation.AngleLerp(npc.rotation + side*MathHelper.PiOver2, 0.2f);
        }

        public override void SetFollowPosition(NPC npc)
        {
            Position = npc.Center + Rotation.ToRotationVector2() * npc.width / 2;
        }
    }
}
