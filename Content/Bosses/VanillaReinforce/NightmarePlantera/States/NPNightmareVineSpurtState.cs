using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 藤蔓喷发：瞬移到玩家斜前方挂着摆动，每 25 帧从身侧左右交替甩出一根荆棘刺，
    /// 攒够时长后一次甩出四根扇形刺封走位，然后拉远收招。<br/>
    /// 节拍结构：淡出 30 帧 → 甩单刺（4~6 × 25 + 40 帧，间隔 25 帧、左右交替）→ 四连扇形 → 拉远绕行 120 帧。<br/>
    /// 公平阀：单刺预警 65 帧、四连扇形预警 110 帧（越危险的那一手给的时间越长）；
    /// 扇形跨度 2 rad 分 4 根，刺与刺之间留得下一个人的宽度，是"要走位"而不是"必中"。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.vineSpurt, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareVineSpurtState : NPNightmareStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移 + 第一根刺</summary>
            Fade,
            /// <summary>摆动甩单刺</summary>
            Spurt,
            /// <summary>四连扇形之后拉远收招</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.vineSpurt;

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>本招甩单刺的总时长（槽 A）。旧代码借用 <c>EXai1</c> 存它，这里改走热字段自用槽。</summary>
        private float spurtFrames;

        protected override void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Spurt:
                    SpurtBeat(ctx);
                    break;
                default:
                    Orbit(ctx, NightmarePlanteraDirector.VineRecoverRadius);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            if (!FadeTickP3(ctx, () => TeleportPos(ctx), postTeleport: () => OnLanded(ctx)))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Spurt);
        }

        /// <summary>落点：玩家朝向再偏 -0.75 或 +0.57 rad，距离 350~450——斜前方而不是正面。</summary>
        private static Vector2 TeleportPos(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            int direction = ctx.Target?.direction ?? 1;

            return ctx.TargetCenter + (new Vector2(direction, 0)
                .RotatedBy(boss.NextAttackFromList(NightmarePlanteraDirector.VineTeleportAngleA, NightmarePlanteraDirector.VineTeleportAngleB))
                * boss.NextAttackFloat(NightmarePlanteraDirector.VineTeleportMin, NightmarePlanteraDirector.VineTeleportMax));
        }

        /// <summary>落地：锚定摆动基准角、抽本招时长，并甩出第一根刺。</summary>
        private void OnLanded(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
            ctx.ShootCount = (npc.Center - ctx.TargetCenter).ToRotation();
            spurtFrames = (boss.AttackRandom.Next(NightmarePlanteraDirector.VineDurationMin, NightmarePlanteraDirector.VineDurationMax + 1)
                * NightmarePlanteraDirector.VineDurationStep) + NightmarePlanteraDirector.VineDurationBase;

            ThrowSpike(ctx, -1, NightmarePlanteraDirector.VineSpikeFirstTime);
        }

        /// <summary>摆动甩单刺：左右交替，到点收束成四连扇形。</summary>
        private void SpurtBeat(NightmarePlanteraContext ctx)
        {
            Orbit(ctx, NightmarePlanteraDirector.VineOrbitRadius);

            if (Timer % NightmarePlanteraDirector.VineSpikeInterval == 0)
            {
                int side = Timer % (NightmarePlanteraDirector.VineSpikeInterval * 2) == 0 ? -1 : 1;
                ThrowSpike(ctx, side, NightmarePlanteraDirector.VineSpikeFirstTime);
            }

            if (Timer <= spurtFrames)
            {
                return;
            }

            Finale(ctx);
            SwitchBeat(ctx, (int)Beat.Recover);
        }

        /// <summary>一次甩出四根扇形刺，跨度 2 rad，起手偏 -1 rad。</summary>
        private static void Finale(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
            ctx.ShootCount = (npc.Center - ctx.TargetCenter).ToRotation();

            int damage = NightmarePlanteraDirector.P3BiteDamage();
            float rot = (npc.Center - ctx.TargetCenter).ToRotation() + NightmarePlanteraDirector.VineFinaleBaseOffset;

            for (int i = 0; i < NightmarePlanteraDirector.VineFinaleCount; i++)
            {
                npc.NewProjectileInAI_Server<VineSpike>(npc.Center, rot.ToRotationVector2(), damage, 8, npc.target,
                    boss.ZenithProjSeed(), rot, NightmarePlanteraDirector.VineFinaleTime);
                rot += NightmarePlanteraDirector.VineFinaleSpread / NightmarePlanteraDirector.VineFinaleCount;
            }

            ctx.MarkDecision();
        }

        private static void ThrowSpike(NightmarePlanteraContext ctx, int side, int shootTime)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            float rot = (npc.Center - ctx.TargetCenter).ToRotation() + (side * NightmarePlanteraDirector.VineSpikeAngle);
            npc.NewProjectileInAI_Server<VineSpike>(npc.Center, rot.ToRotationVector2(),
                NightmarePlanteraDirector.P3BiteDamage(), 8, npc.target, boss.ZenithProjSeed(), rot, shootTime);
            ctx.MarkDecision();
        }

        /// <summary>挂在玩家侧面小幅摆动的通用绕行，甩刺段与收招段只差半径。</summary>
        private void Orbit(NightmarePlanteraContext ctx, float radius)
        {
            ctx.Boss.DoRotation(0.3f);

            float angle = ctx.ShootCount
                + (MathHelper.PiOver4 / 4 * MathF.Sin(Timer * NightmarePlanteraDirector.VineOrbitWave));
            ctx.DeclareApproach(ctx.TargetCenter + (angle.ToRotationVector2() * radius),
                NightmarePlanteraDirector.VineOrbitTurn, NightmarePlanteraDirector.VineOrbitMaxSpeed,
                NightmarePlanteraDirector.VineOrbitBlend, NightmarePlanteraDirector.VineOrbitSpeedRange);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.VineRecoverFrames
                ? NPNightmareP3State.Commit(ctx)
                : null;

        public override void WriteHot(NightmarePlanteraContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = spurtFrames;
        }

        public override void ReadHot(NightmarePlanteraContext ctx)
        {
            base.ReadHot(ctx);
            spurtFrames = ctx.Hot[CoraliteBossHotSlots.A];
        }
    }
}
