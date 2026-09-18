using Coralite.Content.Bosses.ThunderveinDragon.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon.States
{
    /// <summary>
    /// 落雷：吼一声后升空隐身，在地面标出落点，再带着三道雷砸下来；二阶段最多连砸 3 次。<br/>
    /// 节拍：Chase（横向带宽 400~600 px）→ Roar（80 帧吼叫，是“它要升空了”的唯一预告）→ Ascend（30 帧升空 700 px 并淡出）
    /// → Aim（80 帧选点，落点尘与电粒子从散布 30 收到 90 = 倒计时）→ Smash（12 帧下砸 + 三道落雷）
    /// → 二阶段 1/2 概率再选一次点（上限 3 次）→ FinalSmash（落地 30 帧冒雾，再飞 50/25 帧收招）。<br/>
    /// 公平阀：落点在地面上被尘与电粒子标满 80 帧，散布收拢即倒计时；隐身期本体无敌但完全不出手。<br/>
    /// 旧 AI.FallingThunder.cs（P1 :14-270 / P2 :272-578）
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.FallingThunder, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinFallingThunderState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.FallingThunder;

        private enum Beat
        {
            /// <summary>拉开跑道（旧 SonState 0）</summary>
            Chase = 0,
            /// <summary>吼叫（旧 1）</summary>
            Roar = 1,
            /// <summary>升空隐身（旧 2）</summary>
            Ascend = 2,
            /// <summary>选落点（旧 3 / 5 / 7）</summary>
            Aim = 3,
            /// <summary>中途落雷，砸完回去再选点（旧 P2 的 4 / 6）</summary>
            MidSmash = 4,
            /// <summary>最后一次落雷 + 落地演出（旧 P1 的 4 / P2 的 8）</summary>
            FinalSmash = 5,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>锁定的落点（热字段 A / B；旧 localAI[0] Recorder、localAI[1] Recorder2）。</summary>
        private float aimX;
        private float aimY;

        /// <summary>已完成的选点次数（热字段 C）。</summary>
        private int aimCount;

        private bool strikeThisFrame;

        private Vector2 AimPos => new(aimX, aimY);

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            strikeThisFrame = false;

            switch (CurrentBeat)
            {
                case Beat.Roar:
                    UpdateRoar(ctx);
                    break;
                case Beat.Ascend:
                    UpdateAscend(ctx);
                    break;
                case Beat.Aim:
                    UpdateAim(ctx);
                    break;
                case Beat.MidSmash:
                    UpdateMidSmash(ctx);
                    break;
                case Beat.FinalSmash:
                    UpdateFinalSmash(ctx);
                    break;
                default:
                    UpdateChase(ctx);
                    break;
            }
        }

        private void UpdateChase(ThunderveinDragonContext ctx)
        {
            ctx.Npc.QuickSetDirection();
            ctx.DeclareChase(ctx.Target.Center, ThunderveinDirector.FallingChase);
            ctx.DeclareRotationNormal();

            ctx.Boss.GetLengthToTargetPos(ctx.Target.Center, out float xLength, out float yLength);
            bool ready = xLength > ThunderveinDirector.FallingChaseExitX
                && yLength < ThunderveinDirector.FallingChaseExitY
                && Timer > ThunderveinDirector.FallingChaseMinFrames;
            if (Timer <= ThunderveinDirector.FallingChaseFrames && !ready)
            {
                return;
            }

            // 二阶段进本招时清掉“短冲之前那一手”的记忆（沿用旧值 AI.FallingThunder.cs:315）
            if (ctx.Phase != 1)
            {
                ctx.StateBeforeSmallDash = 0;
            }

            ctx.Boss.DashFrame();
            ctx.Boss.ResetAllOldCaches();
            SwitchBeat(ctx, (int)Beat.Roar);
        }

        /// <summary>吼叫 80 帧，末尾以 8 px/f 下坠起跳。旧 :67-107</summary>
        private void UpdateRoar(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DrawShadows = true;
            ctx.ShadowScale = ThunderveinDirector.DashShadowScale;
            ctx.Boss.UpdateAllOldCaches();
            npc.QuickSetDirection();
            ctx.DeclareNoRot();
            ctx.DeclareDamp(ThunderveinDirector.FallingRoarDamp);

            if (T == 0 && npc.frame.Y != 4)
            {
                ctx.Boss.FlyingFrame();
                HoldTimer();
                return;
            }

            if (T == ThunderveinDirector.RoarCueFrame)
            {
                npc.frame.Y = 0;
                npc.frame.X = 1;
                npc.velocity *= 0;
                ctx.DeclareDirect();
                PlayRoarSound(ctx);
            }
            else if (T > ThunderveinDirector.RoarCueFrame && T < ThunderveinDirector.FallingRoarFxEnd)
            {
                RoarFx(ctx);
            }

            if (Timer <= ThunderveinDirector.FallingRoarFrames)
            {
                return;
            }

            npc.velocity = new Vector2(0, ThunderveinDirector.FallingDropSpeed);
            ctx.DeclareDirect();
            ctx.Boss.ResetAllOldCaches();
            SwitchBeat(ctx, (int)Beat.Ascend);
        }

        private static void PlayRoarSound(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, ctx.Npc.Center, pitch: ThunderveinDirector.RoarPitch);
            SoundEngine.PlaySound(CoraliteSoundID.Roar, ctx.Npc.Center);
        }

        /// <summary>升空：先坠 10 帧，第 10 帧 −45 px/f 拔起，10~30 帧淡出。旧 :108-141</summary>
        private void UpdateAscend(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DrawShadows = true;
            ctx.Boss.UpdateAllOldCaches();
            ctx.DeclareKeep();

            if (T < ThunderveinDirector.FallingDropFrames)
            {
                ctx.Boss.FlyingFrame();
                npc.QuickSetDirection();
            }
            else if (T == ThunderveinDirector.FallingDropFrames)
            {
                npc.velocity = new Vector2(0, ThunderveinDirector.FallingAscendSpeed);
                npc.rotation = -MathHelper.PiOver2;
                ctx.Boss.DashFrame();
                ctx.DeclareDirect();
            }
            else
            {
                float fade = 1f - ((T - ThunderveinDirector.FallingDropFrames) / ThunderveinDirector.FallingAscendFadeFrames);
                ctx.SelfAlpha = fade;
                ctx.ShadowAlpha = fade;
            }

            if (Timer <= ThunderveinDirector.FallingAscendFrames)
            {
                return;
            }

            npc.velocity *= 0;
            ctx.DeclareDirect();
            EnterAim(ctx);
        }

        /// <summary>进入选点：隐身、无敌，落点先取玩家当前位置。旧 :129-139</summary>
        private void EnterAim(ThunderveinDragonContext ctx)
        {
            ctx.SelfAlpha = 0f;
            ctx.ShadowAlpha = 0f;
            aimX = ctx.Target.Center.X;
            aimY = ctx.Target.Center.Y;
            SwitchBeat(ctx, (int)Beat.Aim);
        }

        /// <summary>
        /// 选点 80 帧：前 55 帧落点以 20 px/f 追向“玩家 + 速度预判”，之后锁死；本体始终悬在落点正上方 700 px。<br/>
        /// 落点上的尘与跟随电粒子就是全部预告，散布从 30 张到 90。旧 :143-223
        /// </summary>
        private void UpdateAim(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.Invulnerable = true;
            ctx.SelfAlpha = 0f;
            ctx.ShadowAlpha = 0f;
            ctx.DeclareDirect();

            if (T < ThunderveinDirector.FallingAimChaseFrames)
            {
                float lead = T / ThunderveinDirector.FallingAimPredictRamp;
                Vector2 wanted = ctx.Target.Center + (ctx.Target.velocity * ThunderveinDirector.FallingAimPredictFrames * lead);
                Vector2 moved = AimPos.MoveTowards(wanted, ThunderveinDirector.FallingAimMoveStep);
                aimX = moved.X;
                aimY = moved.Y;
                npc.Center = moved + new Vector2(0, -ThunderveinDirector.FallingUpLength);
                SpawnAimDust(moved);
                SpawnFollowParticle(moved, ThunderveinDirector.FallingAimFollowSpread);
            }
            else if (T == ThunderveinDirector.FallingAimChaseFrames)
            {
                npc.Center = AimPos + new Vector2(0, -ThunderveinDirector.FallingUpLength);
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, AimPos);
                }

                SpawnAimDust(AimPos);
            }
            else
            {
                npc.Center = AimPos + new Vector2(0, -ThunderveinDirector.FallingUpLength);
                float spread = ThunderveinDirector.FallingAimFollowSpread
                    + (ThunderveinDirector.FallingAimFollowSpreadGain * (T - ThunderveinDirector.FallingAimChaseFrames)
                        / (ThunderveinDirector.FallingAimFrames - ThunderveinDirector.FallingAimChaseFrames));
                SpawnFollowParticle(AimPos, spread);
            }

            if (Timer <= ThunderveinDirector.FallingAimFrames)
            {
                return;
            }

            aimCount++;
            npc.velocity = new Vector2(0, ThunderveinDirector.FallingUpLength / ThunderveinDirector.FallingSmashFrames);
            npc.rotation = npc.velocity.ToRotation();
            npc.QuickSetDirection();
            ctx.Boss.ResetAllOldCaches();
            strikeThisFrame = true;
            PlayStrikeCue(ctx);

            // 一阶段只砸一次；二阶段无条件掷骰（两端同序消费）后再看次数上限
            if (ctx.Phase == 1)
            {
                SwitchBeat(ctx, (int)Beat.FinalSmash);
                return;
            }

            bool again = ctx.AttackRandBool();
            SwitchBeat(ctx, again && aimCount < ThunderveinDirector.FallingMaxStrikes ? (int)Beat.MidSmash : (int)Beat.FinalSmash);
        }

        private static void SpawnAimDust(Vector2 pos)
        {
            if (Main.dedServ)
            {
                return;
            }

            for (int i = 0; i < ThunderveinDirector.FallingAimDustCount; i++)
            {
                Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail,
                    Helper.NextVec2Dir(ThunderveinDirector.FallingAimDustSpeedMin, ThunderveinDirector.FallingAimDustSpeedMax),
                    newColor: Coralite.ThunderveinYellow,
                    Scale: Main.rand.NextFloat(ThunderveinDirector.AimDustScaleMin, ThunderveinDirector.AimDustScaleMax));
                d.noGravity = true;
            }
        }

        private void SpawnFollowParticle(Vector2 pos, float spread)
        {
            if (Main.dedServ || !Main.rand.NextBool())
            {
                return;
            }

            ElectricParticle_Follow.Spawn(pos, Main.rand.NextVector2Circular(spread, spread), () => AimPos,
                Main.rand.NextFloat(ThunderveinDirector.FallingAimFollowScaleMin, ThunderveinDirector.FallingAimFollowScaleMax));
        }

        private static void PlayStrikeCue(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, ctx.Npc.Center);
            SoundEngine.PlaySound(CoraliteSoundID.Thunder, ctx.Npc.Center);
            ThunderveinDragon.SetBackgroundLight(ThunderveinDirector.FallingStrikeSkyLight, ThunderveinDirector.FallingSmashFrames);
        }

        /// <summary>中途落雷：淡入 → 落地闪一下 → 淡出 → 回去再选点。旧 :495-532（该拍旧代码先自增再判定，所以直接用 Timer）</summary>
        private void UpdateMidSmash(ThunderveinDragonContext ctx)
        {
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.Boss.UpdateAllOldCaches();
            ctx.DeclareKeep();

            int smash = ThunderveinDirector.FallingSmashFrames;
            if (Timer < smash)
            {
                ctx.SelfAlpha = Timer / (float)smash;
            }
            else if (Timer == smash)
            {
                PlayLandFlash(ctx, ThunderveinDirector.FallingMidShakeStrength, ThunderveinDirector.FallingMidShakeVibration,
                    ThunderveinDirector.FallingMidShakeFrames);
            }
            else if (Timer < smash * 2)
            {
                ctx.SelfAlpha = ((smash * 2) - 1 - Timer) / (float)smash;
            }
            else
            {
                ctx.Npc.velocity *= 0;
                ctx.DeclareDirect();
                EnterAim(ctx);
                return;
            }

            ctx.ShadowAlpha = ctx.SelfAlpha;
        }

        /// <summary>最后一次落雷：12 帧下砸 → 落地雾 30 帧 → 飞 50/25 帧收招。旧 :225-268 / :533-576</summary>
        private void UpdateFinalSmash(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.Boss.UpdateAllOldCaches();
            ctx.DeclareKeep();

            int smash = ThunderveinDirector.FallingSmashFrames;
            if (T < smash)
            {
                ctx.SelfAlpha = (T + 1) / (float)smash;
                ctx.ShadowAlpha = ctx.SelfAlpha;
            }
            else if (T == smash)
            {
                npc.velocity *= 0;
                ctx.DeclareDirect();
                npc.QuickSetDirection();
                ctx.DeclareNoRot(1f);
                npc.frame.Y = 0;
                npc.frame.X = 1;
                PlayLandFlash(ctx, ThunderveinDirector.FallingLandShakeStrength, ThunderveinDirector.FallingLandShakeVibration,
                    ThunderveinDirector.FallingLandShakeFrames);
            }
            else if (T < smash + ThunderveinDirector.FallingLandFogFrames)
            {
                if (T % ThunderveinDirector.FallingLandFogInterval == 0)
                {
                    SpawnLandFog(ctx);
                }

                BurstShadowEnvelope(ctx, (T - smash) / (float)ThunderveinDirector.FallingLandFogFrames, ThunderveinDirector.FallingLandShadowScaleTo);
            }
            else
            {
                ctx.Boss.FlyingFrame();
            }
        }

        /// <summary>落地瞬间的天空过曝 + 竖直震屏（纯本地）。</summary>
        private static void PlayLandFlash(ThunderveinDragonContext ctx, int strength, float vibration, int frames)
        {
            if (Main.dedServ)
            {
                return;
            }

            ThunderveinDragon.SetBackgroundLight(ThunderveinDirector.FallingLandSkyLight, ThunderveinDirector.FallingLandSkyFrames);
            Shake(ctx, Vector2.UnitY * ThunderveinDirector.LandShakeDirY, strength, vibration, frames);
        }

        private static void SpawnLandFog(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            PRTLoader.NewParticle(npc.Center + Main.rand.NextVector2Circular(npc.width / ThunderveinDirector.FallingFogSpreadDiv, npc.height / ThunderveinDirector.FallingFogSpreadDiv),
                Vector2.Zero, CoraliteContent.ParticleType<BigFog>(),
                Coralite.ThunderveinYellow * Main.rand.NextFloat(ThunderveinDirector.FallingFogAlphaMin, ThunderveinDirector.FallingFogAlphaMax),
                Main.rand.NextFloat(ThunderveinDirector.FallingFogScaleMin, ThunderveinDirector.FallingFogScaleMax));

            Vector2 pos = npc.Center + Main.rand.NextVector2Circular(npc.width * ThunderveinDirector.FallingSparkSpreadFactor, npc.height * ThunderveinDirector.FallingSparkSpreadFactor);
            Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero,
                newColor: ThunderveinDragon.ThunderveinYellowAlpha,
                Scale: Main.rand.NextFloat(ThunderveinDirector.FallingSparkScaleMin, ThunderveinDirector.FallingSparkScaleMax));
        }

        protected override void OnBeatAdopted(ThunderveinDragonContext ctx, int previousBeat)
        {
            if (CurrentBeat is Beat.MidSmash or Beat.FinalSmash)
            {
                ctx.Boss.ResetAllOldCaches();
                PlayStrikeCue(ctx);
            }
        }

        protected override IVaultState<ThunderveinDragonContext> AuthorityUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            if (strikeThisFrame)
            {
                strikeThisFrame = false;
                ctx.MarkDecision();
                SpawnStrikes(ctx);
            }

            if (CurrentBeat == Beat.FinalSmash
                && Timer > ThunderveinDirector.FallingSmashFrames + ThunderveinDirector.FallingLandFogFrames + ThunderveinDirector.FallingLandFlyFrames(ctx.Phase))
            {
                return EndAttack(ctx);
            }

            return null;
        }

        /// <summary>仅权威端：三道落雷从本体两侧铺开，打向落点下方 250 px。旧 :205-213 / :475-483</summary>
        private void SpawnStrikes(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 target = AimPos + new Vector2(0, ThunderveinDirector.FallingStrikeTargetOffsetY);
            int damage = ThunderveinDirector.FallingStrikeDamage(ctx.Phase);
            int type = ctx.Phase == 1 ? ModContent.ProjectileType<ThunderFalling>() : ModContent.ProjectileType<StrongThunderFalling>();

            for (int i = 0; i < ThunderveinDirector.FallingStrikeCount; i++)
            {
                Vector2 from = npc.Center + new Vector2(
                    -ThunderveinDirector.FallingStrikeSpreadX + (i * ThunderveinDirector.FallingStrikeSpanX / ThunderveinDirector.FallingStrikeCount),
                    Main.rand.Next(ThunderveinDirector.FallingStrikeMinY, ThunderveinDirector.FallingStrikeMaxY));
                npc.NewProjectileInAI_Server(from, target, type, damage, 0, npc.target,
                    ThunderveinDirector.FallingSmashFrames + ThunderveinDirector.FallingStrikeLeadFrames, npc.whoAmI,
                    ThunderveinDirector.FallingStrikeProjAi2(ctx.Phase));
            }
        }

        protected override void WriteSlots(ThunderveinDragonContext ctx)
        {
            ctx.Hot[CoraliteBossHotSlots.A] = aimX;
            ctx.Hot[CoraliteBossHotSlots.B] = aimY;
            ctx.Hot[CoraliteBossHotSlots.C] = aimCount;
        }

        protected override void ReadSlots(ThunderveinDragonContext ctx)
        {
            aimX = ctx.Hot[CoraliteBossHotSlots.A];
            aimY = ctx.Hot[CoraliteBossHotSlots.B];
            aimCount = (int)ctx.Hot[CoraliteBossHotSlots.C];
        }
    }
}
