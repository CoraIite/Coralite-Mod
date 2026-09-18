using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core
{
    /// <summary>运动声明模式；宿主 <c>ApplyDeclaredMovement</c> 两端同跑地把它翻译成 velocity。</summary>
    public enum SlimeEmperorMoveMode
    {
        /// <summary>不干预：重力与物块碰撞自理（本 boss 绝大多数时间都在这个模式，跳跃冲量由跳跃机在两端各自施加）。</summary>
        Keep,
        /// <summary>整体衰减 <c>velocity *= DampFactor</c>（蓄力、刹车）。</summary>
        Damp,
        /// <summary>本帧速度已由状态直接写死（王冠冲刺、下砸），宿主不再动它。</summary>
        Direct,
    }

    /// <summary>跳跃机本帧抛出的事件；旧代码的三个回调（onJumpFinish / onLanding / onStartJump）改为返回值，由权威端消费。</summary>
    public enum SlimeJumpEvent
    {
        None,
        /// <summary>腾空段结束（旧 <c>onJumpFinish</c>）。</summary>
        JumpFinished,
        /// <summary>落地压扁回弹完毕、即将再次蓄力（旧 <c>onLanding</c>）。</summary>
        Landed,
        /// <summary>蓄力完毕离地（旧 <c>onStartJump</c>）。</summary>
        JumpStarted,
    }

    /// <summary>跳跃机主状态。沿用旧值 AI.MiniJump.cs:197-202</summary>
    public enum SlimeJumpStates
    {
        CheckForLanding = 0,
        ReadyToJump = 1,
        Jumping = 2,
    }

    /// <summary>
    /// 史莱姆皇帝 FSM 上下文：每帧声明总线 + 跨帧事实 + 全招式共用的跳跃机。<br/>
    /// 旧代码里不过线的 <c>localAI[0..1]</c>（跳跃机）与 <c>movePhase / movingMode / shoot2State / melee2State</c> 全部搬到这里：<br/>
    /// · 跳跃机状态与手感缩放随热字段过线（见 <see cref="SlimeEmperorStateBase.WriteHot"/>），客户端中途加入也能接上同一拍；<br/>
    /// · 形态与轮换记账跨招式存活、热字段只在状态对得上时收养，所以走 <see cref="WriteFacts"/>。
    /// </summary>
    public sealed class SlimeEmperorContext : CoraliteBossContext
    {
        public SlimeEmperor Boss { get; }

        /// <summary>当前目标玩家（<c>Npc.target</c> 原版同步；无效时指向 255 号占位玩家，不会为 null）。</summary>
        public Player Target => Main.player[Npc.target];

        public SlimeEmperorContext(SlimeEmperor boss) : base(boss.NPC, boss)
        {
            Boss = boss;
            Array.Fill(RecentPicks, -1);
            // 已迁移：关闭 6 px/f 遗留阀，改走决策点 MarkDecision + 45 帧心跳 + 纠偏器。
            UseLegacySpeedValve = false;
        }

        #region 事实（跨帧）

        /// <summary>CheckDead 请求死亡演出（权威端写，<see cref="SlimeEmperorStateBase"/> 的 ServerUpdate 消费）。</summary>
        public bool KillRequested { get; set; }

        /// <summary>目标丢失后重新锁定，需要立刻收招重排（旧 <c>AI()</c> 里的 <c>ResetStates()</c> 分支）。</summary>
        public bool RetargetRequested { get; set; }

        /// <summary>血量比例，钳在 0.65~1；体型、跳跃力度、绘制缩放都吃它。沿用旧值 SlimeEmperor.cs:78</summary>
        public float LifePercentScale => Math.Clamp(Npc.life / (float)Npc.lifeMax, SlimeEmperorDirector.LifeScaleMin, 1f);

        /// <summary>手感缩放（纯表现量，但招式拍点读它作完成判据，故随热字段过线）。</summary>
        public Vector2 Scale
        {
            get => Boss.Scale;
            set => Boss.Scale = value;
        }

        /// <summary>轮换表游标。旧 <c>movePhase</c>。</summary>
        public int MovePhase { get; set; }
        /// <summary>[远程招式2] 独立循环游标。旧 <c>shoot2State</c>。</summary>
        public int Shoot2State { get; set; }
        /// <summary>[近战招式2] 独立循环游标。旧 <c>melee2State</c>。</summary>
        public int Melee2State { get; set; }
        /// <summary>是否处于王冠形态。旧 <c>movingMode</c>（0 常规 / 1 王冠）。</summary>
        public bool CrownForm { get; set; }

        /// <summary>跳跃机主状态。旧 <c>localAI[0]</c>。</summary>
        public int JumpState { get; set; }
        /// <summary>跳跃机内计时兼子阶段（负值是落地后的压扁 / 回弹 / 复原三段）。旧 <c>localAI[1]</c>。</summary>
        public int JumpTimer { get; set; }

        /// <summary>上一手提交的状态 id（-1 无）。</summary>
        public int LastPickedState { get; set; } = -1;

        /// <summary>最近几手的环形记录（查重记账，本轮只记不裁决）。</summary>
        public int[] RecentPicks { get; } = new int[SlimeEmperorDirector.RecentPickWindow];
        private int recentPickCursor;

        /// <summary>过账：写上一手 + 推环形窗口。所有出招都必须经 hub 的 Commit 到这里。</summary>
        public void RecordPick(int stateId)
        {
            LastPickedState = stateId;
            RecentPicks[recentPickCursor] = stateId;
            recentPickCursor = (recentPickCursor + 1) % RecentPicks.Length;
        }

        /// <summary>窗口内某招出现次数。</summary>
        public int RecentCountOf(int stateId)
        {
            int count = 0;
            for (int i = 0; i < RecentPicks.Length; i++)
            {
                if (RecentPicks[i] == stateId)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>危险挑战开关查询（知识系统本身不改动，只是收口）。</summary>
        public bool Dangerous(Slime1Knowledge.Dangerous flag) => Boss.Knowledge.DangerousSet(flag);

        /// <summary>每帧只读事实：本 boss 的朝向只在起跳时由 <c>TargetClosest</c> 刷新，这里不覆盖。</summary>
        public void UpdateFacts()
        {
        }

        #endregion

        #region 声明通道（每帧重声明，BeginFrameDefaults 回默认）

        internal SlimeEmperorMoveMode MoveMode { get; set; }
        /// <summary>Damp 模式的衰减系数。</summary>
        public float DampFactor { get; set; }

        /// <summary>本帧无敌（<c>dontTakeDamage</c>）。原版不同步这个标志，必须两端每帧同声明。</summary>
        public bool Invulnerable { get; set; }
        /// <summary>本帧反弹弹幕（王冠形态 + CrownBonus_S_2）。同样不被原版同步。</summary>
        public bool ReflectsProjectiles { get; set; }
        /// <summary>本帧霸体。</summary>
        public bool SuperArmor { get; set; }
        /// <summary>本帧画残影（下砸、脱战上升）。</summary>
        public bool DrawShadow { get; set; }

        public override void BeginFrameDefaults()
        {
            base.BeginFrameDefaults();

            MoveMode = SlimeEmperorMoveMode.Keep;
            DampFactor = 1f;
            Invulnerable = false;
            ReflectsProjectiles = false;
            SuperArmor = false;
            DrawShadow = false;
        }

        /// <summary>整体衰减。</summary>
        public void DeclareDamp(float factor)
        {
            MoveMode = SlimeEmperorMoveMode.Damp;
            DampFactor = factor;
        }

        /// <summary>状态自管速度（本帧已直接写 velocity）。</summary>
        public void DeclareDirect() => MoveMode = SlimeEmperorMoveMode.Direct;

        #endregion

        #region 同步事实

        /// <summary>形态与三个轮换游标：跨招式存活，热字段只在状态对得上时收养，所以放这里定长过线。</summary>
        protected override void WriteFacts(BinaryWriter writer)
        {
            writer.Write((byte)(CrownForm ? 1 : 0));
            writer.Write((byte)Math.Clamp(MovePhase, 0, byte.MaxValue));
            writer.Write((byte)Math.Clamp(Shoot2State, 0, byte.MaxValue));
            writer.Write((byte)Math.Clamp(Melee2State, 0, byte.MaxValue));
        }

        protected override void ReadFacts(BinaryReader reader)
        {
            CrownForm = reader.ReadByte() != 0;
            MovePhase = reader.ReadByte();
            Shoot2State = reader.ReadByte();
            Melee2State = reader.ReadByte();
        }

        #endregion

        #region 形变与形态切换

        /// <summary>
        /// 招前公共清理（双端跑，客户端经 ai[0] 被动切招时同样需要）。<br/>
        /// 残影与无敌已改为每帧声明通道，这里只留碰撞标志与起跳判定。沿用旧值 SlimeEmperor.cs:412-419
        /// </summary>
        public void PrepareAttack()
        {
            Npc.noTileCollide = false;
            Npc.noGravity = false;
            StartJump();
        }

        /// <summary>
        /// 手感缩放向目标插值，并返回是否已达到给定判据。旧 <c>ScaleToTarget</c>（SlimeEmperor.cs:572-581）的返回值版：
        /// 旧写法把“到没到”与“到了做什么”缝在一个回调里，拆开后换拍才能只在权威端发生。
        /// </summary>
        public void ScaleTo(float targetX, float targetY, float amount)
        {
            if (Dangerous(Slime1Knowledge.Dangerous.SpeedBonus2_1))
            {
                amount *= SlimeEmperorDirector.ScaleSpeedBonusMult;
            }

            Scale = Vector2.Lerp(Scale, new Vector2(targetX, targetY), amount);
        }

        /// <summary>缩放已复原（|Scale.X − 1| 与 |Scale.Y − 1| 任一到位即可，按各招旧判据分别调用）。</summary>
        public bool ScaleRestored(bool byY = false)
            => Math.Abs((byY ? Scale.Y : Scale.X) - 1f) < SlimeEmperorDirector.ScaleRestoreEpsilon;

        /// <summary>
        /// 切王冠形态的一次性部分（两端都要跑：判定盒与碰撞标志原版不同步）。<br/>
        /// 每帧生效的那部分（防御、霸体、反弹、无重力、判定盒尺寸）由宿主 <c>ApplyDeclaredMovement</c> 按 <see cref="CrownForm"/> 重算。
        /// 沿用旧值 SlimeEmperor.cs:926-947
        /// </summary>
        public void EnterCrownForm()
        {
            CrownForm = true;
            MarkDecision();
        }

        /// <summary>切回常规史莱姆形态的一次性部分：王冠归位并随机一个倾角。沿用旧值 SlimeEmperor.cs:952-965</summary>
        public void EnterSlimeForm()
        {
            CrownForm = false;
            Npc.noTileCollide = false;
            Npc.noGravity = false;
            Boss.ResetCrownToTop();
            MarkDecision();
        }

        #endregion

        #region 跳跃机（全招式共用）

        /// <summary>
        /// 起跳前的落地判定：脚下有实心物块且几乎静止就直接进蓄力，否则先等落地。<br/>
        /// 沿用旧值 AI.MiniJump.cs:180-195
        /// </summary>
        public void StartJump()
        {
            JumpTimer = 0;
            if (Math.Abs(Npc.velocity.Y) < 0.1f)
            {
                for (int i = 0; i < Npc.width; i += SlimeEmperorDirector.JumpProbeStep)
                {
                    Tile tile = Framing.GetTileSafely(Npc.BottomLeft + new Vector2(i, 0));
                    if (tile.HasReallySolidTile())
                    {
                        JumpState = (int)SlimeJumpStates.ReadyToJump;
                        Npc.frame.Y = 0;
                        return;
                    }
                }
            }

            JumpState = (int)SlimeJumpStates.CheckForLanding;
        }

        /// <summary>
        /// 通用跳跃机，双端同跑：落地检测 → 压扁 / 回弹 / 复原 → 蓄力 → 腾空施加冲量。<br/>
        /// 旧三个回调改为返回本帧事件，调用方在 <c>AuthorityUpdate</c> 里据此换拍与生成。<br/>
        /// 沿用旧值 AI.MiniJump.cs:28-175
        /// </summary>
        public SlimeJumpEvent UpdateJump(float jumpYVelocity, float jumpXVelocity)
        {
            if (Dangerous(Slime1Knowledge.Dangerous.SpeedBonus1_1))
            {
                jumpYVelocity *= SlimeEmperorDirector.JumpSpeedBonusY;
                jumpXVelocity *= SlimeEmperorDirector.JumpSpeedBonusX;
            }

            switch ((SlimeJumpStates)JumpState)
            {
                default:
                case SlimeJumpStates.CheckForLanding:
                    return UpdateJumpLanding();
                case SlimeJumpStates.ReadyToJump:
                    return UpdateJumpReady();
                case SlimeJumpStates.Jumping:
                    return UpdateJumpAir(jumpYVelocity, jumpXVelocity);
            }
        }

        /// <summary>落地检测与落地后的三段形变。</summary>
        private SlimeJumpEvent UpdateJumpLanding()
        {
            switch (JumpTimer)
            {
                default:
                    if (JumpTimer > SlimeEmperorDirector.JumpLandingTimeout)
                    {
                        //死等太久（悬空 / 被顶住），直接当已落地进蓄力，避免软锁
                        Npc.frame.Y = 0;
                        Npc.noGravity = true;
                        JumpState = (int)SlimeJumpStates.ReadyToJump;
                        JumpTimer = -1;
                        break;
                    }

                    JumpTimer++;

                    if (Math.Abs(Npc.velocity.Y) < SlimeEmperorDirector.JumpRestSpeedY && OnGround())
                    {
                        Npc.frame.Y = 0;
                        Npc.noGravity = true;
                        JumpTimer = -1;
                    }

                    break;

                case -1: //刚落地的变扁
                    Npc.velocity *= 0f;
                    Scale = Vector2.Lerp(Scale, new Vector2(SlimeEmperorDirector.JumpSquashX, SlimeEmperorDirector.JumpSquashY), SlimeEmperorDirector.JumpScaleLerp);
                    if (Scale.X > SlimeEmperorDirector.JumpSquashDone)
                    {
                        JumpTimer = -2;
                    }

                    break;

                case -2: //回弹，王冠被顶起
                    Scale = Vector2.Lerp(Scale, new Vector2(SlimeEmperorDirector.JumpStretchX, SlimeEmperorDirector.JumpStretchY), SlimeEmperorDirector.JumpScaleLerp);
                    if (Scale.X < SlimeEmperorDirector.JumpStretchDone)
                    {
                        Boss.CrownJumpUp(SlimeEmperorDirector.JumpCrownPopLimit, SlimeEmperorDirector.JumpCrownPopSpeed);
                        JumpTimer = -3;
                    }

                    break;

                case -3: //回到正常大小
                    Scale = Vector2.Lerp(Scale, Vector2.One, SlimeEmperorDirector.JumpScaleLerp);
                    if (ScaleRestored(true))
                    {
                        Scale = Vector2.One;
                        JumpState = (int)SlimeJumpStates.ReadyToJump;
                        JumpTimer = 0;
                        return SlimeJumpEvent.Landed;
                    }

                    break;
            }

            return SlimeJumpEvent.None;
        }

        /// <summary>脚下两行内有可站立物块（比玩家高出一截时只认实心块）。沿用旧值 AI.MiniJump.cs:56-85</summary>
        private bool OnGround()
        {
            bool solidOnly = Npc.Center.Y < Target.Center.Y - SlimeEmperorDirector.JumpPlatformMargin;
            for (int i = -SlimeEmperorDirector.JumpProbeStep; i < Npc.width + SlimeEmperorDirector.JumpProbeStep; i += SlimeEmperorDirector.JumpProbeStep)
            {
                for (int j = 0; j < SlimeEmperorDirector.JumpProbeRows; j++)
                {
                    Tile tile = Framing.GetTileSafely(Npc.BottomLeft + new Vector2(i, j));
                    if (!tile.HasTile)
                    {
                        continue;
                    }

                    if (Main.tileSolid[tile.TileType])
                    {
                        return true;
                    }

                    if (!solidOnly && (Main.tileSolidTop[tile.TileType] || TileID.Sets.Platforms[tile.TileType]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>蓄力：帧图推进到第 3 帧离地。</summary>
        private SlimeJumpEvent UpdateJumpReady()
        {
            Npc.frameCounter++;
            if (Npc.frameCounter > SlimeEmperorDirector.JumpFrameInterval)
            {
                Npc.frame.Y++;
                Npc.frameCounter = 0;
            }

            Scale = Vector2.Lerp(Scale, new Vector2(SlimeEmperorDirector.JumpReadyX, SlimeEmperorDirector.JumpReadyY), SlimeEmperorDirector.JumpScaleLerp);

            if (Npc.frame.Y == SlimeEmperorDirector.JumpTakeOffFrame)
            {
                Npc.noGravity = false;
                JumpState = (int)SlimeJumpStates.Jumping;
                JumpTimer = 0;
                Npc.TargetClosest();
                return SlimeJumpEvent.JumpStarted;
            }

            return SlimeJumpEvent.None;
        }

        /// <summary>腾空：帧图推进 + 前 4 帧施加递减的起跳冲量，背身跑远时横向冲量线性收敛。</summary>
        private SlimeJumpEvent UpdateJumpAir(float jumpYVelocity, float jumpXVelocity)
        {
            float targetScaleX = Math.Clamp(1 - (jumpYVelocity / SlimeEmperorDirector.JumpAirScaleYDiv), SlimeEmperorDirector.JumpAirScaleXMin, 1f);
            Scale = Vector2.Lerp(Scale, new Vector2(targetScaleX, SlimeEmperorDirector.JumpAirStretchY), SlimeEmperorDirector.JumpScaleLerp);

            SlimeJumpEvent result = SlimeJumpEvent.None;
            if (Npc.frame.Y < SlimeEmperorDirector.JumpAirLastFrame)
            {
                Npc.frameCounter++;
                if (Npc.frameCounter > SlimeEmperorDirector.JumpFrameInterval)
                {
                    Npc.frameCounter = 0;
                    Npc.frame.Y++;
                }
            }
            else
            {
                JumpTimer = 0;
                JumpState = (int)SlimeJumpStates.CheckForLanding;
                Npc.noGravity = false;
                result = SlimeJumpEvent.JumpFinished;
            }

            if (JumpTimer < SlimeEmperorDirector.JumpImpulseFrames)
            {
                float distanceX = MathF.Abs(Npc.Center.X - Target.MountedCenter.X);
                if (distanceX > SlimeEmperorDirector.JumpRunawayDistance && Npc.direction != MathF.Sign(Target.MountedCenter.X - Npc.Center.X))
                {
                    jumpXVelocity *= Math.Clamp(1 - ((distanceX - SlimeEmperorDirector.JumpRunawayDistance) / SlimeEmperorDirector.JumpRunawayRange), 0, 1);
                }

                float decay = 1 - (2 * JumpTimer / SlimeEmperorDirector.JumpImpulseDecayDiv);
                Npc.velocity.Y -= jumpYVelocity * decay * (SlimeEmperorDirector.JumpImpulseLifeBase - LifePercentScale);
                Npc.velocity.X = MathHelper.Lerp(Npc.velocity.X, Npc.direction * jumpXVelocity * (SlimeEmperorDirector.JumpRunLifeBase - LifePercentScale), SlimeEmperorDirector.JumpRunLerp);
            }

            JumpTimer++;
            return result;
        }

        #endregion
    }
}
