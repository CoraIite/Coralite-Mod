using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 俯冲（有破绽招）：先扇翅爬到目标头顶 460 px 以上（爬不上去就超时放弃）→ 吼一声张嘴锁向 → 三帧后一口气扎下去 → 掠过目标或飞满 100 帧后刹车。<br/>
    /// 公平阀：爬升段本身就是长预告，起冲前锁向、之后不再追瞄；破绽在于嘴前撞到实心物块会把自己撞晕 300 帧。<br/>
    /// 爬升到位与掠过目标都是两端可复现的判定（只读已同步的位置），所以换拍两端各自做；撞墙进眩晕是转移，只在权威端裁决。旧 AI.Dive.cs
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.dive, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonDiveState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.dive;

        private enum Beat
        {
            /// <summary>扇翅爬升到目标头顶足够高。</summary>
            Climb,
            /// <summary>锁向后一口气扎下去。</summary>
            Flight,
            /// <summary>刹车收招，不留残速。</summary>
            Brake,
        }

        /// <summary>刹车段帧数：旧代码把 Timer 直接跳到 100 再跑到 130，等效 30 帧。</summary>
        private static int BrakeFrames => BabyIceDragonDirector.DiveEndFrame - BabyIceDragonDirector.DiveFlightFrames;

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Flight:
                    UpdateFlight(ctx);
                    break;
                case Beat.Brake:
                    ctx.DeclareDamp(BabyIceDragonDirector.DiveBrakeDamp);
                    ctx.DeclareFlyingFrame();
                    break;
                default:
                    UpdateClimb(ctx);
                    break;
            }
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Flight:
                    // 嘴前三个探针撞到实心物块 → 把自己撞晕（转移只在权威端裁决，客户端读 ai[0] 跟随）
                    if (Timer >= BabyIceDragonDirector.DiveLaunchFrame && HitWall(ctx))
                    {
                        return EnterDizzy(ctx, BabyIceDragonDirector.DizzyFrames);
                    }

                    break;
                case Beat.Brake:
                    // 刹车段每帧声明 0.95 衰减，收招时残速已经很小，不再在权威端单独乘一次（那样两端会差一次衰减）
                    if (Timer > BrakeFrames)
                    {
                        return EndAttack(ctx);
                    }

                    break;
                default:
                    if (Timer > BabyIceDragonDirector.DiveClimbTimeout)
                    {
                        return EndAttack(ctx);
                    }

                    break;
            }

            return null;
        }

        /// <summary>爬升：X 缓慢收速、朝向回正、扇翅上飞；够高就吼一声转入俯冲。旧 AI.Dive.cs:17-44</summary>
        private void UpdateClimb(BabyIceDragonContext ctx)
        {
            if (ctx.Npc.Center.Y > ctx.Target.Center.Y - BabyIceDragonDirector.DiveClimbHeight)
            {
                ctx.FaceTarget();
                ctx.DeclareDampX(BabyIceDragonDirector.DiveClimbDampX);
                ctx.DeclareFlyUp();
                ctx.DeclareRotation(BabyIceDragonRotationMode.TowardsZero, BabyIceDragonDirector.DiveClimbRotationStep);
                return;
            }

            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Roar, ctx.Npc.Center);
            }

            ctx.SetFrame(1, 0);
            ctx.DeclareKeep();
            ChangeBeat(ctx, (int)Beat.Flight);
        }

        /// <summary>俯冲：三帧内还能改面向，第三帧锁向起冲 13 px/f，掠过目标或飞满 100 帧转刹车。旧 AI.Dive.cs:46-95</summary>
        private void UpdateFlight(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;

            if (Timer < BabyIceDragonDirector.DiveLaunchFrame)
            {
                ctx.FaceTarget();
                ctx.DeclareKeep();
                return;
            }

            if (Timer == BabyIceDragonDirector.DiveLaunchFrame)
            {
                npc.velocity = (ctx.Target.Center - new Vector2(0, BabyIceDragonDirector.DiveAimOffsetY) - npc.Center).SafeNormalize(Vector2.Zero)
                    * BabyIceDragonDirector.DiveSpeed;
                npc.rotation = npc.velocity.ToRotation() + ctx.FacingFlip;
                ctx.DeclareDirect();
                ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
                ctx.DrawShadows = true;
                ctx.MarkDecision();
                return;
            }

            ctx.DeclareKeep();
            ctx.DrawShadows = true;

            if (Timer >= BabyIceDragonDirector.DiveFlightFrames || npc.Center.Y > ctx.Target.Center.Y - BabyIceDragonDirector.DivePassOffsetY)
            {
                ChangeBeat(ctx, (int)Beat.Brake);
            }
        }

        /// <summary>嘴前三个探针（中间 + 上下各一格）是否撞上实心物块。旧 AI.Dive.cs:69-80</summary>
        private static bool HitWall(BabyIceDragonContext ctx)
        {
            ctx.MouthGeometry(out Vector2 mouthDir, out Vector2 mouthCenter);
            Vector2 side = mouthDir.RotatedBy(BabyIceDragonDirector.QuarterTurn) * BabyIceDragonDirector.DiveProbeSpacing;
            for (int i = -1; i <= 1; i++)
            {
                if (Framing.GetTileSafely(mouthCenter + (i * side)).HasReallySolidTile())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
