using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 噩梦冲刺：贴身瞬移后自转一圈甩开，绕着玩家把半径从 140 拉到 480，顿一下再以 48 的速度直线冲回来。<br/>
    /// 节拍：淡出 30 帧 → 绕行蓄力 78 帧 → 停顿 6 帧 → 冲刺后摇 20 帧。<br/>
    /// 公平阀：蓄力段自身绕圈、速度矢量指向清晰，冲刺方向在"停顿"那一刻才锁定并同步，玩家有整段绕行时间读出侧向。<br/>
    /// 原 <c>NightmareDash</c> + 四个 <c>_Son</c>（Phase.P2_Dream.cs:259-375）与 BT 序列 <c>BuildNightmareDashTree</c>。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.nightmareDash, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamNightmareDashState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>绕行蓄力</summary>
            Windup,
            /// <summary>锁定方向前的停顿</summary>
            Hold,
            /// <summary>冲刺后摇</summary>
            Tail,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.nightmareDash;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Windup:
                    ctx.MeleeDamage = true;
                    WindupBeat(ctx);
                    break;
                case Beat.Hold:
                    ctx.MeleeDamage = true;
                    HoldBeat(ctx);
                    break;
                default:
                    TailBeat(ctx);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx, () => AimPos(ctx, NightmarePlanteraDirector.P2DashTeleportMin, NightmarePlanteraDirector.P2DashTeleportMax),
                fadeTime: NightmarePlanteraDirector.P2DashFadeFrames,
                onTeleport: () => NPDreamNightmareBiteState.Bite(ctx, NightmarePlanteraDirector.P2DashBiteAi1),
                postTeleport: () =>
                {
                    Helper.PlayPitched(CoraliteSoundID.DeathCalling_Item103, npc.Center, pitchAdjust: -0.4f);

                    Vector2 pos = TargetOrSparkle(ctx);
                    npc.rotation = (pos - npc.Center).ToRotation();
                    // 记下"本体相对玩家的方位角"，蓄力段就沿这个方位把半径拉开，冲刺方向因此可预读。
                    ctx.ShootCount = (npc.Center - pos).ToRotation();
                }))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Windup);
        }

        /// <summary>绕行蓄力：前 55 帧自转一圈（预警），之后把头转向目标；半径随进度从 140 拉到 480。</summary>
        private void WindupBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            float factor = Timer / (float)NightmarePlanteraDirector.P2DashWindupFrames;

            if (Timer < NightmarePlanteraDirector.P2DashSpinFrames)
            {
                npc.rotation += MathHelper.TwoPi / NightmarePlanteraDirector.P2DashSpinFrames;
            }
            else
            {
                ctx.DeclareRotation(npc.rotation.AngleTowards((AimLead(ctx, factor) - npc.Center).ToRotation(), 0.3f));
            }

            if (Timer < NightmarePlanteraDirector.P2DashGlideFrame)
            {
                float radius = Helper.Lerp(NightmarePlanteraDirector.P2DashRadiusMin, NightmarePlanteraDirector.P2DashRadiusMax, factor);
                ctx.DeclareApproach(ctx.TargetCenter + (ctx.ShootCount.ToRotationVector2() * radius),
                    NightmarePlanteraDirector.P2DashWindupTurn, NightmarePlanteraDirector.P2DashWindupMaxSpeed,
                    NightmarePlanteraDirector.P2DashWindupBlend, NightmarePlanteraDirector.P2DashWindupSpeedRange);
            }
            else
            {
                ctx.DeclareDamp(NightmarePlanteraDirector.P2DashWindupDamp);
            }

            if (Timer > NightmarePlanteraDirector.P2DashWindupFrames)
            {
                SwitchBeat(ctx, (int)Beat.Hold);
            }
        }

        /// <summary>停顿 6 帧后锁定冲刺方向。锁向是决策点，权威端在这一帧催包（C2）。</summary>
        private void HoldBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareDamp(NightmarePlanteraDirector.P2DashHoldDamp);

            if (Timer <= NightmarePlanteraDirector.P2DashHoldFrames)
            {
                return;
            }

            npc.velocity = (AimLead(ctx, 1f) - npc.Center).SafeNormalize(Vector2.One) * NightmarePlanteraDirector.P2DashSpeed;
            npc.rotation = npc.velocity.ToRotation();
            SwitchBeat(ctx, (int)Beat.Tail);
        }

        /// <summary>冲刺后摇：12 帧后刹车并回正，全程拖花瓣。</summary>
        private void TailBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (Timer > NightmarePlanteraDirector.P2DashTailDampFrame)
            {
                ctx.DeclareDamp(NightmarePlanteraDirector.P2DashTailDamp);
                ctx.DeclareRotationTowardsTarget(0.04f);
            }

            // 抽取用同步种子，两端都要跑：不能门控，否则随机序列会分叉（C3）。
            Vector2 dir = -npc.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < NightmarePlanteraDirector.P2DashTrailDustCount; i++)
            {
                Color c = boss.AttackRandom.Next(0, 2) switch
                {
                    0 => NightmarePlantera.nightPurple,
                    _ => NightmarePlantera.lightPurple,
                };

                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustType<NightmarePetal>(), newColor: c);
                dust.velocity = dir.RotatedBy(boss.NextAttackFloat(-0.3f, 0.3f)) * boss.NextAttackFloat(0.5f, 8);
                dust.noGravity = true;
            }
        }

        /// <summary>冲刺瞄点：玩家朝向前置 80 px（按进度插值）+ 速度 14 帧；场上有美梦光时改瞄光。</summary>
        private static Vector2 AimLead(NightmarePlanteraContext ctx, float factor)
        {
            if (NightmarePlantera.FantasySparkleAlive(out NPC fs))
            {
                return fs.Center;
            }

            Player target = ctx.Target;
            if (target == null)
            {
                return ctx.TargetCenter;
            }

            return target.Center
                + new Vector2(target.direction * factor * NightmarePlanteraDirector.DashAimLead, 0)
                + (target.velocity * NightmarePlanteraDirector.DashAimVelocityLead);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Tail && Timer > NightmarePlanteraDirector.P2DashTailFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
