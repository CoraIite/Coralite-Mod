using Coralite.Content.Bosses.ThunderveinDragon.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon.States
{
    /// <summary>
    /// 电磁炮（不进加权表，只由引力雷球的固定接招进入）：一条持续 100 帧、全程缓慢追瞄的粗光束。<br/>
    /// 节拍：Chase（拉到玩家头顶 300 px 内，最长 240 帧）→ Roll（绕飞一圈 45 帧）→ Aim（锁向 10 帧 + 撒尘，翅膀到位后 35 帧）
    /// → Burst（100 帧，0.014 弧度/帧续瞄，每 20 帧一次震屏，天空亮 0.6）→ Recover（25 帧后摇）。<br/>
    /// 公平阀：追瞄速率只有 0.014 弧度/帧，横向跑动能甩脱；起手预告与吐息同一套（绕飞 + 瞄准尘）。<br/>
    /// 旧 AI.ElectromagneticCannon.cs:11-197
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.ElectromagneticCannon, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinElectromagneticCannonState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.ElectromagneticCannon;

        private enum Beat
        {
            /// <summary>拉近（旧 SonState 0）</summary>
            Chase = 0,
            /// <summary>绕飞一圈（旧 1）</summary>
            Roll = 1,
            /// <summary>瞄准（旧 2）</summary>
            Aim = 2,
            /// <summary>发射（旧 3）</summary>
            Burst = 3,
            /// <summary>后摇（旧 4）</summary>
            Recover = 4,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>锁定的瞄准角（热字段 A）；弹幕经本体 <c>Recorder</c> 读它跟随。</summary>
        private float aimAngle;

        private bool fireThisFrame;

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            fireThisFrame = false;
            ctx.AimAngle = aimAngle;

            switch (CurrentBeat)
            {
                case Beat.Roll:
                    UpdateRoll(ctx);
                    break;
                case Beat.Aim:
                    UpdateAim(ctx);
                    break;
                case Beat.Burst:
                    UpdateBurst(ctx);
                    break;
                case Beat.Recover:
                    ctx.DeclareNoRot();
                    ctx.Boss.FlyingFrame();
                    ctx.DeclareKeep();
                    break;
                default:
                    UpdateChase(ctx);
                    break;
            }
        }

        private void UpdateChase(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 targetPos = ctx.Target.Center + new Vector2(0, ThunderveinDirector.BreathChaseOffsetY);
            npc.direction = npc.spriteDirection = targetPos.X > npc.Center.X ? 1 : -1;
            npc.directionY = targetPos.Y > npc.Center.Y ? 1 : -1;
            ctx.DeclareRotationNormal();
            ctx.DeclareChase(targetPos, ThunderveinDirector.CannonChase);

            ctx.Boss.GetLengthToTargetPos(targetPos, out float xLength, out float yLength);
            bool close = xLength < ThunderveinDirector.BreathChaseExitX && yLength < ThunderveinDirector.BreathChaseExitY;
            if (Timer <= ThunderveinDirector.CannonChaseFrames && !close)
            {
                return;
            }

            ctx.Boss.DashFrame();
            ctx.Boss.ResetAllOldCaches();
            npc.velocity = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero)
                .RotatedBy(-npc.direction * MathHelper.PiOver2) * ThunderveinDirector.RollSpeed;
            ctx.DeclareDirect();
            SwitchBeat(ctx, (int)Beat.Roll);
        }

        private void UpdateRoll(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareDashing();
            ctx.ShadowScale = ThunderveinDirector.DashShadowScale;
            ctx.Boss.UpdateAllOldCaches();

            npc.velocity = npc.velocity.RotatedBy(-npc.spriteDirection * MathHelper.TwoPi / ThunderveinDirector.RollTurnDivisor);
            npc.rotation = npc.velocity.ToRotation();
            ctx.DeclareDirect();

            if (Timer > ThunderveinDirector.BreathRollFrames)
            {
                ctx.DeclareNoRot();
                SwitchBeat(ctx, (int)Beat.Aim);
            }
        }

        private void UpdateAim(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.DeclareDamp(ThunderveinDirector.AimDamp);

            if (T < ThunderveinDirector.CannonAimLockFrames)
            {
                npc.QuickSetDirection();
                ctx.DeclareNoRot();
                aimAngle = (ctx.Target.Center - npc.Center).ToRotation();
                ctx.AimAngle = aimAngle;
            }

            AimDust(ctx, aimAngle);

            if (npc.frame.Y != 4)
            {
                ctx.Boss.FlyingFrame();
                HoldTimer();
                return;
            }

            if (Timer <= ThunderveinDirector.CannonAimFrames)
            {
                return;
            }

            npc.velocity *= 0;
            ctx.DeclareDirect();
            npc.TargetClosest();
            fireThisFrame = true;
            ctx.Boss.ResetAllOldCaches();
            PlayFireCue(ctx);
            SwitchBeat(ctx, (int)Beat.Burst);
        }

        private void PlayFireCue(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            Helper.PlayPitched(CoraliteSoundID.PhantasmalDeathray_Zombie104, ctx.Npc.Center, pitch: ThunderveinDirector.CannonSoundPitch);
            Shake(ctx, aimAngle.ToRotationVector2() * ThunderveinDirector.CannonShakeDirScale, ThunderveinDirector.CannonShakeStrength,
                ThunderveinDirector.BreathShakeVibration, ThunderveinDirector.BreathShakeFrames);
            ThunderveinDragon.SetBackgroundLight(ThunderveinDirector.CannonSkyLight,
                ThunderveinDirector.CannonBurstFrames * 3 / 4, ThunderveinDirector.CannonSkyExchange);
        }

        private void UpdateBurst(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.DeclareKeep();
            ctx.Boss.UpdateAllOldCaches();

            ctx.Boss.GetLengthToTargetPos(ctx.Target.Center, out float xLength, out _);
            if (xLength > ThunderveinDirector.CannonTrackFaceX)
            {
                npc.QuickSetDirection();
            }

            ctx.DeclareNoRot(1f);
            aimAngle = aimAngle.AngleTowards((ctx.Target.Center - ctx.Boss.GetMousePos()).ToRotation(), ThunderveinDirector.CannonTrackRate);
            ctx.AimAngle = aimAngle;
            ctx.Boss.FlyingFrame(true);
            BurstShadowEnvelope(ctx, T / (float)ThunderveinDirector.CannonBurstFrames, ThunderveinDirector.BreathShadowScaleTo);

            if (!Main.dedServ && Timer % ThunderveinDirector.CannonShakeInterval == 0)
            {
                Shake(ctx, Helper.NextVec2Dir(), ThunderveinDirector.BurstShakeStrength,
                    ThunderveinDirector.BurstShakeVibration, ThunderveinDirector.CannonBurstShakeFrames);
            }

            if (Timer <= ThunderveinDirector.CannonBurstFrames)
            {
                return;
            }

            npc.QuickSetDirection();
            ctx.DeclareNoRot();
            SwitchBeat(ctx, (int)Beat.Recover);
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
                npc.NewProjectileDirectInAI<ElectromagneticCannon>(
                    mouth + (aimAngle.ToRotationVector2() * ThunderveinDirector.CannonSpawnDistance), mouth,
                    ThunderveinDirector.CannonDamage(), 0, npc.target,
                    ThunderveinDirector.CannonBurstFrames, npc.whoAmI, ThunderveinDirector.BreathProjAi2);
            }

            if (CurrentBeat == Beat.Recover && Timer > ThunderveinDirector.RecoverFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        protected override void WriteSlots(ThunderveinDragonContext ctx)
            => ctx.Hot[CoraliteBossHotSlots.A] = aimAngle;

        protected override void ReadSlots(ThunderveinDragonContext ctx)
        {
            aimAngle = ctx.Hot[CoraliteBossHotSlots.A];
            ctx.AimAngle = aimAngle;
        }
    }
}
