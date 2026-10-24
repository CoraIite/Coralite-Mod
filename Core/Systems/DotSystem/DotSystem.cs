using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Core.Systems.DotSystem
{
    public class DotSystem : ModSystem
    {
        /// <summary>
        /// 以dot类型ID为索引，存储对应dot具有的标签类型ID
        /// </summary>
        public static int[][] DotTags { get; private set; }

        /// <summary>
        /// 以dot类型ID为索引的图标贴图
        /// </summary>
        public static ATex[] DotAssets { get; private set; }

        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            DotAssets = new ATex[DotLoader.DotCount];
            for (int i = 0; i < DotAssets.Length; i++)
                DotAssets[i] = ModContent.Request<Texture2D>(DotLoader.GetDot(i).Texture);
        }

        public override void PostSetupContent()
        {
            DotTags = new int[DotLoader.DotCount][];
            for (int i = 0; i < DotTags.Length; i++)
                DotTags[i] = DotLoader.GetDot(i).LoadTags();
        }

        public override void Unload()
        {
            DotLoader.Unload();
            DotTags = null;
            DotAssets = null;
        }
    }
}
