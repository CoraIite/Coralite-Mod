using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 尖刺球：瞬移到玩家头顶后绕大圈巡航，每 20 帧撒一颗延迟起爆的黑洞球、每 30 帧补一发噩梦光；
    /// 撒满 110 帧后进入 280 帧的收招段，此时先前撒下的球陆续炸开，场地被逐步封死。<br/>
    /// 节拍：淡出 45 帧 → 播种 110 帧 → 收招 280 帧。<br/>
    /// 公平阀：每颗球的起爆延迟随机 0~360 帧、半径 200，本体全程在远处绕圈不逼身，玩家是在"读场"而不是"读招"。<br/>
    /// 原 <c>SpikeBall</c>（Phase.P2_Dream.cs:909-976）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.spikeBalls, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamSpikeBallsState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>撒球</summary>
            Sow,
            /// <summary>收招</summary>
            Tail,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.spikeBalls;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Sow:
                    SowBeat(ctx);
                    break;
                default:
                    TailBeat(ctx);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx,
                () => ctx.TargetCenter + new Vector2(0, -boss.NextAttackFloat(NightmarePlanteraDirector.P2SpikeBallTeleportMin, NightmarePlanteraDirector.P2SpikeBallTeleportMax)),
                onTeleport: () =>
                {
                    npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
                    npc.velocity *= 0;
                }))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Sow);
        }

        private void SowBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;

            ctx.DeclareRotationTowardsTarget(0.1f);
            Circle(ctx);

            if (Timer % NightmarePlanteraDirector.P2SpikeBallHoleInterval == 0)
            {
                // 两个抽取必须在权威端守卫之外求值：旧代码整块包在 netMode 判断里，客户端因此少走两次
                // AttackRandom，后面所有招式的随机序列都会跟着分叉（C3）。生成本身由 *_Server 兜住。
                float jitter = boss.NextAttackFloat(-NightmarePlanteraDirector.P2SpikeBallHoleJitter, NightmarePlanteraDirector.P2SpikeBallHoleJitter);
                float delay = Timer + boss.AttackRandom.Next(0, NightmarePlanteraDirector.P2SpikeBallHoleDelayMax);

                npc.NewProjectileInAI_Server<BlackHole>(npc.Center, npc.rotation.ToRotationVector2().RotatedBy(jitter),
                    NightmarePlanteraDirector.P2SparkleDamage(), 0, npc.target, delay, NightmarePlanteraDirector.P2SpikeBallHoleRadius);
                ctx.MarkDecision();
            }

            Sparkle(ctx, NightmarePlanteraDirector.P2SpikeBallSparkleInterval);

            if (Timer > NightmarePlanteraDirector.P2SpikeBallSowFrames)
            {
                SwitchBeat(ctx, (int)Beat.Tail);
            }
        }

        /// <summary>收招：继续绕圈，30 帧起每 30 帧补一发光，把先前撒的球炸干净的时间撑满。</summary>
        private void TailBeat(NightmarePlanteraContext ctx)
        {
            ctx.DeclareRotationTowardsTarget(0.3f);
            Circle(ctx);

            if (Timer > NightmarePlanteraDirector.P2SpikeBallSparkleInterval)
            {
                Sparkle(ctx, NightmarePlanteraDirector.P2SpikeBallSparkleInterval);
            }
        }

        private void Circle(NightmarePlanteraContext ctx)
            => CircleMovement(ctx, Timer, NightmarePlanteraDirector.P2SpikeBallCircleDistance, NightmarePlanteraDirector.P2SpikeBallCircleSpeed,
                NightmarePlanteraDirector.P2SpikeBallCircleAccel, NightmarePlanteraDirector.P2SpikeBallCircleRolling);

        private void Sparkle(NightmarePlanteraContext ctx, int interval)
        {
            if (Timer % interval != 0)
            {
                return;
            }

            NPC npc = ctx.Npc;
            npc.NewProjectileInAI_Server<NightmareSparkle_Normal>(npc.Center, npc.rotation.ToRotationVector2(),
                NightmarePlanteraDirector.P2SparkleDamage(), 0);
            ctx.MarkDecision();

            Helper.PlayPitched(CoraliteSoundID.CrystalSerpent_Item109, npc.Center, pitch: -0.5f);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Tail && Timer > NightmarePlanteraDirector.P2SpikeBallTailFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
