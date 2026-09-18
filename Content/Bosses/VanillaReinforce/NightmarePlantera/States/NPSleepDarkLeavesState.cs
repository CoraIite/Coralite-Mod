using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 黑暗飞叶：按间隔连吐叶片，每第 4 发换成三连扇形。血越少间隔越短、持续越长。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.darkLeaves, typeof(NightmarePlanteraContext))]
    internal sealed class NPSleepDarkLeavesState : NightmarePlanteraStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.darkLeaves;

        /// <summary>射击间隔与总时长都只看血量，两端算得出同样的值，不用过线。</summary>
        private static int ShootDelay(NPC npc)
        {
            int delay = Helper.ScaleValueForDiffMode(12, 11, 10, 8);
            if (npc.life < npc.lifeMax * NightmarePlanteraDirector.P1Tier1LifeRatio)
            {
                delay--;
            }

            if (npc.life < npc.lifeMax * NightmarePlanteraDirector.P1Tier2LifeRatio)
            {
                delay--;
            }

            return delay;
        }

        private static int Duration(NPC npc)
        {
            int timeMax = NightmarePlanteraDirector.LeafDurationStep;
            if (npc.life < npc.lifeMax * NightmarePlanteraDirector.P1Tier1LifeRatio)
            {
                timeMax += NightmarePlanteraDirector.LeafDurationStep;
            }

            if (npc.life < npc.lifeMax * NightmarePlanteraDirector.P1Tier2LifeRatio)
            {
                timeMax += NightmarePlanteraDirector.LeafDurationStep;
            }

            return timeMax;
        }

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NPSleepingP1State.Phase1Tick(ctx);
            ctx.MeleeDamage = true;
            ctx.DeclareRotationTowardsTarget(0.1f);

            if (Timer % ShootDelay(ctx.Npc) == 0)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_BlowgunPlus_Item65, ctx.Npc.Center);
                ctx.Npc.velocity = -ctx.Npc.rotation.ToRotationVector2() * NightmarePlanteraDirector.LeafRecoil;
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            int shootDelay = ShootDelay(npc);

            if (Timer % shootDelay == 0)
            {
                Vector2 pos = ctx.Boss.GetPhase1MousePos();
                Vector2 dir = npc.rotation.ToRotationVector2();
                int damage = Helper.ScaleValueForDiffMode(40, 35, 30, 25);

                if (Timer % (shootDelay * NightmarePlanteraDirector.LeafVolleyEvery) == 0)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), pos,
                            dir.RotatedBy(i * NightmarePlanteraDirector.LeafVolleySpread) * NightmarePlanteraDirector.LeafSpeed,
                            ModContent.ProjectileType<DarkLeaf>(), damage, 4, npc.target, 1);
                    }
                }
                else
                {
                    Vector2 vel = dir * NightmarePlanteraDirector.LeafSpeed;
                    if (Main.rand.NextBool(NightmarePlanteraDirector.LeafJitterChance))
                    {
                        vel = vel.RotatedBy(Main.rand.NextFromList(-NightmarePlanteraDirector.LeafJitter, NightmarePlanteraDirector.LeafJitter));
                    }

                    Projectile.NewProjectile(npc.GetSource_FromAI(), pos, vel,
                        ModContent.ProjectileType<DarkLeaf>(), damage, 4, npc.target);
                }

                ctx.MarkDecision();
            }

            return Timer > Duration(npc) ? EndAttack(ctx) : null;
        }
    }
}
