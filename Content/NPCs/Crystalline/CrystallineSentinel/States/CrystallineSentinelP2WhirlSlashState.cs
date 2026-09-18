using Coralite.Content.NPCs.Crystalline.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 二阶段旋风斩（近身范围压制）：<br/>
    /// Ready 22 帧——第 1 帧就放出<b>预警环</b>（半径 = 刀刃半径 + 120，仓库里唯一实体化的预告）并喊“![ ▼ M ▼ ]＃”，
    /// 远了追近、近了刹停，双手收成握刀姿态；第 15 帧两把刀刃落到身侧。<br/>
    /// Charge 80 帧蓄力（第 47 帧一次环形爆发表现）→ Swing 50 帧刀刃沿半径外扩整圈挥砍 → Recover 60 帧收手回悬浮。<br/>
    /// 逃生道 = 预警环之外，或者贴到半径内侧。<br/>
    /// 旧 <c>P2WhirlSlash(false)</c>（CrystallineSentinel.cs:1560-1691）。短版是 <see cref="CrystallineSentinelP2WhirlSlashShortState"/>。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P2WhirlSlash, typeof(CrystallineSentinelContext))]
    internal class CrystallineSentinelP2WhirlSlashState : CrystallineSentinelStateBase
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P2WhirlSlash;

        public override CrystallineSentinelCommon Common => CrystallineSentinelCommon.PhaseTwo;

        protected override int IdleFramesOnFinish => CrystallineSentinelDirector.P2IdleGapFrames;

        /// <summary>短版：刀刃半径对折、蓄力段砍掉 70 帧、预警环寿命对折（螺旋冲刺的连段用）。</summary>
        protected virtual bool LightVer => false;

        /// <summary>旧代码在招式体开头就 <c>Timer++</c>，之后所有拍点读的都是自增后的值，这里对齐。</summary>
        private float PostTimer => AttackTimer + 1;

        private float MaxRange => LightVer
            ? CrystallineSentinelDirector.WhirlMaxRange * CrystallineSentinelDirector.WhirlShortRangeScale
            : CrystallineSentinelDirector.WhirlMaxRange;

        private int ChargeEnd => CrystallineSentinelDirector.WhirlReadyFrames + CrystallineSentinelDirector.WhirlChargeFrames
            - (LightVer ? CrystallineSentinelDirector.WhirlShortChargeCut : 0);

        private int SwingEnd => ChargeEnd + CrystallineSentinelDirector.WhirlSwingFrames;

        private int RecoverEnd => SwingEnd + CrystallineSentinelDirector.WhirlRecoverFrames;

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareDirect();

            if (++npc.frameCounter > CrystallineSentinelDirector.WhirlFrameRate)
            {
                npc.frameCounter = 0;
                if (++npc.frame.Y > CrystallineSentinelDirector.P2BodyFrameMaxY)
                {
                    npc.frame.Y = 0;
                }
            }

            // 巡航：越远飘得越快，方向带确定性抖动
            float speed = Utils.Remap(npc.Distance(ctx.Target.Center), CrystallineSentinelDirector.WhirlSpeedNearDistance,
                CrystallineSentinelDirector.WhirlSpeedFarDistance, CrystallineSentinelDirector.WhirlSpeedNear, CrystallineSentinelDirector.WhirlSpeedFar);
            Vector2 targetSpeed = npc.DirectionTo(ctx.Target.Center).SafeNormalize(Vector2.Zero) * speed;
            npc.velocity = Vector2.Lerp(npc.velocity, targetSpeed, CrystallineSentinelDirector.WhirlSpeedLerp);
            if (PostTimer % CrystallineSentinelDirector.P2WanderInterval == 0)
            {
                npc.velocity = npc.velocity.RotatedBy(Wobble(ctx, CrystallineSentinelDirector.P2WanderAngle));
            }

            CollideSpeed(ctx);
            ctx.FaceTarget();

            if (PostTimer < CrystallineSentinelDirector.WhirlReadyFrames)
            {
                UpdateReady(ctx);
            }
            else if (PostTimer < ChargeEnd)
            {
                if (PostTimer == CrystallineSentinelDirector.WhirlReadyFrames + CrystallineSentinelDirector.WhirlBurstCue)
                {
                    BurstEffect(ctx);
                }
            }
            else if (PostTimer >= SwingEnd && PostTimer < RecoverEnd)
            {
                // 收手
                if (PostTimer % CrystallineSentinelDirector.WhirlFrameRate == 0 && ctx.HandFrame[0] < CrystallineSentinelDirector.MaxHandFrame)
                {
                    ctx.HandFrame[0]++;
                    ctx.HandFrame[1]++;
                }
            }
        }

        private void UpdateReady(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;

            if (PostTimer == 1)
            {
                ctx.SetText(CrystallineSentinelTextType.Angry, CrystallineSentinelDirector.FireTextFrames);
                Telegraph(ctx);
            }

            if (Vector2.Distance(npc.Center, ctx.Target.Center) > CrystallineSentinelDirector.WhirlChaseRange)
            {
                npc.velocity += (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * CrystallineSentinelDirector.WhirlChaseAccel;
                if (npc.velocity.Length() > CrystallineSentinelDirector.WhirlChaseMaxSpeed)
                {
                    npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * CrystallineSentinelDirector.WhirlChaseMaxSpeed;
                }
            }
            else
            {
                npc.velocity *= CrystallineSentinelDirector.WhirlBrakeDamp;
            }

            if (PostTimer % CrystallineSentinelDirector.WhirlFrameRate == 0 && ctx.HandFrame[0] > CrystallineSentinelDirector.WhirlHandFrameMin)
            {
                ctx.HandFrame[0]--;
                ctx.HandFrame[1]--;
            }
        }

        /// <summary>预警环：跟着本体走、半径就是刀刃能扫到的范围。旧 CrystallineSentinel.cs:1599-1605</summary>
        private void Telegraph(CrystallineSentinelContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            CrystallineSentinelTelegraphRing ring = PRTLoader.NewParticle<CrystallineSentinelTelegraphRing>(ctx.Npc.Center, Vector2.Zero,
                Coralite.CrystallinePurple, 1f);
            if (ring == null)
            {
                return;
            }

            ring.FollowNPCIndex = ctx.Npc.whoAmI;
            ring.Radius = MaxRange + CrystallineSentinelDirector.WhirlRingRadiusBonus;
            ring.Lifetime = LightVer ? CrystallineSentinelDirector.WhirlRingLifetime / 2 : CrystallineSentinelDirector.WhirlRingLifetime;
        }

        /// <summary>蓄力爆发：两道对冲刀光 + 大量闪光碎屑（纯本地）。旧 CrystallineSentinel.cs:1643-1674</summary>
        private static void BurstEffect(CrystallineSentinelContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            for (int i = 0; i < 2; i++)
            {
                float scale = Main.rand.NextFloat(0.7f, 1.3f) * 0.75f;
                Vector2 dir = npc.DirectionTo(ctx.Target.Center).RotatedBy(i * MathHelper.Pi);
                Vector2 vel = dir * Main.rand.NextFloat(8, 10) * 3;
                Vector2 pos = npc.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 4));

                CrystallineTrail slash = PRTLoader.NewParticle<CrystallineTrail>(pos, vel, Scale: scale * 0.75f);
                if (slash != null)
                {
                    slash.ScaleY = scale * 1.5f;
                }

                Helper.PlayPitched(AssetDirectory.Sounds.Misc + "SwingWave", 0.4f, 0.1f, npc.Center);

                for (int j = 0; j < 10; j++)
                {
                    Vector2 v = dir.RotateByRandom(-0.2f, 0.2f).RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(1, 5);
                    PRTLoader.NewParticle<CrystallineFlashParticle>(pos + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 10)), v);
                }

                for (int j = 0; j < 20; j++)
                {
                    Vector2 v = dir.RotateByRandom(-0.2f, 0.3f) * Main.rand.NextFloat(2, 10);
                    PRTLoader.NewParticle<CrystallineFlashParticle>(pos + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 10)), v);
                }

                CrystallineRockBlast blast = PRTLoader.NewParticle<CrystallineRockBlast>(npc.Center, dir.RotatedBy(MathHelper.PiOver2));
                if (blast != null)
                {
                    blast.Rotation = blast.Velocity.ToRotation();
                }
            }
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (PostTimer == CrystallineSentinelDirector.WhirlReadyFrames - CrystallineSentinelDirector.WhirlBladeCueLead)
            {
                SpawnBlades(ctx);
                ctx.MarkDecision();
            }

            if (PostTimer >= RecoverEnd)
            {
                return ReturnToIdle(ctx, IdleFramesOnFinish);
            }

            return null;
        }

        /// <summary>两把刀刃分挂身侧半圈，出手方向在这一帧锁死（预告指哪就打哪）。旧 CrystallineSentinel.cs:1621-1635</summary>
        private void SpawnBlades(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            float rot = npc.AngleTo(ctx.Target.Center);

            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = npc.Center + new Vector2(CrystallineSentinelDirector.WhirlBladeSpawnOffset, CrystallineSentinelDirector.WhirlBladeSpawnOffset)
                    .RotatedBy(MathHelper.Pi + (i * MathHelper.PiOver2));
                Projectile slash = npc.NewProjectileDirectInAI_Server<CrystallineSentinelSwingSlash>(pos, Vector2.Zero,
                    CrystallineSentinelDirector.WhirlProjDamage(), CrystallineSentinelDirector.WhirlProjKnockback,
                    npc.target, npc.whoAmI, i, rot + (i * MathHelper.Pi));

                if (slash?.ModProjectile is CrystallineSentinelSwingSlash slashProj)
                {
                    slashProj.MaxRange = MaxRange;
                    slashProj.LightVer = LightVer ? 1 : 0;
                }
            }
        }
    }
}
