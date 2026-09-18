using Coralite.Content.Bosses.Rediancie.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 赤色爆冲（大师二阶段近战）：每 100 帧一轮共三轮——前 20 帧慢速就位，18 帧起手音效 + 闪光即预告，22 帧瞄向玩家上 / 下 100 px（下、上、下交替）以 10 px/f 起冲并获得 2 发弹药，
    /// 71 帧前边冲边在前方每 10 帧放一个爆炸，之后 0.98 刹车；每轮末帧比玩家高时 1/3 概率改下砸；300 帧收招。
    /// 旧 <c>Rediancie.Dash</c>（Rediancie.cs:633-703），分支结构原样保留（20、21 帧落入爆炸段、起冲前先放一个爆炸也是旧行为）。
    /// 冲刺速度由目标位置确定性推导，两端各算；下砸掷骰仅权威端。
    /// </summary>
    [VaultState((int)RediancieStateId.dash, typeof(RediancieContext))]
    internal sealed class RediancieDashState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.dash;

        private int CycleTime => Timer % RediancieDirector.DashCycleFrames;

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            int realTime = CycleTime;

            if (realTime == RediancieDirector.DashCueFrame && !Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.Item4, ctx.Npc.Center);
                PRTLoader.NewParticle(ctx.Npc.Center + new Vector2(0, -16), Vector2.Zero, CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.RedJadeRed, RediancieDirector.DashCueSparkScale);
            }

            // 旧代码 Dash 全程不调常规朝向：就位段与冲刺段保持 rotation 不动
            ctx.DeclareRotation(RediancieRotationMode.Keep);

            if (realTime < RediancieDirector.DashWindupFrames)
            {
                ctx.DeclareChaseX(RediancieDirector.DashWindupSpeed, RediancieDirector.DashWindupAccel, RediancieDirector.DashWindupTurn, RediancieDirector.DashWindupDamp);
                ctx.DeclareChaseYWithDeadZone(RediancieDirector.DashWindupSpeed, RediancieDirector.DashWindupAccel, RediancieDirector.DashWindupTurn, RediancieDirector.DashWindupDamp);
            }
            else if (realTime == RediancieDirector.DashLaunchFrame)
            {
                ctx.SpawnFollowers(RediancieDirector.DashGainFollowers);
                float offsetY = Timer / RediancieDirector.DashCycleFrames % 2 == 0 ? RediancieDirector.DashAimOffsetY : -RediancieDirector.DashAimOffsetY;
                ctx.Npc.velocity = (ctx.Target.Center + new Vector2(0, offsetY) - ctx.Npc.Center).SafeNormalize(Vector2.One) * RediancieDirector.DashSpeed;
                ctx.Npc.rotation = ctx.Npc.velocity.ToRotation() + MathHelper.PiOver2;
                ctx.DeclareDirect();
            }
            else if (realTime < RediancieDirector.DashBoomEnd)
            {
                ctx.DeclareKeep();
            }
            else
            {
                ctx.DeclareDamp(RediancieDirector.DashBrakeDamp);
                ctx.DeclareRotation(RediancieRotationMode.LerpToSpeed, RediancieDirector.RotationLerpDash);
            }

            ctx.UpdateFollowersIdle(Timer);
        }

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            int realTime = CycleTime;

            if (realTime >= RediancieDirector.DashWindupFrames && realTime < RediancieDirector.DashBoomEnd)
            {
                if (realTime % RediancieDirector.DashBoomInterval == 0)
                {
                    Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), Ahead(ctx), Vector2.Zero,
                        ModContent.ProjectileType<Rediancie_Explosion>(), RediancieDirector.DashBoomDamage(), RediancieDirector.DashBoomKnockback);
                }
            }
            else if (realTime == RediancieDirector.DashSlamCheckFrame && AboveTarget(ctx) && Main.rand.NextBool(RediancieDirector.DashSlamChance))
            {
                return Create(RediancieStateId.slamDown);
            }

            if (Timer > RediancieDirector.DashEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
