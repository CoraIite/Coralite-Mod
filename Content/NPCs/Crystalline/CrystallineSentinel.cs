using Coralite.Content.Biomes;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.SmoothFunctions;
using Coralite.Core.Systems.MagikeSystem.Particles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineSentinel : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        [VaultLoaden("{@classPath}" + "CrystallineSentinelFloatStone")]
        public static ATex FloatStone { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelGuard")]
        public static ATex GuardTex { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelExchange")]
        public static ATex ExchangeTex { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2")]
        public static ATex P2Head { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2Float")]
        public static ATex P2Float { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelHand")]
        public static ATex HandTex { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2Spurt")]
        public static ATex P2SpurtTex { get; private set; }

        private SecondOrderDynamics_Vec2[] FloatStoneMoves;
        private Vector2[] FloatStonePos;

        private SecondOrderDynamics_Vec2 P2FloatMover;
        private Vector2 P2FloatCenter;

        private SecondOrderDynamics_Vec2[] P2HandMover;
        private Vector2[] P2HandCenter;
        private int[] P2HandFrame;
        //private int[] P2HandFrameCounter;



        private const int MaxHandFrame = 12;

        private AIStates State
        {
            get => (AIStates)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        private ref float Timer => ref NPC.ai[1];
        private ref float Recorder => ref NPC.ai[2];

        private bool CanHit
        {
            get => NPC.ai[3] == 1;
            set
            {
                if (value)
                    NPC.ai[3] = 1;
                else
                    NPC.ai[3] = 0;
            }
        }

        private TextTypes TextType
        {
            get => (TextTypes)NPC.localAI[2];
            set => NPC.localAI[2] = (int)value;
        }

        private Player Target => Main.player[NPC.target];

        private bool Init = true;
        /// <summary>
        /// 该变量不同步，仅作视觉效果等使用
        /// </summary>
        private ref float Recorder2 => ref NPC.localAI[0];

        private enum AIStates
        {
            P1Idle,
            P1Walking,

            P1Guard,
            P1Spurt,

            Exchange,

            /// <summary>
            /// 二阶段悬浮
            /// </summary>
            P2Idle,
            /// <summary>
            /// 二阶段螺旋冲刺
            /// </summary>
            P2Rolling,
            /// <summary>
            /// 二阶段挥刀
            /// </summary>
            P2Swing,
        }

        private enum TextTypes
        {
            None,
            Surprise,
            Confusion,
            Hurt,
        }

        #region 基础设置

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 21;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 80;
            NPC.damage = 70;
            NPC.defense = 45;

            NPC.lifeMax = 2000;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastHurt;
            NPC.DeathSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            NPC.noGravity = false;
            NPC.value = Item.buyPrice(0, 2);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //固定掉落蕴魔水晶
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineMagike>(), 1, 4, 12));


            //掉落宝石原石
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryRoughGemstone>(), 1, 2, 4));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SeniorRoughGemstone>(), 1, 2, 4));
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return CanHit;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0;//暂时不让它生成

            if (Main.hardMode && spawnInfo.Player.InModBiome<CrystallineSkyIsland>())
            {
                int tileY = spawnInfo.SpawnTileY;
                for (int i = 0; i < 45; i++)
                {
                    Tile t = Framing.GetTileSafely(spawnInfo.SpawnTileX, tileY);
                    if (t.HasTile && Main.tileSolid[t.TileType])
                        break;

                    tileY++;
                }

                Tile t2 = Framing.GetTileSafely(spawnInfo.SpawnTileX, tileY);
                if (!t2.HasTile || !Main.tileSolid[t2.TileType] || !Main.tileBlockLight[t2.TileType])//必须得是遮光物块
                    return 0;

                if (Helper.IsPointOnScreen(new Vector2(spawnInfo.SpawnTileX, tileY) * 16 - Main.screenPosition))
                    return 0;
                else
                    return 0.01f;
            }

        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            for (int i = 0; i < 45; i++)
            {
                Tile t = Framing.GetTileSafely(tileX, tileY);
                if (t.HasTile && Main.tileSolid[t.TileType])
                    break;

                tileY++;
            }

            return NPC.NewNPC(new EntitySource_SpawnNPC(), tileX * 16 + 8, tileY * 16, NPC.type);
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (State == AIStates.P1Guard)//防御状态不绘制血条
                return false;

            return null;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            if (State == AIStates.P1Guard)//防御状态鼠标移上去没效果
            {
                boundingBox = default;
                return;
            }

            boundingBox = Utils.CenteredRectangle(NPC.Center, NPC.Size);
        }

        #endregion

        #region AI

        public override void AI()
        {
            if (Init)
            {
                Init = false;
                Initialize();
            }

            switch (State)
            {
                case AIStates.P1Idle:
                    P1Idle();

                    SetFrame(0, 0);
                    P1FloatStoneMove();
                    break;
                case AIStates.P1Walking:
                    P1Walk();
                    WalkFrame();
                    P1FloatStoneMove();
                    break;
                case AIStates.P1Guard:
                    P1Guard();
                    P1FloatStoneMove();
                    break;
                case AIStates.P1Spurt:
                    P1Spurt();
                    P1FloatStoneMove();
                    break;
                case AIStates.Exchange:
                    Exchange();
                    break;
                case AIStates.P2Idle:
                    P2Idle();
                    break;
                case AIStates.P2Rolling:
                    break;
                case AIStates.P2Swing:
                    P2Swing();
                    break;
                default:
                    break;
            }
        }

        public void Initialize()
        {
            //初始化各种视觉效果运动
            if (!VaultUtils.isServer)
            {
                FloatStonePos ??= [NPC.Center, NPC.Center, NPC.Center];
                FloatStoneMoves ??= [new SecondOrderDynamics_Vec2(0.8f,0.75f,1,NPC.Center)
                    ,new SecondOrderDynamics_Vec2(0.6f,0.5f,1,NPC.Center)
                    ,new SecondOrderDynamics_Vec2(0.6f,0.5f,1,NPC.Center)];
            }
        }

        #region 一阶段非攻击状态

        public void P1Idle()
        {
            //一动不动
            NPC.velocity.X = 0;

            if (Timer % 45 == 0)
                TryTurnToAttack();

            if (Timer < 0)//随便走走
                SwitchStateP1(AIStates.P1Walking, Main.rand.Next(60 * 2, 60 * 4), true);

            Timer--;
        }

        public void P1Walk()
        {
            //走不了或者时间大于一定值后就歇一会
            if (!CanWalkForward() || Timer < 0)
            {
                SwitchStateP1(AIStates.P1Idle, Main.rand.Next(60 * 4, 60 * 6));
                return;
            }

            //走路
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            if (MathF.Abs(NPC.velocity.X) < 0.8f)
                NPC.velocity.X += NPC.direction * 0.1f;

            if (Timer % 45 == 0)
                TryTurnToAttack();

            Timer--;
        }

        public void WalkFrame()
        {
            NPC.frame.X = 1;
            float xSpeed = 8 - MathF.Abs(NPC.velocity.X);

            if (++NPC.frameCounter > xSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y++;
            }

            if (NPC.frame.Y > 10)
                NPC.frame.Y = 0;
        }

        public bool CanWalkForward()
        {
            //检测前方的 （自身宽度）格的下方两格的物块，大于2格物块时才能向前走

            Vector2 pos = NPC.Bottom;

            pos.X += NPC.direction * NPC.width / 2;
            pos.Y += 4;

            int checkX = (int)(NPC.width / 16f);

            int soildTileCount = 0;//脚下有多少格实心的

            for (int i = 0; i < checkX; i++)
            {
                bool hasTile = false;

                for (int j = 0; j < 2; j++)
                {
                    Tile t = Framing.GetTileSafely(pos + new Vector2(NPC.direction * i * 16, j * 16));
                    if (t.HasSolidTile())
                    {
                        hasTile = true;
                        break;
                    }
                }

                if (hasTile)
                    soildTileCount++;
            }

            return soildTileCount >= checkX - 1;
        }

        #endregion

        #region 一阶段攻击状态

        public void P1Spurt()
        {
            //一直走直到与玩家进到一定距离，之后进入戳刺的动画，如果无法攻击到玩家那么就有概率变为防守形态

            switch (Recorder)
            {
                default:
                case 0://朝玩家走
                    {
                        if (Timer % 60 * 2 == 0)
                        {
                            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
                            NPC.spriteDirection = NPC.direction;
                        }

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
                        if (MathF.Abs(NPC.velocity.X) < 1.3f)
                            NPC.velocity.X += NPC.direction * 0.1f;
                        if (MathF.Sign(NPC.velocity.X) != NPC.direction)
                            NPC.velocity.X = 0;
                        WalkFrame();

                        float distance = Vector2.Distance(Target.Center, NPC.Center);
                        if (distance < 16 * 24)
                        {
                            Timer += 2;
                            if (distance < 16 * 10)
                                Timer += 5;
                        }

                        if (Timer > 60 * 8 || !CanWalkForward())
                        {
                            if (CanHitTarget())//可以攻击，进入下一阶段
                            {
                                NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
                                NPC.spriteDirection = NPC.direction;
                                NPC.velocity.X = 0;
                                NPC.frame.X = 2;
                                NPC.frame.Y = 0;
                                Recorder = 1;
                                Timer = 0;

                                NPC.netUpdate = true;
                            }
                            else
                                SwitchStateP1(AIStates.P1Idle, 60);
                        }

                        Timer++;
                    }
                    break;
                case 1://戳
                    {
                        if (Timer > 0 && Timer % 5 == 0)
                        {
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 20)
                                NPC.frame.Y = 20;
                        }

                        if (Timer == 5 * 6)//生成戳刺弹幕
                        {
                            Helper.PlayPitched(CoraliteSoundID.Crystal_Item101, NPC.Center, pitch: 0.8f);
                            CanHit = true;
                            NPC.NewProjectileInAI<CrystallineSentinelSpurtProj>(NPC.Center + new Vector2(NPC.direction * 40, 0)
                                , Vector2.Zero, Helper.ScaleValueForDiffMode(200, 350, 480, 1000)
                                , 0, ai0: NPC.whoAmI, ai1: NPC.direction);
                        }
                        Timer++;

                        if (Timer > 5 * 21)
                            SwitchStateP1(AIStates.P1Guard);
                    }
                    break;
            }
        }

        public void P1Guard()
        {
            //走几秒钟后进入防御阶段
            switch (Recorder)
            {
                default:
                case 0:
                    {
                        if (Timer % 60 == 0)
                        {
                            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
                            NPC.spriteDirection = NPC.direction;
                        }

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
                        if (MathF.Abs(NPC.velocity.X) < 1.3f)
                            NPC.velocity.X += NPC.direction * 0.1f;
                        if (MathF.Sign(NPC.velocity.X) != NPC.direction)
                            NPC.velocity.X = 0;
                        WalkFrame();

                        if (Timer > 60 * 3 || !CanWalkForward())
                        {
                            if (CanHitTarget())//可以攻击，进入下一阶段
                            {
                                NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
                                NPC.spriteDirection = NPC.direction;
                                NPC.velocity.X = 0;
                                SetFrame(3, 0);
                                Recorder = 1;
                                Timer = 0;

                                PRTLoader.NewParticle<MagikeLozengeParticle>(NPC.Center, Vector2.Zero, Coralite.CrystallinePurple, 1f);

                                NPC.netUpdate = true;
                            }
                            else
                                SwitchStateP1(AIStates.P1Idle, 60);
                        }
                        Timer++;
                    }
                    break;
                case 1:
                    {
                        //更新帧图，逐渐变为防御
                        if (Timer > 0 && Timer % 4 == 0)
                        {
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 8)
                                NPC.frame.Y = 8;
                        }

                        if (Timer > 4 * 6)//展开护盾
                        {
                            Recorder2 += 0.1f;
                            if (Recorder2 > 1)
                                Recorder2 = 1;
                        }

                        if (Timer > 4 * 9)
                        {
                            Recorder++;
                            Timer = 0;
                            NPC.frame.Y = 8;
                            NPC.SuperArmor = true;
                            NPC.netUpdate = true;

                            //防御音效
                        }

                        Timer++;
                    }
                    break;
                case 2://防御中
                    {
                        if (Timer > 60 * 3)
                        {
                            Recorder++;
                            Timer = 0;
                            SetFrame(4, 0);
                            NPC.SuperArmor = false;
                            NPC.netUpdate = true;
                        }
                        Timer++;
                    }
                    break;
                case 3://后摇
                    {
                        if (Timer > 0 && Timer % 5 == 0)
                        {
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 4)
                                NPC.frame.Y = 4;
                        }

                        Recorder2 -= 0.08f;
                        if (Recorder2 < 0)
                            Recorder2 = 0;

                        if (Timer > 5 * 5)
                            SwitchStateP1(AIStates.P1Idle, 60);

                        Timer++;
                    }
                    break;
            }
        }

        #endregion

        #region 二阶段

        public void P2Idle()
        {
            if (++NPC.frameCounter > 3)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 7)
                    NPC.frame.Y = 0;
            }

            //检查玩家
            if (Timer % 30 == 0)
                TryTurnToAttack();

            if (Timer % 60 == 0)
            {
                NPC.velocity = NPC.velocity.RotateByRandom(-0.9f, 0.9f);
            }

            Timer++;

            //撞墙反飞
            CollideSpeed();
            SpeedUp(2, 0.05f);


            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            P2FloatCenter = P2FloatMover.Update(1 / 60f, GetP2FloatPos);
            UpdateP2HandPosNormally();

            NPC.velocity *= 0.95f;

            Timer++;
        }

        /// <summary>
        /// 撞墙反方向飞
        /// </summary>
        public void CollideSpeed()
        {
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -0.4f;
            }

            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * -0.4f;
            }
        }

        //将速度加到目标速度
        public void SpeedUp(float maxSpeed, float acc, float blurSpeed = 1f)
        {
            float speed = NPC.velocity.Length();
            if (speed < maxSpeed)
            {
                speed += acc;
                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * speed;
            }
            else if (speed > maxSpeed + blurSpeed)
                NPC.velocity = NPC.velocity * 0.97f;
        }

        public void P2Swing()
        {
            Timer++;

            NPC.velocity *= 0.98f;
            CollideSpeed();

            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            P2FloatCenter = P2FloatMover.Update(1 / 60f, GetP2FloatPos);
            UpdateP2HandPosNormally();

            const int readyTime = 3 * 6 + 1;
            const int idleTime = 100;
            const int delayTime = 3 * 7;

            if (Timer < readyTime)
            {
                if (Timer % 3 == 0)
                {
                    int whichHand = Recorder > 0 ? 1 : 0;
                    P2HandFrame[whichHand]++;
                }
            }
            else if (Timer == readyTime)
            {
                NPC.NewProjectileDirectInAI<CrystallineSentinelSwing>(NPC.Center + (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero)
                    , Vector2.Zero, Helper.GetProjDamage(120, 140, 180)
                    , 1, NPC.target, NPC.whoAmI);
            }
            else if (Timer < readyTime + idleTime)
            {
            }
            else if (Timer < readyTime + idleTime + delayTime)
            {
                if (Timer % 3 == 0)
                {
                    int whichHand = Recorder > 0 ? 1 : 0;
                    if (P2HandFrame[whichHand] < 12)
                        P2HandFrame[whichHand]++;
                }
            }
            else
            {
                SwitchStateP2(AIStates.P2Idle);
            }
        }

        public void P2Rolling()
        {

        }

        #endregion

        public Vector2 GetP2FloatPos => NPC.Center + new Vector2(-NPC.spriteDirection * 4, 18);
        public Vector2 P2LeftHandPos => NPC.Center + new Vector2(-2, -24);
        public Vector2 P2RightHandPos => NPC.Center + new Vector2(-4, -23);

        public void UpdateP2HandPosNormally()
        {
            P2HandCenter[0] = P2HandMover[0].Update(1 / 60f, P2LeftHandPos);
            P2HandCenter[1] = P2HandMover[1].Update(1 / 60f, P2RightHandPos);
        }

        public void Exchange()
        {
            NPC.velocity.X *= 0;
            if (Timer % 5 == 0)
            {
                NPC.frame.Y++;
                if (NPC.frame.Y > 18)
                    NPC.frame.Y = 18;
            }

            if (Timer > 5 * 18)
            {
                SetFrame(0, 0);
                P2FloatCenter = GetP2FloatPos;
                P2FloatMover = new SecondOrderDynamics_Vec2(0.9f, 0.8f, 0, GetP2FloatPos);

                P2HandCenter = [
                    P2LeftHandPos,
                    P2RightHandPos,
                    ];
                P2HandMover = [
                    new SecondOrderDynamics_Vec2(0.9f, 0.8f, 0, P2LeftHandPos),
                    new SecondOrderDynamics_Vec2(0.9f, 0.8f, 0, P2RightHandPos),
                    ];

                P2HandFrame = [MaxHandFrame, MaxHandFrame];
                //P2HandFrameCounter = [0, 0];

                SwitchStateP2(AIStates.P2Idle, 60);

                Timer = 20;
                NPC.velocity = new Vector2(0, -1);

                return;
            }

            Timer++;
        }

        public void P1FloatStoneMove()
        {
            if (VaultUtils.isServer)
                return;

            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = NPC.Center + new Vector2(0, -10) + (4f + i * MathHelper.TwoPi / 3).ToRotationVector2() * 36 * NPC.scale;
                float factor = (int)Main.timeForVisualEffects * 0.02f + i * MathHelper.TwoPi / 2;
                pos += new Vector2(MathF.Cos(factor) * 3, MathF.Sin(factor) * 6);

                FloatStonePos[i] = FloatStoneMoves[i].Update(1 / 60f, pos);
            }
        }

        private void SwitchStateP1(AIStates targetState, int? overrideTime = null, bool randDirection = false)
        {
            Recorder = 0;
            Recorder2 = 0;
            CanHit = false;

            NPC.SuperArmor = false;

            if (!VaultUtils.isClient)
            {
                Timer = overrideTime ?? 0;

                if (randDirection)
                    NPC.direction = NPC.spriteDirection = Main.rand.NextFromList(-1, 1);

                NPC.netUpdate = true;
            }
            else
            {
                Timer = 6;//给客户端6帧时间用于同步延迟
            }

            if (NPC.life < NPC.lifeMax / 2)
            {
                State = AIStates.Exchange;
                SetFrame(0, 0);
                NPC.noGravity = true;
                Timer = 0;
            }
            else
                State = targetState;

        }

        private void SwitchStateP2(AIStates targetState, int? overrideTime = null, bool randDirection = false)
        {
            State = targetState;
            Recorder = 0;
            Recorder2 = 0;
            CanHit = false;

            NPC.SuperArmor = false;

            if (targetState == AIStates.P2Swing)
                Recorder = Main.rand.NextFromList(-1, 1);

            P2HandFrame[0] = 0;
            P2HandFrame[1] = 0;
            if (!VaultUtils.isClient)
            {
                Timer = overrideTime ?? 0;

                if (randDirection)
                    NPC.direction = NPC.spriteDirection = Main.rand.NextFromList(-1, 1);

                NPC.netUpdate = true;
            }
            else
            {
                Timer = 6;//给客户端6帧时间用于同步延迟
            }
        }

        private void TryTurnToAttack()
        {
            NPC.TargetClosest(false);

            //找到玩家了就转向攻击阶段
            if (NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget())//不攻击隐身玩家
                StartAttack();
        }

        /// <summary>
        /// 需要玩家不隐身（隐身是指有隐身药水BUFF和不在使用物品）<br></br>
        /// 需要距离小于1000，并且可以看到玩家
        /// </summary>
        /// <returns></returns>
        public bool CanHitTarget()
            => !(Target.invis && Target.itemAnimation == 0) && Vector2.Distance(NPC.Center, Target.Center) < 1000 &&
                    Collision.CanHit(NPC.Center, 1, 1, Target.TopLeft, Target.width, Target.height);

        public void StartAttack()
        {
            switch (State)
            {
                case AIStates.P1Idle:
                case AIStates.P1Walking:
                    SwitchStateP1(AIStates.P1Spurt);
                    break;
                case AIStates.P2Idle:
                    SwitchStateP2(AIStates.P2Swing);
                    break;
                default:
                    break;
            }

            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;
        }

        public void SetFrame(int frameX, int frameY)
        {
            NPC.frame.X = frameX;
            NPC.frame.Y = frameY;
        }

        #endregion

        #region 网络同步

        #endregion

        #region 绘制

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effect = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            switch (State)
            {
                case AIStates.P1Idle:
                case AIStates.P1Walking:
                case AIStates.P1Guard:
                case AIStates.P1Spurt://一阶段绘制
                    {
                        Texture2D tex = NPC.GetTexture();

                        Rectangle frameBox = tex.Frame(5, Main.npcFrameCount[NPC.type], NPC.frame.X, NPC.frame.Y);

                        spriteBatch.Draw(tex, NPC.Center - screenPos + new Vector2(NPC.spriteDirection * 40, -4), frameBox, drawColor
                            , NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 0);

                        if (FloatStonePos != null)
                        {
                            tex = FloatStone.Value;

                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 pos = FloatStonePos[i] - screenPos;
                                frameBox = tex.Frame(3, 1, i, 0);

                                spriteBatch.Draw(tex, pos, frameBox, drawColor
                                    , 0, frameBox.Size() / 2, NPC.scale, effect, 0);
                            }
                        }

                        if (Recorder2 != 0)
                            DrawGuard(spriteBatch, screenPos);
                    }
                    break;
                case AIStates.Exchange:
                    {
                        Texture2D tex = ExchangeTex.Value;

                        Rectangle frameBox = tex.Frame(1, 19, NPC.frame.X, NPC.frame.Y);

                        spriteBatch.Draw(tex, NPC.Center - screenPos + new Vector2(0, -22), frameBox, drawColor
                            , NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 0);
                    }
                    break;
                case AIStates.P2Idle:
                case AIStates.P2Swing:

                    bool faceLeft = NPC.spriteDirection < 0;

                    if (faceLeft)
                        DrawP2Hand(spriteBatch, screenPos, effect, drawColor, 0);
                    else
                        DrawP2Hand(spriteBatch, screenPos, effect, drawColor, 1);

                    DrawP2Float(spriteBatch, screenPos, effect, drawColor);
                    DrawP2Head(spriteBatch, screenPos, effect, drawColor);

                    if (faceLeft)
                        DrawP2Hand(spriteBatch, screenPos, effect, drawColor, 1);
                    else
                        DrawP2Hand(spriteBatch, screenPos, effect, drawColor, 0);

                    break;
                case AIStates.P2Rolling:
                    break;
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// 0左手，1右手
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="screenPos"></param>
        /// <param name="effect"></param>
        /// <param name="drawColor"></param>
        /// <param name="leftOrRight"></param>
        public void DrawP2Hand(SpriteBatch spriteBatch, Vector2 screenPos, SpriteEffects effect, Color drawColor, int leftOrRight)
        {
            Texture2D tex = HandTex.Value;

            Rectangle frameBox = tex.Frame(2, 13, leftOrRight, P2HandFrame[leftOrRight]);

            spriteBatch.Draw(tex, P2HandCenter[leftOrRight] - screenPos, frameBox, drawColor
                , 0, frameBox.Size() / 2, NPC.scale, effect, 0);
        }

        public void DrawP2Float(SpriteBatch spriteBatch, Vector2 screenPos, SpriteEffects effect, Color drawColor)
        {
            Texture2D tex = P2Float.Value;

            Rectangle frameBox = tex.Frame(1, 8, 0, NPC.frame.Y);

            spriteBatch.Draw(tex, P2FloatCenter - screenPos, frameBox, drawColor
                , 0, frameBox.Size() / 2, NPC.scale, effect, 0);
        }

        public void DrawP2Head(SpriteBatch spriteBatch, Vector2 screenPos, SpriteEffects effect, Color drawColor)
        {
            spriteBatch.Draw(P2Head.Value, NPC.Center - screenPos + new Vector2(0, -10), null, drawColor
                , NPC.rotation, P2Head.Size() / 2, NPC.scale, effect, 0);
        }

        public void DrawGuard(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D guardTex = GuardTex.Value;
            Vector2 pos = NPC.Center + new Vector2(0, -20) - screenPos;
            float scale = Helper.BezierEase(Recorder2);

            //最内层
            var framebox = guardTex.Frame(1, 3, 0, 0);
            spriteBatch.Draw(guardTex, pos, framebox, Color.White, Helper.SqrtEase(Recorder2) * MathHelper.TwoPi, framebox.Size() / 2, scale, 0, 0);

            //外层
            framebox = guardTex.Frame(1, 3, 0, 2);
            spriteBatch.Draw(guardTex, pos, framebox, Color.White, 0, framebox.Size() / 2, scale, 0, 0);

            //绘制闪光
            int visualTimer = (int)(Main.timeForVisualEffects % 30);
            if (visualTimer < 12)
                return;

            framebox = guardTex.Frame(1, 3, 0, 1);
            SpriteEffects effects = visualTimer < (12 + 9) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float alpha = 0.1f + MathF.Sin(visualTimer / 18f * MathHelper.TwoPi) * 0.3f;
            spriteBatch.Draw(guardTex, pos, framebox, Color.White * alpha, 0, framebox.Size() / 2, scale, effects, 0);
        }
        #endregion
    }

    public class CrystallineSentinelSpurtProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float TargetIndex => ref Projectile.ai[0];
        public ref float Direction => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = 120;
            Projectile.height = 32;
            Projectile.timeLeft = 10;
        }

        public override void AI()
        {
            if (!TargetIndex.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;

            Projectile.Center = owner.Center + new Vector2(Direction * Projectile.width / 2, 0);
            float factor = 1 - Projectile.timeLeft / 10f;

            Vector2 pos = Projectile.Center + new Vector2(Direction * (-80 + factor * 120), 8);
            Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(12, 20)
                    , DustID.PurpleTorch, new Vector2(Direction * factor * Main.rand.NextFloat(3, 6), 0), Scale: Main.rand.NextFloat(1, 2));
            d.noGravity = true;

            if (Projectile.timeLeft % 3 == 0)
            {
                PRTLoader.NewParticle<HorizonArcArrowParticle>(pos
                    , new Vector2(Direction * factor * 3, 0), Coralite.CrystallinePurple, 0.45f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineSentinelSwing : BaseSwingProj
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        //public ref float LeftOrRight => ref Projectile.ai[0];
        public ref float OwnerIndex => ref Projectile.ai[0];

        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradient")]
        public static ATex GradientTexture { get; set; }

        public CrystallineSentinelSwing() : base(0.785f, trailCount: 48) { }

        public int delay;
        public int alpha;

        public float dir;
        public float offsetLength;
        public Vector2 velocity;

        public override void SetSwingProperty()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 0;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 0;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 85 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
                dir = (Projectile.Center - npc.Center).ToRotation();

            Projectile.extraUpdates = 2;
            alpha = 0;
            startAngle = 0f;
            totalAngle = 30.5f;
            maxTime = 90 * 3;
            Smoother = Coralite.Instance.BezierEaseSmoother;
            delay = 20;
            distanceToOwner = 0;
            Projectile.localNPCHitCooldown = 60;

            base.InitializeSwing();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 1f);
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;

            if (alpha < 255)
                alpha += 2;
            if (timer % 30 == 0)
                onHitTimer = 0;

            offsetLength = Helper.SinEase(timer, maxTime) * 450;

            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 10;
            if (Projectile.scale > 0.8f)
            {
                Projectile.scale *= 0.999f;
            }

            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        protected override void AIAfter()
        {
            Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
            Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * Projectile.height / 2));//弹幕的底端和顶端计算，用于检测碰撞以及绘制

            if (useShadowTrail || useSlashTrail)
                UpdateCaches();
        }

        protected override Vector2 OwnerCenter()
        {
            if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
                return npc.Center + dir.ToRotationVector2() * offsetLength;

            return base.OwnerCenter();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor, extraRot);
        }

        protected override void DrawSlashTrail()
        {
            if (oldRotate == null)
                return;

            List<VertexPositionColorTexture> bars = new();
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
                Vector2 Center = GetCenter(i) - Main.screenPosition;
                Vector2 Top = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]));

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatBlurSmall.Value);
                    effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                        Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    }
                }, BlendState.NonPremultiplied, SamplerState.PointWrap, RasterizerState.CullNone);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            }
        }
    }
}