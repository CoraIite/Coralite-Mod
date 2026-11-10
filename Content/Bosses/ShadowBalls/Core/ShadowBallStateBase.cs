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
    public enum ShadowBallStates
    {
        OnSpawnAnim = 0,
        OnKillAnmi,
        /// <summary>"你给陆大有~"</summary>
        EscapeAnmi,
        /// <summary>切换阶段动画</summary>
        P1ToP2Exchange,
        /// <summary>连接段</summary>
        Hub,

        //--------------- 一阶段 ---------------

        /// <summary> 一阶段招式：召唤小影子球 </summary>
        SummonSmallShdowBall,
        ///// <summary> 一阶段招式：影之公转 </summary>
        //Revolution,
        /// <summary> 一阶段招式：星轨 </summary>
        Starline,
        /// <summary> 一阶段招式：流星雨（其实是星轨的变种招式） </summary>
        MeteorShower,
        /// <summary> 一阶段招式：月食 </summary>
        LunarEclipse,
        /// <summary> 一阶段招式：影刺 </summary>
        ShadowSpike,
        ///// <summary> 一阶段特殊招式：黑暗窥视 </summary>
        //DarkSeek,
        /// <summary> 一阶段招式：三层小球环绕后旋转激光 </summary>
        RollingLaser,
        /// <summary> 一阶段招式：红移 </summary>
        RedShift,
        /// <summary> 一阶段招式：蓝移 </summary>
        BlueShift,
        /// <summary>一阶段招式：引力</summary>
        Gravity,
        /// <summary>一阶段招式：伽玛射线暴</summary>
        GammaRayBurst,
        /// <summary>一阶段调试</summary>
        P1Test,

        //--------------- 二阶段 ---------------

        /// <summary>二阶段招式：水银爆发</summary>
        MucuryBurst,
        /// <summary>二阶段招式：不稳定喷发</summary>
        UnstableEruption,
        /// <summary>二阶段招式：多方向喷发</summary>
        MultiEruption
    }

    /// <summary>
    /// 影子球本体状态基类。<br/>
    /// · <c>SharedUpdate</c>：两端同跑——写运动 / 朝向 / 表现声明，推进确定性子拍。<br/>
    /// · <see cref="AuthorityUpdate"/>：仅权威端——弹幕与小球生成、掷骰、编排子球换态，返回下一状态；本类把超时兜底垫在它前面。<br/>
    /// · 收招一律 <see cref="EndAttack"/>，出招一律经 <see cref="ShadowBallHubState.Commit"/>；招式体内不得出现 <c>ChangeState</c>。<br/>
    /// · 热槽 A / B 默认给引力牵引的锚点，子类自用槽从 C 起；伽玛射线暴到位后通过覆写读写复用 A 记录距离。
    /// </summary>
    public abstract class ShadowBallStateBase : CoraliteBossState<ShadowBallContext>
    {
        public abstract ShadowBallStates StateIndex { get; }

        public override int StateId => (int)StateIndex;

        /// <summary>
        /// 招式最大时间
        /// </summary>
        protected virtual int TimeoutFrames => 60*30;

        protected sealed override IVaultState<ShadowBallContext> ServerUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            if (Counter++ > TimeoutFrames)
            {
                ctx.Npc.velocity *= 0.6f;
                return EndAttack(ctx);
            }

            return AuthorityUpdate(machine, ctx);
        }

        /// <summary>仅权威端：弹幕 / 小球生成、掷骰、编排子球；返回下一状态或 null。</summary>
        protected abstract IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx);

        #region 公共小件

        /// <summary>按 id 建状态；未注册返回 null（调用方必须兜底，别把 null 直接返回出去）。</summary>
        protected static IVaultState<ShadowBallContext> Create(ShadowBallStates id)
            => VaultStateRegistry<ShadowBallContext>.Create((int)id);

        /// <summary>收招：经 hub 的唯一提交口选下一招。</summary>
        protected static IVaultState<ShadowBallContext> EndAttack(ShadowBallContext ctx)
            => ShadowBallHubState.EndAttack(ctx);

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
