using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Coralite.Content.WorldGeneration.Generators
{
    public static class TextureGeneratorDatas
    {
        /// <summary>
        /// 长宽必须和贴图相等！
        /// </summary>
        /// <param name="tileTex"></param>
        /// <param name="colorToTile"></param>
        /// <returns></returns>
        public static Texture2TileGenerator GetTex2TileGenerator(Texture2D tileTex, Dictionary<Color, int> colorToTile)
        {
            Color[] tileData = new Color[tileTex.Width * tileTex.Height];
            tileTex.GetData(0, tileTex.Bounds, tileData, 0, tileTex.Width * tileTex.Height);

            int x = 0;
            int y = 0;
            Texture2TileGenerator gen = new(tileTex.Width, tileTex.Height);
            for (int m = 0; m < tileData.Length; m++)
            {
                Color tileColor = tileData[m];
                int tileID = colorToTile.ContainsKey(tileColor) ? colorToTile[tileColor] : -1; //if no key assume no action
                gen.tileGen[x, y] = new TileInfo(tileID, 0);
                x++;
                if (x >= tileTex.Width)
                {
                    x = 0;
                    y++;
                }
                if (y >= tileTex.Height)
                    break; //you've somehow reached the end of the texture! (this shouldn't happen!)
            }

            return gen;
        }

        /// <summary>
        /// 长宽必须和贴图相等！
        /// </summary>
        /// <param name="wallTex"></param>
        /// <param name="colorTowall"></param>
        /// <returns></returns>
        public static Texture2WallGenerator GetTex2WallGenerator(Texture2D wallTex, Dictionary<Color, int> colorToWall)
        {
            Color[] wallData = new Color[wallTex.Width * wallTex.Height];
            wallTex.GetData(0, wallTex.Bounds, wallData, 0, wallTex.Width * wallTex.Height);

            int x = 0;
            int y = 0;
            Texture2WallGenerator gen = new(wallTex.Width, wallTex.Height);
            for (int m = 0; m < wallData.Length; m++)
            {
                Color wallColor = wallTex == null ? Color.Black : wallData[m];
                int wallID = colorToWall.ContainsKey(wallColor) ? colorToWall[wallColor] : -1;
                gen.wallGen[x, y] = new WallInfo(wallID);
                x++;
                if (x >= wallTex.Width)
                {
                    x = 0;
                    y++;
                }
                if (y >= wallTex.Height)
                    break; //you've somehow reached the end of the texture! (this shouldn't happen!)
            }

            return gen;
        }

        public static Texture2Object GetTex2ObjectGenerator(Texture2D tex, Dictionary<Color, (int, int)> colorToObject)
        {
            Color[] objectData = new Color[tex.Width * tex.Height];

            tex.GetData(0, tex.Bounds, objectData, 0, tex.Width * tex.Height);

            int x = 0;
            int y = 0;
            Texture2Object gen = new(tex.Width, tex.Height);
            for (int m = 0; m < objectData.Length; m++)
            {
                Color wallColor = tex == null ? Color.Black : objectData[m];
                (int, int) tileInfos = colorToObject.ContainsKey(wallColor) ? colorToObject[wallColor] : (-1, 0);
                gen.tileObjectGen[x, y] = new TileObjectInfo(tileInfos.Item1, tileInfos.Item2);
                x++;
                if (x >= tex.Width)
                {
                    x = 0;
                    y++;
                }
                if (y >= tex.Height)
                    break;
            }

            return gen;
        }

        public static Texture2Liquid GetTex2LiquidGenerator(Texture2D tex, Dictionary<Color, int> colorToLiquid)
        {
            Color[] objectData = new Color[tex.Width * tex.Height];

            tex.GetData(0, tex.Bounds, objectData, 0, tex.Width * tex.Height);

            int x = 0;
            int y = 0;
            Texture2Liquid gen = new(tex.Width, tex.Height);
            for (int m = 0; m < objectData.Length; m++)
            {
                Color liquidColor = tex == null ? Color.Black : objectData[m];
                int liquidInfo = colorToLiquid.ContainsKey(liquidColor) ? colorToLiquid[liquidColor] : -1;
                gen.tileLiquidGen[x, y] = new LiquidInfo(liquidInfo);
                x++;
                if (x >= tex.Width)
                {
                    x = 0;
                    y++;
                }
                if (y >= tex.Height)
                    break;
            }

            return gen;
        }
    }
}
