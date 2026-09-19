using Coralite.Content.NPCs.Crystalline.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.MagikeSystem.Particles;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 一阶段护盾（远程受伤累计过门槛后的反制姿态）：<br/>
    /// Approach：朝玩家走最多 3 秒，看得见就架盾，否则退回站立 1 秒。<br/>
    /// Deploy：帧速 4 推 9 帧，第 24 帧起护盾视觉张开，36 帧后进入霸体防御。<br/>
    /// Guarding：基础 3 秒；玩家一直待在警戒范围外就每 60 帧把计时回拨 60 帧（可无限延长），
    /// 并按 210 帧的冷却插入一次三连飞弹反击（计时倒数到 0 的那 60 帧就是发射动作）；
    /// 玩家贴到近战范围内则 60 帧后主动撤盾、直接接刺击。<br/>
    /// Recover：帧速 5 收 25 帧、护盾视觉收回，回站立 1 秒。<br/>
    /// 旧 <c>P1Guard</c>（CrystallineSentinel.cs:885-1054）。防御期间血条与鼠标悬停框都会隐藏（<c>ShieldUp</c>）。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P1Guard, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP1GuardState : CrystallineSentinelStateBase
    {
        /// <summary>自用槽 A：远程反击的冷却计数（旧代码挂在 <c>Main.GameUpdateCount % 210</c> 上，全局帧号不能参与模拟）。</summary>
        private const int VolleyCooldownSlot = CoraliteBossHotSlots.A;

        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P1Guard;

        private enum Beat
        {
            /// <summary>朝玩家走（旧 Recorder = 0）</summary>
            Approach = 0,
            /// <summary>展开护盾（旧 Recorder = 1）</summary>
            Deploy = 1,
            /// <summary>防御中（旧 Recorder = 2）</summary>
            Guarding = 2,
            /// <summary>收招后摇（旧 Recorder = 3）</summary>
            Recover = 3,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>远程反击冷却，两端每帧同减。</summary>
        private float volleyCooldown;

        public override void WriteHot(CrystallineSentinelContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[VolleyCooldownSlot] = volleyCooldown;
        }

        public override void ReadHot(CrystallineSentinelContext ctx)
        {
            base.ReadHot(ctx);
            volleyCooldown = ctx.Hot[VolleyCooldownSlot];
        }

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Deploy:
                    UpdateDeploy(ctx);
                    break;
                case Beat.Guarding:
                    UpdateGuarding(ctx);
                    break;
                case Beat.Recover:
                    UpdateRecover(ctx);
                    break;
                default:
                    UpdateApproach(ctx);
                    break;
            }
        }

        private void UpdateApproach(CrystallineSentinelContext ctx)
        {
            if (AttackTimer % CrystallineSentinelDirector.TurnInterval == 0)
            {
                ctx.FaceTarget();
            }

            ctx.DeclareGroundWalk(CrystallineSentinelDirector.GuardApproachMaxSpeed, CrystallineSentinelDirector.GuardApproachAccel, true);
            WalkFrame(ctx);
        }

        private void UpdateDeploy(CrystallineSentinelContext ctx)
        {
            ctx.DeclareStandStill();

            if (AttackTimer > 0 && AttackTimer % CrystallineSentinelDirector.GuardDeployFrameRate == 0)
            {
                AdvanceFrame(ctx, CrystallineSentinelDirector.GuardDeployFrameMaxY);
            }

            if (AttackTimer > CrystallineSentinelDirector.GuardDeployShieldStart)
            {
                ctx.GuardFactor = MathHelper.Min(ctx.GuardFactor + CrystallineSentinelDirector.GuardFactorGrow, 1f);
            }
        }

        private void UpdateGuarding(CrystallineSentinelContext ctx)
        {
            ctx.DeclareStandStill();
            ctx.SuperArmor = true;
            ctx.ShieldUp = true;
            ctx.GuardFactor = 1f;
            volleyCooldown--;

            if (AttackTimer % CrystallineSentinelDirector.TurnInterval == 0)
            {
                ctx.FaceTarget();
            }

            if (AttackTimer >= 0)
            {
                SetFrame(ctx, 3, 8);
                return;
            }

            // 发射动作：帧图完全由计时决定（旧代码在 (5,3) 上每帧速推一格），客户端收养后能自洽推出同一段
            int step = (int)((AttackTimer - CrystallineSentinelDirector.GuardVolleyLead) / CrystallineSentinelDirector.GuardFrameRate) + 1;
            SetFrame(ctx, 5, 3 + step);

            for (int i = 0; i < 3; i++)
            {
                if (AtFrame(CrystallineSentinelDirector.GuardVolleyFirstCue + (i * CrystallineSentinelDirector.GuardVolleyCueStep)))
                {
                    MuzzleEffect(ctx, i);
                }
            }
        }

        private void UpdateRecover(CrystallineSentinelContext ctx)
        {
            ctx.DeclareStandStill();

            if (AttackTimer > 0 && AttackTimer % CrystallineSentinelDirector.GuardRecoverFrameRate == 0)
            {
                AdvanceFrame(ctx, CrystallineSentinelDirector.GuardRecoverFrameMaxY);
            }

            ctx.GuardFactor = MathHelper.Max(ctx.GuardFactor - CrystallineSentinelDirector.GuardFactorShrink, 0f);
        }

        /// <summary>出膛表现（音效 + 冲击尘），纯本地。旧 CrystallineSentinel.cs:988-1005</summary>
        private static void MuzzleEffect(CrystallineSentinelContext ctx, int index)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            Helper.PlayPitched(CoraliteSoundID.Crystal_Item101, npc.Center, pitch: 1f);

            Vector2[] poses = [new(2, -72), new(-2, -68), new(-10, -58)];
            Vector2 dustPos = npc.Center + new Vector2(poses[index].X * npc.direction, poses[index].Y - 16);
            Dust d = Dust.NewDustPerfect(dustPos, ModContent.DustType<VinicBigImpact>(), Vector2.Zero, Scale: 0.5f);
            d.rotation = -MathHelper.PiOver2;
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Deploy:
                    if (AttackTimer > CrystallineSentinelDirector.GuardDeployFrames)
                    {
                        NextBeat(ctx, (int)Beat.Guarding, 1f);
                        SetFrame(ctx, 3, CrystallineSentinelDirector.GuardDeployFrameMaxY);
                    }

                    return null;
                case Beat.Guarding:
                    return AuthorityGuarding(ctx);
                case Beat.Recover:
                    if (AttackTimer >= CrystallineSentinelDirector.GuardRecoverFrames)
                    {
                        return ReturnToIdle(ctx, CrystallineSentinelDirector.GuardRecoverIdleFrames);
                    }

                    return null;
                default:
                    return AuthorityApproach(ctx);
            }
        }

        private IVaultState<CrystallineSentinelContext> AuthorityApproach(CrystallineSentinelContext ctx)
        {
            if (AttackTimer <= CrystallineSentinelDirector.GuardApproachTimeout && CanWalkForward(ctx))
            {
                return null;
            }

            // 可以攻击，架盾
            if (CanHitTarget(ctx, out _))
            {
                ctx.FaceTarget();
                ctx.Npc.velocity.X = 0f;
                ctx.GuardCounter = 0;
                NextBeat(ctx, (int)Beat.Deploy);
                SetFrame(ctx, 3, 0);

                if (!Main.dedServ)
                {
                    PRTLoader.NewParticle<MagikeLozengeParticle>(ctx.Npc.Center, Vector2.Zero, Coralite.CrystallinePurple, 1f);
                }

                return null;
            }

            return ReturnToIdle(ctx, CrystallineSentinelDirector.GuardFailIdleFrames);
        }

        private IVaultState<CrystallineSentinelContext> AuthorityGuarding(CrystallineSentinelContext ctx)
        {
            // 被自己的飞弹打中：护盾当场破掉，强推到收招拍（旧 OnHitByMissile 在钩子里直接改 Recorder，这里改成钩子登记、权威端消费）
            if (ctx.ConsumeShieldBreak())
            {
                ctx.GuardCooldown = CrystallineSentinelDirector.GuardCooldownMax;
                NextBeat(ctx, (int)Beat.Recover);
                SetFrame(ctx, 4, 0);
                return null;
            }

            bool canHit = CanHitTarget(ctx, out float distance);

            if (AttackTimer >= 0)
            {
                // 玩家一直在警戒范围外：插入一次远程反击，并把护盾时间往回拨（可无限延长）
                if (canHit && distance > CrystallineSentinelDirector.AlertRange && volleyCooldown <= 0)
                {
                    volleyCooldown = CrystallineSentinelDirector.GuardVolleyInterval;
                    RebaseTimer(ctx, CrystallineSentinelDirector.GuardVolleyLead);
                    SetFrame(ctx, 5, 3);
                    return null;
                }

                if (AttackTimer % CrystallineSentinelDirector.GuardExtendInterval == 0 && distance > CrystallineSentinelDirector.AlertRange)
                {
                    RebaseTimer(ctx, MathHelper.Clamp(AttackTimer - CrystallineSentinelDirector.GuardExtendInterval,
                        CrystallineSentinelDirector.GuardExtendMin, CrystallineSentinelDirector.GuardExtendMax));
                }
            }

            // 玩家彻底跑掉（超出近战范围且仇恨见底）：直接跳到收招
            if (AttackTimer % CrystallineSentinelDirector.TurnInterval == 0
                && Vector2.Distance(ctx.Target.Center, ctx.Npc.Center) > CrystallineSentinelDirector.MeleeRange
                && ctx.AggroCounter <= CrystallineSentinelDirector.AggroCounterMin)
            {
                RebaseTimer(ctx, CrystallineSentinelDirector.GuardHoldFrames + 1);
            }

            for (int i = 0; i < 3; i++)
            {
                if (AtFrame(CrystallineSentinelDirector.GuardVolleyFirstCue + (i * CrystallineSentinelDirector.GuardVolleyCueStep)))
                {
                    LaunchMissile(ctx);
                }
            }

            if (AttackTimer > CrystallineSentinelDirector.GuardHoldFrames)
            {
                ctx.GuardCooldown = CrystallineSentinelDirector.GuardCooldownMax;
                NextBeat(ctx, (int)Beat.Recover, 1f);
                SetFrame(ctx, 4, 0);
                return null;
            }

            // 玩家靠到近战范围就主动解除护盾，立刻发动近战攻击
            if (AttackTimer > CrystallineSentinelDirector.GuardBreakEarliest
                && canHit && distance < CrystallineSentinelDirector.MeleeRange)
            {
                ctx.GuardCooldown = CrystallineSentinelDirector.GuardCooldownMax;
                return CrystallineSentinelHubState.ToState(ctx, CrystallineSentinelStateId.P1Spurt, CrystallineSentinelDirector.GuardBreakSpurtFrames);
            }

            return null;
        }

        /// <summary>护盾反击的飞弹：垂直上抛，随后自己找目标。旧 CrystallineSentinel.cs:993-999</summary>
        private static void LaunchMissile(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 pos = npc.Center + new Vector2(CrystallineSentinelDirector.MissileMuzzleX * npc.direction, CrystallineSentinelDirector.MissileMuzzleY);
            NPC missile = NPC.NewNPCDirect(npc.GetSource_FromAI(), pos, ModContent.NPCType<CrystallineSentinelMissile>());
            missile.velocity = -Vector2.UnitY * CrystallineSentinelDirector.MissileLaunchSpeed;
            missile.netUpdate = true;
            ctx.MarkDecision();
        }
    }
}
