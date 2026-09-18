using Coralite.Content.NPCs.Crystalline.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 二阶段休息（每 6 手强制一次）：速度对折后缓缓下坠，一边冒修理火花一边越来越抖，
    /// 修满（4/3/2.5/1 秒，难度越高越快）时炸一圈闪光并回到悬浮，且额外多给 120 帧喘息。<br/>
    /// 旧 <c>P2Rest</c>（CrystallineSentinel.cs:1693-1742）。改道在 hub 的 <c>ToState</c> 里做，这里只负责演出本身。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P2Rest, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP2RestState : CrystallineSentinelStateBase
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P2Rest;

        public override CrystallineSentinelCommon Common => CrystallineSentinelCommon.PhaseTwo;

        /// <summary>旧代码在起手判定之后才 <c>Timer++</c>，之后的抖动系数与结束判定读的都是自增后的值，这里对齐。</summary>
        private float PostTimer => AttackTimer + 1;

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.SetText(CrystallineSentinelTextType.Broken, CrystallineSentinelDirector.RestTextFrames);

            if (AtFrame(0))
            {
                npc.velocity *= CrystallineSentinelDirector.RestLaunchDamp;
            }

            if (npc.velocity.Y < CrystallineSentinelDirector.RestLimitY)
            {
                npc.velocity.Y += CrystallineSentinelDirector.RestAccelY;
            }

            npc.velocity.X *= CrystallineSentinelDirector.RestDampX;

            // 修理进度：越接近修好抖得越凶（普通难度满 4 秒，高难度提前结束所以抖不满）
            float factor = Utils.Remap(PostTimer, 0, CrystallineSentinelDirector.RestFactorRamp, 0f, 1f);
            ctx.RestFactor = factor;

            // 旧代码这里是两次 Main.rand（幅度 + 正负），两端各掷各的会让位置分叉；换成只吃已同步量的确定性噪声，幅度不变（C1/C3）
            float magnitude = MathHelper.Lerp(CrystallineSentinelDirector.RestShakeMin, CrystallineSentinelDirector.RestShakeMax, Noise(ctx, 31));
            float sign = Noise(ctx, 57) < 0.5f ? -1f : 1f;
            npc.velocity.X += magnitude * (factor + CrystallineSentinelDirector.RestShakeBias) * sign;

            ctx.DeclareDirect();

            if (PostTimer % CrystallineSentinelDirector.RestDustInterval == 0)
            {
                RepairSparks(ctx);
            }
        }

        /// <summary>修理火花（纯本地）：一粒结晶冲击尘 + 一簇上飘烟尘。旧 CrystallineSentinel.cs:1711-1727</summary>
        private static void RepairSparks(CrystallineSentinelContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            float rot = Main.rand.NextFloat(MathHelper.TwoPi)
                + Main.rand.NextFloat(-CrystallineSentinelDirector.RestDustSpread, CrystallineSentinelDirector.RestDustSpread);
            Vector2 pos = npc.Center + (rot.ToRotationVector2()
                * Main.rand.NextFloat(CrystallineSentinelDirector.RestDustRadiusMin, CrystallineSentinelDirector.RestDustRadiusMax));

            Dust impact = Dust.NewDustPerfect(pos, ModContent.DustType<CrystallineImpact>(), Vector2.Zero, Scale: Main.rand.NextFloat(1f, 1.5f));
            impact.rotation = rot;

            for (int k = 0; k < CrystallineSentinelDirector.RestSmokeCount; k++)
            {
                Vector2 vel = new(Main.rand.NextFloat(-CrystallineSentinelDirector.RestSmokeSpeedX, CrystallineSentinelDirector.RestSmokeSpeedX),
                    -Main.rand.NextFloat(CrystallineSentinelDirector.RestSmokeSpeedY));
                Dust smoke = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(npc.getRect()), DustID.Smoke, vel,
                    CrystallineSentinelDirector.RestSmokeAlpha, Scale: Main.rand.NextFloat(1f, 2f));
                smoke.noGravity = true;
            }
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (PostTimer > CrystallineSentinelDirector.RestFrames())
            {
                FinishBurst(ctx);
                return ReturnToIdle(ctx, IdleFramesOnFinish);
            }

            return null;
        }

        /// <summary>修好的环形爆发（纯本地）。旧 CrystallineSentinel.cs:1731-1738</summary>
        private static void FinishBurst(CrystallineSentinelContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            int count = CrystallineSentinelDirector.RestFinishParticleCount;
            for (int i = 0; i < count; i++)
            {
                Vector2 dir = (MathHelper.TwoPi / count * i).ToRotationVector2();
                Vector2 vel = dir.RotateByRandom(-CrystallineSentinelDirector.RestFinishSpread, CrystallineSentinelDirector.RestFinishSpread)
                    * Main.rand.NextFloat(CrystallineSentinelDirector.RestFinishSpeedMin, CrystallineSentinelDirector.RestFinishSpeedMax);
                Vector2 pos = ctx.Npc.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0f, CrystallineSentinelDirector.RestFinishRadius));
                PRTLoader.NewParticle<CrystallineFlashParticle>(pos, vel);
            }
        }

        /// <summary>休息完额外多给 60 帧喘息（旧 <c>SwitchStateP2(P2Idle, -120)</c>）。</summary>
        protected override int IdleFramesOnFinish => CrystallineSentinelDirector.P2IdleGapAfterRest;
    }
}
