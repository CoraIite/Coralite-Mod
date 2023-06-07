using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.ID;

namespace Coralite.Content.WorldGeneration.Generators
{
    public class Texture2TileGenerator : TextureGenerator
    {
        public TileInfo[,] tileGen;

        public Texture2TileGenerator(int width, int height) : base(width, height)
        {
            tileGen = new TileInfo[width, height];
        }

        public void Generate(int x, int y, bool sync)
        {
            for (int x1 = 0; x1 < width; x1++)
                for (int y1 = 0; y1 < height; y1++)
                {
                    int current_x = x + x1;
                    int current_y = y + y1;
                    TileInfo info = tileGen[x1, y1];

                    if (info.tileID != -1)
                        WorldGenHelper.Texture2TileGenerate(current_x, current_y, info.tileID, info.tileStyle, info.tileID > -1, info.liquidAmt == 0, false, sync);
                }
        }

    }

    public class Texture2TileGeneratorDatas
    {
        /// <summary>
        /// 长宽必须和贴图相等！
        /// </summary>
        /// <param name="tileTex"></param>
        /// <param name="colorToTile"></param>
        /// <returns></returns>
        public static Texture2TileGenerator GetTexGenerator(Texture2D tileTex, Dictionary<Color, int> colorToTile)
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
    }

}
