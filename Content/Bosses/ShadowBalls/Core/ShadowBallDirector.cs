using Coralite.Helpers;

namespace Coralite.Content.Bosses.ShadowBalls.Core
{
    /// <summary>
    /// 影子球（本体）唯一数字出口。所有招式的拍点、距离、速度、插值系数都从这里取，状态文件里不留裸数字。<br/>
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

        /// <summary>
        /// 硬锁一：不与上一手相同（D4）。旧代码是纯等权掷骰、没有防复读，这层是本轮按纪律补的记账，池子与权重一字未改。
        /// 池里只有四招，硬锁一后仍有三个候选，不会把轮换压窄到只剩一条。
        /// </summary>
        public const bool ForbidImmediateRepeat = true;

        #endregion

        #region 出生动画 OnSpawnAnim

        /// <summary>出生动画每帧速度衰减。沿用旧值 Others.Anmations.cs:11。</summary>
        public const float SpawnDamp = 0.95f;

        /// <summary>出生动画放完、可以进入轮换的帧数。沿用旧值 Others.Anmations.cs:30。</summary>
        public const int SpawnExitFrames = 60;

        /// <summary>球壳逐帧退掉（露出核心）的总时长 = 帧表行数 × 2，每 2 帧退一行。沿用旧值 Others.Anmations.cs:67,71。</summary>
        public const int SpawnShellFrames = ShadowBall.MaxFrameY * 2;

        /// <summary>球壳退完后停在的帧号。沿用旧值 Others.Anmations.cs:83。</summary>
        public const int SpawnShellRestFrame = 30;

        #endregion

        #region 召唤小球 SummonSmallShdowBall

        /// <summary>减速段每帧速度衰减。沿用旧值 P1.SummonSmallBall.cs:49。</summary>
        public const float SummonSlowDamp = 0.94f;

        /// <summary>减速段第几帧把锁环切成同心圆。沿用旧值 P1.SummonSmallBall.cs:51。</summary>
        public const int SummonLockSwitchFrame = 10;

        /// <summary>减速段总时长；期间锁环半径倍率朝 <see cref="SummonLockExpand"/> 插值。沿用旧值 P1.SummonSmallBall.cs:55,59。</summary>
        public const int SummonSlowFrames = 55;

        /// <summary>减速段锁环张开到的半径倍率与每帧插值系数。沿用旧值 P1.SummonSmallBall.cs:57。</summary>
        public const float SummonLockExpand = 1.3f;
        public const float SummonLockExpandLerp = 0.07f;

        /// <summary>推锁段：先收拢 40 帧到 <see cref="SummonLockShrink"/>，再 10 帧弹出到 <see cref="SummonLockPush"/>。沿用旧值 P1.SummonSmallBall.cs:69,70,74,78。</summary>
        public const int SummonShrinkFrames = 40;
        public const int SummonPushFrames = 10;
        public const float SummonLockShrink = 0.8f;
        public const float SummonLockPush = 1.5f;

        /// <summary>小球出生动画的等待时长，期间锁环半径倍率从 <see cref="SummonLockPush"/> 收回 1。沿用旧值 P1.SummonSmallBall.cs:118,120。</summary>
        public const int SummonWaitFrames = 60 * 2;

        /// <summary>收尾段第几帧把锁环切回常规旋转。沿用旧值 P1.SummonSmallBall.cs:132。</summary>
        public const int SummonLockRestoreFrame = 2;

        /// <summary>收尾段总时长（旧写法是 53 + 100）。沿用旧值 P1.SummonSmallBall.cs:137。</summary>
        public const int SummonEndFrames = 53 + 100;

        /// <summary>伴随小球一起放出的追逐影子弹幕初速。沿用旧值 P1.SummonSmallBall.cs:98。</summary>
        public const float SummonShadowProjSpeed = 14f;

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

        /// <summary>起手前摇。沿用旧值 P1.Revolution.cs:36。</summary>
        public const int RevolutionReadyFrames = 10;

        /// <summary>落点预判：玩家速度平方大于这个值才算"玩家在动"。沿用旧值 P1.Revolution.cs:40。</summary>
        public const float MovingTargetSpeedSq = 1f;

        /// <summary>沿玩家速度方向的预判量与其距离归一化分母、下限。沿用旧值 P1.Revolution.cs:42-43。</summary>
        public const float RevolutionLeadRange = 600f;
        public const float RevolutionLeadMin = 0.2f;
        public const float RevolutionLeadLength = 120f;

        /// <summary>沿"自身→玩家"方向的越位量与其距离归一化分母。沿用旧值 P1.Revolution.cs:46。</summary>
        public const float RevolutionOvershootRange = 800f;
        public const float RevolutionOvershootLength = 200f;

        /// <summary>召回小球的身位换算：与玩家的 X 距离每 <see cref="RevolutionPerLength"/> 像素多叫一个，再加基数。沿用旧值 P1.Revolution.cs:76,78。</summary>
        public const float RevolutionPerLength = 16 * 7;
        public const int RevolutionBaseCount = 6;

        /// <summary>小球归位的等待时长。沿用旧值 P1.Revolution.cs:91。</summary>
        public const int RevolutionGatherFrames = 90;

        /// <summary>放光段：离玩家超过这个距离就缓慢靠近，否则原地衰减。沿用旧值 P1.Revolution.cs:100。</summary>
        public const float RevolutionKeepDistance = 600f;
        public const float RevolutionApproachSpeed = 4f;
        public const float RevolutionApproachLerp = 0.04f;
        public const float RevolutionHoldDamp = 0.94f;

        /// <summary>放光段每隔多少帧抛一颗影子公转弹幕，以及它的初速与存活参数 ai0。沿用旧值 P1.Revolution.cs:105,109。</summary>
        public const int RevolutionShadowInterval = 24;
        public const float RevolutionShadowSpeed = 6f;
        public const float RevolutionShadowLife = 90f;

        /// <summary>放光段总时长。沿用旧值 P1.Revolution.cs:111。</summary>
        public const int RevolutionShootFrames = 180;

        /// <summary>收光后摇的时长与速度衰减。沿用旧值 P1.Revolution.cs:120,121。</summary>
        public const int RevolutionEndFrames = 45;
        public const float RevolutionEndDamp = 0.9f;

        /// <summary>影子公转弹幕伤害（普通/专家/大师/FTW）。沿用旧值 P1.Revolution.cs:107。</summary>
        public static int RevolutionShadowDamage() => Helper.ScaleValueForDiffMode(20, 30, 40, 50);

        #endregion

        #region 星轨 Starline

        /// <summary>
        /// 判定"离玩家远"的距离门槛，远则先引力移动到玩家头顶。沿用旧值 P1.Starline.cs:29,64。
        /// （设计文档 §星轨 状态 0 写的是 16×50，代码是 16×30；以代码为准，差异记在报告里。）
        /// </summary>
        public const float StarlineFarDistance = 16 * 30;

        /// <summary>引力移动的落点：玩家头顶上方。沿用旧值 P1.Starline.cs:33（文档写 −250，代码是 −350）。</summary>
        public const float StarlineHoverHeight = 350f;

        /// <summary>快速接近段的速度区间与其距离归一化参数。沿用旧值 P1.Starline.cs:68-69。</summary>
        public const float StarlineApproachMinSpeed = 10f;
        public const float StarlineApproachMaxSpeed = 30f;
        public const float StarlineApproachNear = 16 * 10;
        public const float StarlineApproachRange = 16 * 30;

        /// <summary>快速接近段的速度插值爬坡时长。沿用旧值 P1.Starline.cs:71。</summary>
        public const float StarlineApproachRampFrames = 80f;

        /// <summary>到位时环射的星星数量与初速。沿用旧值 P1.Starline.cs:86,89。</summary>
        public const int StarlineRingCount = 12;
        public const float StarlineStarSpeed = 12f;
        public const float StarlineStarSpinMin = 0.015f;
        public const float StarlineStarSpinMax = 0.025f;

        /// <summary>每颗星星记录的轨道半径在当前距离上的随机抖动幅度。沿用旧值 P1.Starline.cs:92。</summary>
        public const float StarlineStarJitter = 16 * 10;

        /// <summary>持续攻击段自身保持的最小半径。沿用旧值 P1.Starline.cs:103-106。</summary>
        public const float StarlineKeepDistanceMin = 16 * 20;

        /// <summary>持续攻击段：超过记录半径就缓慢靠近，否则减速。沿用旧值 P1.Starline.cs:121,126。</summary>
        public const float StarlineApproachSpeed = 4f;
        public const float StarlineApproachLerp = 0.05f;
        public const float StarlineHoldDamp = 0.95f;

        /// <summary>持续攻击段的补星间隔与总时长。沿用旧值 P1.Starline.cs:130,148。</summary>
        public const int StarlineShootInterval = 30;
        public const int StarlineShootFrames = 60 * 10;

        /// <summary>收招后摇的时长与速度衰减。沿用旧值 P1.Starline.cs:170,173。</summary>
        public const int StarlineEndFrames = 60;
        public const float StarlineEndDamp = 0.95f;

        /// <summary>星星弹幕伤害（普通/专家/大师）。沿用旧值 P1.Starline.cs:82,132。</summary>
        public static int StarlineStarDamage() => Helper.GetProjDamage(18, 25, 32);

        #endregion

        #region 月食 LunarEclipse

        /// <summary>起手圆环特效的持续时长，到点后开始第一次引力移动。沿用旧值 P1.LunarEclipse.cs:26。</summary>
        public const int EclipseStartFrames = 45;

        /// <summary>每次引力移动的落点半径（以玩家为圆心）。沿用旧值 P1.LunarEclipse.cs:32,107。</summary>
        public const float EclipseOrbitRadius = 450f;

        /// <summary>每轮之间落点角度的推进量。沿用旧值 P1.LunarEclipse.cs:104（旧代码把随机区间注释掉了，固定 2π/3）。</summary>
        public const float EclipseAngleStep = MathHelper.TwoPi / 3f;

        /// <summary>到位后持续朝向玩家的插值爬坡时长与该段总时长。沿用旧值 P1.LunarEclipse.cs:81,86。</summary>
        public const float EclipseFaceRampFrames = 50f;
        public const int EclipseFaceFrames = 100;

        /// <summary>收招后摇时长。沿用旧值 P1.LunarEclipse.cs:135。</summary>
        public const int EclipseEndFrames = 30;

        /// <summary>循环次数（普通/专家/大师/FTW），与设计文档 §月食 招式状态 3 一致。沿用旧值 P1.LunarEclipse.cs:99。</summary>
        public static int EclipseLoopCount() => Helper.ScaleValueForDiffMode(5, 6, 7, 8);

        #endregion

        #region 影刺 ShadowSpike

        /// <summary>起手前摇与期间的速度衰减。沿用旧值 P1.ShadowSpike.cs:33,35。</summary>
        public const int SpikeReadyFrames = 10;
        public const float SpikeReadyDamp = 0.95f;

        /// <summary>落点：玩家下方多少像素。沿用旧值 P1.ShadowSpike.cs:38。</summary>
        public const float SpikeDropHeight = 350f;

        /// <summary>落点 X 的预判量与越位量（与公转同结构，长度都是 300）。沿用旧值 P1.ShadowSpike.cs:41-45。</summary>
        public const float SpikeLeadLength = 300f;

        /// <summary>等待小球期间朝 0 回正的插值。沿用旧值 P1.ShadowSpike.cs:90。</summary>
        public const float SpikeRotationLerp = 0.2f;

        /// <summary>上戳时的初始纵向速度。沿用旧值 P1.ShadowSpike.cs:100。</summary>
        public const float SpikeRiseSpeed = -16f;

        /// <summary>横向距离超过这个值时补的横向速度及其归一化分母。沿用旧值 P1.ShadowSpike.cs:102-104。</summary>
        public const float SpikeDashThreshold = 16 * 30;
        public const float SpikeDashRange = 800f;
        public const float SpikeDashSpeed = 10f;

        /// <summary>收招后摇时长与速度衰减。沿用旧值 P1.ShadowSpike.cs:111,113。</summary>
        public const int SpikeEndFrames = 90;
        public const float SpikeEndDamp = 0.95f;

        /// <summary>小球一字排开时相邻两个的间距，同时也是"叫几个小球"的距离换算单位。沿用旧值 P1.ShadowSpike.cs:9。</summary>
        public const float SpikePerLength = 16 * 7;

        /// <summary>小球平移到位的固定时长（普通/专家/大师/FTW）。沿用旧值 P1.ShadowSpike.cs:128。</summary>
        public static int SpikeBallLerpTime() => Helper.ScaleValueForDiffMode(25, 20, 20, 20);

        /// <summary>小球发射激光的前摇时长（普通/专家/大师/FTW）。沿用旧值 P1.ShadowSpike.cs:134。</summary>
        public static int SpikeBallChannelTime() => Helper.ScaleValueForDiffMode(60, 60, 50, 40);

        /// <summary>小球激光的持续帧数。沿用旧值 P1.ShadowSpike.cs:141。</summary>
        public const int SpikeBallLaserTime = 35;

        #endregion

        #region 旋转激光 RollingLaser

        /// <summary>分层数；小球按索引交错成 3 圈。沿用旧值 P1.RollingLaser.cs:33,35。</summary>
        public const int RollingLayerCount = 3;

        /// <summary>保持身位的落点：玩家头顶上方。沿用旧值 P1.RollingLaser.cs:63。</summary>
        public const float RollingHoverHeight = 300f;

        /// <summary>落点死区半径；在死区外靠近、死区内减速。沿用旧值 P1.RollingLaser.cs:64,67,70。</summary>
        public const float RollingDeadZone = 16 * 5;
        public const float RollingApproachSpeed = 4f;
        public const float RollingApproachLerp = 0.03f;
        public const float RollingHoldDamp = 0.9f;

        /// <summary>最短持续帧数；到点且全部小球就绪才收招。沿用旧值 P1.RollingLaser.cs:72。</summary>
        public const int RollingMinFrames = 800;

        #endregion
    }
}
