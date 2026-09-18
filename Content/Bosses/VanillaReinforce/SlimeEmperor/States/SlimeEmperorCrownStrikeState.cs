using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 王冠冲击：缩进王冠 → 绕着玩家兜圈 → 蓄力锁向 → 一次直线冲撞 → 刹车变回史莱姆。<br/>
    /// 节拍：Shrink0..3（四拍缩进，共用前摇）→ Circle → Dash → Restore → Done。<br/>
    /// 公平阀：缩进四拍本身就是“要变形态了”的长预告；冲撞前有 30 帧（挑战 20）的静止蓄力，
    /// 出手那两帧射向锁死不再追瞄——预告指哪就打哪，之后 70 帧只是直线滑行，玩家侧移即可躲。<br/>
    /// 王冠形态期间防御 +30（挑战 +50 / 无敌档 9999 并带霸体与弹幕反弹），由宿主按 <c>CrownForm</c> 每帧重算。<br/>
    /// 旧 <c>SlimeEmperor.CrownStrike</c>（AI.CrownStrike.cs:12-188）。
    /// </summary>
    [VaultState((int)AIStates.CrownStrike, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorCrownStrikeState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.CrownStrike;

        private enum Beat
        {
            /// <summary>缩进四拍（旧 SonState 0..3），拍号必须与 Director 的四组形变目标对齐</summary>
            Shrink0 = 0,
            Shrink1 = 1,
            Shrink2 = 2,
            Shrink3 = 3,
            /// <summary>王冠形态绕圈（旧 4）</summary>
            Circle = 4,
            /// <summary>蓄力 → 锁向出手 → 冲撞 → 刹车（旧 5）</summary>
            Dash = 5,
            /// <summary>变回史莱姆并复原（旧 6）</summary>
            Restore = 6,
            /// <summary>收招（旧 7）</summary>
            Done = 7,
        }

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Shrink0:
                case Beat.Shrink1:
                case Beat.Shrink2:
                case Beat.Shrink3:
                    UpdateCrownShrinkBeats(ctx);
                    break;

                case Beat.Circle:
                    UpdateCircle(ctx);
                    break;

                case Beat.Dash:
                    UpdateDash(ctx);
                    break;

                case Beat.Restore:
                    CrownShrinkStep(ctx);
                    if (ScaleBeat(ctx, 1f, 1f, SlimeEmperorDirector.CrownStrikeRestoreLerp,
                        ctx.Scale.X > SlimeEmperorDirector.CrownStrikeRestoreDone, (int)Beat.Done))
                    {
                        ctx.Scale = Vector2.One;
                    }

                    break;
            }
        }

        /// <summary>绕圈：与玩家保持 400 px，靠近就反推、远了就追，限速封顶。</summary>
        private void UpdateCircle(SlimeEmperorContext ctx)
        {
            bool fast = ctx.Dangerous(Slime1Knowledge.Dangerous.SpeedBonus2_1);
            int frames = fast ? SlimeEmperorDirector.CrownStrikeCircleFramesFast : SlimeEmperorDirector.CrownStrikeCircleFrames;

            if (Timer >= frames)
            {
                EnterBeat(ctx, (int)Beat.Dash);
                ctx.Npc.TargetClosest();
                return;
            }

            float accel = fast ? SlimeEmperorDirector.CrownStrikeAccelFast : SlimeEmperorDirector.CrownStrikeAccel;
            float maxSpeed = fast ? SlimeEmperorDirector.CrownStrikeMaxSpeedFast : SlimeEmperorDirector.CrownStrikeMaxSpeed;
            KeepDistance(ctx, ctx.Target.Center, SlimeEmperorDirector.CrownStrikeKeepRadius, accel, maxSpeed);
        }

        /// <summary>蓄力 → 出手 → 冲撞 → 刹车。</summary>
        private void UpdateDash(SlimeEmperorContext ctx)
        {
            bool fast = ctx.Dangerous(Slime1Knowledge.Dangerous.SpeedBonus2_1);
            int waitFrames = fast ? SlimeEmperorDirector.CrownStrikeWaitFramesFast : SlimeEmperorDirector.CrownStrikeWaitFrames;
            int dashFrames = fast ? SlimeEmperorDirector.CrownStrikeDashFramesFast : SlimeEmperorDirector.CrownStrikeDashFrames;
            float dashSpeed = fast ? SlimeEmperorDirector.CrownStrikeDashSpeedFast : SlimeEmperorDirector.CrownStrikeDashSpeed;

            //蓄力：静止不动，这是唯一的躲避窗
            if (Timer < waitFrames)
            {
                ctx.DeclareDamp(SlimeEmperorDirector.CrownStrikeDamp);
                return;
            }

            //出手两帧：锁向 + 爆散尘；速度由原版同步带给客户端，MarkDecision 保证这一包立刻出门
            if (Timer < waitFrames + SlimeEmperorDirector.CrownStrikeLaunchFrames)
            {
                Vector2 dir = (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero);
                PlaySound(CoraliteSoundID.SlimeMount_Item81, ctx);
                SpawnLaunchDust(ctx, dir);
                ctx.Npc.velocity = dir * dashSpeed;
                ctx.DeclareDirect();
                ctx.MarkDecision();
                return;
            }

            SpawnTrailDust(ctx);

            if (Timer <= waitFrames + dashFrames + SlimeEmperorDirector.CrownStrikeBrakeDelay)
            {
                return;
            }

            ctx.DeclareDamp(SlimeEmperorDirector.CrownStrikeDamp);
            if (Timer <= waitFrames + dashFrames + SlimeEmperorDirector.CrownStrikeEndDelay)
            {
                return;
            }

            ctx.Npc.velocity.X *= 0;
            EnterBeat(ctx, (int)Beat.Restore);
            PlaySound(CoraliteSoundID.QueenSlime_Item154, ctx);
            ctx.EnterSlimeForm();
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            //缩进四拍跑完、刚变成王冠：在脚下撒一圈弹力球
            if (EnteredBeat == (int)Beat.Circle)
            {
                SpawnElasticBalls(ctx, ctx.Npc.Center, SlimeEmperorDirector.CrownStrikeBallCount(),
                    () => Helper.NextVec2Dir(SlimeEmperorDirector.BallScatterSpeedMin, SlimeEmperorDirector.BallScatterSpeedMax));
            }

            //大师模式收招时向上喷一束尖刺球
            if (EnteredBeat == (int)Beat.Done && Main.masterMode)
            {
                ShootUpward<SpikeGelBall>(ctx, SlimeEmperorDirector.CrownStrikeSpikeCount(), SlimeEmperorDirector.CrownStrikeSpikeDamage(),
                    SlimeEmperorDirector.CrownStrikeSpikeSpeed, SlimeEmperorDirector.CrownStrikeSpikeSpread, SlimeEmperorDirector.SpikeKnockback);
            }

            return ReadyToFinish((int)Beat.Done) ? EndAttack(ctx) : null;
        }

        #region 表现（纯本地）

        /// <summary>出手瞬间的爆散尘，朝冲刺反向撒开。</summary>
        private static void SpawnLaunchDust(SlimeEmperorContext ctx, Vector2 dir)
        {
            if (Main.dedServ)
            {
                return;
            }

            for (int i = 0; i < SlimeEmperorDirector.CrownStrikeLaunchDustCount; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    ctx.Npc.Center + Main.rand.NextVector2Circular(SlimeEmperorDirector.CrownStrikeDustScatter, SlimeEmperorDirector.CrownStrikeDustScatter),
                    DustID.TintableDust,
                    -dir * Main.rand.NextFloat(SlimeEmperorDirector.CrownStrikeLaunchDustSpeedMin, SlimeEmperorDirector.CrownStrikeLaunchDustSpeedMax),
                    SlimeEmperorDirector.GelDustAlpha, SlimeEmperorDirector.GelDustColor, SlimeEmperorDirector.CrownStrikeDustScale);
                dust.noGravity = true;
            }
        }

        /// <summary>冲撞途中的拖尾尘。</summary>
        private static void SpawnTrailDust(SlimeEmperorContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            for (int i = 0; i < SlimeEmperorDirector.CrownStrikeTrailDustCount; i++)
            {
                Helper.SpawnTrailDust(
                    ctx.Npc.Center + Main.rand.NextVector2Circular(SlimeEmperorDirector.CrownStrikeDustScatter, SlimeEmperorDirector.CrownStrikeDustScatter),
                    DustID.TintableDust,
                    _ => -ctx.Npc.velocity * Main.rand.NextFloat(SlimeEmperorDirector.CrownStrikeTrailSpeedMin, SlimeEmperorDirector.CrownStrikeTrailSpeedMax),
                    SlimeEmperorDirector.GelDustAlpha, SlimeEmperorDirector.GelDustColor, SlimeEmperorDirector.CrownStrikeDustScale);
            }
        }

        #endregion
    }
}
