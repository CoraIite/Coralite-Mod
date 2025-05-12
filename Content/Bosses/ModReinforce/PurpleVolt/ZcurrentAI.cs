using Coralite.Helpers;
using System;
using Terraria;
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
        internal AIStates StateRecorder
        {
            get => (AIStates)NPC.localAI[2];
            set => NPC.localAI[2] = (int)value;
        }

        internal ref float UseMoveCount => ref NPC.localAI[3];

        public Point[] oldFrame;
        public int[] oldDirection;

        public int oldSpriteDirection;

        private bool init = true;

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

            //调整身位用招式
            SmallDash,

            //其他
            Roar
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
                case AIStates.Roar:
                    if (Roar())
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
            }
        }

        public bool CheckTarget()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 4500)//没有玩家存活时离开
                {
                    NPC.dontTakeDamage = false;
                    canDrawShadows = false;
                    IsDashing = false;
                    State = AIStates.LightningRaid;
                    NPC.spriteDirection = 1;
                    NPC.rotation = NPC.rotation.AngleTowards(0f, 0.14f);
                    NPC.velocity.X *= 0.98f;
                    //FlyingUp(0.3f, 20, 0.9f);
                    NPC.EncourageDespawn(30);
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

            //if (!VaultUtils.isServer && !SkyManager.Instance["ThunderveinSky"].IsActive())//如果这个天空没激活
            //{
            //    SkyManager.Instance.Activate("ThunderveinSky");
            //}
        }


        public override void PostAI()
        {
            oldSpriteDirection = NPC.spriteDirection;

            if (!VaultUtils.isServer && currentSurrounding )
            {
                Lighting.AddLight(NPC.Center, ZacurrentPink.ToVector3());
                if (Main.rand.NextBool(3))
                {
                    Vector2 offset = Main.rand.NextVector2Circular(100 * NPC.scale, 70 * NPC.scale);
                    ElectricParticle_PurpleFollow.Spawn(NPC.Center, offset, () => NPC.Center, Main.rand.NextFloat(0.75f, 1f));
                }
            }
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
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState()
        {
            //记录旧状态，不包括短冲
            if (State is not AIStates.SmallDash)
            {
                StateRecorder = State;
            }

            WeightedRandom<AIStates> rand = new WeightedRandom<AIStates>();

            if (PurpleVolt)//紫电状态的切换阶段
            {
                SetStateStartValues();
                return;
            }

            //正常状态的阶段切换
            rand.Add(AIStates.LightningRaid);
            rand.Add(AIStates.SmallDash);
            rand.Add(AIStates.Roar);
            rand.Add(AIStates.ElectricBreathSmall);
            rand.Add(AIStates.ElectricBreathMiddle);
            rand.Add(AIStates.ElectricBall);
            rand.Add(AIStates.DashDischarging);

            rand.elements.RemoveAll(p => p.Item1 == StateRecorder);

            //防止复读短冲
            if (State == AIStates.SmallDash)
            {
                rand.elements.RemoveAll(p => p.Item1 == AIStates.SmallDash);
            }

            State = rand.Get();
            State = AIStates.ElectricBreathMiddle;
            SetStateStartValues();
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
                case AIStates.ElectricBreathMiddle:
                    ElectricBreathMiddleSetStartValue();
                    break;
                case AIStates.ElectricBall:
                    LightingBallSetStartValue();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 各种helper

        public void ElectricSound()
        {
            Helper.PlayPitched("Electric/ElectricStrike" + Main.rand.NextFromList(0,2).ToString(), 0.4f, -0.2f, NPC.Center);
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

        private void SetSpriteDirectionFoTarget()
        {
            if (MathF.Abs(Target.Center.X - NPC.Center.X) > 48)
                NPC.spriteDirection = Target.Center.X > NPC.Center.X ? 1 : -1;
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
