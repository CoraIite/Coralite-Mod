using Coralite.Content.Bosses.ThunderveinDragon.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ThunderveinDragon.States
{
    /// <summary>
    /// 十字雷（仅大师模式进入轮换）：吐一颗高速追踪球，飞行一段后向四周炸成交错闪电。<br/>
    /// 节拍：Charge（45 帧，比电球长一倍的蓄力 + 更急的吸入尘）→ Swing（翅膀到位后 10 帧吐出，天空亮 0.25）
    /// → Burst（25 帧，残影 1→1.5）→ Recover（25 帧后摇）。<br/>
    /// 公平阀：蓄力 45 帧是全部“嘴部招”里最长的，代价是弹幕初速 8 且会二次炸开。<br/>
    /// 旧 AI.CrossLightingBall.cs:12-140
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.CrossLightingBall, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinCrossLightingBallState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.CrossLightingBall;

        private enum Beat
        {
            /// <summary>贴近 + 蓄力（旧 SonState 0）</summary>
            Charge = 0,
            /// <summary>收翅吐球（旧 1）</summary>
            Swing = 1,
            /// <summary>爆发（旧 2）</summary>
            Burst = 2,
            /// <summary>后摇（旧 3）</summary>
            Recover = 3,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        private bool fireThisFrame;

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            fireThisFrame = false;

            switch (CurrentBeat)
            {
                case Beat.Swing:
                    UpdateSwing(ctx);
                    break;
                case Beat.Burst:
                    UpdateBurst(ctx);
                    break;
                case Beat.Recover:
                    ctx.Boss.FlyingFrame();
                    ctx.DeclareKeep();
                    break;
                default:
                    UpdateCharge(ctx);
                    break;
            }
        }

        private void UpdateCharge(ThunderveinDragonContext ctx)
        {
            // 吸入尘半径沿用电球的 140→80，只是速度与缩放更急
            float edge = ThunderveinDirector.BallDustEdgeFrom
                - (ThunderveinDirector.BallDustEdgeShrink * Math.Clamp(T / (float)ThunderveinDirector.CrossReadyFrames, 0f, 1f));
            MouthDust(ctx, edge / 2f, ThunderveinDirector.CrossDustSpeedMin, ThunderveinDirector.CrossDustSpeedMax, ThunderveinDirector.CrossDustScaleMax);

            ctx.Npc.QuickSetDirection();
            ctx.DeclareChase(ctx.Target.Center, ThunderveinDirector.CrossChase);
            ctx.DeclareRotationNormal();

            if (ctx.Npc.frame.Y == 0 && Timer > ThunderveinDirector.CrossReadyFrames)
            {
                SwitchBeat(ctx, (int)Beat.Swing);
            }
        }

        private void UpdateSwing(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareDamp(ThunderveinDirector.SwingDamp);
            npc.QuickSetDirection();
            ctx.DeclareNoRot();

            if (npc.frame.Y != 4)
            {
                ctx.Boss.FlyingFrame();
                HoldTimer();
                return;
            }

            if (Timer <= ThunderveinDirector.SwingFrames)
            {
                return;
            }

            npc.TargetClosest();
            fireThisFrame = true;
            ctx.DrawShadows = true;
            ctx.Boss.ResetAllOldCaches();
            PlayFireCue(ctx);
            SwitchBeat(ctx, (int)Beat.Burst);
        }

        private static void PlayFireCue(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            ThunderveinDragon.SetBackgroundLight(ThunderveinDirector.CrossSkyLight, ThunderveinDirector.CrossSkyFrames, ThunderveinDirector.CrossSkyExchange);
            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, ctx.Npc.Center);
        }

        private void UpdateBurst(ThunderveinDragonContext ctx)
        {
            ctx.DeclareKeep();
            ctx.DrawShadows = true;
            ctx.Boss.UpdateAllOldCaches();
            BurstShadowEnvelope(ctx, T / (float)ThunderveinDirector.CrossBurstFrames, ThunderveinDirector.SmallBurstShadowScaleTo);
            BurstMouthFrame(ctx.Npc);

            if (Timer > ThunderveinDirector.CrossBurstFrames)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
            }
        }

        protected override void OnBeatAdopted(ThunderveinDragonContext ctx, int previousBeat)
        {
            if (CurrentBeat == Beat.Burst)
            {
                ctx.Boss.ResetAllOldCaches();
                PlayFireCue(ctx);
            }
        }

        protected override IVaultState<ThunderveinDragonContext> AuthorityUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            if (fireThisFrame)
            {
                fireThisFrame = false;
                ctx.MarkDecision();

                NPC npc = ctx.Npc;
                Vector2 mouth = ctx.Boss.GetMousePos();
                Vector2 velocity = (ctx.Target.Center - mouth).SafeNormalize(Vector2.Zero) * ThunderveinDirector.CrossBallSpeed;
                int type = ctx.Phase == 1
                    ? ModContent.ProjectileType<CrossLightingBallChasable>()
                    : ModContent.ProjectileType<StrongerCrossLightingBallChasable>();
                npc.NewProjectileInAI_Server(mouth, velocity, type, ThunderveinDirector.CrossDamage(), 0, npc.target, npc.whoAmI);
            }

            if (CurrentBeat == Beat.Recover && Timer > ThunderveinDirector.RecoverFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
