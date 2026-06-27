using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        /// <summary>
        /// 连招步进（本地量，由已同步的外层状态 ID 约束，不参与网络同步）
        /// </summary>
        internal int Combo { get; set; }

        /// <summary>
        /// 当前的攻击状态（读取已同步的 <c>ai[0]</c>，由 FSM 写入与同步）
        /// </summary>
        public AIStates State => (AIStates)NPC.ai[0];

        /// <summary>
        /// 招式内部子状态，占用同步的 <c>ai[2]</c>
        /// </summary>
        internal ref float SonState => ref NPC.ai[2];
        /// <summary>
        /// 同步计时器，占用同步的 <c>ai[3]</c>
        /// </summary>
        internal ref float Timer => ref NPC.ai[3];

        internal ref float Recorder => ref NPC.localAI[0];
        internal ref float Recorder2 => ref NPC.localAI[1];
        /// <summary>
        /// 上一个状态，记录这个防止复读（仅服务端选招时读写，无需同步）
        /// </summary>
        internal AIStates StateRecorder
        {
            get => (AIStates)NPC.localAI[2];
            set => NPC.localAI[2] = (int)value;
        }

        /// <summary>连招解锁阈值计数（仅服务端选招时读写，无需同步）</summary>
        internal ref float UseMoveCount => ref NPC.localAI[3];

        /// <summary>
        /// 紫电计数（经 <see cref="SendExtraAI"/> 同步给客户端供血条绘制）
        /// </summary>
        public float PurpleVoltCount { get; set; }

        public Point[] oldFrame;
        public int[] oldDirection;

        public int oldSpriteDirection;

        private bool init = true;

        private HashSet<AIStates> comboRecords;

        #region FSM 基座

        internal ZacurrentDragonContext AiContext;
        internal CoraliteBossStateMachine<ZacurrentDragonContext> StateMachine;
        internal Random AttackRandom;
        private bool aiBootstrapped;

        /// <summary>招式是否完成（包壳法：招式方法返回值在两端写入，仅服务端据此推进状态）</summary>
        internal bool AttackFinished;

        public void RefreshAttackRandom()
        {
            AttackRandom = AiContext?.CreateAttackRandom() ?? new Random(NPC.whoAmI + 1);
        }

        /// <summary>招内确定性随机：从已同步的 AttackSeed 派生，两端同序调用结果一致。</summary>
        internal float AttackRandFloat(float min, float max) => min + ((max - min) * (float)AttackRandom.NextDouble());
        internal int AttackRandSign() => AttackRandom.Next(2) == 0 ? -1 : 1;

        /// <summary>登场后首次闪电突袭固定 Recorder2=1（旧 onSpawnAnmi 行为）。</summary>
        internal bool ForceRecorder2OnNextLightningRaid;

        private void EnsureAiMachine()
        {
            if (aiBootstrapped)
                return;

            AiContext = new ZacurrentDragonContext(this);
            StateMachine = new CoraliteBossStateMachine<ZacurrentDragonContext>(AiContext);
            StateMachine.SetInitialState(VaultStateRegistry<ZacurrentDragonContext>.Create((int)AIStates.onSpawnAnmi));
            RefreshAttackRandom();
            aiBootstrapped = true;
        }

        /// <summary>仅服务端：强制切换顶层状态（客户端经 ai[0] 同步跟随）。</summary>
        internal void ForceState(AIStates state)
        {
            if (VaultUtils.isClient || StateMachine == null)
                return;

            StateMachine.ChangeState((int)state);
        }

        #endregion

        #region AI控制部分

        public enum AIStates
        {
            /// <summary>
            /// 等待，就是啥也不干只正常飞一飞
            /// </summary>
            Waiting,

            //动画阶段
            onSpawnAnmi,
            onKillAnim,

            /// <summary> 紫伏形态的切换 </summary>
            PurpleVoltExchange,

            //单招
            /// <summary> 闪电突袭，先短冲后进行一次长冲 </summary>
            LightningRaidNormal,
            /// <summary> 电流吐息，小 </summary>
            ElectricBreathSmall,
            /// <summary> 电流吐息，中 </summary>
            ElectricBreathMiddle,
            /// <summary> 电球 </summary>
            ElectricBall,
            /// <summary> 冲刺放电 </summary>
            DashDischarging,

            //2阶段单招
            /// <summary> 2阶段指针电球 </summary>
            PointerBall,
            /// <summary> 2阶段指针电球 </summary>
            LightningRaidVolt,

            //连段
            /// <summary>
            /// 吼叫=》电球=》引力电球=》电磁炮=》聚集电流
            /// </summary>
            NormalRoarCombo1,
            /// <summary>
            /// 吼叫=》闪电链=》闪电突袭=》聚集电流
            /// </summary>
            NormalRoarCombo2,
            /// <summary>
            /// 闪电链=》落雷=》冲刺放电=》聚集电流
            /// </summary>
            NormalChainCombo,
            /// <summary>
            /// 指针电球=》闪电链=》电流吐息（中）=》落雷=》聚集电流
            /// </summary>
            NormalPointerCombo,

            /// <summary>
            /// 二阶段超长连段<br></br>
            /// 吼叫=》闪电链=》 循环x3（  指针电球=》闪电突袭x1  ）=》引力电球（超长持续时间）<br></br>
			/// =》电流吐息（中）=》Z电球 =》电伏击穿 =》落雷
            /// </summary>
            VoltBigCombo,
            /// <summary>
            /// 二阶段短连招：闪电链=》电伏击穿=》电流吐息（中）
            /// </summary>
            VoltChainCombo,
            /// <summary>
            /// Z电球=》闪电突袭=》电伏击穿=》指针电流=》落雷
            /// </summary>
            VoltZBallChainCombo,

            //调整身位用招式
            SmallDash,
            SmallDashVolt,

            //其他
            /// <summary>
            /// 从紫伏状态回归，给玩家一些输出时间
            /// </summary>
            Break
        }

        public override void AI()
        {
            if (init)
            {
                Initialize();
                init = false;
            }

            EnsureAiMachine();

            if (CheckTarget())
                return;

            UpdateSky();

            StateMachine.Update();
        }

        public bool CheckTarget()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000 || Main.dayTime)
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 4500 || Main.dayTime)//没有玩家存活时离开
                {
                    NPC.dontTakeDamage = true;
                    canDrawShadows = false;
                    IsDashing = true;
                    NPC.velocity.X *= 0.98f;
                    NPC.velocity.Y = -60;
                    NPC.rotation = NPC.velocity.ToRotation();
                    NPC.EncourageDespawn(30);
                    return true;
                }
            }

            return false;
        }

        public void Initialize()
        {
            ResetAllOldCaches();
            NPC.netUpdate = true;

            if (!VaultUtils.isServer && !SkyManager.Instance["ZacurrentSky"].IsActive())//如果这个天空没激活
            {
                SkyManager.Instance.Activate("ZacurrentSky");
            }
        }

        /// <summary>击杀动画（包壳法：双端跑视觉，权威端负责 <see cref="NPC.Kill"/>）。</summary>
        public void OnKillAnim()
        {
            NPC.velocity = new Vector2(0, -2);
            IsDashing = false;

            shadowAlpha = Math.Clamp(Timer / 60f, 0, 1);

            if (Timer % 4 == 0 && !VaultUtils.isServer)
            {
                float speed = Main.rand.NextFloat(45, 60);
                Vector2 dir = Helper.NextVec2Dir();
                PurpleThunderParticle.Spawn(() => NPC.Center, dir * speed
                    , 14, 7, 7, 70, ZacurrentRed);
            }

            Timer++;
            if (Timer > 120)
            {
                Helper.PlayPitched(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                if (!VaultUtils.isServer)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        float factor = i / 30f;
                        float length = Helper.Lerp(80, 600, factor);

                        for (int j = 0; j < 4; j++)
                        {
                            PRTLoader.NewParticle(NPC.Center + Main.rand.NextVector2CircularEdge(length, length),
                                Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Red>(), Scale: Main.rand.NextFloat(0.9f, 1.3f));
                        }
                    }
                }

                SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, NPC.Center);
                if (!VaultUtils.isClient)
                    NPC.Kill();
            }
        }

        /// <summary>紫伏击穿后的恢复期（包壳法：双端跑视觉，返回是否结束）。</summary>
        public bool BreakState()
        {
            int time = Helper.ScaleValueForDiffMode(60 * 8, 60 * 6, 60 * 5, 60);
            if (Timer == 0)
            {
                Vector2 f() => GetMousePos() + new Vector2(0, -50);
                DizzyStar.Spawn(NPC.Center, -1.57f, time, 10, f);
                DizzyStar.Spawn(NPC.Center, 1.57f, time, 10, f);

                Helper.PlayPitched(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                if (!VaultUtils.isServer)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        float factor = i / 30f;
                        float length = Helper.Lerp(80, 600, factor);

                        for (int j = 0; j < 4; j++)
                        {
                            PRTLoader.NewParticle(NPC.Center + Main.rand.NextVector2CircularEdge(length, length),
                                Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Red>(), Scale: Main.rand.NextFloat(0.9f, 1.3f));
                        }
                    }
                }

                SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, NPC.Center);
            }

            NPC.velocity *= 0.9f;
            FlyingFrame();
            TurnToNoRot();
            Timer++;
            return Timer > time;
        }

        public override void PostAI()
        {
            oldSpriteDirection = NPC.spriteDirection;

            if (!VaultUtils.isServer && currentSurrounding)
            {
                Lighting.AddLight(NPC.Center, ZacurrentPink.ToVector3());
                if (Main.rand.NextBool(3))
                {
                    Vector2 offset = Main.rand.NextVector2Circular(100 * NPC.scale, 70 * NPC.scale);
                    ElectricParticle_PurpleFollow.Spawn(NPC.Center, offset, () => NPC.Center, Main.rand.NextFloat(0.75f, 1f));
                }
            }
        }

        public static void UpdateSky()
        {
            if (VaultUtils.isServer)
                return;

            ZacurrentSky sky = (ZacurrentSky)SkyManager.Instance["ZacurrentSky"];
            if (sky.Timeleft < 100)
                sky.Timeleft += 2;
            if (sky.Timeleft > 100)
                sky.Timeleft = 100;
        }

        public static void SetBackgroundLight(float light, int fadeTime, int exchangeTime = 5)
        {
            if (VaultUtils.isServer)
                return;

            ZacurrentSky sky = (ZacurrentSky)SkyManager.Instance["ZacurrentSky"];
            sky.ExchangeTime = sky.MaxExchangeTime = exchangeTime;
            sky.targetLight = light;
            sky.oldLight = sky.light;
            sky.LightTime = fadeTime;
        }

        #endregion

        #region AI切换部分

        /// <summary>
        /// 重新设置各类与状态相关的数值
        /// </summary>
        public void ResetFields(bool resetCombo = true)
        {
            if (resetCombo)
                Combo = 0;
            SonState = 0;
            Timer = 0;
            Recorder = 0;
            Recorder2 = 0;

            OpenMouse = false;
            IsDashing = false;
            canDrawShadows = false;

            shadowScale = 1;
            shadowAlpha = 1;

            NPC.dontTakeDamage = false;
        }

        /// <summary>
        /// 仅服务端：选取下一个顶层状态。<br/>
        /// 把旧 <c>ChangeState</c> 的全部权重业务（远近、上下区域降概率、防复读 StateRecorder、
        /// 连招解锁阈值 UseMoveCount、紫伏/普通招池切换）搬进此处构造 <see cref="WeightedRandomPicker{T}"/> entries，
        /// 用 <c>Main.rand.Next()</c> 取一次 seed，结果以状态 ID 经 <c>ai[0]</c> 同步，客户端结构上无选招入口。
        /// </summary>
        public IVaultState<ZacurrentDragonContext> PickNextState()
        {
            if (VaultUtils.isClient || StateMachine == null)
                return null;

            //记录旧状态，不包括短冲（防复读）
            if (State is not AIStates.SmallDash and not AIStates.SmallDashVolt)
                StateRecorder = State;

            //进入紫伏状态
            if (!PurpleVolt && PurpleVoltCount == GetPurpleVoltMax())
            {
                comboRecords?.Clear();
                return VaultStateRegistry<ZacurrentDragonContext>.Create((int)AIStates.PurpleVoltExchange);
            }

            List<(AIStates, float)> entries = [];
            if (PurpleVolt)//紫电状态的招池
                BuildPurpleVoltPool(entries);
            else
                BuildNormalPool(entries);//正常状态的招池

            //防复读：移除上一个状态（保证仍有候选项）
            if (entries.Any(p => p.Item1 != StateRecorder))
                entries.RemoveAll(p => p.Item1 == StateRecorder);

            AIStates pick = new WeightedRandomPicker<AIStates>(entries).Pick(Main.rand.Next()).Item;
            RecordCombo(pick);
            return VaultStateRegistry<ZacurrentDragonContext>.Create((int)pick);
        }

        private void BuildPurpleVoltPool(List<(AIStates, float)> entries)
        {
            entries.Add((AIStates.ElectricBreathSmall, 0.5f));//小吐息概率降低

            //在玩家上下区域的时候减少概率
            bool upOrDown
                = MathF.Abs(Target.Center.X - NPC.Center.X) < 16 * 8
                && MathF.Abs(Target.Center.Y - NPC.Center.Y) > 16 * 6;
            entries.Add((AIStates.ElectricBreathMiddle, upOrDown ? 0.4f : 1));

            entries.Add((AIStates.ElectricBall, 0.5f));//普通电球概率降低
            entries.Add((AIStates.PointerBall, 1));

            //距离远的时候提升使用概率
            bool farAway = NPC.Distance(Target.Center) > 650;
            float farawayPercent = farAway
               ? (1.5f + (NPC.Distance(Target.Center) - 650) / 400)
               : 1;
            entries.Add((AIStates.DashDischarging, farawayPercent));
            entries.Add((AIStates.LightningRaidVolt, farawayPercent));
            //防止复读短冲
            if (State != AIStates.SmallDashVolt)
                entries.Add((AIStates.SmallDashVolt, farawayPercent + 3f));

            UseMoveCount++;
            if (UseMoveCount > Helper.ScaleValueForDiffMode(5, 4, 3, 2))
            {
                if (comboRecords != null && comboRecords.Count > 1)
                    AddCombo(entries, AIStates.VoltBigCombo);
                AddCombo(entries, AIStates.VoltChainCombo);
                AddCombo(entries, AIStates.VoltZBallChainCombo);
            }
        }

        private void BuildNormalPool(List<(AIStates, float)> entries)
        {
            entries.Add((AIStates.ElectricBreathSmall, 1));

            //在玩家上下区域的时候减少概率
            bool upOrDown
                = MathF.Abs(Target.Center.X - NPC.Center.X) < 16 * 8
                && MathF.Abs(Target.Center.Y - NPC.Center.Y) > 16 * 6;
            entries.Add((AIStates.ElectricBreathMiddle, upOrDown ? 0.4f : 1));
            entries.Add((AIStates.ElectricBall, 1));

            //距离远的时候提升使用概率
            bool farAway = NPC.Distance(Target.Center) > 650;
            float farawayPercent = farAway
               ? (1.5f + (NPC.Distance(Target.Center) - 650) / 400)
               : 1;
            entries.Add((AIStates.DashDischarging, farawayPercent));
            entries.Add((AIStates.LightningRaidNormal, farawayPercent));
            //防止复读短冲
            if (State != AIStates.SmallDash)
                entries.Add((AIStates.SmallDash, farawayPercent + 1.5f));

            UseMoveCount++;
            if (UseMoveCount > Helper.ScaleValueForDiffMode(5, 4, 3, 2))
            {
                AddCombo(entries, AIStates.NormalRoarCombo1);
                AddCombo(entries, AIStates.NormalRoarCombo2);
                AddCombo(entries, AIStates.NormalChainCombo);
                AddCombo(entries, AIStates.NormalPointerCombo);
            }
        }

        private void RecordCombo(AIStates pick)
        {
            if (pick is AIStates.NormalChainCombo or AIStates.NormalRoarCombo1 or AIStates.NormalRoarCombo2 or AIStates.NormalPointerCombo
                or AIStates.VoltBigCombo or AIStates.VoltChainCombo or AIStates.VoltZBallChainCombo)
            {
                comboRecords.Add(pick);
                UseMoveCount = 0;
            }
        }

        /// <summary>
        /// 添加一个连招
        /// </summary>
        private void AddCombo(List<(AIStates, float)> entries, AIStates combo)
        {
            comboRecords ??= new HashSet<AIStates>();
            if (comboRecords.Count > 2)
                comboRecords.Clear();
            if (!comboRecords.Contains(combo))//没用过的连招才行
                entries.Add((combo, UseMoveCount));
        }

        /// <summary>
        /// 获得紫电
        /// </summary>
        /// <param name="red"></param>
        public void GetPurpleVolt(bool red)
        {
            if (VaultUtils.isClient)
                return;

            /*
             * 默认此阶段DPS有3800
             * 平均2-3次蓄电即可进入紫伏状态，如果一个电球都没打碎那么只需要1次
             * 
             * 红色电流提供1/3的单次蓄电量，每次有6个红色电
             * 每次有24个紫色电球
             * 难度越高上限越高，同时越难打破
             */
            int count = red ? Helper.ScaleValueForDiffMode(30, 40, 50, 80)
                : Helper.ScaleValueForDiffMode(15, 20, 25, 40);

            PurpleVoltCount += count;
            if (PurpleVoltCount > GetPurpleVoltMax())
                PurpleVoltCount = GetPurpleVoltMax();

            NPC.netUpdate = true;
        }

        /// <summary>登场：双端设位，完成后由服务端切入首招（对齐旧 onSpawnAnmi 单帧时间轴）。</summary>
        public bool OnSpawnAnmi()
        {
            NPC.TargetClosest();
            NPC.Center = Target.Center + new Vector2(0, -1500);
            return true;
        }

        public int GetPurpleVoltMax()
            => Helper.ScaleValueForDiffMode(30, 40, 50, 80) * 3 * 5 * 2;

        #endregion

        #region 各种helper

        public void ElectricSound()
        {
            Helper.PlayPitched("Electric/ElectricStrike" + Main.rand.NextFromList(0, 2).ToString(), 0.4f, -0.2f, NPC.Center);
        }

        public void GetLengthToTargetPos(Vector2 targetPos, out float xLength, out float yLength)
        {
            xLength = NPC.Center.X - targetPos.X;
            yLength = NPC.Center.Y - targetPos.Y;

            xLength = Math.Abs(xLength);
            yLength = Math.Abs(yLength);
        }

        /// <summary>
        /// 向上飞，会改变Y速度
        /// </summary>
        /// <param name="acc">加速度</param>
        /// <param name="velMax">速度最大值</param>
        /// <param name="slowDownPercent">减速率</param>
        public void FlyingUp(float acc, float velMax, float slowDownPercent)
        {
            FlyingFrame();

            if (NPC.frame.Y <= 4)
            {
                NPC.velocity.Y -= acc;
                if (NPC.velocity.Y > velMax)
                    NPC.velocity.Y = velMax;
            }
            else
                NPC.velocity.Y *= slowDownPercent;
        }

        /// <summary>
        /// 获取嘴巴的位置
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMousePos()
        {
            return NPC.Center + ((NPC.rotation + (NPC.direction * 0.07f)).ToRotationVector2() * 60 * NPC.scale);
        }

        public void FlyingFrame()
        {
            int frameCounterMax = 4;
            float speed = NPC.velocity.Length();
            if (speed > 8)
                frameCounterMax--;
            if (speed > 14)
                frameCounterMax--;

            if (++NPC.frameCounter > frameCounterMax)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 7)
                    NPC.frame.Y = 0;
            }
        }

        /// <summary>
        /// 根据Y方向速度设置旋转
        /// </summary>
        public void SetRotationNormally(float rate = 0.08f)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += MathHelper.Pi;
            float targetRot = (NPC.velocity.Y * 0.05f * NPC.spriteDirection) + (NPC.spriteDirection > 0 ? 0 : MathHelper.Pi);
            NPC.rotation = NPC.rotation.AngleLerp(targetRot, rate);
        }

        /// <summary>
        /// 将身体回正
        /// </summary>
        /// <param name="rate"></param>
        public void TurnToNoRot(float rate = 0.2f)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;

            NPC.rotation = NPC.rotation.AngleLerp(NPC.spriteDirection > 0 ? 0 : MathHelper.Pi, rate);
        }

        private void SetSpriteDirectionFoTarget(Vector2? targetPos = null, float limit = 48)
        {
            Vector2 p = targetPos ?? Target.Center;
            if (MathF.Abs(p.X - NPC.Center.X) > limit)
                NPC.spriteDirection = p.X > NPC.Center.X ? 1 : -1;
        }

        public void InitOldFrame()
        {
            if (VaultUtils.isServer)
                return;

            oldFrame ??= new Point[trailCacheLength];
            for (int i = 0; i < trailCacheLength; i++)
                oldFrame[i] = new Point(NPC.frame.X, NPC.frame.Y);
        }

        public void InitOldDirection()
        {
            if (VaultUtils.isServer)
                return;

            oldDirection ??= new int[trailCacheLength];
            for (int i = 0; i < trailCacheLength; i++)
                oldDirection[i] = NPC.spriteDirection;
        }

        public void UpdateOldFrame()
        {
            if (VaultUtils.isServer)
                return;

            for (int i = 0; i < oldFrame.Length - 1; i++)
                oldFrame[i] = oldFrame[i + 1];
            oldFrame[^1] = new Point(NPC.frame.X, NPC.frame.Y);
        }

        public void UpdateOldDirection()
        {
            if (VaultUtils.isServer)
                return;

            for (int i = 0; i < oldDirection.Length - 1; i++)
                oldDirection[i] = oldDirection[i + 1];
            oldDirection[^1] = NPC.spriteDirection;
        }

        public void ResetAllOldCaches()
        {
            if (VaultUtils.isServer)
                return;

            NPC.InitOldPosCache(trailCacheLength);
            NPC.InitOldRotCache(trailCacheLength);
            InitOldFrame();
            InitOldDirection();
        }

        public void UpdateAllOldCaches()
        {
            if (VaultUtils.isServer)
                return;

            NPC.UpdateOldPosCache();
            NPC.UpdateOldRotCache();
            UpdateOldFrame();
            UpdateOldDirection();
        }

        #endregion
    }
}
