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
    /// 闪电突袭：三段乱窜之后接一次长冲，二阶段的长冲沿途留下交错电球。<br/>
    /// 节拍：Chase（保持 350~550 px 的横向带宽，最长 180 帧）→ Zig（3 段 12/14 帧的短冲，首段 ±(0.9~1.1)、后两段 (0.6~1.1) 正负交替）
    /// → Windup（向后扇翅 32/24 帧，反向运动即预告）→ BigDash（40 px/f 长冲 25 帧 → 0.8 刹车 → 40/35 帧后摇）。<br/>
    /// 公平阀：乱窜的偏角只在 700 px 内才加（远距离是直线，看得清）；长冲前的扇翅段身体明显后仰。<br/>
    /// 旧 AI.LightningRaid.cs（P1 :12-208 / P2 :210-417）
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.LightningRaid, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinLightningRaidState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.LightningRaid;

        private enum Beat
        {
            /// <summary>拉开跑道（旧 SonState 0）</summary>
            Chase = 0,
            /// <summary>三段乱窜（旧 1）</summary>
            Zig = 1,
            /// <summary>扇翅蓄力（旧 2）</summary>
            Windup = 2,
            /// <summary>长冲 + 刹车 + 后摇（旧 3）</summary>
            BigDash = 3,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>本帧起了一段乱窜，权威端同帧出短冲判定弹幕。</summary>
        private bool zigThisFrame;

        /// <summary>本帧长冲起跳。</summary>
        private bool bigDashThisFrame;

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            zigThisFrame = false;
            bigDashThisFrame = false;

            switch (CurrentBeat)
            {
                case Beat.Zig:
                    UpdateZig(ctx);
                    break;
                case Beat.Windup:
                    UpdateWindup(ctx);
                    break;
                case Beat.BigDash:
                    UpdateBigDash(ctx);
                    break;
                default:
                    UpdateChase(ctx);
                    break;
            }
        }

        private void UpdateChase(ThunderveinDragonContext ctx)
        {
            ctx.Npc.QuickSetDirection();
            ctx.DeclareChase(ctx.Target.Center, ThunderveinDirector.RaidChase);
            ctx.DeclareRotationNormal();

            ctx.Boss.GetLengthToTargetPos(ctx.Target.Center, out float xLength, out float yLength);
            bool ready = xLength > ThunderveinDirector.RaidChaseExitX
                && yLength < ThunderveinDirector.RaidChaseExitY
                && Timer > ThunderveinDirector.RaidChaseMinFrames;
            if (Timer <= ThunderveinDirector.RaidChaseFrames && !ready)
            {
                return;
            }

            ctx.Boss.DashFrame();
            ctx.Boss.ResetAllOldCaches();
            SwitchBeat(ctx, (int)Beat.Zig);
        }

        /// <summary>乱窜：每 12/14 帧换一次方向，共 3 段。旧 :68-131</summary>
        private void UpdateZig(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareDashing();
            ctx.ShadowScale = ThunderveinDirector.DashShadowScale;
            ctx.DeclareDirect();

            int zigFrames = ThunderveinDirector.RaidZigFrames(ctx.Phase);
            if (T == 0 || T % zigFrames == 0)
            {
                npc.TargetClosest();
                zigThisFrame = true;

                float targetRot = (ctx.Target.Center - npc.Center).ToRotation();
                // 偏角无条件抽取（两端 RNG 同序推进），只有 700 px 内才真正应用
                float offset = T == 0
                    ? ctx.AttackRandSign() * ctx.AttackRandFloat(ThunderveinDirector.RaidZigFirstOffsetMin, ThunderveinDirector.RaidZigFirstOffsetMax)
                    : (T / zigFrames > 1 ? -1 : 1) * ctx.AttackRandFloat(ThunderveinDirector.RaidZigOffsetMin, ThunderveinDirector.RaidZigOffsetMax);
                if (ctx.TargetDistance < ThunderveinDirector.RaidZigOffsetDistance)
                {
                    targetRot += offset;
                }

                npc.velocity = targetRot.ToRotationVector2() * (T == 0 ? ThunderveinDirector.RaidZigFirstSpeed : ThunderveinDirector.RaidZigSpeed);
                npc.rotation = npc.velocity.ToRotation();
                npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);

                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, npc.Center);
                }
            }

            ctx.Boss.UpdateAllOldCaches();

            if (Timer <= (zigFrames * ThunderveinDirector.RaidZigCount) - ThunderveinDirector.RaidZigEndEarly)
            {
                return;
            }

            npc.velocity *= 0;
            npc.frame.X = 0;
            npc.frame.Y = 4;
            npc.frameCounter = 0;
            npc.rotation = LevelRotation(npc);
            npc.TargetClosest();
            SwitchBeat(ctx, (int)Beat.Windup);
        }

        /// <summary>
        /// 扇翅蓄力：从翅膀帧 4 起每 8/6 帧推一格，推过第 7 格起冲（= 32/24 帧）。<br/>
        /// 旧代码靠 <c>frameCounter</c> 计数，这里改为纯 Timer 推导，客户端可重建。旧 :132-177
        /// </summary>
        private void UpdateWindup(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.ShadowScale = ThunderveinDirector.DashShadowScale;
            npc.QuickSetDirection();
            ctx.DeclareRotationNormal(ThunderveinDirector.RaidWindupRotRate);

            if (npc.velocity.Length() < ThunderveinDirector.RaidWindupBackSpeed)
            {
                npc.velocity += (npc.Center - ctx.Target.Center).SafeNormalize(Vector2.Zero) * ThunderveinDirector.RaidWindupBackAccel;
            }

            ctx.DeclareDirect();
            ctx.Boss.UpdateAllOldCaches();
            npc.frame.Y = Math.Min(4 + (Timer / ThunderveinDirector.RaidFlapFrameTicks(ctx.Phase)), 7);

            if (Timer < ThunderveinDirector.RaidFlapFrames(ctx.Phase))
            {
                return;
            }

            ctx.Boss.DashFrame();
            Vector2 dir = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero);
            npc.velocity = dir * ThunderveinDirector.RaidBigDashSpeed;
            npc.rotation = npc.velocity.ToRotation();
            npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
            bigDashThisFrame = true;
            PlayLaunchCue(ctx, dir);
            SwitchBeat(ctx, (int)Beat.BigDash);
        }

        private static void PlayLaunchCue(ThunderveinDragonContext ctx, Vector2 dir)
        {
            if (Main.dedServ)
            {
                return;
            }

            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, ctx.Npc.Center);
            ThunderveinDragon.SetBackgroundLight(ThunderveinDirector.RaidSkyLight,
                ThunderveinDirector.RaidBigDashFrames - ThunderveinDirector.RaidSkyLightFadeOffset, ThunderveinDirector.RaidSkyLightExchange);
            Shake(ctx, dir * ThunderveinDirector.RaidShakeDirScale, ThunderveinDirector.RaidShakeStrength,
                ThunderveinDirector.RaidShakeVibration, ThunderveinDirector.RaidShakeFrames);
        }

        /// <summary>长冲 25 帧 → 刹车 → 后摇。旧 :178-207 / :375-415</summary>
        private void UpdateBigDash(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DrawShadows = true;
            ctx.ShadowScale = ThunderveinDirector.DashShadowScale;
            ctx.Boss.UpdateAllOldCaches();

            int bigDash = ThunderveinDirector.RaidBigDashFrames;
            if (T < bigDash)
            {
                ctx.IsDashing = true;
                ctx.CurrentSurrounding = true;
                ctx.DeclareKeep();
                return;
            }

            if (T == bigDash)
            {
                npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);
                npc.rotation = LevelRotation(npc);
                ctx.DeclareKeep();
                return;
            }

            ctx.DeclareDamp(ThunderveinDirector.RaidBrakeDamp);
            ctx.Boss.FlyingFrame();
            if (Math.Abs(npc.velocity.X) < ThunderveinDirector.RaidBrakeStopX)
            {
                npc.QuickSetDirection();
                npc.rotation = LevelRotation(npc);
            }
        }

        protected override void OnBeatAdopted(ThunderveinDragonContext ctx, int previousBeat)
        {
            if (CurrentBeat == Beat.BigDash)
            {
                PlayLaunchCue(ctx, ctx.Npc.velocity.SafeNormalize(Vector2.Zero));
            }
        }

        protected override IVaultState<ThunderveinDragonContext> AuthorityUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;

            if (zigThisFrame)
            {
                zigThisFrame = false;
                ctx.MarkDecision();
                int type = ctx.Phase == 1 ? ModContent.ProjectileType<LightningDash>() : ModContent.ProjectileType<StrongLightningDash>();
                npc.NewProjectileDirectInAI(npc.Center, Vector2.Zero, type, ThunderveinDirector.RaidZigDamage(), 0,
                    -1, ThunderveinDirector.RaidZigFrames(ctx.Phase), npc.whoAmI, ThunderveinDirector.RaidZigProjAi2);
            }

            if (bigDashThisFrame)
            {
                bigDashThisFrame = false;
                ctx.MarkDecision();
                int type = ctx.Phase == 1 ? ModContent.ProjectileType<LightningDash>() : ModContent.ProjectileType<StrongLightningDash>();
                // 一阶段长冲的 owner 是目标玩家，二阶段是 -1（沿用旧值 AI.LightningRaid.cs:155-156 / :352-353）
                npc.NewProjectileDirectInAI(npc.Center, Vector2.Zero, type, ThunderveinDirector.RaidBigDamage(ctx.Phase), 0,
                    ctx.Phase == 1 ? npc.target : -1, ThunderveinDirector.RaidBigDashFrames, npc.whoAmI, ThunderveinDirector.RaidBigProjAi2);
            }

            // 二阶段长冲途中每 9 帧留一颗交错电球
            if (ctx.Phase != 1 && CurrentBeat == Beat.BigDash && T < ThunderveinDirector.RaidBigDashFrames
                && T % ThunderveinDirector.RaidTrailBallInterval == 0)
            {
                npc.NewProjectileDirectInAI<StrongerCrossLightingBall>(npc.Center, Vector2.Zero,
                    ThunderveinDirector.RaidBigDamage(ctx.Phase), 0, -1, npc.whoAmI,
                    npc.rotation + MathHelper.PiOver4 + (T / ThunderveinDirector.RaidTrailBallAngleDivisor * MathHelper.PiOver2)
                        + ThunderveinDirector.RaidTrailBallAngleEpsilon);
            }

            if (CurrentBeat == Beat.BigDash && Timer > ThunderveinDirector.RaidBigDashFrames + ThunderveinDirector.RaidDelayFrames(ctx.Phase))
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
