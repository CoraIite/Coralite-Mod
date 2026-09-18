using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 死亡演出：60 帧内以 1 px/f 上浮、辉光线性亮到满并撒风暴尘 → 一帧爆散（冰星飞散 + 碎冰 + 冰爆光环 + 碎裂声）→ 权威端真死。<br/>
    /// 入口是 <c>CheckDead</c> 登记的 <see cref="BabyIceDragonContext.KillRequested"/>，由状态基类经 ServerUpdate 返回值切进来（C2 / 命中方客户端也会跑 CheckDead，所以那里绝不换态）。<br/>
    /// <c>NPC.Kill()</c> 是生命改动，只在权威端调用；客户端等 SyncNPC 到达，爆散演出靠一次性拍两端各放一次。旧 BabyIceDragon.cs:352-393
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.onKillAnim, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonOnKillAnimState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.onKillAnim;

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            ctx.Invulnerable = true;

            if (Timer <= BabyIceDragonDirector.KillAnimRiseFrames)
            {
                // 旧代码每帧 GlowAlpha += 1/60；改成按 Timer 直接算，两端各自可复现（C3）
                ctx.GlowAlpha = Timer / (float)BabyIceDragonDirector.KillAnimRiseFrames;
                ctx.Npc.velocity = -Vector2.UnitY * BabyIceDragonDirector.KillAnimRiseSpeed;
                ctx.SetFrame(1, 0);
                ctx.DeclareDirect();
                RiseDust(ctx);
                return;
            }

            ctx.GlowAlpha = 1f;
            ctx.DeclareKeep();

            if (CueDue(BabyIceDragonDirector.KillAnimRiseFrames + 1))
            {
                BurstCue(ctx);
            }
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if (Timer > BabyIceDragonDirector.KillAnimRiseFrames)
            {
                ctx.Npc.Kill();
            }

            return null;
        }

        /// <summary>上浮段风暴尘（纯本地）。旧 BabyIceDragon.cs:363-364</summary>
        private static void RiseDust(BabyIceDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            Dust.NewDustPerfect(ctx.Npc.Center + Main.rand.NextVector2Circular(BabyIceDragonDirector.KillAnimDustSpread, BabyIceDragonDirector.KillAnimDustSpread),
                DustID.ApprenticeStorm, Vector2.UnitY);
        }

        /// <summary>爆散：12 颗冰星飞向远处、20 粒碎冰、3 个冰爆光环、碎裂声。旧 BabyIceDragon.cs:368-386</summary>
        private static void BurstCue(BabyIceDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            for (int i = 0; i < BabyIceDragonDirector.KillAnimStarCount; i++)
            {
                Vector2 far = npc.Center + Main.rand.NextVector2CircularEdge(BabyIceDragonDirector.KillAnimStarFarRadius, BabyIceDragonDirector.KillAnimStarFarRadius);
                IceStarLight.Spawn(npc.Center + Main.rand.NextVector2CircularEdge(BabyIceDragonDirector.KillAnimStarSpawnRadius, BabyIceDragonDirector.KillAnimStarSpawnRadius),
                    Main.rand.NextVector2CircularEdge(BabyIceDragonDirector.KillAnimStarSpeed, BabyIceDragonDirector.KillAnimStarSpeed), 1f,
                    () => far, BabyIceDragonDirector.KillAnimStarSpeedLimit);
            }

            for (int j = 0; j < BabyIceDragonDirector.KillAnimIceDustCount; j++)
            {
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(BabyIceDragonDirector.KillAnimIceDustSpread, BabyIceDragonDirector.KillAnimIceDustSpread),
                    ModContent.DustType<CrushedIceDust>(),
                    -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-BabyIceDragonDirector.KillAnimIceDustAngle, BabyIceDragonDirector.KillAnimIceDustAngle))
                        * Main.rand.Next(BabyIceDragonDirector.KillAnimIceDustSpeedMin, BabyIceDragonDirector.KillAnimIceDustSpeedMax),
                    Scale: Main.rand.NextFloat(BabyIceDragonDirector.KillAnimIceDustScaleMin, BabyIceDragonDirector.KillAnimIceDustScaleMax));
            }

            for (int i = 0; i < BabyIceDragonDirector.KillAnimHaloCount; i++)
            {
                PRTLoader.NewParticle(npc.Center, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo>(), Color.White, BabyIceDragonDirector.KillAnimHaloScale);
            }

            Helper.PlayPitched("Icicle/Broken", BabyIceDragonDirector.BrokenSoundVolume, 0f, npc.Center);
        }
    }
}
