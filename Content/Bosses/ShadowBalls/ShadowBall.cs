using Coralite.Core;
using Coralite.Core.SmoothFunctions;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
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

        internal AIStates State => (AIStates)NPC.ai[0];
        internal ref float SonState => ref NPC.ai[1];
        internal ref float Recorder => ref NPC.ai[2];
        internal ref float Timer => ref NPC.ai[3];

        /// <summary>
        /// 锁环的旋转状态
        /// </summary>
        public LockStates LockState = LockStates.Normal;
        /// <summary>
        /// 锁环的半径倍率，越大半径越高
        /// </summary>
        public float LockDistancePercent = 1;

        [VaultLoaden("{@classPath}" + "ShadowLock")]
        public static ATex ShadowLockTex { get; private set; }

        internal ShadowBallContext AiContext;
        internal CoraliteBossStateMachine<ShadowBallContext> StateMachine;
        internal Random AttackRandom;

        public ShadowLock[] shadowLocks;
        public List<ShadowLock> DrawShadowLocks ;

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
                    ShadowBallStateId.ShadowShoot or
                    ShadowBallStateId.ShadowSpike or
                    ShadowBallStateId.DarkSeek => AIPhases.P1_WithSmallBalls,
                    ShadowBallStateId.SmashDown => AIPhases.P2_ShadowPlayer,
                    _ => AIPhases.Others,
                };
            }
        }

        internal int CurrentStateId => StateMachine?.CurrentState?.StateId ?? (int)ShadowBallStateId.OnSpawnAnim;

        internal ref float Recorder2 => ref NPC.localAI[1];
        internal ref float Recorder3 => ref NPC.localAI[0];

        public Player Target => Main.player[NPC.target];

        public List<NPC> smallBalls = new();
        /// <summary>
        /// 生成了夺少的小球
        /// </summary>
        public int SpawnSmallBallCount { get; set; }
        //public int smallBallCount;

        /// <summary>
        /// 核心发光强度，0~1
        /// </summary>
        public float LightStrength;

        /// <summary>
        /// 黑色遮罩的透明度，0~1
        /// </summary>
        public float MaskAlpha;

        /// <summary>
        /// 核心的绘制偏移
        /// </summary>
        public Vector2 CoreOffset;
        /// <summary>
        /// 锁环的渐进插值，用于锁的切换状态
        /// </summary>
        public float LockLerpPercent;

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
            Main.npcFrameCount[Type] = 9;
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
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((3820 + (numPlayers * 1750)) / journeyScale);
                    NPC.damage = 35;
                    NPC.defense = 12;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((4720 + (numPlayers * 2100)) / journeyScale);
                    NPC.damage = 60;
                    NPC.defense = 15;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 80;
                    NPC.defense = 15;
                }

                if (Main.zenithWorld)
                {
                    NPC.scale = 0.6f;
                }

                return;
            }

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

        public enum AIStates
        {
            OnSpawnAnmi,
            OnKillAnmi,
            /// <summary> 你给陆大有~ </summary>
            EscapeAnmi,
            /// <summary> 一阶段和2阶段的切换，使用在2阶段 </summary>
            P1ToP2Exchange,

            //--------------- 一阶段 ---------------

            /// <summary> 一阶段招式：召唤小影子球 </summary>
            SummonSmallShdowBall,
            /// <summary> 一阶段招式：影之公转 </summary>
            Revolution,
            /// <summary> 一阶段招式：星轨 </summary>
            Starline,
            /// <summary> 一阶段招式：月食 </summary>
            LunarEclipse,
            /// <summary> 一阶段招式：小球到场地左右两边射激光 </summary>
            //LeftRightLaser,
            /// <summary> 一阶段招式：照影 </summary>
            ShadowShoot,
            /// <summary> 一阶段招式：影刺 </summary>
            ShadowSpike,
            ///// <summary> 一阶段招式：依次射激光 </summary>
            //RandomLaser_Master,
            /// <summary> 一阶段特殊招式：黑暗窥视 </summary>
            DarkSeek,
            //--------------- 二阶段 ---------------

            /// <summary> 二阶段招式，跳起后斜向下冲刺之后玩家在头顶就升龙拳宰回旋砍，不在就只回旋砍 </summary>
            SmashDown,
            /// <summary> 二阶段招式，与玩家尝试水平后进行斩击，之后大风车 </summary>
            VerticalRolling,
            /// <summary> 二阶段招式，先向斜上方冲刺，之后下砸 </summary>
            SkyJump,
            /// <summary> 二阶段招式，横向冲刺，主要用于过渡 </summary>
            HorizontalDash,
            /// <summary> 二阶段招式，水平冲刺，之后冲向灯之影的位置并向四周抛出弹幕 </summary>
            NightmareKingDash,

            //--------------- 三阶段 ---------------


        }

        public void Initialize()
        {
            //NPC.Center = CoraliteWorld.shadowBallsFightArea.Center.ToVector2();
            NPC.dontTakeDamage = true;

            //MovementLimitRect = CoraliteWorld.shadowBallsFightArea;
            //MovementLimitRect.X += 200;
            //MovementLimitRect.Y += 200;
            //MovementLimitRect.Width -= 400;
            //MovementLimitRect.Height -= 400;

            //CanDamage = false;

            //NPC.oldPos = new Vector2[ShadowCount];
            alpha = 1;

            InitLocks();
        }

        public override void AI()
        {
            if (!spawn)
            {
                Initialize();
                spawn = true;
            }

            EnsureAiMachine();

            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Main.dayTime)
            {
                // 丢失目标：重新索敌后继续当前招式（保留阶段不变，避免顶层状态被强行回退导致阶段判定错乱）。
                NPC.TargetClosest();

                if (Main.dayTime && (Target.dead || !Target.active))
                {
                    NPC.EncourageDespawn(10);
                    NPC.dontTakeDamage = true;
                    NPC.velocity.Y += 0.25f;
                    return;
                }
            }

            Lighting.AddLight(NPC.Center, new Vector3(1f, 0.5f, 1.8f));

            // 一阶段每帧刷新小球列表（两端都跑，用于招式协调与阶段判定），与旧 AI 行为一致。
            if (Phase == (int)AIPhases.P1_WithSmallBalls && CurrentStateId != (int)ShadowBallStateId.OnSpawnAnim)
            {
                GetSmallBalls();
            }

            StateMachine.Update();

            UpdateSharedVisuals();
        }

        private void EnsureAiMachine()
        {
            if (aiBootstrapped)
            {
                return;
            }

            AiContext = new ShadowBallContext(this);
            StateMachine = new CoraliteBossStateMachine<ShadowBallContext>(AiContext);

            // 阶段切换：一阶段处于普通招式时，小球全灭 -> 进入 P1ToP2Exchange（仅服务端裁决，客户端经 ai[0] 同步跟随）。
            PhaseController.For(StateMachine)
                //.OnCondition(_ => IsInPhase1Attack() && smallBallCount == 0,
                //    () => VaultStateRegistry<ShadowBallContext>.Create((int)ShadowBallStateId.P1ToP2Exchange))
                .Apply();

            StateMachine.SetInitialState(VaultStateRegistry<ShadowBallContext>.Create((int)ShadowBallStateId.OnSpawnAnim));
            RefreshAttackRandom();
            aiBootstrapped = true;
        }

        /// <summary>是否处于一阶段（带小球）的常规招式状态（排除出生动画/狂暴/阶段切换）。</summary>
        //private bool IsInPhase1Attack()
        //{
        //    int id = CurrentStateId;
        //    return id >= (int)ShadowBallStateId.RollingLaser && id <= (int)ShadowBallStateId.RandomLaser;
        //}

        private void UpdateSharedVisuals()
        {
            switch (Phase)
            {
                case AIPhases.P1_WithSmallBalls:
                P1_WithSmallBalls:
                    foreach (var shadowLock in shadowLocks)
                    {
                        shadowLock.Update(this);
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
                    switch (State)
                    {
                        default:
                            break;
                        case AIStates.OnSpawnAnmi://略显弱智的写法
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

        public void RefreshAttackRandom()
        {
            AttackRandom = AiContext?.CreateAttackRandom() ?? new Random(NPC.whoAmI + 1);
        }

        /// <summary>
        /// 一阶段招式权重表（等价旧 <c>Main.rand.Next(6)</c> 的均匀分布），仅服务端在 <see cref="CompleteCurrentAttack"/> 内选取。
        /// </summary>
        private static readonly WeightedRandomPicker<ShadowBallStateId> Phase1Picker = new(new (ShadowBallStateId, float)[]
        {
            (ShadowBallStateId.Revolution, 1f),
            //(ShadowBallStateId.ConvergeLaser, 1f),
            //(ShadowBallStateId.LaserWithBeam, 1f),
            //(ShadowBallStateId.LeftRightLaser, 1f),
            //(ShadowBallStateId.RollingShadowPlayer, 1f),
            //(ShadowBallStateId.RandomLaser, 1f),
        });

        /// <summary>
        /// 二阶段招式权重表（等价旧 <c>Main.rand.Next(5)</c> 的均匀分布）。
        /// </summary>
        private static readonly WeightedRandomPicker<ShadowBallStateId> Phase2Picker = new(new (ShadowBallStateId, float)[]
        {
            (ShadowBallStateId.SmashDown, 1f),
            //(ShadowBallStateId.VerticalRolling, 1f),
            //(ShadowBallStateId.SkyJump, 1f),
            //(ShadowBallStateId.HorizontalDash, 1f),
            //(ShadowBallStateId.NightmareKingDash, 1f),
        });

        /// <summary>招式收尾：仅服务端推进到下一个招式状态（ai[0] 自动同步给客户端）。</summary>
        public void CompleteCurrentAttack()
        {
            if (VaultUtils.isClient || StateMachine == null)
            {
                return;
            }

            IVaultState<ShadowBallContext> next = PickNextAttackState();
            if (next != null)
            {
                StateMachine.ChangeState(next);
            }
        }

        /// <summary>
        /// 仅服务端：按当前阶段用权重选招器选取下一个招式。<br/>
        /// 服务端用 <see cref="Main.rand"/> 取 seed，权重选招纯函数化，结果以状态 ID 经 ai[0] 同步，无需再单独同步 seed。
        /// </summary>
        public IVaultState<ShadowBallContext> PickNextAttackState()
        {
            WeightedRandomPicker<ShadowBallStateId> picker =
                /*Phase == (int)AIPhases.P2_ShadowPlayer ?*/ Phase2Picker /*: Phase1Picker*/;

            int seed = Main.rand.Next();
            ShadowBallStateId pick = picker.Pick(seed).Item;
            return VaultStateRegistry<ShadowBallContext>.Create((int)pick);
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

                switch (owner.LockState)
                {
                    case LockStates.Normal:
                        baseRot = Helper.Lerp(baseRot, owner.Timer * 0.01f * (layer + 1), owner.LockLerpPercent);
                        zyRot = Helper.Lerp(zyRot, 1.57f + MathF.Sin(owner.Timer * (0.01f + layer * 0.005f)) * (0.4f + layer * 0.1f), owner.LockLerpPercent);
                        xyRot = Helper.Lerp(xyRot, MathHelper.TwoPi / 3 * layer + owner.Timer * 0.01f, owner.LockLerpPercent);

                        break;
                    case LockStates.ConcentricCircles:
                        baseRot = Helper.Lerp(baseRot, owner.Timer * 0.01f * (layer + 1), 1-owner.LockLerpPercent);
                        zyRot = Helper.Lerp(zyRot, 0, owner.LockLerpPercent);
                        xyRot = Helper.Lerp(xyRot, 0, owner.LockLerpPercent);

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
            public void _3DRotate( float Radius, float baseRot, float zyRot, float xyRot)
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
                rotation = 0;

                //vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationX(-MathHelper.PiOver2));///以Z为轴旋转，用来配合影子球自身的旋转

                zDepth = vector3D.Z * Radius;
            }

            public void Draw(Texture2D tex, SpriteBatch spriteBatch)
            {
                var frameBox = tex.Frame(5, 2, LockCoreFrame, 1);

                Vector2 pos = offset + center - Main.screenPosition;
                Color lightColor = Lighting.GetColor((center + offset).ToTileCoordinates());
                float scale = 1 + Utils.Remap(zDepth / 140, -1, 1, -0.25f, 0.5f);


                spriteBatch.Draw(tex, pos, frameBox, Color.White, offset.ToRotation()+MathHelper.PiOver2, frameBox.Size() / 2, scale, 0, 0);

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

        #endregion


        #region HelperMethods

        /// <summary>
        /// 小球总量上限
        /// </summary>
        /// <returns></returns>
        public int GetMaxSmallBall()
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
        public int GetSmallBallSameTimeLimit()
        {
            int maxSmallBall = Helper.ScaleValueForDiffMode(8, 10, 12, 14);

            if (Main.getGoodWorld)//天顶超级加倍
                maxSmallBall = 20;

            return maxSmallBall;
        }

        /// <summary>
        /// 获取所有小球
        /// </summary>
        /// <returns></returns>
        public bool GetSmallBalls()
        {
            smallBalls.Clear();
            int count = 0;

            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type == ModContent.NPCType<SmallShadowBall>()&&
                    npc.ai[0] == NPC.whoAmI &&
                    npc.ai[1] != (int)SmallShadowBall.AIStates.OnKillAnmi)
                {
                    smallBalls.Add(npc);
                    count++;
                }
            }

            //smallBallCount = count;
            if (count == 0)
                return false;

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
        public bool CheckAllSmallBallsTerminated()
        {
            //if (smallBallCount == 0)
            //{
            //    return false;
            //}

            foreach (var ball in smallBalls)
            {
                if (ball.ModNPC is not SmallShadowBall sb)
                {
                    return false;
                }

                sb.EnsureStateMachinePublic();
                if (sb.StateMachine == null || !sb.StateMachine.IsTerminated)
                {
                    return false;
                }
            }

            return true;
        }

        //private static int NextSmallBallSeed(Random attackRandom)
        //{
        //    return attackRandom.Next();
        //}

        public void SetDirection(Vector2 targetPos, out float xLength, out float yLength)
        {
            xLength = NPC.Center.X - targetPos.X;
            yLength = NPC.Center.Y - targetPos.Y;

            NPC.direction = NPC.spriteDirection = xLength > 0 ? -1 : 1;
            NPC.directionY = yLength > 0 ? -1 : 1;

            xLength = Math.Abs(xLength);
            yLength = Math.Abs(yLength);
        }

        public void SpawnSmallBalls()
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            //for (int i = 0; i < 5; i++)
            //{
            //    int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y,
            //        ModContent.NPCType<SmallShadowBall>(), NPC.whoAmI, NPC.whoAmI);
            //    (Main.npc[index].ModNPC as SmallShadowBall).smallBallType = i;
            //    (Main.npc[index].ModNPC as SmallShadowBall).shadowCircle =
            //        new ShadowCircleController
            //        (ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "SmallCircle" + i, ReLogic.Content.AssetRequestMode.ImmediateLoad));
            //}
        }

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

                        DrawLocks(true, spriteBatch);

                        DrawShadowShellLayerBack(spriteBatch, tex, pos, lightColor);
                        DrawCore(spriteBatch, tex, pos);
                        DrawShadowShellLayerFront(spriteBatch, tex, pos, lightColor);

                        DrawLocks(false, spriteBatch);
                    }
                    break;
                case AIPhases.P2_ShadowPlayer:
                    break;
                case AIPhases.P3_BigBallSmash:
                    break;
                case AIPhases.Others:
                    switch (State)
                    {
                        default:
                            break;
                        case AIStates.OnSpawnAnmi://略显弱智的写法
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
            var frameBox = tex.Frame(7, 1, 3, 0);

            spriteBatch.Draw(tex, center, frameBox, lightColor, Main.GlobalTimeWrappedHourly * 1.5f, frameBox.Size() / 2, NPC.scale, 0, 0);

            //绘制核心
            frameBox = tex.Frame(7, 1, 2, 0);

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
            //绘制最底部花纹
            var frameBox = tex.Frame(7, 1, 6, 0);

            spriteBatch.Draw(tex, center, frameBox, drawColor, 0, frameBox.Size() / 2, NPC.scale, 0, 0);

            //绘制遮罩
            frameBox = tex.Frame(7, 1, 5, 0);

            spriteBatch.Draw(tex, center, frameBox, new Color(255, 255, 255, (byte)(255 * MaskAlpha)), Main.GlobalTimeWrappedHourly * 1.5f, frameBox.Size() / 2, NPC.scale, 0, 0);

            //绘制旋转能量层
            frameBox = tex.Frame(7, 1, 4, 0);

            spriteBatch.Draw(tex, center, frameBox, drawColor, Main.GlobalTimeWrappedHourly * 2f, frameBox.Size() / 2, NPC.scale, 0, 0);
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
            var frameBox = tex.Frame(7, 1, 1, 0);

            spriteBatch.Draw(tex, center, frameBox, new Color(255, 255, 255, (byte)(255 * MaskAlpha)), Main.GlobalTimeWrappedHourly * 1.5f, frameBox.Size() / 2, NPC.scale, 0, 0);

            //绘制最顶部球层
            frameBox = tex.Frame(7, 1, 0, 0);

            spriteBatch.Draw(tex, center, frameBox, drawColor, 0, frameBox.Size() / 2, NPC.scale, 0, 0);
        }
        #endregion

        public void DrawSmallBalls(bool back, SpriteBatch spriteBatch)
        {
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
