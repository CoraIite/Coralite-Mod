using Coralite.Content.NPCs.Crystalline.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 二阶段螺旋冲刺（距离超过 500 px 时的拉近手段）：<br/>
    /// Idle 15 帧站住 → Ready 30 帧把刀刃展开、机身转到冲刺角、同时<b>反向后撤</b>（后撤本身就是预告）
    /// → 第 45 帧一口气加速到 18 px/f 并挂上冲刺判定与拖尾，飞行中每帧只朝玩家修正 0.01 rad（预告指哪就打哪）
    /// → 撞墙立刻反弹进收招 → 收招 45 帧里速度 0.93 衰减、机身线性回正。<br/>
    /// 收招后玩家正好落在 220~280 px 的环带里就直接连一记短旋风斩，否则回悬浮。<br/>
    /// 旧 <c>P2Rolling</c>（CrystallineSentinel.cs:1438-1557）。冲刺帧数 = 距离 ÷ 速度 + 20，两端同算并用热字段对账。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P2Rolling, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP2RollingState : CrystallineSentinelStateBase
    {
        /// <summary>自用槽 A：冲刺持续帧数（旧 ai[2]）。</summary>
        private const int DashFramesSlot = CoraliteBossHotSlots.A;
        /// <summary>自用槽 B：撞墙那一刻的机身角度，收招时从它线性回正（旧 localAI[0]）。</summary>
        private const int StoredRotationSlot = CoraliteBossHotSlots.B;

        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P2Rolling;

        public override CrystallineSentinelCommon Common => CrystallineSentinelCommon.PhaseTwo;

        protected override int IdleFramesOnFinish => CrystallineSentinelDirector.P2IdleGapFrames;

        private float dashFrames;
        private float storedRotation;

        /// <summary>冲刺段末。</summary>
        private float DashEnd => CrystallineSentinelDirector.RollingReadyEnd + dashFrames;

        /// <summary>收招段末。</summary>
        private float RecoverEnd => DashEnd + CrystallineSentinelDirector.RollingRecoverFrames;

        public override void WriteHot(CrystallineSentinelContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[DashFramesSlot] = dashFrames;
            ctx.Hot[StoredRotationSlot] = storedRotation;
        }

        public override void ReadHot(CrystallineSentinelContext ctx)
        {
            base.ReadHot(ctx);
            dashFrames = ctx.Hot[DashFramesSlot];
            storedRotation = ctx.Hot[StoredRotationSlot];
        }

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareDirect();

            if (AtFrame(0))
            {
                ctx.HandSpurtFrameY = 0;
            }

            bool shouldChangeDirection = true;

            if (AttackTimer <= CrystallineSentinelDirector.RollingIdleEnd)
            {
                npc.rotation = 0f;
                ctx.HandSpurtFrameY = 0;
            }
            else if (AttackTimer < CrystallineSentinelDirector.RollingReadyEnd)
            {
                UpdateReady(ctx);
            }
            else if (AtFrame(CrystallineSentinelDirector.RollingReadyEnd))
            {
                Launch(ctx);
            }
            else if (AttackTimer < DashEnd)
            {
                shouldChangeDirection = false;
                UpdateDash(ctx);
            }
            else if (AttackTimer < RecoverEnd)
            {
                shouldChangeDirection = false;
                UpdateRecover(ctx);
            }
            else
            {
                // 收招完毕，等权威端裁决接哪一手
                ctx.CanHit = false;
            }

            if (shouldChangeDirection)
            {
                ctx.FaceTarget();
            }

            P2BodyFrame(ctx);
        }

        private void UpdateReady(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;

            if (AttackTimer % CrystallineSentinelDirector.RollingReadyFrameRate == 0 && ctx.HandSpurtFrameY < CrystallineSentinelDirector.RollingReadyFrameMaxY)
            {
                ctx.HandSpurtFrameY++;
            }

            npc.rotation = Utils.Remap(AttackTimer, CrystallineSentinelDirector.RollingIdleEnd, CrystallineSentinelDirector.RollingReadyEnd,
                0, CrystallineSentinel.ConvertAtan2ToSpecialAngle(npc.AngleTo(ctx.Target.Center)));
            npc.velocity = Vector2.Lerp(npc.velocity, -npc.DirectionTo(ctx.Target.Center) * CrystallineSentinelDirector.RollingBackSpeed,
                CrystallineSentinelDirector.RollingBackLerp);
        }

        /// <summary>起跑：锁定方向与冲刺时长（两端同算，热字段随后对账）。旧 CrystallineSentinel.cs:1471-1487</summary>
        private void Launch(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.velocity = npc.DirectionTo(ctx.Target.Center) * CrystallineSentinelDirector.RollingDashSpeed;
            dashFrames = (npc.Distance(ctx.Target.Center) / CrystallineSentinelDirector.RollingDashSpeed) + CrystallineSentinelDirector.RollingDashExtraFrames;

            if (!Main.dedServ)
            {
                Helper.PlayPitched(AssetDirectory.Sounds.Misc + "HallowDash", 0.4f, 0.1f, npc.Center);
            }
        }

        private void UpdateDash(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;

            if (AttackTimer % CrystallineSentinelDirector.RollingDashFrameRate == 0)
            {
                ctx.HandSpurtFrameY++;
                if (ctx.HandSpurtFrameY > CrystallineSentinelDirector.RollingDashFrameMaxY)
                {
                    ctx.HandSpurtFrameY = CrystallineSentinelDirector.RollingDashFrameLoopY;
                }
            }

            ctx.CanHit = true;
            npc.velocity = Utils.AngleLerp(npc.velocity.ToRotation(), npc.AngleTo(ctx.Target.Center), CrystallineSentinelDirector.RollingHomingLerp)
                .ToRotationVector2() * npc.velocity.Length();

            FlashEffect(ctx);

            if (npc.collideX || npc.collideY)
            {
                ctx.HandSpurtFrameY = CrystallineSentinelDirector.RollingCollideFrameY;
                npc.velocity *= CrystallineSentinelDirector.RollingCollideBounce;
                storedRotation = npc.rotation;
                RebaseTimer(ctx, DashEnd);
                return;
            }

            npc.rotation = CrystallineSentinel.ConvertAtan2ToSpecialAngle(npc.velocity.ToRotation());
        }

        private void UpdateRecover(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.velocity *= CrystallineSentinelDirector.RollingRecoverDamp;
            npc.rotation = Utils.Remap(AttackTimer, DashEnd, RecoverEnd, storedRotation, 0);

            if (AttackTimer % CrystallineSentinelDirector.RollingRecoverFrameRate == 0 && ctx.HandSpurtFrameY > 0)
            {
                ctx.HandSpurtFrameY++;
                if (ctx.HandSpurtFrameY > CrystallineSentinelDirector.RollingRecoverFrameMaxY)
                {
                    ctx.HandSpurtFrameY = 0;
                }
            }
        }

        /// <summary>刀锋掠过的闪光（纯本地）。旧 CrystallineSentinel.cs:1501-1506</summary>
        private static void FlashEffect(CrystallineSentinelContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            for (int i = 0; i < CrystallineSentinelDirector.RollingFlashCount; i++)
            {
                Vector2 pos = new Vector2(npc.spriteDirection * CrystallineSentinelDirector.RollingFlashOffset, 0).RotatedBy(npc.rotation) + npc.Center;
                Vector2 vel = Vector2.Lerp(Main.rand.NextVector2Unit() * Main.rand.NextFloat(CrystallineSentinelDirector.RollingFlashSpeed),
                    npc.velocity, CrystallineSentinelDirector.RollingFlashLerp) * CrystallineSentinelDirector.RollingFlashScale;
                PRTLoader.NewParticle<CrystallineFlashParticle>(pos, vel);
            }
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (AtFrame(CrystallineSentinelDirector.RollingReadyEnd))
            {
                SpawnDashProjectiles(ctx);
                ctx.MarkDecision();
            }

            if (AttackTimer < RecoverEnd)
            {
                return null;
            }

            // 收招后按距离决定接短旋风斩还是回悬浮（旧代码的连段条件，走同一个提交口）
            float dist = Vector2.Distance(ctx.Npc.Center, ctx.Target.Center);
            if (dist > CrystallineSentinelDirector.RollingChainRangeMin && dist < CrystallineSentinelDirector.RollingChainRangeMax)
            {
                return CrystallineSentinelHubState.ToState(ctx, CrystallineSentinelStateId.P2WhirlSlashShort);
            }

            return ReturnToIdle(ctx, IdleFramesOnFinish);
        }

        /// <summary>冲刺判定与拖尾，寿命跟着冲刺帧数。旧 CrystallineSentinel.cs:1477-1487</summary>
        private void SpawnDashProjectiles(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            int endFrame = (int)DashEnd;

            Projectile spurt = npc.NewProjectileDirectInAI_Server<CrystallineSentinelRollingSpurt>(npc.Center, Vector2.Zero,
                CrystallineSentinelDirector.RollingProjDamage(), CrystallineSentinelDirector.RollingProjKnockback,
                npc.target, npc.whoAmI, endFrame);
            if (spurt != null)
            {
                spurt.timeLeft = (int)dashFrames + 1;
            }

            Projectile trail = npc.NewProjectileDirectInAI_Server<CrystallineSentinelRollingTrail>(npc.Center, Vector2.Zero,
                0, 0, npc.target, npc.whoAmI, endFrame);
            if (trail != null)
            {
                trail.timeLeft = (int)dashFrames + 1;
            }
        }
    }
}
