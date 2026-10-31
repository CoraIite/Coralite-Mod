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
    [VaultState((int)ShadowBallStateId.GammaRayBurst, typeof(ShadowBallContext))]
    public sealed class ShadowBallGammaRayBurstState : ShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>0：设置初始值，准备移动</summary>
            Ready,
            /// <summary>1：引力移动，到位后记录距离并编排小球。</summary>
            Move,
            /// <summary>2：准备使用大激光</summary>
            Telegraph,
            /// <summary>3：每 120 帧交替生成环形激光。</summary>
            Firing,
            /// <summary>4：减速 30 帧后收招。</summary>
            Recover,
        }

        private const int WarningInterval = 15;
        private const int WarningCount = 3;
        private const int CycleFrames = 110;

        private int nextWarning;

        public override ShadowBallStateId StateIndex => ShadowBallStateId.GammaRayBurst;

        /// <summary>普通 / 专家 3 轮，大师 4 轮，FTW 5 轮；也是超大激光的持续时间。</summary>
        private static int FiringFrames => Helper.ScaleValueForDiffMode(2, 2, 3, 4) * CycleFrames;

        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            nextWarning = 0;
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready://准备
                    // 朝玩家的横向方向越过玩家 100 像素；X 相同则沿用本帧朝向。
                    int direction = Math.Sign(ctx.Target.Center.X - ctx.Npc.Center.X);
                    if (direction == 0)
                        direction = ctx.Npc.direction;

                    GravityAnchor = ctx.Target.Center + new Vector2(direction * 100f, 0);
                    ctx.GravityMoveReady(GravityAnchor);
                    ctx.SetVelocityLetItGo();
                    SwitchBeat(ctx, (int)Beat.Move);
                    break;

                case Beat.Move:
                    if (ctx.GravityMove(GravityAnchor, Timer))
                    {
                        ctx.Hot[BossSlots.C] = MathHelper.Clamp(Vector2.Distance(ctx.Npc.Center, ctx.Target.Center), 600f, 1000f);
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.AngledRotate);
                        SwitchBeat(ctx, (int)Beat.Telegraph);
                    }
                    break;

                case Beat.Telegraph:
                    ctx.SetVelocitySlowDown(0.95f);
                    // rotation 表示正极方向；两极轴缓动至与“本体→玩家”连线垂直。
                    ctx.SetRotationToward((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.08f);

                    if (Timer >= WarningInterval * WarningCount)
                        SwitchBeat(ctx, (int)Beat.Firing);
                    break;

                case Beat.Firing://生成环形激光
                    ctx.SetRotationToward((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.005f);


                    int tempTimer = Timer % CycleFrames;

                    if (tempTimer == 2)//设置初始速度
                    {
                        float distanceOffset = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) - ctx.Hot[BossSlots.C];
                        if (distanceOffset > 50)
                        {
                            ctx.Npc.velocity = (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero) * Math.Clamp(distanceOffset / 50, -20, 20);
                            ctx.SetVelocityLetItGo();
                        }
                    }

                    ctx.SetVelocitySlowDown(0.97f);

                    // 先换拍再进入权威端，避免在结束帧额外生成一轮弹幕。
                    if (Timer >= FiringFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.Recover);
                    }
                    break;

                case Beat.Recover:
                    ctx.SetVelocitySlowDown(0.95f);
                    break;
            }

            if (!Main.dedServ)
                UpdateAimLines(ctx);
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Telegraph when Timer == 0:
                    ctx.Boss.GetSmallBalls();
                    ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.GammaRayBurst, ctx.Boss.smallBalls.Count);
                    break;

                case Beat.Firing:
                    if (Timer == 0)
                    {
                        for (int side = -1; side <= 1; side += 2)
                        {
                            ctx.Npc.NewProjectileDirectInAI_Server<GammaRayBurstLaser>(PolePosition(ctx.Npc, side),
                                Vector2.Zero, 0, 0, ai0: ctx.Npc.whoAmI, ai1: FiringFrames, ai2: side);
                        }
                    }

                    int tempTimer = Timer % CycleFrames;
                    if (tempTimer == 0)
                    {
                        bool oddCycle = (Timer / CycleFrames) % 2 != 0;
                        int count = oddCycle ? 6 : 5;
                        if (Main.getGoodWorld)
                        {
                            count += 1;
                        }
                        float distance = ctx.Hot[BossSlots.C];
                        float startDistance = distance * (oddCycle ? 0.325f : 0.45f);

                        for (int k = -1; k < 2; k += 2)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                float length = startDistance + i * distance / 4f;
                                // velocity 是激光远端的世界坐标，沿本体 rotation；不是弹幕运动速度。
                                Vector2 endPoint = ctx.Npc.Center + (ctx.Npc.rotation + (k > 0 ? 0 : MathHelper.Pi)).ToRotationVector2() * length;
                                ctx.Npc.NewProjectileDirectInAI_Server<GammaRayBurstRingLaser>(ctx.Npc.Center,
                                    endPoint, SmallShadowBallDirector.RollingLaserDamage(), 0, ai0: ctx.Npc.whoAmI, ai1: i * 8, ai2: k);
                            }
                        }

                        ctx.MarkDecision();
                    }
                    break;

                case Beat.Recover:
                    if (Timer >= 30)
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStateId.P1Test);
                    //return EndAttack(ctx);
                    break;
            }
            return null;
        }

        public override void OnExit(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 正常收招和超时退出都释放小球占位状态，避免小球一直停在空 AI 中。
            if (!VaultUtils.isClient)
            {
                ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.Idle, ctx.Boss.smallBalls.Count,true);
            }

            base.OnExit(machine, ctx);
        }

        /// <summary>两极取本体沿 rotation 正负方向的球面位置。</summary>
        private static Vector2 PolePosition(NPC npc, int side)
            => npc.Center + (npc.rotation+side*MathHelper.PiOver2).ToRotationVector2() * (npc.width * npc.scale / 2f);

        private void UpdateAimLines(ShadowBallContext ctx)
        {
            if ((Beat)BeatIndex == Beat.Telegraph)
            {
                // 补上网络收养刚跳过的预警，但不把已经过期的一轮挤到当前帧。
                while (nextWarning < WarningCount && Timer >= nextWarning * WarningInterval)
                {
                    int age = Timer - nextWarning * WarningInterval;
                    nextWarning++;
                    if (age >= WarningInterval)
                        continue;

                    for (int side = -1; side <= 1; side += 2)
                    {
                        var line = PRTLoader.NewParticle<BigBeamShotParticle>(ctx.Npc.Center, Vector2.Zero, Coralite.ShadowPurple);
                        line.followNpcIndex = ctx.Npc.whoAmI;
                        line.bottomWidth = ctx.Npc.width / 3f;
                        line.targetLength = SmallShadowBallDirector.AimLineLength;
                        line.spawnTime = Math.Min(5, WarningInterval - age);
                        line.contiundTime = WarningInterval - age - line.spawnTime;
                        line.side = side;
                        line.followNpcRot = true;
                        line.Rotation = ctx.Npc.rotation + side * MathHelper.PiOver2;
                    }
                }
            }
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
