using Coralite.Content.WorldGeneration.Generators;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.WorldGeneration
{
    public struct TextureGenerator
    {
        public string BasePath { get; private set; }

        public int? Style { get; private set; }

        /// <summary> 主地形图 </summary>
        public Texture2D MainTex { get; private set; }

        public Texture2D WallTex { get; private set; }

        public Texture2D LiquidTex { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public readonly Point Size => new Point(Width, Height);

        public TextureGenerator(string name, int? style = null, string path = AssetDirectory.WorldGen)
        {
            BasePath = path + name;

            Style = style;
            MainTex = Get(BasePath + style ?? "");

            //加载墙壁
            string wallName = BasePath + "Wall" + Style ?? "";
            if (ModContent.HasAsset(wallName))
                WallTex = Get(wallName);

            //加载液体
            string LiquidName = BasePath + "Liquid" + Style ?? "";
            if (ModContent.HasAsset(LiquidName))
                LiquidTex = Get(LiquidName);

            Width = MainTex.Width;
            Height = MainTex.Height;
        }

        public readonly void Generate(Point Center, Dictionary<Color, int> mainDic, Dictionary<Color, int> wallDic = null)
        {
            int x = Center.X - Width / 2;
            int y = Center.Y - Height / 2;

            GenerateByTopLeft(new Point(x, y), mainDic, wallDic);
        }

        public readonly void GenerateByTopLeft(Point topLeft, Dictionary<Color, int> mainDic, Dictionary<Color, int> wallDic = null)
        {
            int x = topLeft.X;
            int y = topLeft.Y;

            WorldGenHelper.ClearLiuid(x, y, Width, Height);

            Texture2TileGenerator clearGenerator = null;
            Texture2TileGenerator roomGenerator = null;
            Texture2WallGenerator wallClearGenerator = null;
            Texture2WallGenerator wallGenerator = null;
            Texture2Liquid liquidGenerator = null;

            TextureGenerator generator = this;

            bool genned = false;
            bool placed = false;

            while (!genned)
            {
                if (placed)
                    continue;

                Main.QueueMainThreadAction(() =>
                {
                    //清理范围
                    clearGenerator = TextureGeneratorDatas.GetTex2TileClearGenerator(generator.MainTex, CoraliteWorld.clearDic);

                    //生成主体地形
                    roomGenerator = TextureGeneratorDatas.GetTex2TileGenerator(generator.MainTex, mainDic);

                    //生成墙壁
                    if (generator.WallTex != null)
                    {
                        wallClearGenerator = TextureGeneratorDatas.GetTex2WallClearGenerator(generator.WallTex, CoraliteWorld.clearDic);
                        wallGenerator = TextureGeneratorDatas.GetTex2WallGenerator(generator.WallTex, wallDic);
                    }

                    if (generator.LiquidTex != null)
                        liquidGenerator = TextureGeneratorDatas.GetTex2LiquidGenerator(generator.LiquidTex, CoraliteWorld.liquidDic);

                    genned = true;
                });

                placed = true;
            }

            clearGenerator?.Generate(x, y, true);
            roomGenerator?.Generate(x, y, true);
            wallClearGenerator?.Generate(x, y, true);
            wallGenerator?.Generate(x, y, true);
            liquidGenerator?.Generate(x, y, true);
        }

        private Texture2D Get(string name)
            => ModContent.Request<Texture2D>(name, AssetRequestMode.ImmediateLoad).Value;
    }
}
