using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.Core
{
    /// <summary>
    /// 小影子球顶层 FSM 状态 ID，写入 <c>npc.ai[1]</c> 同步。成员与数值沿用旧 <c>SmallShadowBallStateId</c>（线格式不变）。<br/>
    /// 小球不自己选招 —— 出什么招全由本体在权威端下令，<see cref="Idle"/> 就是它的连接段兼收招出口。
    /// </summary>
    public enum SmallShadowBallStateId
    {
        OnSpawnAnim,
        /// <summary>死亡演出。<b>未实现</b>：没有 <c>[VaultState]</c> 注册，也没有任何路径返回它；
        /// 只被本体的名册扫描当作"这颗球不算数了"的标记读（ShadowBall.cs 的 GetSmallBalls）。</summary>
        OnKillAnmi,
        /// <summary>待机环绕，同时是所有招式的收招出口。</summary>
        Idle,

        /// <summary> 一阶段招式：影之公转 </summary>
        Revolution,
        /// <summary> 一阶段招式：星轨 </summary>
        Starline,
        /// <summary> 一阶段招式：月食 </summary>
        LunarEclipse,
        /// <summary>
        /// 一阶段招式：照影。招式体完整且已注册，但<b>当前没有任何下令方</b> ——
        /// 本体那一半（<c>ShadowBall.ShadowShoot</c> 与 <c>ShadowBallStateId.ShadowShoot</c>）被作者在 51b88bc4 里连同设计文档整节一起砍掉了。
        /// </summary>
        ShadowShoot,
        /// <summary> 一阶段招式：影刺 </summary>
        ShadowSpike,
        /// <summary> 一阶段特殊招式：黑暗窥视。<b>未实现</b>：没有招式体也没有注册，本体那边同样只有枚举名。 </summary>
        DarkSeek,
        /// <summary> 一阶段招式：三层小球环绕后旋转激光 </summary>
        RollingLaser,
    }

    /// <summary>
    /// 小影子球状态基类。与本体的 <see cref="ShadowBallStateBase"/> 同形状，只是挂在自己的上下文上：<br/>
    /// · <see cref="SharedUpdate"/>：两端同跑——写位置 / 朝向 / 表现，推进确定性子拍。<br/>
    /// · <see cref="AuthorityUpdate"/>：仅权威端——弹幕生成与转移，本类把超时兜底垫在它前面。<br/>
    /// · <see cref="Timer"/> 在 <see cref="OnUpdate"/> 开头自增，招式体第一帧读到 1（与基座同约定；
    ///   旧包壳态是"跑完招式体再 <c>Timer++</c>"，所以旧招式体第一帧读到的是 0 —— 搬过来的 <c>Timer == 0</c> 型拍点都改成了 1）。<br/>
    /// · 收招一律 <see cref="EndAttack"/>（回待机）；招式体内不得出现 <c>ChangeState</c>。
    /// </summary>
    public abstract class SmallShadowBallStateBase : VaultState<SmallShadowBallContext>
    {
        /// <summary>一次性演出拍的追帧宽限，口径与基座 <c>CoraliteBossState.CueCatchUpGrace</c> 一致。</summary>
        protected const int CueCatchUpGrace = 20;

        public abstract SmallShadowBallStateId StateIndex { get; }

        public override int StateId => (int)StateIndex;

        /// <summary>招式内子拍号（随热槽 Beat 过线）。</summary>
        protected int BeatIndex { get; set; }

        /// <summary>超时兜底帧数。</summary>
        protected virtual int TimeoutFrames => SmallShadowBallDirector.StateTimeoutFrames;

        /// <summary>本状态跑起来需要主人；拿不到主人时本类直接空转（本体消失时 <c>CheckActive</c> 会收走这颗球）。</summary>
        public sealed override IVaultState<SmallShadowBallContext> OnUpdate(
            VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            Timer++;

            if (!ctx.OwnerIndex.GetNPCOwner<ShadowBall>(out NPC ownerNpc) || ownerNpc.ModNPC is not ShadowBall owner)
            {
                return null;
            }

            SharedUpdate(machine, ctx, owner);

            if (VaultUtils.isClient)
            {
                return null;
            }

            // 超时兜底：小球永远不许死在一个招式里；收招回待机（D2）。
            if (Counter++ > TimeoutFrames)
            {
                return EndAttack();
            }

            return AuthorityUpdate(machine, ctx, owner);
        }

        /// <summary>双端执行：写位置 / 朝向 / 表现，推进确定性子拍。</summary>
        protected virtual void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner) { }

        /// <summary>仅权威端：弹幕生成与转移；返回下一状态或 null。</summary>
        protected virtual IVaultState<SmallShadowBallContext> AuthorityUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
            => null;

        public override void OnEnter(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            BeatIndex = 0;

            // 客户端被 NetSync 被动换态时只清本地量：Timer / Counter / Beat 随后由收养路径从热槽恢复。
            if (VaultUtils.isClient)
            {
                return;
            }

            ctx.MarkDecision();
        }

        /// <summary>换拍：写拍号、Timer 清零、权威端标决策点。</summary>
        protected void SwitchBeat(SmallShadowBallContext ctx, int beat)
        {
            BeatIndex = beat;
            Timer = 0;
            ctx.MarkDecision();
        }

        /// <summary>权威端每包前把热字段写进热槽；子类先调 base 再写自用槽 A..H。</summary>
        public virtual void WriteHot(SmallShadowBallContext ctx)
        {
            ctx.Hot[CoraliteBossHotSlots.Timer] = Timer;
            ctx.Hot[CoraliteBossHotSlots.Counter] = Counter;
            ctx.Hot[CoraliteBossHotSlots.Beat] = BeatIndex;
        }

        /// <summary>客户端收包 / 换态后收养热字段；子类先调 base 再读自用槽。</summary>
        public virtual void ReadHot(SmallShadowBallContext ctx)
        {
            Timer = AdoptTimer(Timer, ctx.Hot[CoraliteBossHotSlots.Timer]);
            Counter = (int)ctx.Hot[CoraliteBossHotSlots.Counter];
            BeatIndex = (int)ctx.Hot[CoraliteBossHotSlots.Beat];
        }

        /// <summary>计时器收养带容差；口径与基座 <c>CoraliteBossState.AdoptTimer</c> 一致。</summary>
        protected static int AdoptTimer(int local, float synced, int tolerance = CoraliteBossContext.TimerAdoptTolerance)
        {
            int server = (int)synced;
            return Math.Abs(server - local) > tolerance ? server : local;
        }

        /// <summary>按 id 建状态；未注册返回 null（调用方要兜底）。</summary>
        protected static IVaultState<SmallShadowBallContext> Create(SmallShadowBallStateId id)
            => VaultStateRegistry<SmallShadowBallContext>.Create((int)id);

        /// <summary>收招：回待机。小球不自己选招，所以这里没有提交口 —— 选招的唯一提交口在本体的 hub 上。</summary>
        protected static IVaultState<SmallShadowBallContext> EndAttack()
            => Create(SmallShadowBallStateId.Idle);

        /// <summary>
        /// 招式中使用的简易趋近移动（两端同跑）。旧 <c>SmallShadowBall.MoveToAttackPosition</c>（SmallShadowBall.cs:1157-1164）。
        /// </summary>
        protected static void MoveToAttackPosition(SmallShadowBallContext ctx, Vector2 target, float amount = SmallShadowBallDirector.MoveToLerp)
        {
            NPC npc = ctx.Npc;
            npc.velocity = Vector2.Lerp(npc.velocity,
                (target - npc.Center).SafeNormalize(Vector2.Zero) * SmallShadowBallDirector.MoveToSpeed, amount);

            if (npc.velocity.LengthSquared() > 0.01f)
            {
                npc.rotation = npc.rotation.AngleLerp(npc.velocity.ToRotation(), SmallShadowBallDirector.MoveToRotationLerp);
            }
        }
    }
}
