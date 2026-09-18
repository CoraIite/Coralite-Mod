using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 梦境之光：绕着玩家慢慢公转，分两波在玩家周围摆出 14 发会滚动收束的噩梦光；第二段里其中一发会变成美梦光，
    /// 玩家打下它就能推进梦境进度（不打就吃满第二段的尖刺洞）。这是二阶段唯一"给你选择"的一手。<br/>
    /// 节拍：贴脸瞬移 45 → 布光 210（第 10 / 160 帧各一波）→ 布光带惩罚 200（第 80 帧一波、每 20 帧四向尖刺）。<br/>
    /// 原 <c>DreamSparkle</c>（Phase.P2_Dream.cs:1780-1900）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.dreamSparkle, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamSparkleState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>贴脸瞬移</summary>
            Fade,
            /// <summary>第一段布光</summary>
            Ring,
            /// <summary>第二段布光 + 尖刺惩罚</summary>
            Exchange,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.dreamSparkle;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Ring:
                    RingBeat(ctx);
                    break;
                default:
                    ExchangeBeat(ctx);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx,
                () => ctx.Boss.PickTargetTeleportOffset(NightmarePlanteraDirector.P2TpNearMin, NightmarePlanteraDirector.P2TpNearMax),
                postTeleport: () =>
                {
                    npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
                    npc.velocity = (npc.rotation + MathHelper.Pi).ToRotationVector2();
                    ctx.ShootCount = npc.rotation + MathHelper.Pi;
                }))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Ring);
        }

        private void RingBeat(NightmarePlanteraContext ctx)
        {
            Orbit(ctx);
            ctx.DeclareRotationTowardsVelocity(0.3f);

            if (Timer == NightmarePlanteraDirector.P2DreamRingFrameA || Timer == NightmarePlanteraDirector.P2DreamRingFrameB)
            {
                SparkleRing(ctx, NightmarePlanteraDirector.P2DreamRingRollMin, NightmarePlanteraDirector.P2DreamRingRollMaxExclusive, -1);
            }

            if (Timer > NightmarePlanteraDirector.P2DreamBodyFrames)
            {
                SwitchBeat(ctx, (int)Beat.Exchange);
            }
        }

        /// <summary>
        /// 第二段：旧代码在这里算了一整套绕行数学却<b>没有把结果写回 velocity</b>，本体实际是靠惯性滑行的。
        /// 这是既有手感的一部分，原样保留（D10）——不要"顺手修好"。
        /// </summary>
        private void ExchangeBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;

            if (Timer == NightmarePlanteraDirector.P2DreamExchangeFrame)
            {
                SparkleRing(ctx, NightmarePlanteraDirector.P2DreamExchangeRollMin, NightmarePlanteraDirector.P2DreamExchangeRollMaxExclusive,
                    boss.AttackRandom.Next(NightmarePlanteraDirector.P2DreamRingCount));
            }

            if (Timer < NightmarePlanteraDirector.P2DreamHoleStopFrame && Timer % NightmarePlanteraDirector.P2DreamHoleInterval == 0)
            {
                float angle = (npc.Center - ctx.TargetCenter).ToRotation();
                int damage = NightmarePlanteraDirector.P2SparkleDamage();

                for (int i = 0; i < NightmarePlanteraDirector.P2DreamHoleCount; i++)
                {
                    float spoke = angle + (i * MathHelper.PiOver2);
                    npc.NewProjectileDirectInAI_Server<ConfusionHole>(
                        ctx.TargetCenter + (spoke.ToRotationVector2() * NightmarePlanteraDirector.P2DreamHoleDistance),
                        // 旧 Timer 是 float（ai[3]），这里的除法要保持浮点，否则四根刺的旋进会变成阶梯。
                        (spoke + MathHelper.Pi + (Timer / NightmarePlanteraDirector.P2DreamHoleDriftPeriod * NightmarePlanteraDirector.P2DreamHoleDrift)).ToRotationVector2(),
                        damage, 0, npc.target, NightmarePlanteraDirector.P2DreamHoleTime, boss.ZenithProjSeed(),
                        NightmarePlanteraDirector.P2DreamHoleLen);
                }

                ctx.MarkDecision();
            }
        }

        /// <summary>绕玩家的公转，两段共用。</summary>
        private void Orbit(NightmarePlanteraContext ctx)
        {
            float currentRot = ctx.ShootCount + (Timer / NightmarePlanteraDirector.P2DreamOrbitPeriod * MathHelper.TwoPi);
            ctx.DeclareApproach(ctx.TargetCenter + (currentRot.ToRotationVector2() * NightmarePlanteraDirector.P2DreamOrbitRadius),
                NightmarePlanteraDirector.P2DreamOrbitTurn, NightmarePlanteraDirector.P2DreamOrbitMaxSpeed,
                NightmarePlanteraDirector.P2DreamOrbitBlend, NightmarePlanteraDirector.P2DreamOrbitSpeedRange);
        }

        /// <summary>
        /// 在玩家周围半径 150 处摆一圈 14 发滚动噩梦光，逐发延迟 6 帧形成扫过去的感觉。<br/>
        /// <paramref name="exchangeIndex"/> ≥ 0 时那一发会变成可被打下的美梦光（ai1 = 1）。
        /// </summary>
        private static void SparkleRing(NightmarePlanteraContext ctx, int rollMin, int rollMaxExclusive, int exchangeIndex)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            int damage = NightmarePlanteraDirector.P2SparkleDamage();
            int rollingTime = boss.AttackRandom.Next(rollMin, rollMaxExclusive);
            float rot = boss.NextAttackFloat(MathHelper.TwoPi);
            bool exchanged = false;

            for (int i = 0; i < NightmarePlanteraDirector.P2DreamRingCount; i++)
            {
                Vector2 pos = ctx.TargetCenter
                    + (((i * MathHelper.Pi / NightmarePlanteraDirector.P2DreamRingCount) + rot).ToRotationVector2() * NightmarePlanteraDirector.P2DreamRingRadius);

                float ai1 = 0;
                if (!exchanged && i == exchangeIndex)
                {
                    exchanged = true;
                    ai1 = 1;
                }

                npc.NewProjectileInAI_Server<NightmareSparkle_Rolling>(pos, Vector2.Zero, damage, 0, npc.target,
                    rollingTime + (i * NightmarePlanteraDirector.P2DreamRingStepDelay), ai1, NightmarePlanteraDirector.P2DreamRingRange);
            }

            ctx.MarkDecision();
            Helper.PlayPitched(CoraliteSoundID.EmpressOfLight_Dash_Item160, npc.Center, volumeAdjust: -0.2f, pitch: -1f);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Exchange && Timer > NightmarePlanteraDirector.P2DreamTailFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
