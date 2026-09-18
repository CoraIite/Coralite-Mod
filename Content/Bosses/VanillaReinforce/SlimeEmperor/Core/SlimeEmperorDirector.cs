using Coralite.Helpers;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core
{
    /// <summary>
    /// 史莱姆皇帝战斗调参中心：全部数字与难度档位的唯一出口。<br/>
    /// 本轮是结构迁移，战斗设计不变：每条常量注明来源，查不到设计理由的一律写“沿用旧值 &lt;旧文件&gt;:&lt;行&gt;”（迁移前的行号）。
    /// 招式集合与循环表的设计口径见同目录上级的 <c>SlimeEmperor_AI_Description.md</c>。
    /// </summary>
    internal static class SlimeEmperorDirector
    {
        //==================== 轮换 / 连接段 ====================

        /// <summary>
        /// hub 停留帧数。旧代码收招当帧即 <c>ResetStates</c> 切下一招、零间隔（喘息已含在各招尾段的回弹里），
        /// 为不改节奏取 0：<c>SlimeEmperorHubState.EndAttack</c> 在 0 时直接经 Commit 返回下一招，不多占一帧。
        /// </summary>
        public const int HubFrames = 0;
        /// <summary>查重窗口长度（记账用，本轮不裁决）：15 个状态取 1/3 ≈ 5。</summary>
        public const int RecentPickWindow = 5;
        /// <summary>
        /// 硬锁一（不与上一手相同）开关。三张轮换表本身就允许同招相邻（普通模式 [远程招式2] 的 0/1 槽都是尖刺凝胶球、
        /// [近战招式2] 的 0/1 槽都是分裂，见 AI 说明的“独立招式循环”），为保战斗设计不变此处关闭，Commit 只做提交口与记账。
        /// </summary>
        public const bool ForbidImmediateRepeat = false;

        /// <summary>普通轮换表长度（0..5 六个阶段位）。沿用旧值 SlimeEmperor.cs:758</summary>
        public const int NormalPhaseCount = 6;
        /// <summary>FTW 轮换表长度（0..4 五个阶段位）。沿用旧值 SlimeEmperor.cs:835</summary>
        public const int FtwPhaseCount = 5;
        /// <summary>[远程招式2] 的独立三循环游标上限。沿用旧值 SlimeEmperor.cs:731</summary>
        public const int Shoot2CycleCount = 3;
        /// <summary>[近战招式2] 普通表的独立三循环游标上限。沿用旧值 SlimeEmperor.cs:752</summary>
        public const int Melee2CycleCount = 3;
        /// <summary>[近战招式2] FTW / 挑战表的独立二循环游标上限。沿用旧值 SlimeEmperor.cs:829,912</summary>
        public const int Melee2CycleCountShort = 2;

        //==================== 通用：形变与超时 ====================

        /// <summary>
        /// 所有状态的超时兜底：单招最长的是聚合射击（落地 + 240 帧蓄力 + 回弹，约 400 帧），
        /// 900 帧仍没退就是软锁（等不到落地、等不到形变阈值），强制收招。
        /// </summary>
        public const int StateTimeoutFrames = 900;
        /// <summary>形变阈值判定的通用容差：|Scale − 1| &lt; 0.05 视为已复原。沿用旧值 AI.BodySlam.cs:252</summary>
        public const float ScaleRestoreEpsilon = 0.05f;
        /// <summary>缩进王冠四拍的单拍保险帧数：形变插值卡住时照样往下走。沿用旧值 AI.BodySlam.cs:23</summary>
        public const int CrownShrinkBeatTimeout = 20;
        /// <summary>缩进王冠四拍的形变目标（泰山压顶 / 王冠冲击 / 移位分裂共用同一段前摇）。沿用旧值 AI.BodySlam.cs:23-49</summary>
        public static readonly Vector2[] CrownShrinkTargets =
        [
            new Vector2(1.1f, 0.7f),
            new Vector2(0.6f, 0.8f),
            new Vector2(0.7f, 0.5f),
            new Vector2(0.4f, 0.6f),
        ];
        /// <summary>四拍的完成阈值，依次看 Y / X / Y / X 小于该值。沿用旧值 AI.BodySlam.cs:23-49</summary>
        public static readonly float[] CrownShrinkDone = [0.75f, 0.65f, 0.55f, 0.45f];
        /// <summary>四拍的判据取哪个轴（true 看 Y）。</summary>
        public static readonly bool[] CrownShrinkDoneByY = [true, false, true, false];
        /// <summary>缩进四拍的插值系数。沿用旧值 AI.BodySlam.cs:23</summary>
        public const float CrownShrinkLerp = 0.1f;
        /// <summary>缩进过程中的速度衰减与帧图循环上限（帧图在 0..3 间滚动）。沿用旧值 AI.CrownStrike.cs:196-210</summary>
        public const float CrownShrinkDamp = 0.96f;
        public const int CrownShrinkFrameMax = 3;
        /// <summary>挑战 SpeedBonus2_1 下形变插值提速倍率。沿用旧值 SlimeEmperor.cs:576</summary>
        public const float ScaleSpeedBonusMult = 3f;

        //==================== 王冠形态（CrownMode / SlimeMode）====================

        /// <summary>王冠形态的防御加成 30；挑战 CrownBonus_1 为 50，CrownBonus_S_2 为 9999（附带霸体与弹幕反弹）。沿用旧值 SlimeEmperor.cs:930-938</summary>
        public const int CrownDefenseBonus = 30;
        public const int CrownDefenseBonusChallenge = 50;
        public const int CrownDefenseBonusInvincible = 9999;
        /// <summary>王冠形态的碰撞箱边长 68×scale（正方形，本体宽高在恢复常规形态时由 PostAI 重算）。沿用旧值 SlimeEmperor.cs:945</summary>
        public const int CrownHitboxSize = 68;
        /// <summary>王冠自旋角速度。沿用旧值 SlimeEmperor.cs:535</summary>
        public const float CrownSpinSpeed = 0.3f;
        /// <summary>出生时王冠先摆在本体顶部再上方 50 px，落下来的那一下就是出场表演。沿用旧值 SlimeEmperor.cs:328</summary>
        public const float CrownSpawnOffsetY = -50f;
        /// <summary>常规形态王冠重力：1.25 倍本体重力、落速上限 16、贴地回正插值 0.04。沿用旧值 SlimeEmperor.cs:509-530</summary>
        public const float CrownGravityMult = 1.25f;
        public const float CrownFallMax = 16f;
        public const float CrownRotationBackLerp = 0.04f;
        /// <summary>王冠横向跟随本体的插值。沿用旧值 SlimeEmperor.cs:505</summary>
        public const float CrownFollowLerpX = 0.5f;
        /// <summary>王冠离“地面”还有 2 px 以上才继续受重力，免得贴住时抖。沿用旧值 SlimeEmperor.cs:507</summary>
        public const float CrownGroundEpsilon = 2f;
        /// <summary>落地弹起门槛：本体落速 &lt; 0.5 且王冠落速 &gt; 10 时以 −0.1 反弹并随机一个倾角。沿用旧值 SlimeEmperor.cs:518-524</summary>
        public const float CrownBounceBodySpeed = 0.5f;
        public const float CrownBounceSpeed = 10f;
        public const float CrownBounceFactor = -0.1f;
        /// <summary>王冠倾角 = clamp(落速/40, 0.1, 0.55)。沿用旧值 SlimeEmperor.cs:521,568</summary>
        public const float CrownAngleDiv = 40f;
        public const float CrownAngleMin = 0.1f;
        public const float CrownAngleMax = 0.55f;

        //==================== 通用跳跃机（Jump）====================

        /// <summary>落地检测的死等上限：600 帧还没检测到地面就当作已落地继续（防止悬空软锁）。沿用旧值 AI.MiniJump.cs:43</summary>
        public const int JumpLandingTimeout = 600;
        /// <summary>落地探测：脚下 2 行、横向 −16..宽+16 每 16 px 一列；静止判定 |velocity.Y| &lt; 0.05。沿用旧值 AI.MiniJump.cs:54-85</summary>
        public const float JumpRestSpeedY = 0.05f;
        public const int JumpProbeStep = 16;
        public const int JumpProbeRows = 2;
        /// <summary>比玩家高 100 px 以上时只认实心块，否则平台也算地面。沿用旧值 AI.MiniJump.cs:56</summary>
        public const float JumpPlatformMargin = 100f;
        /// <summary>落地压扁 (1.25, 0.95) 插值 0.15，X &gt; 1.2 转回弹。沿用旧值 AI.MiniJump.cs:91-92</summary>
        public const float JumpSquashX = 1.25f;
        public const float JumpSquashY = 0.95f;
        public const float JumpScaleLerp = 0.15f;
        public const float JumpSquashDone = 1.2f;
        /// <summary>回弹 (0.85, 1.2)，X &lt; 0.9 时王冠上跳（限速 0.05、初速 4）。沿用旧值 AI.MiniJump.cs:99-102</summary>
        public const float JumpStretchX = 0.85f;
        public const float JumpStretchY = 1.2f;
        public const float JumpStretchDone = 0.9f;
        public const float JumpCrownPopLimit = 0.05f;
        public const float JumpCrownPopSpeed = 4f;
        /// <summary>起跳蓄力 (1.25, 0.85)，帧图每 4 帧前进一格，到第 3 帧离地。沿用旧值 AI.MiniJump.cs:122-131</summary>
        public const float JumpReadyX = 1.25f;
        public const float JumpReadyY = 0.85f;
        public const int JumpFrameInterval = 4;
        public const int JumpTakeOffFrame = 3;
        /// <summary>腾空拉伸：X = clamp(1 − 跳速/15, 0.75, 1)、Y = 1.2；帧图到第 5 帧结束腾空段。沿用旧值 AI.MiniJump.cs:143-146</summary>
        public const float JumpAirScaleYDiv = 15f;
        public const float JumpAirScaleXMin = 0.75f;
        public const float JumpAirStretchY = 1.2f;
        public const int JumpAirLastFrame = 5;
        /// <summary>起跳冲量只在前 4 帧施加，纵向按 (1 − 2t/16) 递减、乘 (2 − 血量比例)；横向 Lerp 0.2 逼近 方向×跳速×(1.8 − 血量比例)。沿用旧值 AI.MiniJump.cs:163-172</summary>
        public const int JumpImpulseFrames = 4;
        public const float JumpImpulseDecayDiv = 16f;
        public const float JumpImpulseLifeBase = 2f;
        public const float JumpRunLifeBase = 1.8f;
        public const float JumpRunLerp = 0.2f;
        /// <summary>离玩家超过 400 px 且面朝反向时横向冲量按 (400..700) 线性收敛到 0（防止背身冲刺跑出屏幕）。沿用旧值 AI.MiniJump.cs:165-166</summary>
        public const float JumpRunawayDistance = 400f;
        public const float JumpRunawayRange = 300f;
        /// <summary>挑战 SpeedBonus1_1：纵向跳速 ×2、横向 ×1.75，并额外叠一次重力倍率。沿用旧值 AI.MiniJump.cs:32-33，SlimeEmperor.cs:373</summary>
        public const float JumpSpeedBonusY = 2f;
        public const float JumpSpeedBonusX = 1.75f;
        public const float GravityBonusMult = 2.5f;

        //==================== 小跳步 MiniJump / 大跳 BigJump ====================

        /// <summary>小跳步：三次 (1, 8) 的小跳。沿用旧值 AI.MiniJump.cs:20</summary>
        public const float MiniJumpSpeedY = 1f;
        public const float MiniJumpSpeedX = 8f;
        public const int MiniJumpCount = 3;
        /// <summary>大跳：一次 (3, 10) 的高跳。沿用旧值 SlimeEmperor.cs:442</summary>
        public const float BigJumpSpeedY = 3f;
        public const float BigJumpSpeedX = 10f;
        /// <summary>大跳起跳时在原地撒 1/2/4/6 个弹力球（−Y 方向 2~5 px/f）。沿用旧值 SlimeEmperor.cs:459-466</summary>
        public static int BigJumpBallCount() => Helper.ScaleValueForDiffMode(1, 2, 4, 6);
        public const float BallScatterSpeedMin = 2f;
        public const float BallScatterSpeedMax = 5f;
        /// <summary>大跳落点：FTW 专属，落地瞬间向上喷 4 发尖刺凝胶球（伤害 20、速度 10、散角 ±0.3）。沿用旧值 SlimeEmperor.cs:445-452</summary>
        public const int BigJumpFtwSpikeCount = 4;
        public const int BigJumpFtwSpikeDamage = 20;
        public const float BigJumpFtwSpikeSpeed = 10f;
        public const float BigJumpFtwSpikeSpread = 0.3f;
        public const float SpikeKnockback = 4f;

        //==================== 回血球 HealGelBall（未实装）====================

        /// <summary>
        /// 落地用的 (2, 6) 小跳。这一招只写了“落地”这一拍，之后是空的，三张轮换表也都没有它，
        /// 现实中进不来；迁移时原样保留，靠状态基类的超时兜底退出而不是软锁。沿用旧值 AI.HealGelBall.cs:10
        /// </summary>
        public const float HealGelBallJumpY = 2f;
        public const float HealGelBallJumpX = 6f;

        //==================== 凝胶射击 GelShoot ====================

        /// <summary>两次 (2, 6) 的小跳，每次落地前的腾空结束时射一轮。沿用旧值 AI.GelShoot.cs:20</summary>
        public const float GelShootJumpY = 2f;
        public const float GelShootJumpX = 6f;
        public const int GelShootVolleys = 2;
        /// <summary>斜射 1/1/3/4 发弹弹凝胶球，速度 12，第 i 发相对连线偏 (1 + 0.3i) 弧度（背向玩家一侧）。沿用旧值 AI.GelShoot.cs:27-34</summary>
        public static int GelShootCount() => Helper.ScaleValueForDiffMode(1, 1, 3, 4);
        public static int GelShootDamage() => Helper.GetProjDamage(75, 95, 115);
        public const float GelShootSpeed = 12f;
        public const float GelShootAngleBase = 1f;
        public const float GelShootAngleStep = 0.3f;
        /// <summary>上抛的那一轮比斜射少一发（至少 1 发），速度 16、散角 ±0.5。沿用旧值 AI.GelShoot.cs:37-42</summary>
        public const float GelShootUpSpeed = 16f;
        public const float GelShootUpSpread = 0.5f;
        public const float GelShootKnockback = 4f;

        //==================== 王冠冲击 CrownStrike ====================

        /// <summary>变王冠瞬间在原地撒 1/2/4/6 个弹力球。沿用旧值 AI.CrownStrike.cs:56-64</summary>
        public static int CrownStrikeBallCount() => Helper.ScaleValueForDiffMode(1, 2, 4, 6);
        /// <summary>绕圈段：100 帧（挑战 SpeedBonus2_1 为 40），与玩家保持 400 px，加速 0.5（0.9），限速 8（14）。沿用旧值 AI.CrownStrike.cs:72-94</summary>
        public const int CrownStrikeCircleFrames = 100;
        public const int CrownStrikeCircleFramesFast = 40;
        public const float CrownStrikeKeepRadius = 400f;
        public const float CrownStrikeAccel = 0.5f;
        public const float CrownStrikeAccelFast = 0.9f;
        public const float CrownStrikeMaxSpeed = 8f;
        public const float CrownStrikeMaxSpeedFast = 14f;
        /// <summary>冲撞段：蓄 30 帧（挑战 20）后锁向出手，冲刺 70 帧（50），速度 10（15）。沿用旧值 AI.CrownStrike.cs:106-114</summary>
        public const int CrownStrikeWaitFrames = 30;
        public const int CrownStrikeWaitFramesFast = 20;
        public const int CrownStrikeDashFrames = 70;
        public const int CrownStrikeDashFramesFast = 50;
        public const float CrownStrikeDashSpeed = 10f;
        public const float CrownStrikeDashSpeedFast = 15f;
        /// <summary>蓄力与刹车段的速度衰减 0.96；冲刺结束后再 5 帧开始刹车、15 帧后转回常规形态。沿用旧值 AI.CrownStrike.cs:119,146-151</summary>
        public const float CrownStrikeDamp = 0.96f;
        public const int CrownStrikeBrakeDelay = 5;
        public const int CrownStrikeEndDelay = 15;
        /// <summary>出手瞬间占 2 帧（锁向 + 12 粒爆散尘，散布 80、速度 0.2~4、缩放 2）。沿用旧值 AI.CrownStrike.cs:124-137</summary>
        public const int CrownStrikeLaunchFrames = 2;
        public const int CrownStrikeLaunchDustCount = 12;
        public const float CrownStrikeDustScatter = 80f;
        public const float CrownStrikeLaunchDustSpeedMax = 4f;
        public const float CrownStrikeDustScale = 2f;
        /// <summary>冲刺途中每帧 2 粒拖尾尘（速度取当前速度的 0.2~0.4 反向）。沿用旧值 AI.CrownStrike.cs:142-144</summary>
        public const int CrownStrikeTrailDustCount = 2;
        public const float CrownStrikeTrailSpeedMin = 0.2f;
        public const float CrownStrikeTrailSpeedMax = 0.4f;
        /// <summary>收招回弹到原大小（插值 0.15，X &gt; 0.97 完成）。沿用旧值 AI.CrownStrike.cs:162</summary>
        public const float CrownStrikeRestoreLerp = 0.15f;
        public const float CrownStrikeRestoreDone = 0.97f;
        /// <summary>大师模式收招时向上喷 2/2/3/4 发尖刺凝胶球（伤害 70/95/115、速度 10、散角 ±0.3）。沿用旧值 AI.CrownStrike.cs:167-174</summary>
        public static int CrownStrikeSpikeCount() => Helper.ScaleValueForDiffMode(2, 2, 3, 4);
        public static int CrownStrikeSpikeDamage() => Helper.GetProjDamage(70, 95, 115);
        public const float CrownStrikeSpikeSpeed = 10f;
        public const float CrownStrikeSpikeSpread = 0.3f;

        //==================== 尖刺凝胶球 SpikeGelBall ====================

        /// <summary>落地后压扁 (1.2, 0.9) 插值 0.2，X &gt; 1.15 时射出。沿用旧值 AI.SpikeGelBall.cs:17</summary>
        public const float SpikeFlatX = 1.2f;
        public const float SpikeFlatY = 0.9f;
        public const float SpikeScaleLerp = 0.2f;
        public const float SpikeFlatDone = 1.15f;
        /// <summary>向上喷 2/2/3/5 发尖刺凝胶球（伤害 75/95/115、速度 10、散角 ±0.3）。沿用旧值 AI.SpikeGelBall.cs:21-31</summary>
        public static int SpikeCount() => Helper.ScaleValueForDiffMode(2, 2, 3, 5);
        public static int SpikeDamage() => Helper.GetProjDamage(75, 95, 115);
        public const float SpikeSpeed = 10f;
        public const float SpikeSpread = 0.3f;
        /// <summary>回弹 (0.8, 1.25)，Y &gt; 1.2 转复原；复原 (1, 1) 到 |X − 1| &lt; 0.05。沿用旧值 AI.SpikeGelBall.cs:37-40</summary>
        public const float SpikeStretchX = 0.8f;
        public const float SpikeStretchY = 1.25f;
        public const float SpikeStretchDone = 1.2f;
        /// <summary>落地前的就位小跳 (2, 8)，与黏黏凝胶、分裂、僚机共用。沿用旧值 AI.SpikeGelBall.cs:14</summary>
        public const float GroundJumpY = 2f;
        public const float GroundJumpX = 8f;

        //==================== 黏黏凝胶 StickyGel ====================

        /// <summary>压扁与复原的插值比尖刺球慢（0.05），中间的回弹仍是 0.2。沿用旧值 AI.StickyGel.cs:17,23,44</summary>
        public const float StickySlowLerp = 0.05f;
        /// <summary>射出 2/2/2/4 发黏黏凝胶（伤害 75/95/115、速度 8、以连线反向为基准散 ±2 弧度）。沿用旧值 AI.StickyGel.cs:27-37</summary>
        public static int StickyCount() => Helper.ScaleValueForDiffMode(2, 2, 2, 4);
        public static int StickyDamage() => Helper.GetProjDamage(75, 95, 115);
        public const float StickySpeed = 8f;
        public const float StickySpread = 2f;
        public const float StickyKnockback = 4f;

        //==================== 凝胶僚机 GelFlippy ====================

        /// <summary>压扁 (1.2, 0.9) / 回弹 (0.8, 1.25) 插值 0.2，回弹到位时召唤 1/1/2/2 只僚机。沿用旧值 AI.GelFlippy.cs:19-30</summary>
        public static int FlippyCount() => Helper.ScaleValueForDiffMode(1, 1, 2, 2);
        /// <summary>召唤后给玩家的反应时间：一次 (1, 6) 小跳，再一次 (4, 6) 高跳（挑战 SpeedBonus3_1 直接收招）。沿用旧值 AI.GelFlippy.cs:36-44</summary>
        public const float FlippyJump1Y = 1f;
        public const float FlippyJump2Y = 4f;
        public const float FlippyJumpX = 6f;

        //==================== 分裂 Split ====================

        /// <summary>压扁 (1.25, 0.8) / 回弹 (0.8, 1.25) 插值 0.1，回弹到位时分裂出 1/1/2/2 个分身。沿用旧值 AI.Split.cs:20-32</summary>
        public const float SplitSquashX = 1.25f;
        public const float SplitSquashY = 0.8f;
        public const float SplitScaleLerp = 0.1f;
        public const float SplitSquashDone = 1.2f;
        public static int SplitAvatarCount() => Helper.ScaleValueForDiffMode(1, 1, 2, 2);
        /// <summary>分裂前后各一次 (2, 8) 小跳（挑战 SpeedBonus3_1 省掉后一次）。沿用旧值 AI.Split.cs:17,46</summary>
        public const float SplitJumpY = 2f;
        public const float SplitJumpX = 8f;

        //==================== 聚合射击 PolymerizeShot ====================

        /// <summary>就位小跳 (1, 8)（比其它招的落地跳更低，因为落地后要长时间站桩蓄力）。沿用旧值 AI.PolymerizeShot.cs:19</summary>
        public const float PolyGroundJumpY = 1f;
        public const float PolyGroundJumpX = 8f;
        /// <summary>落地后压扁 (1.2, 0.9) 插值 0.2，X &gt; 1.15 时召回全部分身（分身进入 −1 态朝本体飘）。沿用旧值 AI.PolymerizeShot.cs:22-31</summary>
        public const float PolyCompressLerp = 0.2f;
        /// <summary>蓄力 240/240/150/100 帧（挑战 SpeedBonus3_1 为 60）。沿用旧值 AI.PolymerizeShot.cs:45-48</summary>
        public static int PolymerizeFrames() => Helper.ScaleValueForDiffMode(240, 240, 150, 100);
        public const int PolymerizeFramesChallenge = 60;
        /// <summary>蓄力粒子：每帧 4 粒内旋尘（半径 200 → 20）与 2 粒直冲尘（半径 300 → 60）。沿用旧值 AI.PolymerizeShot.cs:54-70</summary>
        public const int PolyRingDustCount = 4;
        public const float PolyRingRadius = 200f;
        public const float PolyRingShrink = 180f;
        public const int PolyBeamDustCount = 2;
        public const float PolyBeamRadius = 300f;
        public const float PolyBeamShrink = 240f;
        /// <summary>每吞掉一个分身：伤害 +10、弹幕缩放 +0.2、发数 +1、速度 +1；基础 20/25/30 伤害、3 发、速度 10、缩放 1。沿用旧值 AI.PolymerizeShot.cs:75-84</summary>
        public static int PolyBaseDamage() => Helper.GetProjDamage(20, 25, 30);
        public const int PolyDamagePerAvatar = 10;
        public const float PolyBaseScale = 1f;
        public const float PolyScalePerAvatar = 0.2f;
        public const float PolyBaseCount = 3f;
        public const float PolyCountPerAvatar = 1f;
        public const float PolyBaseSpeed = 10f;
        public const float PolySpeedPerAvatar = 1f;
        /// <summary>上限：速度 28、缩放 4、发数 25。沿用旧值 AI.PolymerizeShot.cs:96-101</summary>
        public const float PolyMaxSpeed = 28f;
        public const float PolyMaxScale = 4f;
        public const float PolyMaxCount = 25f;
        /// <summary>弹幕散角 ±0.4、速度抖动 ±2、击退 4。沿用旧值 AI.PolymerizeShot.cs:109-110</summary>
        public const float PolyShootSpread = 0.4f;
        public const float PolyShootSpeedJitter = 2f;
        public const float PolyKnockback = 4f;
        /// <summary>同时向玩家方向丢 3 个弹力球（相对连线 −0.3 / 0 / +0.3 弧度，速度 4~8）。沿用旧值 AI.PolymerizeShot.cs:117-124</summary>
        public const float PolyBallAngleStep = 0.3f;
        public const float PolyBallSpeedMin = 4f;
        public const float PolyBallSpeedMax = 8f;
        /// <summary>吞噬分身时每个分身处 8 粒尘；出手时 发数×8 粒尘。沿用旧值 AI.PolymerizeShot.cs:87,128</summary>
        public const int PolyAvatarDustCount = 8;
        public const float PolyShootDustPerCount = 8f;
        /// <summary>后摇三拍：(0.75, 1.35) 插值 0.2 → (1.2, 0.9) 插值 0.15 → (1, 1) 插值 0.1。沿用旧值 AI.PolymerizeShot.cs:141-155</summary>
        public const float PolyRecoilX = 0.75f;
        public const float PolyRecoilY = 1.35f;
        public const float PolyRecoilDone = 1.3f;
        public const float PolyRecoil2Lerp = 0.15f;
        public const float PolyRestoreLerp = 0.1f;

        //==================== 瞬移（泰山压顶 / 移位分裂共用）====================

        /// <summary>蓄 60 帧（挑战 SpeedBonus2_1 为 30）后瞬移。沿用旧值 AI.BodySlam.cs:60-62，AI.TransportSplit.cs:59</summary>
        public const int TeleportReadyFrames = 60;
        public const int TeleportReadyFramesFast = 30;
        /// <summary>蓄力尘：每帧 6 粒，散布半径从 80 收到 20（收拢本身就是预告）。沿用旧值 AI.BodySlam.cs:66-72</summary>
        public const int TeleportChargeDustCount = 6;
        public const float TeleportChargeWidth = 80f;
        public const float TeleportChargeShrink = 60f;
        /// <summary>瞬移轨迹尘：沿途每 8 px 一粒、散布 48、速度 0.2~3、缩放 1.5~2；三分之一概率用传送尘、其余用凝胶尘。沿用旧值 AI.BodySlam.cs:87-97</summary>
        public const int TeleportTrailTeleporterOdds = 3;
        public const int TeleportTrailStep = 8;
        public const float TeleportTrailScatter = 48f;
        public const float TeleportTrailSpeedMin = 0.2f;
        public const float TeleportTrailSpeedMax = 3f;
        public const float TeleportTrailScaleMin = 1.5f;
        public const float TeleportTrailScaleMax = 2f;
        /// <summary>落点爆散尘：24 粒均分一圈、散布 80、速度 6~8。沿用旧值 AI.BodySlam.cs:99-104</summary>
        public const int TeleportRingDustCount = 24;
        public const float TeleportRingScatter = 80f;
        public const float TeleportRingSpeedMin = 6f;
        public const float TeleportRingSpeedMax = 8f;

        //==================== 泰山压顶 BodySlam ====================

        /// <summary>瞬移落点：玩家正上方 450 px。沿用旧值 AI.BodySlam.cs:82</summary>
        public const float BodySlamHeight = -450f;
        /// <summary>瞬移时在旧位置留 1/2/4/6 个弹力球。沿用旧值 AI.BodySlam.cs:108-116</summary>
        public static int BodySlamBallCount() => Helper.ScaleValueForDiffMode(1, 2, 4, 6);
        /// <summary>头顶追踪 90 帧（挑战 SpeedBonus2_1 为 60）：加速 0.75、限速 9、离锚点 30 px 内改为反推。沿用旧值 AI.BodySlam.cs:130-145</summary>
        public const int BodySlamChaseFrames = 90;
        public const int BodySlamChaseFramesFast = 60;
        public const float BodySlamChaseAccel = 0.75f;
        public const float BodySlamChaseMaxSpeed = 9f;
        public const float BodySlamKeepRadius = 30f;
        /// <summary>追踪后悬停蓄力 25 帧（衰减 0.95，每帧 4 粒尘）——这是下砸前唯一的预告窗。沿用旧值 AI.BodySlam.cs:149-157</summary>
        public const int BodySlamHoverFrames = 25;
        public const float BodySlamHoverDamp = 0.95f;
        public const int BodySlamHoverDustCount = 4;
        /// <summary>出砸：形变 (0.8, 1.25)、横向速度减半、纵向 8 起步。沿用旧值 AI.BodySlam.cs:165-169</summary>
        public const float BodySlamDropScaleX = 0.8f;
        public const float BodySlamDropScaleY = 1.25f;
        public const float BodySlamDropDampX = 0.5f;
        public const float BodySlamDropSpeedY = 8f;
        /// <summary>下坠：加速 1.4、上限 24，第 12 帧起拉残影；120 帧内没砸到地面就强制收招。沿用旧值 AI.BodySlam.cs:177-183</summary>
        public const float BodySlamFallAccel = 1.4f;
        public const float BodySlamFallMax = 24f;
        public const int BodySlamShadowFrame = 12;
        public const int BodySlamFallTimeout = 120;
        /// <summary>只在不高于玩家 100 px 时检测脚下（第 1~2 行）物块。沿用旧值 AI.BodySlam.cs:185-190</summary>
        public const float BodySlamGroundMargin = 100f;
        public const int BodySlamGroundRowFrom = 1;
        public const int BodySlamGroundRowTo = 3;
        /// <summary>砸地：原版女王史莱姆震地弹幕，伤害 40；20 粒烟尘（上抛 5~8、横向 ×7）。沿用旧值 AI.BodySlam.cs:200-208</summary>
        public const int BodySlamSmashDamage = 40;
        public const int BodySlamSmokeCount = 20;
        public const float BodySlamSmokeHeight = 30f;
        public const float BodySlamSmokeUpMin = 5f;
        public const float BodySlamSmokeUpMax = 3f;
        public const float BodySlamSmokeSpreadX = 7f;
        /// <summary>砸地后四拍回弹：(1.3, 0.7) 0.2 → (0.7, 1.3) 0.15 → (1.2, 0.85) 0.15 → (1, 1) 0.1。沿用旧值 AI.BodySlam.cs:226-252</summary>
        public const float BodySlamBounce1X = 1.3f;
        public const float BodySlamBounce1Y = 0.7f;
        public const float BodySlamBounce1Lerp = 0.2f;
        public const float BodySlamBounce1Done = 0.75f;
        public const float BodySlamBounce2X = 0.7f;
        public const float BodySlamBounce2Y = 1.3f;
        public const float BodySlamBounce2Lerp = 0.15f;
        public const float BodySlamBounce2Done = 0.75f;
        public const float BodySlamBounce3X = 1.2f;
        public const float BodySlamBounce3Y = 0.85f;
        public const float BodySlamBounce3Done = 0.9f;
        public const float BodySlamRestoreLerp = 0.1f;
        /// <summary>回弹第二拍时王冠被顶起（限速 10、初速 3）。沿用旧值 AI.BodySlam.cs:244</summary>
        public const float BodySlamCrownPopLimit = 10f;
        public const float BodySlamCrownPopSpeed = 3f;
        /// <summary>大师模式回弹时向上喷 1/1/3/4 发弹弹凝胶球（伤害 75/95/115、速度 16、散角 ±0.5）。沿用旧值 AI.BodySlam.cs:234-241</summary>
        public static int BodySlamGelBallCount() => Helper.ScaleValueForDiffMode(1, 1, 3, 4);
        public static int BodySlamGelBallDamage() => Helper.GetProjDamage(75, 95, 115);
        public const float BodySlamGelBallSpeed = 16f;
        public const float BodySlamGelBallSpread = 0.5f;
        public const float BodySlamGelBallKnockback = 4f;

        //==================== 移位分裂 TransportSplit ====================

        /// <summary>落点：玩家与自身连线的中点横坐标、玩家上方 200 px。沿用旧值 AI.TransportSplit.cs:78</summary>
        public const float TransportSplitHeight = -200f;
        /// <summary>原地留下 1/1/2/2 个分身，落点撒 1/2/4/6 个弹力球。沿用旧值 AI.TransportSplit.cs:85,110</summary>
        public static int TransportSplitAvatarCount() => Helper.ScaleValueForDiffMode(1, 1, 2, 2);
        public static int TransportSplitBallCount() => Helper.ScaleValueForDiffMode(1, 2, 4, 6);
        /// <summary>落点后绕圈 40 帧（挑战 SpeedBonus3_1 为 20）：保持 400 px、加速 0.5、限速 8。沿用旧值 AI.TransportSplit.cs:133-148</summary>
        public const int TransportSplitCircleFrames = 40;
        public const int TransportSplitCircleFramesFast = 20;
        public const float TransportSplitKeepRadius = 400f;
        public const float TransportSplitAccel = 0.5f;
        public const float TransportSplitMaxSpeed = 8f;

        //==================== 通用表现 / 脱战 / 死亡 ====================

        /// <summary>凝胶尘的统一配色与 alpha。沿用旧值 AI.Split.cs:61-62</summary>
        public static readonly Color GelDustColor = new(78, 136, 255, 80);
        public const int GelDustAlpha = 150;
        /// <summary>形变时的凝胶飞沫：6 粒，散布取 (宽, 高) × Scale × 0.7，速度 0.2~1 再减半。沿用旧值 AI.Split.cs:57-64</summary>
        public const int SplitDustCount = 6;
        public const float SplitDustBoxScale = 0.7f;
        public const float SplitDustSpeedMin = 0.2f;
        public const float SplitDustSpeedMax = 1f;
        public const float SplitDustSlow = 0.5f;
        public const float SplitDustScale = 2f;
        /// <summary>脱战：向上 0.3 px/f² 抬升并鼓励消失。沿用旧值 SlimeEmperor.cs:383-385</summary>
        public const float DespawnRiseAccel = 0.3f;
        public const int DespawnEncourageFrames = 10;
        /// <summary>死亡演出：只有一帧——掉王冠 gore 然后真死。沿用旧值 SlimeEmperor.cs:422-433</summary>
        public const int KillAnimFrame = 1;
        /// <summary>挑战失败判定：武器稀有度上限 LightRed / 45 分，护甲 Pink / 9 分。沿用旧值 SlimeEmperor.cs:540,544</summary>
        public const int WeaponLimitScore = 45;
        public const int ArmorLimitScore = 9;
        /// <summary>限次挑战：HitLimit_3 十次、HitLimit_S_5 一次。沿用旧值 SlimeEmperor.cs:316-318</summary>
        public const int HitLimitNormal = 10;
        public const int HitLimitStrict = 1;
        /// <summary>体型：常规形态按血量比例缩放的宽高上限。沿用旧值 SlimeEmperor.cs:95-96</summary>
        public const int BodyWidthMax = 158;
        public const int BodyHeightMax = 100;
        /// <summary>血量比例的下限：残血也只缩到 0.65，不然判定盒小到打不中。沿用旧值 SlimeEmperor.cs:78</summary>
        public const float LifeScaleMin = 0.65f;

        //==================== 生成位与粒子细节（按招式分组；全部沿用旧值，只是从招式体里收上来）====================

        /// <summary>弹幕出生位的随机抖动：以 (宽/3, 高/3) 为半径。沿用旧值 AI.GelShoot.cs:33</summary>
        public const int ProjSpawnJitterDiv = 3;
        /// <summary>弹力球出生位的纵向抖动（横向用本体宽度）。沿用旧值 SlimeEmperor.cs:464</summary>
        public const int BallScatterOffsetY = 32;
        /// <summary>砸地烟尘的 alpha。沿用旧值 AI.BodySlam.cs:205</summary>
        public const int SmokeDustAlpha = 40;

        /// <summary>王冠冲击出手爆散尘的速度下限（上限见 <see cref="CrownStrikeLaunchDustSpeedMax"/>）。沿用旧值 AI.CrownStrike.cs:133</summary>
        public const float CrownStrikeLaunchDustSpeedMin = 0.2f;

        /// <summary>瞬移蓄力尘：速度取当前速度反向的 0.2~0.4、缩放 1~1.5（落点爆散尘共用缩放）。沿用旧值 AI.BodySlam.cs:70-71,102-103</summary>
        public const float TeleportChargeDustSpeedMin = 0.2f;
        public const float TeleportChargeDustSpeedMax = 0.4f;
        public const float TeleportDustScaleMin = 1f;
        public const float TeleportDustScaleMax = 1.5f;

        /// <summary>泰山压顶悬停蓄力尘：散布 80、速度取当前速度反向的 0.2~0.4、缩放 2。沿用旧值 AI.BodySlam.cs:154-156</summary>
        public const float BodySlamHoverDustScatter = 80f;
        public const float BodySlamHoverDustSpeedMin = 0.2f;
        public const float BodySlamHoverDustSpeedMax = 0.4f;
        public const float BodySlamHoverDustScale = 2f;

        /// <summary>聚合射击蓄力内旋尘：出生位再抖 30、切向速度 0.3~2、缩放 2。沿用旧值 AI.PolymerizeShot.cs:57-59</summary>
        public const float PolyRingDustJitter = 30f;
        public const float PolyRingDustSpeedMin = 0.3f;
        public const float PolyRingDustSpeedMax = 2f;
        public const float PolyRingDustScale = 2f;
        /// <summary>聚合射击蓄力直冲尘：向心速度 4~8、缩放 1.2。沿用旧值 AI.PolymerizeShot.cs:66-68</summary>
        public const float PolyBeamDustSpeedMin = 4f;
        public const float PolyBeamDustSpeedMax = 8f;
        public const float PolyBeamDustScale = 1.2f;
        /// <summary>吞噬分身时的连线尘：沿连线取 0~70 px、再抖 48、速度 2~6.5、缩放 1.2。沿用旧值 AI.PolymerizeShot.cs:89-90</summary>
        public const int PolyAvatarDustRange = 70;
        public const float PolyAvatarDustScatter = 48f;
        public const float PolyAvatarDustSpeedMin = 2f;
        public const float PolyAvatarDustSpeedMax = 6.5f;
        public const float PolyAvatarDustScale = 1.2f;
        /// <summary>出手爆散尘：散布 64、速度 2~8、缩放 2。沿用旧值 AI.PolymerizeShot.cs:131-132</summary>
        public const float PolyShootDustScatter = 64f;
        public const float PolyShootDustSpeedMin = 2f;
        public const float PolyShootDustSpeedMax = 8f;
        public const float PolyShootDustScale = 2f;
        /// <summary>聚合射击三个弹力球的出生抖动（横纵都是 32，与其它招的“横向取本体宽度”不同）。沿用旧值 AI.PolymerizeShot.cs:120-121</summary>
        public const int PolyBallScatterOffset = 32;
    }
}
