using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>两种电流吐息共用的小件：侧翼就位飞行与嘴部预警尘。旧 AI.ElectricBreathSmall.cs:112-216</summary>
    internal static class ZacurrentBreathShared
    {
        /// <summary>
        /// 侧翼就位：飞到玩家左 / 右 400 px、指定高度处，同时把吐息角慢慢转向玩家。
        /// <paramref name="targetY"/> 为相对玩家的高度偏移，<paramref name="mouthDust"/> 决定进入 750 px 后是否张嘴铺预警。
        /// 返回 true 表示"追不上，整招放弃"。旧 AI.ElectricBreathSmall.cs:112-182
        /// </summary>
        public static bool FlyToSide(ZacurrentDragonContext ctx, int maxTime, float targetY, bool mouthDust = true)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (ctx.Timer == 0)
            {
                ctx.Recorder = (ctx.Target.Center - npc.Center).ToRotation();
            }

            ctx.Recorder = ctx.Recorder.AngleTowards((ctx.Target.Center - npc.Center).ToRotation(), ZacurrentDirector.BreathAimRate);

            Vector2 targetPos = ctx.Target.Center + new Vector2(ctx.Recorder2 > 0 ? -ZacurrentDirector.BreathSideOffset : ZacurrentDirector.BreathSideOffset, targetY);
            boss.GetLengthToTargetPos(targetPos, out float xLength, out float yLength);

            npc.direction = targetPos.X > npc.Center.X ? 1 : -1;
            npc.directionY = targetPos.Y > npc.Center.Y ? 1 : -1;
            boss.SetSpriteDirectionFoTarget();
            boss.SetRotationNormally();

            if (xLength > ZacurrentDirector.BreathFlyFarX)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.X, npc.direction, ZacurrentDirector.BreathFlySpeedX,
                    ZacurrentDirector.BreathFlyAccelX, ZacurrentDirector.BreathFlyTurnX, ZacurrentDirector.BallChaseDamp);
            }
            else if (xLength < ZacurrentDirector.BreathFlyNearX)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.X, -npc.direction, ZacurrentDirector.BreathFlySpeedX,
                    ZacurrentDirector.BreathFlyAccelX, ZacurrentDirector.BreathFlyTurnX, ZacurrentDirector.BallChaseDamp);
            }
            else
            {
                npc.velocity.X *= ZacurrentDirector.BreathFlyDampX;
                ctx.Timer += ZacurrentDirector.BreathCloseTimeBonus;
            }

            if (npc.directionY < 0)
            {
                boss.FlyingUp(ZacurrentDirector.BreathFlyRiseAccel, ZacurrentDirector.BreathFlyRiseMax, ZacurrentDirector.BreathFlyRiseDamp);
            }
            else if (yLength > ZacurrentDirector.BreathFlyDeadZoneY)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.Y, npc.directionY, ZacurrentDirector.BreathFlySpeedY,
                    ZacurrentDirector.BreathFlyAccelY, ZacurrentDirector.BreathFlyTurnY, ZacurrentDirector.BallChaseDamp);
                boss.FlyingFrame();
            }
            else
            {
                npc.velocity.Y *= ZacurrentDirector.BreathFlyDampY;
                boss.FlyingFrame();
            }

            ctx.DeclareDirect();

            if (mouthDust && npc.Distance(ctx.Target.Center) < ZacurrentDirector.BreathMouthDustDistance)
            {
                boss.OpenMouse = true;
                MouthDust(ctx);

                if (!VaultUtils.isServer && ctx.Timer % ZacurrentDirector.BreathIndicatorInterval == 0 && Main.rand.NextBool(ZacurrentDirector.BreathIndicatorChance))
                {
                    // 落点指示：粒子始终追着射线终点，玩家看粒子就知道这一发往哪打
                    ElectricParticle_PurpleFollow.Spawn(BreathEnd(ctx), Main.rand.NextVector2CircularEdge(ZacurrentDirector.BreathIndicatorRadius, ZacurrentDirector.BreathIndicatorRadius)
                        , () => BreathEnd(ctx));
                }
            }
            else
            {
                boss.OpenMouse = false;
            }

            ctx.Timer++;
            if (ctx.Timer <= maxTime)
            {
                return false;
            }

            // 追不上就别演：直接放弃整招
            if (npc.Distance(ctx.Target.Center) > ZacurrentDirector.BreathGiveUpDistance)
            {
                return true;
            }

            ctx.SonState++;
            ctx.Timer = 0;
            boss.OpenMouse = true;
            ctx.MarkDecision();
            return false;
        }

        /// <summary>当前吐息角下的射线终点（玩家距离 + 延伸量）。</summary>
        public static Vector2 BreathEnd(ZacurrentDragonContext ctx)
        {
            Vector2 mouth = ctx.Boss.GetMousePos();
            return mouth + (ctx.Recorder.ToRotationVector2() * ((ctx.Target.Center - mouth).Length() + ZacurrentDirector.BreathLeadDistance));
        }

        /// <summary>嘴部与落点两头的预警尘。纯本地。旧 AI.ElectricBreathSmall.cs:187-216</summary>
        public static void MouthDust(ZacurrentDragonContext ctx)
        {
            if (VaultUtils.isServer)
            {
                return;
            }

            Vector2 mouth = ctx.Boss.GetMousePos();
            Vector2 dir = ctx.Recorder.ToRotationVector2();

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustPerfect(mouth + (dir * Main.rand.NextFloat(ZacurrentDirector.BreathDustRangeMin, ZacurrentDirector.BreathDustRangeMax)), DustID.PortalBoltTrail
                    , dir.RotateByRandom(-ZacurrentDirector.CannonDustJitter, ZacurrentDirector.CannonDustJitter) * Main.rand.NextFloat(ZacurrentDirector.CannonDustSpeedMin, ZacurrentDirector.CannonDustSpeedMax)
                    , newColor: ZacurrentDragon.ZacurrentDustPurple
                    , Scale: Main.rand.NextFloat(ZacurrentDirector.GravitationDustScaleMin, ZacurrentDirector.GravitationDustScaleMax));
                dust.noGravity = true;
            }

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustPerfect(BreathEnd(ctx) + Main.rand.NextVector2Circular(ZacurrentDirector.BreathDustTargetSpread, ZacurrentDirector.BreathDustTargetSpread), DustID.PortalBoltTrail
                    , -dir.RotateByRandom(-ZacurrentDirector.CannonDustJitter, ZacurrentDirector.CannonDustJitter) * Main.rand.NextFloat(ZacurrentDirector.CannonDustSpeedMin, ZacurrentDirector.CannonDustSpeedMax)
                    , newColor: ZacurrentDragon.ZacurrentDustPurple
                    , Scale: Main.rand.NextFloat(ZacurrentDirector.GravitationDustScaleMin, ZacurrentDirector.GravitationDustScaleMax));
                dust.noGravity = true;
            }
        }

        /// <summary>吐息期的距离带巡航（中间带衰减系数与通用巡航不同，单列一份）。旧 AI.ElectricBreathSmall.cs:33-45</summary>
        public static void CruiseWhileBreathing(ZacurrentDragonContext ctx, float middleDamp)
        {
            NPC npc = ctx.Npc;
            float distance = npc.Center.Distance(ctx.Target.Center);

            if (distance < ZacurrentDirector.BallFlyNearDistance)
            {
                if (npc.velocity.Length() < ZacurrentDirector.BallFlyNearSpeedCap)
                {
                    npc.velocity += (npc.Center - ctx.Target.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.BallFlyAccel;
                }
            }
            else if (distance > ZacurrentDirector.BallFlyFarDistance)
            {
                if (npc.velocity.Length() < ZacurrentDirector.BallFlyFarSpeedCap)
                {
                    npc.velocity += (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.BallFlyAccel;
                }
            }
            else
            {
                npc.velocity *= middleDamp;
            }

            ctx.DeclareDirect();
        }
    }

    /// <summary>
    /// 电流吐息·小（普通形态单招）：左右各来一发的短吐息。<br/>
    /// Fly —— 沿一条弧线（最高 200 px）绕到玩家侧翼，进入 750 px 就张嘴铺预警尘与落点指示粒子。<br/>
    /// Breath —— 准备 26~35 帧（难度越高越短）后吐出，吐息期瞄准速率只有 0.005 rad/f，基本是"吐出去就定了"。<br/>
    /// 吐完把 <see cref="ZacurrentDragonContext.Recorder2"/> 取反，换到另一侧再来一轮，两轮后收招。<br/>
    /// 旧 <c>ZacurrentDragon.ElectricBreathSmall</c>（AI.ElectricBreathSmall.cs:13-110）。
    /// </summary>
    internal static class ZacurrentElectricBreathSmallMove
    {
        private enum Beat
        {
            /// <summary>第一轮就位（旧 SonState 0）</summary>
            Fly1 = 0,
            /// <summary>第一轮吐息（旧 1）</summary>
            Breath1 = 1,
            /// <summary>第二轮就位（旧 2）</summary>
            Fly2 = 2,
            /// <summary>第二轮吐息（旧 3）</summary>
            Breath2 = 3,
        }

        /// <summary>返回 true 表示整招结束。</summary>
        public static bool Run(ZacurrentDragonContext ctx)
        {
            Beat beat = (Beat)(int)ctx.SonState;
            if (beat is Beat.Breath1 or Beat.Breath2)
            {
                return UpdateBreath(ctx);
            }

            // 弧线就位：高度按 sin(sqrt(t)) 走，起手绕高、临近压平
            float arcY = -MathF.Sin(MathF.Sqrt(ctx.Timer / ZacurrentDirector.BreathSmallFlyFrames) * MathHelper.Pi) * ZacurrentDirector.BreathSmallArcHeight;
            return ZacurrentBreathShared.FlyToSide(ctx, ZacurrentDirector.BreathSmallFlyFrames, arcY);
        }

        private static bool UpdateBreath(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.SetRotationNormally();
            ctx.Recorder = ctx.Recorder.AngleTowards((ctx.Target.Center - npc.Center).ToRotation(), ZacurrentDirector.BreathSmallTrackRate);
            ZacurrentBreathShared.CruiseWhileBreathing(ctx, ZacurrentDirector.BreathSmallBandDamp);

            int readyTime = ZacurrentDirector.BreathSmallReadyFrames();
            ctx.Timer++;

            if (ctx.Timer == 1)
            {
                if (!VaultUtils.isServer)
                {
                    Helper.PlayPitched(CoraliteSoundID.TeslaTurret_Electric_NPCHit53, npc.Center);
                }

                return false;
            }

            if (ctx.Timer < readyTime)
            {
                ZacurrentBreathShared.MouthDust(ctx);
                boss.FlyingFrame();

                if (!VaultUtils.isServer && ctx.Timer % ZacurrentDirector.BreathSmallSparkInterval == 0)
                {
                    PurpleThunderParticle.Spawn(boss.GetMousePos,
                        (ctx.Recorder + Main.rand.NextFloat(-ZacurrentDirector.BreathSparkSpread, ZacurrentDirector.BreathSparkSpread)).ToRotationVector2()
                            * Main.rand.NextFloat(ZacurrentDirector.BreathSmallSparkSpeedMin, ZacurrentDirector.BreathSmallSparkSpeedMax),
                        ZacurrentDirector.BreathSmallSparkMaxTime, ZacurrentDirector.BreathSmallSparkFadeTime,
                        ZacurrentDirector.BreathSmallSparkPointCount, ZacurrentDirector.BreathSmallSparkWidth,
                        Main.rand.NextFromList(ZacurrentDragon.ZacurrentPurple, ZacurrentDragon.ZacurrentPink));
                }

                return false;
            }

            if (ctx.Timer == readyTime)
            {
                Fire(ctx);
                return false;
            }

            if (ctx.Timer < ZacurrentDirector.BreathSmallTime + readyTime + ZacurrentDirector.BreathSmallTailFrames)
            {
                npc.velocity *= ZacurrentDirector.BreathSmallHoldDamp;
                ctx.DeclareDirect();
                boss.FlyingFrame();
                return false;
            }

            boss.currentSurrounding = false;
            boss.OpenMouse = false;

            // 换一侧再来一轮，两轮打完收招
            ctx.Recorder2 = -ctx.Recorder2;
            if (ctx.SonState > (int)Beat.Fly2)
            {
                return true;
            }

            ctx.Timer = 0;
            ctx.SonState++;
            ctx.MarkDecision();
            return false;
        }

        private static void Fire(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.currentSurrounding = true;
            Vector2 dir = ctx.Recorder.ToRotationVector2();

            if (!VaultUtils.isServer)
            {
                boss.ElectricSound();
                SoundEngine.PlaySound(CoraliteSoundID.BubbleShield_Electric_NPCHit43, npc.Center);

                PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, dir,
                    ZacurrentDirector.BreathSmallShakeStrength, ZacurrentDirector.BreathSmallShakeVibration, ZacurrentDirector.RaidShakeFrames, ZacurrentDirector.ShakeFalloffDistance);
                Main.instance.CameraModifiers.Add(modifier);
                ZacurrentDragon.SetBackgroundLight(ZacurrentDirector.BreathSmallSkyLight, ZacurrentDirector.BallSkySeadeFrames, ZacurrentDirector.BallSkyExchange);
            }

            if (!VaultUtils.isClient)
            {
                Vector2 mouth = boss.GetMousePos();
                npc.NewProjectileDirectInAI<PurpleElectricBreath>(ZacurrentBreathShared.BreathEnd(ctx), mouth, ZacurrentDirector.BreathSmallDamage(), 0, npc.target
                    , ZacurrentDirector.BreathSmallTime, npc.whoAmI, ZacurrentDirector.BreathProjAi2);
            }

            npc.velocity = -dir * ZacurrentDirector.BreathRecoilSpeed;
            ctx.DeclareDirect();
        }

        /// <summary>进招初始值：先绕到玩家的背面那一侧。旧 AI.ElectricBreathSmall.cs:218-221</summary>
        public static void SetStartValue(ZacurrentDragonContext ctx)
            => ctx.Recorder2 = -MathF.Sign(ctx.Target.Center.X - ctx.Npc.Center.X);
    }

    /// <summary>小吐息单招。旧壳 ZacurrentDragon.States.cs:203-208</summary>
    [VaultState((int)ZacurrentDragon.AIStates.ElectricBreathSmall, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentElectricBreathSmallState : ZacurrentAttackState
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.ElectricBreathSmall;

        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ZacurrentElectricBreathSmallMove.SetStartValue(ctx);

        protected override bool RunAttack(ZacurrentDragonContext ctx) => ZacurrentElectricBreathSmallMove.Run(ctx);
    }
}
