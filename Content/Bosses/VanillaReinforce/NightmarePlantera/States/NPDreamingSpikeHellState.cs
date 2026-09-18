using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 梦境·尖刺地狱：贴着美梦光追，追满 75 帧后每 20 帧朝它钉一根长刺（玩家挡在中间就会吃到），
    /// 美梦光一旦消失立刻进收招；收招时自转着朝匀速旋进的方向连钉七根。<br/>
    /// 节拍：淡出 30 → 追击 500（或美梦光消失）→ 收招 50。<br/>
    /// 原 <c>DreamingSpikeHell</c>（Phase.P2_Dream.cs:2073-2161）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.dreamingSpikeHell, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamingSpikeHellState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>追击钉刺</summary>
            Chase,
            /// <summary>收招连钉</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.dreamingSpikeHell;

        protected override bool DreamingVariant => true;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Chase:
                    ctx.MeleeDamage = true;
                    ChaseBeat(ctx);
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
                fadeTime: NightmarePlanteraDirector.P2DreamingHellFadeFrames,
                postTeleport: () => npc.rotation = (TargetOrSparkle(ctx) - npc.Center).ToRotation()))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Chase);
        }

        private void ChaseBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            bool sparkleAlive = NightmarePlantera.FantasySparkleAlive(out NPC fs);
            Vector2 pos = sparkleAlive ? fs.Center : ctx.TargetCenter;

            // 美梦光没了就没有追击对象，这一帧照旧跑完再进收招（旧代码就是这个顺序）。
            if (!sparkleAlive)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
            }

            DreamingChase(ctx, pos, NightmarePlanteraDirector.P2DreamingHellChaseDistance);

            if (Timer > NightmarePlanteraDirector.P2DreamingHellShootStart
                && Timer % NightmarePlanteraDirector.P2DreamingHellShootInterval == 0)
            {
                npc.NewProjectileInAI_Server<ConfusionHole>(npc.Center, (pos - npc.Center).SafeNormalize(Vector2.Zero),
                    NightmarePlanteraDirector.P2DreamingHellHoleDamage(), 0, npc.target,
                    NightmarePlanteraDirector.P2DreamingHellHoleTime, ctx.Boss.ZenithProjSeedNeg2(),
                    NightmarePlanteraDirector.P2DreamingHellHoleLen);
                ctx.MarkDecision();
            }

            if (Timer > NightmarePlanteraDirector.P2DreamingHellChaseFrames)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
            }
        }

        private void RecoverBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.rotation += NightmarePlanteraDirector.P2DreamingHellSpinStep;

            if (Timer % NightmarePlanteraDirector.P2DreamingHellBurstInterval == 0)
            {
                npc.NewProjectileInAI_Server<ConfusionHole>(npc.Center,
                    (Timer * MathHelper.TwoPi / NightmarePlanteraDirector.P2DreamingHellBurstPeriod).ToRotationVector2(),
                    NightmarePlanteraDirector.P2DreamingHellBurstDamage(), 0, npc.target,
                    NightmarePlanteraDirector.P2DreamingHellHoleTime, ctx.Boss.ZenithProjSeedNeg2(),
                    NightmarePlanteraDirector.P2DreamingHellHoleLen);
                ctx.MarkDecision();
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.P2DreamingHellRecoverFrames
                ? NPDreamP2State.CommitDreaming(ctx)
                : null;
    }
}
