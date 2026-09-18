using Coralite.Content.Bosses.BabyIceDragon.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 转二阶段吼叫动画：全程缓慢收速 → 20 帧合翅停住 → 40 帧吼叫 → 80 帧收招。<br/>
    /// 由 hub 的转阶段闸固定提交（下雨与二阶段招池重填在 <see cref="BabyIceDragonHubState.Commit"/> 里已经做过）。
    /// 全程不翻帧图、不改朝向，和旧代码一致。旧 BabyIceDragon.cs:494-545
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.roaringAnim, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonRoaringAnimState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.roaringAnim;

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            ctx.DeclareDamp(BabyIceDragonDirector.RoarAnimDamp);

            if (Timer == BabyIceDragonDirector.RoarAnimStopFrame)
            {
                ctx.SetFrameY(BabyIceDragonDirector.SpawnFrameY);
                ctx.Npc.velocity = Vector2.Zero;
                ctx.DeclareDirect();
            }

            if (Timer < BabyIceDragonDirector.RoarAnimRoarFrame)
            {
                return;
            }

            if (CueDue(BabyIceDragonDirector.RoarAnimRoarFrame))
            {
                RoarCue(ctx);
            }

            if (Timer < BabyIceDragonDirector.RoarAnimEndFrame)
            {
                RoarParticles(ctx);
            }
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if (Timer >= BabyIceDragonDirector.RoarAnimEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
