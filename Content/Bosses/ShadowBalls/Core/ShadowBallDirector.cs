using Coralite.Helpers;

namespace Coralite.Content.Bosses.ShadowBalls.Core
{
    /// <summary>
    /// 影子球（本体）的共用参数与非状态参数；仅由状态中单个方法使用的常量就近定义在该方法内。<br/>
    /// 每条常量的注释写明"为什么是这个数"；查不到原因的直接标"沿用旧值 &lt;文件&gt;:&lt;行&gt;"，不编造理由。<br/>
    /// 旧行号对应重构前的 <c>Others.Anmations.cs</c> / <c>P1.*.cs</c> / <c>ShadowBall.cs</c>。
    /// </summary>
    internal static class ShadowBallDirector
    {
        #region 全局 / 主控

        /// <summary>脱战下沉加速度。沿用旧值 ShadowBall.cs:360。</summary>
        public const float DespawnGravity = 0.25f;

        /// <summary>脱战请求离场的帧数。沿用旧值 ShadowBall.cs:359。</summary>
        public const int DespawnEncourageFrames = 10;

        /// <summary>自发光颜色（紫）。沿用旧值 ShadowBall.cs:365。</summary>
        public static readonly Vector3 SelfLight = new Vector3(1f, 0.5f, 1.8f);

        /// <summary>锁环渐进插值每帧的加法项与乘法项。沿用旧值 ShadowBall.cs:418。</summary>
        public const float LockLerpStep = 0.005f;
        public const float LockLerpGain = 1.03f;

        /// <summary>锁环计时器的回绕上限（60×60×60 帧 = 1 小时），避免长时间战斗后浮点精度塌掉。沿用旧值 ShadowBall.cs:424。</summary>
        public const int LockTimerWrap = 60 * 60 * 60;

        /// <summary>
        /// 状态超时兜底帧数。任何招式卡住超过这个时长一律收招（D2）。
        /// 取 1800 = 30 秒：比最长的招式（星轨 10 秒攻击段 + 前后摇，约 13 秒）宽裕一倍以上，正常战斗永远碰不到。
        /// </summary>
        public const int StateTimeoutFrames = 60 * 30;

        /// <summary>超时收招时的速度衰减，避免带着残速进下一招（D2）。与其它已迁移 boss 同口径取 0.6。</summary>
        public const float TimeoutDamp = 0.6f;

        /// <summary>
        /// hub 的驻留帧数。旧代码收招当帧即切下一招、零间隔（<c>SwitchP1State</c> 里没有等待），
        /// 所以取 0——引入 hub 不得改变节奏。用 <c>static readonly</c> 而非 <c>const</c>：
        /// 写成 const 0 会让 hub 的喘息分支编译期不可达（CS0162），日后调大时这条分支却已被删。
        /// </summary>
        public static readonly int HubFrames = 0;

        /// <summary>
        /// 选招时"现有小球数量不足多少就先补球"的缺口门槛（普通/专家/大师/FTW）。沿用旧值 ShadowBall.cs:581。
        /// </summary>
        public static int SummonGapThreshold() => Helper.ScaleValueForDiffMode(6, 5, 4, 1);

        #endregion

        #region 召唤小球 SummonSmallShdowBall

        /// <summary>减速段锁环张开到的半径倍率。沿用旧值 P1.SummonSmallBall.cs:57。</summary>
        public const float SummonLockExpand = 1.3f;

        /// <summary>推锁段弹出后、小球出生动画开始时的锁环半径倍率。沿用旧值 P1.SummonSmallBall.cs:69,70,74,78。</summary>
        public const float SummonLockPush = 1.5f;

        #endregion

        #region 引力移动（公转 / 星轨 / 月食 / 影刺共用）

        /// <summary>牵引就位时自身的速度衰减比例。沿用旧值 ShadowBall.cs:940 的默认参数。</summary>
        public const float GravitySlowDown = 0.5f;

        /// <summary>引力加速的爬坡时长；<c>X3Ease(Timer / 45)</c> 决定加速度。沿用旧值 ShadowBall.cs:968。</summary>
        public const float GravityRampFrames = 45f;

        /// <summary>引力加速的每帧增量与常数项。沿用旧值 ShadowBall.cs:968。</summary>
        public const float GravityAccel = 1.7f;
        public const float GravityAccelBase = 0.01f;

        /// <summary>引力移动的速度上限。沿用旧值 ShadowBall.cs:970。</summary>
        public const float GravityMaxSpeed = 45f;

        /// <summary>引力移动期间朝向锚点的转向插值。沿用旧值 ShadowBall.cs:975。</summary>
        public const float GravityRotationLerp = 0.2f;

        /// <summary>
        /// 引力移动的超时兜底帧数。旧代码没有兜底：牵引弹幕若没生成成功就会一直飞下去（D2 要求每个状态都有超时路径）。
        /// 取 240 = 4 秒：全速 45 px/f 跑完 4 秒 ≈ 112 格，比任何一次引力位移（最远 450 px ≈ 28 格）都长得多。
        /// </summary>
        public const int GravityTimeoutFrames = 240;

        #endregion

        #region 影之公转 Revolution

        /// <summary>落点预判：玩家速度平方大于这个值才算"玩家在动"。沿用旧值 P1.Revolution.cs:40。</summary>
        public const float MovingTargetSpeedSq = 1f;

        /// <summary>沿玩家速度方向预判的距离归一化分母与下限。沿用旧值 P1.Revolution.cs:42-43。</summary>
        public const float RevolutionLeadRange = 600f;
        public const float RevolutionLeadMin = 0.2f;

        /// <summary>沿"自身→玩家"方向越位的距离归一化分母。沿用旧值 P1.Revolution.cs:46。</summary>
        public const float RevolutionOvershootRange = 800f;

        /// <summary>召回小球数量的基数，在按与玩家的 X 距离及各招式间距换算后加上。沿用旧值 P1.Revolution.cs:76,78。</summary>
        public const int RevolutionBaseCount = 6;

        /// <summary>影子公转弹幕伤害（普通/专家/大师/FTW）。沿用旧值 P1.Revolution.cs:107。</summary>
        public static int RevolutionShadowDamage() => Helper.ScaleValueForDiffMode(20, 30, 40, 50);

        #endregion

        #region 星轨 Starline

        /// <summary>
        /// 判定"离玩家远"的距离门槛，远则先引力移动到玩家头顶。沿用旧值 P1.Starline.cs:29,64。
        /// （设计文档 §星轨 状态 0 写的是 16×50，代码是 16×30；以代码为准，差异记在报告里。）
        /// </summary>
        public const float StarlineFarDistance = 16 * 30;

        /// <summary>星星弹幕伤害（普通/专家/大师）。沿用旧值 P1.Starline.cs:82,132。</summary>
        public static int StarlineStarDamage() => Helper.GetProjDamage(18, 25, 32);

        #endregion

        #region 月食 LunarEclipse

        /// <summary>循环次数（普通/专家/大师/FTW），与设计文档 §月食 招式状态 3 一致。沿用旧值 P1.LunarEclipse.cs:99。</summary>
        public static int EclipseLoopCount() => Helper.ScaleValueForDiffMode(5, 6, 7, 8);

        #endregion

        #region 影刺 ShadowSpike

        /// <summary>小球一字排开时相邻两个的间距，同时也是"叫几个小球"的距离换算单位。沿用旧值 P1.ShadowSpike.cs:9。</summary>
        public const float SpikePerLength = 16 * 7;

        /// <summary>小球平移到位的固定时长（普通/专家/大师/FTW）。沿用旧值 P1.ShadowSpike.cs:128。</summary>
        public static int SpikeBallLerpTime() => Helper.ScaleValueForDiffMode(25, 20, 20, 20);

        /// <summary>小球发射激光的前摇时长（普通/专家/大师/FTW）。沿用旧值 P1.ShadowSpike.cs:134。</summary>
        public static int SpikeBallChannelTime() => Helper.ScaleValueForDiffMode(60, 60, 50, 40);

        /// <summary>小球激光的持续帧数。沿用旧值 P1.ShadowSpike.cs:141。</summary>
        public const int SpikeBallLaserTime = 35;

        #endregion
    }
}
