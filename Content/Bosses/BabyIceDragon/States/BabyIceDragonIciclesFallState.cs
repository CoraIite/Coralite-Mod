using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 冰锥射击（大师专属）：爬到目标头顶 100 px 以上 → 悬停扫射 80 帧（俯仰角跟着高差走，嘴前冰雾是可见指向）→ 合翅吼叫，从目标头顶 500 px 处落 60 帧冰锥 → 进后摇。<br/>
    /// 公平阀：扫射的俯仰角封顶 ±0.45 弧度，贴到正下方就打不到；落冰锥阶段 boss 完全不动，是玩家的输出窗。旧 AI.IcicleShoot.cs
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.iciclesFall, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonIciclesFallState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.iciclesFall;

        private enum Beat
        {
            /// <summary>爬升并对齐横向。</summary>
            Approach,
            /// <summary>平射冰锥扫射。</summary>
            Sweep,
            /// <summary>吼叫并从头顶落冰锥。</summary>
            Roar,
        }

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Sweep:
                    UpdateSweep(ctx);
                    break;
                case Beat.Roar:
                    UpdateRoar(ctx);
                    break;
                default:
                    UpdateApproach(ctx);
                    break;
            }
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Sweep:
                    if (Timer >= BabyIceDragonDirector.IcicleSweepAimStart && Timer < BabyIceDragonDirector.IcicleSweepEnd
                        && Timer % BabyIceDragonDirector.IcicleShootInterval == 0)
                    {
                        ShootForward(ctx);
                    }

                    break;
                case Beat.Roar:
                    if (Timer >= BabyIceDragonDirector.IcicleRoarFrame && Timer % BabyIceDragonDirector.IcicleFallInterval == 0)
                    {
                        DropIcicle(ctx);
                    }

                    if (Timer >= BabyIceDragonDirector.IcicleRoarEndFrame)
                    {
                        return EnterRest(ctx, BabyIceDragonDirector.RestFramesAfterBreak);
                    }

                    break;
                default:
                    if (Timer > BabyIceDragonDirector.IcicleApproachTimeout)
                    {
                        return EndAttack(ctx);
                    }

                    break;
            }

            return null;
        }

        /// <summary>就位：不够高就扇翅上飞，够高就收 Y 速；横向超过 300 px 就追。旧 AI.IcicleShoot.cs:20-67</summary>
        private void UpdateApproach(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            bool lowerThanTarget = npc.Center.Y > ctx.Target.Center.Y - BabyIceDragonDirector.IcicleClimbHeight;
            float xLength = Math.Abs(npc.Center.X - ctx.Target.Center.X);

            if (lowerThanTarget || xLength > BabyIceDragonDirector.IcicleAlignX)
            {
                ctx.FaceTarget();
                if (lowerThanTarget)
                {
                    ctx.DeclareFlyUp();
                }
                else
                {
                    ctx.DeclareDampY(BabyIceDragonDirector.ClimbHoverDampY);
                    ctx.DeclareFlyingFrame();
                }

                ctx.DeclareApproachX(BabyIceDragonDirector.IcicleApproachDeadZoneX, BabyIceDragonDirector.IcicleApproachSpeedX,
                    BabyIceDragonDirector.IcicleApproachAccelX, BabyIceDragonDirector.IcicleApproachTurnX,
                    BabyIceDragonDirector.IcicleApproachDamp, BabyIceDragonDirector.IcicleApproachDamp);
                return;
            }

            // 旧代码先把 rotation 写成面向值（±1 弧度，随后就被扫射段压平），再更新面向，顺序照搬
            npc.rotation = npc.direction;
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            ctx.FaceTarget();
            ctx.DeclareKeep();
            ChargeCue(ctx);
            ChangeBeat(ctx, (int)Beat.Sweep);
        }

        /// <summary>扫射：全程收速，20 帧前压平朝向，之后俯仰角跟高差走并每 12 帧射一枚冰锥。旧 AI.IcicleShoot.cs:69-107</summary>
        private void UpdateSweep(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareFlyingFrame(1, false);
            ctx.DeclareDamp(BabyIceDragonDirector.IcicleSweepDamp);

            if (Timer < BabyIceDragonDirector.IcicleSweepAimStart)
            {
                npc.rotation = 0f;
                ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
                return;
            }

            if (Timer >= BabyIceDragonDirector.IcicleSweepEnd)
            {
                ctx.SetFrame(0, BabyIceDragonDirector.SpawnFrameY);
                npc.velocity = Vector2.Zero;
                ctx.DeclareDirect();
                ChangeBeat(ctx, (int)Beat.Roar);
                return;
            }

            npc.rotation = Math.Clamp(npc.direction * (ctx.Target.Center.Y - npc.Center.Y) * BabyIceDragonDirector.IcicleAimPerHeight,
                -BabyIceDragonDirector.IcicleAimClamp, BabyIceDragonDirector.IcicleAimClamp);
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            ctx.FaceTarget();

            if (Main.dedServ)
            {
                return;
            }

            if (Timer % BabyIceDragonDirector.IcicleShootInterval == 0)
            {
                // 旧代码把这声音写在服务端守卫里，客户端听不见；表现层按 C1 补齐两端都放
                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, npc.Center);
            }

            if (Timer % BabyIceDragonDirector.IcicleFogInterval == 0)
            {
                Vector2 targetDir = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.One);
                PRTLoader.NewParticle(ctx.MouthCenter(),
                    targetDir.RotatedBy(Main.rand.NextFloat(-BabyIceDragonDirector.IcicleFogAngle, BabyIceDragonDirector.IcicleFogAngle)) * BabyIceDragonDirector.IcicleFogSpeed,
                    CoraliteContent.ParticleType<Fog>(), Color.White, BabyIceDragonDirector.IcicleFogScale);
            }
        }

        /// <summary>吼叫段：朝向插值归零，20 帧吼叫，之后每 6 帧从目标头顶落一枚冰锥。旧 AI.IcicleShoot.cs:109-154</summary>
        private void UpdateRoar(BabyIceDragonContext ctx)
        {
            ctx.DeclareKeep();
            ctx.DeclareRotation(BabyIceDragonRotationMode.LerpToZero, BabyIceDragonDirector.IcicleRoarRotationLerp);

            if (Timer < BabyIceDragonDirector.IcicleRoarFrame)
            {
                return;
            }

            if (CueDue(BabyIceDragonDirector.IcicleRoarFrame))
            {
                RoarCue(ctx);
            }

            if (Timer < BabyIceDragonDirector.IcicleRoarEndFrame)
            {
                RoarParticles(ctx);
            }
        }

        /// <summary>平射一枚冰锥。旧 AI.IcicleShoot.cs:93-98</summary>
        private static void ShootForward(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 targetDir = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.One);
            ctx.SpawnHostile<IcicleProj_Hostile>(npc.GetSource_FromAI(), ctx.MouthCenter(),
                targetDir.RotatedBy(Main.rand.NextFloat(-BabyIceDragonDirector.IcicleShootSpread, BabyIceDragonDirector.IcicleShootSpread)) * BabyIceDragonDirector.IcicleShootSpeed,
                BabyIceDragonDirector.IcicleDamage(), BabyIceDragonDirector.IcicleShootKnockback);
            ctx.MarkDecision();
        }

        /// <summary>
        /// 从目标头顶 500 px 处落一枚冰锥。旧代码先生成再改 <c>velocity</c>（生成包已经出门，靠随后的 netUpdate 补），
        /// 这里改成生成时就带上速度，语义不变但少一次补包。旧 AI.IcicleShoot.cs:130-138
        /// </summary>
        private static void DropIcicle(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 spawn = ctx.Target.Center
                + new Vector2(Main.rand.Next(-BabyIceDragonDirector.IcicleFallOffsetX, BabyIceDragonDirector.IcicleFallOffsetX), BabyIceDragonDirector.IcicleFallOffsetY);
            Vector2 velocity = (ctx.Target.Center + Main.rand.NextVector2Circular(BabyIceDragonDirector.IcicleFallAimSpread, BabyIceDragonDirector.IcicleFallAimSpread) - spawn)
                .SafeNormalize(Vector2.Zero) * BabyIceDragonDirector.IcicleFallSpeed;

            ctx.SpawnHostile<IcicleFalling_Hostile>(npc.GetSource_FromAI(), spawn, velocity,
                BabyIceDragonDirector.IcicleDamage(), BabyIceDragonDirector.IcicleFallKnockback);
            ctx.MarkDecision();
        }
    }
}
