using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 死亡演出：只有一帧——掉王冠 gore，然后权威端真死。<br/>
    /// <c>CheckDead</c> 不在那里换态（命中方客户端也会跑到 <c>CheckDead</c>），它只把血锁到 1 并登记死亡请求，
    /// 由状态基类的 ServerUpdate 经返回值切到这里；权威端 <c>NPC.Kill()</c> 再次触发 <c>CheckDead</c> 时已在本态 → 放行真死。<br/>
    /// 全程声明无敌，免得那一帧被别人的伤害重复结算。<br/>
    /// 旧 <c>SlimeEmperor.OnKillAnim</c>（SlimeEmperor.cs:422-433）。
    /// </summary>
    [VaultState((int)AIStates.OnKillAnim, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorOnKillAnimState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.OnKillAnim;

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            ctx.Invulnerable = true;

            if (Timer == SlimeEmperorDirector.KillAnimFrame && !Main.dedServ)
            {
                ctx.Boss.SpawnCrownGore();
            }
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            if (Timer >= SlimeEmperorDirector.KillAnimFrame)
            {
                ctx.Npc.Kill();
            }

            return null;
        }
    }
}
