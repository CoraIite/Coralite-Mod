using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 蝙蝠与乌鸦：二阶段最长的一手（约 9 秒）。绕着玩家大圈巡航，左右夹击的蝙蝠封走位、瞄准的蝙蝠逼移动、
    /// 绕圈的乌鸦占空间；撒满之后淡出瞬移到侧面爆一圈蝙蝠，再原地收束自旋补两圈，最后贴身绕小圈收招。<br/>
    /// 节拍：淡出 45 → 就位 30 → 撒弹绕行 320（末 45 帧淡出）→ 收束自旋 120 → 收招 40。<br/>
    /// 原 <c>BatsAndCrows</c>（Phase.P2_Dream.cs:978-1201，224 行的单体方法，这里按拍拆开）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.batsAndCrows, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamBatsAndCrowsState : NPDreamStateBase
    {
        /// <summary>每一拍自己摆触手（收束段要绕本体转 3 圈），外壳不能再摆一次。</summary>
        protected override bool AutoTentacle => false;

        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>就位（转到轨道上）</summary>
            Settle,
            /// <summary>绕行撒弹</summary>
            Orbit,
            /// <summary>收束自旋</summary>
            Spin,
            /// <summary>贴身收招</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.batsAndCrows;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Settle:
                    SettleBeat(ctx);
                    break;
                case Beat.Orbit:
                    OrbitBeat(ctx);
                    break;
                case Beat.Spin:
                    SpinBeat(ctx);
                    break;
                default:
                    RecoverBeat(ctx);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.NormallySetTentacle();

            if (!FadeTickP2(ctx,
                () => boss.PickTargetTeleportOffset(NightmarePlanteraDirector.P2BatsTeleportMin, NightmarePlanteraDirector.P2BatsTeleportMax),
                postTeleport: () =>
                {
                    npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
                    // ShootCount 存的是"本体相对玩家的方位角"，绕行段用它当轨道基准角。
                    ctx.ShootCount = (npc.Center - ctx.TargetCenter).ToRotation();
                }))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Settle);
        }

        /// <summary>就位：先沿轨道转 30 帧，再把轨道基准角重新对齐到当前方位，避免撒弹段一开始就横穿玩家。</summary>
        private void SettleBeat(NightmarePlanteraContext ctx)
        {
            Orbit(ctx);

            if (Timer <= NightmarePlanteraDirector.P2BatsSettleFrames)
            {
                return;
            }

            ctx.ShootCount = (ctx.Npc.Center - ctx.TargetCenter).ToRotation();
            SwitchBeat(ctx, (int)Beat.Orbit);
        }

        private void OrbitBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            Orbit(ctx);

            float targetRot = (ctx.TargetCenter - npc.Center).ToRotation();
            int damage = NightmarePlanteraDirector.P2SparkleDamage();

            if (Timer % NightmarePlanteraDirector.P2BatsPairInterval == 0)
            {
                for (int i = -1; i < 2; i += 2)
                {
                    float angle = targetRot + (i * NightmarePlanteraDirector.P2BatsPairAngle)
                        + boss.NextAttackFloat(-NightmarePlanteraDirector.P2BatsPairJitter, NightmarePlanteraDirector.P2BatsPairJitter);
                    npc.NewProjectileInAI_Server<NightmareBat>(boss.GetPhase1MousePos(),
                        angle.ToRotationVector2() * NightmarePlanteraDirector.P2BatsPairSpeed, damage, 1, -1, boss.ZenithProjSeed(), 0);
                }

                ctx.MarkDecision();
                Helper.PlayPitched(CoraliteSoundID.Fairy_NPCHit5, npc.Center, pitch: 0.2f);
            }

            if (Timer > NightmarePlanteraDirector.P2BatsAimInterval && Timer % NightmarePlanteraDirector.P2BatsAimInterval == 0)
            {
                float angle = targetRot + boss.NextAttackFloat(-NightmarePlanteraDirector.P2BatsAimJitter, NightmarePlanteraDirector.P2BatsAimJitter);
                npc.NewProjectileInAI_Server<NightmareBat>(boss.GetPhase1MousePos(),
                    angle.ToRotationVector2() * NightmarePlanteraDirector.P2BatsAimSpeed, damage, 1, -1, boss.ZenithProjSeed(), 0);
                ctx.MarkDecision();
            }

            if (Timer > NightmarePlanteraDirector.P2CrowStartFrame && Timer % NightmarePlanteraDirector.P2CrowInterval == 0)
            {
                float rot = boss.NextAttackFloat(MathHelper.TwoPi);
                for (int i = 0; i < NightmarePlanteraDirector.P2CrowCount; i++)
                {
                    npc.NewProjectileInAI_Server<NightmareCrow>(boss.GetPhase1MousePos(),
                        (rot + (i * MathHelper.TwoPi / NightmarePlanteraDirector.P2CrowCount)).ToRotationVector2() * NightmarePlanteraDirector.P2CrowSpeed,
                        damage, 1, -1, boss.ZenithProjSeedNeg2(), 0,
                        boss.NextAttackFromList(-1, 1) * NightmarePlanteraDirector.P2CrowSpin);
                }

                ctx.MarkDecision();
                Helper.PlayPitched(CoraliteSoundID.BloodThron_Item113, npc.Center, pitch: 0.2f);
            }

            SpinFadeOut(ctx, NightmarePlanteraDirector.P2BatsOrbitFrames, npc.Center);

            if (Timer <= NightmarePlanteraDirector.P2BatsOrbitFrames)
            {
                return;
            }

            Land(ctx);
            SwitchBeat(ctx, (int)Beat.Spin);
        }

        /// <summary>落点：玩家侧方 680~820、上下随机 200 内，垂直下落并当场炸一圈蝙蝠。</summary>
        private static void Land(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.velocity = new Vector2(0, NightmarePlanteraDirector.P2RollingBiteSpeed);
            boss.alpha = 1;
            boss.canDrawWarp = false;

            int direction = Math.Sign(ctx.TargetCenter.X - npc.Center.X);
            npc.Center = ctx.TargetCenter + new Vector2(
                direction * boss.AttackRandom.Next(NightmarePlanteraDirector.P2BatsLandSideMin, NightmarePlanteraDirector.P2BatsLandSideMax),
                boss.AttackRandom.Next(-NightmarePlanteraDirector.P2BatsLandHeight, NightmarePlanteraDirector.P2BatsLandHeight));
            npc.rotation = MathHelper.PiOver2;

            SnapTentacles(ctx);
            ctx.ShootCount = (npc.Center - ctx.TargetCenter).ToRotation();

            BatRing(ctx, NightmarePlanteraDirector.P2BatsRingSpeed, 1, NightmarePlanteraDirector.P2BatsRingSpin);
        }

        /// <summary>收束自旋：转速由 0.35 衰减到 0，每 30 帧一圈蝙蝠（自转方向交替）、每 55 帧一圈乌鸦。</summary>
        private void SpinBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            Vector2 center = npc.Center;
            float rolling = NightmarePlanteraDirector.P2BatsSpinFrames;

            float rotFactor = Math.Clamp(1 - (Timer / rolling), 0, 1);
            npc.rotation += rotFactor * NightmarePlanteraDirector.P2RollingSpinStep;

            CircleMovement(ctx, Timer, NightmarePlanteraDirector.P2BatsSpinOrbitDistance, NightmarePlanteraDirector.P2BatsSpinOrbitSpeed,
                NightmarePlanteraDirector.P2BatsSpinOrbitAccel, NightmarePlanteraDirector.P2BatsSpinOrbitRolling,
                NightmarePlanteraDirector.P2BatsSpinOrbitAngle, ctx.ShootCount);

            SpinFog(ctx);
            SpinTentacles(ctx, center, npc.Center, Timer / rolling, NightmarePlanteraDirector.P2BatsSpinTentacleTurns);

            if (Timer % NightmarePlanteraDirector.P2BatsSpinRingInterval == 0)
            {
                BatRing(ctx, NightmarePlanteraDirector.P2BatsRingSpeed, 0,
                    (Timer % NightmarePlanteraDirector.P2BatsSpinRingFlipPeriod == 0 ? 1 : -1) * NightmarePlanteraDirector.P2BatsRingSpin);
            }

            if (Timer > NightmarePlanteraDirector.P2BatsAimInterval && Timer % NightmarePlanteraDirector.P2BatsSpinCrowInterval == 0)
            {
                int damage = NightmarePlanteraDirector.P2SparkleDamage();
                for (int i = 0; i < NightmarePlanteraDirector.P2BatsRingCount; i++)
                {
                    npc.NewProjectileInAI_Server<NightmareCrow>(boss.GetPhase1MousePos(),
                        (npc.rotation + (i * MathHelper.TwoPi / NightmarePlanteraDirector.P2BatsRingCount)).ToRotationVector2()
                            * NightmarePlanteraDirector.P2BatsSpinCrowSpeed,
                        damage, 1, -1, boss.ZenithProjSeed(), 0, -NightmarePlanteraDirector.P2BatsSpinCrowSpin);
                }

                ctx.MarkDecision();
                Helper.PlayPitched(CoraliteSoundID.BloodThron_Item113, npc.Center, pitch: 0.2f);
            }

            if (Timer > rolling)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
            }
        }

        private void RecoverBeat(NightmarePlanteraContext ctx)
        {
            ctx.Boss.DoRotation(0.3f);
            CircleMovement(ctx, Timer, NightmarePlanteraDirector.P2BatsRecoverDistance, NightmarePlanteraDirector.P2BatsRecoverSpeed);
            ctx.Boss.NormallySetTentacle();
        }

        /// <summary>沿 ShootCount 基准角的大圈巡航，全招通用。</summary>
        private void Orbit(NightmarePlanteraContext ctx)
        {
            ctx.Boss.NormallySetTentacle();
            CircleMovement(ctx, Timer, NightmarePlanteraDirector.P2BatsOrbitDistance, NightmarePlanteraDirector.P2BatsOrbitSpeed,
                NightmarePlanteraDirector.P2BatsOrbitAccel, NightmarePlanteraDirector.P2BatsOrbitRolling,
                NightmarePlanteraDirector.P2BatsOrbitAngle, ctx.ShootCount);
            ctx.Boss.DoRotation(0.3f);
        }

        /// <summary>以本体头朝向为基准炸一圈 7 只蝙蝠。</summary>
        private static void BatRing(NightmarePlanteraContext ctx, float speed, float ai1, float spin)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            int damage = NightmarePlanteraDirector.P2SparkleDamage();

            for (int i = 0; i < NightmarePlanteraDirector.P2BatsRingCount; i++)
            {
                npc.NewProjectileInAI_Server<NightmareBat>(boss.GetPhase1MousePos(),
                    (npc.rotation + (i * MathHelper.TwoPi / NightmarePlanteraDirector.P2BatsRingCount)).ToRotationVector2() * speed,
                    damage, 1, -1, boss.ZenithProjSeedNeg2(), ai1, spin);
            }

            ctx.MarkDecision();
            Helper.PlayPitched(CoraliteSoundID.BloodThron_Item113, npc.Center, pitch: 0.2f);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.P2BatsRecoverFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
