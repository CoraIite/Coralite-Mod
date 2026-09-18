using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 噩梦之咬：淡出瞬移到远处，张嘴预警 75 帧后一路扑到脸上。二阶段最基础的一手。<br/>
    /// 节拍：淡出 45 帧 → 扑咬 66 帧 → 收招 20 帧。<br/>
    /// 场上有美梦光时扑击对象换成光而不是玩家——这是二阶段"引它去打光"的核心机制。<br/>
    /// 原 <c>NightmareBite</c> + 三个 <c>_Son</c>（Phase.P2_Dream.cs:183-257）与 BT 序列 <c>BuildNightmareBiteTree</c>。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.nightmareBite, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamNightmareBiteState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>扑咬</summary>
            Lunge,
            /// <summary>收招</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.nightmareBite;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Lunge:
                    ctx.MeleeDamage = true;
                    LungeToSparkleOrTarget(ctx);
                    if (Timer > NightmarePlanteraDirector.P2BiteLungeFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.Recover);
                    }

                    break;
                default:
                    RecoverBeat(ctx);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx, () => AimPos(ctx, NightmarePlanteraDirector.P2BiteTeleportMin, NightmarePlanteraDirector.P2BiteTeleportMax),
                onTeleport: () => Bite(ctx),
                postTeleport: () => npc.rotation = (TargetOrSparkle(ctx) - npc.Center).ToRotation()))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Lunge);
        }

        /// <summary>收招：刹车，第 10 帧起把头缓缓转回玩家。</summary>
        private void RecoverBeat(NightmarePlanteraContext ctx)
        {
            ctx.DeclareDamp(NightmarePlanteraDirector.P2BiteRecoverDamp);

            if (Timer > NightmarePlanteraDirector.P2BiteRecoverRotFrame)
            {
                ctx.DeclareRotationTowardsTarget(0.04f);
            }
        }

        internal static void Bite(NightmarePlanteraContext ctx, float ai1 = NightmarePlanteraDirector.P2BiteAi1)
        {
            NPC npc = ctx.Npc;
            npc.NewProjectileInAI_Server<NightmareBite>(npc.Center, Vector2.Zero,
                NightmarePlanteraDirector.P2BiteDamage(), 4, ai0: 0, ai1: ai1, ai2: ctx.Boss.ZenithProjSeed());
            ctx.MarkDecision();
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.P2BiteRecoverFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
