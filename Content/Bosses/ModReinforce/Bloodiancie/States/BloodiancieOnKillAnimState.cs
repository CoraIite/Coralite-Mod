using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 死亡演出：向上缓浮、全程无敌，40 帧起掉碎块、230 帧前持续小爆炸特效，245 帧大爆炸（有伤害）并震屏，250 帧真正死亡。
    /// 由 <see cref="BloodiancieContext.KillRequested"/> 经基类 ServerUpdate 进入；本状态不会再切出，只在权威端 <c>NPC.Kill()</c>。
    /// 血玉天空在进入本状态时熄灭——旧代码只在 CheckDead（命中方 + 服务端）与 OnKill 里关，远端客户端要等到真死才黑；改由每个客户端各自关。
    /// </summary>
    [VaultState((int)BloodiancieStateId.onKillAnim, typeof(BloodiancieContext))]
    internal sealed class BloodiancieOnKillAnimState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.onKillAnim;

        /// <summary>演出态不走通用超时收招（收招会让血量 1 的 boss 继续出招）。</summary>
        protected override int TimeoutFrames => int.MaxValue;

        public override void OnEnter(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            base.OnEnter(machine, ctx);
            if (!VaultUtils.isClient)
            {
                ctx.KillRequested = false;
            }

            if (!Main.dedServ)
            {
                SkyManager.Instance.Deactivate("BloodJadeSky");
            }
        }

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            ctx.DeclareHover(BloodiancieDirector.KillAnimDampX, BloodiancieDirector.KillAnimAccelY, BloodiancieDirector.KillAnimLimitY);
            ctx.DeclareRotation(BloodiancieRotationMode.Normal);
            ctx.Invulnerable = true;
            ctx.UpdateFollowersSummon(Timer);

            if (Timer < BloodiancieDirector.KillAnimQuietFrames || Main.dedServ)
            {
                return;
            }

            if (Timer > BloodiancieDirector.KillAnimGoreStart && Timer % BloodiancieDirector.KillAnimGoreInterval == 0)
            {
                SpawnGore(ctx, "Rediancie_Gore2", new Vector2(0, 1), 1.5f);
                SpawnGore(ctx, "Rediancie_Gore3", new Vector2(0, 1), 1.5f);
                SpawnGore(ctx, "Rediancie_Gore4", new Vector2(0, -3), 1.5f);
            }

            if (Timer < BloodiancieDirector.KillAnimSparkEnd && Timer % BloodiancieDirector.KillAnimSparkInterval == 0)
            {
                Helper.RedJadeExplosion(ctx.Npc.Center + Main.rand.NextVector2Circular(BloodiancieDirector.KillAnimScatterX, BloodiancieDirector.KillAnimScatterY));
            }

            if (Timer == BloodiancieDirector.KillAnimBoomFrame)
            {
                Shake(ctx, BloodiancieDirector.BoomShakeStrength, BloodiancieDirector.BoomShakeVibration, BloodiancieDirector.BoomShakeFrames);
            }
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (Timer == BloodiancieDirector.KillAnimBoomFrame)
            {
                SpawnBigBoom(ctx, ctx.Npc.Center, BloodiancieDirector.KillAnimBoomDamage, BloodiancieDirector.KillAnimBoomKnockback);
            }

            if (Timer > BloodiancieDirector.KillAnimEndFrame)
            {
                ctx.Npc.Kill();
            }

            return null;
        }

        /// <summary>碎块（纯本地）。旧 AI.cs:139-141</summary>
        private static void SpawnGore(BloodiancieContext ctx, string goreName, Vector2 baseVelocity, float spread)
        {
            Gore.NewGoreDirect(ctx.Npc.GetSource_Death(),
                ctx.Npc.Center + Main.rand.NextVector2Circular(BloodiancieDirector.KillAnimScatterX, BloodiancieDirector.KillAnimScatterY),
                baseVelocity.RotatedBy(Main.rand.NextFloat(-spread, spread)),
                ctx.ModNpc.Mod.Find<ModGore>(goreName).Type);
        }
    }
}
