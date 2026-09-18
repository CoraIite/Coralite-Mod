using Coralite.Helpers;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core
{
    /// <summary>
    /// 赤血玉灵战斗调参中心：全部数字与阶段档位的唯一出口。<br/>
    /// 本轮是结构迁移，战斗设计不变：每条常量注明来源，查不到设计理由的一律写“沿用旧值 AI.cs:行号”（迁移前的行号）。
    /// 弹药机制与循环表的口径与赤玉灵同源，见 <c>Content/Bosses/Rediancie/Rediancie_AI_Description.md</c>。
    /// </summary>
    internal static class BloodiancieDirector
    {
        //==================== 编制 / 出生 / 脱战 ====================

        /// <summary>出生自带 6 发弹药：比赤玉灵多一倍，首招多段爆炸一开始就有可击碎的护甲。沿用旧值 AI.cs:30</summary>
        public const int SpawnFollowerCount = 6;
        /// <summary>出生点在最近玩家头顶 600 px（约一屏高）：给入场动画留出下沉距离。沿用旧值 AI.cs:35</summary>
        public const float SpawnHeightAboveTarget = 600f;
        /// <summary>目标离开 3000 px 视为脱战。沿用旧值 AI.cs:97</summary>
        public const float DespawnDistance = 3000f;
        /// <summary>脱战：X 衰减、Y 轻微下坠、鼓励消失。沿用旧值 AI.cs:103-105</summary>
        public const float DespawnDampX = 0.97f;
        public const float DespawnGravity = 0.04f;
        public const int DespawnEncourageFrames = 10;

        /// <summary>弹药上限按难度 14/18/24/30（赤玉灵的两倍多，血月强化版本）。沿用旧值 AI.cs:962,993</summary>
        public static int MaxFollowers() => Helper.ScaleValueForDiffMode(14, 18, 24, 30);
        /// <summary>累计受伤每满 500/800/1000/2000 击碎一发弹药。沿用旧值 Bloodiancie.cs:201</summary>
        public static int FollowerBreakDamage() => Helper.ScaleValueForDiffMode(500, 800, 1000, 2000);
        /// <summary>每发弹药 +1 防御。沿用旧值 AI.cs:975</summary>
        public const int DefensePerFollower = 1;

        /// <summary>二阶段阈值：<c>life &lt; lifeMax / 2</c>，整数除法照旧。沿用旧值 AI.cs:802</summary>
        public const int Phase2LifeDivisor = 2;
        /// <summary>选招循环方式数（one_one / two_one / two_two 三选一随机）。沿用旧值 AI.cs:833</summary>
        public const int CyclingTypeCount = 3;

        /// <summary>一轮循环里近战 / 远程各几手。旧 <c>GetAICycling</c>，AI.cs:914-932</summary>
        public static void GetCycling(BloodiancieCyclingType cyclingType, out int meleeCount, out int shootCount)
        {
            switch (cyclingType)
            {
                case BloodiancieCyclingType.two_one:
                    meleeCount = 2;
                    shootCount = 1;
                    break;
                case BloodiancieCyclingType.two_two:
                    meleeCount = 2;
                    shootCount = 2;
                    break;
                default:
                    meleeCount = 1;
                    shootCount = 1;
                    break;
            }
        }

        /// <summary>
        /// 硬锁一（不与上一手相同）开关。旧设计用“可枯竭列表”本身防复读：列表里重复项即权重，抽走一项后同招在本轮不会再出，
        /// 但 <c>explosion</c> 在普通模式的近战列表里占三项，允许两手相连。为保战斗设计不变此处关闭，hub 只做提交口与记账。
        /// </summary>
        public const bool ForbidImmediateRepeat = false;
        /// <summary>查重窗口长度（记账用，本轮不裁决）：12 个状态取 1/3 = 4。</summary>
        public const int RecentPickWindow = 4;
        /// <summary>
        /// hub 停留帧数。旧代码收招当帧即 <c>ResetState</c> 切下一招、零间隔（间隔已含在各招尾段），
        /// 为不改节奏取 0：<see cref="States.BloodiancieHubState.EndAttack"/> 在 0 时直接经 Commit 返回下一招，不多占一帧。
        /// </summary>
        public const int HubFrames = 0;

        //==================== 运动 / 朝向（宿主 ApplyDeclaredMovement 消费）====================

        /// <summary>常规朝向：目标角 = 速度模长 × 0.04 × 面向，按 0.01 弧度/帧逼近。沿用旧值 AI.cs:947-948</summary>
        public const float RotationPerSpeed = 0.04f;
        public const float RotationTowardsStep = 0.01f;
        /// <summary>爆冲刹车段朝向插值。沿用旧值 AI.cs:472</summary>
        public const float RotationLerpDash = 0.1f;
        /// <summary>Hold 模式（漏声明兜底）的速度衰减；正常迁移后不会命中。</summary>
        public const float HoldDamp = 0.9f;

        /// <summary>悬停上浮（远程招通用）：X 衰减 0.98、Y 加速 -0.14、上限 -1.5；比玩家高 150 px 以上改为整体 0.99 衰减。沿用旧值 AI.cs:208-212</summary>
        public const float HoverDampX = 0.98f;
        public const float HoverAccelY = -0.14f;
        public const float HoverLimitY = -1.5f;
        public const float HoverAboveTargetY = -150f;
        public const float HoverFarDamp = 0.99f;
        /// <summary>分轴追踪的通用死区 50 px 与死区内的衰减 0.96（旧代码几乎每个招式的 X / Y 都是这一对）。沿用旧值 AI.cs:266-276</summary>
        public const float ChaseDeadZone = 50f;
        public const float ChaseDeadZoneDamp = 0.96f;

        //==================== 弹药环绕几何（Followers，双端同算）====================

        /// <summary>待机环：角速度 0.08/帧 + 0.15×速度，半径 48 + 速度/2 + 弹药数（弹药越多环越大）。沿用旧值 AI.cs:1033-1034</summary>
        public const float IdleRotPerFrame = 0.08f;
        public const float IdleRotPerSpeed = 0.15f;
        public const float IdleRadius = 48f;
        public const float IdleRadiusPerSpeed = 0.5f;
        public const float IdleRadiusPerFollower = 1f;
        /// <summary>环面绕 X 轴倾角：1.57 − clamp(Δy/200, ±1)×0.4，随玩家高低俯仰。沿用旧值 AI.cs:1036</summary>
        public const float CircleTiltBase = 1.57f;
        public const float CircleTiltRange = 200f;
        public const float CircleTiltMax = 0.4f;
        /// <summary>透视投影深度与逐弹药上下起伏。沿用旧值 AI.cs:1167-1169</summary>
        public const float ProjectionDepth = 1000f;
        public const float IdleBobAmp = 6f;
        public const float IdleBobFreq = 1.2f;
        /// <summary>待机环位置插值 0.6、朝向插值 0.2、缩放 0.9 − z×0.2。沿用旧值 AI.cs:1030,1170-1173</summary>
        public const float IdleLerp = 0.6f;
        public const float FollowerRotLerp = 0.2f;
        public const float IdleScaleBase = 0.9f;
        public const float IdleScaleDepth = 0.2f;

        /// <summary>向上射击环：角速度 0.1、半径 48 + 0.4×Timer 逐渐张开，缩放基准 1 且深度随半径/168 加深。沿用旧值 AI.cs:1045-1046,1191</summary>
        public const float UpShootRotPerFrame = 0.1f;
        public const float UpShootRadiusPerFrame = 0.4f;
        public const float UpShootScaleBase = 1f;
        public const float UpShootScaleDepthGain = 0.4f;
        public const float UpShootScaleLengthRef = 168f;
        /// <summary>召唤 / 死亡环：角速度 0.06、半径 48 + clamp(Timer×30, 0, 30)。沿用旧值 AI.cs:1091-1092</summary>
        public const float SummonRotPerFrame = 0.06f;
        public const float SummonRadiusGrow = 30f;
        /// <summary>烟花环：平面环半径 46 + 30，位置插值从 0.1 在 60 帧内爬到 0.6，朝向插值 0.6。沿用旧值 AI.cs:1103-1112</summary>
        public const float FireworkRadius = 46f;
        public const float FireworkLerpBase = 0.1f;
        public const float FireworkLerpGain = 0.5f;
        public const float FireworkLerpRamp = 60f;
        public const float FireworkRotLerp = 0.6f;

        /// <summary>瞄准环（激光 / 脉冲共用）：因子 = 距离/150 钳 0..1，炮口 = 本体 + 方向×(32 + 32×因子)，环面倾斜 0.2 + 方向分量×因子×1.1。沿用旧值 AI.cs:215-217,1081-1082</summary>
        public const float AimFactorRange = 150f;
        public const float AimMuzzleBase = 32f;
        public const float AimMuzzleGain = 32f;
        public const float AimTiltBase = 0.2f;
        public const float AimTiltGain = 1.1f;
        /// <summary>激光环：半径 46 + 后坐 60。沿用旧值 AI.cs:1080</summary>
        public const float MagicRingRadius = 46f;
        public const float MagicRingRecoil = 60f;
        /// <summary>脉冲环：半径 26 + 后坐 86，炮口弹药再前伸 16、放大 1.3。沿用旧值 AI.cs:1127-1144</summary>
        public const float PulseRingRadius = 26f;
        public const float PulseRingRecoil = 86f;
        public const float PulseMuzzleForward = 16f;
        public const float PulseMuzzleScale = 1.3f;
        /// <summary>后坐曲线 x·sin(x³)/1.186，x = 1.465×(1 − t)：先猛后缓的一记回弹。沿用旧值 AI.cs:1076-1077</summary>
        public const float RecoilCurveX = 1.465f;
        public const float RecoilCurveNorm = 1.186f;
        /// <summary>蓄力段环位置插值从 0.1 按 0.5×Timer/蓄力帧数爬坡（脉冲与激光共用，旧代码没钳上限，到点时已爬过 0.6）。沿用旧值 AI.cs:224,708</summary>
        public const float AimRingLerpBase = 0.1f;
        public const float AimRingLerpGain = 0.5f;
        /// <summary>瞄准环缩放深度 0.3、朝向插值 0.4、角速度 0.1/帧、层距额外 +0.45。沿用旧值 AI.cs:1068,1206-1211</summary>
        public const float AimRingScaleDepth = 0.3f;
        public const float AimRingRotLerp = 0.4f;
        public const float AimRingRotPerFrame = 0.1f;
        public const float AimRingLengthOffsetGain = 0.45f;

        //==================== 出生动画 onSpawnAnim ====================

        /// <summary>
        /// 第 1 帧生成名牌弹幕并以 1.5 px/f 下沉、置无敌。旧代码首帧读到的 <c>Timer</c> 是 0（普通 int 字段从 0 起、<c>Update(); Timer++</c>），
        /// 新基座在 OnUpdate 开头自增，招式体首帧读到 1，因此这里取 1；其余拍点保持旧的绝对帧数，入场演出整体只差 1 帧。沿用旧值 AI.cs:164-169
        /// </summary>
        public const int SpawnAnimNameLineFrame = 1;
        public const float SpawnAnimSinkSpeed = 1.5f;
        /// <summary>每 5 帧撒 Timer/25 粒宝石尘，越接近爆炸越密。沿用旧值 AI.cs:172-179</summary>
        public const int SpawnAnimDustInterval = 5;
        public const int SpawnAnimDustPerFrames = 25;
        public const float SpawnAnimDustScaleGain = 0.2f;
        /// <summary>120 帧后下沉速度按 0.998 缓慢衰减。沿用旧值 AI.cs:182-183</summary>
        public const int SpawnAnimSlowFrame = 120;
        public const float SpawnAnimSlowDamp = 0.998f;
        /// <summary>260 帧大爆炸（伤害 80、击退 8）并点亮血玉天空，270 帧解除无敌进首招多段爆炸。沿用旧值 AI.cs:185-200</summary>
        public const int SpawnAnimBoomFrame = 260;
        public const int SpawnAnimBoomDamage = 80;
        public const float SpawnAnimBoomKnockback = 8f;
        public const int SpawnAnimEndFrame = 270;

        //==================== 死亡动画 onKillAnim ====================

        /// <summary>上浮：X 衰减 0.96、Y 加速 -0.05、上限 -0.5。沿用旧值 AI.cs:130</summary>
        public const float KillAnimDampX = 0.96f;
        public const float KillAnimAccelY = -0.05f;
        public const float KillAnimLimitY = -0.5f;
        /// <summary>30 帧后开演；40 帧起每 20 帧掉一组碎块；230 帧前每 15 帧一次小爆炸特效。沿用旧值 AI.cs:134-145</summary>
        public const int KillAnimQuietFrames = 30;
        public const int KillAnimGoreStart = 40;
        public const int KillAnimGoreInterval = 20;
        public const int KillAnimSparkEnd = 230;
        public const int KillAnimSparkInterval = 15;
        /// <summary>245 帧大爆炸（伤害 150、击退 8），250 帧真正死亡。沿用旧值 Bloodiancie.cs:51、AI.cs:147-159</summary>
        public const int KillAnimBoomFrame = 245;
        public const int KillAnimBoomDamage = 150;
        public const float KillAnimBoomKnockback = 8f;
        public const int KillAnimEndFrame = 250;
        /// <summary>碎块与特效的随机散布范围。沿用旧值 AI.cs:139-145</summary>
        public const float KillAnimScatterX = 30f;
        public const float KillAnimScatterY = 40f;

        //==================== 赤色脉冲 pulse ====================

        /// <summary>蓄力 100 帧（前 60 帧插值爬坡），之后每 45 帧一发直到 255 帧，275 帧收招。沿用旧值 AI.cs:218-259</summary>
        public const int PulseChargeFrames = 100;
        public const int PulseRampFrames = 60;
        public const int PulseCycleFrames = 45;
        public const int PulseFireEnd = 255;
        public const int PulseEndFrame = 275;
        /// <summary>
        /// 后坐曲线的归一化周期 65：旧代码把 <c>realTime % 45</c> 的相位塞进按 65 归一的曲线里（AI.cs:231,1139），
        /// 于是后坐只走到曲线的 69% 就重置。原样保留。
        /// </summary>
        public const float PulseRecoilNorm = 65f;
        /// <summary>收尾环插值 0.08。沿用旧值 AI.cs:235</summary>
        public const float PulseIdleLerp = 0.08f;
        /// <summary>弹幕：伤害 60/80/120、速度 12、瞄准散布 48 px、击退 5。沿用旧值 AI.cs:247-250</summary>
        public static int PulseDamage() => Helper.GetProjDamage(60, 80, 120);
        public const float PulseSpeed = 12f;
        public const float PulseSpread = 48f;
        public const float PulseKnockback = 5f;
        /// <summary>炮口尘：散布 7、回吸 6 px/f、基础缩放 1.1、开火段再按 65 帧周期加到 +2。沿用旧值 AI.cs:226,232</summary>
        public const float PulseDustSpread = 7f;
        public const float PulseDustSpeed = 6f;
        public const float PulseDustScale = 1.1f;
        public const float PulseDustScaleGain = 2f;
        /// <summary>开火段炮口尘的缩放周期也写成 65（与后坐归一化同一个数，和 45 帧的开火周期并不整齐）。沿用旧值 AI.cs:232</summary>
        public const int PulseDustScalePeriod = 65;
        public const int PulseChargeDustCount = 2;
        /// <summary>音效音量。沿用旧值 AI.cs:253</summary>
        public const float BeamSoundVolume = 0.13f;

        //==================== 血雨 bloodRain ====================

        /// <summary>150 帧前追踪：X 9.5（0.25 / 0.3 / 0.97）、Y 6.5（0.2 / 0.4 / 0.97），两轴都带 50 px 死区；之后整体 0.995 衰减。沿用旧值 AI.cs:264-289</summary>
        public const int BloodRainChaseFrames = 150;
        public const float BloodRainSpeedX = 9.5f;
        public const float BloodRainAccelX = 0.25f;
        public const float BloodRainTurnX = 0.3f;
        public const float BloodRainSpeedY = 6.5f;
        public const float BloodRainAccelY = 0.2f;
        public const float BloodRainTurnY = 0.4f;
        public const float BloodRainDamp = 0.97f;
        public const float BloodRainSettleDamp = 0.995f;
        /// <summary>每 5 帧撒 Timer/15 粒尘，散布 count×4。沿用旧值 AI.cs:278-285</summary>
        public const int BloodRainDustInterval = 5;
        public const int BloodRainDustPerFrames = 15;
        /// <summary>160 帧在前方 9 帧位移处大爆炸（伤害 55/55/50/45、击退 8）+ 抛出血球、获得 8 发弹药；180 帧收招。沿用旧值 AI.cs:291-303</summary>
        public const int BloodRainBoomFrame = 160;
        public static int BloodRainBoomDamage() => Helper.ScaleValueForDiffMode(55, 55, 50, 45);
        public const float BloodRainBoomKnockback = 8f;
        public const float BloodRainBallSpeed = 12f;
        public const float BloodRainBallKnockback = 8f;
        public const int BloodRainGainFollowers = 8;
        public const int BloodRainEndFrame = 180;
        /// <summary>“前方”= 速度 × 9 帧，所有大爆炸共用。沿用旧值 AI.cs:295,401,461</summary>
        public const float AheadFrames = 9f;

        //==================== 赤玉烟花 firework ====================

        /// <summary>移动：X 追踪 6 px/f（0.2 / 0.3 / 0.97）无死区，Y 追踪 4 px/f（0.2 / 0.3 / 0.97）带 50 px 死区。沿用旧值 AI.cs:312-319</summary>
        public const float FireworkSpeedX = 6f;
        public const float FireworkAccelX = 0.2f;
        public const float FireworkTurnX = 0.3f;
        public const float FireworkSpeedY = 4f;
        public const float FireworkAccelY = 0.2f;
        public const float FireworkTurnY = 0.3f;
        public const float FireworkDamp = 0.97f;
        /// <summary>第 2 帧起盾（250 帧）+ 无敌 + 反弹，并获得 3 发弹药（FTW 6）。沿用旧值 AI.cs:321-327</summary>
        public const int FireworkShieldFrame = 2;
        public const int FireworkShieldFrames = 250;
        public const int FireworkGainFollowers = 3;
        public const int FireworkGainFollowersFtw = 6;
        /// <summary>49 帧后每 25 帧一轮 3~5 枚（随机），寿命 16 + 10i 递增；265 帧收招。沿用旧值 AI.cs:338-362</summary>
        public const int FireworkWarmup = 49;
        public const int FireworkInterval = 25;
        public const int FireworkVolleyMin = 3;
        public const int FireworkVolleyMax = 6;
        public const int FireworkLifeBase = 16;
        public const int FireworkLifeStep = 10;
        public const float FireworkSpeed = 12f;
        public const float FireworkKnockback = 5f;
        public const int FireworkEndFrame = 265;
        public static int FireworkDamage() => Helper.GetProjDamage(60, 80, 120);

        //==================== 横向爆炸 explosionHorizontally ====================

        /// <summary>
        /// 160 帧前维持 300~350 px 的横向站位：远于 350 靠近、近于 300 后退、区间内 X 衰减（都是 9.5 / 0.3 / 0.4 / 0.97）；
        /// Y 追踪 7.5（0.3 / 0.4 / 0.97）带 50 px 死区；之后整体 0.997 衰减。沿用旧值 AI.cs:368-395
        /// </summary>
        public const int HorizontalChaseFrames = 160;
        public const float HorizontalKeepFar = 350f;
        public const float HorizontalKeepNear = 300f;
        public const float HorizontalSpeedX = 9.5f;
        public const float HorizontalAccelX = 0.3f;
        public const float HorizontalTurnX = 0.4f;
        public const float HorizontalSpeedY = 7.5f;
        public const float HorizontalAccelY = 0.3f;
        public const float HorizontalTurnY = 0.4f;
        public const float HorizontalDamp = 0.97f;
        public const float HorizontalSettleDamp = 0.997f;
        /// <summary>每 5 帧撒 Timer/13 粒尘。沿用旧值 AI.cs:384-391</summary>
        public const int HorizontalDustInterval = 5;
        public const int HorizontalDustPerFrames = 13;
        /// <summary>170 帧大爆炸（伤害 55/55/50/45、击退 8）+ 横向血浪、获得 8 发弹药；210 帧收招。沿用旧值 AI.cs:397-409</summary>
        public const int HorizontalBoomFrame = 170;
        public static int HorizontalBoomDamage() => Helper.ScaleValueForDiffMode(55, 55, 50, 45);
        public const float HorizontalBoomKnockback = 8f;
        public const float HorizontalWaveSpeed = 12f;
        public const float HorizontalWaveKnockback = 8f;
        public const int HorizontalGainFollowers = 8;
        public const int HorizontalEndFrame = 210;

        //==================== 赤色爆冲 dash ====================

        /// <summary>起手期 45 帧只转身不移动，朝向按 Timer/45 插值指向玩家。沿用旧值 AI.cs:421-429</summary>
        public const int DashAimFrames = 45;
        /// <summary>起手期每 4 帧一颗横向星，散布 70。沿用旧值 AI.cs:424-427</summary>
        public const int DashAimStarInterval = 4;
        public const float DashAimStarSpread = 70f;
        public const float DashAimStarScaleMin = 0.5f;
        public const float DashAimStarScaleMax = 0.8f;
        /// <summary>之后每 65 帧一轮，共五轮（45 + 65×5 = 370 帧收招）。沿用旧值 AI.cs:417,477</summary>
        public const int DashCycleFrames = 65;
        public const int DashCycleCount = 5;
        /// <summary>轮内第 18 帧起手音效 + 大闪光。沿用旧值 AI.cs:432-436</summary>
        public const int DashCueFrame = 18;
        public const float DashCueSparkScale = 1.5f;
        /// <summary>轮内前 20 帧慢速就位：X 5（0.2 / 0.3 / 0.97）、Y 4（0.15 / 0.3 / 0.97）带 50 px 死区。沿用旧值 AI.cs:438-448</summary>
        public const int DashWindupFrames = 20;
        public const float DashWindupSpeedX = 5f;
        public const float DashWindupAccelX = 0.2f;
        public const float DashWindupTurnX = 0.3f;
        public const float DashWindupSpeedY = 4f;
        public const float DashWindupAccelY = 0.15f;
        public const float DashWindupTurnY = 0.3f;
        public const float DashWindupDamp = 0.97f;
        /// <summary>轮内第 22 帧起冲：获得 3 发弹药，瞄向玩家下 / 上 50 px（按 Timer/70 奇偶交替），速度 14。沿用旧值 AI.cs:451-455</summary>
        public const int DashLaunchFrame = 22;
        public const int DashGainFollowers = 3;
        public const float DashAimOffsetY = 50f;
        /// <summary>交替周期 70：旧代码用 <c>Timer / 70 % 2</c> 而非轮长 65，原样保留。沿用旧值 AI.cs:454</summary>
        public const int DashAimAlternatePeriod = 70;
        public const float DashSpeed = 14f;
        /// <summary>轮内 51 帧前每 6 帧在前方 9 帧位移处放一个爆炸（伤害 20/30/35/30、击退 5）。沿用旧值 AI.cs:458-462</summary>
        public const int DashBoomEnd = 51;
        public const int DashBoomInterval = 6;
        public static int DashBoomDamage() => Helper.ScaleValueForDiffMode(20, 30, 35, 30);
        public const float DashBoomKnockback = 5f;
        /// <summary>轮内第 52 帧一记大爆炸（伤害 100/100/90/80、击退 5），之后每帧 0.97 刹车。沿用旧值 AI.cs:467-474</summary>
        public const int DashBigBoomFrame = 52;
        public static int DashBigBoomDamage() => Helper.ScaleValueForDiffMode(100, 100, 90, 80);
        public const float DashBigBoomKnockback = 5f;
        public const float DashBrakeDamp = 0.97f;

        //==================== 多段爆炸 explosion ====================

        /// <summary>前段（≤ 210 帧）追踪：X 9（0.28 / 0.38 / 0.97）无死区、Y 6.5（0.2 / 0.25 / 0.97）带 50 px 死区。沿用旧值 AI.cs:487-497</summary>
        public const int ExplosionFirstStage = 210;
        public const float ExplosionSpeedX = 9f;
        public const float ExplosionAccelX = 0.28f;
        public const float ExplosionTurnX = 0.38f;
        public const float ExplosionSpeedY = 6.5f;
        public const float ExplosionAccelY = 0.2f;
        public const float ExplosionTurnY = 0.25f;
        public const float ExplosionDamp = 0.97f;
        /// <summary>前段每 3 帧撒 (Timer%70)/10 粒尘，散布 count×4、缩放 1 + 0.06×count。沿用旧值 AI.cs:499-507</summary>
        public const int ExplosionDustInterval = 3;
        public const int ExplosionDustDivisor = 10;
        public const float ExplosionDustScaleGain = 0.06f;
        /// <summary>前段每 70 帧在前方大爆炸（伤害 50/50/45/45、击退 5）并获得 2 发弹药。沿用旧值 AI.cs:509-515</summary>
        public const int ExplosionInterval = 70;
        public static int ExplosionBoomDamage() => Helper.ScaleValueForDiffMode(50, 50, 45, 45);
        public const float ExplosionBoomKnockback = 5f;
        public const int ExplosionGainFollowers = 2;
        /// <summary>后段（211~309 帧）加速追击：X 11.5（0.38 / 0.54 / 0.97）、Y 7.5（0.3 / 0.44 / 0.97）带 50 px 死区，每 10 帧一个小爆炸（伤害 30/35/35/30）。沿用旧值 AI.cs:520-535</summary>
        public const int ExplosionSecondStage = 310;
        public const float ExplosionRushSpeedX = 11.5f;
        public const float ExplosionRushAccelX = 0.38f;
        public const float ExplosionRushTurnX = 0.54f;
        public const float ExplosionRushSpeedY = 7.5f;
        public const float ExplosionRushAccelY = 0.3f;
        public const float ExplosionRushTurnY = 0.44f;
        public const int ExplosionRushInterval = 10;
        public static int ExplosionRushDamage() => Helper.ScaleValueForDiffMode(30, 35, 35, 30);
        public const float ExplosionRushKnockback = 5f;
        /// <summary>后段每 5 帧撒 Timer/25 粒尘，散布 count×3、缩放 1 + 0.2×count。沿用旧值 AI.cs:537-545</summary>
        public const int ExplosionRushDustInterval = 5;
        public const int ExplosionRushDustPerFrames = 25;
        public const float ExplosionRushDustScaleGain = 0.2f;
        /// <summary>310 帧收尾大爆炸（伤害 80/80/75/70、击退 8）+ 8 发弹药，之后 0.9 硬刹到 320 帧收招。沿用旧值 AI.cs:549-563</summary>
        public static int ExplosionFinalDamage() => Helper.ScaleValueForDiffMode(80, 80, 75, 70);
        public const float ExplosionFinalKnockback = 8f;
        public const int ExplosionFinalGainFollowers = 8;
        public const float ExplosionBrakeDamp = 0.9f;
        public const int ExplosionEndFrame = 320;

        //==================== 向上射击 upShoot ====================

        /// <summary>X：距离超过 250 px 才追（7.5 / 0.2 / 0.3 / 0.97），否则 0.96 衰减。沿用旧值 AI.cs:573-577</summary>
        public const float UpShootKeepX = 250f;
        public const float UpShootSpeedX = 7.5f;
        public const float UpShootAccelX = 0.2f;
        public const float UpShootTurnX = 0.3f;
        public const float UpShootDampX = 0.97f;
        /// <summary>
        /// Y 自管：高于玩家不足 250 px 时以 0.25/帧 上浮到 -6；高出 400 px 以上按 0.2/帧 回落（旧代码这里的钳位写成
        /// <c>if (velocity.Y &lt; 6) velocity.Y = 6</c>，等于一帧直接拍到 +6，原样保留）；区间内 0.98 衰减。沿用旧值 AI.cs:579-593
        /// </summary>
        public const float UpShootRiseBelow = -250f;
        public const float UpShootFallAbove = -400f;
        public const float UpShootRiseAccel = 0.25f;
        public const float UpShootRiseLimit = -6f;
        public const float UpShootFallAccel = 0.2f;
        public const float UpShootFallLimit = 6f;
        public const float UpShootSettleDampY = 0.98f;
        /// <summary>30 帧后、200 帧前每 20 帧从随机一发弹药处向上射 2/3/3/4 枚（伤害 60/80/120、速度 10、散角 ±0.8），260 帧收招。沿用旧值 AI.cs:605-625</summary>
        public const int UpShootWarmup = 30;
        public const int UpShootFireEnd = 200;
        public const int UpShootInterval = 20;
        public static int UpShootDamage() => Helper.GetProjDamage(60, 80, 120);
        public static int UpShootCount() => Helper.ScaleValueForDiffMode(2, 3, 3, 4);
        public const float UpShootSpeed = 10f;
        public const float UpShootSpread = 0.8f;
        public const float UpShootKnockback = 5f;
        public const int UpShootEndFrame = 260;

        //==================== 射出炸弹 shootBomb ====================

        /// <summary>60 帧前追踪：X 9.5（0.2 / 0.3 / 0.97）、Y 7.5（0.2 / 0.4 / 0.97），两轴 50 px 死区；之后整体 0.995 衰减。沿用旧值 AI.cs:630-655</summary>
        public const int BombChaseFrames = 60;
        public const float BombSpeedX = 9.5f;
        public const float BombAccelX = 0.2f;
        public const float BombTurnX = 0.3f;
        public const float BombSpeedY = 7.5f;
        public const float BombAccelY = 0.2f;
        public const float BombTurnY = 0.4f;
        public const float BombDamp = 0.97f;
        public const float BombSettleDamp = 0.995f;
        /// <summary>每 5 帧撒 Timer/5 粒尘（本招蓄力最密）。沿用旧值 AI.cs:644-651</summary>
        public const int BombDustInterval = 5;
        public const int BombDustPerFrames = 5;
        /// <summary>
        /// 70 帧一次性甩出 7 枚扇形炸弹（i = -3..3，间隔 0.3 弧度）：每枚飞行 45~80 帧、初速 = 目标距离 × 0.55~2 / 飞行帧数（上限 16），
        /// 引爆倒计时 120~480 帧；每枚消耗 1 弹药，弹药耗尽提前停下。120 帧收招。沿用旧值 AI.cs:657-680
        /// </summary>
        public const int BombFireFrame = 70;
        public const int BombFanFrom = -3;
        public const int BombFanTo = 4;
        public const float BombFanStep = 0.3f;
        public const int BombFlightMin = 45;
        public const int BombFlightMax = 80;
        public const float BombSpeedScaleMin = 0.55f;
        public const float BombSpeedScaleMax = 2f;
        public const float BombSpeedCap = 16f;
        public const int BombFuseMin = 120;
        public const int BombFuseMax = 480;
        public const float BombKnockback = 4f;
        public const int BombEndFrame = 120;

        //==================== 赤玉激光 magicShoot ====================

        /// <summary>每 3 帧 6 粒回吸尘（散布 7、速度 3、缩放 1.3）。沿用旧值 AI.cs:699-702</summary>
        public const int MagicDustInterval = 3;
        public const int MagicDustCount = 6;
        public const float MagicDustSpread = 7f;
        public const float MagicDustSpeed = 3f;
        public const float MagicDustScale = 1.3f;
        /// <summary>蓄力 60 帧（环插值爬坡），140 帧后收环（插值 0.1），每 15 帧一发，155 帧收招。沿用旧值 AI.cs:706-737</summary>
        public const int MagicChargeFrames = 60;
        public const int MagicAimEnd = 140;
        public const float MagicIdleLerp = 0.1f;
        public const int MagicInterval = 15;
        public const int MagicEndFrame = 155;
        /// <summary>
        /// 后坐周期 15：旧代码写成 <c>Timer % 15 / 15</c>（两边都是 int，整数除法恒为 0），于是激光环的后坐实际不动。
        /// 原样保留（写进报告“建议”）。沿用旧值 AI.cs:1075
        /// </summary>
        public const int MagicRecoilPeriod = 15;
        /// <summary>激光：伤害 60/80/120、速度 14、击退 5。沿用旧值 AI.cs:727-728</summary>
        public static int MagicDamage() => Helper.GetProjDamage(60, 80, 120);
        public const float MagicSpeed = 14f;
        public const float MagicKnockback = 5f;

        //==================== 召唤血玉灵 summon ====================

        /// <summary>每 10 帧在末位弹药处 6 粒尘（散布 20、缩放 1.3）。沿用旧值 AI.cs:758-763</summary>
        public const int SummonDustInterval = 10;
        public const int SummonDustCount = 6;
        public const float SummonDustSpread = 20f;
        public const float SummonDustScale = 1.3f;
        /// <summary>每 40 帧召唤一只，场上小怪上限 2/3/3/4，200 帧收招。沿用旧值 AI.cs:765-791</summary>
        public const int SummonInterval = 40;
        public static int SummonMinionCap() => Helper.ScaleValueForDiffMode(2, 3, 3, 4);
        public const int SummonEndFrame = 200;

        //==================== 通用表现 ====================

        /// <summary>大爆炸震屏（强度 10、6f、20 帧、1000 距离）。沿用旧值 AI.cs:153</summary>
        public const int BoomShakeStrength = 10;
        public const float BoomShakeVibration = 6f;
        public const int BoomShakeFrames = 20;
        public const float ShakeFalloffDistance = 1000f;
        /// <summary>宝石尘散布基准：出生动画 count×3，其余招式 count×4。沿用旧值 AI.cs:177,283</summary>
        public const float ChargeDustSpreadNarrow = 3f;
        public const float ChargeDustSpreadWide = 4f;
        /// <summary>蓄力尘缩放增益：绝大多数招式都是 1 + 0.2×count（多段爆炸前段的 0.06 是唯一例外）。沿用旧值 AI.cs:283,389,504,649</summary>
        public const float ChargeDustScaleGain = 0.2f;
        /// <summary>
        /// 所有状态的超时兜底：本 boss 最长的招式是多段爆炸 320 帧、爆冲 370 帧，取 700 帧——
        /// 370 帧的正常上限之外还留出近一倍余量，真跑到这里就是软锁，强制收招。
        /// </summary>
        public const int StateTimeoutFrames = 700;
    }
}
