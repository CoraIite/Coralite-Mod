using Coralite.Content.Bosses.Rediancie.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 下砸（近战变体，由蓄力 / 爆冲 / 三连炸在玩家上方时掷骰进入）：<br/>
    /// Rise：X 追踪玩家、速度 50 帧内从 10 降到 0.5，前 40 帧向上加速；第 1 帧放预判线；50 帧后转 Fall。<br/>
    /// Fall：Y 加速 1.2 到 22，X 衰减；脚下 3 行有物块即落地（大爆炸 + 震屏 + 5 发弹药 + 向上散射），低于玩家 500 px 视为砸空。<br/>
    /// Land：0.95 刹停，20 帧后收招。旧 <c>Rediancie.SlamDown</c>（Rediancie.cs:1040-1196）。<br/>
    /// 落地由物块碰撞判定，两端各自检测（位置两端同积分），客户端若差一帧由热字段的 Beat / Timer 拉齐；弹幕只在权威端。
    /// </summary>
    [VaultState((int)RediancieStateId.slamDown, typeof(RediancieContext))]
    internal sealed class RediancieSlamDownState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.slamDown;

        private enum Beat
        {
            /// <summary>上升追踪（旧 localAI[0] = 1）</summary>
            Rise = 0,
            /// <summary>下落（旧 2）</summary>
            Fall = 1,
            /// <summary>落地 / 砸空后刹停（旧 3）</summary>
            Land = 2,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>本帧刚触地（SharedUpdate 置位，AuthorityUpdate 同帧消费后清零）。</summary>
        private bool landedThisFrame;

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            landedThisFrame = false;

            switch (CurrentBeat)
            {
                case Beat.Fall:
                    UpdateFall(ctx);
                    break;
                case Beat.Land:
                    ctx.DeclareDamp(RediancieDirector.SlamLandDamp);
                    ctx.DeclareRotation(RediancieRotationMode.Normal);
                    break;
                default:
                    UpdateRise(ctx);
                    break;
            }

            ctx.UpdateFollowersIdle(Timer);
        }

        private void UpdateRise(RediancieContext ctx)
        {
            float targetX = ctx.Target.Center.X - ctx.Npc.Center.X;
            float xSpeed = MathHelper.Lerp(RediancieDirector.SlamRiseSpeedXFrom, RediancieDirector.SlamRiseSpeedXTo, Timer / RediancieDirector.SlamRiseSpeedXRamp);
            ctx.DeclareChaseX(xSpeed, RediancieDirector.SlamRiseAccelX, RediancieDirector.SlamRiseTurnX, RediancieDirector.SlamRiseDampX, Math.Sign(targetX));

            // Y 轴由状态自管：宿主 Chase 只动 X（未声明 ChaseY）
            if (Timer < RediancieDirector.SlamRiseAccelFrames)
            {
                if (ctx.Npc.velocity.Y > 0)
                {
                    ctx.Npc.velocity.Y = 0;
                }

                ctx.Npc.velocity.Y -= RediancieDirector.SlamRiseAccelY;
                if (ctx.Npc.velocity.Y < RediancieDirector.SlamRiseLimitY)
                {
                    ctx.Npc.velocity.Y = RediancieDirector.SlamRiseLimitY;
                }
            }
            else
            {
                ctx.Npc.velocity.Y *= RediancieDirector.SlamRiseSettleDampY;
            }

            if (Timer == 1 && !Main.dedServ)
            {
                BeamShotParticle p = PRTLoader.NewParticle<BeamShotParticle>(ctx.Npc.Center, Vector2.Zero, Coralite.RedJadeRed * 0.8f);
                p.bottomWidth = ctx.Npc.width / RediancieDirector.SlamTelegraphWidthDiv;
                p.targetLength = RediancieDirector.SlamTelegraphLength;
                p.aimBottomWidth = ctx.Npc.width / RediancieDirector.SlamTelegraphAimWidthDiv;
                p.aimTopWidth = ctx.Npc.width / RediancieDirector.SlamTelegraphAimWidthDiv;
                p.followNpcIndex = ctx.Npc.whoAmI;
                p.spawnTime = RediancieDirector.SlamTelegraphSpawnTime;
                p.contiundTime = RediancieDirector.SlamTelegraphHoldTime;
                p.Rotation = MathHelper.PiOver2;
            }

            if (Timer > RediancieDirector.SlamRiseFrames)
            {
                SwitchBeat(ctx, (int)Beat.Fall);
                ctx.Npc.velocity.Y = 0;
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.Item4, ctx.Npc.Center);
                }
            }

            ctx.DeclareRotation(RediancieRotationMode.LerpToZero, RediancieDirector.RotationLerpSlam);
        }

        private void UpdateFall(RediancieContext ctx)
        {
            ctx.Npc.velocity.Y += RediancieDirector.SlamFallAccelY;
            if (ctx.Npc.velocity.Y > RediancieDirector.SlamFallMaxY)
            {
                ctx.Npc.velocity.Y = RediancieDirector.SlamFallMaxY;
            }

            ctx.Npc.velocity.X *= RediancieDirector.SlamFallDampX;
            ctx.DeclareDirect();
            ctx.DeclareRotation(RediancieRotationMode.LerpToZero, RediancieDirector.RotationLerpSlam);

            // 低于玩家太多视为砸空
            if (ctx.Npc.Center.Y - ctx.Target.Center.Y > RediancieDirector.SlamMissDistance)
            {
                SwitchBeat(ctx, (int)Beat.Land);
                ctx.Npc.velocity = Vector2.Zero;
                return;
            }

            // 只在不低于玩家 100 px 时检测脚下物块
            if (ctx.Npc.Center.Y < ctx.Target.Center.Y - RediancieDirector.SlamGroundCheckMargin || !TouchingGround(ctx.Npc))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Land);
            landedThisFrame = true;
            ctx.Npc.velocity = new Vector2(0, RediancieDirector.SlamLandBounceY);
            ctx.Npc.rotation = 0;
            ctx.DeclareRotation(RediancieRotationMode.Keep);
            ctx.SpawnFollowers(RediancieDirector.SlamGainFollowers);

            if (!Main.dedServ)
            {
                Collision.HitTiles(ctx.Npc.BottomLeft, -Vector2.UnitY * 16, ctx.Npc.width, 16);
                Shake(ctx, RediancieDirector.LandShakeStrength, RediancieDirector.LandShakeVibration, RediancieDirector.LandShakeFrames);
                SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, ctx.Npc.Center);
            }
        }

        /// <summary>脚下 width/16 列 × 3 行内有可站立物块。旧 Rediancie.cs:1122-1128</summary>
        private static bool TouchingGround(NPC npc)
        {
            Point position = npc.BottomLeft.ToTileCoordinates();
            int width = npc.width / 16;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < RediancieDirector.SlamGroundCheckRows; j++)
                {
                    if (WorldGen.ActiveAndWalkableTile(position.X + i, position.Y + j))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            if (landedThisFrame)
            {
                landedThisFrame = false;
                SpawnBigBoom(ctx, ctx.Npc.Center, RediancieDirector.SlamBoomDamage(), RediancieDirector.SlamBoomKnockback);

                int damage = RediancieDirector.SlamStrikeDamage();
                int shootCount = RediancieDirector.SlamStrikeCount();
                for (int k = 0; k < shootCount; k++)
                {
                    float angle = -MathHelper.PiOver2 + Main.rand.NextFloat(-RediancieDirector.SlamStrikeSpread, RediancieDirector.SlamStrikeSpread);
                    Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(RediancieDirector.SlamStrikeSpeedMin, RediancieDirector.SlamStrikeSpeedMax);
                    Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), ctx.Npc.Center, velocity,
                        ModContent.ProjectileType<Rediancie_Strike>(), damage, RediancieDirector.SlamStrikeKnockback);
                }
            }

            if (CurrentBeat == Beat.Land && Timer > RediancieDirector.SlamLandFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
