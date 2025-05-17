using Coralite.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class ZacurrentDragon
    {
        /// <summary>
        /// 连招，有一部分连招只有单段
        /// </summary>
        internal int Combo { get; set; }

        /// <summary>
        /// 用于记录当前的攻击状态
        /// </summary>
        public AIStates State
        {
            get => (AIStates)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        /// <summary>
        /// 阶段内部使用
        /// </summary>
        internal ref float SonState => ref NPC.ai[1];
        internal ref float RecorderAI => ref NPC.ai[2];
        internal ref float Timer => ref NPC.ai[3];

        internal ref float Recorder => ref NPC.localAI[0];
        internal ref float Recorder2 => ref NPC.localAI[1];
        /// <summary>
        /// 上一个状态，记录这个防止复读
        /// </summary>
        internal AIStates StateRecorder
        {
            get => (AIStates)NPC.localAI[2];
            set => NPC.localAI[2] = (int)value;
        }

        internal ref float UseMoveCount => ref NPC.localAI[3];

        /// <summary>
        /// 紫电计数
        /// </summary>
        public int PurpleVoltCount { get; set; }

        public Point[] oldFrame;
        public int[] oldDirection;

        public int oldSpriteDirection;

        private bool init = true;

        private HashSet<AIStates> comboRecords;

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
            LightningRaid,
            /// <summary> 电流吐息，小 </summary>
            ElectricBreathSmall,
            /// <summary> 电流吐息，中 </summary>
            ElectricBreathMiddle,
            /// <summary> 电球 </summary>
            ElectricBall,
            /// <summary> 冲刺放电 </summary>
            DashDischarging,
            /// <summary> 闪电链， </summary>
            ThunderChain,

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

            //调整身位用招式
            SmallDash,

            //其他
        }

        public override void AI()
        {
            if (init)
            {
                Initialize();
                init = false;
            }

            if (CheckTarget())
                return;

            UpdateSky();

            switch (State)
            {
                default:
                case AIStates.Waiting:
                    State = AIStates.LightningRaid;
                    NPC.TargetClosest();
                    break;
                case AIStates.onSpawnAnmi:
                    State = AIStates.LightningRaid;
                    NPC.TargetClosest();

                    break;
                case AIStates.onKillAnim:
                    break;
                case AIStates.PurpleVoltExchange:
                    if (PurpleVoltExchange())
                    {
                        ResetFields();
                        PurpleVolt = true;
                        ChangeState();
                    }
                    break;
                case AIStates.LightningRaid:
                    if (LightningRaidNoraml())
                    {
                        ResetFields();
                        ChangeState();
                    }
                    break;
                case AIStates.ThunderChain:
                    break;
                case AIStates.SmallDash:
                    if (SmallDash())
                    {
                        ResetFields();
                        ChangeState();
                    }
                    break;
                case AIStates.ElectricBreathSmall:
                    if (ElectricBreathSmall())
                    {
                        ResetFields();
                        ChangeState();
                    }
                    break;
                case AIStates.ElectricBreathMiddle:
                    if (ElectricBreathMiddle())
                    {
                        ResetFields();
                        ChangeState();
                    }
                    break;
                case AIStates.ElectricBall:
                    if (ElectricBall())
                    {
                        ResetFields();
                        ChangeState();
                    }
                    break;
                case AIStates.DashDischarging:
                    if (DashDischarging())
                    {
                        ResetFields();
                        ChangeState();
                    }
                    break;
                case AIStates.NormalRoarCombo1:
                    switch (Combo)
                    {
                        default:
                        case 0:
                            if (Roar())
                            {
                                ResetFields();
                                ElectricBallSetStartValue();
                                Combo = 1;
                            }
                            break;
                        case 1:
                            if (ElectricBall())
                            {
                                ResetFields();
                                Combo = 2;
                            }
                            break;
                        case 2:
                            if (GravitationThunder())
                            {
                                ResetFields();
                                Combo = 3;
                            }
                            break;
                        case 3:
                            if (ElectromagneticCannon())
                            {
                                ResetFields();
                                Combo = 4;
                            }
                            break;
                        case 4:
                            if (GatherCurrent())
                            {
                                ResetFields();
                                ChangeState();
                            }
                            break;
                    }
                    break;
                case AIStates.NormalRoarCombo2:
                    switch (Combo)
                    {
                        default:
                        case 0:
                            if (Roar())
                            {
                                ResetFields();
                                Combo = 1;
                            }
                            break;
                        case 1:
                            if (ElectricChain(100))
                            {
                                ResetFields();
                                Combo = 2;
                                LightningRaidSetStartValue();
                                Recorder2 = 3;//必定进行3次长冲
                            }
                            break;
                        case 2:
                            if (LightningRaidNoraml())
                            {
                                ResetFields();
                                Combo = 3;
                            }
                            break;
                        case 3:
                            if (GatherCurrent())
                            {
                                ResetFields();
                                ChangeState();
                            }
                            break;
                    }
                    break;
                case AIStates.NormalChainCombo:
                    switch (Combo)
                    {
                        default:
                        case 0:
                            if (ElectricChain(60))
                            {
                                ResetFields();
                                Combo = 1;
                            }
                            break;
                        case 1:
                            if (FallingThunder())
                            {
                                ResetFields();
                                Combo = 2;
                            }
                            break;
                        case 2:
                            if (DashDischarging())
                            {
                                ResetFields();
                                Combo = 3;
                            }
                            break;
                        case 3:
                            if (GatherCurrent())
                            {
                                ResetFields();
                                ChangeState();
                            }
                            break;
                    }
                    break;
                case AIStates.NormalPointerCombo:
                    switch (Combo)
                    {
                        default:
                        case 0:
                            if (AimThunderBall(90))
                            {
                                ResetFields();
                                Combo = 1;
                            }
                            break;
                        case 1:
                            if (ElectricChain(300))
                            {
                                ResetFields();
                                Combo = 2;
                            }
                            break;
                        case 2:
                            if (ElectricBreathMiddle())
                            {
                                ResetFields();
                                Combo = 3;
                            }
                            break;
                        case 3:
                            if (FallingThunder())
                            {
                                ResetFields();
                                Combo = 4;
                            }
                            break;
                        case 4:
                            if (GatherCurrent())
                            {
                                ResetFields();
                                ChangeState();
                            }
                            break;
                    }
                    break;
            }
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
                    State = AIStates.LightningRaid;
                    NPC.velocity.X *= 0.98f;
                    NPC.velocity.Y = -60;
                    NPC.rotation = NPC.velocity.ToRotation();
                    //FlyingUp(0.3f, 20, 0.9f);
                    NPC.EncourageDespawn(30);

                    State = AIStates.SmallDash;
                    Timer = 0;
                    SonState = 0;
                    return true;
                }
            }

            return false;
        }

        public void Initialize()
        {
            ResetAllOldCaches();
            State = AIStates.onSpawnAnmi;
            NPC.netUpdate = true;

            if (!VaultUtils.isServer && !SkyManager.Instance["ZacurrentSky"].IsActive())//如果这个天空没激活
            {
                SkyManager.Instance.Activate("ZacurrentSky");
            }
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
        public void ResetFields()
        {
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
        /// 切换状态
        /// </summary>
        public void ChangeState()
        {
            //记录旧状态，不包括短冲
            if (State is not AIStates.SmallDash)
                StateRecorder = State;

            //进入紫伏状态
            if (PurpleVoltCount == GetPurpleVoltMax())
            {
                State = AIStates.PurpleVoltExchange;
                return;
            }

            WeightedRandom<AIStates> rand = new WeightedRandom<AIStates>();

            if (PurpleVolt)//紫电状态的切换阶段
            {
            }
            else
                NormalMoveExchange(rand);//正常状态的阶段切换

            State = AIStates.PurpleVoltExchange;
            SetStateStartValues();
        }

        private void NormalMoveExchange(WeightedRandom<AIStates> rand)
        {
            rand.Add(AIStates.ElectricBreathSmall);

            //在玩家上下区域的时候减少概率
            bool upOrDown
                = MathF.Abs(Target.Center.X - NPC.Center.X) < 16 * 8
                && MathF.Abs(Target.Center.Y - NPC.Center.Y) > 16 * 6;
            rand.Add(AIStates.ElectricBreathMiddle, upOrDown ? 0.4f : 1);
            rand.Add(AIStates.ElectricBall);

            //距离远的时候提升使用概率
            bool farAway = NPC.Distance(Target.Center) > 650;
            float farawayPercent = farAway
               ? (1.5f + (NPC.Distance(Target.Center) - 650) / 400)
               : 1;
            rand.Add(AIStates.DashDischarging, farawayPercent);
            rand.Add(AIStates.LightningRaid, farawayPercent);
            //防止复读短冲
            if (State != AIStates.SmallDash)
                rand.Add(AIStates.SmallDash, farawayPercent + 1.5f);

            UseMoveCount++;
            if (UseMoveCount > Helper.ScaleValueForDiffMode(5, 4, 3, 2))
            {
                AddCombo(rand, AIStates.NormalRoarCombo1);
                AddCombo(rand, AIStates.NormalRoarCombo2);
                AddCombo(rand, AIStates.NormalChainCombo);
                AddCombo(rand, AIStates.NormalPointerCombo);
            }

            rand.elements.RemoveAll(p => p.Item1 == StateRecorder);

            State = rand.Get();
            RecordCombo();
        }

        private void RecordCombo()
        {
            if (State is AIStates.NormalChainCombo or AIStates.NormalRoarCombo1 or AIStates.NormalRoarCombo2 or AIStates.NormalPointerCombo)
            {
                comboRecords.Add(State);
                UseMoveCount = 0;
            }
        }

        /// <summary>
        /// 添加一个连招
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="combo"></param>
        private void AddCombo(WeightedRandom<AIStates> rand, AIStates combo)
        {
            comboRecords ??= new HashSet<AIStates>();
            if (comboRecords.Count > 2)
                comboRecords.Clear();
            if (!comboRecords.Contains(combo))//没用过的连招才行
                rand.Add(combo, UseMoveCount / Helper.ScaleValueForDiffMode(6, 5, 4, 3));
        }

        private void SetStateStartValues()
        {
            switch (State)
            {
                case AIStates.onSpawnAnmi:
                    break;
                case AIStates.onKillAnim:
                    break;
                case AIStates.PurpleVoltExchange:
                    break;
                case AIStates.LightningRaid:
                    LightningRaidSetStartValue();
                    break;
                case AIStates.ThunderChain:
                    break;
                case AIStates.SmallDash:
                    SmallDashSetStartValue();
                    break;
                case AIStates.ElectricBreathSmall:
                    ElectricBreathSmallSetStartValue();
                    break;
                case AIStates.ElectricBreathMiddle:
                    ElectricBreathMiddleSetStartValue();
                    break;
                case AIStates.ElectricBall:
                    ElectricBallSetStartValue();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 获得紫电
        /// </summary>
        /// <param name="red"></param>
        public void GetPurpleVolt(bool red)
        {
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
