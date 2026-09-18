using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 幻影撕咬：绕着玩家高速公转，沿途撒只有 1 点伤害的假咬吓走位，每一轮结束才真咬一口；三轮之后本体扑上来咬。<br/>
    /// 节拍结构：公转撒假咬（ShootCount 帧）→ 真咬 → 再等 40 帧换轮，共三轮 → 淡出瞬移 → 扑咬 71 帧（第 10、20 帧各一口）→ 后摇 15 帧。<br/>
    /// 公平阀：假咬伤害 1、真咬前 50 帧张嘴预警、本体扑咬前的淡出与瞬移都是可见预告；ShootCount 每轮重抽（8×3 ~ 8×7），
    /// 所以真咬的时刻学不成固定拍子，但每一口都有自己的张嘴预警。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.illusionBite, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareIllusionBiteState : NPNightmareStateBase
    {
        private enum Beat
        {
            /// <summary>公转撒假咬</summary>
            Orbit,
            /// <summary>淡出瞬移到玩家身边</summary>
            Fade,
            /// <summary>扑咬</summary>
            Bite,
            /// <summary>后摇</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.illusionBite;

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>已经打完的公转轮数（槽 A）。旧代码用 <c>SonState</c> 兼任轮数与子拍，这里拆开。</summary>
        private int round;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);
            round = 0;
        }

        protected override void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Orbit:
                    OrbitBeat(ctx);
                    break;
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Bite:
                    BiteBeat(ctx);
                    break;
                default:
                    RecoverBeat(ctx);
                    break;
            }
        }

        /// <summary>绕玩家公转（400 帧一圈、半径 500），沿途撒假咬，本轮计时到点真咬一口。</summary>
        private void OrbitBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            float currentRot = boss.EXai1 + (Timer / (float)NightmarePlanteraDirector.IllusionOrbitPeriod * MathHelper.TwoPi);
            ctx.DeclareApproach(ctx.TargetCenter + (currentRot.ToRotationVector2() * NightmarePlanteraDirector.IllusionOrbitRadius),
                NightmarePlanteraDirector.IllusionOrbitTurn, NightmarePlanteraDirector.IllusionOrbitMaxSpeed,
                0.85f, NightmarePlanteraDirector.IllusionOrbitSpeedRange);
            ctx.DeclareRotationTowardsVelocity(0.3f);

            int shootCount = (int)ctx.ShootCount;

            if (Timer < shootCount && Timer % NightmarePlanteraDirector.IllusionFakeInterval == 0)
            {
                npc.NewProjectileDirectInAI_Server<NightmareBite>(
                    ctx.TargetCenter + boss.NextAttackVector2CircularEdge(NightmarePlanteraDirector.IllusionFakeRadius, NightmarePlanteraDirector.IllusionFakeRadius),
                    Vector2.Zero, NightmarePlanteraDirector.IllusionFakeDamage, 0, npc.target,
                    NightmarePlanteraDirector.IllusionFakeAi0, NightmarePlanteraDirector.IllusionFakeAi1, boss.ZenithProjSeed());
            }

            if (Timer == shootCount)
            {
                Vector2 pos = boss.PickTargetTeleportOffset(NightmarePlanteraDirector.P3BiteTeleportMin, NightmarePlanteraDirector.P3BiteTeleportMax);
                npc.NewProjectileDirectInAI_Server<NightmareBite>(
                    pos + boss.NextAttackVector2CircularEdge(NightmarePlanteraDirector.IllusionRealJitter, NightmarePlanteraDirector.IllusionRealJitter),
                    Vector2.Zero, NightmarePlanteraDirector.P3BiteDamage(), 0, npc.target,
                    NightmarePlanteraDirector.IllusionRealAi0, NightmarePlanteraDirector.IllusionRealAi1, boss.ZenithProjSeedNeg2());
                ctx.MarkDecision();
            }

            if (Timer <= shootCount + NightmarePlanteraDirector.IllusionRealTail)
            {
                return;
            }

            round++;
            boss.EXai1 = (npc.Center - ctx.TargetCenter).ToRotation();
            ctx.ShootCount = boss.PickIllusionBiteShootCount();
            SwitchBeat(ctx, round >= NightmarePlanteraDirector.IllusionRounds ? (int)Beat.Fade : (int)Beat.Orbit);
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (!FadeTickP3(ctx,
                () => ctx.Boss.PickTargetTeleportOffset(NightmarePlanteraDirector.P3BiteTeleportMin, NightmarePlanteraDirector.P3BiteTeleportMax),
                onTeleport: () => npc.NewProjectileInAI_Server<NightmareBite>(npc.Center, Vector2.Zero,
                    NightmarePlanteraDirector.P3BiteDamage(), 4, ai0: 0, ai1: NightmarePlanteraDirector.P3BiteAi1, ai2: ctx.Boss.ZenithProjSeedNeg2()),
                postTeleport: () => npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation()))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Bite);
        }

        /// <summary>扑咬本体：第 10、20 帧各一口，离得远就加速追、贴上就刹。</summary>
        private void BiteBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.MeleeDamage = true;

            if (Timer == NightmarePlanteraDirector.P3BiteFirstFrame || Timer == NightmarePlanteraDirector.P3BiteSecondFrame)
            {
                npc.NewProjectileInAI_Server<NightmareBite>(npc.Center, Vector2.Zero,
                    NightmarePlanteraDirector.P3BiteDamage(), 4, ai0: 0, ai1: NightmarePlanteraDirector.P3BiteAi1, ai2: ctx.Boss.ZenithProjSeedNeg2());
                ctx.MarkDecision();
            }

            LungeToTarget(ctx, 0.3f, NightmarePlanteraDirector.P3LungeDistance, NightmarePlanteraDirector.P3LungeAccel,
                NightmarePlanteraDirector.P3LungeMaxSpeed, NightmarePlanteraDirector.P3LungeDamp);

            if (Timer > NightmarePlanteraDirector.P3BiteBodyFrames)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
            }
        }

        private void RecoverBeat(NightmarePlanteraContext ctx)
        {
            ctx.DeclareDamp(NightmarePlanteraDirector.P3RecoverDamp);
            if (Timer > NightmarePlanteraDirector.P3RecoverRotFrame)
            {
                ctx.Boss.DoRotation(0.04f);
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.P3RecoverFrames
                ? NPNightmareP3State.Commit(ctx)
                : null;

        public override void WriteHot(NightmarePlanteraContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = round;
        }

        public override void ReadHot(NightmarePlanteraContext ctx)
        {
            base.ReadHot(ctx);
            round = (int)ctx.Hot[CoraliteBossHotSlots.A];
        }
    }
}
