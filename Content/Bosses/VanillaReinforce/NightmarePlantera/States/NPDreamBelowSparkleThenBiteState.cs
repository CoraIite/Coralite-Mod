using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 下方闪光咬：瞬移到玩家正下方自旋 360 帧，一边在下方 460 px 处左右大幅摆动、一边向上喷光；
    /// 转完从玩家面朝方向的远处斜向上冲，拖一条裂缝穿过屏幕，落定后引爆。<br/>
    /// 节拍：淡出 45 帧 → 自旋追击 360 帧 → 冲刺贯穿 40 帧 → 后摇 56 帧。<br/>
    /// 与转圈咬的区别：本体一直在玩家脚下移动，弹幕从下往上顶，逼玩家离地。<br/>
    /// 原 <c>BelowSparkleThenBite</c>（Phase.P2_Dream.cs:724-906）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.belowSparkleThenBite, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamBelowSparkleThenBiteState : NPDreamSlitStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.belowSparkleThenBite;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case SlitBeat.Fade:
                    FadeBeat(ctx);
                    break;
                case SlitBeat.Spin:
                    SpinBeat(ctx);
                    break;
                case SlitBeat.Dash:
                    DashBeat(ctx);
                    break;
                default:
                    RecoverBeat(ctx);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            bool done = FadeTickP2(ctx,
                () => ctx.TargetCenter + new Vector2(0, NightmarePlanteraDirector.P2BelowTeleportDown),
                onTeleport: () => npc.rotation = boss.NextAttackFloat(MathHelper.TwoPi));

            boss.NormallySetTentacle();

            if (done)
            {
                SwitchBeat(ctx, (int)SlitBeat.Spin);
            }
        }

        /// <summary>自旋追击：在玩家脚下左右摆，边转边三向喷光，另外每 20 帧往正上方顶一发。</summary>
        private void SpinBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            int rolling = NightmarePlanteraDirector.P2RollingFrames;

            SpinFog(ctx);

            float rotFactor = Math.Clamp(Timer / (rolling / 3f), 0, 1);
            npc.rotation += rotFactor * NightmarePlanteraDirector.P2RollingSpinStep;

            Vector2 center = npc.Center;
            int damage = NightmarePlanteraDirector.P2SparkleDamage();

            if (Timer < rolling)
            {
                Vector2 anchor = ctx.TargetCenter + new Vector2(
                    NightmarePlanteraDirector.P2BelowSwayWidth * MathF.Sin(NightmarePlanteraDirector.P2BelowSwayFrequency * Timer / rolling),
                    NightmarePlanteraDirector.P2BelowSwayHeight);
                ctx.DeclareApproach(anchor, NightmarePlanteraDirector.P2BelowFollowTurn, NightmarePlanteraDirector.P2BelowFollowMaxSpeed,
                    NightmarePlanteraDirector.P2BelowFollowBlend, NightmarePlanteraDirector.P2BelowFollowSpeedRange);

                if (Timer % NightmarePlanteraDirector.P2BelowUpSparkleInterval == 0)
                {
                    npc.NewProjectileInAI_Server<NightmareSparkle_Normal>(center, -Vector2.UnitY, damage, 0);
                    ctx.MarkDecision();
                }
            }
            else
            {
                ctx.DeclareDamp(NightmarePlanteraDirector.P2BelowHoldDamp);
            }

            int delay = NightmarePlanteraDirector.P2BelowDelayBase - (int)(rotFactor * NightmarePlanteraDirector.P2RollingDelayCut);
            if (Timer % delay == 0)
            {
                for (int i = 0; i < NightmarePlanteraDirector.P2BelowSparkleCount; i++)
                {
                    Vector2 dir = (npc.rotation + (i * MathHelper.TwoPi / NightmarePlanteraDirector.P2BelowSparkleCount)).ToRotationVector2();
                    npc.NewProjectileInAI_Server<NightmareSparkle_Normal>(center, dir, damage, 0);
                }

                ctx.MarkDecision();
                Helper.PlayPitched(CoraliteSoundID.CrystalSerpent_Item109, npc.Center, pitch: -0.5f);
            }

            SpinTentacles(ctx, center, center, Timer / (float)rolling);
            SpinFadeOut(ctx, center);

            if (Timer <= rolling)
            {
                return;
            }

            Land(ctx);
            SwitchBeat(ctx, (int)SlitBeat.Dash);
        }

        /// <summary>落点：玩家面朝方向 800~1000、下方 200，斜向上 48 速度冲——冲刺方向永远是"从脚下往玩家脸上翻"。</summary>
        private static void Land(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.alpha = 1;
            boss.canDrawWarp = false;

            int direction = ctx.Target?.direction ?? 1;
            npc.Center = ctx.TargetCenter + new Vector2(
                direction * boss.AttackRandom.Next(NightmarePlanteraDirector.P2BelowBiteSideMin, NightmarePlanteraDirector.P2BelowBiteSideMax),
                NightmarePlanteraDirector.P2BelowBiteHeight);
            npc.velocity = (direction == 1
                ? NightmarePlanteraDirector.P2BelowBiteAngleRight
                : NightmarePlanteraDirector.P2BelowBiteAngleLeft).ToRotationVector2() * NightmarePlanteraDirector.P2RollingBiteSpeed;
            npc.rotation = npc.velocity.ToRotation();

            SnapTentacles(ctx);
            SpawnSlit(ctx);
        }
    }
}
