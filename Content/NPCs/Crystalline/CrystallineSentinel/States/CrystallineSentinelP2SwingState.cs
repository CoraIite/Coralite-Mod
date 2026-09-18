using Coralite.Content.NPCs.Crystalline.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 二阶段挥刀（二连击，两刀左右手交替）：<br/>
    /// 拉近 45 帧（第二刀只有 4 帧，连招不给喘息）→ 抬手 19 帧（手部帧推到出手姿态，起手闪光只在第一刀放）
    /// → 出手（速度对折 + 放出挥刀判定，最长前伸 480 px）→ 滞留 80/90 帧 → 收刀 21 帧。<br/>
    /// 第一刀结束直接接第二刀，第二刀结束回悬浮并留 60 帧喘息。<br/>
    /// 旧 <c>P2Swing</c>（CrystallineSentinel.cs:1309-1436）。用哪只手由权威端在入场时掷出、随热字段过线，挥刀弹幕据此取挂点。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P2Swing, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP2SwingState : CrystallineSentinelStateBase
    {
        /// <summary>自用槽 A：这一刀用哪只手（+1 左 / −1 右，旧 ai[2]）。挥刀弹幕与手部动画都读它，必须过线。</summary>
        private const int HandSignSlot = CoraliteBossHotSlots.A;

        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P2Swing;

        public override CrystallineSentinelCommon Common => CrystallineSentinelCommon.PhaseTwo;

        protected override int IdleFramesOnFinish => CrystallineSentinelDirector.P2IdleGapFrames;

        /// <summary>第几刀（旧 localAI[0] Recorder2）：0 首刀、1 第二刀。借用基座的子拍槽，天然过线。</summary>
        private int SwingIndex => BeatIndex;

        private int handSign = 1;

        /// <summary>拉近段末（第二刀几乎没有）。</summary>
        private int ApproachEnd => SwingIndex == 0
            ? CrystallineSentinelDirector.SwingApproachFrames
            : (int)(CrystallineSentinelDirector.SwingApproachFrames * CrystallineSentinelDirector.SwingSecondApproachScale);

        /// <summary>抬手段末 = 出手拍。</summary>
        private int LaunchFrame => ApproachEnd + CrystallineSentinelDirector.SwingReadyFrames;

        /// <summary>滞留段末（首刀短 10 帧，接第二刀更快）。</summary>
        private int IdleEnd => LaunchFrame + CrystallineSentinelDirector.SwingIdleFrames
            + (SwingIndex == 0 ? CrystallineSentinelDirector.SwingFirstIdleBonus : 0);

        /// <summary>收刀段末。</summary>
        private int DelayEnd => IdleEnd + CrystallineSentinelDirector.SwingDelayFrames;

        public override void OnEnter(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            base.OnEnter(machine, ctx);

            if (!VaultUtils.isClient)
            {
                // 首刀随机左右；掷在入场钩子里，带来换态的那一包就捎着这个槽（客户端不会先看到错的手）
                handSign = Main.rand.NextFromList(-1, 1);
            }
        }

        public override void WriteHot(CrystallineSentinelContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[HandSignSlot] = handSign;
        }

        public override void ReadHot(CrystallineSentinelContext ctx)
        {
            base.ReadHot(ctx);
            handSign = (int)ctx.Hot[HandSignSlot];
        }

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            ctx.DeclareDirect();
            ctx.SwingHandSign = handSign;

            if (AtFrame(0) && SwingIndex == 0)
            {
                TwinkleEffect(ctx);
            }

            P2BodyFrame(ctx);

            if (AttackTimer % CrystallineSentinelDirector.P2WanderInterval == 0)
            {
                ctx.Npc.velocity = ctx.Npc.velocity.RotatedBy(Wobble(ctx, CrystallineSentinelDirector.P2WanderAngle));
            }

            HoverBesideTarget(ctx, CrystallineSentinelDirector.SwingHoverSpeed);
            CollideSpeed(ctx);
            SpeedUp(ctx, CrystallineSentinelDirector.SwingMaxSpeed, CrystallineSentinelDirector.SwingAccel);
            ctx.FaceTarget();

            int hand = handSign > 0 ? 1 : 0;

            if (AttackTimer < ApproachEnd)
            {
                // 拉太远就贴上去（不是锁头，只是把距离拉进刀长）
                if (Vector2.Distance(ctx.Npc.Center, ctx.Target.Center) > CrystallineSentinelDirector.SwingChaseRange)
                {
                    ctx.Npc.velocity += (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero) * CrystallineSentinelDirector.SwingChaseAccel;
                    if (ctx.Npc.velocity.Length() > CrystallineSentinelDirector.SwingChaseMaxSpeed)
                    {
                        ctx.Npc.velocity = ctx.Npc.velocity.SafeNormalize(Vector2.Zero) * CrystallineSentinelDirector.SwingChaseMaxSpeed;
                    }
                }
            }

            if (AtFrame(ApproachEnd))
            {
                ctx.HandFrame[hand] = 0;
            }
            else if (AttackTimer < LaunchFrame)
            {
                AdvanceHandFrame(ctx, hand);
            }
            else if (AtFrame(LaunchFrame))
            {
                ctx.Npc.velocity *= CrystallineSentinelDirector.SwingLaunchDamp;
                ctx.SetText(CrystallineSentinelTextType.Fire, CrystallineSentinelDirector.FireTextFrames);

                if (!Main.dedServ)
                {
                    Helper.PlayPitched(AssetDirectory.Sounds.Misc + "Slash", 0.4f, 0.5f, ctx.Npc.Center);
                }
            }
            else if (AttackTimer >= IdleEnd && AttackTimer < DelayEnd)
            {
                ctx.Npc.velocity *= CrystallineSentinelDirector.SwingDelayDamp;
                AdvanceHandFrame(ctx, hand);
            }
        }

        /// <summary>手部帧每 3 帧推一格，推到收拢为止。旧 CrystallineSentinel.cs:1395-1399,1419-1424</summary>
        private void AdvanceHandFrame(CrystallineSentinelContext ctx, int hand)
        {
            if (AttackTimer % CrystallineSentinelDirector.SwingHandFrameRate != 0)
            {
                return;
            }

            if (ctx.HandFrame[hand] < CrystallineSentinelDirector.MaxHandFrame)
            {
                ctx.HandFrame[hand]++;
            }
        }

        /// <summary>起手闪光：从手掌朝玩家飘出 5 粒（只有首刀有这段预告）。旧 CrystallineSentinel.cs:1320-1336</summary>
        private void TwinkleEffect(CrystallineSentinelContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            int hand = handSign > 0 ? 1 : 0;
            for (int i = 0; i < CrystallineSentinelDirector.SwingTwinkleCount; i++)
            {
                Vector2 pos = handSign > 0 ? ctx.LeftHandPos : ctx.RightHandPos;
                Vector2 vel = pos.DirectionTo(ctx.Target.Center)
                    * Main.rand.NextFloat(CrystallineSentinelDirector.SwingTwinkleSpeedMin, CrystallineSentinelDirector.SwingTwinkleSpeedMax);
                CrystallineSentinelTwinkle twinkle = PRTLoader.NewParticle<CrystallineSentinelTwinkle>(pos, vel);
                if (twinkle == null)
                {
                    continue;
                }

                twinkle.FollowNPCIndex = ctx.Npc.whoAmI;
                twinkle.TargetIndex = ctx.Npc.target;
                twinkle.HandIndex = hand;
                twinkle.Factor = Utils.Remap(i, 0, CrystallineSentinelDirector.SwingTwinkleCount, 0, 1);
                twinkle.startPos = pos;
                twinkle.endPos = pos;
            }
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (AtFrame(LaunchFrame))
            {
                ctx.Npc.NewProjectileDirectInAI_Server<CrystallineSentinelSwing>(
                    handSign > 0 ? ctx.LeftHandPos : ctx.RightHandPos,
                    (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero),
                    CrystallineSentinelDirector.SwingProjDamage(), CrystallineSentinelDirector.SwingProjKnockback,
                    ctx.Npc.target, ctx.Npc.whoAmI);
                ctx.MarkDecision();
            }

            if (AttackTimer < DelayEnd)
            {
                return null;
            }

            if (SwingIndex < CrystallineSentinelDirector.SwingAttackCount - 1)
            {
                // 接第二刀：换手、计时归零
                handSign *= -1;
                NextBeat(ctx, SwingIndex + 1);
                return null;
            }

            return ReturnToIdle(ctx, IdleFramesOnFinish);
        }
    }
}
