using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 尖刺与闪光：瞬移到玩家一侧 800 px 外挂着，一边朝玩家吐噩梦光，一边在对侧不断开尖刺洞
    /// ——把玩家夹在"正面弹幕"和"背后刺墙"之间，逼他往侧向走。<br/>
    /// 节拍结构：淡出 45 帧（落地即在对侧开 8 个预警洞）→ 挂着输出 300 帧（每 12 帧两个尖刺洞、每 28 帧一组噩梦光）→ 绕圈收招 30 帧。<br/>
    /// 公平阀：尖刺洞一律带 75~90 帧预警且落点算了玩家 3 倍速度的前置量（走直线必被预判、变向就能躲）；
    /// 噩梦光每组 1~3 发、散布只有 ±0.15 rad，是可以侧移躲开的点射而不是弹幕墙。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.p3_spikesAndSparkles, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareSpikesAndSparklesState : NPNightmareStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移 + 开场刺墙</summary>
            Fade,
            /// <summary>挂在侧面持续输出</summary>
            Hold,
            /// <summary>绕圈收招</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.p3_spikesAndSparkles;

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>挂哪一侧（0 = 右，1 = 左；槽 A）。旧代码把它塞在 <c>SonState</c> 里兼任子拍。</summary>
        private int side;

        protected override void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Hold:
                    HoldBeat(ctx);
                    break;
                default:
                    ctx.Boss.DoRotation(0.3f);
                    CircleMovement(ctx, Timer, NightmarePlanteraDirector.SpikesCircleDistance, NightmarePlanteraDirector.SpikesCircleSpeed,
                        NightmarePlanteraDirector.SpikesCircleAccel, NightmarePlanteraDirector.SpikesCircleRolling,
                        NightmarePlanteraDirector.SpikesCircleAngleFactor, ctx.ShootCount);
                    break;
            }
        }

        /// <summary>
        /// 淡出。侧别在 <c>onTeleport</c> 里掷出，而落点在其后求值，所以落点用的就是新侧别
        /// ——旧代码的顺序（Phase3_Nightemare.cs:1076-1093），照搬。
        /// </summary>
        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;

            if (!FadeTickP2(ctx,
                () => ctx.TargetCenter + ((side * MathHelper.Pi).ToRotationVector2()
                    * boss.NextAttackFloat(NightmarePlanteraDirector.SpikesTeleportMin, NightmarePlanteraDirector.SpikesTeleportMax)),
                onTeleport: () => OpenSpikeWall(ctx)))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Hold);
        }

        /// <summary>落地瞬间在对侧铺一排 8 个尖刺洞，作为整招的开场预警。</summary>
        private void OpenSpikeWall(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            side = boss.AttackRandom.Next(NightmarePlanteraDirector.SpikesRoundsMin, NightmarePlanteraDirector.SpikesRoundsMaxExclusive) - 1;
            npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
            // 旧代码在这里也抽了一个"发数"但整招没再读过它；保留抽取以免随机流错开。
            boss.AttackRandom.Next(NightmarePlanteraDirector.SpikesShootCountMin, NightmarePlanteraDirector.SpikesShootCountMaxExclusive);

            float baseRot = (side * MathHelper.Pi) + MathHelper.Pi;
            int damage = NightmarePlanteraDirector.P3BiteDamage();
            int half = NightmarePlanteraDirector.SpikesOpeningHoles / 2;

            for (int i = -half; i < half; i++)
            {
                Vector2 dir = (baseRot + (i * NightmarePlanteraDirector.SpikesOpeningSpread)).ToRotationVector2();
                Vector2 pos = ctx.TargetCenter
                    + (dir * boss.NextAttackFloat(NightmarePlanteraDirector.SpikesOpeningDistMin, NightmarePlanteraDirector.SpikesOpeningDistMax))
                    + (ctx.Target?.velocity ?? Vector2.Zero) * 15;

                npc.NewProjectileInAI_Server<ConfusionHole>(pos, -dir, damage, 0, npc.target,
                    NightmarePlanteraDirector.SpikesOpeningTime, boss.ZenithProjSeedNeg2(),
                    boss.AttackRandom.Next(NightmarePlanteraDirector.SpikesOpeningLenMin, NightmarePlanteraDirector.SpikesOpeningLenMax));
            }

            ctx.MarkDecision();
        }

        /// <summary>挂在侧面 850 px 外小幅摆动，每 12 帧两个尖刺洞、每 28 帧一组噩梦光。</summary>
        private void HoldBeat(NightmarePlanteraContext ctx)
        {
            ctx.Boss.DoRotation(0.3f);

            float angle = (side * MathHelper.Pi)
                + (MathHelper.PiOver4 / 2 * MathF.Sin(Timer * NightmarePlanteraDirector.SpikesOrbitWave));
            ctx.DeclareApproach(ctx.TargetCenter + (angle.ToRotationVector2() * NightmarePlanteraDirector.SpikesOrbitRadius),
                NightmarePlanteraDirector.SpikesOrbitTurn, NightmarePlanteraDirector.SpikesOrbitMaxSpeed,
                NightmarePlanteraDirector.SpikesOrbitBlend, NightmarePlanteraDirector.SpikesOrbitSpeedRange);

            if (Timer % NightmarePlanteraDirector.SpikesHoleInterval == 0)
            {
                ShootHoles(ctx);
            }

            if (Timer % NightmarePlanteraDirector.SpikesSparkleInterval == 0)
            {
                ShootSparkles(ctx);
            }

            if (Timer > NightmarePlanteraDirector.SpikesBodyFrames)
            {
                ctx.ShootCount = (ctx.Npc.Center - ctx.TargetCenter).ToRotation();
                SwitchBeat(ctx, (int)Beat.Recover);
            }
        }

        /// <summary>两个尖刺洞：一个从本体朝向那一侧打来，一个从玩家上下方交替打来；落点都带速度前置量。</summary>
        private void ShootHoles(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;
            int damage = NightmarePlanteraDirector.P3BiteDamage();

            Vector2 targetVel = ctx.Target?.velocity ?? Vector2.Zero;
            Vector2 future = targetVel * targetVel.Length() * NightmarePlanteraDirector.SpikesHoleFutureScale;
            Vector2 aim = ctx.TargetCenter + future;

            Vector2 facing = npc.rotation.ToRotationVector2();
            Vector2 pos = ctx.TargetCenter
                + (facing.RotatedBy(boss.NextAttackFloat(-NightmarePlanteraDirector.SpikesHoleJitter, NightmarePlanteraDirector.SpikesHoleJitter))
                    * boss.NextAttackFloat(NightmarePlanteraDirector.SpikesHoleDistMin, NightmarePlanteraDirector.SpikesHoleDistMax))
                + future;
            npc.NewProjectileInAI_Server<ConfusionHole>(pos, (aim - pos).SafeNormalize(Vector2.Zero), damage, 0, npc.target,
                boss.AttackRandom.Next(NightmarePlanteraDirector.SpikesHoleTimeMin, NightmarePlanteraDirector.SpikesHoleTimeMax),
                boss.ZenithProjSeedNeg2(),
                boss.AttackRandom.Next(NightmarePlanteraDirector.SpikesHoleLenMin, NightmarePlanteraDirector.SpikesHoleLenMax));

            float upOrDown = (Timer % NightmarePlanteraDirector.SpikesHole2Period == 0 ? 0f : MathHelper.Pi)
                + boss.NextAttackFloat(MathHelper.PiOver2 - NightmarePlanteraDirector.SpikesHole2Spread,
                    MathHelper.PiOver2 + NightmarePlanteraDirector.SpikesHole2Spread);
            pos = ctx.TargetCenter
                + (upOrDown.ToRotationVector2() * boss.AttackRandom.Next(NightmarePlanteraDirector.SpikesHole2DistMin, NightmarePlanteraDirector.SpikesHole2DistMax))
                + future;
            npc.NewProjectileInAI_Server<ConfusionHole>(pos,
                (aim - pos).SafeNormalize(Vector2.Zero).RotatedBy(boss.NextAttackFloat(-NightmarePlanteraDirector.SpikesHole2Jitter, NightmarePlanteraDirector.SpikesHole2Jitter)),
                damage, 0, npc.target, NightmarePlanteraDirector.SpikesHole2Time, boss.ZenithProjSeedNeg2(),
                boss.AttackRandom.Next(NightmarePlanteraDirector.SpikesHole2LenMin, NightmarePlanteraDirector.SpikesHole2LenMax));

            ctx.MarkDecision();
        }

        /// <summary>一组 1~3 发噩梦光，以头朝向为中心对称散开。</summary>
        private static void ShootSparkles(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;
            int damage = NightmarePlanteraDirector.P3BiteDamage();

            int howMany = boss.AttackRandom.Next(NightmarePlanteraDirector.SpikesSparkleMinCount, NightmarePlanteraDirector.SpikesSparkleMaxCountExclusive);
            float baseRot = npc.rotation - ((howMany - 1) * NightmarePlanteraDirector.SpikesSparkleHalfSpread);

            for (int i = 0; i < howMany; i++)
            {
                npc.NewProjectileInAI_Server<NightmareSparkle_Red>(npc.Center,
                    (baseRot + (i * NightmarePlanteraDirector.SpikesSparkleSpread)).ToRotationVector2(), damage, 0);
            }

            ctx.MarkDecision();
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.SpikesRecoverFrames
                ? NPNightmareP3State.Commit(ctx)
                : null;

        public override void WriteHot(NightmarePlanteraContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = side;
        }

        public override void ReadHot(NightmarePlanteraContext ctx)
        {
            base.ReadHot(ctx);
            side = (int)ctx.Hot[CoraliteBossHotSlots.A];
        }
    }
}
