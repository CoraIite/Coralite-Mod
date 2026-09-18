using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 爪击：瞬移到玩家一侧后贴着半圆轨道来回摆，每 30 帧朝玩家甩一记爪痕（左右交替），甩满 4~6 记后
    /// 淡出蓄力再以 50 的速度直冲穿过去，收招时炸一圈噩梦光。<br/>
    /// 节拍：淡出 45 → 爪击 ShootCount×30 + 60 → 蓄力冲刺 80（或冲出 1400 px 就提前收）→ 收招 90。<br/>
    /// 公平阀：爪痕存活 160 帧、落点带 ±0.25 抖动但方向左右严格交替，是可读的节奏而不是乱打。<br/>
    /// 原 <c>HookSlash</c> + 四个 <c>_Son</c>（Phase.P2_Dream.cs:1203-1303）与 BT 序列 <c>BuildHookSlashTree</c>。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.hookSlash, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamHookSlashState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>半圆轨道甩爪</summary>
            Slash,
            /// <summary>淡出蓄力后直冲</summary>
            Warp,
            /// <summary>收招炸光</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.hookSlash;

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>贴着玩家哪一侧甩（0 = 右、1 = 左）。掷骰在瞬移那一刻，必须过线（C3）。</summary>
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
                case Beat.Slash:
                    SlashBeat(ctx);
                    break;
                case Beat.Warp:
                    WarpBeat(ctx);
                    break;
                default:
                    RecoverBeat(ctx);
                    break;
            }
        }

        /// <summary>
        /// 落点：旧代码是 <c>(SonState - 1) * Pi.ToRotationVector2() * rand(500,600)</c>，而 <c>SonState</c> 在
        /// <c>OnTeleport</c> 里刚被掷成 1 或 2 —— 抽到左侧时退开 500~600，抽到右侧时系数为 0，直接落在玩家身上。
        /// 这是既有落点行为，原样保留（D10）；随机数照抽，否则后续序列错位。
        /// </summary>
        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx,
                () => ctx.TargetCenter + (side * -MathHelper.Pi.ToRotationVector2()
                    * boss.NextAttackFloat(NightmarePlanteraDirector.P2HookTeleportMin, NightmarePlanteraDirector.P2HookTeleportMax)),
                onTeleport: () =>
                {
                    side = boss.AttackRandom.Next(NightmarePlanteraDirector.P2HookSideMin, NightmarePlanteraDirector.P2HookSideMaxExclusive) - 1;
                    npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
                    ctx.ShootCount = boss.AttackRandom.Next(NightmarePlanteraDirector.P2HookRoundsMin, NightmarePlanteraDirector.P2HookRoundsMaxExclusive);
                }))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Slash);
        }

        private void SlashBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            HookSlashMovement(ctx, Timer, side);

            int slashFrames = (int)ctx.ShootCount * NightmarePlanteraDirector.P2HookInterval;
            if (Timer < slashFrames && Timer % NightmarePlanteraDirector.P2HookInterval == 0)
            {
                Vector2 away = (npc.Center - ctx.TargetCenter).SafeNormalize(Vector2.One);
                float angle = away.ToRotation()
                    + (Timer % (NightmarePlanteraDirector.P2HookInterval * 2) == 0 ? MathHelper.PiOver4 : -MathHelper.PiOver4)
                    + boss.NextAttackFloat(-NightmarePlanteraDirector.P2HookJitter, NightmarePlanteraDirector.P2HookJitter);

                npc.NewProjectileInAI_Server<HookSlash>(npc.Center + (away * NightmarePlanteraDirector.P2HookOffset), Vector2.Zero,
                    NightmarePlanteraDirector.P2HookDamage(), 0, npc.target,
                    ai0: boss.ZenithProjSeed0(), ai1: angle, ai2: NightmarePlanteraDirector.P2HookAi2);
                ctx.MarkDecision();
            }

            if (Timer < slashFrames + NightmarePlanteraDirector.P2HookTail)
            {
                return;
            }

            Helper.PlayPitched(CoraliteSoundID.EmpressOfLight_Dash_Item160, npc.Center, pitch: -0.7f);
            boss.canDrawWarp = true;
            SwitchBeat(ctx, (int)Beat.Warp);
        }

        /// <summary>蓄力：20 帧淡出、20 帧淡回，第 40 帧一口气冲出去。</summary>
        private void WarpBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;
            int half = NightmarePlanteraDirector.P2HookWarpHalfFrames;

            if (Timer < half * 2)
            {
                Drift(ctx);
                ctx.DeclareRotationTowardsTarget(0.3f);

                float sign = Timer < half ? -1f : 1f;
                boss.alpha += sign * NightmarePlanteraDirector.P2HookWarpAlphaStep;
                boss.warpScale -= sign * NightmarePlanteraDirector.P2HookWarpScaleStep;

                // 前半段还在淡出，冲刺与提前收招的判定都不参与——旧代码在这里直接 return。
                if (Timer < half)
                {
                    return;
                }
            }

            if (Timer == half * 2)
            {
                boss.alpha = 1;
                boss.warpScale = 0;
                boss.canDrawWarp = false;
                npc.velocity += npc.rotation.ToRotationVector2() * NightmarePlanteraDirector.P2HookDashAccel;
                if (npc.velocity.Length() > NightmarePlanteraDirector.P2HookDashMaxSpeed)
                {
                    npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * NightmarePlanteraDirector.P2HookDashMaxSpeed;
                }

                ctx.MarkDecision();
            }

            if (Timer >= half * 2)
            {
                ctx.MeleeDamage = true;
            }

            if (Timer > NightmarePlanteraDirector.P2HookDashFrames
                || Vector2.Distance(npc.Center, ctx.TargetCenter) > NightmarePlanteraDirector.P2HookDashBreakDistance)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
            }
        }

        /// <summary>蓄力时被动往外飘，给冲刺留出助跑距离。</summary>
        private static void Drift(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            if (npc.velocity.Length() >= NightmarePlanteraDirector.P2HookDriftSpeed)
            {
                return;
            }

            npc.velocity += (npc.Center - ctx.TargetCenter).SafeNormalize(Vector2.Zero) * NightmarePlanteraDirector.P2HookDriftAccel;
        }

        private void RecoverBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.MeleeDamage = true;
            ctx.DeclareDamp(NightmarePlanteraDirector.P2HookRecoverDamp);
            ctx.DeclareRotationTowardsTarget(0.3f);

            if (Timer != NightmarePlanteraDirector.P2HookBurstFrame)
            {
                return;
            }

            int damage = NightmarePlanteraDirector.P2SparkleDamage();
            for (int i = 0; i < NightmarePlanteraDirector.P2HookBurstCount; i++)
            {
                Vector2 dir = (npc.rotation + (i * 1f / NightmarePlanteraDirector.P2HookBurstCount * MathHelper.TwoPi)).ToRotationVector2();
                npc.NewProjectileInAI_Server<NightmareSparkle_Normal>(npc.Center, dir, damage, 0);
            }

            ctx.MarkDecision();
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.P2HookRecoverFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
