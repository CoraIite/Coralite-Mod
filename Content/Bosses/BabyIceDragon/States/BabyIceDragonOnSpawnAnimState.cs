using Coralite.Content.Bosses.BabyIceDragon.Core;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 出场动画：破壳白光 → 撒风暴尘扇翅飘浮 → 合翅停住 → 吼叫 → 扇翅爬升 → 进首招。<br/>
    /// 全程无敌、开物块碰撞（旧代码在出生首帧把 <c>noTileCollide</c> 关掉、直到 <c>ResetStates</c> 才打开，是破壳从地面钻出的表现）。<br/>
    /// 本状态是全 boss 唯一按旧口径「Timer 从 0 起算」的一个（旧 body 首帧读 0，基座首帧读 1），
    /// 所以内部统一用 <c>Timer - 1</c> 对齐 Director 里的拍点。旧 BabyIceDragon.cs:395-492
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.onSpawnAnim, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonOnSpawnAnimState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.onSpawnAnim;

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            int t = Timer - 1;

            ctx.Invulnerable = true;
            ctx.TileCollide = true;

            if (t == 0)
            {
                ctx.GlowAlpha = 1f;
                ctx.Npc.velocity.Y = BabyIceDragonDirector.SpawnAnimRiseSpeed;
                ctx.DeclareDirect();
                return;
            }

            if (t < BabyIceDragonDirector.SpawnAnimDustFrames)
            {
                SpawnDust(ctx);
            }

            if (t < BabyIceDragonDirector.SpawnAnimFlyFrames)
            {
                ctx.DeclareKeep();
                ctx.DeclareFlyingFrame();
                return;
            }

            if (t == BabyIceDragonDirector.SpawnAnimFlyFrames)
            {
                ctx.SetFrameY(BabyIceDragonDirector.SpawnFrameY);
                ctx.Npc.velocity = Vector2.Zero;
                ctx.DeclareDirect();
                return;
            }

            if (t < BabyIceDragonDirector.SpawnAnimRoarFrame)
            {
                ctx.DeclareKeep();
                return;
            }

            if (t < BabyIceDragonDirector.SpawnAnimRoarEnd)
            {
                ctx.DeclareKeep();
                if (CueDue(BabyIceDragonDirector.SpawnAnimRoarFrame + 1))
                {
                    RoarCue(ctx);
                }

                RoarParticles(ctx, t);
                return;
            }

            // 爬升段：X 缓慢收速，扇翅时向上加速
            ctx.DeclareDampX(BabyIceDragonDirector.SpawnAnimDampX);
            ctx.DeclareFlapY(BabyIceDragonDirector.SpawnAnimFlapAccel, BabyIceDragonDirector.SpawnAnimFlapDamp, BabyIceDragonDirector.SpawnAnimFlapLimit);
            ctx.DeclareFlyingFrame(0, false);
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            int t = Timer - 1;

            if (t == 0)
            {
                ctx.SpawnHostile<BabyIceDragon_OnSpawnAnim>(ctx.Npc.GetSource_FromThis(), ctx.Npc.Center, Vector2.Zero, 0, 0);
                ctx.MarkDecision();
                return null;
            }

            if (t >= BabyIceDragonDirector.SpawnAnimEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        /// <summary>破壳风暴尘（纯本地）。旧 BabyIceDragon.cs:413-418</summary>
        private static void SpawnDust(BabyIceDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            Dust dust = Dust.NewDustPerfect(ctx.Npc.Center + Main.rand.NextVector2Circular(BabyIceDragonDirector.SpawnAnimDustSpread, BabyIceDragonDirector.SpawnAnimDustSpread),
                DustID.ApprenticeStorm,
                Vector2.UnitY * Main.rand.NextFloat(BabyIceDragonDirector.SpawnAnimDustSpeedMin, BabyIceDragonDirector.SpawnAnimDustSpeedMax),
                Scale: Main.rand.NextFloat(BabyIceDragonDirector.SpawnAnimDustScaleMin, BabyIceDragonDirector.SpawnAnimDustScaleMax));
            dust.noGravity = true;
        }
    }
}
