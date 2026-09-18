using Coralite.Content.Bosses.ThunderveinDragon.States;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon.Core
{
    /// <summary>
    /// 荒雷龙状态基类。状态索引沿用本体上的 <see cref="ThunderveinDragon.AIStates"/>（天空按它判断冥雷段，数值不变，追加 Hub）。<br/>
    /// · <c>SharedUpdate</c>：两端同跑——写运动 / 朝向 / 无敌 / 带电 / 残影声明，推进确定性子拍，客户端粒子音效。<br/>
    /// · <see cref="AuthorityUpdate"/>：仅权威端——弹幕 / 召唤 / 掷骰，返回下一状态；本类把死亡请求与超时兜底垫在它前面。<br/>
    /// · 收招一律 <see cref="EndAttack"/>，出招一律经 <see cref="ThunderveinHubState.Commit"/>；招式体内不得出现 <c>ChangeState</c>。<br/>
    /// · 计时：旧招式体在拍尾 <c>Timer++</c>。收拍判断（<c>Timer &gt; N</c>）直接用基座 <see cref="VaultState{TContext}.Timer"/>（首帧 1），
    ///   拍首一次性判断与区间（<c>Timer == N</c> / <c>Timer &lt; N</c>）用 <see cref="T"/>（首帧 0）。旧代码等翅膀帧时不计时的拍用 <see cref="HoldTimer"/> 把本帧的 ++ 抵掉。<br/>
    /// · 热字段：子类 override <see cref="WriteSlots"/> / <see cref="ReadSlots"/> 读写自用槽；客户端收养时若拍号刚变且新拍 Timer ≤ 宽限，回调 <see cref="OnBeatAdopted"/> 补放该拍的本地一次性演出（C9）。
    /// </summary>
    internal abstract class ThunderveinStateBase : CoraliteBossState<ThunderveinDragonContext>
    {
        public abstract ThunderveinDragon.AIStates StateIndex { get; }

        public override int StateId => (int)StateIndex;

        /// <summary>拍内 0 起计时（= Timer − 1），对应旧招式体在 <c>Timer++</c> 之前读到的值。</summary>
        protected int T => Timer - 1;

        /// <summary>超时兜底帧数；演出态可覆盖为更大值。</summary>
        protected virtual int TimeoutFrames => ThunderveinDirector.StateTimeoutFrames;

        public override void OnEnter(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            base.OnEnter(machine, ctx);

            // 招内随机在种子落定之后派生：权威端 base.OnEnter 刚掷了新种子，客户端 ai[1] 已随换态包到达。
            // 旧代码在掷种子之前派生，服务端用旧种子、客户端用新种子，两端从第一次 Next() 起就错位——此处修正。
            ctx.RefreshAttackRandom();

            // 旧 ResetStates / OnEnter：换招那一帧若贴图翻了面，给 rotation 补半圈（两端本地朝向数学）。
            if (ctx.Npc.spriteDirection != ctx.Boss.oldSpriteDirection)
            {
                ctx.Npc.rotation += ThunderveinDirector.FlipRotation;
            }

            // 残影包络回默认。旧壳 OnEnter 只在权威端清，客户端会带着上一招的 shadowScale / shadowAlpha 继续画；
            // 这两个量是纯表现、不过线，必须两端各自清。其余视觉标志（残影 / 冲刺 / 带电 / 无敌）已是每帧声明。
            ctx.ShadowAlpha = 1f;
            ctx.ShadowScale = 1f;
        }

        protected sealed override IVaultState<ThunderveinDragonContext> ServerUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            // CheckDead 只登记请求，换态统一从这里经返回值走（客户端读 ai[0] 跟随）。
            if (ctx.KillRequested && StateIndex != ThunderveinDragon.AIStates.onKillAnim)
            {
                return Create(ThunderveinDragon.AIStates.onKillAnim);
            }

            // 超时兜底：状态机永远不许死在一个状态里；收招不留残速。
            if (Counter++ > TimeoutFrames)
            {
                ctx.Npc.velocity *= 0.6f;
                return EndAttack(ctx);
            }

            return AuthorityUpdate(machine, ctx);
        }

        /// <summary>仅权威端：弹幕生成、召唤、掷骰；返回下一状态或 null。</summary>
        protected abstract IVaultState<ThunderveinDragonContext> AuthorityUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx);

        #region 热字段

        public sealed override void WriteHot(ThunderveinDragonContext ctx)
        {
            base.WriteHot(ctx);
            WriteSlots(ctx);
        }

        public sealed override void ReadHot(ThunderveinDragonContext ctx)
        {
            int previousBeat = BeatIndex;
            base.ReadHot(ctx);
            ReadSlots(ctx);

            if (BeatIndex != previousBeat && Timer <= CueCatchUpGrace)
            {
                OnBeatAdopted(ctx, previousBeat);
            }
        }

        /// <summary>权威端每包前写自用槽 A..H（默认无）。</summary>
        protected virtual void WriteSlots(ThunderveinDragonContext ctx) { }

        /// <summary>客户端收养自用槽（默认无）。此时目标可能尚未建立，不要碰目标玩家。</summary>
        protected virtual void ReadSlots(ThunderveinDragonContext ctx) { }

        /// <summary>
        /// 客户端：收养把拍号从 <paramref name="previousBeat"/> 换到当前拍、且新拍刚起（Timer ≤ 宽限）——本机没跑到那次换拍，
        /// 在这里补放该拍入口的纯本地演出（音效 / 震屏 / 天空 / 残影缓存）。只放表现，不碰速度与弹幕。
        /// </summary>
        protected virtual void OnBeatAdopted(ThunderveinDragonContext ctx, int previousBeat) { }

        #endregion

        #region 公共小件

        /// <summary>按 id 建状态；未注册返回 null（调用方要兜底）。</summary>
        protected static IVaultState<ThunderveinDragonContext> Create(ThunderveinDragon.AIStates id)
            => VaultStateRegistry<ThunderveinDragonContext>.Create((int)id);

        /// <summary>收招：经 hub 的唯一提交口选下一招（<see cref="ThunderveinDirector.HubFrames"/> = 0 时不多占一帧）。</summary>
        protected static IVaultState<ThunderveinDragonContext> EndAttack(ThunderveinDragonContext ctx)
            => ThunderveinHubState.EndAttack(ctx);

        /// <summary>旧代码“等翅膀帧到位才计时”的拍：把基座本帧的 ++ 抵掉，Timer 原地不动。</summary>
        protected void HoldTimer() => Timer--;

        /// <summary>纯本地震屏。</summary>
        protected static void Shake(ThunderveinDragonContext ctx, Vector2 direction, int strength, float vibration, int frames)
        {
            if (Main.dedServ)
            {
                return;
            }

            Main.instance.CameraModifiers.Add(new PunchCameraModifier(ctx.Npc.Center, direction, strength, vibration, frames, ThunderveinDirector.ShakeFalloffDistance));
        }

        /// <summary>吼叫声波 / 声线（纯本地）：每 10 帧一圈、每 20 帧一道，出生点在嘴前 60×scale。按 <see cref="T"/> 取模。</summary>
        protected void RoarFx(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            Vector2 pos = ctx.Npc.Center + (ctx.Npc.rotation.ToRotationVector2() * ThunderveinDirector.MouthForward * ctx.Npc.scale);
            if (T % ThunderveinDirector.RoarWaveInterval == 0)
            {
                PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Coralite.ThunderveinYellow, ThunderveinDirector.RoarFxScale);
            }

            if (T % ThunderveinDirector.RoarLineInterval == 0)
            {
                PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, ThunderveinDirector.RoarFxScale);
            }
        }

        /// <summary>沿瞄准线撒尘（纯本地）。旧 AI.LightningBreath.cs:101-113</summary>
        protected static void AimDust(ThunderveinDragonContext ctx, float aimAngle)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            Vector2 pos2 = npc.Center + ((npc.rotation - (npc.direction * ThunderveinDirector.AimDustRotOffset)).ToRotationVector2() * ThunderveinDirector.MouthForward);
            Vector2 dir2 = aimAngle.ToRotationVector2();
            for (int i = 0; i < ThunderveinDirector.AimDustCount; i++)
            {
                Dust d = Dust.NewDustPerfect(pos2 + (dir2 * Main.rand.NextFloat(ThunderveinDirector.AimDustMinDistance, ThunderveinDirector.AimDustMaxDistance)), DustID.PortalBoltTrail,
                    dir2.RotateByRandom(-ThunderveinDirector.AimDustSpread, ThunderveinDirector.AimDustSpread) * Main.rand.NextFloat(ThunderveinDirector.AimDustSpeedMin, ThunderveinDirector.AimDustSpeedMax),
                    newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(ThunderveinDirector.AimDustScaleMin, ThunderveinDirector.AimDustScaleMax));
                d.noGravity = true;
            }
        }

        /// <summary>嘴前吸入尘（纯本地；电球 / 引力雷球 / 十字雷蓄力）。旧 AI.LightningBall.cs:24-31</summary>
        protected static void MouthDust(ThunderveinDragonContext ctx, float edge, float speedMin, float speedMax, float scaleMax)
        {
            if (Main.dedServ)
            {
                return;
            }

            Vector2 pos = ctx.Boss.GetMousePos();
            Vector2 center = pos + Helpers.Helper.NextVec2Dir(edge - 1, edge);
            Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail,
                (pos - center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(speedMin, speedMax),
                newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, scaleMax));
            d.noGravity = true;
        }

        /// <summary>放电吸入尘（纯本地）：环边一粒切向、环内一粒径向。旧 AI.Discharging.cs:160-175</summary>
        protected static void DischargeDust(ThunderveinDragonContext ctx, float edge)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            for (int i = 0; i < ThunderveinDirector.DischargeDustPerFrame; i++)
            {
                Vector2 pos = npc.Center + Main.rand.NextVector2CircularEdge(edge, edge);
                Dust d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail,
                    (pos - npc.Center).SafeNormalize(Vector2.Zero).RotatedBy(npc.direction * MathHelper.PiOver2) * Main.rand.NextFloat(4f, 8f),
                    newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
                pos = npc.Center + Main.rand.NextVector2Circular(edge, edge);
                d = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail,
                    (pos - npc.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(4f, 8f),
                    newColor: Coralite.ThunderveinYellow);
                d.noGravity = true;
            }
        }

        /// <summary>爆发段张嘴帧：frame.X = 1，每 2 帧推进一格直到回到第 0 帧停住。旧 AI.Discharging.cs:127-137</summary>
        protected static void BurstMouthFrame(NPC npc)
        {
            if (npc.frame.Y == 0)
            {
                return;
            }

            npc.frame.X = 1;
            if (++npc.frameCounter > ThunderveinDirector.BurstMouthFrameTicks)
            {
                npc.frameCounter = 0;
                if (++npc.frame.Y > 7)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        /// <summary>爆发段残影包络：<paramref name="progress"/> 0→1 时缩放 1→<paramref name="scaleTo"/>、透明度 1→0（SqrtEase）。</summary>
        protected static void BurstShadowEnvelope(ThunderveinDragonContext ctx, float progress, float scaleTo)
        {
            float factor = Helpers.Helper.SqrtEase(progress);
            ctx.ShadowScale = Helpers.Helper.Lerp(1f, scaleTo, factor);
            ctx.ShadowAlpha = Helpers.Helper.Lerp(1f, 0f, factor);
        }

        /// <summary>冲刺姿态（每帧重申，收养后也成立）：冲刺贴图帧、朝向 = 速度方向、面向 = 速度 X 符号。</summary>
        protected static void DashPose(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.Boss.DashFrame();
            npc.rotation = npc.velocity.ToRotation();
            if (npc.velocity.X != 0f)
            {
                npc.direction = npc.spriteDirection = System.Math.Sign(npc.velocity.X);
            }
        }

        /// <summary>水平朝向（旧 <c>rotation = direction &gt; 0 ? 0 : 3.141f</c>）。</summary>
        protected static float LevelRotation(NPC npc) => npc.direction > 0 ? 0f : ThunderveinDirector.FlipRotation;

        #endregion
    }
}
