using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 聚合射击：落地站桩 → 召回全部分身 → 长蓄力 → 把分身一口吞掉换成一发扇形弹雨。<br/>
    /// 节拍：Ground → Compress（召回）→ Charge（蓄力，最长 240 帧）→ Recoil1 → Recoil2 → Restore → Done。<br/>
    /// 这是全场唯一的长预告招：内旋尘从 200 px 收到 20 px、直冲尘从 300 px 打进来，收拢过程本身就是读秒；
    /// 威力不是定值而是<b>场上分身数</b>——每吞一个 +10 伤害 / +1 发 / +0.2 缩放 / +1 速度，
    /// 所以“分裂之后不清分身”这件事会在这里结账（上限 25 发 / 缩放 4 / 速度 28）。<br/>
    /// 旧 <c>SlimeEmperor.PolymerizeShot</c>（AI.PolymerizeShot.cs:14-168）。
    /// </summary>
    [VaultState((int)AIStates.PolymerizeShot, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorPolymerizeShotState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.PolymerizeShot;

        private enum Beat
        {
            /// <summary>先落地（旧 SonState 0）</summary>
            Ground = 0,
            /// <summary>压缩，到位即召回分身（旧 1）</summary>
            Compress = 1,
            /// <summary>蓄力，满则出手（旧 2）</summary>
            Charge = 2,
            /// <summary>后摇一：拉长（旧 3）</summary>
            Recoil1 = 3,
            /// <summary>后摇二：压扁（旧 4）</summary>
            Recoil2 = 4,
            /// <summary>复原（旧 5）</summary>
            Restore = 5,
            /// <summary>收招（旧 6）</summary>
            Done = 6,
        }

        /// <summary>本帧出手参数，两端在 <c>SharedUpdate</c> 里按同一份分身名单算出，权威端同帧消费。</summary>
        private int shotDamage;
        private float shotScale;
        private int shotCount;
        private float shotSpeed;
        private Vector2 shotDirection;
        private bool shootThisFrame;

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            shootThisFrame = false;

            switch ((Beat)BeatIndex)
            {
                case Beat.Ground:
                    if (ctx.UpdateJump(SlimeEmperorDirector.PolyGroundJumpY, SlimeEmperorDirector.PolyGroundJumpX) == SlimeJumpEvent.Landed)
                    {
                        EnterBeat(ctx, (int)Beat.Compress);
                    }

                    break;

                case Beat.Compress:
                    ScaleBeat(ctx, SlimeEmperorDirector.SpikeFlatX, SlimeEmperorDirector.SpikeFlatY, SlimeEmperorDirector.PolyCompressLerp,
                        ctx.Scale.X > SlimeEmperorDirector.SpikeFlatDone, (int)Beat.Charge);
                    break;

                case Beat.Charge:
                    UpdateCharge(ctx);
                    break;

                case Beat.Recoil1:
                    ScaleBeat(ctx, SlimeEmperorDirector.PolyRecoilX, SlimeEmperorDirector.PolyRecoilY, SlimeEmperorDirector.PolyCompressLerp,
                        ctx.Scale.Y > SlimeEmperorDirector.PolyRecoilDone, (int)Beat.Recoil2);
                    break;

                case Beat.Recoil2:
                    ScaleBeat(ctx, SlimeEmperorDirector.SpikeFlatX, SlimeEmperorDirector.SpikeFlatY, SlimeEmperorDirector.PolyRecoil2Lerp,
                        ctx.Scale.X > SlimeEmperorDirector.SpikeFlatDone, (int)Beat.Restore);
                    break;

                case Beat.Restore:
                    ScaleBeat(ctx, 1f, 1f, SlimeEmperorDirector.PolyRestoreLerp, ctx.ScaleRestored(), (int)Beat.Done);
                    break;
            }
        }

        /// <summary>蓄力拍：帧图滚动 + 两层收拢尘；蓄满的那一帧把分身换算成出手参数并换到后摇。</summary>
        private void UpdateCharge(SlimeEmperorContext ctx)
        {
            AdvanceRollingFrame(ctx.Npc);

            int chargeFrames = ctx.Dangerous(Slime1Knowledge.Dangerous.SpeedBonus3_1)
                ? SlimeEmperorDirector.PolymerizeFramesChallenge
                : SlimeEmperorDirector.PolymerizeFrames();

            if (Timer < chargeFrames)
            {
                SpawnChargeDust(ctx, Timer / (float)chargeFrames);
                return;
            }

            SwallowAvatars(ctx);
            shotDirection = (ctx.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero);
            shootThisFrame = true;

            PlaySound(CoraliteSoundID.QueneSlimeFalling_Item167, ctx);
            SpawnShootDust(ctx);
            EnterBeat(ctx, (int)Beat.Recoil1);
        }

        /// <summary>
        /// 走一遍场上分身：撒吞噬连线尘并把它们换算成本次出手的伤害 / 发数 / 缩放 / 速度。<br/>
        /// 两端跑同一份（分身是同步实体），<b>但不在这里 Kill</b>——杀分身与生成弹幕都归 <see cref="AuthorityUpdate"/>。
        /// </summary>
        private void SwallowAvatars(SlimeEmperorContext ctx)
        {
            shotDamage = SlimeEmperorDirector.PolyBaseDamage();
            shotScale = SlimeEmperorDirector.PolyBaseScale;
            float count = SlimeEmperorDirector.PolyBaseCount;
            shotSpeed = SlimeEmperorDirector.PolyBaseSpeed;

            foreach (NPC avatar in EnumerateAvatars())
            {
                shotDamage += SlimeEmperorDirector.PolyDamagePerAvatar;
                shotScale += SlimeEmperorDirector.PolyScalePerAvatar;
                count += SlimeEmperorDirector.PolyCountPerAvatar;
                shotSpeed += SlimeEmperorDirector.PolySpeedPerAvatar;
                SpawnAvatarDust(ctx, avatar);
            }

            shotSpeed = Math.Min(shotSpeed, SlimeEmperorDirector.PolyMaxSpeed);
            shotScale = Math.Min(shotScale, SlimeEmperorDirector.PolyMaxScale);
            shotCount = (int)Math.Min(count, SlimeEmperorDirector.PolyMaxCount);
        }

        /// <summary>场上全部分身。沿用旧值 AI.PolymerizeShot.cs:80</summary>
        private static IEnumerable<NPC> EnumerateAvatars()
        {
            int type = ModContent.NPCType<SlimeAvatar>();
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == type)
                {
                    yield return npc;
                }
            }
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            //压缩到位：把分身全部召回（ai[3] = −1 是分身的“回收态”），生成后改的量要自带 netUpdate
            if (EnteredBeat == (int)Beat.Charge)
            {
                foreach (NPC avatar in EnumerateAvatars())
                {
                    avatar.ai[3] = -1;
                    avatar.noGravity = true;
                    avatar.velocity *= 0;
                    avatar.netUpdate = true;
                }
            }

            if (shootThisFrame)
            {
                shootThisFrame = false;
                Shoot(ctx);
            }

            return ReadyToFinish((int)Beat.Done) ? EndAttack(ctx) : null;
        }

        private void Shoot(SlimeEmperorContext ctx)
        {
            NPC npc = ctx.Npc;

            foreach (NPC avatar in EnumerateAvatars())
            {
                avatar.Kill();
            }

            for (int i = 0; i < shotCount; i++)
            {
                Vector2 velocity = shotDirection.RotatedBy(Main.rand.NextFloat(-SlimeEmperorDirector.PolyShootSpread, SlimeEmperorDirector.PolyShootSpread))
                    * Main.rand.NextFloat(shotSpeed - SlimeEmperorDirector.PolyShootSpeedJitter, shotSpeed + SlimeEmperorDirector.PolyShootSpeedJitter);
                Projectile p = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, velocity,
                    ModContent.ProjectileType<GelProj>(), shotDamage, SlimeEmperorDirector.PolyKnockback, npc.target);
                p.scale = shotScale;
                p.width = (int)(p.width * shotScale);
                p.height = (int)(p.height * shotScale);
                p.netUpdate = true;
            }

            //顺带朝玩家丢三个弹力球
            for (int i = -1; i < 2; i++)
            {
                Point pos = npc.Center.ToPoint();
                pos.X += Main.rand.Next(-SlimeEmperorDirector.PolyBallScatterOffset, SlimeEmperorDirector.PolyBallScatterOffset);
                pos.Y += Main.rand.Next(-SlimeEmperorDirector.PolyBallScatterOffset, SlimeEmperorDirector.PolyBallScatterOffset);
                NPC ball = NPC.NewNPCDirect(npc.GetSource_FromAI(), pos.X, pos.Y, ModContent.NPCType<ElasticGelBall>());
                ball.velocity = shotDirection.RotatedBy(i * SlimeEmperorDirector.PolyBallAngleStep)
                    * Main.rand.NextFloat(SlimeEmperorDirector.PolyBallSpeedMin, SlimeEmperorDirector.PolyBallSpeedMax);
                ball.netUpdate = true;
            }
        }

        #region 表现（纯本地）

        /// <summary>蓄力两层尘：内旋尘沿切向绕，直冲尘朝心冲，半径都随进度收拢。</summary>
        private static void SpawnChargeDust(SlimeEmperorContext ctx, float factor)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            for (int i = 0; i < SlimeEmperorDirector.PolyRingDustCount; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                Vector2 velocity = dir.RotatedBy(MathHelper.PiOver2)
                    * Main.rand.NextFloat(SlimeEmperorDirector.PolyRingDustSpeedMin, SlimeEmperorDirector.PolyRingDustSpeedMax);
                float radius = SlimeEmperorDirector.PolyRingRadius - (factor * SlimeEmperorDirector.PolyRingShrink);
                Dust dust = Dust.NewDustPerfect(npc.Center + (dir * radius) + Main.rand.NextVector2Circular(SlimeEmperorDirector.PolyRingDustJitter, SlimeEmperorDirector.PolyRingDustJitter),
                    DustID.t_Slime, velocity, SlimeEmperorDirector.GelDustAlpha, SlimeEmperorDirector.GelDustColor, SlimeEmperorDirector.PolyRingDustScale);
                dust.noGravity = true;
            }

            for (int i = 0; i < SlimeEmperorDirector.PolyBeamDustCount; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                Vector2 velocity = -dir * Main.rand.NextFloat(SlimeEmperorDirector.PolyBeamDustSpeedMin, SlimeEmperorDirector.PolyBeamDustSpeedMax);
                float radius = SlimeEmperorDirector.PolyBeamRadius - (factor * SlimeEmperorDirector.PolyBeamShrink);
                Dust dust = Dust.NewDustPerfect(npc.Center + (dir * radius), DustID.LastPrism, velocity,
                    SlimeEmperorDirector.GelDustAlpha, SlimeEmperorDirector.GelDustColor, SlimeEmperorDirector.PolyBeamDustScale);
                dust.noGravity = true;
            }
        }

        /// <summary>吞噬某个分身时的连线尘（从分身朝本体流）。</summary>
        private static void SpawnAvatarDust(SlimeEmperorContext ctx, NPC avatar)
        {
            if (Main.dedServ)
            {
                return;
            }

            Vector2 dir = (ctx.Npc.Center - avatar.Center).SafeNormalize(Vector2.Zero);
            for (int i = 0; i < SlimeEmperorDirector.PolyAvatarDustCount; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    avatar.Center + (dir * Main.rand.Next(SlimeEmperorDirector.PolyAvatarDustRange)) + Main.rand.NextVector2Circular(SlimeEmperorDirector.PolyAvatarDustScatter, SlimeEmperorDirector.PolyAvatarDustScatter),
                    DustID.t_Slime, dir * Main.rand.NextFloat(SlimeEmperorDirector.PolyAvatarDustSpeedMin, SlimeEmperorDirector.PolyAvatarDustSpeedMax),
                    SlimeEmperorDirector.GelDustAlpha, SlimeEmperorDirector.GelDustColor, SlimeEmperorDirector.PolyAvatarDustScale);
                dust.noGravity = true;
            }
        }

        /// <summary>出手爆散尘，量随发数走。</summary>
        private void SpawnShootDust(SlimeEmperorContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            int count = (int)(shotCount * SlimeEmperorDirector.PolyShootDustPerCount);
            for (int i = 0; i < count; i++)
            {
                Dust dust = Dust.NewDustPerfect(ctx.Npc.Center + Main.rand.NextVector2Circular(SlimeEmperorDirector.PolyShootDustScatter, SlimeEmperorDirector.PolyShootDustScatter),
                    DustID.t_Slime, shotDirection * Main.rand.NextFloat(SlimeEmperorDirector.PolyShootDustSpeedMin, SlimeEmperorDirector.PolyShootDustSpeedMax),
                    SlimeEmperorDirector.GelDustAlpha, SlimeEmperorDirector.GelDustColor, SlimeEmperorDirector.PolyShootDustScale);
                dust.noGravity = true;
            }
        }

        #endregion
    }
}
