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
    /// 放电：贴脸招，身体周围张开电流环再整圈炸开。<br/>
    /// 节拍：Charge（贴近玩家 40 帧，吸入尘半径 400→620 持续外扩 = 预告）→ Swing（收翅，翅膀帧到位后 10 帧出手）
    /// → Burst（35 帧爆发，残影 1→2.5 淡出、天空亮 0.5）→ Recover（25 帧后摇）。<br/>
    /// 公平阀：吸入尘从起手第一帧就画出爆炸半径，40+ 帧的蓄力够跑出圈外；爆发期间本体带电减伤 40%。<br/>
    /// 旧 AI.Discharging.cs:13-158
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.Discharging, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinDischargingState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.Discharging;

        private enum Beat
        {
            /// <summary>贴近 + 蓄力（旧 SonState 0）</summary>
            Charge = 0,
            /// <summary>收翅出手（旧 1）</summary>
            Swing = 1,
            /// <summary>爆发（旧 2）</summary>
            Burst = 2,
            /// <summary>后摇（旧 3）</summary>
            Recover = 3,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>本帧出手，权威端同帧生成放电弹幕。</summary>
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
            ctx.Npc.QuickSetDirection();
            ctx.DeclareChase(ctx.Target.Center, ThunderveinDirector.CloseChase);
            ctx.DeclareRotationNormal();

            float edge = ThunderveinDirector.DischargeDustEdgeFrom
                + (ThunderveinDirector.DischargeDustEdgeGain * Math.Clamp(T / (float)ThunderveinDirector.DischargeReadyFrames, 0f, 1f));
            DischargeDust(ctx, edge / 2f);

            // 旧代码等翅膀帧回到 0 才进入挥翅（吸入尘继续画，蓄力窗口 40~80 帧）
            if (ctx.Npc.frame.Y == 0 && Timer > ThunderveinDirector.DischargeReadyFrames)
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
            DischargeDust(ctx, ThunderveinDirector.DischargeSwingDustEdge);

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
            ctx.CurrentSurrounding = true;
            ctx.Boss.ResetAllOldCaches();
            PlayFireCue(ctx);
            SwitchBeat(ctx, (int)Beat.Burst);
        }

        /// <summary>纯本地：出手音、天空过曝、竖直震屏。</summary>
        private static void PlayFireCue(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, ctx.Npc.Center);
            SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, ctx.Npc.Center);
            ThunderveinDragon.SetBackgroundLight(ThunderveinDirector.DischargeSkyLight,
                ThunderveinDirector.DischargeBurstFrames - ThunderveinDirector.DischargeSkyFadeOffset, ThunderveinDirector.DischargeSkyExchange);
            Shake(ctx, Vector2.UnitY * ThunderveinDirector.BurstShakeDirY, ThunderveinDirector.BigBurstShakeStrength,
                ThunderveinDirector.BigBurstShakeVibration, ThunderveinDirector.BigBurstShakeFrames);
        }

        private void UpdateBurst(ThunderveinDragonContext ctx)
        {
            ctx.DeclareKeep();
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.Boss.UpdateAllOldCaches();
            BurstShadowEnvelope(ctx, T / (float)ThunderveinDirector.DischargeBurstFrames, ThunderveinDirector.BurstShadowScaleTo);
            BurstMouthFrame(ctx.Npc);

            if (Timer > ThunderveinDirector.DischargeBurstFrames)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
            }
        }

        /// <summary>客户端被收养进爆发拍且刚起拍时补放出手演出（只放表现）。</summary>
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
                int type = ctx.Phase == 1
                    ? ModContent.ProjectileType<DischargingBurst>()
                    : ModContent.ProjectileType<StrongDischargingBurst>();
                ctx.Npc.NewProjectileDirectInAI(ctx.Npc.Center, Vector2.Zero, type, ThunderveinDirector.DischargeDamage(), 0,
                    ctx.Npc.target, ThunderveinDirector.DischargeBurstFrames, ctx.Npc.whoAmI);
            }

            if (CurrentBeat == Beat.Recover && Timer > ThunderveinDirector.RecoverFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
