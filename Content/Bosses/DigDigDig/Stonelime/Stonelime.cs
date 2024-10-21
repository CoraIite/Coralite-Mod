using Coralite.Content.Items.DigDigDig.Stonelimes;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using static Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor;

namespace Coralite.Content.Bosses.DigDigDig.Stonelime
{
    [AutoloadBossHead]
    public class Stonelime : ModNPC
    {
        public override string Texture => AssetDirectory.DigDigDigBoss + "Stonelime";

        public ref float Width => ref NPC.localAI[3];

        private CrownDatas crown;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.GravityMultiplier *= 2f;

            NPC.boss = true;
            NPC.width = 98;
            NPC.height = 92;
            NPC.damage = 40;
            NPC.defense = 15;
            NPC.lifeMax = 1800;
            NPC.knockBackResist = 0f;
            NPC.HitSound = CoraliteSoundID.DigStone_Tink;
            NPC.DeathSound = CoraliteSoundID.StoneBurst_Item70;
            NPC.value = 10000f;
            NPC.scale = 1.25f;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 5f;

            NPC.BossBar = ModContent.GetInstance<StonelimeBossBar>();
            ModContent.GetInstance<StonelimeBossBar>().Reset(NPC);

            Music = MusicID.Boss1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.7f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<StoneRelic>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StoneMakerCore>(),1,8,14));
        }

        public override void AI()
        {
            float num236 = 1f;
            float num237 = 1f;
            bool init = false;
            bool flag7 = false;
            bool flag8 = false;

            NPC.aiAction = 0;
            if (NPC.ai[3] == 0f && NPC.life > 0)
                NPC.ai[3] = NPC.lifeMax;

            if (Width == 0f)
            {
                Width = 6;
                if (Main.masterMode)
                    Width = 12;

                init = true;
                crown = new CrownDatas();
                crown.Bottom = NPC.Top + new Vector2(0, -50);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[0] = -100f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            CheckTarget();

            if (!Collision.CanHitLine(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0)
                || Math.Abs(NPC.Top.Y - Main.player[NPC.target].Bottom.Y) > 160f)
            {
                NPC.ai[2]++;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.localAI[0]++;
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0]--;
                if (NPC.localAI[0] < 0f)
                    NPC.localAI[0] = 0f;
            }

            if (NPC.timeLeft < 10 && (NPC.ai[0] != 0f || NPC.ai[1] != 0f))
            {
                NPC.ai[0] = 0f;
                NPC.ai[1] = 0f;
                NPC.netUpdate = true;
                flag7 = false;
            }

            Dust dust;
            if (NPC.ai[1] == 5f)
            {
                flag7 = true;
                NPC.aiAction = 1;
                NPC.ai[0]++;
                num236 = MathHelper.Clamp((60f - NPC.ai[0]) / 60f, 0f, 1f);
                num236 = 0.5f + num236 * 0.5f;
                if (NPC.ai[0] >= 60f)
                    flag8 = true;

                if (NPC.ai[0] == 60f)
                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-40f, -NPC.height / 2), NPC.velocity, 734);

                if (NPC.ai[0] >= 60f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.Bottom = new Vector2(NPC.localAI[1], NPC.localAI[2]);
                    NPC.ai[1] = 6f;
                    NPC.ai[0] = 0f;
                    NPC.netUpdate = true;

                    int count = Main.rand.Next(2, 5);
                    for (int i = 0; i < count; i++)
                    {
                        Projectile p = NPC.NewProjectileDirectInAI(NPC.Top, -Vector2.UnitY.RotateByRandom(-0.3f, 0.3f) * 12, ProjectileID.Boulder
                             , Helper.ScaleValueForDiffMode(40, 60, 80, 80), 8, NPC.target);

                        p.friendly = false;
                    }
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[0] >= 120f)
                {
                    NPC.ai[1] = 6f;
                    NPC.ai[0] = 0f;
                }

                if (!flag8)
                {
                    for (int num249 = 0; num249 < 10; num249++)
                    {
                        int num250 = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, DustID.Stone, NPC.velocity.X, NPC.velocity.Y, Scale: 2f);
                        Main.dust[num250].noGravity = true;
                        dust = Main.dust[num250];
                        dust.velocity *= 0.5f;
                    }
                }
            }
            else if (NPC.ai[1] == 6f)
            {
                flag7 = true;
                NPC.aiAction = 0;
                NPC.ai[0]++;
                num236 = MathHelper.Clamp(NPC.ai[0] / 30f, 0f, 1f);
                num236 = 0.5f + num236 * 0.5f;
                if (NPC.ai[0] >= 30f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 0f;
                    NPC.netUpdate = true;
                    NPC.TargetClosest();
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[0] >= 60f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 0f;
                    NPC.TargetClosest();
                }

                for (int num251 = 0; num251 < 10; num251++)
                {
                    int num252 = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, DustID.Stone, NPC.velocity.X, NPC.velocity.Y, Scale: 2f);
                    Main.dust[num252].noGravity = true;
                    dust = Main.dust[num252];
                    dust.velocity *= 2f;
                }
            }

            NPC.dontTakeDamage = (NPC.hide = flag8);
            if (NPC.velocity.Y == 0f)
            {
                NPC.velocity.X *= 0.8f;
                if (NPC.velocity.X > -0.1f && NPC.velocity.X < 0.1f)
                    NPC.velocity.X = 0f;

                if (!flag7)
                {
                    NPC.ai[0] += 2f;
                    if (NPC.life < NPC.lifeMax * 0.8f)
                        NPC.ai[0] += 1f;

                    if (NPC.life < NPC.lifeMax * 0.6f)
                        NPC.ai[0] += 1f;

                    if (NPC.life < NPC.lifeMax * 0.4f)
                        NPC.ai[0] += 2f;

                    if (NPC.life < NPC.lifeMax * 0.2f)
                        NPC.ai[0] += 3f;

                    if (NPC.life < NPC.lifeMax * 0.1f)
                        NPC.ai[0] += 4f;

                    if (NPC.ai[0] >= 0f)
                    {
                        NPC.netUpdate = true;
                        NPC.TargetClosest();
                        if (NPC.ai[1] == 3f)
                        {
                            NPC.velocity.Y = -13f;
                            NPC.velocity.X += 3.5f * NPC.direction;
                            NPC.ai[0] = -200f;
                            NPC.ai[1] = 0f;

                           Projectile p= NPC.NewProjectileDirectInAI(NPC.Top, -Vector2.UnitY.RotateByRandom(-0.3f, 0.3f) * 12, ProjectileID.Boulder
                                , Helper.ScaleValueForDiffMode(40, 60, 80, 80), 8, NPC.target);

                            p.friendly = false;
                        }
                        else if (NPC.ai[1] == 2f)
                        {
                            NPC.velocity.Y = -6f;
                            NPC.velocity.X += 4.5f * NPC.direction;
                            NPC.ai[0] = -120f;
                            NPC.ai[1] += 1f;
                        }
                        else
                        {
                            NPC.velocity.Y = -8f;
                            NPC.velocity.X += 4f * NPC.direction;
                            NPC.ai[0] = -120f;
                            NPC.ai[1] += 1f;
                        }
                    }
                    else if (NPC.ai[0] >= -30f)
                    {
                        NPC.aiAction = 1;
                    }
                }
            }
            else if (NPC.target < 255)
            {
                float num253 = 3f;
                if (Main.getGoodWorld)
                    num253 = 6f;

                if ((NPC.direction == 1 && NPC.velocity.X < num253) || (NPC.direction == -1 && NPC.velocity.X > 0f - num253))
                {
                    if ((NPC.direction == -1 && NPC.velocity.X < 0.1) || (NPC.direction == 1 && NPC.velocity.X > -0.1))
                        NPC.velocity.X += 0.2f * NPC.direction;
                    else
                        NPC.velocity.X *= 0.93f;
                }
            }

            int num254 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, NPC.velocity.X, NPC.velocity.Y, Scale: NPC.scale * 1.2f);
            dust = Main.dust[num254];
            dust.noGravity = true;
            dust.velocity *= 0.5f;
            if (NPC.life <= 0)
                return;

            float scale = 1.25f;
            scale *= num236;
            scale *= num237;

            float factor = Math.Clamp(NPC.life / (float)NPC.lifeMax, 0.5f, 1);

            int width2 = 6;
            if (Main.masterMode)
                width2 = 12;

            width2 = (int)(width2 * factor);
            if (scale != NPC.scale || width2!=(int)Width||init)
            {
                Width = width2;
                NPC.position.X += NPC.width / 2;
                NPC.position.Y += NPC.height;
                NPC.scale = scale;
                NPC.width = (int)(16 * Width * scale);
                NPC.height = (int)(16 * Width * scale);
                NPC.position.X -= NPC.width / 2;
                NPC.position.Y -= NPC.height;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int num256 = (int)(NPC.lifeMax * 0.05f);
            if (!((NPC.life + num256) < NPC.ai[3]))
                return;

            NPC.ai[3] = NPC.life;
            int num257 = Main.rand.Next(1, 4);
            for (int i = 0; i < num257; i++)
            {
                int x = (int)(NPC.position.X + Main.rand.Next(NPC.width - 32));
                int y = (int)(NPC.position.Y + Main.rand.Next(NPC.height - 32));

                int npcIndex = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, ModContent.NPCType<StoneSmallime>());
                Main.npc[npcIndex].velocity.X = Main.rand.Next(-15, 16) * 0.1f;
                Main.npc[npcIndex].velocity.Y = Main.rand.Next(-30, 1) * 0.1f;
                Main.npc[npcIndex].ai[0] = -1000 * Main.rand.Next(3);
                Main.npc[npcIndex].ai[1] = 0f;
                if (Main.netMode == NetmodeID.Server && npcIndex < 200)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
            }
        }

        public override void PostAI()
        {
            int height = (int)( 16 * (Width-2));
            float groundHeight = NPC.Bottom.Y - (NPC.scale * height);
            crown.Bottom.X = MathHelper.Lerp(crown.Bottom.X, NPC.Center.X, 0.5f);

            if (crown.Bottom.Y < groundHeight - 2) //重力
            {
                crown.Velocity_Y += NPC.gravity * 1.25f;
                if (crown.Velocity_Y > 16)
                    crown.Velocity_Y = 16;
            }

            crown.Bottom.Y += crown.Velocity_Y;     //更新位置
            if (crown.Bottom.Y > groundHeight)    //如果超过了地面那么就进行判断
            {
                crown.Bottom.Y = groundHeight;  //将位置拉回
                if (NPC.velocity.Y < 0.5f && crown.Velocity_Y > 5)  //速度很大，向上弹起
                {
                    //随机一个角度
                    float angle = Math.Clamp(crown.Velocity_Y / 40, 0.1f, 0.55f);
                    crown.Rotation = Main.rand.NextFloat(-angle, angle);

                    crown.Velocity_Y *= -0.2f;
                }
                else
                    crown.Velocity_Y = NPC.velocity.Y;  //速度不够直接停止
            }

            crown.Rotation = crown.Rotation.AngleLerp(0, 0.04f);
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !NPC.unlockedSlimeBlueSpawn)
            {
                NPC.unlockedSlimeBlueSpawn = true;
                int num = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X - 10, (int)NPC.Center.Y, 670);
                NPC obj = Main.npc[num];
                Vector2 movementVector = (obj.velocity = new Vector2(Main.rand.NextFloatDirection() * 3f, -10f));
                obj.netUpdate = true;
                WorldGen.CheckAchievement_RealEstateAndTownSlimes();
                ParticleOrchestrator.BroadcastParticleSpawn(ParticleOrchestraType.TownSlimeTransform, new ParticleOrchestraSettings
                {
                    PositionInWorld = NPC.Center,
                    MovementVector = movementVector,
                    UniqueInfoPiece = 0
                });
            }
            NPC.SetEventFlagCleared(ref NPC.downedSlimeKing, 11);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        private void CheckTarget()
        {
            int searchDistance = 3000;

            Player target = Main.player[NPC.target];

            if (target.dead || Vector2.Distance(NPC.Center, target.Center) > searchDistance)
            {
                NPC.TargetClosest();
                target = Main.player[NPC.target];
                if (target.dead || Vector2.Distance(NPC.Center, target.Center) > searchDistance)
                {
                    NPC.EncourageDespawn(10);
                    if (Main.player[NPC.target].Center.X < NPC.Center.X)
                        NPC.direction = 1;
                    else
                        NPC.direction = -1;

                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] != 5f)
                    {
                        NPC.netUpdate = true;
                        NPC.ai[2] = 0f;
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 5f;
                        NPC.localAI[1] = Main.maxTilesX * 16;
                        NPC.localAI[2] = Main.maxTilesY * 16;
                    }
                }
            }

            if (!target.dead && NPC.timeLeft > 10 && NPC.ai[2] >= 300f && NPC.ai[1] < 5f && NPC.velocity.Y == 0f)
            {
                NPC.ai[2] = 0f;
                NPC.ai[0] = 0f;
                NPC.ai[1] = 5f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.TargetClosest(faceTarget: false);
                    target = Main.player[NPC.target];

                    Point SelfP = NPC.Center.ToTileCoordinates();
                    Point targetP = target.Center.ToTileCoordinates();
                    Vector2 dirToTarget = target.Center - NPC.Center;
                    int randWidth = 10;
                    int num242 = 7;
                    int tryCount = 0;
                    bool flag9 = false;
                    if (NPC.localAI[0] >= 360f || dirToTarget.Length() > 2000f)
                    {
                        if (NPC.localAI[0] >= 360f)
                            NPC.localAI[0] = 360f;

                        flag9 = true;
                        tryCount = 100;
                    }

                    while (!flag9 && tryCount < 100)
                    {
                        tryCount++;
                        int randX = Main.rand.Next(targetP.X - randWidth, targetP.X + randWidth + 1);
                        int ranxY = Main.rand.Next(targetP.Y - randWidth, targetP.Y + 1);
                        if ((ranxY >= targetP.Y - num242
                            && ranxY <= targetP.Y + num242
                            && randX >= targetP.X - num242
                            && randX <= targetP.X + num242)
                            || (ranxY == SelfP.Y
                            && randX == SelfP.X)
                            || Main.tile[randX, ranxY].HasUnactuatedTile)
                            continue;

                        int num246 = ranxY;
                        int num247 = 0;
                        if (Main.tile[randX, num246].HasUnactuatedTile && Main.tileSolid[Main.tile[randX, num246].TileType] && !Main.tileSolidTop[Main.tile[randX, num246].TileType])
                        {
                            num247 = 1;
                        }
                        else
                        {
                            for (; num247 < 150 && num246 + num247 < Main.maxTilesY; num247++)
                            {
                                int num248 = num246 + num247;
                                if (Main.tile[randX, num248].HasUnactuatedTile && Main.tileSolid[Main.tile[randX, num248].TileType] && !Main.tileSolidTop[Main.tile[randX, num248].TileType])
                                {
                                    num247--;
                                    break;
                                }
                            }
                        }

                        ranxY += num247;
                        bool flag10 = true;
                        if (flag10 && Main.tile[randX, ranxY].LiquidType == LiquidID.Lava && Main.tile[randX, ranxY].LiquidAmount > 0)
                            flag10 = false;

                        if (flag10 && !Collision.CanHitLine(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0))
                            flag10 = false;

                        if (flag10)
                        {
                            NPC.localAI[1] = randX * 16 + 8;
                            NPC.localAI[2] = ranxY * 16 + 16;
                            break;
                        }
                    }

                    if (tryCount >= 100)
                    {
                        Vector2 bottom = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)].Bottom;
                        NPC.localAI[1] = bottom.X;
                        NPC.localAI[2] = bottom.Y;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Tile[TileID.Stone].Value;

            //检测宽度
            float scale = NPC.scale;
            Vector2 bottom = NPC.Bottom - Main.screenPosition;

            int fullWidth = (int)(16 * Width);//多少格

            //绘制四个角
            DrawStone(spriteBatch, mainTex, scale, bottom + new Vector2(-fullWidth / 2, -fullWidth) * scale, new Rectangle(0, 18 * 3, 16, 16));
            DrawStone(spriteBatch, mainTex, scale, bottom + new Vector2(fullWidth / 2 - 16, -fullWidth) * scale, new Rectangle(18, 18 * 3, 16, 16));
            DrawStone(spriteBatch, mainTex, scale, bottom + new Vector2(-fullWidth / 2, -16) * scale, new Rectangle(0, 18 * 4, 16, 16));
            DrawStone(spriteBatch, mainTex, scale, bottom + new Vector2(fullWidth / 2 - 16, -16) * scale, new Rectangle(18, 18 * 4, 16, 16));

            //绘制四条边            
            for (int i = 1; i < Width - 1; i++)//顶边
                DrawStone(spriteBatch, mainTex, scale, bottom + new Vector2(-fullWidth / 2 + 16 * i, -fullWidth) * scale, new Rectangle((18 * (i % 3 + 1)), 0, 16, 16));
            for (int i = 1; i < Width - 1; i++)//底边
                DrawStone(spriteBatch, mainTex, scale, bottom + new Vector2(-fullWidth / 2 + 16 * i, -16) * scale, new Rectangle((18 * (i % 3 + 1)), 18 * 2, 16, 16));
            for (int i = 1; i < Width - 1; i++)//左边
                DrawStone(spriteBatch, mainTex, scale, bottom + new Vector2(-fullWidth / 2, -fullWidth + 16 * i) * scale, new Rectangle(0, (18 * (i % 3)), 16, 16));
            for (int i = 1; i < Width - 1; i++)//底边
                DrawStone(spriteBatch, mainTex, scale, bottom + new Vector2(fullWidth / 2 - 16, -fullWidth + 16 * i) * scale, new Rectangle(18 * 4, (18 * (i % 3)), 16, 16));

            //填充中心部分
            for (int i = 1; i < Width - 1; i++)//顶边
                for (int j = 1; j < Width - 1; j++)
                {
                    int xFrame = i * 2 + j + (int)(NPC.Center.X / 32);
                    xFrame %= 4;

                    xFrame = Math.Clamp(xFrame, 1, 3);
                    DrawStone(spriteBatch, mainTex, scale, bottom + new Vector2(-fullWidth / 2 + 16 * i, -fullWidth + 16 * j) * scale, new Rectangle(18 * xFrame, 18, 16, 16));
                }

            Texture2D crownTex = TextureAssets.Extra[39].Value;
            Vector2 crownOrigin = new Vector2(crownTex.Width / 2, crownTex.Height);
            Vector2 crownPos = crown.Bottom - screenPos;
            //绘制王冠
            spriteBatch.Draw(crownTex, crownPos, null, drawColor, crown.Rotation, crownOrigin, 1, 0, 0f);

            return false;

            static void DrawStone(SpriteBatch spriteBatch, Texture2D mainTex, float scale, Vector2 pos,Rectangle rect)
            {
                spriteBatch.Draw(mainTex, pos, rect , Lighting.GetColor((pos + Main.screenPosition).ToTileCoordinates()), 0, Vector2.Zero, scale, 0, 0);
            }
        }
    }

}
