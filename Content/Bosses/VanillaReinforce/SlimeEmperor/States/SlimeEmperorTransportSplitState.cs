using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 移位分裂：缩进王冠 → 瞬移到自身与玩家的中点上方 → 在原地留下分身 → 短暂绕圈 → 变回史莱姆。<br/>
    /// 节拍：Shrink0..3 → Teleport → Circle → Restore → Done。<br/>
    /// 这一招是轮换表里的“换位手”：玩家被物块挡住、跳不过去的槽位都会退到它，
    /// 所以它必须任何地形都成立——落点只依赖玩家坐标，不依赖地面。<br/>
    /// 公平阀：瞬移前有 60 帧的蓄力尘（散布从 80 收到 20），收拢过程就是读秒；落点固定在玩家上方 200 px，可预判。<br/>
    /// 旧 <c>SlimeEmperor.TransportSplit</c>（AI.TransportSplit.cs:12-176）。
    /// </summary>
    [VaultState((int)AIStates.TransportSplit, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorTransportSplitState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.TransportSplit;

        private enum Beat
        {
            /// <summary>缩进四拍（旧 SonState 0..3）</summary>
            Shrink0 = 0,
            Shrink1 = 1,
            Shrink2 = 2,
            Shrink3 = 3,
            /// <summary>蓄力 → 瞬移（旧 4）</summary>
            Teleport = 4,
            /// <summary>落点绕圈（旧 5）</summary>
            Circle = 5,
            /// <summary>变回史莱姆并复原（旧 6）</summary>
            Restore = 6,
            /// <summary>收招（旧 7）</summary>
            Done = 7,
        }

        /// <summary>瞬移前的旧位置（分身留在这里），<see cref="AuthorityUpdate"/> 同帧消费。</summary>
        private Vector2 teleportFrom;
        private bool teleportedThisFrame;

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            teleportedThisFrame = false;

            switch ((Beat)BeatIndex)
            {
                case Beat.Shrink0:
                case Beat.Shrink1:
                case Beat.Shrink2:
                case Beat.Shrink3:
                    UpdateCrownShrinkBeats(ctx);
                    break;

                case Beat.Teleport:
                    UpdateTeleport(ctx);
                    break;

                case Beat.Circle:
                    UpdateCircle(ctx);
                    break;

                case Beat.Restore:
                    CrownShrinkStep(ctx);
                    if (ScaleBeat(ctx, 1f, 1f, SlimeEmperorDirector.CrownStrikeRestoreLerp,
                        ctx.Scale.X > SlimeEmperorDirector.CrownStrikeRestoreDone, (int)Beat.Done))
                    {
                        ctx.Scale = Vector2.One;
                    }

                    break;
            }
        }

        /// <summary>
        /// 蓄力 60 帧后瞬移到中点上方。<b>这一招没有挑战加速档</b>（旧代码此处是写死的 60，与泰山压顶不同），照搬。
        /// 位置写入两端同算（只读玩家坐标），权威端另打决策点让快照立刻跟上。
        /// </summary>
        private void UpdateTeleport(SlimeEmperorContext ctx)
        {
            if (Timer < SlimeEmperorDirector.TeleportReadyFrames)
            {
                SpawnTeleportChargeDust(ctx, Timer / (float)SlimeEmperorDirector.TeleportReadyFrames);
                return;
            }

            if (Timer > SlimeEmperorDirector.TeleportReadyFrames)
            {
                EnterBeat(ctx, (int)Beat.Circle);
                ctx.Npc.TargetClosest();
                return;
            }

            NPC npc = ctx.Npc;
            npc.TargetClosest();
            teleportFrom = npc.Center;
            //落点：自身与玩家的中点横坐标、玩家上方 200 px
            npc.Center = new Vector2(
                MathHelper.Lerp(ctx.Target.Center.X, teleportFrom.X, 0.5f),
                ctx.Target.Center.Y + SlimeEmperorDirector.TransportSplitHeight);

            PlaySound(CoraliteSoundID.SlimeMount_Item81, ctx);
            SpawnTeleportDust(ctx, teleportFrom);
            ctx.DeclareDirect();
            ctx.MarkDecision();
            teleportedThisFrame = true;
        }

        /// <summary>落点后与玩家保持 400 px 绕一会，然后落回史莱姆形态。</summary>
        private void UpdateCircle(SlimeEmperorContext ctx)
        {
            int frames = ctx.Dangerous(Slime1Knowledge.Dangerous.SpeedBonus3_1)
                ? SlimeEmperorDirector.TransportSplitCircleFramesFast
                : SlimeEmperorDirector.TransportSplitCircleFrames;

            if (Timer < frames)
            {
                KeepDistance(ctx, ctx.Target.Center, SlimeEmperorDirector.TransportSplitKeepRadius,
                    SlimeEmperorDirector.TransportSplitAccel, SlimeEmperorDirector.TransportSplitMaxSpeed);
                return;
            }

            ctx.Npc.velocity.X *= 0;
            EnterBeat(ctx, (int)Beat.Restore);
            PlaySound(CoraliteSoundID.QueenSlime_Item154, ctx);
            ctx.EnterSlimeForm();
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            if (teleportedThisFrame)
            {
                teleportedThisFrame = false;

                //分身留在旧位置，弹力球撒在落点
                int avatars = SlimeEmperorDirector.TransportSplitAvatarCount();
                for (int i = 0; i < avatars; i++)
                {
                    NPC.NewNPC(ctx.Npc.GetSource_FromAI(), (int)teleportFrom.X, (int)teleportFrom.Y,
                        ModContent.NPCType<SlimeAvatar>(), Target: ctx.Npc.target, ai1: ctx.Npc.whoAmI);
                }

                SpawnElasticBalls(ctx, ctx.Npc.Center, SlimeEmperorDirector.TransportSplitBallCount(),
                    () => Helper.NextVec2Dir(SlimeEmperorDirector.BallScatterSpeedMin, SlimeEmperorDirector.BallScatterSpeedMax));
            }

            return ReadyToFinish((int)Beat.Done) ? EndAttack(ctx) : null;
        }
    }
}
