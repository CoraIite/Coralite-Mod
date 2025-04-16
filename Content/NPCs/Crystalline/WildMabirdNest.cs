using Coralite.Content.Biomes;
using Coralite.Content.Dusts;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.Magike.ItemTransmit;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Coralite.Content.NPCs.Crystalline
{
    public class WildMabirdNest : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        private AIStates State
        {
            get => (AIStates)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        private ref float Timer => ref NPC.ai[1];
        private ref float Recorder => ref NPC.ai[2];
        /// <summary>
        /// 当前有多少的魔鸟
        /// </summary>
        private ref float MabirdCount => ref NPC.ai[3];

        protected Player Target => Main.player[NPC.target];

        private bool init = true;

        private enum AIStates
        {
            Waiting,
            Shoot
        }

        #region 基础设置

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;
        }

        public override void Load()
        {
            this.LoadGore(3);
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 80;
            NPC.damage = 50;
            NPC.defense = 15;

            NPC.lifeMax = 600;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.HitSound = CoraliteSoundID.DigStone_Tink;
            NPC.DeathSound = CoraliteSoundID.StoneBurst_Item70;
            NPC.noGravity = false;
            NPC.value = Item.buyPrice(0, 2);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //固定掉落矽卡岩
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Skarn>(), 1, 2, 6));
            //随机掉落矽卡岩砖与平滑矽卡岩
            npcLoot.Add(new CommonDrop(ModContent.ItemType<SmoothSkarn>(), 4, 2, 6, 3));
            npcLoot.Add(new CommonDrop(ModContent.ItemType<SkarnBrick>(), 5, 2, 6, 2));

            //掉落魔鸟与魔鸟巢
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineMabird>(), 2, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BasicMabirdNest>(), 5));

            //掉落宝石原石
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryRoughGemstone>(), 1, 2, 4));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SeniorRoughGemstone>(), 1, 2, 4));
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
                    return 0.03f;
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

        public override bool? CanFallThroughPlatforms() => false;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        #endregion

        #region AI

        public override void AI()
        {
            if (init)
            {
                init = false;
                Initialize();
            }

            switch (State)
            {
                case AIStates.Waiting:
                    Waiting();
                    break;
                case AIStates.Shoot:
                    Attack();
                    break;
                default:
                    break;
            }

            GetMabird();
        }

        public void Initialize()
        {
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
        }

        public void Waiting()
        {
            if (Timer > 40)
            {
                NPC.TargetClosest(false);
                if (CanAttack())//玩家在8格以内开始发动攻击
                {
                    StartAttack();
                    return;
                }

                Timer = 0;
            }

            Timer++;
        }

        public void Attack()
        {
            if (Timer > 20)
            {
                Timer = 0;
                NPC.TargetClosest(false);
                if (CanAttack())
                {
                    if (MabirdCount > 0)//魔鸟出动！
                    {
                        Vector2 baseOffset = new Vector2(20, 34);
                        if (MabirdCount % 2 == 0)
                        {
                            baseOffset += new Vector2(8, (3 - (int)MabirdCount / 2) * 12);
                        }
                        else
                            baseOffset += new Vector2(0, (3 - (int)(MabirdCount + 1) / 2) * 12);

                        Vector2 pos = NPC.position + baseOffset;

                        if (!VaultUtils.isClient)
                        {
                            int i = NPC.NewNPC(NPC.GetSource_FromAI(), (int)pos.X, (int)pos.Y, ModContent.NPCType<WildMabird>()
                                  , 0, 1);
                            Main.npc[i].velocity = new Vector2(NPC.direction * 8, 0);
                        }

                        Dust d = Dust.NewDustPerfect(pos + new Vector2(NPC.direction * 12, 0), ModContent.DustType<CrystallineImpact>(), Vector2.Zero);
                        d.rotation = NPC.spriteDirection > 0 ? 0 : MathHelper.Pi;

                        Helper.PlayPitched(CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath, NPC.Center);
                        MabirdCount--;
                        NPC.netUpdate = true;
                    }
                }
                else
                    State = AIStates.Waiting;
            }

            Timer++;
        }

        private bool CanAttack()
        {
            return NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead
                                && !(Target.invis && Target.itemAnimation == 0) && Vector2.Distance(NPC.Center, Target.Center) < 16 * 30
                                && Collision.CanHit(NPC.Center, 1, 1, Target.TopLeft, Target.width, Target.height);
        }

        /// <summary>
        /// 每隔固定时间生产一只魔鸟
        /// </summary>
        public void GetMabird()
        {
            if (Recorder > Helper.ScaleValueForDiffMode(60 * 4, 60 * 3, 60 * 2f, 30))
            {
                Recorder = 0;
                if (MabirdCount < 6)
                {
                    MabirdCount++;
                    NPC.netUpdate = true;
                }
            }

            Recorder++;
        }

        public void StartAttack()
        {
            Timer = 0;
            Recorder = 0;
            State = AIStates.Shoot;
        }

        #endregion

        #region 网络同步

        #endregion

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
                this.SpawnGore(3, 3);
        }

        #region 绘制

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();
            Rectangle box = mainTex.Frame(1, 7, 0, (int)MabirdCount);
            spriteBatch.Draw(mainTex, NPC.Center - screenPos, box, drawColor, 0, box.Size() / 2, NPC.scale,
                NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }

        #endregion
    }

    public class WildMabird : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public const int trailCachesLength = 12;

        private AIStates State
        {
            get => (AIStates)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        private ref float Timer => ref NPC.ai[1];
        private ref float Timer2 => ref NPC.ai[2];
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

        private ref float LifeTime => ref NPC.localAI[0];

        protected Player Target => Main.player[NPC.target];

        private bool init = true;

        private enum AIStates
        {
            Idle,
            Fly,
            Attack,
        }

        private List<ColoredVertex> bars;

        #region 基础设置

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 12;
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 32;
            NPC.damage = 50;
            NPC.defense = 20;

            NPC.lifeMax = 180;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.5f;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastHurt;
            NPC.DeathSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            NPC.noGravity = true;
            NPC.value = 0;
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => CanHit;

        #endregion

        #region AI

        public override void AI()
        {
            if (init)
            {
                Initialize();
                init = false;
            }
            Lighting.AddLight(NPC.Center, Coralite.CrystallinePurple.ToVector3() / 2);

            switch (State)
            {
                case AIStates.Idle:
                    NormalFrame();
                    SetSpriteDirectionAndRot();

                    Idle();
                    break;
                case AIStates.Fly:
                    Fly();

                    NormalFrame();
                    SetSpriteDirectionAndRot();
                    break;
                case AIStates.Attack:
                    Attack();

                    LifeTime = 0;
                    break;
                default:
                    break;
            }

            LifeTime++;
            if (LifeTime > 60 * 60)
                NPC.Kill();

            NPC.UpdateOldPosCache(addVelocity: true);
            for (int i = 0; i < NPC.oldRot.Length - 1; i++)
                NPC.oldRot[i] = NPC.oldRot[i + 1];
            NPC.oldRot[^1] = NPC.velocity.ToRotation();
        }

        public void Initialize()
        {
            NPC.oldPos = new Vector2[trailCachesLength];
            NPC.oldRot = new float[trailCachesLength];

            for (int i = 0; i < trailCachesLength; i++)
            {
                NPC.oldPos[i] = NPC.Center;
                NPC.oldRot[i] = NPC.rotation;
            }
        }

        public void Idle()
        {
            if (Timer % 60 == 0)
            {
                NPC.TargetClosest(false);
                if (CanAttack())
                    StartAttack();
            }

            Timer++;

            if (MathF.Abs(NPC.velocity.Y) > 0.02f && !FindStandableTile())
                SwitchState(AIStates.Fly);

            NPC.velocity.X *= 0.94f;

            if (NPC.velocity.Y < 4)
                NPC.velocity.Y += 0.1f;

            if (NPC.collideY)
                NPC.velocity *= 0.5f;
        }

        public void Fly()
        {
            if (Timer > 0 && Timer % 30 == 0)
            {
                NPC.TargetClosest(false);
                if (CanAttack())
                {
                    StartAttack();
                    return;
                }
            }

            //每隔一段时间向下查找可以站立的物块，如果能站上去就飞到物块上

            Timer--;
            if (Timer <= 0 && !VaultUtils.isClient)
            {
                Timer = Main.rand.Next(60, 60 * 3);
                NPC.netUpdate = true;
            }

            if (Timer != 0 && Timer % 60 == 0 && FindStandableTile())//查找可以站立的物块
                SwitchState(AIStates.Idle);

            //X方向的运动
            if (NPC.collideX)
                NPC.velocity *= -0.4f;

            if (MathF.Abs(NPC.velocity.X) < 5)
                NPC.velocity.X += MathF.Sign(NPC.velocity.X) * 0.15f;

            //Y方向的运动
            if (Timer2 < 45)//按照时间飞上飞下
            {
                if (NPC.velocity.Y > 3)
                    NPC.velocity.Y -= 0.1f;
            }
            else if (Timer2 < 45 * 2)
            {
                if (NPC.velocity.Y < 3)
                    NPC.velocity.Y += 0.1f;
            }
            else
                Timer2 = 0;

            Timer2++;
        }

        public void Attack()
        {
            switch (Timer2)
            {
                default:
                case 0://飞到玩家附近
                    {
                        const int chasingTime = 60 * 3;

                        Vector2 targetPos = Target.Center + new Vector2(MathF.Sin(Timer * 0.05f) * 300, -100);

                        NPC.direction = NPC.spriteDirection = targetPos.X > NPC.Center.X ? 1 : -1;
                        NPC.directionY = targetPos.Y > NPC.Center.Y ? 1 : -1;
                        SetSpriteDirectionAndRot();

                        //追踪玩家
                        GetLengthToTargetPos(targetPos, out float xLength, out float yLength);

                        if (xLength > 50)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 7f, 0.2f, 0.3f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (yLength > 20)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 7f, 0.15f, 0.3f, 0.95f);
                        else
                            NPC.velocity.Y *= 0.95f;

                        if (xLength < 50 && yLength < 0)
                            Timer += 10;

                        Timer++;
                        NormalFrame();
                        if (Timer > chasingTime)
                        {
                            NPC.TargetClosest(false);
                            if (CanAttack())
                            {
                                NPC.frame.X = 3;
                                NPC.frame.Y = 0;
                                NPC.knockBackResist = 0;
                                CanHit = true;
                                Timer2++;
                                Timer = 0;
                                float speed = (Target.Center - NPC.Center).Length();
                                speed = 3 + speed / 40;
                                if (speed > 16)
                                    speed = 16;

                                Vector2 dir2 = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);

                                NPC.velocity = dir2 * speed;
                                NPC.netUpdate = true;

                                WindCircle.Spawn(NPC.Center + dir2 * 15, -dir2 * 2, dir2.ToRotation(), Coralite.CrystallinePurple, 0.75f, 0.5f, new Vector2(1.2f, 1f));
                            }
                            else
                            {
                                if (!VaultUtils.isClient)
                                {
                                    if (Main.rand.NextBool())
                                        NPC.velocity.X *= -1;
                                }
                                SwitchState(AIStates.Fly);
                            }
                        }
                    }
                    break;
                case 1://朝玩家攻击
                    {
                        Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(12, 12), ModContent.DustType<CrystallineDustSmall>()
                                , NPC.velocity * Main.rand.NextFloat(-0.3f, -0.1f), newColor: Coralite.CrystallinePurple
                                , Scale: Main.rand.NextFloat(1f, 1.4f));
                        d.noGravity = true;

                        SetSpriteDirectionAndRot();
                        if (++NPC.frameCounter > 3)
                        {
                            NPC.frameCounter = 0;
                            if (++NPC.frame.Y > 5)
                                NPC.frame.Y = 0;
                        }

                        if (Target.Center.Y < NPC.Center.Y)
                            NPC.velocity.Y *= 0.99f;

                        Timer++;
                        if (Timer > 30)
                        {
                            NPC.frame.X = 2;
                            NPC.frame.Y = 0;
                            CanHit = false;
                            Timer2++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://后摇
                    NPC.velocity.X *= 0.97f;

                    SetSpriteDirectionAndRot();
                    NormalFrame();

                    Timer++;
                    if (Timer > 20)
                    {
                        NPC.TargetClosest(false);
                        if (CanAttack())
                            StartAttack();
                        else
                            SwitchState(AIStates.Fly);
                    }
                    break;
            }
        }

        public void GetLengthToTargetPos(Vector2 targetPos, out float xLength, out float yLength)
        {
            xLength = NPC.Center.X - targetPos.X;
            yLength = NPC.Center.Y - targetPos.Y;

            xLength = Math.Abs(xLength);
            yLength = Math.Abs(yLength);
        }

        public bool FindStandableTile(int findCount = 20)
        {
            Point p = NPC.Center.ToTileCoordinates();
            for (int i = 0; i < findCount; i++)
            {
                Tile t = Framing.GetTileSafely(p + new Point(0, i));
                if (t.HasSolidTile())
                    return true;
            }

            return false;
        }

        private void SwitchState(AIStates state)
        {
            State = state;
            Timer = 0;
            Timer2 = 0;
            CanHit = false;

            NPC.netUpdate = true;
        }

        private void StartAttack()
        {
            State = AIStates.Attack;
            Timer = 0;
            Timer2 = 0;
            CanHit = false;
        }

        private bool CanAttack()
        {
            return NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead
                                && !(Target.invis && Target.itemAnimation == 0) && Vector2.Distance(NPC.Center, Target.Center) < 16 * 40
                                && Collision.CanHit(NPC.Center, 1, 1, Target.TopLeft, Target.width, Target.height);
        }

        public void SetSpriteDirectionAndRot()
        {
            if (MathF.Abs(NPC.velocity.X) > 0.02f)
                NPC.spriteDirection = MathF.Sign(NPC.velocity.X);
            NPC.rotation = NPC.velocity.X * 0.04f;
        }

        private void NormalFrame()
        {
            if (NPC.velocity.Length() == 0)
            {
                bool hasSoild = false;
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++)
                    {
                        Tile t = Framing.GetTileSafely((NPC.BottomLeft + new Vector2(i * 8, j * 16)).ToTileCoordinates());
                        if (t.HasUnactuatedTile && Main.tileSolid[t.TileType])
                        {
                            hasSoild = true;
                            goto over;
                        }
                    }

                over:;
                if (hasSoild)
                {
                    if (NPC.frame.X != 0)
                    {
                        NPC.frame.Y = 0;
                    }
                    NPC.frame.X = 0;
                    if (++NPC.frameCounter > 3)
                    {
                        NPC.frameCounter = 0;
                        if (NPC.frame.Y < 7)
                            NPC.frame.Y++;
                    }

                    return;
                }
            }
            else if (NPC.frame.X == 0)
                NPC.frame.Y = 0;

            if (NPC.velocity.Y < 0 && NPC.frame.X != 1 && NPC.frame.Y == 0)//切换至向上飞
                NPC.frame.X = 1;
            if (NPC.velocity.Y >= 0 && NPC.frame.X != 2 && NPC.frame.Y == 0)
                NPC.frame.X = 2;

            switch (NPC.frame.X)
            {
                default:
                    break;
                case 1:
                    FlyUpFrame();
                    break;
                case 2:
                    FlyDownFrame();
                    break;
            }
        }

        private void FlyUpFrame()
        {
            if (NPC.frame.Y < 4)
            {
                if (++NPC.frameCounter > 3)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y++;
                }
            }
            else
            {
                if (++NPC.frameCounter > 8)//延长最后一帧的持续时间
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 0;
                }
            }
        }

        private void FlyDownFrame()
        {
            if (++NPC.frameCounter > 3)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 11)
                    NPC.frame.Y = 0;
            }
        }

        #endregion

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                float r = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < 3; i++)
                {
                    float r2 = r + i * MathHelper.TwoPi / 3;
                    Dust d = Dust.NewDustPerfect(NPC.Center + r2.ToRotationVector2() * 16, ModContent.DustType<CrystallineImpact>(), Vector2.Zero, Scale: Main.rand.NextFloat(1, 1.5f));
                    d.rotation = r2;
                }
            }
        }

        #region 网络同步

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }

        #endregion

        #region 绘制

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawTrails(screenPos);
            //return false;
            SpriteEffects effect = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            NPC.QuickDraw(spriteBatch, screenPos, drawColor, NPC.GetFrameBox(4), effect);

            Vector2 pos = NPC.Center - screenPos + new Vector2(NPC.spriteDirection * 6, 0);

            Helper.DrawPrettyStarSparkle(1, 0, pos, Color.Magenta with { A = 100 } * 0.5f, Coralite.CrystallinePurple
                , 0.5f, 0, 0.5f, 0.5f, 1, MathHelper.PiOver4
                , Vector2.One * 0.75f, Vector2.One * 0.5f);
            return false;
        }

        public virtual void DrawTrails(Vector2 screenPos)
        {
            if (NPC.oldPos == null || NPC.oldPos.Length != trailCachesLength)
                return;

            Texture2D Texture = CoraliteAssets.Trail.LightShotSPA.Value;

            bars ??= [];

            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = NPC.oldPos[i];
                Vector2 normal = (NPC.oldRot[i] + 1.57f).ToRotationVector2();
                Vector2 Top = Center - Main.screenPosition + (normal * 12 * factor);
                Vector2 Bottom = Center - Main.screenPosition - (normal * 12 * factor);

                var color = Coralite.CrystallinePurple * factor;
                bars.Add(new(Top, color, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, color, new Vector3(factor, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            bars.Clear();
        }

        #endregion
    }
}
