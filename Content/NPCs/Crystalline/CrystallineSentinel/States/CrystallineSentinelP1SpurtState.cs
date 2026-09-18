using Coralite.Content.NPCs.Crystalline.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 一阶段刺击：<br/>
    /// Approach：朝玩家走，越近计时走得越快（24 格内 +2、10 格内再 +5），最长 8 秒或走到没路就结算——
    /// 玩家在 16 格内且看得见就进起手，否则退回站立 1 秒。<br/>
    /// Thrust：帧速 5 推满 21 帧的起手动作，第 20 帧放起手音（预告窗 10 帧），第 30 帧开伤害窗 + 前冲 10 px/f 并放出刺击判定，
    /// 之后每帧 0.9 衰减、前方 4 格探到悬崖直接刹死（不冲下坑），105 帧收招。<br/>
    /// 旧 <c>P1Spurt</c>（CrystallineSentinel.cs:784-882）。护盾主动撤盾接这一招时预充 540 帧，所以第一帧就直接进起手。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P1Spurt, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP1SpurtState : CrystallineSentinelStateBase
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P1Spurt;

        private enum Beat
        {
            /// <summary>朝玩家走（旧 Recorder = 0）</summary>
            Approach = 0,
            /// <summary>戳（旧 Recorder = 1）</summary>
            Thrust = 1,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>本帧与目标的距离（SharedUpdate 算一次，AuthorityUpdate 复用，两端同值）。</summary>
        private float distance;

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            distance = Vector2.Distance(ctx.Target.Center, ctx.Npc.Center);

            if (CurrentBeat == Beat.Thrust)
            {
                UpdateThrust(ctx);
                return;
            }

            UpdateApproach(ctx);
        }

        private void UpdateApproach(CrystallineSentinelContext ctx)
        {
            if (AttackTimer % CrystallineSentinelDirector.TurnInterval == 0)
            {
                ctx.FaceTarget();
            }

            ctx.DeclareGroundWalk(CrystallineSentinelDirector.SpurtApproachMaxSpeed, CrystallineSentinelDirector.SpurtApproachAccel, true);
            // 走向玩家的路上被打会被拖慢（旧 OnHitSlow 只在这一拍生效）
            ctx.HitSlowdown = true;
            WalkFrame(ctx);

            // 越近计时推得越快：贴近时几乎立刻起手
            if (distance < CrystallineSentinelDirector.SpurtCloseRange)
            {
                Timer += CrystallineSentinelDirector.SpurtCloseTimerBonus;
                if (distance < CrystallineSentinelDirector.SpurtVeryCloseRange)
                {
                    Timer += CrystallineSentinelDirector.SpurtVeryCloseTimerBonus;
                }
            }
        }

        private void UpdateThrust(CrystallineSentinelContext ctx)
        {
            ctx.DeclareDirect();

            if (AttackTimer > 0 && AttackTimer % CrystallineSentinelDirector.SpurtFrameRate == 0)
            {
                AdvanceFrame(ctx, CrystallineSentinelDirector.SpurtFrameMaxY);
            }

            if (AtFrame(CrystallineSentinelDirector.SpurtDashFrame - CrystallineSentinelDirector.SpurtSoundLead) && !Main.dedServ)
            {
                Helper.PlayPitchedVariants(AssetDirectory.Sounds.Crystalline + "Sentinel_Attack", 0.4f, 0, 0, 2, ctx.Npc.Center);
            }

            if (AtFrame(CrystallineSentinelDirector.SpurtDashFrame))
            {
                ctx.Npc.velocity.X = CrystallineSentinelDirector.SpurtDashSpeed * ctx.Npc.direction;
            }

            // 伤害窗从出手帧开到收招（旧代码 CanHit 置真后一直有效到换状态）
            if (AttackTimer >= CrystallineSentinelDirector.SpurtDashFrame)
            {
                ctx.CanHit = true;
            }

            if (AttackTimer > CrystallineSentinelDirector.SpurtDashFrame)
            {
                ctx.Npc.velocity.X *= CrystallineSentinelDirector.SpurtDampAfterDash;

                if (CliffAhead(ctx))
                {
                    ctx.Npc.velocity.X = 0f;
                }
            }
        }

        /// <summary>前方 4 格内有一格探不到地面就算悬崖。旧 CrystallineSentinel.cs:866-875</summary>
        private static bool CliffAhead(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            for (int i = 0; i < CrystallineSentinelDirector.SpurtCliffCheckCount; i++)
            {
                Point pos = (npc.Bottom + new Vector2(16 * i * npc.direction + npc.velocity.X, 0)).ToTileCoordinates();
                if (!Helper.GroundSearch(pos, new Point(0, 1), CrystallineSentinelDirector.SpurtCliffSearchLength))
                {
                    return true;
                }
            }

            return false;
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (CurrentBeat == Beat.Thrust)
            {
                if (AtFrame(CrystallineSentinelDirector.SpurtDashFrame))
                {
                    ctx.Npc.NewProjectileInAI_Server<CrystallineSentinelSpurtProj>(
                        ctx.Npc.Center + new Vector2(ctx.Npc.direction * CrystallineSentinelDirector.SpurtProjOffsetX, 0),
                        Vector2.Zero, CrystallineSentinelDirector.SpurtProjDamage(), 0,
                        ai0: ctx.Npc.whoAmI, ai1: ctx.Npc.direction);
                    ctx.MarkDecision();
                }

                if (AttackTimer > CrystallineSentinelDirector.SpurtTotalFrames)
                {
                    return ReturnToIdle(ctx, IdleFramesOnFinish);
                }

                return null;
            }

            if (AttackTimer > CrystallineSentinelDirector.SpurtApproachTimeout || !CanWalkForward(ctx))
            {
                // 可以攻击，进入下一拍
                if (CanHitTarget(ctx, out _) && distance < CrystallineSentinelDirector.SpurtEnterRange)
                {
                    ctx.FaceTarget();
                    ctx.Npc.velocity.X = 0f;
                    NextBeat(ctx, (int)Beat.Thrust);
                    SetFrame(ctx, 2, 0);
                    return null;
                }

                return ReturnToIdle(ctx, CrystallineSentinelDirector.SpurtFailIdleFrames);
            }

            return null;
        }
    }
}
