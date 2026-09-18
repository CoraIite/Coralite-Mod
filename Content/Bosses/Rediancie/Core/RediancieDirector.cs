using Coralite.Helpers;

namespace Coralite.Content.Bosses.Rediancie.Core
{
    /// <summary>
    /// 赤玉灵战斗调参中心：全部数字与阶段档位的唯一出口。<br/>
    /// 本轮是结构迁移，战斗设计不变：每条常量注明来源，查不到设计理由的一律写“沿用旧值 Rediancie.cs:行号”（迁移前的行号）。
    /// 弹药机制、循环表与阶段阈值的设计口径见同目录 <c>Rediancie_AI_Description.md</c>。
    /// </summary>
    internal static class RediancieDirector
    {
        //==================== 编制 / 出生 / 脱战 ====================

        /// <summary>出生自带 3 发弹药：让首招三连炸一开始就有可击碎的护甲。沿用旧值 Rediancie.cs:312</summary>
        public const int SpawnFollowerCount = 3;
        /// <summary>出生点在最近玩家头顶 600 px（约一屏高）：给入场动画留出下沉距离。沿用旧值 Rediancie.cs:317</summary>
        public const float SpawnHeightAboveTarget = 600f;
        /// <summary>目标离开 3000 px 视为脱战。沿用旧值 Rediancie.cs:326</summary>
        public const float DespawnDistance = 3000f;
        /// <summary>脱战：X 衰减、Y 轻微下坠、鼓励消失。沿用旧值 Rediancie.cs:332-334</summary>
        public const float DespawnDampX = 0.97f;
        public const float DespawnGravity = 0.04f;
        public const int DespawnEncourageFrames = 10;

        /// <summary>弹药上限按难度 6/9/12/18（设计文档“弹药机制”）。Rediancie.cs:1252</summary>
        public static int MaxFollowers() => Helper.ScaleValueForDiffMode(6, 9, 12, 18);
        /// <summary>
        /// 重建弹药列表时的上限 6/9/12/24：FTW 档与 <see cref="MaxFollowers"/> 不一致（18 vs 24），旧代码即如此。
        /// 沿用旧值 Rediancie.cs:1283；是否统一写进报告“建议”，不在本轮落地。
        /// </summary>
        public static int MaxFollowersOnRespawn() => Helper.ScaleValueForDiffMode(6, 9, 12, 24);
        /// <summary>累计受伤每满 50/60/75/100 击碎一发弹药：伤害越高的难度弹药越耐打。沿用旧值 Rediancie.cs:228</summary>
        public static int FollowerBreakDamage() => Helper.ScaleValueForDiffMode(50, 60, 75, 100);
        /// <summary>每发弹药 +1 防御（设计文档“弹药机制”）。Rediancie.cs:1265</summary>
        public const int DefensePerFollower = 1;

        /// <summary>二阶段阈值：血量低于一半（设计文档“招式循环”）。Rediancie.cs:866</summary>
        public const float Phase2LifeRatio = 0.5f;
        /// <summary>选招循环方式数（one_one / two_one / two_two 三选一随机）。Rediancie.cs:967</summary>
        public const int CyclingTypeCount = 3;

        /// <summary>一轮循环里近战 / 远程各几手（设计文档“基础循环模式”）。旧 GetAICycling，Rediancie.cs:1204-1222</summary>
        public static void GetCycling(RediancieCyclingType cyclingType, out int meleeCount, out int shootCount)
        {
            switch (cyclingType)
            {
                case RediancieCyclingType.two_one:
                    meleeCount = 2;
                    shootCount = 1;
                    break;
                case RediancieCyclingType.two_two:
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
        /// 硬锁一（不与上一手相同）开关。旧设计允许同招连发：two_one / two_two 的两手近战在普通模式必然都是三连炸，
        /// 大师模式的 <c>Main.rand.Next(2)</c> 也允许重复；为保战斗设计不变此处关闭，hub 只做提交口与记账。
        /// 打开后 Commit 会在同池里重掷一次避开上一手，池只有一员时照旧放行。
        /// </summary>
        public const bool ForbidImmediateRepeat = false;
        /// <summary>查重窗口长度（记账用，本轮不裁决）：11 个状态取 1/3 ≈ 4。</summary>
        public const int RecentPickWindow = 4;
        /// <summary>
        /// hub 停留帧数。旧代码收招当帧即 <c>ResetState</c> 切下一招、零间隔（间隔已含在各招尾段），
        /// 为不改节奏取 0：<see cref="States.RediancieHubState.EndAttack"/> 在 0 时直接经 Commit 返回下一招，不多占一帧。
        /// </summary>
        public const int HubFrames = 0;

        //==================== 运动 / 朝向（宿主 ApplyDeclaredMovement 消费）====================

        /// <summary>常规朝向：目标角 = 速度模长 × 0.04 × 面向，按 0.01 弧度/帧逼近。沿用旧值 Rediancie.cs:1237-1238</summary>
        public const float RotationPerSpeed = 0.04f;
        public const float RotationTowardsStep = 0.01f;
        /// <summary>冲刺刹车段朝向插值。沿用旧值 Rediancie.cs:677</summary>
        public const float RotationLerpDash = 0.08f;
        /// <summary>下砸段回正插值。沿用旧值 Rediancie.cs:1094</summary>
        public const float RotationLerpSlam = 0.1f;
        /// <summary>Hold 模式（漏声明兜底）的速度衰减；正常迁移后不会命中。</summary>
        public const float HoldDamp = 0.9f;

        /// <summary>悬停上浮（远程招通用）：X 衰减 0.98、Y 加速 -0.14、上限 -1.5；比玩家高 150 px 以上改为整体 0.99 衰减。沿用旧值 Rediancie.cs:454-458</summary>
        public const float HoverDampX = 0.98f;
        public const float HoverAccelY = -0.14f;
        public const float HoverLimitY = -1.5f;
        public const float HoverAboveTargetY = -150f;
        public const float HoverFarDamp = 0.99f;
        /// <summary>分轴追踪的 Y 死区 50 px 与死区内的 Y 衰减。沿用旧值 Rediancie.cs:515-518</summary>
        public const float ChaseDeadZoneY = 50f;
        public const float ChaseDeadZoneDampY = 0.96f;

        //==================== 弹药环绕几何（Followers，双端同算）====================

        /// <summary>待机环：角速度 0.08/帧 + 0.15×速度，半径 38 + 速度/2。沿用旧值 Rediancie.cs:1323-1324</summary>
        public const float IdleRotPerFrame = 0.08f;
        public const float IdleRotPerSpeed = 0.15f;
        public const float IdleRadius = 38f;
        public const float IdleRadiusPerSpeed = 0.5f;
        /// <summary>环面绕 X 轴倾角：1.57 − clamp(Δy/200, ±1)×0.4，随玩家高低俯仰。沿用旧值 Rediancie.cs:1326</summary>
        public const float CircleTiltBase = 1.57f;
        public const float CircleTiltRange = 200f;
        public const float CircleTiltMax = 0.4f;
        /// <summary>透视投影深度与逐弹药上下起伏。沿用旧值 Rediancie.cs:1457-1459</summary>
        public const float ProjectionDepth = 1000f;
        public const float IdleBobAmp = 6f;
        public const float IdleBobFreq = 1.2f;
        /// <summary>待机环位置插值 0.6、朝向插值 0.2、缩放 0.9 − z×0.2。沿用旧值 Rediancie.cs:1320,1461-1463</summary>
        public const float IdleLerp = 0.6f;
        public const float FollowerRotLerp = 0.2f;
        public const float IdleScaleBase = 0.9f;
        public const float IdleScaleDepth = 0.2f;

        /// <summary>赤玉雨环：角速度 0.1、半径 38 + 0.5×Timer 逐渐张开，缩放深度随半径/168 加深。沿用旧值 Rediancie.cs:1335-1336,1481</summary>
        public const float UpShootRotPerFrame = 0.1f;
        public const float UpShootRadiusPerFrame = 0.5f;
        public const float UpShootScaleDepthGain = 0.4f;
        public const float UpShootScaleLengthRef = 168f;
        /// <summary>召唤 / 死亡环：角速度 0.06、半径 38 + clamp(Timer×30, 0, 30)。沿用旧值 Rediancie.cs:1381-1382</summary>
        public const float SummonRotPerFrame = 0.06f;
        public const float SummonRadiusGrow = 30f;
        /// <summary>烟花环：平面环半径 46 + 30，位置插值从 0.1 在 60 帧内爬到 0.6，朝向插值 0.6。沿用旧值 Rediancie.cs:1393-1402</summary>
        public const float FireworkRadius = 46f;
        public const float FireworkLerpBase = 0.1f;
        public const float FireworkLerpGain = 0.5f;
        public const float FireworkLerpRamp = 60f;
        public const float FireworkRotLerp = 0.6f;

        /// <summary>瞄准环（激光 / 脉冲共用）：因子 = 距离/150 钳 0..1，炮口 = 本体 + 方向×(32 + 32×因子)，环面倾斜 0.2 + 方向分量×因子×1.1。沿用旧值 Rediancie.cs:461-463,1371-1372</summary>
        public const float AimFactorRange = 150f;
        public const float AimMuzzleBase = 32f;
        public const float AimMuzzleGain = 32f;
        public const float AimTiltBase = 0.2f;
        public const float AimTiltGain = 1.1f;
        /// <summary>激光环：半径 36 + 后坐 60；70 帧起才有后坐。沿用旧值 Rediancie.cs:1361-1370</summary>
        public const float MagicRingRadius = 36f;
        public const float MagicRingRecoil = 60f;
        public const int MagicRecoilStart = 70;
        /// <summary>脉冲环：半径 26 + 后坐 86，炮口弹药再前伸 16、放大 1.3。沿用旧值 Rediancie.cs:1417-1434</summary>
        public const float PulseRingRadius = 26f;
        public const float PulseRingRecoil = 86f;
        public const float PulseMuzzleForward = 16f;
        public const float PulseMuzzleScale = 1.3f;
        /// <summary>后坐曲线 x·sin(x³)/1.186，x = 1.465×(1 − t)：先猛后缓的一记回弹。沿用旧值 Rediancie.cs:1366-1367</summary>
        public const float RecoilCurveX = 1.465f;
        public const float RecoilCurveNorm = 1.186f;
        /// <summary>瞄准环缩放深度 0.3、朝向插值 0.4、角速度 0.1/帧。沿用旧值 Rediancie.cs:1358,1422,1499-1501</summary>
        public const float AimRingScaleDepth = 0.3f;
        public const float AimRingRotLerp = 0.4f;
        public const float AimRingRotPerFrame = 0.1f;

        //==================== 出生动画 onSpawnAnim ====================

        /// <summary>第 1 帧生成名牌弹幕并以 1.5 px/f 下沉。沿用旧值 Rediancie.cs:404-408</summary>
        public const int SpawnAnimNameLineFrame = 1;
        public const float SpawnAnimSinkSpeed = 1.5f;
        /// <summary>每 5 帧撒 Timer/25 粒宝石尘，越接近爆炸越密。沿用旧值 Rediancie.cs:412-419</summary>
        public const int SpawnAnimDustInterval = 5;
        public const int SpawnAnimDustPerFrames = 25;
        public const float SpawnAnimDustSpread = 3f;
        public const float SpawnAnimDustScaleGain = 0.2f;
        /// <summary>120 帧后下沉速度按 0.998 缓慢衰减。沿用旧值 Rediancie.cs:422-423</summary>
        public const int SpawnAnimSlowFrame = 120;
        public const float SpawnAnimSlowDamp = 0.998f;
        /// <summary>260 帧大爆炸（伤害 55、击退 8）+ 震屏，270 帧解除无敌进首招三连炸。沿用旧值 Rediancie.cs:425-445</summary>
        public const int SpawnAnimBoomFrame = 260;
        public const int SpawnAnimBoomDamage = 55;
        public const float SpawnAnimBoomKnockback = 8f;
        public const int SpawnAnimEndFrame = 270;

        //==================== 死亡动画 onKillAnim ====================

        /// <summary>上浮：X 衰减 0.96、Y 加速 -0.05、上限 -0.5。沿用旧值 Rediancie.cs:367</summary>
        public const float KillAnimDampX = 0.96f;
        public const float KillAnimAccelY = -0.05f;
        public const float KillAnimLimitY = -0.5f;
        /// <summary>30 帧后开演；40 帧起每 20 帧掉一组碎块；230 帧前每 15 帧一次小爆炸特效。沿用旧值 Rediancie.cs:371-382</summary>
        public const int KillAnimQuietFrames = 30;
        public const int KillAnimGoreStart = 40;
        public const int KillAnimGoreInterval = 20;
        public const int KillAnimSparkEnd = 230;
        public const int KillAnimSparkInterval = 15;
        /// <summary>245 帧大爆炸（伤害 55、击退 8），250 帧真正死亡。沿用旧值 Rediancie.cs:80,384-399</summary>
        public const int KillAnimBoomFrame = 245;
        public const int KillAnimBoomDamage = 55;
        public const float KillAnimBoomKnockback = 8f;
        public const int KillAnimEndFrame = 250;
        /// <summary>碎块与特效的随机散布范围。沿用旧值 Rediancie.cs:376-382</summary>
        public const float KillAnimScatterX = 30f;
        public const float KillAnimScatterY = 40f;

        //==================== 赤色脉冲 pulse ====================

        /// <summary>蓄力 125 帧（前 60 帧插值爬坡），之后每 65 帧一发直到 255 帧，275 帧收招。沿用旧值 Rediancie.cs:464-505</summary>
        public const int PulseChargeFrames = 125;
        public const int PulseRampFrames = 60;
        public const int PulseCycleFrames = 65;
        public const int PulseFireEnd = 255;
        public const int PulseEndFrame = 275;
        /// <summary>收尾环插值 0.08。沿用旧值 Rediancie.cs:481</summary>
        public const float PulseIdleLerp = 0.08f;
        /// <summary>弹幕：伤害 20/25/30、速度 12、瞄准散布 48 px、击退 5。沿用旧值 Rediancie.cs:493-496</summary>
        public static int PulseDamage() => Helper.GetProjDamage(20, 25, 30);
        public const float PulseSpeed = 12f;
        public const float PulseSpread = 48f;
        public const float PulseKnockback = 5f;
        /// <summary>炮口尘：散布 7、回吸 6 px/f、基础缩放 1.1、蓄满再加 2。沿用旧值 Rediancie.cs:472,478</summary>
        public const float PulseDustSpread = 7f;
        public const float PulseDustSpeed = 6f;
        public const float PulseDustScale = 1.1f;
        public const float PulseDustScaleGain = 2f;
        public const int PulseChargeDustCount = 2;
        /// <summary>音效音量。沿用旧值 Rediancie.cs:499</summary>
        public const float BeamSoundVolume = 0.13f;

        //==================== 赤玉烟花 firework ====================

        /// <summary>移动：X 追踪 2 px/f（加速 0.1 / 转向 0.1 / 衰减 0.97），Y 追踪 1 px/f（0.06 / 0.06 / 0.97）。沿用旧值 Rediancie.cs:511-516</summary>
        public const float FireworkSpeedX = 2f;
        public const float FireworkAccelX = 0.1f;
        public const float FireworkTurnX = 0.1f;
        public const float FireworkSpeedY = 1f;
        public const float FireworkAccelY = 0.06f;
        public const float FireworkTurnY = 0.06f;
        public const float FireworkDamp = 0.97f;
        /// <summary>第 2 帧起盾（250 帧）+ 无敌 + 反弹，并获得 3 发弹药（FTW 6）。沿用旧值 Rediancie.cs:520-526</summary>
        public const int FireworkShieldFrame = 2;
        public const int FireworkShieldFrames = 250;
        public const int FireworkGainFollowers = 3;
        public const int FireworkGainFollowersFtw = 6;
        /// <summary>49 帧后每 25 帧一轮 3 发（FTW 4 发），寿命 16 + 10i 递增；265 帧收招。沿用旧值 Rediancie.cs:537-561</summary>
        public const int FireworkWarmup = 49;
        public const int FireworkInterval = 25;
        public const int FireworkPerVolley = 3;
        public const int FireworkPerVolleyFtw = 4;
        public const int FireworkLifeBase = 16;
        public const int FireworkLifeStep = 10;
        public const float FireworkSpeed = 12f;
        public const float FireworkKnockback = 5f;
        public const int FireworkEndFrame = 265;
        public static int FireworkDamage() => Helper.GetProjDamage(20, 25, 30);

        //==================== 蓄力大爆炸 accumulate ====================

        /// <summary>325 帧前快速追踪：X 7.5（0.12 / 0.15 / 0.97）、Y 4.5（0.06 / 0.08 / 0.97）；之后整体 0.995 衰减。沿用旧值 Rediancie.cs:567-589</summary>
        public const int AccumulateChaseFrames = 325;
        public const float AccumulateSpeedX = 7.5f;
        public const float AccumulateAccelX = 0.12f;
        public const float AccumulateTurnX = 0.15f;
        public const float AccumulateSpeedY = 4.5f;
        public const float AccumulateAccelY = 0.06f;
        public const float AccumulateTurnY = 0.08f;
        public const float AccumulateDamp = 0.97f;
        public const float AccumulateSettleDamp = 0.995f;
        /// <summary>蓄力尘与出生动画同参数。沿用旧值 Rediancie.cs:578-585</summary>
        public const int AccumulateDustInterval = 5;
        /// <summary>85 / 125 / 187 帧三次判定：比玩家高 20 px 以上时 1/2 概率改下砸。沿用旧值 Rediancie.cs:592-599</summary>
        public static readonly int[] AccumulateSlamCheckFrames = { 85, 125, 187 };
        public const int AccumulateSlamChance = 2;
        /// <summary>330 帧在前方 9 帧位移处大爆炸（伤害 30/45/70、击退 8）并获得 6 发弹药；340 帧收招。沿用旧值 Rediancie.cs:610-627</summary>
        public const int AccumulateBoomFrame = 330;
        public static int AccumulateBoomDamage() => Helper.GetProjDamage(30, 45, 70);
        public const float AccumulateBoomKnockback = 8f;
        public const int AccumulateGainFollowers = 6;
        public const int AccumulateEndFrame = 340;
        /// <summary>“前方”= 速度 × 9 帧，爆炸与爆冲、三连炸共用。沿用旧值 Rediancie.cs:615,670,751</summary>
        public const float AheadFrames = 9f;
        /// <summary>下砸触发的高度门槛：比玩家高 20 px。沿用旧值 Rediancie.cs:595</summary>
        public const float SlamHeightMargin = 20f;

        //==================== 赤色爆冲 dash ====================

        /// <summary>每 100 帧一轮，300 帧收招（三段）。沿用旧值 Rediancie.cs:635,699</summary>
        public const int DashCycleFrames = 100;
        public const int DashEndFrame = 300;
        /// <summary>第 18 帧起手音效 + 大闪光。沿用旧值 Rediancie.cs:639-643</summary>
        public const int DashCueFrame = 18;
        public const float DashCueSparkScale = 1.5f;
        /// <summary>前 20 帧慢速就位：X / Y 都是 2 px/f（0.1 / 0.1 / 0.97）。沿用旧值 Rediancie.cs:645-653</summary>
        public const int DashWindupFrames = 20;
        public const float DashWindupSpeed = 2f;
        public const float DashWindupAccel = 0.1f;
        public const float DashWindupTurn = 0.1f;
        public const float DashWindupDamp = 0.97f;
        /// <summary>第 22 帧起冲：获得 2 发弹药，瞄向玩家上 / 下 100 px（奇偶轮交替），速度 10。沿用旧值 Rediancie.cs:658-663</summary>
        public const int DashLaunchFrame = 22;
        public const int DashGainFollowers = 2;
        public const float DashAimOffsetY = 100f;
        public const float DashSpeed = 10f;
        /// <summary>71 帧前每 10 帧在前方 9 帧位移处放一个爆炸（伤害 20/25/30、击退 5）。沿用旧值 Rediancie.cs:665-671</summary>
        public const int DashBoomEnd = 71;
        public const int DashBoomInterval = 10;
        public static int DashBoomDamage() => Helper.GetProjDamage(20, 25, 30);
        public const float DashBoomKnockback = 5f;
        /// <summary>刹车段每帧 0.98 衰减；第 99 帧比玩家高时 1/3 概率改下砸。沿用旧值 Rediancie.cs:679-687</summary>
        public const float DashBrakeDamp = 0.98f;
        public const int DashSlamCheckFrame = 99;
        public const int DashSlamChance = 3;

        //==================== 三连炸 explosion ====================

        /// <summary>追踪：X 6.5（0.12 / 0.22 / 0.97）、Y 4.5（0.06 / 0.06 / 0.97）。沿用旧值 Rediancie.cs:708-715</summary>
        public const float ExplosionSpeedX = 6.5f;
        public const float ExplosionAccelX = 0.12f;
        public const float ExplosionTurnX = 0.22f;
        public const float ExplosionSpeedY = 4.5f;
        public const float ExplosionAccelY = 0.06f;
        public const float ExplosionTurnY = 0.06f;
        public const float ExplosionDamp = 0.97f;
        /// <summary>每 3 帧撒 (Timer%80)/10 粒尘，缩放 1 + 0.1×count。沿用旧值 Rediancie.cs:717-724</summary>
        public const int ExplosionDustInterval = 3;
        public const int ExplosionDustDivisor = 10;
        public const float ExplosionDustScaleGain = 0.1f;
        /// <summary>125 / 187 帧、仅大师模式：比玩家高时 1/2 概率改下砸。沿用旧值 Rediancie.cs:728-735</summary>
        public static readonly int[] ExplosionSlamCheckFrames = { 125, 187 };
        public const int ExplosionSlamChance = 2;
        /// <summary>每 80 帧在前方 9 帧位移处爆炸（伤害 20/25/40、击退 5）并获得 1 发弹药；250 帧收招。沿用旧值 Rediancie.cs:746-756</summary>
        public const int ExplosionInterval = 80;
        public static int ExplosionDamage() => Helper.GetProjDamage(20, 25, 40);
        public const float ExplosionKnockback = 5f;
        public const int ExplosionGainFollowers = 1;
        public const int ExplosionEndFrame = 250;

        //==================== 赤玉雨 upShoot ====================

        /// <summary>30 帧后每 40 帧从随机一发弹药处向上射 2/2/3/4 枚（伤害 20/25/35、速度 8、散角 ±0.5），260 帧收招。沿用旧值 Rediancie.cs:780-800</summary>
        public const int UpShootWarmup = 30;
        public const int UpShootInterval = 40;
        public static int UpShootDamage() => Helper.GetProjDamage(20, 25, 35);
        public static int UpShootCount() => Helper.ScaleValueForDiffMode(2, 2, 3, 4);
        public const float UpShootSpeed = 8f;
        public const float UpShootSpread = 0.5f;
        public const float UpShootKnockback = 5f;
        public const int UpShootEndFrame = 260;

        //==================== 赤玉激光 magicShoot ====================

        /// <summary>每 3 帧 6 粒回吸尘（散布 7、速度 3、缩放 1.3）。沿用旧值 Rediancie.cs:816-819</summary>
        public const int MagicDustInterval = 3;
        public const int MagicDustCount = 6;
        public const float MagicDustSpread = 7f;
        public const float MagicDustSpeed = 3f;
        public const float MagicDustScale = 1.3f;
        /// <summary>蓄力 60 帧（环插值爬坡），140 帧后收环（插值 0.1），每 35 帧一发，155 帧收招。沿用旧值 Rediancie.cs:823-854</summary>
        public const int MagicChargeFrames = 60;
        public const int MagicAimEnd = 140;
        public const float MagicIdleLerp = 0.1f;
        public const int MagicInterval = 35;
        public const int MagicEndFrame = 155;
        /// <summary>激光：伤害 20/25/30、速度 10、击退 5。沿用旧值 Rediancie.cs:844-845</summary>
        public static int MagicDamage() => Helper.GetProjDamage(20, 25, 30);
        public const float MagicSpeed = 10f;
        public const float MagicKnockback = 5f;

        //==================== 召唤小赤玉灵 summon ====================

        /// <summary>每 10 帧在末位弹药处 6 粒尘（散布 20、缩放 1.3）。沿用旧值 Rediancie.cs:1001-1007</summary>
        public const int SummonDustInterval = 10;
        public const int SummonDustCount = 6;
        public const float SummonDustSpread = 20f;
        public const float SummonDustScale = 1.3f;
        /// <summary>每 40 帧召唤一只，场上小赤玉灵上限 2/3/3/4，200 帧收招（设计文档“召唤小赤玉灵”）。Rediancie.cs:1011-1037</summary>
        public const int SummonInterval = 40;
        public static int SummonMinionCap() => Helper.ScaleValueForDiffMode(2, 3, 3, 4);
        public const int SummonEndFrame = 200;

        //==================== 下砸 slamDown ====================

        /// <summary>上升段：X 速度从 10 在 50 帧内线性降到 0.5（加速 0.2 / 转向 0.4 / 衰减 0.95）。沿用旧值 Rediancie.cs:1051-1052</summary>
        public const float SlamRiseSpeedXFrom = 10f;
        public const float SlamRiseSpeedXTo = 0.5f;
        public const float SlamRiseSpeedXRamp = 50f;
        public const float SlamRiseAccelX = 0.2f;
        public const float SlamRiseTurnX = 0.4f;
        public const float SlamRiseDampX = 0.95f;
        /// <summary>前 40 帧向上加速 0.3、上限 -6；之后 Y 按 0.95 衰减；50 帧后转下落。沿用旧值 Rediancie.cs:1055-1092</summary>
        public const int SlamRiseAccelFrames = 40;
        public const float SlamRiseAccelY = 0.3f;
        public const float SlamRiseLimitY = -6f;
        public const float SlamRiseSettleDampY = 0.95f;
        public const int SlamRiseFrames = 50;
        /// <summary>第 1 帧预判线：宽 = 本体宽/3、长 100、瞄准宽 = 本体宽/4、20 帧出现 + 15 帧停留。沿用旧值 Rediancie.cs:1071-1083</summary>
        public const int SlamTelegraphLength = 100;
        public const int SlamTelegraphSpawnTime = 20;
        public const int SlamTelegraphHoldTime = 15;
        public const int SlamTelegraphWidthDiv = 3;
        public const int SlamTelegraphAimWidthDiv = 4;
        /// <summary>下落段：Y 加速 1.2、上限 22，X 每帧 0.98 衰减；低于玩家 500 px 视为砸空。沿用旧值 Rediancie.cs:1101-1110</summary>
        public const float SlamFallAccelY = 1.2f;
        public const float SlamFallMaxY = 22f;
        public const float SlamFallDampX = 0.98f;
        public const float SlamMissDistance = 500f;
        /// <summary>只在不低于玩家 100 px 时才检测脚下 3 行物块。沿用旧值 Rediancie.cs:1120-1126</summary>
        public const float SlamGroundCheckMargin = 100f;
        public const int SlamGroundCheckRows = 3;
        /// <summary>落地：反弹 -3、大爆炸（伤害 30/50/70、击退 8）、获得 5 发弹药、向上射 3/4/6/9 枚（散角 ±0.4、速度 8~12、伤害 20/35/45）。沿用旧值 Rediancie.cs:1133-1166</summary>
        public const float SlamLandBounceY = -3f;
        public static int SlamBoomDamage() => Helper.GetProjDamage(30, 50, 70);
        public const float SlamBoomKnockback = 8f;
        public const int SlamGainFollowers = 5;
        public static int SlamStrikeCount() => Helper.ScaleValueForDiffMode(3, 4, 6, 9);
        public static int SlamStrikeDamage() => Helper.GetProjDamage(20, 35, 45);
        public const float SlamStrikeSpread = 0.4f;
        public const float SlamStrikeSpeedMin = 8f;
        public const float SlamStrikeSpeedMax = 12f;
        public const float SlamStrikeKnockback = 5f;
        /// <summary>落地后 0.95 衰减，20 帧后收招。沿用旧值 Rediancie.cs:1184-1186</summary>
        public const float SlamLandDamp = 0.95f;
        public const int SlamLandFrames = 20;

        //==================== 通用表现 ====================

        /// <summary>大爆炸震屏（强度 10、6f、20 帧、1000 距离）与落地震屏（8、5f、15 帧）。沿用旧值 Rediancie.cs:393,1149</summary>
        public const int BoomShakeStrength = 10;
        public const float BoomShakeVibration = 6f;
        public const int BoomShakeFrames = 20;
        public const float ShakeFalloffDistance = 1000f;
        public const int LandShakeStrength = 8;
        public const float LandShakeVibration = 5f;
        public const int LandShakeFrames = 15;
        /// <summary>宝石尘散布基准（出生 / 蓄力 / 三连炸共用 count×3）。沿用旧值 Rediancie.cs:417</summary>
        public const float ChargeDustSpreadPerCount = 3f;
        /// <summary>所有状态的超时兜底：任何招式的正常时长上限是 340 帧，600 帧仍没退就是软锁，强制收招。</summary>
        public const int StateTimeoutFrames = 600;
    }
}
