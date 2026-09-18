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
    /// 虚假撕咬：张嘴扑上来但咬空，收招那一刻从身后甩出两根荆棘刺。二阶段版只有一对刺，起爆时间 35 / 65 错开。<br/>
    /// 节拍：淡出 45 帧 → 张嘴扑 65 帧（第 10 帧一声嚎叫，前 50 帧拖速度线）→ 甩刺后绕行 80 帧。<br/>
    /// 全程没有接触伤害——这一招的威胁完全来自收招的刺，"躲过嘴"不等于躲过招。<br/>
    /// 原 <c>FakeBite</c> + 三个 <c>_Son</c>（Phase.P2_Dream.cs:377-477）与 BT 序列 <c>BuildFakeBiteTree</c>。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.fakeBite, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamFakeBiteState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>张嘴扑击</summary>
            Bite,
            /// <summary>甩刺后的绕行收招</summary>
            Orbit,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.fakeBite;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
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

            if (!FadeTickP2(ctx, () => AimPos(ctx, NightmarePlanteraDirector.P2BiteTeleportMin, NightmarePlanteraDirector.P2BiteTeleportMax),
                onTeleport: () =>
                {
                    npc.NewProjectileInAI_Server<NightmareBite>(npc.Center, Vector2.Zero,
                        NightmarePlanteraDirector.P2BiteDamage(), 4,
                        ai0: NightmarePlanteraDirector.P2FakeBiteAi0, ai1: NightmarePlanteraDirector.P2FakeBiteAi1,
                        ai2: ctx.Boss.ZenithProjSeed());
                    ctx.MarkDecision();
                },
                postTeleport: () => npc.rotation = (TargetOrSparkle(ctx) - npc.Center).ToRotation()))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Bite);
        }

        private void BiteBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (Timer == NightmarePlanteraDirector.P2FakeBiteSoundFrame && !Main.dedServ)
            {
                SoundEngine.PlaySound(CoraliteSoundID.DeathCalling_Item103, npc.Center);
            }

            if (Timer < NightmarePlanteraDirector.P2FakeBiteSpeedLineFrames
                && Timer % NightmarePlanteraDirector.P2FakeBiteSpeedLineInterval == 0)
            {
                SpeedLines(ctx);
            }

            LungeToSparkleOrTarget(ctx);

            if (Timer <= NightmarePlanteraDirector.P2FakeBiteBodyFrames)
            {
                return;
            }

            ctx.ShootCount = (npc.Center - ctx.TargetCenter).ToRotation();
            ThrowSpikes(ctx);
            SwitchBeat(ctx, (int)Beat.Orbit);
        }

        /// <summary>身侧的速度线。抽取用同步种子，不能门控（C3），否则甩刺角度会两端分叉。</summary>
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
                    (ctx.Npc.rotation + (i * NightmarePlanteraDirector.P2SpeedLineSideAngle)).ToRotationVector2()
                        * boss.NextAttackFloat(NightmarePlanteraDirector.P2SpeedLineSpeedMin, NightmarePlanteraDirector.P2SpeedLineSpeedMax),
                    CoraliteContent.ParticleType<SpeedLine>(), c, boss.NextAttackFloat(0.3f, 0.5f));
            }
        }

        /// <summary>甩出一对荆棘刺：初速朝左右两侧，真正的刺沿 ShootCount ± 0.9 生长，起爆时间错开 35 / 65。</summary>
        private static void ThrowSpikes(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            int damage = NightmarePlanteraDirector.P2SpikeDamage();
            float baseAngle = ctx.ShootCount;

            npc.NewProjectileInAI_Server<VineSpike>(npc.Center,
                (baseAngle + NightmarePlanteraDirector.P2FakeBiteSpikeSideAngle).ToRotationVector2(), damage, 8, npc.target,
                boss.ZenithProjSeed0(), baseAngle + NightmarePlanteraDirector.P2FakeBiteSpikeAngle,
                NightmarePlanteraDirector.P2FakeBiteSpikeTimeA);
            npc.NewProjectileInAI_Server<VineSpike>(npc.Center,
                (baseAngle - NightmarePlanteraDirector.P2FakeBiteSpikeSideAngle).ToRotationVector2(), damage, 8, npc.target,
                boss.ZenithProjSeed0(), baseAngle - NightmarePlanteraDirector.P2FakeBiteSpikeAngle,
                NightmarePlanteraDirector.P2FakeBiteSpikeTimeB);

            ctx.MarkDecision();
        }

        /// <summary>收招绕行：半径 450 的半圆锚点 + 正弦摆动，把本体带离玩家，给刺留出表演空间。</summary>
        private void OrbitBeat(NightmarePlanteraContext ctx)
        {
            ctx.Boss.DoRotation(0.3f);

            float angle = ctx.ShootCount
                + (MathHelper.PiOver4 / 4 * MathF.Sin(Timer * NightmarePlanteraDirector.P2FakeBiteOrbitWave));
            ctx.DeclareApproach(ctx.TargetCenter + (angle.ToRotationVector2() * NightmarePlanteraDirector.P2FakeBiteOrbitRadius),
                NightmarePlanteraDirector.P2FakeBiteOrbitTurn, NightmarePlanteraDirector.P2FakeBiteOrbitMaxSpeed,
                NightmarePlanteraDirector.P2FakeBiteOrbitBlend, NightmarePlanteraDirector.P2FakeBiteOrbitSpeedRange);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Orbit && Timer > NightmarePlanteraDirector.P2FakeBiteOrbitFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
