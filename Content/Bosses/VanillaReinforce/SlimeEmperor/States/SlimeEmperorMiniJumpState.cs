using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using InnoVault.StateMachines;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 小跳步：连续三次 (1, 8) 的小跳，全程由上下文的通用跳跃机驱动。<br/>
    /// 节拍：Jump1 → Jump2 → Jump3 → Done，每次腾空段结束（<see cref="SlimeJumpEvent.JumpFinished"/>）换下一拍，
    /// 收招拍空转一帧后经 <c>EndAttack</c> 回提交口。<br/>
    /// 公平阀：起跳前有三格帧图的蓄力预告；背身跑远时横向冲量线性收敛，不会把玩家甩出屏幕。<br/>
    /// 旧 <c>SlimeEmperor.ThreeMiniJump</c>（AI.MiniJump.cs:11-26）。
    /// </summary>
    [VaultState((int)AIStates.MiniJump, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorMiniJumpState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.MiniJump;

        private enum Beat
        {
            /// <summary>第一跳（旧 SonState 0）</summary>
            Jump1 = 0,
            /// <summary>第二跳（旧 1）</summary>
            Jump2 = 1,
            /// <summary>第三跳（旧 2）</summary>
            Jump3 = 2,
            /// <summary>收招（旧 3）</summary>
            Done = SlimeEmperorDirector.MiniJumpCount,
        }

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            if (BeatIndex >= (int)Beat.Done)
            {
                return;
            }

            if (ctx.UpdateJump(SlimeEmperorDirector.MiniJumpSpeedY, SlimeEmperorDirector.MiniJumpSpeedX) == SlimeJumpEvent.JumpFinished)
            {
                EnterBeat(ctx, BeatIndex + 1);
            }
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
            => ReadyToFinish((int)Beat.Done) ? EndAttack(ctx) : null;
    }
}
