using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Helpers;
using System;
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
        internal ref float StateRecorder => ref NPC.localAI[2];
        internal ref float UseMoveCount => ref NPC.localAI[3];

        public Point[] oldFrame;
        public int[] oldDirection;

        public int oldSpriteDirection;

        private bool init=true;

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
            /// <summary> 闪电链， </summary>
            ThunderChain,

            //连段

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

            if (!VaultUtils.isServer && currentSurrounding && Main.rand.NextBool(3))
            {
                Vector2 offset = Main.rand.NextVector2Circular(100 * NPC.scale, 70 * NPC.scale);
                ElectricParticle_Follow.Spawn(NPC.Center, offset, () => NPC.Center, Main.rand.NextFloat(0.75f, 1f));
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

            IsDashing = false;
            canDrawShadows = false;
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState()
        {
            WeightedRandom<AIStates> rand = new WeightedRandom<AIStates>();

            if (PurpleVolt)//紫电状态的切换阶段
            {

                return;
            }

            //正常状态的阶段切换

            rand.Add(AIStates.LightningRaid);

            State = rand.Get();

            switch (State)
            {
                case AIStates.Waiting:
                    break;
                case AIStates.onSpawnAnmi:
                    break;
                case AIStates.onKillAnim:
                    break;
                case AIStates.PurpleVoltExchange:
                    break;
                case AIStates.LightningRaid:
                    LightningRaidSetStartValue();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 各种helper


        public void FlyingFrame()
        {
            if (++NPC.frameCounter > 4)
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
                NPC.rotation += 3.141f;
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
