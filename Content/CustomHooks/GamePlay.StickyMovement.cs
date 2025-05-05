using Coralite.Core;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.CustomHooks
{
    public class StickyMovement : HookGroup
    {
        public override void Load()
        {
            On_Player.StickyMovement += On_Player_StickyMovement;
        }

        public override void Unload()
        {
            On_Player.StickyMovement -= On_Player_StickyMovement;
        }

        private void On_Player_StickyMovement(On_Player.orig_StickyMovement orig, Player self)
        {
            //orig.Invoke(self);

            if (self.shimmering)
                return;

            bool flag = false;
            if (self.mount.Type > 0 && MountID.Sets.Cart[self.mount.Type] && Math.Abs(self.velocity.X) > 5f)
                flag = true;

            //new Vector2(self.Center.X - (num / 2), self.Center.Y - (num2 / 2));
            Vector2 vector = StickyTiles(self.position, self.velocity, self.width, self.height);
            if (vector.Y != -1f && vector.X != -1f)
            {
                int num3 = (int)vector.X;
                int num4 = (int)vector.Y;
                int type = Main.tile[num3, num4].TileType;
                if (self.whoAmI == Main.myPlayer && /*CoraliteSets.Tiles.Sticky.IndexInRange(type) &&*/ CoraliteSets.Tiles.Sticky[type] && (self.velocity.X != 0f || self.velocity.Y != 0f))
                {
                    self.stickyBreak++;
                    if (self.stickyBreak > Main.rand.Next(20, 100) || flag)
                    {
                        self.stickyBreak = 0;
                        WorldGen.KillTile(num3, num4);
                        if (Main.netMode == NetmodeID.MultiplayerClient && !Main.tile[num3, num4].HasTile && Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, num3, num4);
                    }
                }

                if (flag)
                    return;

                self.fallStart = (int)(self.position.Y / 16f);
                if (type != TileID.HoneyBlock)
                    self.jump = 0;

                if (self.velocity.X > 1f)
                    self.velocity.X = 1f;

                if (self.velocity.X < -1f)
                    self.velocity.X = -1f;

                if (self.velocity.X > 0.75 || self.velocity.X < -0.75)
                    self.velocity.X *= 0.85f;
                else
                    self.velocity.X *= 0.6f;

                if (self.gravDir == -1f)
                {
                    if (self.velocity.Y < -1f)
                        self.velocity.Y = -1f;

                    if (self.velocity.Y > 5f)
                        self.velocity.Y = 5f;

                    if (self.velocity.Y > 0f)
                        self.velocity.Y *= 0.96f;
                    else
                        self.velocity.Y *= 0.3f;
                }
                else
                {
                    if (self.velocity.Y > 1f)
                        self.velocity.Y = 1f;

                    if (self.velocity.Y < -5f)
                        self.velocity.Y = -5f;

                    if (self.velocity.Y < 0f)
                        self.velocity.Y *= 0.96f;
                    else
                        self.velocity.Y *= 0.3f;
                }

                if (type != TileID.HoneyBlock || !Main.rand.NextBool(5) || (!(self.velocity.Y > 0.15) && !(self.velocity.Y < 0f)))
                    return;

                if (num3 * 16 < self.position.X + (self.width / 2))
                {
                    int num5 = Dust.NewDust(new Vector2(self.position.X - 4f, num4 * 16), 4, 16, DustID.Honey2, 0f, 0f, 50);
                    Main.dust[num5].scale += Main.rand.Next(0, 6) * 0.1f;
                    Main.dust[num5].velocity *= 0.1f;
                    Main.dust[num5].noGravity = true;
                }
                else
                {
                    int num6 = Dust.NewDust(new Vector2(self.position.X + self.width - 2f, num4 * 16), 4, 16, DustID.Honey2, 0f, 0f, 50);
                    Main.dust[num6].scale += Main.rand.Next(0, 6) * 0.1f;
                    Main.dust[num6].velocity *= 0.1f;
                    Main.dust[num6].noGravity = true;
                }

                if (Main.tile[num3, num4 + 1] != null && Main.tile[num3, num4 + 1].TileType == TileID.HoneyBlock && self.position.Y + self.height > (num4 + 1) * 16)
                {
                    if (num3 * 16 < self.position.X + (self.width / 2))
                    {
                        int num7 = Dust.NewDust(new Vector2(self.position.X - 4f, (num4 * 16) + 16), 4, 16, DustID.Honey2, 0f, 0f, 50);
                        Main.dust[num7].scale += Main.rand.Next(0, 6) * 0.1f;
                        Main.dust[num7].velocity *= 0.1f;
                        Main.dust[num7].noGravity = true;
                    }
                    else
                    {
                        int num8 = Dust.NewDust(new Vector2(self.position.X + self.width - 2f, (num4 * 16) + 16), 4, 16, DustID.Honey2, 0f, 0f, 50);
                        Main.dust[num8].scale += Main.rand.Next(0, 6) * 0.1f;
                        Main.dust[num8].velocity *= 0.1f;
                        Main.dust[num8].noGravity = true;
                    }
                }

                if (Main.tile[num3, num4 + 2] != null && Main.tile[num3, num4 + 2].TileType == TileID.HoneyBlock && self.position.Y + self.height > (num4 + 2) * 16)
                {
                    if (num3 * 16 < self.position.X + (self.width / 2))
                    {
                        int num9 = Dust.NewDust(new Vector2(self.position.X - 4f, (num4 * 16) + 32), 4, 16, DustID.Honey2, 0f, 0f, 50);
                        Main.dust[num9].scale += Main.rand.Next(0, 6) * 0.1f;
                        Main.dust[num9].velocity *= 0.1f;
                        Main.dust[num9].noGravity = true;
                    }
                    else
                    {
                        int num10 = Dust.NewDust(new Vector2(self.position.X + self.width - 2f, (num4 * 16) + 32), 4, 16, DustID.Honey2, 0f, 0f, 50);
                        Main.dust[num10].scale += Main.rand.Next(0, 6) * 0.1f;
                        Main.dust[num10].velocity *= 0.1f;
                        Main.dust[num10].noGravity = true;
                    }
                }
            }
            else
            {
                self.stickyBreak = 0;
            }

        }

        public static Vector2 StickyTiles(Vector2 Position, Vector2 Velocity, int Width, int Height)
        {
            Vector2 vector = Position;
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + Height) / 16f) + 2;
            if (num < 0)
                num = 0;

            if (num2 > Main.maxTilesX)
                num2 = Main.maxTilesX;

            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesY)
                num4 = Main.maxTilesY;

            Vector2 vector2 = default;
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile == null || !tile.HasTile || tile.IsActuated)
                        continue;

                    if (/*CoraliteSets.TileSticky.IndexInRange(tile.TileType) &&*/ CoraliteSets.Tiles.Sticky[tile.TileType])
                    {
                        int num5 = 0;
                        vector2.X = i * 16;
                        vector2.Y = j * 16;
                        if (vector.X + Width > vector2.X - num5 && vector.X < vector2.X + 16f + num5 && vector.Y + Height > vector2.Y && vector.Y < vector2.Y + 16.01)
                        {
                            if (tile.TileType == 51 && (double)(Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) > 0.7 && Main.rand.NextBool(30))
                            {
                                Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.Web);
                            }

                            return new Vector2(i, j);
                        }
                    }
                    else
                    {
                        if (tile.TileType != 229 || tile.Slope != 0)
                            continue;

                        int num6 = 1;
                        vector2.X = i * 16;
                        vector2.Y = j * 16;
                        float num7 = 16.01f;
                        if (Main.tile[i, j].IsHalfBlock)
                        {
                            vector2.Y += 8f;
                            num7 -= 8f;
                        }

                        if (vector.X + Width > vector2.X - num6 && vector.X < vector2.X + 16f + num6 && vector.Y + Height > vector2.Y && vector.Y < vector2.Y + num7)
                        {
                            //if (Main.tile[i, j].type == 51 && (double)(Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) > 0.7 && Main.rand.NextBool(30))
                            //{
                            //    Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, 30);
                            //}

                            return new Vector2(i, j);
                        }
                    }
                }
            }

            return new Vector2(-1f, -1f);
        }


    }
}