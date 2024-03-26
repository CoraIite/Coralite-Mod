using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.NPCs.Shadow
{
    public class ShadowVilliger:ModNPC
    {
        public override string Texture => AssetDirectory.ShadowNPCs + Name;

        public Player Target => Main.player[NPC.target];

        public ref float TargetTimer => ref NPC.ai[0];
        public ref float JumpCount => ref NPC.ai[1];
        public ref int FrameX => ref NPC.frame.X;
        public ref int FrameY => ref NPC.frame.Y;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 52;
            NPC.lifeMax = 120;
            NPC.damage = 20;
            NPC.defense = 10;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(0, 0, 2, 50);

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
                case 0f://寻找玩家
                    JumpCount -= 2f;
                    NPC.TargetClosest(true);
                        FrameX = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            TargetTimer = Main.rand.Next(200, 330);

                    NPC.direction = (Target.position.X - NPC.position.X) > 0 ? 1 : -1;
                    NPC.frameCounter = 0;
                    FrameY = 1;
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
                    if (JumpCount > 2)
                    {
                        NPC.direction *= -1;
                        JumpCount = 0;
                    }
                    //控制帧图部分
                    NPC.frameCounter++;
                    if (NPC.frameCounter % 6 == 0)
                    {
                        FrameY++;
                        if (FrameY >= 12)
                            FrameY = 1;

                        NPC.frameCounter = 0;
                    }

                    if (NPC.velocity.Y != 0)
                    {
                        FrameY = 0;
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
            Texture2D mainTex = NPC.GetTexture();

            Rectangle frameBox = mainTex.Frame(1, 13, 0, FrameY);
            Vector2 origin = frameBox.Size() / 2;
            SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 pos = NPC.Center - screenPos+new Vector2(0,-2);
            Color c = Color.Purple;
            c.A = 0;
            c *= 0.5f;
            for (int i = 0; i < 3; i++)
                spriteBatch.Draw(mainTex, pos + (Main.GlobalTimeWrappedHourly + i * MathHelper.TwoPi / 3).ToRotationVector2() * 4
                    , frameBox, c, NPC.rotation, origin, NPC.scale, effects, 0f);

            spriteBatch.Draw(mainTex, pos, frameBox, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }
    }
}
