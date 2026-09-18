using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 尖刺与闪光：瞬移到玩家一侧，落地先在对侧铺开一排 8 个尖刺洞封掉退路，随后沿半径 850 的半圆轨道缓摆，
    /// 每 12 帧补两个带预测提前量的尖刺洞、每 28 帧吐一发噩梦光，逼玩家在越来越窄的缝里走位。<br/>
    /// 节拍：淡出 45 → 铺刺与吐光 300 → 收招 30。<br/>
    /// 公平阀：尖刺洞都有 60~120 帧的生长预警，本体全程在远处不逼身。<br/>
    /// 原 <c>SpikesAndSparkles</c>（Phase.P2_Dream.cs:1305-1394）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.spikesAndSparkles, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamSpikesAndSparklesState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移 + 铺开首排尖刺</summary>
            Fade,
            /// <summary>绕行补刺</summary>
            Body,
            /// <summary>收招</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.spikesAndSparkles;

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>贴着玩家哪一侧摆（0 = 右、1 = 左）。掷骰在瞬移那一刻，必须过线（C3）。</summary>
        private int side;

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

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Body:
                    BodyBeat(ctx);
                    break;
                default:
                    ctx.Boss.DoRotation(0.3f);
                    CircleMovement(ctx, Timer, NightmarePlanteraDirector.P2BatsOrbitDistance, NightmarePlanteraDirector.P2BatsOrbitSpeed,
                        NightmarePlanteraDirector.P2BatsOrbitAccel, NightmarePlanteraDirector.P2BatsOrbitRolling,
                        NightmarePlanteraDirector.P2BatsOrbitAngle, ctx.ShootCount);
                    break;
            }
        }

        /// <summary>
        /// 落点在刚掷出的那一侧 700~800（旧式 <c>((SonState-1)*Pi).ToRotationVector2()</c>，<c>SonState</c> 已在
        /// <c>OnTeleport</c> 里掷成 1 或 2），落地立刻在对侧铺刺。
        /// </summary>
        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx,
                () => ctx.TargetCenter + ((side * MathHelper.Pi).ToRotationVector2()
                    * boss.NextAttackFloat(NightmarePlanteraDirector.P2SpikesTeleportMin, NightmarePlanteraDirector.P2SpikesTeleportMax)),
                onTeleport: () =>
                {
                    side = boss.AttackRandom.Next(NightmarePlanteraDirector.P2HookSideMin, NightmarePlanteraDirector.P2HookSideMaxExclusive) - 1;
                    npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
                    // 旧代码把 3~4 写进 ShootCount 却没有再读过；抽取本身要保留，否则后续随机序列会整体错位。
                    ctx.ShootCount = boss.AttackRandom.Next(NightmarePlanteraDirector.P2SpikesRollMin, NightmarePlanteraDirector.P2SpikesRollMaxExclusive);
                    OpeningHoles(ctx);
                }))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Body);
        }

        /// <summary>落地首排：8 个尖刺洞在本体对侧扇形铺开，全部朝玩家方向生长。</summary>
        private void OpeningHoles(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            float rot = (side * MathHelper.Pi) + MathHelper.Pi;
            int damage = NightmarePlanteraDirector.P2SparkleDamage();
            int half = NightmarePlanteraDirector.P2SpikesOpeningHoles / 2;

            for (int i = -half; i < half; i++)
            {
                Vector2 dir = (rot + (i * NightmarePlanteraDirector.P2SpikesOpeningSpread)).ToRotationVector2();
                Vector2 pos = ctx.TargetCenter
                    + (dir * boss.NextAttackFloat(NightmarePlanteraDirector.P2SpikesOpeningDistMin, NightmarePlanteraDirector.P2SpikesOpeningDistMax))
                    + ((ctx.Target?.velocity ?? Vector2.Zero) * NightmarePlanteraDirector.P2SpikesOpeningLead);

                npc.NewProjectileInAI_Server<ConfusionHole>(pos, -dir, damage, 0, npc.target,
                    NightmarePlanteraDirector.P2SpikesOpeningTime, boss.ZenithProjSeed(),
                    boss.AttackRandom.Next(NightmarePlanteraDirector.P2SpikesOpeningLenMin, NightmarePlanteraDirector.P2SpikesOpeningLenMax));
            }

            ctx.MarkDecision();
        }

        private void BodyBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;

            ctx.DeclareRotationTowardsTarget(0.3f);

            float angle = (side * MathHelper.Pi)
                + (MathHelper.PiOver4 / 2 * MathF.Sin(Timer * NightmarePlanteraDirector.P2SpikesOrbitWave));
            ctx.DeclareApproach(ctx.TargetCenter + (angle.ToRotationVector2() * NightmarePlanteraDirector.P2SpikesOrbitRadius),
                NightmarePlanteraDirector.P2SpikesOrbitTurn, NightmarePlanteraDirector.P2SpikesOrbitMaxSpeed,
                NightmarePlanteraDirector.P2SpikesOrbitBlend, NightmarePlanteraDirector.P2SpikesOrbitSpeedRange);

            int damage = NightmarePlanteraDirector.P2SparkleDamage();

            if (Timer % NightmarePlanteraDirector.P2SpikesHoleInterval == 0)
            {
                // 提前量按玩家速度的平方缩放：站着不动不会被预判，跑得越快洞铺得越远。
                Vector2 velocity = ctx.Target?.velocity ?? Vector2.Zero;
                Vector2 future = velocity * velocity.Length() * NightmarePlanteraDirector.P2SpikesHoleFutureScale;
                Vector2 aim = ctx.TargetCenter + future;

                Vector2 pos = aim
                    + (npc.rotation.ToRotationVector2().RotatedBy(boss.NextAttackFloat(-NightmarePlanteraDirector.P2SpikesHoleJitter, NightmarePlanteraDirector.P2SpikesHoleJitter))
                        * boss.NextAttackFloat(NightmarePlanteraDirector.P2SpikesOpeningDistMin, NightmarePlanteraDirector.P2SpikesOpeningDistMax));
                npc.NewProjectileInAI_Server<ConfusionHole>(pos, (aim - pos).SafeNormalize(Vector2.Zero), damage, 0, npc.target,
                    boss.AttackRandom.Next(NightmarePlanteraDirector.P2SpikesHoleTimeMin, NightmarePlanteraDirector.P2SpikesHoleTimeMax),
                    boss.ZenithProjSeed(),
                    boss.AttackRandom.Next(NightmarePlanteraDirector.P2SpikesHoleLenMin, NightmarePlanteraDirector.P2SpikesHoleLenMax));

                float upDown = (Timer % NightmarePlanteraDirector.P2SpikesHole2Period == 0 ? 0 : MathHelper.Pi)
                    + boss.NextAttackFloat(MathHelper.PiOver2 - NightmarePlanteraDirector.P2SpikesHole2Spread,
                        MathHelper.PiOver2 + NightmarePlanteraDirector.P2SpikesHole2Spread);
                pos = aim + (upDown.ToRotationVector2()
                    * boss.AttackRandom.Next(NightmarePlanteraDirector.P2SpikesHole2DistMin, NightmarePlanteraDirector.P2SpikesHole2DistMax));
                npc.NewProjectileInAI_Server<ConfusionHole>(pos,
                    (aim - pos).SafeNormalize(Vector2.Zero).RotatedBy(boss.NextAttackFloat(-NightmarePlanteraDirector.P2SpikesHole2Jitter, NightmarePlanteraDirector.P2SpikesHole2Jitter)),
                    damage, 0, npc.target, NightmarePlanteraDirector.P2SpikesHole2Time, boss.ZenithProjSeed(),
                    boss.AttackRandom.Next(NightmarePlanteraDirector.P2SpikesOpeningLenMin, NightmarePlanteraDirector.P2SpikesOpeningLenMax));

                ctx.MarkDecision();
            }

            if (Timer % NightmarePlanteraDirector.P2SpikesSparkleInterval == 0)
            {
                npc.NewProjectileInAI_Server<NightmareSparkle_Normal>(npc.Center, npc.rotation.ToRotationVector2(), damage, 0);
                ctx.MarkDecision();
            }

            if (Timer <= NightmarePlanteraDirector.P2SpikesBodyFrames)
            {
                return;
            }

            ctx.ShootCount = (npc.Center - ctx.TargetCenter).ToRotation();
            SwitchBeat(ctx, (int)Beat.Recover);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.P2SpikesRecoverFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
