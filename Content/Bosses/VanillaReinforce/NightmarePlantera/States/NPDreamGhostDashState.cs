using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 鬼手冲刺：瞬移到玩家前方后连做 8 段锯齿冲刺，每段 17 帧、转折角左右交替 0.45，身后留一串鬼影裂缝；
    /// 收招时把整串裂缝炸成鬼手。<br/>
    /// 节拍：淡出 45 →（冲刺 17）× 8 → 收招 110（第 30 帧引爆）。<br/>
    /// 原 <c>GhostDash</c>（Phase.P2_Dream.cs:1554-1643）。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.ghostDash, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamGhostDashState : NPDreamStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>锯齿冲刺</summary>
            Zigzag,
            /// <summary>收招引爆</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.ghostDash;

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>当前是第几段冲刺（1~8，对齐旧 <c>SonState</c>，转折方向读它的奇偶）。</summary>
        private int dashIndex = 1;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);
            dashIndex = 1;
        }

        public override void WriteHot(NightmarePlanteraContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = dashIndex;
        }

        public override void ReadHot(NightmarePlanteraContext ctx)
        {
            base.ReadHot(ctx);
            dashIndex = (int)ctx.Hot[CoraliteBossHotSlots.A];
        }

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Zigzag:
                    ZigzagBeat(ctx);
                    break;
                default:
                    RecoverBeat(ctx);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (!FadeTickP2(ctx,
                () => boss.PickTargetTeleportOffset(NightmarePlanteraDirector.P2GhostTeleportMin, NightmarePlanteraDirector.P2GhostTeleportMax),
                onTeleport: () =>
                {
                    float targetRot = (ctx.TargetCenter - npc.Center).ToRotation() + MathHelper.PiOver4;
                    npc.velocity = targetRot.ToRotationVector2() * NightmarePlanteraDirector.P2GhostDashSpeed;
                    npc.rotation = targetRot;
                },
                postTeleport: () => SpawnSlit(ctx)))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Zigzag);
        }

        /// <summary>一段冲刺：前 8 帧全速，之后刹车回正，17 帧结束后掐断当前裂缝并折向下一段。</summary>
        private void ZigzagBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;

            if (Timer > NightmarePlanteraDirector.P2GhostDashBrakeFrame)
            {
                ctx.DeclareDamp(NightmarePlanteraDirector.P2GhostDashDamp);
                ctx.DeclareRotationTowardsTarget(0.3f);
            }

            if (Timer <= NightmarePlanteraDirector.P2GhostDashFrames)
            {
                return;
            }

            dashIndex++;

            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(CoraliteSoundID.DeathCalling_Item103, npc.Center);
            }

            StopSlit(ctx);

            // 第 8 段只减速不再起跳（旧 `if (SonState < 8)`），第 9 段才收招。
            if (dashIndex > NightmarePlanteraDirector.P2GhostDashCount)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
                return;
            }

            if (dashIndex >= NightmarePlanteraDirector.P2GhostDashCount)
            {
                SwitchBeat(ctx, (int)Beat.Zigzag);
                return;
            }

            float targetRot = (ctx.TargetCenter - npc.Center).ToRotation()
                + ((dashIndex % 2 == 0 ? -1 : 1) * NightmarePlanteraDirector.P2GhostTurnAngle);
            npc.velocity = targetRot.ToRotationVector2() * NightmarePlanteraDirector.P2GhostDashSpeed;
            npc.rotation = targetRot;

            SpawnSlit(ctx);
            SwitchBeat(ctx, (int)Beat.Zigzag);
        }

        private void RecoverBeat(NightmarePlanteraContext ctx)
        {
            ctx.DeclareDamp(NightmarePlanteraDirector.P2GhostRecoverDamp);
            ctx.Boss.DoRotation(0.3f);

            if (Timer == NightmarePlanteraDirector.P2GhostExplodeFrame)
            {
                GhostSlit.Exposion();
            }

            CircleMovement(ctx, Timer, NightmarePlanteraDirector.P2GhostCircleDistance, NightmarePlanteraDirector.P2GhostCircleSpeed,
                NightmarePlanteraDirector.P2GhostCircleAccel, NightmarePlanteraDirector.P2GhostCircleRolling);
        }

        /// <summary>裂缝索引存在 <c>ShootCount</c> 里（旧代码的用法），客户端拿到的是 -1 直到同步包到达。</summary>
        private static void SpawnSlit(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.ShootCount = npc.NewProjectileInAI_Server<GhostSlit>(npc.Center, Vector2.Zero,
                NightmarePlanteraDirector.P2SparkleDamage(), 0, npc.target, ai1: ctx.Boss.ZenithProjSeed());
            ctx.MarkDecision();
        }

        /// <summary>
        /// 掐断上一段裂缝的跟随。索引为负是客户端尚未收到同步包的常态，旧代码直接下标会越界，这里挡掉。
        /// </summary>
        private static void StopSlit(NightmarePlanteraContext ctx)
        {
            int index = (int)ctx.ShootCount;
            if (index < 0 || index >= Main.maxProjectiles)
            {
                return;
            }

            Projectile projectile = Main.projectile[index];
            if (projectile.active && projectile.type == ProjectileType<GhostSlit>() && projectile.ModProjectile is GhostSlit slit)
            {
                slit.StopTracking();
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.P2GhostRecoverFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
