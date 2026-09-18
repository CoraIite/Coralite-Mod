using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Core;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 龙车（横冲）：先和目标拉到同一水平线 → 停住、闪光 + 吼一声（10 帧起手预告）→ 18 px/f 横向爆冲 → 掠过目标 240 px 或飞满 75 帧后硬刹。<br/>
    /// 公平阀：起手时速度归零并锁面向，之后不再追瞄，玩家上下位移即可脱开；跑道判定只看 X，所以垂直闪避总是成立。旧 AI.HorizontalDash.cs
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.horizontalDash, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonHorizontalDashState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.horizontalDash;

        private enum Beat
        {
            /// <summary>对齐水平线。</summary>
            Approach,
            /// <summary>锁向爆冲。</summary>
            Dash,
            /// <summary>硬刹收招。</summary>
            Brake,
        }

        /// <summary>刹车段帧数：旧代码 76~85 共 10 帧。</summary>
        private static int BrakeFrames => BabyIceDragonDirector.DashEndFrame - BabyIceDragonDirector.DashBrakeStart;

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Dash:
                    UpdateDash(ctx, BabyIceDragonDirector.DashPassDistance);
                    break;
                case Beat.Brake:
                    ctx.DeclareDamp(BabyIceDragonDirector.DashBrakeDamp);
                    ctx.DeclareFlyingFrame(1);
                    break;
                default:
                    if (DashApproach(ctx))
                    {
                        ChangeBeat(ctx, (int)Beat.Dash);
                    }

                    break;
            }
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Brake:
                    if (Timer > BrakeFrames)
                    {
                        return EndAttack(ctx);
                    }

                    break;
                case Beat.Dash:
                    break;
                default:
                    if (Timer > BabyIceDragonDirector.DashApproachTimeout)
                    {
                        return EndAttack(ctx);
                    }

                    break;
            }

            return null;
        }

        /// <summary>
        /// 龙车与龙车变种共用的就位段：高差超过 32 或横向超过 450 就追，到位就停住 + 闪光 + 吼叫并返回 true。旧 AI.HorizontalDash.cs:17-60
        /// </summary>
        internal static bool DashApproach(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            float yLength = Math.Abs(npc.Center.Y - ctx.Target.Center.Y);
            float xLength = Math.Abs(npc.Center.X - ctx.Target.Center.X);

            if (yLength > BabyIceDragonDirector.DashAlignY || xLength > BabyIceDragonDirector.DashAlignX)
            {
                ctx.FaceTarget();
                ctx.DeclareHoverY(0f, 0f, BabyIceDragonDirector.DashAlignY, BabyIceDragonDirector.DashApproachSpeedY,
                    BabyIceDragonDirector.DashApproachAccelY, BabyIceDragonDirector.DashApproachTurnY, BabyIceDragonDirector.DashApproachDamp);
                ctx.DeclareApproachX(BabyIceDragonDirector.DashApproachDeadZoneX, BabyIceDragonDirector.DashApproachSpeedX,
                    BabyIceDragonDirector.DashApproachAccelX, BabyIceDragonDirector.DashApproachTurnX,
                    BabyIceDragonDirector.DashApproachDamp, BabyIceDragonDirector.DashApproachIdleDampX);
                ctx.DeclareFlyingFrame();
                return false;
            }

            ctx.FaceTarget();
            SparkCue(ctx, BabyIceDragonDirector.DashCueSparkScale);
            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Roar, npc.Center);
            }

            npc.velocity = Vector2.Zero;
            ctx.DeclareDirect();
            return true;
        }

        /// <summary>
        /// 龙车与龙车变种共用的冲刺体：张嘴帧，10 帧锁向起冲 18 px/f，之后保持速度并拉残影。旧 AI.HorizontalDash.cs:62-72
        /// </summary>
        internal static void UpdateDashBody(BabyIceDragonContext ctx, int timer, bool faceOnLaunch)
        {
            ctx.DeclareFlyingFrame(1);

            if (timer == BabyIceDragonDirector.DashLaunchFrame)
            {
                NPC npc = ctx.Npc;
                npc.velocity = new Vector2(npc.direction * BabyIceDragonDirector.DashSpeed, 0f);
                ctx.DeclareDirect();
                ctx.DrawShadows = true;
                if (faceOnLaunch)
                {
                    // 龙车起冲后补一次面向（旧 AI.HorizontalDash.cs:71 有、AI.DoubleDash.cs 没有，原样保留差异）
                    ctx.FaceTarget();
                }

                ctx.MarkDecision();
                return;
            }

            ctx.DeclareKeep();
            ctx.DrawShadows = timer > BabyIceDragonDirector.DashLaunchFrame;
        }

        /// <summary>
        /// 龙车与龙车变种共用的越过判定：35 帧后开始判，越过 <paramref name="passDistance"/> 或满 75 帧即该转刹车。旧 AI.HorizontalDash.cs:74-86
        /// </summary>
        internal static bool DashShouldBrake(BabyIceDragonContext ctx, int timer, float passDistance)
        {
            if (timer < BabyIceDragonDirector.DashPassCheckStart)
            {
                return false;
            }

            return timer > BabyIceDragonDirector.DashBrakeStart
                || Math.Abs(ctx.Npc.Center.X - ctx.Target.Center.X) > passDistance;
        }

        /// <summary>冲刺拍：起冲 + 越过判定。</summary>
        private void UpdateDash(BabyIceDragonContext ctx, float passDistance)
        {
            UpdateDashBody(ctx, Timer, true);

            if (DashShouldBrake(ctx, Timer, passDistance))
            {
                ChangeBeat(ctx, (int)Beat.Brake);
            }
        }
    }
}
