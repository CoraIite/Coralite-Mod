using Coralite.Content.Items.Shadow;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.NPCs.Shadow
{
    public class ShadowGolem : ModNPC
    {
        public override string Texture => AssetDirectory.ShadowNPCs + Name;

        public Player Target => Main.player[NPC.target];

        public const int FRAME_WIDTH = 64;
        public const int FRAME_HEIGHT = 64;

        public ref float TargetTimer => ref NPC.ai[0];
        public ref float JumpCount => ref NPC.ai[1];
        public ref float FrameX => ref NPC.localAI[0];
        public ref float FrameY => ref NPC.localAI[1];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("影子巨像");
            Main.npcFrameCount[NPC.type] = 11;
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 48;
            NPC.lifeMax = 175;
            NPC.damage = 20;
            NPC.defense = 8;
            NPC.knockBackResist = 0.1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(0, 0, 3, 50);

            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        #region AI

        public override void AI()
        {
            //1：间隔随机时间，逐渐靠近玩家
            //2：在这个间隔随机时间后如果距离玩家有一定距离或者随机到了并且Y方向速度为0，则原地蓄力射弹幕

            //脱战后消失
            if (Target.dead || !Target.active || (Target.Center - NPC.Center).Length() > 2000f)
                NPC.EncourageDespawn(10);

            switch (TargetTimer)
            {
                case -1f://发射弹幕
                    NPC.velocity *= 0.8f;

                    FrameX = 1;
                    NPC.frameCounter++;
                    if (NPC.frameCounter % 8 == 0)//8帧动一下
                    {
                        FrameY++;
                        if (FrameY == 9)//生成弹幕
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Top + new Vector2(0, -8), (Target.Center - (NPC.Top + new Vector2(0, -8))).SafeNormalize(Vector2.UnitY) * 8f, ModContent.ProjectileType<ShadowGolemBall>(), 40, 4);
                        else if (FrameY >= 11)//发射完成，回到默认状态
                        {
                            TargetTimer = 0;
                            FrameX = 0;
                            FrameY = 0;
                        }

                        NPC.frameCounter = 0;
                    }
                    break;

                case 0f://寻找玩家
                    JumpCount -= 2f;
                    NPC.TargetClosest(true);
                    if (Main.rand.NextBool(3))//进入发射弹幕状态
                    {
                        FrameX = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            TargetTimer = -1;
                    }
                    else//默认状态
                    {
                        FrameX = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            TargetTimer = Main.rand.Next(200, 330);
                    }

                    NPC.direction = (Target.position.X - NPC.position.X) > 0 ? 1 : -1;
                    NPC.frameCounter = 0;
                    FrameY = 0;
                    NPC.netUpdate = true;
                    break;

                default://走动靠近玩家

                    float VelocityLimitX = 1f;
                    float accelX = 0.07f;
                    float SlowDownAccel = 0.8f;

                    if (Math.Abs(NPC.velocity.X) > VelocityLimitX)
                    {
                        if (NPC.velocity.Y == 0f)
                            NPC.velocity *= SlowDownAccel;
                    }
                    //控制X方向的移动
                    else
                    {
                        NPC.velocity.X += NPC.direction * accelX;
                        if (Math.Abs(NPC.velocity.X) > VelocityLimitX)
                            NPC.velocity.X = NPC.direction * VelocityLimitX;
                    }

                    JumpAI();
                    //如果一直在跳那么就往回走
                    if (JumpCount > 3)
                    {
                        NPC.direction *= -1;
                        JumpCount = 0;
                    }
                    //控制帧图部分
                    NPC.frameCounter++;
                    if (NPC.frameCounter % 6 == 0)
                    {
                        FrameY++;
                        if (FrameY >= 6)
                            FrameY = 0;

                        NPC.frameCounter = 0;
                    }

                    TargetTimer -= 1f;
                    if (TargetTimer < -1f)
                        TargetTimer = 0f;
                    break;
            }

            NPC.spriteDirection = NPC.direction;
        }

        public void JumpAI()
        {
            bool FaceSolidTile = false;

            int Top2Tile_X = (int)NPC.Top.X / 16;
            int Top2Tile_Y = (int)(NPC.Top.Y / 16) - 1;
            int Position2Tile_X = (int)NPC.Center.X / 16;
            int Position2Tile_Y = (int)NPC.Center.Y / 16;

            Position2Tile_X += NPC.direction;
            Position2Tile_X += NPC.velocity.X >= 0 ? 1 : -1;

            if (WorldGen.SolidTile(Position2Tile_X, Position2Tile_Y))
                FaceSolidTile = true;
            if (Math.Abs(NPC.oldPosition.X - NPC.position.X) < 0.01f)
                FaceSolidTile = true;

            //如果面对的是实心物块以及头顶上有实心物块那么就跳不起来
            for (int i = -1; i < 2; i++)
                if (WorldGen.SolidTile(Top2Tile_X + i, Top2Tile_Y) && FaceSolidTile)
                {
                    JumpCount += 0.1f;
                    return;
                }

            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

            if (NPC.velocity.Y == 0f && FaceSolidTile)//面前有方块时起跳
            {
                int Bottom2Tile_Y = (int)NPC.Bottom.Y / 16;
                float jumpAccel = 0f;

                for (int i = 1; i < 6; i++)
                {
                    if (WorldGen.SolidTile(Position2Tile_X, Bottom2Tile_Y - i))
                        jumpAccel = -MathF.Sqrt(i) * 3.3f - 0.1f;
                }

                NPC.velocity.Y = jumpAccel;
                JumpCount += 1f;

            }
        }

        #endregion

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;

            Rectangle frameBox = new Rectangle((int)FrameX * FRAME_WIDTH, (int)FrameY * FRAME_HEIGHT, FRAME_WIDTH, FRAME_HEIGHT);
            Vector2 origin = new Vector2(32, 32);
            SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(mainTex, new Vector2(NPC.Center.X, NPC.Top.Y + 16) - screenPos, frameBox, Color.White, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }

        public override void Load()
        {
            for (int i = 0; i < 2; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.ShadowGores + "ShadowGolem_Gore" + i);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Helper.PlayPitched("Shadows/Shadow_Hurt0", 0.4f, 0f, NPC.Center);
                for (int i = 0; i < 3; i++)
                    Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), DustID.Granite, null, 0, default, 1f);
            }
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.Server)
                for (int j = 0; j < 2; j++)
                {
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(1, 1), Mod.Find<ModGore>("ShadowGolem_Gore" + j).Type);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(1, 1), Mod.Find<ModGore>("ShadowGolem_Gore" + j).Type);
                }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowEnergy>(), 2, 3, 6));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneDungeon)
                return 0.05f;
            return 0f;
        }
    }
}
