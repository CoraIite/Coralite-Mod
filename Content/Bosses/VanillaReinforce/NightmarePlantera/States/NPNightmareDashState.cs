using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 噩梦冲刺：瞬移到玩家身边张嘴，原地旋转蓄力并沿着一条越拉越大的弧线绕行，最后一帧爆发直线冲过去；连着来两轮。<br/>
    /// 节拍结构（每轮）：淡出（第一轮 20 帧 / 第二轮 10 帧）→ 蓄力 48 帧（前 40 帧自转 + 绕行半径 140→480）→
    /// 锁向 6 帧 → 一帧爆冲 48 px/f → 刹车。第二轮之后再多两段收招（17 + 14 帧）。<br/>
    /// 公平阀：蓄力的自转与拉开的绕行弧线本身就是预告；锁向发生在爆冲前 6 帧，之后射线不再追瞄——预告指哪打哪。
    /// 冲刺本体不带接触伤害（爆冲的那一帧就关掉了），威胁全在咬击弹幕上，所以贴脸不会被无限碾。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.p3_nightmareDash, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareDashState : NPNightmareStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>自转蓄力 + 绕行拉开跑道</summary>
            Windup,
            /// <summary>锁向后爆冲</summary>
            Launch,
            /// <summary>第一轮的短刹车</summary>
            Brake,
            /// <summary>第二轮收招第一段</summary>
            Tail1,
            /// <summary>第二轮收招第二段</summary>
            Tail2,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.p3_nightmareDash;

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>第几轮冲刺（0 / 1，槽 A）。两轮的淡出时长与收招长度不同。</summary>
        private int round;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);
            round = 0;
        }

        protected override void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Windup:
                    WindupBeat(ctx);
                    break;
                case Beat.Launch:
                    LaunchBeat(ctx);
                    break;
                case Beat.Brake:
                    TrailDust(ctx);
                    if (Timer > NightmarePlanteraDirector.DashRecover1Frames)
                    {
                        ctx.Npc.velocity *= 0;
                        round = 1;
                        SwitchBeat(ctx, (int)Beat.Fade);
                    }

                    break;
                case Beat.Tail1:
                    TailBeat(ctx, NightmarePlanteraDirector.DashRecover2DampFrame);
                    if (Timer > NightmarePlanteraDirector.DashRecover2Frames)
                    {
                        SwitchBeat(ctx, (int)Beat.Tail2);
                    }

                    break;
                default:
                    TailBeat(ctx, NightmarePlanteraDirector.DashRecover3DampFrame);
                    break;
            }
        }

        /// <summary>
        /// 第一轮瞬移到玩家运动方向的前方，第二轮瞬移到背后——同一个偏移取反，抽取次数一致（C3）。
        /// </summary>
        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            int fadeTime = round == 0 ? NightmarePlanteraDirector.DashFadeFrames : NightmarePlanteraDirector.DashFadeFramesShort;

            if (!FadeTickP2(ctx, () =>
                {
                    Vector2 offset = boss.PickTeleportOffset(NightmarePlanteraDirector.DashTeleportMin, NightmarePlanteraDirector.DashTeleportMax);
                    return ctx.TargetCenter + (round == 0 ? offset : -offset);
                },
                fadeTime,
                onTeleport: () => npc.NewProjectileInAI_Server<NightmareBite>(npc.Center, Vector2.Zero,
                    NightmarePlanteraDirector.P3BiteDamage(), 4, ai0: 0, ai1: NightmarePlanteraDirector.P3BiteAi1, ai2: boss.ZenithProjSeedNeg2()),
                postTeleport: () =>
                {
                    if (!Main.dedServ)
                    {
                        Helper.PlayPitched(CoraliteSoundID.DeathCalling_Item103, npc.Center, pitchAdjust: -0.4f);
                    }

                    npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
                    // 锚定这一轮绕行的起始角：从玩家看过来本体在哪一侧，跑道就从那一侧拉开。
                    ctx.ShootCount = (npc.Center - ctx.TargetCenter).ToRotation();
                }))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Windup);
        }

        /// <summary>蓄力：前 40 帧自转并把绕行半径从 140 拉到 480，之后转头锁向，末尾 8 帧只是减速。</summary>
        private void WindupBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            float factor = Timer / (float)NightmarePlanteraDirector.DashWindupFrames;

            // 接触伤害从瞬移落地一直开到爆冲那一帧（旧值：onTeleport 开、son2 launch 关）。
            ctx.MeleeDamage = true;

            if (Timer < NightmarePlanteraDirector.DashSpinFrames)
            {
                npc.rotation += MathHelper.TwoPi / NightmarePlanteraDirector.DashSpinFrames;

                float radius = Helper.Lerp(NightmarePlanteraDirector.DashRadiusMin, NightmarePlanteraDirector.DashRadiusMax, factor);
                ctx.DeclareApproach(ctx.TargetCenter + (ctx.ShootCount.ToRotationVector2() * radius),
                    NightmarePlanteraDirector.DashWindupTurn, NightmarePlanteraDirector.DashWindupMaxSpeed,
                    NightmarePlanteraDirector.DashWindupBlend, NightmarePlanteraDirector.DashWindupSpeedRange);
            }
            else
            {
                ctx.DeclareRotation(npc.rotation.AngleTowards((DashAim(ctx) - npc.Center).ToRotation(), 0.3f));
                ctx.DeclareDamp(NightmarePlanteraDirector.DashWindupDamp);
            }

            if (Timer > NightmarePlanteraDirector.DashWindupFrames)
            {
                SwitchBeat(ctx, (int)Beat.Launch);
            }
        }

        /// <summary>锁向 6 帧后一帧爆冲。冲刺本体不带接触伤害。</summary>
        private void LaunchBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareDamp(NightmarePlanteraDirector.DashHoldDamp);
            ctx.MeleeDamage = true;

            if (Timer <= NightmarePlanteraDirector.DashHoldFrames)
            {
                return;
            }

            npc.velocity = (DashAim(ctx) - npc.Center).SafeNormalize(Vector2.One) * NightmarePlanteraDirector.DashSpeed;
            npc.rotation = npc.velocity.ToRotation();
            ctx.DeclareRotation(npc.rotation);
            ctx.MoveMode = NPMoveMode.Keep;
            ctx.MeleeDamage = false;
            SwitchBeat(ctx, round == 0 ? (int)Beat.Brake : (int)Beat.Tail1);
        }

        /// <summary>冲刺瞄点：玩家朝向前置 80 px + 14 帧速度前置；场上有美梦光时优先撞它。</summary>
        private static Vector2 DashAim(NightmarePlanteraContext ctx)
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
                + new Vector2(target.direction * NightmarePlanteraDirector.DashAimLead, 0)
                + (target.velocity * NightmarePlanteraDirector.DashAimVelocityLead);
        }

        private void TailBeat(NightmarePlanteraContext ctx, int dampFrame)
        {
            if (Timer > dampFrame)
            {
                ctx.DeclareDamp(NightmarePlanteraDirector.DashTailDamp);
                ctx.Boss.DoRotation(0.04f);
            }

            TrailDust(ctx);
        }

        /// <summary>
        /// 冲刺尾迹的花瓣。<b>抽取不能门控</b>：它用的是同步种子派生的 <c>AttackRandom</c>，
        /// 服务端少抽几次后面第二轮的瞬移落点就会与客户端分叉（C3）。只有真正的 Dust 创建是本地的。
        /// </summary>
        private static void TrailDust(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            NightmarePlantera boss = ctx.Boss;
            Vector2 dir = -npc.velocity.SafeNormalize(Vector2.Zero);

            for (int i = 0; i < NightmarePlanteraDirector.DashTrailDustCount; i++)
            {
                float spread = boss.NextAttackFloat(-0.3f, 0.3f);
                float speed = boss.NextAttackFloat(0.5f, 8);
                if (Main.dedServ)
                {
                    continue;
                }

                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustType<NightmarePetal>(),
                    newColor: NightmarePlantera.nightmareRed);
                dust.velocity = dir.RotatedBy(spread) * speed;
                dust.noGravity = true;
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Tail2 && Timer > NightmarePlanteraDirector.DashRecover3Frames
                ? NPNightmareP3State.Commit(ctx)
                : null;

        public override void WriteHot(NightmarePlanteraContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = round;
        }

        public override void ReadHot(NightmarePlanteraContext ctx)
        {
            base.ReadHot(ctx);
            round = (int)ctx.Hot[CoraliteBossHotSlots.A];
        }
    }
}
