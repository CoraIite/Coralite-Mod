using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 噩梦值满的处决：一边在玩家四周立一圈预警尖刺，一边淡出瞬移到玩家身边，然后咬下去。<br/>
    /// 节拍结构：淡出 45 帧（同时每 8 帧落一根尖刺，预警 180 帧）→ 扑咬 55 帧 → 后摇 30 帧 → 回当前阶段选招口。<br/>
    /// 公平阀：尖刺的 180 帧预警 + 咬击自带的 60 帧张嘴时间就是全部逃生窗口；咬击伤害写死 999999，本来就是处决而不是招式。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.suddenDeath, typeof(NightmarePlanteraContext))]
    internal sealed class NPSuddenDeathState : NightmarePlanteraStateBase
    {
        private enum Beat
        {
            /// <summary>淡出 + 立尖刺环</summary>
            Fade,
            /// <summary>扑咬</summary>
            Lunge,
            /// <summary>后摇</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.suddenDeath;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.UpdateFrameNormally();

            switch (CurrentBeat)
            {
                case Beat.Fade:
                    SpikeRing(ctx);
                    if (FadeTickP2(ctx, () => boss.PickTargetTeleportOffset(NightmarePlanteraDirector.SuddenTeleportMin, NightmarePlanteraDirector.SuddenTeleportMax),
                        onTeleport: () => BiteOnce(ctx)))
                    {
                        SwitchBeat(ctx, (int)Beat.Lunge);
                    }

                    break;

                case Beat.Lunge:
                    ctx.MeleeDamage = true;
                    LungeToTarget(ctx, 0.1f, NightmarePlanteraDirector.SuddenLungeDistance,
                        NightmarePlanteraDirector.SuddenLungeAccel, NightmarePlanteraDirector.SuddenLungeMaxSpeed,
                        NightmarePlanteraDirector.SuddenLungeDamp, velocityTurn: 0.3f);

                    if (Timer > NightmarePlanteraDirector.SuddenLungeFrames)
                    {
                        SwitchBeat(ctx, (int)Beat.Recover);
                    }

                    break;

                default:
                    ctx.DeclareDamp(NightmarePlanteraDirector.SuddenRecoverDamp);
                    if (Timer > NightmarePlanteraDirector.SuddenRecoverRotFrame)
                    {
                        boss.DoRotation(0.04f);
                    }

                    break;
            }
        }

        /// <summary>
        /// 每 8 帧在玩家外围半径 1000 处落一根朝内的尖刺，预警 180 帧且不造成伤害——它只是圈禁走位。<br/>
        /// 随机抽取放在双端执行，只有生成被门控：抽取次数两端必须一致，否则后面 <c>FadeTick</c> 的瞬移落点会分叉（C3）。
        /// </summary>
        private void SpikeRing(NightmarePlanteraContext ctx)
        {
            if (Timer % NightmarePlanteraDirector.SuddenSpikeInterval != 0)
            {
                return;
            }

            NPC npc = ctx.Npc;
            Vector2 center = ctx.TargetCenter
                + ctx.Boss.NextAttackVector2CircularEdge(NightmarePlanteraDirector.SuddenSpikeRadius, NightmarePlanteraDirector.SuddenSpikeRadius);

            Projectile spike = npc.NewProjectileDirectInAI_Server<NightmareSpike>(center,
                (ctx.TargetCenter - center).SafeNormalize(Vector2.Zero), 1, 0, npc.target,
                NightmarePlanteraDirector.SuddenSpikeAi1, -2, NightmarePlanteraDirector.SuddenSpikeAi3);

            if (spike?.ModProjectile is NightmareSpike modSpike)
            {
                modSpike.ShootTime = NightmarePlanteraDirector.SuddenSpikeShootTime;
                modSpike.canHitPlayer = false;
            }

            ctx.MarkDecision();
        }

        /// <summary>瞬移落地的那一口。伤害写死处决值，命中判定由咬击弹幕自己做。</summary>
        private static void BiteOnce(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.NewProjectileInAI_Server<NightmareBite>(npc.Center, Vector2.Zero,
                NightmarePlanteraDirector.SuddenBiteDamage, 4, npc.target,
                ai0: 3, ai1: NightmarePlanteraDirector.SuddenBiteAi1, ai2: -2);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.SuddenRecoverFrames
                ? Create(ctx.Boss.ResumeStateId())
                : null;
    }
}
