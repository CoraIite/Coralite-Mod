using Coralite.Content.WorldGeneration.Generators;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
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

        public readonly void Generate(Point Center, Dictionary<Color, int> mainDic, Dictionary<Color, int> wallDic = null, Action<Color, int, int> objectPlacement = null)
        {
            int x = Center.X - Width / 2;
            int y = Center.Y - Height / 2;

            GenerateByTopLeft(new Point(x, y), mainDic, wallDic,objectPlacement);
        }

        public readonly void GenerateByTopLeft(Point topLeft, Dictionary<Color, int> mainDic, Dictionary<Color, int> wallDic = null,Action<Color,int,int> objectPlacement=null)
        {
            int x = topLeft.X;
            int y = topLeft.Y;

            WorldGenHelper.ClearLiuid(x, y, Width, Height);

            Texture2TileGenerator mainGenerator = null;
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
                    //生成主体地形
                    mainGenerator = TextureGeneratorDatas.GetTex2TileGenerator(generator.MainTex, mainDic);

                    //生成墙壁
                    if (generator.WallTex != null)
                        wallGenerator = TextureGeneratorDatas.GetTex2WallGenerator(generator.WallTex, wallDic);

                    if (generator.LiquidTex != null)
                        liquidGenerator = TextureGeneratorDatas.GetTex2LiquidGenerator(generator.LiquidTex, CoraliteWorld.liquidDic);

                    genned = true;
                });

                placed = true;
            }

            mainGenerator?.Clear(x, y);
            mainGenerator?.Generate(x, y);

            wallGenerator?.Generate(x, y, true);

            liquidGenerator?.Generate(x, y, true);

            if (objectPlacement != null)
                mainGenerator?.ObjectPlace(x, y, objectPlacement);
        }

        private Texture2D Get(string name)
            => ModContent.Request<Texture2D>(name, AssetRequestMode.ImmediateLoad).Value;
    }
}
