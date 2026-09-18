using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon.Core
{
    /// <summary>
    /// 冰龙宝宝战斗调参中心：全部数字与阶段档位的唯一出口。<br/>
    /// 本轮是结构迁移，战斗设计不变：每条常量注明来源，查不到设计理由的一律写“沿用旧值 &lt;文件&gt;:行号”（迁移前的行号）。
    /// 招式集合、循环与阶段阈值的设计口径见同目录 <c>BabyIceDragon_AI_Description.md</c>。<br/>
    /// 计时口径：旧招式体在体末 <c>Timer++</c>，换拍后新拍首帧读 1，与基座“OnUpdate 开头自增”一致，拍点数字原样搬运；
    /// 只有出场动画从旧 0 起算，状态里用 <c>Timer - 1</c> 对齐。
    /// </summary>
    internal static class BabyIceDragonDirector
    {
        //==================== 出生 / 脱战 / 目标 ====================

        /// <summary>残影缓存长度：绘制取 7/5/3/1 四帧。沿用旧值 BabyIceDragon.cs:292,1060</summary>
        public const int ShadowCacheLength = 8;
        /// <summary>接触判定盒 86 × 58（比贴图小一圈，给玩家留擦身余量）。沿用旧值 BabyIceDragon.cs:247-248</summary>
        public const int HitboxWidth = 86;
        public const int HitboxHeight = 58;
        /// <summary>血量低于 1/4 时被暴击掉冰鳞，一场最多 8 片。沿用旧值 BabyIceDragon.cs:202</summary>
        public const float DropScaleLifeRatio = 0.25f;
        public const int DropScaleMax = 8;
        /// <summary>出生帧图 Y = 3（合翅）。沿用旧值 BabyIceDragon.cs:296</summary>
        public const int SpawnFrameY = 3;
        /// <summary>目标离开 3000 px 视为脱战。沿用旧值 BabyIceDragon.cs:314</summary>
        public const float DespawnDistance = 3000f;
        /// <summary>目标离开雪原超过 6 秒后脱战（在雪原时计时归零）。沿用旧值 BabyIceDragon.cs:314</summary>
        public const int FlyAwayFrames = 60 * 6;
        /// <summary>脱战：朝向回正 0.14、X 衰减 0.98、扇翅上飞、鼓励消失 30。沿用旧值 BabyIceDragon.cs:321-325</summary>
        public const float DespawnRotationStep = 0.14f;
        public const float DespawnDampX = 0.98f;
        public const int DespawnEncourageFrames = 30;

        /// <summary>二阶段阈值：大师模式血量低于 3/4，其余低于 1/2（设计文档“招式循环”）。BabyIceDragon.cs:853-856</summary>
        public static float Phase2LifeRatio() => Main.masterMode ? 0.75f : 0.5f;
        /// <summary>普通招式计数超过 6（大师）/ 4（其余）后进行一次有破绽动作（设计文档“招式循环 b”）。BabyIceDragon.cs:999-1014</summary>
        public static int VulnerableAfterMoves() => Main.masterMode ? 6 : 4;
        /// <summary>有破绽动作二选一：俯冲 / 蓄力冰球。沿用旧值 BabyIceDragon.cs:875-879</summary>
        public const int VulnerableMoveCount = 2;
        /// <summary>招池枯竭时的兜底招：冰吐息。沿用旧值 BabyIceDragon.cs:895</summary>
        public const BabyIceDragonStateId FallbackMove = BabyIceDragonStateId.iceBreath;
        /// <summary>
        /// hub 停留帧数。旧代码收招当帧即 <c>ResetStates</c> 切下一招、零间隔（招式间的喘息由 rest / dizzy 两个显式状态承担），
        /// 为不改节奏取 0：<see cref="States.BabyIceDragonHubState.EndAttack"/> 在 0 时直接经 Commit 返回下一招，不多占一帧。<br/>
        /// 用 <c>static readonly</c> 而非 <c>const</c>：为 0 的 const 会把 <c>EndAttack</c> 的喘息分支折成编译期死代码（CS0162），
        /// 把这个调参口焊死；这里保住它，改一个数就能让 hub 真正驻留。
        /// </summary>
        public static readonly int HubFrames = 0;
        /// <summary>查重窗口长度（记账用，本轮不裁决）：15 个状态取 1/3 ≈ 5。</summary>
        public const int RecentPickWindow = 5;
        /// <summary>所有状态的超时兜底：最长的招（蓄力冰球 900 帧 + 就位 400 帧）也到不了 1500，仍没退就是软锁。</summary>
        public const int StateTimeoutFrames = 1500;

        /// <summary>
        /// 一阶段招池：4 冰吐息 / 2 龙车 / 4 下砸，用过即移除。沿用旧值 BabyIceDragon.cs:960-967
        /// </summary>
        public static readonly BabyIceDragonStateId[] Phase1Pool =
        {
            BabyIceDragonStateId.iceBreath, BabyIceDragonStateId.iceBreath, BabyIceDragonStateId.iceBreath, BabyIceDragonStateId.iceBreath,
            BabyIceDragonStateId.horizontalDash, BabyIceDragonStateId.horizontalDash,
            BabyIceDragonStateId.smashDown, BabyIceDragonStateId.smashDown, BabyIceDragonStateId.smashDown, BabyIceDragonStateId.smashDown,
        };

        /// <summary>二阶段大师招池：普通三招各 1，冰刺陷阱 2，龙车变种 2，冰雹 1，加两个大师专属招各 1。沿用旧值 BabyIceDragon.cs:969-981</summary>
        public static readonly BabyIceDragonStateId[] Phase2MasterPool =
        {
            BabyIceDragonStateId.iceBreath, BabyIceDragonStateId.horizontalDash, BabyIceDragonStateId.smashDown,
            BabyIceDragonStateId.iceThornsTrap, BabyIceDragonStateId.iceThornsTrap,
            BabyIceDragonStateId.doubleDash, BabyIceDragonStateId.doubleDash,
            BabyIceDragonStateId.iceCloud, BabyIceDragonStateId.iciclesFall, BabyIceDragonStateId.iceTornado,
        };

        /// <summary>二阶段普通招池：2 冰吐息 / 1 龙车 / 2 下砸 / 2 冰刺陷阱 / 2 龙车变种 / 1 冰雹。沿用旧值 BabyIceDragon.cs:984-993</summary>
        public static readonly BabyIceDragonStateId[] Phase2NormalPool =
        {
            BabyIceDragonStateId.iceBreath, BabyIceDragonStateId.iceBreath,
            BabyIceDragonStateId.horizontalDash,
            BabyIceDragonStateId.smashDown, BabyIceDragonStateId.smashDown,
            BabyIceDragonStateId.iceThornsTrap, BabyIceDragonStateId.iceThornsTrap,
            BabyIceDragonStateId.doubleDash, BabyIceDragonStateId.doubleDash,
            BabyIceDragonStateId.iceCloud,
        };

        //==================== 运动 / 朝向 / 帧图（宿主 ApplyDeclaredMovement 消费）====================

        /// <summary>Hold 模式（漏声明兜底）的速度衰减；正常迁移后不会命中。</summary>
        public const float HoldDamp = 0.9f;
        /// <summary>飞行帧：每 7 帧翻一页（frameCounter &gt; 6），Y 帧 0..3 循环。沿用旧值 BabyIceDragon.cs:1028-1034</summary>
        public const int FlyingFrameTicks = 6;
        public const int FlyingFrameMaxY = 3;
        /// <summary>扇翅帧区间 [2, <see cref="FlyingFrameMaxY"/>]：只有这两帧给向上加速度，其余帧减速，手感全在这里。沿用旧值 BabyIceDragon.cs:691-694</summary>
        public const int FlapFrameYStart = 2;
        /// <summary>飞行姿态倾角 = 面向 × Y 速度 × 0.05。沿用旧值 BabyIceDragon.cs:1038</summary>
        public const float TiltPerSpeedY = 0.05f;
        /// <summary>眩晕帧：每 9 帧翻一页（frameCounter &gt; 8），落地才翻页；悬空用第 4 帧。沿用旧值 BabyIceDragon.cs:553-566</summary>
        public const int DizzyFrameTicks = 8;
        public const int DizzyFrameAirY = 4;
        public const int DizzyFrameX = 2;
        /// <summary>扇翅上飞：帧 2 / 3 时向上加速 0.7，否则 Y 衰减 0.94，上限 -8。沿用旧值 BabyIceDragon.cs:686-698</summary>
        public const float FlyUpAccel = 0.7f;
        public const float FlyUpDamp = 0.94f;
        public const float FlyUpLimit = -8f;
        /// <summary>面向死区：与目标 X 距离小于 16 px 不改面向。沿用旧值 BabyIceDragon.cs:711</summary>
        public const float FacingDeadZone = 16f;
        /// <summary>嘴部位置 = 中心 + 面向 × 40 × 缩放。沿用旧值 BabyIceDragon.cs:706</summary>
        public const float MouthOffset = 40f;
        /// <summary>直角：旧代码一律用 1.57 近似 π/2（俯冲探针展开方向、绕冰球的切向），原样沿用以免轨迹漂移。沿用旧值 AI.Dive.cs:73, BabyIceDragon.cs:654</summary>
        public const float QuarterTurn = 1.57f;
        /// <summary>眩晕星星锚点：悬空帧 (30, -24)、其余 (30, -30)。沿用旧值 BabyIceDragon.cs:720-723</summary>
        public const float DizzyStarOffsetX = 30f;
        public const float DizzyStarOffsetYAir = -24f;
        public const float DizzyStarOffsetY = -30f;
        /// <summary>吼叫震屏：方向 (0.8, 0.8)、强度 5、6f=20、40 帧、1000 距离。沿用旧值 BabyIceDragon.cs:448</summary>
        public const float RoarShakeDir = 0.8f;
        public const float RoarShakeStrength = 5f;
        public const float RoarShakeVibration = 20f;
        public const int RoarShakeFrames = 40;
        public const float ShakeFalloffDistance = 1000f;
        /// <summary>吼叫粒子：波纹每 10 帧、线条每 20 帧，缩放 0.1。沿用旧值 BabyIceDragon.cs:458-461</summary>
        public const int RoarWaveInterval = 10;
        public const int RoarLineInterval = 20;
        public const float RoarParticleScale = 0.1f;
        /// <summary>蓄力冰星：4 颗从 100 px 圆周以 3 px/f 飞向嘴前 30 px，限速 16；反向光环缩放 0.8 / 1.2。沿用旧值 BabyIceDragon.cs:624-631</summary>
        public const int ChargeStarCount = 4;
        public const float ChargeStarRadius = 100f;
        public const float ChargeStarSpeed = 3f;
        public const float ChargeStarMouthOffset = 30f;
        public const float ChargeStarSpeedLimit = 16f;
        public const float ChargeHaloScaleSmall = 0.8f;
        public const float ChargeHaloScaleBig = 1.2f;
        /// <summary>残影颜色 (43, 255, 198) × 0.5，逐层 × 0.75、缩放 × 0.98。沿用旧值 BabyIceDragon.cs:1058-1064</summary>
        public static readonly Color ShadowColor = new(43, 255, 198, 255);
        public const float ShadowAlpha = 0.5f;
        public const float ShadowColorFalloff = 0.75f;
        public const float ShadowScaleFalloff = 0.98f;
        /// <summary>辉光可见阈值。沿用旧值 BabyIceDragon.cs:1069</summary>
        public const float GlowVisibleAlpha = 0.1f;

        //==================== 出场动画 onSpawnAnim（旧计时从 0 起）====================

        /// <summary>第 0 帧：生成名牌弹幕、辉光满、以 -0.2 上浮。沿用旧值 BabyIceDragon.cs:400-409</summary>
        public const float SpawnAnimRiseSpeed = -0.2f;
        /// <summary>前 30 帧撒风暴尘（散布 72、速度 2~4、缩放 1~1.5）。沿用旧值 BabyIceDragon.cs:413-418</summary>
        public const int SpawnAnimDustFrames = 30;
        public const float SpawnAnimDustSpread = 72f;
        public const float SpawnAnimDustSpeedMin = 2f;
        public const float SpawnAnimDustSpeedMax = 4f;
        public const float SpawnAnimDustScaleMin = 1f;
        public const float SpawnAnimDustScaleMax = 1.5f;
        /// <summary>100 帧前扇翅飘浮；100 帧合翅停住；130 帧吼叫；170 帧起上飞；260 帧进首招。沿用旧值 BabyIceDragon.cs:423-483</summary>
        public const int SpawnAnimFlyFrames = 100;
        public const int SpawnAnimRoarFrame = 130;
        public const int SpawnAnimRoarEnd = 170;
        public const int SpawnAnimEndFrame = 260;
        /// <summary>上飞段：X 衰减 0.99；扇翅加速 0.3、否则 0.96 衰减、上限 -12。沿用旧值 BabyIceDragon.cs:466-479</summary>
        public const float SpawnAnimDampX = 0.99f;
        public const float SpawnAnimFlapAccel = 0.3f;
        public const float SpawnAnimFlapDamp = 0.96f;
        public const float SpawnAnimFlapLimit = -12f;

        //==================== 吼叫动画 roaringAnim ====================

        /// <summary>全程 0.98 衰减；20 帧合翅停住；40 帧吼叫；80 帧收招。沿用旧值 BabyIceDragon.cs:499-539</summary>
        public const float RoarAnimDamp = 0.98f;
        public const int RoarAnimStopFrame = 20;
        public const int RoarAnimRoarFrame = 40;
        public const int RoarAnimEndFrame = 80;

        //==================== 死亡动画 onKillAnim ====================

        /// <summary>60 帧内以 1 px/f 上浮、辉光线性亮到满，撒风暴尘（散布 100）；之后爆散并真正死亡。沿用旧值 BabyIceDragon.cs:356-388</summary>
        public const int KillAnimRiseFrames = 60;
        public const float KillAnimRiseSpeed = 1f;
        public const float KillAnimDustSpread = 100f;
        /// <summary>爆散：12 颗冰星飞向 2000 px 外、20 粒碎冰（速度 2~6、缩放 1~1.4）、3 个冰爆光环 0.15。沿用旧值 BabyIceDragon.cs:370-383</summary>
        public const int KillAnimStarCount = 12;
        public const float KillAnimStarFarRadius = 2000f;
        public const float KillAnimStarSpawnRadius = 100f;
        public const float KillAnimStarSpeed = 4f;
        public const float KillAnimStarSpeedLimit = 16f;
        public const int KillAnimIceDustCount = 20;
        public const float KillAnimIceDustSpread = 32f;
        public const float KillAnimIceDustAngle = 1.7f;
        public const int KillAnimIceDustSpeedMin = 2;
        public const int KillAnimIceDustSpeedMax = 7;
        public const float KillAnimIceDustScaleMin = 1f;
        public const float KillAnimIceDustScaleMax = 1.4f;
        public const int KillAnimHaloCount = 3;
        public const float KillAnimHaloScale = 0.15f;
        /// <summary>碎裂音效音量。沿用旧值 BabyIceDragon.cs:386</summary>
        public const float BrokenSoundVolume = 0.4f;

        //==================== 眩晕 dizzy / 休息 rest ====================

        /// <summary>眩晕 300 帧（俯冲撞墙、冰球被击破两处都是）。沿用旧值 AI.Dive.cs:77, NPC.IceCube.cs:156</summary>
        public const int DizzyFrames = 300;
        /// <summary>眩晕入场：被弹回 (-面向 × 4, -4)，8 粒碎冰（±0.4 弧度、速度 2~5、缩放 1~1.4），星星轨迹长 10。沿用旧值 BabyIceDragon.cs:908-917</summary>
        public const float DizzyKnockX = 4f;
        public const float DizzyKnockY = -4f;
        public const int DizzyDustCount = 8;
        public const float DizzyDustAngle = 0.4f;
        public const float DizzyDustSpeedMin = 2f;
        public const float DizzyDustSpeedMax = 5f;
        public const float DizzyDustScaleMin = 1f;
        public const float DizzyDustScaleMax = 1.4f;
        public const float DizzyStarRotation = 1.57f;
        public const float DizzyStarLength = 10f;
        /// <summary>眩晕中 X 衰减 0.96、落地判定 Y 速度 &lt; 0.1。沿用旧值 BabyIceDragon.cs:554,568</summary>
        public const float DizzyDampX = 0.96f;
        public const float DizzyGroundedSpeedY = 0.1f;
        /// <summary>
        /// 眩晕结束接的休息：旧 <c>HaveARest(30)</c> 后同帧 <c>Timer--</c>，休息态首帧读 29，等效 29 帧。沿用旧值 BabyIceDragon.cs:551,569
        /// </summary>
        public const int DizzyRestFrames = 29;

        /// <summary>招式后摇：普通 30 帧、大师 10 帧。沿用旧值 AI.IceBreath.cs:107, AI.DoubleDash.cs:176</summary>
        public const int RestFrames = 30;
        public const int RestFramesMaster = 10;
        public static int RestFramesAfterBreath() => Main.masterMode ? RestFramesMaster : RestFrames;
        /// <summary>龙卷风 / 冰锥射击收招：旧 <c>HaveARest(30)</c> 后走 break 再 <c>Timer++</c>，休息态首帧读 31，等效 31 帧。沿用旧值 AI.IceTornado.cs:148, AI.IcicleShoot.cs:153</summary>
        public const int RestFramesAfterBreak = RestFrames + 1;
        /// <summary>冰球爆炸后休息 40 帧。沿用旧值 NPC.IceCube.cs:137</summary>
        public const int RestFramesAfterCubeBurst = 40;
        /// <summary>休息：X 衰减 0.97、朝向回正 0.08；Y 追踪目标上方 100 px（方向按 150 px 判，旧代码即如此），死区 50，速度 6 / 加速 0.14 / 转向 0.1 / 衰减 0.96。沿用旧值 BabyIceDragon.cs:576-583</summary>
        public const float RestDampX = 0.97f;
        public const float RestRotationStep = 0.08f;
        public const float RestHoverDirY = 150f;
        public const float RestHoverY = 100f;
        public const float RestDeadZoneY = 50f;
        public const float RestSpeedY = 6f;
        public const float RestAccelY = 0.14f;
        public const float RestTurnY = 0.1f;
        public const float RestDampY = 0.96f;

        //==================== 俯冲 dive ====================

        /// <summary>爬升到比目标高 460 px 才俯冲；爬升超 400 帧放弃。沿用旧值 AI.Dive.cs:19,26</summary>
        public const float DiveClimbHeight = 460f;
        public const int DiveClimbTimeout = 400;
        /// <summary>爬升段：X 衰减 0.97、朝向回正 0.08。沿用旧值 AI.Dive.cs:22-23</summary>
        public const float DiveClimbDampX = 0.97f;
        public const float DiveClimbRotationStep = 0.08f;
        /// <summary>第 3 帧起冲：瞄准目标上方 30 px、速度 13。沿用旧值 AI.Dive.cs:52-54</summary>
        public const int DiveLaunchFrame = 3;
        public const float DiveAimOffsetY = 30f;
        public const float DiveSpeed = 13f;
        /// <summary>100 帧内：低于目标上方 20 px 即转刹车（Timer 跳到 100）；嘴前 3 个探针（间距 16）撞实心物块则眩晕。沿用旧值 AI.Dive.cs:60-80</summary>
        public const int DiveFlightFrames = 100;
        public const float DivePassOffsetY = 20f;
        public const float DiveProbeSpacing = 16f;
        /// <summary>刹车段到 130 帧：0.95 衰减、朝向回正 0.08。沿用旧值 AI.Dive.cs:84-88</summary>
        public const int DiveEndFrame = 130;
        public const float DiveBrakeDamp = 0.95f;

        //==================== 蓄力冰球 accumulate ====================

        /// <summary>50 帧内就位：Y 追踪目标（死区 50，6 / 0.14 / 0.1 / 0.96）、X 追踪（3 / 0.08 / 0.08 / 0.96）、朝向回正 0.06。沿用旧值 AI 蓄力 BabyIceDragon.cs:601-615</summary>
        public const int AccumulateApproachFrames = 50;
        public const float AccumulateDeadZoneY = 50f;
        public const float AccumulateSpeedY = 6f;
        public const float AccumulateAccelY = 0.14f;
        public const float AccumulateTurnY = 0.1f;
        public const float AccumulateSpeedX = 3f;
        public const float AccumulateAccelX = 0.08f;
        public const float AccumulateTurnX = 0.08f;
        public const float AccumulateChaseDamp = 0.96f;
        public const float AccumulateRotationStep = 0.06f;
        /// <summary>62 帧蓄力音效与粒子；80 帧前 0.92 刹停。沿用旧值 BabyIceDragon.cs:619,638</summary>
        public const int AccumulateChargeCueFrame = 62;
        public const int AccumulateSpawnFrame = 80;
        public const float AccumulateBrakeDamp = 0.92f;
        /// <summary>冰球生成在面前 170 px、下 20 px。沿用旧值 BabyIceDragon.cs:645</summary>
        public const float AccumulateCubeOffsetX = 170f;
        public const float AccumulateCubeOffsetY = 20f;
        /// <summary>绕冰球飞：切向速度 4，朝向跟速度 0.8；嘴部冰雾速度 8、散角 ±0.3、缩放 0.6~0.8。沿用旧值 BabyIceDragon.cs:653-658</summary>
        public const float AccumulateOrbitSpeed = 4f;
        public const float AccumulateOrbitRotationStep = 0.8f;
        public const float AccumulateFogSpeed = 8f;
        public const float AccumulateFogAngle = 0.3f;
        public const float AccumulateFogScaleMin = 0.6f;
        public const float AccumulateFogScaleMax = 0.8f;
        /// <summary>每 20 帧查一次冰球是否还在；900 帧兜底收招。沿用旧值 BabyIceDragon.cs:660,671</summary>
        public const int AccumulateCubeCheckInterval = 20;
        public const int AccumulateEndFrame = 900;

        //==================== 冰吐息 iceBreath ====================

        /// <summary>就位：距离 &gt; 440 时追目标上方 200 px（死区 50；Y 8 / 0.18 / 0.6 / 0.96；X 距 &gt; 200 时 14 / 0.3 / 0.6 / 0.96，否则 0.98），超 400 帧放弃。沿用旧值 AI.IceBreath.cs:20-35</summary>
        public const float BreathApproachDistance = 440f;
        public const float BreathHoverY = 200f;
        public const float BreathDeadZoneY = 50f;
        public const float BreathApproachSpeedY = 8f;
        public const float BreathApproachAccelY = 0.18f;
        public const float BreathApproachTurnY = 0.6f;
        public const float BreathApproachDeadZoneX = 200f;
        public const float BreathApproachSpeedX = 14f;
        public const float BreathApproachAccelX = 0.3f;
        public const float BreathApproachTurnX = 0.6f;
        public const float BreathApproachDamp = 0.96f;
        public const float BreathApproachIdleDampX = 0.98f;
        public const int BreathApproachTimeout = 400;
        /// <summary>吐息段：距离 &gt; 200 时缓慢跟随（Y 3 / 0.14 / 0.1 / 0.96；X 距 &gt; 160 时同参，否则 0.98）。沿用旧值 AI.IceBreath.cs:51-64</summary>
        public const float BreathKeepDistance = 200f;
        public const float BreathFollowSpeed = 3f;
        public const float BreathFollowAccel = 0.14f;
        public const float BreathFollowTurn = 0.1f;
        public const float BreathFollowDeadZoneX = 160f;
        /// <summary>10 帧蓄力音效与粒子；30 帧起每 5 帧吐两发（散角 i×0.05、速度 10、伤害 40/65/90、击退 5、瞄准散布 30），41 帧收招进休息。沿用旧值 AI.IceBreath.cs:67-108</summary>
        public const int BreathChargeCueFrame = 10;
        public const int BreathFireStart = 30;
        public const int BreathFireEnd = 41;
        public const int BreathFireInterval = 5;
        public const float BreathSpreadStep = 0.05f;
        public const float BreathSpeed = 10f;
        public const float BreathAimSpread = 30f;
        public const float BreathKnockback = 5f;
        public static int BreathDamage() => Helper.GetProjDamage(40, 65, 90);

        //==================== 龙车 horizontalDash / 龙车变种 doubleDash ====================

        /// <summary>就位：高差 &gt; 32 或 X 距 &gt; 450 时追（Y 8 / 0.2 / 0.6 / 0.96；X 距 &gt; 400 时 14 / 0.25 / 0.6 / 0.96，否则 0.93），超 400 帧放弃。沿用旧值 AI.HorizontalDash.cs:19-38</summary>
        public const float DashAlignY = 32f;
        public const float DashAlignX = 450f;
        public const float DashApproachSpeedY = 8f;
        public const float DashApproachAccelY = 0.2f;
        public const float DashApproachTurnY = 0.6f;
        public const float DashApproachDeadZoneX = 400f;
        public const float DashApproachSpeedX = 14f;
        public const float DashApproachAccelX = 0.25f;
        public const float DashApproachTurnX = 0.6f;
        public const float DashApproachDamp = 0.96f;
        public const float DashApproachIdleDampX = 0.93f;
        public const int DashApproachTimeout = 400;
        /// <summary>起手闪光缩放 1.2。沿用旧值 AI.HorizontalDash.cs:52</summary>
        public const float DashCueSparkScale = 1.2f;
        /// <summary>第 10 帧起冲（速度 18），35 帧前不判越过；越过 240 px（变种 180）提前进入刹车（Timer 跳到 75）；75 帧后 0.93 刹车，85 帧收招。沿用旧值 AI.HorizontalDash.cs:65-91, AI.DoubleDash.cs:82</summary>
        public const int DashLaunchFrame = 10;
        public const float DashSpeed = 18f;
        public const int DashPassCheckStart = 35;
        public const float DashPassDistance = 240f;
        public const float DoubleDashPassDistance = 180f;
        public const int DashBrakeStart = 75;
        public const float DashBrakeDamp = 0.93f;
        public const int DashEndFrame = 85;
        /// <summary>变种刹停后按目标方位决定接招：与水平夹角在 ±30° 内（即 |角| &lt; π/6 或 &gt; 5π/6）冲撞，否则吐息。沿用旧值 AI.DoubleDash.cs:98</summary>
        public const float DoubleDashChargeAngle = MathHelper.Pi / 6f;
        /// <summary>强化吐息：10 帧蓄力音效，30 帧起每 10 帧吐两发（散角 i×0.02），每 15 帧一枚冰锥（散角 ±0.2、速度 12、击退 8），61 帧收招。沿用旧值 AI.DoubleDash.cs:127-177</summary>
        public const int DoubleBreathFireEnd = 61;
        public const int DoubleBreathFireInterval = 10;
        public const float DoubleBreathSpreadStep = 0.02f;
        public const int DoubleBreathIcicleInterval = 15;
        public const float DoubleBreathIcicleSpread = 0.2f;
        public const float DoubleBreathIcicleSpeed = 12f;
        public const float DoubleBreathIcicleKnockback = 8f;
        /// <summary>强化冲撞：第 1 帧锁向并定冲程 = 距离 + 100 钳 120~400；15 帧以冲程/20 的速度起冲；35 帧后 0.97 刹车；55 帧收招。沿用旧值 AI.DoubleDash.cs:182-236</summary>
        public const int DoubleDashLockFrames = 2;
        public const float DoubleDashLengthBonus = 100f;
        public const float DoubleDashLengthMin = 120f;
        public const float DoubleDashLengthMax = 400f;
        public const int DoubleDashLaunchFrame = 15;
        public const float DoubleDashLengthToSpeed = 20f;
        public const int DoubleDashFlightEnd = 35;
        public const float DoubleDashBrakeDamp = 0.97f;
        public const int DoubleDashEndFrame = 55;
        /// <summary>冲撞速度线粒子：每 2 帧，散布 32，反向 0.2~0.5 倍速，缩放 0.1~0.4。沿用旧值 AI.DoubleDash.cs:218-220</summary>
        public const int SpeedLineInterval = 2;
        public const float SpeedLineSpread = 32f;
        public const float SpeedLineSpeedMin = 0.2f;
        public const float SpeedLineSpeedMax = 0.5f;
        public const float SpeedLineScaleMin = 0.1f;
        public const float SpeedLineScaleMax = 0.4f;

        //==================== 下砸 smashDown ====================

        /// <summary>就位：比目标高 430 且 X 距 ≤ 250 才下砸；X 距 &gt; 150 时 14 / 0.3 / 0.6 / 0.96 追，否则 0.96；不够高就扇翅上飞，够高 Y 衰减 0.9；超 300 帧放弃。沿用旧值 AI.SmashDown.cs:20-37</summary>
        public const float SmashClimbHeight = 430f;
        public const float SmashAlignX = 250f;
        public const float SmashApproachDeadZoneX = 150f;
        public const float SmashApproachSpeedX = 14f;
        public const float SmashApproachAccelX = 0.3f;
        public const float SmashApproachTurnX = 0.6f;
        public const float SmashApproachDamp = 0.96f;
        public const float ClimbHoverDampY = 0.9f;
        public const int SmashApproachTimeout = 300;
        /// <summary>起手：X 速度 × 0.4、Y 归零、闪光 0.8。沿用旧值 AI.SmashDown.cs:50-59</summary>
        public const float SmashStartDampX = 0.4f;
        public const float SmashCueSparkScale = 0.8f;
        /// <summary>下落：朝向跟速度 0.6、Y 加速 0.9、上限 24；40 帧内 X 追（5 / 0.4 / 0.3 / 0.96），之后 0.96。沿用旧值 AI.SmashDown.cs:67-85,135</summary>
        public const float SmashFallRotationStep = 0.6f;
        public const float SmashFallAccelY = 0.9f;
        public const float SmashFallMaxY = 24f;
        public const int SmashChaseFrames = 40;
        public const float SmashChaseSpeedX = 5f;
        public const float SmashChaseAccelX = 0.4f;
        public const float SmashChaseTurnX = 0.3f;
        public const float SmashChaseDamp = 0.96f;
        /// <summary>60 帧起（或已低于目标头顶 32 px）恢复物块碰撞并检测脚下 2 行；200 帧砸空收招。沿用旧值 AI.SmashDown.cs:87-121</summary>
        public const int SmashCollideFrame = 60;
        public const float SmashPassOffsetY = 32f;
        public const int SmashGroundCheckRows = 2;
        public const int SmashTimeoutFrame = 200;
        /// <summary>下落尘：每 2 帧 1 粒霜（散布 32、反向 0.5、缩放 1.8~2）+ 2 粒风暴（散布 48、反向 0.3、缩放 1~1.5）。沿用旧值 AI.SmashDown.cs:70-78</summary>
        public const int SmashDustInterval = 2;
        public const float SmashFrostDustSpread = 32f;
        public const float SmashFrostDustBack = 0.5f;
        public const float SmashFrostDustScaleMin = 1.8f;
        public const float SmashFrostDustScaleMax = 2f;
        public const int SmashStormDustCount = 2;
        public const float SmashStormDustSpread = 48f;
        public const float SmashStormDustBack = 0.3f;
        public const float SmashStormDustScaleMin = 1f;
        public const float SmashStormDustScaleMax = 1.5f;
        /// <summary>砸地冰刺：两侧各 4 根、间隔 2 格、共 20 档角度、缩放 0.4 起递增 0.2，伤害 40/45/60；震屏 (0,1) 强度 20、6f、30 帧。沿用旧值 BabyIceDragon.cs:734-771</summary>
        public const int ThornsPerSide = 4;
        public const int ThornsColumnStep = 2;
        public const int ThornsAngleSteps = 20;
        public const int ThornsAngleIndexStep = 6;
        public const float ThornsAngleFactor = 0.7f;
        public const float ThornsBaseScale = 0.4f;
        public const float ThornsScaleStep = 0.2f;
        public const float ThornsScaleXFactor = 1.1f;
        public static int ThornsDamage() => Helper.GetProjDamage(40, 45, 60);
        public const float ThornsShakeStrength = 20f;
        public const float ThornsShakeVibration = 6f;
        public const int ThornsShakeFrames = 30;
        /// <summary>找地面：从落点向目标脚下最多找 15 格，再上探 / 下探各 8 格。沿用旧值 BabyIceDragon.cs:785-825</summary>
        public const int ThornsSearchRows = 15;
        public const int ThornsProbeRows = 8;
        public const int ThornsWorldMargin = 10;

        //==================== 冰刺陷阱 iceThornsTrap / 冰雹 iceCloud（吼叫招通用）====================

        /// <summary>就位：距离 &gt; 440 时追目标上方 200 px（死区 50；Y 8 / 0.25 / 0.3 / 0.96；X 距 &gt; 160 时 16 / 0.25 / 0.3 / 0.96，否则 0.96），超 400 帧放弃。沿用旧值 AI.IceThronsTrap.cs:20-35</summary>
        public const float RoarAttackApproachDistance = 440f;
        public const float RoarAttackHoverY = 200f;
        public const float RoarAttackDeadZoneY = 50f;
        public const float RoarAttackSpeedY = 8f;
        public const float RoarAttackAccelY = 0.25f;
        public const float RoarAttackTurnY = 0.3f;
        public const float RoarAttackDeadZoneX = 160f;
        public const float RoarAttackSpeedX = 16f;
        public const float RoarAttackAccelX = 0.25f;
        public const float RoarAttackTurnX = 0.3f;
        public const float RoarAttackDamp = 0.96f;
        public const int RoarAttackApproachTimeout = 400;
        /// <summary>吼叫段：全程 0.97 衰减、朝向回正 0.04；30 帧合翅停住；50 帧吼叫；60 帧生成；90 帧前吼叫粒子；120 帧收招。沿用旧值 AI.IceThronsTrap.cs:50-111</summary>
        public const float RoarAttackDamp2 = 0.97f;
        public const float RoarAttackRotationStep = 0.04f;
        public const int RoarAttackStopFrame = 30;
        public const int RoarAttackRoarFrame = 50;
        public const int RoarAttackSpawnFrame = 60;
        public const int RoarAttackParticleEnd = 90;
        public const int RoarAttackEndFrame = 120;
        /// <summary>冰刺球数量 4 / 5 / 6（普通 / 专家 / 大师），环绕目标半径 240~350。沿用旧值 AI.IceThronsTrap.cs:79-89</summary>
        public static int ThornsTrapCount() => Main.masterMode ? 6 : (Main.expertMode ? 5 : 4);
        public const int ThornsTrapRadiusMin = 240;
        public const int ThornsTrapRadiusMax = 350;
        /// <summary>冰云生成在目标上方 300 px、X 随机 ±80，伤害 1、击退 1（伤害由云自己的冰锥承担）。沿用旧值 AI.IceCloud.cs:77-78</summary>
        public const int CloudOffsetX = 80;
        public const float CloudOffsetY = -300f;
        public const int CloudDamage = 1;
        public const float CloudKnockback = 1f;

        //==================== 冰晶龙卷风 iceTornado（大师专属）====================

        /// <summary>就位：距离 &gt; 600 时追目标上方 200 px（死区 50；Y 8 / 0.18 / 0.6 / 0.96；X 距 &gt; 160 时 14 / 0.2 / 0.6 / 0.96，否则 0.98），超 400 帧放弃。沿用旧值 AI.IceTornado.cs:20-37</summary>
        public const float TornadoApproachDistance = 600f;
        public const float TornadoHoverY = 200f;
        public const float TornadoDeadZoneY = 50f;
        public const float TornadoApproachSpeedY = 8f;
        public const float TornadoApproachAccelY = 0.18f;
        public const float TornadoApproachTurnY = 0.6f;
        public const float TornadoApproachDeadZoneX = 160f;
        public const float TornadoApproachSpeedX = 14f;
        public const float TornadoApproachAccelX = 0.2f;
        public const float TornadoApproachTurnX = 0.6f;
        public const float TornadoApproachDamp = 0.96f;
        public const float TornadoApproachIdleDampX = 0.98f;
        public const int TornadoApproachTimeout = 400;
        /// <summary>起手：闪光 0.8、朝目标 8 px/f。沿用旧值 AI.IceTornado.cs:48-52</summary>
        public const float TornadoCueSparkScale = 0.8f;
        public const float TornadoApproachLaunchSpeed = 8f;
        /// <summary>准备：前 2 帧竖直 6 px/f，30 帧内每帧转 2π/30 画一圈；30~60 帧以 2 px/f 后撤、朝向回正 0.14。沿用旧值 AI.IceTornado.cs:61-77</summary>
        public const int TornadoSpinInitFrames = 2;
        public const float TornadoSpinSpeed = 6f;
        public const int TornadoSpinFrames = 30;
        public const float TornadoSpinStepAngle = MathHelper.TwoPi / 30f;
        public const int TornadoBackFrames = 60;
        public const float TornadoBackSpeed = 2f;
        public const float TornadoRotationStep = 0.14f;
        /// <summary>龙卷起手：朝目标方向偏 0.9 弧度以 12 px/f 冲出并无敌；风声音量 0.2。沿用旧值 AI.IceTornado.cs:81-86</summary>
        public const float TornadoLaunchAngle = 0.9f;
        public const float TornadoLaunchSpeed = 12f;
        public const float WindSoundVolume = 0.2f;
        /// <summary>龙卷 200 帧：距离 &lt; 160 加速 1.2 否则 0.65，限速 9.5；每 20 帧风声；每 8 帧透明弹幕（前方 6 帧位移处、0.2 倍速、伤害 40/65/100、击退 10、寿命 40）。沿用旧值 AI.IceTornado.cs:92-116</summary>
        public const int TornadoSpinAttackFrames = 200;
        public const float TornadoCloseDistance = 160f;
        public const float TornadoCloseAccel = 1.2f;
        public const float TornadoFarAccel = 0.65f;
        public const float TornadoMaxSpeed = 9.5f;
        public const int TornadoWindInterval = 20;
        public const int TornadoProjInterval = 8;
        public const float TornadoProjAhead = 6f;
        public const float TornadoProjSpeedFactor = 0.2f;
        public const float TornadoProjKnockback = 10f;
        public const int TornadoProjLife = 40;
        public static int TornadoDamage() => Helper.GetProjDamage(40, 65, 100);
        /// <summary>龙卷尘：每 2 帧 1 粒霜（散布 32、反向 0.3、缩放 1.8~2）；龙卷粒子在前方 8 帧位移处、0.05 倍速、淡入 60、缩放 0.5~0.6。沿用旧值 AI.IceTornado.cs:121-133</summary>
        public const int TornadoDustInterval = 2;
        public const float TornadoDustSpread = 32f;
        public const float TornadoDustBack = 0.3f;
        public const float TornadoDustScaleMin = 1.8f;
        public const float TornadoDustScaleMax = 2f;
        public const float TornadoParticleAhead = 8f;
        public const float TornadoParticleSpeedFactor = 0.05f;
        public const float TornadoParticleFadeIn = 60f;
        public const float TornadoParticleScaleMin = 0.5f;
        public const float TornadoParticleScaleMax = 0.6f;
        /// <summary>龙卷粒子三档配色，等概率取一档。沿用旧值 AI.IceTornado.cs:128-132</summary>
        public static readonly Color[] TornadoColors =
        {
            new(217, 248, 255, 200),
            new(120, 211, 231, 200),
            new(252, 255, 255, 200),
        };
        /// <summary>210 帧前刹车 0.98、朝向回正、解除无敌；之后进休息。沿用旧值 AI.IceTornado.cs:139-148</summary>
        public const int TornadoBrakeEnd = 210;
        public const float TornadoBrakeDamp = 0.98f;

        //==================== 冰锥射击 iciclesFall（大师专属）====================

        /// <summary>就位：低于目标上方 100 px 就扇翅上飞，够高 Y 衰减 0.9；X 距 &gt; 600 需追、&gt; 300 时 14 / 0.24 / 0.24 / 0.96 否则 0.96；超 300 帧放弃。沿用旧值 AI.IcicleShoot.cs:22-39</summary>
        public const float IcicleClimbHeight = 100f;
        public const float IcicleAlignX = 600f;
        public const float IcicleApproachDeadZoneX = 300f;
        public const float IcicleApproachSpeedX = 14f;
        public const float IcicleApproachAccelX = 0.24f;
        public const float IcicleApproachTurnX = 0.24f;
        public const float IcicleApproachDamp = 0.96f;
        public const int IcicleApproachTimeout = 300;
        /// <summary>扫射段：全程 0.97 衰减；20 帧前朝向归零；100 帧前俯仰 = 面向 × 高差 × 0.008 钳 ±0.45，每 12 帧一枚冰锥（散角 ±0.4、速度 12、击退 8），每 2 帧嘴部冰雾（散角 ±0.6、速度 4、缩放 0.8）。沿用旧值 AI.IcicleShoot.cs:72-98</summary>
        public const float IcicleSweepDamp = 0.97f;
        public const int IcicleSweepAimStart = 20;
        public const int IcicleSweepEnd = 100;
        public const float IcicleAimPerHeight = 0.008f;
        public const float IcicleAimClamp = 0.45f;
        public const int IcicleShootInterval = 12;
        public const float IcicleShootSpread = 0.4f;
        public const float IcicleShootSpeed = 12f;
        public const float IcicleShootKnockback = 8f;
        public const int IcicleFogInterval = 2;
        public const float IcicleFogAngle = 0.6f;
        public const float IcicleFogSpeed = 4f;
        public const float IcicleFogScale = 0.8f;
        public static int IcicleDamage() => Helper.GetProjDamage(40, 65, 90);
        /// <summary>吼叫段：朝向 0.14 插值回正；20 帧吼叫；每 6 帧在目标上方 500 px（X 随机 ±100）落一枚冰锥（瞄准散布 40、速度 12、击退 8）；80 帧进休息。沿用旧值 AI.IcicleShoot.cs:111-153</summary>
        public const float IcicleRoarRotationLerp = 0.14f;
        public const int IcicleRoarFrame = 20;
        public const int IcicleFallInterval = 6;
        public const int IcicleFallOffsetX = 100;
        public const float IcicleFallOffsetY = -500f;
        public const float IcicleFallAimSpread = 40f;
        public const float IcicleFallSpeed = 12f;
        public const float IcicleFallKnockback = 8f;
        public const int IcicleRoarEndFrame = 80;
    }
}
