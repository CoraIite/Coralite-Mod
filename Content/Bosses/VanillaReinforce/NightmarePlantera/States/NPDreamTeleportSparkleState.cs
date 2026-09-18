using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 瞬移闪光：贴脸瞬移 → 退到 650 处扇形吐光 → 远瞬移 → 绕玩家两整圈边转边钉刺 → 再贴脸两次吐光 → 收招。<br/>
    /// 节拍（旧 <c>SonState</c> 0~7 原样保留）：近瞬移 → 吐光 65 → 远瞬移 → 自旋 240 → 近瞬移 → 吐光 65 → 近瞬移 → 收招 25。<br/>
    /// 原 <c>TeleportSparkle</c>（Phase.P2_Dream.cs:1645-1778）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.teleportSparkle, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamTeleportSparkleState : NPDreamStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.teleportSparkle;

        /// <summary>拍号直接沿用旧 <c>SonState</c> 的 0~7 编号：它本来就是一串线性节拍，<c>BeatIndex</c> 已随热字段过线。</summary>
        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (BeatIndex)
            {
                case 1:
                case 5:
                    VolleyBeat(ctx);
                    break;
                case 2:
                    FarTeleportBeat(ctx);
                    break;
                case 3:
                    SpinBeat(ctx);
                    break;
                case 7:
                    RecoverBeat(ctx);
                    break;
                default:
                    NearTeleportBeat(ctx);
                    break;
            }
        }

        /// <summary>贴脸瞬移：落到玩家前进方向 150~200，落地把速度朝背离玩家的方向推一格，准备退开吐光。</summary>
        private void NearTeleportBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx,
                () => ctx.Boss.PickTargetTeleportOffset(NightmarePlanteraDirector.P2TpNearMin, NightmarePlanteraDirector.P2TpNearMax),
                fadeTime: NightmarePlanteraDirector.P2TpFadeFrames,
                postTeleport: () =>
                {
                    npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
                    npc.velocity = (npc.rotation + MathHelper.Pi).ToRotationVector2();
                    ctx.ShootCount = npc.rotation + MathHelper.Pi;
                }))
            {
                return;
            }

            SwitchBeat(ctx, BeatIndex + 1);
        }

        /// <summary>退到 650 处，每 15 帧吐一扇噩梦光，扇宽随时间变宽。</summary>
        private void VolleyBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.Boss.DoRotation(0.3f);

            ctx.DeclareApproach(ctx.TargetCenter + (ctx.ShootCount.ToRotationVector2() * NightmarePlanteraDirector.P2TpVolleyRadius),
                NightmarePlanteraDirector.P2TpVolleyTurn, NightmarePlanteraDirector.P2TpVolleyMaxSpeed,
                NightmarePlanteraDirector.P2TpVolleyBlend, NightmarePlanteraDirector.P2TpVolleySpeedRange);

            if (Timer > NightmarePlanteraDirector.P2TpVolleyInterval && Timer % NightmarePlanteraDirector.P2TpVolleyInterval == 0)
            {
                float howmany = NightmarePlanteraDirector.P2TpVolleyGrowth
                    * ((Timer - NightmarePlanteraDirector.P2TpVolleyGrowthOffset) / NightmarePlanteraDirector.P2TpVolleyInterval) / 2f;
                int damage = NightmarePlanteraDirector.P2SparkleDamage();

                for (float i = -(int)howmany; i < howmany; i++)
                {
                    npc.NewProjectileInAI_Server<NightmareSparkle_Normal>(npc.Center,
                        (npc.rotation + (i * NightmarePlanteraDirector.P2TpVolleySpread)).ToRotationVector2(), damage, 0);
                }

                ctx.MarkDecision();
                Helper.PlayPitched(CoraliteSoundID.CrystalSerpent_Item109, npc.Center, pitch: -0.5f);
            }

            if (Timer > NightmarePlanteraDirector.P2TpVolleyFrames)
            {
                SwitchBeat(ctx, BeatIndex + 1);
            }
        }

        /// <summary>远瞬移：落到 400~600，朝向随机，速度切到轨道切线方向准备自旋。</summary>
        private void FarTeleportBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx,
                () => boss.PickTargetTeleportOffset(NightmarePlanteraDirector.P2TpFarMin, NightmarePlanteraDirector.P2TpFarMax),
                fadeTime: NightmarePlanteraDirector.P2TpFadeFrames,
                postTeleport: () =>
                {
                    npc.rotation = boss.NextAttackFloat(MathHelper.TwoPi);
                    ctx.ShootCount = (npc.Center - ctx.TargetCenter).ToRotation();
                    npc.velocity = (ctx.ShootCount + MathHelper.PiOver2).ToRotationVector2();
                }))
            {
                return;
            }

            SwitchBeat(ctx, BeatIndex + 1);
        }

        /// <summary>自旋：240 帧绕玩家两整圈（半径 700），自身每帧转 0.55，每 6 帧朝玩家钉一根摆动的长刺。</summary>
        private void SpinBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            int rolling = NightmarePlanteraDirector.P2TpSpinFrames;

            npc.rotation += NightmarePlanteraDirector.P2TpSpinSelfStep;

            float currentRot = ctx.ShootCount + (Timer / (float)rolling * NightmarePlanteraDirector.P2TpSpinTurns * MathHelper.TwoPi);
            ctx.DeclareApproach(ctx.TargetCenter + (currentRot.ToRotationVector2() * NightmarePlanteraDirector.P2TpSpinRadius),
                NightmarePlanteraDirector.P2TpSpinTurn, NightmarePlanteraDirector.P2TpSpinMaxSpeed,
                NightmarePlanteraDirector.P2TpSpinBlend, NightmarePlanteraDirector.P2TpSpinSpeedRange);

            if (Timer % NightmarePlanteraDirector.P2TpSpinHoleInterval == 0)
            {
                Vector2 dir = (ctx.TargetCenter - npc.Center).SafeNormalize(Vector2.One);
                float swing = MathF.Cos(Timer / NightmarePlanteraDirector.P2TpSpinHoleInterval * NightmarePlanteraDirector.P2TpSpinHoleWave)
                    * NightmarePlanteraDirector.P2TpSpinHoleSwing;

                npc.NewProjectileInAI_Server<ConfusionHole>(npc.Center, dir.RotatedBy(swing),
                    NightmarePlanteraDirector.P2SparkleDamage(), 0, npc.target,
                    NightmarePlanteraDirector.P2TpSpinHoleTime, boss.ZenithProjSeed(), NightmarePlanteraDirector.P2TpSpinHoleLen);
                ctx.MarkDecision();
            }

            if (Timer > rolling)
            {
                SwitchBeat(ctx, BeatIndex + 1);
            }
        }

        /// <summary>收招：25 帧内从 150 拉到 450 并顺时针扫开 PiOver4。</summary>
        private void RecoverBeat(NightmarePlanteraContext ctx)
        {
            float factor = Timer / NightmarePlanteraDirector.P2TpRecoverFrames;
            Vector2 anchor = ctx.TargetCenter
                + ((ctx.ShootCount + (factor * MathHelper.PiOver4)).ToRotationVector2()
                    * Helper.Lerp(NightmarePlanteraDirector.P2TpRecoverNear, NightmarePlanteraDirector.P2TpRecoverFar, factor));

            ctx.DeclareApproach(anchor, NightmarePlanteraDirector.P2TpRecoverTurn, NightmarePlanteraDirector.P2TpVolleyMaxSpeed,
                NightmarePlanteraDirector.P2TpVolleyBlend, NightmarePlanteraDirector.P2TpVolleySpeedRange);
            ctx.Boss.DoRotation(0.3f);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => BeatIndex == 7 && Timer > NightmarePlanteraDirector.P2TpRecoverFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
