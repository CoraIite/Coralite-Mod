using Coralite.Helpers;

namespace Coralite.Content.Bosses.ShadowBalls.Core
{
    /// <summary>
    /// 小影子球唯一数字出口。旧行号对应重构前的 <c>P1S.*.cs</c>。<br/>
    /// 影刺 / 旋转激光共用的三个时长留在 <see cref="ShadowBallDirector"/>（本体的招式节拍由本体定，小球只是跟着走）。
    /// </summary>
    internal static class SmallShadowBallDirector
    {
        #region 全局

        /// <summary>自发光颜色（暗紫）。沿用旧值 SmallShadowBall.cs:141。</summary>
        public static readonly Vector3 SelfLight = new Vector3(0.5f, 0.4f, 0.6f);

        /// <summary>
        /// 状态超时兜底帧数。任何招式卡住超过这个时长一律回待机（D2）。
        /// 取 900 = 15 秒：最长的小球招式是星轨的 225 帧与旋转激光的两轮（约 500 帧），留了近一倍余量。
        /// </summary>
        public const int StateTimeoutFrames = 60 * 15;

        /// <summary>招式中通用趋近移动的目标速度与默认插值。沿用旧值 SmallShadowBall.cs:1159-1163。</summary>
        public const float MoveToSpeed = 18f;
        public const float MoveToLerp = 0.12f;
        public const float MoveToRotationLerp = 0.15f;

        /// <summary>预警线的长度（40 格）。沿用旧值 P1S.ShadowSpike.cs:55 / P1S.RollingLaser.cs:33。</summary>
        public const float AimLineLength = 16 * 40;

        #endregion

        #region 出生动画 OnSpawnAnim

        /// <summary>刚被弹出时沿"背离本体"方向的初速。沿用旧值 P1S.Animations.cs:25。</summary>
        public const float SpawnBurstSpeed = 12f;

        /// <summary>出生初始的深度与锁扣间距。沿用旧值 P1S.Animations.cs:21-22。</summary>
        public const float SpawnDepth = 1f;
        public const float SpawnLockDistance = 4f;

        /// <summary>刚生成时的缩放；绘制端按 &gt; 0.001 才画球体，所以这是"还看不见"的尺寸。沿用旧值 P1S.Animations.cs:28。</summary>
        public const float HiddenScale = 0.0001f;

        /// <summary>飘飞段的速度衰减与超时（等不到影子弹幕就自己往下走）。沿用旧值 P1S.Animations.cs:33,35。</summary>
        public const float SpawnDriftDamp = 0.93f;
        public const int SpawnDriftTimeout = 60 * 5;

        /// <summary>锁扣打开、自身显形的时长与锁扣张开距离。沿用旧值 P1S.Animations.cs:44,49,51。</summary>
        public const float SpawnOpenFrames = 30f;
        public const float SpawnOpenLockDistance = 40f;
        public const float SpawnOpenRotLerp = 0.2f;

        /// <summary>锁扣绕一圈收尾的时长与其间距摆动幅度。沿用旧值 P1S.Animations.cs:60,63,66。</summary>
        public const float SpawnSpinFrames = 25f;
        public const float SpawnSpinLockSwing = 20f;

        #endregion

        #region 待机 Idle

        /// <summary>追位插值的爬坡时长。沿用旧值 P1S.Idle.cs:24。</summary>
        public const float IdleChaseRampFrames = 120f;

        /// <summary>锁扣朝向本体的转向插值。沿用旧值 P1S.Idle.cs:26。</summary>
        public const float IdleLockRotLerp = 0.2f;

        /// <summary>环绕角的基础转速（乘本体的锁环计时器）。沿用旧值 P1S.Idle.cs:29。</summary>
        public const float IdleBaseRotPerTick = 0.01f;

        /// <summary>奇偶索引之间的倾角错开量，让小球不与锁环重叠。沿用旧值 P1S.Idle.cs:36 等处的 <c>dir * 0.3f</c>。</summary>
        public const float IdleTiltSplit = 0.3f;

        /// <summary>常规锁环模式下的倾角摆动：基准 1.57、频率与幅度都按第 3 层取。沿用旧值 P1S.Idle.cs:36-37。</summary>
        public const float IdleNormalTiltBase = 1.57f;
        public const float IdleNormalTiltFreq = 0.01f + (3 * 0.005f);
        public const float IdleNormalTiltAmp = 0.4f + (3 * 0.1f);
        public const float IdleNormalSpinBase = MathHelper.TwoPi / 3 * 3;
        public const float IdleNormalSpinRate = 0.02f;

        /// <summary>带角度的同心圆模式下的固定倾角。沿用旧值 P1S.Idle.cs:48。</summary>
        public const float IdleAngledTilt = 1f;

        /// <summary>带角度旋转模式下的倾角转速（按第 4 层取，反向）。沿用旧值 P1S.Idle.cs:54。</summary>
        public const float IdleAngledRotRate = 0.01f + (4 * 0.005f);

        /// <summary>环绕半径 = 基数 + 小球总数 × 每球增量。沿用旧值 P1S.Idle.cs:60。</summary>
        public const float IdleRadiusBase = 120f;
        public const float IdleRadiusPerBall = 4f;

        /// <summary>追位段的插值系数：基数 + 爬坡增量。沿用旧值 P1S.Idle.cs:70。</summary>
        public const float IdleChaseLerpBase = 0.02f;
        public const float IdleChaseLerpGain = 0.5f;

        /// <summary>进入"贴位"的距离门槛，以及被甩开多远退回追位。沿用旧值 P1S.Idle.cs:72,84。</summary>
        public const float IdleLockOnDistance = 4f;
        public const float IdleBreakDistance = 120f;

        /// <summary>透视投影的视距（把三维环绕压回二维）。沿用旧值 P1S.Idle.cs:103。</summary>
        public const float ProjectionDepth = 1000f;

        /// <summary>深度插值：按与目标深度的差归一化，夹在 0.1 与 1 之间。沿用旧值 P1S.Idle.cs:112。</summary>
        public const float DepthLerpRange = 100f;
        public const float DepthLerpMin = 0.1f;

        #endregion

        #region 影之公转 Revolution

        /// <summary>环绕半径的取样区间：以"本体到玩家"的距离为基准夹在这两个值之间。沿用旧值 P1S.Revolution.cs:19-20。</summary>
        public const float RevolutionRadiusMin = 160f;
        public const float RevolutionRadiusMax = 500f;

        /// <summary>层间距与半径下限。沿用旧值 P1S.Revolution.cs:26。</summary>
        public const float RevolutionRingGap = 70f;
        public const float RevolutionRadiusFloor = 120f;

        /// <summary>环绕角速度：基数 + 奇偶索引增量；再加上按索引错开的初相。沿用旧值 P1S.Revolution.cs:27-28。</summary>
        public const float RevolutionSpinBase = 0.012f;
        public const float RevolutionSpinOddGain = 0.004f;
        public const float RevolutionPhasePerIndex = 1.73f;

        /// <summary>环绕时的趋近插值。沿用旧值 P1S.Revolution.cs:29。</summary>
        public const float RevolutionMoveLerp = 0.08f;

        /// <summary>抛影子弹幕：起始帧、间隔、初速比例与存活参数 ai0。沿用旧值 P1S.Revolution.cs:32-35。</summary>
        public const int RevolutionShootStart = 90;
        public const int RevolutionShootInterval = 45;
        public const float RevolutionShadowSpeedScale = 0.15f;
        public const float RevolutionShadowLife = 80f;

        /// <summary>整招时长。沿用旧值 P1S.Revolution.cs:37。</summary>
        public const int RevolutionFrames = 300;

        /// <summary>小球抛出的影子弹幕伤害（普通/专家/大师/FTW）。沿用旧值 P1S.Revolution.cs:34。</summary>
        public static int RevolutionShadowDamage() => Helper.ScaleValueForDiffMode(18, 26, 34, 42);

        #endregion

        #region 星轨 Starline

        /// <summary>轨道中心在本体上方多少、环半径多大。沿用旧值 P1S.Starline.cs:27。</summary>
        public const float StarlineOrbitHeight = 190f;
        public const float StarlineOrbitRadius = 150f;

        /// <summary>移动到轨道位的时长。沿用旧值 P1S.Starline.cs:36。</summary>
        public const int StarlineMoveFrames = 55;

        /// <summary>发射段的速度衰减与朝向插值。沿用旧值 P1S.Starline.cs:46,48。</summary>
        public const float StarlineFireDamp = 0.82f;
        public const float StarlineFireRotLerp = 0.1f;

        /// <summary>依次发射的基础延迟与每个索引的错开量。沿用旧值 P1S.Starline.cs:52。</summary>
        public const int StarlineFireDelay = 45;
        public const int StarlineFireStagger = 8;

        /// <summary>激光持续帧数（写进弹幕 ai1）。沿用旧值 P1S.Starline.cs:56。</summary>
        public const float StarlineLaserTime = 32f;

        /// <summary>整招时长。沿用旧值 P1S.Starline.cs:58。</summary>
        public const int StarlineFrames = 225;

        #endregion

        #region 月食 LunarEclipse

        /// <summary>贴到本体身前的距离与贴位插值的爬坡时长。沿用旧值 P1S.LunarEclipse.cs:25-26。</summary>
        public const float EclipseHoldDistance = 100f;
        public const float EclipseHoldRampFrames = 30f;

        /// <summary>蓄够多久发射、发射初速。沿用旧值 P1S.LunarEclipse.cs:29,33。</summary>
        public const int EclipseHoldFrames = 30;
        public const float EclipseLaunchSpeed = 20f;

        /// <summary>飞行段每隔多少帧留一颗月相弹幕、共留几颗。沿用旧值 P1S.LunarEclipse.cs:42,50。</summary>
        public const int EclipseSpawnInterval = 5;
        public const int EclipseSpawnCount = 9;

        /// <summary>减速段的衰减与时长。沿用旧值 P1S.LunarEclipse.cs:60,63。</summary>
        public const float EclipseSlowDamp = 0.9f;
        public const int EclipseSlowFrames = 120;

        /// <summary>留圆环弹幕的间隔。沿用旧值 P1S.LunarEclipse.cs:73。</summary>
        public const int EclipseRingInterval = 30;

        #endregion

        #region 照影 ShadowShoot

        /// <summary>聚拢到本体周围的半径与移动时长。沿用旧值 P1S.ShadowShoot.cs:17,25。</summary>
        public const float ShadowShootGatherRadius = 105f;
        public const int ShadowShootMoveFrames = 55;

        /// <summary>发射段的速度衰减、起始帧、错开周期与散列模数。沿用旧值 P1S.ShadowShoot.cs:34,36。</summary>
        public const float ShadowShootDamp = 0.84f;
        public const int ShadowShootStart = 10;
        public const int ShadowShootInterval = 38;
        public const int ShadowShootStaggerMod = 5;

        /// <summary>影子导弹初速与整招时长。沿用旧值 P1S.ShadowShoot.cs:37,41。</summary>
        public const float ShadowShootMissileSpeed = 7f;
        public const int ShadowShootFrames = 165;

        /// <summary>影子导弹伤害（普通/专家/大师/FTW）。沿用旧值 P1S.ShadowShoot.cs:39。</summary>
        public static int ShadowShootMissileDamage() => Helper.ScaleValueForDiffMode(22, 32, 42, 52);

        #endregion

        #region 影刺 ShadowSpike

        /// <summary>
        /// 每个小球出手的身位错开帧数。旧写的是 <c>selfIndex * 10</c>，但那一拍里 <c>Timer</c> 每帧自增两次
        /// （招式体里一次 + 包壳态里一次，P1S.ShadowSpike.cs:22-24），实际就是每个身位 5 帧；这里按实际行为取 5。
        /// </summary>
        public const int SpikeStaggerFrames = 5;

        /// <summary>预备段的速度衰减与朝上的转向插值。沿用旧值 P1S.ShadowSpike.cs:69-70。</summary>
        public const float SpikeChannelDamp = 0.9f;
        public const float SpikeChannelRotLerp = 0.1f;

        /// <summary>激光打完后的收招余量。沿用旧值 P1S.ShadowSpike.cs:85,120。</summary>
        public const int SpikeLaserTail = 30;

        /// <summary>大师模式第二次瞄准的时长、开火帧与那一发的预警线参数。沿用旧值 P1S.ShadowSpike.cs:89,101,106。</summary>
        public const int SpikeReaimFrames = 20;
        public const int SpikeRefireFrame = 35;
        public const int SpikeReaimLineSpawn = 20;
        public const int SpikeReaimLineHold = 15;

        /// <summary>小球激光伤害（普通/专家/大师/FTW）。沿用旧值 P1S.ShadowSpike.cs:77。</summary>
        public static int SpikeLaserDamage() => Helper.ScaleValueForDiffMode(30, 50, 40, 40);

        #endregion

        #region 旋转激光 RollingLaser

        /// <summary>激光持续帧数。沿用旧值 P1S.RollingLaser.cs:15。</summary>
        public const int RollingLaserTime = 60;

        /// <summary>层半径 = 基数 + 层号 × 层间距。沿用旧值 P1S.RollingLaser.cs:88。</summary>
        public const float RollingRadiusBase = 120f;
        public const float RollingRadiusPerLayer = 30f;

        /// <summary>层转速 = 基数 + 层号 × 增量，奇偶层反向。沿用旧值 P1S.RollingLaser.cs:86。</summary>
        public const float RollingSpinBase = 0.005f;
        public const float RollingSpinPerLayer = 0.003f;

        /// <summary>贴位与朝向的插值。沿用旧值 P1S.RollingLaser.cs:90-91。</summary>
        public const float RollingMoveLerp = 0.2f;
        public const float RollingRotLerp = 0.2f;

        /// <summary>每颗小球总共打几轮激光。沿用旧值 P1S.RollingLaser.cs:54（<c>Recorder4 &gt; 1</c> 即第二轮结束收招）。</summary>
        public const int RollingVolleyCount = 2;

        /// <summary>小球激光伤害（普通/专家/大师/FTW）。沿用旧值 P1S.RollingLaser.cs:44 与 P1S.Starline.cs:54。</summary>
        public static int RollingLaserDamage() => Helper.ScaleValueForDiffMode(25, 35, 45, 55);

        #endregion
    }
}
