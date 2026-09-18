using Coralite.Content.Bosses.BabyIceDragon.Core;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 冰吐息（一 / 二阶段主力）：飞到目标头顶 200 px 内 → 10 帧蓄力（冰星聚向嘴前，可见预告）→ 30~41 帧每 5 帧吐两发扇形吐息 → 进后摇。<br/>
    /// 吐息期间仍以 3 px/f 的低速跟随，玩家横向拉开就能脱出锥形；预告窗固定 20 帧，不受随机量影响。旧 AI.IceBreath.cs
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.iceBreath, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonIceBreathState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.iceBreath;

        private enum Beat
        {
            /// <summary>飞到吐息距离内。</summary>
            Approach,
            /// <summary>蓄力 + 吐息。</summary>
            Breath,
        }

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if ((Beat)BeatIndex == Beat.Approach)
            {
                UpdateApproach(ctx);
                return;
            }

            UpdateBreath(ctx);
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if ((Beat)BeatIndex == Beat.Approach)
            {
                return Timer > BabyIceDragonDirector.BreathApproachTimeout ? EndAttack(ctx) : null;
            }

            if (Timer >= BabyIceDragonDirector.BreathFireEnd)
            {
                return EnterRest(ctx, BabyIceDragonDirector.RestFramesAfterBreath());
            }

            if (Timer >= BabyIceDragonDirector.BreathFireStart && Timer % BabyIceDragonDirector.BreathFireInterval == 0)
            {
                Fire(ctx);
            }

            return null;
        }

        /// <summary>就位：距离超过 440 就追到目标上方 200 px。旧 AI.IceBreath.cs:18-45</summary>
        private void UpdateApproach(BabyIceDragonContext ctx)
        {
            if (Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) > BabyIceDragonDirector.BreathApproachDistance)
            {
                ctx.FaceTarget();
                ctx.DeclareHoverY(BabyIceDragonDirector.BreathHoverY, BabyIceDragonDirector.BreathHoverY, BabyIceDragonDirector.BreathDeadZoneY,
                    BabyIceDragonDirector.BreathApproachSpeedY, BabyIceDragonDirector.BreathApproachAccelY, BabyIceDragonDirector.BreathApproachTurnY,
                    BabyIceDragonDirector.BreathApproachDamp);
                ctx.DeclareApproachX(BabyIceDragonDirector.BreathApproachDeadZoneX, BabyIceDragonDirector.BreathApproachSpeedX,
                    BabyIceDragonDirector.BreathApproachAccelX, BabyIceDragonDirector.BreathApproachTurnX,
                    BabyIceDragonDirector.BreathApproachDamp, BabyIceDragonDirector.BreathApproachIdleDampX);
                ctx.DeclareFlyingFrame();
                return;
            }

            ctx.FaceTarget();
            ctx.DeclareKeep();
            ChangeBeat(ctx, (int)Beat.Breath);
        }

        /// <summary>蓄力与吐息段：张嘴帧、低速跟随，10 帧蓄力提示，30 帧起开火。旧 AI.IceBreath.cs:48-105</summary>
        private void UpdateBreath(BabyIceDragonContext ctx)
        {
            ctx.DeclareFlyingFrame(1);

            if (Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) > BabyIceDragonDirector.BreathKeepDistance)
            {
                ctx.FaceTarget();
                ctx.DeclareHoverY(BabyIceDragonDirector.BreathHoverY, BabyIceDragonDirector.BreathHoverY, BabyIceDragonDirector.BreathDeadZoneY,
                    BabyIceDragonDirector.BreathFollowSpeed, BabyIceDragonDirector.BreathFollowAccel, BabyIceDragonDirector.BreathFollowTurn,
                    BabyIceDragonDirector.BreathApproachDamp);
                ctx.DeclareApproachX(BabyIceDragonDirector.BreathFollowDeadZoneX, BabyIceDragonDirector.BreathFollowSpeed,
                    BabyIceDragonDirector.BreathFollowAccel, BabyIceDragonDirector.BreathFollowTurn,
                    BabyIceDragonDirector.BreathApproachDamp, BabyIceDragonDirector.BreathApproachIdleDampX);
            }
            else
            {
                ctx.DeclareKeep();
            }

            if (CueDue(BabyIceDragonDirector.BreathChargeCueFrame))
            {
                ChargeCue(ctx);
            }

            if (Timer >= BabyIceDragonDirector.BreathFireStart && Timer < BabyIceDragonDirector.BreathFireEnd
                && Timer % BabyIceDragonDirector.BreathFireInterval == 0 && !Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, ctx.Npc.Center);
            }
        }

        /// <summary>吐两发：瞄准目标 ±30 px 的散布点，散角 0 与 -0.05。旧 AI.IceBreath.cs:90-103</summary>
        private static void Fire(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 targetDir = (ctx.Target.Center + Main.rand.NextVector2CircularEdge(BabyIceDragonDirector.BreathAimSpread, BabyIceDragonDirector.BreathAimSpread) - npc.Center)
                .SafeNormalize(Vector2.Zero);
            Vector2 mouth = ctx.MouthCenter();
            int damage = BabyIceDragonDirector.BreathDamage();

            for (int i = -1; i < 1; i++)
            {
                ctx.SpawnHostile<IceBreath>(npc.GetSource_FromAI(), mouth,
                    targetDir.RotatedBy(i * BabyIceDragonDirector.BreathSpreadStep) * BabyIceDragonDirector.BreathSpeed,
                    damage, BabyIceDragonDirector.BreathKnockback);
            }

            ctx.MarkDecision();
        }
    }
}
