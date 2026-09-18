using Coralite.Content.Bosses.ShadowBalls.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 月食（小球侧）：被吸到本体身前蓄一会，然后顺着本体的朝向高速射出去，沿途留下月相弹幕。<br/>
    /// 旧 <c>SmallShadowBall.LunarEclipse</c>（P1S.LunarEclipse.cs:8-83）。<br/><br/>
    /// <b>半成品说明</b>：两处生成弹幕的地方旧代码就只有 <c>// TODO</c>（月食弹幕与圆环弹幕都还没做），原样保留；
    /// 收招也照旧不由自己决定——本体在月食收尾时统一把所有小球切回待机。这里只加了基类的超时兜底，
    /// 免得本体那边出意外时这颗球永远停在最后一拍（D2）。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.LunarEclipse, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallLunarEclipseState : SmallShadowBallStateBase
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
            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.WaitAndMove:
                    {
                        // 持续朝"本体中心 + 本体朝向 × 100"插值，插值量随时间爬满。
                        float lerpAmount = MathHelper.Min(Timer / SmallShadowBallDirector.EclipseHoldRampFrames, 1f);
                        Vector2 targetCenter = owner.NPC.Center
                            + (owner.NPC.rotation.ToRotationVector2() * SmallShadowBallDirector.EclipseHoldDistance);
                        ctx.Npc.Center = Vector2.Lerp(ctx.Npc.Center, targetCenter, lerpAmount);

                        if (Timer >= SmallShadowBallDirector.EclipseHoldFrames)
                        {
                            ctx.Npc.velocity = owner.NPC.rotation.ToRotationVector2() * SmallShadowBallDirector.EclipseLaunchSpeed;
                            SwitchBeat(ctx, (int)Beat.ShootEclipse);
                        }
                    }

                    break;

                case Beat.ShootEclipse:
                    // TODO（作者原注）：每 5 帧生成一个月食弹幕，还没做。
                    if (Timer >= SmallShadowBallDirector.EclipseSpawnCount * SmallShadowBallDirector.EclipseSpawnInterval)
                    {
                        SwitchBeat(ctx, (int)Beat.SlowDown);
                    }

                    break;

                case Beat.SlowDown:
                    ctx.Npc.velocity *= SmallShadowBallDirector.EclipseSlowDamp;
                    if (Timer >= SmallShadowBallDirector.EclipseSlowFrames)
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
