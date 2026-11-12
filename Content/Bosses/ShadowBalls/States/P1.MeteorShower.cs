using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States;

[VaultState((int)ShadowBallStates.MeteorShower, typeof(ShadowBallContext))]
public sealed class ShadowBallMeteorShowerState : ShadowBallStateBase
{
    private enum Beat
    {
        /// <summary>看距离决定先牵引还是直接冲</summary>
        CheckDistance,
        /// <summary>被裂隙牵引到玩家头顶</summary>
        GravityMove,
        /// <summary>加速贴近，到位时环射一圈星星</summary>
        FastApproach,
        /// <summary>群星王冠出现，缓慢移动一下</summary>
        StarCrown,
        /// <summary>带你去看流星雨</summary>
        MeteorShowerAttack,
        /// <summary>收招后摇</summary>
        Over,
    }

    public override ShadowBallStates StateIndex => ShadowBallStates.MeteorShower;

    public static int WarnTime
        => Helper.ScaleValueForDiffMode(90, 85, 80, 150);

    /*
     * recorder1用于记录多少轮攻击
     * recorder2用于记录预判线的弹幕索引
     */

    protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
    {
        switch ((Beat)BeatIndex)
        {
            default:
            case Beat.CheckDistance://检测距离
                ctx.SetVelocityLetItGo();
                ctx.SetRotationLetItGo();

                if (Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) > 16 * 30)
                {
                    ctx.GravityAnchor = ctx.Target.Center + new Vector2(0, -350);
                    ctx.GravityMoveReady(ctx.GravityAnchor);
                    SwitchBeat(ctx, (int)Beat.GravityMove);
                }
                else
                {
                    SwitchBeat(ctx, (int)Beat.FastApproach);
                }

                break;

            case Beat.GravityMove://引力移动
                if (ctx.GravityMove(ctx.GravityAnchor, Timer))
                {
                    SwitchBeat(ctx, (int)Beat.StarCrown);
                }

                break;

            case Beat.FastApproach:
                ctx.SetVelocityLetItGo();
                ctx.SetRotationLetItGo();
                ctx.Boss.LayerAlpha = Helper.Clamp(1 - Helper.SinEase(Timer / 60f) * 0.5f, 0, 1);
                ctx.Boss.ShellBreath_HideCore(2);
                ctx.Boss.LockDistancePercent = Helper.Lerp(ctx.Boss.LockDistancePercent, 0.7f, 0.15f);

                float distance = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center);

                if (Timer <= 1)
                {
                    ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);
                    ctx.Npc.velocity = (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero) * Math.Clamp(distance / 50, -30, 30);
                }
                else
                {
                    ctx.CoreOffset_AngledToVel();

                    ctx.Npc.velocity *= 0.97f;
                }

                if (Timer > 60)
                {
                    switch (ctx.Recorder1)
                    {
                        case 0://初次进入这里
                            SwitchBeat(ctx, (int)Beat.StarCrown);
                            break;
                        case 1://结束攻击
                            ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);
                            SwitchBeat(ctx, (int)Beat.Over);
                            break;
                        default://攻击中
                            ctx.Boss.SwitchLockState(ShadowBall.LockStates.AngledRotate);
                            SwitchBeat(ctx, (int)Beat.MeteorShowerAttack);
                            break;
                    }
                }
                break;

            case Beat.StarCrown:
                ctx.SetVelocityApparoch(ctx.Target.Center, 16 * 30, 4, 0.05f, 0.95f);
                ctx.SetRotationLerp((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.15f);
                ctx.CoreOffset_BackToCenter(0.15f);
                ctx.Boss.ShellBreath_ShowCore(2);

                if (Timer > 30)
                {
                    ctx.Boss.SwitchLockState(ShadowBall.LockStates.AngledRotate);
                    SwitchBeat(ctx, (int)Beat.MeteorShowerAttack);
                }
                break;
            case Beat.MeteorShowerAttack:
                ctx.SetVelocitySlowDown(0.97f);
                ctx.SetRotationLerp((ctx.Target.Center.X > ctx.Npc.Center.X ? -MathHelper.PiOver4 : (MathHelper.Pi + MathHelper.PiOver4)), 0.1f);//🤔
                ctx.CoreOffset_BackToCenter(0.15f);
                ctx.Boss.ShellBreath_ShowCore(1);
                ctx.Boss.LockDistancePercent = Helper.Lerp(ctx.Boss.LockDistancePercent, 1f, 0.15f);

                int overTime;
                if (Main.getGoodWorld)
                    overTime = WarnTime / 2;
                else
                    overTime = WarnTime + 30;

                if (Timer > overTime)
                {
                    ctx.Recorder1--;
                    SwitchBeat(ctx, (int)Beat.FastApproach);
                }

                break;
            case Beat.Over:
                ctx.SetVelocitySlowDown(0.95f);
                ctx.SetRotationLerp((ctx.Target.Center - ctx.Npc.Center).ToRotation(), 0.1f);
                ctx.Boss.LockDistancePercent = Helper.Lerp(ctx.Boss.LockDistancePercent, 1f, 0.2f);

                break;
        }
    }

    protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
    {
        switch ((Beat)BeatIndex)
        {
            case Beat.StarCrown:
                {
                    if (Timer == 0)
                    {
                        ctx.Recorder1 = 1 + Main.rand.Next(3, 5);//随机攻击轮数
                        int totalTime;
                        if (Main.getGoodWorld)
                        {
                            ctx.Recorder1 += 1;
                            totalTime = WarnTime / 2 * (int)ctx.Recorder1 + 60;
                        }
                        else
                            totalTime = (WarnTime + 60 + 30) * (int)(ctx.Recorder1 - 1) + 60;

                        SpawnStarCrown(ctx, totalTime);
                        //控制小球
                        ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.StarsCrown, int.MaxValue);
                    }
                }
                return null;
            case Beat.MeteorShowerAttack:
                if (Timer == 0)
                {
                    float distance = ctx.Rand().NextFromList(100, 120, 140) * 16;

                    Vector2 center = ctx.Target.Center + new Vector2(Main.rand.NextFromList(0, ctx.Target.velocity.X * Main.rand.NextFromList(40, 50, 60)), -distance / 2);
                    Vector2 dir = new Vector2(0, 1);

                    if (Main.getGoodWorld)//ftw中额外瞄准玩家
                    {
                        if (Main.rand.NextBool())
                        {
                            dir = (MathHelper.PiOver2.AngleLerp((ctx.Target.Center - center).ToRotation(), 0.5f)).ToRotationVector2();
                        }
                    }

                    SpawnStarGuider(ctx, center, dir, distance, WarnTime);
                    ctx.NetUpdate();
                }

                return null;

            case Beat.Over:
                if (Timer == 0)
                    ctx.Boss.CommandSmallBalls(SmallShadowBallStateId.Idle, int.MaxValue);

                if (Timer > 60)
                    return EndAttack(ctx);
                //return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P1Test);

                return null;
            default:
                return null;
        }
    }

    public void SpawnStarCrown(ShadowBallContext ctx, int time)
    {
        ctx.Npc.NewProjectileDirectInAI_Server<StarCrown>(ctx.Npc.Center, Vector2.Zero, 0, 0, ai0: ctx.Npc.whoAmI, ai1: time);
    }

    /// <summary>
    /// 生成星辰指路人
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="center"></param>
    /// <param name="length"></param>
    public void SpawnStarGuider(ShadowBallContext ctx, Vector2 center,Vector2 dir, float length, int warnTime)
    {
        Projectile p = ctx.Npc.NewProjectileDirectInAI_Server<StarsGuider>(center, dir, 0, 0, ai0: 1, ai1: length, ai2: warnTime);

        ctx.Recorder2 = p.whoAmI;
    }

    public void SpawnStars(ShadowBallContext ctx, int howmany)
    {
        Projectile p = Main.projectile[(int)ctx.Recorder2];

        if (p.ModProjectile is not StarsGuider starGuider)
            return;

        Vector2 dir = (ctx.Target.Center - p.Center).SafeNormalize(Vector2.Zero);
        for (int i = 0; i < howmany; i++)
        {
            float totalR = StarsGuider.ArcHalfAngle * 2 + Main.rand.NextFloat(-0.3f, 0.3f);

            Vector2 pos = p.Center + dir.RotatedBy(-totalR / 2) * (starGuider.Distance + Main.rand.NextFloat(-StarsGuider.ArcLineWidth * 0.35f, StarsGuider.ArcLineWidth * 0.35f));

            ctx.Npc.NewProjectileDirectInAI_Server<ShadowStar>(pos, Vector2.Zero, Helper.GetProjDamage(60, 65, 100), 0, ai0: 0, ai1: ctx.Recorder2, ai2: totalR / (30 + 70 + 30) * Main.rand.NextFloat(0.9f, 1.4f));
        }
    }
}
