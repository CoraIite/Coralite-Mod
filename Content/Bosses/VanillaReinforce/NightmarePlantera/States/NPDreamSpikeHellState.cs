using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 尖刺地狱：先在玩家周围一圈一圈地种尖刺（7 组 × 5 个，每组相位错开 1/15 圈，铺成一张旋转的网），
    /// 再瞬移到侧面连做三轮 120 帧的整圈绕行，一边绕一边每 5 帧朝玩家钉一根长刺；每轮之间淡出换到对侧。<br/>
    /// 节拍：淡出 45 → 种网 265 → 淡出 45 →（绕圈 120 + 淡出 30）× 3。<br/>
    /// 原 <c>SpikeHell</c>（Phase.P2_Dream.cs:1396-1551）。旧代码用 <c>SonState</c> 3/4/5 表达三轮，
    /// 这里保留同一个数（<see cref="son"/>）以复用它的奇偶——换向与落点角度全靠这个奇偶。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.spikeHell, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamSpikeHellState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>起手淡出</summary>
            Fade,
            /// <summary>种尖刺网</summary>
            Net,
            /// <summary>换到侧面的淡出</summary>
            Reposition,
            /// <summary>整圈绕行钉刺</summary>
            Roll,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.spikeHell;

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>旧 <c>SonState</c> 的延续：换位拍是 2，三轮绕行依次是 3 / 4 / 5，超过 5 收招。</summary>
        private int son = 2;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);
            son = 2;
        }

        public override void WriteHot(NightmarePlanteraContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = son;
        }

        public override void ReadHot(NightmarePlanteraContext ctx)
        {
            base.ReadHot(ctx);
            son = (int)ctx.Hot[CoraliteBossHotSlots.A];
        }

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Net:
                    NetBeat(ctx);
                    break;
                case Beat.Reposition:
                    RepositionBeat(ctx);
                    break;
                default:
                    RollBeat(ctx);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx,
                () => ctx.Boss.PickTeleportOffsetAround(ctx.TargetCenter, NightmarePlanteraDirector.P2BiteTeleportMin, NightmarePlanteraDirector.P2BiteTeleportMax),
                postTeleport: () => npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation()))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Net);
        }

        /// <summary>种网：绕大圈的同时每 30 帧在玩家周围半径 450 处放一组 5 个尖刺洞，组与组之间相位错开 1/15 圈。</summary>
        private void NetBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;

            // 旧分派器与招式体里各摆了一次触手，两次 0.2 的插值叠起来才是既有观感，保留。
            boss.NormallySetTentacle();
            CircleMovement(ctx, Timer, NightmarePlanteraDirector.P2BatsOrbitDistance, NightmarePlanteraDirector.P2BatsOrbitSpeed,
                NightmarePlanteraDirector.P2BatsOrbitAccel, NightmarePlanteraDirector.P2BatsOrbitRolling,
                NightmarePlanteraDirector.P2BatsOrbitAngle, ctx.ShootCount);
            boss.DoRotation(0.3f);

            int start = NightmarePlanteraDirector.P2HellRingStartFrame;
            int interval = NightmarePlanteraDirector.P2HellRingInterval;
            int lastFrame = (interval * NightmarePlanteraDirector.P2HellRingCount) + start;

            if (Timer > start - 1 && Timer < lastFrame && (Timer - start) % interval == 0)
            {
                float startAngle = (Timer - start) / interval * NightmarePlanteraDirector.P2HellRingStep;
                int damage = NightmarePlanteraDirector.P2HellRingDamage();

                for (int i = 0; i < NightmarePlanteraDirector.P2HellRingHoles; i++)
                {
                    Vector2 center = ctx.TargetCenter + (startAngle.ToRotationVector2() * NightmarePlanteraDirector.P2HellRingRadius);
                    npc.NewProjectileInAI_Server<ConfusionHole>(center, (ctx.TargetCenter - center).SafeNormalize(Vector2.One), damage, 0,
                        ai0: NightmarePlanteraDirector.P2HellRingHoleTime, ai1: boss.ZenithProjSeed(), ai2: NightmarePlanteraDirector.P2HellRingHoleLen);
                    startAngle += MathHelper.TwoPi / NightmarePlanteraDirector.P2HellRingHoles;
                }

                ctx.MarkDecision();
            }

            if (Timer > (interval * NightmarePlanteraDirector.P2HellRingCount) + NightmarePlanteraDirector.P2HellRingTail)
            {
                SwitchBeat(ctx, (int)Beat.Reposition);
            }
        }

        /// <summary>换位：淡出后落到 <c>son</c> 奇偶决定的一侧，并把绕行方向定成与上一轮相反。</summary>
        private void RepositionBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;

            // 落点角度读的是 OnTeleport 里自增之后的 son（旧 Phase2Fade 就是先 OnTeleport 再求落点）。
            if (!FadeTickP2(ctx,
                () => ctx.TargetCenter + ((son % 2 * MathHelper.Pi).ToRotationVector2()
                    * boss.NextAttackFloat(NightmarePlanteraDirector.P2HellTeleportMin, NightmarePlanteraDirector.P2HellTeleportMax)),
                onTeleport: () =>
                {
                    son++;
                    ctx.ShootCount = son % 2 == 0 ? -1 : 1;
                    npc.rotation = (son % 2 * MathHelper.Pi) + MathHelper.PiOver2;
                    npc.velocity = npc.rotation.ToRotationVector2();
                }))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Roll);
        }

        /// <summary>整圈绕行：120 帧沿半径 800 转满一圈，每 5 帧朝玩家钉一根长刺；转完淡出 30 帧换到对侧。</summary>
        private void RollBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            int rolling = NightmarePlanteraDirector.P2HellRollingFrames;

            if (Timer < rolling)
            {
                float currentRot = (son % 2 * MathHelper.Pi) + (ctx.ShootCount * Timer / rolling * MathHelper.TwoPi);
                ctx.DeclareApproach(ctx.TargetCenter + (currentRot.ToRotationVector2() * NightmarePlanteraDirector.P2HellRollingRadius),
                    NightmarePlanteraDirector.P2HellRollingTurn, NightmarePlanteraDirector.P2HellRollingMaxSpeed,
                    NightmarePlanteraDirector.P2HellRollingBlend, NightmarePlanteraDirector.P2HellRollingSpeedRange);
                ctx.DeclareRotationTowardsVelocity(0.3f);

                if (Timer % NightmarePlanteraDirector.P2HellRollingHoleInterval == 0)
                {
                    npc.NewProjectileInAI_Server<ConfusionHole>(npc.Center, (ctx.TargetCenter - npc.Center).SafeNormalize(Vector2.Zero),
                        NightmarePlanteraDirector.P2SparkleDamage(), 0, npc.target,
                        NightmarePlanteraDirector.P2HellRollingHoleTime, boss.ZenithProjSeed(), NightmarePlanteraDirector.P2HellRollingHoleLen);
                    ctx.MarkDecision();
                }

                return;
            }

            RollFadeOut(ctx, rolling);

            if (Timer <= rolling + NightmarePlanteraDirector.P2HellFadeFrames)
            {
                return;
            }

            son++;
            boss.canDrawWarp = false;
            boss.alpha = 1;

            ctx.ShootCount = son % 2 == 0 ? -1 : 1;
            float angle = son % 2 * MathHelper.Pi;
            npc.Center = ctx.TargetCenter + (angle.ToRotationVector2()
                * boss.NextAttackFloat(NightmarePlanteraDirector.P2HellTeleportMin, NightmarePlanteraDirector.P2HellTeleportMax));
            npc.rotation = angle + (ctx.ShootCount * MathHelper.PiOver2);
            npc.velocity = npc.rotation.ToRotationVector2();

            SnapTentacles(ctx);
            SwitchBeat(ctx, (int)Beat.Roll);
        }

        /// <summary>转完一圈后的 30 帧淡出：刹车 + 淡出 + 扭曲圈，第 22 帧一把星尘。</summary>
        private void RollFadeOut(NightmarePlanteraContext ctx, int rolling)
        {
            NightmarePlantera boss = ctx.Boss;
            float fadeTime = NightmarePlanteraDirector.P2HellFadeFrames;

            if (Timer == rolling && !Main.dedServ)
            {
                SoundEngine.PlaySound(CoraliteSoundID.ShieldDestroyed_NPCDeath58, ctx.Npc.Center);
            }

            boss.DoRotation(0.3f);
            ctx.DeclareDamp(NightmarePlanteraDirector.P2HellFadeDamp);

            if (boss.alpha > 0)
            {
                boss.alpha -= 1 / fadeTime;
                if (boss.alpha < 0)
                {
                    boss.alpha = 0;
                }
            }

            boss.canDrawWarp = true;
            boss.warpScale = MathF.Sin(Timer / fadeTime * MathHelper.Pi) * 2f;

            if (Timer == rolling + (int)(fadeTime * 3 / 4))
            {
                StarBurst(ctx);
            }
        }

        private static void StarBurst(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            for (int i = 0; i < NightmarePlanteraDirector.P3FadeDustCount; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                Dust dust = Dust.NewDustPerfect(ctx.Npc.Center + (dir * boss.AttackRandom.Next(0, 64)),
                    DustType<NightmareStar>(), dir * boss.NextAttackFloat(2f, 6f),
                    newColor: new Color(153, 88, 156, 230), Scale: boss.NextAttackFloat(1f, 4f));
                dust.rotation = dir.ToRotation() + MathHelper.PiOver2;
            }

            Helper.PlayPitched(CoraliteSoundID.NoUse_SuperMagicShoot_Item68, ctx.Npc.Center, pitch: -1f);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => son > NightmarePlanteraDirector.P2HellRounds + 2
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
