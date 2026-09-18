using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 二阶段 → 三阶段转阶段：天空由紫染红，淡出瞬移到玩家头顶 250，然后原地转圈爆开 80 帧。<br/>
    /// 节拍结构：淡出 30 帧 → 爆开 80 帧（每 10 帧震屏一次并回 1.6% 上限血）→ 交给三阶段选招口。<br/>
    /// 全程无敌，没有伤害窗；回血是设计上的"重开一条血条"，不是失误。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.exchange_P2_P3, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareExchangeState : NPNightmareStateBase
    {
        private enum Beat
        {
            /// <summary>染红天空并淡出瞬移</summary>
            Fade,
            /// <summary>转圈爆开</summary>
            Burst,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.exchange_P2_P3;

        protected override int TimeoutFrames => NightmarePlanteraDirector.CinematicTimeoutFrames;

        /// <summary>保留二阶段的触手配色，让它随天空一起过渡到红色。</summary>
        protected override bool ResetPresentationOnEnter => false;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            ctx.Invulnerable = true;

            if (CurrentBeat == Beat.Fade)
            {
                FadeBeat(ctx);
                return;
            }

            BurstBeat(ctx);
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;

            if (!VaultUtils.isServer)
            {
                NightmareSky sky = (NightmareSky)SkyManager.Instance["NightmareSky"];
                sky.color = Color.Lerp(sky.color, NightmarePlantera.nightmareRed, 0.02f);
            }

            if (!FadeTickP3(ctx, () => ctx.TargetCenter + new Vector2(0, -NightmarePlanteraDirector.P3ExchangeRiseHeight),
                onTeleport: () => boss.canDrawWarp = true,
                postTeleport: () => OnLanded(ctx)))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Burst);
        }

        private static void OnLanded(NightmarePlanteraContext ctx)
        {
            ctx.Npc.rotation = (ctx.TargetCenter - ctx.Npc.Center).ToRotation();

            if (Main.dedServ)
            {
                return;
            }

            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, ctx.Npc.Center, pitch: -0.5f);
            Helper.PlayPitched(CoraliteSoundID.EmpressOfLight_Dash_Item160, ctx.Npc.Center, pitch: -0.75f, volumeAdjust: -0.2f);
            Shake(ctx, Vector2.UnitY, 15, 8, 20);
        }

        /// <summary>转圈爆开：扭曲圈外扩、星尘与浓雾、每 10 帧一次震屏与回血。</summary>
        private void BurstBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.rotation += NightmarePlanteraDirector.P3ExchangeRotStep;
            ctx.DeclareDamp(NightmarePlanteraDirector.P3ExchangeDamp);

            boss.warpScale += NightmarePlanteraDirector.P3ExchangeWarpStep;
            if (boss.warpScale > NightmarePlanteraDirector.ExchangeWarpMax)
            {
                boss.canDrawWarp = false;
                boss.warpScale = 0;
            }

            if (Timer % NightmarePlanteraDirector.P3ExchangeHealInterval == 0)
            {
                Shake(ctx, Helper.NextVec2Dir(), 20, 12, 10);
                Heal(ctx);
            }

            if (!Main.dedServ && Timer % NightmarePlanteraDirector.P3ExchangeSoundInterval == 0)
            {
                Helper.PlayPitched(CoraliteSoundID.FireBallExplosion_Item74, npc.Center, pitchAdjust: -0.2f, volumeAdjust: -0.2f);
            }

            // 下面的尘土与雾用同步种子派生的 AttackRandom，抽取次数必须两端一致，所以整块不门控
            // （服务端上 Dust / PRT 本身就是无害的空转，这也是旧代码的做法）。
            Vector2 dir = Helper.NextVec2Dir();
            Dust dust = Dust.NewDustPerfect(npc.Center + (dir * boss.NextAttackFloat(64f)), DustType<NightmareDust>(),
                dir * boss.NextAttackFloat(2f, 4f), Scale: boss.NextAttackFloat(1f, 2f));
            dust.noGravity = true;

            if (Timer % NightmarePlanteraDirector.ExchangeDustInterval == 0)
            {
                dir = Helper.NextVec2Dir();
                dust = Dust.NewDustPerfect(npc.Center + (dir * boss.NextAttackFloat(64f)), DustType<NightmareStar>(),
                    dir * boss.NextAttackFloat(4f, 8f), newColor: NightmarePlantera.nightmareRed, Scale: boss.NextAttackFloat(1f, 4f));
                dust.rotation = dir.ToRotation() + MathHelper.PiOver2;

                dir = Helper.NextVec2Dir();
                Dust.NewDustPerfect(npc.Center + (dir * boss.NextAttackFloat(64f)), DustID.VilePowder,
                    dir * boss.NextAttackFloat(4f, 10f), newColor: NightmarePlantera.nightmareRed, Scale: boss.NextAttackFloat(1f, 1.3f));
            }

            for (int i = 0; i < NightmarePlanteraDirector.P3ExchangeFogPerFrame; i++)
            {
                Color color = boss.AttackRandom.Next(0, 2) switch
                {
                    0 => new Color(110, 68, 200),
                    _ => NightmarePlantera.nightmareRed
                };

                PRTLoader.NewParticle(npc.Center + Main.rand.NextVector2Circular(64, 64), Helper.NextVec2Dir(6, 24f),
                    CoraliteContent.ParticleType<BigFog>(), color, Scale: boss.NextAttackFloat(0.5f, 1.5f));
            }
        }

        /// <summary>回血只在权威端改，客户端靠 <c>HealEffect</c> 之外的常规血量同步跟上（C1：生命改动是副作用）。</summary>
        private static void Heal(NightmarePlanteraContext ctx)
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            NPC npc = ctx.Npc;
            int heal = (int)(npc.lifeMax * NightmarePlanteraDirector.P3ExchangeHealRatio);
            npc.life = Math.Min(npc.life + heal, npc.lifeMax);
            npc.HealEffect(heal);
            ctx.MarkDecision();
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Burst && Timer > NightmarePlanteraDirector.P3ExchangeFrames
                ? NPNightmareP3State.Commit(ctx)
                : null;
    }
}
