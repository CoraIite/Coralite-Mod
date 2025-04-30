using Coralite.Content.Biomes;
using Coralite.Content.Dusts;
using Coralite.Content.Items.Banner;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Coralite.Content.NPCs.Crystalline
{
    public class CrystallineHumming : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

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

        private bool CanDrawShadowTrail
        {
            get => NPC.localAI[0] == 1;
            set
            {
                if (value)
                    NPC.localAI[0] = 1;
                else
                    NPC.localAI[0] = 0;
            }
        }

        private ref float TextTimer => ref NPC.localAI[1];
        private TextTypes TextType
        {
            get => (TextTypes)NPC.localAI[2];
            set => NPC.localAI[2] = (int)value;
        }
        private ref float Recorder2 => ref NPC.localAI[3];

        private Player Target => Main.player[NPC.target];

        private bool Init = true;

        private enum AIStates
        {
            Idle_Flying,
            Idle_Floating,

            Attack_Dash,
            Attack_Shoot,
            Attack_Floating,
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
            Main.npcFrameCount[Type] = 8;
            NPC.QuickTrailSets(Helper.NPCTrailingMode.RecordAll, 8);
        }

        public override void Load()
        {
            this.LoadGore(4);
            this.RegisterBestiaryDescription();
        }

        public override void SetDefaults()
        {
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CrystallineHummingBannerItem>();


            NPC.width = 54;
            NPC.height = 54;
            NPC.damage = 70;
            NPC.defense = 20;

            NPC.lifeMax = 700;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.5f;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastHurt;
            NPC.DeathSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, 25, 0);

            SpawnModBiomes = [ModContent.GetInstance<CrystallineSkyIsland>().Type];
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Main.getGoodWorld)
            {
                NPC.scale = 0.75f;
                NPC.width = (int)(NPC.width * NPC.scale);
                NPC.height = (int)(NPC.height * NPC.scale);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.AddTags(                
                this.GetBestiaryDescription()
                );
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            npcHitbox = Utils.CenteredRectangle(NPC.Center, new Vector2(NPC.scale * 42));
            return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.hardMode && spawnInfo.Player.InModBiome<CrystallineSkyIsland>())
                return 0.05f;

            return 0;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //固定掉落蕴魔水晶
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineMagike>(), 1, 2, 5));

            //小概率掉落锋芒
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineLance>(), 20));

            //掉落宝石原石
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryRoughGemstone>(), 3, 1, 3));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SeniorRoughGemstone>(), 3, 1, 3));
        }

        public override bool? CanFallThroughPlatforms() => true;

        #endregion

        #region AI

        /*
         * 蕴魔蜂鸟
         * 
         * 待机阶段：到处飞行，撞墙就反方向飞
         * 飞行一段时间后悬停，朝向另一个方向飞
         * 
         * 每隔一段时间检测周围玩家，如果有可攻击的玩家那么就转变为进攻状态
         * 
         * 进攻阶段：主要有2个招式
         * 冲刺，帧图为5~7，悬停后瞄准玩家冲刺
         * 
         * 弹幕，悬停后短暂蓄力，之后射出水晶刺，水晶刺撞到物块后会爆开生成衍生弹幕
         * 
         * 休息阶段：随意乱飞
         */

        public override void AI()
        {
            if (Init)
            {
                Initialize();
                Init = false;
            }

            switch (State)
            {
                case AIStates.Idle_Flying:
                    IdleFlying();

                    SetSpriteDirectionAndRot();
                    NormalFlyFrame();
                    break;
                case AIStates.Idle_Floating:
                    IdleFloating();

                    SetSpriteDirectionAndRot();
                    NormalFlyFrame();
                    break;
                case AIStates.Attack_Dash:
                    AttackDash();
                    break;
                case AIStates.Attack_Shoot:
                    AttackShoot();
                    break;
                case AIStates.Attack_Floating:
                    AttackFloating();
                    AttackDirection();

                    NormalFlyFrame();
                    break;
                default:
                    break;
            }

            if (TextTimer > 0)
            {
                TextTimer--;
                if (TextTimer == 0)
                    TextType = TextTypes.None;
            }

            Lighting.AddLight(NPC.Center, new Vector3(0.16f, 0.06f, 0.2f));
        }

        public void Initialize()
        {
            State = AIStates.Idle_Floating;

            if (!VaultUtils.isClient)
            {
                Recorder = Main.rand.Next(60 * 3, 60 * 5);
                Timer = 0;
                NPC.netUpdate = true;
            }
            else
                Recorder = 100;//防止客户端出问题
        }

        #region 待机状态

        public void IdleFlying()
        {
            //飞一段时间就停
            if (Timer > Recorder)
            {
                State = AIStates.Idle_Floating;
                if (!VaultUtils.isClient)
                {
                    Recorder = Main.rand.Next(60 * 3, 60 * 12);
                    Timer = 0;
                    CanHit = false;
                    NPC.netUpdate = true;
                }
                else
                    Recorder = 100;

                return;
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
            SpeedUp(4, 0.15f);
        }

        public void IdleFloating()
        {
            //停一段时间开飞
            if (Timer > Recorder)
            {
                State = AIStates.Idle_Flying;
                if (!VaultUtils.isClient)
                {
                    NPC.velocity = Main.rand.NextVector2CircularEdge(2, 2);
                    Recorder = Main.rand.Next(60, 60 * 5);
                    Timer = 0;
                    CanHit = false;
                    NPC.netUpdate = true;
                }
                else
                    Recorder = 100;
                return;
            }

            //尝试开始攻击
            if (Timer % 30 == 0)
                TryTurnToAttack();

            if (Timer % 120 == 0)
            {
                NPC.velocity = NPC.velocity.RotateByRandom(-MathHelper.Pi, MathHelper.Pi);
            }

            Timer++;

            //撞墙反飞
            CollideSpeed();
            SpeedUp(0.5f, 0.02f, 0.2f);
        }

        private void TryTurnToAttack()
        {
            NPC.TargetClosest(false);

            //找到玩家了就转向攻击阶段
            if (NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget())
                StartAttack();//不攻击隐身玩家
        }

        /// <summary>
        /// 转变成待机模式，随机飞行或悬浮
        /// </summary>
        public void TurnToIdle(bool Confused = false)
        {
            Timer = 0;
            CanHit = false;
            CanDrawShadowTrail = false;

            if (!VaultUtils.isClient)
            {
                State = Main.rand.NextFromList(AIStates.Idle_Floating, AIStates.Idle_Flying);

                Recorder = Main.rand.Next(60 * 3, 60 * 5);
                NPC.netUpdate = true;
            }
            else
            {
                State = AIStates.Idle_Floating;
                Recorder = 100;//防止客户端出问题
            }

            if (Confused)
            {
                TextTimer = 90;
                TextType = TextTypes.Confusion;
            }
        }

        private void StartAttack()
        {
            Timer = 0;
            Recorder = 0;
            Recorder2 = 0;
            CanHit = false;
            CanDrawShadowTrail = false;

            if (State == AIStates.Idle_Floating || State == AIStates.Idle_Flying)
            {
                TextTimer = 90;
                TextType = TextTypes.Surprise;
            }

            //Main.NewText("开始攻击！");
            if (VaultUtils.isClient)
            {
                State = Main.rand.NextFromList(AIStates.Attack_Shoot, AIStates.Attack_Dash);
                NPC.netUpdate = true;
            }
            else
            {
                State = AIStates.Attack_Floating;
                Recorder = 20;
            }

            //State = AIStates.Attack_Shoot;//测试用
        }

        #endregion

        #region 攻击状态

        /// <summary>
        /// 使用<see cref="Recorder"/>当作子状态机
        /// </summary>
        public void AttackDash()
        {
            const int Chasing = 0;
            const int Attacking = 1;

            switch (Recorder)
            {
                default:
                case Chasing://到玩家附近
                    {
                        Vector2 velocity = NPC.velocity;

                        float distance = NPC.Distance(Target.Center);

                        if (distance > 16 * 25)//太远就飞近点
                        {
                            velocity += (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.5f;
                            if (velocity.Length() > 14)
                                velocity = velocity.SafeNormalize(Vector2.Zero) * 14;
                        }
                        else if (distance < 16 * 10)//太近就飞远点
                        {
                            velocity -= (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.35f;
                            if (velocity.Length() > 14)
                                velocity = velocity.SafeNormalize(Vector2.Zero) * 14;
                        }
                        else//不远不近悬浮并大幅加快计时器速度
                        {
                            velocity *= 0.95f;
                            Timer += 10;
                        }

                        NPC.velocity = velocity;
                        AttackDirection();
                        NormalFlyFrame();

                        Timer++;
                        if (Timer > 10 * 60)
                        {
                            //可以打到玩家，进行下一步攻击动作
                            if (NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget())
                            {
                                Recorder = Attacking;
                                Timer = 0;
                                NPC.netUpdate = true;
                            }
                            else
                                TurnToIdle(true);
                        }
                    }
                    break;
                case Attacking:

                    const int DashTime = 25;

                    do
                    {
                        if (Timer < 20)//准备
                        {
                            NPC.velocity *= 0.96f;
                            AttackDirection();
                            NormalFlyFrame();

                            //粒子
                            if (Timer % 3 == 0)
                            {
                                float distance = Main.rand.NextFloat(64, 80);
                                Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                                var p = PRTLoader.NewParticle<ChaseablePixelLine>(pos
                                    , (NPC.Center - pos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3.5f, 5f), newColor: Main.rand.NextFromList(Coralite.CrystallinePurple, Coralite.MagicCrystalPink)
                                      , Scale: Main.rand.NextFloat(1f, 1.5f));

                                p.entity = NPC;
                                p.fadeFactor = 0.9f;
                                p.TrailCount = 20;
                            }

                            Timer++;

                            break;
                        }

                        if (Timer == 20)//准备攻击
                        {
                            if (NPC.frame.Y < 7)
                            {
                                if (++NPC.frameCounter > 5)
                                {
                                    NPC.frameCounter = 0;
                                    NPC.frame.Y++;
                                }
                            }
                            else//开始攻击
                            {
                                Timer++;
                                CanHit = true;
                                CanDrawShadowTrail = true;
                                NPC.knockBackResist = 0;

                                Helper.PlayPitched(CoraliteSoundID.Shoot_DD2_JavelinThrowersAttack, NPC.Center);

                                NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20;
                                NPC.rotation = NPC.velocity.ToRotation()
                                    + (NPC.spriteDirection > 0 ? (-0.65f) : (0.65f + MathHelper.Pi));
                                NPC.netUpdate = true;
                            }

                            break;
                        }

                        if (Timer < 20 + DashTime)//冲刺过程中
                        {
                            Timer++;
                            bool collide = NPC.collideX || NPC.collideY;

                            if (NPC.Distance(Target.Center) > 16 * 40 || collide)//离玩家太远立刻停止攻击
                            {
                                if (collide)//撞墙后自己受伤
                                {
                                    Helper.PlayPitched(CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath, NPC.Center);
                                    NPC.SimpleStrikeNPC(Helper.ScaleValueForDiffMode(75, 85, 100, 30), 0, damageVariation: true, noPlayerInteraction: true);
                                    TextTimer = 90;
                                    TextType = TextTypes.Hurt;

                                    Vector2 dir = -NPC.velocity.SafeNormalize(Vector2.Zero);
                                    for (int i = 0; i < 12; i++)
                                        Dust.NewDustPerfect(NPC.Center - dir * 32, ModContent.DustType<CrystallineDustSmall>()
                                            , dir.RotateByRandom(-0.4f, 0.4f) * Main.rand.NextFloat(1, 4), Scale: Main.rand.NextFloat(1, 1.6f));

                                    NPC.velocity *= -0.4f;
                                }

                                Timer = 20 + DashTime;
                                NPC.netUpdate = true;
                            }
                            break;
                        }

                        if (Timer == 20 + DashTime)
                        {
                            CanHit = false;
                            CanDrawShadowTrail = false;
                            NPC.knockBackResist = 0.5f;
                            NPC.netUpdate = true;
                        }

                        NPC.velocity *= 0.94f;
                        NPC.rotation = NPC.rotation.AngleLerp(0, 0.2f);
                        NormalFlyFrame();
                        if (Timer > 20 + DashTime + 60)
                            RecheckAttack();

                        Timer++;

                    } while (false);
                    break;
            }
        }

        /// <summary>
        /// 使用<see cref="Recorder"/>当作子状态机
        /// </summary>
        public void AttackShoot()
        {
            const int Chasing = 0;
            const int Attacking = 1;

            switch (Recorder)
            {
                default:
                case Chasing://到玩家附近
                    {
                        Vector2 velocity = NPC.velocity;

                        float distance = NPC.Distance(Target.Center);

                        if (distance > 16 * 25)//太远就飞近点
                        {
                            velocity += (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.5f;
                            if (velocity.Length() > 14)
                                velocity = velocity.SafeNormalize(Vector2.Zero) * 14;
                        }
                        else if (distance < 16 * 10)//太近就飞远点
                        {
                            velocity -= (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.35f;
                            if (velocity.Length() > 14)
                                velocity = velocity.SafeNormalize(Vector2.Zero) * 14;
                        }
                        else//不远不近悬浮并大幅加快计时器速度
                        {
                            velocity *= 0.95f;
                            Timer += 10;
                        }

                        NPC.velocity = velocity;
                        AttackDirection();
                        NormalFlyFrame();

                        Timer++;
                        if (Timer > 10 * 60)
                        {
                            //可以打到玩家，进行下一步攻击动作
                            if (NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget())
                            {
                                Recorder = Attacking;
                                Recorder2 = NPC.rotation + (NPC.spriteDirection > 0 ? 0 : MathHelper.Pi);
                                Timer = 0;
                                NPC.netUpdate = true;
                            }
                            else
                                TurnToIdle(true);
                        }
                    }
                    break;
                case Attacking:

                    const int ReadyTime = 30;

                    do
                    {
                        if (Timer < ReadyTime)//准备
                        {
                            NPC.velocity *= 0.96f;
                            AttackDirection();
                            NormalFlyFrame();

                            float rot = (Target.Center - NPC.Center).ToRotation();
                            Recorder2 = Recorder2.AngleLerp(rot, 0.25f);
                            NPC.rotation = NPC.rotation.AngleLerp(Recorder2 + (NPC.spriteDirection > 0 ? (-0.65f) : (0.65f + MathHelper.Pi)), 0.35f);

                            //粒子
                            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(48, 48);
                            Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<PixelPoint>(),
                                (NPC.Center - pos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 3f)
                                , newColor: Main.rand.NextFromList(Coralite.CrystallinePurple, Coralite.MagicCrystalPink), Scale: Main.rand.NextFloat(0.8f, 1.5f));

                            d.noGravity = true;

                            Timer++;

                            break;
                        }

                        if (Timer == ReadyTime)//准备攻击
                        {
                            AttackDirection();

                            float rot = (Target.Center - NPC.Center).ToRotation();
                            Recorder2 = Recorder2.AngleLerp(rot, 0.08f);
                            NPC.rotation = Recorder2 + (NPC.spriteDirection > 0 ? (-0.65f) : (0.65f + MathHelper.Pi));

                            if (NPC.frame.Y < 7)
                            {
                                if (++NPC.frameCounter > 5)
                                {
                                    NPC.frameCounter = 0;
                                    NPC.frame.Y++;
                                }
                            }
                            else//开始攻击，射出
                            {
                                Timer++;
                                Vector2 dir2 = Recorder2.ToRotationVector2();
                                NPC.velocity = -dir2 * 3;

                                if (!VaultUtils.isClient)
                                {
                                    NPC.NewProjectileInAI<CrystallineSpike>(NPC.Center + dir2 * 22, dir2 * 9
                                        , Helper.GetProjDamage(80, 100, 140), 0);

                                    if (Main.getGoodWorld)
                                        for (int i = -1; i < 2; i += 2)
                                            NPC.NewProjectileInAI<CrystallineSpike>(NPC.Center + dir2 * 22, dir2.RotatedBy(i * 0.4f) * 9
                                                , Helper.GetProjDamage(60, 80, 100), 0);
                                }

                                Helper.PlayPitched(CoraliteSoundID.Stinger_Item17, NPC.Center, pitch: 0.5f);
                                Helper.PlayPitched(CoraliteSoundID.Swing_DD2_MonkStaffSwing, NPC.Center);
                                WindCircle.Spawn(NPC.Center + dir2 * 15, -dir2 * 2, Recorder2, Coralite.CrystallinePurple, 0.75f, 1.3f, new Vector2(1.2f, 1f));

                                NPC.netUpdate = true;
                            }

                            break;
                        }

                        NPC.rotation = NPC.rotation.AngleLerp(0, 0.2f);
                        NPC.velocity *= 0.96f;

                        if (Timer == ReadyTime + 1)
                        {
                            if (NPC.frame.Y > 3)
                            {
                                if (++NPC.frameCounter > 5)
                                {
                                    NPC.frameCounter = 0;
                                    NPC.frame.Y--;
                                }
                            }
                            else
                                Timer++;

                            break;
                        }

                        NormalFlyFrame();
                        if (Timer > ReadyTime + 1 + 60)
                            RecheckAttack();

                        Timer++;

                    } while (false);
                    break;
            }
        }

        public void AttackFloating()
        {
            //飞一段时间就继续攻击
            if (Timer > Recorder)
            {
                RecheckAttack();
                return;
            }

            //检查玩家
            if (Timer % 45 == 0)
            {
                if (!(NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget()))
                    TurnToIdle(true);
            }

            Timer++;

            Vector2 velocity = NPC.velocity;

            float distance = NPC.Distance(Target.Center);

            if (distance > 16 * 35)//太远就飞近点
            {
                velocity += (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.5f;
                if (velocity.Length() > 14)
                    velocity = velocity.SafeNormalize(Vector2.Zero) * 14;

                NPC.velocity = velocity;
            }
            else if (distance < 16 * 10)//太近就飞远点
            {
                velocity -= (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.35f;
                if (velocity.Length() > 14)
                    velocity = velocity.SafeNormalize(Vector2.Zero) * 14;

                NPC.velocity = velocity;
            }
            else//不远不近随便飘一会
            {
                if (Timer % 45 == 0)
                    NPC.velocity = NPC.velocity.RotateByRandom(-MathHelper.Pi, MathHelper.Pi);

                SpeedUp(3f, 0.15f);
            }

            //撞墙反飞
            CollideSpeed();
        }

        public void RecheckAttack()
        {
            NPC.TargetClosest();
            if (NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget())
            {
                Timer = 0;
                Recorder = 0;
                Recorder2 = 0;
                CanHit = false;
                CanDrawShadowTrail = false;

                if (VaultUtils.isClient)
                {
                    State = AIStates.Attack_Floating;
                    Recorder = 20;
                }
                else
                {
                    switch (State)//随机攻击动作
                    {
                        case AIStates.Attack_Dash:
                        case AIStates.Attack_Shoot:
                            State = AIStates.Attack_Floating;
                            Recorder = Main.rand.Next(60, 60 * 4);
                            break;
                        case AIStates.Attack_Floating:
                            State = Main.rand.NextFromList(AIStates.Attack_Dash, AIStates.Attack_Shoot);
                            break;
                        default:
                            TurnToIdle();
                            break;
                    }

                    NPC.netUpdate = true;
                }
            }
            else
                TurnToIdle();
        }

        #endregion

        /// <summary>
        /// 需要玩家不隐身（隐身是指有隐身药水BUFF和不在使用物品）<br></br>
        /// 需要距离小于800，并且可以看到玩家
        /// </summary>
        /// <returns></returns>
        public bool CanHitTarget()
            => !(Target.invis && Target.itemAnimation == 0) && Vector2.Distance(NPC.Center, Target.Center) < 800 &&
                    Collision.CanHit(NPC.Center, 1, 1, Target.TopLeft, Target.width, Target.height);

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

        public void SetSpriteDirectionAndRot()
        {
            NPC.spriteDirection = MathF.Sign(NPC.velocity.X);
            NPC.rotation = NPC.velocity.X * 0.1f;
        }

        public void AttackDirection()
        {
            float xLength = Target.Center.X - NPC.Center.X;

            if (xLength > 8)
                NPC.spriteDirection = 1;
            else if (xLength < -8)
                NPC.spriteDirection = -1;

            NPC.rotation = NPC.velocity.X * 0.1f;
        }

        public void NormalFlyFrame()
        {
            if (++NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 3)
                    NPC.frame.Y = 0;
            }
        }

        #endregion

        public override void HitEffect(NPC.HitInfo hit)
        {
            Hit_TurnToAttack();
            Hit_ReduceRestTime();

            if (NPC.life <= 0)
            {
                this.SpawnGore(4, 2);
                for (int i = 0; i < 6; i++)
                {
                    Dust d = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<CrystallineDustSmall>()
                        , Helper.NextVec2Dir(0.5f, 2.5f));
                    d.noGravity = true;
                }
            }
        }

        public void Hit_TurnToAttack()
        {
            if (State != AIStates.Idle_Floating && State != AIStates.Idle_Flying)
                return;

            TryTurnToAttack();
        }

        public void Hit_ReduceRestTime()
        {
            if (State != AIStates.Attack_Floating)
                return;

            Timer += 20;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return CanHit;
        }

        #region 绘制

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();

            Vector2 pos = NPC.Center - screenPos;
            var frameBox = mainTex.Frame(1, 8, NPC.frame.X, NPC.frame.Y);
            SpriteEffects effect = NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = frameBox.Size() / 2;

            //绘制残影
            if (CanDrawShadowTrail)
            {
                Vector2 offset = NPC.Size / 2 - screenPos;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 p = NPC.oldPos[i] + offset;
                    spriteBatch.Draw(mainTex, p, frameBox, drawColor * (0.35f - i * 0.35f / 8), NPC.oldRot[i]
                        , origin, NPC.scale, effect, 0);
                }
            }

            //绘制本体
            spriteBatch.Draw(mainTex, pos, frameBox, drawColor, NPC.rotation, origin, NPC.scale, effect, 0);

            if (TextTimer > 0)
            {
                string text = TextType switch
                {
                    TextTypes.Confusion => "( o O ) ?",
                    TextTypes.Surprise => "( o o ) !",
                    TextTypes.Hurt => "( x x )",
                    _ => ""
                };

                Utils.DrawBorderString(spriteBatch, text, pos + new Vector2(0, -48), Coralite.CrystallinePurple
                    , 1f, 0.5f, 0.5f);
            }
            return false;
        }

        #endregion
    }

    public class CrystallineSpike : ModProjectile
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 7);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 2 * 180;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 8)
                Projectile.velocity.Y += 0.05f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, Coralite.CrystallinePurple.ToVector3() / 2);
        }

        public override void OnKill(int timeLeft)
        {
            if (!VaultUtils.isClient)
            {
                Vector2 dir2 = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = -1; i < 2; i++)
                    Projectile.NewProjectileFromThis<CrystallinePieces>(Projectile.Center
                        , dir2.RotatedBy(i * 0.65f) * 12, (int)(Projectile.damage * 0.66f), 0, i + 1);
            }

            Helper.PlayPitched(CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact, Projectile.Center);
            Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 12; i++)
                Dust.NewDustPerfect(Projectile.Center - dir * 32, ModContent.DustType<CrystallineDustSmall>()
                    , dir.RotateByRandom(-0.4f, 0.4f) * Main.rand.NextFloat(1, 4), Scale: Main.rand.NextFloat(1, 1.6f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 7, 1, 7, 1, 0.785f, -1);
            Projectile.QuickDraw(lightColor, 0.785f);
            return false;
        }
    }

    public class CrystallinePieces : ModProjectile
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 5);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.97f;
            Projectile.velocity.Y += 0.25f;
            Projectile.rotation += Projectile.velocity.Length() * 0.04f;
            Lighting.AddLight(Projectile.Center, Coralite.CrystallinePurple.ToVector3() / 2);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<CrystallineDustSmall>()
                    , Helper.NextVec2Dir(0.5f, 1.5f));
                d.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            var frameBox = tex.Frame(3, 1, (int)Projectile.ai[0], 0);

            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 5, 1, 5, 1, Projectile.scale, frameBox, 0);
            Projectile.QuickDraw(frameBox, lightColor, 0);
            return false;
        }
    }
}
