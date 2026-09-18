using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon.Core
{
    /// <summary>
    /// 荒雷龙战斗调参中心：全部数字与阶段档位的唯一出口。<br/>
    /// 本轮是结构迁移，战斗设计不变：每条常量注明来源，查不到设计理由的一律写“沿用旧值 文件:行号”（迁移前 <c>AI.*.cs</c> / <c>ThunderveinDragon.cs</c> 的行号）。
    /// 招式意图见同目录 <c>ThunderveinDragon_AI_Description.md</c>。<br/>
    /// 计时约定：旧招式体在拍尾 <c>Timer++</c>，所以“收拍判断 <c>Timer &gt; N</c>”直接用基座 Timer（1 起），
    /// “拍首一次性判断 <c>Timer == N</c> / 区间 <c>Timer &lt; N</c>”用 <see cref="ThunderveinStateBase.T"/>（= Timer − 1，0 起）——常量本身全部保持旧值。
    /// </summary>
    internal static class ThunderveinDirector
    {
        //==================== 出生 / 目标 / 脱战 ====================

        /// <summary>出生点在召唤弹幕（或目标）头顶 400 px，留出下沉入场距离。沿用旧值 ThunderveinDragon.cs:520,524</summary>
        public const float SpawnHeightAboveAnchor = 400f;
        /// <summary>目标超过 3000 px 重新索敌；重新索敌后仍超过 4500 px（或死亡）则离场。沿用旧值 ThunderveinDragon.cs:377,381</summary>
        public const float RetargetDistance = 3000f;
        public const float DespawnDistance = 4500f;
        /// <summary>脱战：朝向回正 0.14 弧度/帧、X 衰减 0.98、向上飞（加速 0.3 / 上限 20 / 减速 0.9）、鼓励消失 30。沿用旧值 ThunderveinDragon.cs:387-390</summary>
        public const float DespawnRotRate = 0.14f;
        public const float DespawnDampX = 0.98f;
        public const float DespawnFlyUpAccel = 0.3f;
        public const float DespawnFlyUpMax = 20f;
        public const float DespawnFlyUpSlow = 0.9f;
        public const int DespawnEncourageFrames = 30;

        /// <summary>
        /// 阶段血量阈值（大师 / FTW 提前一档）：P2 = 75% / 50%，P3（首次冥雷）= 50% / 25%，P4（二次冥雷）= 25% / 12.5%。
        /// 沿用旧值 ThunderveinDragon.cs:444-447
        /// </summary>
        public static float PhaseLifeRatio(int nextPhase)
        {
            bool masterLike = Main.masterMode || Main.getGoodWorld;
            return nextPhase switch
            {
                2 => masterLike ? 0.75f : 0.5f,
                3 => masterLike ? 0.5f : 0.25f,
                _ => masterLike ? 0.25f : 0.125f,
            };
        }

        //==================== 选招（hub Commit；旧 ResetStates ThunderveinDragon.cs:645-739）====================

        /// <summary>玩家在背后时闪电突袭额外 +5 权重（设计文档“招式切换时玩家在背后时使用概率增加”）。ThunderveinDragon.cs:667-671</summary>
        public const int BehindRaidWeight = 5;
        /// <summary>上一手不是短冲时短冲 +7 权重（“简单的位移招式，会穿插在各类招式之间”）。ThunderveinDragon.cs:673-677</summary>
        public const int SmallDashWeight = 7;
        /// <summary>距离小于 420 px 时放电 +4 权重（“只有玩家距离一定范围内才会使用”）。ThunderveinDragon.cs:679-683</summary>
        public const float DischargeDistance = 420f;
        public const int DischargeWeight = 4;
        /// <summary>距离大于 800 px 远程招 +7；大于 1400 px 改为落雷 +7（“距离玩家较远时使用概率大幅增加”）。ThunderveinDragon.cs:687-708</summary>
        public const float FarDistance = 800f;
        public const float VeryFarDistance = 1400f;
        public const int FarWeight = 7;
        /// <summary>二阶段起累计出招超过 7 手后，引力雷球按累计手数加权，用过即归零。沿用旧值 ThunderveinDragon.cs:711-731</summary>
        public const int GravitationUnlockCount = 7;
        /// <summary>查重窗口（记账用，本轮不裁决）：15 个状态取约 1/4。</summary>
        public const int RecentPickWindow = 4;
        /// <summary>
        /// hub 停留帧数。旧代码收招当帧即 <c>ResetStates</c> 切下一招、零间隔（后摇已含在各招尾段），
        /// 为不改节奏取 0：<c>EndAttack</c> 直接经 Commit 返回下一招，不多占一帧。
        /// </summary>
        public const int HubFrames = 0;
        /// <summary>
        /// 所有状态的超时兜底。最长的正常招式是冥雷（追击 240 + 旋转 65 + 幻影四轮约 730 + 收尾约 100 ≈ 1150 帧），
        /// 1800 帧（30 s）仍未退出就是软锁，强制收招。
        /// </summary>
        public const int StateTimeoutFrames = 1800;
        /// <summary>引力雷球收招后的固定接招池：闪电突袭 / 冲刺放电 / 电磁炮三选一（设计文档“下一次招式必定会使用……其中之一”）。AI.GravitationThunder.cs:134-139</summary>
        public const int GravitationFollowUpCount = 3;

        //==================== 运动 / 朝向（宿主 ApplyDeclaredMovement 消费）====================

        /// <summary>常规朝向：目标角 = 竖直速度 × 0.05 × 面向，AngleLerp 0.08。沿用旧值 ThunderveinDragon.cs:812-817</summary>
        public const float RotationPerVelY = 0.05f;
        public const float RotationNormalRate = 0.08f;
        /// <summary>回正朝向默认插值 0.2。沿用旧值 ThunderveinDragon.cs:824</summary>
        public const float NoRotRate = 0.2f;
        /// <summary>贴图翻面时给 rotation 补半圈；旧代码写的是 3.141f 而非 MathHelper.Pi，保持原值。沿用旧值 ThunderveinDragon.cs:648,815,827</summary>
        public const float FlipRotation = 3.141f;
        /// <summary>Hold 模式（漏声明兜底）的速度衰减；正常迁移后不会命中。</summary>
        public const float HoldDamp = 0.9f;
        /// <summary>追踪模式各轴的超速衰减 / 死区衰减 0.95（所有追踪块共用）。沿用旧值 AI.FallingThunder.cs:33-48 等</summary>
        public const float ChaseDamp = 0.95f;
        /// <summary>吐息类招式的追踪目标点在玩家头顶 300 px。沿用旧值 AI.LightningBreath.cs:25</summary>
        public const float BreathChaseOffsetY = -300f;

        /// <summary>落雷追击：保持 400～600 px 的横向带宽，X 18/0.35/0.6，Y 阈值 70、15/0.25/0.6，向上飞 0.4/15/0.93。沿用旧值 AI.FallingThunder.cs:32-49</summary>
        public static readonly ThunderveinChaseProfile FallingChase = new(400f, 600f, 18f, 0.35f, 0.6f, 70f, 15f, 0.25f, 0.6f, true, 0.4f, 15f, 0.93f, false);
        /// <summary>闪电突袭追击：带宽 350～550 px，其余同落雷。沿用旧值 AI.LightningRaid.cs:30-48</summary>
        public static readonly ThunderveinChaseProfile RaidChase = new(350f, 550f, 18f, 0.35f, 0.6f, 70f, 15f, 0.25f, 0.6f, true, 0.4f, 15f, 0.93f, false);
        /// <summary>吐息追击（目标点玩家头顶 300）：X 超 400 才前进 18/0.3/0.6，Y 70、15/0.25/0.6，向上飞 0.75/20/0.9。沿用旧值 AI.LightningBreath.cs:34-50</summary>
        public static readonly ThunderveinChaseProfile BreathChase = new(0f, 400f, 18f, 0.3f, 0.6f, 70f, 15f, 0.25f, 0.6f, true, 0.75f, 20f, 0.9f, false);
        /// <summary>电磁炮追击：同吐息但向上飞 0.55/20/0.9。沿用旧值 AI.ElectromagneticCannon.cs:33-49</summary>
        public static readonly ThunderveinChaseProfile CannonChase = new(0f, 400f, 18f, 0.3f, 0.6f, 70f, 15f, 0.25f, 0.6f, true, 0.55f, 20f, 0.9f, false);
        /// <summary>冥雷追击：带宽 200～600 px，X 18/0.3/0.6，向上飞 0.55/20/0.9。沿用旧值 AI.StygianThunder.cs:43-61</summary>
        public static readonly ThunderveinChaseProfile StygianChase = new(200f, 600f, 18f, 0.3f, 0.6f, 70f, 15f, 0.25f, 0.6f, true, 0.55f, 20f, 0.9f, false);
        /// <summary>近身贴脸（放电）：X 超 50 才动 6/0.15/0.4，Y 70、4/0.15/0.3，向上飞 0.35/15/0.9。沿用旧值 AI.Discharging.cs:31-47</summary>
        public static readonly ThunderveinChaseProfile CloseChase = new(0f, 50f, 6f, 0.15f, 0.4f, 70f, 4f, 0.15f, 0.3f, true, 0.35f, 15f, 0.9f, false);
        /// <summary>近身贴脸（电球 / 引力雷球）：同放电，飞行帧张嘴。沿用旧值 AI.LightningBall.cs:38-54</summary>
        public static readonly ThunderveinChaseProfile CloseChaseOpenMouth = new(0f, 50f, 6f, 0.15f, 0.4f, 70f, 4f, 0.15f, 0.3f, true, 0.35f, 15f, 0.9f, true);
        /// <summary>十字雷蓄力：X 超 150 才动 8/0.2/0.4，Y 阈值 100，张嘴。沿用旧值 AI.CrossLightingBall.cs:38-54</summary>
        public static readonly ThunderveinChaseProfile CrossChase = new(0f, 150f, 8f, 0.2f, 0.4f, 100f, 4f, 0.15f, 0.3f, true, 0.35f, 15f, 0.9f, true);
        /// <summary>冲刺放电蓄力段：X 超 50 才动 4/0.15/0.4，Y 70、4/0.15/0.3，无向上飞分支；飞行帧由该拍自管。沿用旧值 AI.DashDischarging.cs:92-118</summary>
        public static readonly ThunderveinChaseProfile DashDischargeCharge = new(0f, 50f, 4f, 0.15f, 0.4f, 70f, 4f, 0.15f, 0.3f, false, 0f, 0f, 1f, false, false);

        //==================== 出生动画 onSpawnAnmi（ThunderveinDragon.cs:495-598）====================

        /// <summary>淡入 80 帧（透明度 = Timer/80），随后生成名牌弹幕。沿用旧值 ThunderveinDragon.cs:534-542</summary>
        public const int SpawnFadeInFrames = 80;
        /// <summary>名牌出现后停 30 帧再吼。沿用旧值 ThunderveinDragon.cs:550</summary>
        public const int SpawnAppearFrames = 30;
        /// <summary>吼叫段：第 15 帧张嘴定格 + 音效，15～130 帧每 10 帧声波 / 每 20 帧声线 + 震屏（8, 12, 20），150 帧后进入首招。沿用旧值 ThunderveinDragon.cs:563-592</summary>
        public const float SpawnRoarDamp = 0.9f;
        public const int SpawnRoarCueFrame = 15;
        public const int SpawnRoarFxEnd = 130;
        public const int SpawnRoarFrames = 150;
        public const int SpawnRoarShakeStrength = 8;
        public const float SpawnRoarShakeVibration = 12f;
        public const int SpawnRoarShakeFrames = 20;
        /// <summary>吼叫 / 落雷 / 转阶段共用：每 10 帧一圈声波、每 20 帧一道声线，缩放 0.2，嘴部前伸 60 px。沿用旧值 AI.FallingThunder.cs:90-94</summary>
        public const int RoarWaveInterval = 10;
        public const int RoarLineInterval = 20;
        public const float RoarFxScale = 0.2f;
        public const float MouthForward = 60f;
        /// <summary>吼叫音效音调 0.4。沿用旧值 AI.FallingThunder.cs:85</summary>
        public const float RoarPitch = 0.4f;
        /// <summary>震屏衰减距离 1000（全部震屏共用）。沿用旧值 ThunderveinDragon.cs:583</summary>
        public const float ShakeFalloffDistance = 1000f;

        //==================== 死亡动画 onKillAnim（ThunderveinDragon.cs:600-634）====================

        /// <summary>前 60 帧以 1 px/f 上浮、白光叠加透明度 = Timer/60；第 60 帧爆出 30 圈 × 5 粒紫电粒子（半径 80→400，缩放 0.9～1.3）并真正死亡。沿用旧值 ThunderveinDragon.cs:602-630</summary>
        public const int KillAnimRiseFrames = 60;
        public const float KillAnimRiseSpeed = 1f;
        public const int KillAnimRingCount = 30;
        public const int KillAnimParticlesPerRing = 5;
        public const float KillAnimRingRadiusFrom = 80f;
        public const float KillAnimRingRadiusTo = 400f;
        public const float KillAnimParticleScaleMin = 0.9f;
        public const float KillAnimParticleScaleMax = 1.3f;

        //==================== 短冲 SmallDash（AI.SmallDash.cs）====================

        /// <summary>单段短冲时长 P1 15 / P2 17 帧，提前 2 帧收（<c>Timer &gt; smallDashTime - 2</c>）。沿用旧值 AI.SmallDash.cs:13,128,53</summary>
        public static int SmallDashFrames(int phase) => phase == 1 ? 15 : 17;
        public const int SmallDashEndEarly = 2;
        /// <summary>短冲速度 35；首段带 ±(0.9～1) 弧度随机偏角（确定性 AttackRandom），后续段直冲。沿用旧值 AI.SmallDash.cs:44-45</summary>
        public const float SmallDashSpeed = 35f;
        public const float SmallDashOffsetMin = 0.9f;
        public const float SmallDashOffsetMax = 1f;
        /// <summary>段末与玩家仍超过 700 px 才追加下一段；最多 P1 2 段 / P2 3 段。沿用旧值 AI.SmallDash.cs:55,170-222</summary>
        public const float SmallDashContinueDistance = 700f;
        public static int SmallDashMaxCount(int phase) => phase == 1 ? 2 : 3;
        /// <summary>收势 7 帧，回正 0.2。沿用旧值 AI.SmallDash.cs:116-119</summary>
        public const int SmallDashSettleFrames = 7;
        /// <summary>短冲弹幕伤害 20/30/40，弹幕 ai2 = 55。沿用旧值 AI.SmallDash.cs:35-39</summary>
        public static int SmallDashDamage() => Helper.GetProjDamage(20, 30, 40);
        public const float SmallDashProjAi2 = 55f;
        /// <summary>冲刺类残影放大 1.15。沿用旧值 AI.SmallDash.cs:28</summary>
        public const float DashShadowScale = 1.15f;

        //==================== 闪电突袭 LightningRaid（AI.LightningRaid.cs）====================

        /// <summary>乱窜单段 P1 12 / P2 14 帧，三段共 3×t−2 帧；长冲 25 帧。沿用旧值 AI.LightningRaid.cs:14-15,213-214,118</summary>
        public static int RaidZigFrames(int phase) => phase == 1 ? 12 : 14;
        public const int RaidZigCount = 3;
        public const int RaidZigEndEarly = 2;
        public const int RaidBigDashFrames = 25;
        /// <summary>追击最长 180 帧；至少 20 帧后横向超 350、纵向小于 250 即起手。沿用旧值 AI.LightningRaid.cs:24,53</summary>
        public const int RaidChaseFrames = 180;
        public const int RaidChaseMinFrames = 20;
        public const float RaidChaseExitX = 350f;
        public const float RaidChaseExitY = 250f;
        /// <summary>首段乱窜 35 px/f、偏角 ±(0.9～1.1)；后续 30 px/f、偏角 (0.6～1.1) 正负交替；只有 700 px 内才加偏角。沿用旧值 AI.LightningRaid.cs:86-110</summary>
        public const float RaidZigFirstSpeed = 35f;
        public const float RaidZigSpeed = 30f;
        public const float RaidZigFirstOffsetMin = 0.9f;
        public const float RaidZigFirstOffsetMax = 1.1f;
        public const float RaidZigOffsetMin = 0.6f;
        public const float RaidZigOffsetMax = 1.1f;
        public const float RaidZigOffsetDistance = 700f;
        /// <summary>乱窜弹幕伤害 20/30/70（ai2 55）；长冲 P1 50/60/120、P2 60/70/140（ai2 75）。沿用旧值 AI.LightningRaid.cs:74,152,349</summary>
        public static int RaidZigDamage() => Helper.GetProjDamage(20, 30, 70);
        public static int RaidBigDamage(int phase) => phase == 1 ? Helper.GetProjDamage(50, 60, 120) : Helper.GetProjDamage(60, 70, 140);
        public const float RaidZigProjAi2 = 55f;
        public const float RaidBigProjAi2 = 75f;
        /// <summary>起跳蓄力：速度不足 8 时每帧远离玩家 0.65，朝向 0.2 回正。沿用旧值 AI.LightningRaid.cs:135-138</summary>
        public const float RaidWindupBackSpeed = 8f;
        public const float RaidWindupBackAccel = 0.65f;
        public const float RaidWindupRotRate = 0.2f;
        /// <summary>
        /// 向后扇翅时长：旧代码从 frame.Y=4 起每 8 帧（P2 6 帧）推进一帧，超过第 7 帧起跳，即 4×8 = 32 / 4×6 = 24 帧。
        /// 改为按 Timer 判定（客户端可重建），翅膀帧仍按同一节奏推进。AI.LightningRaid.cs:143-148,340-345
        /// </summary>
        public static int RaidFlapFrames(int phase) => phase == 1 ? 32 : 24;
        public static int RaidFlapFrameTicks(int phase) => phase == 1 ? 8 : 6;
        /// <summary>长冲 40 px/f；结束后 0.8 刹车，横速小于 2 时重新面向玩家。沿用旧值 AI.LightningRaid.cs:163,192-194</summary>
        public const float RaidBigDashSpeed = 40f;
        public const float RaidBrakeDamp = 0.8f;
        public const float RaidBrakeStopX = 2f;
        /// <summary>长冲后后摇 P1 40 / P2 35 帧，FTW 15。沿用旧值 AI.LightningRaid.cs:201-203,409-411</summary>
        public static int RaidDelayFrames(int phase) => Main.getGoodWorld ? 15 : (phase == 1 ? 40 : 35);
        /// <summary>起跳：天空亮 0.4 持续 bigDash−3、过渡 8；震屏（方向×2.3，14，5，20）。沿用旧值 AI.LightningRaid.cs:167-171</summary>
        public const float RaidSkyLight = 0.4f;
        public const int RaidSkyLightFadeOffset = 3;
        public const int RaidSkyLightExchange = 8;
        public const float RaidShakeDirScale = 2.3f;
        public const int RaidShakeStrength = 14;
        public const float RaidShakeVibration = 5f;
        public const int RaidShakeFrames = 20;
        /// <summary>P2 长冲途中每 9 帧留一个交错电球（伤害同长冲），角度 = 朝向 + π/4 + (Timer/20)·π/2 + 0.001。沿用旧值 AI.LightningRaid.cs:381-387</summary>
        public const int RaidTrailBallInterval = 9;
        public const float RaidTrailBallAngleDivisor = 20f;
        public const float RaidTrailBallAngleEpsilon = 0.001f;

        //==================== 闪电吐息 / 电磁炮吐息 LightningBreath（AI.LightningBreath.cs）====================

        /// <summary>吐息持续 P1 45 / P2 30 帧；追击最长 P1 180 / P2 150 帧，横向小于 400 且纵向小于 100 即起手。沿用旧值 AI.LightningBreath.cs:14,201,23,210,53</summary>
        public static int BreathBurstFrames(int phase) => phase == 1 ? 45 : 30;
        public static int BreathChaseFrames(int phase) => phase == 1 ? 180 : 150;
        public const float BreathChaseExitX = 400f;
        public const float BreathChaseExitY = 100f;
        /// <summary>绕飞一圈：切向 20 px/f 起飞，每帧转 2π/60，共 45 帧。沿用旧值 AI.LightningBreath.cs:66-77</summary>
        public const float RollSpeed = 20f;
        public const float RollTurnDivisor = 60f;
        public const int BreathRollFrames = 45;
        /// <summary>瞄准：前 P1 10 / P2 20 帧锁向，翅膀帧到 4 后再计 P1 35 / P2 45 帧；期间速度 0.9 衰减。沿用旧值 AI.LightningBreath.cs:91-121,283-308</summary>
        public static int BreathAimLockFrames(int phase) => phase == 1 ? 10 : 20;
        public static int BreathAimFrames(int phase) => phase == 1 ? 35 : 45;
        public const float AimDamp = 0.9f;
        /// <summary>瞄准尘：每帧 3 粒，沿瞄准线 20～1220 px，散角 ±0.3，速度 2～6，缩放 1～1.5；出尘点 = 嘴前 60、角度偏 0.25。沿用旧值 AI.LightningBreath.cs:101-110</summary>
        public const int AimDustCount = 3;
        public const float AimDustMinDistance = 20f;
        public const float AimDustMaxDistance = 1220f;
        public const float AimDustSpread = 0.3f;
        public const float AimDustSpeedMin = 2f;
        public const float AimDustSpeedMax = 6f;
        public const float AimDustScaleMin = 1f;
        public const float AimDustScaleMax = 1.5f;
        public const float AimDustRotOffset = 0.25f;
        /// <summary>P1 吐息弹幕伤害 70/80/90，起点在瞄准线 1800 px 处，ai2 85；P2 电磁炮 100/130/180，起点 2000 px。沿用旧值 AI.LightningBreath.cs:130-134,317-321</summary>
        public static int BreathDamage() => Helper.GetProjDamage(70, 80, 90);
        public const float BreathSpawnDistance = 1800f;
        public const float BreathProjAi2 = 85f;
        public static int BreathCannonDamage() => Helper.GetProjDamage(100, 130, 180);
        public const float BreathCannonSpawnDistance = 2000f;
        /// <summary>出手震屏：P1（方向，20，20，20）、P2（方向×2，24，20，20）；天空亮 0.3 持续 burst/2。沿用旧值 AI.LightningBreath.cs:141,146,328,333</summary>
        public const int BreathShakeStrength = 20;
        public const float BreathShakeVibration = 20f;
        public const int BreathShakeFrames = 20;
        public const float CannonShakeDirScale = 2f;
        public const int CannonShakeStrength = 24;
        public const float BreathSkyLight = 0.3f;
        /// <summary>吐息中残影 1→2 放大并淡出。沿用旧值 AI.LightningBreath.cs:157-159</summary>
        public const float BreathShadowScaleTo = 2f;
        /// <summary>P2 吐息中持续追瞄 0.015 弧度/帧、横向超 500 才翻面；10 帧后每 10 帧震屏（7，12，10）。沿用旧值 AI.LightningBreath.cs:346-361</summary>
        public const float BreathTrackRate = 0.015f;
        public const float BreathTrackFaceX = 500f;
        public const int BreathBurstShakeStart = 10;
        public const int BreathBurstShakeInterval = 10;
        public const int BurstShakeStrength = 7;
        public const float BurstShakeVibration = 12f;
        public const int BreathBurstShakeFrames = 10;
        /// <summary>P2 每次吐息后 5/7 概率追加，最多 3 次。沿用旧值 AI.LightningBreath.cs:368,277-278</summary>
        public const int BreathContinueNum = 5;
        public const int BreathContinueDen = 7;
        public const int BreathMaxBursts = 3;
        /// <summary>后摇 25 帧（吐息 / 放电 / 电磁炮 / 冲刺放电 / 十字雷共用）。沿用旧值 AI.LightningBreath.cs:192</summary>
        public const int RecoverFrames = 25;

        //==================== 落雷 FallingThunder（AI.FallingThunder.cs）====================

        /// <summary>升空高度 700，下砸 12 帧。沿用旧值 AI.FallingThunder.cs:16-17</summary>
        public const float FallingUpLength = 700f;
        public const int FallingSmashFrames = 12;
        /// <summary>追击最长 240 帧；至少 30 帧后横向超 350、纵向小于 250 即起手。沿用旧值 AI.FallingThunder.cs:26,55</summary>
        public const int FallingChaseFrames = 240;
        public const int FallingChaseMinFrames = 30;
        public const float FallingChaseExitX = 350f;
        public const float FallingChaseExitY = 250f;
        /// <summary>吼叫：0.9 衰减，第 15 帧张嘴 + 音效，15～70 帧声波，80 帧后以 8 px/f 下坠起跳。沿用旧值 AI.FallingThunder.cs:73-102</summary>
        public const float FallingRoarDamp = 0.9f;
        public const int RoarCueFrame = 15;
        public const int FallingRoarFxEnd = 70;
        public const int FallingRoarFrames = 80;
        public const float FallingDropSpeed = 8f;
        /// <summary>升空：前 10 帧下坠，第 10 帧 −45 px/f 竖直起飞，10～30 帧淡出，30 帧后隐身悬于目标上方。沿用旧值 AI.FallingThunder.cs:111-133</summary>
        public const int FallingDropFrames = 10;
        public const float FallingAscendSpeed = -45f;
        public const int FallingAscendFrames = 30;
        public const float FallingAscendFadeFrames = 20f;
        /// <summary>选点：前 55 帧落点向“玩家 + 速度×28×(Timer/30)”每帧靠近 20 px（旧代码这个系数不封顶，越晚预判越远），之后锁定到 80 帧。沿用旧值 AI.FallingThunder.cs:145-153</summary>
        public const int FallingAimChaseFrames = 55;
        public const int FallingAimFrames = 80;
        public const float FallingAimPredictFrames = 28f;
        public const float FallingAimPredictRamp = 30f;
        public const float FallingAimMoveStep = 20f;
        /// <summary>落点标记：每帧 2 粒尘（速度 2～4，缩放 1～1.5）；跟随电粒子散布 30，锁定后随时间张到 90。沿用旧值 AI.FallingThunder.cs:155-188</summary>
        public const int FallingAimDustCount = 2;
        public const float FallingAimDustSpeedMin = 2f;
        public const float FallingAimDustSpeedMax = 4f;
        public const float FallingAimFollowSpread = 30f;
        public const float FallingAimFollowSpreadGain = 60f;
        /// <summary>落点跟随电粒子缩放 0.5~0.75。沿用旧值 AI.FallingThunder.cs:165</summary>
        public const float FallingAimFollowScaleMin = 0.5f;
        public const float FallingAimFollowScaleMax = 0.75f;
        /// <summary>落雷：3 道，X 从 −250 起每 500/3 一道，Y 随机 40～300，指向落点下方 250；伤害 P1 70/80/90（ai2 60）、P2 100/130/150（ai2 50），ai0 = 下砸帧 + 8。沿用旧值 AI.FallingThunder.cs:198-211,468-481</summary>
        public const int FallingStrikeCount = 3;
        public const float FallingStrikeSpreadX = 250f;
        public const float FallingStrikeSpanX = 500f;
        public const int FallingStrikeMinY = 40;
        public const int FallingStrikeMaxY = 300;
        public const float FallingStrikeTargetOffsetY = 250f;
        public const int FallingStrikeLeadFrames = 8;
        public static int FallingStrikeDamage(int phase) => phase == 1 ? Helper.GetProjDamage(70, 80, 90) : Helper.GetProjDamage(100, 130, 150);
        public static float FallingStrikeProjAi2(int phase) => phase == 1 ? 60f : 50f;
        /// <summary>落雷天空：出手时亮 0.25 持续 12 帧，落地时亮 0.4 持续 40 帧。沿用旧值 AI.FallingThunder.cs:221,235</summary>
        public const float FallingStrikeSkyLight = 0.25f;
        public const float FallingLandSkyLight = 0.4f;
        public const int FallingLandSkyFrames = 40;
        /// <summary>落地震屏（竖直×1.3，20，22，25）；P2 中途落雷震屏（16，20，20）。沿用旧值 AI.FallingThunder.cs:242,509</summary>
        public const float LandShakeDirY = 1.3f;
        public const int FallingLandShakeStrength = 20;
        public const float FallingLandShakeVibration = 22f;
        public const int FallingLandShakeFrames = 25;
        public const int FallingMidShakeStrength = 16;
        public const float FallingMidShakeVibration = 20f;
        public const int FallingMidShakeFrames = 20;
        /// <summary>落地后 30 帧每 2 帧冒雾（缩放 1.5～2）+ 电光，残影 1→2.5 放大淡出；再飞行 P1 50 / P2 25 帧收招。沿用旧值 AI.FallingThunder.cs:245-265,553-573</summary>
        public const int FallingLandFogFrames = 30;
        public const int FallingLandFogInterval = 2;
        public const float FallingFogScaleMin = 1.5f;
        public const float FallingFogScaleMax = 2f;
        public const float FallingFogAlphaMin = 0.5f;
        public const float FallingFogAlphaMax = 0.8f;
        /// <summary>落地雾散布 = 体型 / 5，电光散布 = 体型 × 0.8，电光缩放 0.1~0.3。沿用旧值 AI.FallingThunder.cs:249-253</summary>
        public const float FallingFogSpreadDiv = 5f;
        public const float FallingSparkSpreadFactor = 0.8f;
        public const float FallingSparkScaleMin = 0.1f;
        public const float FallingSparkScaleMax = 0.3f;
        public const float FallingLandShadowScaleTo = 2.5f;
        public static int FallingLandFlyFrames(int phase) => phase == 1 ? 50 : 25;
        /// <summary>P2 最多连续落雷 3 次，每次落雷后 1/2 概率追加（确定性 AttackRandom）。设计文档“最多连续使用 3 次”，AI.FallingThunder.cs:456-463</summary>
        public const int FallingMaxStrikes = 3;

        //==================== 放电 Discharging（AI.Discharging.cs）====================

        /// <summary>放电爆发 35 帧；蓄力 40 帧且翅膀帧回 0 才挥翅。沿用旧值 AI.Discharging.cs:15,25,57</summary>
        public const int DischargeBurstFrames = 35;
        public const int DischargeReadyFrames = 40;
        /// <summary>蓄力吸入尘：半径从 400 涨到 620（取半），每帧 4 组；挥翅段半径 (40+580)/2。沿用旧值 AI.Discharging.cs:50-54,69</summary>
        public const float DischargeDustEdgeFrom = 400f;
        public const float DischargeDustEdgeGain = 220f;
        public const float DischargeSwingDustEdge = (40f + 580f) / 2f;
        public const int DischargeDustPerFrame = 4;
        /// <summary>挥翅：速度 0.96 衰减，翅膀帧到 4 后 10 帧出手。沿用旧值 AI.Discharging.cs:66,80</summary>
        public const float SwingDamp = 0.96f;
        public const int SwingFrames = 10;
        /// <summary>放电伤害 100/130/180；天空亮 0.5 持续 burst−3、过渡 8；震屏（竖直×1.4，26，26，25）。沿用旧值 AI.Discharging.cs:90,107,111</summary>
        public static int DischargeDamage() => Helper.GetProjDamage(100, 130, 180);
        public const float DischargeSkyLight = 0.5f;
        public const int DischargeSkyFadeOffset = 3;
        public const int DischargeSkyExchange = 8;
        public const float BurstShakeDirY = 1.4f;
        public const int BigBurstShakeStrength = 26;
        public const float BigBurstShakeVibration = 26f;
        public const int BigBurstShakeFrames = 25;
        /// <summary>爆发中残影 1→2.5 放大淡出，张嘴帧每 2 帧推进一格直到回 0。沿用旧值 AI.Discharging.cs:123-137</summary>
        public const float BurstShadowScaleTo = 2.5f;
        public const int BurstMouthFrameTicks = 1;

        //==================== 冲刺放电 DashDischarging（AI.DashDischarging.cs）====================

        /// <summary>长冲最长 60 帧，与玩家距离小于 200 即停；爆发 35 帧。沿用旧值 AI.DashDischarging.cs:14-15,73</summary>
        public const int DashDischargeDashFrames = 60;
        public const float DashDischargeStopDistance = 200f;
        public const float DashDischargeTrackX = 100f;
        /// <summary>
        /// 蓄力扇翅：从 frame.Y=1 起每 6 帧推进一帧、超过第 7 帧起冲 = 7×6 = 42 帧；旧代码不清 frameCounter、时长随上一招残留浮动 ≤ 5 帧，
        /// 现按 Timer 判定并在拍首清零 frameCounter（客户端可重建）。AI.DashDischarging.cs:24-46
        /// </summary>
        public const int DashDischargeFlapFrames = 42;
        public const int DashDischargeFlapTicks = 6;
        public const int DashDischargeFlapStartFrame = 1;
        public const float DashDischargeWindupShadowScale = 1.2f;
        /// <summary>长冲 40 px/f（每帧重新指向玩家）。沿用旧值 AI.DashDischarging.cs:53,69</summary>
        public const float DashDischargeSpeed = 40f;
        /// <summary>蓄力段最长 (7×4)+20 = 48 帧，吸入尘半径 200→620（取半），翅膀帧每 8 帧推进到 4。沿用旧值 AI.DashDischarging.cs:102-117</summary>
        public const int DashDischargeChargeFrames = (7 * 4) + 20;
        public const float DashDischargeDustEdgeFrom = 200f;
        public const float DashDischargeDustEdgeGain = 420f;
        public const int DashDischargeChargeFrameTicks = 7;
        /// <summary>冲刺放电伤害 120/140/200；爆发中速度 0.985 衰减。沿用旧值 AI.DashDischarging.cs:127,161</summary>
        public static int DashDischargeDamage() => Helper.GetProjDamage(120, 140, 200);
        public const float DashDischargeBurstDamp = 0.985f;

        //==================== 电磁炮 ElectromagneticCannon（AI.ElectromagneticCannon.cs；仅由引力雷球接招进入）====================

        /// <summary>电磁炮持续 100 帧；追击最长 240 帧；瞄准锁向 10 帧、翅膀到位后 35 帧出手。沿用旧值 AI.ElectromagneticCannon.cs:13,22,93,118</summary>
        public const int CannonBurstFrames = 100;
        public const int CannonChaseFrames = 240;
        public const int CannonAimLockFrames = 10;
        public const int CannonAimFrames = 35;
        /// <summary>电磁炮伤害 100/130/150，起点在瞄准线 1800 px；音效音调 0.3；天空亮 0.6 持续 3/4 burst、过渡 14。沿用旧值 AI.ElectromagneticCannon.cs:127-142</summary>
        public static int CannonDamage() => Helper.GetProjDamage(100, 130, 150);
        public const float CannonSpawnDistance = 1800f;
        public const float CannonSoundPitch = 0.3f;
        public const float CannonSkyLight = 0.6f;
        public const int CannonSkyExchange = 14;
        /// <summary>发射中追瞄 0.014 弧度/帧、横向超 50 才翻面；每 20 帧震屏（7，12，20）。沿用旧值 AI.ElectromagneticCannon.cs:155-168</summary>
        public const float CannonTrackRate = 0.014f;
        public const float CannonTrackFaceX = 50f;
        public const int CannonShakeInterval = 20;
        public const int CannonBurstShakeFrames = 20;

        //==================== 一二阶段切换 ExchangeP1_P2（AI.ExchangeP1_P2.cs）====================

        /// <summary>电粒子爆发 40 帧，吼叫 60 帧，再 25 帧后收招。沿用旧值 AI.ExchangeP1_P2.cs:14-15,109</summary>
        public const int ExchangeBurstFrames = 40;
        public const int ExchangeRoarFrames = 60;
        public const int ExchangeRoarTail = 25;
        /// <summary>收翅蓄力：速度 0.8 衰减；前 2 帧等翅膀帧到 4；残影两段收束 17 帧（2.5→1）+ 9 帧（1.5→1）。沿用旧值 AI.ExchangeP1_P2.cs:24-58</summary>
        public const float ExchangeReadyDamp = 0.8f;
        public const int ExchangeWaitFrames = 2;
        public const int ExchangeFirstChargeFrames = 17;
        public const int ExchangeSecondChargeFrames = 9;
        public const float ExchangeFirstShadowScaleFrom = 2.5f;
        public const float ExchangeSecondShadowScaleFrom = 1.5f;
        public const float ExchangeRoarShadowScale = 1.2f;
        /// <summary>吼叫电粒子：每帧 5 粒，半径 80→1400，缩放 0.9～1.3；残影 1→2.5 淡出。沿用旧值 AI.ExchangeP1_P2.cs:83-99</summary>
        public const int ExchangeParticlesPerFrame = 5;
        public const float ExchangeRingRadiusFrom = 80f;
        public const float ExchangeRingRadiusTo = 1400f;
        public const float ExchangeParticleScaleMin = 0.9f;
        public const float ExchangeParticleScaleMax = 1.3f;
        public const float ExchangeRoarShadowScaleTo = 2.5f;

        //==================== 引力雷球 GravitationThunder（AI.GravitationThunder.cs）====================

        /// <summary>爆发 40 帧；蓄力 45 帧且翅膀帧回 0；嘴前吸入尘半径 240→100（取半），速度 2～4。沿用旧值 AI.GravitationThunder.cs:14,22-30</summary>
        public const int GravitationBurstFrames = 40;
        public const int GravitationReadyFrames = 45;
        public const float GravitationDustEdgeFrom = 240f;
        public const float GravitationDustEdgeShrink = 140f;
        public const float MouthDustSpeedMin = 2f;
        public const float MouthDustSpeedMax = 4f;
        /// <summary>嘴前吸入尘缩放上限（引力雷球与电球共用）。沿用旧值 AI.GravitationThunder.cs:30、AI.LightningBall.cs:30</summary>
        public const float MouthDustScaleMax = 1.5f;
        /// <summary>引力雷球伤害 150/200/250、初速 2；残影 1→1.5；后摇 30 帧。沿用旧值 AI.GravitationThunder.cs:84-87,101,128</summary>
        public static int GravitationDamage() => Helper.GetProjDamage(150, 200, 250);
        public const float GravitationBallSpeed = 2f;
        public const float SmallBurstShadowScaleTo = 1.5f;
        public const int GravitationRecoverFrames = 30;

        //==================== 电球 LightningBall（AI.LightningBall.cs）====================

        /// <summary>爆发 25 帧；蓄力 25 帧且翅膀帧回 0；吸入尘半径 140→80（取半）。沿用旧值 AI.LightningBall.cs:14,22-25</summary>
        public const int BallBurstFrames = 25;
        public const int BallReadyFrames = 25;
        public const float BallDustEdgeFrom = 140f;
        public const float BallDustEdgeShrink = 60f;
        /// <summary>P1 单电球 35/45/95 初速 2；P2 45/55/125，三选一：三向（±0.35）/ 单球 + 直线链球（链球初速 7）/ 旋转链球。沿用旧值 AI.LightningBall.cs:88-128</summary>
        public static int BallDamageP1() => Helper.GetProjDamage(35, 45, 95);
        public static int BallDamageP2() => Helper.GetProjDamage(45, 55, 125);
        public const float BallSpeed = 2f;
        public const float BallTripleSpread = 0.35f;
        public const float ChainBallSpeed = 7f;
        public const int BallVariantCount = 3;
        /// <summary>电球后摇 10 帧。沿用旧值 AI.LightningBall.cs:171</summary>
        public const int BallRecoverFrames = 10;

        //==================== 十字雷 CrossLightingBall（AI.CrossLightingBall.cs）====================

        /// <summary>爆发 25 帧；蓄力 45 帧；吸入尘速度 4～6、缩放 1～1.8。沿用旧值 AI.CrossLightingBall.cs:14,22,29-30</summary>
        public const int CrossBurstFrames = 25;
        public const int CrossReadyFrames = 45;
        public const float CrossDustSpeedMin = 4f;
        public const float CrossDustSpeedMax = 6f;
        public const float CrossDustScaleMax = 1.8f;
        /// <summary>十字雷球伤害 80/100/160、初速 8；出手天空亮 0.25 持续 20、过渡 6。沿用旧值 AI.CrossLightingBall.cs:84-96</summary>
        public static int CrossDamage() => Helper.GetProjDamage(80, 100, 160);
        public const float CrossBallSpeed = 8f;
        public const float CrossSkyLight = 0.25f;
        public const int CrossSkyFrames = 20;
        public const int CrossSkyExchange = 6;

        //==================== 冥雷 StygianThunder（AI.StygianThunder.cs）====================

        /// <summary>终结雷暴 50 帧；幻影被打破后 140 帧重新显形收招。沿用旧值 AI.StygianThunder.cs:13,26</summary>
        public const int StygianBurstFrames = 50;
        public const int StygianBrokenFrames = 140;
        /// <summary>追击最长 240 帧；横向在 200～600、纵向小于 300 即起手。沿用旧值 AI.StygianThunder.cs:32,64</summary>
        public const int StygianChaseFrames = 240;
        public const float StygianChaseExitXMin = 200f;
        public const float StygianChaseExitXMax = 600f;
        public const float StygianChaseExitY = 300f;
        /// <summary>旋转入背景：切向 20 px/f，每帧转 2π/60 并 0.996 衰减，65 帧内淡出。沿用旧值 AI.StygianThunder.cs:83-91</summary>
        public const int StygianRollFrames = 65;
        public const float StygianRollDamp = 0.996f;
        /// <summary>隐身期悬于目标头顶 400 px。沿用旧值 AI.StygianThunder.cs:131,136</summary>
        public const float StygianHoverOffsetY = -400f;
        /// <summary>
        /// 幻影计数走 ai[2]（从属 <c>ThunderPhantom</c> 每完成一轮雷暴给本体 SonState +1，超过 5 自毁）：
        /// 旧代码进入幻影段时 SonState 恰为 2，所以幻影共打 4 轮。沿用旧值 AI.StygianThunder.cs:114-117、ThunderPhantom.cs:186-188
        /// </summary>
        public const int StygianPhantomSonStateStart = 2;
        public const int StygianPhantomSonStateEnd = 5;
        /// <summary>闪现到头顶后 10 帧显形、速度 0.96 衰减，翅膀到位后 30 帧放终结雷暴。沿用旧值 AI.StygianThunder.cs:137-148</summary>
        public const float StygianFlashFadeFrames = 10f;
        public const float StygianFlashDamp = 0.96f;
        public const int StygianFlashFrames = 30;
        /// <summary>终结雷暴固定伤害 400，从头顶 200 打到脚下 800，ai (20, whoAmI, 70)；天空亮 0.7 持续 50、过渡 12。沿用旧值 AI.StygianThunder.cs:154-176</summary>
        public static int StygianEndDamage() => Helper.GetProjDamage(400, 400, 400);
        public const float StygianEndFromY = -200f;
        public const float StygianEndToY = 800f;
        public const float StygianEndProjAi0 = 20f;
        public const float StygianEndProjAi2 = 70f;
        public const float StygianSkyLight = 0.7f;
        public const int StygianSkyExchange = 12;
        /// <summary>冥雷后摇 20 帧。沿用旧值 AI.StygianThunder.cs:216</summary>
        public const int StygianRecoverFrames = 20;
    }
}
