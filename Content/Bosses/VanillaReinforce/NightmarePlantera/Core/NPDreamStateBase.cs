using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core
{
    /// <summary>
    /// 二阶段（梦境）招式的公共外壳。把旧 <c>Dream_Phase2()</c> 分派器里那圈"每招都要做的事"收进来：<br/>
    /// 天空续命 + 推帧动画 + 补触手 → 摆触手目标点 → 招式本体 → 推进触手模拟。<br/>
    /// 招式只需要实现 <see cref="Phase2Update"/> 与 <see cref="NightmarePlanteraStateBase.AuthorityUpdate"/>。
    /// </summary>
    internal abstract class NPDreamStateBase : NightmarePlanteraStateBase
    {
        /// <summary>
        /// 是否由外壳每帧摆触手目标点。旧分派器里转圈咬、下方闪光咬、蝙蝠与乌鸦三招前面没有 <c>NormallySetTentacle()</c>
        /// ——它们自己按节拍摆触手（转圈时要甩开、瞬移后要归位），外壳再摆一次会把动作抹平。
        /// </summary>
        protected virtual bool AutoTentacle => true;

        /// <summary>
        /// 是否走梦境变体的起手复位。旧 <c>SetPhase2DreamingStates</c>（Phase.P2_Dream.cs:2466-2477）比
        /// <c>SetPhase2States</c> 少复位 <c>useDreamMove</c> / <c>DreamMoveCount</c> / <c>fantasyKillCount</c> / 触手星帧
        /// ——梦境战斗正是靠这几个量累计进度，清了就永远打不完。
        /// </summary>
        protected virtual bool DreamingVariant => false;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);

            if (DreamingVariant)
            {
                ApplyDreamingPresentation(ctx);
                return;
            }

            ApplyPhase2Presentation(ctx);
        }

        /// <summary>
        /// 二阶段每招起手的表现复位（旧 <c>SetPhase2States</c> 的双端那一半，Phase.P2_Dream.cs:2401-2410）。<br/>
        /// 必须在 <c>OnEnter</c> 里做而不是在提交口里做：客户端不选招，它是被 NetSync 换过来的，只有 <c>OnEnter</c> 两端都会跑。<br/>
        /// <c>dontTakeDamage</c> 与接触伤害不在这里写——它们已经是每帧声明的通道。
        /// </summary>
        protected static void ApplyPhase2Presentation(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.warpScale = 0;
            boss.canDrawWarp = false;
            boss.useDreamMove = false;
            boss.tentacleColor = NightmarePlantera.lightPurple;
            boss.DreamMoveCount = 0;
            boss.fantasyKillCount = 0;
            boss.tentacleStarFrame = 0;
            boss.alpha = 1;
        }

        /// <summary>梦境变体的起手复位（旧 <c>SetPhase2DreamingStates</c> 的双端那一半，:2468-2474）。</summary>
        protected static void ApplyDreamingPresentation(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.warpScale = 0;
            boss.haveBeenPhase2 = true;
            boss.canDrawWarp = false;
            boss.tentacleColor = NightmarePlantera.lightPurple;
            boss.alpha = 1;
        }

        protected sealed override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;

            // 触手要在服务端也存在：下面的 NormallySetTentacle / NormallyUpdateTentacle 两端都会跑，
            // 旧代码只在非服务端建它，专用服务器上一进二阶段就会空引用。贴图为 null 无所谓，服务端不绘制。
            boss.EnsureRotateTentacles();

            if (!VaultUtils.isServer)
            {
                ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = NightmarePlanteraDirector.SkyTimeleft;
                boss.UpdateFrameNormally();
            }

            if (AutoTentacle)
            {
                boss.NormallySetTentacle();
            }

            Phase2Update(machine, ctx);
            boss.NormallyUpdateTentacle();
        }

        /// <summary>招式本体（双端）：写运动 / 表现声明，推进确定性子拍，调 <c>*_Server</c> 系列生成。</summary>
        protected abstract void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx);

        /// <summary>场上有美梦光时一切咬击都优先扑它——二阶段"打光"机制的核心，原样保留。</summary>
        protected static Vector2 TargetOrSparkle(NightmarePlanteraContext ctx)
            => NightmarePlantera.FantasySparkleAlive(out NPC fs) ? fs.Center : ctx.TargetCenter;

        /// <summary>咬击落点：以"当前扑击对象"为心的环形随机半径。</summary>
        protected static Vector2 AimPos(NightmarePlanteraContext ctx, float min, float max)
            => ctx.Boss.PickTeleportOffsetAround(TargetOrSparkle(ctx), min, max);

        /// <summary>二阶段扑咬：朝当前扑击对象加速，近了就刹车。旧 <c>NightmareBite_Son1</c> 等五处共用同一段。</summary>
        protected static void LungeToSparkleOrTarget(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 pos = TargetOrSparkle(ctx);
            npc.rotation = npc.rotation.AngleTowards((pos - npc.Center).ToRotation(), 0.3f);

            if (Vector2.Distance(npc.Center, pos) > NightmarePlanteraDirector.P2LungeDistance)
            {
                float speed = npc.velocity.Length() + NightmarePlanteraDirector.P2LungeAccel;
                if (speed > NightmarePlanteraDirector.P2LungeMaxSpeed)
                {
                    speed = NightmarePlanteraDirector.P2LungeMaxSpeed;
                }

                npc.velocity = npc.velocity.ToRotation().AngleTowards(npc.rotation, 0.3f).ToRotationVector2() * speed;
                return;
            }

            npc.velocity *= NightmarePlanteraDirector.P2LungeDamp;
        }

        #region 自旋表演公共件（转圈咬 / 下方闪光咬 / 蝙蝠与乌鸦共用）

        /// <summary>自旋时把触手甩成一个绕锚点的三叉风车。</summary>
        protected static void SpinTentacles(NightmarePlanteraContext ctx, Vector2 center, Vector2 anchor, float factor,
            float turns = NightmarePlanteraDirector.P2RollingTentacleSpinTurns)
        {
            RotateTentacle[] tentacles = ctx.Boss.rotateTentacles;
            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = tentacles[i];
                float targetRot = (factor * MathHelper.TwoPi * turns) + (i * MathHelper.TwoPi / 3);
                Vector2 selfPos = Vector2.Lerp(tentacle.pos,
                    center + (NightmarePlanteraDirector.P2RollingTentacleRadius * targetRot.ToRotationVector2()), 0.2f);
                tentacle.SetValue(selfPos, anchor, targetRot);
                tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20, 0.7f);
            }
        }

        /// <summary>自旋段每帧的雾气。纯表现，但抽取走同步种子，两端都要跑（C3）。</summary>
        protected static void SpinFog(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            for (int i = 0; i < NightmarePlanteraDirector.P2RollingFogPerFrame; i++)
            {
                Color color = boss.AttackRandom.Next(0, 2) switch
                {
                    0 => new Color(110, 68, 200),
                    _ => new Color(122, 110, 134)
                };

                PRTLoader.NewParticle(ctx.Npc.Center + Main.rand.NextVector2Circular(32, 32), Helper.NextVec2Dir(2, 10f),
                    CoraliteContent.ParticleType<BigFog>(), color, Scale: boss.NextAttackFloat(0.5f, 1f));
            }
        }

        /// <summary>
        /// 挂在自旋计时尾巴上的淡出：与 <c>FadeTick</c> 同一套表现，但它不是独立一拍，所以按"总长 − 45"倒数。
        /// </summary>
        protected void SpinFadeOut(NightmarePlanteraContext ctx, int rollingFrames, Vector2 dustCenter)
        {
            NightmarePlantera boss = ctx.Boss;
            int fadeTime = NightmarePlanteraDirector.P2RollingFadeFrames;

            if (Timer <= rollingFrames - fadeTime)
            {
                return;
            }

            if (Timer == rollingFrames - fadeTime && !Main.dedServ)
            {
                SoundEngine.PlaySound(CoraliteSoundID.ShieldDestroyed_NPCDeath58, ctx.Npc.Center);
            }

            if (boss.alpha > 0)
            {
                boss.alpha -= 1 / (float)fadeTime;
                if (boss.alpha < 0)
                {
                    boss.alpha = 0;
                }
            }

            // 旧代码这里的系数用的是 Timer / fadeTime（此刻 Timer 已经三百多），扭曲圈因此在收尾段快速脉动。
            // 这是既有观感的一部分，原样保留（D10）。
            boss.canDrawWarp = true;
            boss.warpScale = MathF.Sin(Timer / (float)fadeTime * MathHelper.Pi) * 2f;

            if (Timer != rollingFrames - (fadeTime * 3 / 4))
            {
                return;
            }

            for (int i = 0; i < NightmarePlanteraDirector.P3FadeDustCount; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                Dust dust = Dust.NewDustPerfect(dustCenter + (dir * boss.AttackRandom.Next(0, 64)), DustType<NightmareStar>(),
                    dir * boss.NextAttackFloat(2f, 6f), newColor: new Color(153, 88, 156, 230), Scale: boss.NextAttackFloat(1f, 4f));
                dust.rotation = dir.ToRotation() + MathHelper.PiOver2;
            }

            Helper.PlayPitched(CoraliteSoundID.NoUse_SuperMagicShoot_Item68, ctx.Npc.Center, pitch: -1f);
        }

        /// <summary>瞬移落点归位触手，避免连线绘制拖出一条横跨全屏的线（C10）。</summary>
        protected static void SnapTentacles(NightmarePlanteraContext ctx)
            => ctx.Boss.ResetTentaclesTo(ctx.Npc.Center, ctx.Npc.rotation);

        #endregion
    }
}
