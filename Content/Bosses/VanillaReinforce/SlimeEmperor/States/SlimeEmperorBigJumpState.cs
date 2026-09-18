using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using InnoVault.StateMachines;
using Terraria;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 大跳：一次 (3, 10) 的高跳，起跳瞬间在脚下撒一圈弹力球，腾空段结束即收招。<br/>
    /// 节拍只有 Jump → Done，跳跃机的三段事件各管一件事：<br/>
    /// · <see cref="SlimeJumpEvent.Landed"/>（只在进招时人在空中才会经过）：FTW 世界向上喷 4 发尖刺凝胶球；<br/>
    /// · <see cref="SlimeJumpEvent.JumpStarted"/>：离地瞬间撒弹力球，蓄力帧图就是它的预告；<br/>
    /// · <see cref="SlimeJumpEvent.JumpFinished"/>：腾空结束，换收招拍。<br/>
    /// 旧 <c>SlimeEmperor.BigJump</c>（SlimeEmperor.cs:436-476）。
    /// </summary>
    [VaultState((int)AIStates.BigJump, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorBigJumpState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.BigJump;

        private enum Beat
        {
            /// <summary>起跳到腾空结束（旧 SonState 0）</summary>
            Jump = 0,
            /// <summary>收招（旧 1）</summary>
            Done = 1,
        }

        /// <summary>本帧跳跃机抛出的事件，供 <see cref="AuthorityUpdate"/> 同帧消费。</summary>
        private SlimeJumpEvent jumpEvent;

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            jumpEvent = SlimeJumpEvent.None;
            if (BeatIndex >= (int)Beat.Done)
            {
                return;
            }

            jumpEvent = ctx.UpdateJump(SlimeEmperorDirector.BigJumpSpeedY, SlimeEmperorDirector.BigJumpSpeedX);
            if (jumpEvent == SlimeJumpEvent.JumpFinished)
            {
                EnterBeat(ctx, (int)Beat.Done);
            }
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            switch (jumpEvent)
            {
                //FTW 专属：落地回弹结束时向上喷一圈尖刺球
                case SlimeJumpEvent.Landed when Main.getGoodWorld:
                    ShootUpward<SpikeGelBall>(ctx, SlimeEmperorDirector.BigJumpFtwSpikeCount, SlimeEmperorDirector.BigJumpFtwSpikeDamage,
                        SlimeEmperorDirector.BigJumpFtwSpikeSpeed, SlimeEmperorDirector.BigJumpFtwSpikeSpread, SlimeEmperorDirector.SpikeKnockback);
                    break;

                //离地瞬间在原地撒弹力球
                case SlimeJumpEvent.JumpStarted:
                    SpawnElasticBalls(ctx, ctx.Npc.Center, SlimeEmperorDirector.BigJumpBallCount(),
                        () => -Vector2.UnitY * Main.rand.NextFloat(SlimeEmperorDirector.BallScatterSpeedMin, SlimeEmperorDirector.BallScatterSpeedMax));
                    break;
            }

            jumpEvent = SlimeJumpEvent.None;
            return ReadyToFinish((int)Beat.Done) ? EndAttack(ctx) : null;
        }
    }
}
