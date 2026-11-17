using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Content.Bosses.VanillaReinforce.EoC;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.P1ToP2Exchange, typeof(ShadowBallContext))]
    public sealed class P1ToP2Exchange : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.P1ToP2Exchange;

        private enum Beat
        {
            /// <summary>
            /// 随便晃一晃和生成水银粒子
            /// </summary>
            Shaking,
            /// <summary>
            /// 生成一些冲击波和转阶段特效
            /// </summary>
            Burst,
            Over
        }

        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            ctx.Npc.TargetClosest();
        }

        public override void OnExit(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.Npc.dontTakeDamage = false;
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.SetVelocityLetItGo();

            switch ((Beat)BeatIndex)
            {
                case Beat.Shaking:
                    {
                        if (Timer==1)//聚焦
                        {
                            if (!VaultUtils.isServer && Vector2.Distance(Main.LocalPlayer.Center, ctx.Npc.Center)<16*80)
                            {
                                Main.instance.CameraModifiers.Add(new FocusModifyer(ctx.Npc.Center, "ShadowBallFocus", 30, 80 + 50));
                            }

                            ctx.Boss.SwitchLockState(ShadowBall.LockStates.Dizziness);
                        }

                        float f = Timer / (30f + 80f);

                        //随机震动
                        ctx.CoreOffsetRadius = Helper.Lerp(ctx.CoreOffsetRadius, 4, 0.1f);
                        if (Timer % 4 == 0)
                            ctx.CoreOffsetAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                        ctx.CoreOffset_AngledTo(0.2f);

                        if (Timer < 80)
                            ctx.Boss.LockDistancePercent = Helper.Lerp(ctx.Boss.LockDistancePercent, 0.6f, Timer / 80f);
                        else
                            ctx.Boss.LockDistancePercent = Helper.Lerp(0.6f, 1.5f, (Timer - 80f) / 30f);

                        ctx.Boss.LayerAlpha = 1 - Helper.SqrtEase(f);

                        if (Timer % 5 == 0)
                        {
                            float r = Main.rand.NextFloat(MathHelper.TwoPi);
                            Vector2 dir = r.ToRotationVector2();
                            ctx.SpawnMucuryDust(ctx.Npc.Center + dir * Main.rand.NextFloat(12, 30), r);
                        }

                        if (Timer > 30 + 80)
                        {
                            ctx.Boss.P2Draw = true;

                            //锁环生成石块
                            foreach (var shadowLock in ctx.Boss.shadowLocks)
                                shadowLock.SpawnGore(ctx.Boss);

                            //生成粒子
                            for (int i = 0; i < 8; i++)
                            {
                                float r = Main.rand.NextFloat(MathHelper.TwoPi);
                                ctx.SpawnMucuryDust(ctx.Npc.Center + r.ToRotationVector2() * Main.rand.NextFloat(12, 30), r);
                            }

                            SwitchBeat(ctx, (int)Beat.Burst);
                        }
                    }
                    break;
                case Beat.Burst:
                    float f2=Timer/45f;
                    ctx.Npc.rotation += Helper.SinEase(f2) * 0.1f;
                    if (Timer<15f)
                    {
                        ctx.CoreOffsetRadius = Helper.Lerp(0, 40, Helper.SqrtEase(Timer / 15f));
                    }
                    else
                    {
                        ctx.CoreOffsetRadius = Helper.Lerp(40, 20, Helper.BezierEase((Timer - 15) / 30f));
                    }

                    ctx.CoreOffset_AngledTo_P2(0.2f);

                    if (Timer > 45)
                    {
                        Vector2 center = ctx.Npc.Center;
                        ctx.Npc.width = ctx.Npc.height = 40;
                        ctx.Npc.Center = center;
                        SwitchBeat(ctx, (int)Beat.Over);
                    }

                    break;
                case Beat.Over:
                    break;
                default:
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                default:
                    return null;
                case Beat.Burst:
                    {
                        if (Timer==0) //生成吼叫特效
                        {
                            Color color = Color.Silver * 0.7f;

                            ctx.Npc.NewProjectileDirectInAI_Server<FightingRoar>(ctx.Npc.Center, Vector2.Zero, 0, 0, ai0: color.R, ai1: color.G, ai2: color.B);
                        }
                    }
                    return null;
                case Beat.Over:
                    return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P2Test);

                    //return ShadowBallHubState.EndAttack(ctx);
            }
        }
    }
}
