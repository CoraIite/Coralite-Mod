using Coralite.Helpers;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core
{
    /// <summary>
    /// 兹雷龙调参中心：全部数字的唯一出口。<br/>
    /// 每条注释写“为什么是这个数”；查不到原因的写“沿用旧值 &lt;文件&gt;:&lt;行&gt;”——本 boss 的招式节拍是作者手调的成品，
    /// 绝大多数常量属于后者，改动任何一条都会改变已上线的手感（D10）。
    /// </summary>
    internal static class ZacurrentDirector
    {
        //==================== 全局：脱战 / 兜底 / 通用表现 ====================

        /// <summary>丢失目标判定距离：超过就重新 TargetClosest。沿用旧值 ZcurrentAI.cs:208</summary>
        public const float RetargetDistance = 3000f;
        /// <summary>离场判定距离：重新选目标后仍超过就撤退。沿用旧值 ZcurrentAI.cs:212</summary>
        public const float DespawnDistance = 4500f;
        /// <summary>离场时 X 衰减。沿用旧值 ZcurrentAI.cs:217</summary>
        public const float DespawnDampX = 0.98f;
        /// <summary>离场时 Y 速度（向上冲出屏幕）。沿用旧值 ZcurrentAI.cs:218</summary>
        public const float DespawnRiseY = -60f;
        /// <summary>离场催促帧数。沿用旧值 ZcurrentAI.cs:220</summary>
        public const int DespawnEncourageFrames = 30;

        /// <summary>
        /// 单招超时兜底帧数（D2）。60×40 = 2400 帧远大于最长单招（聚集电流 570 帧），
        /// 只在招式被卡住（目标消失、子拍条件永不成立）时救场，正常战斗永不触发。
        /// </summary>
        public const int StateTimeoutFrames = 60 * 40;
        /// <summary>
        /// 连段超时兜底帧数。二阶段超长连段（吼叫 + 闪电链 + 3 轮指针电球/闪电突袭 + 引力电球 6 秒 + 吐息 + Z 电球 + 击穿 + 落雷）
        /// 满打满算约 60×60 帧，取 60×150 留三倍余量。
        /// </summary>
        public const int ComboTimeoutFrames = 60 * 150;
        /// <summary>超时救场时的刹速系数：这条路径上手感不重要，只求不带着残速进下一招。</summary>
        public const float TimeoutBrake = 0.6f;

        /// <summary>
        /// hub 停留帧数。旧代码招式返回 true 的当帧就 <c>PickNextState</c> 立刻切下一招，没有间隙，
        /// 所以取 0：hub 只是提交口，不占帧，节奏不变。<br/>
        /// 用 <c>static readonly</c> 而不是 <c>const</c>：它是可调旋钮，写成 const 会让编译期把 0 折进去、
        /// 把 hub 里"停留一帧"的那条分支判成不可达代码（CS0162）。
        /// </summary>
        public static readonly int HubFrames = 0;

        /// <summary>震屏衰减距离，所有招式共用。沿用旧值 AI.Roar.cs:40</summary>
        public const float ShakeFalloffDistance = 1000f;
        /// <summary>电球音效音高。沿用旧值 AI.Roar.cs:30</summary>
        public const float ElectricOrbPitch = 0.4f;
        /// <summary>电粒子随机缩放下限 / 上限，所有蓄力环共用。沿用旧值 AI.PointerBall.cs:31</summary>
        public const float ElectricParticleScaleMin = 0.9f;
        public const float ElectricParticleScaleMax = 1.3f;
        /// <summary>扇翅膀蓄力时每帧生成的电粒子数。沿用旧值 AI.PointerBall.cs:28</summary>
        public const int ElectricParticlePerFrame = 2;
        /// <summary>蓄力环收缩的起 / 止半径。沿用旧值 AI.PointerBall.cs:26</summary>
        public const float ChargeRingRadiusMin = 80f;
        public const float ChargeRingRadiusMax = 750f;
        /// <summary>“向后扇一下翅膀”的每帧步进（帧图 0..7 走完即进入下一拍）。沿用旧值 AI.ElectricChain.cs:32</summary>
        public const int WingFrameTime = 5;
        /// <summary>飞行帧图的最后一帧下标（0..7 共 8 帧，第 8 帧是冲刺专用图）。沿用旧值 ZcurrentAI.cs:603</summary>
        public const int WingFrameMax = 7;
        /// <summary>蓄力期整体衰减。沿用旧值 AI.PointerBall.cs:21</summary>
        public const float ChargeDamp = 0.8f;
        /// <summary>蓄力期朝向插值速率（比常规 0.08 快，让蓄力姿态立刻对准玩家）。沿用旧值 AI.PointerBall.cs:23</summary>
        public const float ChargeRotRate = 0.2f;

        //==================== 宿主：运动法则与动画 ====================

        /// <summary>常规朝向插值速率。沿用旧值 ZcurrentAI.cs:611</summary>
        public const float RotationRate = 0.08f;
        /// <summary>朝向目标角 = Y 速度 × 此系数。沿用旧值 ZcurrentAI.cs:615</summary>
        public const float RotationPerSpeedY = 0.05f;
        /// <summary>回正插值速率。沿用旧值 ZcurrentAI.cs:623</summary>
        public const float TurnToNoRotRate = 0.2f;
        /// <summary>嘴部锚点距中心距离（× scale）。沿用旧值 ZcurrentAI.cs:588</summary>
        public const float MouthDistance = 60f;
        /// <summary>嘴部锚点的朝向偏移（× direction）。沿用旧值 ZcurrentAI.cs:588</summary>
        public const float MouthAngleOffset = 0.07f;
        /// <summary>左右翻面的 X 死区：小于这个距离不翻面，防止贴脸时抖动。沿用旧值 ZcurrentAI.cs:631</summary>
        public const float SpriteFlipDeadZone = 48f;

        //==================== 紫伏（蓄电 / 击穿） ====================

        /// <summary>紫伏上限：单次红电蓄电量 × 3（红电占 1/3）× 5 × 2，平均 2~3 次蓄电进入紫伏。沿用旧值 ZcurrentAI.cs:542</summary>
        public static int PurpleVoltMax() => PurpleVoltRedGain() * 3 * 5 * 2;
        /// <summary>红色电流单次蓄电量（难度越高上限越高、也越难打破）。沿用旧值 ZcurrentAI.cs:523</summary>
        public static int PurpleVoltRedGain() => Helper.ScaleValueForDiffMode(30, 40, 50, 80);
        /// <summary>紫色电球单次蓄电量（红电的一半）。沿用旧值 ZcurrentAI.cs:524</summary>
        public static int PurpleVoltBallGain() => Helper.ScaleValueForDiffMode(15, 20, 25, 40);
        /// <summary>紫伏状态下承受伤害比例（只吃 5%，逼玩家去打电球）。沿用旧值 ZacurrentDragon.cs:223</summary>
        public const float PurpleVoltDamageTaken = 0.05f;
        /// <summary>伤害转成的扣电量系数：难度越高越难打破。沿用旧值 ZacurrentDragon.cs:227</summary>
        public static float PurpleVoltBreakScale() => Helper.ScaleValueForDiffMode(1, 1, 0.6f, 0.4f);

        //==================== 受击判定 ====================

        /// <summary>头部判定框边长（打头有加成）。沿用旧值 ZacurrentDragon.cs:218</summary>
        public const float HeadHitBoxSize = 60f;
        /// <summary>打头伤害加成。沿用旧值 ZacurrentDragon.cs:199</summary>
        public const float HeadHitBonus = 0.25f;
        /// <summary>敌对弹幕（玩家反射类）减伤。沿用旧值 ZacurrentDragon.cs:202</summary>
        public const float HostileProjReduction = 0.5f;
        /// <summary>电流环绕期间减伤。沿用旧值 ZacurrentDragon.cs:212</summary>
        public const float CurrentSurroundingReduction = 0.5f;

        //==================== 吼叫（连段起手：定身 + 镜头震 + 声波） ====================

        /// <summary>吼叫期速度衰减。沿用旧值 AI.Roar.cs:19</summary>
        public const float RoarDamp = 0.9f;
        /// <summary>起手门：翅膀帧图要先扇到第 4 帧才开始计时，保证每次吼叫的姿态一致。沿用旧值 AI.Roar.cs:20</summary>
        public const int RoarStartFrameY = 4;
        /// <summary>开嗓帧：这一帧定住速度、张嘴、点亮电流环绕。沿用旧值 AI.Roar.cs:26</summary>
        public const int RoarShoutFrame = 15;
        /// <summary>声波表现的结束帧。沿用旧值 AI.Roar.cs:35</summary>
        public const int RoarEffectEndFrame = 130;
        /// <summary>声波 / 震屏间隔（每 10 帧一圈波，每 20 帧一道线）。沿用旧值 AI.Roar.cs:38,45</summary>
        public const int RoarWaveInterval = 10;
        public const int RoarLineInterval = 20;
        /// <summary>吼叫震屏强度 / 抖动 / 持续帧。沿用旧值 AI.Roar.cs:40</summary>
        public const int RoarShakeStrength = 8;
        public const float RoarShakeVibration = 12f;
        public const int RoarShakeFrames = 20;
        /// <summary>声波粒子初始缩放与逐帧放大。沿用旧值 AI.Roar.cs:42-43</summary>
        public const float RoarWaveScale = 0.2f;
        public const float RoarWaveScaleMul = 1.15f;
        /// <summary>吼叫期背景压暗：亮度 / 渐变帧 / 过渡帧。沿用旧值 AI.Roar.cs:48</summary>
        public const float RoarSkyLight = 0.3f;
        public const int RoarSkyFade = 30;
        public const int RoarSkyExchange = 40;
        /// <summary>整段吼叫时长。沿用旧值 AI.Roar.cs:52</summary>
        public const int RoarFrames = 150;

        //==================== 短冲（身位调整；紫伏版更短更快） ====================

        /// <summary>普通短冲：单次持续帧 / 速度。沿用旧值 ZacurrentDragon.States.cs:193</summary>
        public const int SmallDashFrames = 10;
        public const int SmallDashSpeed = 45;
        /// <summary>紫伏短冲：更短更快。沿用旧值 ZacurrentDragon.States.cs:200</summary>
        public const int SmallDashVoltFrames = 8;
        public const int SmallDashVoltSpeed = 60;
        /// <summary>冲刺残影弹幕的第三个 ai 参数。沿用旧值 AI.SmallDash.cs:20</summary>
        public const int SmallDashProjAi2 = 10;
        /// <summary>冲刺伤害。沿用旧值 AI.SmallDash.cs:18</summary>
        public static int SmallDashDamage() => Helper.GetProjDamage(20, 30, 70);
        /// <summary>距离小于此值就不朝玩家冲（贴脸时故意冲偏，给逃生空间）。沿用旧值 AI.SmallDash.cs:26</summary>
        public const float SmallDashNoAimDistance = 700f;
        /// <summary>贴脸时的冲刺偏角范围。沿用旧值 AI.SmallDash.cs:27</summary>
        public const float SmallDashOffsetMin = 0.7f;
        public const float SmallDashOffsetMax = 1.2f;
        /// <summary>修正朝向的起始时机 = 单次冲刺时长的 1/4（起手四分之一帧不修正，保证起手方向是可读的预告）。沿用旧值 AI.SmallDash.cs:32</summary>
        public const int SmallDashHomingStartDiv = 4;
        /// <summary>冲刺途中修正朝向的距离门槛与归一化范围。沿用旧值 AI.SmallDash.cs:37-38</summary>
        public const float SmallDashHomingDistance = 500f;
        public const float SmallDashHomingRange = 600f;
        /// <summary>修正朝向的基础角速度、以及“速度每超出参考值多少就多转一点”的换算。沿用旧值 AI.SmallDash.cs:42</summary>
        public const float SmallDashTurnBase = 0.2f;
        public const float SmallDashTurnSpeedRef = 45f;
        public const float SmallDashTurnSpeedDiv = 200f;
        /// <summary>短冲次数随机范围（含下界、不含上界）。沿用旧值 AI.SmallDash.cs:71</summary>
        public const int SmallDashRepeatMin = 1;
        public const int SmallDashRepeatMax = 4;
        /// <summary>距离越远追加的短冲次数门槛。沿用旧值 AI.SmallDash.cs:74-77</summary>
        public const float SmallDashExtraDistance1 = 700f;
        public const float SmallDashExtraDistance2 = 1000f;

        //==================== 指针电球 / 追踪雷球（扇翅膀蓄力 → 三波环形撒球） ====================

        /// <summary>指针电球蓄力环收缩时长（紫伏版更短）。沿用旧值 AI.PointerBall.cs:25</summary>
        public const float PointerChargeRamp = 40f;
        /// <summary>指针电球扇翅膀的每帧步进（比通用 5 更快）。沿用旧值 AI.PointerBall.cs:36</summary>
        public const int PointerWingFrameTime = 3;
        /// <summary>普通追踪雷球蓄力环收缩时长。沿用旧值 AI.AimThunderBall.cs:28</summary>
        public const float AimChargeRamp = 55f;
        /// <summary>撒球间隔与波数：3 波、每 15 帧一波、每波 3 颗。沿用旧值 AI.PointerBall.cs:62,64,76</summary>
        public const int AimShootInterval = 15;
        public const int AimShootWaves = 3;
        public const int AimShootCount = 3;
        /// <summary>每波的环形起始角随帧数缓慢旋转，让三波不重叠。沿用旧值 AI.PointerBall.cs:69</summary>
        public const float AimSpinPerFrame = 0.08f;
        /// <summary>每颗球的角度抖动。沿用旧值 AI.PointerBall.cs:69</summary>
        public const float AimSpreadJitter = 0.2f;
        /// <summary>撒球初速范围。沿用旧值 AI.PointerBall.cs:70</summary>
        public const float AimBallSpeedMin = 4f;
        public const float AimBallSpeedMax = 8f;
        /// <summary>后发的球追踪时间更长（每帧 +1/2），保证同一批球大致同时命中。沿用旧值 AI.PointerBall.cs:70</summary>
        public const int AimTimeGainDiv = 2;
        /// <summary>紫伏指针电球伤害。沿用旧值 AI.PointerBall.cs:66</summary>
        public static int PointerBallDamage() => Helper.GetProjDamage(80, 100, 120);
        /// <summary>普通追踪雷球伤害（单招池里没有它，只出现在连段里，所以更高）。沿用旧值 AI.AimThunderBall.cs:69</summary>
        public static int AimThunderBallDamage() => Helper.GetProjDamage(80, 140, 180);
        /// <summary>单招指针电球的追踪时长。沿用旧值 ZacurrentDragon.States.cs:233</summary>
        public const int PointerBallAimTime = 120;

        //==================== 闪电链（扇翅膀 → 斜切冲刺 → 绕玩家转圈布链 → 后摇） ====================

        /// <summary>拉开距离时的速度上限与每帧加速。沿用旧值 AI.ElectricChain.cs:28-29</summary>
        public const float ChainRetreatSpeedCap = 8f;
        public const float ChainRetreatAccel = 0.65f;
        /// <summary>绕飞起始角相对“背离玩家方向”的偏移（45°，让斜切进场）。沿用旧值 AI.ElectricChain.cs:40</summary>
        public const float ChainAnchorOffset = MathHelper.PiOver4;
        /// <summary>冲刺期残影放大。沿用旧值 AI.ElectricChain.cs:43</summary>
        public const float ChainShadowScale = 1.2f;
        /// <summary>起手冲刺的瞄点：玩家外侧 500 px（不直冲玩家，切过去）。沿用旧值 AI.ElectricChain.cs:49</summary>
        public const float ChainDashLead = 500f;
        /// <summary>起手冲刺速度。沿用旧值 AI.ElectricChain.cs:50</summary>
        public const float ChainDashSpeed = 50f;
        /// <summary>起手背景压暗：亮度 / 渐变帧 / 过渡帧。沿用旧值 AI.ElectricChain.cs:53</summary>
        public const float ChainSkyLight = 0.4f;
        public const int ChainSkyFade = 25;
        public const int ChainSkyExchange = 8;
        /// <summary>起手风环表现：缩放 / 放大率 / 拉伸。沿用旧值 AI.ElectricChain.cs:55-56</summary>
        public const float ChainWindScale = 0.6f;
        public const float ChainWindScaleMul = 3f;
        /// <summary>链球伤害。沿用旧值 AI.ElectricChain.cs:58</summary>
        public static int ChainDamage() => Helper.GetProjDamage(200, 250, 300);
        /// <summary>绕飞时长。沿用旧值 AI.ElectricChain.cs:68</summary>
        public const int ChainRollFrames = 60;
        /// <summary>绕飞圈数（1.5 圈，收尾角度与起手错开）。沿用旧值 AI.ElectricChain.cs:70</summary>
        public const float ChainRollTurns = 1.5f;
        /// <summary>绕飞半径（绕玩家 600 px）。沿用旧值 AI.ElectricChain.cs:73</summary>
        public const float ChainOrbitRadius = 600f;
        /// <summary>追点速度换算：距离 300 px 以上给满速 100。沿用旧值 AI.ElectricChain.cs:80</summary>
        public const float ChainApproachRange = 300f;
        public const float ChainMaxSpeed = 100f;
        /// <summary>绕飞转向角速度与速度插值率。沿用旧值 AI.ElectricChain.cs:82</summary>
        public const float ChainTurnRate = 0.35f;
        public const float ChainSpeedLerp = 0.85f;
        /// <summary>布链间隔与每段链的额外寿命。沿用旧值 AI.ElectricChain.cs:86,90</summary>
        public const int ChainSpawnInterval = 3;
        public const int ChainLifeGain = 30;
        /// <summary>绕飞结束的刹速。沿用旧值 AI.ElectricChain.cs:100</summary>
        public const float ChainRollExitDamp = 0.2f;
        /// <summary>后摇的距离带：太近继续退、太远追上来、中间衰减。沿用旧值 AI.ElectricChain.cs:111,116,122</summary>
        public const float ChainNearDistance = 400f;
        public const float ChainFarDistance = 900f;
        public const float ChainRecoverNearSpeedCap = 8f;
        public const float ChainRecoverFarSpeedCap = 12f;
        public const float ChainRecoverDamp = 0.95f;
        /// <summary>后摇时长。沿用旧值 AI.ElectricChain.cs:125</summary>
        public const int ChainRecoverFrames = 20;

        //==================== 各招式在连段里的参数（连段是手调的节拍，参数不可改） ====================

        /// <summary>吼叫连段 1 / 2 的闪电链持续。沿用旧值 ZacurrentDragon.Combos.cs:68,100,144,188,261</summary>
        public const int ChainTimeRoarCombo2 = 100;
        public const int ChainTimeChainCombo = 60;
        public const int ChainTimePointerCombo = 300;
        public const int ChainTimeVoltBigCombo = 10;
        public const int ChainTimeVoltChainCombo = 140;
        /// <summary>普通指针连段里的追踪雷球追踪时长。沿用旧值 ZacurrentDragon.Combos.cs:137</summary>
        public const int AimTimeNormalPointerCombo = 90;
        /// <summary>Z 电球连段末段的指针电球追踪时长（比单招更长）。沿用旧值 ZacurrentDragon.Combos.cs:316</summary>
        public const int PointerAimTimeZBallCombo = 180;
        /// <summary>吼叫连段 2 / Z 电球连段固定 3 次长冲。沿用旧值 ZacurrentDragon.Combos.cs:73,296</summary>
        public const int LightningRaidLongDashesCombo = 3;
        /// <summary>超长连段里每轮只做 1 次长冲。沿用旧值 ZacurrentDragon.Combos.cs:201</summary>
        public const int LightningRaidLongDashesBigCombo = 1;
        /// <summary>超长连段里引力电球的超长持续。沿用旧值 ZacurrentDragon.Combos.cs:215</summary>
        public const int GravitationTimeBigCombo = 60 * 6;
        /// <summary>超长连段里中吐息的后摇被压到 2 帧（连段自己接下一手，不需要喘息）。沿用旧值 ZacurrentDragon.Combos.cs:222</summary>
        public const int BreathMiddleRestBigCombo = 2;
        /// <summary>超长连段里"指针电球 → 闪电突袭"的循环轮数。沿用旧值 ZacurrentDragon.Combos.cs:194-212</summary>
        public const int VoltBigComboRaidLoops = 3;

        //==================== 选招池 ====================

        /// <summary>小吐息在紫伏池里概率减半（紫伏期节奏更快，小招不该占位）。沿用旧值 ZcurrentAI.cs:420</summary>
        public const float WeightBreathSmallVolt = 0.5f;
        /// <summary>普通池小吐息权重。沿用旧值 ZcurrentAI.cs:454</summary>
        public const float WeightBreathSmallNormal = 1f;
        /// <summary>玩家在正上 / 正下方时中吐息降权（吐息是横扫的，垂直位打不到）。沿用旧值 ZcurrentAI.cs:426</summary>
        public const float WeightBreathMiddleUpDown = 0.4f;
        public const float WeightBreathMiddleNormal = 1f;
        /// <summary>“正上下方”判定：X 差 8 格内、Y 差 6 格外。沿用旧值 ZcurrentAI.cs:424-425</summary>
        public const float UpDownCheckX = 16 * 8;
        public const float UpDownCheckY = 16 * 6;
        /// <summary>紫伏池普通电球降权（紫伏期有更强的指针电球）。沿用旧值 ZcurrentAI.cs:428</summary>
        public const float WeightElectricBallVolt = 0.5f;
        public const float WeightElectricBallNormal = 1f;
        /// <summary>指针电球权重。沿用旧值 ZcurrentAI.cs:429</summary>
        public const float WeightPointerBall = 1f;
        /// <summary>远距离判定与远距离加权曲线（越远越倾向位移招）。沿用旧值 ZcurrentAI.cs:432-435</summary>
        public const float FarAwayDistance = 650f;
        public const float FarAwayWeightBase = 1.5f;
        public const float FarAwayWeightDiv = 400f;
        /// <summary>短冲额外权重：紫伏版 +3，普通版 +1.5（位移招是喘息，不能太稀）。沿用旧值 ZcurrentAI.cs:440,472</summary>
        public const float SmallDashExtraWeightVolt = 3f;
        public const float SmallDashExtraWeightNormal = 1.5f;
        /// <summary>连招解锁阈值：每出这么多手单招后把连招放进池子（难度越高越快）。沿用旧值 ZcurrentAI.cs:443,475</summary>
        public static int ComboUnlockMoves() => Helper.ScaleValueForDiffMode(5, 4, 3, 2);
        /// <summary>连招记录上限：攒够这么多就清空，让整批连招重新可用。沿用旧值 ZcurrentAI.cs:500</summary>
        public const int ComboRecordCap = 2;
        /// <summary>超长连段的额外门槛：必须已经用过一个以上别的连招（它是收尾大招）。沿用旧值 ZcurrentAI.cs:445</summary>
        public const int VoltBigComboRequireRecords = 1;
        /// <summary>最近几手的查重窗口长度（记账用，本轮只记不裁决，裁决仍是旧的“硬锁上一手”）。</summary>
        public const int RecentPickWindow = 4;

        // ==== 追加区（按招式分区，逐招落地时追加在此之后） ====

        #region 闪电链：起手风环

        /// <summary>起手风环的反向初速（沿冲刺方向的反方向甩出去）。沿用旧值 AI.ElectricChain.cs:55</summary>
        public const float ChainWindSpeed = 2f;
        /// <summary>起手风环的拉伸比（横向拉长 1.25）。沿用旧值 AI.ElectricChain.cs:56</summary>
        public static readonly Vector2 ChainWindStretch = new Vector2(1.25f, 1f);

        #endregion

        #region 引力电球（吐出一颗长时间牵引玩家的雷球）

        /// <summary>默认持续时长（连段里会被改成 6 秒）。沿用旧值 AI.GravitationThunder.cs:12</summary>
        public const int GravitationDefaultTime = 60 * 5;
        /// <summary>吐球前的追踪时长；翅膀回到第 0 帧且过了这个时间才转下一拍。沿用旧值 AI.GravitationThunder.cs:20</summary>
        public const int GravitationReadyFrames = 45;
        /// <summary>嘴前汇聚尘的收束半径：从 240 收到 100（除以 2 后是实际散布）。沿用旧值 AI.GravitationThunder.cs:23-25</summary>
        public const float GravitationGatherEdgeMax = 240f;
        public const float GravitationGatherEdgeShrink = 140f;
        /// <summary>汇聚尘的速度与缩放范围。沿用旧值 AI.GravitationThunder.cs:27-28</summary>
        public const float GravitationDustSpeedMin = 2f;
        public const float GravitationDustSpeedMax = 4f;
        public const float GravitationDustScaleMin = 1f;
        public const float GravitationDustScaleMax = 1.5f;
        /// <summary>追踪段：X 距离超过这个值才平移，Y 距离超过这个值才升降。沿用旧值 AI.GravitationThunder.cs:36,43</summary>
        public const float GravitationChaseDeadZoneX = 50f;
        public const float GravitationChaseDeadZoneY = 70f;
        /// <summary>追踪段 X 的运动参数（速度上限 / 加速 / 转向 / 衰减）。沿用旧值 AI.GravitationThunder.cs:37</summary>
        public const float GravitationChaseSpeedX = 8f;
        public const float GravitationChaseAccelX = 0.3f;
        public const float GravitationChaseTurnX = 0.6f;
        public const float GravitationChaseDampX = 0.95f;
        /// <summary>追踪段 Y 的运动参数。沿用旧值 AI.GravitationThunder.cs:45</summary>
        public const float GravitationChaseSpeedY = 5f;
        public const float GravitationChaseAccelY = 0.3f;
        public const float GravitationChaseTurnY = 0.5f;
        public const float GravitationChaseDampY = 0.95f;
        /// <summary>向上飞的加速 / 上限 / 减速。沿用旧值 AI.GravitationThunder.cs:42</summary>
        public const float GravitationRiseAccel = 0.35f;
        public const float GravitationRiseMax = 15f;
        public const float GravitationRiseDamp = 0.9f;
        /// <summary>吐球拍：刹速、起手翅膀帧、张嘴到出球的间隔、出球初速。沿用旧值 AI.GravitationThunder.cs:66,70,76,84</summary>
        public const float GravitationShootDamp = 0.96f;
        public const int GravitationShootFrameY = 4;
        public const int GravitationShootDelay = 10;
        public const float GravitationBallSpeed = 2f;
        /// <summary>引力雷球伤害。沿用旧值 AI.GravitationThunder.cs:80</summary>
        public static int GravitationBallDamage() => Helper.GetProjDamage(150, 200, 250);
        /// <summary>出球后的残影膨胀段长度与残影缩放上限。沿用旧值 AI.GravitationThunder.cs:14,96-98</summary>
        public const int GravitationBurstFrames = 40;
        public const float GravitationBurstShadowScale = 1.5f;
        /// <summary>膨胀段的翅膀步进（很快，表现"抖了一下"）。沿用旧值 AI.GravitationThunder.cs:102</summary>
        public const int GravitationBurstWingFrameTime = 1;
        /// <summary>后摇帧数。沿用旧值 AI.GravitationThunder.cs:123</summary>
        public const int GravitationRecoverFrames = 30;

        #endregion

        #region 闪电突袭 / 电伏击穿共用：蓄力后撤、大冲、折返

        /// <summary>身体压平时的朝向值（沿用旧代码的 3.141 近似值，不要改成 MathHelper.Pi，差值会让贴图翻面时机变化）。沿用旧值 AI.LightningRaidNormal.cs:189</summary>
        public const float FlatRotation = 3.141f;
        /// <summary>蓄力后撤段长度与收翅步进（帧图从 7 倒着走回 0 = 把翅膀收起来蓄力）。沿用旧值 AI.LightningRaidNormal.cs:93,114</summary>
        public const int RaidReadyFrames = 6 * 9;
        public const int RaidWingFrameTime = 6;
        /// <summary>蓄力段的距离带：贴脸就退、过远就追、中间衰减。沿用旧值 AI.LightningRaidNormal.cs:99-110</summary>
        public const float RaidHoldNearDistance = 800f;
        public const float RaidHoldFarDistance = 1000f;
        public const float RaidHoldSpeedCap = 8f;
        public const float RaidHoldDamp = 0.9f;
        /// <summary>蓄力电粒子：沿冲刺方向铺出 1000 px 的"跑道"，外加半径从 80 涨到 250 的环。沿用旧值 AI.LightningRaidNormal.cs:124,129</summary>
        public const float RaidParticleForward = 1000f;
        public const float RaidParticleRadiusMin = 80f;
        public const float RaidParticleRadiusMax = 250f;
        /// <summary>蓄力期残影从 1 膨胀到 2.5 同时淡出（紫伏版才有）。沿用旧值 AI.LightningRaidVolt.cs:129-130</summary>
        public const float RaidShadowScaleMax = 2.5f;
        /// <summary>大冲起手残影缩放（紫伏版）。沿用旧值 AI.LightningRaidVolt.cs:151</summary>
        public const float RaidDashShadowScale = 1.1f;
        /// <summary>大冲的追踪窗口（冲程的 40%~80%，头尾不追 = 起手方向即承诺、收尾不赖着不走）。沿用旧值 AI.LightningRaidNormal.cs:172</summary>
        public const float RaidHomingStart = 0.4f;
        public const float RaidHomingEnd = 0.8f;
        /// <summary>大冲追踪的距离归一化范围。沿用旧值 AI.LightningRaidNormal.cs:175</summary>
        public const float RaidHomingRange = 1000f;
        /// <summary>冲完的刹车与"停稳"阈值。沿用旧值 AI.LightningRaidNormal.cs:194,180</summary>
        public const float RaidBrakeDamp = 0.8f;
        public const float RaidSettleSpeed = 2f;
        /// <summary>每轮之间的休息（难度越高越短）；还有下一轮时只歇 5 帧。沿用旧值 AI.LightningRaidNormal.cs:206,209</summary>
        public static int RaidRestFrames() => Helper.ScaleValueForDiffMode(60, 40, 30, 15);
        public const int RaidRestWhenChaining = 5;
        /// <summary>第二轮起的短冲次数基数（远距离 +1）。沿用旧值 AI.LightningRaidNormal.cs:221-224</summary>
        public const int RaidNextRoundDashes = 2;
        /// <summary>大冲起手的镜头冲击与风环（两招共用）。沿用旧值 AI.LightningRaidNormal.cs:165-169</summary>
        public const float RaidPunchDirScale = 2.3f;
        public const int RaidShakeStrength = 16;
        public const float RaidShakeVibration = 5f;
        public const int RaidShakeFrames = 20;
        public const float RaidSkyLight = 0.4f;
        public const int RaidSkyFadeLead = 3;
        public const int RaidSkyExchange = 8;
        /// <summary>折返冲刺（Z 字）的偏角与四个折点（占冲程的 4/20、7/20、13/20、16/20）。沿用旧值 AI.VoltBreak.cs:138-158</summary>
        public const float ZMoveAngle = 0.3f;
        public const int ZMoveStep1 = 4;
        public const int ZMoveStep2 = 7;
        public const int ZMoveStep3 = 13;
        public const int ZMoveStep4 = 16;
        public const int ZMoveDenominator = 20;

        #endregion

        #region 闪电突袭（普通形态）

        /// <summary>短冲单次时长 / 速度 / 残影弹幕 ai2。沿用旧值 AI.LightningRaidNormal.cs:29,38,45</summary>
        public const int RaidSmallDashFrames = 10;
        public const int RaidSmallDashSpeed = 45;
        public const int RaidSmallDashProjAi2 = 10;
        /// <summary>短冲途中的修正角速度。沿用旧值 AI.LightningRaidNormal.cs:68</summary>
        public const float RaidSmallDashTurn = 0.2f;
        /// <summary>大冲时长 / 速度 / 残影弹幕 ai2。沿用旧值 AI.LightningRaidNormal.cs:30,145,153</summary>
        public const int RaidBigDashFrames = 25;
        public const float RaidBigDashSpeed = 50f;
        public const int RaidBigDashProjAi2 = 15;
        /// <summary>大冲追踪角速度。沿用旧值 AI.LightningRaidNormal.cs:179</summary>
        public const float RaidBigDashTurn = 0.08f;
        /// <summary>大冲伤害。沿用旧值 AI.LightningRaidNormal.cs:151</summary>
        public static int RaidBigDashDamage() => Helper.GetProjDamage(130, 160, 200);
        /// <summary>首轮短冲次数随机范围与远距离追加门槛。沿用旧值 AI.LightningRaidNormal.cs:249-255</summary>
        public const int RaidStartDashMin = 2;
        public const int RaidStartDashMax = 4;
        /// <summary>整招最多重复几轮（登场首招固定 1 轮）。沿用旧值 AI.LightningRaidNormal.cs:258</summary>
        public const int RaidRoundMax = 4;

        #endregion

        #region 闪电突袭（紫伏形态：更短更快，大冲带 Z 字折返）

        /// <summary>短冲单次时长 / 速度 / 残影弹幕 ai2 / 修正角速度。沿用旧值 AI.LightningRaidVolt.cs:21,32,39,61</summary>
        public const int RaidVoltSmallDashFrames = 7;
        public const int RaidVoltSmallDashSpeed = 60;
        public const int RaidVoltSmallDashProjAi2 = 6;
        public const float RaidVoltSmallDashTurn = 0.35f;
        /// <summary>大冲时长 / 速度 / 残影弹幕 ai2 / 追踪角速度。沿用旧值 AI.LightningRaidVolt.cs:22,24,141,176</summary>
        public const int RaidVoltBigDashFrames = 26;
        public const float RaidVoltBigDashSpeed = 55f;
        public const int RaidVoltBigDashProjAi2 = 14;
        public const float RaidVoltBigDashTurn = 0.1f;
        /// <summary>大冲伤害（紫伏版更高）。沿用旧值 AI.LightningRaidVolt.cs:139</summary>
        public static int RaidVoltBigDashDamage() => Helper.GetProjDamage(150, 175, 200);

        #endregion

        #region 电伏击穿（紫伏连段件：绕到侧面 → 横穿全场 + 沿途落雷）

        /// <summary>绕位目标：玩家左 / 右 700 px；蓄力时收到 650 px。沿用旧值 AI.VoltBreak.cs:22,54</summary>
        public const float BreakSideDistance = 700f;
        public const float BreakSideHoldDistance = 650f;
        /// <summary>绕位速度换算与插值，超时与到位判定。沿用旧值 AI.VoltBreak.cs:29-36</summary>
        public const float BreakApproachRange = 300f;
        public const float BreakApproachMaxSpeed = 60f;
        public const float BreakApproachLerp = 0.85f;
        public const int BreakApproachTimeout = 60 * 3;
        public const float BreakArriveDistance = 80f;
        /// <summary>收翅蓄力段长度（帧图从第 8 帧倒着收回去）。沿用旧值 AI.VoltBreak.cs:40,47</summary>
        public const int BreakChargeFrames = 6 * 6;
        public const int BreakChargeStartFrameY = 8;
        /// <summary>蓄力期微调身位的运动参数。沿用旧值 AI.VoltBreak.cs:58,67</summary>
        public const float BreakHoldSpeedX = 8f;
        public const float BreakHoldAccelX = 0.3f;
        public const float BreakHoldTurnX = 0.6f;
        public const float BreakHoldSpeedY = 8f;
        public const float BreakHoldAccelY = 0.3f;
        public const float BreakHoldTurnY = 0.5f;
        /// <summary>横冲时长 / 速度 / 残影弹幕 ai2。沿用旧值 AI.VoltBreak.cs:15-16,109</summary>
        public const int BreakDashFrames = 26;
        public const float BreakDashSpeed = 55f;
        public const int BreakDashProjAi2 = 14;
        /// <summary>横冲伤害。沿用旧值 AI.VoltBreak.cs:107</summary>
        public static int BreakDashDamage() => Helper.GetProjDamage(130, 160, 200);
        /// <summary>沿途落雷：每 3 帧一发，落在自己下方 400 px，延迟随时间推移拉长。沿用旧值 AI.VoltBreak.cs:161-166</summary>
        public const int BreakThunderInterval = 3;
        public const float BreakThunderDropY = 400f;
        public const float BreakThunderDelayBase = 35f;
        public const int BreakThunderAi2 = 65;
        public static int BreakThunderDamage() => Helper.GetProjDamage(100, 150, 200);

        #endregion

        #region 电球 / Z 电球共用：嘴前汇聚尘、贴近追踪、距离带巡航

        /// <summary>嘴前汇聚尘的收束半径：从 140 收到 80（除以 2 后是实际散布）。沿用旧值 AI.ElectricBall.cs:25-26</summary>
        public const float BallGatherEdgeMax = 140f;
        public const float BallGatherEdgeShrink = 60f;
        /// <summary>追踪段死区。沿用旧值 AI.ElectricBall.cs:38,44</summary>
        public const float BallChaseDeadZoneX = 50f;
        public const float BallChaseDeadZoneY = 70f;
        /// <summary>追踪段 X / Y 的运动参数（比引力电球更慢更稳，因为要边追边蓄）。沿用旧值 AI.ElectricBall.cs:39,47</summary>
        public const float BallChaseSpeedX = 6f;
        public const float BallChaseAccelX = 0.15f;
        public const float BallChaseTurnX = 0.4f;
        public const float BallChaseSpeedY = 4f;
        public const float BallChaseAccelY = 0.15f;
        public const float BallChaseTurnY = 0.3f;
        public const float BallChaseDamp = 0.95f;
        /// <summary>挥翅拍的刹速与出手翅膀帧。沿用旧值 AI.ElectricBall.cs:68,72</summary>
        public const float BallSwingDamp = 0.96f;
        public const int BallSwingFrameY = 4;
        /// <summary>距离带巡航：贴脸就退、拉远就追、中距衰减。沿用旧值 AI.ElectricBall.cs:234-247</summary>
        public const float BallFlyNearDistance = 400f;
        public const float BallFlyFarDistance = 900f;
        public const float BallFlyNearSpeedCap = 8f;
        public const float BallFlyFarSpeedCap = 20f;
        public const float BallFlyAccel = 0.65f;
        /// <summary>出手镜头冲击（强度 20、抖 15、20 帧）。沿用旧值 AI.ElectricBall.cs:210-211</summary>
        public const int BallShakeStrength = 20;
        public const float BallShakeVibration = 15f;
        public const int BallShakeFrames = 20;
        /// <summary>出手背景压暗。沿用旧值 AI.ElectricBall.cs:214</summary>
        public const float BallSkyLight = 0.4f;
        public const int BallSkySeadeFrames = 25;
        public const int BallSkyExchange = 8;
        /// <summary>预判起效的玩家速度门槛。沿用旧值 AI.ElectricBall.cs:149</summary>
        public const float BallPredictSpeedGate = 4f;

        #endregion

        #region 电球（普通形态单招：贴近 → 连射小球 → 两轮组合弹幕）

        /// <summary>贴近段时长。沿用旧值 AI.ElectricBall.cs:22</summary>
        public const int ElectricBallReadyFrames = 35;
        /// <summary>挥翅到出手的间隔。沿用旧值 AI.ElectricBall.cs:77</summary>
        public const int ElectricBallSwingDelay = 10;
        /// <summary>连射段：每 20 帧一颗，整段长 20×4 帧（拍点落在 0/20/40/60/80，共 5 颗）。沿用旧值 AI.ElectricBall.cs:97,112</summary>
        public const int ElectricBallShootInterval = 20;
        public const int ElectricBallShootRounds = 4;
        /// <summary>连射球的初速与每发后坐。沿用旧值 AI.ElectricBall.cs:102,104</summary>
        public const float ElectricBallShootSpeed = 4f;
        public const float ElectricBallShootRecoil = 3f;
        /// <summary>连射球伤害。沿用旧值 AI.ElectricBall.cs:100</summary>
        public static int ElectricBallShootDamage() => Helper.GetProjDamage(60, 80, 120);
        /// <summary>组合弹幕段：蓄 40 帧出手，再 30 帧后进下一轮 / 收招。沿用旧值 AI.ElectricBall.cs:130,217</summary>
        public const int ElectricBallVolleyReady = 40;
        public const int ElectricBallVolleyRecover = 30;
        /// <summary>组合弹幕伤害。沿用旧值 AI.ElectricBall.cs:145</summary>
        public static int ElectricBallVolleyDamage() => Helper.GetProjDamage(100, 125, 145);
        /// <summary>预判提前量（玩家在动时把落点推到身前 340 px）。沿用旧值 AI.ElectricBall.cs:150</summary>
        public const float ElectricBallPredictLead = 340f;
        /// <summary>三重球的外圈 / 内圈散角，与链球初速。沿用旧值 AI.ElectricBall.cs:163,170,184</summary>
        public const float ElectricBallSpreadOuter = 0.35f;
        public const float ElectricBallSpreadInner = 0.15f;
        public const float ElectricBallVolleySpeed = 3f;
        public const float ElectricBallChainSpeed = 7f;
        /// <summary>样式数：3 种组合（三重 / 单球加直链 / 旋转链）。沿用旧值 AI.ElectricBall.cs:255</summary>
        public const int ElectricBallStyleCount = 3;

        #endregion

        #region Z 电球（紫伏连段件：起手更快、弹幕更密的红色版本）

        /// <summary>贴近段时长（远短于普通电球，紫伏期节奏更快）。沿用旧值 AI.ZThunderBall.cs:22</summary>
        public const int ZBallReadyFrames = 10;
        /// <summary>挥翅到出手的间隔。沿用旧值 AI.ZThunderBall.cs:77</summary>
        public const int ZBallSwingDelay = 5;
        /// <summary>连射段：每 16 帧一颗，整段长 16×5 帧（拍点落在 0/16/…/80，共 6 颗）。沿用旧值 AI.ZThunderBall.cs:97,113</summary>
        public const int ZBallShootInterval = 16;
        public const int ZBallShootRounds = 5;
        /// <summary>连射球的初速、随机散角与每发后坐。沿用旧值 AI.ZThunderBall.cs:103,106</summary>
        public const float ZBallShootSpeed = 4f;
        public const float ZBallShootJitter = 0.2f;
        public const float ZBallShootRecoil = 2f;
        /// <summary>组合弹幕段：蓄 35 帧出手，再 15 帧进下一轮 / 收招。沿用旧值 AI.ZThunderBall.cs:131,201</summary>
        public const int ZBallVolleyReady = 35;
        public const int ZBallVolleyRecover = 15;
        /// <summary>组合弹幕伤害。沿用旧值 AI.ZThunderBall.cs:146</summary>
        public static int ZBallVolleyDamage() => Helper.GetProjDamage(140, 160, 180);
        /// <summary>预判提前量（比普通电球短，因为弹速更快）。沿用旧值 AI.ZThunderBall.cs:151</summary>
        public const float ZBallPredictLead = 140f;
        /// <summary>三重球的外圈 / 内圈散角，与链球初速。沿用旧值 AI.ZThunderBall.cs:164,181,168</summary>
        public const float ZBallSpreadOuter = 0.5f;
        public const float ZBallSpreadInner = 0.3f;
        public const float ZBallVolleySpeed = 3f;
        public const float ZBallChainSpeed = 8f;
        /// <summary>样式数：2 种组合（单球加直链 / 旋转链）。沿用旧值 AI.ZThunderBall.cs:237</summary>
        public const int ZBallStyleCount = 2;

        #endregion

        #region 落雷（连段收尾件：隐身升空 → 悬顶跟踪 → 砸下来）

        /// <summary>悬顶高度与砸落时长（700 px / 12 帧 ≈ 58 px/f）。沿用旧值 AI.FallingThunder.cs:17-18</summary>
        public const int FallingUpLength = 700;
        public const int FallingSmashFrames = 12;
        /// <summary>就位段上限与运动参数：把横向距离维持在 400~600 px。沿用旧值 AI.FallingThunder.cs:25-48</summary>
        public const int FallingChaseTimeout = 60 * 4;
        public const float FallingHoldNearX = 400f;
        public const float FallingHoldFarX = 600f;
        public const float FallingChaseSpeedX = 24f;
        public const float FallingChaseAccelX = 0.5f;
        public const float FallingChaseTurnX = 0.6f;
        public const float FallingRiseAccel = 0.4f;
        public const float FallingRiseMax = 15f;
        public const float FallingRiseDamp = 0.93f;
        public const float FallingChaseSpeedY = 20f;
        public const float FallingChaseAccelY = 0.4f;
        public const float FallingChaseTurnY = 0.6f;
        /// <summary>提前结束就位的条件：横向已拉开 350、纵向已对齐 250，且至少走了 30 帧。沿用旧值 AI.FallingThunder.cs:54</summary>
        public const float FallingArriveX = 350f;
        public const float FallingArriveY = 250f;
        public const int FallingArriveMinFrames = 30;
        /// <summary>升空段：先下沉 10 帧蓄势，再以 -50 px/f 冲上天并在 20 帧里淡出，24 帧后彻底隐身。沿用旧值 AI.FallingThunder.cs:60-87</summary>
        public const float FallingPreDropSpeed = 20f;
        public const float FallingRiseShadowScale = 1.15f;
        public const int FallingPreDropFrames = 10;
        public const float FallingRiseSpeed = -50f;
        public const float FallingFadeFrames = 20f;
        public const int FallingVanishFrames = 24;
        /// <summary>悬顶段总长与"停止跟踪"的时点（最后 45 帧锁死落点，这就是逃生窗）。沿用旧值 AI.FallingThunder.cs:104-105</summary>
        public const int FallingAimFrames = 200;
        public const int FallingTrackStopLead = 45;
        /// <summary>跟踪落点的推进速度与玩家速度预判系数。沿用旧值 AI.FallingThunder.cs:135-137</summary>
        public const float FallingTrackStep = 20f;
        public const float FallingTrackPredictFrames = 28f;
        public const float FallingTrackRampFrames = 80f;
        /// <summary>骚扰落雷：每 45 帧一发（带 250 px 预判、左右交替偏移、80 px 抖动）、每 60 帧一发直接砸玩家脚下。沿用旧值 AI.FallingThunder.cs:108-131</summary>
        public const int FallingHarassInterval = 45;
        public const int FallingHarassDirectInterval = 60;
        public const float FallingHarassPredict = 250f;
        public const float FallingHarassSwingPerFrame = 2f;
        public const float FallingHarassDropY = 260f;
        public const float FallingHarassJitter = 80f;
        public const int FallingHarassLife = 75;
        public const int FallingHarassDirectLife = 60;
        public const int FallingHarassAi2 = 65;
        public static int FallingHarassDamage() => Helper.GetProjDamage(100, 150, 200);
        /// <summary>落点指示粒子的散布半径（锁定后逐渐张大到 90，提示"要落了"）。沿用旧值 AI.FallingThunder.cs:154,176</summary>
        public const float FallingMarkRadius = 30f;
        public const float FallingMarkRadiusGrow = 60f;
        /// <summary>主落雷：三道，横向铺 500 px、纵向随机错开 40~300，指向落点下方 250 px。沿用旧值 AI.FallingThunder.cs:197-202</summary>
        public const int FallingBoltCount = 3;
        public const float FallingBoltSpreadX = 500f;
        public const float FallingBoltOffsetX = -250f;
        public const int FallingBoltOffsetYMin = 40;
        public const int FallingBoltOffsetYMax = 300;
        public const float FallingBoltAimBelow = 250f;
        public const int FallingBoltLifeGain = 8;
        public const int FallingBoltAi2 = 60;
        public static int FallingBoltDamage() => Helper.GetProjDamage(180, 200, 260);
        /// <summary>砸落与落地：起跳时压暗 0.25，落地改 0.4 并持续 40 帧。沿用旧值 AI.FallingThunder.cs:212,226</summary>
        public const float FallingSkyLightDash = 0.25f;
        public const float FallingSkyLightLand = 0.4f;
        public const int FallingSkyLandFade = 40;
        /// <summary>落地镜头冲击。沿用旧值 AI.FallingThunder.cs:233</summary>
        public const float FallingLandPunchY = 1.3f;
        public const int FallingLandShakeStrength = 20;
        public const float FallingLandShakeVibration = 22f;
        public const int FallingLandShakeFrames = 25;
        /// <summary>落地烟雾段与后摇。沿用旧值 AI.FallingThunder.cs:236,251</summary>
        public const int FallingLandSmokeFrames = 30;
        public const int FallingRecoverFrames = 20;
        public const int FallingSmokeInterval = 2;
        public const float FallingLandShadowScaleMax = 2.5f;

        #endregion

        #region 冲刺放电（远距离单招：贴上去 → 原地充电 → 五向放电）

        /// <summary>贴近冲刺的时长上限与"够近了"的判定。沿用旧值 AI.DashDischarging.cs:15,76</summary>
        public const int DischargeDashFrames = 60;
        public const float DischargeArriveDistance = 200f;
        /// <summary>贴近冲刺的 X 死区（超过才翻面，防止贴脸抖动）。沿用旧值 AI.DashDischarging.cs:69</summary>
        public const float DischargeFaceDeadZoneX = 100f;
        /// <summary>冲到位后的刹速。沿用旧值 AI.DashDischarging.cs:80</summary>
        public const float DischargeArriveDamp = 0.1f;
        /// <summary>充电段时长与身位微调参数。沿用旧值 AI.DashDischarging.cs:97-106</summary>
        public const int DischargeChargeFrames = (7 * 4) + 20;
        public const float DischargeHoldSpeedX = 7.3f;
        public const float DischargeHoldAccelX = 0.28f;
        public const float DischargeHoldTurnX = 0.6f;
        public const float DischargeHoldSpeedY = 5f;
        public const float DischargeHoldAccelY = 0.3f;
        public const float DischargeHoldTurnY = 0.5f;
        /// <summary>充电段抬头的帧步进（慢，给玩家读条时间）。沿用旧值 AI.DashDischarging.cs:115</summary>
        public const int DischargeWingFrameTime = 7;
        /// <summary>充电环：半径从 150 张到 540（除以 2 后是实际散布），每帧 4 组尘。沿用旧值 AI.DashDischarging.cs:108-111</summary>
        public const float DischargeRingMin = 150f;
        public const float DischargeRingMax = 540f;
        public const int DischargeDustPerFrame = 4;
        public const float DischargeDustSpeedMin = 4f;
        public const float DischargeDustSpeedMax = 8f;
        /// <summary>放电持续时长。沿用旧值 AI.DashDischarging.cs:16</summary>
        public const int DischargeBurstFrames = 60;
        /// <summary>起爆伤害与镜头冲击（32 强度、震 26、25 帧，是全 boss 第二重的一下）。沿用旧值 AI.DashDischarging.cs:130,143</summary>
        public static int DischargeBurstDamage() => Helper.GetProjDamage(180, 200, 245);
        public const float DischargePunchY = 1.4f;
        public const int DischargeShakeStrength = 32;
        public const float DischargeShakeVibration = 26f;
        public const int DischargeShakeFrames = 25;
        /// <summary>起爆背景压暗。沿用旧值 AI.DashDischarging.cs:150</summary>
        public const float DischargeSkyLight = 0.5f;
        /// <summary>放电期：整体刹速、残影脉动 3 次、帧图快速循环。沿用旧值 AI.DashDischarging.cs:162-172</summary>
        public const float DischargeBurstDamp = 0.985f;
        public const float DischargeShadowCycles = 3f;
        public const float DischargeShadowScaleMax = 2.5f;
        public const int DischargeBurstWingFrameTime = 1;
        /// <summary>放电期前 2/3 还能慢慢转向（0.01 rad/f），之后锁死。沿用旧值 AI.DashDischarging.cs:176-177</summary>
        public const int DischargeTrackNumerator = 2;
        public const int DischargeTrackDenominator = 3;
        public const float DischargeTrackRate = 0.01f;
        /// <summary>预警尘的基础臂长与末端伸出量。沿用旧值 AI.DashDischarging.cs:178,235</summary>
        public const float DischargeArmLength = 530f;
        public const float DischargeArmGrow = 600f;
        /// <summary>放电的路数（五向等分）与出膛几何：臂根 300 px、射线再向外 600 px。沿用旧值 AI.DashDischarging.cs:188-193</summary>
        public const int DischargeArmCount = 5;
        public const float DischargeArmRoot = 300f;
        public const float DischargeArmReach = 600f;
        public const int DischargeArmProjAi0 = 15;
        public const int DischargeArmProjAi2 = 70;
        public static int DischargeArmDamage() => Helper.GetProjDamage(160, 180, 200);
        /// <summary>后摇帧数。沿用旧值 AI.DashDischarging.cs:209</summary>
        public const int DischargeRecoverFrames = 25;

        #endregion

        #region 电流吐息共用：侧翼就位飞行、嘴部预警尘

        /// <summary>就位点在玩家左 / 右 400 px（<see cref="ZacurrentDragonContext.Recorder2"/> 决定哪一侧）。沿用旧值 AI.ElectricBreathSmall.cs:119</summary>
        public const float BreathSideOffset = 400f;
        /// <summary>就位飞行的 X 死区（超过 300 追、小于 100 退，中间衰减并加速走完）。沿用旧值 AI.ElectricBreathSmall.cs:130-138</summary>
        public const float BreathFlyFarX = 300f;
        public const float BreathFlyNearX = 100f;
        public const float BreathFlySpeedX = 24f;
        public const float BreathFlyAccelX = 0.4f;
        public const float BreathFlyTurnX = 0.6f;
        public const float BreathFlyDampX = 0.92f;
        /// <summary>X 已经对上时每帧多记 20 帧：就位段不该为了对齐一个像素磨蹭。沿用旧值 AI.ElectricBreathSmall.cs:137</summary>
        public const int BreathCloseTimeBonus = 20;
        /// <summary>就位飞行的 Y 参数。沿用旧值 AI.ElectricBreathSmall.cs:140-150</summary>
        public const float BreathFlyDeadZoneY = 50f;
        public const float BreathFlyRiseAccel = 0.9f;
        public const float BreathFlyRiseMax = 20f;
        public const float BreathFlyRiseDamp = 0.85f;
        public const float BreathFlySpeedY = 16f;
        public const float BreathFlyAccelY = 0.45f;
        public const float BreathFlyTurnY = 1f;
        public const float BreathFlyDampY = 0.9f;
        /// <summary>就位期的瞄准插值速率（把吐息角慢慢转向玩家）。沿用旧值 AI.ElectricBreathSmall.cs:117</summary>
        public const float BreathAimRate = 0.08f;
        /// <summary>进入这个距离才张嘴 + 铺预警尘（远了不预告，避免满屏乱飘）。沿用旧值 AI.ElectricBreathSmall.cs:153</summary>
        public const float BreathMouthDustDistance = 750f;
        /// <summary>落点指示粒子：每 2 帧 1/3 概率一颗，散布半径 28。沿用旧值 AI.ElectricBreathSmall.cs:156-157</summary>
        public const int BreathIndicatorInterval = 2;
        public const int BreathIndicatorChance = 3;
        public const float BreathIndicatorRadius = 28f;
        /// <summary>吐息落点在玩家身后的延伸量。沿用旧值 AI.ElectricBreathSmall.cs:84</summary>
        public const float BreathLeadDistance = 200f;
        /// <summary>就位超时后玩家还在这个距离外就直接放弃整招（追不上就别演）。沿用旧值 AI.ElectricBreathSmall.cs:173</summary>
        public const float BreathGiveUpDistance = 1400f;
        /// <summary>嘴部预警尘的铺设范围与抖动。沿用旧值 AI.ElectricBreathSmall.cs:199-209</summary>
        public const float BreathDustRangeMin = 20f;
        public const float BreathDustRangeMax = 150f;
        public const float BreathDustTargetSpread = 32f;
        /// <summary>吐息出膛的后坐速度。沿用旧值 AI.ElectricBreathSmall.cs:88</summary>
        public const float BreathRecoilSpeed = 8f;
        /// <summary>吐息弹幕的第三个 ai 参数（射线宽度档）。沿用旧值 AI.ElectricBreathSmall.cs:85</summary>
        public const int BreathProjAi2 = 70;

        #endregion

        #region 电流吐息（小：两轮左右交替的短吐息）

        /// <summary>单次吐息时长。沿用旧值 AI.ElectricBreathSmall.cs:15</summary>
        public const int BreathSmallTime = 30;
        /// <summary>就位段上限 6 秒，期间沿一条正弦弧线绕上去（弧高 200 px）。沿用旧值 AI.ElectricBreathSmall.cs:23,25</summary>
        public const int BreathSmallFlyFrames = 60 * 6;
        public const float BreathSmallArcHeight = 200f;
        /// <summary>吐息前的准备时长（难度越高越短 = 预告越窄）。沿用旧值 AI.ElectricBreathSmall.cs:47</summary>
        public static int BreathSmallReadyFrames() => Helper.ScaleValueForDiffMode(35, 33, 29, 26);
        /// <summary>吐息期的瞄准跟踪速率，慢到几乎不动：吐出去就定了。沿用旧值 AI.ElectricBreathSmall.cs:31</summary>
        public const float BreathSmallTrackRate = 0.005f;
        /// <summary>准备期的雷电粒子：每 5 帧一条，散角 ±0.4，速度 12~18，外形 (9,15,9,20)。沿用旧值 AI.ElectricBreathSmall.cs:58-61</summary>
        public const int BreathSmallSparkInterval = 5;
        public const float BreathSparkSpread = 0.4f;
        public const float BreathSmallSparkSpeedMin = 12f;
        public const float BreathSmallSparkSpeedMax = 18f;
        public const int BreathSmallSparkMaxTime = 9;
        public const int BreathSmallSparkFadeTime = 15;
        public const int BreathSmallSparkPointCount = 9;
        public const float BreathSmallSparkWidth = 20f;
        /// <summary>出膛镜头冲击与背景压暗。沿用旧值 AI.ElectricBreathSmall.cs:72-74</summary>
        public const int BreathSmallShakeStrength = 20;
        public const float BreathSmallShakeVibration = 20f;
        public const float BreathSmallSkyLight = 0.25f;
        /// <summary>小吐息伤害。沿用旧值 AI.ElectricBreathSmall.cs:82</summary>
        public static int BreathSmallDamage() => Helper.GetProjDamage(110, 130, 150);
        /// <summary>吐息中的刹速与吐完的收尾余量。沿用旧值 AI.ElectricBreathSmall.cs:90-92</summary>
        public const float BreathSmallHoldDamp = 0.95f;
        public const int BreathSmallTailFrames = 15;
        /// <summary>吐息期的距离带衰减（比通用巡航更黏一点）。沿用旧值 AI.ElectricBreathSmall.cs:45</summary>
        public const float BreathSmallBandDamp = 0.9f;

        #endregion

        #region 电流吐息（中：绕飞一圈 → 长蓄力 → 一发粗吐息 + 电球）

        /// <summary>单次吐息时长（小吐息的两倍）。沿用旧值 AI.ElectricBreathMiddle.cs:17</summary>
        public const int BreathMiddleTime = 60;
        /// <summary>就位段上限 7 秒，固定飞到玩家上方 250 px。沿用旧值 AI.ElectricBreathMiddle.cs:23,25</summary>
        public const int BreathMiddleFlyFrames = 60 * 7;
        public const float BreathMiddleHoverY = -250f;
        /// <summary>绕飞：45 帧、每帧转 1/50 圈、起手切向速度 28。沿用旧值 AI.ElectricBreathMiddle.cs:29-40</summary>
        public const int BreathMiddleRollFrames = 45;
        public const float BreathMiddleRollDivisor = 50f;
        public const float BreathMiddleRollSpeed = 28f;
        /// <summary>绕飞结束的刹速。沿用旧值 AI.ElectricBreathMiddle.cs:56</summary>
        public const float BreathMiddleRollExitDamp = 0.25f;
        /// <summary>蓄力段的身位微调参数。沿用旧值 AI.ElectricBreathMiddle.cs:77-87</summary>
        public const float BreathMiddleHoldFarX = 100f;
        public const float BreathMiddleHoldNearX = 20f;
        public const float BreathMiddleHoldSpeedX = 15f;
        public const float BreathMiddleHoldAccelX = 0.4f;
        public const float BreathMiddleHoldTurnX = 0.6f;
        public const float BreathMiddleHoldSpeedY = 6f;
        public const float BreathMiddleHoldAccelY = 0.25f;
        public const float BreathMiddleHoldTurnY = 1f;
        /// <summary>蓄力时长。沿用旧值 AI.ElectricBreathMiddle.cs:94</summary>
        public const int BreathMiddleChargeFrames = 35;
        /// <summary>
        /// 蓄力期的瞄准：把射线落点朝玩家每帧推进 13 px，转速随蓄力推进衰减到 0
        /// ——越接近出手越锁死，玩家在最后半秒可以确定地走出去。沿用旧值 AI.ElectricBreathMiddle.cs:98-99
        /// </summary>
        public const float BreathMiddleAimStep = 13f;
        public const float BreathMiddleAimRate = 0.03f;
        /// <summary>蓄力期粒子节拍：每 3 帧一次（嘴部尘 + 一条雷弧，1/3 概率再补一条从地面窜起的长雷）。沿用旧值 AI.ElectricBreathMiddle.cs:102-126</summary>
        public const int BreathMiddleSparkInterval = 3;
        public const float BreathMiddleSparkSpeedMin = 25f;
        public const float BreathMiddleSparkSpeedMax = 35f;
        public const int BreathMiddleSparkMaxTime = 6;
        public const int BreathMiddleSparkFadeTime = 10;
        public const int BreathMiddleSparkPointCount = 7;
        public const float BreathMiddleSparkWidth = 50f;
        public const int BreathMiddleGroundBoltChance = 3;
        public const float BreathMiddleGroundBoltSpeedMin = 45f;
        public const float BreathMiddleGroundBoltSpeedMax = 60f;
        public const float BreathMiddleGroundBoltSpread = 0.5f;
        public const float BreathMiddleGroundBoltDistance = 12f;
        public const int BreathMiddleGroundBoltMaxTime = 12;
        public const int BreathMiddleGroundBoltFadeTime = 5;
        public const int BreathMiddleGroundBoltPointCount = 25;
        public const float BreathMiddleGroundBoltWidth = 70f;
        /// <summary>蓄力期抬头的帧上限（抬到第 4 帧就停）。沿用旧值 AI.ElectricBreathMiddle.cs:131</summary>
        public const int BreathMiddleAimFrameY = 4;
        /// <summary>出膛镜头冲击（75 是全 boss 最强的一下）与背景压暗。沿用旧值 AI.ElectricBreathMiddle.cs:144-146</summary>
        public const int BreathMiddleShakeStrength = 75;
        public const float BreathMiddleShakeVibration = 25f;
        /// <summary>中吐息伤害与落点延伸（比小吐息更远）。沿用旧值 AI.ElectricBreathMiddle.cs:154,156</summary>
        public static int BreathMiddleDamage() => Helper.GetProjDamage(180, 200, 240);
        public const float BreathMiddleLeadDistance = 350f;
        /// <summary>伴随的滚动电球的第三个 ai 参数。沿用旧值 AI.ElectricBreathMiddle.cs:159</summary>
        public const int BreathMiddleBallAi2 = 35;
        /// <summary>吐息收尾余量与后摇（后摇可被连段覆盖）。沿用旧值 AI.ElectricBreathMiddle.cs:168,171</summary>
        public const int BreathMiddleTailFrames = 10;
        public static int BreathMiddleRestFrames() => Helper.ScaleValueForDiffMode(40, 35, 30, 20);

        #endregion

        #region 电磁炮（连段件：绕半圈就位 → 锁定 → 一道 2.5 秒的定向长射线）

        /// <summary>射线持续时长。沿用旧值 AI.ElectromagneticCannon.cs:13</summary>
        public const int CannonBurstFrames = 150;
        /// <summary>就位段：飞到玩家上方 300 px，超时 4 秒。沿用旧值 AI.ElectromagneticCannon.cs:20,22</summary>
        public const int CannonChaseTimeout = 60 * 4;
        public const float CannonHoverAbove = 300f;
        /// <summary>就位段的运动参数（比其它招快得多，因为要抢在玩家换位前站好）。沿用旧值 AI.ElectromagneticCannon.cs:31-40</summary>
        public const float CannonChaseDeadZoneX = 400f;
        public const float CannonChaseSpeedX = 24f;
        public const float CannonChaseAccelX = 0.6f;
        public const float CannonChaseTurnX = 0.8f;
        public const float CannonChaseSpeedY = 18f;
        public const float CannonChaseAccelY = 0.4f;
        public const float CannonChaseTurnY = 0.8f;
        public const float CannonRiseAccel = 0.55f;
        public const float CannonRiseMax = 20f;
        /// <summary>高度已经对上时每帧多记 2 帧，提前结束就位。沿用旧值 AI.ElectromagneticCannon.cs:46</summary>
        public const int CannonCloseTimeBonus = 2;
        /// <summary>到位判定的 Y 容差。沿用旧值 AI.ElectromagneticCannon.cs:51</summary>
        public const float CannonArriveY = 100f;
        /// <summary>绕飞起手速度（垂直于连线切出去）与残影缩放。沿用旧值 AI.ElectromagneticCannon.cs:58,63</summary>
        public const float CannonRollLaunchSpeed = 20f;
        public const float CannonRollShadowScale = 1.15f;
        /// <summary>绕飞时长与每帧转过的圈数分母（45 帧转 3/4 圈）。沿用旧值 AI.ElectromagneticCannon.cs:70-71</summary>
        public const int CannonRollFrames = 45;
        public const float CannonRollDivisor = 60f;
        /// <summary>瞄准段：刹速、锁定窗口（前 10 帧还跟手，之后角度锁死 = 预告即承诺）、抬头到位后再数 35 帧。沿用旧值 AI.ElectromagneticCannon.cs:88,91,116</summary>
        public const float CannonAimDamp = 0.9f;
        public const int CannonAimLockFrames = 10;
        public const int CannonAimFrames = 35;
        /// <summary>炮口锚点（相对本体朝向偏 0.25 弧度、前移 60 px）与预警尘的铺设范围。沿用旧值 AI.ElectromagneticCannon.cs:100-106</summary>
        public const float CannonMuzzleAngleOffset = 0.25f;
        public const float CannonMuzzleDistance = 60f;
        public const int CannonDustPerFrame = 3;
        public const float CannonDustRangeMin = 20f;
        public const float CannonDustRangeMax = 1220f;
        public const float CannonDustJitter = 0.3f;
        public const float CannonDustSpeedMin = 2f;
        public const float CannonDustSpeedMax = 6f;
        /// <summary>射线伤害与出膛参数：从 1800 px 外朝嘴部生成（弹幕自己向外延伸）。沿用旧值 AI.ElectromagneticCannon.cs:126-129</summary>
        public static int CannonDamage() => Helper.GetProjDamage(100, 130, 150);
        public const float CannonSpawnLead = 1800f;
        public const int CannonProjAi2 = 85;
        /// <summary>出膛镜头冲击。沿用旧值 AI.ElectromagneticCannon.cs:135</summary>
        public const float CannonPunchDirScale = 2f;
        public const int CannonShakeStrength = 24;
        public const float CannonShakeVibration = 20f;
        public const int CannonShakeFrames = 20;
        /// <summary>出膛背景压暗：亮度 0.6、渐变覆盖射线时长的 3/4。沿用旧值 AI.ElectromagneticCannon.cs:140</summary>
        public const float CannonSkyLight = 0.6f;
        public const int CannonSkyFadeNumerator = 3;
        public const int CannonSkyFadeDenominator = 4;
        public const int CannonSkyExchange = 14;
        /// <summary>射线期：朝向取射线远端 2000 px 处（死区放宽到 80，防止射线扫过头顶时疯狂翻面）。沿用旧值 AI.ElectromagneticCannon.cs:151</summary>
        public const float CannonFaceLead = 2000f;
        public const float CannonFaceDeadZone = 80f;
        /// <summary>射线扫动角速度：0.017 rad/f ≈ 每秒 58°，慢到玩家能跑出去，这是这一招唯一的逃生道。沿用旧值 AI.ElectromagneticCannon.cs:154</summary>
        public const float CannonTrackRate = 0.017f;
        /// <summary>射线期残影膨胀上限与持续震屏（每 20 帧一下）。沿用旧值 AI.ElectromagneticCannon.cs:157,160-163</summary>
        public const float CannonBurstShadowScale = 2f;
        public const int CannonBurstShakeInterval = 20;
        public const int CannonBurstShakeStrength = 7;
        public const float CannonBurstShakeVibration = 12f;
        /// <summary>后摇帧数。沿用旧值 AI.ElectromagneticCannon.cs:183</summary>
        public const int CannonRecoverFrames = 25;

        #endregion

        #region 聚集电流（连段收尾：铺满全场的电球田，同时自己短暂无敌）

        /// <summary>撒球窗口长度与整段长度。沿用旧值 AI.GatherCurrent.cs:57-58</summary>
        public const int GatherSpawnFrames = 24 * 10;
        public const int GatherMaxFrames = (60 * 5) + (24 * 10);
        /// <summary>撒球期自身无敌，直到撒球结束前 60 帧解除——这 60 帧是留给玩家的反击提示。沿用旧值 AI.GatherCurrent.cs:86</summary>
        public const int GatherInvulEndLead = 60;
        /// <summary>残影的脉动：整段里循环 20 次，每次从 1 涨到 2.5 同时淡出。沿用旧值 AI.GatherCurrent.cs:60-62</summary>
        public const float GatherShadowCycles = 20f;
        public const float GatherShadowScaleMax = 2.5f;
        /// <summary>零散内收尘：每 3 帧一粒，半径从 800 收到 200。沿用旧值 AI.GatherCurrent.cs:65-67</summary>
        public const int GatherDustInterval = 3;
        public const float GatherDustRadius = 800f;
        public const float GatherDustRadiusShrink = 600f;
        public const int GatherDustJitter = 80;
        public const float GatherDustSpeedMin = 4f;
        public const float GatherDustSpeedMax = 8f;
        public const float GatherDustScaleMin = 1.5f;
        public const float GatherDustScaleMax = 2f;
        /// <summary>整圈内收尘：每 30 帧一圈 70 粒，半径从 800 收到 100。沿用旧值 AI.GatherCurrent.cs:74-83</summary>
        public const int GatherRingInterval = 30;
        public const float GatherRingRadiusShrink = 700f;
        public const int GatherRingDustCount = 70;
        public const float GatherRingDustSpeedMin = 2f;
        public const float GatherRingDustSpeedMax = 5f;
        public const float GatherRingDustScaleMin = 1f;
        public const float GatherRingDustScaleMax = 1.5f;
        /// <summary>紫电球：每 10 帧一个，落点角每次推进 1/6 圈 + 0.15 弧度（错开成螺旋），半径 800~900。沿用旧值 AI.GatherCurrent.cs:93-101</summary>
        public const int GatherVoltBallInterval = 10;
        public const float GatherVoltBallAngleStep = (MathHelper.TwoPi / 6) + 0.15f;
        public const int GatherVoltBallRadiusMin = 800;
        public const int GatherVoltBallRadiusMax = 900;
        /// <summary>红电球：每 40 帧一个（紫电球的 4 倍间隔），角步进 1/3 圈 + 0.3，半径 550~800。沿用旧值 AI.GatherCurrent.cs:104-113</summary>
        public const int GatherRedBallIntervalMul = 4;
        public const float GatherRedBallAngleStep = (MathHelper.TwoPi / 3) + 0.3f;
        public const int GatherRedBallRadiusMin = 550;
        public const int GatherRedBallRadiusMax = 800;
        /// <summary>红电球伤害。沿用旧值 AI.GatherCurrent.cs:112</summary>
        public static int GatherRedBallDamage() => Helper.GetProjDamage(200, 250, 300);
        /// <summary>后摇帧数。沿用旧值 AI.GatherCurrent.cs:134</summary>
        public const int GatherRecoverFrames = 30;

        #endregion

        #region 特殊态：登场 / 死亡 / 击穿恢复 / 紫伏形态切换

        /// <summary>hub 兜底驻留时的刹车，取击穿恢复期同值。沿用旧值 ZcurrentAI.cs:309</summary>
        public const float IdleDamp = 0.9f;

        /// <summary>登场落点：玩家正上方 1500 px，够高才有“从云层砸下来”的观感。沿用旧值 ZcurrentAI.cs:537</summary>
        public const float SpawnHeightAboveTarget = 1500f;

        /// <summary>死亡演出：匀速上浮，残影在 60 帧内淡入，总长 120 帧后炸开并真正 Kill。沿用旧值 ZcurrentAI.cs:242,245,256</summary>
        public const float KillAnimRiseSpeed = -2f;
        public const float KillAnimFadeFrames = 60f;
        public const int KillAnimFrames = 120;
        /// <summary>死亡演出雷电粒子：每 4 帧一条，速度 45~60。沿用旧值 ZcurrentAI.cs:247,249</summary>
        public const int KillAnimThunderInterval = 4;
        public const float KillAnimThunderSpeedMin = 45f;
        public const float KillAnimThunderSpeedMax = 60f;
        /// <summary>雷电粒子外形参数（寿命 / 淡出 / 折点数 / 宽度），对应 PurpleThunderParticle.Spawn。沿用旧值 ZcurrentAI.cs:252</summary>
        public const int ThunderParticleMaxTime = 14;
        public const int ThunderParticleFadeTime = 7;
        public const int ThunderParticlePointCount = 7;
        public const float ThunderParticleWidth = 70f;

        /// <summary>爆开电环：30 圈、每圈 4 颗，半径 80→600。沿用旧值 ZcurrentAI.cs:261-270 / 293-302</summary>
        public const int BurstRingCount = 30;
        public const int BurstPerRing = 4;
        public const float BurstRadiusMin = 80f;
        public const float BurstRadiusMax = 600f;
        public const float BurstParticleScaleMin = 0.9f;
        public const float BurstParticleScaleMax = 1.3f;

        /// <summary>击穿恢复期长度（难度越高越短），头顶转圈星星挂同样时长。沿用旧值 ZcurrentAI.cs:283</summary>
        public static int BreakFrames() => Helper.ScaleValueForDiffMode(60 * 8, 60 * 6, 60 * 5, 60);
        /// <summary>转圈星星的半径与它绕的锚点上移量。沿用旧值 ZcurrentAI.cs:286-288</summary>
        public const float BreakStarLength = 10f;
        public const float BreakStarAnchorUp = -50f;
        /// <summary>两颗星星的起始相位（正负 1.57 ≈ ±90°，一左一右）。沿用旧值 ZcurrentAI.cs:287-288</summary>
        public const float BreakStarRotation = 1.57f;

        /// <summary>紫伏切换：收翅两段蓄力 17 + 9 帧，然后吼叫；电环 40 帧铺满、吼叫 60 帧、收尾再留 25 帧。沿用旧值 AI.PurpleVoltChange.cs:14-15,39-40,110</summary>
        public const float ExchangeFirstChargeTime = 17f;
        public const float ExchangeSecondChargeTime = 9f;
        public const int ExchangeBurstTime = 40;
        public const int ExchangeRoaringTime = 60;
        public const int ExchangeEndDelay = 25;
        /// <summary>收翅阶段的刹车与残影缩放曲线（两段各自从大缩到 1，制造两次“吸气”）。沿用旧值 AI.PurpleVoltChange.cs:22,45-52</summary>
        public const float ExchangeDamp = 0.8f;
        public const float ExchangeShadowScaleFirst = 2.5f;
        public const float ExchangeShadowScaleSecond = 1.5f;
        /// <summary>吼叫起手的残影尺寸，以及吼叫期残影从 1 膨胀到 2.5 同时淡出。沿用旧值 AI.PurpleVoltChange.cs:63,96-97</summary>
        public const float ExchangeRoarShadowScale = 1.2f;
        public const float ExchangeRoarShadowScaleMax = 2.5f;
        /// <summary>吼叫期电环：40 帧内半径 80→1400，每帧 5 颗。沿用旧值 AI.PurpleVoltChange.cs:84-86</summary>
        public const float ExchangeBurstRadiusMin = 80f;
        public const float ExchangeBurstRadiusMax = 1400f;
        public const int ExchangeBurstPerFrame = 5;
        /// <summary>吼叫波纹：每 10 帧一圈冲击波、每 20 帧一道直线。沿用旧值 AI.PurpleVoltChange.cs:99-105</summary>
        public const int RoaringWaveInterval = 10;
        public const int RoaringLineInterval = 20;
        public const float RoaringWaveScale = 0.2f;
        public const float RoaringWaveScaleMul = 1.15f;

        #endregion
    }
}
