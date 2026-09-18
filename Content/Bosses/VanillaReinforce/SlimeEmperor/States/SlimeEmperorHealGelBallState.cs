using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using InnoVault.StateMachines;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 回血球：<b>未实装</b>。旧代码只写了“先落地”这一拍，落地之后是空分支、也不收招；
    /// 三张轮换表都没有它，召唤它的开关（<c>CanUseHealGelBall</c>）在主控里是注释掉的，所以实战进不来。<br/>
    /// 迁移时保留原样（不补招式内容，那属于战斗设计），只把“落地后空转到死”改成由状态基类的超时兜底收招，
    /// 万一有人把它塞进轮换表也不会软锁。<br/>
    /// 旧 <c>SlimeEmperor.HealGelBall</c>（AI.HealGelBall.cs:5-18）。
    /// </summary>
    [VaultState((int)AIStates.HealGelBall, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorHealGelBallState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.HealGelBall;

        private enum Beat
        {
            /// <summary>落地（旧 SonState 0）</summary>
            Ground = 0,
            /// <summary>旧代码的空分支（旧 1）：无内容，等超时兜底</summary>
            Idle = 1,
        }

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            if (BeatIndex != (int)Beat.Ground)
            {
                return;
            }

            if (ctx.UpdateJump(SlimeEmperorDirector.HealGelBallJumpY, SlimeEmperorDirector.HealGelBallJumpX) == SlimeJumpEvent.Landed)
            {
                EnterBeat(ctx, (int)Beat.Idle);
            }
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
            => null;
    }
}
