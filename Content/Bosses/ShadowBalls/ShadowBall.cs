using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core;
using Coralite.Core.SmoothFunctions;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    ///                                               马赛克
    ///           ○○○○○○○○○○ ○                        l   l  
    ///       ○○○○○○○○○○○○○○○○○ ○                     l   l
    ///     ○○○○○○○○○○○○○○○○○○○○○ ○              _ _  l   l_  
    ///    ○○○○○{影}○○○○○○○○○○○{球}○ ○          !  !  l   l l ˉl
    ///   ○○○○○{影影影}○○子○○○{球球球}○ ○        l               l
    /// ○○○○○○○○{影}○○○○○○○○○○○{球}○○○ ○        l               l
    /// ○○○○○○○○○○○○○○○○○○○○○○○○○○○○○ ○          l             l
    ///  ○○○○○○==○○○○○○○○○○○○○○○○○○○ ○           l            l
    ///    ○○○○○○==○○○○○○○○○○○○○○○○ ○            l           l
    ///     ○○○○○○○=========○○○○○ ○              l           l
    ///       ○○○○○○○○○○○○○○○○○ ○
    ///           ○○○○○○○○○○ ○
    /// 
    ///             就贼搁赤玉灵嗷，别让我在影之城看见你嗷，
    ///                 抓到你，指定没你好果汁吃
    ///                     你记住我说的话嗷！
    /// 
    /// </summary>
    [VaultLoaden(AssetDirectory.ShadowBalls)]
    public partial class ShadowBall : ModNPC, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        /// <summary> 锁环的旋转状态 </summary>
        public LockStates LockState = LockStates.AngledRotate;
        /// <summary> 锁环的半径倍率，越大半径越高 </summary>
        public float LockDistancePercent = 1;

        [VaultLoaden("{@classPath}" + "ShadowLock")]
        public static ATex ShadowLockTex { get; private set; }

        internal ShadowBallContext AiContext;
        internal CoraliteBossStateMachine<ShadowBallContext> StateMachine;

        public ShadowLock[] shadowLocks;
        public List<ShadowLock> DrawShadowLocks;

        public const int MaxFrameX = 7;
        public const int MaxFrameY = 45;

        /// <summary>
        /// 一阶段的球壳的帧
        /// </summary>
        public int ShellFrame = MaxFrameY - 1;

        internal AIPhases Phase
        {
            get
            {
                ShadowBallStateId stateId = (ShadowBallStateId)(StateMachine?.CurrentState?.StateId ?? (int)ShadowBallStateId.OnSpawnAnim);
                return stateId switch
                {
                    ShadowBallStateId.OnSpawnAnim or
                    ShadowBallStateId.OnKillAnmi or
                    ShadowBallStateId.EscapeAnmi or
                    ShadowBallStateId.P1ToP2Exchange => AIPhases.Others,

                    ShadowBallStateId.SummonSmallShdowBall or
                    ShadowBallStateId.Revolution or
                    ShadowBallStateId.Starline or
                    ShadowBallStateId.LunarEclipse or
                    ShadowBallStateId.ShadowSpike or
                    ShadowBallStateId.RollingLaser or
                    ShadowBallStateId.RedShift or
                    ShadowBallStateId.BlueShift or
                    ShadowBallStateId.DarkSeek => AIPhases.P1_WithSmallBalls,
                    ShadowBallStateId.SmashDown => AIPhases.P2_ShadowPlayer,
                    _ => AIPhases.Others,
                };
            }
        }

        internal int CurrentStateId => StateMachine?.CurrentState?.StateId ?? (int)ShadowBallStateId.OnSpawnAnim;

        public Player Target => Main.player[NPC.target];

        public List<SmallShadowBall> drawSmallBalls = new();
        public List<NPC> smallBalls = new();
        /// <summary> 生成了夺少的小球 </summary>
        public int SpawnSmallBallCount { get; set; }
        //public int smallBallCount;

        /// <summary> 核心发光强度，0~1 </summary>
        public float LightStrength;

        /// <summary> 黑色遮罩的透明度，0~1 </summary>
        public float MaskAlpha;
        /// <summary> 其他球层的透明度 </summary>
        public float LayerAlpha;

        /// <summary> 核心的绘制偏移 </summary>
        public Vector2 CoreOffset;
        /// <summary> 锁环的渐进插值，用于锁的切换状态 </summary>
        public float LockLerpPercent;
        /// <summary> 因为timer会不断重置所以锁环单独用一个计时器 </summary>
        public float LockTimer;

        public bool CanDamage = false;


        internal static readonly RasterizerState OverflowHiddenRasterizerState = new()
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        //public const int ShadowCount = 16;

        /// <summary>
        /// NPC的透明度
        /// </summary>
        public float alpha;

        private bool spawn;
        private bool aiBootstrapped;

        #region tmlHooks

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.MustAlwaysDraw[Type] = true;
            Main.npcFrameCount[Type] = MaxFrameY;
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;
            NPC.damage = 50;
            NPC.defense = 6;
            NPC.lifeMax = 4500;
            NPC.knockBackResist = 0f;
            //NPC.scale = 1.2f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 10, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            //NPC.BossBar = GetInstance<BabyIceDragonBossBar>();

            //BGM：冰结寒流
            //if (!Main.dedServ)
            //    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/IcyColdStream");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            //if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            //{
            //    if (nPCStrengthHelper.IsExpertMode)
            //    {
            //        NPC.lifeMax = (int)((3820 + (numPlayers * 1750)) / journeyScale);
            //        NPC.damage = 35;
            //        NPC.defense = 12;
            //    }

            //    if (nPCStrengthHelper.IsMasterMode)
            //    {
            //        NPC.lifeMax = (int)((4720 + (numPlayers * 2100)) / journeyScale);
            //        NPC.damage = 60;
            //        NPC.defense = 15;
            //    }

            //    if (Main.getGoodWorld)
            //    {
            //        NPC.damage = 80;
            //        NPC.defense = 15;
            //    }

            //    if (Main.zenithWorld)
            //    {
            //        NPC.scale = 0.6f;
            //    }

            //    return;
            //}

            NPC.lifeMax = 3820 + (numPlayers * 1750);
            NPC.damage = 35;
            NPC.defense = 12;

            if (Main.masterMode)
            {
                NPC.lifeMax = 4720 + (numPlayers * 2100);
                NPC.damage = 60;
                NPC.defense = 15;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 5320 + (numPlayers * 2200);
                NPC.damage = 80;
                NPC.defense = 15;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 0.6f;
            }
        }

        public override bool CheckDead()
        {
            //if ((int)State != (int)AIStates.onKillAnim)
            //{
            //    State = (int)AIStates.onKillAnim;
            //    Timer = 0;
            //    NPC.dontTakeDamage = true;
            //    NPC.life = 1;
            //    return false;
            //}

            return true;
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<BabyIceDragonRelic>()));
            //npcLoot.Add(ItemDropRule.BossBag(ItemType<BabyIceDragonBossBag>()));
            //npcLoot.Add(ItemDropRule.Common(ItemType<BabyIceDragonTrophy>(), 10));

            //LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            //notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleCrystal>(), 1, 3, 5));
            //npcLoot.Add(notExpertRule);
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (CanDamage)
                return base.CanHitPlayer(target, ref cooldownSlot);

            return false;
        }

        #endregion

        #region AI

        public enum AIPhases
        {
            /// <summary> 一阶段 </summary>
            P1_WithSmallBalls,
            /// <summary> 二阶段 </summary>
            P2_ShadowPlayer,
            /// <summary> 三阶段 </summary>
            P3_BigBallSmash,
            Others
        }

        public void Initialize()
        {
            NPC.dontTakeDamage = true;
            alpha = 1;

            InitLocks();
        }

        /// <summary>
        /// 固定顺序（Phase 1 逐字镜像 Rediancie）：懒构造 → 客户端纠偏帧首 → 目标与脱战 → 只读事实 → 声明回默认 →
        /// 状态机 → 热字段兜底收养 → 落地运动 → 共享模拟（锁环）与表现 → 客户端记预测。
        /// </summary>
        public override void AI()
        {
            EnsureAiMachine();

            if (!spawn)
            {
                Initialize();
                spawn = true;
            }

            if (VaultUtils.isClient)
                AiContext.Net.BeginClientFrame(NPC);

            if (!FindTarget())
            {
                // 脱战：白天且没有活着的目标时缓缓下沉离场（两端同算的运动数学，不经状态机）。
                NPC.EncourageDespawn(ShadowBallDirector.DespawnEncourageFrames);
                NPC.dontTakeDamage = true;
                NPC.velocity.Y += ShadowBallDirector.DespawnGravity;

                if (VaultUtils.isClient)
                    AiContext.Net.EndClientFrame(NPC);
                return;
            }

            Lighting.AddLight(NPC.Center, ShadowBallDirector.SelfLight);

            // 小球名册每帧在两端重建：小球的待机环绕几何读的就是 selfIndex 与总数，
            // 名册只在少数招式里刷新的话，中途加入的客户端会拿 0 个小球去做除法（NaN），战斗中也会与服务端排布错位。
            GetSmallBalls();

            AiContext.UpdateFacts();
            AiContext.BeginFrameDefaults();

            // 状态只写声明；转移仅 ServerUpdate 返回值，客户端由 ai[0] 跟随
            StateMachine.Update();
            AiContext.ConsumePendingHotAdopt();

            ApplyDeclaredMovement();

            // 锁环不是纯表现：小球的待机位置读它，所以两端同跑。
            UpdateSharedVisuals();

            if (VaultUtils.isClient)
                AiContext.Net.EndClientFrame(NPC);
        }

        /// <summary>目标与脱战判定照旧；返回 false 表示该离场。旧 ShadowBall.cs:351-363。</summary>
        private bool FindTarget()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Main.dayTime)
            {
                // 丢失目标：重新索敌后继续当前招式（保留阶段不变，避免顶层状态被强行回退导致阶段判定错乱）。
                NPC.TargetClosest();

                if (Main.dayTime && (Target.dead || !Target.active))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 懒构造；初态从 ai[0] 重建——中途加入的客户端靠这个不重放入场动画，未注册 id 回退出生动画。
        /// 上下文构造里已置 <c>UseLegacySpeedValve = false</c>。
        /// </summary>
        private void EnsureAiMachine()
        {
            if (aiBootstrapped)
            {
                return;
            }

            AiContext = new ShadowBallContext(this);
            StateMachine = new CoraliteBossStateMachine<ShadowBallContext>(AiContext);

            IVaultState<ShadowBallContext> initial =
                VaultStateRegistry<ShadowBallContext>.Create((int)NPC.ai[CoraliteBossContext.StateAiSlot])
                ?? VaultStateRegistry<ShadowBallContext>.Create((int)ShadowBallStateId.OnSpawnAnim);

            StateMachine.SetInitialState(initial);
            aiBootstrapped = true;
        }

        /// <summary>
        /// 两端同跑：把本帧声明翻译成 velocity / rotation。状态里没有裸的运动法则，全局规则只在这里改一处。
        /// </summary>
        private void ApplyDeclaredMovement()
        {
            ShadowBallContext ctx = AiContext;

            switch (ctx.MoveMode)
            {
                case ShadowBallMoveMode.Damp:
                    NPC.velocity *= ctx.DampFactor;
                    break;
                case ShadowBallMoveMode.Approach:
                    {
                        Vector2 toPoint = ctx.ApproachPoint - NPC.Center;
                        if (toPoint.LengthSquared() > ctx.ApproachDeadZone * ctx.ApproachDeadZone)
                            NPC.velocity = Vector2.Lerp(NPC.velocity,
                                toPoint.SafeNormalize(Vector2.Zero) * ctx.ApproachSpeed, ctx.ApproachLerp);
                        else
                            NPC.velocity *= ctx.DampFactor;
                    }
                    break;
                default:
                    // Keep / Direct：本帧速度由状态自己负责，宿主不动。
                    break;
            }

            if (ctx.RotationMode == ShadowBallRotationMode.LerpTo)
                NPC.rotation = NPC.rotation.AngleLerp(ctx.RotationTarget, ctx.RotationLerp);
        }

        private void UpdateSharedVisuals()
        {
            switch (Phase)
            {
                case AIPhases.P1_WithSmallBalls:
                P1_WithSmallBalls:
                    {
                        foreach (var shadowLock in shadowLocks)
                            shadowLock.Update(this);

                        if (LockLerpPercent < 1)
                        {
                            LockLerpPercent = (LockLerpPercent + ShadowBallDirector.LockLerpStep) * ShadowBallDirector.LockLerpGain;
                            if (LockLerpPercent > 1)
                                LockLerpPercent = 1;
                        }

                        LockTimer++;
                        if (LockTimer > ShadowBallDirector.LockTimerWrap)
                            LockTimer = 0;
                    }
                    break;
                case AIPhases.P2_ShadowPlayer:
                    //if (ShadowPlayer != null && !Main.dedServ)
                    //{
                    //    ShadowPlayer.direction = NPC.spriteDirection;
                    //    ShadowPlayer.velocity = NPC.velocity;
                    //    ShadowPlayer.Center = NPC.Center;
                    //    ShadowPlayer.UpdateDyes();
                    //    ShadowPlayer.UpdateSocialShadow();
                    //    ShadowPlayer.PlayerFrame();
                    //}

                    break;
                case AIPhases.P3_BigBallSmash:
                    break;
                case AIPhases.Others:
                    switch ((ShadowBallStateId)CurrentStateId)
                    {
                        default:
                            break;
                        case ShadowBallStateId.OnSpawnAnim://略显弱智的写法
                            goto P1_WithSmallBalls;
                    }
                    break;
                default:
                    break;
            }

            //if (Phase == (int)AIPhases.P1_WithSmallBalls)
            //{
            //UpdateFrameNormally();

            //if (shadowCircle != null)
            //{
            //    shadowCircle[0].xRotation += 0.03f;
            //    shadowCircle[0].zRotation = NPC.rotation - 1.57f;
            //    shadowCircle[0].selfRotation += 0.002f;
            //    if (shadowCircle[0].selfRotation > 1)
            //        shadowCircle[0].selfRotation -= 1;
            //    shadowCircle[0].Update();
            //    shadowCircle[1].xRotation += 0.03f;
            //    shadowCircle[1].zRotation = NPC.rotation;
            //    shadowCircle[1].selfRotation += 0.002f;
            //    if (shadowCircle[1].selfRotation > 1)
            //        shadowCircle[1].selfRotation -= 1;
            //    shadowCircle[1].Update();
            //    shadowCircle[2].xRotation += 0.01f;
            //    shadowCircle[2].zRotation = 0f;
            //    shadowCircle[2].selfRotation += 0.005f;
            //    if (shadowCircle[2].selfRotation > 1)
            //        shadowCircle[2].selfRotation -= 1;
            //    shadowCircle[2].Update();
            //}
            //}
            //else if (Phase == AIPhases.P2_ShadowPlayer && ShadowPlayer != null && !Main.dedServ)
            //{
            //    ShadowPlayer.direction = NPC.spriteDirection;
            //    ShadowPlayer.velocity = NPC.velocity;
            //    ShadowPlayer.Center = NPC.Center;
            //    ShadowPlayer.UpdateDyes();
            //    ShadowPlayer.UpdateSocialShadow();
            //    ShadowPlayer.PlayerFrame();
            //}
        }

        #endregion

        #region NetWork

        /// <summary>
        /// 热字段（Timer / Counter / Beat / 引力锚点与各招自用槽）+ boss 事实（本次要召唤几个小球）随 SyncNPC 原子过线。
        /// </summary>
        public override void SendExtraAI(BinaryWriter writer)
        {
            EnsureAiMachine();
            AiContext.WriteNet(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            EnsureAiMachine();
            AiContext.ReadNet(reader);
        }

        #endregion

        #region States

        //public void ExchangeToPhase2()
        //{
        //    Timer = 0;
        //    SonState = 0;
        //    Recorder = 0;
        //    Recorder2 = 0;

        //    NPC.TargetClosest();
        //    ApplyPhase2Hitbox();
        //ExchangeToPhase2VisualOnly();
        //}

        //public void ApplyPhase2Hitbox()
        //{
        //    Vector2 center = NPC.Center;
        //    NPC.width = (int)(32 * NPC.scale);
        //    NPC.height = (int)(48 * NPC.scale);
        //    NPC.Center = center;
        //}

        //public void ExchangeToPhase2VisualOnly()
        //{
        //    if (Main.dedServ)
        //    {
        //        return;
        //    }

        //    ShadowPlayer = Target.clientClone();
        //    ShadowPlayer.armor[10] = new Item(ModContent.ItemType<ShadowHead>());
        //    ShadowPlayer.armor[11] = new Item(ModContent.ItemType<ShadowBreastplate>());
        //    ShadowPlayer.armor[12] = new Item(ModContent.ItemType<ShadowLegs>());
        //    ShadowPlayer.ResetVisibleAccessories();
        //}

        #endregion

        #region Locks

        public enum LockStates
        {
            /// <summary>
            /// 普通的行星环旋转
            /// </summary>
            Normal,
            /// <summary>
            /// 同心圆，3个环叠在一起
            /// </summary>
            ConcentricCircles,
            /// <summary>
            /// 带角度的同心圆
            /// </summary>
            ConcentricCirclesAngled,
            /// <summary>
            /// 带角度的旋转，3个环错开
            /// </summary>
            AngledRotate,
        }

        /// <summary>
        /// 环绕在身边的东西，仅在一阶段有
        /// </summary>
        public class ShadowLock
        {
            public Vector2 center;
            public Vector2 offset;
            public float zDepth;
            public float rotation;
            public float alpha;
            /// <summary>
            /// 自身在这一圈层的索引比例
            /// </summary>
            public float indexPercent;

            private SecondOrderDynamics_Vec2 smoother;

            public bool active = true;
            /// <summary>
            /// 小球的索引
            /// </summary>
            public int smallBallIndex;
            public byte LockCoreFrame;

            /// <summary>
            /// 在哪一层，一共3圈
            /// </summary>
            public byte layer;
            public float baseRot, zyRot, xyRot;

            public ShadowLock(ShadowBall owner, float indexPercent, int layer)
            {
                smoother = new SecondOrderDynamics_Vec2(5f - layer, 0.8f, 1, owner.NPC.Center);
                this.indexPercent = indexPercent;
                this.layer = (byte)layer;
            }

            public void Update(ShadowBall owner)
            {
                //if (!active)
                //    return;
                //active = Main.rand.NextBool( 3);
                //Dead();
                //if (active)
                //{
                //    LockCoreFrame = 0;
                //}
                float centerDistance = Vector2.DistanceSquared(center, owner.NPC.Center);

                if (centerDistance > 200 * 200)
                    smoother.Reset(owner.NPC.Center);

                center = smoother.Update(1 / 60f, owner.NPC.Center);
                baseRot = Helper.Lerp(baseRot, (layer == 1 ? -1 : 1) * owner.LockTimer * 0.01f * (layer + 1), owner.LockLerpPercent);

                switch (owner.LockState)
                {
                    case LockStates.Normal:
                        zyRot = zyRot.AngleLerp((1.57f + MathF.Sin(owner.LockTimer * (0.01f + layer * 0.005f)) * (0.4f + layer * 0.1f)) % MathHelper.TwoPi, owner.LockLerpPercent);
                        xyRot = xyRot.AngleLerp((MathHelper.TwoPi / 3 * layer + owner.LockTimer * 0.01f) % MathHelper.TwoPi, owner.LockLerpPercent);

                        rotation = Helper.Lerp(rotation, 0, owner.LockLerpPercent);

                        break;
                    case LockStates.ConcentricCircles:
                        zyRot = zyRot.AngleLerp(0, owner.LockLerpPercent);
                        xyRot = xyRot.AngleLerp(0, owner.LockLerpPercent);

                        rotation = rotation.AngleLerp(offset.ToRotation() + MathHelper.PiOver2, owner.LockLerpPercent);

                        break;
                    case LockStates.ConcentricCirclesAngled:
                        zyRot = zyRot.AngleLerp(1f, owner.LockLerpPercent);
                        xyRot = xyRot.AngleLerp(owner.NPC.rotation + MathHelper.PiOver2, owner.LockLerpPercent);

                        rotation = rotation.AngleLerp(offset.ToRotation() + MathHelper.PiOver2, owner.LockLerpPercent);

                        break;
                    case LockStates.AngledRotate:
                        zyRot = zyRot.AngleLerp((owner.LockTimer * (0.01f + layer * 0.005f)) * (layer == 1 ? -1 : 1) % MathHelper.TwoPi, owner.LockLerpPercent);
                        xyRot = xyRot.AngleLerp(owner.NPC.rotation + MathHelper.PiOver2, owner.LockLerpPercent);

                        rotation = rotation.AngleLerp(owner.NPC.rotation, owner.LockLerpPercent);
                        break;
                    default:
                        break;
                }

                _3DRotate((80 + 25 * layer) * owner.LockDistancePercent, baseRot, zyRot, xyRot);
            }

            /// <summary>
            /// 锁飞出去，之后不再绘制锁扣
            /// </summary>
            public void LockOut(NPC smallBall)
            {
                active = false;
                smallBallIndex = smallBall.whoAmI;
            }

            public void Dead()
            {
                active = false;
                LockCoreFrame = (byte)Main.rand.Next(1, 5);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="Radius"></param>
            /// <param name="baseRot">自身旋转</param>
            /// <param name="zyRot"></param>
            /// <param name="xyRot"></param>
            public void _3DRotate(float Radius, float baseRot, float zyRot, float xyRot)
            {
                float rot = baseRot + indexPercent * MathHelper.TwoPi;

                Vector2 vector2D = rot.ToRotationVector2();
                Vector3 vector3D = Vector3.Transform(vector2D.Vec3(), Matrix.CreateRotationX(zyRot));
                ///将二维的向量转为3维的并绕着X轴旋转一下
                vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationZ(xyRot));///以Z为轴旋转，用来配合影子球自身的旋转

                //将3维向量投影到二维
                float k1 = -1000 / (vector3D.Z - 1000);

                Vector2 targetDir = k1 * new Vector2(vector3D.X, vector3D.Y);
                Vector2 targetCenter = (targetDir * Radius);
                offset = targetCenter;// smoother.Update(1 / 60f, targetCenter);

                //vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationX(-MathHelper.PiOver2));///以Z为轴旋转，用来配合影子球自身的旋转

                zDepth = vector3D.Z * Radius;
            }

            public void Draw(Texture2D tex, SpriteBatch spriteBatch)
            {
                var frameBox = tex.Frame(5, 2, LockCoreFrame, 1);

                Vector2 pos = offset + center - Main.screenPosition;
                Color lightColor = Color.Lerp(Lighting.GetColor((center + offset).ToTileCoordinates()), Color.White, 0.4f);
                float scale = 1 + Utils.Remap(zDepth / 140, -1, 1, -0.25f, 0.5f);


                spriteBatch.Draw(tex, pos, frameBox, Color.White, offset.ToRotation() + MathHelper.PiOver2, frameBox.Size() / 2, scale, 0, 0);

                if (!active)//不活跃了就表示这个锁已经出去了，之绘制锁扣
                    return;

                frameBox = tex.Frame(5, 2, 0, 0);

                spriteBatch.Draw(tex, pos, frameBox, lightColor, rotation, frameBox.Size() / 2, scale, 0, 0);
            }
        }

        public void InitLocks()
        {
            int lockMax = GetMaxSmallBall();

            shadowLocks = new ShadowLock[lockMax];

            lockMax /= 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < lockMax; j++)
                {
                    int index = i * lockMax + j;
                    shadowLocks[index] = new ShadowLock(this, j / (float)lockMax, i);
                }
        }

        /// <summary>
        /// 切换锁环的环绕状态
        /// </summary>
        /// <param name="newState"></param>
        public void SwitchLockState(LockStates newState)
        {
            LockState = newState;
            LockLerpPercent = 0;
        }

        #endregion


        #region HelperMethods

        /// <summary>
        /// 小球总量上限
        /// </summary>
        /// <returns></returns>
        public static int GetMaxSmallBall()
        {
            int maxSmallBall = Helper.ScaleValueForDiffMode(10, 12, 16, 24) * 3;

            if (Main.getGoodWorld)//天顶超级加倍
                maxSmallBall = 30 * 3;

            return maxSmallBall;
        }

        /// <summary>
        /// 小球的同场上限是多少，根据不同难度改变
        /// </summary>
        /// <returns></returns>
        public static int GetSmallBallSameTimeLimit()
        {
            int maxSmallBall = Helper.ScaleValueForDiffMode(8, 10, 12, 16);

            if (Main.getGoodWorld)//天顶超级加倍
                maxSmallBall = 20;

            return maxSmallBall;
        }

        /// <summary>
        /// 获取所有小球，同时会设置小球索引
        /// </summary>
        /// <returns></returns>
        public int GetSmallBalls()
        {
            smallBalls.Clear();
            int index = 0;

            foreach (var npc in Main.ActiveNPCs)
                if (npc.type == ModContent.NPCType<SmallShadowBall>() &&
                    npc.ai[0] == NPC.whoAmI &&
                    npc.ai[1] != (int)SmallShadowBallStateId.OnKillAnmi)
                {
                    smallBalls.Add(npc);
                    (npc.ModNPC as SmallShadowBall).selfIndex = index;
                    index++;
                }

            return smallBalls.Count;
        }

        /// <summary>
        /// 建议在调用<see cref="GetSmallBalls"/>之后调用
        /// </summary>
        public void SmallBallStartAttack()
        {
            foreach (var ball in smallBalls)
            {
                (ball.ModNPC as SmallShadowBall).StartAttack();
            }
        }

        /// <summary>还有没有锁扣可以弹出去变成小球（= 设计文档里的"自身影子量"）。</summary>
        public bool HasActiveLock()
        {
            if (shadowLocks == null)
                return false;

            foreach (ShadowLock shadowLock in shadowLocks)
                if (shadowLock.active)
                    return true;

            return false;
        }

        #region 跨实体编排（只在权威端裁决，子球经自己的 ai[1] 同步给客户端）

        /// <summary>
        /// 把名册里前 <paramref name="howMany"/> 个小球切到指定招式。<br/>
        /// 这是本体对子球唯一的命令入口：调用点全在各状态的 <c>AuthorityUpdate</c> 里，
        /// 子球那边 <c>ServerChangeState</c> 也自带权威端守卫，客户端只从子球的 <c>ai[1]</c> 跟随（C1 / C2 / C10）。<br/>
        /// <paramref name="readyRest"/> 为真时把没被叫到的小球直接标就绪，免得本体死等（旧影刺的写法）。
        /// </summary>
        public void CommandSmallBalls(SmallShadowBallStateId state, int howMany, bool readyRest = false)
        {
            if (VaultUtils.isClient)
                return;

            for (int i = 0; i < smallBalls.Count; i++)
            {
                if (smallBalls[i].ModNPC is not SmallShadowBall smallBall)
                    continue;

                if (i < howMany)
                    smallBall.SwitchState(state);
                else if (readyRest)
                    smallBall.SetReady();
            }
        }

        /// <summary>从当前待机的小球里随机挑一个切到指定招式；没有待机的就什么也不做。旧 P1.LunarEclipse.cs:45-68。</summary>
        public void CommandOneIdleSmallBall(SmallShadowBallStateId state)
        {
            if (VaultUtils.isClient)
                return;

            List<NPC> idleSmallBalls = [];
            foreach (NPC ball in smallBalls)
                if (ball.active && ball.ModNPC is SmallShadowBall smallBall
                    && smallBall.CurrentStateId == (int)SmallShadowBallStateId.Idle)
                    idleSmallBalls.Add(ball);

            if (idleSmallBalls.Count < 1)
                return;

            (Main.rand.Next(idleSmallBalls).ModNPC as SmallShadowBall).SwitchState(state);
        }

        /// <summary>
        /// 旋转激光的分层派活：小球按索引均分成 <paramref name="layerCount"/> 层，每个小球记下自己在本层的序号、层号与本层总数；
        /// 除不尽剩下的直接标就绪。小球不足层数就返回 false（本体据此收招）。旧 P1.RollingLaser.cs:17-58。
        /// </summary>
        public bool CommandRollingLaserLayers(int layerCount)
        {
            if (VaultUtils.isClient)
                return true;

            int smallBallCount = GetSmallBalls();
            if (smallBallCount < layerCount)
                return false;

            SmallBallStartAttack();

            int perLayer = smallBallCount / layerCount;
            for (int layer = 0; layer < layerCount; layer++)
                for (int i = 0; i < perLayer; i++)
                {
                    if (smallBalls[i + (layer * perLayer)].ModNPC is SmallShadowBall smallBall)
                    {
                        smallBall.SwitchState(SmallShadowBallStateId.RollingLaser);
                        smallBall.ServerSetRollingLayer(i, layer, perLayer);
                    }
                }

            for (int i = perLayer * layerCount; i < smallBalls.Count; i++)
                if (smallBalls[i].ModNPC is SmallShadowBall extra)
                    extra.SetReady();

            return true;
        }

        #endregion

        public bool CheckSmallBallReady()
        {
            foreach (var ball in smallBalls)
            {
                if (!(ball.ModNPC as SmallShadowBall).Ready)
                    return false;
            }

            return true;
        }

        //public bool CheckSmallBallsReady()
        //{
        //    if (smallBallCount == 0)
        //    {
        //        return false;
        //    }

        //    foreach (var ball in smallBalls)
        //    {
        //        if (ball.ModNPC is not SmallShadowBall sb || !sb.IsOrchestrationReady(NPC))
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        /// <summary>服务端：所有子球当前顶层 FSM 均已 MarkTerminated（招结束）。</summary>
        //public bool CheckAllSmallBallsTerminated()
        //{
        //    //if (smallBallCount == 0)
        //    //{
        //    //    return false;
        //    //}

        //    foreach (var ball in smallBalls)
        //    {
        //        if (ball.ModNPC is not SmallShadowBall sb)
        //        {
        //            return false;
        //        }

        //        sb.EnsureStateMachinePublic();
        //        if (sb.StateMachine == null || !sb.StateMachine.IsTerminated)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //private static int NextSmallBallSeed(Random attackRandom)
        //{
        //    return attackRandom.Next();
        //}

        //public void SetDirection(Vector2 targetPos, out float xLength, out float yLength)
        //{
        //    xLength = NPC.Center.X - targetPos.X;
        //    yLength = NPC.Center.Y - targetPos.Y;

        //    NPC.direction = NPC.spriteDirection = xLength > 0 ? -1 : 1;
        //    NPC.directionY = yLength > 0 ? -1 : 1;

        //    xLength = Math.Abs(xLength);
        //    yLength = Math.Abs(yLength);
        //}

        //public void SpawnSmallBalls()
        //{
        //    if (VaultUtils.isClient)
        //    {
        //        return;
        //    }

        //for (int i = 0; i < 5; i++)
        //{
        //    int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y,
        //        ModContent.NPCType<SmallShadowBall>(), NPC.whoAmI, NPC.whoAmI);
        //    (Main.npc[index].ModNPC as SmallShadowBall).smallBallType = i;
        //    (Main.npc[index].ModNPC as SmallShadowBall).shadowCircle =
        //        new ShadowCircleController
        //        (ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "SmallCircle" + i, ReLogic.Content.AssetRequestMode.ImmediateLoad));
        //}
        //}

        //public void MovementLimit()
        //{
        //Vector2 center = NPC.Center;
        //center.X = Math.Clamp(center.X, MovementLimitRect.X, MovementLimitRect.X + MovementLimitRect.Width);
        //center.Y = Math.Clamp(center.Y, MovementLimitRect.Y, MovementLimitRect.Y + MovementLimitRect.Height);
        //NPC.Center = center;
        //}

        //public void InitCaches()
        //{
        //    for (int i = 0; i < ShadowCount; i++)
        //        NPC.oldPos[i] = NPC.Center;
        //}

        //public void UpdateCachesNormally()
        //{
        //    for (int i = ShadowCount - 1; i > 0; i--)
        //        NPC.oldPos[i] = NPC.oldPos[i - 1];
        //    NPC.oldPos[0] = NPC.Center;
        //}

        //public void UpdateFrameNormally()
        //{
        //    if (++NPC.frameCounter > 4)
        //    {
        //        NPC.frameCounter = 0;
        //        if (++NPC.frame.Y > 8)
        //            NPC.frame.Y = 0;
        //    }
        //}

        /// <summary>
        /// 让拖尾数组随机出现在NPC周围的一个圆圈范围
        /// </summary>
        /// <param name="width"></param>
        //public void UpdateCacheRandom(float width, int percent)
        //{
        //    for (int i = 0; i < ShadowCount; i++)
        //    {
        //        if (Main.rand.NextBool(percent, 100))
        //            NPC.oldPos[i] = NPC.Center + Main.rand.NextVector2Circular(width, width);
        //    }
        //}

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D tex = NPC.GetTexture();
            Vector2 pos = NPC.Center - Main.screenPosition;
            Color lightColor = Lighting.GetColor(NPC.Center.ToTileCoordinates(), Color.White);

            switch (Phase)
            {
                default:
                case AIPhases.P1_WithSmallBalls:
                P1_WithSmallBalls:
                    {
                        PrepareShadowLockLists();
                        PrepareSmallBallLists();

                        DrawSmallBalls(true, spriteBatch);
                        DrawLocks(true, spriteBatch);

                        DrawShadowShellLayerBack(spriteBatch, tex, pos, lightColor);
                        DrawCore(spriteBatch, tex, pos);
                        DrawShadowShellLayerFront(spriteBatch, tex, pos, lightColor);

                        DrawLocks(false, spriteBatch);
                        DrawSmallBalls(false, spriteBatch);
                    }
                    break;
                case AIPhases.P2_ShadowPlayer:
                    break;
                case AIPhases.P3_BigBallSmash:
                    break;
                case AIPhases.Others:
                    switch ((ShadowBallStateId)CurrentStateId)
                    {
                        default:
                            break;
                        case ShadowBallStateId.OnSpawnAnim://略显弱智的写法
                            goto P1_WithSmallBalls;
                    }
                    break;
            }

            void PrepareShadowLockLists()
            {
                DrawShadowLocks ??= new List<ShadowLock>(36);

                DrawShadowLocks.Clear();
                if (shadowLocks != null)
                    foreach (var shadowLock in shadowLocks)
                        DrawShadowLocks.Add(shadowLock);

                DrawShadowLocks.Sort((a, b) => a.zDepth.CompareTo(b.zDepth));
            }

            void PrepareSmallBallLists()
            {
                drawSmallBalls ??= new List<SmallShadowBall>();

                drawSmallBalls.Clear();
                foreach (var npc in Main.ActiveNPCs)
                    if (npc.type == ModContent.NPCType<SmallShadowBall>() && npc.ai[0] == NPC.whoAmI)
                        drawSmallBalls.Add((npc.ModNPC as SmallShadowBall));

                drawSmallBalls.Sort((a, b) => a.zDepth.CompareTo(b.zDepth));
            }
        }

        #region 绘制球体
        /// <summary>
        /// 绘制核心
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tex"></param>
        /// <param name="center"></param>
        /// <param name="drawColor"></param>
        public void DrawCore(SpriteBatch spriteBatch, Texture2D tex, Vector2 center)
        {
            Color lightColor = Color.White;
            //NON所以这样控制透明度
            lightColor.A = (byte)(lightColor.A * LightStrength);
            center += CoreOffset;

            //绘制核心发光层
            var frameBox = tex.Frame(MaxFrameX, MaxFrameY, 3, 0);

            spriteBatch.Draw(tex, center, frameBox, lightColor, Main.GlobalTimeWrappedHourly * 1.5f, frameBox.Size() / 2, NPC.scale, 0, 0);

            //绘制核心
            frameBox = tex.Frame(MaxFrameX, MaxFrameY, 2, 0);

            spriteBatch.Draw(tex, center, frameBox, Color.White, NPC.rotation, frameBox.Size() / 2, NPC.scale, 0, 0);
        }

        /// <summary>
        /// 绘制影子球层背后的部分
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="center"></param>
        /// <param name="drawColor"></param>
        public void DrawShadowShellLayerBack(SpriteBatch spriteBatch, Texture2D tex, Vector2 center, Color drawColor)
        {
            drawColor.A = (byte)(255 * LayerAlpha);
            //绘制最底部花纹
            var frameBox = tex.Frame(MaxFrameX, MaxFrameY, 6, 0);

            spriteBatch.Draw(tex, center, frameBox, drawColor, 0, frameBox.Size() / 2, NPC.scale, 0, 0);

            //绘制遮罩
            frameBox = tex.Frame(MaxFrameX, MaxFrameY, 5, 0);

            spriteBatch.Draw(tex, center, frameBox, new Color(255, 255, 255, (byte)(255 * MaskAlpha)), 0, frameBox.Size() / 2, NPC.scale, 0, 0);

            //绘制旋转能量层
            frameBox = tex.Frame(MaxFrameX, MaxFrameY, 4, 0);

            spriteBatch.Draw(tex, center, frameBox, drawColor, -Main.GlobalTimeWrappedHourly * 2f, frameBox.Size() / 2, NPC.scale, 0, 0);
        }

        /// <summary>
        /// 绘制影子球层前面的部分
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="tex"></param>
        /// <param name="center"></param>
        /// <param name="drawColor"></param>
        public void DrawShadowShellLayerFront(SpriteBatch spriteBatch, Texture2D tex, Vector2 center, Color drawColor)
        {
            //绘制遮罩
            var frameBox = tex.Frame(MaxFrameX, MaxFrameY, 1, 0);

            spriteBatch.Draw(tex, center, frameBox, new Color(255, 255, 255, (byte)(255 * MaskAlpha)), 0, frameBox.Size() / 2, NPC.scale, 0, 0);

            //绘制最顶部球层
            frameBox = tex.Frame(MaxFrameX, MaxFrameY, 0, ShellFrame);

            spriteBatch.Draw(tex, center, frameBox, drawColor, MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.3f, frameBox.Size() / 2, NPC.scale, 0, 0);
        }
        #endregion

        public void DrawSmallBalls(bool back, SpriteBatch spriteBatch)
        {
            if (back)
                for (int i = 0; i < drawSmallBalls.Count; i++)
                {
                    if (drawSmallBalls[i].zDepth >= 0)
                        return;
                    drawSmallBalls[i].DrawSelf(spriteBatch);
                }
            else
                for (int i = 0; i < drawSmallBalls.Count; i++)
                {
                    if (drawSmallBalls[i].zDepth >= 0)
                        drawSmallBalls[i].DrawSelf(spriteBatch);
                }
        }

        public void DrawLocks(bool back, SpriteBatch spriteBatch)
        {
            Texture2D tex = ShadowLockTex.Value;

            if (back)
                for (int i = 0; i < DrawShadowLocks.Count; i++)
                {
                    if (DrawShadowLocks[i].zDepth >= 0)
                        return;
                    DrawShadowLocks[i].Draw(tex, spriteBatch);
                }
            else
                for (int i = 0; i < DrawShadowLocks.Count; i++)
                {
                    if (DrawShadowLocks[i].zDepth >= 0)
                        DrawShadowLocks[i].Draw(tex, spriteBatch);
                }
        }

        #endregion
    }
}
