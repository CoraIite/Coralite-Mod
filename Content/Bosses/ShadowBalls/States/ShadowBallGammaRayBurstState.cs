using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Content.Particles;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>伽玛射线暴：引力越位、两极预警、持续激光与交错环形激光，最后减速收招。</summary>
    [VaultState((int)ShadowBallStateId.GammaRayBurst, typeof(ShadowBallContext))]
    public sealed class ShadowBallGammaRayBurstState : ShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>0：根据横向位置选定引力锚点。</summary>
            Ready,
            /// <summary>1：引力移动，到位后记录距离并编排小球。</summary>
            Move,
            /// <summary>2：三轮两极预警，同时转向。</summary>
            Telegraph,
            /// <summary>3：每 120 帧交替生成环形激光。</summary>
            Firing,
            /// <summary>4：减速 30 帧后收招。</summary>
            Recover,
        }

        private const int WarningInterval = 15;
        private const int WarningCount = 3;
        private const int CycleFrames = 120;

        /// <summary>到位时的玩家距离，限制在 200～300；状态 2 起复用热槽 A。</summary>
        private float targetDistance;

        // 预警粒子只在本地记账；保存两极符号，使每轮预警随本体的两极一起转动。
        private readonly List<(BeamShotParticle Particle, int Side)> aimLines = new();
        private int nextWarning;

        public override ShadowBallStateId StateIndex => ShadowBallStateId.GammaRayBurst;

        /// <summary>普通 / 专家 3 轮，大师 4 轮，FTW 5 轮；也是超大激光的持续时间。</summary>
        private static int FiringFrames => Helper.ScaleValueForDiffMode(3, 3, 4, 5) * CycleFrames;

        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            targetDistance = 0;
            nextWarning = 0;
            aimLines.Clear();
        }

        public override void WriteHot(ShadowBallContext ctx)
        {
            base.WriteHot(ctx);
            // 状态 0/1 仍由基类用 A/B 同步锚点；到位后不再使用锚点，A 改为距离。
            if (BeatIndex >= (int)Beat.Telegraph)
                ctx.Hot[BossSlots.A] = targetDistance;
        }

        public override void ReadHot(ShadowBallContext ctx)
        {
            base.ReadHot(ctx);
            if (BeatIndex >= (int)Beat.Telegraph)
                targetDistance = ctx.Hot[BossSlots.A];
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ready:
                    // 朝玩家的横向方向越过玩家 100 像素；X 相同则沿用本帧朝向。
                    int direction = Math.Sign(ctx.Target.Center.X - ctx.Npc.Center.X);
                    if (direction == 0)
                        direction = ctx.Npc.direction;
                    GravityAnchor = ctx.Target.Center + new Vector2(direction * 100f, 0);
                    ctx.GravityMoveReady(GravityAnchor);
                    ctx.DeclareDirect();
                    SwitchBeat(ctx, (int)Beat.Move);
                    break;

                case Beat.Move:
                    if (ctx.GravityMove(GravityAnchor, Timer))
                    {
                        targetDistance = MathHelper.Clamp(Vector2.Distance(ctx.Npc.Center, ctx.Target.Center), 200f, 300f);
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.AngledRotate);
                        SwitchBeat(ctx, (int)Beat.Telegraph);
                    }
                    break;

                case Beat.Telegraph:
                    ctx.DeclareDamp(0.95f);
                    // rotation 表示正极方向；两极轴缓动至与“本体→玩家”连线垂直。
                    ctx.DeclareRotation((ctx.Target.Center - ctx.Npc.Center).ToRotation() + MathHelper.PiOver2, 0.08f);
                    if (Timer >= WarningInterval * WarningCount)
                        SwitchBeat(ctx, (int)Beat.Firing);
                    break;

                case Beat.Firing:
                    // 距离误差超过 200 才修正；速度及插值沿用公转招式的缓慢趋近参数。
                    float distanceError = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) - targetDistance;
                    if (MathF.Abs(distanceError) > 200f)
                    {
                        Vector2 directionToPlayer = (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.UnitX);
                        ctx.Npc.velocity = Vector2.Lerp(ctx.Npc.velocity,
                            directionToPlayer * Math.Sign(distanceError) * 4f, 0.04f);
                        ctx.DeclareDirect();
                    }
                    else
                        ctx.DeclareDamp(0.95f);

                    // 先换拍再进入权威端，避免在结束帧额外生成一轮弹幕。
                    if (Timer >= FiringFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.Recover);
                        ctx.DeclareDamp(0.95f);
                    }
                    break;

                case Beat.Recover:
                    ctx.DeclareDamp(0.95f);
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
                    // SharedUpdate 刚换拍的当帧 Timer 为 0，同时生成两极激光和第一轮环形激光。
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
                        float startDistance = targetDistance * (oddCycle ? 3f / 8f : 1f / 2f);
                        for (int i = 0; i < count; i++)
                        {
                            float length = startDistance + i * targetDistance / 4f;
                            // velocity 是激光远端的世界坐标，沿本体 rotation；不是弹幕运动速度。
                            Vector2 endPoint = ctx.Npc.Center + ctx.Npc.rotation.ToRotationVector2() * length;
                            ctx.Npc.NewProjectileDirectInAI_Server<GammaRayBurstRingLaser>(ctx.Npc.Center,
                                endPoint, 0, 0, ai0: ctx.Npc.whoAmI, ai1: targetDistance / 8f);
                        }
                        ctx.MarkDecision();
                    }
                    break;

                case Beat.Recover:
                    if (Timer >= 30)
                        return EndAttack(ctx);
                    break;
            }
            return null;
        }

        public override void OnExit(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 正常收招和超时退出都释放小球占位状态，避免小球一直停在空 AI 中。
            if (!VaultUtils.isClient)
            {
                ctx.Boss.GetSmallBalls();
                foreach (NPC npc in ctx.Boss.smallBalls)
                    if (npc.ModNPC is SmallShadowBall ball
                        && ball.CurrentStateId == (int)SmallShadowBallStateId.GammaRayBurst)
                        ball.SwitchState(SmallShadowBallStateId.Idle);
            }
            aimLines.Clear();
            base.OnExit(machine, ctx);
        }

        /// <summary>两极取本体沿 rotation 正负方向的球面位置。</summary>
        private static Vector2 PolePosition(NPC npc, int side)
            => npc.Center + npc.rotation.ToRotationVector2() * (side * npc.width * npc.scale / 2f);

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
                        var line = PRTLoader.NewParticle<BeamShotParticle>(PolePosition(ctx.Npc, side), Vector2.Zero, Coralite.ShadowPurple);
                        line.followNpcIndex = -1;
                        line.bottomWidth = ctx.Npc.width / 3f;
                        line.targetLength = SmallShadowBallDirector.AimLineLength;
                        line.spawnTime = Math.Min(5, WarningInterval - age);
                        line.contiundTime = WarningInterval - age - line.spawnTime;
                        aimLines.Add((line, side));
                    }
                }
            }

            for (int i = aimLines.Count - 1; i >= 0; i--)
            {
                var (line, side) = aimLines[i];
                if (!line.active)
                {
                    aimLines.RemoveAt(i);
                    continue;
                }
                line.Position = PolePosition(ctx.Npc, side);
                line.Rotation = ctx.Npc.rotation + (side < 0 ? MathHelper.Pi : 0);
            }
        }
    }
}
