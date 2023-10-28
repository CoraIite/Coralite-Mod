using Microsoft.Xna.Framework;
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


    }

}
