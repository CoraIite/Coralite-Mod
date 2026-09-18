using Coralite.Content.Bosses.ThunderveinDragon.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ThunderveinDragon.States
{
    /// <summary>
    /// 短冲：调整身位用的位移招，穿插在各类招式之间。<br/>
    /// 节拍：Dash（35 px/f 冲 15/17 帧，首段带 ±(0.9~1) 弧度随机偏角、后续段直冲）→ 段末与玩家仍超过 700 px 就再来一段
    /// （P1 最多 2 段 / P2 最多 3 段）→ Settle（7 帧回正收势）。<br/>
    /// 公平阀：首段偏角让它不会直接贴脸，冲刺本体伤害挂在 <c>LightningDash</c> 弹幕上而不是接触伤害。<br/>
    /// 旧 AI.SmallDash.cs（P1 :11-124 / P2 :126-248）
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.SmallDash, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinSmallDashState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.SmallDash;

        private enum Beat
        {
            /// <summary>冲刺段（旧 SonState 0/1/2）</summary>
            Dash = 0,
            /// <summary>收势（旧 P1 的 2 / P2 的 3）</summary>
            Settle = 1,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>已起跳的段数（热字段 A）。</summary>
        private int dashCount;

        /// <summary>本帧起跳，权威端同帧出冲刺判定弹幕。</summary>
        private bool launchThisFrame;

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            launchThisFrame = false;

            if (CurrentBeat == Beat.Settle)
            {
                UpdateSettle(ctx);
                return;
            }

            UpdateDash(ctx);
        }

        private void UpdateDash(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;

            // 冲刺姿态每帧重申，客户端收养后也成立
            ctx.DeclareDashing();
            ctx.ShadowScale = ThunderveinDirector.DashShadowScale;
            ctx.DeclareDirect();

            if (T == 0)
            {
                npc.TargetClosest();
                ctx.Boss.DashFrame();
                if (dashCount == 0)
                {
                    ctx.Boss.ResetAllOldCaches();
                }

                float targetRot = (ctx.Target.Center - npc.Center).ToRotation();
                // 首段带随机偏角（确定性 AttackRandom，两端同序消费）
                if (dashCount == 0)
                {
                    targetRot += ctx.AttackRandSign() * ctx.AttackRandFloat(ThunderveinDirector.SmallDashOffsetMin, ThunderveinDirector.SmallDashOffsetMax);
                }

                npc.velocity = targetRot.ToRotationVector2() * ThunderveinDirector.SmallDashSpeed;
                npc.rotation = npc.velocity.ToRotation();
                npc.direction = npc.spriteDirection = Math.Sign(npc.velocity.X);

                dashCount++;
                launchThisFrame = true;
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, npc.Center);
                }
            }

            ctx.Boss.UpdateAllOldCaches();

            if (Timer <= ThunderveinDirector.SmallDashFrames(ctx.Phase) - ThunderveinDirector.SmallDashEndEarly)
            {
                return;
            }

            bool again = ctx.TargetDistance > ThunderveinDirector.SmallDashContinueDistance
                && dashCount < ThunderveinDirector.SmallDashMaxCount(ctx.Phase);

            npc.velocity *= 0;
            npc.TargetClosest();

            if (again)
            {
                // 同一拍再起一段：Timer 清零，下一帧 T == 0 重新起跳
                SwitchBeat(ctx, (int)Beat.Dash);
                return;
            }

            npc.frame.X = 0;
            npc.frame.Y = 4;
            npc.frameCounter = 0;
            npc.rotation = LevelRotation(npc);
            SwitchBeat(ctx, (int)Beat.Settle);
        }

        /// <summary>收势：旧代码此拍不再推进残影缓存（残影定格）也不清 <c>canDrawShadows</c>，保持原样。旧 AI.SmallDash.cs:113-121</summary>
        private void UpdateSettle(ThunderveinDragonContext ctx)
        {
            ctx.DrawShadows = true;
            ctx.ShadowScale = ThunderveinDirector.DashShadowScale;
            ctx.Boss.FlyingFrame();
            ctx.DeclareNoRot();
            ctx.DeclareKeep();
        }

        protected override IVaultState<ThunderveinDragonContext> AuthorityUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            if (launchThisFrame)
            {
                launchThisFrame = false;
                ctx.MarkDecision();
                int type = ctx.Phase == 1 ? ModContent.ProjectileType<LightningDash>() : ModContent.ProjectileType<StrongLightningDash>();
                ctx.Npc.NewProjectileDirectInAI(ctx.Npc.Center, Vector2.Zero, type, ThunderveinDirector.SmallDashDamage(), 0,
                    ctx.Npc.target, ThunderveinDirector.SmallDashFrames(ctx.Phase), ctx.Npc.whoAmI, ThunderveinDirector.SmallDashProjAi2);
            }

            if (CurrentBeat == Beat.Settle && Timer > ThunderveinDirector.SmallDashSettleFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        protected override void WriteSlots(ThunderveinDragonContext ctx)
            => ctx.Hot[CoraliteBossHotSlots.A] = dashCount;

        protected override void ReadSlots(ThunderveinDragonContext ctx)
            => dashCount = (int)ctx.Hot[CoraliteBossHotSlots.A];
    }
}
