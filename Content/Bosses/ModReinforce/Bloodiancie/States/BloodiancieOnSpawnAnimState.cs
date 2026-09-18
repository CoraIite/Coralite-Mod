using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 出场动画：第 1 帧生成名牌弹幕并以 1.5 px/f 下沉，全程无敌；尘越撒越密；260 帧大爆炸并点亮血玉天空；
    /// 270 帧解除无敌、灌满一阶段选招表、固定首招多段爆炸。
    /// 节拍全部按 Timer 到点，两端各自推进；只有名牌弹幕与大爆炸在权威端生成，天空只在有画面的一端开。
    /// </summary>
    [VaultState((int)BloodiancieStateId.onSpawnAnim, typeof(BloodiancieContext))]
    internal sealed class BloodiancieOnSpawnAnimState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.onSpawnAnim;

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            // 第 1 帧起下沉（Direct 写一次），之后 Keep 保持匀速，120 帧后缓慢衰减
            if (Timer == BloodiancieDirector.SpawnAnimNameLineFrame)
            {
                ctx.Npc.velocity = new Vector2(0, BloodiancieDirector.SpawnAnimSinkSpeed);
                ctx.DeclareDirect();
            }
            else if (Timer > BloodiancieDirector.SpawnAnimSlowFrame)
            {
                ctx.DeclareDamp(BloodiancieDirector.SpawnAnimSlowDamp);
            }
            else
            {
                ctx.DeclareKeep();
            }

            // 出生第 1 帧到解除帧之间无敌（旧代码首帧置位、270 帧解除）
            ctx.Invulnerable = Timer >= BloodiancieDirector.SpawnAnimNameLineFrame && Timer < BloodiancieDirector.SpawnAnimEndFrame;

            if (Timer % BloodiancieDirector.SpawnAnimDustInterval == 0)
            {
                ChargeDust(ctx, Timer / BloodiancieDirector.SpawnAnimDustPerFrames,
                    BloodiancieDirector.SpawnAnimDustScaleGain, BloodiancieDirector.ChargeDustSpreadNarrow);
            }

            // 天空是纯表现，每个有画面的客户端各自开（旧代码在服务端也调，无意义）
            if (Timer == BloodiancieDirector.SpawnAnimBoomFrame && !Main.dedServ)
            {
                SkyManager.Instance.Activate("BloodJadeSky");
            }

            ctx.DeclareRotation(BloodiancieRotationMode.Normal);
            ctx.UpdateFollowersIdle(Timer);
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (Timer == BloodiancieDirector.SpawnAnimNameLineFrame)
            {
                ctx.Npc.NewProjectileDirectInAI_Server<Bloodiancie_OnSpawnAnim>(ctx.Npc.Center, Vector2.Zero, 0, 0);
            }

            if (Timer == BloodiancieDirector.SpawnAnimBoomFrame)
            {
                SpawnBigBoom(ctx, ctx.Npc.Center, BloodiancieDirector.SpawnAnimBoomDamage, BloodiancieDirector.SpawnAnimBoomKnockback);
            }

            if (Timer == BloodiancieDirector.SpawnAnimEndFrame)
            {
                // 首招前先灌满一阶段的可枯竭列表（旧 AI.cs:194）
                BloodiancieHubState.RefillLists(ctx, 1);
                return BloodiancieHubState.CommitFixed(ctx, BloodiancieStateId.explosion);
            }

            return null;
        }
    }
}
