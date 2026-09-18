using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core
{
    /// <summary>
    /// 史莱姆皇帝状态基类。<br/>
    /// · <c>SharedUpdate</c>：两端同跑——跳跃机、形变、确定性推拍、粒子音效；推拍时用 <see cref="EnterBeat"/> 登记拍号。<br/>
    /// · <see cref="AuthorityUpdate"/>：仅权威端——弹幕 / 召唤 / 掷骰，返回下一状态；本类把死亡请求、重锁目标与超时兜底垫在它前面。<br/>
    /// · 收招一律 <see cref="EndAttack"/>，出招一律经 <see cref="SlimeEmperorHubState.Commit"/>；招式体内不得出现 <c>ChangeState</c>。
    /// </summary>
    internal abstract class SlimeEmperorStateBase : CoraliteBossState<SlimeEmperorContext>
    {
        public abstract SlimeEmperor.AIStates StateIndex { get; }

        public override int StateId => (int)StateIndex;

        /// <summary>超时兜底帧数；演出态可覆盖。</summary>
        protected virtual int TimeoutFrames => SlimeEmperorDirector.StateTimeoutFrames;

        /// <summary>
        /// 入场拍的计时修正。基座在招式体之前自增 Timer，招式体首帧读到 1；
        /// 而旧代码是“跑完体再 ++”，<b>只有进状态的第一拍</b>首帧读到 0（其余拍都是在上一拍体内清零、随后 ++，首帧同样是 1）。
        /// 入场拍里的 <c>Timer</c> 阈值一律读这个值，拍点才与旧版一帧不差。
        /// </summary>
        protected int EntryTimer => Timer - 1;

        /// <summary>
        /// 本帧刚进入的拍号（-1 表示没换拍）。两端在 <c>SharedUpdate</c> 里按确定性判据推拍并登记，
        /// <see cref="AuthorityUpdate"/> 据此只在权威端做生成与掷骰；每帧末由本类清掉。
        /// </summary>
        protected int EnteredBeat { get; private set; } = -1;

        /// <summary>换拍并登记（两端都调；<c>SwitchBeat</c> 内部会在权威端打决策点）。</summary>
        protected void EnterBeat(SlimeEmperorContext ctx, int beat)
        {
            SwitchBeat(ctx, beat);
            EnteredBeat = beat;
        }

        /// <summary>
        /// 是否到了收招帧。旧代码换到收招 SonState 的那一帧只做换号，<b>下一帧</b>的 switch 才走 <c>ResetStates</c>；
        /// 这里同样让收招拍空转一帧，收招节奏与旧版一帧不差。
        /// </summary>
        protected bool ReadyToFinish(int finishBeat) => BeatIndex == finishBeat && EnteredBeat < 0;

        public override void OnEnter(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            base.OnEnter(machine, ctx);     // 权威端：ResetAttackLocals + RollAttackSeed；客户端只清本地量
            EnteredBeat = -1;
            ctx.PrepareAttack();            // 招前公共清理 + 起跳判定（双端）
        }

        protected sealed override IVaultState<SlimeEmperorContext> ServerUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            // CheckDead 只登记请求，换态统一从这里经返回值走（客户端读 ai[0] 跟随）。
            if (ctx.KillRequested && StateIndex != SlimeEmperor.AIStates.OnKillAnim)
            {
                return Create(SlimeEmperor.AIStates.OnKillAnim);
            }

            // 目标丢失后重新锁定：旧 AI() 在这里直接 ResetStates，现在走收招口。
            if (ctx.RetargetRequested)
            {
                ctx.RetargetRequested = false;
                if (StateIndex != SlimeEmperor.AIStates.OnKillAnim)
                {
                    return EndAttack(ctx);
                }
            }

            // 超时兜底：状态机永远不许死在一个状态里；卡在王冠形态时顺手还原。
            if (Counter++ > TimeoutFrames)
            {
                if (ctx.CrownForm)
                {
                    ctx.EnterSlimeForm();
                }

                return EndAttack(ctx);
            }

            IVaultState<SlimeEmperorContext> next = AuthorityUpdate(machine, ctx);
            EnteredBeat = -1;
            return next;
        }

        /// <summary>仅权威端：弹幕生成、召唤、掷骰；返回下一状态或 null。</summary>
        protected abstract IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx);

        #region 热字段：跳跃机与手感缩放

        /// <summary>A 跳跃机主状态、B 跳跃机计时、C/D 手感缩放。四个量全招式共用，放在基类里写读，客户端中途加入也能接上同一拍。</summary>
        public override void WriteHot(SlimeEmperorContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = ctx.JumpState;
            ctx.Hot[CoraliteBossHotSlots.B] = ctx.JumpTimer;
            ctx.Hot[CoraliteBossHotSlots.C] = ctx.Scale.X;
            ctx.Hot[CoraliteBossHotSlots.D] = ctx.Scale.Y;
        }

        public override void ReadHot(SlimeEmperorContext ctx)
        {
            base.ReadHot(ctx);
            ctx.JumpState = (int)ctx.Hot[CoraliteBossHotSlots.A];
            ctx.JumpTimer = (int)ctx.Hot[CoraliteBossHotSlots.B];
            ctx.Scale = new Vector2(ctx.Hot[CoraliteBossHotSlots.C], ctx.Hot[CoraliteBossHotSlots.D]);
        }

        #endregion

        #region 公共小件

        /// <summary>按 id 建状态；未注册返回 null（调用方要兜底）。</summary>
        protected static IVaultState<SlimeEmperorContext> Create(SlimeEmperor.AIStates id)
            => VaultStateRegistry<SlimeEmperorContext>.Create((int)id);

        /// <summary>收招：经 hub 的唯一提交口选下一招（<see cref="SlimeEmperorDirector.HubFrames"/> = 0 时不多占一帧）。</summary>
        protected static IVaultState<SlimeEmperorContext> EndAttack(SlimeEmperorContext ctx)
            => SlimeEmperorHubState.EndAttack(ctx);

        /// <summary>
        /// 形变一拍：先插值、再判断是否到位，到位就换到 <paramref name="nextBeat"/> 并返回 true。<br/>
        /// <paramref name="done"/> 是<b>实参</b>，在插值之前就求好了——旧 <c>ScaleToTarget</c> 把判据当参数传，
        /// 求值顺序正是“先判后插”，这里靠同样的写法一帧不差地保住它。
        /// </summary>
        protected bool ScaleBeat(SlimeEmperorContext ctx, float targetX, float targetY, float lerp, bool done, int nextBeat)
        {
            ctx.ScaleTo(targetX, targetY, lerp);
            if (!done)
            {
                return false;
            }

            EnterBeat(ctx, nextBeat);
            return true;
        }

        /// <summary>
        /// 与某个锚点保持给定距离：近了反推、远了追进，速度封顶。两段王冠绕圈与泰山压顶的头顶追踪共用。<br/>
        /// 两端同跑（位置与目标都是同步量），所以客户端的轨迹与服务端一致，只靠纠偏器吃掉相位差。
        /// 沿用旧值 AI.CrownStrike.cs:83-95
        /// </summary>
        protected static void KeepDistance(SlimeEmperorContext ctx, Vector2 anchor, float radius, float accel, float maxSpeed)
        {
            NPC npc = ctx.Npc;
            Vector2 targetVec = anchor - npc.Center;
            Vector2 dir = targetVec.SafeNormalize(Vector2.Zero);

            if (targetVec.Length() < radius)
            {
                npc.velocity -= dir * accel;
            }
            else
            {
                npc.velocity += dir * accel;
            }

            if (npc.velocity.Length() > maxSpeed)
            {
                npc.velocity = Vector2.Normalize(npc.velocity) * maxSpeed;
            }

            ctx.DeclareDirect();
        }

        /// <summary>弹幕出生位：本体中心附近 (宽/3, 高/3) 的随机抖动。沿用旧值 AI.GelShoot.cs:33</summary>
        protected static Vector2 ProjSpawnPos(SlimeEmperorContext ctx)
            => ctx.Npc.Center + Main.rand.NextVector2Circular(
                ctx.Npc.width / SlimeEmperorDirector.ProjSpawnJitterDiv,
                ctx.Npc.height / SlimeEmperorDirector.ProjSpawnJitterDiv);

        /// <summary>
        /// 向上喷一组弹幕（以 −Y 为基准对称散角，速度固定）；仅权威端。
        /// 尖刺凝胶球（大跳 FTW / 尖刺球 / 王冠冲击收招）与弹弹凝胶球（泰山压顶回弹）共用这一口。
        /// </summary>
        protected static void ShootUpward<T>(SlimeEmperorContext ctx, int count, int damage, float speed, float spread, float knockback)
            where T : ModProjectile
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-spread, spread)) * speed;
                ctx.SpawnHostile<T>(ctx.Npc.GetSource_FromAI(), ProjSpawnPos(ctx), velocity, damage, knockback, ctx.Npc.target);
            }
        }

        /// <summary>
        /// 在给定位置附近撒 N 个弹力球；仅权威端。<br/>
        /// 生成包在 <c>NewNPC</c> 内部就已发出，之后改的速度必须自带 <c>netUpdate</c> 才能到客户端。沿用旧值 SlimeEmperor.cs:467
        /// </summary>
        protected static void SpawnElasticBalls(SlimeEmperorContext ctx, Vector2 center, int count, Func<Vector2> velocity)
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            NPC boss = ctx.Npc;
            for (int i = 0; i < count; i++)
            {
                Point pos = center.ToPoint();
                pos.X += Main.rand.Next(-boss.width, boss.width);
                pos.Y += Main.rand.Next(-SlimeEmperorDirector.BallScatterOffsetY, SlimeEmperorDirector.BallScatterOffsetY);
                NPC ball = NPC.NewNPCDirect(boss.GetSource_FromAI(), pos.X, pos.Y, ModContent.NPCType<ElasticGelBall>());
                ball.velocity = velocity();
                ball.netUpdate = true;
            }
        }

        /// <summary>本地音效（服务端静音）。</summary>
        protected static void PlaySound(SoundStyle style, SlimeEmperorContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            SoundEngine.PlaySound(style, ctx.Npc.Center);
        }

        /// <summary>
        /// 瞬移蓄力尘（纯本地）：散布半径随进度从 80 收到 20。收拢过程本身就是“马上要瞬移”的预告，
        /// 所以它必须两端都看得见——只按 <c>!Main.dedServ</c> 门控，不按权威端。沿用旧值 AI.BodySlam.cs:64-74
        /// </summary>
        protected static void SpawnTeleportChargeDust(SlimeEmperorContext ctx, float factor)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            float width = SlimeEmperorDirector.TeleportChargeWidth - (factor * SlimeEmperorDirector.TeleportChargeShrink);
            for (int i = 0; i < SlimeEmperorDirector.TeleportChargeDustCount; i++)
            {
                Dust dust = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(width, width), DustID.Teleporter,
                    -npc.velocity * Main.rand.NextFloat(SlimeEmperorDirector.TeleportChargeDustSpeedMin, SlimeEmperorDirector.TeleportChargeDustSpeedMax),
                    newColor: Coralite.MagicCrystalPink,
                    Scale: Main.rand.NextFloat(SlimeEmperorDirector.TeleportDustScaleMin, SlimeEmperorDirector.TeleportDustScaleMax));
                dust.noGravity = true;
            }
        }

        /// <summary>瞬移落地（纯本地）：旧位到新位的一条轨迹尘 + 落点一圈爆散尘。沿用旧值 AI.BodySlam.cs:87-105</summary>
        protected static void SpawnTeleportDust(SlimeEmperorContext ctx, Vector2 from)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            float length = (from - npc.Center).Length();
            Vector2 dir = (npc.Center - from).SafeNormalize(Vector2.Zero);

            for (int i = 0; i < length; i += SlimeEmperorDirector.TeleportTrailStep)
            {
                int type = Main.rand.Next(SlimeEmperorDirector.TeleportTrailTeleporterOdds) switch
                {
                    0 => DustID.Teleporter,
                    _ => DustID.TintableDust
                };
                Dust dust = Dust.NewDustPerfect(from + (dir * i) + Main.rand.NextVector2Circular(SlimeEmperorDirector.TeleportTrailScatter, SlimeEmperorDirector.TeleportTrailScatter),
                    type, dir * Main.rand.NextFloat(SlimeEmperorDirector.TeleportTrailSpeedMin, SlimeEmperorDirector.TeleportTrailSpeedMax),
                    SlimeEmperorDirector.GelDustAlpha, SlimeEmperorDirector.GelDustColor,
                    Main.rand.NextFloat(SlimeEmperorDirector.TeleportTrailScaleMin, SlimeEmperorDirector.TeleportTrailScaleMax));
                dust.noGravity = true;
            }

            for (int i = 0; i < SlimeEmperorDirector.TeleportRingDustCount; i++)
            {
                Dust dust = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(SlimeEmperorDirector.TeleportRingScatter, SlimeEmperorDirector.TeleportRingScatter),
                    DustID.Teleporter,
                    (i * (MathHelper.TwoPi / SlimeEmperorDirector.TeleportRingDustCount)).ToRotationVector2()
                        * Main.rand.NextFloat(SlimeEmperorDirector.TeleportRingSpeedMin, SlimeEmperorDirector.TeleportRingSpeedMax),
                    newColor: Coralite.MagicCrystalPink,
                    Scale: Main.rand.NextFloat(SlimeEmperorDirector.TeleportDustScaleMin, SlimeEmperorDirector.TeleportDustScaleMax));
                dust.noGravity = true;
            }
        }

        /// <summary>凝胶飞沫，形变时撒（纯本地）。沿用旧值 AI.Split.cs:55-66</summary>
        protected static void SpawnSplitGelDust(SlimeEmperorContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            int width = (int)(npc.width * ctx.Scale.X * SlimeEmperorDirector.SplitDustBoxScale);
            int height = (int)(npc.height * ctx.Scale.Y * SlimeEmperorDirector.SplitDustBoxScale);
            for (int i = 0; i < SlimeEmperorDirector.SplitDustCount; i++)
            {
                Dust dust = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(width, height), DustID.TintableDust,
                    Helper.NextVec2Dir(SlimeEmperorDirector.SplitDustSpeedMin, SlimeEmperorDirector.SplitDustSpeedMax),
                    SlimeEmperorDirector.GelDustAlpha, SlimeEmperorDirector.GelDustColor, SlimeEmperorDirector.SplitDustScale);
                dust.noGravity = true;
                dust.velocity *= SlimeEmperorDirector.SplitDustSlow;
            }
        }

        #endregion

        #region 缩进王冠（泰山压顶 / 王冠冲击 / 移位分裂共用的前摇四拍）

        /// <summary>缩进过程中的每帧表现：减速、王冠贴住身体、撒飞沫、帧图在 0..3 滚动。沿用旧值 AI.CrownStrike.cs:194-212</summary>
        protected static void CrownShrinkStep(SlimeEmperorContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DeclareDamp(SlimeEmperorDirector.CrownShrinkDamp);
            ctx.Boss.PinCrownToBody();
            SpawnSplitGelDust(ctx);
            AdvanceRollingFrame(npc);
        }

        /// <summary>帧图在 0..3 之间滚动（缩进王冠与聚合射击蓄力共用）。沿用旧值 AI.CrownStrike.cs:204-211</summary>
        protected static void AdvanceRollingFrame(NPC npc)
        {
            npc.frameCounter++;
            if (npc.frameCounter <= SlimeEmperorDirector.JumpFrameInterval)
            {
                return;
            }

            npc.frameCounter = 0;
            npc.frame.Y++;
            if (npc.frame.Y > SlimeEmperorDirector.CrownShrinkFrameMax)
            {
                npc.frame.Y = 0;
            }
        }

        /// <summary>
        /// 推进缩进四拍（拍号 0..3 必须与状态自己的 Beat 0..3 对齐）：判据在插值<b>之前</b>求值，沿用旧 <c>ScaleToTarget</c> 的求值顺序。<br/>
        /// 四拍跑完时切王冠形态并换到拍 4，返回 true（权威端据此做该拍的生成）。
        /// </summary>
        protected bool UpdateCrownShrinkBeats(SlimeEmperorContext ctx)
        {
            int beat = BeatIndex;
            int timer = beat == 0 ? EntryTimer : Timer;
            if (beat == 0 && timer < 2)
            {
                ctx.Npc.noGravity = true;
            }

            CrownShrinkStep(ctx);

            float axis = SlimeEmperorDirector.CrownShrinkDoneByY[beat] ? ctx.Scale.Y : ctx.Scale.X;
            bool done = axis < SlimeEmperorDirector.CrownShrinkDone[beat] || timer > SlimeEmperorDirector.CrownShrinkBeatTimeout;

            Vector2 target = SlimeEmperorDirector.CrownShrinkTargets[beat];
            ctx.ScaleTo(target.X, target.Y, SlimeEmperorDirector.CrownShrinkLerp);

            if (!done)
            {
                return false;
            }

            //第 2、4 拍收尾各一声挤压音
            if (beat == 1 || beat == 3)
            {
                PlaySound(CoraliteSoundID.QueenSlime_Item154, ctx);
            }

            EnterBeat(ctx, beat + 1);
            if (beat < 3)
            {
                return false;
            }

            ctx.EnterCrownForm();
            ctx.Npc.TargetClosest();
            return true;
        }

        #endregion
    }
}
