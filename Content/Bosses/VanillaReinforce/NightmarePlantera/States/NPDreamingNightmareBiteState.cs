using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 梦境·噩梦之咬：梦境战斗（<c>useDreamMove</c>）里的咬。先半透明无敌地绕着美梦光盯梢，
    /// 再瞬移张嘴扑上去——伤害档位远高于常规二阶段，这是"没能保住美梦光"的惩罚。<br/>
    /// 节拍：盯梢 ShootCount → 淡出 45 → 扑咬 55 → 收招 20。<br/>
    /// 原 <c>DreamingNightmareBite</c>（Phase.P2_Dream.cs:1933-2051）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.dreamingNightmareBite, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamingNightmareBiteState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>盯梢</summary>
            Stalk,
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>扑咬</summary>
            Lunge,
            /// <summary>收招</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.dreamingNightmareBite;

        protected override bool DreamingVariant => true;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Stalk:
                    StalkBeat(ctx);
                    break;
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Lunge:
                    ctx.MeleeDamage = true;
                    DreamingChase(ctx, TargetOrSparkle(ctx), NightmarePlanteraDirector.P2DreamingBiteDistance);
                    if (Timer > NightmarePlanteraDirector.P2DreamingBiteLungeFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.Recover);
                    }

                    break;
                default:
                    RecoverBeat(ctx);
                    break;
            }
        }

        private void StalkBeat(NightmarePlanteraContext ctx)
        {
            DreamingStalk(ctx);

            if (Timer <= ctx.ShootCount)
            {
                return;
            }

            ctx.Boss.alpha = 1;
            SwitchBeat(ctx, (int)Beat.Fade);
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx, () => AimPos(ctx, NightmarePlanteraDirector.P2BiteTeleportMin, NightmarePlanteraDirector.P2BiteTeleportMax),
                onTeleport: () =>
                {
                    npc.NewProjectileInAI_Server<NightmareBite>(npc.Center, Vector2.Zero,
                        NightmarePlanteraDirector.P2DreamingBiteDamage(), 4,
                        ai0: 0, ai1: NightmarePlanteraDirector.P2DreamingBiteAi1, ai2: ctx.Boss.ZenithProjSeedNeg2());
                    ctx.MarkDecision();
                },
                postTeleport: () => npc.rotation = (TargetOrSparkle(ctx) - npc.Center).ToRotation()))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Lunge);
        }

        private void RecoverBeat(NightmarePlanteraContext ctx)
        {
            ctx.DeclareDamp(NightmarePlanteraDirector.P2DreamingBiteRecoverDamp);

            if (Timer == NightmarePlanteraDirector.P2DreamingBiteSoundFrame)
            {
                Helper.PlayPitched(CoraliteSoundID.BottleExplosion_Item107, ctx.Npc.Center, pitch: 1);
            }

            if (Timer > NightmarePlanteraDirector.P2DreamingBiteRotFrame)
            {
                ctx.DeclareRotationTowardsTarget(0.04f);
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.P2DreamingBiteRecoverFrames
                ? NPDreamP2State.CommitDreaming(ctx)
                : null;
    }
}
