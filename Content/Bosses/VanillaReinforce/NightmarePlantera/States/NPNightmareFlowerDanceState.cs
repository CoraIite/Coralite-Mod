using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 群花乱舞：瞬移到玩家身边，三条触手甩成一朵旋转的花，一边自转一边吐出一圈又一圈黑暗飞叶；
    /// 叶阵铺满之后横穿战场拉出一道裂隙，最后绕回来引爆它。<br/>
    /// 节拍结构：淡出 60 帧 → 蓄力 120 帧（每 6 帧一圈 7 发叶，前 60 帧扇面右旋、后 60 帧左旋）→
    /// 横穿冲刺 20 帧 + 刹车 15 帧 → 绕圈 92 帧（第 35 帧引爆裂隙）。<br/>
    /// 公平阀：叶阵的旋向在蓄力中点翻转，所以缺口是连续移动的而不是死角；冲刺沿玩家侧后 450 起手、
    /// 垂直偏 700 落点，路径在起手时就定死，不追瞄。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.flowerDance, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareFlowerDanceState : NPNightmareStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>自转蓄力 + 叶阵</summary>
            Charge,
            /// <summary>横穿冲刺并留下裂隙</summary>
            Dash,
            /// <summary>刹车</summary>
            Brake,
            /// <summary>绕圈并引爆裂隙</summary>
            Detonate,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.flowerDance;

        /// <summary>蓄力段自己摆触手（甩成花），外壳不要插手。</summary>
        protected override bool AutoTentacle => false;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Charge:
                    ChargeBeat(ctx);
                    break;
                case Beat.Dash:
                    DashBeat(ctx);
                    break;
                case Beat.Brake:
                    BrakeBeat(ctx);
                    break;
                default:
                    DetonateBeat(ctx);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.NormallySetTentacle();

            if (!FadeTickP3(ctx,
                () => boss.PickTargetTeleportOffset(NightmarePlanteraDirector.P3BiteTeleportMin, NightmarePlanteraDirector.P3BiteTeleportMax),
                NightmarePlanteraDirector.FlowerFadeFrames,
                onTeleport: () => boss.canDrawWarp = true,
                postTeleport: () => OnLanded(ctx)))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Charge);
        }

        private static void OnLanded(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();

            // 叶阵的整体基准角每次随机，所以玩家不能背板；抽取两端同跑（C3）。
            ctx.Boss.EXai1 = ctx.Boss.NextAttackFloat(MathHelper.TwoPi);

            if (Main.dedServ)
            {
                return;
            }

            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, npc.Center, pitch: -0.5f);
            Helper.PlayPitched(CoraliteSoundID.EmpressOfLight_Dash_Item160, npc.Center, pitch: -0.75f, volumeAdjust: -0.2f);
            Shake(ctx, Vector2.UnitY, 15, 8, 20);
        }

        /// <summary>蓄力：触手甩成旋转的花，本体自转，每 6 帧吐一圈 7 发叶；扇面在中点翻转旋向。</summary>
        private void ChargeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            SpinTentacles(ctx, Timer / (float)NightmarePlanteraDirector.FlowerChargeFrames,
                NightmarePlanteraDirector.FlowerTentacleRadius, NightmarePlanteraDirector.FlowerTentacleSpinTurns);

            if (Timer % NightmarePlanteraDirector.FlowerCameraInterval == 0)
            {
                Shake(ctx, Helper.NextVec2Dir(), NightmarePlanteraDirector.FlowerShakeLevel, 8, 10);
            }

            boss.warpScale += NightmarePlanteraDirector.FlowerWarpStep;
            if (boss.warpScale > NightmarePlanteraDirector.ExchangeWarpMax)
            {
                boss.canDrawWarp = false;
                boss.warpScale = 0;
            }

            npc.rotation += NightmarePlanteraDirector.FlowerRotStep;
            ctx.DeclareDamp(NightmarePlanteraDirector.FlowerDamp);

            ChargeDust(ctx);

            if (!Main.dedServ && Timer % NightmarePlanteraDirector.FlowerSoundInterval == 0)
            {
                Helper.PlayPitched(CoraliteSoundID.NoUse_BlowgunPlus_Item65, npc.Center, pitchAdjust: -0.2f, volumeAdjust: -0.2f);
            }

            if (Timer % NightmarePlanteraDirector.FlowerLeafInterval == 0)
            {
                LeafRing(ctx);
            }

            if (Timer > NightmarePlanteraDirector.FlowerChargeFrames)
            {
                LaunchDash(ctx);
                SwitchBeat(ctx, (int)Beat.Dash);
            }
        }

        /// <summary>一圈 7 发叶。<c>ShootCount</c> 兼任扇面偏移计数，每 9 步换一次颜色并跳 3 步。</summary>
        private void LeafRing(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            int color = 0;
            bool firstHalf = Timer < NightmarePlanteraDirector.FlowerChargeHalfFrames;

            if (ctx.ShootCount % NightmarePlanteraDirector.FlowerLeafColorEvery < 1)
            {
                color = 1;
                ctx.ShootCount += firstHalf ? NightmarePlanteraDirector.FlowerLeafColorStep : -NightmarePlanteraDirector.FlowerLeafColorStep;
            }

            for (int i = 0; i < NightmarePlanteraDirector.FlowerLeafPerRing; i++)
            {
                float angle = ctx.Boss.EXai1
                    + (ctx.ShootCount * NightmarePlanteraDirector.FlowerLeafSpread)
                    + (i * MathHelper.TwoPi / NightmarePlanteraDirector.FlowerLeafPerRing);
                npc.NewProjectileInAI_Server<DarkLeaf>(npc.Center,
                    angle.ToRotationVector2() * NightmarePlanteraDirector.FlowerLeafSpeed,
                    NightmarePlanteraDirector.P3BiteDamage(), 0, ai0: color);
            }

            ctx.ShootCount += firstHalf ? 1 : -1;
            ctx.MarkDecision();
        }

        /// <summary>起跑：退到玩家侧后 450 再垂直偏 700，路径在这一帧定死。</summary>
        private static void LaunchDash(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            Vector2 toTarget = (ctx.TargetCenter - npc.Center).SafeNormalize(Vector2.Zero);
            npc.velocity = toTarget.RotatedBy(MathHelper.PiOver2) * NightmarePlanteraDirector.FlowerDashSpeed;
            boss.alpha = 1;
            boss.canDrawWarp = false;

            npc.Center = ctx.TargetCenter
                + (toTarget * NightmarePlanteraDirector.FlowerDashBack)
                + (toTarget.RotatedBy(-MathHelper.PiOver2) * NightmarePlanteraDirector.FlowerDashSide);
            npc.rotation = npc.velocity.ToRotation();
            boss.ResetTentaclesTo(npc.Center, npc.rotation);

            npc.NewProjectileInAI_Server<NightmareBite>(npc.Center, Vector2.Zero,
                NightmarePlanteraDirector.FlowerDanceBiteDamage(), 4, ai0: 0,
                ai1: NightmarePlanteraDirector.FlowerDashBiteAi1, ai2: boss.ZenithProjSeed());
            npc.NewProjectileInAI_Server<NightmareSlit>(npc.Center, Vector2.Zero,
                NightmarePlanteraDirector.FlowerSlitDamage(), 4);
            ctx.MarkDecision();
        }

        /// <summary>横穿：前 20 帧全速直线，之后刹车 15 帧并停掉裂隙的跟随。</summary>
        private void DashBeat(NightmarePlanteraContext ctx)
        {
            ctx.Boss.NormallySetTentacle();

            if (Timer < NightmarePlanteraDirector.FlowerDashFrames)
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Brake);
        }

        private void BrakeBeat(NightmarePlanteraContext ctx)
        {
            ctx.Boss.NormallySetTentacle();
            ctx.DeclareDamp(NightmarePlanteraDirector.FlowerDashDamp);
            ctx.Boss.DoRotation(0.04f);

            if (Timer < NightmarePlanteraDirector.FlowerDashTail)
            {
                return;
            }

            NightmareSlit.StopTracking();
            SwitchBeat(ctx, (int)Beat.Detonate);
        }

        /// <summary>绕回来，第 35 帧引爆裂隙。</summary>
        private void DetonateBeat(NightmarePlanteraContext ctx)
        {
            ctx.Boss.DoRotation(0.3f);
            CircleMovement(ctx, Timer, NightmarePlanteraDirector.FlowerCircleDistance, NightmarePlanteraDirector.FlowerCircleSpeed,
                NightmarePlanteraDirector.FlowerCircleAccel, NightmarePlanteraDirector.FlowerCircleRolling);

            if (Timer == NightmarePlanteraDirector.FlowerExplodeFrame)
            {
                NightmareSlit.Exposion();
            }

            ctx.Boss.NormallySetTentacle();
        }

        /// <summary>把三条触手甩成一朵转动的花。</summary>
        private static void SpinTentacles(NightmarePlanteraContext ctx, float factor, float radius, float turns)
        {
            NightmarePlantera boss = ctx.Boss;
            if (boss.rotateTentacles == null)
            {
                return;
            }

            Vector2 center = ctx.Npc.Center;
            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = boss.rotateTentacles[i];
                float targetRot = (factor * MathHelper.TwoPi * turns) + (i * MathHelper.TwoPi / 3);
                Vector2 selfPos = Vector2.Lerp(tentacle.pos, center + (radius * targetRot.ToRotationVector2()), 0.2f);
                tentacle.SetValue(selfPos, center, targetRot);
                tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20, 0.7f);
            }
        }

        /// <summary>蓄力段的星尘与浓雾。抽取两端同跑（后面冲刺落点与叶阵角度都在同一个随机流上，C3）。</summary>
        private void ChargeDust(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            Vector2 dir = Helper.NextVec2Dir();
            Dust dust = Dust.NewDustPerfect(npc.Center + (dir * boss.NextAttackFloat(64f)), DustType<NightmareDust>(),
                dir * boss.NextAttackFloat(2f, 4f), Scale: boss.NextAttackFloat(1f, 2f));
            dust.noGravity = true;

            if (Timer % NightmarePlanteraDirector.FlowerDustInterval == 0)
            {
                dir = Helper.NextVec2Dir();
                dust = Dust.NewDustPerfect(npc.Center + (dir * boss.NextAttackFloat(64f)), DustType<NightmareStar>(),
                    dir * boss.NextAttackFloat(4f, 8f), newColor: NightmarePlantera.nightmareRed, Scale: boss.NextAttackFloat(1f, 4f));
                dust.rotation = dir.ToRotation() + MathHelper.PiOver2;

                dir = Helper.NextVec2Dir();
                Dust.NewDustPerfect(npc.Center + (dir * boss.NextAttackFloat(64f)), DustID.VilePowder,
                    dir * boss.NextAttackFloat(4f, 10f), newColor: NightmarePlantera.nightmareRed, Scale: boss.NextAttackFloat(1f, 1.3f));
            }

            for (int i = 0; i < NightmarePlanteraDirector.FlowerFogPerFrame; i++)
            {
                Color color = boss.AttackRandom.Next(0, 2) switch
                {
                    0 => new Color(110, 68, 200),
                    _ => NightmarePlantera.nightmareRed
                };

                PRTLoader.NewParticle(npc.Center + Main.rand.NextVector2Circular(64, 64), Helper.NextVec2Dir(6, 16f),
                    CoraliteContent.ParticleType<BigFog>(), color, Scale: boss.NextAttackFloat(0.5f, 1.5f));
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            if (CurrentBeat != Beat.Detonate || Timer <= NightmarePlanteraDirector.FlowerEndFrames)
            {
                return null;
            }

            ctx.Npc.velocity *= 0;
            return NPNightmareP3State.Commit(ctx);
        }
    }
}
