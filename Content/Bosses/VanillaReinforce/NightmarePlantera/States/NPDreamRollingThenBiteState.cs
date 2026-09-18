using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 转圈咬：瞬移到玩家侧方原地自旋 360 帧，边转边越转越快地向四周喷噩梦光；转完淡出，
    /// 从玩家斜上方 600 px 以 48 的速度直落，身后拖一条会跟着走的裂缝，落定后引爆整条裂缝。<br/>
    /// 节拍：淡出 30 帧 → 自旋 360 帧 → 冲刺贯穿 40 帧 → 后摇 56 帧。<br/>
    /// 原 <c>RollingThenBite</c>（Phase.P2_Dream.cs:562-722）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.rollingThenBite, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamRollingThenBiteState : NPDreamSlitStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.rollingThenBite;

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

        /// <summary>淡出瞬移到玩家背面 300~400，落地时朝向随机——自旋马上开始，朝向本来就没有意义。</summary>
        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            bool done = FadeTickP2(ctx,
                () => ctx.TargetCenter + (new Vector2(-(ctx.Target?.direction ?? 1), 0) * boss.NextAttackFloat(300, 400)),
                fadeTime: NightmarePlanteraDirector.P2DashFadeFrames,
                onTeleport: () => npc.rotation = boss.NextAttackFloat(MathHelper.TwoPi));

            boss.NormallySetTentacle();

            if (done)
            {
                SwitchBeat(ctx, (int)SlitBeat.Spin);
            }
        }

        /// <summary>原地自旋喷光：转速在前 1/3 段拉满，弹幕间隔同步从 30 缩到 10。</summary>
        private void SpinBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;
            int rolling = NightmarePlanteraDirector.P2RollingFrames;

            SpinFog(ctx);

            float rotFactor = Math.Clamp(Timer / (rolling / 3f), 0, 1);
            npc.rotation += rotFactor * NightmarePlanteraDirector.P2RollingSpinStep;

            int delay = NightmarePlanteraDirector.P2RollingDelayBase - (int)(rotFactor * NightmarePlanteraDirector.P2RollingDelayCut);
            if (Timer % delay == 0)
            {
                int damage = NightmarePlanteraDirector.P2SparkleDamage();
                for (int i = 0; i < NightmarePlanteraDirector.P2RollingSparkleCount; i++)
                {
                    Vector2 dir = (npc.rotation + (i * MathHelper.PiOver2)).ToRotationVector2();
                    npc.NewProjectileInAI_Server<NightmareSparkle_Normal>(npc.Center, dir, damage, 0);
                }

                ctx.MarkDecision();
                Helper.PlayPitched(CoraliteSoundID.CrystalSerpent_Item109, npc.Center, pitchAdjust: -0.5f);
            }

            Vector2 center = npc.Center;
            SpinTentacles(ctx, center, npc.Center, Timer / (float)rolling);
            SpinFadeOut(ctx, npc.Center);

            if (Timer <= rolling)
            {
                return;
            }

            Land(ctx);
            SwitchBeat(ctx, (int)SlitBeat.Dash);
        }

        /// <summary>落点：玩家左右任一侧 400~600、上方 600，垂直下落。</summary>
        private static void Land(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.velocity = new Vector2(0, NightmarePlanteraDirector.P2RollingBiteSpeed);
            boss.alpha = 1;
            boss.canDrawWarp = false;

            int direction = Math.Sign(ctx.TargetCenter.X - npc.Center.X);
            npc.Center = ctx.TargetCenter + new Vector2(
                direction * boss.AttackRandom.Next(NightmarePlanteraDirector.P2RollingBiteSideMin, NightmarePlanteraDirector.P2RollingBiteSideMax),
                NightmarePlanteraDirector.P2RollingBiteHeight);
            npc.rotation = MathHelper.PiOver2;

            SnapTentacles(ctx);
            SpawnSlit(ctx);
        }
    }
}
