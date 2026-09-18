using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 黑暗之触：瞄准 30 帧后吐出抓取触手，血越少同时吐得越多、缠得越久。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.darkTentacle, typeof(NightmarePlanteraContext))]
    internal sealed class NPSleepDarkTentacleState : NightmarePlanteraStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.darkTentacle;

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NPSleepingP1State.Phase1Tick(ctx);
            ctx.MeleeDamage = true;

            if (Timer < NightmarePlanteraDirector.TentacleAimFrames)
            {
                ctx.DeclareRotationTowardsTarget(0.1f);
                return;
            }

            if (Timer == NightmarePlanteraDirector.TentacleAimFrames)
            {
                SoundEngine.PlaySound(CoraliteSoundID.SpiderStaff_Item83, ctx.Npc.Center);
                ctx.Npc.velocity = -ctx.Npc.rotation.ToRotationVector2() * NightmarePlanteraDirector.TentacleRecoil;
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (Timer == NightmarePlanteraDirector.TentacleAimFrames)
            {
                int notFreeTime = NightmarePlanteraDirector.TentacleNotFreeTime;
                int maxFreeTime = NightmarePlanteraDirector.TentacleMaxFreeTime;
                int extraTentacle = 0;

                if (npc.life < npc.lifeMax * NightmarePlanteraDirector.P1Tier1LifeRatio)
                {
                    notFreeTime += NightmarePlanteraDirector.TentacleNotFreeStep;
                    maxFreeTime += NightmarePlanteraDirector.TentacleMaxFreeStep;
                    extraTentacle += NightmarePlanteraDirector.TentacleExtraStep;
                }

                if (npc.life < npc.lifeMax * NightmarePlanteraDirector.P1Tier2LifeRatio)
                {
                    notFreeTime += NightmarePlanteraDirector.TentacleNotFreeStep;
                    maxFreeTime += NightmarePlanteraDirector.TentacleMaxFreeStep;
                    extraTentacle += NightmarePlanteraDirector.TentacleExtraStep;
                }

                int index = npc.NewNpcInAI_Server<NightmareCatcher>(npc.Center, npc.whoAmI, ai2: notFreeTime, ai3: maxFreeTime);
                Main.npc[index].velocity = npc.rotation.ToRotationVector2() * NightmarePlanteraDirector.TentacleSpeed;

                for (int i = 0; i < extraTentacle; i++)
                {
                    int index2 = npc.NewNpcInAI_Server<NightmareCatcher>(npc.Center, npc.whoAmI,
                        ai2: notFreeTime * NightmarePlanteraDirector.TentacleExtraTimeScale,
                        ai3: maxFreeTime * NightmarePlanteraDirector.TentacleExtraTimeScale);
                    float angle = Main.rand.NextFromList(-NightmarePlanteraDirector.TentacleExtraAngle, NightmarePlanteraDirector.TentacleExtraAngle)
                        + Main.rand.NextFloat(-NightmarePlanteraDirector.TentacleExtraAngleJitter, NightmarePlanteraDirector.TentacleExtraAngleJitter);
                    Main.npc[index2].velocity = (npc.rotation + angle).ToRotationVector2() * NightmarePlanteraDirector.TentacleSpeed;
                }

                ctx.MarkDecision();
            }

            return Timer >= NightmarePlanteraDirector.TentacleEndFrames ? EndAttack(ctx) : null;
        }
    }
}
