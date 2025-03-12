using Coralite.Content.Biomes;
using Coralite.Content.Dusts;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
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

        #region 基础设置

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
            NPC.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 38;
            NPC.damage = 80;
            NPC.defense = 20;

            NPC.lifeMax = 1200;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastHurt;
            NPC.DeathSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, 40, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.hardMode && spawnInfo.Player.InModBiome<CrystallineSkyIsland>())
                return 0.02f;

            return 0;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //固定掉落蕴魔水晶
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineMagike>(), 1, 2, 5));

            //小概率掉落魔鸟


            //概率掉落蕴魔海燕麦母体
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryRoughGemstone>(), 8));

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
                    State = AIStates.Idle_Floating;
                    break;
                case AIStates.Attack_Floating:
                    State = AIStates.Idle_Floating;
                    break;
                default:
                    break;
            }
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
        public void TurnToIdle()
        {
            if (!VaultUtils.isClient)
            {
                State = Main.rand.NextFromList(AIStates.Idle_Floating, AIStates.Idle_Flying);

                Recorder = Main.rand.Next(60 * 3, 60 * 5);
                Timer = 0;
                CanHit = false;
                NPC.netUpdate = true;
            }
            else
            {
                State = AIStates.Idle_Floating;
                Recorder = 100;//防止客户端出问题
            }
        }

        private void StartAttack()
        {
            //Main.NewText("开始攻击！");
            State = Main.rand.NextFromList(AIStates.Attack_Shoot, AIStates.Attack_Dash);
            State = AIStates.Attack_Dash;
            Timer = 0;
            Recorder = 0;
            NPC.netUpdate = true;
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
                                TurnToIdle();
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
                            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(48, 48);
                            var p = PRTLoader.NewParticle<PixelLine>(pos
                                , (NPC.Center - pos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1.5f, 3), newColor: Coralite.CrystallineMagikePurple
                                  , Scale: 1);

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
                                if (collide)
                                {
                                    Helper.PlayPitched(CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath, NPC.Center);
                                    NPC.SimpleStrikeNPC(50, 0);
                                }
                                Timer = 20 + 80;
                                NPC.netUpdate = true;
                            }
                            break;
                        }

                        if (Timer == 20 + DashTime)
                        {
                            CanHit = false;
                            CanDrawShadowTrail = false;
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

        public void RecheckAttack()
        {
            NPC.TargetClosest();
            if (NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget())
            {
                Timer = 0;
                Recorder = 0;
                CanHit = false;

                if (VaultUtils.isClient)
                    State = AIStates.Attack_Floating;
                else
                {
                    switch (State)//随机攻击动作
                    {
                        case AIStates.Attack_Dash:
                            State = AIStates.Attack_Floating;
                            break;
                        case AIStates.Attack_Shoot:
                            State = AIStates.Attack_Floating;
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

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            Hit_TurnToAttack();
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            Hit_TurnToAttack();
        }

        public void Hit_TurnToAttack()
        {
            if (State != AIStates.Idle_Floating && State != AIStates.Idle_Flying)
                return;

            TryTurnToAttack();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return CanHit;
        }

        #region 网络同步

        #endregion

        #region 绘制

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();

            Vector2 pos = NPC.Center - screenPos;
            var frameBox = mainTex.Frame(1, 8, NPC.frame.X, NPC.frame.Y);
            SpriteEffects effect = NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(mainTex, pos, frameBox, drawColor, NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 1);
            return false;
        }

        #endregion
    }

    public class CrystallineSpike
    {

    }

    public class CrystallinePieces
    {

    }
}
