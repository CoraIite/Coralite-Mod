using Coralite.Content.Bosses.ModReinforce.PurpleVolt.States;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core
{
    /// <summary>
    /// 兹雷龙状态基类。<br/>
    /// · <c>SharedUpdate</c>：两端同跑——运动数学、帧图、子拍推进、粒子音效（<c>!Main.dedServ</c>）。<br/>
    /// · <see cref="AuthorityUpdate"/>：仅权威端——弹幕 / NPC 生成、掷骰，并<b>返回下一状态</b>；
    ///   本类把死亡请求、紫伏击穿请求与超时兜底垫在它前面。<br/>
    /// · 收招一律 <see cref="EndAttack"/>，出招一律经 <see cref="ZacurrentHubState.Commit"/>；招式体内不得出现 <c>ChangeState</c>。<br/><br/>
    /// 招内计时用 <see cref="ZacurrentDragonContext.Timer"/>（ai[3]）而不是基座的 <see cref="VaultState{T}.Timer"/>：
    /// 招式体“帧末自增”的旧语义与基座“帧首自增”不同，沿用 ai[3] 能让搬过来的 <c>Timer == N</c> 拍点一帧不差，
    /// 而且 ai[3] 由原版 SyncNPC 同步，比热字段多一层保障。基座的 Timer / Counter 在这里只做超时兜底与帧差对账。
    /// </summary>
    public abstract class ZacurrentStateBase : CoraliteBossState<ZacurrentDragonContext>
    {
        public abstract ZacurrentDragon.AIStates StateIndex { get; }

        public override int StateId => (int)StateIndex;

        /// <summary>超时兜底帧数（D2）。默认值远大于任何单招的真实时长，只在招式被卡住时救场。</summary>
        protected virtual int TimeoutFrames => ZacurrentDirector.StateTimeoutFrames;

        public override void OnEnter(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            // 基座：计数器清零；权威端另外清 ai[2]/ai[3] 并掷出新的 AttackSeed（客户端不清、不掷）。
            base.OnEnter(machine, ctx);

            // 在 roll 之后刷新，保证两端从同一份 AttackSeed 派生同一条随机序列。
            ctx.RefreshAttackRandom();
            ctx.AttackFinished = false;

            if (VaultUtils.isClient)
            {
                // 客户端被 NetSync 换态：ai[2]/ai[3] 与热字段由包决定，这里只把视觉 / 判定布尔归位。
                ctx.ResetVisualFlags();
                return;
            }

            ctx.ResetAttackFields();
            OnStateEnter(ctx);

            // OnStateEnter 只在权威端跑，它掷的那一两次骰会把权威端的随机流推得比客户端靠前，
            // 之后招式体两端同跑的 AttackRandFloat 就会取到不同的值（短冲的冲刺角就是这么歪掉的）。
            // 这里再刷一次：两端都从同一 AttackSeed 的第 0 次抽取开始跑招式体。进招初始值本身由热字段过线。
            ctx.RefreshAttackRandom();
        }

        /// <summary>仅权威端：进入状态时设置确定性初始值（默认无）。</summary>
        protected virtual void OnStateEnter(ZacurrentDragonContext ctx) { }

        protected sealed override IVaultState<ZacurrentDragonContext> ServerUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            // CheckDead / ModifyIncomingHit 只登记请求，换态统一从这里经返回值走（客户端读 ai[0] 跟随）。
            if (ctx.KillRequested && StateIndex != ZacurrentDragon.AIStates.onKillAnim)
            {
                return Create(ZacurrentDragon.AIStates.onKillAnim);
            }

            if (ctx.BreakRequested && StateIndex is not ZacurrentDragon.AIStates.Break and not ZacurrentDragon.AIStates.onKillAnim)
            {
                ctx.BreakRequested = false;
                ctx.MarkDecision();
                return Create(ZacurrentDragon.AIStates.Break);
            }

            // 超时兜底：状态机永远不许死在一个状态里；收招不留残速。
            if (Counter++ > TimeoutFrames)
            {
                ctx.Npc.velocity *= ZacurrentDirector.TimeoutBrake;
                return EndAttack(ctx);
            }

            return AuthorityUpdate(machine, ctx);
        }

        /// <summary>仅权威端：生成、掷骰、返回下一状态（null = 留在本状态）。</summary>
        protected virtual IVaultState<ZacurrentDragonContext> AuthorityUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
            => null;

        #region 热字段

        public override void WriteHot(ZacurrentDragonContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = ctx.Recorder;
            ctx.Hot[CoraliteBossHotSlots.B] = ctx.Recorder2;
            ctx.Hot[CoraliteBossHotSlots.C] = ctx.Combo;
        }

        public override void ReadHot(ZacurrentDragonContext ctx)
        {
            base.ReadHot(ctx);
            ctx.Recorder = ctx.Hot[CoraliteBossHotSlots.A];
            ctx.Recorder2 = ctx.Hot[CoraliteBossHotSlots.B];
            ctx.Combo = (int)ctx.Hot[CoraliteBossHotSlots.C];
        }

        #endregion

        #region 公共小件

        /// <summary>按 id 建状态；未注册返回 null（调用方要兜底）。</summary>
        protected static IVaultState<ZacurrentDragonContext> Create(ZacurrentDragon.AIStates id)
            => VaultStateRegistry<ZacurrentDragonContext>.Create((int)id);

        /// <summary>收招：经 hub 的唯一提交口选下一招。</summary>
        protected static IVaultState<ZacurrentDragonContext> EndAttack(ZacurrentDragonContext ctx)
            => ZacurrentHubState.NextAfterAttack(ctx);

        /// <summary>
        /// 嘴前汇聚尘：散布半径随蓄力进度收束，这是所有"张嘴吐东西"招式共用的预告。纯表现。
        /// <paramref name="ramp"/> 为蓄满所需帧数。旧 AI.ElectricBall.cs:24-31
        /// </summary>
        public static void MouthGatherDust(ZacurrentDragonContext ctx, float ramp)
        {
            if (VaultUtils.isServer)
            {
                return;
            }

            Vector2 pos = ctx.Boss.GetMousePos();
            float edge = (ZacurrentDirector.BallGatherEdgeMax
                - (ZacurrentDirector.BallGatherEdgeShrink * Math.Clamp(ctx.Timer / ramp, 0, 1))) / 2;
            Vector2 center = pos + Helper.NextVec2Dir(edge - 1, edge);
            Dust dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail,
                (pos - center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(ZacurrentDirector.GravitationDustSpeedMin, ZacurrentDirector.GravitationDustSpeedMax),
                newColor: ZacurrentDragon.ZacurrentDustPurple,
                Scale: Main.rand.NextFloat(ZacurrentDirector.GravitationDustScaleMin, ZacurrentDirector.GravitationDustScaleMax));
            dust.noGravity = true;
        }

        /// <summary>
        /// 蓄力期的慢速贴近：X 出死区才平移、Y 在龙下方时爬升否则出死区才升降，全程按 Y 速度摆姿态。
        /// 旧 AI.ElectricBall.cs:33-56（Z 电球同款）
        /// </summary>
        public static void ChaseTargetWhileCharging(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            npc.QuickSetDirection();
            boss.GetLengthToTargetPos(ctx.Target.Center, out float xLength, out float yLength);

            if (xLength > ZacurrentDirector.BallChaseDeadZoneX)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.X, npc.direction, ZacurrentDirector.BallChaseSpeedX,
                    ZacurrentDirector.BallChaseAccelX, ZacurrentDirector.BallChaseTurnX, ZacurrentDirector.BallChaseDamp);
            }
            else
            {
                npc.velocity.X *= ZacurrentDirector.BallChaseDamp;
            }

            if (npc.directionY < 0)
            {
                boss.FlyingUp(ZacurrentDirector.GravitationRiseAccel, ZacurrentDirector.GravitationRiseMax, ZacurrentDirector.GravitationRiseDamp);
            }
            else if (yLength > ZacurrentDirector.BallChaseDeadZoneY)
            {
                Helper.Movement_SimpleOneLine(ref npc.velocity.Y, npc.directionY, ZacurrentDirector.BallChaseSpeedY,
                    ZacurrentDirector.BallChaseAccelY, ZacurrentDirector.BallChaseTurnY, ZacurrentDirector.BallChaseDamp);
                boss.FlyingFrame();
            }
            else
            {
                npc.velocity.Y *= ZacurrentDirector.BallChaseDamp;
                boss.FlyingFrame();
            }

            boss.SetRotationNormally();
            ctx.DeclareDirect();
        }

        /// <summary>
        /// 输出期的距离带巡航：贴脸就后退、拉远就追上来、中距衰减——让"站桩连射"始终保持在可读距离。
        /// 旧 AI.ElectricBall.cs:232-248（Z 电球同款）
        /// </summary>
        public static void CruiseInBand(ZacurrentDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            float distance = npc.Center.Distance(ctx.Target.Center);

            if (distance < ZacurrentDirector.BallFlyNearDistance)
            {
                if (npc.velocity.Length() < ZacurrentDirector.BallFlyNearSpeedCap)
                {
                    npc.velocity += (npc.Center - ctx.Target.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.BallFlyAccel;
                }
            }
            else if (distance > ZacurrentDirector.BallFlyFarDistance)
            {
                if (npc.velocity.Length() < ZacurrentDirector.BallFlyFarSpeedCap)
                {
                    npc.velocity += (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ZacurrentDirector.BallFlyAccel;
                }
            }
            else
            {
                npc.velocity *= ZacurrentDirector.BallChaseDamp;
            }

            ctx.DeclareDirect();
        }

        /// <summary>出手时的镜头冲击 + 背景压暗，组合弹幕共用。纯本地。旧 AI.ElectricBall.cs:208-215</summary>
        public static void VolleyImpact(ZacurrentDragonContext ctx, Vector2 dir)
        {
            if (VaultUtils.isServer)
            {
                return;
            }

            PunchCameraModifier modifier = new PunchCameraModifier(ctx.Npc.Center, dir,
                ZacurrentDirector.BallShakeStrength, ZacurrentDirector.BallShakeVibration, ZacurrentDirector.BallShakeFrames, ZacurrentDirector.ShakeFalloffDistance);
            Main.instance.CameraModifiers.Add(modifier);

            ZacurrentDragon.SetBackgroundLight(ZacurrentDirector.BallSkyLight, ZacurrentDirector.BallSkySeadeFrames, ZacurrentDirector.BallSkyExchange);
        }

        #endregion
    }

    /// <summary>
    /// 招式态：<c>SharedUpdate</c> 两端跑招式体（返回“是否结束”写进 <see cref="ZacurrentDragonContext.AttackFinished"/>），
    /// 权威端据此经 hub 提交下一招。招式体内的生成与掷骰自己带权威端守卫。
    /// </summary>
    public abstract class ZacurrentAttackState : ZacurrentStateBase
    {
        /// <summary>招式体：两端同跑，返回 true 表示整招结束。</summary>
        protected abstract bool RunAttack(ZacurrentDragonContext ctx);

        protected override void SharedUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
            => ctx.AttackFinished = RunAttack(ctx);

        protected override IVaultState<ZacurrentDragonContext> AuthorityUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            // 不在这里统一刹速：每个招式的最后一拍都是自带的后摇（FlyingFrame + 衰减），
            // 额外乘一个系数会改变旧手感（D10）。超时兜底那条路才刹，那是救场路径。
            return ctx.AttackFinished ? EndAttack(ctx) : null;
        }
    }
}
