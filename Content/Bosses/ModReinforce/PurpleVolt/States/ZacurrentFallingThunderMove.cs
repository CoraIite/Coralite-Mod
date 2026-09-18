using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 落雷（连段收尾件，本 boss 唯一会隐身的招）：<br/>
    /// Chase —— 把横向距离调到 400~600 px。<br/>
    /// Rise —— 先下沉 10 帧蓄势，再以 -50 px/f 冲上天，20 帧里淡出到完全隐形（这期间无敌）。<br/>
    /// Aim —— 悬在玩家头顶 700 px 外跟踪 200 帧，落点每帧朝玩家推进 20 px；
    /// <b>最后 45 帧停止跟踪</b>，落点由指示粒子标死——这就是全招的逃生窗。期间还每 45 / 60 帧丢一发骚扰落雷逼玩家动起来。<br/>
    /// Smash —— 12 帧砸到落点，同时三道主落雷横向铺开 500 px；落地烟雾 30 帧后收招。<br/>
    /// 悬顶段整段都在写 <c>NPC.Center</c>（瞬移量远超 160 px），客户端由基座纠偏器直接认服务端位置（C7）。<br/>
    /// 旧 <c>ZacurrentDragon.FallingThunder</c>（AI.FallingThunder.cs:15-258）。
    /// </summary>
    internal static class ZacurrentFallingThunderMove
    {
        private enum Beat
        {
            /// <summary>调整横向距离（旧 SonState 0）</summary>
            Chase = 0,
            /// <summary>隐身升空（旧 1）</summary>
            Rise = 1,
            /// <summary>悬顶跟踪落点（旧 2）</summary>
            Aim = 2,
            /// <summary>砸落 + 落地（旧 3）</summary>
            Smash = 3,
        }

        /// <summary>落点（存在 <see cref="ZacurrentDragonContext.Recorder"/> / <see cref="ZacurrentDragonContext.Recorder2"/>，随热字段过线）。</summary>
        private static Vector2 MarkPos(ZacurrentDragonContext ctx) => new Vector2(ctx.Recorder, ctx.Recorder2);

        /// <summary>返回 true 表示整招结束。</summary>
        public static bool Run(ZacurrentDragonContext ctx)
        {
            switch ((Beat)(int)ctx.SonState)
            {
                case Beat.Rise:
                    UpdateRise(ctx);
                    return false;
                case Beat.Aim:
                    UpdateAim(ctx);
                    return false;
                case Beat.Smash:
                    return UpdateSmash(ctx);
                default:
                    UpdateChase(ctx);
                    return false;
            }
        }

        private static void UpdateChase(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.QuickSetDirection();
            boss.GetLengthToTargetPos(ctx.Target.Center, out float xLength, out float yLength);

            // 太近就退、太远就追：这一招要的是一段横向距离，不是贴脸
            if (xLength < ZacurrentDirector.FallingHoldNearX)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.X, -npc.direction, ZacurrentDirector.FallingChaseSpeedX,
                    ZacurrentDirector.FallingChaseAccelX, ZacurrentDirector.FallingChaseTurnX, ZacurrentDirector.BallChaseDamp);
            }
            else if (xLength > ZacurrentDirector.FallingHoldFarX)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.X, npc.direction, ZacurrentDirector.FallingChaseSpeedX,
                    ZacurrentDirector.FallingChaseAccelX, ZacurrentDirector.FallingChaseTurnX, ZacurrentDirector.BallChaseDamp);
            }
            else
            {
                npc.velocity.X *= ZacurrentDirector.BallChaseDamp;
            }

            if (npc.directionY < 0)
            {
                boss.FlyingUp(ZacurrentDirector.FallingRiseAccel, ZacurrentDirector.FallingRiseMax, ZacurrentDirector.FallingRiseDamp);
            }
            else if (yLength > ZacurrentDirector.BallChaseDeadZoneY)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.Y, npc.directionY, ZacurrentDirector.FallingChaseSpeedY,
                    ZacurrentDirector.FallingChaseAccelY, ZacurrentDirector.FallingChaseTurnY, ZacurrentDirector.BallChaseDamp);
                boss.FlyingFrame();
            }
            else
            {
                npc.velocity.Y *= ZacurrentDirector.BallChaseDamp;
                boss.FlyingFrame();
            }

            boss.SetRotationNormally();
            ctx.DeclareDirect();

            ctx.Timer++;
            bool arrived = xLength > ZacurrentDirector.FallingArriveX && yLength < ZacurrentDirector.FallingArriveY
                && ctx.Timer > ZacurrentDirector.FallingArriveMinFrames;
            if (ctx.Timer <= ZacurrentDirector.FallingChaseTimeout && !arrived)
            {
                return;
            }

            ctx.SonState = (int)Beat.Rise;
            ctx.Timer = 0;
            boss.ResetAllOldCaches();
            boss.canDrawShadows = true;
            boss.shadowScale = ZacurrentDirector.FallingRiseShadowScale;
            boss.shadowAlpha = 1;
            npc.velocity = new Vector2(0, ZacurrentDirector.FallingPreDropSpeed);
            ctx.MarkDecision();
        }

        private static void UpdateRise(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();

            if (ctx.Timer < ZacurrentDirector.FallingPreDropFrames)
            {
                boss.FlyingFrame();
                npc.QuickSetDirection();
            }
            else if (ctx.Timer == ZacurrentDirector.FallingPreDropFrames)
            {
                npc.velocity = new Vector2(0, ZacurrentDirector.FallingRiseSpeed);
                npc.rotation = -MathHelper.PiOver2;
                boss.IsDashing = true;
            }
            else
            {
                boss.selfAlpha = boss.shadowAlpha = 1 - ((ctx.Timer - ZacurrentDirector.FallingPreDropFrames) / ZacurrentDirector.FallingFadeFrames);
            }

            ctx.DeclareDirect();

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.FallingVanishFrames)
            {
                return;
            }

            ctx.SonState = (int)Beat.Aim;
            ctx.Timer = 0;
            npc.velocity *= 0;
            boss.selfAlpha = boss.shadowAlpha = 0;
            boss.canDrawShadows = false;
            boss.IsDashing = false;
            ctx.MarkDecision();

            // 落点初值 = 玩家当前位置
            ctx.Recorder = ctx.Target.Center.X;
            ctx.Recorder2 = ctx.Target.Center.Y;
        }

        private static void UpdateAim(ZacurrentDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            int trackStop = ZacurrentDirector.FallingAimFrames - ZacurrentDirector.FallingTrackStopLead;

            // 隐身期间不可攻击（原版 SyncNPC 不同步 dontTakeDamage，只能每帧两端声明）
            ctx.Invulnerable = true;

            if (ctx.Timer < trackStop)
            {
                SpawnHarass(ctx);
                TrackMark(ctx, trackStop);
            }
            else if (ctx.Timer == trackStop)
            {
                // 落点锁死的那一帧给个声音
                npc.Center = MarkPos(ctx) + new Vector2(0, -ZacurrentDirector.FallingUpLength);
                if (!VaultUtils.isServer)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, MarkPos(ctx));
                    MarkDust(ctx);
                }
            }
            else
            {
                npc.Center = MarkPos(ctx) + new Vector2(0, -ZacurrentDirector.FallingUpLength);
                if (!VaultUtils.isServer && Main.rand.NextBool())
                {
                    // 指示圈越张越大 = 倒计时
                    float radius = ZacurrentDirector.FallingMarkRadius
                        + (ZacurrentDirector.FallingMarkRadiusGrow * (ctx.Timer - trackStop) / (ZacurrentDirector.FallingAimFrames - trackStop));
                    ElectricParticle_PurpleFollow.Spawn(MarkPos(ctx), Main.rand.NextVector2Circular(radius, radius),
                        () => MarkPos(ctx), Main.rand.NextFloat(0.5f, 0.75f));
                }
            }

            ctx.DeclareDirect();

            ctx.Timer++;
            if (ctx.Timer <= ZacurrentDirector.FallingAimFrames)
            {
                return;
            }

            Smash(ctx);
        }

        /// <summary>骚扰落雷：逼玩家在锁定期间不能站着不动。旧 AI.FallingThunder.cs:108-131</summary>
        private static void SpawnHarass(ZacurrentDragonContext ctx)
        {
            NPC npc = ctx.Npc;

            if (ctx.Timer % ZacurrentDirector.FallingHarassInterval == 0)
            {
                Vector2 pos = ctx.Target.Center;
                if (ctx.Target.velocity.Length() > ZacurrentDirector.BallPredictSpeedGate)
                {
                    pos += ctx.Target.velocity.SafeNormalize(Vector2.Zero) * ZacurrentDirector.FallingHarassPredict;
                }

                // 左右交替、越往后偏得越远
                pos += new Vector2((ctx.Timer / ZacurrentDirector.FallingHarassInterval % 2 == 0 ? -1 : 1) * ctx.Timer * ZacurrentDirector.FallingHarassSwingPerFrame,
                    ZacurrentDirector.FallingHarassDropY);
                pos += Main.rand.NextVector2CircularEdge(ZacurrentDirector.FallingHarassJitter, ZacurrentDirector.FallingHarassJitter);

                npc.NewProjectileInAI_Server<PurpleSmallThunderFall>(pos, Vector2.Zero, ZacurrentDirector.FallingHarassDamage(), 0
                    , npc.target, ZacurrentDirector.FallingHarassLife, npc.whoAmI, ZacurrentDirector.FallingHarassAi2);

                if (!VaultUtils.isServer)
                {
                    Helper.PlayPitched(CoraliteSoundID.Ding_Item4, npc.Center, pitch: 0.3f);
                }
            }

            if (ctx.Timer % ZacurrentDirector.FallingHarassDirectInterval != 0)
            {
                return;
            }

            npc.NewProjectileInAI_Server<PurpleSmallThunderFall>(ctx.Target.Center, Vector2.Zero, ZacurrentDirector.FallingHarassDamage(), 0
                , npc.target, ZacurrentDirector.FallingHarassDirectLife, npc.whoAmI, ZacurrentDirector.FallingHarassAi2);

            if (!VaultUtils.isServer)
            {
                Helper.PlayPitched(CoraliteSoundID.Ding_Item4, npc.Center, pitch: 0.3f);
            }
        }

        /// <summary>落点跟踪：带玩家速度预判，每帧最多推进 20 px，所以一直跑就能把落点甩在身后。旧 AI.FallingThunder.cs:133-157</summary>
        private static void TrackMark(ZacurrentDragonContext ctx, int trackStop)
        {
            NPC npc = ctx.Npc;

            float ramp = ctx.Timer / ZacurrentDirector.FallingTrackRampFrames;
            Vector2 wanted = ctx.Target.Center + (ctx.Target.velocity * ZacurrentDirector.FallingTrackPredictFrames * ramp);
            Vector2 mark = MarkPos(ctx).MoveTowards(wanted, ZacurrentDirector.FallingTrackStep);
            ctx.Recorder = mark.X;
            ctx.Recorder2 = mark.Y;
            npc.Center = mark + new Vector2(0, -ZacurrentDirector.FallingUpLength);

            // 前半段不给标记，让玩家先动起来
            if (ctx.Timer <= ZacurrentDirector.FallingAimFrames / 2 || VaultUtils.isServer)
            {
                return;
            }

            MarkDust(ctx);
            if (Main.rand.NextBool())
            {
                ElectricParticle_PurpleFollow.Spawn(mark, Main.rand.NextVector2Circular(ZacurrentDirector.FallingMarkRadius, ZacurrentDirector.FallingMarkRadius),
                    () => MarkPos(ctx), Main.rand.NextFloat(0.5f, 0.75f));
            }
        }

        private static void MarkDust(ZacurrentDragonContext ctx)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(MarkPos(ctx), DustID.PortalBoltTrail, Helper.NextVec2Dir(2, 4),
                    newColor: ZacurrentDragon.ZacurrentPurple,
                    Scale: Main.rand.NextFloat(ZacurrentDirector.GravitationDustScaleMin, ZacurrentDirector.GravitationDustScaleMax));
                dust.noGravity = true;
            }
        }

        private static void Smash(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;
            Vector2 mark = MarkPos(ctx);

            ctx.SonState = (int)Beat.Smash;
            ctx.Timer = 0;
            ctx.Invulnerable = false;
            ctx.MarkDecision();

            npc.velocity = new Vector2(0, ZacurrentDirector.FallingUpLength / (float)ZacurrentDirector.FallingSmashFrames);
            npc.rotation = npc.velocity.ToRotation();
            npc.QuickSetDirection();
            boss.ResetAllOldCaches();

            if (!VaultUtils.isClient)
            {
                int damage = ZacurrentDirector.FallingBoltDamage();
                for (int i = 0; i < ZacurrentDirector.FallingBoltCount; i++)
                {
                    Vector2 from = npc.Center + new Vector2(ZacurrentDirector.FallingBoltOffsetX + (i * ZacurrentDirector.FallingBoltSpreadX / ZacurrentDirector.FallingBoltCount),
                        Main.rand.Next(ZacurrentDirector.FallingBoltOffsetYMin, ZacurrentDirector.FallingBoltOffsetYMax));
                    npc.NewProjectileDirectInAI<PurpleThunderFalling>(from, mark + new Vector2(0, ZacurrentDirector.FallingBoltAimBelow), damage, 0, npc.target
                        , ZacurrentDirector.FallingSmashFrames + ZacurrentDirector.FallingBoltLifeGain, npc.whoAmI, ZacurrentDirector.FallingBoltAi2);
                }
            }

            if (!VaultUtils.isServer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, npc.Center);
                SoundEngine.PlaySound(CoraliteSoundID.Thunder, npc.Center);
                ZacurrentDragon.SetBackgroundLight(ZacurrentDirector.FallingSkyLightDash, ZacurrentDirector.FallingSmashFrames);
            }

            boss.IsDashing = true;
            boss.canDrawShadows = true;
            boss.currentSurrounding = true;
        }

        private static bool UpdateSmash(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            boss.UpdateAllOldCaches();

            if (ctx.Timer < ZacurrentDirector.FallingSmashFrames)
            {
                boss.selfAlpha += 1 / (float)ZacurrentDirector.FallingSmashFrames;
                boss.shadowAlpha = boss.selfAlpha;
            }
            else if (ctx.Timer == ZacurrentDirector.FallingSmashFrames)
            {
                boss.IsDashing = false;
                npc.velocity *= 0;
                npc.QuickSetDirection();
                boss.TurnToNoRot(1);
                npc.frame.Y = 0;

                if (!VaultUtils.isServer)
                {
                    ZacurrentDragon.SetBackgroundLight(ZacurrentDirector.FallingSkyLightLand, ZacurrentDirector.FallingSkyLandFade);
                    PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, Vector2.UnitY * ZacurrentDirector.FallingLandPunchY,
                        ZacurrentDirector.FallingLandShakeStrength, ZacurrentDirector.FallingLandShakeVibration, ZacurrentDirector.FallingLandShakeFrames, ZacurrentDirector.ShakeFalloffDistance);
                    Main.instance.CameraModifiers.Add(modifier);
                }
            }
            else if (ctx.Timer < ZacurrentDirector.FallingSmashFrames + ZacurrentDirector.FallingLandSmokeFrames)
            {
                LandSmoke(ctx);

                float factor = Helper.SqrtEase((ctx.Timer - ZacurrentDirector.FallingSmashFrames) / ZacurrentDirector.FallingLandSmokeFrames);
                boss.shadowScale = Helper.Lerp(1f, ZacurrentDirector.FallingLandShadowScaleMax, factor);
                boss.shadowAlpha = Helper.Lerp(1f, 0f, factor);
            }
            else if (ctx.Timer < ZacurrentDirector.FallingSmashFrames + ZacurrentDirector.FallingLandSmokeFrames + ZacurrentDirector.FallingRecoverFrames)
            {
                boss.FlyingFrame();
            }
            else
            {
                return true;
            }

            ctx.DeclareDirect();
            ctx.Timer++;
            return false;
        }

        private static void LandSmoke(ZacurrentDragonContext ctx)
        {
            if (VaultUtils.isServer || ctx.Timer % ZacurrentDirector.FallingSmokeInterval != 0)
            {
                return;
            }

            NPC npc = ctx.Npc;
            PRTLoader.NewParticle(npc.Center + Main.rand.NextVector2Circular(npc.width / 5, npc.height / 5),
                Vector2.Zero, CoraliteContent.ParticleType<BigFog>(), ZacurrentDragon.ZacurrentPurple * Main.rand.NextFloat(0.5f, 0.8f),
                Main.rand.NextFloat(1.5f, 2f));

            Vector2 pos = npc.Center + Main.rand.NextVector2Circular(npc.width * 0.8f, npc.height * 0.8f);
            Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero,
                newColor: ZacurrentDragon.ZacurrentPurple, Scale: Main.rand.NextFloat(0.1f, 0.3f));
        }
    }
}
