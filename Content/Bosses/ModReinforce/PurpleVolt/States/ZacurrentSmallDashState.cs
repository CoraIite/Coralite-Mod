using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 短冲（身位调整）：每一次都是"起手锁向一帧 → 冲 N 帧 → 硬刹"，靠 <see cref="ZacurrentDragonContext.Recorder"/> 记剩余次数。<br/>
    /// 公平阀：距离小于 700 px 时故意冲偏（<see cref="ZacurrentDirector.SmallDashOffsetMin"/>~Max 弧度），贴脸不会被直接撞；
    /// 冲刺中段只在 500 px 内做小幅修正，起手方向即承诺。<br/>
    /// 旧 <c>ZacurrentDragon.SmallDash&lt;T&gt;</c>（AI.SmallDash.cs:13-81）。
    /// </summary>
    internal static class ZacurrentSmallDashMove
    {
        /// <summary>一次短冲的完整节拍；返回 true 表示剩余次数耗尽、整招结束。</summary>
        public static bool Run<TProj>(ZacurrentDragonContext ctx, int dashTime, int dashSpeed) where TProj : ModProjectile
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (ctx.Timer == 0)
            {
                // 冲刺残影弹幕（内部自带权威端守卫），随后确定性地定下本次冲刺方向
                npc.NewProjectileInAI_Server<TProj>(npc.Center, Vector2.Zero, ZacurrentDirector.SmallDashDamage(), 0
                    , npc.target, dashTime - 1, npc.whoAmI, ZacurrentDirector.SmallDashProjAi2);

                boss.ElectricSound();
                float targetRot = (ctx.Target.Center - npc.Center).ToRotation();

                // 距离太近就不朝玩家冲，给逃生空间
                if (Vector2.Distance(npc.Center, ctx.Target.Center) < ZacurrentDirector.SmallDashNoAimDistance)
                {
                    targetRot += ctx.AttackRandSign() * ctx.AttackRandFloat(ZacurrentDirector.SmallDashOffsetMin, ZacurrentDirector.SmallDashOffsetMax);
                }

                npc.velocity = targetRot.ToRotationVector2() * dashSpeed;
                npc.rotation = npc.velocity.ToRotation();
                npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
                boss.IsDashing = true;
            }
            else if (ctx.Timer > dashTime / ZacurrentDirector.SmallDashHomingStartDiv && ctx.Timer < dashTime)
            {
                // 途中小幅修正：越近修正越强，速度越高转得越快
                float distance = npc.Center.Distance(ctx.Target.Center);
                if (distance < ZacurrentDirector.SmallDashHomingDistance)
                {
                    float factor = 1 - Math.Clamp(distance / ZacurrentDirector.SmallDashHomingRange, 0.01f, 1);
                    float targetDir = (ctx.Target.Center - npc.Center).ToRotation() + MathHelper.Pi;
                    float turn = (ZacurrentDirector.SmallDashTurnBase + ((dashSpeed - ZacurrentDirector.SmallDashTurnSpeedRef) / ZacurrentDirector.SmallDashTurnSpeedDiv)) * factor;
                    float velocityDir = npc.velocity.ToRotation().AngleTowards(targetDir, turn);
                    npc.velocity = velocityDir.ToRotationVector2() * dashSpeed;
                    npc.rotation = velocityDir;
                }
            }

            ctx.DeclareDirect();
            boss.UpdateAllOldCaches();

            ctx.Timer++;
            if (ctx.Timer > dashTime)
            {
                ctx.Timer = 0;
                npc.velocity *= 0;
                boss.IsDashing = false;

                ctx.Recorder--;
                if (ctx.Recorder < 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 进招初始值：按与玩家的距离决定短冲次数（用 AttackRandom 派生，两端同序得同一结果）。旧 AI.SmallDash.cs:68-81
        /// </summary>
        public static void SetStartValue(ZacurrentDragonContext ctx)
        {
            ctx.Recorder = ctx.AttackRandom.Next(ZacurrentDirector.SmallDashRepeatMin, ZacurrentDirector.SmallDashRepeatMax);

            float distance = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center);
            if (distance > ZacurrentDirector.SmallDashExtraDistance1)
            {
                ctx.Recorder++;
            }

            if (distance > ZacurrentDirector.SmallDashExtraDistance2)
            {
                ctx.Recorder++;
            }

            ctx.Boss.ResetAllOldCaches();
            ctx.Boss.canDrawShadows = true;
        }
    }

    /// <summary>普通短冲：1~3 次（远距离追加），紫色残影。旧壳 ZacurrentDragon.States.cs:189-194</summary>
    [VaultState((int)ZacurrentDragon.AIStates.SmallDash, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentSmallDashState : ZacurrentAttackState
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.SmallDash;

        protected override void OnStateEnter(ZacurrentDragonContext ctx) => ZacurrentSmallDashMove.SetStartValue(ctx);

        protected override bool RunAttack(ZacurrentDragonContext ctx)
            => ZacurrentSmallDashMove.Run<PurpleDash>(ctx, ZacurrentDirector.SmallDashFrames, ZacurrentDirector.SmallDashSpeed);
    }

    /// <summary>
    /// 紫伏短冲：更短更快的单次冲刺，红色残影。<br/>
    /// 旧实现不调 SetStartValue，<see cref="ZacurrentDragonContext.Recorder"/> 维持 0 → 冲一次即结束。旧壳 ZacurrentDragon.States.cs:196-201
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.SmallDashVolt, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentSmallDashVoltState : ZacurrentAttackState
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.SmallDashVolt;

        protected override bool RunAttack(ZacurrentDragonContext ctx)
            => ZacurrentSmallDashMove.Run<RedDash>(ctx, ZacurrentDirector.SmallDashVoltFrames, ZacurrentDirector.SmallDashVoltSpeed);
    }
}
