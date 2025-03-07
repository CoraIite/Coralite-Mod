using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Coralite.Content.WorldGeneration.Generators
{
    public static class TextureGeneratorDatas
    {
        /// <summary>
        /// 长宽必须和贴图相等！
        /// 获取清除的生成器
        /// </summary>
        /// <param name="tileTex"></param>
        /// <param name="colorToTile"></param>
        /// <returns></returns>
        public static Texture2TileGenerator GetTex2TileGenerator(Texture2D tileTex, Dictionary<Color, int> colorToTile)
        {
            //读图
            Color[] tileData = new Color[tileTex.Width * tileTex.Height];
            tileTex.GetData(0, tileTex.Bounds, tileData, 0, tileTex.Width * tileTex.Height);

            int x = 0;
            int y = 0;

            
            Texture2TileGenerator gen = new(tileTex.Width, tileTex.Height);
            for (int m = 0; m < tileData.Length; m++)
            {
                //获取颜色
                Color tileColor = tileData[m];

                //如果是透明的颜色那么就清除掉
                int tileID = GenerateType.Clear;
                if (colorToTile.TryGetValue(tileColor,out int tileType))
                    tileID = tileType;
                if (tileColor == Color.Transparent)
                    tileID = GenerateType.Ignore;

                gen.tileGen[x, y] = new TileInfo(tileColor, tileID, 0);
                x++;
                if (x >= tileTex.Width)
                {
                    x = 0;
                    y++;
                }
                if (y >= tileTex.Height)
                    break; 
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
                Color wallColor = wallData[m];

                int wallID = GenerateType.Clear;
                if (colorToWall.TryGetValue(wallColor, out int wallType))
                    wallID = wallType;
                else if (wallColor == Color.Transparent)
                    wallID = GenerateType.Ignore;

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

        /// <summary>
        /// 长宽必须和贴图相等！
        /// </summary>
        /// <param name="wallTex"></param>
        /// <param name="colorTowall"></param>
        /// <returns></returns>
        public static Texture2WallGenerator GetTex2WallClearGenerator(Texture2D wallTex, Dictionary<Color, int> colorToWall)
        {
            Color[] wallData = new Color[wallTex.Width * wallTex.Height];
            wallTex.GetData(0, wallTex.Bounds, wallData, 0, wallTex.Width * wallTex.Height);

            int x = 0;
            int y = 0;
            Texture2WallGenerator gen = new(wallTex.Width, wallTex.Height);
            for (int m = 0; m < wallData.Length; m++)
            {
                Color wallColor = wallData[m];
                int wallID = wallColor == Color.Transparent ? -1 : -2;
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
