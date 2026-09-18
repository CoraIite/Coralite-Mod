using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using Coralite.Content.Bosses.Rediancie;
using Coralite.Content.Particles;
using Coralite.Core;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 赤色爆冲（大师二阶段近战）：先 45 帧原地转身瞄准（每 4 帧一颗横向星即预告），之后每 65 帧一轮共五轮——
    /// 轮内前 20 帧慢速就位，18 帧起手音效 + 闪光，22 帧瞄向玩家下 / 上 50 px（按 70 帧周期奇偶交替）以 14 px/f 起冲并获得 3 发弹药，
    /// 51 帧前边冲边在前方每 6 帧放一个爆炸，52 帧一记大爆炸，之后 0.97 刹车；370 帧收招。
    /// 旧 <c>Bloodiancie.Dash</c>（AI.cs:415-481），分支结构原样保留（20、21 帧落入冲刺段、起冲前不放爆炸也是旧行为）。
    /// 冲刺速度由目标位置确定性推导，两端各算。
    /// </summary>
    [VaultState((int)BloodiancieStateId.dash, typeof(BloodiancieContext))]
    internal sealed class BloodiancieDashState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.dash;

        /// <summary>瞄准段之后的轮内帧（瞄准段为负值，不会命中任何轮内拍点）。</summary>
        private int CycleTime => (Timer - BloodiancieDirector.DashAimFrames) % BloodiancieDirector.DashCycleFrames;

        private bool Aiming => Timer < BloodiancieDirector.DashAimFrames;

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            // 旧代码 Dash 全程不调常规朝向：就位段与冲刺段保持 rotation 不动
            ctx.DeclareRotation(BloodiancieRotationMode.Keep);

            if (Aiming)
            {
                ctx.DeclareKeep();
                ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp((ctx.Target.Center - ctx.Npc.Center).ToRotation() + MathHelper.PiOver2,
                    Timer / (float)BloodiancieDirector.DashAimFrames);

                if (Timer % BloodiancieDirector.DashAimStarInterval == 0 && !Main.dedServ)
                {
                    PRTLoader.NewParticle(ctx.Npc.Center + Main.rand.NextVector2Circular(BloodiancieDirector.DashAimStarSpread, BloodiancieDirector.DashAimStarSpread),
                        Vector2.UnitY * -2, CoraliteContent.ParticleType<HorizontalStar>(), Coralite.RedJadeRed,
                        Main.rand.NextFloat(BloodiancieDirector.DashAimStarScaleMin, BloodiancieDirector.DashAimStarScaleMax));
                }

                ctx.UpdateFollowersIdle(Timer);
                return;
            }

            int realTime = CycleTime;

            if (realTime == BloodiancieDirector.DashCueFrame && !Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.Item4, ctx.Npc.Center);
                PRTLoader.NewParticle(ctx.Npc.Center + new Vector2(0, -16), Vector2.Zero,
                    CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.RedJadeRed, BloodiancieDirector.DashCueSparkScale);
            }

            if (realTime < BloodiancieDirector.DashWindupFrames)
            {
                ctx.DeclareChaseX(BloodiancieDirector.DashWindupSpeedX, BloodiancieDirector.DashWindupAccelX,
                    BloodiancieDirector.DashWindupTurnX, BloodiancieDirector.DashWindupDamp);
                ctx.DeclareChaseYWithDeadZone(BloodiancieDirector.DashWindupSpeedY, BloodiancieDirector.DashWindupAccelY,
                    BloodiancieDirector.DashWindupTurnY, BloodiancieDirector.DashWindupDamp);
            }
            else if (realTime == BloodiancieDirector.DashLaunchFrame)
            {
                ctx.SpawnFollowers(BloodiancieDirector.DashGainFollowers);
                float offsetY = Timer / BloodiancieDirector.DashAimAlternatePeriod % 2 == 0
                    ? BloodiancieDirector.DashAimOffsetY
                    : -BloodiancieDirector.DashAimOffsetY;
                ctx.Npc.velocity = (ctx.Target.Center + new Vector2(0, offsetY) - ctx.Npc.Center).SafeNormalize(Vector2.One) * BloodiancieDirector.DashSpeed;
                ctx.Npc.rotation = ctx.Npc.velocity.ToRotation() + MathHelper.PiOver2;
                ctx.DeclareDirect();
            }
            else if (realTime < BloodiancieDirector.DashBoomEnd)
            {
                ctx.DeclareKeep();
            }
            else
            {
                ctx.DeclareDamp(BloodiancieDirector.DashBrakeDamp);
                ctx.DeclareRotation(BloodiancieRotationMode.LerpToSpeed, BloodiancieDirector.RotationLerpDash);
            }

            ctx.UpdateFollowersIdle(Timer);
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            if (!Aiming)
            {
                int realTime = CycleTime;

                if (realTime >= BloodiancieDirector.DashWindupFrames && realTime < BloodiancieDirector.DashBoomEnd)
                {
                    if (realTime % BloodiancieDirector.DashBoomInterval == 0)
                    {
                        Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), Ahead(ctx), Vector2.Zero,
                            ModContent.ProjectileType<Rediancie_Explosion>(), BloodiancieDirector.DashBoomDamage(), BloodiancieDirector.DashBoomKnockback);
                    }
                }
                else if (realTime == BloodiancieDirector.DashBigBoomFrame)
                {
                    SpawnBigBoom(ctx, Ahead(ctx), BloodiancieDirector.DashBigBoomDamage(), BloodiancieDirector.DashBigBoomKnockback);
                }
            }

            if (Timer > BloodiancieDirector.DashAimFrames + (BloodiancieDirector.DashCycleFrames * BloodiancieDirector.DashCycleCount))
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
