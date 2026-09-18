using Coralite.Helpers;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core
{
    /// <summary>
    /// 梦魇世纪花的唯一数字出口。<br/>
    /// 每条常量注明来源：能查到设计意图的写意图，查不到的写“沿用旧值 &lt;文件&gt;:&lt;行&gt;”——不编造理由。<br/>
    /// 纯粹的局部插值系数（AngleTowards 的 0.3、Lerp 的 0.85 一类）留在状态里，它们只描述“这一步走多少”，
    /// 搬出来反而看不懂，这是 skill 允许的例外。
    /// </summary>
    internal static class NightmarePlanteraDirector
    {
        //==================== 全局 ====================

        /// <summary>状态超时兜底帧数。旧代码没有兜底，取 40 秒——比最长的演出（三阶段瞬移转圈 360 + 收招）还长一倍，
        /// 只在真卡死时兜底，不会截断任何一招。</summary>
        public const int StateTimeoutFrames = 60 * 40;

        /// <summary>演出态（入场 / 转阶段）的超时兜底：这些招本来就长，给 60 秒。</summary>
        public const int CinematicTimeoutFrames = 60 * 60;

        /// <summary>帧动画换帧间隔与总帧数。沿用旧值 Phase.P2_Dream.cs:172-177</summary>
        public const int FrameInterval = 7;
        public const int FrameCount = 4;

        /// <summary>脱战下沉重力与催促离场帧数。沿用旧值 NightmarePlantera.cs:450-452</summary>
        public const float DespawnGravity = 0.25f;
        public const int DespawnEncourageFrames = 10;

        /// <summary>天空效果的存活倒计时，每帧重置。沿用旧值 Phase.P2_Dream.cs:38</summary>
        public const int SkyTimeleft = 100;

        /// <summary>一阶段结束血线（3/4）。沿用旧值 NightmarePlantera.cs:652</summary>
        public const float Phase1EndLifeRatio = 3f / 4f;
        /// <summary>二阶段结束血线（1/8），PhaseController 的唯一条件。沿用旧值 NightmarePlantera.cs:509</summary>
        public const float Phase2EndLifeRatio = 1f / 8f;
        /// <summary>二阶段选招时的提前转三阶段血线（1/5）。沿用旧值 Phase.P2_Dream.cs:2421</summary>
        public const float Phase2SelfExchangeLifeRatio = 1f / 5f;

        //==================== 狂暴（白天） ====================

        /// <summary>向上加速与最高上升速度。沿用旧值 AI.Rampage.cs:20-24</summary>
        public const float RampageRiseAccel = 0.25f;
        public const float RampageMaxRiseSpeed = -32f;

        //==================== P0 入场演出 ====================

        /// <summary>吸收裂缝段时长；镜头拉近每帧步进。沿用旧值 Phase.P0_OnSpawnAnmi.cs:28,68</summary>
        public const int SpawnAbsorbFrames = 60;
        public const float SpawnCameraZoomInStep = 0.03f;
        /// <summary>吸能弹幕投放间隔与半径区间。沿用旧值 Phase.P0_OnSpawnAnmi.cs:56-64</summary>
        public const int SpawnEnergyInterval = 8;
        public const int SpawnEnergyMinDistance = 400;
        public const int SpawnEnergyMaxDistance = 600;
        public const int SpawnEnergyMaxCount = 6;

        /// <summary>成形段：前 60 帧继续吸能（间隔 15、半径 200~300），扭曲每帧涨 1/60。沿用旧值 :87-115</summary>
        public const int SpawnFormFogFrames = 60;
        public const int SpawnFormEnergyInterval = 15;
        public const int SpawnFormEnergyMinDistance = 200;
        public const int SpawnFormEnergyMaxDistance = 300;
        public const float SpawnFormWarpStep = 1f / 60f;
        /// <summary>60~100 帧淡入本体（每帧 0.75/60），之后放尘土到 140 帧，140~200 收扭曲。沿用旧值 :119-142</summary>
        public const int SpawnFormAlphaEndFrame = 100;
        public const float SpawnFormAlphaStep = 0.75f / 60f;
        public const float SpawnFormRotStep = 0.02f;
        public const int SpawnFormDustEndFrame = 140;
        public const int SpawnFormFrames = 200;
        /// <summary>破壳爆发：3 发定向 + 12 发散射。沿用旧值 :150-161</summary>
        public const int SpawnBurstMainCount = 3;
        public const float SpawnBurstMainSpeed = 48f;
        public const int SpawnBurstExtraCount = 12;
        public const int SpawnBurstExtraSpeedMin = 10;
        public const int SpawnBurstExtraSpeedMax = 20;
        public const float SpawnShakeLevel = 5f;

        /// <summary>收尾段：20 帧内镜头回位、本体淡入、扭曲外扩。沿用旧值 :181-203</summary>
        public const int SpawnSettleFrames = 20;
        public const float SpawnSettleCameraStep = 1f / 20f;
        public const float SpawnSettleAlphaStep = 0.75f / 20f;
        public const float SpawnSettleWarpStep = 0.3f;
        public const float SpawnSettleRotStep = 0.04f;
        public const int SpawnSettleFogPerFrame = 6;

        //==================== P1 入梦 ====================

        /// <summary>钩爪数量，本体位置由三只钩爪的重心决定。沿用旧值 Phase.P1_Sleeping.cs:18,49-57</summary>
        public const int HookCount = 3;
        /// <summary>移动：基础限速 / 加速度，两档血量提速。沿用旧值 Phase.P1_Sleeping.cs:65-74</summary>
        public const float P1SpeedBase = 2.5f;
        public const float P1AccelBase = 0.05f;
        public const float P1SpeedTier1 = 5f;
        public const float P1AccelTier1 = 0.075f;
        public const float P1SpeedTier2 = 7f;
        /// <summary>两档提速血线 15/16 与 13/16。沿用旧值 Phase.P1_Sleeping.cs:67,73</summary>
        public const float P1Tier1LifeRatio = 15f / 16f;
        public const float P1Tier2LifeRatio = 13f / 16f;
        /// <summary>专家 / FTW 的移动加成。沿用旧值 Phase.P1_Sleeping.cs:76-88</summary>
        public const float P1ExpertSpeedAdd = 1f;
        public const float P1ExpertSpeedMul = 1.1f;
        public const float P1ExpertAccelAdd = 0.03f;
        public const float P1ExpertAccelMul = 1.2f;
        public const float P1GoodSpeedMul = 1.15f;
        public const float P1GoodAccelMul = 1.3f;
        /// <summary>钩爪重心到玩家的最大延伸距离（专家 +250）。沿用旧值 Phase.P1_Sleeping.cs:96-99</summary>
        public const int P1MaxReach = 800;
        public const int P1ExpertReachAdd = 250;

        /// <summary>待机时长；受击与低血量各让计时多走一格。沿用旧值 Phase.P1_Sleeping.cs:155-167</summary>
        public const int P1IdleFrames = 180;
        /// <summary>贴脸判定距离与沉眠之雾的触发概率分母。沿用旧值 Phase.P1_Sleeping.cs:344</summary>
        public const float P1CloseDistance = 300f;
        public const int P1FogChance = 3;

        /// <summary>沉眠之雾：蓄力 160 帧、收招 200 帧；蓄力圈半径 60 收到 10。沿用旧值 Phase.P1_Sleeping.cs:179-212</summary>
        public const int FogChargeFrames = 160;
        public const int FogEndFrames = 200;
        public const float FogRingRadius = 60f;
        public const float FogRingShrink = 50f;
        public const float FogSpread = 0.35f;
        public const float FogSpeedMin = 6f;
        public const float FogSpeedMax = 14f;
        public const float FogRecoil = 6f;

        /// <summary>黑暗之触：30 帧瞄准、75 帧收招；触手自由前摇 60、最长 240，两档血量各 -20 / +80 / +2 条。沿用旧值 :224-264</summary>
        public const int TentacleAimFrames = 30;
        public const int TentacleEndFrames = 75;
        public const int TentacleNotFreeTime = 60;
        public const int TentacleMaxFreeTime = 240;
        public const int TentacleNotFreeStep = -20;
        public const int TentacleMaxFreeStep = 80;
        public const int TentacleExtraStep = 2;
        public const float TentacleExtraTimeScale = 0.75f;
        public const float TentacleSpeed = 8f;
        public const float TentacleRecoil = 6f;
        public const float TentacleExtraAngle = 1.1f;
        public const float TentacleExtraAngleJitter = 0.2f;

        /// <summary>黑暗飞叶：射击间隔（难度缩放后每档血量再 -1），每 4 次射一组三连。沿用旧值 :277-299</summary>
        public const int LeafVolleyEvery = 4;
        public const float LeafSpeed = 13f;
        public const float LeafVolleySpread = 0.45f;
        public const float LeafJitter = 0.35f;
        public const int LeafJitterChance = 3;
        public const float LeafRecoil = 2f;
        /// <summary>持续时长 14×4，每档血量再加一段。沿用旧值 Phase.P1_Sleeping.cs:316-321</summary>
        public const int LeafDurationStep = 14 * 4;

        //==================== P1 → P2 转阶段演出 ====================

        public const float ExchangeDamp = 0.9f;
        public const float ExchangeCameraStep = 0.02f;
        /// <summary>蓄力 80 帧 → 第一次炸开 140 帧 → 第二次炸开 100 帧 → 标题 120 帧。沿用旧值 Phase.Exchange_P1_P2.cs:47,90,131,188</summary>
        public const int ExchangeChargeFrames = 80;
        public const int ExchangeBurst1Frames = 140;
        public const int ExchangeBurst2Frames = 100;
        public const int ExchangeTitleFrames = 120;
        public const float ExchangeWarpStep = 0.3f;
        public const float ExchangeWarpMax = 10f;
        public const float ExchangeShake1 = 3f;
        public const int ExchangeShake1Delay = 2;
        public const float ExchangeShake2 = 8f;
        public const int ExchangeShake2Delay = 3;
        public const int ExchangeFogPerFrame = 3;
        public const int ExchangeSoundInterval = 8;
        public const int ExchangeDustInterval = 3;
        /// <summary>标题段：字号涨到 16、天空在第 30 帧点亮、120 帧后淡出。沿用旧值 :170-195</summary>
        public const float ExchangeNameScaleMax = 16f;
        public const float ExchangeNameScaleStep = 1f;
        public const int ExchangeSkyFrame = 30;
        public const float ExchangeNameAlphaStep = 0.05f;
        public const float ExchangeNameAlphaEnd = 0.06f;

        //==================== 秒杀（噩梦值满） ====================

        /// <summary>尖刺环：每 8 帧一根，半径 1000，预警 180 帧。沿用旧值 Phase.SuddenDeath.cs:15-21</summary>
        public const int SuddenSpikeInterval = 8;
        public const float SuddenSpikeRadius = 1000f;
        public const int SuddenSpikeShootTime = 180;
        public const float SuddenSpikeAi1 = 40f;
        public const float SuddenSpikeAi3 = 1100f;
        public const float SuddenTeleportMin = 450f;
        public const float SuddenTeleportMax = 600f;
        /// <summary>咬击伤害写死 999999（秒杀本来就是处决）。沿用旧值 Phase.SuddenDeath.cs:33</summary>
        public const int SuddenBiteDamage = 999999;
        public const float SuddenBiteAi1 = 60f;
        /// <summary>扑咬：距离 220 以外加速到 24。沿用旧值 Phase.SuddenDeath.cs:42-50</summary>
        public const float SuddenLungeDistance = 220f;
        public const float SuddenLungeAccel = 0.65f;
        public const float SuddenLungeMaxSpeed = 24f;
        public const int SuddenLungeFrames = 55;
        public const float SuddenLungeDamp = 0.8f;
        public const float SuddenRecoverDamp = 0.9f;
        public const int SuddenRecoverRotFrame = 15;
        public const int SuddenRecoverFrames = 30;

        //==================== P3 公共 ====================

        /// <summary>三阶段瞬移淡出的默认帧数与尘土数量。沿用旧值 Phase3_Nightemare.cs:1397,1430</summary>
        public const int P3FadeFrames = 30;
        public const int P3FadeDustCount = 16;
        public const int P3TeleportFogCount = 8;
        /// <summary>咬击落点：以玩家为心的环形随机半径。沿用旧值 Phase3_Nightemare.cs:270,363</summary>
        public const float P3BiteTeleportMin = 450f;
        public const float P3BiteTeleportMax = 600f;
        /// <summary>扑咬三件套：距离阈值、加速系数、限速。沿用旧值 Phase3_Nightemare.cs:308-318</summary>
        public const float P3LungeDistance = 220f;
        public const float P3LungeAccel = 0.8f;
        public const float P3LungeMaxSpeed = 30f;
        public const float P3LungeDamp = 0.8f;
        /// <summary>三阶段转阶段后的额外防御。沿用旧值 Phase3_Nightemare.cs:1546</summary>
        public const int P3ExtraDefense = 10;
        /// <summary>选招循环长度：MoveCount 0~10 走一轮固定表。沿用旧值 Phase3_Nightemare.cs:1533</summary>
        public const int P3MoveCycleLength = 10;

        /// <summary>
        /// 硬锁一（不许与上一手相同）。<b>默认关</b>：旧设计里 MoveCount 2 / 5 / 7 三个槽都是"随机一种咬击"，
        /// 连着出同一种咬是原本就允许的节奏（Phase3_Nightemare.cs:1507-1520），开了它等于改战斗设计。
        /// 这是一个留给调参的阀，不是默认值。
        /// </summary>
        public const bool P3ForbidImmediateRepeat = false;

        /// <summary>窗口查重上限：同一招在最近 <see cref="NightmarePlanteraContext.RecentPickCapacity"/> 手里最多出现几次。同上，默认不限。</summary>
        public const int P3RepeatCap = int.MaxValue;

        //==================== P2 → P3 转阶段 ====================

        /// <summary>抬到玩家头顶 250 蓄力。沿用旧值 Phase3_Nightemare.cs:127</summary>
        public const float P3ExchangeRiseHeight = 250f;
        /// <summary>蓄力 80 帧，每 10 帧回 1.6% 上限血。沿用旧值 Phase3_Nightemare.cs:166-172,203</summary>
        public const int P3ExchangeFrames = 80;
        public const int P3ExchangeHealInterval = 10;
        public const float P3ExchangeHealRatio = 0.016f;
        public const float P3ExchangeWarpStep = 0.3f;
        public const float P3ExchangeRotStep = 0.3f;
        public const float P3ExchangeDamp = 0.9f;
        public const int P3ExchangeFogPerFrame = 3;
        public const int P3ExchangeSoundInterval = 8;

        //==================== 幻影撕咬 illusionBite ====================

        /// <summary>绕玩家的公转半径与角速度（400 帧一圈）。沿用旧值 Phase3_Nightemare.cs:222-232</summary>
        public const float IllusionOrbitRadius = 500f;
        public const int IllusionOrbitPeriod = 400;
        public const float IllusionOrbitTurn = 0.08f;
        public const float IllusionOrbitSpeedRange = 450f;
        public const float IllusionOrbitMaxSpeed = 34f;
        /// <summary>每 8 帧一次假咬（伤害 1，只吓人）。沿用旧值 Phase3_Nightemare.cs:243-256</summary>
        public const int IllusionFakeInterval = 8;
        public const int IllusionFakeDamage = 1;
        public const float IllusionFakeAi0 = 2f;
        public const float IllusionFakeAi1 = 40f;
        /// <summary>真咬前的落点抖动与预警时长。沿用旧值 Phase3_Nightemare.cs:270-276</summary>
        public const float IllusionRealJitter = 100f;
        public const float IllusionRealAi0 = 1f;
        public const float IllusionRealAi1 = 50f;
        /// <summary>真咬后再等 40 帧收束。沿用旧值 Phase3_Nightemare.cs:280</summary>
        public const int IllusionRealTail = 40;
        /// <summary>落地咬：第 10、20 帧各一次，71 帧结束。沿用旧值 Phase3_Nightemare.cs:296-322</summary>
        public const int P3BiteFirstFrame = 10;
        public const int P3BiteSecondFrame = 20;
        public const int P3BiteBodyFrames = 71;
        public const float P3BiteAi1 = 60f;
        /// <summary>收招：阻尼 0.88，第 5 帧起回正，15 帧结束。沿用旧值 Phase3_Nightemare.cs:388-401</summary>
        public const float P3RecoverDamp = 0.88f;
        public const int P3RecoverRotFrame = 5;
        public const int P3RecoverFrames = 15;

        //==================== 噩梦撕咬 P3_nightmareBite ====================

        public const int P3NightmareBiteBodyFrames = 72;
        public const int P3NightmareBiteRotFrame = 10;
        public const int P3NightmareBiteRecoverFrames = 20;

        //==================== 噩梦冲刺 P3_nightmareDash ====================

        /// <summary>两轮冲刺共用。绕行半径从 140 拉到 480，40 帧转满一圈。沿用旧值 Phase3_Nightemare.cs:448-470</summary>
        public const int DashTeleportMin = 125;
        public const int DashTeleportMax = 150;
        public const int DashFadeFrames = 20;
        public const int DashFadeFramesShort = 10;
        public const int DashWindupFrames = 48;
        public const int DashSpinFrames = 40;
        public const float DashRadiusMin = 140f;
        public const float DashRadiusMax = 480f;
        public const float DashWindupTurn = 0.5f;
        public const float DashWindupSpeedRange = 300f;
        public const float DashWindupMaxSpeed = 56f;
        public const float DashWindupBlend = 0.85f;
        public const float DashWindupDamp = 0.98f;
        public const float DashHoldDamp = 0.97f;
        public const int DashHoldFrames = 6;
        public const float DashSpeed = 48f;
        public const int DashRecover1Frames = 15;
        public const int DashRecover2DampFrame = 12;
        public const int DashRecover2Frames = 17;
        public const int DashRecover3DampFrame = 10;
        public const int DashRecover3Frames = 14;

        //==================== 虚假撕咬 P3_fakeBite ====================

        public const float FakeBiteAi0 = 4f;
        public const float FakeBiteAi1 = 50f;
        public const int FakeBiteBodyFrames = 70;
        public const int FakeBiteSpeedLineFrames = 60;
        public const int FakeBiteSpeedLineInterval = 3;
        /// <summary>收招同时从藤蔓里放 3 对尖刺，越往后越晚越窄。沿用旧值 Phase3_Nightemare.cs:655-672</summary>
        public const int FakeBiteSpikePairs = 3;
        public const int FakeBiteSpikeBaseTime = 35;
        public const int FakeBiteSpikeTimeStep = 45;
        public const float FakeBiteSpikeAngle = 0.9f;
        public const float FakeBiteSpikeAngleStep = -0.15f;
        public const float FakeBiteSpikeBaseStep = 0.25f;
        /// <summary>收招绕行：半径 450，正弦摆幅 PiOver4/4，145 帧。沿用旧值 Phase3_Nightemare.cs:680-696</summary>
        public const float FakeBiteOrbitRadius = 450f;
        public const float FakeBiteOrbitWaveSpeed = 0.0314f;
        public const float FakeBiteOrbitTurn = 0.5f;
        public const float FakeBiteOrbitSpeedRange = 400f;
        public const float FakeBiteOrbitMaxSpeed = 56f;
        public const float FakeBiteOrbitBlend = 0.15f;
        public const int FakeBiteOrbitFrames = 145;

        //==================== 三重尖刺地狱 tripleSpikeHell ====================

        public const int TripleTeleportMin = 700;
        public const int TripleTeleportMax = 800;
        public const int TripleRounds = 3;
        public const int TripleRollingFrames = 120;
        public const float TripleOrbitRadius = 800f;
        public const float TripleOrbitTurn = 0.08f;
        public const float TripleOrbitSpeedRange = 300f;
        public const float TripleOrbitMaxSpeed = 56f;
        public const float TripleOrbitBlend = 0.85f;
        public const int TripleHoleInterval = 7;
        public const float TripleHoleOffset = 200f;
        public const float TripleHoleAi1 = 40f;
        public const float TripleHoleAi3 = 1100f;
        public const int TripleFadeFrames = 25;

        //==================== 群花乱舞 flowerDance ====================

        public const int FlowerFadeFrames = 60;
        public const int FlowerChargeFrames = 120;
        public const float FlowerTentacleRadius = 170f;
        public const int FlowerCameraInterval = 10;
        public const float FlowerWarpStep = 0.3f;
        public const float FlowerRotStep = 0.3f;
        public const float FlowerDamp = 0.5f;
        public const int FlowerFogPerFrame = 2;
        public const int FlowerSoundInterval = 8;
        public const int FlowerLeafInterval = 6;
        public const int FlowerLeafPerRing = 7;
        public const float FlowerLeafSpeed = 12f;
        /// <summary>冲刺起手：退到玩家侧后 450，再垂直偏 700 作为冲刺终点。沿用旧值 Phase3_Nightemare.cs:928-944</summary>
        public const float FlowerDashBack = 450f;
        public const float FlowerDashSide = 700f;
        public const float FlowerDashSpeed = 48f;
        public const float FlowerDashBiteAi1 = 10f;
        public const int FlowerDashFrames = 20;
        public const float FlowerDashDamp = 0.9f;
        public const int FlowerDashTail = 15;
        /// <summary>收招绕圈与炸开时刻。沿用旧值 Phase3_Nightemare.cs:985-1008</summary>
        public const float FlowerCircleDistance = 340f;
        public const float FlowerCircleSpeed = 16f;
        public const float FlowerCircleAccel = 0.25f;
        public const float FlowerCircleRolling = 180f;
        public const int FlowerExplodeFrame = 35;
        public const int FlowerEndFrames = 9 * 3 + 65;

        //==================== 超级钩爪斩 superHookSlash ====================

        public const int HookSlashTeleportMin = 500;
        public const int HookSlashTeleportMax = 600;
        public const int HookSlashMinRounds = 2;
        public const int HookSlashMaxRounds = 3;
        public const int HookSlashInterval = 15;
        public const int HookSlashPerVolley = 2;
        public const float HookSlashOffset = 64f;
        public const float HookSlashSpreadStep = 0.25f;
        public const float HookSlashAi2 = 120f;
        public const int HookSlashTail = 80;
        public const float HookSlashRecoverDamp = 0.96f;
        public const int HookSlashRecoverFrames = 135;

        //==================== 尖刺与闪光 P3_SpikesAndSparkles ====================

        public const int SpikesTeleportMin = 700;
        public const int SpikesTeleportMax = 800;
        public const int SpikesOpeningHoles = 8;
        public const float SpikesOpeningSpread = 0.4f;
        public const int SpikesOpeningDistMin = 500;
        public const int SpikesOpeningDistMax = 800;
        public const float SpikesOpeningTime = 90f;
        public const int SpikesOpeningLenMin = 800;
        public const int SpikesOpeningLenMax = 1200;
        public const float SpikesOrbitRadius = 850f;
        public const float SpikesOrbitTurn = 0.08f;
        public const float SpikesOrbitSpeedRange = 400f;
        public const float SpikesOrbitMaxSpeed = 30f;
        public const float SpikesOrbitBlend = 0.25f;
        public const int SpikesHoleInterval = 12;
        public const int SpikesSparkleInterval = 28;
        public const float SpikesSparkleSpread = 0.15f;
        public const int SpikesBodyFrames = 300;
        public const float SpikesCircleDistance = 650f;
        public const float SpikesCircleSpeed = 56f;
        public const float SpikesCircleAccel = 0.7f;
        public const float SpikesCircleRolling = 550f;
        public const float SpikesCircleAngleFactor = 0.34f;
        public const int SpikesRecoverFrames = 30;

        //==================== 瞬移闪光 P3_teleportSparkles ====================

        public const int TpSparkleOpenDistMin = 300;
        public const int TpSparkleOpenDistMax = 400;
        public const int TpSparkleFrames = 360;
        public const int TpSparkleRampFrames = 120;
        public const float TpSparkleRotStep = 0.35f;
        public const int TpSparkleFogPerFrame = 2;
        public const int TpSparkleBaseDelay = 30;
        public const int TpSparkleDelayCut = 20;
        public const int TpSparklePerVolley = 5;
        public const float TpSparkleTentacleRadius = 170f;
        public const int TpSparkleReTeleportInterval = 100;
        public const int TpSparkleReTeleportMin = 550;
        public const int TpSparkleReTeleportMax = 650;
        public const int TpSparkleEndDistMin = 400;
        public const int TpSparkleEndDistMax = 600;
        public const float TpSparkleEndHeight = -600f;
        public const float TpSparkleCircleDistance = 340f;
        public const float TpSparkleCircleSpeed = 16f;
        public const float TpSparkleCircleAccel = 0.25f;
        public const float TpSparkleCircleRolling = 180f;
        public const int TpSparkleRecoverFrames = 45;

        //==================== 藤蔓喷发 vineSpurt ====================

        public const float VineTeleportAngleA = -0.75f;
        public const float VineTeleportAngleB = 0.57f;
        public const int VineTeleportMin = 350;
        public const int VineTeleportMax = 450;
        public const int VineDurationMin = 4;
        public const int VineDurationMax = 6;
        public const int VineDurationStep = 25;
        public const int VineDurationBase = 40;
        public const float VineSpikeAngle = 0.9f;
        public const int VineSpikeFirstTime = 65;
        public const int VineSpikeInterval = 25;
        public const float VineOrbitRadius = 400f;
        public const float VineOrbitTurn = 0.3f;
        public const float VineOrbitSpeedRange = 300f;
        public const float VineOrbitMaxSpeed = 56f;
        public const float VineOrbitBlend = 0.45f;
        public const int VineFinaleCount = 4;
        public const float VineFinaleSpread = 2f;
        public const int VineFinaleTime = 110;
        public const float VineRecoverRadius = 550f;
        public const int VineRecoverFrames = 120;
        /// <summary>收尾四连的起手偏角与绕行正弦频率。沿用旧值 Phase3_Nightemare.cs:1359,1328</summary>
        public const float VineFinaleBaseOffset = -1f;
        public const float VineOrbitWave = 0.0314f;

        //==================== 三阶段补充（写状态时补齐的裸数字） ====================

        /// <summary>三阶段咬击类弹幕的统一伤害档位。整个三阶段十几处都是这一组数，集中到一处。沿用旧值 Phase3_Nightemare.cs:246 等</summary>
        public static int P3BiteDamage() => Helper.ScaleValueForDiffMode(30, 20, 15, 15);

        /// <summary>幻影撕咬的回合数（三轮假咬 + 一次真咬）与假咬的环形落点半径。沿用旧值 Phase3_Nightemare.cs:217-237</summary>
        public const int IllusionRounds = 3;
        public const float IllusionFakeRadius = 400f;

        /// <summary>冲刺瞄点的前置量：玩家朝向 80 px + 速度 14 帧。沿用旧值 Phase3_Nightemare.cs:468,509</summary>
        public const float DashAimLead = 80f;
        public const float DashAimVelocityLead = 14f;
        /// <summary>冲刺后摇每帧的花瓣拖尾数量与收招阻尼。沿用旧值 Phase3_Nightemare.cs:522,541</summary>
        public const int DashTrailDustCount = 5;
        public const float DashTailDamp = 0.9f;

        /// <summary>虚假撕咬收招时荆棘刺的初速偏角（±2 rad，纯视觉方向）。沿用旧值 Phase3_Nightemare.cs:669-670</summary>
        public const float FakeBiteSpikeSideAngle = 2f;

        /// <summary>三重尖刺地狱的三臂布局（每臂间隔 TwoPi/3）与收束段阻尼。沿用旧值 Phase3_Nightemare.cs:720,765</summary>
        public const int TripleSpikeArms = 3;
        public const float TripleCloseDamp = 0.93f;

        /// <summary>群花乱舞：星尘间隔、触手转圈圈数、蓄力前后半段分界、震屏强度。沿用旧值 Phase3_Nightemare.cs:868-938</summary>
        public const int FlowerDustInterval = 4;
        public const float FlowerTentacleSpinTurns = 10f;
        public const int FlowerChargeHalfFrames = 60;
        public const float FlowerShakeLevel = 5f;
        /// <summary>横穿冲刺那一下的咬击与裂隙伤害档位（比常规咬击低，因为路径固定、躲得掉）。沿用旧值 Phase3_Nightemare.cs:964-966</summary>
        public static int FlowerDanceBiteDamage() => Helper.ScaleValueForDiffMode(20, 10, 5, 5);
        public static int FlowerSlitDamage() => Helper.ScaleValueForDiffMode(45, 30, 25, 20);

        /// <summary>叶环的每发偏角、换色周期与换色时的跳步。沿用旧值 Phase3_Nightemare.cs:924-935</summary>
        public const float FlowerLeafSpread = 0.14f;
        public const int FlowerLeafColorEvery = 9;
        public const int FlowerLeafColorStep = 3;

        /// <summary>爪击左右交替的周期（每 30 帧换一侧）。沿用旧值 Phase3_Nightemare.cs:1039</summary>
        public const int HookSlashAlternatePeriod = 30;

        /// <summary>尖刺与闪光：绕行正弦频率、两个尖刺洞的随机区间、噩梦光的发数与散布。沿用旧值 Phase3_Nightemare.cs:1101-1136</summary>
        public const float SpikesOrbitWave = 0.0314f;
        public const int SpikesRoundsMin = 1;
        public const int SpikesRoundsMaxExclusive = 3;
        public const int SpikesShootCountMin = 3;
        public const int SpikesShootCountMaxExclusive = 5;
        public const float SpikesHoleJitter = 0.4f;
        public const float SpikesHoleFutureScale = 3f;
        public const int SpikesHoleDistMin = 500;
        public const int SpikesHoleDistMax = 800;
        public const int SpikesHoleTimeMin = 60;
        public const int SpikesHoleTimeMax = 80;
        public const int SpikesHoleLenMin = 600;
        public const int SpikesHoleLenMax = 900;
        public const float SpikesHole2Spread = 0.3f;
        public const int SpikesHole2Period = 24;
        public const int SpikesHole2DistMin = 500;
        public const int SpikesHole2DistMax = 700;
        public const float SpikesHole2Jitter = 0.2f;
        public const float SpikesHole2Time = 75f;
        public const int SpikesHole2LenMin = 800;
        public const int SpikesHole2LenMax = 1200;
        public const int SpikesSparkleMinCount = 1;
        public const int SpikesSparkleMaxCountExclusive = 4;
        public const float SpikesSparkleHalfSpread = 0.075f;

        /// <summary>瞬移闪光的触手转圈圈数。沿用旧值 Phase3_Nightemare.cs:1218</summary>
        public const float TpSparkleTentacleSpinTurns = 10f;

        //==================== P2 公共 ====================

        /// <summary>二阶段瞬移淡出的默认帧数。沿用旧值 Phase.P2_Dream.cs:2254</summary>
        public const int P2FadeFrames = 45;
        /// <summary>咬击落点：以扑击对象为心的环形随机半径。沿用旧值 Phase.P2_Dream.cs:204,397,494</summary>
        public const float P2BiteTeleportMin = 450f;
        public const float P2BiteTeleportMax = 600f;
        /// <summary>扑咬三件套：距离阈值、加速系数、限速、刹车阻尼。沿用旧值 Phase.P2_Dream.cs:228-241</summary>
        public const float P2LungeDistance = 220f;
        public const float P2LungeAccel = 0.65f;
        public const float P2LungeMaxSpeed = 26f;
        public const float P2LungeDamp = 0.8f;
        /// <summary>二阶段咬击类弹幕的统一伤害档位。整个二阶段十几处都是这一组数。沿用旧值 Phase.P2_Dream.cs:209 等</summary>
        public static int P2BiteDamage() => Helper.ScaleValueForDiffMode(20, 10, 10, 10);
        /// <summary>二阶段荆棘刺 / 尖刺类弹幕的伤害档位。沿用旧值 Phase.P2_Dream.cs:457 等</summary>
        public static int P2SpikeDamage() => Helper.ScaleValueForDiffMode(30, 20, 15, 15);
        /// <summary>选招轮换表长度：MoveCount 0~11 走一轮。沿用旧值 Phase.P2_Dream.cs:2459</summary>
        public const int P2MoveCycleLength = 11;

        /// <summary>
        /// 二阶段的硬锁一与窗口查重。<b>默认关</b>：旧轮换表里 1 / 6 / 8 三个槽都是"随机一种咬击"，
        /// 连着出同一种咬是原本就允许的节奏（Phase.P2_Dream.cs:2433-2447），开了它等于改战斗设计。
        /// </summary>
        public const bool P2ForbidImmediateRepeat = false;
        public const int P2RepeatCap = int.MaxValue;

        //==================== 噩梦之咬 nightmareBite ====================

        /// <summary>咬击预警时长 75；扑咬 66 帧；收招阻尼 0.9，第 10 帧起回正，20 帧结束。沿用旧值 :210-256</summary>
        public const float P2BiteAi1 = 75f;
        public const int P2BiteLungeFrames = 66;
        public const float P2BiteRecoverDamp = 0.9f;
        public const int P2BiteRecoverRotFrame = 10;
        public const int P2BiteRecoverFrames = 20;

        //==================== 噩梦冲刺 nightmareDash ====================

        /// <summary>贴身瞬移半径 125~150，淡出只给 30 帧（起手要快）。沿用旧值 Phase.P2_Dream.cs:279-286</summary>
        public const int P2DashTeleportMin = 125;
        public const int P2DashTeleportMax = 150;
        public const int P2DashFadeFrames = 30;
        public const float P2DashBiteAi1 = 90f;
        /// <summary>绕行蓄力 78 帧：前 55 帧自转一圈，半径从 140 拉到 480，第 70 帧起放手滑行。沿用旧值 :301-331</summary>
        public const int P2DashWindupFrames = 78;
        public const int P2DashSpinFrames = 55;
        public const int P2DashGlideFrame = 70;
        public const float P2DashRadiusMin = 140f;
        public const float P2DashRadiusMax = 480f;
        public const float P2DashWindupTurn = 0.5f;
        public const float P2DashWindupSpeedRange = 300f;
        public const float P2DashWindupMaxSpeed = 56f;
        public const float P2DashWindupBlend = 0.85f;
        public const float P2DashWindupDamp = 0.98f;
        /// <summary>蓄满后停 6 帧再以 48 速度冲出；瞄点前置 = 玩家朝向 80 px + 速度 14 帧。沿用旧值 :340-351</summary>
        public const float P2DashHoldDamp = 0.97f;
        public const int P2DashHoldFrames = 6;
        public const float P2DashSpeed = 48f;
        /// <summary>冲刺后摇：12 帧后开始刹车，每帧 5 片花瓣拖尾，20 帧结束。沿用旧值 :357-374</summary>
        public const int P2DashTailDampFrame = 12;
        public const float P2DashTailDamp = 0.9f;
        public const int P2DashTrailDustCount = 5;
        public const int P2DashTailFrames = 20;

        //==================== 虚假撕咬 fakeBite ====================

        /// <summary>张嘴预警 65 帧、ai0=4 表示"只张嘴不咬"。沿用旧值 Phase.P2_Dream.cs:402</summary>
        public const float P2FakeBiteAi0 = 4f;
        public const float P2FakeBiteAi1 = 65f;
        public const int P2FakeBiteBodyFrames = 65;
        public const int P2FakeBiteSoundFrame = 10;
        public const int P2FakeBiteSpeedLineFrames = 50;
        public const int P2FakeBiteSpeedLineInterval = 3;
        /// <summary>速度线的甩出速度区间（纯表现）。沿用旧值 Phase.P2_Dream.cs:432</summary>
        public const float P2SpeedLineSpeedMin = 12f;
        public const float P2SpeedLineSpeedMax = 24f;
        /// <summary>速度线相对头朝向的左右偏角。沿用旧值 Phase.P2_Dream.cs:432</summary>
        public const float P2SpeedLineSideAngle = 2.6f;
        /// <summary>收招甩两根荆棘刺：夹角 ±0.9、初速偏角 ±2，起爆时间 35 / 65 错开。沿用旧值 :458-459</summary>
        public const float P2FakeBiteSpikeAngle = 0.9f;
        public const float P2FakeBiteSpikeSideAngle = 2f;
        public const int P2FakeBiteSpikeTimeA = 35;
        public const int P2FakeBiteSpikeTimeB = 65;
        /// <summary>收招绕行：半径 450，正弦摆幅 PiOver4/4，80 帧。沿用旧值 :465-477</summary>
        public const float P2FakeBiteOrbitRadius = 450f;
        public const float P2FakeBiteOrbitWave = 0.0314f;
        public const float P2FakeBiteOrbitTurn = 0.5f;
        public const float P2FakeBiteOrbitSpeedRange = 400f;
        public const float P2FakeBiteOrbitMaxSpeed = 56f;
        public const float P2FakeBiteOrbitBlend = 0.15f;
        public const int P2FakeBiteOrbitFrames = 80;

        //==================== 美梦相助 fantasyHelp ====================

        /// <summary>与噩梦之咬同一套节拍，只是预警更长（125）、扑咬更久（116），并在起手放一只美梦光。沿用旧值 :500-538</summary>
        public const float P2FantasyHelpAi1 = 125f;
        public const int P2FantasyHelpLungeFrames = 116;
        public const float P2FantasyHelpSparkleAi0 = 3f;

        //==================== 转圈咬 rollingThenBite / 下方闪光咬 belowSparkleThenBite ====================

        /// <summary>自旋段总长 360 帧、末尾 45 帧淡出。两招共用同一段。沿用旧值 Phase.P2_Dream.cs:582-583,745-746</summary>
        public const int P2RollingFrames = 360;
        public const int P2RollingFadeFrames = 45;
        /// <summary>自旋加速：前 1/3 段把每帧转速从 0 拉到 0.35。沿用旧值 :595-596</summary>
        public const float P2RollingSpinStep = 0.35f;
        /// <summary>弹幕间隔从 30 缩到 10（转得越快打得越密），下方版本从 35 缩到 15。沿用旧值 :598,785</summary>
        public const int P2RollingDelayBase = 30;
        public const int P2RollingDelayCut = 20;
        public const int P2BelowDelayBase = 35;
        /// <summary>自旋时四向 / 三向喷噩梦光。沿用旧值 :602,789</summary>
        public const int P2RollingSparkleCount = 4;
        public const int P2BelowSparkleCount = 3;
        /// <summary>自旋时触手绕本体转 10 圈、半径 170。沿用旧值 :618-620</summary>
        public const float P2RollingTentacleRadius = 170f;
        public const float P2RollingTentacleSpinTurns = 10f;
        /// <summary>自旋段每帧的雾气数量。沿用旧值 :592</summary>
        public const int P2RollingFogPerFrame = 1;
        /// <summary>转圈咬的落点：玩家侧方 400~600、上方 600，以 48 的速度直落。沿用旧值 :658-665</summary>
        public const int P2RollingBiteSideMin = 400;
        public const int P2RollingBiteSideMax = 600;
        public const float P2RollingBiteHeight = -600f;
        public const float P2RollingBiteSpeed = 48f;
        /// <summary>裂缝咬击的预警只有 10 帧——它靠 600 px 的助跑本身做预警。沿用旧值 :676</summary>
        public const float P2SlitBiteAi1 = 10f;
        /// <summary>裂缝弹幕的伤害档位（比常规咬击高，路径固定所以躲得掉）。沿用旧值 :677</summary>
        public static int P2SlitDamage() => Helper.ScaleValueForDiffMode(45, 30, 25, 20);
        /// <summary>二阶段噩梦光 / 裂缝咬击的伤害档位。沿用旧值 :601,675</summary>
        public static int P2SparkleDamage() => Helper.ScaleValueForDiffMode(20, 10, 5, 5);
        /// <summary>冲刺贯穿 20 帧后刹车，再 20 帧收手；后摇绕圈半径 340、56 帧，第 20 帧引爆裂缝。沿用旧值 :683-712</summary>
        public const int P2SlitDashFrames = 20;
        public const int P2SlitBrakeFrames = 20;
        public const float P2SlitBrakeDamp = 0.9f;
        public const float P2SlitCircleDistance = 340f;
        public const float P2SlitCircleSpeed = 16f;
        public const float P2SlitCircleAccel = 0.25f;
        public const float P2SlitCircleRolling = 180f;
        public const int P2SlitExplodeFrame = 20;
        public const int P2SlitRecoverFrames = (9 * 4) + 20;
        /// <summary>下方闪光咬：瞬移到玩家下方 350，自旋时在下方 460 处左右摆 620。沿用旧值 :733,764</summary>
        public const float P2BelowTeleportDown = 350f;
        public const float P2BelowSwayWidth = 620f;
        public const float P2BelowSwayHeight = 460f;
        public const float P2BelowSwayFrequency = 24f;
        public const float P2BelowFollowTurn = 0.52f;
        public const float P2BelowFollowSpeedRange = 200f;
        public const float P2BelowFollowMaxSpeed = 66f;
        public const float P2BelowFollowBlend = 0.85f;
        public const float P2BelowHoldDamp = 0.95f;
        public const int P2BelowUpSparkleInterval = 20;
        /// <summary>下方闪光咬的落点：玩家侧方 800~1000、下方 200，斜向上 48 速度冲。沿用旧值 :847-849</summary>
        public const int P2BelowBiteSideMin = 800;
        public const int P2BelowBiteSideMax = 1000;
        public const float P2BelowBiteHeight = 200f;
        public const float P2BelowBiteAngleRight = -0.785f * 3;
        public const float P2BelowBiteAngleLeft = -0.785f;

        //==================== 尖刺球 spikeBalls ====================

        /// <summary>瞬移到玩家头顶 300~400。沿用旧值 Phase.P2_Dream.cs:918</summary>
        public const float P2SpikeBallTeleportMin = 300f;
        public const float P2SpikeBallTeleportMax = 400f;
        /// <summary>绕圈半径 540、限速 16、360 帧一圈。沿用旧值 :930</summary>
        public const float P2SpikeBallCircleDistance = 540f;
        public const float P2SpikeBallCircleSpeed = 16f;
        public const float P2SpikeBallCircleAccel = 0.25f;
        public const float P2SpikeBallCircleRolling = 360f;
        /// <summary>每 20 帧一颗黑洞球（延迟随机 0~360 帧、半径 200），每 30 帧一发噩梦光。沿用旧值 :932-946</summary>
        public const int P2SpikeBallHoleInterval = 20;
        public const float P2SpikeBallHoleJitter = 0.3f;
        public const int P2SpikeBallHoleDelayMax = 360;
        public const float P2SpikeBallHoleRadius = 200f;
        public const int P2SpikeBallSparkleInterval = 30;
        /// <summary>投球段 110 帧，收招段 280 帧。沿用旧值 :948,969</summary>
        public const int P2SpikeBallSowFrames = 110;
        public const int P2SpikeBallTailFrames = 280;

        //==================== 蝙蝠与乌鸦 batsAndCrows ====================

        /// <summary>瞬移到玩家前进方向 600~800。沿用旧值 Phase.P2_Dream.cs:992</summary>
        public const int P2BatsTeleportMin = 600;
        public const int P2BatsTeleportMax = 800;
        /// <summary>绕行参数（半径 650、限速 56、550 帧一圈、转向 0.34）。沿用旧值 :1007</summary>
        public const float P2BatsOrbitDistance = 650f;
        public const float P2BatsOrbitSpeed = 56f;
        public const float P2BatsOrbitAccel = 0.7f;
        public const float P2BatsOrbitRolling = 550f;
        public const float P2BatsOrbitAngle = 0.34f;
        /// <summary>就位段 30 帧，撒弹段 320 帧（末 45 帧淡出）。沿用旧值 :1009,1059-1060</summary>
        public const int P2BatsSettleFrames = 30;
        public const int P2BatsOrbitFrames = 320;
        /// <summary>左右夹击蝙蝠：每 10 帧一对，夹角 ±0.6，速度 18。沿用旧值 :1025-1032</summary>
        public const int P2BatsPairInterval = 10;
        public const float P2BatsPairAngle = 0.6f;
        public const float P2BatsPairJitter = 0.04f;
        public const float P2BatsPairSpeed = 18f;
        /// <summary>瞄准蝙蝠：20 帧后每 20 帧一发，散布 ±0.55，速度 10。沿用旧值 :1038-1043</summary>
        public const int P2BatsAimInterval = 20;
        public const float P2BatsAimJitter = 0.55f;
        public const float P2BatsAimSpeed = 10f;
        /// <summary>绕圈乌鸦：40 帧后每 28 帧三只，速度 11，自转 ±0.017。沿用旧值 :1046-1054</summary>
        public const int P2CrowInterval = 28;
        public const int P2CrowStartFrame = 40;
        public const int P2CrowCount = 3;
        public const float P2CrowSpeed = 11f;
        public const float P2CrowSpin = 0.017f;
        /// <summary>落点：玩家侧方 680~820、上下 ±200。沿用旧值 :1100-1101</summary>
        public const int P2BatsLandSideMin = 680;
        public const int P2BatsLandSideMax = 820;
        public const int P2BatsLandHeight = 200;
        /// <summary>落点爆一圈 7 只蝙蝠，速度 10、自转 0.015。沿用旧值 :1115-1118</summary>
        public const int P2BatsRingCount = 7;
        public const float P2BatsRingSpeed = 10f;
        public const float P2BatsRingSpin = 0.015f;
        /// <summary>收束自旋段 120 帧：转速从 0.35 衰减到 0，绕行半径 640。沿用旧值 :1128-1134</summary>
        public const float P2BatsSpinFrames = 120f;
        public const float P2BatsSpinOrbitDistance = 640f;
        public const float P2BatsSpinOrbitSpeed = 40f;
        public const float P2BatsSpinOrbitAccel = 0.4f;
        public const float P2BatsSpinOrbitRolling = 720f;
        public const float P2BatsSpinOrbitAngle = 0.14f;
        public const float P2BatsSpinTentacleTurns = 3f;
        /// <summary>自旋段每 30 帧一圈蝙蝠（自转方向每 60 帧翻一次），20 帧后每 55 帧一圈乌鸦（速度 12、自转 0.0175）。沿用旧值 :1155-1176</summary>
        public const int P2BatsSpinRingInterval = 30;
        public const int P2BatsSpinRingFlipPeriod = 60;
        public const int P2BatsSpinCrowInterval = 55;
        public const float P2BatsSpinCrowSpeed = 12f;
        public const float P2BatsSpinCrowSpin = 0.0175f;
        /// <summary>收招：贴身绕小圈 40 帧。沿用旧值 :1190-1194</summary>
        public const float P2BatsRecoverDistance = 100f;
        public const float P2BatsRecoverSpeed = 30f;
        public const int P2BatsRecoverFrames = 40;

        //==================== 爪击 hookSlash ====================

        /// <summary>瞬移到玩家右侧 500~600（旧式 <c>(SonState-1)*Pi</c> 在 SonState=0 时恒为右侧）。沿用旧值 :1217</summary>
        public const int P2HookTeleportMin = 500;
        public const int P2HookTeleportMax = 600;
        /// <summary>左右侧选择与爪击轮数 4~6（含）。沿用旧值 :1220-1222</summary>
        public const int P2HookSideMin = 1;
        public const int P2HookSideMaxExclusive = 3;
        public const int P2HookRoundsMin = 4;
        public const int P2HookRoundsMaxExclusive = 7;
        /// <summary>每 30 帧一爪，左右交替 PiOver4，落点抖动 ±0.25，刀身偏移 64、存活 160。沿用旧值 :1229-1235</summary>
        public const int P2HookInterval = 30;
        public const float P2HookJitter = 0.25f;
        public const float P2HookOffset = 64f;
        public const float P2HookAi2 = 160f;
        public const int P2HookTail = 60;
        public static int P2HookDamage() => Helper.ScaleValueForDiffMode(40, 30, 25, 25);
        /// <summary>蓄力冲刺：20 帧淡出 + 20 帧淡回，第 40 帧起冲，冲刺加速 20、限速 50。沿用旧值 :1248-1279</summary>
        public const int P2HookWarpHalfFrames = 20;
        public const float P2HookWarpAlphaStep = 0.5f / 20f;
        public const float P2HookWarpScaleStep = 1.5f / 20f;
        public const float P2HookDriftSpeed = 12f;
        public const float P2HookDriftAccel = 0.15f;
        public const float P2HookDashAccel = 20f;
        public const float P2HookDashMaxSpeed = 50f;
        public const int P2HookDashFrames = 80;
        public const float P2HookDashBreakDistance = 1400f;
        /// <summary>收招：阻尼 0.96，第 14 帧炸一圈 7 发噩梦光，90 帧结束。沿用旧值 :1291-1301</summary>
        public const float P2HookRecoverDamp = 0.96f;
        public const int P2HookBurstFrame = 14;
        public const int P2HookBurstCount = 7;
        public const int P2HookRecoverFrames = 90;

        //==================== 尖刺与闪光 spikesAndSparkles ====================

        /// <summary>瞬移到玩家一侧 700~800，随后在对侧铺开一排 8 个尖刺洞。沿用旧值 Phase.P2_Dream.cs:1314-1329</summary>
        public const int P2SpikesTeleportMin = 700;
        public const int P2SpikesTeleportMax = 800;
        public const int P2SpikesOpeningHoles = 8;
        public const float P2SpikesOpeningSpread = 0.4f;
        public const int P2SpikesOpeningDistMin = 500;
        public const int P2SpikesOpeningDistMax = 800;
        public const float P2SpikesOpeningTime = 120f;
        public const int P2SpikesOpeningLenMin = 800;
        public const int P2SpikesOpeningLenMax = 1200;
        public const float P2SpikesOpeningLead = 15f;
        /// <summary>本体绕半径 850 的半圆轨道缓摆。沿用旧值 :1339-1349</summary>
        public const float P2SpikesOrbitRadius = 850f;
        public const float P2SpikesOrbitWave = 0.0314f;
        public const float P2SpikesOrbitTurn = 0.08f;
        public const float P2SpikesOrbitSpeedRange = 400f;
        public const float P2SpikesOrbitMaxSpeed = 30f;
        public const float P2SpikesOrbitBlend = 0.25f;
        /// <summary>每 12 帧两个尖刺洞（一个沿头朝向、一个上下交替），每 28 帧一发噩梦光，本体段 300 帧。沿用旧值 :1351-1373</summary>
        public const int P2SpikesHoleInterval = 12;
        public const float P2SpikesHoleJitter = 0.4f;
        public const float P2SpikesHoleFutureScale = 3f;
        public const int P2SpikesHoleTimeMin = 60;
        public const int P2SpikesHoleTimeMax = 80;
        public const int P2SpikesHoleLenMin = 600;
        public const int P2SpikesHoleLenMax = 900;
        public const int P2SpikesHole2Period = 24;
        public const float P2SpikesHole2Spread = 0.3f;
        public const int P2SpikesHole2DistMin = 500;
        public const int P2SpikesHole2DistMax = 700;
        public const float P2SpikesHole2Jitter = 0.2f;
        public const float P2SpikesHole2Time = 75f;
        public const int P2SpikesSparkleInterval = 28;
        public const int P2SpikesBodyFrames = 300;
        /// <summary>收招沿蝙蝠那套大圈轨道 30 帧。沿用旧值 :1384-1387</summary>
        public const int P2SpikesRecoverFrames = 30;
        /// <summary>起手时多抽一次 3~4（旧代码写进 ShootCount 但没用上；抽取要保留，否则随机序列分叉）。沿用旧值 :1319</summary>
        public const int P2SpikesRollMin = 3;
        public const int P2SpikesRollMaxExclusive = 5;

        //==================== 尖刺地狱 spikeHell ====================

        /// <summary>第一段：绕大圈的同时每 30 帧在玩家周围半径 450 处放一组 5 个尖刺洞，共 7 组。沿用旧值 :1428-1442</summary>
        public const int P2HellRingInterval = 30;
        public const int P2HellRingStartFrame = 10;
        public const int P2HellRingCount = 7;
        public const int P2HellRingHoles = 5;
        public const float P2HellRingRadius = 450f;
        public const float P2HellRingStep = MathHelper.TwoPi / 15f;
        public const float P2HellRingHoleTime = 60f;
        public const float P2HellRingHoleLen = 440f;
        public const int P2HellRingTail = 55;
        public static int P2HellRingDamage() => Helper.ScaleValueForDiffMode(35, 20, 5, 5);
        /// <summary>第二段：三轮绕圈，每轮 120 帧转一整圈（半径 800）后淡出 30 帧换到对侧重来。沿用旧值 :1449-1540</summary>
        public const int P2HellRollingFrames = 120;
        public const float P2HellRollingRadius = 800f;
        public const float P2HellRollingTurn = 0.08f;
        public const float P2HellRollingSpeedRange = 300f;
        public const float P2HellRollingMaxSpeed = 56f;
        public const float P2HellRollingBlend = 0.85f;
        public const int P2HellRollingHoleInterval = 5;
        public const float P2HellRollingHoleTime = 45f;
        public const float P2HellRollingHoleLen = 1300f;
        public const float P2HellFadeFrames = 30f;
        public const float P2HellFadeDamp = 0.93f;
        public const int P2HellTeleportMin = 700;
        public const int P2HellTeleportMax = 800;
        /// <summary>绕圈轮数：旧代码用 SonState 3/4/5 三格表达。沿用旧值 :1459-1461</summary>
        public const int P2HellRounds = 3;

        //==================== 鬼手冲刺 ghostDash ====================

        /// <summary>瞬移到玩家前进方向 750~900。沿用旧值 Phase.P2_Dream.cs:1565-1568</summary>
        public const float P2GhostTeleportMin = 750f;
        public const float P2GhostTeleportMax = 900f;
        /// <summary>锯齿冲刺：8 段，每段 17 帧（第 8 帧起刹车），转折角 ±0.45，首段 +PiOver4，速度 36。沿用旧值 :1572-1618</summary>
        public const int P2GhostDashCount = 8;
        public const int P2GhostDashBrakeFrame = 8;
        public const int P2GhostDashFrames = 17;
        public const float P2GhostDashDamp = 0.94f;
        public const float P2GhostDashSpeed = 36f;
        public const float P2GhostTurnAngle = 0.45f;
        /// <summary>收招：阻尼 0.93，第 30 帧把鬼影裂缝整条炸成鬼手，绕圈 110 帧。沿用旧值 :1624-1637</summary>
        public const float P2GhostRecoverDamp = 0.93f;
        public const int P2GhostExplodeFrame = 30;
        public const float P2GhostCircleDistance = 540f;
        public const float P2GhostCircleSpeed = 16f;
        public const float P2GhostCircleAccel = 0.25f;
        public const float P2GhostCircleRolling = 360f;
        public const int P2GhostRecoverFrames = 110;

        //==================== 瞬移闪光 teleportSparkle ====================

        /// <summary>贴身瞬移 150~200（淡出只给 30 帧），远瞬移 400~600。沿用旧值 Phase.P2_Dream.cs:1657,1711</summary>
        public const float P2TpNearMin = 150f;
        public const float P2TpNearMax = 200f;
        public const float P2TpFarMin = 400f;
        public const float P2TpFarMax = 600f;
        public const int P2TpFadeFrames = 30;
        /// <summary>吐光段：绕半径 650 的锚点，15 帧后每 15 帧一扇，扇宽随时间涨，65 帧结束。沿用旧值 :1676-1701</summary>
        public const float P2TpVolleyRadius = 650f;
        public const float P2TpVolleyTurn = 0.08f;
        public const float P2TpVolleySpeedRange = 400f;
        public const float P2TpVolleyMaxSpeed = 30f;
        public const float P2TpVolleyBlend = 0.25f;
        public const int P2TpVolleyInterval = 15;
        public const float P2TpVolleyGrowth = 3f;
        public const float P2TpVolleyGrowthOffset = 25f;
        public const float P2TpVolleySpread = 0.25f;
        public const int P2TpVolleyFrames = 65;
        /// <summary>自旋段：240 帧内绕玩家转两整圈（半径 700），自身每帧转 0.55，每 6 帧钉一根长刺。沿用旧值 :1727-1753</summary>
        public const int P2TpSpinFrames = 240;
        public const float P2TpSpinSelfStep = 0.55f;
        public const float P2TpSpinRadius = 700f;
        public const float P2TpSpinTurns = 2f;
        public const float P2TpSpinTurn = 0.35f;
        public const float P2TpSpinSpeedRange = 300f;
        public const float P2TpSpinMaxSpeed = 56f;
        public const float P2TpSpinBlend = 0.85f;
        public const int P2TpSpinHoleInterval = 6;
        public const float P2TpSpinHoleWave = MathHelper.TwoPi / 12f;
        public const float P2TpSpinHoleSwing = 1.6f;
        public const float P2TpSpinHoleTime = 30f;
        public const float P2TpSpinHoleLen = 1300f;
        /// <summary>收招：25 帧内从 150 拉到 450 并顺时针扫 PiOver4。沿用旧值 :1759-1771</summary>
        public const float P2TpRecoverFrames = 25f;
        public const float P2TpRecoverNear = 150f;
        public const float P2TpRecoverFar = 450f;
        public const float P2TpRecoverTurn = 0.3f;

        //==================== 梦境之光 dreamSparkle ====================

        /// <summary>绕玩家的公转半径 500、400 帧一圈。沿用旧值 Phase.P2_Dream.cs:1808-1819</summary>
        public const float P2DreamOrbitRadius = 500f;
        public const float P2DreamOrbitPeriod = 400f;
        public const float P2DreamOrbitTurn = 0.08f;
        public const float P2DreamOrbitSpeedRange = 450f;
        public const float P2DreamOrbitMaxSpeed = 34f;
        public const float P2DreamOrbitBlend = 0.85f;
        /// <summary>两波噩梦光环（第 10、160 帧），每波 14 发、半径 150、逐发延迟 6 帧、飞行 550。沿用旧值 :1822-1833</summary>
        public const int P2DreamRingFrameA = 10;
        public const int P2DreamRingFrameB = 160;
        public const int P2DreamRingCount = 14;
        public const float P2DreamRingRadius = 150f;
        public const int P2DreamRingStepDelay = 6;
        public const float P2DreamRingRange = 550f;
        public const int P2DreamRingRollMin = 60;
        public const int P2DreamRingRollMaxExclusive = 80;
        public const int P2DreamBodyFrames = 210;
        /// <summary>第二段：第 80 帧的那一波里随机有一发会变成美梦光（玩家要打下它），延迟拉长到 150~180。沿用旧值 :1856-1874</summary>
        public const int P2DreamExchangeFrame = 80;
        public const int P2DreamExchangeRollMin = 150;
        public const int P2DreamExchangeRollMaxExclusive = 180;
        /// <summary>第二段每 20 帧在玩家四周 600 处放一圈尖刺洞，140 帧后停；整段 200 帧。沿用旧值 :1879-1893</summary>
        public const int P2DreamHoleInterval = 20;
        public const int P2DreamHoleStopFrame = 140;
        public const int P2DreamHoleCount = 4;
        public const float P2DreamHoleDistance = 600f;
        public const float P2DreamHoleDrift = 0.1f;
        public const float P2DreamHoleDriftPeriod = 28f;
        public const float P2DreamHoleTime = 45f;
        public const float P2DreamHoleLen = 700f;
        public const int P2DreamTailFrames = 200;

        //==================== 二阶段待机 p2_Idle ====================

        /// <summary>待机时长由选招口写进 <c>ShootCount</c>。沿用旧值 Phase.P2_Dream.cs:2454</summary>
        public const float P2IdleFrames = 300f;
        /// <summary>待机时半透明到 0.75，头随机小幅摆动。沿用旧值 :1910-1926</summary>
        public const float P2IdleAlphaFloor = 0.75f;
        public const float P2IdleAlphaStep = 0.02f;
        public const float P2IdleWobbleMin = -0.2f;
        public const float P2IdleWobbleMax = 0.3f;

        //==================== 梦境战斗（useDreamMove）公共 ====================

        /// <summary>盯梢段：半透明地绕着美梦光转（半径 350），时长由选招口写进 <c>ShootCount</c>。沿用旧值 :1942-1963</summary>
        public const float P2DreamingStalkRadius = 350f;
        public const float P2DreamingAlphaBase = 0.6f;
        public const float P2DreamingAlphaWave = 0.35f;
        public const float P2DreamingAlphaSpeed = 2f;
        /// <summary>梦境里的扑击更急：距离阈值更近、加速 0.85、刹车 0.78、头转 0.5。沿用旧值 :2006-2019</summary>
        public const float P2DreamingLungeAccel = 0.85f;
        public const float P2DreamingLungeMaxSpeed = 26f;
        public const float P2DreamingLungeDamp = 0.78f;
        public const float P2DreamingRotateStep = 0.5f;
        /// <summary>盯梢时长的默认值，选招口按轮次写进 <c>ShootCount</c>。沿用旧值 :2502,2519</summary>
        public const float P2DreamingStalkFrames = 60f;
        /// <summary>每一拍新放的美梦光落点：玩家左右任一侧 500、上方 300。沿用旧值 :2503,2512,2520</summary>
        public const float P2DreamingSparkleSide = 500f;
        public const float P2DreamingSparkleHeight = -300f;
        /// <summary>被 boss 杀掉的美梦光攒到这个数就结束梦境战斗（惩罚段落幕）。沿用旧值 :2486,2610</summary>
        public const int P2DreamingKillCap = 7;

        //==================== 梦境·噩梦之咬 dreamingNightmareBite ====================

        /// <summary>梦境里的咬击伤害远高于常规（这是"你没打掉光"的惩罚）。沿用旧值 :1984</summary>
        public static int P2DreamingBiteDamage() => Helper.ScaleValueForDiffMode(80, 70, 60, 60);
        public const float P2DreamingBiteAi1 = 60f;
        public const float P2DreamingBiteDistance = 200f;
        public const int P2DreamingBiteLungeFrames = 55;
        public const float P2DreamingBiteRecoverDamp = 0.9f;
        public const int P2DreamingBiteSoundFrame = 7;
        public const int P2DreamingBiteRotFrame = 10;
        public const int P2DreamingBiteRecoverFrames = 20;

        //==================== 梦境·待机 dreaming_Idle ====================

        /// <summary>幻想之神在场时的等待上限（60 秒），到点无论如何回常规二阶段。沿用旧值 :2063</summary>
        public const int P2DreamingIdleFrames = 3600;
        public const float P2DreamingIdleDamp = 0.8f;

        //==================== 梦境·尖刺地狱 dreamingSpikeHell ====================

        public const int P2DreamingHellFadeFrames = 30;
        public const float P2DreamingHellChaseDistance = 160f;
        /// <summary>追到 75 帧后每 20 帧朝美梦光钉一根刺，整段 500 帧。沿用旧值 :2128-2140</summary>
        public const int P2DreamingHellShootStart = 75;
        public const int P2DreamingHellShootInterval = 20;
        public const int P2DreamingHellChaseFrames = 500;
        public static int P2DreamingHellHoleDamage() => Helper.ScaleValueForDiffMode(20, 10, 15, 15);
        public const float P2DreamingHellHoleTime = 30f;
        public const float P2DreamingHellHoleLen = 1300f;
        /// <summary>收招：自转 0.2，每 7 帧朝一个匀速旋进的方向钉一根刺（49 帧一整圈），50 帧结束。沿用旧值 :2146-2155</summary>
        public const float P2DreamingHellSpinStep = 0.2f;
        public const int P2DreamingHellBurstInterval = 7;
        public const float P2DreamingHellBurstPeriod = 49f;
        public const int P2DreamingHellRecoverFrames = 50;
        public static int P2DreamingHellBurstDamage() => Helper.ScaleValueForDiffMode(50, 60, 60, 60);

        //==================== 梦境·美梦猎杀 dreamingFantasyHunting ====================

        /// <summary>射击段保持 360 的距离，每 25 帧打 1 或 3 发红光（夹角 0.15）。沿用旧值 :2209-2232</summary>
        public const float P2DreamingHuntDistance = 360f;
        public const int P2DreamingHuntInterval = 25;
        public const float P2DreamingHuntSpread = 0.15f;
        public const int P2DreamingHuntFrames = 120;
        public static int P2DreamingHuntDamage() => Helper.ScaleValueForDiffMode(50, 50, 25, 25);
    }
}
