using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 梦境·美梦猎杀：半透明盯梢一段时间后现身，隔着 360 的距离朝美梦光连续点射红光——
    /// boss 在主动击杀美梦光，玩家要在这 120 帧里把它护住。<br/>
    /// 节拍：盯梢 ShootCount → 点射 120。<br/>
    /// 原 <c>DreamingFantasyHunting</c>（Phase.P2_Dream.cs:2163-2243）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.dreamingFantasyHunting, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamingFantasyHuntingState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>盯梢</summary>
            Stalk,
            /// <summary>点射</summary>
            Shoot,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.dreamingFantasyHunting;

        protected override bool DreamingVariant => true;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            if (CurrentBeat == Beat.Stalk)
            {
                StalkBeat(ctx);
                return;
            }

            ShootBeat(ctx);
        }

        private void StalkBeat(NightmarePlanteraContext ctx)
        {
            DreamingStalk(ctx);

            if (Timer <= ctx.ShootCount)
            {
                return;
            }

            ctx.Boss.alpha = 1;
            SwitchBeat(ctx, (int)Beat.Shoot);
        }

        private void ShootBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            Vector2 pos = TargetOrSparkle(ctx);

            DreamingChase(ctx, pos, NightmarePlanteraDirector.P2DreamingHuntDistance);

            if (Timer % NightmarePlanteraDirector.P2DreamingHuntInterval != 0)
            {
                return;
            }

            // 抽取必须在权威端守卫之外：旧代码整块包在 netMode 判断里，客户端少走一次 AttackRandom 就会整体错位（C3）。
            int howmany = boss.NextAttackFromList(1, 3);
            float baseRot = (pos - npc.Center).ToRotation() - (howmany / 2 * NightmarePlanteraDirector.P2DreamingHuntSpread);
            int damage = NightmarePlanteraDirector.P2DreamingHuntDamage();

            for (int i = 0; i < howmany; i++)
            {
                npc.NewProjectileInAI_Server<NightmareSparkle_Red>(npc.Center,
                    (baseRot + (i * NightmarePlanteraDirector.P2DreamingHuntSpread)).ToRotationVector2(), damage, 0);
            }

            ctx.MarkDecision();
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Shoot && Timer > NightmarePlanteraDirector.P2DreamingHuntFrames
                ? NPDreamP2State.CommitDreaming(ctx)
                : null;
    }
}
