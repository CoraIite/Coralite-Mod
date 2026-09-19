using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 一阶段 → 二阶段转阶段演出：拉镜头蓄力 → 炸开一次 → 再炸开一次（放雾）→ 打标题卡 → 交给二阶段。<br/>
    /// 节拍结构：蓄力 80 帧 / 第一次炸开到第 140 帧 / 第二次炸开 100 帧 / 标题 120 帧后淡出。<br/>
    /// 全程无敌（转阶段不该被打断），没有伤害窗，所以没有"公平阀"要求。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.exchange_P1_P2, typeof(NightmarePlanteraContext))]
    internal sealed class NPExchangeP1P2State : NightmarePlanteraStateBase
    {
        private enum Beat
        {
            /// <summary>蓄力，镜头拉向自身</summary>
            Charge,
            /// <summary>第一次炸开</summary>
            Burst1,
            /// <summary>第二次炸开，放大量雾</summary>
            Burst2,
            /// <summary>标题卡</summary>
            Title,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.exchange_P1_P2;

        protected override int TimeoutFrames => NightmarePlanteraDirector.CinematicTimeoutFrames;

        private Beat CurrentBeat => (Beat)BeatIndex;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);
            ctx.Boss.PreparePhase1Exchange();
        }

        public override void OnExit(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnExit(machine, ctx);
            // 演出被任何理由打断（脱战、天亮、秒杀）时标题卡也要收掉，否则会一直挂在屏幕上。
            ctx.Boss.StopNameCard();
        }

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            ctx.Boss.UpdateFrameNormally();
            ctx.Invulnerable = true;

            switch (CurrentBeat)
            {
                case Beat.Charge:
                    ChargeBeat(ctx);
                    // 旧代码这里只 State++ 不清 Timer，第一次炸开的长度是“到第 140 帧”而不是“140 帧”；
                    // 换拍必须保留这个共享计时，否则这一段会凭空长一倍（Phase.Exchange_P1_P2.cs:47-64）。
                    if (Timer > NightmarePlanteraDirector.ExchangeChargeFrames)
                    {
                        BeatIndex = (int)Beat.Burst1;
                        ctx.MarkDecision();
                        FirstBurst(ctx);
                    }

                    break;
                case Beat.Burst1:
                    Burst1Beat(ctx);
                    if (Timer > NightmarePlanteraDirector.ExchangeBurst1Frames)
                    {
                        SwitchBeat(ctx, (int)Beat.Burst2);
                        SecondBurst(ctx);
                    }

                    break;
                case Beat.Burst2:
                    Burst2Beat(ctx);
                    if (Timer > NightmarePlanteraDirector.ExchangeBurst2Frames)
                    {
                        SwitchBeat(ctx, (int)Beat.Title);
                        OpenTitle(ctx);
                    }

                    break;
                default:
                    TitleBeat(ctx);
                    break;
            }
        }

        private static void ChargeBeat(NightmarePlanteraContext ctx)
        {
            ctx.DeclareDamp(NightmarePlanteraDirector.ExchangeDamp);

            if (Main.dedServ || !Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera camera) || camera.factor >= 1)
            {
                return;
            }

            camera.useScreenMove = true;
            camera.factor += NightmarePlanteraDirector.ExchangeCameraStep;
            if (camera.factor > 1)
            {
                camera.factor = 1;
            }
        }

        /// <summary>第一次炸开：扭曲圈外扩，星尘与黑雾。</summary>
        private void Burst1Beat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            GrowWarp(boss);

            if (Main.dedServ)
            {
                return;
            }

            ctx.Npc.rotation += Main.rand.NextFloat(-0.2f, 0.3f);
            Vector2 dir = Helper.NextVec2Dir();
            Dust dust = Dust.NewDustPerfect(ctx.Npc.Center + (dir * Main.rand.NextFloat(64f)), ModContent.DustType<NightmareDust>(),
                dir * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 2f));
            dust.noGravity = true;

            if (Timer % NightmarePlanteraDirector.ExchangeDustInterval != 0)
            {
                return;
            }

            dir = Helper.NextVec2Dir();
            dust = Dust.NewDustPerfect(ctx.Npc.Center + (dir * Main.rand.NextFloat(64f)), ModContent.DustType<NightmareStar>(),
                dir * Main.rand.NextFloat(8f, 16f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 4f));
            dust.rotation = dir.ToRotation() + MathHelper.PiOver2;

            dir = Helper.NextVec2Dir();
            Dust.NewDustPerfect(ctx.Npc.Center + (dir * Main.rand.NextFloat(64f)), DustID.VilePowder,
                dir * Main.rand.NextFloat(8f, 16f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 1.3f));
        }

        /// <summary>第二次炸开：持续的爆炸音与成片浓雾。</summary>
        private void Burst2Beat(NightmarePlanteraContext ctx)
        {
            GrowWarp(ctx.Boss);

            if (Main.dedServ)
            {
                return;
            }

            if (Timer % NightmarePlanteraDirector.ExchangeSoundInterval == 0)
            {
                Helper.PlayPitched(CoraliteSoundID.FireBallExplosion_Item74, ctx.Npc.Center, volumeAdjust: -0.4f, pitchAdjust: -0.4f);
            }

            for (int i = 0; i < NightmarePlanteraDirector.ExchangeFogPerFrame; i++)
            {
                Color color = Main.rand.Next(0, 2) switch
                {
                    0 => new Color(110, 68, 200),
                    _ => new Color(122, 110, 134)
                };

                PRTLoader.NewParticle(ctx.Npc.Center + Main.rand.NextVector2Circular(64, 64), Helper.NextVec2Dir(6, 36f),
                    CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 3f));
            }
        }

        /// <summary>标题卡：镜头回位、字号涨满、第 30 帧点亮天空，120 帧后淡出。</summary>
        private void TitleBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            ctx.DeclareDamp(0f);

            if (boss.nameScale < NightmarePlanteraDirector.ExchangeNameScaleMax)
            {
                boss.nameScale += NightmarePlanteraDirector.ExchangeNameScaleStep;
            }

            // 淡出两端同跑：收招条件读 nameAlpha，服务器上不推进就会卡在演出里直到超时兜底。
            if (Timer >= NightmarePlanteraDirector.ExchangeTitleFrames)
            {
                boss.nameAlpha -= NightmarePlanteraDirector.ExchangeNameAlphaStep;
                if (boss.nameAlpha < NightmarePlanteraDirector.ExchangeNameAlphaEnd)
                {
                    boss.StopNameCard();
                }
            }

            if (Main.dedServ)
            {
                return;
            }

            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera camera) && camera.factor > 0)
            {
                camera.factor -= NightmarePlanteraDirector.ExchangeCameraStep;
                if (camera.factor < 0)
                {
                    camera.factor = 0;
                    camera.useScreenMove = false;
                }
            }

            if (Timer == NightmarePlanteraDirector.ExchangeSkyFrame && !SkyManager.Instance["NightmareSky"].IsActive())
            {
                SkyManager.Instance.Activate("NightmareSky");
                ((NightmareSky)SkyManager.Instance["NightmareSky"]).color = NightmarePlantera.lightPurple;
            }

            ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = NightmarePlanteraDirector.SkyTimeleft;
        }

        /// <summary>扭曲圈外扩到上限后自己关掉。</summary>
        private static void GrowWarp(NightmarePlantera boss)
        {
            boss.warpScale += NightmarePlanteraDirector.ExchangeWarpStep;
            if (boss.warpScale > NightmarePlanteraDirector.ExchangeWarpMax)
            {
                boss.canDrawWarp = false;
                boss.warpScale = 0;
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            // 换拍与演出都在 SharedUpdate 里按 Timer 确定性推进（客户端也要看到爆炸与标题）；
            // 这里只剩"演出播完了 → 交给二阶段"这一个换态决定。
            if (CurrentBeat != Beat.Title
                || Timer < NightmarePlanteraDirector.ExchangeTitleFrames
                || ctx.Boss.nameAlpha >= NightmarePlanteraDirector.ExchangeNameAlphaEnd)
            {
                return null;
            }

            ctx.Boss.OnExchangeToP2();
            return Create(NightmarePlanteraStateId.dream_P2);
        }

        private static void FirstBurst(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.canDrawWarp = true;
            boss.warpScale = 0;

            if (Main.dedServ)
            {
                return;
            }

            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera camera))
            {
                camera.useShake = true;
                camera.shakeLevel = NightmarePlanteraDirector.ExchangeShake1;
                camera.shakeDelay = NightmarePlanteraDirector.ExchangeShake1Delay;
            }

            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, ctx.Npc.Center, pitch: -0.5f);
            Helper.PlayPitched(CoraliteSoundID.EmpressOfLight_Dash_Item160, ctx.Npc.Center, volumeAdjust: -0.2f, pitchAdjust: -0.75f);
        }

        private static void SecondBurst(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.canDrawWarp = true;
            boss.warpScale = 0;

            if (Main.dedServ)
            {
                return;
            }

            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, ctx.Npc.Center, pitch: -0.5f);
            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera camera))
            {
                camera.shakeLevel = NightmarePlanteraDirector.ExchangeShake2;
                camera.shakeDelay = NightmarePlanteraDirector.ExchangeShake2Delay;
                camera.useShake = true;
            }
        }

        private static void OpenTitle(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            ctx.Npc.velocity *= 0;
            ctx.Npc.frame.Y = 1;
            boss.StartNameCard();

            if (Main.dedServ)
            {
                return;
            }

            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, ctx.Npc.Center, pitch: -0.5f);
            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera camera))
            {
                camera.useShake = false;
            }

            //Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/NightmarePlantera");
            boss.Music = MusicID.OtherworldPlantera; //把音乐再打开
        }
    }
}
