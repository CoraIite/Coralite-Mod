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
    /// 引力雷球（二阶段解锁，累计出招超过门槛后按手数加权）：吐一颗把玩家往里拽的重球。<br/>
    /// 节拍：Charge（贴近 45 帧，嘴前吸入尘半径 240→100 收拢）→ Swing（翅膀到位后 10 帧吐出）
    /// → Burst（40 帧，残影 1→1.5）→ Recover（30 帧后摇，收招后固定接闪电突袭 / 冲刺放电 / 电磁炮三选一）。<br/>
    /// 公平阀：起手最长的嘴部招；固定接招让“被拽住之后挨什么”是可学的三选一，而不是整张轮换表。<br/>
    /// 旧 AI.GravitationThunder.cs:12-147
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.GravitationThunder, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinGravitationThunderState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.GravitationThunder;

        private enum Beat
        {
            /// <summary>贴近 + 蓄力（旧 SonState 0）</summary>
            Charge = 0,
            /// <summary>收翅吐球（旧 1）</summary>
            Swing = 1,
            /// <summary>爆发（旧 2）</summary>
            Burst = 2,
            /// <summary>后摇 + 固定接招（旧 3）</summary>
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
            float edge = ThunderveinDirector.GravitationDustEdgeFrom
                - (ThunderveinDirector.GravitationDustEdgeShrink * Math.Clamp(T / (float)ThunderveinDirector.GravitationReadyFrames, 0f, 1f));
            MouthDust(ctx, edge / 2f, ThunderveinDirector.MouthDustSpeedMin, ThunderveinDirector.MouthDustSpeedMax, ThunderveinDirector.MouthDustScaleMax);

            ctx.Npc.QuickSetDirection();
            ctx.DeclareChase(ctx.Target.Center, ThunderveinDirector.CloseChaseOpenMouth);
            ctx.DeclareRotationNormal();

            if (ctx.Npc.frame.Y == 0 && Timer > ThunderveinDirector.GravitationReadyFrames)
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
            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, npc.Center);
            }

            SwitchBeat(ctx, (int)Beat.Burst);
        }

        private void UpdateBurst(ThunderveinDragonContext ctx)
        {
            ctx.DeclareKeep();
            ctx.DrawShadows = true;
            ctx.Boss.UpdateAllOldCaches();
            BurstShadowEnvelope(ctx, T / (float)ThunderveinDirector.GravitationBurstFrames, ThunderveinDirector.SmallBurstShadowScaleTo);
            BurstMouthFrame(ctx.Npc);

            if (Timer > ThunderveinDirector.GravitationBurstFrames)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
            }
        }

        protected override void OnBeatAdopted(ThunderveinDragonContext ctx, int previousBeat)
        {
            if (CurrentBeat == Beat.Burst)
            {
                ctx.Boss.ResetAllOldCaches();
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
                Vector2 velocity = (ctx.Target.Center - mouth).SafeNormalize(Vector2.Zero) * ThunderveinDirector.GravitationBallSpeed;
                npc.NewProjectileDirectInAI<GravitationThunderBall>(mouth, velocity, ThunderveinDirector.GravitationDamage(), 0, npc.target);
            }

            if (CurrentBeat != Beat.Recover || Timer <= ThunderveinDirector.GravitationRecoverFrames)
            {
                return null;
            }

            // 固定接招：旧 ResetToSelectedState，不经加权表也不推进累计手数，但仍过唯一提交口记账
            ThunderveinDragon.AIStates next = ctx.AttackRandom.Next(ThunderveinDirector.GravitationFollowUpCount) switch
            {
                0 => ThunderveinDragon.AIStates.LightningRaid,
                1 => ThunderveinDragon.AIStates.DashDischarging,
                _ => ThunderveinDragon.AIStates.ElectromagneticCannon,
            };

            return ThunderveinHubState.CommitFixed(ctx, next);
        }
    }
}
