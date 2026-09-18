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
    /// 冲刺放电（二阶段起进入轮换）：先拉开跑道冲脸，停下后立刻原地放电。<br/>
    /// 节拍：Windup（42 帧向后扇翅 + 远离玩家蓄力，反向运动本身即预告）→ Dash（40 px/f 追着玩家冲，最长 60 帧或贴到 200 px 内）
    /// → Charge（48 帧蓄力，吸入尘半径 200→620 外扩）→ Burst（35 帧爆发，残影 1→2.5，天空亮 0.5）→ Recover（25 帧后摇）。<br/>
    /// 公平阀：冲刺段本体不带判定（伤害在放电弹幕上），Charge 的 48 帧足够跑出爆炸半径。<br/>
    /// 旧 AI.DashDischarging.cs:12-193
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.DashDischarging, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinDashDischargingState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.DashDischarging;

        private enum Beat
        {
            /// <summary>后撤扇翅蓄力（旧 SonState 0）</summary>
            Windup = 0,
            /// <summary>长冲（旧 1）</summary>
            Dash = 1,
            /// <summary>贴身蓄力（旧 2）</summary>
            Charge = 2,
            /// <summary>爆发（旧 3）</summary>
            Burst = 3,
            /// <summary>后摇（旧 4）</summary>
            Recover = 4,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        private bool launchThisFrame;
        private bool fireThisFrame;

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            launchThisFrame = false;
            fireThisFrame = false;

            switch (CurrentBeat)
            {
                case Beat.Dash:
                    UpdateDash(ctx);
                    break;
                case Beat.Charge:
                    UpdateCharge(ctx);
                    break;
                case Beat.Burst:
                    UpdateBurst(ctx);
                    break;
                case Beat.Recover:
                    ctx.Boss.FlyingFrame();
                    ctx.DeclareKeep();
                    break;
                default:
                    UpdateWindup(ctx);
                    break;
            }
        }

        /// <summary>
        /// 后撤蓄力：速度不足 8 时每帧远离玩家 0.65，翅膀帧从 1 起每 6 帧推进一格，到第 8 格（42 帧）起冲。<br/>
        /// 旧代码靠 <c>frameCounter</c> 计数且入拍不清零，时长会随上一招残留浮动 ≤ 5 帧；这里改为纯 Timer 推导，客户端可重建。旧 :22-60
        /// </summary>
        private void UpdateWindup(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.QuickSetDirection();
            ctx.DeclareRotationNormal(ThunderveinDirector.RaidWindupRotRate);

            if (npc.velocity.Length() < ThunderveinDirector.RaidWindupBackSpeed)
            {
                npc.velocity += (npc.Center - ctx.Target.Center).SafeNormalize(Vector2.Zero) * ThunderveinDirector.RaidWindupBackAccel;
            }

            ctx.DeclareDirect();
            ctx.Boss.UpdateAllOldCaches();
            ctx.DrawShadows = true;
            ctx.ShadowScale = ThunderveinDirector.DashDischargeWindupShadowScale;

            npc.frame.Y = Math.Min(ThunderveinDirector.DashDischargeFlapStartFrame + (Timer / ThunderveinDirector.DashDischargeFlapTicks), 7);

            if (Timer < ThunderveinDirector.DashDischargeFlapFrames)
            {
                return;
            }

            ctx.Boss.DashFrame();
            npc.velocity = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ThunderveinDirector.DashDischargeSpeed;
            npc.rotation = npc.velocity.ToRotation();
            npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
            launchThisFrame = true;
            PlayLaunchCue(ctx);
            SwitchBeat(ctx, (int)Beat.Dash);
        }

        private static void PlayLaunchCue(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, ctx.Npc.Center);
            ThunderveinDragon.SetBackgroundLight(ThunderveinDirector.RaidSkyLight,
                ThunderveinDirector.DashDischargeDashFrames - ThunderveinDirector.RaidSkyLightFadeOffset, ThunderveinDirector.RaidSkyLightExchange);
        }

        /// <summary>长冲：每帧重新指向玩家，贴到 200 px 内或 60 帧到点就刹停。旧 :62-84</summary>
        private void UpdateDash(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareDashing();
            ctx.ShadowScale = ThunderveinDirector.DashDischargeWindupShadowScale;
            ctx.Boss.UpdateAllOldCaches();

            ctx.Boss.GetLengthToTargetPos(ctx.Target.Center, out float xLength, out _);
            if (xLength > ThunderveinDirector.DashDischargeTrackX)
            {
                npc.QuickSetDirection();
            }

            npc.velocity = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ThunderveinDirector.DashDischargeSpeed;
            npc.rotation = npc.velocity.ToRotation();
            ctx.DeclareDirect();

            if (Timer <= ThunderveinDirector.DashDischargeDashFrames && ctx.TargetDistance >= ThunderveinDirector.DashDischargeStopDistance)
            {
                return;
            }

            npc.velocity *= 0;
            npc.frame.X = 0;
            npc.frame.Y = 0;
            npc.frameCounter = 0;
            SwitchBeat(ctx, (int)Beat.Charge);
        }

        /// <summary>贴身蓄力：翅膀帧每 8 帧推一格到 4（纯 Timer 推导），48 帧到点出手。旧 :85-152</summary>
        private void UpdateCharge(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.Boss.UpdateAllOldCaches();
            npc.QuickSetDirection();
            ctx.DeclareNoRot();
            ctx.DeclareChase(ctx.Target.Center, ThunderveinDirector.DashDischargeCharge);

            float edge = ThunderveinDirector.DashDischargeDustEdgeFrom
                + (ThunderveinDirector.DashDischargeDustEdgeGain * Math.Clamp(Timer / (float)ThunderveinDirector.DashDischargeChargeFrames, 0f, 1f));
            DischargeDust(ctx, edge / 2f);

            // 旧代码 ++frameCounter > 7 即每 8 帧推一格，推到第 4 格停住
            npc.frame.Y = Math.Min(Timer / (ThunderveinDirector.DashDischargeChargeFrameTicks + 1), 4);

            if (Timer <= ThunderveinDirector.DashDischargeChargeFrames)
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

        private static void PlayFireCue(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, ctx.Npc.Center);
            SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, ctx.Npc.Center);
            Shake(ctx, Vector2.UnitY * ThunderveinDirector.BurstShakeDirY, ThunderveinDirector.BigBurstShakeStrength,
                ThunderveinDirector.BigBurstShakeVibration, ThunderveinDirector.BigBurstShakeFrames);
            ThunderveinDragon.SetBackgroundLight(ThunderveinDirector.DischargeSkyLight,
                ThunderveinDirector.DischargeBurstFrames - ThunderveinDirector.DischargeSkyFadeOffset, ThunderveinDirector.DischargeSkyExchange);
        }

        private void UpdateBurst(ThunderveinDragonContext ctx)
        {
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.Boss.UpdateAllOldCaches();
            ctx.DeclareNoRot();
            ctx.DeclareDamp(ThunderveinDirector.DashDischargeBurstDamp);
            BurstShadowEnvelope(ctx, T / (float)ThunderveinDirector.DischargeBurstFrames, ThunderveinDirector.BurstShadowScaleTo);
            BurstMouthFrame(ctx.Npc);

            if (Timer > ThunderveinDirector.DischargeBurstFrames)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
            }
        }

        protected override void OnBeatAdopted(ThunderveinDragonContext ctx, int previousBeat)
        {
            switch (CurrentBeat)
            {
                case Beat.Dash:
                    PlayLaunchCue(ctx);
                    break;
                case Beat.Burst:
                    ctx.Boss.ResetAllOldCaches();
                    PlayFireCue(ctx);
                    break;
                default:
                    break;
            }
        }

        protected override IVaultState<ThunderveinDragonContext> AuthorityUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            if (launchThisFrame)
            {
                launchThisFrame = false;
                ctx.MarkDecision();
            }

            if (fireThisFrame)
            {
                fireThisFrame = false;
                ctx.MarkDecision();
                ctx.Npc.NewProjectileDirectInAI<StrongDischargingBurst>(ctx.Npc.Center, Vector2.Zero,
                    ThunderveinDirector.DashDischargeDamage(), 0, ctx.Npc.target,
                    ThunderveinDirector.DischargeBurstFrames, ctx.Npc.whoAmI);
            }

            if (CurrentBeat == Beat.Recover && Timer > ThunderveinDirector.RecoverFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
