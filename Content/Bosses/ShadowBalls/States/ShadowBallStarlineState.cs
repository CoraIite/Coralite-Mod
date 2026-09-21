using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 星轨：靠到玩家头顶后环射一圈星星，星星到位后绕着自身旋转，期间持续补星。<br/>
    /// 旧 <c>ShadowBall.Starline</c>（P1.Starline.cs:12-182）；旧收尾是 <c>SwitchState_Test(OnSpawnAnmi)</c> 的调试死循环，已换回 hub。
    /// </summary>
    [VaultState((int)ShadowBallStateId.Starline, typeof(ShadowBallContext))]
    public sealed class ShadowBallStarlineState : ShadowBallStateBase
    {
        /// <summary>星轨的子拍。旧 <c>SonState</c> 0~4。</summary>
        private enum Beat
        {
            /// <summary>看距离决定先牵引还是直接冲。</summary>
            CheckDistance,
            /// <summary>被裂隙牵引到玩家头顶。</summary>
            GravityMove,
            /// <summary>加速贴近，到位时环射一圈星星。</summary>
            FastApproach,
            /// <summary>持续攻击段，保持记录下来的半径并每 30 帧补一颗星。</summary>
            ContinuousAttack,
            /// <summary>收招后摇。</summary>
            End,
        }

        /// <summary>持续攻击段要保持的半径（旧 <c>Recorder2</c>）。客户端的趋近运动会读它，所以进热槽 C。</summary>
        private float keepRadius;

        public override ShadowBallStateId StateIndex => ShadowBallStateId.Starline;

        public override void WriteHot(ShadowBallContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[BossSlots.C] = keepRadius;
        }

        public override void ReadHot(ShadowBallContext ctx)
        {
            base.ReadHot(ctx);
            keepRadius = ctx.Hot[BossSlots.C];
        }

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 引力移动的落点：玩家头顶上方。沿用旧值 P1.Starline.cs:33（文档写 −250，代码是 −350）。
            const float StarlineHoverHeight = 350f;

            // 持续攻击段：超过记录半径就缓慢靠近，否则减速。沿用旧值 P1.Starline.cs:121,126。
            const float StarlineApproachSpeed = 4f;
            const float StarlineApproachLerp = 0.05f;
            const float StarlineHoldDamp = 0.95f;

            // 收招后摇的时长与速度衰减。沿用旧值 P1.Starline.cs:170,173。
            const float StarlineEndDamp = 0.95f;

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.CheckDistance:
                    ctx.DeclareKeep();
                    if (Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) > ShadowBallDirector.StarlineFarDistance)
                    {
                        GravityAnchor = ctx.Target.Center + new Vector2(0, -StarlineHoverHeight);
                        ctx.GravityMoveReady(GravityAnchor);
                        SwitchBeat(ctx, (int)Beat.GravityMove);
                    }
                    else
                    {
                        SwitchBeat(ctx, (int)Beat.FastApproach);
                    }

                    break;

                case Beat.GravityMove:
                    if (ctx.GravityMove(GravityAnchor, Timer))
                    {
                        SwitchBeat(ctx, (int)Beat.FastApproach);
                    }

                    break;

                case Beat.FastApproach:
                    UpdateFastApproach(ctx);
                    break;

                case Beat.ContinuousAttack:
                    ctx.DeclareApproach(ctx.Target.Center, keepRadius,
                        StarlineApproachSpeed, StarlineApproachLerp,
                        StarlineHoldDamp);
                    break;

                case Beat.End:
                    ctx.DeclareDamp(StarlineEndDamp);
                    break;
            }
        }

        /// <summary>加速贴近；进到门槛内就清速并记下保持半径，环射交给权威端那一帧。旧 P1.Starline.cs:60-110。</summary>
        private void UpdateFastApproach(ShadowBallContext ctx)
        {
            // 快速接近段的速度区间与其距离归一化参数。沿用旧值 P1.Starline.cs:68-69。
            const float StarlineApproachMinSpeed = 10f;
            const float StarlineApproachMaxSpeed = 30f;
            const float StarlineApproachNear = 16 * 10;
            const float StarlineApproachRange = 16 * 30;

            // 快速接近段的速度插值爬坡时长。沿用旧值 P1.Starline.cs:71。
            const float StarlineApproachRampFrames = 80f;

            // 持续攻击段自身保持的最小半径。沿用旧值 P1.Starline.cs:103-106。
            const float StarlineKeepDistanceMin = 16 * 20;

            ctx.DeclareDirect();

            float distance = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center);
            if (distance > ShadowBallDirector.StarlineFarDistance)
            {
                float targetSpeed = Helper.Lerp(StarlineApproachMinSpeed, StarlineApproachMaxSpeed,
                    Helper.Clamp((distance - StarlineApproachNear) / StarlineApproachRange, 0, 1));

                Vector2 direction = (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero);
                ctx.Npc.velocity = Vector2.SmoothStep(ctx.Npc.velocity, direction * targetSpeed,
                    Helper.Clamp(Timer / StarlineApproachRampFrames, 0, 1));
                return;
            }

            ctx.Npc.velocity = Vector2.Zero;

            keepRadius = (int)distance;
            if (keepRadius < StarlineKeepDistanceMin)
            {
                keepRadius = StarlineKeepDistanceMin;
            }

            SwitchBeat(ctx, (int)Beat.ContinuousAttack);
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 持续攻击段的补星间隔与总时长。沿用旧值 P1.Starline.cs:130,148。
            const int StarlineShootInterval = 30;
            const int StarlineShootFrames = 60 * 10;

            // 收招后摇的时长与速度衰减。沿用旧值 P1.Starline.cs:170,173。
            const int StarlineEndFrames = 60;

            switch ((Beat)BeatIndex)
            {
                // Timer == 0 = 本帧 SharedUpdate 刚切到持续攻击段，也就是"到位"的那一帧。
                case Beat.ContinuousAttack when Timer == 0:
                    ShootStarRing(ctx);
                    ctx.MarkDecision();
                    return null;

                case Beat.ContinuousAttack:
                    if (Timer % StarlineShootInterval == 0)
                    {
                        ShootStar(ctx, Helper.NextVec2Dir());
                        ctx.MarkDecision();
                    }

                    if (Timer > StarlineShootFrames)
                    {
                        // 旧代码这里还有一个遍历 ShadowBallStar 的空循环（P1.Starline.cs:153-159，作者留的 TODO：
                        // "设置消失状态的具体逻辑留空，由用户自己实现"）。空循环没搬，TODO 记在这里。
                        SwitchBeat(ctx, (int)Beat.End);
                    }

                    return null;

                case Beat.End:
                    if (Timer > StarlineEndFrames)
                    {
                        return EndAttack(ctx);
                    }

                    return null;

                default:
                    return null;
            }
        }

        /// <summary>到位环射 12 颗星星，每颗的轨道半径在当前距离上随机抖动。旧 P1.Starline.cs:80-99。</summary>
        private static void ShootStarRing(ShadowBallContext ctx)
        {
            // 到位时环射的星星数量与初速。沿用旧值 P1.Starline.cs:86,89。
            const int StarlineRingCount = 12;

            for (int i = 0; i < StarlineRingCount; i++)
            {
                float angle = i * MathHelper.TwoPi / StarlineRingCount;
                ShootStar(ctx, angle.ToRotationVector2());
            }
        }

        /// <summary>
        /// 放一颗星星：ai0 记它该停在离本体多远，ai1 记本体下标（弹幕靠它找主人）。旧 P1.Starline.cs:88-97 / :132-144。
        /// </summary>
        private static void ShootStar(ShadowBallContext ctx, Vector2 offsetDir)
        {
            // 到位时环射的星星数量与初速。沿用旧值 P1.Starline.cs:86,89。
            const float StarlineStarSpeed = 12f;
            const float StarlineStarSpinMin = 0.015f;
            const float StarlineStarSpinMax = 0.025f;

            // 每颗星星记录的轨道半径在当前距离上的随机抖动幅度。沿用旧值 P1.Starline.cs:92。
            const float StarlineStarJitter = 16 * 10;

            float projDistance = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center)
                + Main.rand.NextFloat(-StarlineStarJitter, StarlineStarJitter);

            Vector2 velocity = new Vector2(StarlineStarSpeed,
                Main.rand.NextFloat(StarlineStarSpinMin, StarlineStarSpinMax));

            ctx.Npc.NewProjectileDirectInAI_Server<ShadowBallStar>(ctx.Npc.Center + offsetDir, velocity,
                ShadowBallDirector.StarlineStarDamage(), 0, ai0: projDistance, ai1: ctx.Npc.whoAmI);
        }
    }
}
