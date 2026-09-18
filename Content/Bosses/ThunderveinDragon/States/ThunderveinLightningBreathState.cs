using Coralite.Content.Bosses.ThunderveinDragon.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ThunderveinDragon.States
{
    /// <summary>
    /// 闪电吐息（二阶段改为电磁炮，并会连吐最多 3 次）。<br/>
    /// 节拍：Chase（拉到玩家头顶 300 px 内，最长 180/150 帧）→ Roll（切向 20 px/f 绕飞一圈 45 帧，横过来的身位就是预告）
    /// → Aim（锁向 10/20 帧后沿瞄准线撒尘，翅膀到位再等 35/45 帧）→ Burst（45/30 帧吐息，二阶段以 0.015 弧度/帧续瞄）
    /// → 二阶段 5/7 概率再来一轮（上限 3）→ Recover（25 帧后摇）。<br/>
    /// 公平阀：绕飞 + 撒尘合计 65 帧以上的可见预告，且出手方向在 Aim 前 10/20 帧就锁死——预告指哪打哪。<br/>
    /// 旧 AI.LightningBreath.cs（P1 :12-197 / P2 :199-396）
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.LightningBreath, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinLightningBreathState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.LightningBreath;

        private enum Beat
        {
            /// <summary>拉近（旧 SonState 0）</summary>
            Chase = 0,
            /// <summary>绕飞一圈（旧 1）</summary>
            Roll = 1,
            /// <summary>瞄准（旧 2 / 4 / 6）</summary>
            Aim = 2,
            /// <summary>吐息（旧 3 / 5 / 7）</summary>
            Burst = 3,
            /// <summary>后摇（旧 P1 的 4 / P2 的 8）</summary>
            Recover = 4,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>锁定的瞄准角（热字段 A）；弹幕经本体 <c>Recorder</c> 读它跟随。</summary>
        private float aimAngle;

        /// <summary>已吐出的轮数（热字段 B）。</summary>
        private int burstCount;

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

        /// <summary>拉近到玩家头顶 300 px。旧 :21-70</summary>
        private void UpdateChase(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 targetPos = ctx.Target.Center + new Vector2(0, ThunderveinDirector.BreathChaseOffsetY);
            npc.direction = npc.spriteDirection = targetPos.X > npc.Center.X ? 1 : -1;
            npc.directionY = targetPos.Y > npc.Center.Y ? 1 : -1;
            ctx.DeclareRotationNormal();
            ctx.DeclareChase(targetPos, ThunderveinDirector.BreathChase);

            ctx.Boss.GetLengthToTargetPos(targetPos, out float xLength, out float yLength);
            bool close = xLength < ThunderveinDirector.BreathChaseExitX && yLength < ThunderveinDirector.BreathChaseExitY;
            if (Timer <= ThunderveinDirector.BreathChaseFrames(ctx.Phase) && !close)
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

        /// <summary>绕飞一圈：每帧把速度转 2π/60，45 帧刚好大半圈。旧 :71-88</summary>
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

        /// <summary>瞄准：前 10/20 帧锁向，之后沿锁定线撒尘；翅膀帧到位才计时。旧 :89-151</summary>
        private void UpdateAim(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.DeclareDamp(ThunderveinDirector.AimDamp);

            if (T < ThunderveinDirector.BreathAimLockFrames(ctx.Phase))
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

            if (Timer <= ThunderveinDirector.BreathAimFrames(ctx.Phase))
            {
                return;
            }

            npc.velocity *= 0;
            ctx.DeclareDirect();
            npc.TargetClosest();
            fireThisFrame = true;
            burstCount++;
            ctx.Boss.ResetAllOldCaches();
            PlayFireCue(ctx);
            SwitchBeat(ctx, (int)Beat.Burst);
        }

        /// <summary>纯本地：出手音 + 沿瞄准线的震屏 + 天空过曝。</summary>
        private void PlayFireCue(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, ctx.Npc.Center);
            SoundEngine.PlaySound(CoraliteSoundID.BubbleShield_Electric_NPCHit43, ctx.Npc.Center);

            if (ctx.Phase == 1)
            {
                Shake(ctx, aimAngle.ToRotationVector2(), ThunderveinDirector.BreathShakeStrength,
                    ThunderveinDirector.BreathShakeVibration, ThunderveinDirector.BreathShakeFrames);
            }
            else
            {
                Shake(ctx, aimAngle.ToRotationVector2() * ThunderveinDirector.CannonShakeDirScale, ThunderveinDirector.CannonShakeStrength,
                    ThunderveinDirector.BreathShakeVibration, ThunderveinDirector.BreathShakeFrames);
            }

            ThunderveinDragon.SetBackgroundLight(ThunderveinDirector.BreathSkyLight, ThunderveinDirector.BreathBurstFrames(ctx.Phase) / 2);
        }

        /// <summary>吐息：一阶段定格张嘴，二阶段持续追瞄并每 10 帧抖一下。旧 :153-185 / :340-383</summary>
        private void UpdateBurst(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.DeclareKeep();
            ctx.Boss.UpdateAllOldCaches();
            BurstShadowEnvelope(ctx, T / (float)ThunderveinDirector.BreathBurstFrames(ctx.Phase), ThunderveinDirector.BreathShadowScaleTo);

            if (ctx.Phase == 1)
            {
                BurstMouthFrame(npc);
            }
            else
            {
                ctx.Boss.GetLengthToTargetPos(ctx.Target.Center, out float xLength, out _);
                if (xLength > ThunderveinDirector.BreathTrackFaceX)
                {
                    npc.QuickSetDirection();
                }

                ctx.DeclareNoRot(1f);
                ctx.Boss.FlyingFrame(true);
                aimAngle = aimAngle.AngleTowards((ctx.Target.Center - ctx.Boss.GetMousePos()).ToRotation(), ThunderveinDirector.BreathTrackRate);
                ctx.AimAngle = aimAngle;

                if (!Main.dedServ && T > ThunderveinDirector.BreathBurstShakeStart && T % ThunderveinDirector.BreathBurstShakeInterval == 0)
                {
                    Shake(ctx, Helper.NextVec2Dir(), ThunderveinDirector.BurstShakeStrength,
                        ThunderveinDirector.BurstShakeVibration, ThunderveinDirector.BreathBurstShakeFrames);
                }
            }

            if (Timer <= ThunderveinDirector.BreathBurstFrames(ctx.Phase))
            {
                return;
            }

            npc.QuickSetDirection();
            ctx.DeclareNoRot();

            // 一阶段只吐一次；二阶段无条件掷骰（两端同序消费）后再看轮数上限
            if (ctx.Phase == 1)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
                return;
            }

            bool again = ctx.AttackRandBool(ThunderveinDirector.BreathContinueNum, ThunderveinDirector.BreathContinueDen);
            SwitchBeat(ctx, again && burstCount < ThunderveinDirector.BreathMaxBursts ? (int)Beat.Aim : (int)Beat.Recover);
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
                Fire(ctx);
            }

            if (CurrentBeat == Beat.Recover && Timer > ThunderveinDirector.RecoverFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        /// <summary>仅权威端：一阶段吐息弹幕从瞄准线 1800 px 处反向打回嘴部，二阶段换成 2000 px 的电磁炮。旧 :130-135 / :317-322</summary>
        private void Fire(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 mouth = ctx.Boss.GetMousePos();

            if (ctx.Phase == 1)
            {
                npc.NewProjectileDirectInAI<LightingBreath>(mouth + (aimAngle.ToRotationVector2() * ThunderveinDirector.BreathSpawnDistance), mouth,
                    ThunderveinDirector.BreathDamage(), 0, npc.target,
                    ThunderveinDirector.BreathBurstFrames(ctx.Phase), npc.whoAmI, ThunderveinDirector.BreathProjAi2);
                return;
            }

            npc.NewProjectileDirectInAI<ElectromagneticCannon>(mouth + (aimAngle.ToRotationVector2() * ThunderveinDirector.BreathCannonSpawnDistance), mouth,
                ThunderveinDirector.BreathCannonDamage(), 0, npc.target,
                ThunderveinDirector.BreathBurstFrames(ctx.Phase), npc.whoAmI, ThunderveinDirector.BreathProjAi2);
        }

        protected override void WriteSlots(ThunderveinDragonContext ctx)
        {
            ctx.Hot[CoraliteBossHotSlots.A] = aimAngle;
            ctx.Hot[CoraliteBossHotSlots.B] = burstCount;
        }

        protected override void ReadSlots(ThunderveinDragonContext ctx)
        {
            aimAngle = ctx.Hot[CoraliteBossHotSlots.A];
            burstCount = (int)ctx.Hot[CoraliteBossHotSlots.B];
            ctx.AimAngle = aimAngle;
        }
    }
}
