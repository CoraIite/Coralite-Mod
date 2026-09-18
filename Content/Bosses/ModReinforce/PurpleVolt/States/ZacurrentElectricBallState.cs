using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 电球（普通形态单招）：<br/>
    /// Ready —— 边贴近边在嘴前收束紫尘 35 帧，尘收完即出手。<br/>
    /// Swing —— 抬翅到位后 10 帧张嘴。<br/>
    /// Burst —— 每 20 帧一颗直射电球，每发都有后坐把自己推离玩家，整段 4 轮。<br/>
    /// Volley ×2 —— 蓄 40 帧（嘴前再收一次尘做预告）后按 <see cref="ZacurrentDragonContext.Recorder"/> 定下的样式
    /// 打出一组组合弹幕（三重球 / 单球加直链 / 旋转链），出手瞬间镜头冲击 + 背景压暗；两轮用同一样式，玩家学一次就能读两次。<br/>
    /// 预判阀：玩家移动速度超过 4 px/f 时把落点推到身前 340 px——站着不动反而不会被预判。<br/>
    /// 旧 <c>ZacurrentDragon.ElectricBall</c>（AI.ElectricBall.cs:15-256）。
    /// </summary>
    internal static class ZacurrentElectricBallMove
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
            ZacurrentStateBase.MouthGatherDust(ctx, ZacurrentDirector.ElectricBallReadyFrames);
            ZacurrentStateBase.ChaseTargetWhileCharging(ctx);

            ctx.Timer++;
            if (ctx.Npc.frame.Y == 0 && ctx.Timer > ZacurrentDirector.ElectricBallReadyFrames)
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
            if (ctx.Timer <= ZacurrentDirector.ElectricBallSwingDelay)
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

            if (ctx.Timer % ZacurrentDirector.ElectricBallShootInterval == 0)
            {
                if (!VaultUtils.isServer)
                {
                    Helper.PlayPitched(CoraliteSoundID.TeslaTurret_Electric_NPCHit53, npc.Center);
                }

                Vector2 mouth = boss.GetMousePos();
                npc.NewProjectileInAI_Server<PurpleLightningBall>(mouth,
                    (ctx.Target.Center - mouth).SafeNormalize(Vector2.Zero) * ZacurrentDirector.ElectricBallShootSpeed,
                    ZacurrentDirector.ElectricBallShootDamage(), 0, npc.target);

                // 每发后坐把自己推离玩家，连射期间距离自然拉开
                npc.velocity -= (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.ElectricBallShootRecoil;
            }

            ctx.Timer++;
            if (ctx.Timer > ZacurrentDirector.ElectricBallShootInterval)
            {
                boss.FlyingFrame();
            }

            if (ctx.Timer <= ZacurrentDirector.ElectricBallShootInterval * ZacurrentDirector.ElectricBallShootRounds)
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
            NPC npc = ctx.Npc;

            ZacurrentStateBase.CruiseInBand(ctx);
            boss.FlyingFrame();
            boss.UpdateAllOldCaches();
            boss.SetSpriteDirectionFoTarget();
            boss.TurnToNoRot();

            ctx.Timer++;

            if (ctx.Timer < ZacurrentDirector.ElectricBallVolleyReady)
            {
                ZacurrentStateBase.MouthGatherDust(ctx, ZacurrentDirector.ElectricBallVolleyReady);
                return false;
            }

            if (ctx.Timer == ZacurrentDirector.ElectricBallVolleyReady)
            {
                Fire(ctx);
                return false;
            }

            if (ctx.Timer <= ZacurrentDirector.ElectricBallVolleyReady + ZacurrentDirector.ElectricBallVolleyRecover)
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

        /// <summary>出手帧：按样式打出一组组合弹幕，并把自己往后推一下。</summary>
        private static void Fire(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;
            Vector2 mouth = boss.GetMousePos();

            // 预判：玩家在动才提前量，站桩反而不会被预判
            Vector2 targetPos = ctx.Target.Center;
            if (ctx.Target.velocity.Length() > ZacurrentDirector.BallPredictSpeedGate)
            {
                targetPos += ctx.Target.velocity.SafeNormalize(Vector2.Zero) * ZacurrentDirector.ElectricBallPredictLead;
            }

            Vector2 dir = (targetPos - mouth).SafeNormalize(Vector2.Zero);
            int damage = ZacurrentDirector.ElectricBallVolleyDamage();

            if (!VaultUtils.isClient)
            {
                switch ((int)ctx.Recorder)
                {
                    case 1://单电球 + 直线链球
                        SpreadBalls(ctx, mouth, dir, damage, ZacurrentDirector.ElectricBallSpreadOuter, ZacurrentDirector.ElectricBallVolleySpeed);
                        npc.NewProjectileDirectInAI<ZacurrentChainBall>(mouth, dir * ZacurrentDirector.ElectricBallChainSpeed, damage, 0, npc.target, 0);
                        break;
                    case 2://旋转链球
                        npc.NewProjectileDirectInAI<ZacurrentChainBall>(mouth, dir * ZacurrentDirector.ElectricBallChainSpeed, damage, 0, npc.target, 1);
                        SideBalls(ctx, mouth, dir, damage, ZacurrentDirector.ElectricBallSpreadInner);
                        break;
                    default://3 重电球
                        SpreadBalls(ctx, mouth, dir, damage, ZacurrentDirector.ElectricBallSpreadOuter, ZacurrentDirector.ElectricBallVolleySpeed);
                        SideBalls(ctx, mouth, dir, damage, ZacurrentDirector.ElectricBallSpreadInner);
                        break;
                }
            }

            npc.velocity -= (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.ElectricBallShootRecoil;

            if (!VaultUtils.isServer)
            {
                Helper.PlayPitched("Electric/ElectricShoot", 0.4f, -0.1f, npc.Center);
            }

            ZacurrentStateBase.VolleyImpact(ctx, dir);
        }

        /// <summary>正中 + 左右外圈三发。</summary>
        private static void SpreadBalls(ZacurrentDragonContext ctx, Vector2 mouth, Vector2 dir, int damage, float spread, float speed)
        {
            for (int i = -1; i < 2; i++)
            {
                ctx.Npc.NewProjectileDirectInAI<PurpleLightningBall>(mouth, dir.RotatedBy(i * spread) * speed, damage, 0, ctx.Npc.target);
            }
        }

        /// <summary>左右内圈两发（速度取单位向量，慢球做补位）。</summary>
        private static void SideBalls(ZacurrentDragonContext ctx, Vector2 mouth, Vector2 dir, int damage, float spread)
        {
            for (int i = -1; i < 2; i += 2)
            {
                ctx.Npc.NewProjectileDirectInAI<PurpleLightningBall>(mouth, dir.RotatedBy(i * spread), damage, 0, ctx.Npc.target);
            }
        }

        /// <summary>进招初始值：掷出本次的组合弹幕样式（两轮共用）。旧 AI.ElectricBall.cs:250-256</summary>
        public static void SetStartValue(ZacurrentDragonContext ctx)
            => ctx.Recorder = ctx.AttackRandom.Next(ZacurrentDirector.ElectricBallStyleCount);
    }

    /// <summary>电球单招。旧壳 ZacurrentDragon.States.cs:217-222</summary>
    [VaultState((int)ZacurrentDragon.AIStates.ElectricBall, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentElectricBallState : ZacurrentAttackState
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.ElectricBall;

        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ZacurrentElectricBallMove.SetStartValue(ctx);

        protected override bool RunAttack(ZacurrentDragonContext ctx) => ZacurrentElectricBallMove.Run(ctx);
    }
}
