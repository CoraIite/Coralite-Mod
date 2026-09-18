using Coralite.Content.NPCs.Crystalline.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 死亡演出：无敌 + 速度 0.9 衰减，一路炸碎岩爆点与结晶闪光，50~80 帧之间按 Gore 帧号逐块崩开，90 帧时真死。<br/>
    /// 旧 <c>P2Dying</c>（CrystallineSentinel.cs:1744-1785）。<c>CheckDead</c> 只登记 <c>KillRequested</c>，
    /// 换到本态由状态基类经返回值完成；本态自己在末帧把血清零再 <c>checkDead</c>（此时主控的死亡闸已放行）。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P2Dying, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP2DyingState : CrystallineSentinelStateBase
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P2Dying;

        public override CrystallineSentinelCommon Common => CrystallineSentinelCommon.PhaseTwo;

        /// <summary>演出态放宽超时兜底，免得被通用上限提前踢出去。</summary>
        protected override int TimeoutFrames => CrystallineSentinelDirector.AnimStateTimeoutFrames;

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            ctx.DeclareDamp(CrystallineSentinelDirector.DyingDamp);
            ctx.Invulnerable = true;

            Debris(ctx);
        }

        /// <summary>
        /// 崩解表现（纯本地）：碎岩爆点 3 帧一个、结晶闪光每帧一粒、50~80 帧之间每 2 帧崩一块 Gore。<br/>
        /// 旧代码这些全跑在两端（<c>Main.rand</c> 只喂粒子，不写位置），这里维持原样只加 <c>Main.dedServ</c> 门。旧 CrystallineSentinel.cs:1748-1777
        /// </summary>
        private void Debris(CrystallineSentinelContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;

            if (AttackTimer % CrystallineSentinelDirector.DyingBlastInterval == 0)
            {
                Vector2 offset = Main.rand.NextVector2Unit()
                    * Main.rand.NextFloat(CrystallineSentinelDirector.DyingBlastRadiusMin, CrystallineSentinelDirector.DyingBlastRadiusMax);
                CrystallineRockBlast blast = PRTLoader.NewParticle<CrystallineRockBlast>(npc.Center + offset, Vector2.Zero);
                if (blast != null)
                {
                    blast.Rotation = offset.ToRotation();
                }
            }

            Vector2 flashPos = npc.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(CrystallineSentinelDirector.DyingFlashRadius));
            Vector2 flashVel = npc.rotation.ToRotationVector2().RotateRandom(MathHelper.TwoPi)
                * Main.rand.NextFloat(CrystallineSentinelDirector.DyingFlashSpeedMin, CrystallineSentinelDirector.DyingFlashSpeedMax);
            PRTLoader.NewParticle<CrystallineFlashParticle>(flashPos,
                flashVel * Utils.Remap(AttackTimer, 0, CrystallineSentinelDirector.DyingFrames, CrystallineSentinelDirector.DyingFlashScaleStart, 1f));

            if (AttackTimer >= CrystallineSentinelDirector.DyingGoreStart && AttackTimer <= CrystallineSentinelDirector.DyingGoreEnd
                && AttackTimer % CrystallineSentinelDirector.DyingGoreInterval == 0)
            {
                SpawnGore(ctx, (int)(AttackTimer - CrystallineSentinelDirector.DyingGoreStart) / CrystallineSentinelDirector.DyingGoreInterval);
            }
        }

        /// <summary>一块碎片：帧号决定它朝哪个方位飞，并额外带一个上抛（死亡碎块不重组）。旧 CrystallineSentinel.cs:1764-1776</summary>
        private static void SpawnGore(CrystallineSentinelContext ctx, int step)
        {
            int frame = CrystallineGore1.TimerToFrame(step);
            float dir = Utils.Remap(frame, 0, CrystallineSentinelDirector.ExchangeGoreFrames, 0, MathHelper.TwoPi);
            float offset = Main.rand.NextFloat(CrystallineSentinelDirector.GoreOffsetMin, CrystallineSentinelDirector.GoreOffsetMax);
            float speed = Main.rand.NextFloat(CrystallineSentinelDirector.GoreSpeedMin, CrystallineSentinelDirector.GoreSpeedMax);
            Vector2 vel = (dir.ToRotationVector2() * speed) + new Vector2(0, CrystallineSentinelDirector.DyingGoreRiseY);

            CrystallineGore1 gore = PRTLoader.NewParticle<CrystallineGore1>(ctx.Npc.Center + (dir.ToRotationVector2() * offset), vel);
            if (gore == null)
            {
                return;
            }

            gore.FollowNPCIndex = ctx.Npc.whoAmI;
            gore.Frame.Y = frame;
            gore.TimeToRebuild = CrystallineSentinelDirector.DyingGoreNoRebuild;
            gore.Dir = dir;
            gore.Offset = offset;
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (AttackTimer + 1 > CrystallineSentinelDirector.DyingFrames)
            {
                NPC npc = ctx.Npc;
                npc.life = 0;
                npc.dontTakeDamage = false;
                npc.checkDead();
            }

            return null;
        }
    }
}
