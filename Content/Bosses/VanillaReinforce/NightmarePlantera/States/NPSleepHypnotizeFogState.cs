using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 沉眠之雾：嘴前收束一圈尘土蓄力 160 帧，然后吐出三团催眠雾并后坐。
    /// 吃到雾会涨噩梦值，涨满就是处决——所以蓄力圈必须看得见，别把它挪进权威端。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.hypnotizeFog, typeof(NightmarePlanteraContext))]
    internal sealed class NPSleepHypnotizeFogState : NightmarePlanteraStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.hypnotizeFog;

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NPSleepingP1State.Phase1Tick(ctx);
            ctx.MeleeDamage = true;
            ctx.DeclareRotationTowardsTarget(0.1f);

            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (Timer < NightmarePlanteraDirector.FogChargeFrames)
            {
                if (Main.dedServ)
                {
                    return;
                }

                Vector2 pos = boss.GetPhase1MousePos();
                float factor = Timer / (float)NightmarePlanteraDirector.FogChargeFrames;
                float width = NightmarePlanteraDirector.FogRingRadius - (factor * NightmarePlanteraDirector.FogRingShrink);

                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2CircularEdge(width, width),
                        ModContent.DustType<NightmareDust>(), Scale: Main.rand.NextFloat(1f, 1.4f));
                    dust.velocity = ((pos - dust.position).SafeNormalize(Vector2.Zero) * (3 - (factor * 3))) + (npc.velocity * factor);
                    dust.noGravity = true;
                }

                Dust.NewDustPerfect(pos, DustID.VilePowder, Helper.NextVec2Dir(1f, 3), Scale: Main.rand.NextFloat(1f, 2f));
                return;
            }

            if (Timer == NightmarePlanteraDirector.FogChargeFrames)
            {
                SoundEngine.PlaySound(CoraliteSoundID.SpiritFlame_Item117, npc.Center);
                npc.velocity = -npc.rotation.ToRotationVector2() * NightmarePlanteraDirector.FogRecoil;
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (Timer == NightmarePlanteraDirector.FogChargeFrames)
            {
                Vector2 pos = ctx.Boss.GetPhase1MousePos();
                Vector2 dir = npc.rotation.ToRotationVector2();
                int damage = Helper.ScaleValueForDiffMode(40, 35, 30, 25);
                for (int i = -1; i < 2; i++)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), pos,
                        dir.RotatedBy(i * NightmarePlanteraDirector.FogSpread)
                            * Main.rand.NextFloat(NightmarePlanteraDirector.FogSpeedMin, NightmarePlanteraDirector.FogSpeedMax),
                        ModContent.ProjectileType<HypnotizeFog>(), damage, 4, ctx.Target.whoAmI);
                }

                ctx.MarkDecision();
            }

            return Timer >= NightmarePlanteraDirector.FogEndFrames ? EndAttack(ctx) : null;
        }
    }
}
