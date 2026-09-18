using Coralite.Content.Bosses.ShadowBalls.States;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.Core
{
    /// <summary>
    /// 影子球本体的顶层状态索引，写入 <c>npc.ai[0]</c> 同步。成员与数值沿用旧 <c>ShadowBallStateId</c>（线格式不变），追加 <see cref="Hub"/>。<br/><br/>
    /// <b>有名无实现的 id</b>：下面标注"未实现"的成员<b>没有</b> <c>[VaultState]</c> 注册，
    /// <c>VaultStateRegistry.Create</c> 拿到它们会返回 <see langword="null"/>，而返回 null 等于不切换 = 软锁。
    /// 因此全目录内<b>没有任何一条路径</b>会返回它们：轮换表里没有，<see cref="ShadowBallP1ToP2ExchangeState"/> 的出口也做了 null 兜底。
    /// 保留成员是为了不动线格式，并给作者留下原有的设计位置。
    /// </summary>
    public enum ShadowBallStateId
    {
        OnSpawnAnim = 0,
        /// <summary>死亡演出。<b>未实现</b>：旧 <c>CheckDead</c> 里的整段被作者注释掉了（ShadowBall.cs:232-242），当前直接真死。</summary>
        OnKillAnmi,
        /// <summary>撤退演出（"你给陆大有~"）。<b>未实现</b>：全目录无对应招式体。</summary>
        EscapeAnmi,
        /// <summary>一阶段 → 二阶段的切换演出。已注册，但触发它的 <c>PhaseController</c> 条件被作者注释掉了，当前不可达。</summary>
        P1ToP2Exchange,

        //--------------- 一阶段 ---------------

        /// <summary> 一阶段招式：召唤小影子球 </summary>
        SummonSmallShdowBall,
        /// <summary> 一阶段招式：影之公转 </summary>
        Revolution,
        /// <summary> 一阶段招式：星轨 </summary>
        Starline,
        /// <summary> 一阶段招式：月食 </summary>
        LunarEclipse,
        /// <summary> 一阶段招式：影刺。已注册且招式体完整，但旧轮换表里没有它，当前只能靠作者手动加进池子。 </summary>
        ShadowSpike,
        /// <summary> 一阶段特殊招式：黑暗窥视。<b>未实现</b>：设计文档有整节（§特殊破绽招式 黑暗窥视），代码里没有任何招式体。 </summary>
        DarkSeek,
        /// <summary> 一阶段招式：三层小球环绕后旋转激光 </summary>
        RollingLaser,
        /// <summary> 一阶段招式：红移。招式体是空的（旧 P1.RedShift.cs 只有 TODO），轮换表里也没有它。 </summary>
        RedShift,
        /// <summary> 一阶段招式：蓝移。同 <see cref="RedShift"/>。 </summary>
        BlueShift,

        //--------------- 二阶段 ---------------

        /// <summary> 二阶段招式：跳起斜冲后回旋砍。<b>未实现</b>：旧 <c>[VaultState]</c> 被注释掉，招式体也不存在。 </summary>
        SmashDown,

        /// <summary>连接段 + 唯一提交口（新增；<see cref="ShadowBallDirector.HubFrames"/> 为 0 时通常不驻留）。</summary>
        Hub,
    }

    /// <summary>
    /// 影子球本体状态基类。<br/>
    /// · <c>SharedUpdate</c>：两端同跑——写运动 / 朝向 / 表现声明，推进确定性子拍。<br/>
    /// · <see cref="AuthorityUpdate"/>：仅权威端——弹幕与小球生成、掷骰、编排子球换态，返回下一状态；本类把超时兜底垫在它前面。<br/>
    /// · 收招一律 <see cref="EndAttack"/>，出招一律经 <see cref="ShadowBallHubState.Commit"/>；招式体内不得出现 <c>ChangeState</c>。<br/>
    /// · 热槽 A / B 固定给引力牵引的锚点（每个状态都写，反正定长块里这两格不要钱），子类自用槽从 C 起。
    /// </summary>
    public abstract class ShadowBallStateBase : CoraliteBossState<ShadowBallContext>
    {
        public abstract ShadowBallStateId StateIndex { get; }

        public override int StateId => (int)StateIndex;

        /// <summary>超时兜底帧数；演出态可覆盖为更大值。</summary>
        protected virtual int TimeoutFrames => ShadowBallDirector.StateTimeoutFrames;

        /// <summary>
        /// 引力牵引的目标锚点。两端在同一拍各自确定性算出同一个点（不再依赖跨机器不一致的弹幕下标），
        /// 同时随热槽 A / B 过线，让中途加入的客户端也能接着牵引跑完。
        /// </summary>
        protected Vector2 GravityAnchor { get; set; }

        protected sealed override IVaultState<ShadowBallContext> ServerUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            // 超时兜底：状态机永远不许死在一个状态里；收招不留残速（D2）。
            if (Counter++ > TimeoutFrames)
            {
                ctx.Npc.velocity *= ShadowBallDirector.TimeoutDamp;
                return EndAttack(ctx);
            }

            return AuthorityUpdate(machine, ctx);
        }

        /// <summary>仅权威端：弹幕 / 小球生成、掷骰、编排子球；返回下一状态或 null。</summary>
        protected abstract IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx);

        public override void WriteHot(ShadowBallContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = GravityAnchor.X;
            ctx.Hot[CoraliteBossHotSlots.B] = GravityAnchor.Y;
        }

        public override void ReadHot(ShadowBallContext ctx)
        {
            base.ReadHot(ctx);
            GravityAnchor = new Vector2(ctx.Hot[CoraliteBossHotSlots.A], ctx.Hot[CoraliteBossHotSlots.B]);
        }

        #region 公共小件

        /// <summary>按 id 建状态；未注册返回 null（调用方必须兜底，别把 null 直接返回出去）。</summary>
        protected static IVaultState<ShadowBallContext> Create(ShadowBallStateId id)
            => VaultStateRegistry<ShadowBallContext>.Create((int)id);

        /// <summary>收招：经 hub 的唯一提交口选下一招。</summary>
        protected static IVaultState<ShadowBallContext> EndAttack(ShadowBallContext ctx)
            => ShadowBallHubState.EndAttack(ctx);

        /// <summary>
        /// 落点预判：在玩家当前位置上叠"沿玩家速度方向的提前量"与"沿自身→玩家方向的越位量"，
        /// 两者都按与玩家的距离归一化并夹在 <see cref="ShadowBallDirector.RevolutionLeadMin"/> 与 1 之间。<br/>
        /// 旧 P1.Revolution.cs:38-46 与 P1.ShadowSpike.cs:38-45 是同一份写法（后者只取 X 分量）。
        /// </summary>
        protected static Vector2 PredictTarget(ShadowBallContext ctx, float leadLength, float overshootLength)
        {
            Player target = ctx.Target;
            Vector2 aimPos = target.Center;
            float distance = Vector2.Distance(ctx.Npc.Center, target.Center);

            if (target.velocity.LengthSquared() > ShadowBallDirector.MovingTargetSpeedSq)
            {
                aimPos += target.velocity.SafeNormalize(Vector2.Zero)
                    * Helper.Clamp(distance / ShadowBallDirector.RevolutionLeadRange, ShadowBallDirector.RevolutionLeadMin, 1f) * leadLength;
            }

            aimPos += (target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero)
                * Helper.Clamp(distance / ShadowBallDirector.RevolutionOvershootRange, ShadowBallDirector.RevolutionLeadMin, 1f) * overshootLength;

            return aimPos;
        }

        /// <summary>
        /// 召回几个小球：与玩家的 X 距离每 <paramref name="perLength"/> 像素多叫一个，再加基数，最后被现有数量截断。
        /// 旧 P1.Revolution.cs:74-80 与 P1.ShadowSpike.cs:71-76 的同一份算式。
        /// </summary>
        protected static int CallBackCount(ShadowBallContext ctx, int smallBallCount, float perLength)
        {
            float length = MathF.Abs(ctx.Target.Center.X - ctx.Npc.Center.X);
            int howMany = (int)(length / perLength) + ShadowBallDirector.RevolutionBaseCount;
            return howMany > smallBallCount ? smallBallCount : howMany;
        }

        #endregion
    }
}
