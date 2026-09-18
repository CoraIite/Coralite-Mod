using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 虚假撕咬：张嘴扑上来，但咬空——收招的那一刻从藤蔓里甩出三对荆棘刺，真正的杀招在"以为躲过去了"之后。<br/>
    /// 节拍结构：淡出 30 帧 → 扑咬 70 帧（第 10、20 帧各一口 + 前 60 帧拖速度线）→ 收招甩三对刺 → 绕行 145 帧。<br/>
    /// 公平阀：三对刺的起爆时间递增（35 / 80 / 125 帧）且越往后夹角越窄，玩家有连续三次可读的躲避窗；
    /// 咬击本体照旧带张嘴预警。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.p3_fakeBite, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareFakeBiteState : NPNightmareStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>扑咬</summary>
            Bite,
            /// <summary>甩刺后的绕行收招</summary>
            Orbit,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.p3_fakeBite;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Bite:
                    BiteBeat(ctx);
                    break;
                default:
                    OrbitBeat(ctx);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (!FadeTickP3(ctx, () => AimPos(ctx, NightmarePlanteraDirector.P3BiteTeleportMin, NightmarePlanteraDirector.P3BiteTeleportMax),
                onTeleport: () => Bite(ctx),
                postTeleport: () =>
                {
                    if (!Main.dedServ)
                    {
                        SoundEngine.PlaySound(CoraliteSoundID.DeathCalling_Item103, npc.Center);
                    }

                    npc.rotation = (TargetOrSparkle(ctx) - npc.Center).ToRotation();
                }))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Bite);
        }

        /// <summary>场上有美梦光时优先扑它——这是二阶段"打光"机制在三阶段的残留，原样保留。</summary>
        private static Vector2 TargetOrSparkle(NightmarePlanteraContext ctx)
            => NightmarePlantera.FantasySparkleAlive(out NPC fs) ? fs.Center : ctx.TargetCenter;

        private static Vector2 AimPos(NightmarePlanteraContext ctx, float min, float max)
            => ctx.Boss.PickTeleportOffsetAround(TargetOrSparkle(ctx), min, max);

        /// <summary>扑咬本体：两口咬击 + 身侧的速度线，收招那一帧甩出三对荆棘刺。</summary>
        private void BiteBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.MeleeDamage = true;

            if (Timer == NightmarePlanteraDirector.P3BiteFirstFrame || Timer == NightmarePlanteraDirector.P3BiteSecondFrame)
            {
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.DeathCalling_Item103, npc.Center);
                }

                Bite(ctx);
            }

            if (Timer < NightmarePlanteraDirector.FakeBiteSpeedLineFrames
                && Timer % NightmarePlanteraDirector.FakeBiteSpeedLineInterval == 0)
            {
                SpeedLines(ctx);
            }

            LungeToTarget(ctx, 0.3f, NightmarePlanteraDirector.P3LungeDistance, NightmarePlanteraDirector.P3LungeAccel,
                NightmarePlanteraDirector.P3LungeMaxSpeed, NightmarePlanteraDirector.P3LungeDamp);

            if (Timer <= NightmarePlanteraDirector.FakeBiteBodyFrames)
            {
                return;
            }

            ctx.ShootCount = (npc.Center - ctx.TargetCenter).ToRotation();
            ThrowSpikes(ctx);
            SwitchBeat(ctx, (int)Beat.Orbit);
        }

        /// <summary>身侧的速度线。抽取用同步种子，不能门控（C3），否则收招甩刺的角度会两端分叉。</summary>
        private static void SpeedLines(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            for (int i = -1; i < 2; i += 2)
            {
                Color c = boss.AttackRandom.Next(0, 1) switch
                {
                    0 => boss.tentacleColor,
                    _ => boss.tentacleColor * 2f,
                };

                PRTLoader.NewParticle(ctx.Npc.Center + Main.rand.NextVector2Circular(32, 32),
                    (ctx.Npc.rotation + (i * 2.6f)).ToRotationVector2() * boss.NextAttackFloat(3f, 8f),
                    CoraliteContent.ParticleType<SpeedLine>(), c, boss.NextAttackFloat(0.3f, 0.5f));
            }
        }

        /// <summary>三对荆棘刺：越往后起爆越晚、夹角越窄，基准角每对外移 0.25。</summary>
        private static void ThrowSpikes(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            int damage = NightmarePlanteraDirector.P3BiteDamage();
            float baseAngle = ctx.ShootCount;

            for (int i = 0; i < NightmarePlanteraDirector.FakeBiteSpikePairs; i++)
            {
                int time = NightmarePlanteraDirector.FakeBiteSpikeBaseTime + (NightmarePlanteraDirector.FakeBiteSpikeTimeStep * i);
                float angle = NightmarePlanteraDirector.FakeBiteSpikeAngle + (i * NightmarePlanteraDirector.FakeBiteSpikeAngleStep);

                npc.NewProjectileInAI_Server<VineSpike>(npc.Center,
                    (ctx.ShootCount + NightmarePlanteraDirector.FakeBiteSpikeSideAngle).ToRotationVector2(),
                    damage, 8, npc.target, boss.ZenithProjSeed(), baseAngle + angle, time);
                npc.NewProjectileInAI_Server<VineSpike>(npc.Center,
                    (ctx.ShootCount - NightmarePlanteraDirector.FakeBiteSpikeSideAngle).ToRotationVector2(),
                    damage, 8, npc.target, boss.ZenithProjSeed(), baseAngle - angle, time);
                baseAngle += NightmarePlanteraDirector.FakeBiteSpikeBaseStep;
            }

            ctx.MarkDecision();
        }

        /// <summary>收招绕行：半径 450 的半圆锚点 + 正弦摆动，把本体带离玩家，给刺留出表演空间。</summary>
        private void OrbitBeat(NightmarePlanteraContext ctx)
        {
            ctx.Boss.DoRotation(0.3f);

            float angle = ctx.ShootCount
                + (MathHelper.PiOver4 / 4 * MathF.Sin(Timer * NightmarePlanteraDirector.FakeBiteOrbitWaveSpeed));
            ctx.DeclareApproach(ctx.TargetCenter + (angle.ToRotationVector2() * NightmarePlanteraDirector.FakeBiteOrbitRadius),
                NightmarePlanteraDirector.FakeBiteOrbitTurn, NightmarePlanteraDirector.FakeBiteOrbitMaxSpeed,
                NightmarePlanteraDirector.FakeBiteOrbitBlend, NightmarePlanteraDirector.FakeBiteOrbitSpeedRange);
        }

        private static void Bite(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.NewProjectileInAI_Server<NightmareBite>(npc.Center, Vector2.Zero,
                NightmarePlanteraDirector.P3BiteDamage(), 4,
                ai0: NightmarePlanteraDirector.FakeBiteAi0, ai1: NightmarePlanteraDirector.FakeBiteAi1,
                ai2: ctx.Boss.ZenithProjSeedNeg2());
            ctx.MarkDecision();
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Orbit && Timer > NightmarePlanteraDirector.FakeBiteOrbitFrames
                ? NPNightmareP3State.Commit(ctx)
                : null;
    }
}
