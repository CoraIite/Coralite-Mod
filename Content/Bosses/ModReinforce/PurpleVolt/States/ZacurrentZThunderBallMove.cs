using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// Z 电球（紫伏连段件）：电球的红色高速版本——起手只蓄 10 帧、连射间隔 16 帧、组合弹幕散角更大。<br/>
    /// 节拍与 <see cref="ZacurrentElectricBallMove"/> 同构（贴近 → 挥翅 → 连射 → 两轮组合弹幕），
    /// 但每个数字都不同，刻意各写一份，改一招不会波及另一招（D10）。<br/>
    /// 旧 <c>ZacurrentDragon.ZThunderBall</c>（AI.ZThunderBall.cs:16-238）。
    /// </summary>
    internal static class ZacurrentZThunderBallMove
    {
        private enum Beat
        {
            /// <summary>贴近蓄力（旧 SonState 0）</summary>
            Ready = 0,
            /// <summary>挥翅张嘴（旧 SonState 1）</summary>
            Swing = 1,
            /// <summary>连射小球（旧 SonState 2）</summary>
            Burst = 2,
            /// <summary>第一轮组合弹幕（旧 SonState 3）</summary>
            Volley1 = 3,
            /// <summary>第二轮组合弹幕（旧 SonState 4）</summary>
            Volley2 = 4,
        }

        /// <summary>返回 true 表示整招结束。</summary>
        public static bool Run(ZacurrentDragonContext ctx)
        {
            switch ((Beat)(int)ctx.SonState)
            {
                case Beat.Swing:
                    UpdateSwing(ctx);
                    return false;
                case Beat.Burst:
                    UpdateBurst(ctx);
                    return false;
                case Beat.Volley1:
                case Beat.Volley2:
                    return UpdateVolley(ctx);
                default:
                    UpdateReady(ctx);
                    return false;
            }
        }

        private static void UpdateReady(ZacurrentDragonContext ctx)
        {
            ZacurrentStateBase.MouthGatherDust(ctx, ZacurrentDirector.ZBallReadyFrames);
            ZacurrentStateBase.ChaseTargetWhileCharging(ctx);

            ctx.Timer++;
            if (ctx.Npc.frame.Y == 0 && ctx.Timer > ZacurrentDirector.ZBallReadyFrames)
            {
                ctx.SonState = (int)Beat.Swing;
                ctx.Timer = 0;
                ctx.MarkDecision();
            }
        }

        private static void UpdateSwing(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.velocity *= ZacurrentDirector.BallSwingDamp;
            ctx.DeclareDirect();
            npc.QuickSetDirection();
            boss.TurnToNoRot();

            if (npc.frame.Y != ZacurrentDirector.BallSwingFrameY)
            {
                boss.FlyingFrame();
                return;
            }

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.ZBallSwingDelay)
            {
                return;
            }

            ctx.SonState = (int)Beat.Burst;
            ctx.Timer = 0;
            npc.frame.Y = 0;
            boss.OpenMouse = true;
            boss.canDrawShadows = true;
            boss.ResetAllOldCaches();
            ctx.MarkDecision();
        }

        private static void UpdateBurst(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            ZacurrentStateBase.CruiseInBand(ctx);
            boss.UpdateAllOldCaches();
            boss.SetSpriteDirectionFoTarget();
            boss.TurnToNoRot();

            if (ctx.Timer % ZacurrentDirector.ZBallShootInterval == 0)
            {
                if (!VaultUtils.isServer)
                {
                    Helper.PlayPitched(CoraliteSoundID.TeslaTurret_Electric_NPCHit53, npc.Center);
                }

                Vector2 mouth = boss.GetMousePos();
                if (!VaultUtils.isClient)
                {
                    npc.NewProjectileDirectInAI<RedLightningBall>(mouth,
                        (ctx.Target.Center - mouth).SafeNormalize(Vector2.Zero)
                            .RotateByRandom(-ZacurrentDirector.ZBallShootJitter, ZacurrentDirector.ZBallShootJitter) * ZacurrentDirector.ZBallShootSpeed,
                        ZacurrentDirector.ElectricBallShootDamage(), 0, npc.target);
                }

                npc.velocity -= (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.ZBallShootRecoil;
            }

            ctx.Timer++;
            if (ctx.Timer > ZacurrentDirector.ZBallShootInterval)
            {
                boss.FlyingFrame();
            }

            if (ctx.Timer <= ZacurrentDirector.ZBallShootInterval * ZacurrentDirector.ZBallShootRounds)
            {
                return;
            }

            ctx.SonState = (int)Beat.Volley1;
            ctx.Timer = 0;
            ctx.MarkDecision();
        }

        private static bool UpdateVolley(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;

            ZacurrentStateBase.CruiseInBand(ctx);
            boss.FlyingFrame();
            boss.UpdateAllOldCaches();
            boss.SetSpriteDirectionFoTarget();
            boss.TurnToNoRot();

            ctx.Timer++;

            if (ctx.Timer < ZacurrentDirector.ZBallVolleyReady)
            {
                ZacurrentStateBase.MouthGatherDust(ctx, ZacurrentDirector.ZBallVolleyReady);
                return false;
            }

            if (ctx.Timer == ZacurrentDirector.ZBallVolleyReady)
            {
                Fire(ctx);
                return false;
            }

            if (ctx.Timer <= ZacurrentDirector.ZBallVolleyReady + ZacurrentDirector.ZBallVolleyRecover)
            {
                return false;
            }

            if ((Beat)(int)ctx.SonState == Beat.Volley1)
            {
                ctx.SonState = (int)Beat.Volley2;
                ctx.Timer = 0;
                ctx.MarkDecision();
                return false;
            }

            return true;
        }

        private static void Fire(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;
            Vector2 mouth = boss.GetMousePos();

            Vector2 targetPos = ctx.Target.Center;
            if (ctx.Target.velocity.Length() > ZacurrentDirector.BallPredictSpeedGate)
            {
                targetPos += ctx.Target.velocity.SafeNormalize(Vector2.Zero) * ZacurrentDirector.ZBallPredictLead;
            }

            Vector2 dir = (targetPos - mouth).SafeNormalize(Vector2.Zero);
            int damage = ZacurrentDirector.ZBallVolleyDamage();

            if (!VaultUtils.isClient)
            {
                if ((int)ctx.Recorder == 1)
                {
                    //旋转链球
                    npc.NewProjectileDirectInAI<RedChainBall>(mouth, dir * ZacurrentDirector.ZBallChainSpeed, damage, 0, npc.target, 1);
                    for (int i = -1; i < 2; i += 2)
                    {
                        npc.NewProjectileDirectInAI<RedLightningBall>(mouth, dir.RotatedBy(i * ZacurrentDirector.ZBallSpreadInner), damage, 0, npc.target);
                    }
                }
                else
                {
                    //单电球 + 直线链球
                    for (int i = -1; i < 2; i++)
                    {
                        npc.NewProjectileDirectInAI<RedLightningBall>(mouth,
                            dir.RotatedBy(i * ZacurrentDirector.ZBallSpreadOuter) * ZacurrentDirector.ZBallVolleySpeed, damage, 0, npc.target);
                    }

                    npc.NewProjectileDirectInAI<RedChainBall>(mouth, dir * ZacurrentDirector.ZBallChainSpeed, damage, 0, npc.target, 0);
                }
            }

            npc.velocity -= (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.ElectricBallShootRecoil;

            if (!VaultUtils.isServer)
            {
                Helper.PlayPitched("Electric/ElectricShoot", 0.4f, -0.1f, npc.Center);
            }

            ZacurrentStateBase.VolleyImpact(ctx, dir);
        }

        /// <summary>进招初始值：掷出本次的组合弹幕样式（两轮共用）。旧 AI.ZThunderBall.cs:234-238</summary>
        public static void SetStartValue(ZacurrentDragonContext ctx)
            => ctx.Recorder = ctx.AttackRandom.Next(ZacurrentDirector.ZBallStyleCount);
    }
}
