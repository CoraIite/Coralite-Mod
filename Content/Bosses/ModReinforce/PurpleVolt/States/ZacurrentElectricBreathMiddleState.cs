using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 电流吐息·中（普通 / 紫伏通用单招，也是多条连段的主力件）：<br/>
    /// Fly —— 飞到玩家上方 250 px（这一段不铺预警，免得和后面的绕飞混淆）。<br/>
    /// Roll —— 垂直切出去绕 45 帧，摆出侧身蓄力姿态，绕飞本身就是"要放大吐息"的信号。<br/>
    /// Charge —— 35 帧蓄力：射线落点每帧朝玩家推进 13 px，但转速随蓄力线性衰减到 0
    /// ——最后半秒角度彻底锁死，玩家在这半秒里走出去就能躲开。<br/>
    /// Breath —— 60 帧粗吐息 + 一颗滚动电球，收尾后按难度歇 20~40 帧（连段可覆盖这个休息值）。<br/>
    /// 旧 <c>ZacurrentDragon.ElectricBreathMiddle</c>（AI.ElectricBreathMiddle.cs:14-210）。
    /// </summary>
    internal static class ZacurrentElectricBreathMiddleMove
    {
        private enum Beat
        {
            /// <summary>飞到玩家上方（旧 SonState 0）</summary>
            Fly = 0,
            /// <summary>绕飞一圈（旧 1）</summary>
            Roll = 1,
            /// <summary>锁定蓄力（旧 2）</summary>
            Charge = 2,
            /// <summary>吐息 + 后摇（旧 3）</summary>
            Breath = 3,
        }

        /// <summary>返回 true 表示整招结束。<paramref name="restTime"/> 为空时按难度取默认后摇。</summary>
        public static bool Run(ZacurrentDragonContext ctx, int? restTime = null)
        {
            switch ((Beat)(int)ctx.SonState)
            {
                case Beat.Roll:
                    UpdateRoll(ctx);
                    return false;
                case Beat.Charge:
                    UpdateCharge(ctx);
                    return false;
                case Beat.Breath:
                    return UpdateBreath(ctx, restTime);
                default:
                    return ZacurrentBreathShared.FlyToSide(ctx, ZacurrentDirector.BreathMiddleFlyFrames, ZacurrentDirector.BreathMiddleHoverY, false);
            }
        }

        private static void UpdateRoll(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (ctx.Timer == 0)
            {
                npc.FaceTarget();
                boss.ResetAllOldCaches();
                boss.IsDashing = true;
                boss.canDrawShadows = true;
                npc.direction = ctx.Target.Center.X > npc.Center.X ? 1 : -1;
                npc.velocity = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero)
                    .RotatedBy(-npc.direction * MathHelper.PiOver2) * ZacurrentDirector.BreathMiddleRollSpeed;
                ctx.MarkDecision();

                if (!VaultUtils.isServer)
                {
                    Helper.PlayPitched("Electric/LightningBeam", 0.4f, 0, npc.Center);
                }
            }

            ctx.Timer++;
            if (ctx.Timer < ZacurrentDirector.BreathMiddleRollFrames)
            {
                boss.UpdateAllOldCaches();
                npc.velocity = npc.velocity.RotatedBy(-npc.spriteDirection * MathHelper.TwoPi / ZacurrentDirector.BreathMiddleRollDivisor);
                npc.rotation = npc.velocity.ToRotation();
                ctx.DeclareDirect();
                return;
            }

            npc.frame.Y = 0;
            npc.velocity *= ZacurrentDirector.BreathMiddleRollExitDamp;
            ctx.DeclareDirect();
            ctx.SonState = (int)Beat.Charge;
            ctx.Timer = 0;
            boss.IsDashing = false;
            boss.currentSurrounding = true;
            boss.OpenMouse = true;
            ctx.Recorder = (ctx.Target.Center - npc.Center).ToRotation();
            npc.FaceTarget();
            boss.TurnToNoRot();
            ctx.MarkDecision();
        }

        private static void UpdateCharge(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            Vector2 holdPos = ctx.Target.Center
                + new Vector2(ctx.Recorder2 > 0 ? -ZacurrentDirector.BreathSideOffset : ZacurrentDirector.BreathSideOffset, ZacurrentDirector.BreathMiddleHoverY);
            boss.GetLengthToTargetPos(holdPos, out float xLength, out float yLength);

            npc.direction = holdPos.X > npc.Center.X ? 1 : -1;
            npc.directionY = holdPos.Y > npc.Center.Y ? 1 : -1;

            if (xLength > ZacurrentDirector.BreathMiddleHoldFarX)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.X, npc.direction, ZacurrentDirector.BreathMiddleHoldSpeedX,
                    ZacurrentDirector.BreathMiddleHoldAccelX, ZacurrentDirector.BreathMiddleHoldTurnX, ZacurrentDirector.BallChaseDamp);
            }
            else if (xLength < ZacurrentDirector.BreathMiddleHoldNearX)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.X, -npc.direction, ZacurrentDirector.BreathMiddleHoldSpeedX,
                    ZacurrentDirector.BreathMiddleHoldAccelX, ZacurrentDirector.BreathMiddleHoldTurnX, ZacurrentDirector.BallChaseDamp);
            }
            else
            {
                npc.velocity.X *= ZacurrentDirector.BreathFlyDampX;
            }

            if (yLength > ZacurrentDirector.BreathFlyDeadZoneY)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.Y, npc.directionY, ZacurrentDirector.BreathMiddleHoldSpeedY,
                    ZacurrentDirector.BreathMiddleHoldAccelY, ZacurrentDirector.BreathMiddleHoldTurnY, ZacurrentDirector.BallChaseDamp);
            }
            else
            {
                npc.velocity.Y *= ZacurrentDirector.BreathFlyDampY;
            }

            ctx.DeclareDirect();
            boss.UpdateAllOldCaches();
            boss.SetSpriteDirectionFoTarget();
            boss.SetRotationNormally();
            ZacurrentBreathShared.MouthDust(ctx);

            // 落点每帧朝玩家推 13 px，转速随蓄力衰减到 0：越到后面越锁死
            Vector2 mouth = boss.GetMousePos();
            Vector2 currentEnd = mouth + (ctx.Recorder.ToRotationVector2() * (ctx.Target.Center - npc.Center).Length());
            float aimRot = (currentEnd.MoveTowards(ctx.Target.Center, ZacurrentDirector.BreathMiddleAimStep) - mouth).ToRotation();
            ctx.Recorder = ctx.Recorder.AngleTowards(aimRot, ZacurrentDirector.BreathMiddleAimRate * (1 - (ctx.Timer / ZacurrentDirector.BreathMiddleChargeFrames)));

            ctx.Timer++;
            if (ctx.Timer % ZacurrentDirector.BreathMiddleSparkInterval == 0)
            {
                ChargeSparks(ctx);

                // 抬头帧图，抬到第 4 帧为止
                if (npc.frame.Y < ZacurrentDirector.BreathMiddleAimFrameY)
                {
                    npc.frame.Y++;
                }
            }

            if (ctx.Timer <= ZacurrentDirector.BreathMiddleChargeFrames)
            {
                return;
            }

            Fire(ctx);
        }

        /// <summary>蓄力期的雷弧：嘴前一条，外加 1/3 概率一条从远处窜回来的长雷。纯本地。</summary>
        private static void ChargeSparks(ZacurrentDragonContext ctx)
        {
            if (VaultUtils.isServer)
            {
                return;
            }

            ZacurrentDragon boss = ctx.Boss;
            ZacurrentBreathShared.MouthDust(ctx);

            PurpleThunderParticle.Spawn(boss.GetMousePos,
                (ctx.Recorder + Main.rand.NextFloat(-ZacurrentDirector.BreathSparkSpread, ZacurrentDirector.BreathSparkSpread)).ToRotationVector2()
                    * Main.rand.NextFloat(ZacurrentDirector.BreathMiddleSparkSpeedMin, ZacurrentDirector.BreathMiddleSparkSpeedMax),
                ZacurrentDirector.BreathMiddleSparkMaxTime, ZacurrentDirector.BreathMiddleSparkFadeTime,
                ZacurrentDirector.BreathMiddleSparkPointCount, ZacurrentDirector.BreathMiddleSparkWidth,
                Main.rand.NextFromList(ZacurrentDragon.ZacurrentPurple, ZacurrentDragon.ZacurrentPink));

            if (!Main.rand.NextBool(ZacurrentDirector.BreathMiddleGroundBoltChance))
            {
                return;
            }

            float speed = Main.rand.NextFloat(ZacurrentDirector.BreathMiddleGroundBoltSpeedMin, ZacurrentDirector.BreathMiddleGroundBoltSpeedMax);
            Vector2 dir = Vector2.UnitY.RotateByRandom(-ZacurrentDirector.BreathMiddleGroundBoltSpread, ZacurrentDirector.BreathMiddleGroundBoltSpread);
            Vector2 offset = -dir * speed * ZacurrentDirector.BreathMiddleGroundBoltDistance;
            PurpleThunderParticle.Spawn(() => ctx.Npc.Center + offset, dir * speed,
                ZacurrentDirector.BreathMiddleGroundBoltMaxTime, ZacurrentDirector.BreathMiddleGroundBoltFadeTime,
                ZacurrentDirector.BreathMiddleGroundBoltPointCount, ZacurrentDirector.BreathMiddleGroundBoltWidth,
                Main.rand.NextFromList(ZacurrentDragon.ZacurrentPurple, ZacurrentDragon.ZacurrentPink));
        }

        private static void Fire(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            ctx.SonState = (int)Beat.Breath;
            ctx.Timer = 0;
            npc.frame.Y = 0;
            ctx.MarkDecision();

            Vector2 dir = ctx.Recorder.ToRotationVector2();
            if (!VaultUtils.isServer)
            {
                PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, dir,
                    ZacurrentDirector.BreathMiddleShakeStrength, ZacurrentDirector.BreathMiddleShakeVibration, ZacurrentDirector.RaidShakeFrames, ZacurrentDirector.ShakeFalloffDistance);
                Main.instance.CameraModifiers.Add(modifier);
                ZacurrentDragon.SetBackgroundLight(ZacurrentDirector.BallSkyLight, ZacurrentDirector.BallSkySeadeFrames, ZacurrentDirector.BallSkyExchange);
            }

            if (!VaultUtils.isClient)
            {
                Vector2 mouth = boss.GetMousePos();
                Vector2 end = mouth + (dir * ((ctx.Target.Center - mouth).Length() + ZacurrentDirector.BreathMiddleLeadDistance));
                int damage = ZacurrentDirector.BreathMiddleDamage();

                npc.NewProjectileDirectInAI<PurpleElectricBreath>(end, mouth, damage, 0, npc.target
                    , ZacurrentDirector.BreathMiddleTime, npc.whoAmI, ZacurrentDirector.BreathProjAi2);
                npc.NewProjectileDirectInAI<PurpleElectricBall>(end, mouth, damage, 0, npc.target
                    , npc.whoAmI, Main.rand.NextFloat(MathHelper.TwoPi), ZacurrentDirector.BreathMiddleBallAi2);
            }

            npc.velocity = -dir * ZacurrentDirector.BreathRecoilSpeed;
            ctx.DeclareDirect();
        }

        private static bool UpdateBreath(ZacurrentDragonContext ctx, int? restTime)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;
            int rest = restTime ?? ZacurrentDirector.BreathMiddleRestFrames();
            int tail = ZacurrentDirector.BreathMiddleTime + ZacurrentDirector.BreathMiddleTailFrames;

            ctx.Timer++;
            if (ctx.Timer < tail)
            {
                npc.velocity *= ZacurrentDirector.BreathSmallHoldDamp;
                ctx.DeclareDirect();
                boss.UpdateAllOldCaches();
                boss.TurnToNoRot();
                return false;
            }

            if (ctx.Timer == tail)
            {
                boss.currentSurrounding = false;
                boss.canDrawShadows = false;
                boss.OpenMouse = false;
                return false;
            }

            if (ctx.Timer < tail + rest)
            {
                boss.FlyingFrame();
                ZacurrentBreathShared.CruiseWhileBreathing(ctx, ZacurrentDirector.BallChaseDamp);
                return false;
            }

            return true;
        }

        /// <summary>进招初始值：掷出绕到哪一侧。旧 AI.ElectricBreathMiddle.cs:212-215</summary>
        public static void SetStartValue(ZacurrentDragonContext ctx)
            => ctx.Recorder2 = ctx.AttackRandom.Next(2);
    }

    /// <summary>中吐息单招。旧壳 ZacurrentDragon.States.cs:210-215</summary>
    [VaultState((int)ZacurrentDragon.AIStates.ElectricBreathMiddle, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentElectricBreathMiddleState : ZacurrentAttackState
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.ElectricBreathMiddle;

        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ZacurrentElectricBreathMiddleMove.SetStartValue(ctx);

        protected override bool RunAttack(ZacurrentDragonContext ctx) => ZacurrentElectricBreathMiddleMove.Run(ctx);
    }
}
