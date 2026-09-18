using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 等待态：枚举里的 0 号，出招池里没有它，只作为“ai[0] 还是默认值”时的安全落点——
    /// 正常飞一飞，权威端下一帧就固定提交闪电突袭。旧 ZacurrentDragon.States.cs:70-79
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.Waiting, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentWaitingState : ZacurrentStateBase
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.Waiting;

        // 旧实现这里一帧就走，两端都不做任何运动或动画，照搬不动。

        protected override IVaultState<ZacurrentDragonContext> AuthorityUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            ctx.Npc.TargetClosest();
            return ZacurrentHubState.CommitFixed(ctx, ZacurrentDragon.AIStates.LightningRaidNormal);
        }
    }
}
