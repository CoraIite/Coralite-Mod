using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.NPCs
{
    public abstract class EasyWorm_LikeVanila : ModNPC
    {
        /* ai[0] = follower
        * ai[1] = following
        * ai[2] = distanceFromTail
        * ai[3] = head
        */
        public bool head;
        public bool tail;
        public int minLength;
        public int maxLength;
        public int headType;
        public int bodyType;
        public int tailType;
        public bool flies = false;
        public bool directional = false;
        public float speed;
        public float turnSpeed;

        public virtual void Init() { }

        public virtual bool ShouldRun()
        {
            return false;
        }

        public virtual void CustomBehavior() { }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return head ? null : false;
        }

        public void ControlSpriteDirection(float reference)
        {
            if (directional)
            {
                if (reference < 0f)
                    NPC.spriteDirection = 1;
                else if (reference > 0f)
                    NPC.spriteDirection = -1;
            }
        }

        public override void AI()
        {
            if (NPC.localAI[1] == 0f)
            {
                NPC.localAI[1] = 1f;
                Init();
            }

            if (NPC.ai[3] > 0f)
                NPC.realLife = (int)NPC.ai[3];

            if (!head && NPC.timeLeft < 300)
                NPC.timeLeft = 300;

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead)
                NPC.TargetClosest(true);

            if (Main.player[NPC.target].dead && NPC.timeLeft > 300)
                NPC.timeLeft = 300;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                var entitySource = NPC.GetSource_FromAI();

                if (!tail && NPC.ai[0] == 0f)
                {
                    if (head)
                    {
                        NPC.ai[3] = NPC.whoAmI;
                        NPC.realLife = NPC.whoAmI;
                        NPC.ai[2] = Main.rand.Next(minLength, maxLength + 1);
                        NPC.ai[0] = NPC.NewNPC(entitySource, (int)(NPC.position.X + (NPC.width / 2)), (int)(NPC.position.Y + NPC.height), bodyType, NPC.whoAmI);
                    }
                    else if (NPC.ai[2] > 0f)
                        NPC.ai[0] = NPC.NewNPC(entitySource, (int)(NPC.position.X + (NPC.width / 2)), (int)(NPC.position.Y + NPC.height), NPC.type, NPC.whoAmI);
                    else
                        NPC.ai[0] = NPC.NewNPC(entitySource, (int)(NPC.position.X + (NPC.width / 2)), (int)(NPC.position.Y + NPC.height), tailType, NPC.whoAmI);

                    Main.npc[(int)NPC.ai[0]].ai[3] = NPC.ai[3];
                    Main.npc[(int)NPC.ai[0]].realLife = NPC.realLife;
                    Main.npc[(int)NPC.ai[0]].ai[1] = NPC.whoAmI;
                    Main.npc[(int)NPC.ai[0]].ai[2] = NPC.ai[2] - 1f;
                    NPC.netUpdate = true;
                }

                if (!head && (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[1]].type != headType && Main.npc[(int)NPC.ai[1]].type != bodyType))
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.active = false;
                }

                if (!tail && (!Main.npc[(int)NPC.ai[0]].active || Main.npc[(int)NPC.ai[0]].type != bodyType && Main.npc[(int)NPC.ai[0]].type != tailType))
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.active = false;
                }

                if (!NPC.active && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
            }
            int position2Tile_Xless1 = (int)(NPC.position.X / 16f) - 1;
            int right2Tile_Xplus2 = (int)(NPC.Right.X / 16f) + 2;
            int Position2Tile_Yless1 = (int)(NPC.position.Y / 16f) - 1;
            int bottom2Tile_Yplus2 = (int)(NPC.Bottom.Y / 16f) + 2;
            #region 防止以上内容超界
            if (position2Tile_Xless1 < 0)
                position2Tile_Xless1 = 0;
            if (right2Tile_Xplus2 > Main.maxTilesX)
                right2Tile_Xplus2 = Main.maxTilesX;
            if (Position2Tile_Yless1 < 0)
                Position2Tile_Yless1 = 0;
            if (bottom2Tile_Yplus2 > Main.maxTilesY)
                bottom2Tile_Yplus2 = Main.maxTilesY;
            #endregion
            bool canFly = flies;
            if (!canFly)
                for (int x = position2Tile_Xless1; x < right2Tile_Xplus2; x++)
                    for (int y = Position2Tile_Yless1; y < bottom2Tile_Yplus2; y++)
                        if (Main.tile[x, y] != null && (Main.tile[x, y].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[x, y].TileType] || Main.tileSolidTop[(int)Main.tile[x, y].TileType] && Main.tile[x, y].TileFrameY == 0) || Main.tile[x, y].LiquidAmount > 64))
                        {
                            Vector2 vector17;
                            vector17.X = (x * 16);
                            vector17.Y = (y * 16);
                            if (NPC.position.X + (float)NPC.width > vector17.X && NPC.position.X < vector17.X + 16f && NPC.position.Y + (float)NPC.height > vector17.Y && NPC.position.Y < vector17.Y + 16f)
                            {
                                canFly = true;
                                if (Main.rand.NextBool(100) && NPC.behindTiles && Main.tile[x, y].HasUnactuatedTile)
                                    WorldGen.KillTile(x, y, true, true, false);
                                //这有什么用？
                                //if (Main.netMode != NetmodeID.MultiplayerClient && Main.tile[num184, num185].TileType == 2)
                                //    ushort arg_BFCA_0 = Main.tile[num184, num185 - 1].TileType;
                            }
                        }
            if (!canFly && head)
            {
                Rectangle hitBox = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int farAwayDis = 1000;
                bool farAwayFromPlayer = true;
                for (int i = 0; i < 255; i++)
                    if (Main.player[i].active)
                    {
                        Rectangle farAwayRect = new Rectangle((int)Main.player[i].position.X - farAwayDis, (int)Main.player[i].position.Y - farAwayDis, farAwayDis * 2, farAwayDis * 2);
                        if (hitBox.Intersects(farAwayRect))
                        {
                            farAwayFromPlayer = false;
                            break;
                        }
                    }

                if (farAwayFromPlayer)
                    canFly = true;
            }

            ControlSpriteDirection(NPC.velocity.X);

            float speed = this.speed;
            float turnSpeed = this.turnSpeed;
            Vector2 center2TopLeft = NPC.Center;
            float distance2TargetX = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2);
            float distance2TargetY = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2);
            distance2TargetX = (int)(distance2TargetX / 16f) * 16;
            distance2TargetY = (int)(distance2TargetY / 16f) * 16;
            center2TopLeft.X = (int)(center2TopLeft.X / 16f) * 16;
            center2TopLeft.Y = (int)(center2TopLeft.Y / 16f) * 16;
            distance2TargetX -= center2TopLeft.X;
            distance2TargetY -= center2TopLeft.Y;
            float distance2Target = (float)System.Math.Sqrt((double)(distance2TargetX * distance2TargetX + distance2TargetY * distance2TargetY));
            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
            {
                try
                {
                    center2TopLeft = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    distance2TargetX = Main.npc[(int)NPC.ai[1]].position.X + (Main.npc[(int)NPC.ai[1]].width / 2) - center2TopLeft.X;
                    distance2TargetY = Main.npc[(int)NPC.ai[1]].position.Y + (Main.npc[(int)NPC.ai[1]].height / 2) - center2TopLeft.Y;
                }
                catch { }
                NPC.rotation = (float)System.Math.Atan2((double)distance2TargetY, (double)distance2TargetX) + 1.57f;
                distance2Target = (float)System.Math.Sqrt((double)(distance2TargetX * distance2TargetX + distance2TargetY * distance2TargetY));
                int width = NPC.width;
                distance2Target = (distance2Target - width) / distance2Target;
                distance2TargetX *= distance2Target;
                distance2TargetY *= distance2Target;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + distance2TargetX;
                NPC.position.Y = NPC.position.Y + distance2TargetY;

                ControlSpriteDirection(distance2TargetX);
            }
            else
            {
                if (!canFly)
                {
                    NPC.TargetClosest(true);
                    NPC.velocity.Y = NPC.velocity.Y + 0.11f;
                    if (NPC.velocity.Y > speed)
                        NPC.velocity.Y = speed;
                    if ((double)(System.Math.Abs(NPC.velocity.X) + System.Math.Abs(NPC.velocity.Y)) < (double)speed * 0.4)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X = NPC.velocity.X - turnSpeed * 1.1f;
                        else
                            NPC.velocity.X = NPC.velocity.X + turnSpeed * 1.1f;
                    }
                    else if (NPC.velocity.Y == speed)
                    {
                        if (NPC.velocity.X < distance2TargetX)
                            NPC.velocity.X = NPC.velocity.X + turnSpeed;
                        else if (NPC.velocity.X > distance2TargetX)
                            NPC.velocity.X = NPC.velocity.X - turnSpeed;
                    }
                    else if (NPC.velocity.Y > 4f)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X = NPC.velocity.X + turnSpeed * 0.9f;
                        else
                            NPC.velocity.X = NPC.velocity.X - turnSpeed * 0.9f;
                    }
                }
                else
                {
                    if (!flies && NPC.behindTiles && NPC.soundDelay == 0)
                    {
                        float num195 = distance2Target / 40f;
                        if (num195 < 10f)
                            num195 = 10f;
                        if (num195 > 20f)
                            num195 = 20f;
                        NPC.soundDelay = (int)num195;
                        SoundEngine.PlaySound(SoundID.WormDig, NPC.position);
                    }
                    distance2Target = (float)System.Math.Sqrt((double)(distance2TargetX * distance2TargetX + distance2TargetY * distance2TargetY));
                    float num196 = System.Math.Abs(distance2TargetX);
                    float num197 = System.Math.Abs(distance2TargetY);
                    float num198 = speed / distance2Target;
                    distance2TargetX *= num198;
                    distance2TargetY *= num198;
                    if (ShouldRun())
                    {
                        bool flag20 = true;
                        for (int i = 0; i < 255; i++)
                            if (Main.player[i].active && !Main.player[i].dead && Main.player[i].ZoneCorrupt)
                                flag20 = false;

                        if (flag20)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient && (double)(NPC.position.Y / 16f) > (Main.rockLayer + (double)Main.maxTilesY) / 2.0)
                            {
                                NPC.active = false;
                                int num200 = (int)NPC.ai[0];
                                while (num200 > 0 && num200 < 200 && Main.npc[num200].active && Main.npc[num200].aiStyle == NPC.aiStyle)
                                {
                                    int num201 = (int)Main.npc[num200].ai[0];
                                    Main.npc[num200].active = false;
                                    NPC.life = 0;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num200, 0f, 0f, 0f, 0, 0, 0);
                                    num200 = num201;
                                }
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            }
                            distance2TargetX = 0f;
                            distance2TargetY = speed;
                        }
                    }

                        if (NPC.velocity.X > 0f && distance2TargetX > 0f || NPC.velocity.X < 0f && distance2TargetX < 0f || NPC.velocity.Y > 0f && distance2TargetY > 0f || NPC.velocity.Y < 0f && distance2TargetY < 0f)
                        {
                            if (NPC.velocity.X < distance2TargetX)
                            NPC.velocity.X = NPC.velocity.X + turnSpeed;
                        else if (NPC.velocity.X > distance2TargetX)
                            NPC.velocity.X = NPC.velocity.X - turnSpeed;

                        if (NPC.velocity.Y < distance2TargetY)
                            NPC.velocity.Y = NPC.velocity.Y + turnSpeed;
                        else if (NPC.velocity.Y > distance2TargetY)
                            NPC.velocity.Y = NPC.velocity.Y - turnSpeed;

                        if ((double)System.Math.Abs(distance2TargetY) < (double)speed * 0.2 && (NPC.velocity.X > 0f && distance2TargetX < 0f || NPC.velocity.X < 0f && distance2TargetX > 0f))
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y = NPC.velocity.Y + turnSpeed * 2f;
                            else
                                NPC.velocity.Y = NPC.velocity.Y - turnSpeed * 2f;
                        }
                        if ((double)System.Math.Abs(distance2TargetX) < (double)speed * 0.2 && (NPC.velocity.Y > 0f && distance2TargetY < 0f || NPC.velocity.Y < 0f && distance2TargetY > 0f))
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X = NPC.velocity.X + turnSpeed * 2f;
                            else
                                NPC.velocity.X = NPC.velocity.X - turnSpeed * 2f;
                        }
                    }
                    else if (num196 > num197)
                    {
                        if (NPC.velocity.X < distance2TargetX)
                            NPC.velocity.X = NPC.velocity.X + turnSpeed * 1.1f;
                        else if (NPC.velocity.X > distance2TargetX)
                            NPC.velocity.X = NPC.velocity.X - turnSpeed * 1.1f;

                        if ((double)(System.Math.Abs(NPC.velocity.X) + System.Math.Abs(NPC.velocity.Y)) < (double)speed * 0.5)
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y = NPC.velocity.Y + turnSpeed;
                            else
                                NPC.velocity.Y = NPC.velocity.Y - turnSpeed;
                        }
                    }
                    else
                    {
                        if (NPC.velocity.Y < distance2TargetY)
                            NPC.velocity.Y = NPC.velocity.Y + turnSpeed * 1.1f;
                        else if (NPC.velocity.Y > distance2TargetY)
                            NPC.velocity.Y = NPC.velocity.Y - turnSpeed * 1.1f;

                        if ((double)(System.Math.Abs(NPC.velocity.X) + System.Math.Abs(NPC.velocity.Y)) < (double)speed * 0.5)
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X = NPC.velocity.X + turnSpeed;
                            else
                                NPC.velocity.X = NPC.velocity.X - turnSpeed;
                        }
                    }
                }
                NPC.rotation = (float)System.Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;
                if (head)
                {
                    if (canFly)
                    {
                        if (NPC.localAI[0] != 1f)
                            NPC.netUpdate = true;
                        NPC.localAI[0] = 1f;
                    }
                    else
                    {
                        if (NPC.localAI[0] != 0f)
                            NPC.netUpdate = true;
                        NPC.localAI[0] = 0f;
                    }
                    if ((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f || NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f || NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f || NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f) && !NPC.justHit)
                    {
                        NPC.netUpdate = true;
                        return;
                    }
                }
            }

            CustomBehavior();
        }
    }
}
