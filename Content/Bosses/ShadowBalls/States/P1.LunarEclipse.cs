using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.LunarEclipse, typeof(ShadowBallContext))]
    public sealed class ShadowBallLunarEclipse : ShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>刚开始的特效</summary>
            Ready,
            /// <summary>引力移动</summary>
            GravityMove,
            /// <summary>准备动作</summary>
            LunarAttack,
            /// <summary>间隔阶段，检测是否能够再继续</summary>
            Check,
            /// <summary>后摇</summary>
            Over,
        }

        public override ShadowBallStates StateIndex => ShadowBallStates.LunarEclipse;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Ready://准备阶段
                    const int readyTime = 115;

                    ctx.SetVelocityLetItGo();
                    ctx.Boss.LockDistancePercent = Helper.Lerp(1, 1.5f, Helper.SinEase(Timer / (float)readyTime));
                    ctx.Boss.CommonShellBreath(2);

                    

                    if (Timer == 1 || Timer == 20 || Timer == 40)//生成准备弹幕
                    {
                        if (Timer == 1)
                        {
                            ctx.Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCircles);
                        }
                        ctx.Npc.NewProjectileDirectInAI_Server<EclipseMoon>(ctx.Npc.Center, Vector2.Zero, Helper.GetProjDamage(50, 60, 80), 0, ai0: 4, ai2: 80 + (Timer / 20) * 30);
                    }

                    if (Timer >= readyTime)
                    {
                        //随机角度
                        ctx.Recorder1 = (float)(ctx.Rand().NextFloat(MathHelper.TwoPi));
                        ctx.GravityAnchor = ctx.Target.Center + (Vector2.UnitX * 450);

                        ctx.GravityMoveReady(ctx.GravityAnchor);
                        SwitchBeat(ctx, (int)Beat.GravityMove);
                    }

                    break;
                case Beat.GravityMove://引力移动
                    if (ctx.GravityMove(ctx.GravityAnchor, Timer))
                    {
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);
                        ctx.SpawnAimLine(16 * 30, 15, 15);
                        ctx.Boss.ShellBreath_HideCore(0);

                        SwitchBeat(ctx, (int)Beat.LunarAttack);
                    }

                    break;

                case Beat.LunarAttack:
                    const int LockPreTime = 15;

                    if (Timer < LockPreTime + 15 + 15)
                    {
                        ctx.SetRotationLerp((ctx.Target.Center - ctx.Npc.Center).ToRotation(), Helper.Clamp(Timer / 50f, 0, 1));
                        ctx.CoreOffsetAngle = ctx.Npc.rotation;

                        if (Timer < LockPreTime)
                        {
                            float f = Timer / (float)LockPreTime;
                            ctx.Boss.LockDistancePercent = Helper.Lerp(1, 1.5f, Helper.SqrtEase(f));
                            ctx.CoreOffsetRadius = Helper.Lerp(0, 40, Helper.BezierEase(f));
                            ctx.Boss.LayerAlpha = Helper.Lerp(1, 0.6f, Helper.SqrtEase(f));
                        }
                        else if (Timer == LockPreTime)
                        {
                            ctx.Boss.SwitchLockState(ShadowBall.LockStates.ConcentricCirclesAngled);
                            ctx.Boss.LayerAlpha = 0.7f;
                        }
                        else if (Timer < LockPreTime + 15)
                        {
                            float f = (Timer - LockPreTime) / 15f;
                            ctx.Boss.LockDistancePercent = Helper.Lerp(1.5f, 0.5f, Helper.SqrtEase(f));
                            ctx.Boss.LayerAlpha = Helper.Lerp(0.6f, 1, Helper.X2Ease(f));
                        }
                        else
                        {
                            ctx.Boss.LayerAlpha = 1;
                            ctx.Boss.LockDistancePercent = Helper.Lerp(0.5f, 2, (Timer - LockPreTime - 15) / 15f);
                        }

                        ctx.CoreOffset_AngledTo(0.16f);
                    }
                    else if (Timer == LockPreTime + 15 + 15)
                    {
                        ctx.Boss.SwitchLockState(ShadowBall.LockStates.AngledRotate); 
                        ctx.Boss.ShellBreath_ShowCore(1);
                    }
                    else
                    {
                        ctx.CoreOffset_BackToCenter(0.06f);

                        ctx.Boss.LockDistancePercent = Helper.Lerp(ctx.Boss.LockDistancePercent, 1, 0.1f);
                    }

                    ctx.SetVelocityLetItGo();
                    if (Timer >= 120)
                    {
                        SwitchBeat(ctx, (int)Beat.Check);
                    }

                    break;

                case Beat.Check:
                    ctx.Boss.LayerAlpha = 1;

                    ctx.SetVelocityLetItGo();
                    ctx.Recorder2++;

                    if (ctx.Recorder2 < ShadowBallDirector.EclipseLoopCount())
                    {
                        ctx.Recorder1 += MathHelper.TwoPi / 3f + ctx.Rand().NextFloat(-0.3f, 0.3f);
                        ctx.GravityAnchor = ctx.Target.Center + (ctx.Recorder1.ToRotationVector2() * 450);
                        ctx.GravityMoveReady(ctx.GravityAnchor);
                        SwitchBeat(ctx, (int)Beat.GravityMove);
                    }
                    else
                    {
                        SwitchBeat(ctx, (int)Beat.Over);
                    }

                    break;

                case Beat.Over:
                    ctx.SetVelocityLetItGo();
                    ctx.Boss.CommonShellBreath(2);

                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            const int EclipseEndFrames = 30;

            switch ((Beat)BeatIndex)
            {
                case Beat.LunarAttack:
                    if (Timer == 0)
                    {
                        ctx.Boss.CommandOneIdleSmallBall(SmallShadowBallStateId.LunarEclipse);
                        ctx.NetUpdate();
                    }

                    return null;

                case Beat.Over:
                    if (Timer == 0)
                    {
                        ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.Idle, ctx.Boss.smallBalls.Count);
                        ctx.NetUpdate();
                    }

                    if (Timer >= EclipseEndFrames)
                        return EndAttack(ctx);
                    //return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P1Test);

                    return null;

                default:
                    return null;
            }
        }
    }
}
