using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Coralite.Content.WorldGeneration
{
    public struct TextureGenerator
    {
        private string BasePath;

        /// <summary> 主地形图 </summary>
        private Texture2D MainTex;
        /// <summary> 清理地形图 </summary>
        private Texture2D ClearTexTex;

        private Texture2D WallTex;
        private Texture2D WallClearTex;

        private Texture2D LiquidTex;

        public TextureGenerator(string name, int? style = null, string path = AssetDirectory.WorldGen)
        {
            BasePath = path + name;

            MainTex = Get(BasePath + style ?? "");
            ClearTexTex = Get(BasePath + "Clear" + style ?? "");
        }

        /// <summary>
        /// 自动根据创建时候的路径和名称获取墙壁图
        /// </summary>
        /// <param name="style"></param>
        public void SetWallTex(int? style = null)
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

        public void Generate(Point TopLeft)
        {

        }

        private Texture2D Get(string name)
            => ModContent.Request<Texture2D>(name, AssetRequestMode.ImmediateLoad).Value;
    }
}
