using Coralite.Content.Bosses.Rediancie.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 死亡演出：向上缓浮、全程无敌，40 帧起掉碎块、230 帧前持续小爆炸特效，245 帧大爆炸（有伤害），250 帧真正死亡。
    /// 由 <see cref="RediancieContext.KillRequested"/> 经基类 ServerUpdate 进入；本状态不会再切出，只在权威端 <c>NPC.Kill()</c>。
    /// </summary>
    [VaultState((int)RediancieStateId.onKillAnim, typeof(RediancieContext))]
    internal sealed class RediancieOnKillAnimState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.onKillAnim;

        /// <summary>演出态不走通用超时收招（收招会让血量 1 的 boss 继续出招）。</summary>
        protected override int TimeoutFrames => int.MaxValue;

        public override void OnEnter(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            base.OnEnter(machine, ctx);
            if (!VaultUtils.isClient)
            {
                ctx.KillRequested = false;
            }
        }

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            ctx.DeclareHover(RediancieDirector.KillAnimDampX, RediancieDirector.KillAnimAccelY, RediancieDirector.KillAnimLimitY);
            ctx.DeclareRotation(RediancieRotationMode.Normal);
            ctx.Invulnerable = true;
            ctx.UpdateFollowersSummon(Timer);

            if (Timer < RediancieDirector.KillAnimQuietFrames || Main.dedServ)
            {
                return;
            }

            if (Timer > RediancieDirector.KillAnimGoreStart && Timer % RediancieDirector.KillAnimGoreInterval == 0)
            {
                SpawnGore(ctx, "Rediancie_Gore2", new Vector2(0, 1), 1.5f);
                SpawnGore(ctx, "Rediancie_Gore3", new Vector2(0, 1), 1.5f);
                SpawnGore(ctx, "Rediancie_Gore4", new Vector2(0, -3), 1.5f);
            }

            if (Timer < RediancieDirector.KillAnimSparkEnd && Timer % RediancieDirector.KillAnimSparkInterval == 0)
            {
                Helper.RedJadeExplosion(ctx.Npc.Center + Main.rand.NextVector2Circular(RediancieDirector.KillAnimScatterX, RediancieDirector.KillAnimScatterY));
            }

            if (Timer == RediancieDirector.KillAnimBoomFrame)
            {
                Shake(ctx, RediancieDirector.BoomShakeStrength, RediancieDirector.BoomShakeVibration, RediancieDirector.BoomShakeFrames);
            }
        }

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            if (Timer == RediancieDirector.KillAnimBoomFrame)
            {
                SpawnBigBoom(ctx, ctx.Npc.Center, RediancieDirector.KillAnimBoomDamage, RediancieDirector.KillAnimBoomKnockback);
            }

            if (Timer > RediancieDirector.KillAnimEndFrame)
            {
                ctx.Npc.Kill();
            }

            return null;
        }

        /// <summary>碎块（纯本地）。旧 Rediancie.cs:376-378</summary>
        private static void SpawnGore(RediancieContext ctx, string goreName, Vector2 baseVelocity, float spread)
        {
            Gore.NewGoreDirect(ctx.Npc.GetSource_Death(),
                ctx.Npc.Center + Main.rand.NextVector2Circular(RediancieDirector.KillAnimScatterX, RediancieDirector.KillAnimScatterY),
                baseVelocity.RotatedBy(Main.rand.NextFloat(-spread, spread)),
                ctx.ModNpc.Mod.Find<ModGore>(goreName).Type);
        }
    }
}
