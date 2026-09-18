using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 瞬移闪光：瞬移到玩家面前，触手收成一朵越转越快的花，一边旋转一边朝五个方向撒噩梦光；
    /// 每 100 帧原地瞬移换一个角度重新开火，最后跳到玩家斜上方收招。<br/>
    /// 节拍结构：淡出 30 帧 → 旋转开火 360 帧（自转速率在前 120 帧线性拉满，发射间隔从 30 帧压到 10 帧，
    /// 每 100 帧换一次站位）→ 绕圈收招 45 帧。<br/>
    /// 公平阀：五发一组均分整圈，缺口恒定 72°，所以"贴着缺口走"永远成立；自转加速是可见可听的
    /// （间隔变短伴随音高不变的连续射击声），玩家能预判密度上升。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.p3_teleportSparkles, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareTeleportSparklesState : NPNightmareStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>旋转开火</summary>
            Spin,
            /// <summary>绕圈收招</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.p3_teleportSparkles;

        /// <summary>旋转段自己把触手收成花，外壳不要插手。</summary>
        protected override bool AutoTentacle => false;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Spin:
                    SpinBeat(ctx);
                    break;
                default:
                    ctx.Boss.DoRotation(0.3f);
                    CircleMovement(ctx, Timer, NightmarePlanteraDirector.TpSparkleCircleDistance, NightmarePlanteraDirector.TpSparkleCircleSpeed,
                        NightmarePlanteraDirector.TpSparkleCircleAccel, NightmarePlanteraDirector.TpSparkleCircleRolling);
                    ctx.Boss.NormallySetTentacle();
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;
            int direction = ctx.Target?.direction ?? 1;

            if (!FadeTickP3(ctx,
                () => ctx.TargetCenter + (new Vector2(-direction, 0)
                    * boss.NextAttackFloat(NightmarePlanteraDirector.TpSparkleOpenDistMin, NightmarePlanteraDirector.TpSparkleOpenDistMax)),
                onTeleport: () => npc.rotation = boss.NextAttackFloat(MathHelper.TwoPi)))
            {
                boss.NormallySetTentacle();
                return;
            }

            boss.NormallySetTentacle();
            SwitchBeat(ctx, (int)Beat.Spin);
        }

        /// <summary>旋转开火：自转速率与发射密度随时间线性上升，每 100 帧原地换站位。</summary>
        private void SpinBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            float rampFactor = Math.Clamp(Timer / (float)NightmarePlanteraDirector.TpSparkleRampFrames, 0, 1);
            npc.rotation += rampFactor * NightmarePlanteraDirector.TpSparkleRotStep;

            Fog(ctx);

            int delay = NightmarePlanteraDirector.TpSparkleBaseDelay - (int)(rampFactor * NightmarePlanteraDirector.TpSparkleDelayCut);
            if (Timer % delay == 0)
            {
                SparkleRing(ctx);
            }

            SpinTentacles(ctx, Timer / (float)NightmarePlanteraDirector.TpSparkleFrames);

            if (Timer % NightmarePlanteraDirector.TpSparkleReTeleportInterval == 0)
            {
                ReTeleport(ctx);
            }

            if (Timer <= NightmarePlanteraDirector.TpSparkleFrames)
            {
                return;
            }

            // 收招前跳到玩家斜上方，给后面的绕圈留出半径。
            boss.alpha = 1;
            boss.canDrawWarp = false;
            int direction = Math.Sign(ctx.TargetCenter.X - npc.Center.X);
            npc.Center = ctx.TargetCenter
                + new Vector2(direction * boss.AttackRandom.Next(NightmarePlanteraDirector.TpSparkleEndDistMin, NightmarePlanteraDirector.TpSparkleEndDistMax),
                    NightmarePlanteraDirector.TpSparkleEndHeight);
            npc.rotation = MathHelper.PiOver2;
            boss.ResetTentaclesTo(npc.Center, npc.rotation);
            SwitchBeat(ctx, (int)Beat.Recover);
        }

        /// <summary>五发一组，均分整圈。</summary>
        private static void SparkleRing(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            int damage = NightmarePlanteraDirector.P3BiteDamage();

            for (int i = 0; i < NightmarePlanteraDirector.TpSparklePerVolley; i++)
            {
                Vector2 dir = (npc.rotation + (i * MathHelper.TwoPi / NightmarePlanteraDirector.TpSparklePerVolley)).ToRotationVector2();
                npc.NewProjectileInAI_Server<NightmareSparkle_Red>(npc.Center, dir, damage, 0);
            }

            ctx.MarkDecision();

            if (!Main.dedServ)
            {
                Helper.PlayPitched(CoraliteSoundID.CrystalSerpent_Item109, npc.Center, pitch: -0.5f);
            }
        }

        /// <summary>原地换站位：沿本体→玩家的连线退到 550~650 px 外，撒一圈星尘当到位提示。</summary>
        private static void ReTeleport(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.velocity *= 0;
            boss.alpha = 1;
            boss.canDrawWarp = false;
            boss.warpScale = 0;
            npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();

            Vector2 away = (npc.Center - ctx.TargetCenter).SafeNormalize(Vector2.Zero);
            npc.Center = ctx.TargetCenter
                + (away * boss.AttackRandom.Next(NightmarePlanteraDirector.TpSparkleReTeleportMin, NightmarePlanteraDirector.TpSparkleReTeleportMax));

            for (int i = 0; i < NightmarePlanteraDirector.P3FadeDustCount; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                Dust dust = Dust.NewDustPerfect(npc.Center + (dir * boss.AttackRandom.Next(0, 64)), DustType<NightmareStar>(),
                    dir * boss.NextAttackFloat(2f, 6f), newColor: NightmarePlantera.nightmareRed, Scale: boss.NextAttackFloat(1f, 4f));
                dust.rotation = dir.ToRotation() + MathHelper.PiOver2;
            }

            if (!Main.dedServ)
            {
                Helper.PlayPitched(CoraliteSoundID.NoUse_SuperMagicShoot_Item68, npc.Center, pitch: -1f);
            }

            boss.ResetTentaclesTo(npc.Center, npc.rotation);
        }

        /// <summary>触手收成一朵转动的花。</summary>
        private static void SpinTentacles(NightmarePlanteraContext ctx, float factor)
        {
            NightmarePlantera boss = ctx.Boss;
            if (boss.rotateTentacles == null)
            {
                return;
            }

            Vector2 center = ctx.Npc.Center;
            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = boss.rotateTentacles[i];
                float targetRot = (factor * MathHelper.TwoPi * NightmarePlanteraDirector.TpSparkleTentacleSpinTurns) + (i * MathHelper.TwoPi / 3);
                Vector2 selfPos = Vector2.Lerp(tentacle.pos,
                    center + (NightmarePlanteraDirector.TpSparkleTentacleRadius * targetRot.ToRotationVector2()), 0.2f);
                tentacle.SetValue(selfPos, center, targetRot);
                tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20, 0.7f);
            }
        }

        /// <summary>常驻浓雾。抽取两端同跑（换站位的距离在同一个随机流上，C3）。</summary>
        private static void Fog(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;

            for (int i = 0; i < NightmarePlanteraDirector.TpSparkleFogPerFrame; i++)
            {
                Color color = boss.AttackRandom.Next(0, 2) switch
                {
                    0 => new Color(110, 68, 200),
                    _ => NightmarePlantera.nightmareRed
                };

                PRTLoader.NewParticle(ctx.Npc.Center + Main.rand.NextVector2Circular(64, 64), Helper.NextVec2Dir(6, 24f),
                    CoraliteContent.ParticleType<BigFog>(), color, Scale: boss.NextAttackFloat(0.5f, 1.5f));
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            if (CurrentBeat != Beat.Recover || Timer <= NightmarePlanteraDirector.TpSparkleRecoverFrames)
            {
                return null;
            }

            ctx.Npc.velocity *= 0;
            return NPNightmareP3State.Commit(ctx);
        }
    }
}
