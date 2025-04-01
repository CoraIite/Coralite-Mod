using Coralite.Content.Biomes;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.SmoothFunctions;
using Coralite.Core.Systems.MagikeSystem.Particles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    [AutoLoadTexture(Path =AssetDirectory.CrystallineNPCs)]
    public class CrystallineSentinel : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        [AutoLoadTexture(Name = "CrystallineSentinelFloatStone")]
        public static ATex FloatStone { get; private set; }
        [AutoLoadTexture(Name = "CrystallineSentinelGuard")]
        public static ATex GuardTex { get; private set; }

        private SecondOrderDynamics_Vec2[] FloatStoneMoves;
        private Vector2[] FloatStonePos;

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
            return base.CanHitPlayer(target, ref cooldownSlot);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
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

            return 0;
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
                boundingBox = default;

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

                    //懒得单独封个方法了，就这样
                    NPC.frame.X = 0;
                    NPC.frame.Y = 0;
                    P1FloatStoneMove();
                    break;
                case AIStates.P1Walking:
                    P1Walk();
                    WalkFrame();
                    P1FloatStoneMove();
                    break;
                case AIStates.P1Guard:
                    State = AIStates.P1Idle;
                    P1FloatStoneMove();
                    break;
                case AIStates.P1Spurt:
                    P1Spurt();
                    P1FloatStoneMove();
                    break;
                case AIStates.Exchange:
                    break;
                case AIStates.P2Idle:
                    break;
                case AIStates.P2Rolling:
                    break;
                case AIStates.P2Swing:
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
                SwitchState(AIStates.P1Walking, Main.rand.Next(60 * 2, 60 * 4), true);

            Timer--;
        }

        public void P1Walk()
        {
            //走不了或者时间大于一定值后就歇一会
            if (!CanWalkForward() || Timer < 0)
            {
                SwitchState(AIStates.P1Idle, Main.rand.Next(60 * 4, 60 * 6));
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
                if (++NPC.frame.Y > 10)
                    NPC.frame.Y = 0;
            }
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

                        if (Timer > 60 * 8||!CanWalkForward())
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
                                SwitchState(AIStates.P1Idle, 60);
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
                            SwitchState(AIStates.P1Guard);
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
                                NPC.frame.X = 3;
                                NPC.frame.Y = 0;
                                Recorder = 1;
                                Timer = 0;

                                PRTLoader.NewParticle<MagikeLozengeParticle>(NPC.Center, Vector2.Zero, Coralite.CrystallinePurple, 1.5f);

                                NPC.netUpdate = true;
                            }
                            else
                                SwitchState(AIStates.P1Idle, 60);
                        }
                        Timer++;
                    }
                    break;
                case 1:
                    {
                        //更新帧图，逐渐变为展开形态
                    }
                    break;
                case 2:
                    {

                    }
                    break;

            }
        }

        #endregion

        public void P1FloatStoneMove()
        {
            if (VaultUtils.isServer)
                return;

            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = NPC.Center +new Vector2(0,-10)+ (1f + i * MathHelper.TwoPi / 3).ToRotationVector2() * 36*NPC.scale;
                float factor = (int)Main.timeForVisualEffects * 0.02f + i * MathHelper.TwoPi / 2;
                pos += new Vector2(MathF.Cos(factor) * 3, MathF.Sin(factor) * 6);

                FloatStonePos[i] = FloatStoneMoves[i].Update(1 / 60f, pos);
            }
        }

        private void SwitchState(AIStates targetState,int? overrideTime=null,bool randDirection=false)
        {
            State = targetState;
            Recorder = 0;
            Recorder2 = 0;
            CanHit = false;

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
                    SwitchState(AIStates.P1Spurt);
                    break;
                case AIStates.P2Idle:
                    SwitchState(AIStates.P2Rolling);
                    break;
                default:
                    break;
            }

            NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;
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

                        if (State==AIStates.P1Guard)//绘制格挡特效
                        {

                        }
                    }
                    break;
                case AIStates.Exchange:
                    break;
                case AIStates.P2Idle:
                    break;
                case AIStates.P2Rolling:
                    break;
                case AIStates.P2Swing:
                    break;
                default:
                    break;
            }


            return false;
        }

        #endregion
    }

    public class CrystallineSentinelSpurtProj:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float TargetIndex => ref Projectile.ai[0];
        public ref float Direction => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = 120;
            Projectile.height = 32;
            Projectile.timeLeft = 15;
        }

        public override void AI()
        {
            if (!TargetIndex.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;

            Projectile.Center = owner.Center + new Vector2(Direction * Projectile.width / 2, 0);
            float factor = 1 - Projectile.timeLeft / 15f;

            Vector2 pos = Projectile.Center + new Vector2(Direction * (-80 + factor * 120), 8);
            Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(12, 20)
                    , DustID.PurpleTorch, new Vector2(Direction * factor * Main.rand.NextFloat(3,6), 0), Scale: Main.rand.NextFloat(1, 2));
            d.noGravity = true;

            if (Projectile.timeLeft % 3 == 0)
            {
                PRTLoader.NewParticle<HorizonArcArrowParticle>(pos
                    , new Vector2(Direction * factor * 3, 0), Coralite.CrystallinePurple, 0.45f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

}
