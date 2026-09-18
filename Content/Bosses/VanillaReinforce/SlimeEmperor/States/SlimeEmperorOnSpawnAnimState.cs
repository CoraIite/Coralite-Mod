using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using InnoVault.StateMachines;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 出生态：这个 boss 没有入场演出，进来就该打。<br/>
    /// 旧写法是在 <c>OnEnter</c> 里服务端直接 <c>ResetStates()</c>——在换态钩子里再换态，客户端会被拉一下；
    /// 现在改为第一帧经 <c>EndAttack</c> 走提交口，选招与记账和其它收招走同一条路。<br/>
    /// 正常开局 <c>OnSpawn</c> 就把 ai[0] 写成泰山压顶，所以这个状态只有在外部把 ai[0] 写成 12 时才会进来。<br/>
    /// 旧 <c>SlimeEmperorOnSpawnAnimState.OnEnter</c>（SlimeEmperor.States.cs:117-126）。
    /// </summary>
    [VaultState((int)AIStates.OnSpawnAnim, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorOnSpawnAnimState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.OnSpawnAnim;

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
            => EndAttack(ctx);
    }
}
