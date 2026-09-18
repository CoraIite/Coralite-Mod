using Coralite.Content.Bosses.BabyIceDragon.Core;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 招式后摇：悬在目标头顶上方扇翅缓一口气，帧数由收招的那一招指定（普通 30 / 大师 10 / 龙卷与冰锥 31 / 眩晕后 29 / 冰球炸完 40）。<br/>
    /// 这是 skill 说的「声明出来的间隙」，招式之间的喘息全靠它和 <see cref="BabyIceDragonDizzyState"/>，所以 hub 停留帧数是 0、节奏不变。<br/>
    /// 旧代码在这里先 <c>AngleTowards(0, 0.08)</c> 又立刻被 <c>NormallyFlyingFrame</c> 的倾斜覆盖，等效只有倾斜，这里按等效写。旧 BabyIceDragon.cs:573-594 + :935-951
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.rest, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonRestState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.rest;

        /// <summary>本次休息帧数（权威端量：只用来裁决退出，客户端跟 ai[0] 走，不需要过线）。</summary>
        private int restFrames = BabyIceDragonDirector.RestFrames;

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if (Timer == 1)
            {
                ctx.FaceTarget();
            }

            ctx.DeclareDampX(BabyIceDragonDirector.RestDampX);
            ctx.DeclareHoverY(BabyIceDragonDirector.RestHoverDirY, BabyIceDragonDirector.RestHoverY, BabyIceDragonDirector.RestDeadZoneY,
                BabyIceDragonDirector.RestSpeedY, BabyIceDragonDirector.RestAccelY, BabyIceDragonDirector.RestTurnY, BabyIceDragonDirector.RestDampY);
            ctx.DeclareFlyingFrame();
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            // 请求必须在本状态里消费掉，否则退出时会被基类的前置检查立刻拉回来；休息中又被请求一次就重开一段（旧 HaveARest 直接重设计时器）
            if (ctx.PendingRestFrames > 0)
            {
                restFrames = ctx.PendingRestFrames;
                ctx.PendingRestFrames = 0;
                ctx.Npc.TargetClosest();
                if (Timer > 1)
                {
                    Timer = 0;
                }

                ctx.MarkDecision();
                return null;
            }

            if (Timer > restFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
