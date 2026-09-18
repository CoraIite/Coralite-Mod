using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 紫伏形态切换（紫电攒满时由 hub 强制进入，不参与随机选招）：<br/>
    /// Inhale —— 收翅两段"吸气"（17 + 9 帧），残影各从大缩到 1，做出两次蓄势。<br/>
    /// Roar —— 吼叫 60 帧，红电环 40 帧内从 80 px 铺到 1400 px 扫过全屏，收尾再留 25 帧。<br/>
    /// 这一段全程没有攻击判定，是明确的转阶段演出窗口；结束后权威端点亮紫伏旗。<br/>
    /// 旧 <c>ZacurrentDragon.PurpleVoltExchange</c>（AI.PurpleVoltChange.cs:12-114）与旧壳 ZacurrentDragon.States.cs:149-169。
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.PurpleVoltExchange, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentPurpleVoltExchangeState : ZacurrentAttackState
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.PurpleVoltExchange;

        private enum Beat
        {
            /// <summary>收翅两段蓄力（旧 SonState 0）</summary>
            Inhale = 0,
            /// <summary>吼叫铺电环（旧 1）</summary>
            Roar = 1,
        }

        protected override bool RunAttack(ZacurrentDragonContext ctx)
        {
            if ((Beat)(int)ctx.SonState == Beat.Roar)
            {
                return UpdateRoar(ctx);
            }

            UpdateInhale(ctx);
            return false;
        }

        private static void UpdateInhale(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.velocity *= ZacurrentDirector.ExchangeDamp;
            ctx.DeclareDirect();
            npc.QuickSetDirection();
            boss.TurnToNoRot();

            // 起手门：翅膀没抬到位就只走帧图
            if (ctx.Timer < 2 && npc.frame.Y != ZacurrentDirector.BallSwingFrameY)
            {
                boss.FlyingFrame();
                return;
            }

            if (ctx.Timer < 2)
            {
                boss.canDrawShadows = true;
                boss.ResetAllOldCaches();
            }

            boss.UpdateAllOldCaches();

            // 两段"吸气"：残影各从大缩回 1，看起来像连吸两口
            if (ctx.Timer < ZacurrentDirector.ExchangeFirstChargeTime)
            {
                float factor = ctx.Timer / ZacurrentDirector.ExchangeFirstChargeTime;
                boss.shadowScale = Helper.Lerp(ZacurrentDirector.ExchangeShadowScaleFirst, 1f, factor);
                boss.shadowAlpha = Helper.Lerp(0f, 1f, factor);
            }
            else if (ctx.Timer < ZacurrentDirector.ExchangeFirstChargeTime + ZacurrentDirector.ExchangeSecondChargeTime)
            {
                float factor = (ctx.Timer - ZacurrentDirector.ExchangeFirstChargeTime) / ZacurrentDirector.ExchangeSecondChargeTime;
                boss.shadowScale = Helper.Lerp(ZacurrentDirector.ExchangeShadowScaleSecond, 1f, factor);
                boss.shadowAlpha = Helper.Lerp(0f, 1f, factor);
            }

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.ExchangeFirstChargeTime + ZacurrentDirector.ExchangeSecondChargeTime)
            {
                return;
            }

            ctx.SonState = (int)Beat.Roar;
            ctx.Timer = 0;
            npc.frame.Y = 0;
            boss.OpenMouse = true;
            boss.shadowScale = ZacurrentDirector.ExchangeRoarShadowScale;
            ctx.MarkDecision();

            if (!VaultUtils.isServer)
            {
                Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, npc.Center, pitch: ZacurrentDirector.ElectricOrbPitch);
                SoundEngine.PlaySound(CoraliteSoundID.Roar, npc.Center);
            }

            if (!VaultUtils.isClient)
            {
                npc.NewProjectileDirectInAI<ZacurrentExchangeAnmi>(npc.Center, Vector2.Zero, 1, 0, npc.target
                    , ZacurrentDirector.ExchangeBurstTime, npc.whoAmI);
            }
        }

        private static bool UpdateRoar(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();

            if (!VaultUtils.isServer)
            {
                if (ctx.Timer < ZacurrentDirector.ExchangeBurstTime)
                {
                    float length = Helper.Lerp(ZacurrentDirector.ExchangeBurstRadiusMin, ZacurrentDirector.ExchangeBurstRadiusMax,
                        ctx.Timer / ZacurrentDirector.ExchangeBurstTime);
                    for (int i = 0; i < ZacurrentDirector.ExchangeBurstPerFrame; i++)
                    {
                        PRTLoader.NewParticle(npc.Center + Main.rand.NextVector2CircularEdge(length, length),
                            Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Red>(),
                            Scale: Main.rand.NextFloat(ZacurrentDirector.BurstParticleScaleMin, ZacurrentDirector.BurstParticleScaleMax));
                    }
                }

                if (ctx.Timer < ZacurrentDirector.ExchangeRoaringTime)
                {
                    float factor = ctx.Timer / ZacurrentDirector.ExchangeRoaringTime;
                    boss.shadowScale = Helper.Lerp(1f, ZacurrentDirector.ExchangeRoarShadowScaleMax, factor);
                    boss.shadowAlpha = Helper.Lerp(1f, 0f, factor);

                    Vector2 pos = npc.Center + (npc.rotation.ToRotationVector2() * ZacurrentDirector.MouthDistance);
                    if ((int)ctx.Timer % ZacurrentDirector.RoaringWaveInterval == 0)
                    {
                        RoaringWave wave = PRTLoader.NewParticle<RoaringWave>(pos, Vector2.Zero, ZacurrentDragon.ZacurrentPurple, ZacurrentDirector.RoaringWaveScale);
                        wave.ScaleMul = ZacurrentDirector.RoaringWaveScaleMul;
                    }

                    if ((int)ctx.Timer % ZacurrentDirector.RoaringLineInterval == 0)
                    {
                        PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, ZacurrentDirector.RoaringWaveScale);
                    }
                }
            }

            ctx.Timer++;
            return ctx.Timer > ZacurrentDirector.ExchangeRoaringTime + ZacurrentDirector.ExchangeEndDelay;
        }

        protected override IVaultState<ZacurrentDragonContext> AuthorityUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            if (!ctx.AttackFinished)
            {
                return null;
            }

            // 演出结束才真正进入紫伏形态（紫电计数在 hub 判定时已满）
            ctx.Boss.PurpleVolt = true;
            ctx.MarkDecision();
            return EndAttack(ctx);
        }
    }
}
