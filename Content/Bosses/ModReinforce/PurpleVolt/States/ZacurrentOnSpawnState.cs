using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 登场：两端把本体瞬移到玩家正上方 1500 px（同一帧、同一算式，服务端随后发包对齐），
    /// 权威端固定接闪电突袭并让那一招做满 3 次长冲（从天而降的开场）。旧 ZcurrentAI.cs:534-539 / ZacurrentDragon.States.cs:81-104
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.onSpawnAnmi, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentOnSpawnState : ZacurrentStateBase
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.onSpawnAnmi;

        protected override void SharedUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            if (ctx.AttackFinished)
            {
                return;
            }

            ctx.Npc.TargetClosest();
            ctx.Npc.Center = ctx.Target.Center - new Vector2(0, ZacurrentDirector.SpawnHeightAboveTarget);
            ctx.AttackFinished = true;
        }

        protected override IVaultState<ZacurrentDragonContext> AuthorityUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            if (!ctx.AttackFinished)
            {
                return null;
            }

            ctx.ForceRecorder2OnNextLightningRaid = true;
            ctx.MarkDecision();
            return ZacurrentHubState.CommitFixed(ctx, ZacurrentDragon.AIStates.LightningRaidNormal);
        }
    }
}
