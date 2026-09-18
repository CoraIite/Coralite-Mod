using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 出生演出：吸空间裂缝 → 在雾里成形 → 破壳爆开。全程无敌，结束后交给一阶段。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.onSpawnAnmi_P0, typeof(NightmarePlanteraContext))]
    internal sealed class NPSpawnAnimState : NightmarePlanteraStateBase
    {
        private enum Beat
        {
            /// <summary>吸收裂缝</summary>
            Absorb,
            /// <summary>成形</summary>
            Form,
            /// <summary>破壳收尾</summary>
            Settle,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.onSpawnAnmi_P0;

        protected override int TimeoutFrames => NightmarePlanteraDirector.CinematicTimeoutFrames;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            ctx.Boss.UpdateFrameNormally();
            ctx.Invulnerable = true;

            switch (CurrentBeat)
            {
                case Beat.Absorb:
                    AbsorbBeat(ctx);
                    break;
                case Beat.Form:
                    FormBeat(ctx);
                    break;
                default:
                    SettleBeat(ctx);
                    break;
            }
        }

        private void AbsorbBeat(NightmarePlanteraContext ctx)
        {
            if (!Main.dedServ)
            {
                if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera camera) && camera.factor < 1)
                {
                    camera.useScreenMove = true;
                    camera.factor += NightmarePlanteraDirector.SpawnCameraZoomInStep;
                    if (camera.factor > 1)
                    {
                        camera.factor = 1;
                    }
                }

                SpawnFog(ctx, 2, 32f, 1, 3f, 0.5f, 1f);
            }

            if (Timer > NightmarePlanteraDirector.SpawnAbsorbFrames)
            {
                SoundEngine.PlaySound(CoraliteSoundID.ShieldDestroyed_NPCDeath58, ctx.Npc.Center);
                ctx.Boss.canDrawWarp = true;
                SwitchBeat(ctx, (int)Beat.Form);
            }
        }

        private void FormBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;
            npc.rotation += 0.03f;

            if (!Main.dedServ && Main.rand.NextBool())
            {
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(300, 300), ModContent.DustType<NightmareStar>(),
                    new Vector2(0, Main.rand.NextFromList(-1, 1) * 4), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 4f));
            }

            if (Timer < NightmarePlanteraDirector.SpawnFormFogFrames)
            {
                if (!Main.dedServ)
                {
                    SpawnFog(ctx, 2, 32f, 1, 3f, 0.5f, 1f);
                }

                boss.warpScale += NightmarePlanteraDirector.SpawnFormWarpStep;
                return;
            }

            if (Timer < NightmarePlanteraDirector.SpawnFormAlphaEndFrame)
            {
                boss.alpha += NightmarePlanteraDirector.SpawnFormAlphaStep;
                npc.rotation += NightmarePlanteraDirector.SpawnFormRotStep;
                return;
            }

            if (!Main.dedServ)
            {
                Vector2 dir = Helper.NextVec2Dir();
                Dust dust = Dust.NewDustPerfect(npc.Center + (dir * Main.rand.NextFloat(128f)), ModContent.DustType<NightmareDust>(),
                    dir * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 2f));
                dust.noGravity = true;

                if (Timer % 2 == 0)
                {
                    dir = Helper.NextVec2Dir();
                    Dust.NewDustPerfect(npc.Center + (dir * Main.rand.NextFloat(128f)), DustID.VilePowder,
                        dir * Main.rand.NextFloat(8f, 16f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 1.3f));
                }
            }

            if (Timer < NightmarePlanteraDirector.SpawnFormDustEndFrame)
            {
                return;
            }

            if (Timer < NightmarePlanteraDirector.SpawnFormFrames)
            {
                boss.warpScale -= NightmarePlanteraDirector.SpawnFormWarpStep;
            }
        }

        private void SettleBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.alpha += NightmarePlanteraDirector.SpawnSettleAlphaStep;
            boss.warpScale += NightmarePlanteraDirector.SpawnSettleWarpStep;
            ctx.Npc.rotation += NightmarePlanteraDirector.SpawnSettleRotStep;

            if (Main.dedServ)
            {
                return;
            }

            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera camera) && camera.factor > 0)
            {
                camera.useScreenMove = true;
                camera.factor -= NightmarePlanteraDirector.SpawnSettleCameraStep;
                if (camera.factor < 0)
                {
                    camera.factor = 0;
                }
            }

            SpawnFog(ctx, NightmarePlanteraDirector.SpawnSettleFogPerFrame, 64f, 8, 26f, 0.5f, 2f);
        }

        /// <summary>出生演出用的紫灰双色浓雾（纯本地）。</summary>
        private static void SpawnFog(NightmarePlanteraContext ctx, int count, float spread, float speedMin, float speedMax, float scaleMin, float scaleMax)
        {
            for (int i = 0; i < count; i++)
            {
                Color color = Main.rand.Next(0, 2) switch
                {
                    0 => new Color(110, 68, 200),
                    _ => new Color(122, 110, 134)
                };

                PRTLoader.NewParticle(ctx.Npc.Center + Main.rand.NextVector2Circular(spread, spread), Helper.NextVec2Dir(speedMin, speedMax),
                    CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(scaleMin, scaleMax));
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            switch (CurrentBeat)
            {
                case Beat.Absorb:
                    if (Timer > 2 && Timer % NightmarePlanteraDirector.SpawnEnergyInterval == 0)
                    {
                        SpawnEnergyRing(ctx, NightmarePlanteraDirector.SpawnEnergyMinDistance, NightmarePlanteraDirector.SpawnEnergyMaxDistance);
                    }

                    break;

                case Beat.Form:
                    if (Timer < NightmarePlanteraDirector.SpawnFormFogFrames)
                    {
                        if (Timer % NightmarePlanteraDirector.SpawnFormEnergyInterval == 0)
                        {
                            SpawnEnergyRing(ctx, NightmarePlanteraDirector.SpawnFormEnergyMinDistance, NightmarePlanteraDirector.SpawnFormEnergyMaxDistance);
                        }

                        break;
                    }

                    if (Timer > NightmarePlanteraDirector.SpawnFormFrames)
                    {
                        Burst(ctx);
                        SwitchBeat(ctx, (int)Beat.Settle);
                    }

                    break;

                default:
                    if (Timer > NightmarePlanteraDirector.SpawnSettleFrames)
                    {
                        if (!Main.dedServ && Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera camera))
                        {
                            camera.Reset();
                        }

                        return NPSleepingP1State.Commit(ctx);
                    }

                    break;
            }

            return null;
        }

        private static void SpawnEnergyRing(NightmarePlanteraContext ctx, int minDistance, int maxDistance)
        {
            NPC npc = ctx.Npc;
            float angle = Main.rand.NextFloat(MathHelper.TwoPi);
            int howMany = Main.rand.Next(NightmarePlanteraDirector.SpawnEnergyMaxCount);
            for (int i = 0; i < howMany; i++)
            {
                float distance = Main.rand.Next(minDistance, maxDistance);
                npc.NewProjectileInAI_Server<NightmareSpawnEnergy>(
                    npc.Center + ((angle + (i * MathHelper.TwoPi / howMany)).ToRotationVector2() * distance),
                    Vector2.Zero, 1, 1, -1, -1, distance, 1);
            }
        }

        private static void Burst(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, npc.Center, pitch: -0.5f);

            float angle = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < NightmarePlanteraDirector.SpawnBurstMainCount; i++)
            {
                npc.NewProjectileDirectInAI_Server<NightmareBurst>(npc.Center,
                    angle.ToRotationVector2() * NightmarePlanteraDirector.SpawnBurstMainSpeed, 1, 1, -1, angle);
                angle += MathHelper.TwoPi / NightmarePlanteraDirector.SpawnBurstMainCount;
            }

            for (int i = 0; i < NightmarePlanteraDirector.SpawnBurstExtraCount; i++)
            {
                float tor = Main.rand.NextFloat(6.282f);
                npc.NewProjectileDirectInAI_Server<NightmareBurst>(npc.Center,
                    tor.ToRotationVector2() * Main.rand.Next(NightmarePlanteraDirector.SpawnBurstExtraSpeedMin, NightmarePlanteraDirector.SpawnBurstExtraSpeedMax),
                    1, 1, -1, tor);
            }

            if (!Main.dedServ && Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera camera))
            {
                camera.useShake = true;
                camera.shakeLevel = NightmarePlanteraDirector.SpawnShakeLevel;
                camera.ShakeVec2 = Helper.NextVec2Dir();
            }

            ctx.Boss.Music = MusicID.Plantera;
        }
    }
}
