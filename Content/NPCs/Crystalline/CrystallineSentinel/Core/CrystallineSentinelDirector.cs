using Coralite.Helpers;

namespace Coralite.Content.NPCs.Crystalline.Core
{
    /// <summary>
    /// 结晶战斗体调参中心：全部数字与阶段档位的唯一出口。<br/>
    /// 本轮是结构迁移，战斗设计不变：每条常量注明来源，查不到设计理由的一律写“沿用旧值 CrystallineSentinel.cs:行号”（迁移前的行号）。
    /// </summary>
    internal static class CrystallineSentinelDirector
    {
        //==================== 感知 / 索敌 / 脱战 ====================

        /// <summary>警戒范围 40 格：进入后转向玩家并弹出警戒粒子。沿用旧值 CrystallineSentinel.cs:137</summary>
        public const int AlertRange = 16 * 40;
        /// <summary>近战触发范围 16 格：玩家进到这里直接把仇恨拉满。沿用旧值 CrystallineSentinel.cs:138</summary>
        public const int MeleeRange = 16 * 16;
        /// <summary>解除警戒的额外余量（AlertRange 的基础上再加 4 格），避免在边界上反复进出警戒。沿用旧值 CrystallineSentinel.cs:640</summary>
        public const float AlertClearMargin = MeleeRange * 0.25f;
        /// <summary>仇恨计数下限 −5 秒：到底后重新索敌。沿用旧值 CrystallineSentinel.cs:134</summary>
        public const int AggroCounterMin = -5 * 60;
        /// <summary>仇恨计数上限 5 秒：受击或玩家贴近时拉满。沿用旧值 CrystallineSentinel.cs:135</summary>
        public const int AggroCounterMax = 5 * 60;
        /// <summary>可攻击距离上限 1000 px（还要求不隐身且视线可达）。沿用旧值 CrystallineSentinel.cs:2001</summary>
        public const float CanHitDistance = 1000f;
        /// <summary>头部发光亮度（每帧 AddLight）。沿用旧值 CrystallineSentinel.cs:608</summary>
        public const float HeadLight = 0.5f;
        /// <summary>发现玩家的颜文字 120 帧、失去目标 180 帧。沿用旧值 CrystallineSentinel.cs:628,643</summary>
        public const int AlertTextFrames = 120;
        public const int ConfusionTextFrames = 180;
        /// <summary>出手 / 暴怒类颜文字 120 帧。沿用旧值 CrystallineSentinel.cs:1080,1405,1597</summary>
        public const int FireTextFrames = 120;
        /// <summary>受击类颜文字（挡住 / 被自己的飞弹打到）90 帧。沿用旧值 CrystallineSentinel.cs:396,437</summary>
        public const int HitTextFrames = 90;

        /// <summary>二阶段脱战距离 3000 px。沿用旧值 CrystallineSentinel.cs:1213</summary>
        public const float DespawnDistance = 3000f;
        /// <summary>脱战：朝向回正、X 衰减、向上加速逃离，并鼓励消失。沿用旧值 CrystallineSentinel.cs:1221-1227</summary>
        public const float DespawnRotationStep = 0.14f;
        public const float DespawnDampX = 0.98f;
        public const float DespawnAccelY = 0.4f;
        public const float DespawnExtraAccelY = 0.2f;
        public const float DespawnLimitY = -16f;
        public const int DespawnEncourageFrames = 30;

        //==================== 阶段 / 冷却 / 轮换记账 ====================

        /// <summary>碎岩攻击血线：低于 75% 时下一次飞弹替换为碎岩（一次性）。沿用旧值 CrystallineSentinel.cs:151</summary>
        public const float RockReleaseThreshold = 0.75f;
        /// <summary>二阶段血线：低于 50% 转阶段。沿用旧值 CrystallineSentinel.cs:155</summary>
        public const float Phase2Threshold = 0.5f;
        /// <summary>开盾冷却 10 秒。沿用旧值 CrystallineSentinel.cs:124</summary>
        public const float GuardCooldownMax = 10 * 60;
        /// <summary>飞弹冷却 10 秒。沿用旧值 CrystallineSentinel.cs:127</summary>
        public const float MissileCooldownMax = 10 * 60;
        /// <summary>开盾计数门槛：远距离受伤累计超过这个值才尝试开盾。沿用旧值 CrystallineSentinel.cs:1953</summary>
        public const float GuardCounterThreshold = 300f;
        /// <summary>只有目标在 500 px 之外的弹幕伤害才累计进开盾计数（贴身打不出盾）。沿用旧值 CrystallineSentinel.cs:378</summary>
        public const float GuardCounterRange = 500f;
        /// <summary>二阶段每 6 次攻击后强制休息一轮。沿用旧值 CrystallineSentinel.cs:1918</summary>
        public const int RestAttackCount = 6;
        /// <summary>二阶段选招距离门槛：超过 500 px 用螺旋冲刺拉近。沿用旧值 CrystallineSentinel.cs:2025</summary>
        public const float P2RollingPickDistance = 500f;
        /// <summary>查重窗口长度（记账用）：15 个状态取 1/3 ≈ 5。</summary>
        public const int RecentPickWindow = 5;
        /// <summary>
        /// 硬锁一（不与上一手相同）。旧代码的防复读是“复读两次后强制换另一招”（<c>AttackRepeater</c>），
        /// 允许连续两次同招，所以这里关闭硬锁、保留旧语义；hub 只做提交口与记账。沿用旧值 CrystallineSentinel.cs:2029-2033
        /// </summary>
        public const bool ForbidImmediateRepeat = false;
        /// <summary>
        /// hub 停留帧数。旧代码收招当帧即切下一状态、零间隔（喘息由 P1Idle / P2Idle 的入场预充时间承担），
        /// 所以为 0：hub 只在注册表建不出状态时驻留一帧兜底。
        /// </summary>
        public const int HubFrames = 0;
        /// <summary>状态超时兜底帧数：最长的招式（旋风斩 ≈ 212 帧、二阶段休息 ≈ 240 帧）之外留足余量。</summary>
        public const int StateTimeoutFrames = 60 * 20;
        /// <summary>演出态（转阶段 / 死亡）的超时兜底：转阶段含 72 帧定格，总长约 200 帧。</summary>
        public const int AnimStateTimeoutFrames = 60 * 30;

        //==================== 一阶段：站立 / 闲逛 ====================

        /// <summary>站立与闲逛期间每 45 帧做一次决策（先试开盾再试攻击）。沿用旧值 CrystallineSentinel.cs:661,719</summary>
        public const int P1DecisionInterval = 45;
        /// <summary>站立结束后随便走走的时长 2~4 秒。沿用旧值 CrystallineSentinel.cs:669</summary>
        public const int P1WalkFramesMin = 60 * 2;
        public const int P1WalkFramesMax = 60 * 4;
        /// <summary>闲逛结束后站立 4~6 秒。沿用旧值 CrystallineSentinel.cs:710</summary>
        public const int P1IdleFramesMin = 60 * 4;
        public const int P1IdleFramesMax = 60 * 6;
        /// <summary>脱战站立时每次回血 10% 上限血量（并重建浮石）。沿用旧值 CrystallineSentinel.cs:672</summary>
        public const float P1IdleHealRatio = 0.1f;
        /// <summary>闲逛速度上限与加速度。沿用旧值 CrystallineSentinel.cs:716-717</summary>
        public const float P1WalkMaxSpeed = 1.2f;
        public const float P1WalkAccel = 0.1f;
        /// <summary>走路帧速 = 8 − |vx|（走得越快切帧越快），帧列 1，末帧 10。沿用旧值 CrystallineSentinel.cs:731-743</summary>
        public const float WalkFrameRateBase = 8f;
        public const int WalkFrameColumn = 1;
        public const int WalkFrameMaxY = 10;
        /// <summary>前方可行走检测：脚下 3 行内有实心块的列数不少于总列数 − 1。沿用旧值 CrystallineSentinel.cs:746-777</summary>
        public const int WalkCheckRows = 3;
        public const int WalkCheckYOffset = 4;

        //==================== 一阶段：刺击 ====================

        /// <summary>一阶段各接近段的转向间隔 60 帧（转向不跟手 = 给玩家绕背的空间）。沿用旧值 CrystallineSentinel.cs:793,894,1064,1090</summary>
        public const int TurnInterval = 60;
        /// <summary>刺击接近段：速度上限 4.5、加速度 0.07。沿用旧值 CrystallineSentinel.cs:800-801</summary>
        public const float SpurtApproachMaxSpeed = 4.5f;
        public const float SpurtApproachAccel = 0.07f;
        /// <summary>接近段加速收敛：24 格内计时 +2、10 格内再 +5（越近越快进入起手）。沿用旧值 CrystallineSentinel.cs:807-812</summary>
        public const float SpurtCloseRange = 16 * 24;
        public const float SpurtVeryCloseRange = 16 * 10;
        public const int SpurtCloseTimerBonus = 2;
        public const int SpurtVeryCloseTimerBonus = 5;
        /// <summary>接近段最长 8 秒（或走到悬崖）就结算一次。沿用旧值 CrystallineSentinel.cs:814</summary>
        public const int SpurtApproachTimeout = 60 * 8;
        /// <summary>起手要求玩家在 16 格内，否则退回站立 1 秒。沿用旧值 CrystallineSentinel.cs:816,829</summary>
        public const float SpurtEnterRange = 16 * 16;
        public const int SpurtFailIdleFrames = 60;
        /// <summary>戳刺段帧速 5、末帧 20。沿用旧值 CrystallineSentinel.cs:835-839</summary>
        public const int SpurtFrameRate = 5;
        public const int SpurtFrameMaxY = 20;
        /// <summary>出手帧 = 30（= 5 × 6），提前 10 帧放起手音（预告窗）。沿用旧值 CrystallineSentinel.cs:842-846</summary>
        public const int SpurtDashFrame = 5 * 6;
        public const int SpurtSoundLead = 10;
        /// <summary>出手瞬间的水平速度。沿用旧值 CrystallineSentinel.cs:852</summary>
        public const float SpurtDashSpeed = 10f;
        /// <summary>刺击弹幕出生点在身前 40 px。沿用旧值 CrystallineSentinel.cs:856</summary>
        public const float SpurtProjOffsetX = 40f;
        /// <summary>刺击伤害 200/350/480/1000。沿用旧值 CrystallineSentinel.cs:857</summary>
        public static int SpurtProjDamage() => Helper.ScaleValueForDiffMode(200, 350, 480, 1000);
        /// <summary>出手后每帧衰减 0.9；前方 4 格内探到悬崖就直接刹死（不冲下坑）。沿用旧值 CrystallineSentinel.cs:863-875</summary>
        public const float SpurtDampAfterDash = 0.9f;
        public const int SpurtCliffCheckCount = 4;
        public const int SpurtCliffSearchLength = 2;
        /// <summary>戳刺段总长 105 帧（= 5 × 21）。沿用旧值 CrystallineSentinel.cs:878</summary>
        public const int SpurtTotalFrames = 5 * 21;

        //==================== 一阶段：护盾 ====================

        /// <summary>护盾接近段：速度上限 1.3、加速度 0.1、最长 3 秒。沿用旧值 CrystallineSentinel.cs:901-906</summary>
        public const float GuardApproachMaxSpeed = 1.3f;
        public const float GuardApproachAccel = 0.1f;
        public const int GuardApproachTimeout = 60 * 3;
        /// <summary>接近失败退回站立 1 秒。沿用旧值 CrystallineSentinel.cs:921</summary>
        public const int GuardFailIdleFrames = 60;
        /// <summary>展开段帧速 4、末帧 8；24 帧后护盾视觉开始张开，36 帧后进入防御。沿用旧值 CrystallineSentinel.cs:928-947</summary>
        public const int GuardDeployFrameRate = 4;
        public const int GuardDeployFrameMaxY = 8;
        public const int GuardDeployShieldStart = 4 * 6;
        public const int GuardDeployFrames = 4 * 9;
        /// <summary>护盾视觉张开 / 收起速度。沿用旧值 CrystallineSentinel.cs:936,1045</summary>
        public const float GuardFactorGrow = 0.1f;
        public const float GuardFactorShrink = 0.08f;
        /// <summary>防御段帧速 5、末帧 15。沿用旧值 CrystallineSentinel.cs:954,978-982</summary>
        public const int GuardFrameRate = 5;
        public const int GuardFrameMaxY = 15;
        /// <summary>
        /// 防御中远程反击的最小间隔 210 帧。旧代码用 <c>Main.GameUpdateCount % 210 == 0</c> 开闸，
        /// 两端的模拟不能挂在全局帧号上（C3），改为状态自带的冷却计数，节奏一致。沿用旧值 CrystallineSentinel.cs:958
        /// </summary>
        public const int GuardVolleyInterval = 210;
        /// <summary>反击时把计时拉到 −60（= −12 × 5）播放发射动作。沿用旧值 CrystallineSentinel.cs:960</summary>
        public const int GuardVolleyLead = -12 * GuardFrameRate;
        /// <summary>反击动作中三发飞弹的出膛拍：−40 / −30 / −20。沿用旧值 CrystallineSentinel.cs:986</summary>
        public const int GuardVolleyFirstCue = -GuardFrameRate * 8;
        public const int GuardVolleyCueStep = 10;
        /// <summary>玩家持续远离时每 60 帧把计时回拨 60（护盾可无限延长），下限 1、上限 3 秒。沿用旧值 CrystallineSentinel.cs:964-966</summary>
        public const int GuardExtendInterval = 60;
        public const int GuardExtendMin = 1;
        public const int GuardExtendMax = 3 * 60;
        /// <summary>防御总时长 3 秒；玩家彻底失去仇恨时直接跳到收招。沿用旧值 CrystallineSentinel.cs:975,1012</summary>
        public const int GuardHoldFrames = 3 * 60;
        /// <summary>防御 60 帧后玩家进近战范围就主动撤盾接刺击（刺击预充 9 秒接近时间）。沿用旧值 CrystallineSentinel.cs:1023-1031</summary>
        public const int GuardBreakEarliest = 60;
        public const int GuardBreakSpurtFrames = 60 * 9;
        /// <summary>收招段帧速 5、末帧 4，总长 25 帧后回站立 1 秒。沿用旧值 CrystallineSentinel.cs:1039-1050</summary>
        public const int GuardRecoverFrameRate = 5;
        public const int GuardRecoverFrameMaxY = 4;
        public const int GuardRecoverFrames = 5 * 5;
        public const int GuardRecoverIdleFrames = 60;

        //==================== 一阶段：飞弹 ====================

        /// <summary>飞弹接近段：速度上限 1.0、加速度 0.1、最长 2 秒。沿用旧值 CrystallineSentinel.cs:1067-1072</summary>
        public const float MissileApproachMaxSpeed = 1f;
        public const float MissileApproachAccel = 0.1f;
        public const int MissileApproachTimeout = 60 * 2;
        /// <summary>发射段帧速 5、末帧 19，总长 100 帧。沿用旧值 CrystallineSentinel.cs:1088-1129</summary>
        public const int MissileFrameRate = 5;
        public const int MissileFrameMaxY = 19;
        public const int MissileFireFrames = 20 * MissileFrameRate;
        /// <summary>三发飞弹的出膛拍：35 / 45 / 55（= 帧速 × 7 起，间隔 10）。沿用旧值 CrystallineSentinel.cs:1104</summary>
        public const int MissileFirstCue = MissileFrameRate * 7;
        public const int MissileCueStep = 10;
        /// <summary>飞弹出膛点（相对本体中心，X 随朝向翻转）与初速 9。沿用旧值 CrystallineSentinel.cs:1106-1107</summary>
        public const float MissileMuzzleX = -5f;
        public const float MissileMuzzleY = -27f;
        public const float MissileLaunchSpeed = 9f;
        /// <summary>三发飞弹的初始散射角：0.3 / −0.2 / −0.7。沿用旧值 CrystallineSentinel.cs:1107</summary>
        public const float MissileSpreadBase = 0.3f;
        public const float MissileSpreadStep = -0.5f;
        /// <summary>收招段 60 帧后回站立 1 秒。沿用旧值 CrystallineSentinel.cs:1142-1144</summary>
        public const int MissileRecoverFrames = 60;
        public const int MissileRecoverIdleFrames = 60;

        //==================== 一阶段：碎岩 ====================

        /// <summary>碎岩帧列 6、帧速 5、末帧 10，总长 50 帧。沿用旧值 CrystallineSentinel.cs:1157-1193</summary>
        public const int RockFrameColumn = 6;
        public const int RockFrameRate = 5;
        public const int RockFrameMaxY = 10;
        public const int RockTotalFrames = 10 * RockFrameRate;
        /// <summary>出手拍 35（= 7 × 5）：三块浮石同时脱离本体。沿用旧值 CrystallineSentinel.cs:1197</summary>
        public const int RockReleaseFrame = 7 * RockFrameRate;
        /// <summary>浮石数量 3、出生半径 30 px、初速 rand(12,18) × 0.4。沿用旧值 CrystallineSentinel.cs:1178-1184</summary>
        public const int RockCount = 3;
        public const float RockSpawnRadius = 30f;
        public const float RockSpeedMin = 12f;
        public const float RockSpeedMax = 18f;
        public const float RockSpeedScale = 0.4f;

        //==================== 转阶段演出 ====================

        /// <summary>转阶段帧速 6、末帧 19。沿用旧值 CrystallineSentinel.cs:1812,1845-1849</summary>
        public const int ExchangeFrameRate = 6;
        public const int ExchangeFrameMaxY = 19;
        /// <summary>碎裂 Gore 在前 16 帧按帧号逐块崩开。沿用旧值 CrystallineSentinel.cs:1823-1824</summary>
        public const int ExchangeGoreFrames = 16;
        /// <summary>Gore 的重组延迟 = 帧速 × 11 ± 14。沿用旧值 CrystallineSentinel.cs:1835</summary>
        public const int ExchangeGoreRebuild = ExchangeFrameRate * 11;
        public const int ExchangeGoreRebuildJitter = 14;
        /// <summary>碎裂段时长：帧图每 6 帧推一格、第 36 帧顶到第 7 帧，再走 5 帧进定格。沿用旧值 CrystallineSentinel.cs:1819,1845</summary>
        public const int ExchangeBreakFrames = 41;
        /// <summary>
        /// 第 7 帧的定格时长 45 帧。旧代码用 <c>Main.GameUpdateCount % 12 != 0</c> 让计时在 39~42 这 4 个值上各卡约 12 帧
        /// （≈ 48 帧，减去本来就要走的 4 帧 ≈ 45）；全局帧号不能参与模拟（C3），这里换成等长的显式定格。沿用旧值 CrystallineSentinel.cs:1819-1822
        /// </summary>
        public const int ExchangeHoldFrame = 7;
        public const int ExchangeHoldFrames = 45;
        /// <summary>
        /// 演出结束转入二阶段悬浮时的预充帧数。旧代码先 <c>SwitchStateP2(P2Idle, 60)</c>、紧接着又 <c>Timer = 20</c> 把它盖掉，
        /// 真正生效的是 20（所以转阶段后第一次攻击决策落在第 35 帧）。沿用旧值 CrystallineSentinel.cs:1871-1873
        /// </summary>
        public const int ExchangeToIdleFrames = 20;
        public const float ExchangeLaunchY = -1f;
        /// <summary>颜文字时长：碎裂 90 帧、暴怒 120 帧。沿用旧值 CrystallineSentinel.cs:1817,1870</summary>
        public const int ExchangeBrokenTextFrames = 90;
        public const int ExchangeAngryTextFrames = 120;

        //==================== 二阶段：通用飞行 ====================

        /// <summary>二阶段身体帧速 3（frameCounter > 3），末帧 7。沿用旧值 CrystallineSentinel.cs:1239-1243</summary>
        public const float P2BodyFrameRate = 3f;
        public const int P2BodyFrameMaxY = 7;
        /// <summary>每 30 帧把速度随机偏转 ±0.9 rad（飞行不走直线）。沿用旧值 CrystallineSentinel.cs:1253-1256</summary>
        public const int P2WanderInterval = 30;
        public const float P2WanderAngle = 0.9f;
        /// <summary>悬停位：玩家背侧、水平距离至少 120 px 的 0.7 倍处、上方 120 px。沿用旧值 CrystallineSentinel.cs:1259-1263</summary>
        public const float P2HoverMinX = 120f;
        public const float P2HoverXFactor = 0.7f;
        public const float P2HoverOffsetY = -120f;
        public const float P2HoverArriveRange = 40f;
        public const float P2HoverTurnAngle = 0.5f;
        public const float P2HoverLerp = 0.02f;
        /// <summary>悬浮态巡航速度 9、限速 7 / 加速 0.1、整体衰减 0.97。沿用旧值 CrystallineSentinel.cs:1263-1273</summary>
        public const float P2IdleHoverSpeed = 9f;
        public const float P2IdleMaxSpeed = 7f;
        public const float P2IdleAccel = 0.1f;
        public const float P2IdleDamp = 0.97f;
        /// <summary>撞墙反弹系数。沿用旧值 CrystallineSentinel.cs:1286,1292</summary>
        public const float P2CollideBounce = -0.4f;
        /// <summary>SpeedUp 的超速衰减与默认模糊带宽。沿用旧值 CrystallineSentinel.cs:1297-1306</summary>
        public const float P2SpeedUpOverDamp = 0.97f;
        public const float P2SpeedUpBlur = 1f;
        /// <summary>悬浮态每 35 帧做一次攻击决策（入场预充期内不决策）。沿用旧值 CrystallineSentinel.cs:1247</summary>
        public const int P2DecisionInterval = 35;
        /// <summary>收招回悬浮的预充值 −60（= 60 帧喘息），休息后为 −120。沿用旧值 CrystallineSentinel.cs:1434,1740</summary>
        public const int P2IdleGapFrames = -60;
        public const int P2IdleGapAfterRest = -120;

        //==================== 二阶段：挥刀 ====================

        /// <summary>挥刀期间的巡航参数（比悬浮慢一档：目标速 3、限速 4 / 加速 0.04）。沿用旧值 CrystallineSentinel.cs:1358-1362</summary>
        public const float SwingHoverSpeed = 3f;
        public const float SwingMaxSpeed = 4f;
        public const float SwingAccel = 0.04f;
        /// <summary>首刀的拉近段 45 帧（= 3 × 15），第二刀只留 4 帧（连招不给喘息）。沿用旧值 CrystallineSentinel.cs:1367,1375</summary>
        public const int SwingApproachFrames = 3 * 15;
        public const float SwingSecondApproachScale = 0.1f;
        /// <summary>抬手段 19 帧（= 3 × 6 + 1）：手部帧每 3 帧推一格，推到 12 就是出手姿态。沿用旧值 CrystallineSentinel.cs:1368,1395-1399</summary>
        public const int SwingReadyFrames = 3 * 6 + 1;
        public const int SwingHandFrameRate = 3;
        /// <summary>出刀后的滞留 90 帧，首刀少 10 帧（首刀更快接第二刀）。沿用旧值 CrystallineSentinel.cs:1369,1372</summary>
        public const int SwingIdleFrames = 90;
        public const int SwingFirstIdleBonus = -10;
        /// <summary>收刀段 21 帧（= 3 × 7），期间速度每帧 0.9 衰减。沿用旧值 CrystallineSentinel.cs:1370,1417</summary>
        public const int SwingDelayFrames = 3 * 7;
        public const float SwingDelayDamp = 0.9f;
        /// <summary>拉近段：超过 350 px 才追，加速度 0.08、限速 12。沿用旧值 CrystallineSentinel.cs:1381-1386</summary>
        public const float SwingChaseRange = 350f;
        public const float SwingChaseAccel = 0.08f;
        public const float SwingChaseMaxSpeed = 12f;
        /// <summary>出手瞬间速度对折。沿用旧值 CrystallineSentinel.cs:1404</summary>
        public const float SwingLaunchDamp = 0.5f;
        /// <summary>刀光伤害 120/140/180。沿用旧值 CrystallineSentinel.cs:1409</summary>
        public static int SwingProjDamage() => Helper.GetProjDamage(120, 140, 180);
        public const float SwingProjKnockback = 1f;
        /// <summary>起手闪光粒子 5 粒（只在首刀放，第二刀无预告=连招压迫）。沿用旧值 CrystallineSentinel.cs:1322-1331</summary>
        public const int SwingTwinkleCount = 5;
        public const float SwingTwinkleSpeedMin = 1f;
        public const float SwingTwinkleSpeedMax = 4f;
        /// <summary>一次挥刀 = 两刀。沿用旧值 CrystallineSentinel.cs:1428-1432</summary>
        public const int SwingAttackCount = 2;

        //==================== 二阶段：螺旋冲刺 ====================

        /// <summary>起手静止段 15 帧、蓄力段到 45 帧（蓄力本身即预告：反向后撤 + 刀刃展开）。沿用旧值 CrystallineSentinel.cs:1448-1449</summary>
        public const int RollingIdleEnd = 15;
        public const int RollingReadyEnd = RollingIdleEnd + 30;
        /// <summary>蓄力段帧速 5、末帧 5；反向后撤目标速 6、插值 0.03。沿用旧值 CrystallineSentinel.cs:1460-1469</summary>
        public const int RollingReadyFrameRate = 5;
        public const int RollingReadyFrameMaxY = 5;
        public const float RollingBackSpeed = 6f;
        public const float RollingBackLerp = 0.03f;
        /// <summary>冲刺速度 18；冲刺帧数 = 距离 ÷ 速度 + 20（够冲过玩家）。沿用旧值 CrystallineSentinel.cs:1473-1476</summary>
        public const float RollingDashSpeed = 18f;
        public const float RollingDashExtraFrames = 20f;
        /// <summary>冲刺段帧速 5、循环区间 5~12；每帧 3 粒刀锋闪光，取样点在身前 96 px。沿用旧值 CrystallineSentinel.cs:1490-1505</summary>
        public const int RollingDashFrameRate = 5;
        public const int RollingDashFrameMaxY = 12;
        public const int RollingDashFrameLoopY = 5;
        public const int RollingFlashCount = 3;
        public const float RollingFlashOffset = 96f;
        public const float RollingFlashSpeed = 5f;
        public const float RollingFlashLerp = 0.25f;
        public const float RollingFlashScale = 0.35f;
        /// <summary>冲刺中每帧朝玩家修正 0.01 rad（几乎锁死，预告指哪打哪）。沿用旧值 CrystallineSentinel.cs:1499</summary>
        public const float RollingHomingLerp = 0.01f;
        /// <summary>撞墙：立刻进收招、反弹 0.8 倍、刀刃定格在第 9 帧。沿用旧值 CrystallineSentinel.cs:1509-1514</summary>
        public const float RollingCollideBounce = -0.8f;
        public const int RollingCollideFrameY = 9;
        /// <summary>收招段 45 帧：速度 0.93 衰减、朝向线性回正，刀刃帧速 4 收到 15 后归零。沿用旧值 CrystallineSentinel.cs:1451,1520-1529</summary>
        public const int RollingRecoverFrames = 45;
        public const float RollingRecoverDamp = 0.93f;
        public const int RollingRecoverFrameRate = 4;
        public const int RollingRecoverFrameMaxY = 15;
        /// <summary>收招后若玩家正落在 220~280 px 环带内，直接接一记短旋风斩（连段）。沿用旧值 CrystallineSentinel.cs:1537-1539</summary>
        public const float RollingChainRangeMin = 220f;
        public const float RollingChainRangeMax = 280f;
        /// <summary>冲刺弹幕伤害 120/140/180。沿用旧值 CrystallineSentinel.cs:1478</summary>
        public static int RollingProjDamage() => Helper.GetProjDamage(120, 140, 180);
        public const float RollingProjKnockback = 1f;

        //==================== 二阶段：旋风斩 ====================

        /// <summary>刀刃外扩半径 220 px，短版对折（逃生道 = 环带外侧）。沿用旧值 CrystallineSentinel.cs:1562-1564</summary>
        public const float WhirlMaxRange = 220f;
        public const float WhirlShortRangeScale = 0.5f;
        /// <summary>旋风斩身体帧速 3。沿用旧值 CrystallineSentinel.cs:1565</summary>
        public const float WhirlFrameRate = 3f;
        /// <summary>巡航：速度按距离 100~600 映射到 1~2，插值 0.02。沿用旧值 CrystallineSentinel.cs:1575-1577</summary>
        public const float WhirlSpeedNearDistance = 100f;
        public const float WhirlSpeedFarDistance = 600f;
        public const float WhirlSpeedNear = 1f;
        public const float WhirlSpeedFar = 2f;
        public const float WhirlSpeedLerp = 0.02f;
        /// <summary>起手段 22 帧（= 3 × 7 + 1）、蓄力段 +80（短版 +10）、挥砍段 +50、收招段 +60。沿用旧值 CrystallineSentinel.cs:1586-1591</summary>
        public const int WhirlReadyFrames = (int)(WhirlFrameRate * 7) + 1;
        public const int WhirlChargeFrames = 80;
        public const int WhirlShortChargeCut = 70;
        public const int WhirlSwingFrames = 50;
        public const int WhirlRecoverFrames = 60;
        /// <summary>预警环：半径 = 刀刃半径 + 120，存活 75 帧（短版对折）——仓库唯一的实体化预告。沿用旧值 CrystallineSentinel.cs:1599-1604</summary>
        public const float WhirlRingRadiusBonus = 120f;
        public const int WhirlRingLifetime = 75;
        /// <summary>起手段：超过 400 px 追击（加速 0.15、限速 12），否则 0.9 刹停。沿用旧值 CrystallineSentinel.cs:1607-1614</summary>
        public const float WhirlChaseRange = 400f;
        public const float WhirlChaseAccel = 0.15f;
        public const float WhirlChaseMaxSpeed = 12f;
        public const float WhirlBrakeDamp = 0.9f;
        /// <summary>起手段手部帧收到 6（握刀姿态），收招段推回 12。沿用旧值 CrystallineSentinel.cs:1616-1619,1681-1684</summary>
        public const int WhirlHandFrameMin = 6;
        /// <summary>出手拍 = 起手段末 −7 帧：两把刀刃同时落地。沿用旧值 CrystallineSentinel.cs:1621</summary>
        public const int WhirlBladeCueLead = 7;
        /// <summary>刀刃出生偏移 (10,10) 绕半圈分布。沿用旧值 CrystallineSentinel.cs:1628</summary>
        public const float WhirlBladeSpawnOffset = 10f;
        /// <summary>刀刃伤害 120/140/180。沿用旧值 CrystallineSentinel.cs:1630</summary>
        public static int WhirlProjDamage() => Helper.GetProjDamage(120, 140, 180);
        public const float WhirlProjKnockback = 1f;
        /// <summary>蓄力爆发的表现拍：起手段末 + 47 帧（= 87 × 0.55）。沿用旧值 CrystallineSentinel.cs:1643</summary>
        public const int WhirlBurstCue = (int)(87 * 0.55f);

        //==================== 二阶段：休息 / 死亡 ====================

        /// <summary>休息起手速度对折，随后 X 衰减 0.95、Y 缓慢下坠到 1.5。沿用旧值 CrystallineSentinel.cs:1698-1704</summary>
        public const float RestLaunchDamp = 0.5f;
        public const float RestDampX = 0.95f;
        public const float RestAccelY = 0.02f;
        public const float RestLimitY = 1.5f;
        /// <summary>随机横向抖动：幅度随进度从 0.2 涨到 1.2 倍（越修越抖）。沿用旧值 CrystallineSentinel.cs:1705-1707</summary>
        public const float RestShakeMin = 0.1f;
        public const float RestShakeMax = 0.5f;
        public const float RestShakeBias = 0.2f;
        /// <summary>抖动系数的参考时长 4 秒（与普通难度的休息时长一致）。沿用旧值 CrystallineSentinel.cs:1705</summary>
        public const float RestFactorRamp = 4 * 60;
        /// <summary>休息时长 4/3/2.5/1 秒（难度越高修得越快）。沿用旧值 CrystallineSentinel.cs:1729</summary>
        public static int RestFrames() => Helper.ScaleValueForDiffMode(4 * 60, 3 * 60, 2 * 60 + 30, 60);
        /// <summary>修复完成的环形爆发 40 粒，散开角 ±0.2 rad、速度 1~5、出生散布 10 px。沿用旧值 CrystallineSentinel.cs:1731-1737</summary>
        public const int RestFinishParticleCount = 40;
        public const float RestFinishSpread = 0.2f;
        public const float RestFinishSpeedMin = 1f;
        public const float RestFinishSpeedMax = 5f;
        public const float RestFinishRadius = 10f;
        /// <summary>颜文字：休息期间持续刷“x x”，2 帧一次。沿用旧值 CrystallineSentinel.cs:1695</summary>
        public const int RestTextFrames = 2;
        /// <summary>修理火花每 5 帧一簇：结晶冲击尘 1 粒（半径 5~45、散开 ±0.4 rad）+ 烟尘 5 粒。沿用旧值 CrystallineSentinel.cs:1711-1725</summary>
        public const int RestDustInterval = 5;
        public const float RestDustRadiusMin = 5f;
        public const float RestDustRadiusMax = 45f;
        public const float RestDustSpread = 0.4f;
        public const int RestSmokeCount = 5;
        public const float RestSmokeSpeedX = 3f;
        public const float RestSmokeSpeedY = 6f;
        public const int RestSmokeAlpha = 80;
        /// <summary>休息期间的绘制抖动幅度：本体随修理进度放大到 4.5 px、双手到 5.5 px。沿用旧值 CrystallineSentinel.cs:2134,2141</summary>
        public const float RestShakeDrawBody = 4.5f;
        public const float RestShakeDrawHand = 5.5f;

        /// <summary>死亡演出：速度 0.9 衰减，90 帧后真死。沿用旧值 CrystallineSentinel.cs:1746,1779</summary>
        public const float DyingDamp = 0.9f;
        public const int DyingFrames = 90;
        /// <summary>CheckDead 在死亡演出 60 帧后放行真死（演出本体在 90 帧结束时自己调 Kill）。沿用旧值 CrystallineSentinel.cs:458</summary>
        public const int DyingDeadGate = 60;
        /// <summary>碎块 Gore 在 50~80 帧间每 2 帧崩一块。沿用旧值 CrystallineSentinel.cs:1762-1764</summary>
        public const int DyingGoreStart = 50;
        public const int DyingGoreEnd = 80;
        public const int DyingGoreInterval = 2;
        /// <summary>碎岩爆点 3 帧一个，散布 10~50 px。沿用旧值 CrystallineSentinel.cs:1749-1753</summary>
        public const int DyingBlastInterval = 3;
        public const float DyingBlastRadiusMin = 10f;
        public const float DyingBlastRadiusMax = 50f;
        /// <summary>死亡演出每帧一粒结晶闪光：出生散布 16 px、速度 1~8，整段从 0.3 倍涨到满速。沿用旧值 CrystallineSentinel.cs:1757-1759</summary>
        public const float DyingFlashRadius = 16f;
        public const float DyingFlashSpeedMin = 1f;
        public const float DyingFlashSpeedMax = 8f;
        public const float DyingFlashScaleStart = 0.3f;
        /// <summary>死亡 Gore 的散布 2~20 px、速度 1~7，并附一个 3 px/f 的上抛（与转阶段碎裂的区别就在这一下）。沿用旧值 CrystallineSentinel.cs:1767-1770</summary>
        public const float GoreOffsetMin = 2f;
        public const float GoreOffsetMax = 20f;
        public const float GoreSpeedMin = 1f;
        public const float GoreSpeedMax = 7f;
        public const float DyingGoreRiseY = -3f;
        /// <summary>死亡 Gore 不重组（旧代码给了一个近乎无限大的重组延迟）。沿用旧值 CrystallineSentinel.cs:1774</summary>
        public const int DyingGoreNoRebuild = 999999999;

        //==================== 部位挂点 / 表现 ====================

        /// <summary>手部帧上限 12（0 = 出手姿态，12 = 收拢）。沿用旧值 CrystallineSentinel.cs:75</summary>
        public const int MaxHandFrame = 12;
        /// <summary>头部挂点（警戒粒子 / 发光 / 音源）。沿用旧值 CrystallineSentinel.cs:1799</summary>
        public static Vector2 HeadOffset => new(0, -30);
        /// <summary>二阶段浮游炮挂点（X 随朝向翻转）。沿用旧值 CrystallineSentinel.cs:1789</summary>
        public const float FloatOffsetX = -4f;
        public const float FloatOffsetY = 18f;
        /// <summary>二阶段左右手挂点。沿用旧值 CrystallineSentinel.cs:1790-1791</summary>
        public static Vector2 LeftHandOffset => new(-2, -24);
        public static Vector2 RightHandOffset => new(-4, -23);
        /// <summary>一阶段三块浮石的二阶动力学参数（首块更硬）。沿用旧值 CrystallineSentinel.cs:567-581</summary>
        public const int FloatStoneCount = 3;
        public const float FloatStoneF = 0.6f;
        public const float FloatStoneZ = 0.5f;
        public const float FloatStoneFirstF = 0.8f;
        public const float FloatStoneFirstZ = 0.75f;
        public const float FloatStoneR = 1f;
        /// <summary>二阶段浮游炮 / 手部的二阶动力学参数。沿用旧值 CrystallineSentinel.cs:1856-1865</summary>
        public const float P2FloatF = 0.9f;
        public const float P2FloatZ = 0.8f;
        public const float P2FloatR = 0f;
        public const float P2HandR = 0.5f;
        /// <summary>浮游炮跟随响应：速度 0~14 映射到 1/40~1/3（越快越跟手）。沿用旧值 CrystallineSentinel.cs:1803</summary>
        public const float P2FloatFollowSpeedRef = 14f;
        public const float P2FloatFollowMin = 1 / 40f;
        public const float P2FloatFollowMax = 1 / 3f;
        /// <summary>手部跟随响应固定 1/20。沿用旧值 CrystallineSentinel.cs:1805-1806</summary>
        public const float P2HandFollow = 1 / 20f;
    }
}
