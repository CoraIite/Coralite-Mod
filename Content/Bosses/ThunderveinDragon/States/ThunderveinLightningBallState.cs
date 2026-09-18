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
    /// 电球：吐一颗追踪电球，二阶段改为三选一（三向散射 / 单球 + 直线链球 / 旋转链球）。<br/>
    /// 节拍：Charge（贴近 25 帧，嘴前吸入尘半径 140→80 收拢 = 预告）→ Swing（收翅，翅膀到位后 10 帧吐出）
    /// → Burst（25 帧，残影 1→1.5 淡出）→ Recover（10 帧后摇，全招最短）。<br/>
    /// 公平阀：吸入尘从嘴前收拢，出口方向即弹幕方向；本招不带电、不减伤，是轮换里的“轻手”。<br/>
    /// 旧 AI.LightningBall.cs:12-176
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.LightningBall, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinLightningBallState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.LightningBall;

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
            float edge = ThunderveinDirector.BallDustEdgeFrom
                - (ThunderveinDirector.BallDustEdgeShrink * Math.Clamp(T / (float)ThunderveinDirector.BallReadyFrames, 0f, 1f));
            MouthDust(ctx, edge / 2f, ThunderveinDirector.MouthDustSpeedMin, ThunderveinDirector.MouthDustSpeedMax, ThunderveinDirector.MouthDustScaleMax);

            ctx.Npc.QuickSetDirection();
            ctx.DeclareChase(ctx.Target.Center, ThunderveinDirector.CloseChaseOpenMouth);
            ctx.DeclareRotationNormal();

            if (ctx.Npc.frame.Y == 0 && Timer > ThunderveinDirector.BallReadyFrames)
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
            BurstShadowEnvelope(ctx, T / (float)ThunderveinDirector.BallBurstFrames, ThunderveinDirector.SmallBurstShadowScaleTo);
            BurstMouthFrame(ctx.Npc);

            if (Timer > ThunderveinDirector.BallBurstFrames)
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
                Fire(ctx);
            }

            if (CurrentBeat == Beat.Recover && Timer > ThunderveinDirector.BallRecoverFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        /// <summary>
        /// 仅权威端。二阶段的三选一走 <see cref="ThunderveinDragonContext.AttackRandom"/>——旧代码同样只在权威端抽这一次，
        /// 客户端不消费（本招此后不再用招内随机，两端不会错位）。旧 AI.LightningBall.cs:86-131
        /// </summary>
        private static void Fire(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 mouth = ctx.Boss.GetMousePos();
            Vector2 dir = (ctx.Target.Center - mouth).SafeNormalize(Vector2.Zero);

            if (ctx.Phase == 1)
            {
                npc.NewProjectileInAI_Server<LightningBall>(mouth, dir * ThunderveinDirector.BallSpeed,
                    ThunderveinDirector.BallDamageP1(), 0, npc.target);
                return;
            }

            int damage = ThunderveinDirector.BallDamageP2();
            switch (ctx.AttackRandom.Next(ThunderveinDirector.BallVariantCount))
            {
                default:
                case 0://三重电球
                    for (int i = -1; i < 2; i++)
                    {
                        npc.NewProjectileInAI_Server<StrongLightningBall>(mouth,
                            dir.RotatedBy(i * ThunderveinDirector.BallTripleSpread), damage, 0, npc.target);
                    }

                    break;
                case 1://单电球 + 直线链球
                    npc.NewProjectileInAI_Server<StrongLightningBall>(mouth, dir * ThunderveinDirector.BallSpeed, damage, 0, npc.target);
                    npc.NewProjectileInAI_Server<ChainBall>(mouth, dir * ThunderveinDirector.ChainBallSpeed, damage, 0, npc.target, 0);
                    break;
                case 2://旋转链球
                    npc.NewProjectileInAI_Server<ChainBall>(mouth, dir * ThunderveinDirector.ChainBallSpeed, damage, 0, npc.target, 1);
                    break;
            }
        }
    }
}
