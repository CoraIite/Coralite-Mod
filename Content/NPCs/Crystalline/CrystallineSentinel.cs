using Coralite.Content.Biomes;
using Coralite.Content.Dusts;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Particles;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.SmoothFunctions;
using Coralite.Core.Systems.MagikeSystem.Particles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineSentinel : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        [VaultLoaden("{@classPath}" + "CrystallineSentinel_Glow")]
        public static ATex GlowTex { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelFloatStone")]
        public static ATex FloatStone { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelGuard")]
        public static ATex GuardTex { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelExchange")]
        public static ATex ExchangeTex { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelExchange_Glow")]
        public static ATex ExchangeTex_Glow { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2")]
        public static ATex P2Head { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2_Glow")]
        public static ATex P2Head_Glow { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2Float")]
        public static ATex P2Float { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2Float_Glow")]
        public static ATex P2Float_Glow { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelHand")]
        public static ATex HandTex { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelHand_Glow")]
        public static ATex HandTex_Glow { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2Spurt")]
        public static ATex P2SpurtTex { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2Spurt_Glow")]
        public static ATex P2SpurtTex_Glow { get; private set; }

        private SecondOrderDynamics_Vec2 P2FloatMover;
        private Vector2 P2FloatCenter;

        private SecondOrderDynamics_Vec2[] P2HandMover;
        private Vector2[] P2HandCenter;
        private int[] P2HandFrame;
        private int P2HandSpurtFrameY;
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

        /// <summary>
        /// 开盾计数器，受到伤害时目标在较远距离外会增加计数，达到一定伤害后会尝试开盾，参见<see cref="TryTurnToGuard"/>
        /// </summary>
        private ref float GuardCounter => ref NPC.localAI[1];

        private ref float Alerted => ref NPC.localAI[3];

        private float GuardCooldown;
        private const float GuardCooldownMax = 10 * 60;

        /// <summary>
        /// 一阶段仇恨计数器，玩家攻击时增加，否则减少
        /// 处于正数时会尝试攻击，达到最低时重新索敌，切换目标时重置为-1，参见<see cref="TryTurnToAttack"/>
        /// </summary>
        private int AggroCounter = 0;
        private const int AggroCounterMin = -5 * 60;
        private const int AggroCounterMax = 5 * 60;

        private const int AlertRange = 16 * 40;
        private const int MeleeRange = 16 * 16; //近战触发范围

        private bool ReleasedRock = false;
        private enum AIStates
        {
            /// <summary>
            /// 一阶段站立
            /// </summary>
            P1Idle,
            /// <summary>
            /// 一阶段闲逛
            /// </summary>
            P1Walking,
            /// <summary>
            /// 一阶段护盾
            /// </summary>
            P1Guard,
            /// <summary>
            /// 一阶段刺击
            /// </summary>
            P1Spurt,
            /// <summary>
            /// 一阶段飞弹
            /// </summary>
            P1Missile,
            /// <summary>
            /// 一阶段碎岩攻击
            /// </summary>
            P1Rock,
            /// <summary>
            /// 转阶段
            /// </summary>
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
            /// <summary>
            /// 二阶段旋风斩
            /// </summary>
            P2WhirlSlash
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
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
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
            if (State == AIStates.P1Guard && Recorder is 1 or 2)//防御状态不绘制血条
                return false;

            return null;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            if (State == AIStates.P1Guard && Recorder is 1 or 2)//防御状态鼠标移上去没效果
            {
                boundingBox = default;
                return;
            }

            boundingBox = Utils.CenteredRectangle(NPC.Center, NPC.Size);
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (Target == player)
                AggroCounter = AggroCounterMax;
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (Vector2.Distance(NPC.Center, Target.Center) > 500f)
                GuardCounter += damageDone;

            if (Target == Main.player[projectile.owner])
                AggroCounter = AggroCounterMax;
        }

        public override bool CanBeHitByNPC(NPC attacker)
        {
            return attacker.type == ModContent.NPCType<CrystallineSentinelMissile>() && attacker.ai[0] > 60;
            //return false;
        }
        public void OnHitByMissile()
        {
            ApplyGuardCD();

            if (State is not AIStates.P1Guard || Recorder is not 2)
                return;
            Recorder++;
            Timer = 0;
            SetFrame(4, 0);
            NPC.SuperArmor = false;
            NPC.netUpdate = true;

            for (int i = 0; i < 6 * 6; i++)
            {
                Vector2 position = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(16);
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 6);
                PRTLoader.NewParticle<CrystalFlashParticle>(position, vel * 0.75f + NPC.velocity * 0.25f);
            }
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
                    break;
                case AIStates.P1Walking:
                    P1Walk();
                    WalkFrame();
                    break;
                case AIStates.P1Guard:
                    P1Guard();
                    break;
                case AIStates.P1Spurt:
                    P1Spurt();
                    break;
                case AIStates.P1Missile:
                    P1Missile();
                    break;
                case AIStates.P1Rock:
                    P1Rock();
                    break;
                case AIStates.Exchange:
                    Exchange();
                    break;
                case AIStates.P2Idle:
                    P2Idle();
                    break;
                case AIStates.P2Rolling:
                    P2Rolling();
                    break;
                case AIStates.P2Swing:
                    P2Swing();
                    break;
                case AIStates.P2WhirlSlash:
                    P2WhirlSlash();
                    break;
                default:
                    break;
            }

            if (IsPhase2)
                P2NormalAI();
            else
                P1NormalAI();

            //Main.NewText($"{Main.GameUpdateCount} {State} {Timer} {Recorder} {AggroCounter} {GuardCounter}");
            //Main.NewText($"{Main.GameUpdateCount} {State} Timer:{Timer} Recorder:{Recorder} " +
            //    $"framex:{NPC.frame.X} framey:{NPC.frame.Y} {GuardCounter}");
        }

        public void Initialize()
        {
            //初始化各种视觉效果运动
            if (!VaultUtils.isServer)
            {
                float f = 0.6f;
                float z = 0.5f;
                float r = 1f;
                for(int i = 0; i < 3; i++)
                {
                    var prt = PRTLoader.NewParticle<CrystallineSentinelFloatStone>(NPC.Center, Vector2.Zero);
                    prt.FollowNPCIndex = NPC.whoAmI;
                    prt.ai[0] = i;
                    if(i == 0)
                    {
                        f = 0.8f;
                        z = 0.75f;
                    }
                    prt.FloatStoneMoves ??= new SecondOrderDynamics_Vec2(f, z, r, NPC.Center);
                }
            }

            AggroCounter = -1;
            Alerted = 0f; // 确保初始未警戒
        }

        #region 一阶段非攻击状态
        /// <summary>
        /// 一阶段常态AI，一阶段所有状态都会执行
        /// </summary>
        public void P1NormalAI()
        {
            GuardCooldown--;
            AggroCounter--;
            AggroCounter = (int)MathHelper.Clamp(AggroCounter, AggroCounterMin, AggroCounterMax);

            float light = 0.5f;
            Lighting.AddLight(HeadPos, light, light, light);
            //确保有当前目标引用
            if (!Target.Alives())
                NPC.TargetClosest(false);

            float distanceToTarget = float.MaxValue;
            if (Target.Alives())
                distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);

            //进入警戒范围：朝向玩家并生成攻击指示
            if (Target.Alives() && distanceToTarget <= AlertRange && Alerted == 0f)
            {
                Alerted = 1f;
                NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
                NPC.velocity = Vector2.UnitX * NPC.velocity.Length() * NPC.direction;

                var prt = PRTLoader.NewParticle<CrystallineAlertParticle>(HeadPos, Vector2.Zero,Color.Red);
                prt?.FollowNPCIndex = NPC.whoAmI;
                prt?.Rotation = NPC.AngleTo(Target.Center);

                Helper.PlayPitchedVariants(AssetDirectory.Sounds.Crystalline + "Sentinel_Alert", 1f, 0, 0, 3, HeadPos);
            }

            //如果玩家离开较远，则清除警戒
            if (Alerted == 1f && (!Target.Alives() || distanceToTarget > AlertRange + MeleeRange * 0.25))
            {
                Alerted = 0f;
            }

            //玩家进入近战范围，进入战斗状态
            if (Target.Alives() && distanceToTarget <= MeleeRange)
            {
                AggroCounter = AggroCounterMax;
            }


            if (NPC.life < NPC.lifeMax * 3 / 4 && !ReleasedRock)
            {
                SwitchStateP1(AIStates.P1Rock);
                ReleasedRock = true;
            }

            if (NPC.life < NPC.lifeMax / 2 && State != AIStates.Exchange)
                SwitchStateP1(AIStates.Exchange);
        }

        public void P1Idle()
        {
            //一动不动
            NPC.velocity.X = 0;

            if (Timer % 45 == 0)
            {
                if (!TryTurnToGuard())//尝试开盾，否则尝试攻击
                    TryTurnToAttack();
            }

            if (Timer-- < 0)//随便走走
                SwitchStateP1(AIStates.P1Walking, Main.rand.Next(60 * 2, 60 * 4), true);

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
            {
                if (!TryTurnToGuard())//尝试开盾，否则尝试攻击
                    TryTurnToAttack();
                // 开盾后清零计数器，避免重复触发
                if (State == AIStates.P1Guard)
                    GuardCounter = 0;
            }

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

                        if (Timer++ > 60 * 8 || !CanWalkForward())
                        {
                            if (CanHitTarget(out _) && distance < 16 * 16)//可以攻击，进入下一阶段
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

                        int whichFrameToDash = 5 * 6;


                        if (Timer == whichFrameToDash)//生成戳刺弹幕
                        {
                            //冲刺速度
                            float dashSpeed = 10;
                            NPC.velocity.X = dashSpeed * NPC.direction;
                            Helper.PlayPitched(CoraliteSoundID.Crystal_Item101, NPC.Center, pitch: 0.8f);
                            CanHit = true;
                            NPC.NewProjectileInAI<CrystallineSentinelSpurtProj>(NPC.Center + new Vector2(NPC.direction * 40, 0)
                                , Vector2.Zero, Helper.ScaleValueForDiffMode(200, 350, 480, 1000)
                                , 0, ai0: NPC.whoAmI, ai1: NPC.direction);
                        }
                        if(Timer > whichFrameToDash)
                        {

                            float speedMult = 0.9f;
                            NPC.velocity.X *= speedMult;


                            //检测悬崖
                            bool checkCliff = false;
                            for (int i = 0; i < 4; i++)
                            {
                                Point pos = (NPC.Bottom + new Vector2(16 * i * NPC.direction + NPC.velocity.X, 0)).ToTileCoordinates();
                                if (!Helper.GroundSearch(pos, new Point(0, 1), 2))
                                    checkCliff = true;
                            }
                            if (checkCliff)
                                NPC.velocity.X *= 0f;
                        }

                        if (Timer++ > 5 * 21)
                            SwitchStateP1(AIStates.P1Idle);
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
                        //准备阶段
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
                        if (Timer++ > 60 * 3 || !CanWalkForward())
                        {
                            if (CanHitTarget(out _))//可以攻击，进入下一阶段
                            {
                                NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
                                NPC.spriteDirection = NPC.direction;
                                NPC.velocity.X = 0;
                                SetFrame(3, 0);
                                Recorder = 1;
                                Timer = 0;
                                GuardCounter = 0; //开盾后清零计数器
                                PRTLoader.NewParticle<MagikeLozengeParticle>(NPC.Center, Vector2.Zero, Coralite.CrystallinePurple, 1f);
                                NPC.netUpdate = true;
                            }
                            else
                                SwitchStateP1(AIStates.P1Idle, 60);
                        }
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
                        int frameRate = 5;
                        // 防御期间，每180帧检查一次攻击条件，满足则发射飞弹
                        if (Timer >= 0)
                        {
                            if (CanHitTarget(out float distance) && distance > AlertRange && Main.GameUpdateCount % 210 == 0)
                            {
                                Timer = -12 * frameRate;
                                SetFrame(5, 3);
                            }
                            //如果玩家一直在远距离则延长护盾时间，可无限延长
                            if (Timer % 60 == 0 && Timer >= 0 && distance > AlertRange)
                                Timer = MathHelper.Clamp(Timer - 60, 1, 3 * 60);

                        }

                        if(Timer % 60 == 0)
                        {
                            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
                            NPC.spriteDirection = NPC.direction;

                            if (Vector2.Distance(Target.Center, NPC.Center) > MeleeRange && AggroCounter <= AggroCounterMin)
                                Timer = 60 * 3 + 1;
                        }

                        if (Timer > 0 && Timer % frameRate == 0)
                        {
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 15)
                                NPC.frame.Y = 15;
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            if (Timer < 0 && Timer == -frameRate * 8 + i * 10)
                            {
                                Helper.PlayPitched(CoraliteSoundID.Crystal_Item101, NPC.Center, pitch: 1f);
                                Vector2 pos = NPC.Center + new Vector2(-5 * NPC.direction, -27);

                                Vector2 vel = (-Vector2.UnitY * 9).RotatedBy(0f);
                                //Vector2 dir = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                                // 发射飞弹弹幕
                                int type = ModContent.NPCType<CrystallineSentinelMissile>();
                                var missile = NPC.NewNPCDirect(NPC.GetSource_FromAI(),
                                    pos,
                                    type
                                );
                                missile.velocity = vel;


                                Vector2[] poses = [new(2, -72), new(-2, -68), new(-10, -58)];
                                Vector2 dustpos = NPC.Center + new Vector2(poses[i].X * NPC.direction, poses[i].Y - 16);
                                Dust d = Dust.NewDustPerfect(dustpos, ModContent.DustType<VinicBigImpact>(), Vector2.Zero, Scale: 0.5f);
                                d.rotation = -MathHelper.PiOver2;
                            }
                        }
                        if (Timer >= 0)
                            SetFrame(3, 8);
                        else if (Timer % frameRate == 0)
                            NPC.frame.Y++;

                        if (Timer > 60 * 3)
                        {
                            Recorder++;
                            Timer = 0;
                            SetFrame(4, 0);
                            NPC.SuperArmor = false;
                            NPC.netUpdate = true;
                            ApplyGuardCD();
                        }

                        //当玩家靠近到能近战攻击到的范围差不多的时候会主动解除护盾然后立刻发动近战攻击
                        if (Timer++ > 60)
                        {
                            if (CanHitTarget(out float distance) && distance < MeleeRange)
                            {
                                NPC.SuperArmor = false;
                                NPC.netUpdate = true;
                                ApplyGuardCD();

                                SwitchStateP1(AIStates.P1Spurt, 60 * 9);
                            }
                        }

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
                        Timer++;
                        if (Timer > 5 * 5)
                            SwitchStateP1(AIStates.P1Idle, 60);
                    }
                    break;
            }
        }

        public void P1Missile()
        {
            //走几秒后发射飞弹
            switch (Recorder)
            {
                default:
                case 0:
                    {
                        if (Timer % 60 == 0)
                            NPC.spriteDirection = NPC.direction = (Target.Center.X > NPC.Center.X).ToDirectionInt();

                        if (MathF.Abs(NPC.velocity.X) < 1f)
                            NPC.velocity.X += NPC.direction * 0.1f;
                        if (MathF.Sign(NPC.velocity.X) != NPC.direction)
                            NPC.velocity.X = 0;
                        WalkFrame();
                        if(Timer > 60 * 2 || !CanWalkForward())
                        {
                            NPC.spriteDirection = NPC.direction = (Target.Center.X > NPC.Center.X).ToDirectionInt();
                            NPC.velocity.X = 0;
                            SetFrame(5, 0);
                            Recorder++;
                            Timer = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;
                case 1:
                    {

                        NPC.velocity.X = 0;

                        int frameRate = 5;

                        if (Timer % 60 == 0)
                        {
                            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
                            NPC.spriteDirection = NPC.direction;
                        }
                        if (Timer > 0 && Timer % frameRate == 0)
                        {
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 20)
                                NPC.frame.Y = 20;
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            if (Timer == frameRate * 7 + i * 10)
                            {
                                Vector2 pos = NPC.Center + new Vector2(-5 * NPC.direction, -27);
                                Vector2 vel = (-Vector2.UnitY * 9).RotatedBy(0.3f - i * 0.5f);

                                Helper.PlayPitched(CoraliteSoundID.Crystal_Item101, NPC.Center, pitch: 1f);
                                // 发射飞弹弹幕
                                int type = ModContent.NPCType<CrystallineSentinelMissile>();
                                var missile = NPC.NewNPCDirect(NPC.GetSource_FromAI(),
                                    pos,
                                    type
                                );
                                missile.velocity = vel;

                                Vector2[] poses = [new(2, -72), new(-2, -68), new(-10, -58)];
                                Vector2 dustpos = NPC.Center + new Vector2(poses[i].X * NPC.direction, poses[i].Y - 16);
                                Dust d = Dust.NewDustPerfect(dustpos, ModContent.DustType<VinicBigImpact>(), Vector2.Zero, Scale: 0.5f);
                                d.rotation = -MathHelper.PiOver2;
                                //Dust d = Dust.NewDustPerfect(, ModContent.DustType<CrystallineImpact>(), Vector2.Zero);
                                //d.rotation = -MathHelper.PiOver2 /*+ MathHelper.PiOver2*/;
                            }
                        }

                        if (Timer > 20 * frameRate)
                        {
                            NPC.spriteDirection = NPC.direction = (Target.Center.X > NPC.Center.X).ToDirectionInt();
                            NPC.velocity.X = 0;
                            SetFrame(0, 0);
                            Recorder++;
                            Timer = 0;
                            NPC.netUpdate = true;
                        }

                    }
                    break;
                case 2:
                    {
                        if (Timer++ > 60)
                        {
                            SwitchStateP1(AIStates.P1Idle, 60);
                            return;
                        }
                    }
                    break;
            }

        }

        public void P1Rock()
        {
            if(Timer == 0)
            {
                SetFrame(6, 0);
                NPC.velocity = Vector2.Zero;
            }
            int frameRate = 5;
            if (Timer > 0 && Timer % frameRate == 0)
            {
                NPC.frame.Y++;
                if (NPC.frame.Y > 10)
                    NPC.frame.Y = 10;
            }
            NPC.velocity.X = 0;

            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            Timer++;

            if (Timer > 10 * frameRate)
                SwitchStateP1(AIStates.P1Idle);
        }
        public bool CheckCanReleaseRock() => State == AIStates.P1Rock && Timer > 7 * 5;

        public void ApplyGuardCD() => GuardCooldown = GuardCooldownMax;

        #endregion
        #region 二阶段
        /// <summary>
        /// 二阶段常态AI，二阶段所有状态都会执行
        /// </summary>
        public void P2NormalAI()
        {
            UpdateP2HandPosNormally();

            NPC.noGravity = true;
        }

        public void P2Idle()
        {
            if (++NPC.frameCounter > 3)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 7)
                    NPC.frame.Y = 0;
            }

            //检查玩家
            if (Timer % 60 == 0 && Timer >= 0)
            {
                if (TryTurnToAttack())
                    return;
            }

            if (Timer % 60 == 0)
            {
                NPC.velocity = NPC.velocity.RotateByRandom(-0.9f, 0.9f);
            }

            //撞墙反飞
            CollideSpeed();
            SpeedUp(2, 0.05f);


            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;


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
            if (Timer == 0)
            {
                Recorder = Main.rand.NextFromList(-1, 1);
                int whichHand = Recorder > 0 ? 1 : 0;
                P2HandFrame[whichHand] = 0;
            }

            if (++NPC.frameCounter > 3)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 7)
                    NPC.frame.Y = 0;
            }

            Timer++;

            NPC.velocity *= 0.98f;
            CollideSpeed();

            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;


            const int readyTime = 3 * 6 + 1;
            const int idleTime = 90;
            const int delayTime = 3 * 7;

            if (Timer < readyTime)
            {
                if (Vector2.Distance(NPC.Center, Target.Center) > 300)
                {
                    NPC.velocity += (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.2f;
                    if (NPC.velocity.Length() > 12)
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * 12;
                }
                else
                    NPC.velocity *= 0.9f;

                if (Timer % 3 == 0)
                {
                    int whichHand = Recorder > 0 ? 1 : 0;
                    P2HandFrame[whichHand]++;
                }
            }
            else if (Timer == readyTime)
            {
                NPC.velocity *= 0.5f;

                NPC.NewProjectileDirectInAI<CrystallineSentinelSwing>((Recorder > 0 ? P2LeftHandPos : P2RightHandPos)
                    , (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero), Helper.GetProjDamage(120, 140, 180)
                    , 1, NPC.target, NPC.whoAmI);
            }
            else if (Timer < readyTime + idleTime)
            {
            }
            else if (Timer < readyTime + idleTime + delayTime)
            {
                NPC.velocity *= 0.9f;

                if (Timer % 3 == 0)
                {
                    int whichHand = Recorder > 0 ? 1 : 0;
                    if (P2HandFrame[whichHand] < 12)
                        P2HandFrame[whichHand]++;
                }
            }
            else
            {
                SwitchStateP2(AIStates.P2Idle, -120);
            }
        }

        public void P2Rolling()
        {
            //NPC.direction = NPC.spriteDirection = 1;
            bool shouldChangeDirection = true;
            //ai不够用，不用recorder了，直接靠timer来控制动画和行为
            if (Timer == 0)
            {
                P2HandSpurtFrameY = 0;
            }
            int idleEnd = 15;
            int readyEnd = idleEnd + 30;
            int dashEnd = (int)(readyEnd + Recorder);
            int dashFinishedEnd = dashEnd + 30;
            if (Timer <= idleEnd)
            {
                NPC.rotation = 0/*Target.Center.Y - NPC.Center.Y > 0 ? MathHelper.Pi : -MathHelper.Pi*/;
                P2HandSpurtFrameY = 0;

            }
            else if (Timer < readyEnd)//准备阶段，转向玩家并反向加速
            {
                int frameRate = 5;
                if (Timer % frameRate == 0)
                {
                    P2HandSpurtFrameY++;
                    if (P2HandSpurtFrameY > 5)
                        P2HandSpurtFrameY = 5;
                }
                NPC.rotation = Utils.Remap(Timer, idleEnd, readyEnd, 0,ConvertAtan2ToSpecialAngle(NPC.AngleTo(Target.Center)));
                NPC.velocity = Vector2.Lerp(NPC.velocity, -NPC.DirectionTo(Target.Center) * 6f, 0.03f);
            }
            else if (Timer == readyEnd)//准备完毕，开始冲刺
            {
                float dashSpeed = 18f;
                NPC.velocity = NPC.DirectionTo(Target.Center) * dashSpeed;

                Recorder = NPC.Distance(Target.Center) / dashSpeed + 20;

                var spurt = NPC.NewProjectileDirectInAI<CrystallineSentinelRollingSpurt>(NPC.Center, Vector2.Zero, 
                    Helper.GetProjDamage(120, 140, 180), 1, NPC.target, NPC.whoAmI);
                spurt.timeLeft = (int)(Recorder + 1);

                var trail = NPC.NewProjectileDirectInAI<CrystallineSentinelRollingTrail>(NPC.Center, Vector2.Zero,
                    0, 0, NPC.target, NPC.whoAmI);
                trail.timeLeft = (int)(Recorder + 1);
            }
            else if (Timer < dashEnd)//冲刺过程
            {

                int frameRate = 5;
                if(Timer % frameRate == 0)
                {
                    P2HandSpurtFrameY++;
                    if (P2HandSpurtFrameY > 12)
                        P2HandSpurtFrameY = 5;
                }
                CanHit = true;
                shouldChangeDirection = false;
                NPC.velocity = Utils.AngleLerp(NPC.velocity.ToRotation(), NPC.AngleTo(Target.Center), 0.01f).ToRotationVector2() * NPC.velocity.Length();
                NPC.rotation = ConvertAtan2ToSpecialAngle(NPC.velocity.ToRotation());

                for (int i = 0; i < 3; i++)
                {
                    Vector2 pos = new Vector2(NPC.spriteDirection * 96, 0).RotatedBy(NPC.rotation) + NPC.Center;
                    Vector2 vel = Vector2.Lerp(Main.rand.NextVector2Unit() * Main.rand.NextFloat(5), NPC.velocity, 0.25f) * 0.35f;
                    PRTLoader.NewParticle<CrystalFlashParticle>(pos, vel);
                }
            }
            else if (Timer < dashFinishedEnd)//冲刺后摇，逐渐减速并且逐渐恢复朝向
            {
                NPC.velocity *= 0.93f;
                NPC.rotation = Utils.Remap(Timer, dashEnd, dashFinishedEnd, ConvertAtan2ToSpecialAngle(NPC.velocity.ToRotation()), 0);

                shouldChangeDirection = false;
                int frameRate = 5;
                if (Timer % frameRate == 0 && P2HandSpurtFrameY > 0)
                {
                    P2HandSpurtFrameY++;
                    if (P2HandSpurtFrameY > 15)
                        P2HandSpurtFrameY = 0;
                }

            }
            else
            {
                CanHit = false;
                SwitchStateP2(AIStates.P2Idle, -60);
                return;
            }

            if (shouldChangeDirection)
                NPC.spriteDirection = NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;

            if (++NPC.frameCounter > 3)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 7)
                    NPC.frame.Y = 0;
            }
            Timer++;
        }

        public void P2WhirlSlash()
        {

            const float frameRate = 3;
            if (++NPC.frameCounter > frameRate)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 7)
                    NPC.frame.Y = 0;
            }
            Timer++;

            NPC.velocity *= 0.98f;
            CollideSpeed();


            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            int readyEnd = (int)(frameRate * 7 + 1);
            int chargeEnd = readyEnd + 80;
            int swingEnd = chargeEnd + 50;
            int idleEnd = swingEnd + 60;

            if (Timer < readyEnd)
            {
                if(Timer == 1)
                {
                    var ring = PRTLoader.NewParticle<CrystallineSentinelTelegraphRing>(NPC.Center, Vector2.Zero, Coralite.CrystallinePurple, 1f);
                    ring.FollowNPCIndex = NPC.whoAmI;
                    ring.Radius = 300;
                    ring.Lifetime = 75;
                }

                if (Vector2.Distance(NPC.Center, Target.Center) > 400)
                {
                    NPC.velocity += (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.15f;
                    if (NPC.velocity.Length() > 12)
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * 12;
                }
                else
                    NPC.velocity *= 0.9f;

                if(Timer % frameRate == 0 && P2HandFrame[0] > 6)
                {
                    P2HandFrame[0]--;
                    P2HandFrame[1]--;
                }
                if(Timer == readyEnd - 7)
                {

                    //for (int i = 0; i < 2; i++)
                    //{
                    //    float scale = Main.rand.NextFloat(0.7f, 1.3f) * 0.75f;
                    //    Vector2 vel = NPC.DirectionTo(Target.Center).RotatedBy(i * MathHelper.Pi) * Main.rand.NextFloat(8, 10) * 3;
                    //    Vector2 pos = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 4);
                    //    var slash = PRTLoader.NewParticle<CrystallineTrail>(pos, vel, Scale: scale * 0.5f);
                    //    slash.ScaleY = scale * 1.5f;
                    //}


                    float rot = NPC.AngleTo(Target.Center);
                    for (int i = 0; i < 2; i++)
                    {
                        //Vector2 pos = NPC.Center + new Vector2(10, 0).RotatedBy(i * MathHelper.Pi);
                        Vector2 pos = NPC.Center + new Vector2(10, 10).RotatedBy(MathHelper.Pi + i * MathHelper.PiOver2);
                        NPC.NewProjectileDirectInAI<CrystallineSentinelSwingSlash>(pos, Vector2.Zero,
                            Helper.GetProjDamage(120, 140, 180), 1, NPC.target, NPC.whoAmI, i, rot + i * MathHelper.Pi);
                    }
                }
            }
            else if (Timer == readyEnd)
            {
                NPC.velocity *= 0.5f;

            }
            else if (Timer < chargeEnd)
            {
            }
            else if (Timer < swingEnd)
            {
                NPC.velocity *= 0.9f;
            }
            else if (Timer < idleEnd)
            {

                NPC.velocity *= 0.9f;

                if (Timer % frameRate == 0 && P2HandFrame[0] < 12)
                {
                    P2HandFrame[0]++;
                    P2HandFrame[1]++;
                }
            }
            else
            {
                SwitchStateP2(AIStates.P2Idle, -120);
            }
        }

        #endregion

        public Vector2 GetP2FloatPos => NPC.Center + new Vector2(-NPC.spriteDirection * 4, 18);
        public Vector2 P2LeftHandPos => NPC.Center + new Vector2(-2, -24);
        public Vector2 P2RightHandPos => NPC.Center + new Vector2(-4, -23);

        /// <summary>
        /// 是否处于二阶段，依旧唐氏打表
        /// </summary>
        public bool IsPhase2 => State is AIStates.P2Idle or AIStates.P2Rolling or AIStates.P2Swing or AIStates.P2WhirlSlash;

        public Vector2 HeadPos => NPC.Center + new Vector2(0, -30);

        public void UpdateP2HandPosNormally()
        {
            float t = Utils.Remap(NPC.velocity.Length(), 0, 14f, 1 / 40f, 1 / 3f);
            P2FloatCenter = P2FloatMover.Update(t, GetP2FloatPos);
            P2HandCenter[0] = P2HandMover[0].Update(1 / 40f, P2LeftHandPos);
            P2HandCenter[1] = P2HandMover[1].Update(1 / 40f, P2RightHandPos);
        }

        public void Exchange()
        {
            NPC.velocity.X *= 0;
            if (Timer == 0)
            {
                SetFrame(0, 0);
            }
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
                    new SecondOrderDynamics_Vec2(0.9f, 0.8f, 0.5f, P2LeftHandPos),
                    new SecondOrderDynamics_Vec2(0.9f, 0.8f, 0.5f, P2RightHandPos),
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

            SetFrame(0, 0);
            State = targetState;

        }

        private void SwitchStateP2(AIStates targetState, int? overrideTime = null, bool randDirection = false)
        {
            State = targetState;
            Recorder = 0;
            Recorder2 = 0;
            CanHit = false;

            NPC.SuperArmor = false;

            P2HandFrame[0] = 12;
            P2HandFrame[1] = 12;
            P2HandSpurtFrameY = 0;
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

        private bool TryTurnToGuard()
        {
            if(GuardCounter > 240)
            {
                SwitchStateP1(AIStates.P1Guard);
                return true;
            }
            return false;
        }

        private bool TryTurnToAttack()
        {
            if (!IsPhase2)
            {
                //一阶段索敌
                if (Target.Distance(NPC.Center) > MeleeRange && AggroCounter <= AggroCounterMin)
                {
                    Helper.TargetCloestIgnoreIndex(NPC, false, NPC.target);
                    if (NPC.target != NPC.oldTarget)
                        AggroCounter = -1;
                }

                if (AggroCounter < 0)//敌不动，我不动
                    return false;
            }
            else
            {
                //二阶段索敌，目标似了才重新选目标
                if (!Target.Alives())
                    NPC.TargetClosest(false);
            }

            //找到玩家了就转向攻击阶段
            if (NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget(out float distance))//不攻击隐身玩家
            {
                StartAttack(distance);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 需要玩家不隐身（隐身是指有隐身药水BUFF和不在使用物品）<br></br>
        /// 需要距离小于1000，并且可以看到玩家
        /// </summary>
        /// <returns></returns>
        public bool CanHitTarget(out float distance)
        {
            distance = Vector2.Distance(NPC.Center, Target.Center);
            return !(Target.invis && Target.itemAnimation == 0) && distance < 1000 &&
                    Collision.CanHit(NPC.Center, 1, 1, Target.TopLeft, Target.width, Target.height);
        }
        public void StartAttack(float distance)
        {
            switch (State)
            {
                case AIStates.P1Idle:
                case AIStates.P1Walking:
                    // 近距离用近战，远程用飞弹
                    if (distance < 600)
                        SwitchStateP1(AIStates.P1Spurt);
                    else
                        SwitchStateP1(AIStates.P1Missile);
                    break;
                case AIStates.P2Idle:
                    //SwitchStateP2(AIStates.P2Swing);
                    //SwitchStateP2(AIStates.P2Rolling);
                    SwitchStateP2(AIStates.P2WhirlSlash);
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
            //NPC.rotation = NPC.AngleTo(Main.MouseWorld);
            switch (State)
            {
                case AIStates.P1Idle:
                case AIStates.P1Walking:
                case AIStates.P1Guard:
                case AIStates.P1Spurt:
                case AIStates.P1Missile:
                case AIStates.P1Rock://一阶段绘制
                    {
                        //每帧尺寸：174*112
                        Texture2D tex = NPC.GetTexture();

                        Rectangle frameBox = tex.Frame(7, Main.npcFrameCount[NPC.type], NPC.frame.X, NPC.frame.Y);

                        Vector2 drawPos = NPC.Center - screenPos + new Vector2(NPC.spriteDirection * 40, -16);
                        spriteBatch.Draw(tex, drawPos, frameBox, drawColor, NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 0);
                        spriteBatch.Draw(GlowTex.Value, drawPos, frameBox, Color.White, NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 0);

                        if (Recorder2 != 0)
                            DrawGuard(spriteBatch, screenPos);
                    }
                    break;
                case AIStates.Exchange:
                    {
                        Texture2D tex = ExchangeTex.Value;

                        Rectangle frameBox = tex.Frame(1, 19, NPC.frame.X, NPC.frame.Y);

                        Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, -22);
                        spriteBatch.Draw(tex, drawPos, frameBox, drawColor, NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 0);
                        spriteBatch.Draw(ExchangeTex_Glow.Value, drawPos, frameBox, Color.White, NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 0);
                    }
                    break;
                case AIStates.P2Idle:
                case AIStates.P2Swing:
                case AIStates.P2WhirlSlash:

                    bool faceLeft = NPC.spriteDirection < 0;
                    SpriteEffects effect2 = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    if (faceLeft)
                        DrawP2Hand(spriteBatch, screenPos, effect, drawColor, 0, 0);
                    else
                        DrawP2Hand(spriteBatch, screenPos, effect, drawColor, 0, 1);

                    DrawP2Float(spriteBatch, screenPos, effect, drawColor);
                    DrawP2Head(spriteBatch, screenPos, effect, drawColor);

                    if (faceLeft)
                        DrawP2Hand(spriteBatch, screenPos, effect, drawColor, 1, 1);
                    else
                        DrawP2Hand(spriteBatch, screenPos, effect, drawColor, 1, 0);

                    break;
                case AIStates.P2Rolling:

                    //if (NPC.spriteDirection < 0)
                    //    effect = SpriteEffects.FlipVertically;
                    DrawP2Float(spriteBatch, screenPos, effect, drawColor);
                    DrawP2Head(spriteBatch, screenPos, effect, drawColor);

                    {
                        Texture2D tex = P2SpurtTex.Value;

                        Rectangle frameBox = tex.Frame(1, 16, 0, P2HandSpurtFrameY);
                        SpriteEffects effect3 = NPC.spriteDirection <= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                        Vector2 origin = NPC.spriteDirection > 0 ? new(60,40) : new(151,35);
                        float rot = NPC.rotation;
                        if (Timer >= 60)
                            origin = NPC.spriteDirection > 0 ? new(60, 40) : new(151, 35);
                        float offsetY = Utils.Remap(Timer, 0f, 15f, -36, -12);
                        Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, offsetY);
                        spriteBatch.Draw(tex, drawPos, frameBox, drawColor, rot, origin, NPC.scale, effect3, 0);
                        spriteBatch.Draw(P2SpurtTex_Glow.Value, drawPos, frameBox, Color.White, rot, origin, NPC.scale, effect3, 0);

                        //spriteBatch.Draw(TextureAssets.Extra[98].Value, NPC.Center - Main.screenPosition, null, Color.Red, NPC.rotation + MathHelper.PiOver2, TextureAssets.Extra[98].Value.Size()/2,1f, 0, 0);
                    }

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
        public void DrawP2Hand(SpriteBatch spriteBatch, Vector2 screenPos, SpriteEffects effect, Color drawColor, int leftOrRight, int texLorR)
        {
            Texture2D tex = HandTex.Value;

            Rectangle frameBox = tex.Frame(2, 13, texLorR, P2HandFrame[leftOrRight]);

            spriteBatch.Draw(tex, P2HandCenter[leftOrRight] - screenPos, frameBox, drawColor
                , 0, frameBox.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(HandTex_Glow.Value, P2HandCenter[leftOrRight] - screenPos, frameBox, Color.White
                , 0, frameBox.Size() / 2, NPC.scale, effect, 0);
        }

        public void DrawP2Float(SpriteBatch spriteBatch, Vector2 screenPos, SpriteEffects effect, Color drawColor)
        {
            Texture2D tex = P2Float.Value;

            Rectangle frameBox = tex.Frame(1, 8, 0, NPC.frame.Y);
            float rot = Utils.Remap(MathF.Abs(NPC.velocity.X), 0, 14f, 0, MathHelper.PiOver4) * (NPC.velocity.X > 0).ToDirectionInt();

            spriteBatch.Draw(tex, P2FloatCenter - screenPos, frameBox, drawColor
                , rot, frameBox.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(P2Float_Glow.Value, P2FloatCenter - screenPos, frameBox, Color.White
                , rot, frameBox.Size() / 2, NPC.scale, effect, 0);
        }

        public void DrawP2Head(SpriteBatch spriteBatch, Vector2 screenPos, SpriteEffects effect, Color drawColor)
        {
            spriteBatch.Draw(P2Head.Value, NPC.Center - screenPos + new Vector2(0, -10), null, drawColor
                , NPC.rotation, P2Head.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(P2Head_Glow.Value, NPC.Center - screenPos + new Vector2(0, -10), null, Color.White
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

        public static float ConvertAtan2ToSpecialAngle(float theta)
        {
            // 1. 计算 |cos(theta)|
            float cosTheta = MathF.Cos(theta);
            float absCosTheta = MathF.Abs(cosTheta);

            // 2. 计算核心角度 [0, π/2]
            float angle = MathF.Acos(Math.Clamp(absCosTheta, -1f, 1f));

            // 3. 根据 sin(theta) 判断上下半圆
            float sinTheta = MathF.Sin(theta);
            float finalAngle = sinTheta > 0 ? -angle : angle;

            // 4. 右半圆反转符号
            if (theta.ToRotationVector2().X > 0)
            {
                finalAngle = -finalAngle;
            }

            return finalAngle;
        }
    }

    public class CrystallineSentinelFloatStone() : BaseFrameParticle(3,1,1)
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;
        public int FollowNPCIndex = -1;
        public SecondOrderDynamics_Vec2 FloatStoneMoves;
        public ref float Index => ref ai[0];
        public ref float State => ref ai[1];
        public ref float Timer => ref ai[2];
        public override void SetProperty()
        {
            Color = Color.White;
        }
        public override void AI()
        {
            if (State < 1 && FollowNPCIndex.GetNPCOwner(out NPC npc, Kill))
                FollowNPC(npc);

        }
        public void FollowNPC(NPC npc)
        {
            Vector2 pos = npc.Center + new Vector2(0, -10) + (4f + Index * MathHelper.TwoPi / 3).ToRotationVector2() * 36 * npc.scale;
            float factor = (int)Main.timeForVisualEffects * 0.02f + Index * MathHelper.TwoPi / 2;
            pos += new Vector2(MathF.Cos(factor) * 3, MathF.Sin(factor) * 6);
            Position = FloatStoneMoves.Update(1 / 60f, pos);

            if (npc.ModNPC is CrystallineSentinel sentinel && sentinel.CheckCanReleaseRock())//浮石发射
            {

                float speed = Main.rand.NextFloat(12, 18) * (0.4f + Index * 0.1f);
                Vector2 vel = -Vector2.UnitY.RotatedBy(-MathHelper.Pi / 3 + Index * MathHelper.PiOver4).RotateByRandom(0.1f, 0.5f) * speed;

                var floatStone = NPC.NewNPCDirect(this.FromObjectGetParent(), Position, ModContent.NPCType<CrystallineSentinelFloatStoneGrow>());
                floatStone.velocity = vel;

                var blast = PRTLoader.NewParticle<CrystallineRockBlast>(Position, Vector2.Zero);
                blast.Rotation = Velocity.ToRotation();

                Kill();
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            Rectangle frameBox = TexValue.Frame(3, 1, (int)Index, 0);
            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox, Color, Rotation,
                frameBox.Size() / 2, Scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public class CrystallineSentinelFloatStoneGrow : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;
        public ref float Timer => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 15;
            NPCID.Sets.ProjectileNPC[Type] = true;
            NPCID.Sets.NeverDropsResourcePickups[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.damage = 2;
            NPC.lifeMax = 20;
            NPC.friendly = false;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
        }
        public override bool PreHoverInteract(bool mouseIntersects)
        {
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.GravityMultiplier *= 2f;
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableKnockback();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<Stonebound>(), 2 * 60);
        }
        public override void AI()
        {
            float size = Utils.Remap(NPC.frame.Y, 0, 14, 8, 32);
            NPC.width = NPC.height = (int)size;

            NPC.velocity *= 0.99f;
            Timer++;

            if (State < 1)
            {
                if (Timer % 30 == 0)
                {
                    var rock = NPC.NewNPCDirect(this.FromObjectGetParent(), NPC.Center, ModContent.NPCType<CrystallineSentinelRock>());
                    rock.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 16) * 0.5f + NPC.velocity * 0.5f;

                    var blast = PRTLoader.NewParticle<CrystallineRockBlast>(NPC.Center, rock.velocity * 0.4f);
                    blast.Rotation = blast.Velocity.ToRotation();
                }

                if (Timer > 60)
                {
                    State = 2;
                    Timer = 0;
                }

                if (Main.rand.NextBool(3))
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20) * 0.5f;
                    Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 3f) * 0.5f + NPC.velocity * 0.5f;
                    Dust d = Dust.NewDustPerfect(pos, DustID.Stone, vel);
                    d.noGravity = true;
                }
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                if(Timer > 440)
                {
                    NPC.noGravity = false;

                    if (NPC.velocity.Y > 2f)
                        Timer--;

                    if (Timer > 560)
                        NPC.active = false;
                }
                if (MathF.Abs(NPC.velocity.Y) > 0.1f)
                    NPC.rotation += 0.015f * MathF.Sin(NPC.whoAmI * 1138);
            }

            float frameRate = 5;
            if (NPC.frameCounter++ % frameRate == 0 && NPC.frame.Y < 14)
            {
                NPC.frame.Y++;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = NPC.GetTexture();
            Rectangle frameBox = tex.Frame(1, 15, 0, NPC.frame.Y);


            float breatheAlpha = 1f + 0.2f * MathF.Sin((float)(Main.timeForVisualEffects * 0.1f));
            breatheAlpha = Utils.Remap(Timer, 0, 30, 1f, breatheAlpha);
            if (State < 1)
                breatheAlpha = 1;
            spriteBatch.Draw(tex, NPC.Center - screenPos, frameBox, Color.White * breatheAlpha, NPC.rotation,
                frameBox.Size() / 2 + new Vector2(0, 7), NPC.scale * 1.1f, SpriteEffects.None, 0);

            if (State > 1)
            {
                Texture2D rockTex = TextureAssets.Npc[ModContent.NPCType<CrystallineSentinelRock>()].Value;
                var rockFrame = rockTex.Frame(1, 9, 0, 0);

                float alphaFactor = Utils.Remap(Timer, 0, 60, 0f, 1f);
                spriteBatch.Draw(rockTex, NPC.Center - screenPos, rockFrame, Color.White * alphaFactor, NPC.rotation,
                    rockFrame.Size() / 2, NPC.scale * 1f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
    public class CrystallineAlertParticle : Particle
    {
        public override string Texture => AssetDirectory.Blank;
        public int FollowNPCIndex = -1;
        public ref float Alpha => ref ai[0];
        public override void SetProperty()
        {
            Lifetime = 60;
        }
        public override void AI()
        {
            if (FollowNPCIndex.GetNPCOwner(out NPC npc, Kill))
                Follow(npc);
            Alpha = Utils.Remap(LifetimeCompletion, 0f, 1f, 1f, 0f);

        }
        public virtual void Follow(NPC npc) => Position += (npc.position - npc.oldPosition);
        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TextureAssets.Extra[ExtrasID.MartianProbeScanWave].Value;

            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color * Alpha * 0.35f, Rotation,
                new Vector2(0, tex.Height / 2), Scale * 1.5f, SpriteEffects.None, 0);

            Texture2D halo = CoraliteAssets.LightBall.BallAlpha.Value;
            Color haloColor = Color.Lerp(Color.White, Color, 0.5f);
            spriteBatch.Draw(halo, Position - Main.screenPosition, null, haloColor * Alpha * 0.125f, 0,
                halo.Size() / 2, Scale, 0, 0);
            spriteBatch.Draw(halo, Position - Main.screenPosition, null, haloColor * Alpha * 0.125f, 0,
                halo.Size() / 2, Scale * 0.5f, 0, 0);
            return false;
        }
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
    public class CrystallineSentinelRollingSpurt : ModProjectile
    {
        public override string Texture => AssetDirectory.Particles + "LightShot";
        public ref float OwnerIndex => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 31;
        }
        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;
            Vector2 dir = (Vector2.UnitX * owner.spriteDirection).RotatedBy(owner.rotation);
            Vector2 pos = new Vector2(owner.spriteDirection * 84,0).RotatedBy(owner.rotation) + owner.Center;
            Projectile.Center = pos;
            Projectile.rotation = dir.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Projectile.GetTextureValue();

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Coralite.CrystallinePurple with { A = 0}, Projectile.rotation, 
                texture.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0);

            return false;
        }
    }

    public class CrystallineSentinelRollingTrail : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;
        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float FadeoutFactor => ref Projectile.ai[1];
        public ref float MaxtimeLeft => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            Helper.QuickTrailSets(Type, Helper.TrailingMode.RecordAll, 20);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;
            if (Projectile.localAI[0] == 0)
            {
                MaxtimeLeft = Projectile.timeLeft;
                Projectile.localAI[0] = 1;
            }
            Vector2 dir = (Vector2.UnitX * owner.spriteDirection).RotatedBy(owner.rotation);
            Vector2 pos = new Vector2(owner.spriteDirection * 114, 0).RotatedBy(owner.rotation) + owner.Center;
            Projectile.Center = pos;

            float fadein = Utils.Remap(Projectile.timeLeft, MaxtimeLeft - 20, MaxtimeLeft, 1f, 0f);
            float fadeout = Utils.Remap(Projectile.timeLeft, 0, 20, 0f, 1f);
            Projectile.Opacity = fadein * fadeout;
            FadeoutFactor = fadeout;
            Projectile.rotation = owner.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            for (int j = -1; j < 2; j += 2)
            {
                Vector2 lastTrailPos = Vector2.Zero;
                float innerRot = 0;
                float mult = 24;
                float maxRadius = 32f;
                float minRadius = 6f;
                int total = (int)(Projectile.oldPos.Length * mult - mult);
                Vector2 scale = new Vector2(0.4f, 0.2f) * 0.25f * Projectile.scale;
                for (int i = 0; i < total - 1; i++)
                {
                    var roundI = (int)(i / mult);
                    if (Projectile.oldPos[roundI] == Vector2.Zero || Projectile.oldPos[roundI + 1] == Vector2.Zero)
                        continue;

                    float factor = 1 - (float)i / total;
                    float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
                    float radius = Utils.Remap(factor, 0, 1, minRadius, maxRadius);
                    Vector2 oldpos = Vector2.Lerp(Projectile.oldPos[roundI], Projectile.oldPos[roundI + 1], lerpFactor);
                    float oldrot = MathHelper.Lerp(Projectile.oldRot[roundI], Projectile.oldRot[roundI + 1], lerpFactor);
                    float phase = (float)(-i * 0.025f - Projectile.timeLeft * 0.35f + Main.timeForVisualEffects * (0.04f + j * 0f));
                    float phaseoffset = phase + (j > 0 ? MathHelper.Pi : 0);
                    float fake3dAlpha = phaseoffset % MathHelper.TwoPi < MathHelper.Pi ? Utils.Remap(MathF.Abs(MathF.Cos(phaseoffset)), 0f, 1f, 0f, 1f) : 1f;
                    float y = MathF.Cos(phase) * j;

                    Vector2 dir = (oldrot + MathHelper.PiOver2).ToRotationVector2() * y;

                    //dir.X *= 0.8f;
                    float fadein = Utils.Remap(factor, 0.7f, 1f, 1f, 0f);
                    float fadeinFactor = MathHelper.Lerp(1f, fadein, FadeoutFactor);
                    Vector2 trailPos = oldpos + dir * radius * fadeinFactor;
                    var normalDir = lastTrailPos - trailPos;
                    innerRot -= 0.018f;
                    lastTrailPos = trailPos;


                    if (i == 0)
                        continue;

                    float alpha = factor * Projectile.Opacity * fake3dAlpha;
                    Color drawColor = j < 0 ? Coralite.CrystallinePurple : new Color(134, 156, 255);
                    Main.spriteBatch.Draw(star, trailPos - Main.screenPosition, null, drawColor with { A = 0 } * alpha, normalDir.ToRotation() + MathHelper.PiOver2, star.Size() / 2, scale, 0, 0);
                }
            }

            return false;
        }
    }

    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineSentinelSwing : BaseSwingProj
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        //public ref float LeftOrRight => ref Projectile.ai[0];
        public ref float OwnerIndex => ref Projectile.ai[0];

        [VaultLoaden("{@classPath}" + "CrystallineSentinelSwing_Glow")]
        public static ATex GlowTex { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradient")]
        public static ATex GradientTexture { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradientBlack")]
        public static ATex GradientTextureBlack { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradientThin")]
        public static ATex GradientTextureThin {  get; set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradientThin2")]
        public static ATex GradientTextureThin2 {  get; set; }

        public CrystallineSentinelSwing() : base(0.785f, trailCount: 62) { }

        public int delay;
        public int alpha;

        public float dir;
        public float offsetLength;
        public float maxLength;
        public Vector2 velocity;

        public override void SetSwingProperty()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 20;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 0;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 30 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
            {
                dir = Projectile.velocity.ToRotation();//(Projectile.Center - npc.Center).ToRotation();
                maxLength = Vector2.Distance(Main.player[npc.target].Center, npc.Center) + 140;

                if (maxLength > 480)
                    maxLength = 480;
            }

            Projectile.extraUpdates = 2;
            alpha = 0;
            startAngle = 0f;
            totalAngle = 40.5f;
            maxTime = 90 * 3;
            Smoother = Coralite.Instance.BezierEaseSmoother;
            delay = 20;
            distanceToOwner = -Projectile.height / 2;
            Projectile.localNPCHitCooldown = 60;
            Projectile.InitOldPosCache(62);

            base.InitializeSwing();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 1f);
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;

            if (timer % 30 == 0)
                onHitTimer = 0;

            alpha = (int)(Helper.SinEase(timer, maxTime) * 255);
            if (timer < maxTime / 2)
                offsetLength = Helper.SqrtEase(timer, maxTime / 2) * maxLength;
            else
                offsetLength = Helper.SinEase(timer, maxTime) * maxLength;
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
            {
                UpdateCaches();
                UpdateOldPosCaches();
            }
        }
        public void UpdateOldPosCaches()
        {
            for (int i = oldRotate.Length - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
            }
            Projectile.oldPos[0] = Top;
        }
        protected override Vector2 OwnerCenter()
        {
            if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
            {
                CrystallineSentinel cs = npc.ModNPC as CrystallineSentinel;
                Vector2 pos = (npc.ai[2] > 0 ? cs.P2LeftHandPos : cs.P2RightHandPos);

                return pos + dir.ToRotationVector2() * offsetLength;
            }

            return base.OwnerCenter();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor, extraRot);
            base.DrawSelf(GlowTex.Value, origin, Color.White * (alpha / 255f), extraRot);
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
                Vector2 Center = Projectile.oldPos[i];
                Vector2 Top = Center /*+ (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]))*/;
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]))
                    - (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));

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
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Vanilla.Value);
                    effect.Parameters["gradientTexture"].SetValue(GradientTextureThin.Value);
                    
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

    public class CrystallineSentinelSwingSlash : BaseSwingProj
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + "CrystallineSentinelSwing";
        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float Offset => ref Projectile.ai[1];
        public CrystallineSentinelSwingSlash() : base(1.047f, trailCount: 62) { }
        public int delay;

        public override void SetSwingProperty()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 20;
            trailBottomWidth = 40;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 0;
            useSlashTrail = true;
            useTurnOnStart = false;
        }
        protected override float ControlTrailBottomWidth(float factor)
        {
            return 30 * Projectile.scale;
        }
        protected override void InitializeSwing()
        {
            if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
            {

            }
            float angleFromOwner = (Projectile.Center - npc.Center).ToRotation();

            Projectile.extraUpdates = 2;
            int euMult = 1 + Projectile.extraUpdates;
            //Projectile.Opacity = 0;
            //startAngle = angleFromOwner;
            //totalAngle = MathHelper.TwoPi;
            minTime = (80 + 7) * euMult;
            maxTime = minTime + 35 * euMult;
            Smoother = Coralite.Instance.BezierEaseSmoother;/*new CustomSmoother(Helper.EaseOutQuad);*/
            delay = 20 * euMult;

            distanceToOwner = -Projectile.height / 2;
            Projectile.InitOldPosCache(62);

            base.InitializeSwing();
            totalAngle = MathHelper.TwoPi;
            _Rotation = startAngle = angleFromOwner;


        }
        protected override void AIBefore()
        {

            //RotateVec2 = _Rotation.ToRotationVector2();
            //Projectile.Center = OwnerCenter() + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + distanceToOwner));
            //Projectile.rotation = _Rotation;

        }
        protected override void BeforeSlash()
        {
            //_Rotation += 0.01f;
            float maxRange = 220;
            float distCharge = Utils.MultiLerp(Timer / (minTime * 2 / 3), -Projectile.height, Projectile.height * 0.75f, Projectile.height / 4);/*Utils.Remap(Timer, 0, minTime * 2 / 3, -Projectile.height / 2, Projectile.height / 4);*/
            float distFadein = Helper.HeavyEase(Utils.Remap(Timer, minTime * 2 / 3, minTime, 0f, 1f));
            float dist = maxRange * distFadein;
            distanceToOwner = distCharge + dist;
            if(Timer == (int)(minTime * 0.65f))
            {
                float scale = Main.rand.NextFloat(0.7f, 1.3f) * 0.75f;
                Vector2 vel = RotateVec2 * Main.rand.NextFloat(8, 10) * 1.3f;
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 4);
                var slash = PRTLoader.NewParticle<CrystallineSlash>(pos, vel, Scale: scale * 0.5f);
                slash.ScaleY = scale * 1.75f;
            }
            if (Timer > minTime * 0.7f)
                _Rotation += 0.01f;
            //if(Timer > minTime * 2 / 3 && Timer < minTime * 0.8f && Timer % 3 == 0)
            //{
            //    float scale = Main.rand.NextFloat(0.7f, 1.3f) * 0.75f;
            //    Vector2 vel = RotateVec2 * Main.rand.NextFloat(8, 10) * 1f;
            //    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 8);
            //    //var speedline = PRTLoader.NewParticle<SpeedLine>(pos, vel, Coralite.CrystallinePurple,scale);
            //    var slash = PRTLoader.NewParticle<CrystallineSlash>(pos, vel, Scale: scale * 0.5f);
            //    slash.ScaleY = scale;
            //}
            float rotFactor = Helper.EaseOutQuad(Utils.Remap(Timer, 0, minTime / 3, 1f, 0f));
            float indexFactor = Offset > 0 ? -1 : 1;
            float finalRot = /*0f;*/
            (Projectile.ai[2] - _Rotation) * Utils.Remap(Timer, 0, minTime * 0.55f, 0f, 1f);
            float extraRot = _Rotation
                + finalRot
                - (MathHelper.Pi + MathHelper.PiOver2 * 2) * Helper.EaseOutCubic(Utils.Remap(Timer, 0, minTime * 0.55f, 1f, 0f));
            ;
            RotateVec2 = extraRot.ToRotationVector2();
            Projectile.Center = OwnerCenter() + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + distanceToOwner));
            Projectile.rotation = extraRot - ((MathHelper.Pi) * rotFactor ) * indexFactor;

            if (Timer == minTime)
            {
                if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
                {
                    startAngle = (Projectile.Center - npc.Center).ToRotation();
                    //Projectile.ai[2] = distanceToOwner;
                }
            }
        }
        protected override void OnSlash()
        {

            //float maxRange = 240;
            //float distFadein = Utils.Remap(Timer, minTime, minTime + 40, 0, 1f);
            //float distFadeout = Utils.Remap(Timer, maxTime - 40, maxTime, 1f, 0f);
            //float dist = maxRange * distFadein * distFadeout;
            //distanceToOwner = -Projectile.height / 2 + dist;

            //float smooth = Utils.Remap(Timer, minTime, minTime + 10, 0, 1f) * Utils.Remap(Timer, maxTime - 10, maxTime, 1f, 0f);
            //distanceToOwner = Projectile.ai[2] + MathF.Cos(Timer * 0.15f) * 20 * smooth;
            float fadein = Utils.Remap(Timer, 0, minTime, 0f, 1f);
            float fadeout = Utils.Remap(Timer, maxTime, maxTime + delay, 1f, 0f);
            Projectile.Opacity = fadein * fadeout;
            float factor = Utils.Remap(Timer, minTime, maxTime, 0, 1f);
            float extraRot = Utils.Remap(factor, 0, 1f, 0.2f, 0.01f);
            _Rotation += factor;

            base.OnSlash();
        }
        protected override void AfterSlash()
        {
            _Rotation += 0.01f;
            float maxRange = 220;
            float distFadeout = Helper.BezierEase(Utils.Remap(Timer, maxTime, maxTime + delay, 1, 0f));
            float dist = distFadeout * maxRange;
            distanceToOwner = Projectile.height / 4 + dist;
            Projectile.Opacity = distFadeout;

            RotateVec2 = _Rotation.ToRotationVector2();
            Projectile.Center = OwnerCenter() + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + distanceToOwner));
            Projectile.rotation = _Rotation;

            if (Timer > maxTime + delay)
                Projectile.Kill();
        }
        protected override void AIAfter()
        {

            //RotateVec2 = _Rotation.ToRotationVector2();
            //Projectile.Center = OwnerCenter() + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + distanceToOwner));
            //Projectile.rotation = _Rotation;
            RotateVec2 = Projectile.rotation.ToRotationVector2();
            Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
            Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * (Projectile.height / 2 + trailBottomWidth)));//弹幕的底端和顶端计算，用于检测碰撞以及绘制


            if (Offset == 0)
                Main.NewText($"{Timer}/{minTime}/{maxTime} {_Rotation} {distanceToOwner} {Timer / (1 + Projectile.extraUpdates)}");

            if (useShadowTrail || useSlashTrail)
            {
                UpdateCaches();
                UpdateOldPosCaches();
            }

            if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 pos = Vector2.Lerp(npc.Center, Projectile.Center, Main.rand.NextFloat());
                    Vector2 vel = npc.DirectionTo(pos).RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(1, 4f);
                    //Dust d = Dust.NewDustPerfect(pos, DustID.UnusedWhiteBluePurple
                    //    , vel * Main.rand.NextFloat(0.7f, 1.3f), newColor: Coralite.CrystallinePurple
                    //    , Scale: Main.rand.NextFloat(1f, 2f) * 0.5f);
                    //d.noGravity = true;
                    //Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<CrystallineDustSmall>()
                    //    , vel, Scale:Main.rand.NextFloat(0.5f,1f));
                    //d.noGravity = true;

                    //float factor = Utils.Remap(Timer, minTime, maxTime, 0f, 1f);

                    //if (factor > 0.3f && factor < 0.7f && Main.rand.NextBool(5))
                    //{
                    //    Vector2 v = npc.DirectionTo(pos).RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(8, 12f);
                    //    var line = PRTLoader.NewParticle<SpeedLine>(Top, v, Coralite.CrystallinePurple, Main.rand.NextFloat(0.3f,0.5f));
                        
                    //}
                }


            }
        }
        public void UpdateOldPosCaches()
        {
            for (int i = oldRotate.Length - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
            }
            Projectile.oldPos[0] = Top;
        }
        public static float EaseInOutQuint(float time, float duration)
        {
            if ((time /= duration * 0.5f) < 1f)
            {
                return 0.5f * time * time * time * time * time;
            }
            return 0.5f * ((time -= 2f) * time * time * time * time + 2f);
        }
        protected override Vector2 OwnerCenter()
        {
            if(OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
            {
                return npc.Center;
            }
            return base.OwnerCenter();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            bool ret = base.PreDraw(ref lightColor);

            if (useSlashTrail && VisualEffectSystem.DrawKniefLight && Timer <= minTime)
                DrawSlashTrail();

            return ret;
        }
        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor * Projectile.Opacity, extraRot);
            base.DrawSelf(CrystallineSentinelSwing.GlowTex.Value, origin, Color.White * Projectile.Opacity, extraRot);
        }

        protected override void DrawSlashTrail()
        {
            if (oldRotate == null)
                return;

            List<VertexPositionColorTexture> bars = new();
            GetCurrentTrailCount(out float count);

            float alpha = Projectile.Opacity * 255f;
            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
                Vector2 Center = Projectile.oldPos[i];
                Vector2 Top = Center /*+ (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]))*/;
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]))
                    - (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));
                
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
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Vanilla.Value);
                    effect.Parameters["gradientTexture"].SetValue(CrystallineSentinelSwing.GradientTextureThin.Value);

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
    public class CrystallineSentinelTelegraphRing : Particle
    {
        public override string Texture => AssetDirectory.Blank;
        public int FollowNPCIndex = -1;
        public ref float Alpha => ref ai[0];
        public ref float Radius => ref ai[1];
        public override void SetProperty()
        {
            Lifetime = 60;
        }
        public override void AI()
        {
            if(FollowNPCIndex.GetNPCOwner(out NPC npc, Kill))
                Follow(npc);
            Alpha = Helper.SinEase(LifetimeCompletion);
            Rotation += 0.01f;
            //Main.NewText($"{Main.GameUpdateCount} {Alpha}");

            float radius = 240 - 85 / 2 - 40;
            {
                Vector2 pos = Position + Main.rand.NextVector2Unit() * radius;
                Dust d = Dust.NewDustPerfect(pos, DustID.RainbowTorch, Vector2.Zero, newColor:Coralite.CrystallinePurple);
                d.noGravity = true;
            }
        }
        public virtual void Follow(NPC npc) => Position += (npc.position - npc.oldPosition);

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            int pointCount = (int)(MathHelper.TwoPi * Radius / 12) / 8;
            var star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 origin = new(36, 36);
            Vector2 scale = new Vector2(0.25f, 1f) * 0.8f * Scale;
            float radiusEaser = Helper.BezierEase(Utils.Remap(LifetimeCompletion, 0, 0.4f, 0.2f, 1f));
            float radius = Radius * radiusEaser;
            for (int i = 0; i < pointCount; i++)
            {
                float dir = MathHelper.TwoPi / pointCount * i + Rotation;
                Vector2 pos = Position + dir.ToRotationVector2() * radius;
                Color color = Color * Alpha * 1f;
                spriteBatch.Draw(star, pos - Main.screenPosition, null, color, dir, origin, scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }

    public class CrystallineSlash : Particle
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;
        public ref float Alpha => ref ai[0];
        public ref float ScaleY => ref ai[1];
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Frame.X = Main.rand.Next(0, 2);
            Rotation = Velocity.ToRotation() + 1.57f;
            Color = Color.White;
            Alpha = 1;
        }
        public override void AI()
        {
            Opacity++;
            Lighting.AddLight(Position, Color.ToVector3() * 0.3f);

            if (Opacity > 4)
            {
                Velocity *= 0.98f;
                ScaleY += 0.1f;
                Scale -= 0.02f;
                Alpha = Utils.MultiLerp((Opacity - 4) / 20, 1f, 0.9f, 0.8f, 0.6f, 0.3f, 0f);
            }

            if (Opacity > 36 || Alpha < 0.01f)
                active = false;

        }
        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TexValue;
            Rectangle frameBox = mainTex.Frame(3, 1, Frame.X, 0);
            Vector2 origin = frameBox.Size() / 2;
            Vector2 scale = new(Scale, ScaleY);
            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frameBox, Color.White * Alpha, Rotation, origin, scale, SpriteEffects.None, 0);
            return false;
        }
    }

    public class CrystallineTrail : TrailParticle
    {
        public ref float Alpha => ref ai[0];
        public ref float ScaleY => ref ai[1];
        public override void SetProperty()
        {

            Color = Color.White;
            Alpha = 1;
            Rotation = Velocity.ToRotation() + 1.57f;
            InitializeCaches(48);
            trail ??= new Trail(Main.graphics.GraphicsDevice, 48, new EmptyMeshGenerator(), WidthFunction, ColorFunction);
        }
        public float WidthFunction(float factor)
        {
            return 28 * 14 * Scale * (1 - factor);
        }
        public Color ColorFunction(Vector2 coords) => Color.White;
        public override void AI()
        {
            Opacity++;


            if (Opacity > 4)
            {
                Velocity *= 0.99f;
                ScaleY -= 0.1f;
                Scale -= 0.02f;
                Alpha = Utils.MultiLerp((Opacity - 4) / 20, 1f, 0.9f, 0.8f, 0.6f, 0.3f, 0f);
            }

            if (Opacity > 36 || Alpha < 0.01f)
                active = false;

            UpdatePositionCache(48);
            if (!Main.dedServ)
                trail.TrailPositions = oldPositions;

        }
        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            return false;
        }

        public override void DrawPrimitive()
        {
            if (trail == null)
                return;

            Effect effect = ShaderLoader.GetShader("AlphaGradientTrail");

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatBlurVMirror.Value);
            effect.Parameters["gradientTexture"].SetValue(CrystallineSentinelSwing.GradientTextureBlack.Value);
            effect.Parameters["alpha"].SetValue(Alpha);

            trail?.DrawTrail(effect);
        }
    }

    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineSentinelRock : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        [VaultLoaden("{@classPath}" + "CrystallineSentinelRock_Glow")]
        public static ATex GlowTex { get; set; }
        public ref float Timer => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 9;
            NPCID.Sets.ProjectileNPC[Type] = true;
            NPCID.Sets.NeverDropsResourcePickups[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.damage = 2;
            NPC.lifeMax = 20;
            NPC.friendly = false;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
        }

        public override bool PreHoverInteract(bool mouseIntersects)
        {
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            NPC.GravityMultiplier *= 2f;
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableKnockback();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<Stonebound>(), 2 * 60);
        }
        public override void AI()
        {
            float size = Utils.Remap(NPC.frame.Y, 0, 8, 32, 8);
            NPC.width = NPC.height = (int)size;


            float randFactor = MathF.Sin(NPC.whoAmI * 1241);
            Timer++;
            if (Timer == 60 + (int)(20 * randFactor))
            {
                Split();
            }
            if (Timer == 120 + (int)(20 * randFactor) && NPC.frame.Y < 6)
            {
                //Split();
            }

            if (Timer > 480 + 30 * randFactor)
            {
                NPC.noGravity = false;

                if (NPC.velocity.Y > 2f)
                    Timer--;

                if (Timer > 560)
                    NPC.active = false;
            }
            else
            {

                if(NPC.frame.Y < 7)//漂浮
                {
                    Vector2 vel = -Vector2.UnitY * (0.4f + 0.15f * MathF.Sin(NPC.whoAmI * 498));
                    NPC.velocity = Vector2.Lerp(NPC.velocity, vel, 0.05f);
                }
                else//减速
                {
                    float velDec = Utils.Remap(Timer, 0, 120, 0.98f, 0.94f);
                    NPC.velocity *= velDec;

                }
            }
            if (MathF.Abs(NPC.velocity.Y) > 0.1f)
                NPC.rotation += 0.015f * MathF.Sin(NPC.whoAmI * 1138);
        }
        public override void OnKill()
        {
            Split();
        }
        public void Split(int min = 1, int max = 2)
        {
            if (NPC.frame.Y > 6)
                return;
            int count = Main.rand.Next(min, max + 1);
            bool toPlr = Main.rand.NextBool();//是否尝试朝附近玩家分裂
            Player plr = null;
            if (toPlr)
            {
                plr = NPC.FindPlayer();
            }
            for (int i = 0; i < count; i++)
            {
                if (Main.rand.NextBool(6))
                    continue;
                int frameIncrease = Main.rand.Next(1, 4);
                int frame = (int)MathHelper.Clamp(NPC.frame.Y + frameIncrease, 0, 8);

                float velFactor = Utils.Remap(NPC.velocity.Length(), 0, 12f, 0f, 0.8f);
                var rock = NPC.NewNPCDirect(this.FromObjectGetParent(), NPC.Center, ModContent.NPCType<CrystallineSentinelRock>());
                Vector2 randVel = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4, 8f) * 0.45f;
                if (toPlr && plr != null)
                    randVel.X = (NPC.DirectionTo(plr.Center) * Main.rand.NextFloat(6, 10) * 0.75f).X;
                rock.velocity = Vector2.Lerp(randVel, NPC.velocity * 0.5f, velFactor);
                rock.frame.Y = frame;

                var blast = PRTLoader.NewParticle<CrystallineRockBlast>(NPC.Center + rock.velocity.SafeNormalize(Main.rand.NextVector2Unit()) * 20, Vector2.Zero);
                blast.Rotation = rock.velocity.ToRotation();
                //blast.Scale *= 1f;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = NPC.GetTexture();
            var frameBox = tex.Frame(1, 9, 0, NPC.frame.Y);

            float alphaFactor = Utils.Remap(Timer, 0, 60, 0f, 1f);
            float breatheAlpha = 1f + 0.2f * MathF.Sin((float)(Main.timeForVisualEffects * 0.1f));
            breatheAlpha = Utils.Remap(Timer, 40, 70, 1f, breatheAlpha);

            spriteBatch.Draw(GlowTex.Value, NPC.Center - screenPos, frameBox, Color.White * breatheAlpha, NPC.rotation, frameBox.Size() / 2, NPC.scale * 1.1f, SpriteEffects.None, 0);
            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, frameBox, Color.White * alphaFactor, NPC.rotation, frameBox.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class CrystallineRockBlast() : BaseFrameParticle(3, 7, 4)
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;
        public ref float Timer => ref ai[0];
        public override void SetProperty()
        {
            Scale = Main.rand.NextFloat(0.7f, 1.3f) * 0.7f;
            Color = Color.White;
            base.SetProperty();
        }
        public override void AI()
        {
            Velocity *= 0.96f;
            Timer++;
            if (Timer > 4 * 7)
                Kill();
            base.AI();
        }
         public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            Rectangle frameBox = TexValue.Frame(3, 7, Frame.X, Frame.Y);
            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox, Color, Rotation + MathHelper.PiOver2,
                frameBox.Size() / 2, Scale, SpriteEffects.None, 0);
            return false;
        }
    }

    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineSentinelMissile : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradientBlack")]
        public static ATex GradientTextureBlack { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelShieldParticle")]
        public static ATex ShieldParticle { get; set;  }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelShieldParticle_Glow")]
        public static ATex ShieldParticleGlow { get; set; }
        public ref float Timer => ref NPC.ai[0];
        public ref float DeathTimer => ref NPC.ai[1];
        public ref float CounterFactor => ref NPC.ai[2];
        private Trail trail;
        private PRTGroup shardGroup;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPC.QuickTrailSets(Helper.NPCTrailingMode.RecordAll, 24);
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 40;
            NPC.damage = 70;
            NPC.lifeMax = 7000;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.SuperArmor = true;
            NPC.aiStyle = -1;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastHurt;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.frame.Y = 2;
            CounterFactor = 4;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        public override bool PreHoverInteract(bool mouseIntersects)
        {
            return false;
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 2.5f;
            DeathTimer = 0;
            modifiers.HideCombatText();
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.velocity.Length() < 0.6f)
                NPC.velocity = NPC.velocity.ToRotation().ToRotationVector2() * 0.6f;
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {

            if (NPC.velocity.Length() < 0.6f)
                NPC.velocity = NPC.velocity.ToRotation().ToRotationVector2() * 0.6f;
        }
        public override bool CanHitNPC(NPC target) => target.type == ModContent.NPCType<CrystallineSentinel>();
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            NPC.Kill();

            //命中开盾期间的战斗体时破盾，话说居然没有target的onhitbynpc接口
            if (target.ModNPC is not CrystallineSentinel sentinel)
                return;
            sentinel.OnHitByMissile();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            NPC.Kill();
        }
        public override bool? CanFallThroughPlatforms() => true;
        public override void AI()
        {
            if (!VaultUtils.isServer)
                shardGroup ??= [];
            //trail ??= new Trail(Main.graphics.GraphicsDevice, 20, new EmptyMeshGenerator(), WidthFunction, ColorFunction);

            
            float velDecr = Utils.Remap(Timer,0,640,0.017f,0.12f);
            //if (Timer < 30f)
            //    NPC.velocity *= 0.98f;
            if (NPC.velocity.Length() > 0.5f)
                NPC.velocity -= NPC.velocity.SafeNormalize(Vector2.One) * velDecr;
            else if(Timer > 120f)
            {
                DeathTimer++;
            }

            if (DeathTimer > 120)
                NPC.Kill();

            NPC.TargetClosest(false);

            var target = Main.player[NPC.target];
            if (CounterFactor > 0)
            {
                if (target.Alives() && target.Distance(NPC.Center) < 1200f)
                {
                    float chaseFactor = Utils.Remap(Timer, 15,120, 0.005f, 0.095f) * Utils.Remap(CounterFactor, 0, 10, 0f, 1f);

                    float timeFactor = Utils.Remap(Timer, 0f, 480f, 1f, 0.05f);
                    float speedModifier = Utils.Remap(Timer, 15, 30, 0.01f, 0.15f) * Utils.Remap(CounterFactor, 0, 10, 0.5f, 1f) * timeFactor;
                    Movement(target.Center, speedModifier);
                    //NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target.Center) * 9f, chaseFactor);
                    //NPC.velocity = NPC.velocity.Length() * Utils.AngleLerp(NPC.velocity.ToRotation(), NPC.AngleTo(target.Center), chaseFactor).ToRotationVector2();

                    var normal = NPC.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);

                    float velFactor = Utils.Remap(NPC.velocity.Length(), 0, 10, 0.03f, 0.5f);
                    Vector2 disturb = normal * MathF.Sin(Timer * 0.02f + NPC.whoAmI) * 0.3f * velFactor;
                    if (NPC.velocity.Length() < 2f)
                        disturb /= 2f;
                    NPC.velocity += disturb;
                }
            }
            //Main.NewText($"{CounterFactor}");
            if (CounterFactor < 0)
                CounterFactor -= 0.01f;
            else
                CounterFactor += 0.01f;
            CounterFactor = MathHelper.Clamp(CounterFactor, 0, 10);
            if (NPC.velocity.Length() > 0.1f)
                NPC.rotation = NPC.velocity.ToRotation();

            if (!VaultUtils.isServer)
            {
                //if(NPC.oldPosition != Vector2.Zero)
                //{
                //    foreach (var p in shardGroup)
                //        p.Position += NPC.position - NPC.oldPosition;
                //}
                shardGroup?.Update();
            }

            float scale = Utils.Remap(NPC.velocity.Length(), 0, 10, 2, 1f);
            {
                NPC.position = NPC.Center;
                //NPC.scale = scale;
                NPC.height = NPC.width = (int)(40 * scale);
                NPC.Center = NPC.position;
            }
            if (Timer > 0 && Timer % 120 == 0 && NPC.frame.Y > 0)
                NPC.frame.Y--;

            if(Timer % 2 == 0)
            {
                float range = Utils.Remap(NPC.velocity.Length(), 0, 10, 33, 11);
                int prtCount = 1;
                if (range > 18)
                    prtCount++;
                for (int i = 0; i < prtCount; i++)
                {
                    Vector2 position = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0,range);
                    Vector2 vel = NPC.DirectionTo(position).RotatedBy(-MathHelper.PiOver2) * Main.rand.NextFloat(1, 4);
                    var prt = PRTLoader.NewParticle<CrystalFlashParticle>(position - vel * 5, vel * 0.75f + NPC.velocity * 0.25f);
                    prt.Scale /= 2f;
                }
            }

            Timer++;
        }

        public void Movement(Vector2 targetPos, float speedModifier, float cap = 10f)
        {
            if(Math.Abs(NPC.Center.X - targetPos.X) > 5f)
            {
                if (NPC.Center.X < targetPos.X)
                {
                    NPC.velocity.X += speedModifier;
                    if (NPC.velocity.X < 0)
                        NPC.velocity.X += speedModifier * 2;
                }
                else
                {
                    NPC.velocity.X -= speedModifier;
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X -= speedModifier * 2;
                }
            }
            if (NPC.Center.Y < targetPos.Y)
            {
                NPC.velocity.Y += speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2;
            }
            else
            {
                NPC.velocity.Y -= speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(NPC.velocity.X) > cap)
                NPC.velocity.X = cap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > cap)
                NPC.velocity.Y = cap * Math.Sign(NPC.velocity.Y);
        }
        public override void OnKill()
        {
            int prtCount = 18;
            for (int i = 0; i < prtCount; i++)
            {
                Vector2 pos = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 12);
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 3.5f);
                var prt = PRTLoader.NewParticle<CrystallineFragmentParticle>(pos, vel);
                prt.Scale = Main.rand.NextFloat(0.4f, 1f);
            }


            for (int i = 0; i < 6 * 6; i++)
            {
                Vector2 position = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(16);
                Vector2 vel = NPC.rotation.ToRotationVector2().RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1, 6);
                PRTLoader.NewParticle<CrystalFlashParticle>(position, vel * 0.75f + NPC.velocity * 0.25f);
            }

            float range = 80;
            foreach(var p in Main.ActivePlayers)
            {
                if (p.Distance(NPC.Center) > range)
                    continue;

                p.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage, (p.Center.X > NPC.Center.X).ToDirectionInt());
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            CoraliteSystem.InitBars();

            int length = NPC.oldPos.Length - 1;
            float timer = /*(float)(Main.timeForVisualEffects * 0.05f)*/0;
            for (int i = 0; i < length; i++)
            {
                if (NPC.oldPos[i + 1] == Vector2.Zero || NPC.oldPos[i] == Vector2.Zero)
                    continue;
                float factor = 1 - i / (float)length;

                var normal = (NPC.oldPos[i+1] - NPC.oldPos[i]).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);
                float width = WidthFunction(factor);
                Vector2 top = NPC.oldPos[i] + NPC.Size / 2 /*- Main.screenPosition*/ + normal * width;
                Vector2 bottom = NPC.oldPos[i] + NPC.Size / 2 /*- Main.screenPosition*/ - normal * width;
                Color color = ColorFunction(factor);

                CoraliteSystem.Vertexes.Add(new(top, color, new Vector3(factor + timer, 0, 0)));
                CoraliteSystem.Vertexes.Add(new(bottom, color, new Vector3(factor + timer, 1, 0)));

            }

            Effect effect = ShaderLoader.GetShader("TurbulenceArrow");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());

            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            effect.Parameters["udissolveS"].SetValue(0.5f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Laser.Body.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.AirFlow2.Value);
            effect.Parameters["uGradient"].SetValue(GradientTextureBlack.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.VanillaFlowA.Value);
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                var arr = CoraliteSystem.Vertexes.ToArray();
                Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;

                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, arr, 0, CoraliteSystem.Vertexes.Count - 2);

            }
            shardGroup?.Draw(spriteBatch);

            Texture2D mainTex = NPC.GetTexture();
            Vector2 pos = NPC.Center - screenPos;
            var frameBox = mainTex.Frame(1, 3, NPC.frame.X, NPC.frame.Y);
            Vector2 origin = frameBox.Size() / 2;


            //绘制本体
            spriteBatch.Draw(mainTex, pos, frameBox, Color.Lerp(drawColor, Color.White, 0.5f), NPC.rotation + MathHelper.PiOver2, origin, NPC.scale * 1.5f, 0, 0);

            DrawShard();

            //Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, $"{CounterFactor}", NPC.position.X - Main.screenPosition.X, NPC.position.Y - Main.screenPosition.Y, Color.White, Color.Black, Vector2.Zero);
            return false;
        }

        public void DrawShard()
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;

            var baseTime = 30;
            var step = 15;
            float maxShardCount = 14f;
            int drawcount = (int)Utils.Remap(Timer, baseTime, baseTime + step * 6, 1, maxShardCount + 1);
            for (int i = 0; i < drawcount; i++)
            {
                float fadeinFactor = Utils.Remap(Timer, baseTime + i * step, baseTime + i * step * 2, 0f, 1f);
                float factor = (i + 1f) / (maxShardCount + 1);
                float rotSpeed = Utils.Remap(factor,0f,1f,0.05f,0.02f);
                float maxRadius = Utils.Remap(NPC.velocity.Length(), 0f, 10f, 80f, 20f);
                float radius = Utils.MultiLerp(factor, [0, 0.25f,0.5f, 1f]) * maxRadius;
                float visualSpeed = /*Utils.Remap(factor, 0f, 1f, 0.05f, 0.01f);*/0.01f;
                float dir = (float)(-Timer * rotSpeed + i * 1214f - Main.timeForVisualEffects * visualSpeed);
                Vector2 targetPos = NPC.Center + dir.ToRotationVector2() * radius - Main.screenPosition;

                int trailLength = (int)(MathHelper.TwoPi * radius / 16f);
                Vector2 scale = new(0.2f, 0.3f);
                float iTimeFactor = MathF.Sin((float)(Main.timeForVisualEffects * 0.04f + i * 91208)) * 0.5f + 0.5f;
                for (int j = 0; j < trailLength; j++)
                {
                    float trailFactor = j / (float)trailLength;
                    float trailDir = dir + MathHelper.Pi * 2f / 3f * trailFactor;
                    Vector2 trailPos = NPC.Center + trailDir.ToRotationVector2() * radius - Main.screenPosition;
                    float alpha = Utils.Remap(trailFactor, 0, 1f, 1, 0f) * 0.5f * fadeinFactor * iTimeFactor;
                    Main.spriteBatch.Draw(star, trailPos, null, Color.Violet with { A = 0 } * alpha, trailDir, star.Size() / 2, scale, 0, 0);
                }
                
                var frameBox = ShieldParticle.Frame(1, 13, 0, (NPC.whoAmI * 12901 + i * 109) % 13);

                float shieldScale = 0.75f;
                Main.spriteBatch.Draw(ShieldParticle.Value, targetPos, frameBox, Color.White * fadeinFactor * 0.5f * iTimeFactor, dir + NPC.whoAmI * 634f, frameBox.Size() / 2, shieldScale, 0,0);
                Main.spriteBatch.Draw(ShieldParticleGlow.Value, targetPos, frameBox, Color.White * fadeinFactor * iTimeFactor, dir + NPC.whoAmI * 634f, frameBox.Size() / 2, shieldScale, 0, 0);
            }
        }

        public static float WidthFunction(float factor) => Utils.Remap(factor, 0, 1, 2, 18);

        public static Color ColorFunction(float factor) => Color.Lerp(Color.Black, Color.White, factor) * Utils.Remap(factor, 0, 1, 0, 1);
    }
    public class CrystallineFragmentParticle : Particle
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;
        public readonly int MaxFrame = 7;
        public ref float Alpha => ref ai[0];
        public ref float Offset => ref ai[1];
        public ref float FrameY => ref ai[2];
        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Lifetime = 42 + Main.rand.Next(-18, 12);
            Color = Color.White;
            Scale = Main.rand.NextFloat(0.5f, 1.5f);
            Offset = Main.rand.Next(100);
            FrameY = Main.rand.Next(MaxFrame);
        }
        public override void AI()
        {

            Alpha = Utils.Remap(LifetimeCompletion, 0f, 1f, 1f, 0f);
        }
        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            var frameBox = tex.Frame(1, MaxFrame, 0, Frame.Y);

            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox, Color * Alpha, Rotation, frameBox.Size() / 2, Scale, 0, 0);
            return false;
        }
    }
    public class CrystallineSentinelShieldParticle : Particle
    {
        private readonly int maxFrameY = 13;
        private readonly int frameHeight = 30;

        public int Owner = -1;
        public float RotateRadiusFactor = 0f;
        public int RotateDir = 1;
        public ref float offset => ref ai[0];
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;
        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Frame.Y = Main.rand.Next(maxFrameY) * frameHeight;
            Color = Color.White;
            RotateRadiusFactor = Main.rand.NextFloat();
            offset = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void AI()
        {
            Owner.GetNPCOwner(out NPC npc/*, () => active = false*/);

            //Main.NewText($"{npc.whoAmI} {offset} {Main.GameUpdateCount}");
            if (npc == null)
                return;
            float rotSpeed = 0.05f;
            float maxRadius = Utils.Remap(npc.velocity.Length(), 0f, 10f, 100f, 20f);
            Vector2 targetPos = npc.Center + (Time * rotSpeed + offset).ToRotationVector2() * RotateRadiusFactor * maxRadius;
            Position = Vector2.Lerp(Position, targetPos, 0.1f);
        }
        public void Follow(NPC npc)
        {
            Position += npc.position - npc.oldPosition;
        }
        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;

            var frameBox = tex.Frame(1, maxFrameY, Frame.X, Frame.Y);
            Color color = Lighting.GetColor(Position.ToTileCoordinates(), Color);
            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox
                , color, Rotation, frameBox.Size() / 2, Scale, 0, 0);
            return false;
        }
    }
}