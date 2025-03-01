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
        public string BasePath {  get;private set; }

        public int? style {  get; private set; }

        /// <summary> 主地形图 </summary>
        public Texture2D MainTex { get; private set; }
        /// <summary> 清理地形图 </summary>
        public Texture2D ClearTex { get; private set; }

        public Texture2D WallTex { get; private set; }
        public Texture2D WallClearTex { get; private set; }

        public Texture2D LiquidTex { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Point Size=>new Point(Width, Height);

        public TextureGenerator(string name, int? style = null, string path = AssetDirectory.WorldGen)
        {
            BasePath = path + name;

            this.style = style;
            MainTex = Get(BasePath + style ?? "");
            ClearTex = Get(BasePath + "Clear" + style ?? "");

            Width = MainTex.Width;
            Height = MainTex.Height;
        }

        /// <summary>
        /// 自动根据创建时候的路径和名称获取墙壁图
        /// </summary>
        /// <param name="style"></param>
        public void SetWallTex()
        {
            WallTex = Get(BasePath + "Wall" + style ?? "");
            WallClearTex = Get(BasePath + "WallClear" + style ?? "");
        }

        /// <summary>
        /// 自动根据创建时候的路径和名称获取液体图
        /// </summary>
        /// <param name="style"></param>
        public void SetLiquidTex(int? style = null)
        {
            LiquidTex = Get(BasePath + "Liquid" + style ?? "");
        }

        public readonly void Generate(Point Center, Dictionary<Color, int> mainDic, Dictionary<Color, int> wallDic = null)
        {
            int x = Center.X - Width / 2;
            int y = Center.Y - Height / 2;

            GenerateByTopLeft(new Point(x, y), mainDic, wallDic);
        }

        public readonly void GenerateByTopLeft(Point topLeft, Dictionary<Color, int> mainDic, Dictionary<Color, int> wallDic)
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
                    clearGenerator = TextureGeneratorDatas.GetTex2TileGenerator(generator.ClearTex, CoraliteWorld.clearDic);

                    //生成主体地形
                    roomGenerator = TextureGeneratorDatas.GetTex2TileGenerator(generator.MainTex, mainDic);

                    //清理范围
                    if (generator.WallClearTex != null)
                        wallClearGenerator = TextureGeneratorDatas.GetTex2WallGenerator(generator.WallClearTex, CoraliteWorld.clearDic);

                    //生成墙壁
                    if (generator.WallTex != null)
                        wallGenerator = TextureGeneratorDatas.GetTex2WallGenerator(generator.WallTex, wallDic);

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
