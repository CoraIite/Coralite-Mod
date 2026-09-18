using Coralite.Content.Bosses.Rediancie.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 出场动画：第 1 帧生成名牌弹幕并缓慢下沉，全程无敌；尘越撒越密；260 帧大爆炸 + 震屏；270 帧解除无敌、固定首招三连炸。
    /// 节拍全部按 Timer 到点，两端各自推进；只有名牌弹幕与大爆炸在权威端生成。
    /// </summary>
    [VaultState((int)RediancieStateId.onSpawnAnim, typeof(RediancieContext))]
    internal sealed class RediancieOnSpawnAnimState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.onSpawnAnim;

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            // 第 1 帧起下沉（Direct 写一次），之后 Keep 保持匀速，120 帧后缓慢衰减
            if (Timer == RediancieDirector.SpawnAnimNameLineFrame)
            {
                ctx.Npc.velocity = new Vector2(0, RediancieDirector.SpawnAnimSinkSpeed);
                ctx.DeclareDirect();
            }
            else if (Timer > RediancieDirector.SpawnAnimSlowFrame)
            {
                ctx.DeclareDamp(RediancieDirector.SpawnAnimSlowDamp);
            }
            else
            {
                ctx.DeclareKeep();
            }

            // 出生第 1 帧到解除帧之间无敌（旧代码 Timer==1 置位、270 帧解除）
            ctx.Invulnerable = Timer >= RediancieDirector.SpawnAnimNameLineFrame && Timer < RediancieDirector.SpawnAnimEndFrame;

            if (Timer % RediancieDirector.SpawnAnimDustInterval == 0)
            {
                ChargeDust(ctx, Timer / RediancieDirector.SpawnAnimDustPerFrames, RediancieDirector.SpawnAnimDustScaleGain);
            }

            if (Timer == RediancieDirector.SpawnAnimBoomFrame)
            {
                Shake(ctx, RediancieDirector.BoomShakeStrength, RediancieDirector.BoomShakeVibration, RediancieDirector.BoomShakeFrames);
            }

            ctx.DeclareRotation(RediancieRotationMode.Normal);
            ctx.UpdateFollowersIdle(Timer);
        }

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            if (Timer == RediancieDirector.SpawnAnimNameLineFrame)
            {
                ctx.Npc.NewProjectileDirectInAI_Server<Rediancie_OnSpawnAnim>(ctx.Npc.Center, Vector2.Zero, 0, 0);
            }

            if (Timer == RediancieDirector.SpawnAnimBoomFrame)
            {
                SpawnBigBoom(ctx, ctx.Npc.Center, RediancieDirector.SpawnAnimBoomDamage, RediancieDirector.SpawnAnimBoomKnockback);
            }

            if (Timer == RediancieDirector.SpawnAnimEndFrame)
            {
                return RediancieHubState.CommitFixed(ctx, RediancieStateId.explosion);
            }

            return null;
        }
    }
}
