using Coralite.Content.Bosses.ThunderveinDragon.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ThunderveinDragon.States
{
    /// <summary>
    /// 一二阶段切换演出（由 <c>PhaseController</c> 在血量阈值上、且当前处于可打断招式时触发，不经加权表）。<br/>
    /// 节拍：Ready（收翅 26 帧，残影两段由外向内收束 2.5→1、1.5→1，是“它在攒东西”的预告）
    /// → Roar（吼叫 85 帧，40 帧内电粒子环由 80 px 铺到 1400 px，残影反向 1→2.5 炸开）。<br/>
    /// 公平阀：全程不出手、不伤害，玩家有约 110 帧回血换位。<br/>
    /// 旧 AI.ExchangeP1_P2.cs:12-115
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.ExchangeP1_P2, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinExchangeP1P2State : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.ExchangeP1_P2;

        private enum Beat
        {
            /// <summary>收翅蓄力（旧 SonState 0）</summary>
            Ready = 0,
            /// <summary>吼叫（旧 1）</summary>
            Roar = 1,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        private bool roarThisFrame;

        public override void OnEnter(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            base.OnEnter(machine, ctx);

            // 本态不经 hub 的 Commit（PhaseController 直接换态），自行过账，保证轮换记账不漏手
            if (!VaultUtils.isClient)
            {
                ctx.RecordPick((int)StateIndex);
            }
        }

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            roarThisFrame = false;
            ctx.DrawShadows = true;

            if (CurrentBeat == Beat.Roar)
            {
                UpdateRoar(ctx);
                return;
            }

            UpdateReady(ctx);
        }

        private void UpdateReady(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareDamp(ThunderveinDirector.ExchangeReadyDamp);
            npc.QuickSetDirection();
            ctx.DeclareNoRot();

            // 起拍前等翅膀帧回到 4，期间不计时
            if (T < ThunderveinDirector.ExchangeWaitFrames && npc.frame.Y != 4)
            {
                ctx.Boss.FlyingFrame();
                HoldTimer();
                return;
            }

            if (T < ThunderveinDirector.ExchangeWaitFrames)
            {
                ctx.Boss.ResetAllOldCaches();
            }

            ctx.Boss.UpdateAllOldCaches();

            if (T < ThunderveinDirector.ExchangeFirstChargeFrames)
            {
                float factor = T / (float)ThunderveinDirector.ExchangeFirstChargeFrames;
                ctx.ShadowScale = Helper.Lerp(ThunderveinDirector.ExchangeFirstShadowScaleFrom, 1f, factor);
                ctx.ShadowAlpha = Helper.Lerp(0f, 1f, factor);
            }
            else if (T < ThunderveinDirector.ExchangeFirstChargeFrames + ThunderveinDirector.ExchangeSecondChargeFrames)
            {
                float factor = (T - ThunderveinDirector.ExchangeFirstChargeFrames) / (float)ThunderveinDirector.ExchangeSecondChargeFrames;
                ctx.ShadowScale = Helper.Lerp(ThunderveinDirector.ExchangeSecondShadowScaleFrom, 1f, factor);
                ctx.ShadowAlpha = Helper.Lerp(0f, 1f, factor);
            }

            if (Timer <= ThunderveinDirector.ExchangeFirstChargeFrames + ThunderveinDirector.ExchangeSecondChargeFrames)
            {
                return;
            }

            npc.frame.Y = 0;
            npc.frame.X = 1;
            ctx.ShadowScale = ThunderveinDirector.ExchangeRoarShadowScale;
            roarThisFrame = true;
            PlayRoarSound(ctx);
            SwitchBeat(ctx, (int)Beat.Roar);
        }

        private static void PlayRoarSound(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, ctx.Npc.Center, pitch: ThunderveinDirector.RoarPitch);
            SoundEngine.PlaySound(CoraliteSoundID.Roar, ctx.Npc.Center);
        }

        private void UpdateRoar(ThunderveinDragonContext ctx)
        {
            ctx.DeclareKeep();
            ctx.Boss.UpdateAllOldCaches();

            if (T < ThunderveinDirector.ExchangeRoarFrames)
            {
                // 残影反向炸开：两端同算（纯表现量，不过线）
                float factor = T / (float)ThunderveinDirector.ExchangeRoarFrames;
                ctx.ShadowScale = Helper.Lerp(1f, ThunderveinDirector.ExchangeRoarShadowScaleTo, factor);
                ctx.ShadowAlpha = Helper.Lerp(1f, 0f, factor);
                RoarFx(ctx);
            }

            if (Main.dedServ || T >= ThunderveinDirector.ExchangeBurstFrames)
            {
                return;
            }

            float length = Helper.Lerp(ThunderveinDirector.ExchangeRingRadiusFrom, ThunderveinDirector.ExchangeRingRadiusTo,
                T / (float)ThunderveinDirector.ExchangeBurstFrames);
            for (int i = 0; i < ThunderveinDirector.ExchangeParticlesPerFrame; i++)
            {
                PRTLoader.NewParticle(ctx.Npc.Center + Main.rand.NextVector2CircularEdge(length, length), Vector2.Zero,
                    CoraliteContent.ParticleType<ElectricParticle>(),
                    Scale: Main.rand.NextFloat(ThunderveinDirector.ExchangeParticleScaleMin, ThunderveinDirector.ExchangeParticleScaleMax));
            }
        }

        protected override void OnBeatAdopted(ThunderveinDragonContext ctx, int previousBeat)
        {
            if (CurrentBeat == Beat.Roar)
            {
                PlayRoarSound(ctx);
            }
        }

        protected override IVaultState<ThunderveinDragonContext> AuthorityUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            if (roarThisFrame)
            {
                roarThisFrame = false;
                ctx.MarkDecision();
                ctx.Npc.NewProjectileDirectInAI<ThunderveinExchangePhaseAnmi>(ctx.Npc.Center, Vector2.Zero, 1, 0,
                    ctx.Npc.target, ThunderveinDirector.ExchangeBurstFrames, ctx.Npc.whoAmI);
            }

            if (CurrentBeat == Beat.Roar && Timer > ThunderveinDirector.ExchangeRoarFrames + ThunderveinDirector.ExchangeRoarTail)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
