using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 美梦相助：与噩梦之咬同一套节拍，但预警拉到 125 帧、扑咬拖到 116 帧，并在瞬移那一刻放出第一只美梦光。<br/>
    /// 这是二阶段"打光"机制的教学关——超长的预警就是给玩家认识美梦光的时间，整场只出一次
    /// （<c>useFantasyHelp</c> 用完即关，见 <c>NightmarePlantera.AttackNet.cs</c> 的 <c>UseFantasyHelp</c>）。<br/>
    /// 原 <c>FantayHelp</c>（Phase.P2_Dream.cs:479-560）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.fantasyHelp, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamFantasyHelpState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移 + 放光</summary>
            Fade,
            /// <summary>扑咬</summary>
            Lunge,
            /// <summary>收招</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.fantasyHelp;

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
                    if (Timer > NightmarePlanteraDirector.P2FantasyHelpLungeFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.Recover);
                    }

                    break;
                default:
                    ctx.DeclareDamp(NightmarePlanteraDirector.P2BiteRecoverDamp);
                    if (Timer > NightmarePlanteraDirector.P2BiteRecoverRotFrame)
                    {
                        ctx.DeclareRotationTowardsTarget(0.04f);
                    }

                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx, () => AimPos(ctx, NightmarePlanteraDirector.P2BiteTeleportMin, NightmarePlanteraDirector.P2BiteTeleportMax),
                onTeleport: () =>
                {
                    NPDreamNightmareBiteState.Bite(ctx, NightmarePlanteraDirector.P2FantasyHelpAi1);
                    npc.NewNpcInAI_Server<FantasySparkle>(ctx.TargetCenter, npc.whoAmI,
                        NightmarePlanteraDirector.P2FantasyHelpSparkleAi0, target: npc.target);
                },
                postTeleport: () => npc.rotation = (TargetOrSparkle(ctx) - npc.Center).ToRotation()))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Lunge);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.P2BiteRecoverFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
