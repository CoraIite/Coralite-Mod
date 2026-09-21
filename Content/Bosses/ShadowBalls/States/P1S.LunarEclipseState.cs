using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 月食（小球侧）：被吸到本体身前蓄一会，然后顺着本体的朝向高速射出去，沿途留下月相弹幕。<br/>
    /// 旧 <c>SmallShadowBall.LunarEclipse</c>（P1S.LunarEclipse.cs:8-83）。<br/><br/>
    /// 飞行阶段每隔3帧生成月食弹幕，按计时依次传入0~8月相；末段圆环弹幕仍待实现。
    /// 收招也照旧不由自己决定——本体在月食收尾时统一把所有小球切回待机。这里只加了基类的超时兜底，
    /// 免得本体那边出意外时这颗球永远停在最后一拍（D2）。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.LunarEclipse, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallLunarEclipse : SmallShadowBallStateBase
    {
        /// <summary>月食的子拍。旧 <c>SonState</c> 0~3。</summary>
        private enum Beat
        {
            /// <summary>被吸到本体身前蓄力。</summary>
            WaitAndMove,
            /// <summary>射出去，沿途留月相。</summary>
            ShootEclipse,
            /// <summary>减速。</summary>
            SlowDown,
            /// <summary>留圆环弹幕，等本体收尾。</summary>
            ShootRing,
        }

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.LunarEclipse;

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            const float minShootSpeed = 25f * 2 / 3 + 4;
            const float maxShootSpeed = 60f * 2 / 3 + 4;

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.WaitAndMove:
                    {
                        const float EclipseHoldRampFrames = 20;
                        const int EclipseHoldFrames = 30;

                        // 持续朝"本体中心 + 本体朝向 × 100"插值，插值量随时间爬满。
                        float lerpAmount = MathHelper.Min(Timer / EclipseHoldRampFrames, 1f);
                        Vector2 targetCenter = owner.NPC.Center
                            + (owner.NPC.rotation.ToRotationVector2() * SmallShadowBallDirector.EclipseHoldDistance);
                        ctx.Npc.Center = Vector2.Lerp(ctx.Npc.Center, targetCenter, lerpAmount);

                        if (Timer >= EclipseHoldFrames)
                        {
                            ctx.Hot[BossSlots.A] = owner.NPC.rotation;
                            ctx.Npc.velocity = ctx.Hot[BossSlots.A].ToRotationVector2() * minShootSpeed;
                            SwitchBeat(ctx, (int)Beat.ShootEclipse);
                        }
                    }

                    break;

                case Beat.ShootEclipse:
                    const int SpawnTime = 3;
                    const int SpawnCount = 9;
                    const int ShootTime = SpawnCount * SpawnTime;

                    float p = MathHelper.Clamp(Timer / (float)ShootTime, 0, 1);
                    float speed = MathHelper.Lerp(minShootSpeed, maxShootSpeed,
                        MathHelper.Clamp(MathF.Sin(p * MathHelper.Pi), 0, 1));

                    //速度渐变
                    ctx.Npc.velocity = ctx.Hot[BossSlots.A].ToRotationVector2() * speed;

                    if (Timer > 0
                        && Timer <= SpawnCount * SpawnTime
                        && Timer % SpawnTime == 0)
                    {
                        // 第3~27帧依次生成月相0~8，先生成最后一颗再切换子拍。
                        int eclipseType = (Timer / SpawnTime) - 1;
                        ctx.Npc.NewProjectileDirectInAI_Server<EclipseMoon>(ctx.Npc.Center, ctx.Npc.velocity,
                            Helper.GetProjDamage(50, 60, 80), 0, owner.NPC.target, ai0: eclipseType, ai2: (speed-4) * 3 / 2);
                    }

                    if (Timer >= SpawnCount * SpawnTime)
                    {
                        SwitchBeat(ctx, (int)Beat.SlowDown);
                    }

                    break;

                case Beat.SlowDown:
                    ctx.Npc.velocity *= 0.8f;
                    if (Timer >= 120)
                    {
                        SwitchBeat(ctx, (int)Beat.ShootRing);
                    }

                    break;

                case Beat.ShootRing:
                    // TODO（作者原注）：每 30 帧生成一个圆环弹幕，还没做。
                    // 这一拍没有自己的完成路径——本体在月食收尾时会把所有小球切回待机；兜底由基类超时负责。
                    break;
            }
        }
    }
}
