using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Content.Bosses.VanillaReinforce.EoC;
using Coralite.Content.Dusts;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 出场动画：从裂隙里探出发光核心，球壳逐帧退开，休息 60 帧后进入 P1Test 调试入口。<br/>
    /// 旧 <c>ShadowBall.OnSpawnAnmi</c>（Others.Anmations.cs:7-88）；补球与测试招式的选择交给 P1Test。
    /// </summary>
    [VaultState((int)ShadowBallStateId.OnSpawnAnim, typeof(ShadowBallContext))]
    public sealed class ShadowBallOnSpawnAnimState : ShadowBallStateBase
    {
        /// <summary>出场动画的子拍。两拍都由整段累计 <c>Timer</c> 驱动，所以换拍时<b>不</b>清 Timer（退出判定读的是同一个 Timer）。</summary>
        private enum Beat
        {
            /// <summary>核心出现</summary>
            CoreAppear,
            /// <summary>球壳逐帧退开、外层逐渐显形。</summary>
            RaiseShell,
            /// <summary>锁扣出现</summary>
            LockAppear,
            /// <summary>吼叫</summary>
            Roar,
            /// <summary>球壳停在固定帧。</summary>
            Rest,
        }

        public override ShadowBallStateId StateIndex => ShadowBallStateId.OnSpawnAnim;

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 出生动画每帧速度衰减。沿用旧值 Others.Anmations.cs:11。
            const float SpawnDamp = 0.95f;

            // 球壳退完后停在的帧号。沿用旧值 Others.Anmations.cs:83。
            const int SpawnShellRestFrame = 30;

            ShadowBall boss = ctx.Boss;

            boss.MaskAlpha = 0f;
            boss.LockDistancePercent = 1f;
            ctx.SetVelocitySlowDown(SpawnDamp);

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.CoreAppear:

                    if (Timer==2)//初始化随机的内啥，核心效果
                    {
                        ctx.Hot[0] = Main.rand.NextFloat(MathHelper.TwoPi);
                        ctx.Hot[1] = 40;
                    }

                    if (Timer < SpawnProj.SplitExpandTime)
                    {
                        break;
                    }
                    else if (Timer < SpawnProj.SplitExpandTime + SpawnProj.ShadowBallAppearTime)
                    {
                        float f = (float)(Timer - SpawnProj.SplitExpandTime) / SpawnProj.ShadowBallAppearTime;
                        f = Helper.HeavyEase(f);
                        boss.CoreScale = boss.CoreAlpha = boss.LightStrength = f;
                        boss.CoreOffset = ctx.Hot[BossSlots.C].ToRotationVector2() * ctx.Hot[BossSlots.D];

                        //if (boss.CoreOffset.X is float.NaN)
                        //{
                        //    int a = 1;
                        //}
                        ctx.Hot[BossSlots.C] += 0.4f * (1 - Helper.X2Ease( f));
                        ctx.Hot[BossSlots.D] = Helper.Lerp(40, 0, f);
                        ctx.Npc.rotation += 0.4f * (1 - Helper.X2Ease(f));
                    }
                    else
                    {
                        ctx.Hot[BossSlots.C] = 0;
                        ctx.Hot[BossSlots.D] = 0;

                        SwitchBeat(ctx, (int)Beat.RaiseShell);
                    }
                    break;
                case Beat.RaiseShell:
                    if (Timer < SpawnProj.ShadowProjSpawnTime)
                    {
                        boss.LayerAlpha = Timer / (float)SpawnProj.ShadowProjSpawnTime;

                        if (Timer > 60 && Timer % 2 == 0 && boss.ShellFrame > 0)
                        {
                            boss.ShellFrame--;
                        }
                    }
                    else
                    {
                        boss.LockDistancePercent = 2;

                        SwitchBeat(ctx, (int)Beat.LockAppear);
                    }

                    break;
                case Beat.LockAppear:
                    if (Timer % 2 == 0 && boss.ShellFrame < SpawnShellRestFrame)
                    {
                        boss.ShellFrame++;
                    }
                    if (Timer < SpawnProj.LockAppearTime)
                    {
                        int t = Timer - 2;
                        boss.LockDistancePercent = 3 - 2 * Helper.X2Ease((float)Timer / SpawnProj.LockAppearTime);

                        if (boss.shadowLocks.IndexInRange(t))
                        {
                            boss.shadowLocks[t].Init = false;
                        }
                    }
                    else
                    {
                        SwitchBeat(ctx, (int)Beat.Roar);
                        boss.SwitchLockState(ShadowBall.LockStates.Normal);
                    }

                    break;
                case Beat.Roar:
                    if (Timer < (int)(SpawnProj.RoarTime * 0.25f))
                    {
                        boss.LockDistancePercent = 1 - 0.5f * Helper.X2Ease(Timer / (SpawnProj.RoarTime * 0.25f));
                    }
                    else if (Timer == (int)(SpawnProj.RoarTime * 0.25f))
                    {
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(boss.NPC.Center, Main.rand.NextVector2Unit(), 5f, 10f, 60));
                    }
                    else
                    {
                        if (Timer < SpawnProj.RoarTime * 0.5f)
                        {
                            boss.LockDistancePercent = 0.5f + 2f * Helper.SqrtEase((Timer - (SpawnProj.RoarTime * 0.25f)) / (SpawnProj.RoarTime * (0.5f - 0.25f)));
                        }
                        else
                        {
                            boss.LockDistancePercent = 2.5f - 1.5f * Helper.X2Ease((Timer - (SpawnProj.RoarTime * 0.5f)) /( SpawnProj.RoarTime * (1 - 0.5f)));
                        }
                        if (Timer < SpawnProj.RoarTime * 0.8f)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 Vel = Vector2.One.RotateRandom(MathHelper.Pi) * Main.rand.NextFloat(60f);
                                if (Main.rand.NextBool(4))
                                    Vel *= 1.5f;
                                else if (Main.rand.NextBool(8))
                                    Vel *= 2f;
                                int dust = Dust.NewDust(boss.NPC.Center, 0, 0, ModContent.DustType<GlowDust_Line>(), 0, 0, 100, Coralite.ShadowPurple * 0.7f, 1.5f);
                                Main.dust[dust].position = boss.NPC.Center;
                                Main.dust[dust].velocity = Vel;
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].scale *= 1f;
                            }
                        }

                        if (Timer>SpawnProj.RoarTime)
                        {
                            boss.LockDistancePercent = 1f;
                            SwitchBeat(ctx, (int)Beat.Rest);
                        }
                    }
                    break;
                case Beat.Rest:
                    boss.UseBreath = true;
                    boss.ShellFrame = SpawnShellRestFrame;
                    break;
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 出生动画放完、可以进入轮换的帧数。
            const int SpawnExitFrames = 60;

            switch ((Beat)BeatIndex)
            {
                case Beat.CoreAppear:
                    if (Timer == 2)
                    {
                        ctx.Boss.NPC.NewProjectileDirectInAI<ShadowBall_NameLine>(ctx.Boss.NPC.Center, Vector2.Zero, 0, 0);
                    }
                    break;
                case Beat.RaiseShell:
                    break;
                case Beat.LockAppear:
                    break;
                case Beat.Roar:
                    if (Timer == (int)(SpawnProj.RoarTime * 0.25f)
                        || Timer == (int)(SpawnProj.RoarTime * 0.45f)
                        || Timer == (int)(SpawnProj.RoarTime * 0.65f))
                    {
                        ctx.Boss.NPC.NewProjectileDirectInAI<FightingRoar>(ctx.Boss.NPC.Center, Vector2.Zero, 0, 0,
                            ai0: Color.Gray.R, ai1: Color.Gray.G, ai2: Color.Gray.B);
                    }
                    break;
                case Beat.Rest:
                    if (Timer > SpawnExitFrames)
                    {
                        return ShadowBallHubState.CommitTest(ctx, ShadowBallStateId.P1Test);
                    }

                    break;
                default:
                    break;
            }

            return null;
        }
    }
}
