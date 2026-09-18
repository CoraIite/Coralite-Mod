using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 噩梦撕咬：淡出瞬移到玩家身边张嘴，然后扑上来咬三口。三阶段最基础的一手，任何距离任何高度都能起手，
    /// 所以它同时是 hub 的无门槛末位保底。<br/>
    /// 节拍结构：淡出 30 帧 → 扑咬 72 帧（落地那口 + 第 10、20 帧各一口）→ 后摇 20 帧。<br/>
    /// 公平阀：淡出与瞬移都是可见预告，咬击弹幕自带 60 帧张嘴时间；扑咬阶段贴脸会刹车，不会无限碾。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.p3_nightmareBite, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareBiteState : NPNightmareStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>扑咬</summary>
            Bite,
            /// <summary>后摇</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.p3_nightmareBite;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            switch (CurrentBeat)
            {
                case Beat.Fade:
                    if (FadeTickP3(ctx,
                        () => ctx.Boss.PickTargetTeleportOffset(NightmarePlanteraDirector.P3BiteTeleportMin, NightmarePlanteraDirector.P3BiteTeleportMax),
                        onTeleport: () => OnTeleportBite(ctx),
                        postTeleport: () => npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation()))
                    {
                        SwitchBeat(ctx, (int)Beat.Bite);
                    }

                    break;

                case Beat.Bite:
                    ctx.MeleeDamage = true;

                    if (Timer == NightmarePlanteraDirector.P3BiteFirstFrame || Timer == NightmarePlanteraDirector.P3BiteSecondFrame)
                    {
                        Bite(ctx);
                    }

                    LungeToTarget(ctx, 0.3f, NightmarePlanteraDirector.P3LungeDistance, NightmarePlanteraDirector.P3LungeAccel,
                        NightmarePlanteraDirector.P3LungeMaxSpeed, NightmarePlanteraDirector.P3LungeDamp);

                    if (Timer > NightmarePlanteraDirector.P3NightmareBiteBodyFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.Recover);
                    }

                    break;

                default:
                    ctx.DeclareDamp(NightmarePlanteraDirector.P3RecoverDamp);
                    if (Timer > NightmarePlanteraDirector.P3NightmareBiteRotFrame)
                    {
                        ctx.Boss.DoRotation(0.04f);
                    }

                    break;
            }
        }

        /// <summary>落地那一口，附带一声冲刺音——这是"我要咬了"的听觉预告。</summary>
        private static void OnTeleportBite(NightmarePlanteraContext ctx)
        {
            Bite(ctx);
            if (!Main.dedServ)
            {
                Helper.PlayPitched(CoraliteSoundID.EmpressOfLight_Dash_Item160, ctx.Npc.Center, pitch: -1f, volumeAdjust: -0.2f);
            }
        }

        private static void Bite(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.NewProjectileInAI_Server<NightmareBite>(npc.Center, Vector2.Zero,
                NightmarePlanteraDirector.P3BiteDamage(), 4, ai0: 0, ai1: NightmarePlanteraDirector.P3BiteAi1,
                ai2: ctx.Boss.ZenithProjSeedNeg2());
            ctx.MarkDecision();
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.P3NightmareBiteRecoverFrames
                ? NPNightmareP3State.Commit(ctx)
                : null;
    }
}
