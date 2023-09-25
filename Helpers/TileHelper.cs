using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Helpers
{
    public static class TileHelper
    {
        public static bool topSlope(this Tile tile)
        {
            byte b = (byte)tile.Slope;
            if (b != 1)
                return b == 2;

            return true;
        }

        public static bool HasSolidTile(this Tile tile)
        {
            return tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
        }

        public static Vector2 FindTopLeft(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null)
                return new Vector2(x, y);
            TileObjectData data = TileObjectData.GetTileData(tile.TileType, 0);
            x -= tile.TileFrameX / 18 % data.Width;
            y -= tile.TileFrameY / 18 % data.Height;
            return new Vector2(x, y);
        }

        public static void DrawMultWine(int i,int j, int sizeX,int sizeY,float? windRotOffset = 1f)
        {
            float windCycle = Main.instance.TilesRenderer.GetWindCycle(i, j, CoraliteTileDrawing.sunflowerWindCounter);
            Vector2 screenPosition = Main.Camera.UnscaledPosition;

            float num = windCycle;
            int totalPushTime = 60;
            float pushForcePerFrame = 1.26f;
            float highestWindGridPushComplex = 0f;
            if (CoraliteTileDrawing.GetHighestWindGridPushComplex != null)
                highestWindGridPushComplex = CoraliteTileDrawing.GetHighestWindGridPushComplex(Main.instance.TilesRenderer, i, j, sizeX, sizeY, totalPushTime, pushForcePerFrame, 3, true);

            windCycle += highestWindGridPushComplex;
            Vector2 vector = new Vector2(i * 16 - (int)screenPosition.X + sizeX * 16f * 0.5f, j * 16 - (int)screenPosition.Y);
            float num2 = 0.15f;
            Tile tile = Main.tile[i, j];
            int type = tile.TileType;
            Vector2 vector2 = new Vector2(0f, -2f);
            vector += vector2;
            if (((uint)(type - 591) > 1u) ? (sizeX == 1 && WorldGen.IsBelowANonHammeredPlatform(i, j)) : (WorldGen.IsBelowANonHammeredPlatform(i, j) && WorldGen.IsBelowANonHammeredPlatform(i + 1, j)))
            {
                vector.Y -= 8f;
                vector2.Y -= 8f;
            }

            float num4 = 1f;
            float num5 = 0f;
            bool flag2 = false;

            if (flag2)
                vector += new Vector2(0f, 16f);

            num2 *= -1f;
            if (!WorldGen.InAPlaceWithWind(i, j, sizeX, sizeY))
                windCycle -= num;

            for (int m = i; m < i + sizeX; m++)
            {
                for (int n = j; n < j + sizeY; n++)
                {
                    Tile tile2 = Main.tile[m, n];
                    ushort type2 = tile2.TileType;
                    if (type2 != type || !CoraliteTileDrawing.IsVisible(tile2))
                        continue;

                    short tileFrameX = tile2.TileFrameX;
                    short tileFrameY = tile2.TileFrameY;
                    float num7 = (n - j + 1) / (float)sizeY;
                    if (num7 == 0f)
                        num7 = 0.1f;

                    if (windRotOffset.HasValue)
                        num7 = windRotOffset.Value;

                    if (flag2 && n == j)
                        num7 = 0f;

                    Main.instance.TilesRenderer.GetTileDrawData(m, n, tile2, type2, ref tileFrameX, ref tileFrameY, out var tileWidth, out var tileHeight, out var tileTop, out var halfBrickHeight, out var addFrX, out var addFrY, out var tileSpriteEffect, out var _, out var _, out var _);
                    Color tileLight = Lighting.GetColor(m, n);
                    tileLight = tile2.IsTileFullbright ? Color.White : tileLight;

                    Vector2 vector3 = new Vector2(m * 16 - (int)screenPosition.X, n * 16 - (int)screenPosition.Y + tileTop);
                    vector3 += vector2;
                    Vector2 vector4 = new Vector2(windCycle * num4, Math.Abs(windCycle) * num5 * num7);
                    Vector2 vector5 = vector - vector3;
                    Texture2D tileDrawTexture = Main.instance.TilesRenderer.GetTileDrawTexture(tile2, m, n);
                    if (tileDrawTexture != null)
                    {
                        Vector2 vector6 = vector + new Vector2(0f, vector4.Y);
                        Rectangle rectangle = new Rectangle(tileFrameX + addFrX, tileFrameY + addFrY, tileWidth, tileHeight - halfBrickHeight);
                        float rotation = windCycle * num2 * num7;

                        Main.spriteBatch.Draw(tileDrawTexture, vector6, rectangle, tileLight, rotation, vector5, 1f, tileSpriteEffect, 0f);
                    }
                }
            }
        }

    }
}
